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
using OasysUnits.Units;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to get GSA reaction forces
  /// </summary>
  public class ReactionForces : GH_OasysDropDownComponent {
    public override Guid ComponentGuid => new Guid("37274bd4-aea7-4377-8462-c9035e75e316");
    public override GH_Exposure Exposure => GH_Exposure.secondary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.ReactionForces;
    private ForceUnit _forceUnit = DefaultUnits.ForceUnit;
    private MomentUnit _momentUnit = DefaultUnits.MomentUnit;

    public ReactionForces() : base("Reaction Forces", "ReactForce", "Reaction Force result values",
      CategoryName.Name(), SubCategoryName.Cat5()) {
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

    protected override void InitialiseDropdowns() {
      _spacerDescriptions = new List<string>(new[] {
        "Envelope",
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
        "Reaction Forces in Global X-direction" + axis + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Force Y [" + forceunitAbbreviation + "]", "Fy",
        "Reaction Forces in Global Y-direction" + axis + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Force Z [" + forceunitAbbreviation + "]", "Fz",
        "Reaction Forces in Global Z-direction" + axis + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Force |XYZ| [" + forceunitAbbreviation + "]", "|F|",
        "Combined |XYZ| Reaction Forces" + axis + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Moment XX [" + momentunitAbbreviation + "]", "Mxx",
        "Reaction Moments around Global X-axis" + axis + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Moment YY [" + momentunitAbbreviation + "]", "Myy",
        "Reaction Moments around Global Y-axis" + axis + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Moment ZZ [" + momentunitAbbreviation + "]", "Mzz",
        "Reaction Moments around Global Z-axis" + axis + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Moment |XYZ| [" + momentunitAbbreviation + "]", "|M|",
        "Combined |XXYYZZ| Reaction Moments" + axis + note, GH_ParamAccess.tree);
      pManager.AddIntegerParameter("Nodes IDs", "ID", "Node IDs for each result value",
        GH_ParamAccess.list);
    }

    protected override void SolveInternal(IGH_DataAccess da) {
      GsaResult result;
      string nodeList = "All";

      var outTransX = new DataTree<GH_UnitNumber>();
      var outTransY = new DataTree<GH_UnitNumber>();
      var outTransZ = new DataTree<GH_UnitNumber>();
      var outTransXyz = new DataTree<GH_UnitNumber>();
      var outRotX = new DataTree<GH_UnitNumber>();
      var outRotY = new DataTree<GH_UnitNumber>();
      var outRotZ = new DataTree<GH_UnitNumber>();
      var outRotXyz = new DataTree<GH_UnitNumber>();
      var outIDs = new DataTree<int>();

      var ghTypes = new List<GH_ObjectWrapper>();
      if (!da.GetDataList(0, ghTypes)) {
        return;
      }

      foreach (GH_ObjectWrapper ghTyp in ghTypes) {
        switch (ghTyp?.Value) {
          case null:
            this.AddRuntimeWarning("Input is null");
            return;

          case GsaResultGoo goo:
            result = (GsaResult)goo.Value;
            nodeList = Inputs.GetNodeListDefinition(this, da, 1, result.Model);
            break;

          default:
            this.AddRuntimeError("Error converting input to GSA Result");
            return;
        }

        ReadOnlyCollection<int> nodeIds = result.NodeIds(nodeList);
        INodeResultSubset<IInternalForce, ResultVector6<NodeExtremaKey>> resultSet
          = result.NodeReactionForces.ResultSubset(nodeIds);

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
              IInternalForce res = resultSet.Subset[id][p - 1];
              outTransX.Add(new GH_UnitNumber(res.X.ToUnit(_forceUnit)), path);
              outTransY.Add(new GH_UnitNumber(res.Y.ToUnit(_forceUnit)), path);
              outTransZ.Add(new GH_UnitNumber(res.Z.ToUnit(_forceUnit)), path);
              outTransXyz.Add(new GH_UnitNumber(res.Xyz.ToUnit(_forceUnit)), path);
              outRotX.Add(new GH_UnitNumber(res.Xx.ToUnit(_momentUnit)), path);
              outRotY.Add(new GH_UnitNumber(res.Yy.ToUnit(_momentUnit)), path);
              outRotZ.Add(new GH_UnitNumber(res.Zz.ToUnit(_momentUnit)), path);
              outRotXyz.Add(new GH_UnitNumber(res.Xxyyzz.ToUnit(_momentUnit)), path);
              outIDs.Add(id, path);
            }
          }
        } else {
          NodeExtremaKey key = ExtremaHelper.ReactionForceExtremaKey(resultSet, _selectedItems[0]);
          IInternalForce extrema = resultSet.GetExtrema(key);
          var path = new GH_Path(result.CaseId, key.Permutation);
          outTransX.Add(new GH_UnitNumber(extrema.X.ToUnit(_forceUnit)), path);
          outTransY.Add(new GH_UnitNumber(extrema.Y.ToUnit(_forceUnit)), path);
          outTransZ.Add(new GH_UnitNumber(extrema.Z.ToUnit(_forceUnit)), path);
          outTransXyz.Add(new GH_UnitNumber(extrema.Xyz.ToUnit(_forceUnit)), path);
          outRotX.Add(new GH_UnitNumber(extrema.Xx.ToUnit(_momentUnit)), path);
          outRotY.Add(new GH_UnitNumber(extrema.Yy.ToUnit(_momentUnit)), path);
          outRotZ.Add(new GH_UnitNumber(extrema.Zz.ToUnit(_momentUnit)), path);
          outRotXyz.Add(new GH_UnitNumber(extrema.Xxyyzz.ToUnit(_momentUnit)), path);
          outIDs.Add(key.Id, path);
        }

        PostHog.Result(result.CaseType, 0, "Force");
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
        _spacerDescriptions.Insert(0, "Envelope");
        _dropDownItems.Insert(0, ExtremaHelper.Vector6ReactionForces.ToList());
        _selectedItems.Insert(0, _dropDownItems[0][0]);
      }

      _forceUnit = (ForceUnit)UnitsHelper.Parse(typeof(ForceUnit), _selectedItems[1]);
      _momentUnit = (MomentUnit)UnitsHelper.Parse(typeof(MomentUnit), _selectedItems[2]);

      base.UpdateUIFromSelectedItems();
    }
  }
}
