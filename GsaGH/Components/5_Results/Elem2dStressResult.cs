using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using OasysGH;
using OasysGH.Components;
using OasysGH.Parameters;
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysUnits;
using OasysUnits.Units;

namespace GsaGH.Components {
  /// <summary>
  /// Component to retrieve non-geometric objects from a GSA model
  /// </summary>
  public class Elem2dStress : GH_OasysDropDownComponent {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("b5eb8a78-e0dd-442b-bbd7-0384d6c944cb");
    public override GH_Exposure Exposure => GH_Exposure.quinary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Properties.Resources.Stress2D;

    public Elem2dStress() : base("2D Stresses",
      "Stress2D",
      "2D Projected Stress result values",
      CategoryName.Name(),
      SubCategoryName.Cat5()) {
        Hidden = true;
    } // sets the initial state of the component to hidden
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaResultsParameter(), "Result", "Res", "GSA Result", GH_ParamAccess.list);
      pManager.AddTextParameter("Element filter list", "El", "Filter results by list." + Environment.NewLine +
          "Element list should take the form:" + Environment.NewLine +
          " 1 11 to 20 step 2 P1 not (G1 to G6 step 3) P11 not (PA PB1 PS2 PM3 PA4 M1)" + Environment.NewLine +
          "Refer to GSA help file for definition of lists and full vocabulary.", GH_ParamAccess.item, "All");
      pManager.AddNumberParameter("Stress Layer", "σL", "Layer within the cross-section to get results." +
                           Environment.NewLine + "Input a number between -1 and 1, representing the normalised thickness," +
                           Environment.NewLine + "default value is zero => middle of the element.", GH_ParamAccess.item, 0);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      string unitAbbreviation = Pressure.GetAbbreviation(_stresshUnit);

      string note = Environment.NewLine + "DataTree organised as { CaseID ; Permutation ; ElementID } " +
                    Environment.NewLine + "fx. {1;2;3} is Case 1, Permutation 2, Element 3, where each " +
          Environment.NewLine + "branch contains a list of results in the following order:" +
          Environment.NewLine + "Vertex(1), Vertex(2), ..., Vertex(i), Centre" +
          Environment.NewLine + "+ve in-plane stresses: tensile(ie. + ve direct strain)." +
          Environment.NewLine + "+ve bending stress gives rise to tension on the top surface." +
          Environment.NewLine + "+ve shear stresses: +ve shear strain.";

