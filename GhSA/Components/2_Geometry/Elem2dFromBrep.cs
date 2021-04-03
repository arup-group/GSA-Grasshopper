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
    public class Elem2dFromBrep : GH_Component, IGH_PreviewObject
    {
        #region Name and Ribbon Layout
        // This region handles how the component in displayed on the ribbon
        // including name, exposure level and icon
        public override Guid ComponentGuid => new Guid("4fa7ccd9-530e-4036-b2bf-203017b55611");
        public Elem2dFromBrep()
          : base("Element2d from Brep", "Elem2dFromBrep", "Mesh non-planar Breps",
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
            pManager.AddBrepParameter("Brep", "B", "Planar Brep (non-planar geometry will be automatically converted to an average plane of exterior boundary control points))", GH_ParamAccess.item);
            pManager.AddPointParameter("Incl. Points", "(P)", "Inclusion points (will automatically be projected onto Brep)", GH_ParamAccess.list);
            pManager.AddCurveParameter("Incl. Curves", "(C)", "Inclusion curves (will automatically be made planar and projected onto brep, and converted to Arcs and Lines)", GH_ParamAccess.list);
            pManager.AddGenericParameter("2D Property", "PA", "GSA 2D Property. Input either a GSA 2D Property or an Integer to use a Section already defined in model", GH_ParamAccess.item);
            pManager.AddNumberParameter("Mesh Size", "Ms", "Targe mesh size", GH_ParamAccess.item, 0);

            pManager.HideParameter(0);
            pManager.HideParameter(1);
            pManager.HideParameter(2);

            pManager[1].Optional = true;
            pManager[2].Optional = true;
            pManager[3].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("2D Elements", "E2D", "GSA 2D Elements", GH_ParamAccess.list);
        }
        #endregion

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GH_Brep ghbrep = new GH_Brep();
            if (DA.GetData(0, ref ghbrep))
            {
                if (ghbrep == null) { AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Brep input is null"); }
                Brep brep = new Brep();
                if (GH_Convert.ToBrep(ghbrep, ref brep, GH_Conversion.Both))
                {
                    // 1 Points
                    List<Point3d> pts = new List<Point3d>();
                    List<GH_Point> ghpts = new List<GH_Point>();
                    if (DA.GetDataList(1, ghpts))
                    {
                        for (int i = 0; i < ghpts.Count; i++)
                        {
                            Point3d pt = new Point3d();
                            if (GH_Convert.ToPoint3d(ghpts[i], ref pt, GH_Conversion.Both))
                                pts.Add(pt);
                        }
                    }

                    // 2 Curves
                    List<Curve> crvs = new List<Curve>();
                    List<GH_Curve> ghcrvs = new List<GH_Curve>();
                    if (DA.GetDataList(2, ghcrvs))
                    {
                        for (int i = 0; i < ghcrvs.Count; i++)
                        {
                            Curve crv = null;
                            if (GH_Convert.ToCurve(ghcrvs[i], ref crv, GH_Conversion.Both))
                                crvs.Add(crv);
                        }
                    }
                    
                    // 4 mesh size
                    GH_Number ghmsz = new GH_Number();
                    double meshSize = 0;
                    if (DA.GetData(4, ref ghmsz))
                    {
                        GH_Convert.ToDouble(ghmsz, out double m_size, GH_Conversion.Both);
                        meshSize = m_size;
                    }
                    
                    // build new element2d with brep, crv and pts
                    GsaElement2d elem2d = new GsaElement2d(brep, crvs, pts, meshSize);

                    // 3 section
                    GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
                    GsaProp2d prop2d = new GsaProp2d();
                    if (DA.GetData(3, ref gh_typ))
                    {
                        if (gh_typ.Value is GsaProp2dGoo)
                            gh_typ.CastTo(ref prop2d);
                        else
                        {
                            if (GH_Convert.ToInt32(gh_typ.Value, out int idd, GH_Conversion.Both))
                                prop2d.ID = idd;
                            else
                            {
                                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert PA input to a 2D Property of reference integer");
                                return;
                            }
                        }
                    }
                    else
                        prop2d.ID = 1;
                    List<GsaProp2d> prop2Ds = new List<GsaProp2d>();
                    for (int i = 0; i < elem2d.Elements.Count; i++)
                        prop2Ds.Add(prop2d);
                    elem2d.Properties = prop2Ds;

                    DA.SetData(0, new GsaElement2dGoo(elem2d));
                }
            }
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

