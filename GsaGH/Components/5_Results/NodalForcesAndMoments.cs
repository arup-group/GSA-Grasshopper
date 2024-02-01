using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
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
        ReadOnlyCollection<int> elementIds = result.NodeIds(nodeList);
        IMeshResultSubset<IMeshQuantity<IForce2d>, IForce2d, ResultTensor2InAxis<Entity2dExtremaKey>> forces
          = result.for.ResultSubset(elementIds);
        IMeshResultSubset<IMeshQuantity<IShear2d>, IShear2d, ResultVector2<Entity2dExtremaKey>> shears = result.Element2dShearForces.ResultSubset(elementIds);
        IMeshResultSubset<IMeshQuantity<IMoment2d>, IMoment2d, ResultTensor2AroundAxis<Entity2dExtremaKey>> moments = result.Element2dMoments.ResultSubset(elementIds);

        List<int> permutations = result.SelectedPermutationIds ?? new List<int>() {
          1,
        };
        if (permutations.Count == 1 && permutations[0] == -1) {
          permutations = Enumerable.Range(1, forces.Subset.Values.First().Count).ToList();
        }

        if (_selectedItems[0] == ExtremaHelper.Vector6ReactionForces[0]) {
          Parallel.For(0, 3, thread => {
            switch (thread) {
              case 0:
                foreach (KeyValuePair<int, IList<IMeshQuantity<IForce2d>>> kvp in forces
                 .Subset) {
                  foreach (int p in permutations) {
                    var path = new GH_Path(result.CaseId,
                      result.SelectedPermutationIds == null ? 0 : p, kvp.Key);
                    outX.AddRange(
                      kvp.Value[p - 1].Results()
                       .Select(r => new GH_UnitNumber(r.Nx.ToUnit(_forceUnit))), path);
                    outY.AddRange(
                      kvp.Value[p - 1].Results()
                       .Select(r => new GH_UnitNumber(r.Ny.ToUnit(_forceUnit))), path);
                    outXy.AddRange(
                      kvp.Value[p - 1].Results()
                       .Select(r => new GH_UnitNumber(r.Nxy.ToUnit(_forceUnit))), path);
                  }
                }

                break;

              case 1:
                foreach (KeyValuePair<int, IList<IMeshQuantity<IShear2d>>> kvp in shears
                 .Subset) {
                  foreach (int p in permutations) {
                    var path = new GH_Path(result.CaseId,
                      result.SelectedPermutationIds == null ? 0 : p, kvp.Key);
                    outQx.AddRange(
                      kvp.Value[p - 1].Results()
                       .Select(r => new GH_UnitNumber(r.Qx.ToUnit(_forceUnit))), path);
                    outQy.AddRange(
                      kvp.Value[p - 1].Results()
                       .Select(r => new GH_UnitNumber(r.Qy.ToUnit(_forceUnit))), path);
                  }
                }

                break;

              case 2:
                foreach (KeyValuePair<int, IList<IMeshQuantity<IMoment2d>>> kvp in moments
                 .Subset) {
                  foreach (int p in permutations) {
                    var path = new GH_Path(result.CaseId,
                      result.SelectedPermutationIds == null ? 0 : p, kvp.Key);
                    outXx.AddRange(
                      kvp.Value[p - 1].Results()
                       .Select(r => new GH_UnitNumber(r.Mx.ToUnit(_momentUnit))), path);
                    outYy.AddRange(
                      kvp.Value[p - 1].Results()
                       .Select(r => new GH_UnitNumber(r.My.ToUnit(_momentUnit))), path);
                    outXxyy.AddRange(
                      kvp.Value[p - 1].Results()
                       .Select(r => new GH_UnitNumber(r.Mxy.ToUnit(_momentUnit))), path);
                    outWaxx.AddRange(
                      kvp.Value[p - 1].Results().Select(r
                        => new GH_UnitNumber(r.WoodArmerX.ToUnit(_momentUnit))), path);
                    outWayy.AddRange(
                      kvp.Value[p - 1].Results().Select(r
                        => new GH_UnitNumber(r.WoodArmerY.ToUnit(_momentUnit))), path);
                  }
                }

                break;
            }
          });
        } else {
          Entity2dExtremaKey key = ExtremaHelper.Elem2dForcesAndMomentsExtremaKey(forces, moments, shears, _selectedItems[0]);
          if (key != null) {
            IForce2d forceExtrema = forces.GetExtrema(key);
            int perm = result.CaseType == CaseType.AnalysisCase ? 0 : 1;
            var path = new GH_Path(result.CaseId, key.Permutation + perm, key.Id);
            outX.Add(new GH_UnitNumber(forceExtrema.Nx.ToUnit(_forceUnit)), path);
            outY.Add(new GH_UnitNumber(forceExtrema.Ny.ToUnit(_forceUnit)), path);
            outXy.Add(new GH_UnitNumber(forceExtrema.Nxy.ToUnit(_forceUnit)), path);

            IShear2d shearExtrema = shears.GetExtrema(key);
            outQx.Add(new GH_UnitNumber(shearExtrema.Qx.ToUnit(_forceUnit)), path);
            outQy.Add(new GH_UnitNumber(shearExtrema.Qy.ToUnit(_forceUnit)), path);

            IMoment2d momentExtrema = moments.GetExtrema(key);
            outXx.Add(new GH_UnitNumber(momentExtrema.Mx.ToUnit(_momentUnit)), path);
            outYy.Add(new GH_UnitNumber(momentExtrema.My.ToUnit(_momentUnit)), path);
            outXxyy.Add(new GH_UnitNumber(momentExtrema.Mxy.ToUnit(_momentUnit)), path);
            outWaxx.Add(new GH_UnitNumber(momentExtrema.WoodArmerX.ToUnit(_momentUnit)), path);
            outWayy.Add(new GH_UnitNumber(momentExtrema.WoodArmerY.ToUnit(_momentUnit)), path);
          }
        }

        PostHog.Result(result.CaseType, 2, "Force");
      }

      da.SetDataTree(0, outX);
      da.SetDataTree(1, outY);
      da.SetDataTree(2, outXy);
      da.SetDataTree(3, outQx);
      da.SetDataTree(4, outQy);
      da.SetDataTree(5, outXx);
      da.SetDataTree(6, outYy);
      da.SetDataTree(7, outXxyy);
      da.SetDataTree(8, outWaxx);
      da.SetDataTree(9, outWayy);
    }

    protected override void UpdateUIFromSelectedItems() {
      if (_selectedItems.Count == 2) {
        _spacerDescriptions.Insert(0, "Max/Min");
        _dropDownItems.Insert(0, ExtremaHelper.Elem2dForcesAndMoments.ToList());
        _selectedItems.Insert(0, _dropDownItems[0][0]);
      }
      _forceUnit
        = (ForcePerLengthUnit)UnitsHelper.Parse(typeof(ForcePerLengthUnit), _selectedItems[1]);
      _momentUnit = (ForceUnit)UnitsHelper.Parse(typeof(ForceUnit), _selectedItems[2]);
      base.UpdateUIFromSelectedItems();
    }
  }
}
