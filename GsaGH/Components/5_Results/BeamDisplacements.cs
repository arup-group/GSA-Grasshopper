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
  ///   Component to get GSA beam displacement values
  /// </summary>
  public class BeamDisplacements : GH_OasysDropDownComponent {
    public override Guid ComponentGuid => new Guid("1b7e99e8-c3c9-42c3-9474-792ddd17388d");
    public override GH_Exposure Exposure => GH_Exposure.tertiary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.BeamDisplacements;
    private LengthUnit _lengthUnit = DefaultUnits.LengthUnitResult;

    public BeamDisplacements() : base("Beam Displacements", "BeamDisp",
      "Element1D Translation and Rotation result values", CategoryName.Name(),
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
      pManager.AddParameter(new GsaElementMemberListParameter());
      pManager[1].Optional = true;
      pManager.AddIntegerParameter("Intermediate Points", "nP",
        "Number of intermediate equidistant points (default 3)", GH_ParamAccess.item, 3);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      string unitAbbreviation = Length.GetAbbreviation(_lengthUnit);

      string note = ResultNotes.Note1dResults;

      pManager.AddGenericParameter("Translations X [" + unitAbbreviation + "]", "Ux",
        "Translations in Local Element X-direction." + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Translations Y [" + unitAbbreviation + "]", "Uy",
        "Translations in Local Element Y-direction." + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Translations Z [" + unitAbbreviation + "]", "Uz",
        "Translations in Local Element Z-direction." + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Translations |XYZ| [" + unitAbbreviation + "]", "|U|",
        "Combined |XYZ| Translations." + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Rotations XX [rad]", "Rxx",
        "Rotations around Local Element X-axis." + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Rotations YY [rad]", "Ryy",
        "Rotations around Local Element Y-axis." + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Rotations ZZ [rad]", "Rzz",
        "Rotations around Local Element Z-axis." + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Rotations |XYZ| [rad]", "|R|",
        "Combined |XXYYZZ| Rotations." + note, GH_ParamAccess.tree);
    }

    protected override void SolveInternal(IGH_DataAccess da) {
      var result = new GsaResult();

      string elementlist = "All";

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
          elementlist = Inputs.GetElementListDefinition(this, da, 1, result.Model);
        } else {
          this.AddRuntimeError("Error converting input to GSA Result");
          return;
        }

        List<GsaResultValues> vals
          = result.Element1DDisplacementValues(elementlist, positionsCount, -1, _lengthUnit);

        List<int> permutations = result.SelectedPermutationIds is null ? new List<int>() {
          1,
        } : result.SelectedPermutationIds;
        if (permutations.Count == 1 && permutations[0] == -1) {
          permutations = Enumerable.Range(1, vals.Count).ToList();
        }

        // loop through all permutations (analysis case will just have one)
        foreach (int perm in permutations) {
          if (vals[perm - 1].XyzResults.Count == 0 & vals[perm - 1].XxyyzzResults.Count == 0) {
            string acase = result.ToString().Replace('}', ' ').Replace('{', ' ');
            this.AddRuntimeWarning("Case " + acase + " contains no Element1D results.");
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
                    if (res.Count == 0) {
                      continue;
                    }

                    var path = new GH_Path(result.CaseId,
                      result.SelectedPermutationIds == null ? 0 : perm, elementId);

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

      PostHog.Result(result.Type, 1, GsaResultValues.ResultType.Displacement);
    }

    protected override void UpdateUIFromSelectedItems() {
      _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[0]);
      base.UpdateUIFromSelectedItems();
    }
  }
}
