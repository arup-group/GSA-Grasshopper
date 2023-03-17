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
  /// Component to GSA 2D Element displacement values
  /// </summary>
  public class Elem2dDisplacement : GH_OasysDropDownComponent {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("22f87d33-4f9a-49d6-9f3d-b366e446a75f");
    public override GH_Exposure Exposure => GH_Exposure.quinary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Properties.Resources.Displacement2D;

    public Elem2dDisplacement() : base("2D Displacements",
      "Disp2D",
      "2D Translation and Rotation result values",
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
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      string unitAbbreviation = Length.GetAbbreviation(_lengthUnit);

      string note = Environment.NewLine + "DataTree organised as { CaseID ; Permutation ; ElementID } " +
                    Environment.NewLine + "fx. {1;2;3} is Case 1, Permutation 2, Element 3, where each " +
          Environment.NewLine + "branch contains a list of results in the following order:" +
          Environment.NewLine + "Vertex(1), Vertex(2), ..., Vertex(i), Centre";

      pManager.AddGenericParameter("Translations X [" + unitAbbreviation + "]", "Ux", "Translations in X-direction in Global Axis." + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Translations Y [" + unitAbbreviation + "]", "Uy", "Translations in Y-direction in Global Axis." + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Translations Z [" + unitAbbreviation + "]", "Uz", "Translations in Z-direction in Global Axis." + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Translations |XYZ| [" + unitAbbreviation + "]", "|U|", "Combined |XYZ| Translations in Global Axis." + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Rotations XX [rad]", "Rxx", "Rotations around X-axis in Global Axis." + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Rotations YY [rad]", "Ryy", "Rotations around Y-axis in Global Axiss." + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Rotations ZZ [rad]", "Rzz", "Rotations around Z-axis in Global Axis." + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Rotations |XYZ| [rad]", "|R|", "Combined |XXYYZZ| Rotations in Global Axis." + note, GH_ParamAccess.tree);
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

        List<GsaResultsValues> vals = result.Element2DDisplacementValues(elementlist, _lengthUnit);

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

                  outTransX.AddRange(res.Select(x => new GH_UnitNumber(x.Value.X.ToUnit(_lengthUnit))), path); // use ToUnit to capture changes in dropdown
                  outTransY.AddRange(res.Select(x => new GH_UnitNumber(x.Value.Y.ToUnit(_lengthUnit))), path);
                  outTransZ.AddRange(res.Select(x => new GH_UnitNumber(x.Value.Z.ToUnit(_lengthUnit))), path);
                  outTransXyz.AddRange(res.Select(x => new GH_UnitNumber(x.Value.XYZ.ToUnit(_lengthUnit))), path);
                }

                break;
              }
              case 1: {
                foreach (KeyValuePair<int, ConcurrentDictionary<int, GsaResultQuantity>> kvp in vals[perm - 1].xxyyzzResults) {
                  int elementId = kvp.Key;
                  ConcurrentDictionary<int, GsaResultQuantity> res = kvp.Value;

                  var path = new GH_Path(
                    result.CaseId,
                    result.SelectedPermutationIds == null
                      ? 0
                      : perm,
                    elementId);
                  if (res.Count == 0) { continue; }

                  outRotX.AddRange(res.Select(x => new GH_UnitNumber(x.Value.X)), path); // always use [rad] units
                  outRotY.AddRange(res.Select(x => new GH_UnitNumber(x.Value.Y)), path);
                  outRotZ.AddRange(res.Select(x => new GH_UnitNumber(x.Value.Z)), path);
                  outRotXyz.AddRange(res.Select(x => new GH_UnitNumber(x.Value.XYZ)), path);
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

      Helpers.PostHog.Result(result.Type, 2, GsaResultsValues.ResultType.Displacement);
    }

    #region Custom UI
    private LengthUnit _lengthUnit = DefaultUnits.LengthUnitResult;
    public override void InitialiseDropdowns() {
      SpacerDescriptions = new List<string>(new []
        {
          "Unit",
        });

      DropDownItems = new List<List<string>>();
      SelectedItems = new List<string>();

      // Length
      DropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
      SelectedItems.Add(Length.GetAbbreviation(_lengthUnit));

      IsInitialised = true;
    }

    public override void SetSelected(int i, int j) {
      SelectedItems[i] = DropDownItems[i][j];
      _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), SelectedItems[i]);
      base.UpdateUI();
    }
    public override void UpdateUIFromSelectedItems() {
      _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), SelectedItems[0]);
      base.UpdateUIFromSelectedItems();
    }

    public override void VariableParameterMaintenance() {
      string unitAbbreviation = Length.GetAbbreviation(_lengthUnit);
      int i = 0;
      Params.Output[i++].Name = "Translations X [" + unitAbbreviation + "]";
      Params.Output[i++].Name = "Translations Y [" + unitAbbreviation + "]";
      Params.Output[i++].Name = "Translations Z [" + unitAbbreviation + "]";
      Params.Output[i].Name = "Translations |XYZ| [" + unitAbbreviation + "]";
    }
    #endregion
  }
}

