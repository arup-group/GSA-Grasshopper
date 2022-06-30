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
  public class Elem2dStress : GH_Component, IGH_VariableParameterComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("b5eb8a78-e0dd-442b-bbd7-0384d6c944cb");
    public Elem2dStress()
      : base("2D Stresses", "Stress2D", "2D Projected Stress result values",
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat5())
    { this.Hidden = true; } // sets the initial state of the component to hidden
    public override GH_Exposure Exposure => GH_Exposure.quinary;

    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.Stress2D;
    #endregion

    #region Custom UI
    //This region overrides the typical component layout
    public override void CreateAttributes()
    {
      if (first)
      {
        dropdownitems = new List<List<string>>();
        selecteditems = new List<string>();

        // Stress
        dropdownitems.Add(Units.FilteredStressUnits);
        selecteditems.Add(stresshUnit.ToString());

        first = false;
      }
      m_attributes = new UI.MultiDropDownComponentUI(this, SetSelected, dropdownitems, selecteditems, spacerDescriptions);
    }
    public void SetSelected(int i, int j)
    {
      // change selected item
      selecteditems[i] = dropdownitems[i][j];

      stresshUnit = (PressureUnit)Enum.Parse(typeof(PressureUnit), selecteditems[i]);

      // update name of inputs (to display unit on sliders)
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
      Params.OnParametersChanged();
      this.OnDisplayExpired(true);
    }
    private void UpdateUIFromSelectedItems()
    {
      stresshUnit = (PressureUnit)Enum.Parse(typeof(PressureUnit), selecteditems[0]);

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
            "Unit"
    });
    private bool first = true;
    private PressureUnit stresshUnit = Units.StressUnit;
    string unitAbbreviation;
    #endregion

    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddGenericParameter("Result", "Res", "GSA Result", GH_ParamAccess.list);
      pManager.AddTextParameter("Element filter list", "El", "Filter results by list." + System.Environment.NewLine +
          "Element list should take the form:" + System.Environment.NewLine +
          " 1 11 to 20 step 2 P1 not (G1 to G6 step 3) P11 not (PA PB1 PS2 PM3 PA4 M1)" + System.Environment.NewLine +
          "Refer to GSA help file for definition of lists and full vocabulary.", GH_ParamAccess.item, "All");
      pManager.AddNumberParameter("Stress Layer", "σL", "Layer within the cross-section to get results." +
                           System.Environment.NewLine + "Input a number between -1 and 1, representing the normalised thickness," +
                           System.Environment.NewLine + "default value is zero => middle of the element.", GH_ParamAccess.item, 0);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      IQuantity quantity = new Pressure(0, stresshUnit);
      unitAbbreviation = string.Concat(quantity.ToString().Where(char.IsLetter));

      string note = System.Environment.NewLine + "DataTree organised as { CaseID ; Permutation ; ElementID } " +
                    System.Environment.NewLine + "fx. {1;2;3} is Case 1, Permutation 2, Element 3, where each " +
          System.Environment.NewLine + "branch contains a list of results in the following order:" +
          System.Environment.NewLine + "Vertex(1), Vertex(2), ..., Vertex(i), Centre" +
          System.Environment.NewLine + "+ve in-plane stresses: tensile(ie. + ve direct strain)." +
          System.Environment.NewLine + "+ve bending stress gives rise to tension on the top surface." +
          System.Environment.NewLine + "+ve shear stresses: +ve shear strain.";

      pManager.AddGenericParameter("Stress XX [" + unitAbbreviation + "]", "xx", "Stress in XX-direction in Global Axis." + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Stress YY [" + unitAbbreviation + "]", "yy", "Stress in YY-direction in Global Axis." + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Stress ZZ [" + unitAbbreviation + "]", "zz", "Stress in ZZ-direction in Global Axis." + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Stress XY [" + unitAbbreviation + "]", "xy", "Stress in XY-direction in Global Axis." + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Stress YZ [" + unitAbbreviation + "]", "yz", "Stress in YZ-direction in Global Axis." + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Stress ZX [" + unitAbbreviation + "]", "zx", "Stress in ZX-direction in Global Axis." + note, GH_ParamAccess.tree);
    }


    protected override void SolveInstance(IGH_DataAccess DA)
    {
      // Result to work on
      GsaResult result = new GsaResult();

      // Get filter case
      string elementlist = "All";
      GH_String gh_Type = new GH_String();
      if (DA.GetData(1, ref gh_Type))
        GH_Convert.ToString(gh_Type, out elementlist, GH_Conversion.Both);

      // Get layer
      double layer = 0;
      GH_String gh_Type1 = new GH_String();
      if (DA.GetData(2, ref gh_Type1))
        GH_Convert.ToDouble(gh_Type1, out layer, GH_Conversion.Both);

      // data trees to output
      DataTree<GH_UnitNumber> out_XX = new DataTree<GH_UnitNumber>();
      DataTree<GH_UnitNumber> out_YY = new DataTree<GH_UnitNumber>();
      DataTree<GH_UnitNumber> out_ZZ = new DataTree<GH_UnitNumber>();
      DataTree<GH_UnitNumber> out_XY = new DataTree<GH_UnitNumber>();
      DataTree<GH_UnitNumber> out_YZ = new DataTree<GH_UnitNumber>();
      DataTree<GH_UnitNumber> out_ZX = new DataTree<GH_UnitNumber>();

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

          List<GsaResultsValues> vals = result.Element2DStressValues(elementlist, layer, stresshUnit);

          List<int> permutations = result.SelectedPermutationIDs;

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

                  GH_Path p = new GH_Path(result.CaseID, permutations[index], elementID);

                  out_XX.AddRange(kvp.Value.Select(x => new GH_UnitNumber(x.Value.X.ToUnit(stresshUnit))), p); // use ToUnit to capture changes in dropdown
                  out_YY.AddRange(kvp.Value.Select(x => new GH_UnitNumber(x.Value.Y.ToUnit(stresshUnit))), p);
                  out_ZZ.AddRange(kvp.Value.Select(x => new GH_UnitNumber(x.Value.Z.ToUnit(stresshUnit))), p);
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

                  GH_Path p = new GH_Path(result.CaseID, permutations[index], elementID);

                  out_XY.AddRange(kvp.Value.Select(x => new GH_UnitNumber(x.Value.X)), p); // always use [rad] units
                  out_YZ.AddRange(kvp.Value.Select(x => new GH_UnitNumber(x.Value.Y)), p);
                  out_ZX.AddRange(kvp.Value.Select(x => new GH_UnitNumber(x.Value.Z)), p);
                }
              }
            });
          }
        }

        DA.SetDataTree(0, out_XX);
        DA.SetDataTree(1, out_YY);
        DA.SetDataTree(2, out_ZZ);
        DA.SetDataTree(3, out_XY);
        DA.SetDataTree(4, out_YZ);
        DA.SetDataTree(5, out_ZX);
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
      IQuantity quantity = new Pressure(0, stresshUnit);
      unitAbbreviation = string.Concat(quantity.ToString().Where(char.IsLetter));

      int i = 0;
      Params.Output[i++].Name = "Stress XX [" + unitAbbreviation + "]";
      Params.Output[i++].Name = "Stress YY [" + unitAbbreviation + "]";
      Params.Output[i++].Name = "Stress ZZ [" + unitAbbreviation + "]";
      Params.Output[i++].Name = "Stress XY [" + unitAbbreviation + "]";
      Params.Output[i++].Name = "Stress YZ [" + unitAbbreviation + "]";
      Params.Output[i++].Name = "Stress ZX [" + unitAbbreviation + "]";
    }
    #endregion
  }
}

