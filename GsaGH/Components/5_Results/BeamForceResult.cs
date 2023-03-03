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

namespace GsaGH.Components
{
    /// <summary>
    /// Component to get GSA beam force values
    /// </summary>
    public class BeamForces : GH_OasysDropDownComponent
  {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("5dee1b78-7b47-4c65-9d17-446140fc4e0d");
    public override GH_Exposure Exposure => GH_Exposure.quarternary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.BeamForces;

    public BeamForces() : base("Beam Forces and Moments",
      "BeamForces",
      "Element1D Force and Moment result values",
      CategoryName.Name(),
      SubCategoryName.Cat5())
    { this.Hidden = true; } // sets the initial state of the component to hidden
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddParameter(new GsaResultsParameter(), "Result", "Res", "GSA Result", GH_ParamAccess.list);
      pManager.AddTextParameter("Element filter list", "El", "Filter results by list." + Environment.NewLine +
          "Element list should take the form:" + Environment.NewLine +
          " 1 11 to 20 step 2 P1 not (G1 to G6 step 3) P11 not (PA PB1 PS2 PM3 PA4 M1)" + Environment.NewLine +
          "Refer to GSA help file for definition of lists and full vocabulary.", GH_ParamAccess.item, "All");
      pManager.AddIntegerParameter("Intermediate Points", "nP", "Number of intermediate equidistant points (default 3)", GH_ParamAccess.item, 3);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      string forceunitAbbreviation = Force.GetAbbreviation(this.ForceUnit);
      string momentunitAbbreviation = Moment.GetAbbreviation(this.MomentUnit);

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

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      // Result to work on
      GsaResult result = new GsaResult();

      // Get filer case
      string elementlist = "All";
      GH_String gh_Type = new GH_String();
      if (DA.GetData(1, ref gh_Type))
        GH_Convert.ToString(gh_Type, out elementlist, GH_Conversion.Both);

      if (elementlist.ToLower() == "all" || elementlist == "")
        elementlist = "All";

      // Get number of divisions
      GH_Integer gh_Div = new GH_Integer();
      DA.GetData(2, ref gh_Div);
      GH_Convert.ToInt32(gh_Div, out int positionsCount, GH_Conversion.Both);
      positionsCount = Math.Abs(positionsCount) + 2; // taken absolute value and add 2 end points.

      // data trees to output
      DataTree<GH_UnitNumber> out_transX = new DataTree<GH_UnitNumber>();
      DataTree<GH_UnitNumber> out_transY = new DataTree<GH_UnitNumber>();
      DataTree<GH_UnitNumber> out_transZ = new DataTree<GH_UnitNumber>();
      DataTree<GH_UnitNumber> out_transXYZ = new DataTree<GH_UnitNumber>();
      DataTree<GH_UnitNumber> out_rotX = new DataTree<GH_UnitNumber>();
      DataTree<GH_UnitNumber> out_rotY = new DataTree<GH_UnitNumber>();
      DataTree<GH_UnitNumber> out_rotZ = new DataTree<GH_UnitNumber>();
      DataTree<GH_UnitNumber> out_rotXYZ = new DataTree<GH_UnitNumber>();

