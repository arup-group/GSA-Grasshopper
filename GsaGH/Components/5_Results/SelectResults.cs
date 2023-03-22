using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GsaGH.Helpers.GH;
using GsaGH.Helpers.GsaAPI;
using GsaGH.Parameters;
using OasysGH;
using OasysGH.Components;
using OasysUnits;

namespace GsaGH.Components {
  /// <summary>
  /// Component to select results from a GSA Model
  /// </summary>
  public class SelectResult : GH_OasysDropDownComponent {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("c803bba4-a026-4f95-b588-9d76455a53fa");
    public override GH_Exposure Exposure => GH_Exposure.primary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Properties.Resources.SelectResult;

    private GsaResult.CaseType _resultType = GsaResult.CaseType.AnalysisCase;
    private int _caseId = 1;
    private List<int> _permutationIDs = new List<int>() { -1 };
    private GsaModel _gsaModel;
    private Dictionary<Tuple<GsaResult.CaseType, int>, GsaResult> _resultCache; // this is the cache object!
    private ReadOnlyDictionary<int, AnalysisCaseResult> _analysisCaseResults;
    private ReadOnlyDictionary<int, CombinationCaseResult> _combinationCaseResults;

    public SelectResult() : base("Select Results",
      "SelRes",
      "Select AnalysisCase or Combination Result from an analysed GSA model",
      CategoryName.Name(),
      SubCategoryName.Cat5()) {
      Hidden = true;
    }
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaModelParameter(), "GSA Model", "GSA", "GSA model containing some results", GH_ParamAccess.item);
      pManager.AddTextParameter("Result Type", "T", "Result type. " +
          Environment.NewLine + "Accepted inputs are: " +
          Environment.NewLine + "'AnalysisCase' or 'Combination'", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Case", "ID", "Case ID(s)", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Permutation", "P", "Permutations (only applicable for combination cases).", GH_ParamAccess.list);
      pManager[1].Optional = true;
      pManager[2].Optional = true;
      pManager[3].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaResultsParameter(), "Result", "Res", "GSA Result", GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess da) {
      var inModel = new GsaModel();
      var ghTyp = new GH_ObjectWrapper();
      if (!da.GetData(0, ref ghTyp)) {
        return;
      }

      if (ghTyp.Value is GsaModelGoo) {
        ghTyp.CastTo(ref inModel);
        if (_gsaModel != null) {
          if (inModel.Guid != _gsaModel.Guid) {
            _gsaModel = inModel;
            UpdateDropdowns();
            _resultCache = new Dictionary<Tuple<GsaResult.CaseType, int>, GsaResult>();
            _analysisCaseResults = null;
            _combinationCaseResults = null;
          }
        }
        else {
          _gsaModel = inModel;
          UpdateDropdowns();
          _resultCache = new Dictionary<Tuple<GsaResult.CaseType, int>, GsaResult>();
        }
      }
      else {
        this.AddRuntimeError("Error converting input to GSA Model");
        return;
      }

      var ghType = new GH_String();
      if (da.GetData(1, ref ghType)) {
        if (GH_Convert.ToString(ghType, out string type, GH_Conversion.Both)) {
          if (type.ToUpper().StartsWith("A")) {
            _resultType = GsaResult.CaseType.AnalysisCase;
            SelectedItems[0] = DropDownItems[0][0];
            if (_resultType != GsaResult.CaseType.AnalysisCase) {
              _resultType = GsaResult.CaseType.AnalysisCase;
              if (DropDownItems.Count > 2) {
                DropDownItems.RemoveAt(2);
                if (SelectedItems.Count > 2)
                  SelectedItems.RemoveAt(2);
              }
              if (SelectedItems[1] != "   ") {
                if (SelectedItems[1] != "All")
                  SelectedItems[1] = "A" + _caseId.ToString();
                else
                  UpdateDropdowns();
              }
            }
          }
          else if (type.ToUpper().StartsWith("C")) {
            SelectedItems[0] = DropDownItems[0][1];
            if (_resultType != GsaResult.CaseType.Combination) {
              _resultType = GsaResult.CaseType.Combination;
              if (DropDownItems.Count < 3) {
                DropDownItems.Add(new List<string>() { "All" });
                if (SelectedItems.Count < 3)
                  SelectedItems.Add("All");
              }
              if (SelectedItems[1] != "   ") {
                if (SelectedItems[1] != "All")
                  SelectedItems[1] = "C" + _caseId.ToString();
                else
                  UpdateDropdowns();
              }
            }
          }
        }
      }

      var ghACase = new GH_Integer();
      if (da.GetData(2, ref ghACase)) {
        if (GH_Convert.ToInt32(ghACase, out int analCase, GH_Conversion.Both)) {
          if (_resultType == GsaResult.CaseType.Combination && _caseId != analCase)
            UpdatePermutations();
          _caseId = analCase;
          if (analCase < 1)
            SelectedItems[1] = "All";
          else
            SelectedItems[1] = (_resultType == GsaResult.CaseType.AnalysisCase) ? "A" + analCase : "C" + analCase;
        }
      }

      if (_resultType != GsaResult.CaseType.AnalysisCase) {
        var ghPermutations = new List<int>();
        if (da.GetDataList(3, ghPermutations)) {
          UpdatePermutations();
          _permutationIDs = ghPermutations;
          if (_permutationIDs.Count == 1) {
            if (_permutationIDs[0] < 1)
              SelectedItems[2] = "All";
            else
              SelectedItems[2] = "P" + _permutationIDs[0];
          }
          else
            SelectedItems[2] = "from input";
        }
      }

      switch (_resultType) {
        case GsaResult.CaseType.AnalysisCase:
          if (_analysisCaseResults == null) {
            _analysisCaseResults = _gsaModel.Model.Results();
            if (_analysisCaseResults == null || _analysisCaseResults.Count == 0) {
              this.AddRuntimeError("No Analysis Case Results exist in Model");
              return;
            }
          }

          if (!_analysisCaseResults.ContainsKey(_caseId)) {
            this.AddRuntimeError("Analysis Case A" + _caseId + " does not exist in model");
            return;
          }

          if (!_resultCache.ContainsKey(new Tuple<GsaResult.CaseType, int>(GsaResult.CaseType.AnalysisCase, _caseId))) {
            _resultCache.Add(new Tuple<GsaResult.CaseType, int>(GsaResult.CaseType.AnalysisCase, _caseId),
              new GsaResult(_gsaModel, _analysisCaseResults[_caseId], _caseId));
          }
          break;

        case GsaResult.CaseType.Combination:
          if (_combinationCaseResults == null) {
            _combinationCaseResults = _gsaModel.Model.CombinationCaseResults();
            if (_combinationCaseResults == null || _combinationCaseResults.Count == 0) {
              this.AddRuntimeError("No Combination Case Results exist in Model");
              return;
            }
          }

          if (!_combinationCaseResults.ContainsKey(_caseId)) {
            this.AddRuntimeError("Combination Case does not exist in model");
            return;
          }

          IReadOnlyDictionary<int, ReadOnlyCollection<NodeResult>> tempNodeCombResult = _combinationCaseResults[_caseId].NodeResults(_gsaModel.Model.Nodes().Keys.First().ToString());
          int nP = tempNodeCombResult[tempNodeCombResult.Keys.First()].Count;
          if (_permutationIDs.Count == 1 && _permutationIDs[0] == -1)
            _permutationIDs = Enumerable.Range(1, nP).ToList();
          else {
            if (_permutationIDs.Count == 0) {
              this.AddRuntimeWarning("Combination Case C" + _caseId + " does not contain results");
              return;
            }
            if (_permutationIDs.Max() > nP) {
              this.AddRuntimeError("Combination Case C" + _caseId + " only contains " + nP + " permutations but the highest permutation in input is " + _permutationIDs.Max());
              return;
            }
          }

          if (!_resultCache.ContainsKey(new Tuple<GsaResult.CaseType, int>(GsaResult.CaseType.Combination, _caseId))) {
            _resultCache.Add(new Tuple<GsaResult.CaseType, int>(GsaResult.CaseType.Combination, _caseId),
              new GsaResult(_gsaModel, _combinationCaseResults[_caseId], _caseId, _permutationIDs));
          }
          else {
            _resultCache[new Tuple<GsaResult.CaseType, int>(_resultType, _caseId)].SelectedPermutationIds = _permutationIDs;
          }
          break;
      }

      da.SetData(0, new GsaResultGoo(_resultCache[new Tuple<GsaResult.CaseType, int>(_resultType, _caseId)]));
    }

    #region Custom UI
    private readonly List<string> _type = new List<string>(new[]
    {
      "AnalysisCase",
      "Combination",
    });

    private void UpdateDropdowns() {
      if (_gsaModel == null)
        return;

      Tuple<List<string>, List<int>, DataTree<int?>> modelResults = ResultHelper.GetAvalailableResults(_gsaModel);
      string type = (_resultType == GsaResult.CaseType.AnalysisCase)
        ? "Analysis"
        : "Combination";

      var cases = new List<string>();
      for (int i = 0; i < modelResults.Item1.Count; i++) {
        if (modelResults.Item1[i] != type)
          continue;
        cases.Add(type[0] + modelResults.Item2[i].ToString());
      }
      DropDownItems[1] = cases;
      SelectedItems[1] = type[0] + _caseId.ToString();

      if (_resultType == GsaResult.CaseType.Combination) {
        if (DropDownItems.Count < 3) {
          DropDownItems.Add(new List<string>() { "All" });
          SelectedItems.Add("All");
          UpdatePermutations();
        }
        else {
          DropDownItems[2] = new List<string>() { "All" };
          SelectedItems[2] = "All";
        }
        List<int?> ints = modelResults.Item3.Branch(new GH_Path(_caseId));
        DropDownItems[2].AddRange(ints.Select(x => "P" + x.ToString()).ToList());
      }
      else if (DropDownItems.Count > 2) {
        DropDownItems.RemoveAt(2);
        SelectedItems.RemoveAt(2);
      }
    }

    private void UpdatePermutations() {
      if (_resultType != GsaResult.CaseType.Combination) {
        return;
      }

      if (_combinationCaseResults == null)
        _combinationCaseResults = _gsaModel.Model.CombinationCaseResults();
      IReadOnlyDictionary<int, ReadOnlyCollection<NodeResult>> tempNodeCombResult = _combinationCaseResults[_caseId].NodeResults(_gsaModel.Model.Nodes().Keys.First().ToString());
      int nP = tempNodeCombResult[tempNodeCombResult.Keys.First()].Count;
      var permutationsInCase = Enumerable.Range(1, nP).ToList();
      if (DropDownItems.Count < 3)
        DropDownItems.Add(new List<string>());
      DropDownItems[2] = new List<string> { "All" };
      if (SelectedItems.Count < 3)
        SelectedItems.Add("");
      SelectedItems[2] = "All";
      DropDownItems[2].AddRange(permutationsInCase.Select(x => "P" + x.ToString()).ToList());
      _permutationIDs = new List<int>() { -1 };
    }

    public override void InitialiseDropdowns() {
      SpacerDescriptions = new List<string>(new[]
        {
          "Type",
          "Case ID",
          "Permutation",
        });

      DropDownItems = new List<List<string>>();
      SelectedItems = new List<string>();

      DropDownItems.Add(_type);
      SelectedItems.Add(DropDownItems[0][0]);

      DropDownItems.Add(new List<string>() { "   " });
      SelectedItems.Add("   ");

      IsInitialised = true;
    }

    public override void SetSelected(int i, int j) {
      SelectedItems[i] = DropDownItems[i][j];

      switch (i) {
        case 0 when SelectedItems[i] == _type[0]: {
            if (_resultType == GsaResult.CaseType.AnalysisCase)
              return;
            _resultType = GsaResult.CaseType.AnalysisCase;
            UpdateDropdowns();
            break;
          }
        case 0: {
            if (SelectedItems[i] == _type[1]) {
              if (_resultType == GsaResult.CaseType.Combination)
                return;
              _resultType = GsaResult.CaseType.Combination;
              UpdateDropdowns();
            }

            break;
          }
        case 1 when SelectedItems[i].ToLower() == "all":
          _caseId = -1;
          break;
        case 1: {
            int newId = int.Parse(string.Join("", SelectedItems[i].ToCharArray().Where(char.IsDigit)));
            if (newId != _caseId) {
              _caseId = newId;
              if (_resultType == GsaResult.CaseType.Combination)
                UpdatePermutations();
            }

            break;
          }
        case 2 when SelectedItems[i].ToLower() != "all":
          _permutationIDs = new List<int>() { int.Parse(string.Join("", SelectedItems[i].ToCharArray().Where(char.IsDigit))) };
          break;
        case 2:
          _permutationIDs = new List<int>() { -1 };
          break;
      }

      base.UpdateUI();
    }
    public override void UpdateUIFromSelectedItems() {
      if (SelectedItems[0] == _type[0])
        _resultType = GsaResult.CaseType.AnalysisCase;
      else if (SelectedItems[0] == _type[1])
        _resultType = GsaResult.CaseType.Combination;

      if (SelectedItems[1].ToLower() == "all")
        _caseId = -1;
      else {
        int newId = int.Parse(string.Join("", SelectedItems[1].ToCharArray().Where(char.IsDigit)));
        if (newId != _caseId)
          _caseId = newId;
      }

      if (SelectedItems.Count > 2) {
        _permutationIDs = SelectedItems[2].ToLower() == "all"
          ? new List<int>() { -1 }
          : new List<int>() { int.Parse(string.Join("", SelectedItems[2]
            .ToCharArray()
            .Where(char.IsDigit))) };
      }

      base.UpdateUIFromSelectedItems();
    }
    #endregion
  }
}
