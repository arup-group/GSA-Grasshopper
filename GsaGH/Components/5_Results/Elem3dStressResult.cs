using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using GsaGH.Helpers;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
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
  public class Elem3dStress : GH_OasysDropDownComponent {
    public override Guid ComponentGuid => new Guid("c9bdab98-0fe2-4852-b99c-c626515b3781");
    public override GH_Exposure Exposure => GH_Exposure.senary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.Stress3D;
    private PressureUnit _stresshUnit = DefaultUnits.StressUnitResult;

    public Elem3dStress() : base("3D Stresses", "Stress3D", "3D Element Stress result values",
      CategoryName.Name(), SubCategoryName.Cat5()) {
      Hidden = true;
    }

    public override void SetSelected(int i, int j) {
      _selectedItems[i] = _dropDownItems[i][j];
      _stresshUnit = (PressureUnit)UnitsHelper.Parse(typeof(PressureUnit), _selectedItems[i]);
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

    protected override void InitialiseDropdowns() {
      _spacerDescriptions = new List<string>(new[] {
        "Unit",
      });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Stress));
      _selectedItems.Add(_stresshUnit.ToString());

      _isInitialised = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaResultsParameter(), "Result", "Res", "GSA Result",
        GH_ParamAccess.list);
      pManager.AddGenericParameter("Element filter list", "El",
        "Filter results by list (by default 'all')" + Environment.NewLine
        + "Input a GSA List or a text string taking the form:" + Environment.NewLine
        + " 1 11 to 20 step 2 P1 not (G1 to G6 step 3) P11 not (PA PB1 PS2 PM3 PA4 M1)"
        + Environment.NewLine
        + "Refer to GSA help file for definition of lists and full vocabulary.",
        GH_ParamAccess.item);
      pManager[1].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      string unitAbbreviation = Pressure.GetAbbreviation(_stresshUnit);

      string note = Environment.NewLine
        + "DataTree organised as { CaseID ; Permutation ; ElementID } " + Environment.NewLine
        + "fx. {1;2;3} is Case 1, Permutation 2, Element 3, where each " + Environment.NewLine
        + "branch contains a list of results in the following order:" + Environment.NewLine
        + "Vertex(1), Vertex(2), ..., Vertex(i), Centre." + Environment.NewLine
        + "+ve stresses: tensile (ie. +ve direct strain)";

      pManager.AddGenericParameter("Stress XX [" + unitAbbreviation + "]", "xx",
        "Stress in XX-direction in Global Axis." + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Stress YY [" + unitAbbreviation + "]", "yy",
        "Stress in YY-direction in Global Axis." + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Stress ZZ [" + unitAbbreviation + "]", "zz",
        "Stress in ZZ-direction in Global Axis." + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Stress XY [" + unitAbbreviation + "]", "xy",
        "Stress in XY-direction in Global Axis." + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Stress YZ [" + unitAbbreviation + "]", "yz",
        "Stress in YZ-direction in Global Axis." + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Stress ZX [" + unitAbbreviation + "]", "zx",
        "Stress in ZX-direction in Global Axis." + note, GH_ParamAccess.tree);
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      var result = new GsaResult();

      string elementlist = "All";
      var ghType = new GH_ObjectWrapper();
      if (da.GetData(1, ref ghType)) {
        if (ghType.Value is GsaListGoo listGoo) {
          if (listGoo.Value.EntityType != EntityType.Element
            && listGoo.Value.EntityType != EntityType.Member) {
            this.AddRuntimeWarning(
            "List must be of type Element to apply to element filter");
          }
          elementlist = listGoo.Value.EntityType == EntityType.Member
            ? "\"" + "Child Elements of " + listGoo.Value.Name + "\""
            : "\"" + listGoo.Value.Name + "\"";
        } else {
          GH_Convert.ToString(ghType.Value, out elementlist, GH_Conversion.Both);
        }
      }

      if (string.IsNullOrEmpty(elementlist) || elementlist.ToLower() == "all") {
        elementlist = "All";
      }

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

      foreach (GH_ObjectWrapper ghTyp in ghTypes) {
        switch (ghTyp?.Value) {
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

        List<GsaResultsValues> vals = result.Element3DStressValues(elementlist, _stresshUnit);

        List<int> permutations = result.SelectedPermutationIds ?? new List<int>() {
          1,
        };
        if (permutations.Count == 1 && permutations[0] == -1) {
          permutations = Enumerable.Range(1, vals.Count).ToList();
        }

        foreach (int perm in permutations) {
          if (vals[perm - 1].XyzResults.Count == 0 & vals[perm - 1].XxyyzzResults.Count == 0) {
            string acase = result.ToString().Replace('}', ' ').Replace('{', ' ');
            this.AddRuntimeWarning("Case " + acase + " contains no Element3D results.");
            continue;
          }

          Parallel.For(0, 2, thread => // split computation in two for xyz and xxyyzz
          {
            switch (thread) {
              case 0: {
                foreach (KeyValuePair<int, ConcurrentDictionary<int, GsaResultQuantity>> kvp in
                  vals[perm - 1].XyzResults) {
                  int elementId = kvp.Key;
                  ConcurrentDictionary<int, GsaResultQuantity> res = kvp.Value;
                  if (res.Count == 0) {
                    continue;
                  }

                  var path = new GH_Path(result.CaseId,
                    result.SelectedPermutationIds == null ? 0 : perm, elementId);

                  outXx.AddRange(res.Select(x => new GH_UnitNumber(x.Value.X.ToUnit(_stresshUnit))),
                    path); // use ToUnit to capture changes in dropdown
                  outYy.AddRange(res.Select(x => new GH_UnitNumber(x.Value.Y.ToUnit(_stresshUnit))),
                    path);
                  outZz.AddRange(res.Select(x => new GH_UnitNumber(x.Value.Z.ToUnit(_stresshUnit))),
                    path);
                }

                break;
              }
              case 1: {
                foreach (KeyValuePair<int, ConcurrentDictionary<int, GsaResultQuantity>> kvp in
                  vals[perm - 1].XxyyzzResults) {
                  int elementId = kvp.Key;
                  ConcurrentDictionary<int, GsaResultQuantity> res = kvp.Value;
                  if (res.Count == 0) {
                    continue;
                  }

                  var path = new GH_Path(result.CaseId,
                    result.SelectedPermutationIds == null ? 0 : perm, elementId);

                  outXy.AddRange(res.Select(x => new GH_UnitNumber(x.Value.X.ToUnit(_stresshUnit))),
                    path);
                  outYz.AddRange(res.Select(x => new GH_UnitNumber(x.Value.Y.ToUnit(_stresshUnit))),
                    path);
                  outZx.AddRange(res.Select(x => new GH_UnitNumber(x.Value.Z.ToUnit(_stresshUnit))),
                    path);
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

      PostHog.Result(result.Type, 3, GsaResultsValues.ResultType.Stress);
    }

    protected override void UpdateUIFromSelectedItems() {
      _stresshUnit = (PressureUnit)UnitsHelper.Parse(typeof(PressureUnit), _selectedItems[0]);
      base.UpdateUIFromSelectedItems();
    }
  }
}
