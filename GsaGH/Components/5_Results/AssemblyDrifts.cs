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
  ///   Component to get GSA assembly drift values
  /// </summary>
  public class AssemblyDrifts : GH_OasysDropDownComponent {
    public override Guid ComponentGuid => new Guid("0017f820-606b-43ec-a0af-573022980fb9");
    public override GH_Exposure Exposure => GH_Exposure.septenary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.AssemblyDrifts;
    private LengthUnit _lengthUnit = DefaultUnits.LengthUnitResult;

    public AssemblyDrifts() : base("Assembly Drifts", "AssemblyDrifts",
      "Assembly Drift result values", CategoryName.Name(), SubCategoryName.Cat5()) {
      Hidden = true;
    }

    public override void SetSelected(int i, int j) {
      _selectedItems[i] = _dropDownItems[i][j];
      _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[1]);
      base.UpdateUI();
    }

    public override void VariableParameterMaintenance() {
      string unitAbbreviation = Length.GetAbbreviation(_lengthUnit);
      int i = 0;
      Params.Output[i++].Name = "Drift X [" + unitAbbreviation + "]";
      Params.Output[i++].Name = "Drift Y [" + unitAbbreviation + "]";
      Params.Output[i].Name = "Drift XY [" + unitAbbreviation + "]";
    }

    protected override void InitialiseDropdowns() {
      _spacerDescriptions = new List<string>(new[] {
        "Max/Min",
        "Unit",
      });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      _dropDownItems.Add(ExtremaHelper.AssemblyDrifts.ToList());
      _selectedItems.Add(_dropDownItems[0][0]);

      _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
      _selectedItems.Add(Length.GetAbbreviation(_lengthUnit));

      _isInitialised = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaResultParameter(), "Result", "Res", "GSA Result", GH_ParamAccess.list);
      pManager.AddParameter(new GsaAssemblyListParameter());
      pManager[1].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      string unitAbbreviation = Length.GetAbbreviation(_lengthUnit);
      string note = ResultNotes.NoteAssemblyResults;

      pManager.AddGenericParameter("Drift X [" + unitAbbreviation + "]", "Dx",
        "Drift in Local Assembly X-direction" + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Drift Y [" + unitAbbreviation + "]", "Dy",
        "Drift in Local Assembly Y-direction" + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Drift XY [" + unitAbbreviation + "]", "In-plane",
        "Drift in Local Assembly XY-plane" + note, GH_ParamAccess.tree);
    }

    protected override void SolveInternal(IGH_DataAccess da) {
      GsaResult result;
      string assemblylist = "All";

      var ghTypes = new List<GH_ObjectWrapper>();
      da.GetDataList(0, ghTypes);

      var outTransX = new DataTree<GH_UnitNumber>();
      var outTransY = new DataTree<GH_UnitNumber>();
      var outTransXy = new DataTree<GH_UnitNumber>();

      foreach (GH_ObjectWrapper ghTyp in ghTypes) {
        result = Inputs.GetResultInput(this, ghTyp);
        if (result == null) {
          return;
        }

        assemblylist = Inputs.GetAssemblyListDefinition(this, da, 1, result.Model);

        ReadOnlyCollection<int> assemblyIds = result.AssemblyIds(assemblylist);
        Parameters.Results.AssemblyDrifts resultSet = result.AssemblyDrifts.ResultSubset(assemblyIds);

        List<int> permutations = result.SelectedPermutationIds ?? new List<int>() {
          1,
        };
        if (permutations.Count == 1 && permutations[0] == -1) {
          permutations = Enumerable.Range(1, resultSet.Subset.Values.First().Count).ToList();
        }

        if (_selectedItems[0] == ExtremaHelper.Vector6Displacements[0]) {
          foreach (KeyValuePair<int, IList<IEntity1dQuantity<Drift>>> kvp in resultSet.Subset) {
            foreach (int p in permutations) {
              var path = new GH_Path(result.CaseId, result.SelectedPermutationIds == null ? 0 : p, kvp.Key);
              outTransX.AddRange(kvp.Value[p - 1].Results.Values.Select(
                r => new GH_UnitNumber(r.X.ToUnit(_lengthUnit))), path);
              outTransY.AddRange(kvp.Value[p - 1].Results.Values.Select(
                r => new GH_UnitNumber(r.Y.ToUnit(_lengthUnit))), path);
              outTransXy.AddRange(kvp.Value[p - 1].Results.Values.Select(
                r => new GH_UnitNumber(r.Xy.ToUnit(_lengthUnit))), path);
            }
          }
        } else {
          Entity1dExtremaKey key = ExtremaHelper.AssemblyDriftsExtremaKey(resultSet, _selectedItems[0]);
          IDrift<Length> extrema = resultSet.GetExtrema(key);
          int perm = result.CaseType == CaseType.AnalysisCase ? 0 : 1;
          var path = new GH_Path(result.CaseId, key.Permutation + perm, key.Id);
          outTransX.Add(new GH_UnitNumber(extrema.X.ToUnit(_lengthUnit)), path);
          outTransY.Add(new GH_UnitNumber(extrema.Y.ToUnit(_lengthUnit)), path);
          outTransXy.Add(new GH_UnitNumber(extrema.Xy.ToUnit(_lengthUnit)), path);
        }

        PostHog.Result(result.CaseType, 1, "AssemblyDrift");
      }

      da.SetDataTree(0, outTransX);
      da.SetDataTree(1, outTransY);
      da.SetDataTree(2, outTransXy);
    }

    protected override void UpdateUIFromSelectedItems() {
      _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[1]);
      base.UpdateUIFromSelectedItems();
    }
  }
}
