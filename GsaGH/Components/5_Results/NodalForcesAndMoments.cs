using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;

using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
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

using ForceUnit = OasysUnits.Units.ForceUnit;
using MomentUnit = OasysUnits.Units.MomentUnit;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to retrieve non-geometric objects from a GSA model
  /// </summary>
  public class NodalForcesAndMoments : GH_OasysDropDownComponent {
    public override Guid ComponentGuid => new Guid("3da705d4-d926-4c80-ab35-c64f4463fd3f");
    public override GH_Exposure Exposure => GH_Exposure.secondary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.NodalForcesAndMoments;
    private ForceUnit _forceUnit = DefaultUnits.ForceUnit;
    private MomentUnit _momentUnit = DefaultUnits.MomentUnit;
    public NodalForcesAndMoments() : base("Nodal Forces and Moments", "NodalForces",
      "Nodal Force and Moment result values", CategoryName.Name(), SubCategoryName.Cat5()) {
      Hidden = true;
    }

    public override void SetSelected(int i, int j) {
      _selectedItems[i] = _dropDownItems[i][j];
      switch (i) {
        case 0:
          _forceUnit
            = (ForceUnit)UnitsHelper.Parse(typeof(ForceUnit), _selectedItems[0]);
          break;

        case 1:
          _momentUnit = (MomentUnit)UnitsHelper.Parse(typeof(MomentUnit), _selectedItems[1]);
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
      Params.Output[i++].Name = "Force |F| [" + forceunitAbbreviation + "]";
      Params.Output[i++].Name = "Moment XX [" + momentunitAbbreviation + "]";
      Params.Output[i++].Name = "Moment YY [" + momentunitAbbreviation + "]";
      Params.Output[i++].Name = "Moment ZZ [" + momentunitAbbreviation + "]";
      Params.Output[i].Name = "Moment |M| [" + momentunitAbbreviation + "]";
    }

    protected override void InitialiseDropdowns() {
      _spacerDescriptions = new List<string>(new[] {
        "Force Unit",
        "Moment Unit",
      });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

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
        "Nodal Force in X-direction" + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Force Y [" + forceunitAbbreviation + "]", "Fy",
        "Nodal Force in Y-direction" + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Force Z [" + forceunitAbbreviation + "]", "Fz",
        "Nodal Force in Z-direction" + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Force |XYZ| [" + forceunitAbbreviation + "]", "|F|",
        "Combined |XYZ| Nodal Force" + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Moment XX [" + momentunitAbbreviation + "]", "Mxx",
        "Nodal Moment around X-axis" + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Moment YY [" + momentunitAbbreviation + "]", "Myy",
        "Nodal Moment around Y-axis" + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Moment ZZ [" + momentunitAbbreviation + "]", "Mzz",
        "Nodal Moment around Z-axis" + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Moment |XYZ| [" + momentunitAbbreviation + "]", "|M|",
        "Combined |XXYYZZ| Nodal Moment" + note, GH_ParamAccess.tree);
      pManager.AddIntegerParameter("Element IDs", "ID", "Element IDs for each result value",
        GH_ParamAccess.list);
    }

    protected override void SolveInternal(IGH_DataAccess da) {
      GsaResult result = null;
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
        result.NodalForcesAndMoments.SetStandardAxis(axisId);

        nodeList = Inputs.GetNodeListDefinition(this, da, 1, result.Model);
        ReadOnlyCollection<int> nodeIds = result.NodeIds(nodeList);
        Parameters.Results.NodalForcesAndMoments resultSet = result.NodalForcesAndMoments.ResultSubset(nodeIds);

        if (resultSet.Ids.Count < 1) {
          this.AddRuntimeWarning("There is no nodal forces and moments");
          return;
        }

        List<int> permutations = result.SelectedPermutationIds ?? new List<int>() {
          1,
        };
        if (permutations.Count == 1 && permutations[0] == -1) {
          permutations = Enumerable.Range(1, resultSet.Subset.Values.First().Count).ToList();
        }

        foreach (int id in resultSet.Ids) {
          foreach (int p in permutations) {
            IDictionary<int, IReactionForce> res = resultSet.Subset[id][p - 1];
            var path = new GH_Path(result.CaseId, result.SelectedPermutationIds == null ? 0 : p, id);
            foreach (KeyValuePair<int, IReactionForce> force in res) {
              outTransX.Add(new GH_UnitNumber(force.Value.XToUnit(_forceUnit)), path);
              outTransY.Add(new GH_UnitNumber(force.Value.YToUnit(_forceUnit)), path);
              outTransZ.Add(new GH_UnitNumber(force.Value.ZToUnit(_forceUnit)), path);
              outTransXyz.Add(new GH_UnitNumber(force.Value.XyzToUnit(_forceUnit)), path);
              outRotX.Add(new GH_UnitNumber(force.Value.XxToUnit(_momentUnit)), path);
              outRotY.Add(new GH_UnitNumber(force.Value.YyToUnit(_momentUnit)), path);
              outRotZ.Add(new GH_UnitNumber(force.Value.ZzToUnit(_momentUnit)), path);
              outRotXyz.Add(new GH_UnitNumber(force.Value.XxyyzzToUnit(_momentUnit)), path);
              outIDs.Add(force.Key, path);
            }
          }
        }

        PostHog.Result(result.CaseType, 2, "NodalForcesAndMoments");
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
      _forceUnit = (ForceUnit)UnitsHelper.Parse(typeof(ForceUnit), _selectedItems[0]);
      _momentUnit = (MomentUnit)UnitsHelper.Parse(typeof(MomentUnit), _selectedItems[1]);
      base.UpdateUIFromSelectedItems();
    }
  }
}
