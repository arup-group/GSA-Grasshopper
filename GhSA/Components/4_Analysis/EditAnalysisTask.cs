using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GsaGH.Parameters;


namespace GsaGH.Components
{
    /// <summary>
    /// Component to retrieve non-geometric objects from a GSA model
    /// </summary>
    public class EditAnalysisTask: GH_Component
    {
        #region Name and Ribbon Layout
        // This region handles how the component in displayed on the ribbon
        // including name, exposure level and icon
        public override Guid ComponentGuid => new Guid("efc2aae5-7ebf-4032-89d5-8fec8830989d");
        public EditAnalysisTask()
          : base("Edit Analysis Task", "EditTask", "Modify GSA Analysis Tasks",
                Ribbon.CategoryName.Name(),
                Ribbon.SubCategoryName.Cat4())
        { this.Hidden = true; } // sets the initial state of the component to hidden
        public override GH_Exposure Exposure => GH_Exposure.quarternary | GH_Exposure.obscure;

        protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.EditAnalysisTask;
        #endregion

        #region Custom UI
        //This region overrides the typical component layout
        #endregion

        #region Input and output

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Analysis Task", "ΣT", "GSA Analysis Task to Edit", GH_ParamAccess.item);
            pManager.AddTextParameter("Name", "Na", "Set Task Name", GH_ParamAccess.item);
            pManager.AddGenericParameter("Analysis Cases", "ΣAs", "Set List of GSA Analysis Cases", GH_ParamAccess.list);
            pManager.AddTextParameter("Solver Type", "sT", "Set Solver Type" + System.Environment.NewLine +
                "Default is 1: Static - Accepted inputs are:" + System.Environment.NewLine +
                "Static" + System.Environment.NewLine +
                "Static_P_delta" + System.Environment.NewLine +
                "Nonlinear_static" + System.Environment.NewLine +
                "Modal_dynamic" + System.Environment.NewLine +
                "Modal_P_delta" + System.Environment.NewLine +
                "Ritz" + System.Environment.NewLine +
                "Ritz_P_Delta" + System.Environment.NewLine +
                "Response_spectrum" + System.Environment.NewLine +
                "Pseudo_Response_spectrum" + System.Environment.NewLine +
                "Linear_time_history" + System.Environment.NewLine +
                "Harmonic" + System.Environment.NewLine +
                "Footfall" + System.Environment.NewLine +
                "Periodic" + System.Environment.NewLine +
                "Buckling" + System.Environment.NewLine +
                "Form_finding" + System.Environment.NewLine +
                "Envelope" + System.Environment.NewLine +
                "Model_stability" + System.Environment.NewLine +
                "Model_stability_P_delta", GH_ParamAccess.item);
            pManager.AddIntegerParameter("TaskID", "ID", "The Task number - only set this if you want to append cases to an existing task.", GH_ParamAccess.item);
            pManager[1].Optional = true;
            pManager[2].Optional = true;
            pManager[3].Optional = true;
            pManager[4].Optional = true;
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Analysis Task", "ΣT", "GSA Analysis Task to Edit", GH_ParamAccess.item);
            pManager.AddTextParameter("Name", "Na", "Task Name", GH_ParamAccess.item);
            pManager.AddGenericParameter("Analysis Cases", "ΣAs", "List of GSA Analysis Cases", GH_ParamAccess.list);
            pManager.AddTextParameter("Solver Type", "sT", "Solver Type", GH_ParamAccess.item);
            pManager.AddIntegerParameter("TaskID", "ID", "The Task number if the Analysis Case ever belonged to a model", GH_ParamAccess.item);
        }
        #endregion

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
            if (DA.GetData(0, ref gh_typ))
            {
                GsaAnalysisTask gsaTask = null;
                if (gh_typ.Value is GsaAnalysisTaskGoo)
                {
                    gsaTask = ((GsaAnalysisTaskGoo)gh_typ.Value).Value.Duplicate();

                    string name = gsaTask.Name;
                    if (DA.GetData(1, ref name))
                        gsaTask.Name = name;

                    List<GH_ObjectWrapper> gh_types = new List<GH_ObjectWrapper>();
                    if (DA.GetDataList(2, gh_types))
                    {
                        List<GsaAnalysisCase> cases = new List<GsaAnalysisCase>();
                        for (int i = 0; i < gh_types.Count; i++)
                        {
                            GH_ObjectWrapper gh_typ2 = gh_types[i];
                            if (gh_typ2 == null) { Params.Owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Analysis Case input (index: " + i + ") is null and has been ignored"); continue; }
                            if (gh_typ2.Value is GsaAnalysisCaseGoo)
                            {
                                cases.Add(((GsaAnalysisCaseGoo)gh_typ2.Value).Value.Duplicate());
                            }
                            else
                            {
                                string typ = gh_typ2.Value.GetType().ToString();
                                typ = typ.Replace("GsaGH.Parameters.", "");
                                typ = typ.Replace("Goo", "");
                                Params.Owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert Analysis Case input parameter of type " +
                                    typ + " to GsaAnalysisCase");
                                return;
                            }
                        }
                        gsaTask.Cases = cases;
                    }

                    string type = gsaTask.Type.ToString();
                    if (DA.GetData(3, ref type))
                        gsaTask.Type = (GsaAnalysisTask.AnalysisType)Enum.Parse(typeof(GsaAnalysisTask.AnalysisType), type);

                    int id = 0;
                    if (DA.GetData(4, ref id))
                        gsaTask.SetID(id);

                    DA.SetData(0, new GsaAnalysisTaskGoo(gsaTask));
                    DA.SetData(1, gsaTask.Name);
                    DA.SetDataList(2, new List<GsaAnalysisCaseGoo>(gsaTask.Cases.Select(x => new GsaAnalysisCaseGoo(x))));
                    DA.SetData(3, type);
                    DA.SetData(4, gsaTask.ID);
                }
                else
                {
                    string type = gh_typ.Value.GetType().ToString();
                    type = type.Replace("GsaGH.Parameters.", "");
                    type = type.Replace("Goo", "");
                    Params.Owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert Analysis Task input parameter of type " +
                        type + " to GsaAnalysisTask");
                    return;
                }
            }
        }
    }
}

