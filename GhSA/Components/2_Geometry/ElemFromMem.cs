using System;
using System.Collections.Generic;
using Grasshopper.Kernel.Attributes;
using Grasshopper.GUI.Canvas;
using Grasshopper.GUI;
using Grasshopper.Kernel;
using Grasshopper;
using Rhino.Geometry;
using System.Windows.Forms;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GhSA.Parameters;
using System.Resources;

namespace GhSA.Components
{
    /// <summary>
    /// Component to edit a Node
    /// </summary>
    public class ElemFromMem : GH_Component, IGH_PreviewObject
    {
        #region Name and Ribbon Layout
        // This region handles how the component in displayed on the ribbon
        // including name, exposure level and icon
        public override Guid ComponentGuid => new Guid("3de73a08-b72c-45e4-a650-e4c6515266c5");
        public ElemFromMem()
          : base("Elements from Members", "ElemFromMem", "Create Elements from Members",
                Ribbon.CategoryName.Name(),
                Ribbon.SubCategoryName.Cat2())
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.tertiary | GH_Exposure.obscure;

        protected override System.Drawing.Bitmap Icon => GhSA.Properties.Resources.CreateElementsFromMembers;
        #endregion

        #region Custom UI
        //This region overrides the typical component layout


        #endregion

        #region Input and output

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("1D Members", "M1D", "GSA 1D Member to create 1D Elements from", GH_ParamAccess.list);
            pManager.AddGenericParameter("1D Members", "M2D", "GSA 2D Member to create 2D Elements from", GH_ParamAccess.list);

            pManager[0].Optional = true;
            pManager[1].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("1D Elements", "E1D", "GSA 1D Elements", GH_ParamAccess.list);
            pManager.AddGenericParameter("2D Elements", "E2D", "GSA 2D Elements", GH_ParamAccess.list);
        }
        #endregion

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            #region inputs
            // Get Member1d input
            GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
            List<GH_ObjectWrapper> gh_types = new List<GH_ObjectWrapper>();
            List<GsaMember1d> in_mem1ds = new List<GsaMember1d>();
            if (DA.GetDataList(0, gh_types))
            {
                for (int i = 0; i < gh_types.Count; i++)
                {
                    gh_typ = gh_types[i];
                    if (gh_typ.Value is GsaMember1dGoo)
                    {
                        GsaMember1d gsamem1 = new GsaMember1d();
                        gh_typ.CastTo(ref gsamem1);
                        in_mem1ds.Add(gsamem1);
                    }
                    else
                    {
                        AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Error in Mem1D input");
                        return;
                    }
                }
            }

            // Get Member2d input
            gh_types = new List<GH_ObjectWrapper>();
            List<GsaMember2d> in_mem2ds = new List<GsaMember2d>();
            if (DA.GetDataList(1, gh_types))
            {
                for (int i = 0; i < gh_types.Count; i++)
                {
                    gh_typ = gh_types[i];
                    if (gh_typ.Value is GsaMember2dGoo)
                    {
                        GsaMember2d gsamem2 = new GsaMember2d();
                        gh_typ.CastTo(ref gsamem2);
                        in_mem2ds.Add(gsamem2);
                    }
                    else
                    {
                        AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Error in Mem2D input");
                        return;
                    }
                }
            }

            if (in_mem1ds.Count < 1 & in_mem2ds.Count < 1)
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Input parameters failed to collect data");
            #endregion

            Model model = GhSA.Util.GsaMesher.GsaReMesh(in_mem2ds, in_mem1ds);

            model.CreateElementsFromMembers();

            Tuple<DataTree<GsaElement1dGoo>, DataTree<GsaElement2dGoo>> elementTuple = Util.Gsa.GsaImport.GsaGetElem(model, "all", true);

            DA.SetDataList(0, elementTuple.Item1.AllData());
            DA.SetDataList(1, elementTuple.Item2.AllData());

            element2ds = elementTuple.Item2.AllData();

        }
        List<GsaElement2dGoo> element2ds;
        public override void DrawViewportMeshes(IGH_PreviewArgs args)
        {

            base.DrawViewportMeshes(args);

            if (element2ds != null)
            {
                foreach (GsaElement2dGoo element in element2ds)
                {
                    if (element == null) { continue; }
                    //Draw shape.
                    if (element.Value.Mesh != null)
                    {
                        if (!(element.Value.Elements[0].ParentMember.Member > 0)) // only draw mesh shading if no parent member exist.
                        {
                            if (this.Attributes.Selected)
                                args.Display.DrawMeshShaded(element.Value.Mesh, UI.Colour.Element2dFaceSelected);
                            else
                                args.Display.DrawMeshShaded(element.Value.Mesh, UI.Colour.Element2dFace);
                        }
                    }
                }
            }
        }

        public override void DrawViewportWires(IGH_PreviewArgs args)
        {
            base.DrawViewportWires(args);

            if (element2ds != null)
            {
                foreach (GsaElement2dGoo element in element2ds)
                {
                    if (element == null) { continue; }
                    //Draw lines
                    if (element.Value.Mesh != null)
                    {
                        if (element.Value.Elements[0].ParentMember.Member > 0) // only draw mesh shading if no parent member exist.
                        {
                            for (int i = 0; i < element.Value.Mesh.TopologyEdges.Count; i++)
                            {
                                if (element.Value.Mesh.TopologyEdges.GetConnectedFaces(i).Length > 1)
                                    args.Display.DrawLine(element.Value.Mesh.TopologyEdges.EdgeLine(i), System.Drawing.Color.FromArgb(255, 229, 229, 229), 1);
                            }
                        }
                        else
                        {
                            if (this.Attributes.Selected)
                            {
                                for (int i = 0; i < element.Value.Mesh.TopologyEdges.Count; i++)
                                    args.Display.DrawLine(element.Value.Mesh.TopologyEdges.EdgeLine(i), UI.Colour.Element2dEdgeSelected, 2);
                            }
                            else
                            {
                                for (int i = 0; i < element.Value.Mesh.TopologyEdges.Count; i++)
                                    args.Display.DrawLine(element.Value.Mesh.TopologyEdges.EdgeLine(i), UI.Colour.Element2dEdge, 1);
                            }
                        }
                    }
                }
            }
        }
    }
}

