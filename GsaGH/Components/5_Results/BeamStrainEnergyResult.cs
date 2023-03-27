﻿using System;
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

namespace GsaGH.Components {
  /// <summary>
  /// Component to get GSA Beam strain energy density results
  /// </summary>
  public class BeamStrainEnergy : GH_OasysDropDownComponent {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("c1a927cb-ad0e-4a69-94ce-9ad079047d21");
    public override GH_Exposure Exposure => GH_Exposure.quarternary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Properties.Resources.StrainEnergyDensity;

    public BeamStrainEnergy() : base("Beam Strain Energy Density",
      "StrainEnergy",
      "Element1D Strain Energy Density result values",
      CategoryName.Name(),
      SubCategoryName.Cat5()) => Hidden = true;
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaResultsParameter(), "Result", "Res", "GSA Result", GH_ParamAccess.list);
      pManager.AddTextParameter("Element filter list", "El", "Filter results by list." + Environment.NewLine +
          "Element list should take the form:" + Environment.NewLine +
          " 1 11 to 20 step 2 P1 not (G1 to G6 step 3) P11 not (PA PB1 PS2 PM3 PA4 M1)" + Environment.NewLine +
          "Refer to GSA help file for definition of lists and full vocabulary.", GH_ParamAccess.item, "All");
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      string unitAbbreviation = Energy.GetAbbreviation(_energyUnit) + "/m\u00B3";
      string note = Environment.NewLine + "DataTree organised as { CaseID ; Permutation ; ElementID } " +
                    Environment.NewLine + "fx. {1;2;3} is Case 1, Permutation 2, Element 3, where each " +
          Environment.NewLine + "branch contains a list of results per element position.";

      pManager.AddGenericParameter("Strain energy density [" + unitAbbreviation + "]", "E", "Strain energy density. The strain energy density for a beam is a measure of how hard the beam is working. The average strain energy density is the average density along the element or member." + note, GH_ParamAccess.tree);
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

      int positionsCount = 3;
      if (!_average) {
        var ghDivisions = new GH_Integer();
        if (da.GetData(2, ref ghDivisions))
          GH_Convert.ToInt32(ghDivisions, out positionsCount, GH_Conversion.Both);
        else
          positionsCount = 3;
        positionsCount = Math.Abs(positionsCount) + 2; // taken absolute value and add 2 end points.
      }

      var outTransX = new DataTree<GH_UnitNumber>();

      var ghTypes = new List<GH_ObjectWrapper>();
      if (!da.GetDataList(0, ghTypes)) {
        return;
      }

      foreach (GH_ObjectWrapper ghTyp in ghTypes) {
        switch (ghTyp?.Value) {
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

        List<GsaResultsValues> vals = _average ?
          result.Element1DAverageStrainEnergyDensityValues(elementlist, _energyUnit) :
          result.Element1DStrainEnergyDensityValues(elementlist, positionsCount, _energyUnit);

        List<int> permutations = result.SelectedPermutationIds ?? new List<int>() { 1 };
        if (permutations.Count == 1 && permutations[0] == -1)
          permutations = Enumerable.Range(1, vals.Count).ToList();

        foreach (int perm in permutations) {
          if (vals[perm - 1].XyzResults.Count == 0) {
            string acase = result.ToString().Replace('}', ' ').Replace('{', ' ');
            this.AddRuntimeWarning("Case " + acase + " contains no Element1D results.");
            continue;
          }
          foreach (KeyValuePair<int, ConcurrentDictionary<int, GsaResultQuantity>> kvp in vals[perm - 1].XyzResults) {
            int elementId = kvp.Key;
            ConcurrentDictionary<int, GsaResultQuantity> res = kvp.Value;
            if (res.Count == 0) { continue; }

            var path = new GH_Path(result.CaseId, result.SelectedPermutationIds == null ? 0 : perm, elementId);

            outTransX.AddRange(res.Select(x => new GH_UnitNumber(x.Value.X.ToUnit(_energyUnit))), path);
          }
        }
      }

      da.SetDataTree(0, outTransX);

      Helpers.PostHog.Result(result.Type, 1, GsaResultsValues.ResultType.StrainEnergy);
    }

    #region Custom UI
    private readonly List<string> _checkboxText = new List<string>() { "Average" };
    private List<bool> _initialCheckState = new List<bool>() { true };
    private bool _average = true;
    private EnergyUnit _energyUnit = DefaultUnits.EnergyUnit;
    public override void InitialiseDropdowns() {
      SpacerDescriptions = new List<string>(new[]
        {
          "Energy Unit",
          "Settings",
        });

      DropDownItems = new List<List<string>>();
      SelectedItems = new List<string>();

      DropDownItems.Add(UnitsHelper.GetFilteredAbbreviations((EngineeringUnits.Energy)));
      SelectedItems.Add(Energy.GetAbbreviation(_energyUnit));

      IsInitialised = true;
    }

    public override void CreateAttributes() {
      if (!IsInitialised)
        InitialiseDropdowns();
      m_attributes = new OasysGH.UI.DropDownCheckBoxesComponentAttributes(this, SetSelected, DropDownItems, SelectedItems, SetAnalysis, _initialCheckState, _checkboxText, SpacerDescriptions);
    }

    public override void SetSelected(int i, int j) {
      SelectedItems[i] = DropDownItems[i][j];
      _energyUnit = (EnergyUnit)UnitsHelper.Parse(typeof(EnergyUnit), SelectedItems[i]);
      base.UpdateUI();
    }

    public override void UpdateUIFromSelectedItems() {
      _energyUnit = (EnergyUnit)UnitsHelper.Parse(typeof(EnergyUnit), SelectedItems[0]);
      UpdateInputs();
      base.UpdateUIFromSelectedItems();
    }

    public void SetAnalysis(List<bool> value) {
      _average = value[0];
      UpdateInputs();
    }

    private void UpdateInputs() {
      RecordUndoEvent("Toggled Average");
      if (_average) {
        while (Params.Input.Count > 2)
          Params.UnregisterInputParameter(Params.Input[2], true);
      }
      else {
        if (Params.Input.Count < 3)
          Params.RegisterInputParam(new Param_Integer());
      }

      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      Params.OnParametersChanged();
      ExpireSolution(true);
    }

    public override void VariableParameterMaintenance() {
      string unitAbbreviation = Energy.GetAbbreviation(_energyUnit) + "/m\u00B3";
      Params.Output[0].Name = "Strain energy density [" + unitAbbreviation + "]";

      if (_average) {
        return;
      }

      Params.Input[2].Name = "Intermediate Points";
      Params.Input[2].NickName = "nP";
      Params.Input[2].Description = "Number of intermediate equidistant points (default 3)";
      Params.Input[2].Optional = true;
    }
    #endregion

    #region (de)serialization
    public override bool Write(GH_IO.Serialization.GH_IWriter writer) {
      writer.SetBoolean("checked", _average);
      return base.Write(writer);
    }
    public override bool Read(GH_IO.Serialization.GH_IReader reader) {
      _average = reader.GetBoolean("checked");
      _initialCheckState = new List<bool>() { _average };
      return base.Read(reader);
    }
    #endregion
  }
}

