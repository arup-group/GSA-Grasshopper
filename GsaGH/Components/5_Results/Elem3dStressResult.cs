﻿using System;
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
  public class Elem3dStress : GH_OasysComponent, IGH_VariableParameterComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("c9bdab98-0fe2-4852-b99c-c626515b3781");
    public override GH_Exposure Exposure => GH_Exposure.senary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.Stress3D;

    public Elem3dStress() : base("3D Stresses",
      "Stress3D",
      "3D Element Stress result values",
      Ribbon.CategoryName.Name(),
      Ribbon.SubCategoryName.Cat5())
    { this.Hidden = true; } // sets the initial state of the component to hidden
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
        dropdownitems.Add(FilteredUnits.FilteredStressUnits);
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
    private PressureUnit stresshUnit = DefaultUnits.StressUnitResult;
    string unitAbbreviation;
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
      IQuantity quantity = new Pressure(0, stresshUnit);
      unitAbbreviation = string.Concat(quantity.ToString().Where(char.IsLetter));

      string note = System.Environment.NewLine + "DataTree organised as { CaseID ; Permutation ; ElementID } " +
                    System.Environment.NewLine + "fx. {1;2;3} is Case 1, Permutation 2, Element 3, where each " +
          System.Environment.NewLine + "branch contains a list of results in the following order:" +
          System.Environment.NewLine + "Vertex(1), Vertex(2), ..., Vertex(i), Centre." +
          System.Environment.NewLine + "+ve stresses: tensile (ie. +ve direct strain)";

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

          List<GsaResultsValues> vals = result.Element3DStressValues(elementlist, stresshUnit);

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

                  out_XX.AddRange(res.Select(x => new GH_UnitNumber(x.Value.X.ToUnit(stresshUnit))), p); // use ToUnit to capture changes in dropdown
                  out_YY.AddRange(res.Select(x => new GH_UnitNumber(x.Value.Y.ToUnit(stresshUnit))), p);
                  out_ZZ.AddRange(res.Select(x => new GH_UnitNumber(x.Value.Z.ToUnit(stresshUnit))), p);
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

                  out_XY.AddRange(res.Select(x => new GH_UnitNumber(x.Value.X)), p); // always use [rad] units
                  out_YZ.AddRange(res.Select(x => new GH_UnitNumber(x.Value.Y)), p);
                  out_ZX.AddRange(res.Select(x => new GH_UnitNumber(x.Value.Z)), p);
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