      // Get Model
      List<GH_ObjectWrapper> gh_types = new List<GH_ObjectWrapper>();
      if (DA.GetDataList(0, gh_types))
      {
        List<GsaResult> results = new List<GsaResult>();

        for (int i = 0; i < gh_types.Count; i++) // loop through all case/combinations
        {
          GH_ObjectWrapper gh_typ = gh_types[i];
          if (gh_typ == null || gh_typ.Value == null)
          {
            this.AddRuntimeWarning("Input is null");
            return;
          }
          if (gh_typ.Value is GsaResultGoo)
          {
            result = ((GsaResultGoo)gh_typ.Value).Value;
          }
          else
          {
            this.AddRuntimeError("Error converting input to GSA Result");
            return;
          }

          List<GsaResultsValues> vals = result.Element1DForceValues(elementlist, positionsCount, this.ForceUnit, this.MomentUnit);

          List<int> permutations = (result.SelectedPermutationIDs == null ? new List<int>() { 1 } : result.SelectedPermutationIDs);
          if (permutations.Count == 1 && permutations[0] == -1)
            permutations = Enumerable.Range(1, vals.Count).ToList();

          // loop through all permutations (analysis case will just have one)
          foreach (int perm in permutations)
          {
            if (vals[perm - 1].xyzResults.Count == 0 & vals[perm - 1].xxyyzzResults.Count == 0)
            {
              string acase = result.ToString().Replace('}', ' ').Replace('{', ' ');
              this.AddRuntimeWarning("Case " + acase + " contains no Element1D results.");
              continue;
            }
            Parallel.For(0, 2, thread => // split computation in two for xyz and xxyyzz
            {
              if (thread == 0)
              {
                //do xyz part of results
                // loop through all elements
                foreach (KeyValuePair<int, ConcurrentDictionary<int, GsaResultQuantity>> kvp in vals[perm - 1].xyzResults)
                {
                  int elementID = kvp.Key;
                  ConcurrentDictionary<int, GsaResultQuantity> res = kvp.Value;
                  if (res.Count == 0) { continue; }

                  GH_Path path = new GH_Path(result.CaseId, result.SelectedPermutationIDs == null ? 0 : perm, elementID);

                  out_transX.AddRange(res.Select(x => new GH_UnitNumber(x.Value.X.ToUnit(this.ForceUnit))), path); // use ToUnit to capture changes in dropdown
                  out_transY.AddRange(res.Select(x => new GH_UnitNumber(x.Value.Y.ToUnit(this.ForceUnit))), path);
                  out_transZ.AddRange(res.Select(x => new GH_UnitNumber(x.Value.Z.ToUnit(this.ForceUnit))), path);
                  out_transXYZ.AddRange(res.Select(x => new GH_UnitNumber(x.Value.XYZ.ToUnit(this.ForceUnit))), path);
                }
              }
              if (thread == 1)
              {
                //do xxyyzz
                // loop through all elements
                foreach (KeyValuePair<int, ConcurrentDictionary<int, GsaResultQuantity>> kvp in vals[perm - 1].xxyyzzResults)
                {
                  int elementID = kvp.Key;
                  ConcurrentDictionary<int, GsaResultQuantity> res = kvp.Value;
                  if (res.Count == 0) { continue; }

                  GH_Path path = new GH_Path(result.CaseId, result.SelectedPermutationIDs == null ? 0 : perm, elementID);

                  out_rotX.AddRange(res.Select(x => new GH_UnitNumber(x.Value.X.ToUnit(this.MomentUnit))), path); // always use [rad] units
                  out_rotY.AddRange(res.Select(x => new GH_UnitNumber(x.Value.Y.ToUnit(this.MomentUnit))), path);
                  out_rotZ.AddRange(res.Select(x => new GH_UnitNumber(x.Value.Z.ToUnit(this.MomentUnit))), path);
                  out_rotXYZ.AddRange(res.Select(x => new GH_UnitNumber(x.Value.XYZ.ToUnit(this.MomentUnit))), path);
                }
              }
            });
          }
        }

        DA.SetDataTree(0, out_transX);
        DA.SetDataTree(1, out_transY);
        DA.SetDataTree(2, out_transZ);
        DA.SetDataTree(3, out_transXYZ);
        DA.SetDataTree(4, out_rotX);
        DA.SetDataTree(5, out_rotY);
        DA.SetDataTree(6, out_rotZ);
        DA.SetDataTree(7, out_rotXYZ);

        Helpers.PostHog.Result(result.Type, 1, GsaResultsValues.ResultType.Force);
      }
    }

    #region Custom UI
    private ForceUnit ForceUnit = DefaultUnits.ForceUnit;
    private MomentUnit MomentUnit = DefaultUnits.MomentUnit;
    public override void InitialiseDropdowns()
    {
      this.SpacerDescriptions = new List<string>(new string[]
        {
          "Force Unit", "Moment Unit"
        });

      this.DropDownItems = new List<List<string>>();
      this.SelectedItems = new List<string>();

      // force
      this.DropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Force));
      this.SelectedItems.Add(Force.GetAbbreviation(this.ForceUnit));

      // moment
      this.DropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Moment));
      this.SelectedItems.Add(Moment.GetAbbreviation(this.MomentUnit));

      this.IsInitialised = true;
    }

    public override void SetSelected(int i, int j)
    {
      this.SelectedItems[i] = this.DropDownItems[i][j];
      if (i == 0)
        this.ForceUnit = (ForceUnit)UnitsHelper.Parse(typeof(ForceUnit), this.SelectedItems[i]);
      else if (i == 1)
        this.MomentUnit = (MomentUnit)UnitsHelper.Parse(typeof(MomentUnit), this.SelectedItems[i]);
      base.UpdateUI();
    }
    public override void UpdateUIFromSelectedItems()
    {
      this.ForceUnit = (ForceUnit)UnitsHelper.Parse(typeof(ForceUnit), this.SelectedItems[0]);
      this.MomentUnit = (MomentUnit)UnitsHelper.Parse(typeof(MomentUnit), this.SelectedItems[1]);
      base.UpdateUIFromSelectedItems();
    }

    public override void VariableParameterMaintenance()
    {
      string forceunitAbbreviation = Force.GetAbbreviation(this.ForceUnit);
      string momentunitAbbreviation = Moment.GetAbbreviation(this.MomentUnit);
      int i = 0;
      Params.Output[i++].Name = "Force X [" + forceunitAbbreviation + "]";
      Params.Output[i++].Name = "Force Y [" + forceunitAbbreviation + "]";
      Params.Output[i++].Name = "Force Z [" + forceunitAbbreviation + "]";
      Params.Output[i++].Name = "Force |XYZ| [" + forceunitAbbreviation + "]";
      Params.Output[i++].Name = "Moment XX [" + momentunitAbbreviation + "]";
      Params.Output[i++].Name = "Moment YY [" + momentunitAbbreviation + "]";
      Params.Output[i++].Name = "Moment ZZ [" + momentunitAbbreviation + "]";
      Params.Output[i++].Name = "Moment |XXYYZZ| [" + momentunitAbbreviation + "]";
    }
    #endregion
  }
}

