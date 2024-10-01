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
  ///   Component to retrieve non-geometric objects from a GSA model
  /// </summary>
  public class Element2dStresses : GH_OasysDropDownComponent {
    public override Guid ComponentGuid => new Guid("b5eb8a78-e0dd-442b-bbd7-0384d6c944cb");
    public override GH_Exposure Exposure => GH_Exposure.quinary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.Element2dStresses;
    private PressureUnit _stresshUnit = DefaultUnits.StressUnitResult;

    public Element2dStresses() : base("Element 2D Stresses", "Stress2D", "2D Projected Stress result values",
      CategoryName.Name(), SubCategoryName.Cat5()) {
      Hidden = true;
    }

    public override void SetSelected(int i, int j) {
      _selectedItems[i] = _dropDownItems[i][j];
      _stresshUnit = (PressureUnit)UnitsHelper.Parse(typeof(PressureUnit), _selectedItems[1]);
      base.UpdateUI();
    }

    public override void VariableParameterMaintenance() {
      string unitAbbreviation = Pressure.GetAbbreviation(_stresshUnit);
      int i = 0;
      Params.Output[i++].Name = "Stress XX [" + unitAbbreviation + "]";
      Params.Output[i++].Name = "Stress YY [" + unitAbbreviation + "]";
      Params.Output[i++].Name = "Stress ZZ [" + unitAbbreviation + "]";
      Params.Output[i++].Name = "Stress XY [" + unitAbbreviation + "]";
      Params.Output[i++].Name = "Stress YZ [" + unitAbbreviation + "]";
      Params.Output[i].Name = "Stress ZX [" + unitAbbreviation + "]";
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

      _dropDownItems.Add(ExtremaHelper.Tensor3Stresses.ToList());
      _selectedItems.Add(_dropDownItems[0][0]);

      _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Stress));
      _selectedItems.Add(Pressure.GetAbbreviation(_stresshUnit));

      _isInitialised = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaResultParameter(), "Result", "Res", "GSA Result",
        GH_ParamAccess.list);
      pManager.AddParameter(new GsaElementMemberListParameter());
      pManager.AddIntegerParameter("Stress Layer", "σL",
        "Layer within the cross-section to get results." + Environment.NewLine
        + "Input an integer between -1 and 1, representing the normalised thickness,"
        + Environment.NewLine + "default value is zero => middle of the element.",
        GH_ParamAccess.item, 0);
      pManager.AddIntegerParameter("Axis", "Ax", "Standard Axis: Global (0), Local (-1), Natural (-2), Default (-10), XElevation (-11), YElevation (-12), GlobalCylindrical (-13), Vertical (-14)", GH_ParamAccess.item);
      pManager[1].Optional = true;
      pManager[3].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      string unitAbbreviation = Pressure.GetAbbreviation(_stresshUnit);

      string note = ResultNotes.Note2dStressResults;

      pManager.AddGenericParameter("Stress XX [" + unitAbbreviation + "]", "xx",
        "Stress in XX-direction" + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Stress YY [" + unitAbbreviation + "]", "yy",
        "Stress in YY-direction" + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Stress ZZ [" + unitAbbreviation + "]", "zz",
        "Stress in ZZ-direction" + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Stress XY [" + unitAbbreviation + "]", "xy",
        "Stress in XY-direction" + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Stress YZ [" + unitAbbreviation + "]", "yz",
        "Stress in YZ-direction" + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Stress ZX [" + unitAbbreviation + "]", "zx",
        "Stress in ZX-direction" + note, GH_ParamAccess.tree);
    }

    protected override void SolveInternal(IGH_DataAccess da) {
      GsaResult result = null;
      string elementlist = "All";

      Layer2d layer = Layer2d.Middle;
      var ghLayer = new GH_Number();
      if (da.GetData(2, ref ghLayer)) {
        GH_Convert.ToInt32(ghLayer, out int val, GH_Conversion.Both);
        layer = val switch {
          -1 => Layer2d.Bottom,
          0 => Layer2d.Middle,
          1 => Layer2d.Top,
          _ => throw new ArgumentException("Layer must be between -1 and 1"),
        };
      }

      var outXx = new DataTree<GH_UnitNumber>();
      var outYy = new DataTree<GH_UnitNumber>();
      var outZz = new DataTree<GH_UnitNumber>();
      var outXy = new DataTree<GH_UnitNumber>();
      var outYz = new DataTree<GH_UnitNumber>();
      var outZx = new DataTree<GH_UnitNumber>();

      var ghTypes = new List<GH_ObjectWrapper>();
      da.GetDataList(0, ghTypes);

      foreach (GH_ObjectWrapper ghTyp in ghTypes) {
        result = Inputs.GetResultInput(this, ghTyp);
        if (result == null) {
          return;
        }

        int axisId = -10;
        da.GetData(3, ref axisId);
        result.Element2dStresses.SetStandardAxis(axisId);

        elementlist = Inputs.GetElementListDefinition(this, da, 1, result.Model);
        ReadOnlyCollection<int> elementIds = result.ElementIds(elementlist, 2);
        IMeshResultSubset<IMeshQuantity<IStress>, IStress, ResultTensor3<Entity2dExtremaKey>> resultSet =
          result.Element2dStresses.ResultSubset(elementIds, layer);

        List<int> permutations = result.SelectedPermutationIds ?? new List<int>() {
          1,
        };
        if (permutations.Count == 1 && permutations[0] == -1) {
          permutations = Enumerable.Range(1, resultSet.Subset.Values.First().Count).ToList();
        }

        if (_selectedItems[0] == ExtremaHelper.Vector6Displacements[0]) {
          foreach (KeyValuePair<int, IList<IMeshQuantity<IStress>>> kvp in resultSet.Subset) {
            foreach (int p in permutations) {
              var path = new GH_Path(result.CaseId, result.SelectedPermutationIds == null ? 0 : p, kvp.Key);
              outXx.AddRange(kvp.Value[p - 1].Results().Select(
                r => new GH_UnitNumber(r.Xx.ToUnit(_stresshUnit))), path);
              outYy.AddRange(kvp.Value[p - 1].Results().Select(
                r => new GH_UnitNumber(r.Yy.ToUnit(_stresshUnit))), path);
              outZz.AddRange(kvp.Value[p - 1].Results().Select(
                r => new GH_UnitNumber(r.Zz.ToUnit(_stresshUnit))), path);
              outXy.AddRange(kvp.Value[p - 1].Results().Select(
                r => new GH_UnitNumber(r.Xy.ToUnit(_stresshUnit))), path);
              outYz.AddRange(kvp.Value[p - 1].Results().Select(
                r => new GH_UnitNumber(r.Yz.ToUnit(_stresshUnit))), path);
              outZx.AddRange(kvp.Value[p - 1].Results().Select(
                r => new GH_UnitNumber(r.Zx.ToUnit(_stresshUnit))), path);
            }
          }
        } else {
          Entity2dExtremaKey key = ExtremaHelper.StressExtremaKey(resultSet, _selectedItems[0]);
          IStress extrema = resultSet.GetExtrema(key);
          int perm = result.CaseType == CaseType.AnalysisCase ? 0 : 1;
          var path = new GH_Path(result.CaseId, key.Permutation + perm, key.Id);
          outXx.Add(new GH_UnitNumber(extrema.Xx.ToUnit(_stresshUnit)), path);
          outYy.Add(new GH_UnitNumber(extrema.Yy.ToUnit(_stresshUnit)), path);
          outZz.Add(new GH_UnitNumber(extrema.Zz.ToUnit(_stresshUnit)), path);
          outXy.Add(new GH_UnitNumber(extrema.Xy.ToUnit(_stresshUnit)), path);
          outYz.Add(new GH_UnitNumber(extrema.Yz.ToUnit(_stresshUnit)), path);
          outZx.Add(new GH_UnitNumber(extrema.Zx.ToUnit(_stresshUnit)), path);
        }

        PostHog.Result(result.CaseType, 2, "Stress");
      }

      da.SetDataTree(0, outXx);
      da.SetDataTree(1, outYy);
      da.SetDataTree(2, outZz);
      da.SetDataTree(3, outXy);
      da.SetDataTree(4, outYz);
      da.SetDataTree(5, outZx);
    }

    protected override void UpdateUIFromSelectedItems() {
      if (_selectedItems.Count == 1) {
        _spacerDescriptions.Insert(0, "Max/Min");
        _dropDownItems.Insert(0, ExtremaHelper.Tensor3Stresses.ToList());
        _selectedItems.Insert(0, _dropDownItems[0][0]);
      }

      _stresshUnit = (PressureUnit)UnitsHelper.Parse(typeof(PressureUnit), _selectedItems[1]);
      base.UpdateUIFromSelectedItems();
    }
  }
}
