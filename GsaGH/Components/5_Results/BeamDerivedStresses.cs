using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;

using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
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
using OasysGH.Units;
using OasysGH.Units.Helpers;

using OasysUnits;
using OasysUnits.Units;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to get GSA beam displacement values
  /// </summary>
  public class BeamDerivedStresses : GH_OasysDropDownComponent {
    public override Guid ComponentGuid => new Guid("bd54fa3e-ea2c-4195-b1a8-120d1a213b75");
    public override GH_Exposure Exposure => GH_Exposure.quarternary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.BeamDerivedStresses;
    private PressureUnit _stressUnit = DefaultUnits.StressUnitResult;

    public BeamDerivedStresses() : base("Beam Derived Stresses", "vonMises",
      "Element1D Derived Stress results like von Mises", CategoryName.Name(),
      SubCategoryName.Cat5()) {
      Hidden = true;
    }

    public override void SetSelected(int i, int j) {
      _selectedItems[i] = _dropDownItems[i][j];
      _stressUnit = (PressureUnit)UnitsHelper.Parse(typeof(PressureUnit), _selectedItems[1]);
      base.UpdateUI();
    }

    public override void VariableParameterMaintenance() {
      string unitAbbreviation = Pressure.GetAbbreviation(_stressUnit);
      int i = 0;
      Params.Output[i++].Name = "Elastic Shear Y [" + unitAbbreviation + "]";
      Params.Output[i++].Name = "Elastic Shear Z [" + unitAbbreviation + "]";
      Params.Output[i++].Name = "Torsional [" + unitAbbreviation + "]";
      Params.Output[i++].Name = "von Mises [" + unitAbbreviation + "]";
    }

    protected override void InitialiseDropdowns() {
      _spacerDescriptions = new List<string>(new[] {
        "Max/Min",
        "Unit",
      });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      _dropDownItems.Add(ExtremaHelper.Stress1dDerived.ToList());
      _selectedItems.Add(_dropDownItems[0][0]);

      _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Stress));
      _selectedItems.Add(Pressure.GetAbbreviation(_stressUnit));

      _isInitialised = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaResultParameter(), "Result", "Res", "GSA Result",
        GH_ParamAccess.list);
      pManager.AddParameter(new GsaElementMemberListParameter());
      pManager[1].Optional = true;
      pManager.AddIntegerParameter("Intermediate Points", "nP",
        "Number of intermediate equidistant points (default 3)", GH_ParamAccess.item, 3);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      string unitAbbreviation = Pressure.GetAbbreviation(_stressUnit);
      string note = "\n+ve stresses: tensile";
      pManager.AddGenericParameter("Elastic Shear Y [" + unitAbbreviation + "]", "SEy",
        "The maximum elastic shear stresses in local Y-axis" + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Elastic Shear Z [" + unitAbbreviation + "]", "SEz",
        "The maximum elastic shear stresses in local Z-axis" + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Torsional [" + unitAbbreviation + "]", "St",
        "Torsional stress: St = Mxx/Ct \nwhere Ct is the ‘torsion modulus’. (Refer to the " +
        "GSA manual for details.) ", GH_ParamAccess.tree);
      pManager.AddGenericParameter("von Mises [" + unitAbbreviation + "]", "vM",
        "Von Mises stress: This is calculated assuming the maximum through thickness " +
        "stress and torsional stress coexist. " +
        "\nIn most cases this is an over-estimate of the von Mises stress. " +
        "\nVon Mises stress is not calculated for cases with enveloping operators in them.", GH_ParamAccess.tree);
    }

    protected override void SolveInternal(IGH_DataAccess da) {
      GsaResult result;
      string elementlist = "All";
      var ghDivisions = new GH_Integer();
      da.GetData(2, ref ghDivisions);
      GH_Convert.ToInt32(ghDivisions, out int positionsCount, GH_Conversion.Both);
      positionsCount = Math.Abs(positionsCount) + 2; // taken absolute value and add 2 end points.

      var ghTypes = new List<GH_ObjectWrapper>();
      da.GetDataList(0, ghTypes);

      var outShearY = new DataTree<GH_UnitNumber>();
      var outShearZ = new DataTree<GH_UnitNumber>();
      var outTorsional = new DataTree<GH_UnitNumber>();
      var outVonMises = new DataTree<GH_UnitNumber>();

      foreach (GH_ObjectWrapper ghTyp in ghTypes) {
        result = Inputs.GetResultInput(this, ghTyp);
        if (result == null) {
          return;
        }

        elementlist = Inputs.GetElementListDefinition(this, da, 1, result.Model);
        ReadOnlyCollection<int> elementIds = result.ElementIds(elementlist, 1);
        IEntity1dResultSubset<IStress1dDerived, ResultDerivedStress1d<Entity1dExtremaKey>> resultSet =
          result.Element1dDerivedStresses.ResultSubset(elementIds, positionsCount);

        List<int> permutations = result.SelectedPermutationIds ?? new List<int>() {
          1,
        };
        if (permutations.Count == 1 && permutations[0] == -1) {
          permutations = Enumerable.Range(1, resultSet.Subset.Values.First().Count).ToList();
        }

        if (_selectedItems[0] == ExtremaHelper.Stress1d[0]) {
          foreach (KeyValuePair<int, IList<IEntity1dQuantity<IStress1dDerived>>> kvp in resultSet.Subset) {
            foreach (int p in permutations) {
              var path = new GH_Path(result.CaseId, result.SelectedPermutationIds == null ? 0 : p, kvp.Key);
              outShearY.AddRange(kvp.Value[p - 1].Results.Values.Select(
                r => new GH_UnitNumber(r.ElasticShearY.ToUnit(_stressUnit))), path);
              outShearZ.AddRange(kvp.Value[p - 1].Results.Values.Select(
                r => new GH_UnitNumber(r.ElasticShearZ.ToUnit(_stressUnit))), path);
              outTorsional.AddRange(kvp.Value[p - 1].Results.Values.Select(
                r => new GH_UnitNumber(r.Torsional.ToUnit(_stressUnit))), path);
              outVonMises.AddRange(kvp.Value[p - 1].Results.Values.Select(
                r => new GH_UnitNumber(r.VonMises.ToUnit(_stressUnit))), path);
            }
          }
        } else {
          Entity1dExtremaKey key = ExtremaHelper.Stress1dDerivedExtremaKey(resultSet, _selectedItems[0]);
          IStress1dDerived extrema = resultSet.GetExtrema(key);
          int perm = result.CaseType == CaseType.AnalysisCase ? 0 : 1;
          var path = new GH_Path(result.CaseId, key.Permutation + perm, key.Id);
          outShearY.Add(new GH_UnitNumber(extrema.ElasticShearY.ToUnit(_stressUnit)), path);
          outShearZ.Add(new GH_UnitNumber(extrema.ElasticShearZ.ToUnit(_stressUnit)), path);
          outTorsional.Add(new GH_UnitNumber(extrema.Torsional.ToUnit(_stressUnit)), path);
          outVonMises.Add(new GH_UnitNumber(extrema.VonMises.ToUnit(_stressUnit)), path);
        }

        PostHog.Result(result.CaseType, 1, "Displacement");
      }

      da.SetDataTree(0, outShearY);
      da.SetDataTree(1, outShearZ);
      da.SetDataTree(2, outTorsional);
      da.SetDataTree(3, outVonMises);
    }

    protected override void UpdateUIFromSelectedItems() {
      _stressUnit = (PressureUnit)UnitsHelper.Parse(typeof(PressureUnit), _selectedItems[1]);
      base.UpdateUIFromSelectedItems();
    }
  }
}
