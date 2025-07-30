using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using GsaAPI;

using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Parameters.Results;
using GsaGH.Properties;

using OasysGH;
using OasysGH.Components;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to select results from a GSA Model .
  /// </summary>
  public class GetResult : GH_OasysComponent {
    public override Guid ComponentGuid => new Guid("799e1ac7-a310-4a65-a737-f5f5d0077879");
    public override GH_Exposure Exposure => GH_Exposure.primary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.GetResult;
    private ReadOnlyDictionary<int, AnalysisCaseResult> _analysisCaseResults;
    private ReadOnlyDictionary<int, CombinationCaseResult> _combinationCaseResults;
    private Guid _modelGuid;
    private Dictionary<Tuple<CaseType, int>, GsaResult> _result;
    private int _tempNodeId;

    public GetResult() : base("Get Result", "GetRes",
      "Get AnalysisCase or Combination Result from an analysed GSA model", CategoryName.Name(),
      SubCategoryName.Cat5()) {
      Hidden = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaModelParameter(), "GSA Model", "GSA",
        "GSA model containing some results", GH_ParamAccess.item);
      pManager.AddTextParameter("Result Type", "T",
        "Result type. " + Environment.NewLine + "Accepted inputs are: " + Environment.NewLine
        + "'AnalysisCase' or 'Combination'", GH_ParamAccess.item, "A");
      pManager.AddIntegerParameter("Case", "ID", "Case ID(s)", GH_ParamAccess.item, 1);
      pManager.AddIntegerParameter("Permutation", "P",
        "Permutations (only applicable for combination cases).", GH_ParamAccess.list);
      pManager[3].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaResultParameter(), "Result", "Res", "GSA Result",
        GH_ParamAccess.item);
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      var model = new GsaModel();
      var ghTyp = new GH_ObjectWrapper();
      if (!da.GetData(0, ref ghTyp)) {
        return;
      }

      if (ghTyp.Value is GsaModelGoo modelGoo) {
        if (_modelGuid == new Guid()) {
          if (modelGoo.Value.Guid != _modelGuid) {
            model = modelGoo.Value;
            _result = new Dictionary<Tuple<CaseType, int>, GsaResult>();
            _analysisCaseResults = null;
            _combinationCaseResults = null;
          }
        } else {
          model = modelGoo.Value;
          _modelGuid = model.Guid;
          _result = new Dictionary<Tuple<CaseType, int>, GsaResult>();
        }
      } else {
        this.AddRuntimeError("Error converting input " + Params.Input[0].NickName
          + " to GSA Model");
        return;
      }

      CaseType resultType = CaseType.AnalysisCase;
      var ghType = new GH_String();
      if (da.GetData(1, ref ghType)) {
        if (GH_Convert.ToString(ghType, out string type, GH_Conversion.Both)) {
          if (type.ToUpper().StartsWith("A")) {
            resultType = CaseType.AnalysisCase;
          } else if (type.ToUpper().StartsWith("C")) {
            resultType = CaseType.CombinationCase;
          } else {
            this.AddRuntimeError("Error converting input " + Params.Input[1].NickName
              + " to 'Analysis' or 'Combination'");
            return;
          }
        }
      }

      int caseId = 1;
      var aCase = new GH_Integer();
      if (da.GetData(2, ref aCase)) {
        if (GH_Convert.ToInt32(aCase, out int analCase, GH_Conversion.Both)) {
          caseId = analCase;
        }

        if (caseId < 1) {
          this.AddRuntimeError("Input " + Params.Input[2].NickName + " must be above 0");
          return;
        }
      }

      var permutationIDs = new List<int>();
      if (resultType != CaseType.AnalysisCase) {
        var ghPerms = new List<int>();
        if (da.GetDataList(3, ghPerms)) {
          permutationIDs = ghPerms;
        } else {
          this.AddRuntimeRemark("By default, all permutations have been selected.");
          permutationIDs = new List<int>() {
            -1,
          };
        }
      }

      if (Params.Input[1].SourceCount == 0 && Params.Input[2].SourceCount == 0) {
        this.AddRuntimeRemark("By default, Analysis Case 1 has been selected.");
      }

      switch (resultType) {
        case CaseType.AnalysisCase:
          if (_analysisCaseResults == null) {
            _analysisCaseResults = model.ApiModel.Results();
            if (_analysisCaseResults == null || _analysisCaseResults.Count == 0) {
              this.AddRuntimeError("No Analysis Case Results exist in Model");
              return;
            }
          }

          if (!_analysisCaseResults.ContainsKey(caseId)) {
            this.AddRuntimeError("Analysis Case does not exist in model");
            return;
          }

          if (!_result.ContainsKey(
            new Tuple<CaseType, int>(CaseType.AnalysisCase, caseId))) {
            _result.Add(new Tuple<CaseType, int>(CaseType.AnalysisCase, caseId),
              new GsaResult(model, _analysisCaseResults[caseId], caseId));
          }

          break;

        case CaseType.CombinationCase:
          if (_combinationCaseResults == null) {
            _combinationCaseResults = model.ApiModel.CombinationCaseResults();
            if (_combinationCaseResults == null || _combinationCaseResults.Count == 0) {
              this.AddRuntimeError("No Combination Case Results exist in Model");
              return;
            }
          }

          if (!_combinationCaseResults.ContainsKey(caseId)) {
            this.AddRuntimeError("Combination Case does not exist in model");
            return;
          }

          if (_tempNodeId == 0) {
            _tempNodeId = model.ApiModel.Nodes().Keys.First();
          }

          ReadOnlyDictionary<int, ReadOnlyCollection<Double6>> tempNodeCombResult
            = _combinationCaseResults[caseId].NodeDisplacement(_tempNodeId.ToString());
          int nP = tempNodeCombResult[tempNodeCombResult.Keys.First()].Count;
          var allPermutations = Enumerable.Range(1, nP).ToList();
          if (permutationIDs.Count == 1 && permutationIDs[0] == -1) {
            permutationIDs = allPermutations;
          } else {
            if (permutationIDs.Intersect(allPermutations).Count() != permutationIDs.Count()) {
              IEnumerable<int> missing = permutationIDs.Except(allPermutations);
              this.AddRuntimeError($"Combination Case C{caseId} does not contain permutation(s) " +
                string.Join(", ", missing));
              return;
            }
          }

          if (!_result.ContainsKey(
            new Tuple<CaseType, int>(CaseType.CombinationCase, caseId))) {
            _result.Add(new Tuple<CaseType, int>(CaseType.CombinationCase, caseId),
              new GsaResult(model, _combinationCaseResults[caseId], caseId, permutationIDs));
          }

          break;
      }

      da.SetData(0,
        new GsaResultGoo(_result[new Tuple<CaseType, int>(resultType, caseId)]));
    }

    // this is the cache object!
  }
}
