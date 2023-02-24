using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GsaGH.Parameters;
using GsaGH.Helpers.GH;
using OasysGH;
using OasysGH.Components;
using OasysUnits;
using Grasshopper;
using GsaGH.Helpers.GsaAPI;
using Rhino.Commands;
using Grasshopper.Kernel.Data;

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


    private GsaResult.CaseType _resultType = GsaResult.CaseType.AnalysisCase;
    private int _caseID = 1;
    private List<int> _permutationIDs = new List<int>() { -1 };
    private GsaModel _gsaModel;
    private Dictionary<Tuple<GsaResult.CaseType, int>, GsaResult> _resultCache; // this is the cache object!
    private ReadOnlyDictionary<int, AnalysisCaseResult> _analysisCaseResults;
    private ReadOnlyDictionary<int, CombinationCaseResult> _combinationCaseResults;

    public SelectResult() : base("Select Results",
      "SelRes",
      "Select AnalysisCase or Combination Result from an analysed GSA model",
      CategoryName.Name(),
      SubCategoryName.Cat5())
    { this.Hidden = true; } // sets the initial state of the component to hidden
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
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

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddParameter(new GsaResultsParameter(), "Result", "Res", "GSA Result", GH_ParamAccess.item);
    }
    #endregion


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
              this.UpdateDropdowns();
              this._resultCache = new Dictionary<Tuple<GsaResult.CaseType, int>, GsaResult>();
              this._analysisCaseResults = null;
              this._combinationCaseResults = null;
            }
          }
          else
          {
            // first time
            _gsaModel = in_Model;
            this.UpdateDropdowns();
            this._resultCache = new Dictionary<Tuple<GsaResult.CaseType, int>, GsaResult>();
          }
        }
        else
        {
          this.AddRuntimeError("Error converting input to GSA Model");
          return;
        }

        // Get case type
        GH_String gh_Type = new GH_String();
        if (DA.GetData(1, ref gh_Type))
        {
          string type = "";
          if (GH_Convert.ToString(gh_Type, out type, GH_Conversion.Both))
          {
            if (type.ToUpper().StartsWith("A"))
            {
              this._resultType = GsaResult.CaseType.AnalysisCase;
              this.SelectedItems[0] = DropDownItems[0][0];
              if (this._resultType != GsaResult.CaseType.AnalysisCase)
              {
                this._resultType = GsaResult.CaseType.AnalysisCase;
                if (this.DropDownItems.Count > 2)
                {
                  this.DropDownItems.RemoveAt(2);
                  if (this.SelectedItems.Count > 2)
                    this.SelectedItems.RemoveAt(2);
                }
                if (this.SelectedItems[1] != "   ")
                {
                  if (this.SelectedItems[1] != "All")
                    this.SelectedItems[1] = "A" + _caseID.ToString();
                  else
                    this.UpdateDropdowns();
                }
              }
            }
            else if (type.ToUpper().StartsWith("C"))
            {
              this.SelectedItems[0] = this.DropDownItems[0][1];
              if (_resultType != GsaResult.CaseType.Combination)
              {
                this._resultType = GsaResult.CaseType.Combination;
                if (this.DropDownItems.Count < 3)
                {
                  this.DropDownItems.Add(new List<string>() { "All" });
                  if (this.SelectedItems.Count < 3)
                    this.SelectedItems.Add("All");
                }
                if (this.SelectedItems[1] != "   ")
                {
                  if (this.SelectedItems[1] != "All")
                    this.SelectedItems[1] = "C" + _caseID.ToString();
                  else
                    this.UpdateDropdowns();
                }
              }
            }
          }
        }

        // Get analysis case 
        GH_Integer gh_aCase = new GH_Integer();
        if (DA.GetData(2, ref gh_aCase))
        {
          int analCase = 1;
          if (GH_Convert.ToInt32(gh_aCase, out analCase, GH_Conversion.Both))
          {
            if (this._resultType == GsaResult.CaseType.Combination && this._caseID != analCase)
              this.UpdatePermutations();
            this._caseID = analCase;
            if (analCase < 1)
              this.SelectedItems[1] = "All";
            else
              this.SelectedItems[1] = (_resultType == GsaResult.CaseType.AnalysisCase) ? "A" + analCase : "C" + analCase;
          }
        }

        // Get permutation case 
        if (this._resultType != GsaResult.CaseType.AnalysisCase)
        {
          List<int> gh_perms = new List<int>();
          if (DA.GetDataList(3, gh_perms))
          {
            this.UpdatePermutations();
            this._permutationIDs = gh_perms;
            if (this._permutationIDs.Count == 1)
            {
              if (_permutationIDs[0] < 1)
                SelectedItems[2] = "All";
              else
                SelectedItems[2] = "P" + _permutationIDs[0];
            }
            else
              SelectedItems[2] = "from input";
          }
        }

        // Get results from model and create result object
        switch (this._resultType)
        {
          case GsaResult.CaseType.AnalysisCase:
            if (this._analysisCaseResults == null)
            {
              this._analysisCaseResults = this._gsaModel.Model.Results();
              if (this._analysisCaseResults == null || this._analysisCaseResults.Count == 0)
              {
                this.AddRuntimeError("No Analysis Case Results exist in Model");
                return;
              }
            }

            if (!this._analysisCaseResults.ContainsKey(this._caseID))
            {
              this.AddRuntimeError("Analysis Case A" + this._caseID + " does not exist in model");
              return;
            }

            if (!this._resultCache.ContainsKey(new Tuple<GsaResult.CaseType, int>(GsaResult.CaseType.AnalysisCase, this._caseID)))
            {
              this._resultCache.Add(new Tuple<GsaResult.CaseType, int>(GsaResult.CaseType.AnalysisCase, this._caseID),
                  new GsaResult(this._gsaModel, this._analysisCaseResults[this._caseID], this._caseID));
            }
            break;

          case GsaResult.CaseType.Combination:
            if (this._combinationCaseResults == null)
            {
              this._combinationCaseResults = this._gsaModel.Model.CombinationCaseResults();
              if (this._combinationCaseResults == null || _combinationCaseResults.Count == 0)
              {
                this.AddRuntimeError("No Combination Case Results exist in Model");
                return;
              }
            }

            if (!this._combinationCaseResults.ContainsKey(this._caseID))
            {
              this.AddRuntimeError("Combination Case does not exist in model");
              return;
            }

            IReadOnlyDictionary<int, ReadOnlyCollection<NodeResult>> tempNodeCombResult = _combinationCaseResults[this._caseID].NodeResults(this._gsaModel.Model.Nodes().Keys.First().ToString());
            int nP = tempNodeCombResult[tempNodeCombResult.Keys.First()].Count;
            if (this._permutationIDs.Count == 1 && this._permutationIDs[0] == -1)
              this._permutationIDs = Enumerable.Range(1, nP).ToList();
            else
            {
              if (this._permutationIDs.Count == 0)
              {
                this.AddRuntimeWarning("Combination Case C" + this._caseID + " does not contain results");
                return;
              }
              if (this._permutationIDs.Max() > nP)
              {
                this.AddRuntimeError("Combination Case C" + this._caseID + " only contains " + nP + " permutations but the highest permutation in input is " + this._permutationIDs.Max());
                return;
              }
            }

            if (!this._resultCache.ContainsKey(new Tuple<GsaResult.CaseType, int>(GsaResult.CaseType.Combination, this._caseID)))
            {
              this._resultCache.Add(new Tuple<GsaResult.CaseType, int>(GsaResult.CaseType.Combination, this._caseID),
                  new GsaResult(this._gsaModel, this._combinationCaseResults[this._caseID], this._caseID, this._permutationIDs));
            }
            else
            {
              this._resultCache[new Tuple<GsaResult.CaseType, int>(this._resultType, this._caseID)].SelectedPermutationIDs = this._permutationIDs;
            }
            break;
        }

        DA.SetData(0, new GsaResultGoo(this._resultCache[new Tuple<GsaResult.CaseType, int>(this._resultType, this._caseID)]));
      }
    }

    #region Custom UI
    List<string> _type = new List<string>(new string[]
    {
      "AnalysisCase",
      "Combination"
    });


    private void UpdateDropdowns()
    {
      if (this._gsaModel == null)
        return;

      Tuple<List<string>, List<int>, DataTree<int?>> modelResults = ResultHelper.GetAvalailableResults(this._gsaModel);
      string type = (this._resultType == GsaResult.CaseType.AnalysisCase) ? "Analysis" : "Combination";

      List<string> cases = new List<string>() { };
      for (int i = 0; i < modelResults.Item1.Count; i++)
      {
        if (modelResults.Item1[i] != type)
          continue;
        cases.Add(type[0] + modelResults.Item2[i].ToString());
      }
      this.DropDownItems[1] = cases;
      this.SelectedItems[1] = type[0] + _caseID.ToString();

      if (this._resultType == GsaResult.CaseType.Combination)
      {
        if (this.DropDownItems.Count < 3)
        {
          this.DropDownItems.Add(new List<string>() { "All" });
          this.SelectedItems.Add("All");
          this.UpdatePermutations();
        }
        else
        {
          this.DropDownItems[2] = new List<string>() { "All" };
          this.SelectedItems[2] = "All";
        }
        List<int?> ints = modelResults.Item3.Branch(new GH_Path(this._caseID));
        this.DropDownItems[2].AddRange(ints.Select(x => "P" + x.ToString()).ToList());
      }
      else if (this.DropDownItems.Count > 2)
      {
        this.DropDownItems.RemoveAt(2);
        this.SelectedItems.RemoveAt(2);
      }
    }

    private void UpdatePermutations()
    {
      if (this._resultType == GsaResult.CaseType.Combination)
      {
        if (this._combinationCaseResults == null)
          this._combinationCaseResults = this._gsaModel.Model.CombinationCaseResults();
        IReadOnlyDictionary<int, ReadOnlyCollection<NodeResult>> tempNodeCombResult = this._combinationCaseResults[this._caseID].NodeResults(this._gsaModel.Model.Nodes().Keys.First().ToString());
        int nP = tempNodeCombResult[tempNodeCombResult.Keys.First()].Count;
        List<int> permutationsInCase = Enumerable.Range(1, nP).ToList();
        if (this.DropDownItems.Count < 3)
          this.DropDownItems.Add(new List<string>());
        this.DropDownItems[2] = new List<string> { "All" };
        if (this.SelectedItems.Count < 3)
          this.SelectedItems.Add("");
        this.SelectedItems[2] = "All";
        this.DropDownItems[2].AddRange(permutationsInCase.Select(x => "P" + x.ToString()).ToList());
        this._permutationIDs = new List<int>() { -1 };
      }
    }

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
          if (this._resultType == GsaResult.CaseType.AnalysisCase)
            return;
          this._resultType = GsaResult.CaseType.AnalysisCase;
          this.UpdateDropdowns();
        }
        else if (this.SelectedItems[i] == this._type[1])
        {
          if (this._resultType == GsaResult.CaseType.Combination)
            return;
          this._resultType = GsaResult.CaseType.Combination;
          this.UpdateDropdowns();
        }
      }

      if (i == 1)
      {
        if (this.SelectedItems[i].ToLower() == "all")
          this._caseID = -1;
        else
        {
          int newID = int.Parse(string.Join("", this.SelectedItems[i].ToCharArray().Where(Char.IsDigit)));
          if (newID != _caseID)
          {
            this._caseID = newID;
            if (this._resultType == GsaResult.CaseType.Combination)
              this.UpdatePermutations();
          }
        }
      }

      if (i == 2)
      {
        if (this.SelectedItems[i].ToLower() != "all")
          this._permutationIDs = new List<int>() { int.Parse(string.Join("", this.SelectedItems[i].ToCharArray().Where(Char.IsDigit))) };
        else
          this._permutationIDs = new List<int>() { -1 };
      }

      base.UpdateUI();
    }
    public override void UpdateUIFromSelectedItems()
    {
      if (this.SelectedItems[0] == this._type[0])
        this._resultType = GsaResult.CaseType.AnalysisCase;
      else if (this.SelectedItems[0] == this._type[1])
        this._resultType = GsaResult.CaseType.Combination;

      if (this.SelectedItems[1].ToLower() == "all")
        this._caseID = -1;
      else
      {
        int newID = int.Parse(string.Join("", this.SelectedItems[1].ToCharArray().Where(Char.IsDigit)));
        if (newID != _caseID)
          this._caseID = newID;
      }

      if (this.SelectedItems.Count > 2)
      {
        if (this.SelectedItems[2].ToLower() == "all")
          this._permutationIDs = new List<int>() { -1 };
        else
          this._permutationIDs = new List<int>() { int.Parse(string.Join("", this.SelectedItems[2].ToCharArray().Where(Char.IsDigit))) };
      }

      base.UpdateUIFromSelectedItems();
    }
    #endregion
  }
}
