using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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
      string unitAbbreviation = Energy.GetAbbreviation(this.EnergyUnit) + "/m\u00B3";
      string note = Environment.NewLine + "DataTree organised as { CaseID ; Permutation ; ElementID } " +
                    Environment.NewLine + "fx. {1;2;3} is Case 1, Permutation 2, Element 3, where each " +
          Environment.NewLine + "branch contains a list of results per element position.";

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

      if (elementlist.ToLower() == "all" || elementlist == "")
        elementlist = "All";

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

          List<GsaResultsValues> vals = Average ?
            result.Element1DAverageStrainEnergyDensityValues(elementlist, this.EnergyUnit) :
            result.Element1DStrainEnergyDensityValues(elementlist, positionsCount, this.EnergyUnit);

          List<int> permutations = (result.SelectedPermutationIDs == null ? new List<int>() { 1 } : result.SelectedPermutationIDs);
          if (permutations.Count == 1 && permutations[0] == -1)
            permutations = Enumerable.Range(1, vals.Count).ToList();

          // loop through all permutations (analysis case will just have one)
          foreach (int perm in permutations)
          {
            if (vals[perm - 1].xyzResults.Count == 0)
            {
              string acase = result.ToString().Replace('}', ' ').Replace('{', ' ');
              this.AddRuntimeWarning("Case " + acase + " contains no Element1D results.");
              continue;
            }
            // loop through all elements
            foreach (KeyValuePair<int, ConcurrentDictionary<int, GsaResultQuantity>> kvp in vals[perm - 1].xyzResults)
            {
              int elementID = kvp.Key;
              ConcurrentDictionary<int, GsaResultQuantity> res = kvp.Value;
              if (res.Count == 0) { continue; }

              GH_Path path = new GH_Path(result.CaseID, result.SelectedPermutationIDs == null ? 0 : perm, elementID);

              out_transX.AddRange(res.Select(x => new GH_UnitNumber(x.Value.X.ToUnit(this.EnergyUnit))), path); 
            }
          }
        }

        DA.SetDataTree(0, out_transX);

        Helpers.PostHog.Result(result.Type, 1, GsaResultsValues.ResultType.StrainEnergy);
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
      this.DropDownItems.Add(UnitsHelper.GetFilteredAbbreviations((EngineeringUnits.Energy)));
      this.SelectedItems.Add(Energy.GetAbbreviation(this.EnergyUnit));

      this.IsInitialised = true;
    }

    public override void CreateAttributes()
    {
      if (!IsInitialised)
        InitialiseDropdowns();
      m_attributes = new OasysGH.UI.DropDownCheckBoxesComponentAttributes(this, SetSelected, this.DropDownItems, this.SelectedItems, SetAnalysis, this.m_initialCheckState, this.m_checkboxText, this.SpacerDescriptions);
    }

    public override void SetSelected(int i, int j)
    {
      this.SelectedItems[i] = this.DropDownItems[i][j];
      this.EnergyUnit = (EnergyUnit)UnitsHelper.Parse(typeof(EnergyUnit), this.SelectedItems[i]);
      base.UpdateUI();
    }

    public override void UpdateUIFromSelectedItems()
    {
      this.EnergyUnit = (EnergyUnit)UnitsHelper.Parse(typeof(EnergyUnit), SelectedItems[0]);
      this.UpdateInputs();
      base.UpdateUIFromSelectedItems();
    }

    public void SetAnalysis(List<bool> value)
    {
      this.Average = value[0];
      this.UpdateInputs();
    }

    private void UpdateInputs()
    {
      RecordUndoEvent("Toggled Average");
      if (this.Average)
      {
        //remove input parameters
        while (this.Params.Input.Count > 2)
          this.Params.UnregisterInputParameter(Params.Input[2], true);
      }
      else
      {
        //add input parameters
        if (this.Params.Input.Count < 3)
          this.Params.RegisterInputParam(new Param_Integer());
      }

      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      this.Params.OnParametersChanged();
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
      writer.SetBoolean("checked", this.Average);
      return base.Write(writer);
    }
    public override bool Read(GH_IO.Serialization.GH_IReader reader)
    {
      this.Average = reader.GetBoolean("checked");
      m_initialCheckState = new List<bool>() { this.Average };
      return base.Read(reader);
    }
    #endregion
  }
}

