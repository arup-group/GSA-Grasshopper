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

namespace GsaGH.Components
{
    /// <summary>
    /// Component to select results from a GSA Model
    /// </summary>
    public class GetResult : GH_OasysComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("799e1ac7-a310-4a65-a737-f5f5d0077879");
    public override GH_Exposure Exposure => GH_Exposure.primary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.GetResults;

    public GetResult() : base("Get Results",
      "GetRes",
      "Get AnalysisCase or Combination Result from an analysed GSA model",
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
      pManager[3].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddParameter(new GsaResultsParameter(), "Result", "Res", "GSA Result", GH_ParamAccess.item);
    }
    #endregion

    Guid _modelGUID = new Guid(); // chache model to 
    ReadOnlyDictionary<int, AnalysisCaseResult> _analysisCaseResults;
    ReadOnlyDictionary<int, CombinationCaseResult> _combinationCaseResults;
    int tempNodeID = 0;
    Dictionary<Tuple<GsaResult.ResultType, int>, GsaResult> Result; // this is the cache object!
    protected override void SolveInstance(IGH_DataAccess DA)
    {
      // Model to work on
      GsaModel model = new GsaModel();

      // Get Model
      GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
      if (DA.GetData(0, ref gh_typ))
      {
        if (gh_typ.Value is GsaModelGoo)
        {
          GsaModel in_Model = new GsaModel();
          gh_typ.CastTo(ref in_Model);
          if (_modelGUID == new Guid())
          {
            if (in_Model.Guid != _modelGUID) // only get results if GUID is not similar
            {
              model = in_Model;
              Result = new Dictionary<Tuple<GsaResult.ResultType, int>, GsaResult>();
              _analysisCaseResults = null;
              _combinationCaseResults = null;
            }
          }
          else
          {
            // first time
            model = in_Model;
            _modelGUID = model.Guid;
            Result = new Dictionary<Tuple<GsaResult.ResultType, int>, GsaResult>();
          }
        }
        else
        {
          AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Error converting input " + Params.Input[0].NickName + " to GSA Model");
          return;
        }

        // Get case type
        GsaResult.ResultType resultType = GsaResult.ResultType.AnalysisCase;
        GH_String gh_Type = new GH_String();
        if (DA.GetData(1, ref gh_Type))
        {
          string type = "";
          if (GH_Convert.ToString(gh_Type, out type, GH_Conversion.Both))
          {
            if (type.ToUpper().StartsWith("A"))
            {
              resultType = GsaResult.ResultType.AnalysisCase;
            }
            else if (type.ToUpper().StartsWith("C"))
            {
              resultType = GsaResult.ResultType.Combination;
            }
            else
            {
              AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Error converting input " + Params.Input[1].NickName + " to 'Analysis' or 'Combination'");
              return;
            }
          }
        }

        // Get analysis case 
        int caseID = 1;
        GH_Integer gh_aCase = new GH_Integer();
        if (DA.GetData(2, ref gh_aCase))
        {
          int analCase = 1;
          if (GH_Convert.ToInt32(gh_aCase, out analCase, GH_Conversion.Both))
            caseID = analCase;
          if (caseID < 1)
          {
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Input " + Params.Input[2].NickName + " must be above 0");
            return;
          }
        }

        // Get permutation case 
        List<int> permutationIDs = new List<int>();
        if (resultType != GsaResult.ResultType.AnalysisCase)
        {
          List<int> gh_perms = new List<int>();
          if (DA.GetDataList(3, gh_perms))
          {
            permutationIDs = gh_perms;
          }
          else
          {
            AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "By default, all permutations have been selected.");
            permutationIDs = new List<int>() { -1 };
          }
        }
        if (this.Params.Input[1].SourceCount == 0 && this.Params.Input[2].SourceCount == 0)
          AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "By default, Analysis Case 1 has been selected.");

        // Get results from model and create result object
        switch (resultType)
        {
          case GsaResult.ResultType.AnalysisCase:
            if (_analysisCaseResults == null)
            {
              _analysisCaseResults = model.Model.Results();
              if (_analysisCaseResults == null || _analysisCaseResults.Count == 0)
              {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No Analysis Case Results exist in Model");
                return;
              }
            }

            if (!_analysisCaseResults.ContainsKey(caseID))
            {
              AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Analysis Case does not exist in model");
              return;
            }

            if (!Result.ContainsKey(new Tuple<GsaResult.ResultType, int>(GsaResult.ResultType.AnalysisCase, caseID)))
            {
              Result.Add(new Tuple<GsaResult.ResultType, int>(GsaResult.ResultType.AnalysisCase, caseID),
                  new GsaResult(model, _analysisCaseResults[caseID], caseID));
            }
            break;

          case GsaResult.ResultType.Combination:
            if (_combinationCaseResults == null)
            {
              _combinationCaseResults = model.Model.CombinationCaseResults();
              if (_combinationCaseResults == null || _combinationCaseResults.Count == 0)
              {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "No Combination Case Results exist in Model");
                return;
              }
            }

            if (!_combinationCaseResults.ContainsKey(caseID))
            {
              AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Combination Case does not exist in model");
              return;
            }

            if (tempNodeID == 0)
            {
              tempNodeID = model.Model.Nodes().Keys.First();
            }

            IReadOnlyDictionary<int, ReadOnlyCollection<NodeResult>> tempNodeCombResult = _combinationCaseResults[caseID].NodeResults(tempNodeID.ToString());
            int nP = tempNodeCombResult[tempNodeCombResult.Keys.First()].Count;
            if (permutationIDs.Count == 1 && permutationIDs[0] == -1)
              permutationIDs = Enumerable.Range(1, nP).ToList();
            else
            {
              if (permutationIDs.Max() > nP)
              {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Combination Case C" + caseID + " only contains " + nP + " permutations but the highest permutation in input is " + permutationIDs.Max());
                return;
              }
            }

            if (!Result.ContainsKey(new Tuple<GsaResult.ResultType, int>(GsaResult.ResultType.Combination, caseID)))
            {
              Result.Add(new Tuple<GsaResult.ResultType, int>(GsaResult.ResultType.Combination, caseID),
                  new GsaResult(model, _combinationCaseResults[caseID], caseID, permutationIDs));
            }
            break;
        }
        
        DA.SetData(0, new GsaResultGoo(Result[new Tuple<GsaResult.ResultType, int>(resultType, caseID)]));
      }
    }
  }
}
