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
            pManager.AddGenericParameter("Analysis Cases", "ΣC", "List of Analysis Cases in model", GH_ParamAccess.list);
        }
        #endregion

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GsaModel gsaModel = new GsaModel();
            if (DA.GetData(0, ref gsaModel))
            {
                Tuple<List<GsaAnalysisTaskGoo>, List<GsaAnalysisCaseGoo>> tuple = Util.Gsa.FromGSA.GetAnalysisTasksAndCombinations(gsaModel);

                DA.SetDataList(0, tuple.Item1);
                DA.SetDataList(1, tuple.Item2);
            }
        }
    }
}

