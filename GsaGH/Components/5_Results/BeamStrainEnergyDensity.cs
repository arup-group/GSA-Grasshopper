using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;

using GH_IO.Serialization;

using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;

using GsaGH.Components.Helpers;
using GsaGH.Helpers;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Parameters.Results;
using GsaGH.Properties;

using OasysGH;
using OasysGH.Components;
using OasysGH.Parameters;
using OasysGH.UI;
using OasysGH.Units;
using OasysGH.Units.Helpers;

using OasysUnits;

using EnergyUnit = OasysUnits.Units.EnergyUnit;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to get GSA Beam strain energy density results
  /// </summary>
  public class BeamStrainEnergyDensity : GH_OasysDropDownComponent {
    public override Guid ComponentGuid => new Guid("c1a927cb-ad0e-4a69-94ce-9ad079047d21");
    public override GH_Exposure Exposure => GH_Exposure.quarternary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.BeamStrainEnergyDensity;
    private readonly List<string> _checkboxText = new List<string>() {
      "Average",
    };
    private bool _average = true;
    private EnergyUnit _energyUnit = DefaultUnits.EnergyUnit;
    private List<bool> _initialCheckState = new List<bool>() {
      true,
    };

    public BeamStrainEnergyDensity() : base("Beam Strain Energy Density", "StrainEnergy",
      "Element1D Strain Energy Density result values", CategoryName.Name(),
      SubCategoryName.Cat5()) {
      Hidden = true;
    }

    public override void CreateAttributes() {
      if (!_isInitialised) {
        InitialiseDropdowns();
      }

      m_attributes = new DropDownCheckBoxesComponentAttributes(this, SetSelected, _dropDownItems,
        _selectedItems, SetAnalysis, _initialCheckState, _checkboxText, _spacerDescriptions);
    }

    public override bool Read(GH_IReader reader) {
      _average = reader.GetBoolean("checked");
      _initialCheckState = new List<bool>() {
        _average,
      };
      return base.Read(reader);
    }

    public void SetAnalysis(List<bool> value) {
      _average = value[0];
      UpdateInputs();
    }

    public override void SetSelected(int i, int j) {
      _selectedItems[i] = _dropDownItems[i][j];
      if (i == 1) {
        _energyUnit = (EnergyUnit)UnitsHelper.Parse(typeof(EnergyUnit), _selectedItems[i]);
      }

      base.UpdateUI();
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

    public override bool Write(GH_IWriter writer) {
      writer.SetBoolean("checked", _average);
      return base.Write(writer);
    }

    protected override void InitialiseDropdowns() {
      _spacerDescriptions = new List<string>(new[] {
        "Max/Min",
        "Energy Unit",
        "Settings",
      });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      _dropDownItems.Add(new List<string>(new[] {
        "All",
        "Max",
        "Min",
      }));
      _selectedItems.Add(_dropDownItems[0][0]);

      _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Energy));
      _selectedItems.Add(Energy.GetAbbreviation(_energyUnit));

      _isInitialised = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaResultParameter(), "Result", "Res", "GSA Result",
        GH_ParamAccess.list);
      pManager.AddGenericParameter("Element filter list", "El",
        "Filter results by list (by default 'all')" + Environment.NewLine
        + "Input a GSA List or a text string taking the form:" + Environment.NewLine
        + " 1 11 to 20 step 2 P1 not (G1 to G6 step 3) P11 not (PA PB1 PS2 PM3 PA4 M1)"
        + Environment.NewLine
        + "Refer to GSA help file for definition of lists and full vocabulary.",
        GH_ParamAccess.item);
      pManager[1].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      string unitAbbreviation = Energy.GetAbbreviation(_energyUnit) + "/m\u00B3";
      string note = ResultNotes.Note1dResults;

      pManager.AddGenericParameter("Strain energy density [" + unitAbbreviation + "]", "E",
        "Strain energy density. The strain energy density for a beam is a measure of how hard the beam is working. The average strain energy density is the average density along the element or member."
        + note, GH_ParamAccess.tree);
    }

    protected override void SolveInternal(IGH_DataAccess da) {
      GsaResult result;
      string elementlist = "All";
      int positionsCount = 3;
      if (!_average) {
        var ghDivisions = new GH_Integer();
        if (da.GetData(2, ref ghDivisions)) {
          GH_Convert.ToInt32(ghDivisions, out positionsCount, GH_Conversion.Both);
        } else {
          positionsCount = 3;
        }

        positionsCount = Math.Abs(positionsCount) + 2; // taken absolute value and add 2 end points.
      }

      var outResults = new DataTree<GH_UnitNumber>();

      var ghTypes = new List<GH_ObjectWrapper>();
      da.GetDataList(0, ghTypes);

      foreach (GH_ObjectWrapper ghTyp in ghTypes) {
        result = Inputs.GetResultInput(this, ghTyp);
        if (result == null) {
          return;
        }

        elementlist = Inputs.GetElementListDefinition(this, da, 1, result.Model);
        ReadOnlyCollection<int> elementIds = result.ElementIds(elementlist, 1);
        if (_average) {
          IEntity0dResultSubset<IEnergyDensity, Entity0dExtremaKey> resultSet =
            result.Element1dAverageStrainEnergyDensities.ResultSubset(elementIds);

          List<int> permutations = result.SelectedPermutationIds ?? new List<int>() {
            1,
          };
          if (permutations.Count == 1 && permutations[0] == -1) {
            permutations = Enumerable.Range(1, resultSet.Subset.Values.First().Count).ToList();
          }

          if (_selectedItems[0] == "All") {
            foreach (KeyValuePair<int, IList<IEnergyDensity>> kvp in resultSet.Subset) {
              foreach (int p in permutations) {
                var path = new GH_Path(result.CaseId,
                  result.SelectedPermutationIds == null ? 0 : p, kvp.Key);
                outResults.Add(new GH_UnitNumber(kvp.Value[p - 1].EnergyDensity
                  .ToUnit(_energyUnit)), path);
              }
            }
          } else {
            Entity0dExtremaKey key = _selectedItems[0] == "Max" ? resultSet.Max : resultSet.Min;
            IEnergyDensity extrema = resultSet.GetExtrema(key);
            int perm = result.CaseType == CaseType.AnalysisCase ? 0 : 1;
            var path = new GH_Path(result.CaseId, key.Permutation + perm, key.Id);
            outResults.Add(new GH_UnitNumber(extrema.EnergyDensity
                  .ToUnit(_energyUnit)), path);
          }
        } else {
          IEntity1dResultSubset<IEnergyDensity, Entity1dExtremaKey> resultSet =
            result.Element1dStrainEnergyDensities.ResultSubset(elementIds, positionsCount);

          List<int> permutations = result.SelectedPermutationIds ?? new List<int>() {
            1,
          };
          if (permutations.Count == 1 && permutations[0] == -1) {
            permutations = Enumerable.Range(1, resultSet.Subset.Values.First().Count).ToList();
          }

          if (_selectedItems[0] == "All") {
            foreach (KeyValuePair<int, IList<IEntity1dQuantity<IEnergyDensity>>> kvp in resultSet.Subset) {
              foreach (int p in permutations) {
                var path = new GH_Path(result.CaseId, result.SelectedPermutationIds == null ? 0 : p, kvp.Key);
                outResults.AddRange(kvp.Value[p - 1].Results.Values.Select(
                  r => new GH_UnitNumber(r.EnergyDensity.ToUnit(_energyUnit))), path);
              }
            }
          } else {
            Entity1dExtremaKey key = _selectedItems[0] == "Max" ? resultSet.Max : resultSet.Min;
            IEnergyDensity extrema = resultSet.GetExtrema(key);
            int perm = result.CaseType == CaseType.AnalysisCase ? 0 : 1;
            var path = new GH_Path(result.CaseId, key.Permutation + perm, key.Id);
            outResults.Add(new GH_UnitNumber(extrema.EnergyDensity.ToUnit(_energyUnit)), path);
          }
        }

        PostHog.Result(result.CaseType, 1, "StrainEnergy");
      }

      da.SetDataTree(0, outResults);
    }

    protected override void UpdateUIFromSelectedItems() {
      if (_selectedItems.Count == 1) {
        _spacerDescriptions.Insert(0, "Max/Min");
        _dropDownItems.Insert(0, new List<string>(new[] {
          "All",
          "Max",
          "Min",
        }));
        _selectedItems.Insert(0, _dropDownItems[0][0]);
      }

      _energyUnit = (EnergyUnit)UnitsHelper.Parse(typeof(EnergyUnit), _selectedItems[1]);
      UpdateInputs();
      base.UpdateUIFromSelectedItems();
    }

    private void UpdateInputs() {
      RecordUndoEvent("Toggled Average");
      if (_average) {
        while (Params.Input.Count > 2) {
          Params.UnregisterInputParameter(Params.Input[2], true);
        }
      } else {
        if (Params.Input.Count < 3) {
          Params.RegisterInputParam(new Param_Integer());
        }
      }

      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      Params.OnParametersChanged();
      ExpireSolution(true);
    }
  }
}
