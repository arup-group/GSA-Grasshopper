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
  ///   Component to get GSA beam displacement values
  /// </summary>
  public class BeamDisplacement : GH_OasysDropDownComponent {
    public override Guid ComponentGuid => new Guid("21ec9005-1b2f-4eb8-8171-b2c0190a4a54");
    public override GH_Exposure Exposure => GH_Exposure.quarternary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.BeamDisplacement;
    private LengthUnit _lengthUnit = DefaultUnits.LengthUnitResult;

    public BeamDisplacement() : base("Beam Displacements",
              "BeamDisp",
      "Element1D Translation and Rotation result values",
      CategoryName.Name(),
      SubCategoryName.Cat5()) {
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
      Params.Output[i++]
        .Name = "Translations X [" + unitAbbreviation + "]";
      Params.Output[i++]
        .Name = "Translations Y [" + unitAbbreviation + "]";
      Params.Output[i++]
        .Name = "Translations Z [" + unitAbbreviation + "]";
      Params.Output[i]
        .Name = "Translations |XYZ| [" + unitAbbreviation + "]";
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
      pManager.AddParameter(new GsaResultsParameter(),
        "Result",
        "Res",
        "GSA Result",
        GH_ParamAccess.list);
      pManager.AddTextParameter("Element filter list",
        "El",
        "Filter results by list."
        + Environment.NewLine
        + "Element list should take the form:"
        + Environment.NewLine
        + " 1 11 to 20 step 2 P1 not (G1 to G6 step 3) P11 not (PA PB1 PS2 PM3 PA4 M1)"
        + Environment.NewLine
        + "Refer to GSA help file for definition of lists and full vocabulary.",
        GH_ParamAccess.item,
        "All");
      pManager.AddIntegerParameter("Intermediate Points",
        "nP",
        "Number of intermediate equidistant points (default 3)",
        GH_ParamAccess.item,
        3);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      string unitAbbreviation = Length.GetAbbreviation(_lengthUnit);

      string note = Environment.NewLine
        + "DataTree organised as { CaseID ; Permutation ; ElementID } "
        + Environment.NewLine
        + "fx. {1;2;3} is Case 1, Permutation 2, Element 3, where each "
        + Environment.NewLine
        + "branch contains a list of results per element position.";

      pManager.AddGenericParameter("Translations X [" + unitAbbreviation + "]",
        "Ux",
        "Translations in X-direction in Global Axis." + note,
        GH_ParamAccess.tree);
      pManager.AddGenericParameter("Translations Y [" + unitAbbreviation + "]",
        "Uy",
        "Translations in Y-direction in Global Axis." + note,
        GH_ParamAccess.tree);
      pManager.AddGenericParameter("Translations Z [" + unitAbbreviation + "]",
        "Uz",
        "Translations in Z-direction in Global Axis." + note,
        GH_ParamAccess.tree);
      pManager.AddGenericParameter("Translations |XYZ| [" + unitAbbreviation + "]",
        "|U|",
        "Combined |XYZ| Translations in Global Axis." + note,
        GH_ParamAccess.tree);
      pManager.AddGenericParameter("Rotations XX [rad]",
        "Rxx",
        "Rotations around X-axis in Global Axis." + note,
        GH_ParamAccess.tree);
      pManager.AddGenericParameter("Rotations YY [rad]",
        "Ryy",
        "Rotations around Y-axis in Global Axiss." + note,
        GH_ParamAccess.tree);
      pManager.AddGenericParameter("Rotations ZZ [rad]",
        "Rzz",
        "Rotations around Z-axis in Global Axis." + note,
        GH_ParamAccess.tree);
      pManager.AddGenericParameter("Rotations |XYZ| [rad]",
        "|R|",
        "Combined |XXYYZZ| Rotations in Global Axis." + note,
        GH_ParamAccess.tree);
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      var result = new GsaResult();

      string elementlist = "All";
      var ghType = new GH_String();
      if (da.GetData(1, ref ghType)) {
        GH_Convert.ToString(ghType, out elementlist, GH_Conversion.Both);
      }

      if (elementlist.ToLower() == "all" || elementlist == "") {
        elementlist = "All";
      }

      var ghDivisions = new GH_Integer();
      da.GetData(2, ref ghDivisions);
      GH_Convert.ToInt32(ghDivisions, out int positionsCount, GH_Conversion.Both);
      positionsCount = Math.Abs(positionsCount) + 2; // taken absolute value and add 2 end points.

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
        if (ghTyp?.Value == null) {
          this.AddRuntimeWarning("Input is null");
          return;
        }

        if (ghTyp.Value is GsaResultGoo goo) {
          result = goo.Value;
        }
        else {
          this.AddRuntimeError("Error converting input to GSA Result");
          return;
        }

        List<GsaResultsValues> vals
          = result.Element1DDisplacementValues(elementlist, positionsCount, _lengthUnit);

        List<int> permutations = result.SelectedPermutationIds is null
          ? new List<int>() {
            1,
          }
          : result.SelectedPermutationIds;
        if (permutations.Count == 1 && permutations[0] == -1) {
          permutations = Enumerable.Range(1, vals.Count)
            .ToList();
        }

        // loop through all permutations (analysis case will just have one)
        foreach (int perm in permutations) {
          if (vals[perm - 1]
              .XyzResults.Count
            == 0
            & vals[perm - 1]
              .XxyyzzResults.Count
            == 0) {
            string acase = result.ToString()
              .Replace('}', ' ')
              .Replace('{', ' ');
            this.AddRuntimeWarning("Case " + acase + " contains no Element1D results.");
            continue;
          }

          Parallel.For(0,
            2,
            thread => // split computation in two for xyz and xxyyzz
            {
              switch (thread) {
                case 0: {
                    foreach (KeyValuePair<int, ConcurrentDictionary<int, GsaResultQuantity>> kvp in
                      vals
                          [perm - 1]
                        .XyzResults) {
                      int elementId = kvp.Key;
                      ConcurrentDictionary<int, GsaResultQuantity> res = kvp.Value;
                      if (res.Count == 0) {
                        continue;
                      }

                      var path = new GH_Path(result.CaseId,
                        result.SelectedPermutationIds == null
                          ? 0
                          : perm,
                        elementId);

                      outTransX.AddRange(
                        res.Select(x => new GH_UnitNumber(x.Value.X.ToUnit(_lengthUnit))),
                        path); // use ToUnit to capture changes in dropdown
                      outTransY.AddRange(
                        res.Select(x => new GH_UnitNumber(x.Value.Y.ToUnit(_lengthUnit))),
                        path);
                      outTransZ.AddRange(
                        res.Select(x => new GH_UnitNumber(x.Value.Z.ToUnit(_lengthUnit))),
                        path);
                      outTransXyz.AddRange(res.Select(x
                          => new GH_UnitNumber(x.Value.Xyz.ToUnit(_lengthUnit))),
                        path);
                    }

                    break;
                  }
                case 1: {
                    foreach (KeyValuePair<int, ConcurrentDictionary<int, GsaResultQuantity>> kvp in
                      vals
                          [perm - 1]
                        .XxyyzzResults) {
                      int elementId = kvp.Key;
                      ConcurrentDictionary<int, GsaResultQuantity> res = kvp.Value;
                      if (res.Count == 0) {
                        continue;
                      }

                      var path = new GH_Path(result.CaseId,
                        result.SelectedPermutationIds == null
                          ? 0
                          : perm,
                        elementId);

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

      PostHog.Result(result.Type, 1, GsaResultsValues.ResultType.Displacement);
    }

    protected override void UpdateUIFromSelectedItems() {
      _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[0]);
      base.UpdateUIFromSelectedItems();
    }
  }
}
