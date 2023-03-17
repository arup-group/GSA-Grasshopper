﻿using System;
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
  /// Component to get GSA beam force values
  /// </summary>
  public class BeamForces : GH_OasysDropDownComponent {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("5dee1b78-7b47-4c65-9d17-446140fc4e0d");
    public override GH_Exposure Exposure => GH_Exposure.quarternary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Properties.Resources.BeamForces;

    public BeamForces() : base("Beam Forces and Moments",
      "BeamForces",
      "Element1D Force and Moment result values",
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
      pManager.AddIntegerParameter("Intermediate Points", "nP", "Number of intermediate equidistant points (default 3)", GH_ParamAccess.item, 3);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      string forceunitAbbreviation = Force.GetAbbreviation(_forceUnit);
      string momentunitAbbreviation = Moment.GetAbbreviation(_momentUnit);

      string forcerule = Environment.NewLine + "+ve axial forces are tensile";
      string momentrule = Environment.NewLine + "Moments follow the right hand grip rule";
      string note = Environment.NewLine + "DataTree organised as { CaseID ; Permutation ; ElementID } " +
                    Environment.NewLine + "fx. {1;2;3} is Case 1, Permutation 2, Element 3, where each " +
          Environment.NewLine + "branch contains a list of results per element position.";

      pManager.AddGenericParameter("Force X [" + forceunitAbbreviation + "]", "Fx", "Element Axial Forces in Local Element X-direction." + forcerule + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Force Y [" + forceunitAbbreviation + "]", "Fy", "Element Shear Forces in Local Element Y-direction." + forcerule + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Force Z [" + forceunitAbbreviation + "]", "Fz", "Element Shear Forces in Local Element Z-direction." + forcerule + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Force |YZ| [" + forceunitAbbreviation + "]", "|Fyz|", "Total |YZ| Element Shear Forces." + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Moment XX [" + momentunitAbbreviation + "]", "Mxx", "Element Torsional Moments around Local Element X-axis." + momentrule + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Moment YY [" + momentunitAbbreviation + "]", "Myy", "Element Bending Moments around Local Element Y-axis." + momentrule + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Moment ZZ [" + momentunitAbbreviation + "]", "Mzz", "Element Bending Moments around Local Element Z-axis." + momentrule + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Moment |YZ| [" + momentunitAbbreviation + "]", "|Myz|", "Total |YYZZ| Element Bending Moments." + note, GH_ParamAccess.tree);
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

      var results = new List<GsaResult>();

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

        List<GsaResultsValues> vals = result.Element1DForceValues(elementlist, positionsCount, _forceUnit, _momentUnit);

        List<int> permutations = result.SelectedPermutationIds == null
          ? new List<int>() { 1 }
          : result.SelectedPermutationIds;
        if (permutations.Count == 1 && permutations[0] == -1)
          permutations = Enumerable.Range(1, vals.Count).ToList();

        foreach (int perm in permutations) {
          if (vals[perm - 1].xyzResults.Count == 0
              & vals[perm - 1].xxyyzzResults.Count == 0) {
            string acase = result.ToString().Replace('}', ' ').Replace('{', ' ');
            this.AddRuntimeWarning("Case " + acase + " contains no Element1D results.");
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
                      : perm, elementId);

                  outTransX.AddRange(res.Select(x => new GH_UnitNumber(x.Value.X.ToUnit(_forceUnit))), path); // use ToUnit to capture changes in dropdown
                  outTransY.AddRange(res.Select(x => new GH_UnitNumber(x.Value.Y.ToUnit(_forceUnit))), path);
                  outTransZ.AddRange(res.Select(x => new GH_UnitNumber(x.Value.Z.ToUnit(_forceUnit))), path);
                  outTransXyz.AddRange(res.Select(x => new GH_UnitNumber(x.Value.XYZ.ToUnit(_forceUnit))), path);
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
                      : perm, elementId);

                  outRotX.AddRange(res.Select(x => new GH_UnitNumber(x.Value.X.ToUnit(_momentUnit))), path); // always use [rad] units
                  outRotY.AddRange(res.Select(x => new GH_UnitNumber(x.Value.Y.ToUnit(_momentUnit))), path);
                  outRotZ.AddRange(res.Select(x => new GH_UnitNumber(x.Value.Z.ToUnit(_momentUnit))), path);
                  outRotXyz.AddRange(res.Select(x => new GH_UnitNumber(x.Value.XYZ.ToUnit(_momentUnit))), path);
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

      Helpers.PostHog.Result(result.Type, 1, GsaResultsValues.ResultType.Force);
    }

    #region Custom UI
    private ForceUnit _forceUnit = DefaultUnits.ForceUnit;
    private MomentUnit _momentUnit = DefaultUnits.MomentUnit;
    public override void InitialiseDropdowns() {
      SpacerDescriptions = new List<string>(new []
        {
          "Force Unit", "Moment Unit",
        });

      DropDownItems = new List<List<string>>();
      SelectedItems = new List<string>();

      // force
      DropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Force));
      SelectedItems.Add(Force.GetAbbreviation(_forceUnit));

      // moment
      DropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Moment));
      SelectedItems.Add(Moment.GetAbbreviation(_momentUnit));

      IsInitialised = true;
    }

    public override void SetSelected(int i, int j) {
      SelectedItems[i] = DropDownItems[i][j];
      switch (i)
      {
        case 0:
          _forceUnit = (ForceUnit)UnitsHelper.Parse(typeof(ForceUnit), SelectedItems[i]);
          break;
        case 1:
          _momentUnit = (MomentUnit)UnitsHelper.Parse(typeof(MomentUnit), SelectedItems[i]);
          break;
      }
      base.UpdateUI();
    }
    public override void UpdateUIFromSelectedItems() {
      _forceUnit = (ForceUnit)UnitsHelper.Parse(typeof(ForceUnit), SelectedItems[0]);
      _momentUnit = (MomentUnit)UnitsHelper.Parse(typeof(MomentUnit), SelectedItems[1]);
      base.UpdateUIFromSelectedItems();
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
    #endregion
  }
}

