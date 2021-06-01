using System;
using System.Linq;
using System.Collections.ObjectModel;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaAPI;
using Rhino.Geometry;
using GhSA.Parameters;
using GrasshopperAsyncComponent;
using System.Collections.Generic;
using Grasshopper.Kernel.Data;

namespace GhSA.Components
{
    public class Analyse : GH_AsyncComponent
    {
        #region Name and Ribbon Layout
        public Analyse()
            : base("Async Analyse Model", "AsyncAnalyse", "Assemble and Analyse a GSA Model using Multi-threading",
                Ribbon.CategoryName.Name(),
                Ribbon.SubCategoryName.Cat4())
        { BaseWorker = new AnalysisWorker(); this.Hidden = true; }

        public override Guid ComponentGuid => new Guid("b9ca86f7-fda1-4c5e-ae75-5e570d4885e9");
        public override GH_Exposure Exposure => GH_Exposure.secondary | GH_Exposure.obscure;

        protected override System.Drawing.Bitmap Icon => GhSA.Properties.Resources.AnalyseAsync;
        #endregion

        #region Custom UI
        //This region overrides the typical component layout
        public override void CreateAttributes()
        {
            m_attributes = new UI.CheckBoxComponentUI(this, SetAnalysis, checkboxText, initialCheckState, "Settings");
        }

        List<string> checkboxText = new List<string>() { "Analyse task(s)", "ElemsFromMems" };
        List<bool> initialCheckState = new List<bool>() { true, true };
        public static bool Analysis;
        public static bool ReMesh;

        public void SetAnalysis(List<bool> value)
        {
            Analysis = value[0];
            ReMesh = value[1];
        }
        #endregion

