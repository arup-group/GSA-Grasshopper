using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Parameters;
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
  /// Component to get GSA Beam strain energy density results
  /// </summary>
  public class BeamStrainEnergy : GH_OasysDropDownComponent
  {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("c1a927cb-ad0e-4a69-94ce-9ad079047d21");
    public override GH_Exposure Exposure => GH_Exposure.quarternary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.StrainEnergyDensity;

    public BeamStrainEnergy() : base("Beam Strain Energy Density",
      "StrainEnergy",
      "Element1D Strain Energy Density result values",
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
      string unitAbbreviation = new Energy(0, EnergyUnit).ToString("a") + "/m\u00B3";
      string note = System.Environment.NewLine + "DataTree organised as { CaseID ; Permutation ; ElementID } " +
                    System.Environment.NewLine + "fx. {1;2;3} is Case 1, Permutation 2, Element 3, where each " +
          System.Environment.NewLine + "branch contains a list matching the NodeIDs in the ID output.";

      pManager.AddGenericParameter("Strain energy density [" + unitAbbreviation + "]", "E", "Strain energy density. The strain energy density for a beam is a measure of how hard the beam is working. The average strain energy density is the average density along the element or member." + note, GH_ParamAccess.tree);
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

      // Get number of divisions
      int positionsCount = 3;
      if (!Average)
      {
        GH_Integer gh_Div = new GH_Integer();
        if (DA.GetData(2, ref gh_Div))
          GH_Convert.ToInt32(gh_Div, out positionsCount, GH_Conversion.Both);
        else
          positionsCount = 3;
        positionsCount = Math.Abs(positionsCount) + 2; // taken absolute value and add 2 end points.
      }

      // data trees to output
      DataTree<GH_UnitNumber> out_transX = new DataTree<GH_UnitNumber>();

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

          List<GsaResultsValues> vals = Average ?
            result.Element1DStrainEnergyDensityValues(elementlist, EnergyUnit) :
            result.Element1DStrainEnergyDensityValues(elementlist, positionsCount, EnergyUnit);

          List<int> permutations = (result.SelectedPermutationIDs == null ? new List<int>() { 0 } : result.SelectedPermutationIDs);

          // loop through all permutations (analysis case will just have one)
          for (int index = 0; index < vals.Count; index++)
          {
            if (vals[index].xyzResults.Count == 0)
            {
              string[] typ = result.ToString().Split('{');
              string acase = typ[1].Replace('}', ' ');
              AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Case " + acase + "contains no Element1D results.");
              continue;
            }
            // loop through all elements
            foreach (KeyValuePair<int, ConcurrentDictionary<int, GsaResultQuantity>> kvp in vals[index].xyzResults)
            {
              int elementID = kvp.Key;
              ConcurrentDictionary<int, GsaResultQuantity> res = kvp.Value;
              if (res.Count == 0) { continue; }

              GH_Path p = new GH_Path(result.CaseID, permutations[index], elementID);

              out_transX.AddRange(res.Select(x => new GH_UnitNumber(x.Value.X.ToUnit(EnergyUnit))), p); // use ToUnit to capture changes in dropdown
            }
          }
        }

        DA.SetDataTree(0, out_transX);
      }
    }

    #region Custom UI
    List<string> m_checkboxText = new List<string>() { "Average" };
    List<bool> m_initialCheckState = new List<bool>() { true };
    bool Average = true;
    EnergyUnit EnergyUnit = DefaultUnits.EnergyUnit;
    public override void InitialiseDropdowns()
    {
      this.SpacerDescriptions = new List<string>(new string[]
        {
          "Energy Unit", "Settings"
        });

      this.DropDownItems = new List<List<string>>();
      this.SelectedItems = new List<string>();

      // Energy
      this.DropDownItems.Add(FilteredUnits.FilteredEnergyUnits);
      this.SelectedItems.Add(this.EnergyUnit.ToString());

      this.IsInitialised = true;
    }
    public override void CreateAttributes()
    {
      m_attributes = new OasysGH.UI.DropDownCheckBoxesComponentAttributes(this, SetSelected, this.DropDownItems, this.SelectedItems, SetAnalysis, this.m_initialCheckState, this.m_checkboxText, this.SpacerDescriptions);
    }
    public override void SetSelected(int i, int j)
    {
      this.SelectedItems[i] = this.DropDownItems[i][j];
      this.EnergyUnit = (EnergyUnit)Enum.Parse(typeof(EnergyUnit), this.SelectedItems[i]);
      base.UpdateUI();
    }
    public override void UpdateUIFromSelectedItems()
    {
      this.EnergyUnit = (EnergyUnit)Enum.Parse(typeof(EnergyUnit), SelectedItems[0]);
      UpdateInputs();
      base.UpdateUIFromSelectedItems();
    }

    public void SetAnalysis(List<bool> value)
    {
      this.Average = value[0];
      UpdateInputs();
    }
    private void UpdateInputs()
    {
      RecordUndoEvent("Toggled Average");
      if (Average)
      {
        //remove input parameters
        while (Params.Input.Count > 2)
          Params.UnregisterInputParameter(Params.Input[2], true);
      }
      else
      {
        //add input parameters
        Params.RegisterInputParam(new Param_Integer());
      }

      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      Params.OnParametersChanged();
      ExpireSolution(true);
    }

    public override void VariableParameterMaintenance()
    {
      string unitAbbreviation = Energy.GetAbbreviation(this.EnergyUnit) + "/m\u00B3";
      Params.Output[0].Name = "Strain energy density [" + unitAbbreviation + "]";

      if (!Average)
      {
        Params.Input[2].Name = "Intermediate Points";
        Params.Input[2].NickName = "nP";
        Params.Input[2].Description = "Number of intermediate equidistant points (default 3)";
        Params.Input[2].Optional = true;
      }
    }
    #endregion

    #region (de)serialization
    public override bool Write(GH_IO.Serialization.GH_IWriter writer)
    {
      writer.SetBoolean("checked", Average);
      return base.Write(writer);
    }
    public override bool Read(GH_IO.Serialization.GH_IReader reader)
    {
      Average = reader.GetBoolean("checked");
      return base.Read(reader);
    }
    #endregion
  }
}

