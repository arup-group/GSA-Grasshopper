using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Grasshopper;
using Grasshopper.Kernel;
using GsaAPI;
using GsaGH.Parameters;


namespace GsaGH.Components
{
    /// <summary>
    /// Component to retrieve non-geometric objects from a GSA model
    /// </summary>
    public class GetAnalysis : GH_Component
    {
        #region Name and Ribbon Layout
        // This region handles how the component in displayed on the ribbon
        // including name, exposure level and icon
        public override Guid ComponentGuid => new Guid("566a94d2-a022-4f12-a645-0366deb1476c");
        public GetAnalysis()
          : base("Get Model Analysis Tasks", "GetAnalysisTasks", "Get Analysis Tasks and their Cases from GSA model",
                Ribbon.CategoryName.Name(),
                Ribbon.SubCategoryName.Cat0())
        { this.Hidden = true; } // sets the initial state of the component to hidden
        public override GH_Exposure Exposure => GH_Exposure.secondary | GH_Exposure.obscure;

        protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.GetAnalysisTask;
        #endregion

        #region Custom UI
        //This region overrides the typical component layout
        #endregion

        #region Input and output

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("GSA Model", "GSA", "GSA model containing some Analysis Cases and Tasks", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Analysis Tasks", "ΣT", "List of Analysis Tasks in model", GH_ParamAccess.list);
            pManager.AddGenericParameter("Analysis Cases", "ΣC", "Tree with list of Analysis Cases per Analysis Task", GH_ParamAccess.tree);
        }
        #endregion

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GsaModel gsaModel = new GsaModel();
            if (DA.GetData(0, ref gsaModel))
            {
                Model model = gsaModel.Model;
                ReadOnlyDictionary<int, AnalysisTask> tasks = model.AnalysisTasks();

                List<GsaAnalysisTaskGoo> tasksList = new List<GsaAnalysisTaskGoo>();
                DataTree<GsaAnalysisCaseGoo> tree = new DataTree<GsaAnalysisCaseGoo>();

                foreach (KeyValuePair<int, AnalysisTask> item in tasks)
                {
                    GsaAnalysisTask task = new GsaAnalysisTask(item.Key, item.Value, model);
                    tasksList.Add(new GsaAnalysisTaskGoo(task));
                    tree.AddRange(task.Cases.Select(c => new GsaAnalysisCaseGoo(c)).ToList(), new Grasshopper.Kernel.Data.GH_Path(item.Key));
                }

                DA.SetDataList(0, tasksList);
                DA.SetDataTree(1, tree);
            }
        }
    }
}