        #region input and output
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("GSA Model(s)", "GSA", "Existing GSA Model(s) to append to" + System.Environment.NewLine +
                "If you input more than one model they will be merged" + System.Environment.NewLine + "with first model in list taking priority for IDs", GH_ParamAccess.list);
            pManager.AddGenericParameter("GSA Properties", "Prob", "Sections and Prop2Ds to add/set in model" + System.Environment.NewLine +
                "Properties already added to Elements or Members" + System.Environment.NewLine + "will automatically be added with Geometry input", GH_ParamAccess.list);
            pManager.AddGenericParameter("GSA Geometry", "Geo", "Nodes, Element1Ds, Element2Ds, Member1Ds, Member2Ds and Member3Ds to add/set in model", GH_ParamAccess.list);
            pManager.AddGenericParameter("GSA Load", "Load", "Loads to add to the model" + System.Environment.NewLine + "You can also use this input to add Edited GridPlaneSurfaces", GH_ParamAccess.list);
            for (int i = 0; i < pManager.ParamCount; i++)
                pManager[i].Optional = true;
        }
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Model", "GSA", "GSA Model", GH_ParamAccess.item);
        }
        #endregion

        public class AnalysisWorker : WorkerInstance
        {
            public AnalysisWorker() : base(null) { }
            public override WorkerInstance Duplicate() => new AnalysisWorker();

            #region fields
            List<GsaModel> Models { get; set; }
            List<GsaNode> Nodes { get; set; }
            List<GsaElement1d> Elem1ds { get; set; }
            List<GsaElement2d> Elem2ds { get; set; }
            List<GsaElement3d> Elem3ds { get; set; }
            List<GsaMember1d> Mem1ds { get; set; }
            List<GsaMember2d> Mem2ds { get; set; }
            List<GsaMember3d> Mem3ds { get; set; }
            List<GsaLoad> Loads { get; set; }
            List<GsaSection> Sections { get; set; }
            List<GsaProp2d> Prop2Ds { get; set; }
            List<GsaGridPlaneSurface> GridPlaneSurfaces { get; set; }
            bool hasInput = false;
            GsaModel OutModel { get; set; }
            IGH_Component component { get; set; }
            #endregion

            public override void GetData(IGH_DataAccess DA, GH_ComponentParamServer Params)
            {
                #region GetData
                Models = null;
                Nodes = null;
                Elem1ds = null;
                Elem2ds = null;
                Elem3ds = null;
                Mem1ds = null;
                Mem2ds = null;
                Mem3ds = null;
                Loads = null;
                Sections = null;
                Prop2Ds = null;
                GridPlaneSurfaces = null;
                OutModel = null;
                component = Params.Owner;

                // Get Model input
                List<GH_ObjectWrapper> gh_types = new List<GH_ObjectWrapper>();
                if (DA.GetDataList(0, gh_types))
                {
                    List<GsaModel> in_models = new List<GsaModel>();
                    for (int i = 0; i < gh_types.Count; i++)
                    {
                        GH_ObjectWrapper gh_typ = gh_types[i];
                        if (gh_typ == null) { Params.Owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Model input (index: " + i + ") is null and has been ignored"); continue; }
                        if (gh_typ.Value is GsaModelGoo)
                        {
                            GsaModel in_model = new GsaModel();
                            gh_typ.CastTo(ref in_model);
                            in_models.Add(in_model);
                        }
                        else
                        {
                            string type = gh_typ.Value.GetType().ToString();
                            type = type.Replace("GhSA.Parameters.", "");
                            type = type.Replace("Goo", "");
                            Params.Owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert GSA input parameter of type " +
                                type + " to GsaModel");
                            return;
                        }
                    }
                    Models = in_models;
                }

                // Get Section Property input
                gh_types = new List<GH_ObjectWrapper>();
                if (DA.GetDataList(1, gh_types))
                {
                    List<GsaSection> in_sect = new List<GsaSection>();
                    List<GsaProp2d> in_prop = new List<GsaProp2d>();
                    for (int i = 0; i < gh_types.Count; i++)
                    {
                        GH_ObjectWrapper gh_typ = gh_types[i];
                        if (gh_typ == null) { Params.Owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Property input (index: " + i + ") is null and has been ignored"); continue; }
                        if (gh_typ.Value is GsaSectionGoo)
                        {
                            GsaSection gsasection = new GsaSection();
                            gh_typ.CastTo(ref gsasection);
                            in_sect.Add(gsasection.Duplicate());
                        }
                        else if (gh_typ.Value is GsaProp2dGoo)
                        {
                            GsaProp2d gsaprop = new GsaProp2d();
                            gh_typ.CastTo(ref gsaprop);
                            in_prop.Add(gsaprop.Duplicate());
                        }
                        else
                        {
                            string type = gh_typ.Value.GetType().ToString();
                            type = type.Replace("GhSA.Parameters.", "");
                            type = type.Replace("Goo", "");
                            Params.Owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert Prop input parameter of type " + 
                                type + " to GsaSection or GsaProp2d");
                            return;
                        }
                    }
                    if (in_sect.Count > 0)
                        Sections = in_sect;
                    if (in_prop.Count > 0)
                        Prop2Ds = in_prop;
                }

                // Get Geometry input
                gh_types = new List<GH_ObjectWrapper>();
                List<GsaNode> in_nodes = new List<GsaNode>();
                List<GsaElement1d> in_elem1ds = new List<GsaElement1d>();
                List<GsaElement2d> in_elem2ds = new List<GsaElement2d>();
                List<GsaElement3d> in_elem3ds = new List<GsaElement3d>();
                List<GsaMember1d> in_mem1ds = new List<GsaMember1d>();
                List<GsaMember2d> in_mem2ds = new List<GsaMember2d>();
                List<GsaMember3d> in_mem3ds = new List<GsaMember3d>();
                if (DA.GetDataList(2, gh_types))
                {
                    for (int i = 0; i < gh_types.Count; i++)
                    {
                        GH_ObjectWrapper gh_typ = gh_types[i];
                        if (gh_typ == null) { Params.Owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Geometry input (index: " + i + ") is null and has been ignored"); continue; }

                        if (gh_typ.Value is GsaNodeGoo)
                        {
                            GsaNode gsanode = new GsaNode();
                            gh_typ.CastTo(ref gsanode);
                            in_nodes.Add(gsanode.Duplicate());
                        }
                        else if (gh_typ.Value is GsaElement1dGoo)
                        {
                            GsaElement1d gsaelem1 = new GsaElement1d();
                            gh_typ.CastTo(ref gsaelem1);
                            in_elem1ds.Add(gsaelem1.Duplicate());
                        }
                        else if (gh_typ.Value is GsaElement2dGoo)
                        {
                            GsaElement2d gsaelem2 = new GsaElement2d();
                            gh_typ.CastTo(ref gsaelem2);
                            in_elem2ds.Add(gsaelem2.Duplicate());
                        }
                        else if (gh_typ.Value is GsaElement3dGoo)
                        {
                            GsaElement3d gsaelem3 = new GsaElement3d();
                            gh_typ.CastTo(ref gsaelem3);
                            in_elem3ds.Add(gsaelem3.Duplicate());
                        }
                        else if (gh_typ.Value is GsaMember1dGoo)
                        {
                            GsaMember1d gsamem1 = new GsaMember1d();
                            gh_typ.CastTo(ref gsamem1);
                            in_mem1ds.Add(gsamem1.Duplicate());
                        }
                        else if (gh_typ.Value is GsaMember2dGoo)
                        {
                            GsaMember2d gsamem2 = new GsaMember2d();
                            gh_typ.CastTo(ref gsamem2);
                            in_mem2ds.Add(gsamem2.Duplicate());
                        }
                        else if (gh_typ.Value is GsaMember3dGoo)
                        {
                            GsaMember3d gsamem3 = new GsaMember3d();
                            gh_typ.CastTo(ref gsamem3);
                            in_mem3ds.Add(gsamem3.Duplicate());
                        }
                        else
                        {
                            string type = gh_typ.Value.GetType().ToString();
                            type = type.Replace("GhSA.Parameters.", "");
                            type = type.Replace("Goo", "");
                            Params.Owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert Geometry input parameter of type " +
                                type + System.Environment.NewLine + " to Node, Element1D, Element2D, Element3D, Member1D, Member2D or Member3D");
                            return;
                        }
                    }
                    if (in_nodes.Count > 0)
                        Nodes = in_nodes;
                    if (in_elem1ds.Count > 0)
                        Elem1ds = in_elem1ds;
                    if (in_elem2ds.Count > 0)
                        Elem2ds = in_elem2ds;
                    if (in_elem3ds.Count > 0)
                        Elem3ds = in_elem3ds;
                    if (in_mem1ds.Count > 0)
                        Mem1ds = in_mem1ds;
                    if (in_mem2ds.Count > 0)
                        Mem2ds = in_mem2ds;
                    if (in_mem3ds.Count > 0)
                        Mem3ds = in_mem3ds;
                }


                // Get Loads input
                gh_types = new List<GH_ObjectWrapper>();
                if (DA.GetDataList(3, gh_types))
                {
                    List<GsaLoad> in_loads = new List<GsaLoad>();
                    List<GsaGridPlaneSurface> in_gps = new List<GsaGridPlaneSurface>();
                    for (int i = 0; i < gh_types.Count; i++)
                    {
                        GH_ObjectWrapper gh_typ = gh_types[i];
                        if (gh_typ == null) { Params.Owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Load input (index: " + i + ") is null and has been ignored"); continue; }

                        if (gh_typ.Value is GsaLoadGoo)
                        {
                            GsaLoad gsaload = null;
                            gh_typ.CastTo(ref gsaload);
                            in_loads.Add(gsaload.Duplicate());
                        }
                        else if (gh_typ.Value is GsaGridPlaneSurfaceGoo)
                        {
                            GsaGridPlaneSurface gsaGPS = new GsaGridPlaneSurface();
                            gh_typ.CastTo(ref gsaGPS);
                            in_gps.Add(gsaGPS.Duplicate());
                        }
                        else
                        {
                            string type = gh_typ.Value.GetType().ToString();
                            type = type.Replace("GhSA.Parameters.", "");
                            type = type.Replace("Goo", "");
                            Params.Owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert Load input parameter of type " +
                                type + " to Load or GridPlaneSurface");
                            return;
                        }
                    }
                    if (in_loads.Count > 0)
                        Loads = in_loads;
                    if (in_gps.Count > 0)
                        GridPlaneSurfaces = in_gps;
                }

                #endregion

                // manually add a warning if no input is set, as all inputs are optional
                if (Models == null & Nodes == null & Elem1ds == null & Elem2ds == null &
                    Mem1ds == null & Mem2ds == null & Mem3ds == null & Sections == null
                    & Prop2Ds == null & Loads == null & GridPlaneSurfaces == null)
                {
                    hasInput = false;
                    Params.Owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Input parameters failed to collect data");
                    return;
                }
                else
                    hasInput = true;
            }
            public override void SetData(IGH_DataAccess DA)
            {
                // 👉 Checking for cancellation!
                if (CancellationToken.IsCancellationRequested) return;

                if (hasInput)
                    DA.SetData(0, new GsaModelGoo(OutModel));
            }

            public override void DoWork(Action<string, double> ReportProgress, Action Done)
            {
                #region DoWork
                if (hasInput)
                {
                    GsaModel analysisModel = null;
                    if (Models != null)
                    {
                        if (Models.Count > 0)
                        {
                            if (Models.Count > 1)
                            {
                                ReportProgress("Merging models...", -2);
                                analysisModel = Util.Gsa.ToGSA.Models.MergeModel(Models);
                                ReportProgress("Models merged", -1);
                            }
                            else
                            {
                                analysisModel = Models[0].Clone();
                                ReportProgress("Model cloned", -1);
                            }
                        }
                    }
                    if (analysisModel != null)
                        OutModel = analysisModel;
                    else
                        OutModel = new GsaModel();

                    // Assemble model
                    ReportProgress("Assembling model...", -2);
                    Model gsa = Util.Gsa.ToGSA.Assemble.AssembleModel(analysisModel, Nodes, Elem1ds, Elem2ds, Elem3ds, Mem1ds, Mem2ds, Mem3ds, Sections, Prop2Ds, Loads, GridPlaneSurfaces, this, ReportProgress);
                    if (gsa == null) { return; }
                    ReportProgress("Model assembled", -1);

                    #region meshing
                    // Create elements from members
                    if (ReMesh)
                    {
                        ReportProgress("Meshing", 0);
                        gsa.CreateElementsFromMembers();
                        ReportProgress("Model meshed", -1);
                    }
                    #endregion

                    #region analysis
                    //analysis
                    if (Analysis)
                    {
                        
                        IReadOnlyDictionary<int, AnalysisTask> gsaTasks = gsa.AnalysisTasks();
                        if (gsaTasks.Count < 1)
                        {
                            ReportProgress("Model contains no Analysis Tasks", -255);
                            ReportProgress("Model assembled", -1);
                        }
                        else
                        {
                            foreach (KeyValuePair<int, AnalysisTask> task in gsaTasks)
                            {
                                if (CancellationToken.IsCancellationRequested) return;
                                ReportProgress("Analysing Task " + task.Key.ToString(), -2);

                                bool anal;
                                try
                                {
                                    anal = gsa.Analyse(task.Key);
                                }
                                catch (Exception)
                                {

                                    throw;
                                }


                                if (!anal)
                                {
                                    component.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Warning Analysis Case " + task.Key + " could not be analysed");
                                    ReportProgress("Warning Analysis Case " + task.Key + " could not be analysed", -10);
                                }
                            }
                            ReportProgress("Model analysed", -1);
                        }
                    }
                    
                    
                    #endregion
                    OutModel.Model = gsa;
                    Done();
                }
                #endregion
            }
        }
        #region (de)serialization
        public override bool Write(GH_IO.Serialization.GH_IWriter writer)
        {
            writer.SetBoolean("Analyse", Analysis);
            writer.SetBoolean("ReMesh", ReMesh);
            return base.Write(writer);
        }
        public override bool Read(GH_IO.Serialization.GH_IReader reader)
        {
            try
            {
                Analysis = reader.GetBoolean("Analyse");
                ReMesh = reader.GetBoolean("ReMesh");
            }
            catch (Exception)
            {
                Analysis = true;
                ReMesh = true;
            }

            initialCheckState = new List<bool>();
            initialCheckState.Add(Analysis);
            this.CreateAttributes();
            return base.Read(reader);
        }
        #endregion
    }
}