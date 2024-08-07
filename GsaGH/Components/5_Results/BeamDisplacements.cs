using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;

using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;

using GsaGH.Components.Helpers;
using GsaGH.Helpers;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Parameters.Results;
using GsaGH.Properties;

using OasysGH;
using OasysGH.Components;
using OasysGH.Parameters;
using OasysGH.Units;
using OasysGH.Units.Helpers;

using OasysUnits;
using OasysUnits.Units;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to get GSA beam displacement values
  /// </summary>
  public class BeamDisplacements : GH_OasysDropDownComponent {
    public override Guid ComponentGuid => new Guid("1b7e99e8-c3c9-42c3-9474-792ddd17388d");
    public override GH_Exposure Exposure => GH_Exposure.quarternary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.BeamDisplacements;
    private LengthUnit _lengthUnit = DefaultUnits.LengthUnitResult;

    public BeamDisplacements() : base("Beam Displacements", "BeamDisp",
      "Element1D Translation and Rotation result values", CategoryName.Name(),
      SubCategoryName.Cat5()) {
      Hidden = true;
    }

    public override void SetSelected(int i, int j) {
      _selectedItems[i] = _dropDownItems[i][j];
      _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[1]);
      base.UpdateUI();
    }

    public override void VariableParameterMaintenance() {
      string unitAbbreviation = Length.GetAbbreviation(_lengthUnit);
      int i = 0;
      Params.Output[i++].Name = "Translations X [" + unitAbbreviation + "]";
      Params.Output[i++].Name = "Translations Y [" + unitAbbreviation + "]";
      Params.Output[i++].Name = "Translations Z [" + unitAbbreviation + "]";
      Params.Output[i].Name = "Translations |XYZ| [" + unitAbbreviation + "]";
    }

    protected override void BeforeSolveInstance() {
      base.BeforeSolveInstance();

      if (Params.Input.Count < 4) {
        Params.RegisterInputParam(new Param_Integer());
        Params.Input[3].Name = "Axis";
        Params.Input[3].NickName = "Ax";
        Params.Input[3].Description = "Standard Axis: Global (0), Local (-1), Natural (-2), Default (-10), XElevation (-11), YElevation (-12), GlobalCylindrical (-13), Vertical (-14)";
        Params.Input[3].Access = GH_ParamAccess.item;
        Params.Input[3].Optional = true;
      }
    }

    protected override void InitialiseDropdowns() {
      _spacerDescriptions = new List<string>(new[] {
        "Max/Min",
        "Unit",
      });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      _dropDownItems.Add(ExtremaHelper.Vector6Displacements.ToList());
      _selectedItems.Add(_dropDownItems[0][0]);

      _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
      _selectedItems.Add(Length.GetAbbreviation(_lengthUnit));

      _isInitialised = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaResultParameter(), "Result", "Res", "GSA Result",
        GH_ParamAccess.list);
      pManager.AddParameter(new GsaElementMemberListParameter());
      pManager.AddIntegerParameter("Intermediate Points", "nP",
        "Number of intermediate equidistant points (default 3)", GH_ParamAccess.item, 3);
      pManager.AddIntegerParameter("Axis", "Ax", "Standard Axis: Global (0), Local (-1), Natural (-2), Default (-10), XElevation (-11), YElevation (-12), GlobalCylindrical (-13), Vertical (-14)", GH_ParamAccess.item);
      pManager[1].Optional = true;
      pManager[3].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      string unitAbbreviation = Length.GetAbbreviation(_lengthUnit);
      string note = ResultNotes.Note1dResults;

      pManager.AddGenericParameter("Translation X [" + unitAbbreviation + "]", "Ux",
        "Translation in X-direction" + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Translation Y [" + unitAbbreviation + "]", "Uy",
        "Translation in Y-direction" + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Translation Z [" + unitAbbreviation + "]", "Uz",
        "Translation in Z-direction" + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Translation |XYZ| [" + unitAbbreviation + "]", "|U|",
        "Combined |XYZ| Translation" + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Rotation XX [rad]", "Rxx",
        "Rotation around X-axis" + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Rotation YY [rad]", "Ryy",
        "Rotation around Y-axis" + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Rotation ZZ [rad]", "Rzz",
        "Rotation around Z-axis" + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Rotation |XYZ| [rad]", "|R|",
        "Combined |XXYYZZ| Rotation" + note, GH_ParamAccess.tree);
    }

    protected override void SolveInternal(IGH_DataAccess da) {
      GsaResult result;
      string elementlist = "All";
      var ghDivisions = new GH_Integer();
      da.GetData(2, ref ghDivisions);
      GH_Convert.ToInt32(ghDivisions, out int positionsCount, GH_Conversion.Both);
      positionsCount = Math.Abs(positionsCount) + 2; // taken absolute value and add 2 end points.

      var ghTypes = new List<GH_ObjectWrapper>();
      da.GetDataList(0, ghTypes);

      var outTransX = new DataTree<GH_UnitNumber>();
      var outTransY = new DataTree<GH_UnitNumber>();
      var outTransZ = new DataTree<GH_UnitNumber>();
      var outTransXyz = new DataTree<GH_UnitNumber>();
      var outRotX = new DataTree<GH_UnitNumber>();
      var outRotY = new DataTree<GH_UnitNumber>();
      var outRotZ = new DataTree<GH_UnitNumber>();
      var outRotXyz = new DataTree<GH_UnitNumber>();

      foreach (GH_ObjectWrapper ghTyp in ghTypes) {
        result = Inputs.GetResultInput(this, ghTyp);
        if (result == null) {
          return;
        }

        int axisId = -10;
        da.GetData(3, ref axisId);
        result.Element1dDisplacements.SetStandardAxis(axisId);

        elementlist = Inputs.GetElementListDefinition(this, da, 1, result.Model);
        ReadOnlyCollection<int> elementIds = result.ElementIds(elementlist, 1);
        IEntity1dResultSubset<IDisplacement, ResultVector6<Entity1dExtremaKey>> resultSet =
          result.Element1dDisplacements.ResultSubset(elementIds, positionsCount);

        List<int> permutations = result.SelectedPermutationIds ?? new List<int>() {
          1,
        };
        if (permutations.Count == 1 && permutations[0] == -1) {
          permutations = Enumerable.Range(1, resultSet.Subset.Values.First().Count).ToList();
        }

        if (_selectedItems[0] == ExtremaHelper.Vector6Displacements[0]) {
          foreach (KeyValuePair<int, IList<IEntity1dQuantity<IDisplacement>>> kvp in resultSet.Subset) {
            foreach (int p in permutations) {
              var path = new GH_Path(result.CaseId, result.SelectedPermutationIds == null ? 0 : p, kvp.Key);
              outTransX.AddRange(kvp.Value[p - 1].Results.Values.Select(
                r => new GH_UnitNumber(r.X.ToUnit(_lengthUnit))), path);
              outTransY.AddRange(kvp.Value[p - 1].Results.Values.Select(
                r => new GH_UnitNumber(r.Y.ToUnit(_lengthUnit))), path);
              outTransZ.AddRange(kvp.Value[p - 1].Results.Values.Select(
                r => new GH_UnitNumber(r.Z.ToUnit(_lengthUnit))), path);
              outTransXyz.AddRange(kvp.Value[p - 1].Results.Values.Select(
                r => new GH_UnitNumber(r.Xyz.ToUnit(_lengthUnit))), path);
              outRotX.AddRange(kvp.Value[p - 1].Results.Values.Select(
                r => new GH_UnitNumber(r.Xx)), path);
              outRotY.AddRange(kvp.Value[p - 1].Results.Values.Select(
                r => new GH_UnitNumber(r.Yy)), path);
              outRotZ.AddRange(kvp.Value[p - 1].Results.Values.Select(
                r => new GH_UnitNumber(r.Zz)), path);
              outRotXyz.AddRange(kvp.Value[p - 1].Results.Values.Select(
                r => new GH_UnitNumber(r.Xxyyzz)), path);
            }
          }
        } else {
          Entity1dExtremaKey key = ExtremaHelper.DisplacementExtremaKey(resultSet, _selectedItems[0]);
          IDisplacement extrema = resultSet.GetExtrema(key);
          int perm = result.CaseType == CaseType.AnalysisCase ? 0 : 1;
          var path = new GH_Path(result.CaseId, key.Permutation + perm, key.Id);
          outTransX.Add(new GH_UnitNumber(extrema.X.ToUnit(_lengthUnit)), path);
          outTransY.Add(new GH_UnitNumber(extrema.Y.ToUnit(_lengthUnit)), path);
          outTransZ.Add(new GH_UnitNumber(extrema.Z.ToUnit(_lengthUnit)), path);
          outTransXyz.Add(new GH_UnitNumber(extrema.Xyz.ToUnit(_lengthUnit)), path);
          outRotX.Add(new GH_UnitNumber(extrema.Xx), path);
          outRotY.Add(new GH_UnitNumber(extrema.Yy), path);
          outRotZ.Add(new GH_UnitNumber(extrema.Zz), path);
          outRotXyz.Add(new GH_UnitNumber(extrema.Xxyyzz), path);
        }

        PostHog.Result(result.CaseType, 1, "Displacement");
      }

      da.SetDataTree(0, outTransX);
      da.SetDataTree(1, outTransY);
      da.SetDataTree(2, outTransZ);
      da.SetDataTree(3, outTransXyz);
      da.SetDataTree(4, outRotX);
      da.SetDataTree(5, outRotY);
      da.SetDataTree(6, outRotZ);
      da.SetDataTree(7, outRotXyz);
    }

    protected override void UpdateUIFromSelectedItems() {
      if (_selectedItems.Count == 1) {
        _spacerDescriptions.Insert(0, "Max/Min");
        _dropDownItems.Insert(0, ExtremaHelper.Vector6Displacements.ToList());
        _selectedItems.Insert(0, _dropDownItems[0][0]);
      }

      _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[1]);
      base.UpdateUIFromSelectedItems();
    }
  }
}
