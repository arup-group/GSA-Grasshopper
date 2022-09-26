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
using OasysGH.Parameters;
using OasysUnits;
using OasysUnits.Units;

namespace GsaGH.Components
{
  /// <summary>
  /// Component to retrieve non-geometric objects from a GSA model
  /// </summary>
  public class BeamDisplacement : GH_OasysComponent, IGH_VariableParameterComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("21ec9005-1b2f-4eb8-8171-b2c0190a4a54");
    public BeamDisplacement()
      : base("Beam Displacements", "BeamDisp", "Element1D Translation and Rotation result values",
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat5())
    { this.Hidden = true; } // sets the initial state of the component to hidden
    public override GH_Exposure Exposure => GH_Exposure.quarternary;

    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.BeamDisplacement;
    #endregion

    #region Custom UI
    //This region overrides the typical component layout
    public override void CreateAttributes()
    {
      if (first)
      {
        dropdownitems = new List<List<string>>();
        selecteditems = new List<string>();

        // length
        //dropdownitems.Add(Enum.GetNames(typeof(Units.LengthUnit)).ToList());
        dropdownitems.Add(Units.FilteredLengthUnits);
        selecteditems.Add(lengthUnit.ToString());

        IQuantity quantity = new Length(0, lengthUnit);
        unitAbbreviation = string.Concat(quantity.ToString().Where(char.IsLetter));

        first = false;
      }
      m_attributes = new UI.MultiDropDownComponentUI(this, SetSelected, dropdownitems, selecteditems, spacerDescriptions);
    }
    public void SetSelected(int i, int j)
    {
      // change selected item
      selecteditems[i] = dropdownitems[i][j];

      lengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), selecteditems[i]);

      // update name of inputs (to display unit on sliders)
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
      Params.OnParametersChanged();
      this.OnDisplayExpired(true);
    }
    private void UpdateUIFromSelectedItems()
    {
      lengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), selecteditems[0]);

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
    private LengthUnit lengthUnit = Units.LengthUnitResult;
    string unitAbbreviation;
    #endregion

    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddGenericParameter("Result", "Res", "GSA Result", GH_ParamAccess.list);
      pManager.AddTextParameter("Element filter list", "El", "Filter results by list." + System.Environment.NewLine +
          "Element list should take the form:" + System.Environment.NewLine +
          " 1 11 to 20 step 2 P1 not (G1 to G6 step 3) P11 not (PA PB1 PS2 PM3 PA4 M1)" + System.Environment.NewLine +
          "Refer to GSA help file for definition of lists and full vocabulary.", GH_ParamAccess.item, "All");
      pManager.AddIntegerParameter("Intermediate Points", "nP", "Number of intermediate equidistant points (default 3)", GH_ParamAccess.item, 3);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      IQuantity quantity = new Length(0, lengthUnit);
      unitAbbreviation = string.Concat(quantity.ToString().Where(char.IsLetter));

      string note = System.Environment.NewLine + "DataTree organised as { CaseID ; Permutation ; ElementID } " +
                    System.Environment.NewLine + "fx. {1;2;3} is Case 1, Permutation 2, Element 3, where each " +
          System.Environment.NewLine + "branch contains a list of results per element position.";

      pManager.AddGenericParameter("Translations X [" + unitAbbreviation + "]", "Ux", "Translations in X-direction in Global Axis." + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Translations Y [" + unitAbbreviation + "]", "Uy", "Translations in Y-direction in Global Axis." + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Translations Z [" + unitAbbreviation + "]", "Uz", "Translations in Z-direction in Global Axis." + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Translations |XYZ| [" + unitAbbreviation + "]", "|U|", "Combined |XYZ| Translations in Global Axis." + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Rotations XX [rad]", "Rxx", "Rotations around X-axis in Global Axis." + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Rotations YY [rad]", "Ryy", "Rotations around Y-axis in Global Axiss." + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Rotations ZZ [rad]", "Rzz", "Rotations around Z-axis in Global Axis." + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Rotations |XYZ| [rad]", "|R|", "Combined |XXYYZZ| Rotations in Global Axis." + note, GH_ParamAccess.tree);
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
          if (gh_typ.Value is GsaResultGoo)
          {
            result = ((GsaResultGoo)gh_typ.Value).Value;
          }
          else
          {
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Error converting input to GSA Result");
            return;
          }

          List<GsaResultsValues> vals = result.Element1DDisplacementValues(elementlist, positionsCount, lengthUnit);
          
          List<int> permutations = (result.SelectedPermutationIDs == null ? new List<int>() { 0 } : result.SelectedPermutationIDs);

          // loop through all permutations (analysis case will just have one)
          for (int index = 0; index < vals.Count; index++)
          {
            if (vals[index].xyzResults.Count == 0 & vals[index].xxyyzzResults.Count == 0)
            {
              string[] typ = result.ToString().Split('{');
              string acase = typ[1].Replace('}', ' ');
              AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Case " + acase + "contains no Element1D results.");
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

                  out_transX.AddRange(res.Select(x => new GH_UnitNumber(x.Value.X.ToUnit(lengthUnit))), p); // use ToUnit to capture changes in dropdown
                  out_transY.AddRange(res.Select(x => new GH_UnitNumber(x.Value.Y.ToUnit(lengthUnit))), p);
                  out_transZ.AddRange(res.Select(x => new GH_UnitNumber(x.Value.Z.ToUnit(lengthUnit))), p);
                  out_transXYZ.AddRange(res.Select(x => new GH_UnitNumber(x.Value.XYZ.ToUnit(lengthUnit))), p);
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
      IQuantity quantity = new Length(0, lengthUnit);
      unitAbbreviation = string.Concat(quantity.ToString().Where(char.IsLetter));

      int i = 0;
      Params.Output[i++].Name = "Translations X [" + unitAbbreviation + "]";
      Params.Output[i++].Name = "Translations Y [" + unitAbbreviation + "]";
      Params.Output[i++].Name = "Translations Z [" + unitAbbreviation + "]";
      Params.Output[i++].Name = "Translations |XYZ| [" + unitAbbreviation + "]";

    }
    #endregion
  }
}

