using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;

using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;

using GsaAPI;

using GsaGH.Helpers.GH;
using GsaGH.Helpers.GsaApi;
using GsaGH.Parameters;
using GsaGH.Parameters.Results;
using GsaGH.Properties;

using OasysGH;
using OasysGH.Components;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to select results from a GSA Model
  /// </summary>
  public class SelectResult : GH_OasysDropDownComponent {
    public override Guid ComponentGuid => new Guid("c803bba4-a026-4f95-b588-9d76455a53fa");
    public override GH_Exposure Exposure => GH_Exposure.primary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.SelectResult;
    private readonly List<string> _type = new List<string>(new[] {
      "AnalysisCase",
      "Combination",
    });
    private ReadOnlyDictionary<int, AnalysisCaseResult> _analysisCaseResults;
    private int _caseId = 1;
    private ReadOnlyDictionary<int, CombinationCaseResult> _combinationCaseResults;
    private GsaModel _gsaModel;
    private List<int> _permutationIDs = new List<int>() {
      -1,
    };
    private Dictionary<Tuple<CaseType, int>, GsaResult> _resultCache;
    private CaseType _resultType = CaseType.AnalysisCase;

    public SelectResult() : base("Select Result", "SelRes",
      "Select AnalysisCase or Combination Result from an analysed GSA model", CategoryName.Name(),
      SubCategoryName.Cat5()) {
      Hidden = true;
    }

    public override void SetSelected(int i, int j) {
      _selectedItems[i] = _dropDownItems[i][j];

      switch (i) {
        case 0 when _selectedItems[i] == _type[0]: {
            if (_resultType == CaseType.AnalysisCase) {
              return;
            }

            _resultType = CaseType.AnalysisCase;
            UpdateDropdowns();
            break;
          }
        case 0: {
            if (_selectedItems[i] == _type[1]) {
              if (_resultType == CaseType.CombinationCase) {
                return;
              }

              _resultType = CaseType.CombinationCase;
              UpdateDropdowns();
            }

            break;
          }
        case 1: {
            int newId = int.Parse(
              string.Join(string.Empty, _selectedItems[i].ToCharArray().Where(char.IsDigit)));
            if (newId != _caseId) {
              _caseId = newId;
              if (_resultType == CaseType.CombinationCase) {
                UpdatePermutations();
              }
            }

            break;
          }
        case 2 when _selectedItems[i].ToLower() != "all":
          _permutationIDs = new List<int>() {
            int.Parse(string.Join(string.Empty, _selectedItems[i].ToCharArray().Where(char.IsDigit))),
          };
          break;

        case 2:
          _permutationIDs = new List<int>() {
            -1,
          };
          break;
      }

      base.UpdateUI();
    }

    protected override void InitialiseDropdowns() {
      _spacerDescriptions = new List<string>(new[] {
        "Type",
        "Case ID",
        "Permutation",
      });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      _dropDownItems.Add(_type);
      _selectedItems.Add(_dropDownItems[0][0]);

      _dropDownItems.Add(new List<string>() {
        "   ",
      });
      _selectedItems.Add("   ");

      _isInitialised = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaModelParameter(), "GSA Model", "GSA",
        "GSA model containing some results", GH_ParamAccess.item);
      pManager.AddTextParameter("Result Type", "T",
        "Result type. " + Environment.NewLine + "Accepted inputs are: " + Environment.NewLine
        + "'AnalysisCase' or 'Combination'", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Case", "ID", "Case ID(s)", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Permutation", "P",
        "Permutations (only applicable for combination cases).", GH_ParamAccess.list);
      pManager[1].Optional = true;
      pManager[2].Optional = true;
      pManager[3].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaResultParameter(), "Result", "Res", "GSA Result",
        GH_ParamAccess.item);
    }

    protected override void SolveInternal(IGH_DataAccess da) {
      var ghTyp = new GH_ObjectWrapper();
      da.GetData(0, ref ghTyp);

      if (ghTyp.Value is GsaModelGoo modelGoo) {
        if (_gsaModel != null) {
          if (modelGoo.Value.Guid != _gsaModel.Guid) {
            _gsaModel = modelGoo.Value;
            UpdateDropdowns();
            _resultCache = new Dictionary<Tuple<CaseType, int>, GsaResult>();
            _analysisCaseResults = null;
            _combinationCaseResults = null;
          }
        } else {
          _gsaModel = modelGoo.Value;
          UpdateDropdowns();
          _resultCache = new Dictionary<Tuple<CaseType, int>, GsaResult>();
        }
      } else {
        this.AddRuntimeError("Error converting input to GSA Model");
        return;
      }

      var ghType = new GH_String();
      if (da.GetData(1, ref ghType)) {
        if (GH_Convert.ToString(ghType, out string type, GH_Conversion.Both)) {
          if (type.ToUpper().StartsWith("A")) {
            _selectedItems[0] = _dropDownItems[0][0];
            if (_resultType != CaseType.AnalysisCase) {
              _resultType = CaseType.AnalysisCase;
              if (_dropDownItems.Count > 2) {
                _dropDownItems.RemoveAt(2);
                if (_selectedItems.Count > 2) {
                  _selectedItems.RemoveAt(2);
                }
              }

              if (_selectedItems[1] != "   ") {
                if (_selectedItems[1] != "All") {
                  _selectedItems[1] = "A" + _caseId.ToString();
                } else {
                  UpdateDropdowns();
                }
              }
            }
          } else if (type.ToUpper().StartsWith("C")) {
            _selectedItems[0] = _dropDownItems[0][1];
            if (_resultType != CaseType.CombinationCase) {
              _resultType = CaseType.CombinationCase;
              if (_dropDownItems.Count < 3) {
                _dropDownItems.Add(new List<string>() {
                  "All",
                });
                if (_selectedItems.Count < 3) {
                  _selectedItems.Add("All");
                }
              }

              if (_selectedItems[1] != "   ") {
                if (_selectedItems[1] != "All") {
                  _selectedItems[1] = "C" + _caseId.ToString();
                } else {
                  UpdateDropdowns();
                }
              }
            }
          }
        }
      }

      var ghACase = new GH_Integer();
      if (da.GetData(2, ref ghACase)) {
        if (GH_Convert.ToInt32(ghACase, out int analCase, GH_Conversion.Both)) {
          if (_resultType == CaseType.CombinationCase && _caseId != analCase) {
            UpdatePermutations();
          }

          _caseId = analCase;
          if (analCase < 1) {
            _selectedItems[1] = "All";
          } else {
            _selectedItems[1] = (_resultType == CaseType.AnalysisCase) ? "A" + analCase :
              "C" + analCase;
          }
        }
      }

      if (_resultType != CaseType.AnalysisCase) {
        var ghPermutations = new List<int>();
        if (da.GetDataList(3, ghPermutations)) {
          UpdatePermutations();
          _permutationIDs = ghPermutations;
          if (_permutationIDs.Count == 1) {
            if (_permutationIDs[0] < 1) {
              _selectedItems[2] = "All";
            } else {
              _selectedItems[2] = "P" + _permutationIDs[0];
            }
          } else {
            _selectedItems[2] = "from input";
          }
        }
      }

      var key = new Tuple<CaseType, int>(_resultType, _caseId);
      switch (_resultType) {
        case CaseType.AnalysisCase:
          if (_analysisCaseResults == null) {
            _analysisCaseResults = _gsaModel.ApiModel.Results();
            if (_analysisCaseResults == null || _analysisCaseResults.Count == 0) {
              this.AddRuntimeError("No Analysis Case Results exist in Model");
              return;
            }
          }

          if (!_analysisCaseResults.ContainsKey(_caseId)) {
            this.AddRuntimeError("Analysis Case A" + _caseId + " does not exist in model");
            return;
          }

          if (!_resultCache.ContainsKey(key)) {
            var result = new GsaResult(_gsaModel, _analysisCaseResults[_caseId], _caseId);
            _resultCache.Add(key, result);
          }

          break;

        case CaseType.CombinationCase:
          if (_combinationCaseResults == null) {
            _combinationCaseResults = _gsaModel.ApiModel.CombinationCaseResults();
            if (_combinationCaseResults == null || _combinationCaseResults.Count == 0) {
              this.AddRuntimeError("No Combination Case Results exist in Model");
              return;
            }
          }

          if (!_combinationCaseResults.ContainsKey(_caseId)) {
            this.AddRuntimeError("Combination Case does not exist in model");
            return;
          }

          ReadOnlyDictionary<int, ReadOnlyCollection<Double6>> tempNodeCombResult
            = _combinationCaseResults[_caseId]
             .NodeDisplacement(_gsaModel.ApiModel.Nodes().Keys.First().ToString());
          int nP = tempNodeCombResult[tempNodeCombResult.Keys.First()].Count;
          if (_permutationIDs.Count == 1 && _permutationIDs[0] == -1) {
            _permutationIDs = Enumerable.Range(1, nP).ToList();
          } else {
            if (_permutationIDs.Count == 0) {
              this.AddRuntimeWarning("Combination Case C" + _caseId + " does not contain results");
              return;
            }

            if (_permutationIDs.Max() > nP) {
              this.AddRuntimeError("Combination Case C" + _caseId + " only contains " + nP
                + " permutations but the highest permutation in input is " + _permutationIDs.Max());
              return;
            }
          }

          if (!_resultCache.ContainsKey(key)) {
            var result = new GsaResult(_gsaModel, _combinationCaseResults[_caseId], _caseId,
              _permutationIDs);
            _resultCache.Add(key, result);
          } else {
            _resultCache[key].SelectedPermutationIds = _permutationIDs;
          }

          break;
      }

      GsaResult item = _resultCache[key];
      da.SetData(0, new GsaResultGoo(item));
    }

    protected override void UpdateUIFromSelectedItems() {
      if (_selectedItems[0] == _type[0]) {
        _resultType = CaseType.AnalysisCase;
      } else if (_selectedItems[0] == _type[1]) {
        _resultType = CaseType.CombinationCase;
      }

      if (_selectedItems[1].ToLower() == "all") {
        _caseId = -1;
      } else {
        int newId = int.Parse(string.Join(string.Empty, _selectedItems[1].ToCharArray().Where(char.IsDigit)));
        if (newId != _caseId) {
          _caseId = newId;
        }
      }

      if (_selectedItems.Count > 2) {
        _permutationIDs = _selectedItems[2].ToLower() == "all" ? new List<int>() {
          -1,
        } : new List<int>() {
          int.Parse(string.Join(string.Empty, _selectedItems[2].ToCharArray().Where(char.IsDigit))),
        };
      }

      base.UpdateUIFromSelectedItems();
    }

    private void UpdateDropdowns() {
      if (_gsaModel == null) {
        return;
      }

      Tuple<List<string>, List<int>, DataTree<int?>> modelResults
        = ResultHelper.GetAvalailableResults(_gsaModel);
      string type = (_resultType == CaseType.AnalysisCase) ? "Analysis" : "Combination";

      var cases = new List<string>();
      for (int i = 0; i < modelResults.Item1.Count; i++) {
        if (modelResults.Item1[i] != type) {
          continue;
        }

        cases.Add(type[0] + modelResults.Item2[i].ToString());
      }

      _dropDownItems[1] = cases;
      _selectedItems[1] = type[0] + _caseId.ToString();

      if (_resultType == CaseType.CombinationCase) {
        if (_dropDownItems.Count < 3) {
          _dropDownItems.Add(new List<string>() {
            "All",
          });
          _selectedItems.Add("All");
        } else if (
          _dropDownItems[2].Count - 1 == modelResults.Item3.Branch(new GH_Path(_caseId)).Count) {
          return;
        }

        UpdatePermutations();

      } else if (_dropDownItems.Count > 2) {
        _dropDownItems.RemoveAt(2);
        _selectedItems.RemoveAt(2);
      }
    }

    private void UpdatePermutations() {
      if (_resultType != CaseType.CombinationCase) {
        return;
      }

      _combinationCaseResults ??= _gsaModel.ApiModel.CombinationCaseResults();

      if (_combinationCaseResults.Count == 0) {
        return;
      }

      if (!_combinationCaseResults.ContainsKey(_caseId)) {
        _caseId = _combinationCaseResults.First().Key;
        _selectedItems[1] = $"C{_caseId}";
      }

      ReadOnlyDictionary<int, ReadOnlyCollection<Double6>> tempNodeCombResult
        = _combinationCaseResults[_caseId]
         .NodeDisplacement(_gsaModel.ApiModel.Nodes().Keys.First().ToString());
      int nP = tempNodeCombResult[tempNodeCombResult.Keys.First()].Count;
      var permutationsInCase = Enumerable.Range(1, nP).ToList();
      if (_dropDownItems.Count < 3) {
        _dropDownItems.Add(new List<string>());
      }

      _dropDownItems[2] = new List<string> {
        "All",
      };
      if (_selectedItems.Count < 3) {
        _selectedItems.Add(string.Empty);
      }

      _selectedItems[2] = "All";
      _dropDownItems[2].AddRange(permutationsInCase.Select(x => $"P{x}").ToList());
      _permutationIDs = new List<int>() {
        -1,
      };
    }
  }
}
