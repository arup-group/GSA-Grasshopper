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
using System.Linq;
using Rhino.Collections;

namespace GhSA.Components
{
    /// <summary>
    /// Component to edit a 2D Member
    /// </summary>
    public class EditMember2d : GH_Component, IGH_PreviewObject
    {
        #region Name and Ribbon Layout
        // This region handles how the component in displayed on the ribbon
        // including name, exposure level and icon
        public override Guid ComponentGuid => new Guid("955e572d-1293-4ac6-b436-54135f7714f6");
        public EditMember2d()
          : base("Edit 2D Member", "Mem2dEdit", "Modify GSA 2D Member",
                Ribbon.CategoryName.Name(),
                Ribbon.SubCategoryName.Cat2())
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.secondary | GH_Exposure.obscure;

        protected override System.Drawing.Bitmap Icon => GhSA.Properties.Resources.EditMem2D;
        #endregion

        #region Custom UI
        //This region overrides the typical component layout


        #endregion

        #region Input and output

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            
            pManager.AddGenericParameter("2D Member", "M2D", "GSA 2D Member to Modify", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Member2d Number", "ID", "Set Member Number. If ID is set it will replace any existing 2d Member in the model", GH_ParamAccess.item);
            pManager.AddBrepParameter("Brep", "B", "Reposition Member Brep (non-planar geometry will be automatically converted to an average plane from exterior boundary control points)", GH_ParamAccess.item);
            pManager.AddPointParameter("Incl. Points", "(P)", "Add inclusion points (will automatically be projected onto Brep)", GH_ParamAccess.list);
            pManager.AddCurveParameter("Incl. Curves", "(C)", "Add inclusion curves (will automatically be made planar and projected onto brep, and converted to Arcs and Lines)", GH_ParamAccess.list);
            pManager.AddGenericParameter("2D Property", "PA", "Change Section Property. Input either a GSA 2D Property or an Integer to use a Section already defined in model", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Member2d Group", "Gr", "Set Member 2d Group", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Member Type", "mT", "Set 2D Member Type" + System.Environment.NewLine
                + "Default is 1: Generic 2D - Accepted inputs are:" + System.Environment.NewLine +
                "4: Slab" + System.Environment.NewLine +
                "5: Wall" + System.Environment.NewLine +
                "7: Ribbed Slab" + System.Environment.NewLine +
                "12: Void-cutter", GH_ParamAccess.item);
            pManager.AddIntegerParameter("2D Element Type", "aT", "Set Member 2D Analysis Element Type" + System.Environment.NewLine +
                "Accepted inputs are:" + System.Environment.NewLine +
                "0: Linear - Tri3/Quad4 Elements (default)" + System.Environment.NewLine +
                "1: Quadratic - Tri6/Quad8 Elements" + System.Environment.NewLine +
                "2: Rigid Diaphragm", GH_ParamAccess.item);

            pManager.AddGenericParameter("Offset", "Of", "Set Member Offset", GH_ParamAccess.item);
            
            pManager.AddNumberParameter("Mesh Size", "Ms", "Set target mesh size", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Mesh With Others", "M/o", "Mesh with others?", GH_ParamAccess.item, true);
            pManager.AddTextParameter("Member2d Name", "Na", "Set Name of Member2d", GH_ParamAccess.item);
            pManager.AddColourParameter("Member2d Colour", "Co", "Set Member 2d Colour", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Dummy Member", "Dm", "Set Member to Dummy", GH_ParamAccess.item);

            for (int i = 1; i < pManager.ParamCount; i++)
                pManager[i].Optional = true;

            pManager.HideParameter(0);
            pManager.HideParameter(2);
            pManager.HideParameter(3);
            pManager.HideParameter(4);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("2D Member", "M2D", "Modified GSA 2D Member", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Member Number", "ID", "Get Member Number", GH_ParamAccess.item);
            pManager.AddBrepParameter("Brep", "B", "Member Brep", GH_ParamAccess.item);
            pManager.HideParameter(2);
            pManager.AddPointParameter("Incl. Points", "(P)", "Get Inclusion points", GH_ParamAccess.list);
            pManager.HideParameter(3);
            pManager.AddCurveParameter("Incl. Curves", "(C)", "Get Inclusion curves", GH_ParamAccess.list);
            pManager.HideParameter(4);
            pManager.AddGenericParameter("2D Property", "PA", "Get 2D Section Property", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Member Group", "Gr", "Get Member Group", GH_ParamAccess.item);

            pManager.AddIntegerParameter("Member Type", "mT", "Get 2D Member Type", GH_ParamAccess.item);
            pManager.AddIntegerParameter("2D Element Type", "aT", "Get Member 2D Analysis Element Type" + System.Environment.NewLine + 
                "0: Linear (Tri3/Quad4), 1: Quadratic (Tri6/Quad8), 2: Rigid Diaphragm", GH_ParamAccess.item);

            pManager.AddGenericParameter("Offset", "Of", "Get Member Offset", GH_ParamAccess.item);
            pManager.AddNumberParameter("Mesh Size", "Ms", "Get Targe mesh size", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Mesh With Others", "M/o", "Get if to mesh with others", GH_ParamAccess.item);
            
            pManager.AddTextParameter("Member Name", "Na", "Get Name of Member", GH_ParamAccess.item);
            pManager.AddColourParameter("Member Colour", "Co", "Get Member Colour", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Dummy Member", "Dm", "Get if Member is Dummy", GH_ParamAccess.item);
        }
        #endregion

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GsaMember2d gsaMember2d = new GsaMember2d();
            if (DA.GetData(0, ref gsaMember2d))
            {
                if (gsaMember2d == null) { AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Member2D input is null"); }
                GsaMember2d mem = gsaMember2d.Duplicate();

                // #### inputs ####

                // 1 ID
                GH_Integer ghID = new GH_Integer();
                if (DA.GetData(1, ref ghID))
                {
                    if (GH_Convert.ToInt32(ghID, out int id, GH_Conversion.Both))
                        mem.ID = id;
                }

                // 2/3/4 Brep, incl.pts and incl.lns
                Brep brep = mem.Brep; //existing brep
                GH_Brep ghbrep = new GH_Brep();
                CurveList crvlist = new CurveList(mem.InclusionLines);
                List<Curve> crvs = crvlist.ToList();
                List<GH_Curve> ghcrvs = new List<GH_Curve>();
                List<GH_Point> ghpts = new List<GH_Point>();
                List<Point3d> pts = mem.InclusionPoints;

                if ((DA.GetData(2, ref ghbrep)) || (DA.GetDataList(3, ghpts)) || (DA.GetDataList(4, ghcrvs)))
                {
                    // 2 brep
                    if (DA.GetData(2, ref ghbrep))
                        if (GH_Convert.ToBrep(ghbrep, ref brep, GH_Conversion.Both))
                            mem.Brep = brep;

                    // 3 inclusion points
                    if (DA.GetDataList(3, ghpts))
                    {
                        for (int i = 0; i < ghpts.Count; i++)
                        {
                            Point3d pt = new Point3d();
                            if (GH_Convert.ToPoint3d(ghpts[i], ref pt, GH_Conversion.Both))
                                pts.Add(pt);
                        }
                    }

                    // 4 inclusion lines
                    if (DA.GetDataList(4, ghcrvs))
                    {
                        for (int i = 0; i < ghcrvs.Count; i++)
                        {
                            Curve crv = null;
                            if (GH_Convert.ToCurve(ghcrvs[i], ref crv, GH_Conversion.Both))
                            {
                                crvs.Add(crv);
                            }
                        }
                    }

                    GsaMember2d tmpmem = new GsaMember2d(brep, crvs, pts);
                    mem.PolyCurve = tmpmem.PolyCurve;
                    mem.Topology = tmpmem.Topology;
                    mem.TopologyType = tmpmem.TopologyType;
                    mem.VoidTopology = tmpmem.VoidTopology;
                    mem.VoidTopologyType = tmpmem.VoidTopologyType;
                    mem.InclusionLines = tmpmem.InclusionLines;
                    mem.IncLinesTopology = tmpmem.IncLinesTopology;
                    mem.IncLinesTopologyType = tmpmem.IncLinesTopologyType;
                    mem.InclusionPoints = tmpmem.InclusionPoints;

                    mem = tmpmem;
                }

                // 5 section
                GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
                
                if (DA.GetData(5, ref gh_typ))
                {
                    GsaProp2d prop2d = new GsaProp2d();
                    if (gh_typ.Value is GsaProp2dGoo)
                    {
                        gh_typ.CastTo(ref prop2d);
                        mem.Property = prop2d;
                        mem.Member.Property = 0;
                    }
                    else
                    {
                        if (GH_Convert.ToInt32(gh_typ.Value, out int idd, GH_Conversion.Both))
                        {
                            mem.Member.Property = idd;
                            mem.Property = null;
                        }
                        else
                        {
                            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert PA input to a 2D Property of reference integer");
                            return;
                        }
                    }
                }

                // 6 Group
                GH_Integer ghgrp = new GH_Integer();
                if (DA.GetData(6, ref ghgrp))
                {
                    if (GH_Convert.ToInt32(ghgrp, out int grp, GH_Conversion.Both))
                        mem.Member.Group = grp;
                }

                // 7 type
                GH_Integer ghint = new GH_Integer();
                if (DA.GetData(7, ref ghint))
                {
                    if (GH_Convert.ToInt32(ghint, out int type, GH_Conversion.Both))
                        mem.Member.Type = (MemberType)type;//Util.Gsa.GsaToModel.Member2dType(type);
                }

                // 8 element type / analysis order
                GH_Integer ghinteg = new GH_Integer();
                if (DA.GetData(8, ref ghinteg))
                {
                    if (GH_Convert.ToInt32(ghinteg, out int type, GH_Conversion.Both))
                        mem.Member.Type2D = (AnalysisOrder)type; //Util.Gsa.GsaToModel.AnalysisOrder(type);
                }

                // 9 offset
                GsaOffset offset = new GsaOffset();
                if (DA.GetData(9, ref offset))
                {
                    mem.Member.Offset.Z = offset.Z;
                }

                // 10 mesh size
                GH_Number ghmsz = new GH_Number();
                if (DA.GetData(10, ref ghmsz))
                {
                    if (GH_Convert.ToDouble(ghmsz, out double msz, GH_Conversion.Both))
                        mem.Member.MeshSize = msz;
                }

                // 11 mesh with others
                GH_Boolean ghbool = new GH_Boolean();
                if (DA.GetData(11, ref ghbool))
                {
                    if (GH_Convert.ToBoolean(ghbool, out bool mbool, GH_Conversion.Both))
                    {
                        //mem.member.MeshWithOthers
                    }
                }

                // 12 name
                GH_String ghnm = new GH_String();
                if (DA.GetData(12, ref ghnm))
                {
                    if (GH_Convert.ToString(ghnm, out string name, GH_Conversion.Both))
                        mem.Member.Name = name;
                }

                // 13 Colour
                GH_Colour ghcol = new GH_Colour();
                if (DA.GetData(13, ref ghcol))
                {
                    if (GH_Convert.ToColor(ghcol, out System.Drawing.Color col, GH_Conversion.Both))
                        mem.Member.Colour = col;
                }

                // 14 Dummy
                GH_Boolean ghdum = new GH_Boolean();
                if (DA.GetData(14, ref ghdum))
                {
                    if (GH_Convert.ToBoolean(ghdum, out bool dum, GH_Conversion.Both))
                        mem.Member.IsDummy = dum;
                }

                // #### outputs ####

                DA.SetData(0, new GsaMember2dGoo(mem));
                DA.SetData(1, mem.ID);
                DA.SetData(2, mem.Brep);
                DA.SetDataList(3, mem.InclusionPoints);
                DA.SetDataList(4, mem.InclusionLines);
                
                DA.SetData(5, mem.Property);
                DA.SetData(6, mem.Member.Group);
                
                DA.SetData(7, mem.Member.Type);
                DA.SetData(8, mem.Member.Type2D);
                
                GsaOffset gsaOffset = new GsaOffset
                {
                    Z = mem.Member.Offset.Z
                };
                DA.SetData(9, new GsaOffsetGoo(gsaOffset));

                DA.SetData(10, mem.Member.MeshSize);
                //DA.SetData(11, mem.Member.MeshWithOthers);
                
                DA.SetData(12, mem.Member.Name);
                DA.SetData(13, mem.Member.Colour);
                DA.SetData(14, mem.Member.IsDummy);
            }
        }
    }
}