      pManager.AddGenericParameter("Stress XX [" + unitAbbreviation + "]", "xx", "Stress in XX-direction in Global Axis." + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Stress YY [" + unitAbbreviation + "]", "yy", "Stress in YY-direction in Global Axis." + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Stress ZZ [" + unitAbbreviation + "]", "zz", "Stress in ZZ-direction in Global Axis." + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Stress XY [" + unitAbbreviation + "]", "xy", "Stress in XY-direction in Global Axis." + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Stress YZ [" + unitAbbreviation + "]", "yz", "Stress in YZ-direction in Global Axis." + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Stress ZX [" + unitAbbreviation + "]", "zx", "Stress in ZX-direction in Global Axis." + note, GH_ParamAccess.tree);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess da) {
      var result = new GsaResult();

      string elementlist = "All";
      var ghType = new GH_String();
      if (da.GetData(1, ref ghType))
        GH_Convert.ToString(ghType, out elementlist, GH_Conversion.Both);

      if (elementlist.ToLower() == "all" || elementlist == "")
        elementlist = "All";

      double layer = 0;
      var ghType1 = new GH_String();
      if (da.GetData(2, ref ghType1))
        GH_Convert.ToDouble(ghType1, out layer, GH_Conversion.Both);

      var outXx = new DataTree<GH_UnitNumber>();
      var outYy = new DataTree<GH_UnitNumber>();
      var outZz = new DataTree<GH_UnitNumber>();
      var outXy = new DataTree<GH_UnitNumber>();
      var outYz = new DataTree<GH_UnitNumber>();
      var outZx = new DataTree<GH_UnitNumber>();

      var ghTypes = new List<GH_ObjectWrapper>();
      if (!da.GetDataList(0, ghTypes)) {
        return;
      }

      foreach (GH_ObjectWrapper ghTyp in ghTypes)
      {
        switch (ghTyp?.Value)
        {
          case null:
            this.AddRuntimeWarning("Input is null");
            return;
          case GsaResultGoo goo:
            result = goo.Value;
            break;
          default:
            this.AddRuntimeError("Error converting input to GSA Result");
            return;
        }

        List<GsaResultsValues> vals = result.Element2DStressValues(elementlist, layer, _stresshUnit);

        List<int> permutations = result.SelectedPermutationIds ?? new List<int>() { 1 };
        if (permutations.Count == 1 && permutations[0] == -1)
          permutations = Enumerable.Range(1, vals.Count).ToList();

        foreach (int perm in permutations) {
          if (vals[perm - 1].xyzResults.Count == 0 & vals[perm - 1].xxyyzzResults.Count == 0) {
            string acase = result.ToString().Replace('}', ' ').Replace('{', ' ');
            this.AddRuntimeWarning("Case " + acase + " contains no Element2D results.");
            continue;
          }
          Parallel.For(0, 2, thread => // split computation in two for xyz and xxyyzz
          {
            switch (thread)
            {
              case 0: {
                foreach (KeyValuePair<int, ConcurrentDictionary<int, GsaResultQuantity>> kvp in vals[perm - 1].xyzResults) {
                  int elementId = kvp.Key;
                  ConcurrentDictionary<int, GsaResultQuantity> res = kvp.Value;
                  if (res.Count == 0) { continue; }

                  var path = new GH_Path(
                    result.CaseId,
                    result.SelectedPermutationIds == null
                      ? 0
                      : perm,
                    elementId);

                  outXx.AddRange(res.Select(x => new GH_UnitNumber(x.Value.X.ToUnit(_stresshUnit))), path); // use ToUnit to capture changes in dropdown
                  outYy.AddRange(res.Select(x => new GH_UnitNumber(x.Value.Y.ToUnit(_stresshUnit))), path);
                  outZz.AddRange(res.Select(x => new GH_UnitNumber(x.Value.Z.ToUnit(_stresshUnit))), path);
                }

                break;
              }
              case 1: {
                foreach (KeyValuePair<int, ConcurrentDictionary<int, GsaResultQuantity>> kvp in vals[perm - 1].xxyyzzResults) {
                  int elementId = kvp.Key;
                  ConcurrentDictionary<int, GsaResultQuantity> res = kvp.Value;
                  if (res.Count == 0) { continue; }

                  var path = new GH_Path(
                    result.CaseId,
                    result.SelectedPermutationIds == null
                      ? 0
                      : perm,
                    elementId);

                  outXy.AddRange(res.Select(x => new GH_UnitNumber(x.Value.X.ToUnit(_stresshUnit))), path); // always use [rad] units
                  outYz.AddRange(res.Select(x => new GH_UnitNumber(x.Value.Y.ToUnit(_stresshUnit))), path);
                  outZx.AddRange(res.Select(x => new GH_UnitNumber(x.Value.Z.ToUnit(_stresshUnit))), path);
                }

                break;
              }
            }
          });
        }
      }

      da.SetDataTree(0, outXx);
      da.SetDataTree(1, outYy);
      da.SetDataTree(2, outZz);
      da.SetDataTree(3, outXy);
      da.SetDataTree(4, outYz);
      da.SetDataTree(5, outZx);

      Helpers.PostHog.Result(result.Type, 2, GsaResultsValues.ResultType.Stress);
    }

    #region Custom UI
    private PressureUnit _stresshUnit = DefaultUnits.StressUnitResult;
    public override void InitialiseDropdowns() {
      SpacerDescriptions = new List<string>(new []
        {
          "Unit",
        });

      DropDownItems = new List<List<string>>();
      SelectedItems = new List<string>();

      // Stress
      DropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Stress));
      SelectedItems.Add(_stresshUnit.ToString());

      IsInitialised = true;
    }

    public override void SetSelected(int i, int j) {
      SelectedItems[i] = DropDownItems[i][j];
      _stresshUnit = (PressureUnit)UnitsHelper.Parse(typeof(PressureUnit), SelectedItems[i]);
      base.UpdateUI();
    }
    public override void UpdateUIFromSelectedItems() {
      _stresshUnit = (PressureUnit)UnitsHelper.Parse(typeof(PressureUnit), SelectedItems[0]);
      base.UpdateUIFromSelectedItems();
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
    #endregion
  }
}

