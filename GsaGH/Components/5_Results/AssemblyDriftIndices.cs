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

namespace GsaGH.Components {
  /// <summary>
  ///   Component to get GSA assembly drift index values
  /// </summary>
  public class AssemblyDriftIndices : GH_OasysDropDownComponent {
    public override Guid ComponentGuid => new Guid("daf4ab65-cad8-4c8f-9a34-7897c260e6d5");
    public override GH_Exposure Exposure => GH_Exposure.septenary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.AssemblyDriftIndices;

    public AssemblyDriftIndices() : base("Assembly Drift Indices", "AssemblyDriftIndices",
      "Assembly Drift Index result values", CategoryName.Name(), SubCategoryName.Cat5()) {
      Hidden = true;
    }

    public override void SetSelected(int i, int j) {
      _selectedItems[i] = _dropDownItems[i][j];
      base.UpdateUI();
    }

    public override void VariableParameterMaintenance() {
      int i = 0;
      Params.Output[i++].Name = "Drift Index X";
      Params.Output[i++].Name = "Drift Index Y";
      Params.Output[i].Name = "Drift Index XY";
    }

    protected override void InitialiseDropdowns() {
      _spacerDescriptions = new List<string>(new[] {
        "Max/Min"
      });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      _dropDownItems.Add(ExtremaHelper.AssemblyDriftIndices.ToList());
      _selectedItems.Add(_dropDownItems[0][0]);

      _isInitialised = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaResultParameter(), "Result", "Res", "GSA Result", GH_ParamAccess.list);
      pManager.AddParameter(new GsaAssemblyListParameter());
      pManager[1].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      string note = ResultNotes.NoteAssemblyResults;

      pManager.AddGenericParameter("Drift Index X", "DIx",
        "Drift Index in Local Assembly X-direction" + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Drift Index Y", "DIy",
        "Drift Index in Local Assembly Y-direction" + note, GH_ParamAccess.tree);
      pManager.AddGenericParameter("Drift Index XY", "In-plane",
        "Drift Index in Local Assembly XY-plane" + note, GH_ParamAccess.tree);
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
        Parameters.Results.AssemblyDriftIndices resultSet = result.AssemblyDriftIndices.ResultSubset(assemblyIds);

        List<int> permutations = result.SelectedPermutationIds ?? new List<int>() {
          1,
        };
        if (permutations.Count == 1 && permutations[0] == -1) {
          permutations = Enumerable.Range(1, resultSet.Subset.Values.First().Count).ToList();
        }

        if (_selectedItems[0] == ExtremaHelper.Vector6Displacements[0]) {
          foreach (KeyValuePair<int, IList<IEntity1dQuantity<DriftIndex>>> kvp in resultSet.Subset) {
            foreach (int p in permutations) {
              var path = new GH_Path(result.CaseId, result.SelectedPermutationIds == null ? 0 : p, kvp.Key);
              outTransX.AddRange(kvp.Value[p - 1].Results.Values.Select(r => new GH_UnitNumber(r.X)), path);
              outTransY.AddRange(kvp.Value[p - 1].Results.Values.Select(r => new GH_UnitNumber(r.Y)), path);
              outTransXy.AddRange(kvp.Value[p - 1].Results.Values.Select(r => new GH_UnitNumber(r.Xy)), path);
            }
          }
        } else {
          Entity1dExtremaKey key = ExtremaHelper.AssemblyDriftIndicesExtremaKey(resultSet, _selectedItems[0]);
          DriftIndex extrema = resultSet.GetExtrema(key);
          int perm = result.CaseType == CaseType.AnalysisCase ? 0 : 1;
          var path = new GH_Path(result.CaseId, key.Permutation + perm, key.Id);
          outTransX.Add(new GH_UnitNumber(extrema.X), path);
          outTransY.Add(new GH_UnitNumber(extrema.Y), path);
          outTransXy.Add(new GH_UnitNumber(extrema.Xy), path);
        }

        PostHog.Result(result.CaseType, 1, "AssemblyDriftIndex");
      }

      da.SetDataTree(0, outTransX);
      da.SetDataTree(1, outTransY);
      da.SetDataTree(2, outTransXy);
    }
  }
}
