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
using GsaGH.Parameters;
using OasysGH;
using OasysGH.Components;

namespace GsaGH.Components
{
    /// <summary>
    /// Component to select results from a GSA Model
    /// </summary>
    public class ResultsInfo : GH_OasysComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("6874415d-a86c-4a0d-8c84-36b39f2e5255");
    public override GH_Exposure Exposure => GH_Exposure.primary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.ResultsInfo;

    public ResultsInfo() : base("Get Result Cases",
      "GetCases",
      "Get Analysis or Combination Case IDs from a GSA model with Results",
      CategoryName.Name(),
      SubCategoryName.Cat5())
    { this.Hidden = true; } // sets the initial state of the component to hidden
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddParameter(new GsaModelParameter(), "GSA Model", "GSA", "GSA model containing some results", GH_ParamAccess.item);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddTextParameter("Result Type", "T", "Result type", GH_ParamAccess.list);
      pManager.AddIntegerParameter("Case", "ID", "Case ID(s)", GH_ParamAccess.list);
      pManager.AddIntegerParameter("Permutation", "P", "Permutations (only applicable for combination cases).", GH_ParamAccess.tree);
    }
    #endregion

    Guid _modelGUID = new Guid(); // chache model to 
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
              ClearData();
            }
          }
          else
          {
            // first time
            model = in_Model;
            _modelGUID = model.Guid;
          }
        }
        else
        {
          AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Error converting input " + Params.Input[0].NickName + " to GSA Model");
          return;
        }

        ReadOnlyDictionary<int, AnalysisCaseResult> analysisCaseResults = model.Model.Results();
        ReadOnlyDictionary<int, CombinationCaseResult> combinationCaseResults = model.Model.CombinationCaseResults();
        int tempNodeID = model.Model.Nodes().Keys.First();

        List<string> type = new List<string>();
        List<int> caseId = new List<int>();
        DataTree<int?> perm = new DataTree<int?>();
        int i = 0;
        foreach (int a in analysisCaseResults.Keys)
        {
          type.Add("Analysis");
          caseId.Add(a);
          perm.Add(null, new GH_Path(i++));
        }
        foreach (int c in combinationCaseResults.Keys)
        {
          type.Add("Combination");
          caseId.Add(c);
          IReadOnlyDictionary<int, ReadOnlyCollection<NodeResult>> tempNodeCombResult = combinationCaseResults[c].NodeResults(tempNodeID.ToString());
          int nP = tempNodeCombResult[tempNodeCombResult.Keys.First()].Count;
          List<int> permutationsInCase = Enumerable.Range(1, nP).ToList();
          GH_Path path = new GH_Path(i++);
          foreach (int p in permutationsInCase) 
            perm.Add(p, path);
        }

        DA.SetDataList(0, type);
        DA.SetDataList(1, caseId);
        DA.SetDataTree(2, perm);
      }
    }
  }
}
