using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Grasshopper.Kernel;
using Rhino.Geometry;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GsaGH.Parameters;
using UnitsNet.Units;
using UnitsNet;
using System.Linq;
using Oasys.Units;
using GsaGH.Util.GH;
using GsaGH.Util.Gsa;
using UnitsNet.GH;
using Grasshopper;
using Grasshopper.Kernel.Data;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace GsaGH.Components
{
  /// <summary>
  /// Component to retrieve non-geometric objects from a GSA model
  /// </summary>
  public class Elem2dForces : GH_Component, IGH_VariableParameterComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("ea42e671-710e-4fd3-a113-1724049159cf");
    public Elem2dForces()
      : base("2D Forces and Moments", "Forces2D", "2D Projected Force and Moment result values",
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat5())
    { this.Hidden = true; } // sets the initial state of the component to hidden
    public override GH_Exposure Exposure => GH_Exposure.quinary;

    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.Forces2D;
    #endregion

    #region Custom UI
    //This region overrides the typical component layout
    public override void CreateAttributes()
    {
      if (first)
      {
        dropdownitems = new List<List<string>>();
        selecteditems = new List<string>();

        // force
        dropdownitems.Add(Units.FilteredForcePerLengthUnits);
        selecteditems.Add(forceUnit.ToString());

        // moment
        dropdownitems.Add(Units.FilteredForceUnits);
        selecteditems.Add(momentUnit.ToString());


        first = false;
      }
      m_attributes = new UI.MultiDropDownComponentUI(this, SetSelected, dropdownitems, selecteditems, spacerDescriptions);
    }
    public void SetSelected(int i, int j)
    {
      // change selected item
      selecteditems[i] = dropdownitems[i][j];
      if (i == 0)
        forceUnit = (ForcePerLengthUnit)Enum.Parse(typeof(ForcePerLengthUnit), selecteditems[i]);
      else if (i == 1)
        momentUnit = (ForceUnit)Enum.Parse(typeof(ForceUnit), selecteditems[i]);

      // update name of inputs (to display unit on sliders)
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
      Params.OnParametersChanged();
      this.OnDisplayExpired(true);
    }
    private void UpdateUIFromSelectedItems()
    {
      forceUnit = (ForcePerLengthUnit)Enum.Parse(typeof(ForcePerLengthUnit), selecteditems[0]);
      momentUnit = (ForceUnit)Enum.Parse(typeof(ForceUnit), selecteditems[1]);

      CreateAttributes();
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
      Params.OnParametersChanged();
      this.OnDisplayExpired(true);
    }
    // list of lists with all dropdown lists conctent
    List<List<string>> dropdownitems;
    // list of selected items
    List<string> selecteditems;
    // list of descriptions 
    List<string> spacerDescriptions = new List<string>(new string[]
    {
            "Force Unit",
            "Moment / length Unit"
    });
    private bool first = true;
    private ForcePerLengthUnit forceUnit = Units.ForcePerLengthUnit;
    private ForceUnit momentUnit = Units.ForceUnit;
    string forceunitAbbreviation;
    string momentunitAbbreviation;
    #endregion

    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddGenericParameter("Result", "Res", "GSA Result", GH_ParamAccess.list);
      pManager.AddTextParameter("Element filter list", "El", "Filter results by list." + System.Environment.NewLine +
          "Element list should take the form:" + System.Environment.NewLine +
          " 1 11 to 20 step 2 P1 not (G1 to G6 step 3) P11 not (PA PB1 PS2 PM3 PA4 M1)" + System.Environment.NewLine +
          "Refer to GSA help file for definition of lists and full vocabulary.", GH_ParamAccess.item, "All");
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      IQuantity force = new ForcePerLength(0, forceUnit);
      forceunitAbbreviation = string.Concat(force.ToString().Where(char.IsLetter));
      IQuantity moment = new Force(0, momentUnit);
      momentunitAbbreviation = string.Concat(moment.ToString().Where(char.IsLetter));

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


    protected override void SolveInstance(IGH_DataAccess DA)
    {
      // Result to work on
      GsaResult result = new GsaResult();

      // Get filer case
      string elementlist = "All";
      GH_String gh_Type = new GH_String();
      if (DA.GetData(1, ref gh_Type))
        GH_Convert.ToString(gh_Type, out elementlist, GH_Conversion.Both);

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
          if (gh_typ.Value is GsaResultGoo)
          {
            result = ((GsaResultGoo)gh_typ.Value).Value;
          }
          else
          {
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Error converting input to GSA Result");
            return;
          }

          List<GsaResultsValues> vals = result.Element2DForceValues(elementlist, forceUnit, momentUnit);
          List<GsaResultsValues> valsShear = result.Element2DShearValues(elementlist, forceUnit);

          // loop through all permutations (analysis case will just have one)
          for (int permutation = 0; permutation < vals.Count; permutation++)
          {
            if (vals[permutation].xyzResults.Count == 0 & vals[permutation].xxyyzzResults.Count == 0)
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
                foreach (KeyValuePair<int, ConcurrentDictionary<int, GsaResultQuantity>> kvp in vals[permutation].xyzResults)
                {
                  int elementID = kvp.Key;
                  ConcurrentDictionary<int, GsaResultQuantity> res = kvp.Value;

                  GH_Path p = new GH_Path(result.CaseID, permutation + 1, elementID);

                  out_X.AddRange(kvp.Value.Select(x => new GH_UnitNumber(x.Value.X.ToUnit(forceUnit))), p); // use ToUnit to capture changes in dropdown
                  out_Y.AddRange(kvp.Value.Select(x => new GH_UnitNumber(x.Value.Y.ToUnit(forceUnit))), p);
                  out_XY.AddRange(kvp.Value.Select(x => new GH_UnitNumber(x.Value.Z.ToUnit(forceUnit))), p);
                }
              }
              if (thread == 1)
              {
                            //do xxyyzz

                            // loop through all elements
                foreach (KeyValuePair<int, ConcurrentDictionary<int, GsaResultQuantity>> kvp in vals[permutation].xxyyzzResults)
                {
                  int elementID = kvp.Key;
                  ConcurrentDictionary<int, GsaResultQuantity> res = kvp.Value;

                  GH_Path p = new GH_Path(result.CaseID, permutation + 1, elementID);

                  out_XX.AddRange(kvp.Value.Select(x => new GH_UnitNumber(x.Value.X.ToUnit(momentUnit))), p); // always use [rad] units
                  out_YY.AddRange(kvp.Value.Select(x => new GH_UnitNumber(x.Value.Y.ToUnit(momentUnit))), p);
                  out_XXYY.AddRange(kvp.Value.Select(x => new GH_UnitNumber(x.Value.Z.ToUnit(momentUnit))), p);
                }
              }
              if (thread == 2)
              {
                            //do shear

                            // loop through all elements
                foreach (KeyValuePair<int, ConcurrentDictionary<int, GsaResultQuantity>> kvp in valsShear[permutation].xyzResults)
                {
                  int elementID = kvp.Key;
                  ConcurrentDictionary<int, GsaResultQuantity> res = kvp.Value;

                  GH_Path p = new GH_Path(result.CaseID, permutation + 1, elementID);

                  out_qX.AddRange(kvp.Value.Select(x => new GH_UnitNumber(x.Value.X.ToUnit(forceUnit))), p); // always use [rad] units
                  out_qY.AddRange(kvp.Value.Select(x => new GH_UnitNumber(x.Value.Y.ToUnit(forceUnit))), p);
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
    #region (de)serialization
    public override bool Write(GH_IO.Serialization.GH_IWriter writer)
    {
      Util.GH.DeSerialization.writeDropDownComponents(ref writer, dropdownitems, selecteditems, spacerDescriptions);
      return base.Write(writer);
    }
    public override bool Read(GH_IO.Serialization.GH_IReader reader)
    {
      Util.GH.DeSerialization.readDropDownComponents(ref reader, ref dropdownitems, ref selecteditems, ref spacerDescriptions);

      first = false;
      UpdateUIFromSelectedItems();

      return base.Read(reader);
    }
    #endregion

    #region IGH_VariableParameterComponent null implementation
    bool IGH_VariableParameterComponent.CanInsertParameter(GH_ParameterSide side, int index)
    {
      return false;
    }
    bool IGH_VariableParameterComponent.CanRemoveParameter(GH_ParameterSide side, int index)
    {
      return false;
    }
    IGH_Param IGH_VariableParameterComponent.CreateParameter(GH_ParameterSide side, int index)
    {
      return null;
    }
    bool IGH_VariableParameterComponent.DestroyParameter(GH_ParameterSide side, int index)
    {
      return false;
    }
    void IGH_VariableParameterComponent.VariableParameterMaintenance()
    {
      IQuantity force = new ForcePerLength(0, forceUnit);
      forceunitAbbreviation = string.Concat(force.ToString().Where(char.IsLetter));
      IQuantity moment = new Force(0, momentUnit);
      momentunitAbbreviation = string.Concat(moment.ToString().Where(char.IsLetter));

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

