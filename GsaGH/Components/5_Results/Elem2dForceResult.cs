using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
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
  /// Component to retrieve non-geometric objects from a GSA model
  /// </summary>
  public class Elem2dForces : GH_OasysDropDownComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("ea42e671-710e-4fd3-a113-1724049159cf");
    public override GH_Exposure Exposure => GH_Exposure.quinary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.Forces2D;

    public Elem2dForces() : base("2D Forces and Moments",
      "Forces2D",
      "2D Projected Force and Moment result values",
      Ribbon.CategoryName.Name(),
      Ribbon.SubCategoryName.Cat5())
    { this.Hidden = true; } // sets the initial state of the component to hidden
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddParameter(new GsaResultsParameter(), "Result", "Res", "GSA Result", GH_ParamAccess.list);
      pManager.AddTextParameter("Element filter list", "El", "Filter results by list." + System.Environment.NewLine +
          "Element list should take the form:" + System.Environment.NewLine +
          " 1 11 to 20 step 2 P1 not (G1 to G6 step 3) P11 not (PA PB1 PS2 PM3 PA4 M1)" + System.Environment.NewLine +
          "Refer to GSA help file for definition of lists and full vocabulary.", GH_ParamAccess.item, "All");
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      string forceunitAbbreviation = ForcePerLength.GetAbbreviation(this.ForceUnit);
      string momentunitAbbreviation = Force.GetAbbreviation(this.MomentUnit);

      string forcerule = System.Environment.NewLine + "+ve in plane force resultant: tensile";
      string momentrule = System.Environment.NewLine + "+ve moments correspond to +ve stress on the top (eg. Mx +ve if top Sxx +ve)";
      string note = System.Environment.NewLine + "DataTree organised as { CaseID ; Permutation ; ElementID } " +
                    System.Environment.NewLine + "fx. {1;2;3} is Case 1, Permutation 2, Element 3, where each " +
                    System.Environment.NewLine + "branch contains a list of results in the following order: " +
                    System.Environment.NewLine + "Vertex(1), Vertex(2), ..., Vertex(i), Centre" +
                    System.Environment.NewLine + "Element results are NOT averaged at nodes";

      pManager.AddGenericParameter("Force X [" + forceunitAbbreviation + "]", "Nx", "Element in-plane Forces in Local X-direction." + forcerule + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Force Y [" + forceunitAbbreviation + "]", "Ny", "Element in-plane Forces in Local Y-direction." + forcerule + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Force XY [" + forceunitAbbreviation + "]", "Nxy", "Element in-plane Forces in Local XY-direction." + forcerule + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Shear X [" + forceunitAbbreviation + "]", "Qx", "Element through thickness Shears in Local XZ-plane." + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Shear Y [" + forceunitAbbreviation + "]", "Qz", "Element through thickness Shears in Local YZ-plane." + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Moment X [" + momentunitAbbreviation + "]", "Mx", "Element Moments around Local Element X-axis." + momentrule + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Moment Y [" + momentunitAbbreviation + "]", "My", "Element Moments around Local Element X-axis." + momentrule + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Moment XY [" + momentunitAbbreviation + "]", "Mxy", "Element Moments around Local Element XY-axis." + momentrule + note, GH_ParamAccess.tree);
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

      // data trees to output
      DataTree<GH_UnitNumber> out_X = new DataTree<GH_UnitNumber>();
      DataTree<GH_UnitNumber> out_Y = new DataTree<GH_UnitNumber>();
      DataTree<GH_UnitNumber> out_XY = new DataTree<GH_UnitNumber>();
      DataTree<GH_UnitNumber> out_qX = new DataTree<GH_UnitNumber>();
      DataTree<GH_UnitNumber> out_qY = new DataTree<GH_UnitNumber>();
      DataTree<GH_UnitNumber> out_XX = new DataTree<GH_UnitNumber>();
      DataTree<GH_UnitNumber> out_YY = new DataTree<GH_UnitNumber>();
      DataTree<GH_UnitNumber> out_XXYY = new DataTree<GH_UnitNumber>();

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
            AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Input is null");
            return;
          }
          if (gh_typ.Value is GsaResultGoo)
          {
            result = ((GsaResultGoo)gh_typ.Value).Value;
          }
          else
          {
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Error converting input to GSA Result");
            return;
          }

          List<GsaResultsValues> vals = result.Element2DForceValues(elementlist, ForceUnit, MomentUnit);
          List<GsaResultsValues> valsShear = result.Element2DShearValues(elementlist, ForceUnit);

          List<int> permutations = (result.SelectedPermutationIDs == null ? new List<int>() { 0 } : result.SelectedPermutationIDs);

          // loop through all permutations (analysis case will just have one)
          for (int index = 0; index < vals.Count; index++)
          {
            if (vals[index].xyzResults.Count == 0 & vals[index].xxyyzzResults.Count == 0)
            {
              string[] typ = result.ToString().Split('{');
              string acase = typ[1].Replace('}', ' ');
              AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Case " + acase + "contains no Element2D results.");
              continue;
            }
            Parallel.For(0, 3, thread => // split computation in three for xyz and xxyyzz and shear
            {
              if (thread == 0)
              {
                //do xyz part of results
                // loop through all elements
                foreach (KeyValuePair<int, ConcurrentDictionary<int, GsaResultQuantity>> kvp in vals[index].xyzResults)
                {
                  int elementID = kvp.Key;
                  ConcurrentDictionary<int, GsaResultQuantity> res = kvp.Value;
                  if (res.Count == 0) { continue; }

                  GH_Path p = new GH_Path(result.CaseID, permutations[index], elementID);

                  out_X.AddRange(res.Select(x => new GH_UnitNumber(x.Value.X.ToUnit(ForceUnit))), p); // use ToUnit to capture changes in dropdown
                  out_Y.AddRange(res.Select(x => new GH_UnitNumber(x.Value.Y.ToUnit(ForceUnit))), p);
                  out_XY.AddRange(res.Select(x => new GH_UnitNumber(x.Value.Z.ToUnit(ForceUnit))), p);
                }
              }
              if (thread == 1)
              {
                //do xxyyzz
                // loop through all elements
                foreach (KeyValuePair<int, ConcurrentDictionary<int, GsaResultQuantity>> kvp in vals[index].xxyyzzResults)
                {
                  int elementID = kvp.Key;
                  ConcurrentDictionary<int, GsaResultQuantity> res = kvp.Value;
                  if (res.Count == 0) { continue; }

                  GH_Path p = new GH_Path(result.CaseID, permutations[index], elementID);

                  out_XX.AddRange(res.Select(x => new GH_UnitNumber(x.Value.X.ToUnit(MomentUnit))), p); // always use [rad] units
                  out_YY.AddRange(res.Select(x => new GH_UnitNumber(x.Value.Y.ToUnit(MomentUnit))), p);
                  out_XXYY.AddRange(res.Select(x => new GH_UnitNumber(x.Value.Z.ToUnit(MomentUnit))), p);
                }
              }
              if (thread == 2)
              {
                //do shear

                // loop through all elements
                foreach (KeyValuePair<int, ConcurrentDictionary<int, GsaResultQuantity>> kvp in valsShear[index].xyzResults)
                {
                  int elementID = kvp.Key;
                  ConcurrentDictionary<int, GsaResultQuantity> res = kvp.Value;

                  GH_Path p = new GH_Path(result.CaseID, permutations[index], elementID);

                  out_qX.AddRange(res.Select(x => new GH_UnitNumber(x.Value.X.ToUnit(ForceUnit))), p); // always use [rad] units
                  out_qY.AddRange(res.Select(x => new GH_UnitNumber(x.Value.Y.ToUnit(ForceUnit))), p);
                }
              }
            });
          }
        }

        DA.SetDataTree(0, out_X);
        DA.SetDataTree(1, out_Y);
        DA.SetDataTree(2, out_XY);
        DA.SetDataTree(3, out_qX);
        DA.SetDataTree(4, out_qY);
        DA.SetDataTree(5, out_XX);
        DA.SetDataTree(6, out_YY);
        DA.SetDataTree(7, out_XXYY);
      }
    }

    #region Custom UI
    private ForcePerLengthUnit ForceUnit = DefaultUnits.ForcePerLengthUnit;
    private ForceUnit MomentUnit = DefaultUnits.ForceUnit;
    public override void InitialiseDropdowns()
    {
      this.SpacerDescriptions = new List<string>(new string[]
        {
          "Force Unit", "Moment Unit"
        });

      this.DropDownItems = new List<List<string>>();
      this.SelectedItems = new List<string>();

      // force
      this.DropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.ForcePerLength));
      this.SelectedItems.Add(ForcePerLength.GetAbbreviation(this.ForceUnit));

      // moment
      this.DropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Force));
      this.SelectedItems.Add(Force.GetAbbreviation(this.MomentUnit));

      this.IsInitialised = true;
    }

    public override void SetSelected(int i, int j)
    {
      this.SelectedItems[i] = this.DropDownItems[i][j];
      if (i == 0)
        this.ForceUnit = (ForcePerLengthUnit)Enum.Parse(typeof(ForcePerLengthUnit), this.SelectedItems[i]);
      else if (i == 1)
        this.MomentUnit = (ForceUnit)Enum.Parse(typeof(ForceUnit), this.SelectedItems[i]);
      base.UpdateUI();
    }
    public override void UpdateUIFromSelectedItems()
    {
      this.ForceUnit = (ForcePerLengthUnit)Enum.Parse(typeof(ForcePerLengthUnit), this.SelectedItems[0]);
      this.MomentUnit = (ForceUnit)Enum.Parse(typeof(ForceUnit), this.SelectedItems[1]);
      base.UpdateUIFromSelectedItems();
    }

    public override void VariableParameterMaintenance()
    {
      string forceunitAbbreviation = ForcePerLength.GetAbbreviation(this.ForceUnit);
      string momentunitAbbreviation = Force.GetAbbreviation(this.MomentUnit);
      int i = 0;
      Params.Output[i++].Name = "Force X [" + forceunitAbbreviation + "]";
      Params.Output[i++].Name = "Force Y [" + forceunitAbbreviation + "]";
      Params.Output[i++].Name = "Force Z [" + forceunitAbbreviation + "]";
      Params.Output[i++].Name = "Shear X [" + forceunitAbbreviation + "]";
      Params.Output[i++].Name = "Shear Y [" + forceunitAbbreviation + "]";
      Params.Output[i++].Name = "Moment X [" + momentunitAbbreviation + "]";
      Params.Output[i++].Name = "Moment Y [" + momentunitAbbreviation + "]";
      Params.Output[i++].Name = "Moment XY [" + momentunitAbbreviation + "]";
    }
    #endregion
  }
}

