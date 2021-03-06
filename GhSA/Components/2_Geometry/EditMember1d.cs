﻿using System;
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
    /// Component to edit a 1D Member
    /// </summary>
    public class EditMember1d : GH_Component, IGH_PreviewObject
    {
        #region Name and Ribbon Layout
        // This region handles how the component in displayed on the ribbon
        // including name, exposure level and icon
        public override Guid ComponentGuid => new Guid("2a121578-f9ff-4d80-ae90-2982faa425a6");
        public EditMember1d()
          : base("Edit 1D Member", "Mem1dEdit", "Modify GSA 1D Member",
                Ribbon.CategoryName.Name(),
                Ribbon.SubCategoryName.Cat2())
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.secondary;

        protected override System.Drawing.Bitmap Icon => GhSA.Properties.Resources.EditMem1D;
        #endregion

        #region Custom UI
        //This region overrides the typical component layout


        #endregion

        #region Input and output

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            
            pManager.AddGenericParameter("1D Member", "M1D", "GSA 1D Member to Modify", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Member1d Number", "ID", "Set Member Number. If ID is set it will replace any existing 1D Member in the model", GH_ParamAccess.item);
            pManager.AddCurveParameter("Curve", "C", "Member Curve", GH_ParamAccess.item); 
            pManager.AddGenericParameter("Section", "PB", "Change Section Property", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Member1d Group", "Gr", "Set Member 1D Group", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Member Type", "mT", "Set 1D Member Type" + System.Environment.NewLine +
                "Default is 0: Generic 1D - Accepted inputs are:" + System.Environment.NewLine +
                "2: Beam" + System.Environment.NewLine +
                "3: Column" + System.Environment.NewLine +
                "6: Cantilever" + System.Environment.NewLine +
                "8: Compos" + System.Environment.NewLine +
                "9: Pile" + System.Environment.NewLine +
                "11: Void cutter", GH_ParamAccess.item);
            pManager.AddIntegerParameter("1D Element Type", "eT", "Set Element 1D Type" + System.Environment.NewLine +
                "Accepted inputs are:" + System.Environment.NewLine +
                "1: Bar" + System.Environment.NewLine +
                "2: Beam" + System.Environment.NewLine +
                "3: Spring" + System.Environment.NewLine +
                "9: Link" + System.Environment.NewLine +
                "10: Cable" + System.Environment.NewLine +
                "19: Spacer" + System.Environment.NewLine +
                "20: Strut" + System.Environment.NewLine +
                "21: Tie" + System.Environment.NewLine +
                "23: Rod" + System.Environment.NewLine +
                "24: Damper", GH_ParamAccess.item);
            pManager.AddGenericParameter("Offset", "Of", "Set Member Offset", GH_ParamAccess.item);
            pManager.AddGenericParameter("Start release", "⭰", "Set Release (Bool6) at Start of Member", GH_ParamAccess.item);
            pManager.AddGenericParameter("End release", "⭲", "Set Release (Bool6) at End of Member", GH_ParamAccess.item);
            pManager.AddNumberParameter("Orientation Angle", "⭮A", "Set Member Orientation Angle in degrees", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Orientation Node", "⭮N", "Set Member Orientation Node (ID referring to node number in model)", GH_ParamAccess.item);
            pManager.AddNumberParameter("Mesh Size", "Ms", "Set Member Mesh Size", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Mesh With Others", "M/o", "Mesh with others?", GH_ParamAccess.item, true);
            
            pManager.AddTextParameter("Member1d Name", "Na", "Set Name of Member1d", GH_ParamAccess.item);
            pManager.AddColourParameter("Member1d Colour", "Co", "Set Member 1D Colour", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Dummy Member", "Dm", "Set Member to Dummy", GH_ParamAccess.item);

            for (int i = 1; i < pManager.ParamCount; i++)
                pManager[i].Optional = true;

            pManager.HideParameter(0);
            pManager.HideParameter(2);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("1D Member", "M1D", "Modified GSA 1D Member", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Member1d Number", "ID", "Get Member Number", GH_ParamAccess.item);
            pManager.AddCurveParameter("Curve", "C", "Member Curve", GH_ParamAccess.item);
            pManager.HideParameter(2);
            pManager.AddGenericParameter("Section", "PB", "Change Section Property. Input either a GSA Section or an Integer to use a Section already defined in model", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Member Group", "Gr", "Get Member Group", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Member Type", "mT", "Get 1D Member Type", GH_ParamAccess.item);
            pManager.AddIntegerParameter("1D Element Type", "eT", "Get Element 1D Type", GH_ParamAccess.item);
            pManager.AddGenericParameter("Offset", "Of", "Get Member Offset", GH_ParamAccess.item);
            pManager.AddGenericParameter("Start release", "⭰", "Get Release (Bool6) at Start of Member", GH_ParamAccess.item);
            pManager.AddGenericParameter("End release", "⭲", "Get Release (Bool6) at End of Member", GH_ParamAccess.item);
            pManager.AddNumberParameter("Orientation Angle", "⭮A", "Get Member Orientation Angle in degrees", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Orientation Node", "⭮N", "Get Member Orientation Node (ID referring to node number in model)", GH_ParamAccess.item);
            pManager.AddNumberParameter("Mesh Size", "Ms", "Get Member Mesh Size", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Mesh With Others", "M/o", "Get if to mesh with others", GH_ParamAccess.item);
            
            pManager.AddTextParameter("Member Name", "Na", "Get Name of Member1d", GH_ParamAccess.item);
            
            pManager.AddColourParameter("Member Colour", "Co", "Get Member Colour", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Dummy Member", "Dm", "Get it Member is Dummy", GH_ParamAccess.item);
        }
        #endregion

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GsaMember1d gsaMember1d = new GsaMember1d();
            if (DA.GetData(0, ref gsaMember1d))
            {
                if (gsaMember1d == null) { AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Member1D input is null"); }
                GsaMember1d mem = gsaMember1d.Duplicate();

                // #### inputs ####
                // 1 ID
                GH_Integer ghID = new GH_Integer();
                if (DA.GetData(1, ref ghID))
                {
                    if (GH_Convert.ToInt32(ghID, out int id, GH_Conversion.Both))
                        mem.ID = id;
                }

                // 2 curve
                GH_Curve ghcrv = new GH_Curve();
                if (DA.GetData(2, ref ghcrv))
                {
                    Curve crv = null;
                    if (GH_Convert.ToCurve(ghcrv, ref crv, GH_Conversion.Both))
                    {
                        GsaMember1d tempmem = new GsaMember1d(crv);
                        mem.PolyCurve = tempmem.PolyCurve;
                        mem.Topology = tempmem.Topology;
                        mem.TopologyType = tempmem.TopologyType;
                    }
                }

                // 3 section
                GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
                if (DA.GetData(3, ref gh_typ))
                {
                    GsaSection section = new GsaSection();
                    if (gh_typ.Value is GsaSectionGoo)
                    {
                        gh_typ.CastTo(ref section);
                        mem.Section = section;
                        mem.Member.Property = 0;
                    }
                    else
                    {
                        if (GH_Convert.ToInt32(gh_typ.Value, out int idd, GH_Conversion.Both))
                        {
                            mem.Member.Property = idd;
                            mem.Section = null;
                        }
                        else
                        {
                            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert PB input to a Section Property of reference integer");
                            return;
                        }
                    }
                }

                // 4 Group
                GH_Integer ghgrp = new GH_Integer();
                if (DA.GetData(4, ref ghgrp))
                {
                    if (GH_Convert.ToInt32(ghgrp, out int grp, GH_Conversion.Both))
                        mem.Member.Group = grp;
                }

                // 5 type
                GH_Integer ghint = new GH_Integer();
                if (DA.GetData(5, ref ghint))
                {
                    if (GH_Convert.ToInt32(ghint, out int type, GH_Conversion.Both))
                        mem.Member.Type = (MemberType)type;
                }
                
                // 6 element type
                GH_Integer ghinteg = new GH_Integer();
                if (DA.GetData(6, ref ghinteg))
                {
                    if (GH_Convert.ToInt32(ghinteg, out int type, GH_Conversion.Both))
                        mem.Member.Type1D = (ElementType)type; 
                }

                // 7 offset
                GsaOffset offset = new GsaOffset();
                if (DA.GetData(7, ref offset))
                {
                    mem.Member.Offset.X1 = offset.X1;
                    mem.Member.Offset.X2 = offset.X2;
                    mem.Member.Offset.Y = offset.Y;
                    mem.Member.Offset.Z = offset.Z;
                }

                // 8 start release
                GsaBool6 start = new GsaBool6();
                if (DA.GetData(8, ref start))
                {
                    mem.ReleaseStart = start;
                }

                // 9 end release
                GsaBool6 end = new GsaBool6();
                if (DA.GetData(9, ref end))
                {
                    mem.ReleaseEnd = end;
                }

                // 10 orientation angle
                GH_Number ghangle = new GH_Number();
                if (DA.GetData(10, ref ghangle))
                {
                    if (GH_Convert.ToDouble(ghangle, out double angle, GH_Conversion.Both))
                        mem.Member.OrientationAngle = angle;
                }

                // 11 orientation node
                GH_Integer ghori = new GH_Integer();
                if (DA.GetData(11, ref ghori))
                {
                    if (GH_Convert.ToInt32(ghori, out int orient, GH_Conversion.Both))
                        mem.Member.OrientationNode = orient;
                }

                // 12 mesh size
                GH_Number ghmsz = new GH_Number();
                if (DA.GetData(12, ref ghmsz))
                {
                    if (GH_Convert.ToDouble(ghmsz, out double msz, GH_Conversion.Both))
                        mem.Member.MeshSize = msz;
                }

                // 13 mesh with others
                GH_Boolean ghbool = new GH_Boolean();
                if (DA.GetData(13, ref ghbool))
                {
                    if (GH_Convert.ToBoolean(ghbool, out bool mbool, GH_Conversion.Both))
                    {
                        //mem.member.MeshWithOthers
                    }
                }

                // 14 name
                GH_String ghnm = new GH_String();
                if (DA.GetData(14, ref ghnm))
                {
                    if (GH_Convert.ToString(ghnm, out string name, GH_Conversion.Both))
                        mem.Member.Name = name;
                }

                // 15 Colour
                GH_Colour ghcol = new GH_Colour();
                if (DA.GetData(15, ref ghcol))
                {
                    if (GH_Convert.ToColor(ghcol, out System.Drawing.Color col, GH_Conversion.Both))
                        mem.Member.Colour = col;
                }

                // 16 Dummy
                GH_Boolean ghdum = new GH_Boolean();
                if (DA.GetData(16, ref ghdum))
                {
                    if (GH_Convert.ToBoolean(ghdum, out bool dum, GH_Conversion.Both))
                        mem.Member.IsDummy = dum;
                }

                // #### outputs ####
                DA.SetData(0, new GsaMember1dGoo(mem));
                DA.SetData(1, mem.ID);
                DA.SetData(2, mem.PolyCurve);
                DA.SetData(3, new GsaSectionGoo(mem.Section));
                DA.SetData(4, mem.Member.Group);
                DA.SetData(5, mem.Member.Type);
                DA.SetData(6, mem.Member.Type1D);

                GsaOffset gsaOffset = new GsaOffset
                {
                    X1 = mem.Member.Offset.X1,
                    X2 = mem.Member.Offset.X2,
                    Y = mem.Member.Offset.Y,
                    Z = mem.Member.Offset.Z
                };
                DA.SetData(7, new GsaOffsetGoo(gsaOffset));

                DA.SetData(8, mem.ReleaseStart);
                DA.SetData(9, mem.ReleaseEnd);

                DA.SetData(10, mem.Member.OrientationAngle);
                DA.SetData(11, mem.Member.OrientationNode);

                DA.SetData(12, mem.Member.MeshSize);
                //DA.SetData(13, mem.member.MeshSize); //mesh with others bool
                
                DA.SetData(14, mem.Member.Name);
                
                DA.SetData(15, mem.Member.Colour);
                DA.SetData(16, mem.Member.IsDummy);
            }
        }
    }
}

