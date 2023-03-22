﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Parameters;
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
  public class Elem2dForces : GH_OasysDropDownComponent {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("ea42e671-710e-4fd3-a113-1724049159cf");
    public override GH_Exposure Exposure => GH_Exposure.quinary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Properties.Resources.Forces2D;

    public Elem2dForces() : base("2D Forces and Moments",
      "Forces2D",
      "2D Projected Force and Moment result values",
      CategoryName.Name(),
      SubCategoryName.Cat5()) {
      Hidden = true;
    }
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
      string forceunitAbbreviation = ForcePerLength.GetAbbreviation(_forceUnit);
      string momentunitAbbreviation = Force.GetAbbreviation(_momentUnit);

      string forcerule = Environment.NewLine + "+ve in plane force resultant: tensile";
      string momentrule = Environment.NewLine + "+ve moments correspond to +ve stress on the top (eg. Mx +ve if top Sxx +ve)";
      string note = Environment.NewLine + "DataTree organised as { CaseID ; Permutation ; ElementID } " +
                    Environment.NewLine + "fx. {1;2;3} is Case 1, Permutation 2, Element 3, where each " +
                    Environment.NewLine + "branch contains a list of results in the following order: " +
                    Environment.NewLine + "Vertex(1), Vertex(2), ..., Vertex(i), Centre" +
                    Environment.NewLine + "Element results are NOT averaged at nodes";

      pManager.AddGenericParameter("Force X [" + forceunitAbbreviation + "]", "Nx", "Element in-plane Forces in Local X-direction." + forcerule + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Force Y [" + forceunitAbbreviation + "]", "Ny", "Element in-plane Forces in Local Y-direction." + forcerule + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Force XY [" + forceunitAbbreviation + "]", "Nxy", "Element in-plane Forces in Local XY-direction." + forcerule + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Shear X [" + forceunitAbbreviation + "]", "Qx", "Element through thickness Shears in Local XZ-plane." + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Shear Y [" + forceunitAbbreviation + "]", "Qz", "Element through thickness Shears in Local YZ-plane." + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Moment X [" + momentunitAbbreviation + "]", "Mx", "Element Moments around Local Element X-axis." + momentrule + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Moment Y [" + momentunitAbbreviation + "]", "My", "Element Moments around Local Element Y-axis." + momentrule + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Moment XY [" + momentunitAbbreviation + "]", "Mxy", "Element Moments around Local Element XY-axis." + momentrule + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Wood-Armer X [" + momentunitAbbreviation + "]", "M*x", "Element Wood-Armer Moments (Mx + sgn(Mx)·|Mxy|) around Local Element X-axis." + momentrule + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Wood-Armer Y [" + momentunitAbbreviation + "]", "M*y", "Element Wood-Armer Moments (My + sgn(My)·|Mxy|) around Local Element Y-axis." + momentrule + note, GH_ParamAccess.tree);
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

        List<GsaResultsValues> vals = result.Element2DForceValues(elementlist, _forceUnit, _momentUnit);
        List<GsaResultsValues> valsShear = result.Element2DShearValues(elementlist, _forceUnit);

        List<int> permutations = result.SelectedPermutationIds ?? new List<int>() { 1 };
        if (permutations.Count == 1 && permutations[0] == -1)
          permutations = Enumerable.Range(1, vals.Count).ToList();

        foreach (int perm in permutations) {
          if (vals[perm - 1].xyzResults.Count == 0 & vals[perm - 1].xxyyzzResults.Count == 0) {
            string acase = result.ToString().Replace('}', ' ').Replace('{', ' ');
            this.AddRuntimeWarning("Case " + acase + " contains no Element2D results.");
            continue;
          }
          Parallel.For(0, 3, thread => // split computation in three for xyz and xxyyzz and shear
          {
            switch (thread) {
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

                    outX.AddRange(res.Select(x => new GH_UnitNumber(x.Value.X.ToUnit(_forceUnit))), path); // use ToUnit to capture changes in dropdown
                    outY.AddRange(res.Select(x => new GH_UnitNumber(x.Value.Y.ToUnit(_forceUnit))), path);
                    outXy.AddRange(res.Select(x => new GH_UnitNumber(x.Value.Z.ToUnit(_forceUnit))), path);
                    // Wood-Armer moment M*x is stored in .XYZ
                    outWaxx.AddRange(res.Select(x => new GH_UnitNumber(x.Value.XYZ.ToUnit(_momentUnit))), path);
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

                    outXx.AddRange(res.Select(x => new GH_UnitNumber(x.Value.X.ToUnit(_momentUnit))), path); // always use [rad] units
                    outYy.AddRange(res.Select(x => new GH_UnitNumber(x.Value.Y.ToUnit(_momentUnit))), path);
                    outXxyy.AddRange(res.Select(x => new GH_UnitNumber(x.Value.Z.ToUnit(_momentUnit))), path);
                    // Wood-Armer moment M*y 
                    outWayy.AddRange(res.Select(x => new GH_UnitNumber(x.Value.XYZ.ToUnit(_momentUnit))), path);
                  }
                  break;
                }
              case 2: {
                  foreach (KeyValuePair<int, ConcurrentDictionary<int, GsaResultQuantity>> kvp in valsShear[perm - 1].xyzResults) {
                    int elementId = kvp.Key;
                    ConcurrentDictionary<int, GsaResultQuantity> res = kvp.Value;

                    var path = new GH_Path(
                      result.CaseId,
                      result.SelectedPermutationIds == null
                        ? 0
                        : perm,
                      elementId);

                    outQx.AddRange(res.Select(x => new GH_UnitNumber(x.Value.X.ToUnit(_forceUnit))), path); // always use [rad] units
                    outQy.AddRange(res.Select(x => new GH_UnitNumber(x.Value.Y.ToUnit(_forceUnit))), path);
                  }

                  break;
                }
            }
          });
        }
      }

      da.SetDataTree(0, outX);
      da.SetDataTree(1, outY);
      da.SetDataTree(2, outXy);
      da.SetDataTree(3, outQx);
      da.SetDataTree(4, outQy);
      da.SetDataTree(5, outXx);
      da.SetDataTree(6, outYy);
      da.SetDataTree(7, outXxyy);

      Helpers.PostHog.Result(result.Type, 2, GsaResultsValues.ResultType.Force);
      da.SetDataTree(8, outWaxx);
      da.SetDataTree(9, outWayy);
    }

    #region Custom UI
    private ForcePerLengthUnit _forceUnit = DefaultUnits.ForcePerLengthUnit;
    private ForceUnit _momentUnit = DefaultUnits.ForceUnit;
    public override void InitialiseDropdowns() {
      SpacerDescriptions = new List<string>(new[]
        {
          "Force Unit",
          "Moment Unit",
        });

      DropDownItems = new List<List<string>>();
      SelectedItems = new List<string>();

      DropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.ForcePerLength));
      SelectedItems.Add(ForcePerLength.GetAbbreviation(_forceUnit));

      DropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Force));
      SelectedItems.Add(Force.GetAbbreviation(_momentUnit));

      IsInitialised = true;
    }

    public override void SetSelected(int i, int j) {
      SelectedItems[i] = DropDownItems[i][j];
      switch (i) {
        case 0:
          _forceUnit = (ForcePerLengthUnit)UnitsHelper.Parse(typeof(ForcePerLengthUnit), SelectedItems[i]);
          break;
        case 1:
          _momentUnit = (ForceUnit)UnitsHelper.Parse(typeof(ForceUnit), SelectedItems[i]);
          break;
      }
      base.UpdateUI();
    }
    public override void UpdateUIFromSelectedItems() {
      _forceUnit = (ForcePerLengthUnit)UnitsHelper.Parse(typeof(ForcePerLengthUnit), SelectedItems[0]);
      _momentUnit = (ForceUnit)UnitsHelper.Parse(typeof(ForceUnit), SelectedItems[1]);
      base.UpdateUIFromSelectedItems();
    }

    public override void VariableParameterMaintenance() {
      string forceunitAbbreviation = ForcePerLength.GetAbbreviation(_forceUnit);
      string momentunitAbbreviation = Force.GetAbbreviation(_momentUnit);

      if (Params.Output.Count != 10) {
        Params.RegisterOutputParam(new Param_GenericObject());
        string momentrule = Environment.NewLine + "+ve moments correspond to +ve stress on the top (eg. Mx +ve if top Sxx +ve)";
        string note = Environment.NewLine + "DataTree organised as { CaseID ; Permutation ; ElementID } " +
                      Environment.NewLine + "fx. {1;2;3} is Case 1, Permutation 2, Element 3, where each " +
                      Environment.NewLine + "branch contains a list of results in the following order: " +
                      Environment.NewLine + "Vertex(1), Vertex(2), ..., Vertex(i), Centre" +
                      Environment.NewLine + "Element results are NOT averaged at nodes";
        Params.Output[8].NickName = "M*x";
        Params.Output[8].Description = "Element Wood-Armer Moments (Mx + sgn(Mx)·|Mxy|) around Local Element X-axis." + momentrule + note;
        Params.Output[8].Access = GH_ParamAccess.tree;

        Params.RegisterOutputParam(new Param_GenericObject());
        Params.Output[9].NickName = "M*y";
        Params.Output[9].Description = "Element Wood-Armer Moments (My + sgn(My)·|Mxy|) around Local Element Y-axis." + momentrule + note;
        Params.Output[9].Access = GH_ParamAccess.tree;

        Params.Output[6].Description = "Element Moments around Local Element Y-axis." + momentrule + note;
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
    #endregion
  }
}

