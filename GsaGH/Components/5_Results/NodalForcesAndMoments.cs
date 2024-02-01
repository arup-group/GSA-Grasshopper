﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using GH_IO.Serialization;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;
using GsaAPI;
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
using ForceUnit = OasysUnits.Units.ForceUnit;
using MomentUnit = OasysUnits.Units.MomentUnit;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to retrieve non-geometric objects from a GSA model
  /// </summary>
  public class NodalForcesAndMoments : GH_OasysDropDownComponent {
    public override Guid ComponentGuid => new Guid("3da705d4-d926-4c80-ab35-c64f4463fd3f");
    public override GH_Exposure Exposure => GH_Exposure.quinary;
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
        case 1:
          _forceUnit
            = (ForceUnit)UnitsHelper.Parse(typeof(ForceUnit), _selectedItems[1]);
          break;

        case 2:
          _momentUnit = (MomentUnit)UnitsHelper.Parse(typeof(MomentUnit), _selectedItems[2]);
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
      pManager[1].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      string forceunitAbbreviation = Force.GetAbbreviation(_forceUnit);
      string momentunitAbbreviation = Moment.GetAbbreviation(_momentUnit);

      string note = ResultNotes.NoteNodeResults;
      string axis = " in Global Axis.";

      pManager.AddGenericParameter("Force X [" + forceunitAbbreviation + "]", "Fx",
        "Nodal Forces in Global X-direction" + axis + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Force Y [" + forceunitAbbreviation + "]", "Fy",
        "Nodal Forces in Global Y-direction" + axis + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Force Z [" + forceunitAbbreviation + "]", "Fz",
        "Nodal Forces in Global Z-direction" + axis + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Force |XYZ| [" + forceunitAbbreviation + "]", "|F|",
        "Combined |XYZ| Nodal Forces" + axis + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Moment XX [" + momentunitAbbreviation + "]", "Mxx",
        "Nodal Moments around Global X-axis" + axis + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Moment YY [" + momentunitAbbreviation + "]", "Myy",
        "Nodal Moments around Global Y-axis" + axis + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Moment ZZ [" + momentunitAbbreviation + "]", "Mzz",
        "Nodal Moments around Global Z-axis" + axis + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Moment |XYZ| [" + momentunitAbbreviation + "]", "|M|",
        "Combined |XXYYZZ| Nodal Moments" + axis + note, GH_ParamAccess.tree);
      pManager.AddIntegerParameter("Nodes IDs", "ID", "Node IDs for each result value",
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

        nodeList = Inputs.GetNodeListDefinition(this, da, 1, result.Model);
        ReadOnlyCollection<int> nodeIds = result.NodeIds(nodeList);
        IEntity0dResultSubset<IReactionForce, ResultVector6<Entity0dExtremaKey>> resultSet = result.NodalForcesAndMoments.ResultSubset(nodeIds);

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

        if (_selectedItems[0] == ExtremaHelper.Vector6ReactionForces[0]) {
          foreach (int id in resultSet.Ids) {
            foreach (int p in permutations) {
              var path = new GH_Path(result.CaseId, result.SelectedPermutationIds == null ? 0 : p);
              IReactionForce res = resultSet.Subset[id][p - 1];
              outTransX.Add(new GH_UnitNumber(res.XToUnit(_forceUnit)), path);
              outTransY.Add(new GH_UnitNumber(res.YToUnit(_forceUnit)), path);
              outTransZ.Add(new GH_UnitNumber(res.ZToUnit(_forceUnit)), path);
              outTransXyz.Add(new GH_UnitNumber(res.XyzToUnit(_forceUnit)), path);
              outRotX.Add(new GH_UnitNumber(res.XxToUnit(_momentUnit)), path);
              outRotY.Add(new GH_UnitNumber(res.YyToUnit(_momentUnit)), path);
              outRotZ.Add(new GH_UnitNumber(res.ZzToUnit(_momentUnit)), path);
              outRotXyz.Add(new GH_UnitNumber(res.XxyyzzToUnit(_momentUnit)), path);
              outIDs.Add(id, path);
            }
          }
        } else {
          Entity0dExtremaKey key = ExtremaHelper.ReactionForceExtremaKey(resultSet, _selectedItems[0]);
          IReactionForce extrema = resultSet.GetExtrema(key);
          int perm = result.CaseType == CaseType.AnalysisCase ? 0 : 1;
          var path = new GH_Path(result.CaseId, key.Permutation + perm, key.Id);
          outTransX.Add(new GH_UnitNumber(extrema.XToUnit(_forceUnit)), path);
          outTransY.Add(new GH_UnitNumber(extrema.YToUnit(_forceUnit)), path);
          outTransZ.Add(new GH_UnitNumber(extrema.ZToUnit(_forceUnit)), path);
          outTransXyz.Add(new GH_UnitNumber(extrema.XyzToUnit(_forceUnit)), path);
          outRotX.Add(new GH_UnitNumber(extrema.XxToUnit(_momentUnit)), path);
          outRotY.Add(new GH_UnitNumber(extrema.YyToUnit(_momentUnit)), path);
          outRotZ.Add(new GH_UnitNumber(extrema.ZzToUnit(_momentUnit)), path);
          outRotXyz.Add(new GH_UnitNumber(extrema.XxyyzzToUnit(_momentUnit)), path);
          outIDs.Add(key.Id, path);
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
      _forceUnit = (ForceUnit)UnitsHelper.Parse(typeof(ForceUnit), _selectedItems[1]);
      _momentUnit = (MomentUnit)UnitsHelper.Parse(typeof(MomentUnit), _selectedItems[2]);
      base.UpdateUIFromSelectedItems();
    }
  }
}
