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
    /// Component to get GSA 3D element displacement values
    /// </summary>
    public class Elem3dDisplacement : GH_OasysDropDownComponent
  {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("b24e0b5d-6376-43bf-9844-15443ce3b9dd");
    public override GH_Exposure Exposure => GH_Exposure.senary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.Displacement3D;

    public Elem3dDisplacement() : base("3D Displacements",
      "Disp3D",
      "3D Translation and Rotation result values",
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
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      string unitAbbreviation = Length.GetAbbreviation(this.LengthUnit);

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

          List<GsaResultsValues> vals = result.Element3DDisplacementValues(elementlist, LengthUnit);

          List<int> permutations = (result.SelectedPermutationIDs == null ? new List<int>() { 0 } : result.SelectedPermutationIDs);

          // loop through all permutations (analysis case will just have one)
          for (int index = 0; index < vals.Count; index++)
          {
            if (vals[index].xyzResults.Count == 0 & vals[index].xxyyzzResults.Count == 0)
            {
              string[] typ = result.ToString().Split('{');
              string acase = typ[1].Replace('}', ' ');
              AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Case " + acase + "contains no Element3D results.");
              continue;
            }
            Parallel.For(0, 2, thread => // split computation in two for xyz and xxyyzz
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

                  out_transX.AddRange(res.Select(x => new GH_UnitNumber(x.Value.X.ToUnit(LengthUnit))), p); // use ToUnit to capture changes in dropdown
                  out_transY.AddRange(res.Select(x => new GH_UnitNumber(x.Value.Y.ToUnit(LengthUnit))), p);
                  out_transZ.AddRange(res.Select(x => new GH_UnitNumber(x.Value.Z.ToUnit(LengthUnit))), p);
                  out_transXYZ.AddRange(res.Select(x => new GH_UnitNumber(x.Value.XYZ.ToUnit(LengthUnit))), p);
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

                  out_rotX.AddRange(res.Select(x => new GH_UnitNumber(x.Value.X)), p); // always use [rad] units
                  out_rotY.AddRange(res.Select(x => new GH_UnitNumber(x.Value.Y)), p);
                  out_rotZ.AddRange(res.Select(x => new GH_UnitNumber(x.Value.Z)), p);
                  out_rotXYZ.AddRange(res.Select(x => new GH_UnitNumber(x.Value.XYZ)), p);
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
      }
    }

    #region Custom UI
    private LengthUnit LengthUnit = DefaultUnits.LengthUnitResult;
    public override void InitialiseDropdowns()
    {
      this.SpacerDescriptions = new List<string>(new string[]
        {
          "Unit",
        });

      this.DropDownItems = new List<List<string>>();
      this.SelectedItems = new List<string>();

      // Length
      this.DropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
      this.SelectedItems.Add(Length.GetAbbreviation(this.LengthUnit));

      this.IsInitialised = true;
    }

    public override void SetSelected(int i, int j)
    {
      this.SelectedItems[i] = this.DropDownItems[i][j];
      this.LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), this.SelectedItems[i]);
      base.UpdateUI();
    }

    public override void UpdateUIFromSelectedItems()
    {
      this.LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), this.SelectedItems[0]);
      base.UpdateUIFromSelectedItems();
    }

    public override void VariableParameterMaintenance()
    {
      string unitAbbreviation = Length.GetAbbreviation(this.LengthUnit);
      int i = 0;
      Params.Output[i++].Name = "Translations X [" + unitAbbreviation + "]";
      Params.Output[i++].Name = "Translations Y [" + unitAbbreviation + "]";
      Params.Output[i++].Name = "Translations Z [" + unitAbbreviation + "]";
      Params.Output[i++].Name = "Translations |XYZ| [" + unitAbbreviation + "]";
    }
    #endregion
  }
}

