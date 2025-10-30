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
  public class Element2dForcesAndMoments : GH_OasysDropDownComponent {
    public override Guid ComponentGuid => new Guid("ea42e671-710e-4fd3-a113-1724049159cf");
    public override GH_Exposure Exposure => GH_Exposure.quinary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.Element2dForcesAndMoments;
    private ForcePerLengthUnit _forceUnit = DefaultUnits.ForcePerLengthUnit;
    private ForceUnit _momentUnit = DefaultUnits.ForceUnit;
    private const string ForceComponentWarning = "Result is not available for the force component";
    private const string ShearComponentWarning = "Result is not available for the shear component";
    private const string MomentComponentWarning = "Result is not available for the moment component";


    public Element2dForcesAndMoments() : base("Element 2D Forces and Moments", "Forces2D",
      "2D Projected Force and Moment result values", CategoryName.Name(), SubCategoryName.Cat5()) {
      Hidden = true;
    }

    public override void SetSelected(int i, int j) {
      _selectedItems[i] = _dropDownItems[i][j];
      switch (i) {
        case 1:
          _forceUnit
            = (ForcePerLengthUnit)UnitsHelper.Parse(typeof(ForcePerLengthUnit), _selectedItems[1]);
          break;

        case 2:
          _momentUnit = (ForceUnit)UnitsHelper.Parse(typeof(ForceUnit), _selectedItems[2]);
          break;
      }

      base.UpdateUI();
    }

    public override void VariableParameterMaintenance() {
      string forceunitAbbreviation = ForcePerLength.GetAbbreviation(_forceUnit);
      string momentunitAbbreviation = Force.GetAbbreviation(_momentUnit);

      if (Params.Output.Count != 10) {
        Params.RegisterOutputParam(new Param_GenericObject());
        string momentrule = Environment.NewLine
          + "+ve moments correspond to +ve stress on the top (eg. Mx +ve if top Sxx +ve)";
        string note = ResultNotes.Note2dForceResults;
        Params.Output[8].NickName = "M*x";
        Params.Output[8].Description
          = "Element Wood-Armer Moment (Mx + sgn(Mx)·|Mxy|) around X-axis"
          + momentrule + note;
        Params.Output[8].Access = GH_ParamAccess.tree;

        Params.RegisterOutputParam(new Param_GenericObject());
        Params.Output[9].NickName = "M*y";
        Params.Output[9].Description
          = "Element Wood-Armer Moment (My + sgn(My)·|Mxy|) around Y-axis"
          + momentrule + note;
        Params.Output[9].Access = GH_ParamAccess.tree;

        Params.Output[6].Description
          = "Element Moment around Y-axis" + momentrule + note;
      }

      int i = 0;
      Params.Output[i++].Name = "Force X [" + forceunitAbbreviation + "]";
      Params.Output[i++].Name = "Force Y [" + forceunitAbbreviation + "]";
      Params.Output[i++].Name = "Force Z [" + forceunitAbbreviation + "]";
      Params.Output[i++].Name = "Shear X [" + forceunitAbbreviation + "]";
      Params.Output[i++].Name = "Shear Y [" + forceunitAbbreviation + "]";
      Params.Output[i++].Name = "Moment X [" + momentunitAbbreviation + "]";
      Params.Output[i++].Name = "Moment Y [" + momentunitAbbreviation + "]";
      Params.Output[i++].Name = "Moment XY [" + momentunitAbbreviation + "]";
      Params.Output[i++].Name = "Wood-Armer X [" + momentunitAbbreviation + "]";
      Params.Output[i].Name = "Wood-Armer Y [" + momentunitAbbreviation + "]";
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

      _dropDownItems.Add(ExtremaHelper.Elem2dForcesAndMoments.ToList());
      _selectedItems.Add(_dropDownItems[0][0]);

      _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.ForcePerLength));
      _selectedItems.Add(ForcePerLength.GetAbbreviation(_forceUnit));

      _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Force));
      _selectedItems.Add(Force.GetAbbreviation(_momentUnit));

      _isInitialised = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaResultParameter(), "Result", "Res", "GSA Result",
        GH_ParamAccess.list);
      pManager.AddParameter(new GsaElementMemberListParameter());
      pManager.AddIntegerParameter("Axis", "Ax", "Standard Axis: Global (0), Local (-1), Natural (-2), Default (-10), XElevation (-11), YElevation (-12), GlobalCylindrical (-13), Vertical (-14)", GH_ParamAccess.item);
      pManager[1].Optional = true;
      pManager[2].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      string forceunitAbbreviation = ForcePerLength.GetAbbreviation(_forceUnit);
      string momentunitAbbreviation = Force.GetAbbreviation(_momentUnit);

      string forcerule = Environment.NewLine + "+ve in plane force resultant: tensile";
      string momentrule = Environment.NewLine
        + "+ve moments correspond to +ve stress on the top (eg. Mx +ve if top Sxx +ve)";
      string note = Environment.NewLine
        + "DataTree organised as { CaseID ; Permutation ; ElementID } " + Environment.NewLine
        + "fx. {1;2;3} is Case 1, Permutation 2, Element 3, where each " + Environment.NewLine
        + "branch contains a list of results in the following order: " + Environment.NewLine
        + "Vertex(1), Vertex(2), ..., Vertex(i), Centre" + Environment.NewLine
        + "Element results are NOT averaged at nodes";

      pManager.AddGenericParameter("Force X [" + forceunitAbbreviation + "]", "Nx",
        "Element in-plane Force in X-direction" + forcerule + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Force Y [" + forceunitAbbreviation + "]", "Ny",
        "Element in-plane Force in Y-direction" + forcerule + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Force XY [" + forceunitAbbreviation + "]", "Nxy",
        "Element in-plane Force in XY-direction" + forcerule + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Shear X [" + forceunitAbbreviation + "]", "Qx",
        "Element through thickness Shear in XZ-plane" + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Shear Y [" + forceunitAbbreviation + "]", "Qz",
        "Element through thickness Shear in YZ-plane" + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Moment X [" + momentunitAbbreviation + "]", "Mx",
        "Element Moment around X-axis" + momentrule + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Moment Y [" + momentunitAbbreviation + "]", "My",
        "Element Moment around Y-axis" + momentrule + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Moment XY [" + momentunitAbbreviation + "]", "Mxy",
        "Element Moment around XY-axis" + momentrule + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Wood-Armer X [" + momentunitAbbreviation + "]", "M*x",
        "Element Wood-Armer Moment (Mx + sgn(Mx)·|Mxy|) around X-axis" + momentrule
        + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Wood-Armer Y [" + momentunitAbbreviation + "]", "M*y",
        "Element Wood-Armer Moment (My + sgn(My)·|Mxy|) around Y-axis" + momentrule
        + note, GH_ParamAccess.tree);
    }

    protected override void SolveInternal(IGH_DataAccess da) {
      GsaResult result = null;
      string elementlist = "All";

      var ghTypes = new List<GH_ObjectWrapper>();
      da.GetDataList(0, ghTypes);

      var outX = new DataTree<GH_UnitNumber>();
      var outY = new DataTree<GH_UnitNumber>();
      var outXy = new DataTree<GH_UnitNumber>();
      var outQx = new DataTree<GH_UnitNumber>();
      var outQy = new DataTree<GH_UnitNumber>();
      var outXx = new DataTree<GH_UnitNumber>();
      var outYy = new DataTree<GH_UnitNumber>();
      var outXxyy = new DataTree<GH_UnitNumber>();
      var outWaxx = new DataTree<GH_UnitNumber>();
      var outWayy = new DataTree<GH_UnitNumber>();

      foreach (GH_ObjectWrapper ghTyp in ghTypes) {
        result = Inputs.GetResultInput(this, ghTyp);
        if (result == null) {
          return;
        }

        int axisId = -10;
        da.GetData(2, ref axisId);
        result.Element2dForces.SetStandardAxis(axisId);
        result.Element2dShearForces.SetStandardAxis(axisId);
        result.Element2dMoments.SetStandardAxis(axisId);

        elementlist = Inputs.GetElementListDefinition(this, da, 1, result.Model);
        ReadOnlyCollection<int> elementIds = result.ElementIds(elementlist, 2);
        IMeshResultSubset<IMeshQuantity<IForce2d>, IForce2d, ResultTensor2InAxis<Entity2dExtremaKey>> forces
          = result.Element2dForces.ResultSubset(elementIds);
        IMeshResultSubset<IMeshQuantity<IShear2d>, IShear2d, ResultVector2<Entity2dExtremaKey>> shears = result.Element2dShearForces.ResultSubset(elementIds);
        IMeshResultSubset<IMeshQuantity<IMoment2d>, IMoment2d, ResultTensor2AroundAxis<Entity2dExtremaKey>> moments = result.Element2dMoments.ResultSubset(elementIds);

        List<int> permutations = result.SelectedPermutationIds ?? new List<int>() {
          1,
        };
        if (permutations.Count == 1 && permutations[0] == -1) {
          permutations = Enumerable.Range(1, forces.Subset.Values.First().Count).ToList();
        }

        if (_selectedItems[0] == ExtremaHelper.Elem2dForcesAndMoments[0]) {
          Parallel.For(0, 3, thread => {
            switch (thread) {
              case 0:
                foreach (KeyValuePair<int, IList<IMeshQuantity<IForce2d>>> kvp in forces
                 .Subset) {
                  foreach (int p in permutations) {
                    int idx = p - 1;
                    if (idx >= 0 && idx < kvp.Value.Count) {
                      var path = new GH_Path(result.CaseId,
                      result.SelectedPermutationIds == null ? 0 : p, kvp.Key);
                      outX.AddRange(
                        kvp.Value[idx].Results()
                         .Select(r => new GH_UnitNumber(r.Nx.ToUnit(_forceUnit))), path);
                      outY.AddRange(
                        kvp.Value[idx].Results()
                         .Select(r => new GH_UnitNumber(r.Ny.ToUnit(_forceUnit))), path);
                      outXy.AddRange(
                        kvp.Value[idx].Results()
                         .Select(r => new GH_UnitNumber(r.Nxy.ToUnit(_forceUnit))), path);
                    } else {
                      this.AddRuntimeWarning(ForceComponentWarning);
                    }
                  }
                }

                break;

              case 1:
                foreach (KeyValuePair<int, IList<IMeshQuantity<IShear2d>>> kvp in shears
                 .Subset) {
                  foreach (int p in permutations) {
                    int idx = p - 1;
                    if (idx >= 0 && idx < kvp.Value.Count) {
                      var path = new GH_Path(result.CaseId,
                      result.SelectedPermutationIds == null ? 0 : p, kvp.Key);
                      outQx.AddRange(
                        kvp.Value[idx].Results()
                         .Select(r => new GH_UnitNumber(r.Qx.ToUnit(_forceUnit))), path);
                      outQy.AddRange(
                        kvp.Value[idx].Results()
                         .Select(r => new GH_UnitNumber(r.Qy.ToUnit(_forceUnit))), path);
                    } else {
                      this.AddRuntimeWarning(ShearComponentWarning);
                    }
                  }
                }

                break;

              case 2:
                foreach (KeyValuePair<int, IList<IMeshQuantity<IMoment2d>>> kvp in moments
                 .Subset) {
                  foreach (int p in permutations) {
                    int idx = p - 1;
                    if (idx >= 0 && idx < kvp.Value.Count) {
                      var path = new GH_Path(result.CaseId,
                      result.SelectedPermutationIds == null ? 0 : p, kvp.Key);
                      outXx.AddRange(
                        kvp.Value[idx].Results()
                         .Select(r => new GH_UnitNumber(r.Mx.ToUnit(_momentUnit))), path);
                      outYy.AddRange(
                        kvp.Value[idx].Results()
                         .Select(r => new GH_UnitNumber(r.My.ToUnit(_momentUnit))), path);
                      outXxyy.AddRange(
                        kvp.Value[idx].Results()
                         .Select(r => new GH_UnitNumber(r.Mxy.ToUnit(_momentUnit))), path);
                      outWaxx.AddRange(
                        kvp.Value[idx].Results().Select(r
                          => new GH_UnitNumber(r.WoodArmerX.ToUnit(_momentUnit))), path);
                      outWayy.AddRange(
                        kvp.Value[idx].Results().Select(r
                          => new GH_UnitNumber(r.WoodArmerY.ToUnit(_momentUnit))), path);
                    } else {
                      this.AddRuntimeWarning(MomentComponentWarning);
                    }
                  }
                }
                break;
            }
          });
        } else {
          Entity2dExtremaKey key = ExtremaHelper.Elem2dForcesAndMomentsExtremaKey(forces, moments, shears, _selectedItems[0]);
          if (key != null) {
            int perm = result.CaseType == CaseType.AnalysisCase ? 0 : 1;
            var path = new GH_Path(result.CaseId, key.Permutation + perm, key.Id);
            string warningMessage = string.Empty;
            try {
              IForce2d forceExtrema = forces.GetExtrema(key);
              outX.Add(new GH_UnitNumber(forceExtrema.Nx.ToUnit(_forceUnit)), path);
              outY.Add(new GH_UnitNumber(forceExtrema.Ny.ToUnit(_forceUnit)), path);
              outXy.Add(new GH_UnitNumber(forceExtrema.Nxy.ToUnit(_forceUnit)), path);
            } catch (Exception) {
              this.AddRuntimeWarning(ForceComponentWarning);
            }

            try {
              IShear2d shearExtrema = shears.GetExtrema(key);
              outQx.Add(new GH_UnitNumber(shearExtrema.Qx.ToUnit(_forceUnit)), path);
              outQy.Add(new GH_UnitNumber(shearExtrema.Qy.ToUnit(_forceUnit)), path);
            } catch (Exception) {
              this.AddRuntimeWarning(ShearComponentWarning);
            }


            try {
              IMoment2d momentExtrema = moments.GetExtrema(key);
              outXx.Add(new GH_UnitNumber(momentExtrema.Mx.ToUnit(_momentUnit)), path);
              outYy.Add(new GH_UnitNumber(momentExtrema.My.ToUnit(_momentUnit)), path);
              outXxyy.Add(new GH_UnitNumber(momentExtrema.Mxy.ToUnit(_momentUnit)), path);
              outWaxx.Add(new GH_UnitNumber(momentExtrema.WoodArmerX.ToUnit(_momentUnit)), path);
              outWayy.Add(new GH_UnitNumber(momentExtrema.WoodArmerY.ToUnit(_momentUnit)), path);
            } catch (Exception) {
              this.AddRuntimeWarning(MomentComponentWarning);
            }
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
