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
  ///   Component to get GSA spring reaction forces
  /// </summary>
  public class SpringReactionForces : GH_OasysDropDownComponent {
    public override Guid ComponentGuid => new Guid("60f6a109-577d-4e90-8790-7f8cf110b230");
    public override GH_Exposure Exposure => GH_Exposure.secondary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.SpringReactionForces;
    private ForceUnit _forceUnit = DefaultUnits.ForceUnit;
    private MomentUnit _momentUnit = DefaultUnits.MomentUnit;

    public SpringReactionForces() : base("Spring Reaction Forces", "SpringForce",
      "Spring Reaction Force result values", CategoryName.Name(), SubCategoryName.Cat5()) {
      Hidden = true;
    }

    public override void SetSelected(int i, int j) {
      _selectedItems[i] = _dropDownItems[i][j];
      switch (i) {
        case 1:
          _forceUnit = (ForceUnit)UnitsHelper.Parse(typeof(ForceUnit), _selectedItems[i]);
          break;

        case 2:
          _momentUnit = (MomentUnit)UnitsHelper.Parse(typeof(MomentUnit), _selectedItems[i]);
          break;
      }

      base.UpdateUI();
    }

    public override void VariableParameterMaintenance() {
      string forceunitAbbreviation = Force.GetAbbreviation(_forceUnit);
      string momentunitAbbreviation = Moment.GetAbbreviation(_momentUnit);
      int i = 0;
      Params.Output[i++].Name = "Force X [" + forceunitAbbreviation + "]";
      Params.Output[i++].Name = "Force Y [" + forceunitAbbreviation + "]";
      Params.Output[i++].Name = "Force Z [" + forceunitAbbreviation + "]";
      Params.Output[i++].Name = "Force |XYZ| [" + forceunitAbbreviation + "]";
      Params.Output[i++].Name = "Moment XX [" + momentunitAbbreviation + "]";
      Params.Output[i++].Name = "Moment YY [" + momentunitAbbreviation + "]";
      Params.Output[i++].Name = "Moment ZZ [" + momentunitAbbreviation + "]";
      Params.Output[i].Name = "Moment |XXYYZZ| [" + momentunitAbbreviation + "]";
    }

    protected override void BeforeSolveInstance() {
      base.BeforeSolveInstance();

      if (Params.Input.Count < 3) {
        Params.RegisterInputParam(new Param_Integer());
        Params.Input[2].Name = "Axis";
        Params.Input[2].NickName = "Ax";
        Params.Input[2].Description = "Standard Axis: Global (0), Local (-1), Natural (-2), Default (-10), XElevation (-11), YElevation (-12), GlobalCylindrical (-13), Vertical (-14)";
        Params.Input[2].Access = GH_ParamAccess.item;
        Params.Input[2].Optional = true;
      }
    }

    protected override void InitialiseDropdowns() {
      _spacerDescriptions = new List<string>(new[] {
        "Max/Min",
        "Force Unit",
        "Moment Unit",
      });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      _dropDownItems.Add(ExtremaHelper.Vector6ReactionForces.ToList());
      _selectedItems.Add(_dropDownItems[0][0]);

      _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Force));
      _selectedItems.Add(Force.GetAbbreviation(_forceUnit));

      _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Moment));
      _selectedItems.Add(Moment.GetAbbreviation(_momentUnit));

      _isInitialised = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaResultParameter(), "Result", "Res", "GSA Result",
        GH_ParamAccess.list);
      pManager.AddParameter(new GsaNodeListParameter());
      pManager.AddIntegerParameter("Axis", "Ax", "Standard Axis: Global (0), Local (-1), Natural (-2), Default (-10), XElevation (-11), YElevation (-12), GlobalCylindrical (-13), Vertical (-14)", GH_ParamAccess.item);
      pManager[1].Optional = true;
      pManager[2].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      string forceunitAbbreviation = Force.GetAbbreviation(_forceUnit);
      string momentunitAbbreviation = Moment.GetAbbreviation(_momentUnit);

      string note = ResultNotes.NoteNodeResults;

      pManager.AddGenericParameter("Force X [" + forceunitAbbreviation + "]", "Fx",
        "Reaction Force in X-direction" + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Force Y [" + forceunitAbbreviation + "]", "Fy",
        "Reaction Force in Y-direction" + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Force Z [" + forceunitAbbreviation + "]", "Fz",
        "Reaction Force in Z-direction" + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Force |XYZ| [" + forceunitAbbreviation + "]", "|F|",
        "Combined |XYZ| Reaction Force" + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Moment XX [" + momentunitAbbreviation + "]", "Mxx",
        "Reaction Moment around X-axis" + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Moment YY [" + momentunitAbbreviation + "]", "Myy",
        "Reaction Moment around Y-axis" + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Moment ZZ [" + momentunitAbbreviation + "]", "Mzz",
        "Reaction Moment around Z-axis" + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Moment |XYZ| [" + momentunitAbbreviation + "]", "|M|",
        "Combined |XXYYZZ| Reaction Moment" + note, GH_ParamAccess.tree);
      pManager.AddIntegerParameter("Node IDs", "ID", "Node IDs for each result value",
        GH_ParamAccess.list);
    }

    protected override void SolveInternal(IGH_DataAccess da) {
      GsaResult result;

      string nodeList = "All";

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
      var outIDs = new DataTree<int>();

      foreach (GH_ObjectWrapper ghTyp in ghTypes) {
        result = Inputs.GetResultInput(this, ghTyp);
        if (result == null) {
          return;
        }

        int axisId = -10;
        da.GetData(2, ref axisId);
        result.NodeSpringForces.SetStandardAxis(axisId);

        nodeList = Inputs.GetNodeListDefinition(this, da, 1, result.Model);
        ReadOnlyCollection<int> nodeIds = result.NodeIds(nodeList);
        IEntity0dResultSubset<IReactionForce, ResultVector6<Entity0dExtremaKey>> resultSet
          = result.NodeSpringForces.ResultSubset(nodeIds);

        List<int> permutations = result.SelectedPermutationIds ?? new List<int>() {
          1,
        };

        if (permutations.Count == 1 && permutations[0] == -1) {
          permutations = Enumerable.Range(1, resultSet.Subset.Values.First().Count).ToList();
        }

        if (_selectedItems[0] == ExtremaHelper.Vector6ReactionForces[0]) {
          foreach (KeyValuePair<int, IList<IReactionForce>> kvp in resultSet.Subset) {
            foreach (int p in permutations) {
              var path = new GH_Path(result.CaseId, result.SelectedPermutationIds == null ? 0 : p);
              outTransX.Add(new GH_UnitNumber(kvp.Value[p - 1].XToUnit(_forceUnit)), path);
              outTransY.Add(new GH_UnitNumber(kvp.Value[p - 1].YToUnit(_forceUnit)), path);
              outTransZ.Add(new GH_UnitNumber(kvp.Value[p - 1].ZToUnit(_forceUnit)), path);
              outTransXyz.Add(new GH_UnitNumber(kvp.Value[p - 1].XyzToUnit(_forceUnit)), path);
              outRotX.Add(new GH_UnitNumber(kvp.Value[p - 1].Xx), path);
              outRotY.Add(new GH_UnitNumber(kvp.Value[p - 1].Yy), path);
              outRotZ.Add(new GH_UnitNumber(kvp.Value[p - 1].Zz), path);
              outRotXyz.Add(new GH_UnitNumber(kvp.Value[p - 1].Xxyyzz), path);
              outIDs.Add(kvp.Key, path);
            }
          }
        } else {
          Entity0dExtremaKey key = ExtremaHelper.ReactionForceExtremaKey(resultSet, _selectedItems[0]);
          IReactionForce extrema = resultSet.GetExtrema(key);
          int perm = result.CaseType == CaseType.AnalysisCase ? 0 : 1;
          var path = new GH_Path(result.CaseId, key.Permutation + perm);
          outTransX.Add(new GH_UnitNumber(extrema.XToUnit(_forceUnit)), path);
          outTransY.Add(new GH_UnitNumber(extrema.YToUnit(_forceUnit)), path);
          outTransZ.Add(new GH_UnitNumber(extrema.ZToUnit(_forceUnit)), path);
          outTransXyz.Add(new GH_UnitNumber(extrema.XyzToUnit(_forceUnit)), path);
          outRotX.Add(new GH_UnitNumber(extrema.Xx), path);
          outRotY.Add(new GH_UnitNumber(extrema.Yy), path);
          outRotZ.Add(new GH_UnitNumber(extrema.Zz), path);
          outRotXyz.Add(new GH_UnitNumber(extrema.Xxyyzz), path);
          outIDs.Add(key.Id, path);
        }

        PostHog.Result(result.CaseType, 0, "Force", "Spring");
      }

      da.SetDataTree(0, outTransX);
      da.SetDataTree(1, outTransY);
      da.SetDataTree(2, outTransZ);
      da.SetDataTree(3, outTransXyz);
      da.SetDataTree(4, outRotX);
      da.SetDataTree(5, outRotY);
      da.SetDataTree(6, outRotZ);
      da.SetDataTree(7, outRotXyz);
      da.SetDataTree(8, outIDs);
    }

    protected override void UpdateUIFromSelectedItems() {
      if (_selectedItems.Count == 2) {
        _spacerDescriptions.Insert(0, "Max/Min");
        _dropDownItems.Insert(0, ExtremaHelper.Vector6ReactionForces.ToList());
        _selectedItems.Insert(0, _dropDownItems[0][0]);
      }

      _forceUnit = (ForceUnit)UnitsHelper.Parse(typeof(ForceUnit), _selectedItems[1]);
      _momentUnit = (MomentUnit)UnitsHelper.Parse(typeof(MomentUnit), _selectedItems[2]);

      base.UpdateUIFromSelectedItems();
    }
  }
}
