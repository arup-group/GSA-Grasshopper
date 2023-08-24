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
using GsaGH.Components.Helpers;
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
  ///   Component to GSA 2D Element displacement values
  /// </summary>
  public class Elem2dDisplacement : GH_OasysDropDownComponent {
    public override Guid ComponentGuid => new Guid("22f87d33-4f9a-49d6-9f3d-b366e446a75f");
    public override GH_Exposure Exposure => GH_Exposure.quarternary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources._2DDisplacements;
    private LengthUnit _lengthUnit = DefaultUnits.LengthUnitResult;

    public Elem2dDisplacement() : base("2D Displacements", "Disp2D",
      "2D Translation and Rotation result values", CategoryName.Name(), SubCategoryName.Cat5()) {
      Hidden = true;
    }

    public override void SetSelected(int i, int j) {
      _selectedItems[i] = _dropDownItems[i][j];
      _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[i]);
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

    protected override void InitialiseDropdowns() {
      _spacerDescriptions = new List<string>(new[] {
        "Unit",
      });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
      _selectedItems.Add(Length.GetAbbreviation(_lengthUnit));

      _isInitialised = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaResultParameter(), "Result", "Res", "GSA Result",
        GH_ParamAccess.list);
      pManager.AddParameter(new GsaElementListParameter());
      pManager[1].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      string unitAbbreviation = Length.GetAbbreviation(_lengthUnit);

      string note = ResultNotes.Note2dResults;

      pManager.AddGenericParameter("Translations X [" + unitAbbreviation + "]", "Ux",
        "Translations in X-direction in Global Axis." + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Translations Y [" + unitAbbreviation + "]", "Uy",
        "Translations in Y-direction in Global Axis." + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Translations Z [" + unitAbbreviation + "]", "Uz",
        "Translations in Z-direction in Global Axis." + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Translations |XYZ| [" + unitAbbreviation + "]", "|U|",
        "Combined |XYZ| Translations." + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Rotations XX [rad]", "Rxx",
        "Rotations around X-axis in Global Axis." + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Rotations YY [rad]", "Ryy",
        "Rotations around Y-axis in Global Axiss." + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Rotations ZZ [rad]", "Rzz",
        "Rotations around Z-axis in Global Axis." + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Rotations |XYZ| [rad]", "|R|",
        "Combined |XXYYZZ| Rotations." + note, GH_ParamAccess.tree);
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      var result = new GsaResult();
      string elementlist = "All";

      var outTransX = new DataTree<GH_UnitNumber>();
      var outTransY = new DataTree<GH_UnitNumber>();
      var outTransZ = new DataTree<GH_UnitNumber>();
      var outTransXyz = new DataTree<GH_UnitNumber>();
      var outRotX = new DataTree<GH_UnitNumber>();
      var outRotY = new DataTree<GH_UnitNumber>();
      var outRotZ = new DataTree<GH_UnitNumber>();
      var outRotXyz = new DataTree<GH_UnitNumber>();

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
            elementlist = Inputs.GetElementListNameForResults(this, da, 1, result.Model);
            break;

          default:
            this.AddRuntimeError("Error converting input to GSA Result");
            return;
        }

        List<GsaResultsValues> vals = result.Element2DDisplacementValues(elementlist, _lengthUnit);

        List<int> permutations = result.SelectedPermutationIds ?? new List<int>() {
          1,
        };
        if (permutations.Count == 1 && permutations[0] == -1) {
          permutations = Enumerable.Range(1, vals.Count).ToList();
        }

        foreach (int perm in permutations) {
          if (vals[perm - 1].XyzResults.Count == 0 & vals[perm - 1].XxyyzzResults.Count == 0) {
            string acase = result.ToString().Replace('}', ' ').Replace('{', ' ');
            this.AddRuntimeWarning("Case " + acase + " contains no Element2D results.");
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

                  outTransX.AddRange(
                    res.Select(x => new GH_UnitNumber(x.Value.X.ToUnit(_lengthUnit))),
                    path); // use ToUnit to capture changes in dropdown
                  outTransY.AddRange(
                    res.Select(x => new GH_UnitNumber(x.Value.Y.ToUnit(_lengthUnit))), path);
                  outTransZ.AddRange(
                    res.Select(x => new GH_UnitNumber(x.Value.Z.ToUnit(_lengthUnit))), path);
                  outTransXyz.AddRange(
                    res.Select(x => new GH_UnitNumber(x.Value.Xyz.ToUnit(_lengthUnit))), path);
                }

                break;
              }
              case 1: {
                foreach (KeyValuePair<int, ConcurrentDictionary<int, GsaResultQuantity>> kvp in
                  vals[perm - 1].XxyyzzResults) {
                  int elementId = kvp.Key;
                  ConcurrentDictionary<int, GsaResultQuantity> res = kvp.Value;

                  var path = new GH_Path(result.CaseId,
                    result.SelectedPermutationIds == null ? 0 : perm, elementId);
                  if (res.Count == 0) {
                    continue;
                  }

                  outRotX.AddRange(res.Select(x => new GH_UnitNumber(x.Value.X)),
                    path); // always use [rad] units
                  outRotY.AddRange(res.Select(x => new GH_UnitNumber(x.Value.Y)), path);
                  outRotZ.AddRange(res.Select(x => new GH_UnitNumber(x.Value.Z)), path);
                  outRotXyz.AddRange(res.Select(x => new GH_UnitNumber(x.Value.Xyz)), path);
                }

                break;
              }
            }
          });
        }
      }

      da.SetDataTree(0, outTransX);
      da.SetDataTree(1, outTransY);
      da.SetDataTree(2, outTransZ);
      da.SetDataTree(3, outTransXyz);
      da.SetDataTree(4, outRotX);
      da.SetDataTree(5, outRotY);
      da.SetDataTree(6, outRotZ);
      da.SetDataTree(7, outRotXyz);

      PostHog.Result(result.Type, 2, GsaResultsValues.ResultType.Displacement);
    }

    protected override void UpdateUIFromSelectedItems() {
      _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[0]);
      base.UpdateUIFromSelectedItems();
    }
  }
}
