using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GsaGH.Parameters;
using GsaGH.Util.GH;
using OasysGH;
using OasysGH.Components;
using OasysGH.Units.Helpers;
using OasysUnits;
using OasysUnits.Units;

namespace GsaGH.Components
{
  /// <summary>
  /// Component to select results from a GSA Model
  /// </summary>
  public class SelectResult : GH_OasysDropDownComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("c803bba4-a026-4f95-b588-9d76455a53fa");
    public override GH_Exposure Exposure => GH_Exposure.primary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.SelectResult;

    public SelectResult() : base("Select Results",
      "SelRes",
      "Select AnalysisCase or Combination Result from an analysed GSA model",
      Ribbon.CategoryName.Name(),
      Ribbon.SubCategoryName.Cat5())
    { this.Hidden = true; } // sets the initial state of the component to hidden
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddParameter(new GsaModelParameter(), "GSA Model", "GSA", "GSA model containing some results", GH_ParamAccess.item);
      pManager.AddTextParameter("Result Type", "rT", "Result type. " +
          System.Environment.NewLine + "Accepted inputs are: " +
          System.Environment.NewLine + "'AnalysisCase' or 'Combination'", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Case", "C", "Case ID(s)" +
          System.Environment.NewLine + "Use -1 for 'all'", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Permutation", "P", "Permutations (only applicable for combination cases). " +
          System.Environment.NewLine + "Leave blank for 'all'", GH_ParamAccess.list);
      pManager[1].Optional = true;
      pManager[2].Optional = true;
      pManager[3].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddParameter(new GsaResultsParameter(), "Result", "Res", "GSA Result", GH_ParamAccess.list);
    }
    #endregion

    int _caseID = 1;
    List<int> _permutations = new List<int>();
    Dictionary<Tuple<GsaResult.ResultType, int>, GsaResult> Result; // this is the cache object!
    GsaModel _gsaModel;
    ReadOnlyDictionary<int, AnalysisCaseResult> _analysisCaseResults;
    ReadOnlyDictionary<int, CombinationCaseResult> _combinationCaseResults;
    protected override void SolveInstance(IGH_DataAccess DA)
    {
      // Model to work on
      GsaModel in_Model = new GsaModel();

      // Get Model
      GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
      if (DA.GetData(0, ref gh_typ))
      {
        if (gh_typ.Value is GsaModelGoo)
        {
          gh_typ.CastTo(ref in_Model);
          if (_gsaModel != null)
          {
            if (in_Model.Guid != _gsaModel.Guid) // only get results if GUID is not similar
            {
              _gsaModel = in_Model;
              updateCases = true;
              ClearData();
              Result = new Dictionary<Tuple<GsaResult.ResultType, int>, GsaResult>();
              tempNodeID = _gsaModel.Model.Nodes().Keys.First();
            }
          }
          else
          {
            // first time
            _gsaModel = in_Model;
            updateCases = true;
            Result = new Dictionary<Tuple<GsaResult.ResultType, int>, GsaResult>();
            tempNodeID = _gsaModel.Model.Nodes().Keys.First();
          }
        }
        else
        {
          AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Error converting input to GSA Model");
          return;
        }

        // Get case type
        bool caseSetByInput = false;
        GH_String gh_Type = new GH_String();
        if (DA.GetData(1, ref gh_Type))
        {
          string type = "";
          if (GH_Convert.ToString(gh_Type, out type, GH_Conversion.Both))
          {
            if (type.ToUpper().StartsWith("A"))
            {
              ResultType = GsaResult.ResultType.AnalysisCase;
              SelectedItems[0] = DropDownItems[0][0];
              if (ResultType != GsaResult.ResultType.AnalysisCase)
              {
                ResultType = GsaResult.ResultType.AnalysisCase;
                if (DropDownItems.Count > 2)
                {
                  DropDownItems.RemoveAt(2);
                  if (SelectedItems.Count > 2)
                  {
                    SelectedItems.RemoveAt(2);
                  }
                }
                if (SelectedItems[1] != "   ")
                {
                  if (SelectedItems[1] != "All")
                  {
                    SelectedItems[1] = "A" + _caseID.ToString();
                  }
                  else
                    updateCases = true;
                }
              }
            }
            else if (type.ToUpper().StartsWith("C"))
            {
              SelectedItems[0] = DropDownItems[0][1];
              if (ResultType != GsaResult.ResultType.Combination)
              {
                ResultType = GsaResult.ResultType.Combination;
                if (DropDownItems.Count < 3)
                {
                  DropDownItems.Add(new List<string>() { "All" });
                  if (SelectedItems.Count < 3)
                  {
                    SelectedItems.Add("All");
                  }
                }
                if (SelectedItems[1] != "   ")
                {
                  if (SelectedItems[1] != "All")
                  {
                    SelectedItems[1] = "C" + _caseID.ToString();
                  }
                  else
                    updateCases = true;
                }
              }
            }
          }
          caseSetByInput = true;
        }

        // Get analysis case 
        GH_Integer gh_aCase = new GH_Integer();
        if (DA.GetData(2, ref gh_aCase))
        {
          int analCase = 1;
          if (GH_Convert.ToInt32(gh_aCase, out analCase, GH_Conversion.Both))
          {
            if (ResultType == GsaResult.ResultType.Combination && _caseID != analCase)
              updatePermutations = true;
            _caseID = analCase;
            if (analCase < 1)
              SelectedItems[1] = "All";
            else
              SelectedItems[1] = (ResultType == GsaResult.ResultType.AnalysisCase) ? "A" + analCase : "C" + analCase;

            updateCases = false;
            caseSetByInput = true;
          }
        }

        // Get permutation case 
        if (ResultType != GsaResult.ResultType.AnalysisCase)
        {
          List<int> gh_perms = new List<int>();
          if (DA.GetDataList(3, gh_perms))
          {
            _permutations = gh_perms;
            updatePermutations = false;
            if (_permutations.Count == 1)
            {
              if (_permutations[0] < 1)
                SelectedItems[2] = "All";
              else
                SelectedItems[2] = "P" + _permutations[0];
            }
            else
              SelectedItems[2] = "from input";
          }
        }

        AnalysisCaseResult analysisCaseResult;
        CombinationCaseResult combinationCaseResult;
        IReadOnlyDictionary<int, ReadOnlyCollection<NodeResult>> tempNodeResult;
        if (ResultType == GsaResult.ResultType.AnalysisCase)
        {
          if (_analysisCaseResults == null)
            updateCases = true;
        }
        else
        {
          if (_combinationCaseResults == null)
            updateCases = true;
        }
        if (_caseID < 0)
          updateCases = false;

        // skip 'reflection' if inputs have been set
        if (ResultType == GsaResult.ResultType.AnalysisCase && this.Params.Input[2].SourceCount > 0)
        {
          _analysisCaseResults = _gsaModel.Model.Results();
          if (_analysisCaseResults == null || _analysisCaseResults.Count == 0 || !_analysisCaseResults.ContainsKey(_caseID))
          {
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "The GSA Model contains no results for Analysis Case A" + _caseID);
            return;
          }
          goto GetResults;
        }
        if (ResultType == GsaResult.ResultType.Combination && this.Params.Input[2].SourceCount > 0)
        {
          _combinationCaseResults = _gsaModel.Model.CombinationCaseResults();
          if (_combinationCaseResults == null || _combinationCaseResults.Count == 0 || !_combinationCaseResults.ContainsKey(_caseID))
          {
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "The GSA Model contains no results for Combination Case C" + _caseID);
            return;
          }
          tempNodeResult = _combinationCaseResults[_caseID].NodeResults(tempNodeID.ToString());
          foreach (int permutation in _permutations)
          {
            if (!tempNodeResult.ContainsKey(permutation))
            {
              AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "The GSA Model contains no results for Permutaion P" + permutation + " for Combination Case C" + _caseID);
            }
          }
          goto GetResults;
        }
        if (caseSetByInput &&
          ((ResultType == GsaResult.ResultType.AnalysisCase && _analysisCaseResults == null) ||
          (ResultType == GsaResult.ResultType.Combination && _combinationCaseResults == null)))
        {
          if (ResultType == GsaResult.ResultType.AnalysisCase)
            _analysisCaseResults = _gsaModel.Model.Results();
          else
            _combinationCaseResults = _gsaModel.Model.CombinationCaseResults();
        }

        // 'reflect' model results and create dropdown lists
        if (updateCases ||
          (ResultType == GsaResult.ResultType.AnalysisCase && _analysisCaseResults == null) ||
          (ResultType == GsaResult.ResultType.Combination && _combinationCaseResults == null))
        {
          switch (ResultType)
          {
            case GsaResult.ResultType.AnalysisCase:
              _analysisCaseResults = _gsaModel.Model.Results();
              if (_analysisCaseResults == null || _analysisCaseResults.Count == 0)
              {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "The GSA Model contains no results. Please analyse the model first.");
                return;
              }
              DropDownItems[1] = new List<string>();
              DropDownItems[1].Add("All");
              // add analysis cases to dropdown menu
              List<int> caseIDs = _analysisCaseResults.Keys.OrderBy(x => x).ToList();
              foreach (int key in caseIDs)
                DropDownItems[1].Add("A" + key.ToString());
              // trim excess dropdown lists
              if (DropDownItems.Count > 2)
                DropDownItems.RemoveAt(2);
              // set selected item to first case
              SelectedItems[1] = DropDownItems[1][1];
              // remove excess selected items
              if (SelectedItems.Count > 2)
                SelectedItems.RemoveAt(2);
              if (!caseIDs.Contains(_caseID))
              {
                _caseID = caseIDs.First();
                SelectedItems[1] = "A" + _caseID;
              }
              updateCases = false;
              ExpireSolution(true);
              return;

            case GsaResult.ResultType.Combination:
              _combinationCaseResults = _gsaModel.Model.CombinationCaseResults();
              if (_combinationCaseResults == null || _combinationCaseResults.Count == 0)
              {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "The GSA Model contains no Combination Cases.");
                return;
              }
              DropDownItems[1] = new List<string>();
              DropDownItems[1].Add("All");
              // add analysis cases to dropdown menu
              List<int> comboIDs = _combinationCaseResults.Keys.OrderBy(x => x).ToList();
              foreach (int key in comboIDs)
                DropDownItems[1].Add("C" + key.ToString());
              // set selected item to first case
              SelectedItems[1] = DropDownItems[1][1];
              // update permutations
              if (!comboIDs.Contains(_caseID)) // if we are coming from analysis cases then test if case exist
              {
                _caseID = comboIDs.First(); //otherwise revert to first in list
                SelectedItems[1] = "C" + _caseID;
              }
              combinationCaseResult = _combinationCaseResults[_caseID];
              tempNodeResult = combinationCaseResult.NodeResults(tempNodeID.ToString());
              int nP = tempNodeResult[tempNodeResult.Keys.First()].Count;
              _permutations = Enumerable.Range(1, nP).ToList();
              if (DropDownItems.Count < 3)
                DropDownItems.Add(new List<string>());
              DropDownItems[2] = new List<string>();
              DropDownItems[2].Add("All");
              if (nP > 1)
                for (int i = 1; i < nP + 1; i++)
                  DropDownItems[2].Add("P" + i.ToString());
              updateCases = false;
              updatePermutations = false;
              ExpireSolution(true);
              return;
          }
        }

        if (ResultType == GsaResult.ResultType.Combination & updatePermutations | (_caseID > 0 & _permutations.Count > 0))
        {
          // calc permutations
          if (_combinationCaseResults == null)
            _combinationCaseResults = _gsaModel.Model.CombinationCaseResults();
          if (!_combinationCaseResults.ContainsKey(_caseID))
            _caseID = _combinationCaseResults.Keys.OrderBy(x => x).ToList().First();
          combinationCaseResult = _combinationCaseResults[_caseID];
          tempNodeResult = combinationCaseResult.NodeResults(tempNodeID.ToString());
          int nP = tempNodeResult[tempNodeResult.Keys.First()].Count;
          _permutations = Enumerable.Range(1, nP).ToList();
          if (ResultType == GsaResult.ResultType.Combination & updatePermutations)
          {
            if (DropDownItems.Count < 3)
              DropDownItems.Add(new List<string>());
            DropDownItems[2] = new List<string>();
            DropDownItems[2].Add("All");
            if (nP > 1)
              for (int i = 1; i < nP + 1; i++)
                DropDownItems[2].Add("P" + i.ToString());
            updatePermutations = false;
            ExpireSolution(true);
            return;
          }
          if (_permutations.Max() > nP)
          {
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Combination Case C" + _caseID + " does not contain more than " + nP + " permutations but the maximum input Permutation is " + _permutations.Max());
            return;
          }
        }

      GetResults:
        // Get results from model and create result object
        switch (ResultType)
        {
          case GsaResult.ResultType.AnalysisCase:
            if (_caseID > 0)
            {
              if (_analysisCaseResults == null)
              {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No Analysis Case Results exist in Model");
                return;
              }
              if (!_analysisCaseResults.ContainsKey(_caseID))
              {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Analysis Case does not exist in model");
                return;
              }
              if (!Result.ContainsKey(new Tuple<GsaResult.ResultType, int>(GsaResult.ResultType.AnalysisCase, _caseID)))
              {
                Result.Add(new Tuple<GsaResult.ResultType, int>(GsaResult.ResultType.AnalysisCase, _caseID),
                    new GsaResult(_gsaModel, _analysisCaseResults[_caseID], _caseID));
              }
            }
            else
            {
              if (_analysisCaseResults == null)
              {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No Analysis Case Results exist in Model");
                return;
              }
              foreach (int key in _analysisCaseResults.Keys)
              {
                if (!Result.ContainsKey(new Tuple<GsaResult.ResultType, int>(GsaResult.ResultType.AnalysisCase, key)))
                {
                  Result.Add(new Tuple<GsaResult.ResultType, int>(GsaResult.ResultType.AnalysisCase, key),
                      new GsaResult(_gsaModel, _analysisCaseResults[key], key));
                }
              }
            }
            break;

          case GsaResult.ResultType.Combination:
            if (_caseID > 0)
            {
              if (_combinationCaseResults == null)
              {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No Combination Case Results exist in Model");
                return;
              }
              if (!_combinationCaseResults.ContainsKey(_caseID))
              {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Combination Case does not exist in model");
                return;
              }
              if (!Result.ContainsKey(new Tuple<GsaResult.ResultType, int>(GsaResult.ResultType.Combination, _caseID)))
              {
                Result.Add(new Tuple<GsaResult.ResultType, int>(GsaResult.ResultType.Combination, _caseID),
                    new GsaResult(_gsaModel, _combinationCaseResults[_caseID], _caseID, _permutations));
              }
              else
              {
                if (Result[new Tuple<GsaResult.ResultType, int>(ResultType, _caseID)].SelectedPermutationIDs != _permutations)
                  Result[new Tuple<GsaResult.ResultType, int>(ResultType, _caseID)].SelectedPermutationIDs = _permutations.OrderBy(x => x).ToList();
              }
            }
            else
            {
              if (_combinationCaseResults == null)
              {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No Combination Case Results exist in Model");
                return;
              }
              foreach (int key in _combinationCaseResults.Keys)
              {
                if (!Result.ContainsKey(new Tuple<GsaResult.ResultType, int>(GsaResult.ResultType.Combination, key)))
                {
                  // update number of permutations in case
                  if (SelectedItems[2].ToLower() == "all")
                  {
                    combinationCaseResult = _combinationCaseResults[key];
                    tempNodeResult = combinationCaseResult.NodeResults(tempNodeID.ToString());
                    int nP = tempNodeResult[tempNodeResult.Keys.First()].Count;
                    _permutations = Enumerable.Range(1, nP).ToList();
                  }

                  Result.Add(new Tuple<GsaResult.ResultType, int>(GsaResult.ResultType.Combination, key),
                      new GsaResult(_gsaModel, _combinationCaseResults[key], key, _permutations));
                }
              }
            }
            break;
        }

        if (_caseID > 0)
        {
          DA.SetData(0, new GsaResultGoo(Result[new Tuple<GsaResult.ResultType, int>(ResultType, _caseID)]));
        }
        else
        {
          // in case all results are selected
          List<GsaResultGoo> results = new List<GsaResultGoo>();
          List<int> caseIDs = Result.Keys.Where(x => x.Item1 == ResultType).Select(x => x.Item2).ToList();
          caseIDs.Sort();
          foreach (int id in caseIDs)
            results.Add(new GsaResultGoo(Result[new Tuple<GsaResult.ResultType, int>(ResultType, id)]));

          DA.SetDataList(0, results);
        }
      }
    }

    #region Custom UI
    List<string> _type = new List<string>(new string[]
    {
      "AnalysisCase",
      "Combination"
    });
    GsaResult.ResultType ResultType = GsaResult.ResultType.AnalysisCase;
    bool updatePermutations;
    bool updateCases;
    int tempNodeID = 0;
    public override void InitialiseDropdowns()
    {
      this.SpacerDescriptions = new List<string>(new string[]
        {
          "Type", "Case ID", "Permutation"
        });

      this.DropDownItems = new List<List<string>>();
      this.SelectedItems = new List<string>();

      // type
      this.DropDownItems.Add(this._type);
      this.SelectedItems.Add(this.DropDownItems[0][0]);

      // placeholder
      this.DropDownItems.Add(new List<string>() { "   " });
      this.SelectedItems.Add("   ");

      this.IsInitialised = true;
    }

    public override void SetSelected(int i, int j)
    {
      this.SelectedItems[i] = this.DropDownItems[i][j];

      if (i == 0) //change is made to first dropdown list
      {
        if (this.SelectedItems[i] == this._type[0])
        {
          if (this.ResultType == GsaResult.ResultType.AnalysisCase)
            return;
          this.ResultType = GsaResult.ResultType.AnalysisCase;
          if (this.SelectedItems[1] != "   ")
          {
            if (this.SelectedItems[1] != "All")
            {
              this.SelectedItems[1] = "A" + this._caseID.ToString();
            }
            if (this.DropDownItems.Count > 2)
              this.DropDownItems.RemoveAt(2);
            this.updateCases = true;
          }
        }
        else if (this.SelectedItems[i] == this._type[1])
        {
          if (this.ResultType == GsaResult.ResultType.Combination)
            return;
          this.ResultType = GsaResult.ResultType.Combination;
          if (this.DropDownItems.Count < 3)
          {
            this.DropDownItems.Add(new List<string>() { "All" });
            if (this.SelectedItems.Count < 3)
            {
              this.SelectedItems.Add("All");
            }
          }
          if (this.SelectedItems[1] != "   ")
          {
            if (this.SelectedItems[1] != "All")
            {
              this.SelectedItems[1] = "C" + this._caseID.ToString();
            }
            this.updateCases = true;
          }
        }
      }

      if (i == 1)
      {
        if (this.SelectedItems[i].ToLower() == "all")
        {
          this._caseID = -1;
          this.updateCases = false;
        }
        else
        {
          int newID = int.Parse(string.Join("", this.SelectedItems[i].ToCharArray().Where(Char.IsDigit)));
          if (newID != _caseID)
          {
            this._caseID = newID;
            this.updatePermutations = true;
          }
        }
      }

      if (i == 2)
      {
        if (this.SelectedItems[i].ToLower() == "all")
        {
          this._permutations = new List<int>();
          this.updatePermutations = true;
        }
        else
          this._permutations = new List<int>() { int.Parse(string.Join("", this.SelectedItems[i].ToCharArray().Where(Char.IsDigit))) };
      }

      base.UpdateUI();
    }
    public override void UpdateUIFromSelectedItems()
    {
      if (this.SelectedItems[0] == this._type[0])
        this.ResultType = GsaResult.ResultType.AnalysisCase;
      else if (this.SelectedItems[0] == this._type[1])
        this.ResultType = GsaResult.ResultType.Combination;

      if (this.SelectedItems[1].ToLower() == "all")
      {
        this._caseID = -1;
        this.updateCases = false;
      }
      else
      {
        int newID = int.Parse(string.Join("", this.SelectedItems[1].ToCharArray().Where(Char.IsDigit)));
        if (newID != _caseID)
        {
          this._caseID = newID;
          this.updatePermutations = true;
        }
      }

      if (this.SelectedItems.Count > 2)
      {
        if (this.SelectedItems[2].ToLower() == "all")
        {
          this._permutations = new List<int>();
          this.updatePermutations = true;
        }
        else
          this._permutations = new List<int>() { int.Parse(string.Join("", this.SelectedItems[2].ToCharArray().Where(Char.IsDigit))) };
      }

      base.UpdateUIFromSelectedItems();
    }
    #endregion
  }
}
