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
  public class BeamStresses : GH_OasysDropDownComponent {
    public override Guid ComponentGuid => new Guid("b0fd4d6a-d50c-4c3b-91f5-dae5b1707d2c");
    public override GH_Exposure Exposure => GH_Exposure.quarternary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.BeamStresses;
    private PressureUnit _stressUnit = DefaultUnits.StressUnitResult;

    public BeamStresses() : base("Beam Stresses", "BeamStress",
      "Element1D Stress result values", CategoryName.Name(),
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
      Params.Output[i++].Name = "Axial [" + unitAbbreviation + "]";
      Params.Output[i++].Name = "Shear Y [" + unitAbbreviation + "]";
      Params.Output[i++].Name = "Shear Z [" + unitAbbreviation + "]";
      Params.Output[i++].Name = "Bending Y +ve z [" + unitAbbreviation + "]";
      Params.Output[i++].Name = "Bending Y -ve z [" + unitAbbreviation + "]";
      Params.Output[i++].Name = "Bending Z +ve y [" + unitAbbreviation + "]";
      Params.Output[i++].Name = "Bending Z -ve y [" + unitAbbreviation + "]";
      Params.Output[i++].Name = "Combined C1 [" + unitAbbreviation + "]";
      Params.Output[i++].Name = "Combined C2 [" + unitAbbreviation + "]";
    }

    protected override void InitialiseDropdowns() {
      _spacerDescriptions = new List<string>(new[] {
        "Max/Min",
        "Unit",
      });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      _dropDownItems.Add(ExtremaHelper.Stress1d.ToList());
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
      pManager.AddGenericParameter("Axial [" + unitAbbreviation + "]", "A",
        "Axial stress:\nA = Fx/Area" + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Shear Y [" + unitAbbreviation + "]", "Sy",
        "Shear stresses:\nSy = Fy/Ay\nwhere Ay and Az are the shear areas calculated in " +
        "accordance with selected code. Note that torsional stresses are ignored.",
        GH_ParamAccess.tree);
      pManager.AddGenericParameter("Shear Z [" + unitAbbreviation + "]", "Sz",
        "Shear stresses:\nSz = Fz/Az\nwhere Ay and Az are the shear areas calculated in " +
        "accordance with selected code. Note that torsional stresses are ignored.",
        GH_ParamAccess.tree);
      pManager.AddGenericParameter("Bending Y +ve z [" + unitAbbreviation + "]", "By+",
        "Bending stresses:\nFor sections that have Iyz=0:\nBy = Myy/Iyy x Dz" +
        "\n- where Dz & Dy are the distances from the centroid to the edge of the" +
        "\nsection in the +ve z and y directions respectively." +
        "\nFor sections that have non-zero Iyz: refer to the GSA manual.", GH_ParamAccess.tree);
      pManager.AddGenericParameter("Bending Y -ve z [" + unitAbbreviation + "]", "By-",
        "Bending stresses:\nFor sections that have Iyz=0:\nBy = Myy/Iyy x Dz" +
        "\n- where Dz & Dy are the distances from the centroid to the edge of the" +
        "\nsection in the +ve z and y directions respectively." +
        "\nFor sections that have non-zero Iyz: refer to the GSA manual.", GH_ParamAccess.tree);
      pManager.AddGenericParameter("Bending Z +ve y [" + unitAbbreviation + "]", "Bz+",
        "Bending stresses:\nFor sections that have Iyz=0:\nBz = -Mzz/Izz x Dy" +
        "\n- where Dz & Dy are the distances from the centroid to the edge of the" +
        "\nsection in the +ve z and y directions respectively." +
        "\nFor sections that have non-zero Iyz: refer to the GSA manual.", GH_ParamAccess.tree);
      pManager.AddGenericParameter("Bending Z -ve y [" + unitAbbreviation + "]", "Bz-",
        "Bending stresses:\nFor sections that have Iyz=0:\nBz = -Mzz/Izz x Dy" +
        "\n- where Dz & Dy are the distances from the centroid to the edge of the" +
        "\nsection in the +ve z and y directions respectively." +
        "\nFor sections that have non-zero Iyz: refer to the GSA manual.", GH_ParamAccess.tree);
      pManager.AddGenericParameter("Combined C1 [" + unitAbbreviation + "]", "C1",
        "C1 is the maximum extreme fibre longitudinal stress due to axial forces " +
        "and transverse bending\nC1 and C2 stresses are not calculated for cases with " +
        "enveloping operators in them." + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Combined C2 [" + unitAbbreviation + "]", "C2",
        "C2 is the minimum extreme fibre longitudinal stress due to axial forces " +
        "and transverse bending\nC1 and C2 stresses are not calculated for cases with " +
        "enveloping operators in them." + note, GH_ParamAccess.tree);
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

      var outAxial = new DataTree<GH_UnitNumber>();
      var outShearY = new DataTree<GH_UnitNumber>();
      var outShearZ = new DataTree<GH_UnitNumber>();
      var outBendingYyPos = new DataTree<GH_UnitNumber>();
      var outBendingYyNeg = new DataTree<GH_UnitNumber>();
      var outBendingZzPos = new DataTree<GH_UnitNumber>();
      var outBenidngZzNeg = new DataTree<GH_UnitNumber>();
      var outC1 = new DataTree<GH_UnitNumber>();
      var outC2 = new DataTree<GH_UnitNumber>();

      foreach (GH_ObjectWrapper ghTyp in ghTypes) {
        result = Inputs.GetResultInput(this, ghTyp);
        if (result == null) {
          return;
        }

        elementlist = Inputs.GetElementListDefinition(this, da, 1, result.Model);
        ReadOnlyCollection<int> elementIds = result.ElementIds(elementlist, 1);
        IEntity1dResultSubset<IStress1d, ResultStress1d<Entity1dExtremaKey>> resultSet =
          result.Element1dStresses.ResultSubset(elementIds, positionsCount);

        List<int> permutations = result.SelectedPermutationIds ?? new List<int>() {
          1,
        };
        if (permutations.Count == 1 && permutations[0] == -1) {
          permutations = Enumerable.Range(1, resultSet.Subset.Values.First().Count).ToList();
        }

        if (_selectedItems[0] == ExtremaHelper.Stress1d[0]) {
          foreach (KeyValuePair<int, IList<IEntity1dQuantity<IStress1d>>> kvp in resultSet.Subset) {
            foreach (int p in permutations) {
              var path = new GH_Path(result.CaseId, result.SelectedPermutationIds == null ? 0 : p, kvp.Key);
              outAxial.AddRange(kvp.Value[p - 1].Results.Values.Select(
                r => new GH_UnitNumber(r.Axial.ToUnit(_stressUnit))), path);
              outShearY.AddRange(kvp.Value[p - 1].Results.Values.Select(
                r => new GH_UnitNumber(r.ShearY.ToUnit(_stressUnit))), path);
              outShearZ.AddRange(kvp.Value[p - 1].Results.Values.Select(
                r => new GH_UnitNumber(r.ShearZ.ToUnit(_stressUnit))), path);
              outBendingYyPos.AddRange(kvp.Value[p - 1].Results.Values.Select(
                r => new GH_UnitNumber(r.BendingYyPositiveZ.ToUnit(_stressUnit))), path);
              outBendingYyNeg.AddRange(kvp.Value[p - 1].Results.Values.Select(
                r => new GH_UnitNumber(r.BendingYyNegativeZ.ToUnit(_stressUnit))), path);
              outBendingZzPos.AddRange(kvp.Value[p - 1].Results.Values.Select(
                r => new GH_UnitNumber(r.BendingZzPositiveY.ToUnit(_stressUnit))), path);
              outBenidngZzNeg.AddRange(kvp.Value[p - 1].Results.Values.Select(
                r => new GH_UnitNumber(r.BendingZzNegativeY.ToUnit(_stressUnit))), path);
              outC1.AddRange(kvp.Value[p - 1].Results.Values.Select(
                r => new GH_UnitNumber(r.CombinedC1.ToUnit(_stressUnit))), path);
              outC2.AddRange(kvp.Value[p - 1].Results.Values.Select(
                r => new GH_UnitNumber(r.CombinedC2.ToUnit(_stressUnit))), path);
            }
          }
        } else {
          Entity1dExtremaKey key = ExtremaHelper.Stress1dExtremaKey(resultSet, _selectedItems[0]);
          IStress1d extrema = resultSet.GetExtrema(key);
          int perm = result.CaseType == CaseType.AnalysisCase ? 0 : 1;
          var path = new GH_Path(result.CaseId, key.Permutation + perm, key.Id);
          outAxial.Add(new GH_UnitNumber(extrema.Axial.ToUnit(_stressUnit)), path);
          outShearY.Add(new GH_UnitNumber(extrema.ShearY.ToUnit(_stressUnit)), path);
          outShearZ.Add(new GH_UnitNumber(extrema.ShearZ.ToUnit(_stressUnit)), path);
          outBendingYyPos.Add(new GH_UnitNumber(extrema.BendingYyPositiveZ.
            ToUnit(_stressUnit)), path);
          outBendingYyNeg.Add(new GH_UnitNumber(extrema.BendingYyNegativeZ.
            ToUnit(_stressUnit)), path);
          outBendingZzPos.Add(new GH_UnitNumber(extrema.BendingZzPositiveY.
            ToUnit(_stressUnit)), path);
          outBenidngZzNeg.Add(new GH_UnitNumber(extrema.BendingZzNegativeY.
            ToUnit(_stressUnit)), path);
          outC1.Add(new GH_UnitNumber(extrema.CombinedC1.ToUnit(_stressUnit)), path);
          outC2.Add(new GH_UnitNumber(extrema.CombinedC2.ToUnit(_stressUnit)), path);
        }

        PostHog.Result(result.CaseType, 1, "Displacement");
      }

      da.SetDataTree(0, outAxial);
      da.SetDataTree(1, outShearY);
      da.SetDataTree(2, outShearZ);
      da.SetDataTree(3, outBendingYyPos);
      da.SetDataTree(4, outBendingYyNeg);
      da.SetDataTree(5, outBendingZzPos);
      da.SetDataTree(6, outBenidngZzNeg);
      da.SetDataTree(7, outC1);
      da.SetDataTree(8, outC2);
    }

    protected override void UpdateUIFromSelectedItems() {
      _stressUnit = (PressureUnit)UnitsHelper.Parse(typeof(PressureUnit), _selectedItems[1]);
      base.UpdateUIFromSelectedItems();
    }
  }
}
