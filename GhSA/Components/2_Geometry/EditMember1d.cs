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

        //protected override Bitmap Icon => Resources.CrossSections;
        #endregion

        #region Custom UI
        //This region overrides the typical component layout
        

        #endregion

        #region Input and output
        
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            
            pManager.AddGenericParameter("1D Member", "Mem1d", "GSA 1D Member to Modify", GH_ParamAccess.item);
            pManager.AddGenericParameter("Section", "PB", "Change Section Property", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Member Type", "Typ", "Set 1D Member Type", GH_ParamAccess.item);
            pManager.AddIntegerParameter("1D Element Type", "Ty1D", "Set Element 1D Type", GH_ParamAccess.item);
            pManager.AddGenericParameter("Offset", "Off", "Set Member Offset", GH_ParamAccess.item);
            pManager.AddGenericParameter("Start release", "B6-S", "Set Release (Bool6) at Start of Member", GH_ParamAccess.item);
            pManager.AddGenericParameter("End release", "B6-E", "Set Release (Bool6) at End of Member", GH_ParamAccess.item);
            pManager.AddNumberParameter("Orientation Angle", "OrA", "Set Member Orientation Angle in degrees", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Orientation Node", "OrN", "Set Member Orientation Node (ID referring to node number in model)", GH_ParamAccess.item);
            pManager.AddNumberParameter("Mesh Size", "Ms", "Set Member Mesh Size", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Mesh With Others", "M/o", "Mesh with others?", GH_ParamAccess.item, true);
            pManager.AddIntegerParameter("Member1d Number", "ID", "Set Member Number. If ID is set it will replace any existing 1D Member in the model", GH_ParamAccess.item);
            pManager.AddTextParameter("Member1d Name", "Na", "Set Name of Member1d", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Member1d Group", "Grp", "Set Member 1D Group", GH_ParamAccess.item);
            pManager.AddColourParameter("Member1d Colour", "Col", "Set Member 1D Colour", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Dummy Member", "Dum", "Set Member to Dummy", GH_ParamAccess.item);

            pManager[1].Optional = true;
            pManager[2].Optional = true;
            pManager[3].Optional = true;
            pManager[4].Optional = true;
            pManager[5].Optional = true;
            pManager[6].Optional = true;
            pManager[7].Optional = true;
            pManager[8].Optional = true;
            pManager[9].Optional = true;
            pManager[10].Optional = true;
            pManager[11].Optional = true;
            pManager[12].Optional = true;
            pManager[13].Optional = true;
            pManager[14].Optional = true;
            pManager[15].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("1D Member", "Mem1d", "Modified GSA 1D Member", GH_ParamAccess.item);
            pManager.AddCurveParameter("Curve", "Crv", "Member Curve", GH_ParamAccess.item);
            pManager.AddGenericParameter("Section", "PB", "Change Section Property. Input either a GSA Section or an Integer to use a Section already defined in model", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Member Type", "Typ", "Get 1D Member Type", GH_ParamAccess.item);
            pManager.AddIntegerParameter("1D Element Type", "Ty1D", "Get Element 1D Type", GH_ParamAccess.item);
            pManager.AddGenericParameter("Offset", "Off", "Get Member Offset", GH_ParamAccess.item);
            pManager.AddGenericParameter("Start release", "B6-S", "Get Release (Bool6) at Start of Member", GH_ParamAccess.item);
            pManager.AddGenericParameter("End release", "B6-E", "Get Release (Bool6) at End of Member", GH_ParamAccess.item);
            pManager.AddNumberParameter("Orientation Angle", "OrA", "Get Member Orientation Angle in degrees", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Orientation Node", "OrN", "Get Member Orientation Node (ID referring to node number in model)", GH_ParamAccess.item);
            pManager.AddNumberParameter("Mesh Size", "MSz", "Get Member Mesh Size", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Mesh With Others", "MwO", "Get if to mesh with others", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Member1d Number", "ID", "Get Member Number", GH_ParamAccess.item);
            pManager.AddTextParameter("Member Name", "Na", "Get Name of Member1d", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Member Group", "Grp", "Get Member Group", GH_ParamAccess.item);
            pManager.AddColourParameter("Member Colour", "Col", "Get Member Colour", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Dummy Member", "Dum", "Get it Member is Dummy", GH_ParamAccess.item);
        }
        #endregion

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GsaMember1d gsaMember1d = new GsaMember1d();
            if (DA.GetData(0, ref gsaMember1d))
            {
                GsaMember1d mem = gsaMember1d.Duplicate();

                // #### inputs ####

                

                // 1 section
                GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
                if (DA.GetData(1, ref gh_typ))
                {
                    GsaSection section = new GsaSection();
                    if (gh_typ.Value is GsaSection)
                        gh_typ.CastTo(ref section);
                    else if (gh_typ.Value is GH_Number)
                    {
                        if (GH_Convert.ToInt32((GH_Number)gh_typ.Value, out int idd, GH_Conversion.Both))
                            section.ID = idd;
                    }
                    mem.Section = section;
                }
                

                // 2 type
                GH_Integer ghint = new GH_Integer();
                if (DA.GetData(2, ref ghint))
                {
                    if (GH_Convert.ToInt32(ghint, out int type, GH_Conversion.Both))
                        mem.Member.Type = Util.Gsa.GsaToModel.Member1dType(type);
                }
                
                // 3 element type
                GH_Integer ghinteg = new GH_Integer();
                if (DA.GetData(3, ref ghinteg))
                {
                    if (GH_Convert.ToInt32(ghinteg, out int type, GH_Conversion.Both))
                        mem.Member.Type1D = Util.Gsa.GsaToModel.Element1dType(type);
                }

                // 4 offset
                GsaOffset offset = new GsaOffset();
                if (DA.GetData(4, ref offset))
                {
                    mem.Member.Offset.X1 = offset.X1;
                    mem.Member.Offset.X2 = offset.X2;
                    mem.Member.Offset.Y = offset.Y;
                    mem.Member.Offset.Z = offset.Z;
                }

                // 5 start release
                GsaBool6 start = new GsaBool6();
                if (DA.GetData(5, ref start))
                {
                    mem.ReleaseStart = start;
                }

                // 6 end release
                GsaBool6 end = new GsaBool6();
                if (DA.GetData(6, ref end))
                {
                    mem.ReleaseEnd = end;
                }

                // 7 orientation angle
                GH_Number ghangle = new GH_Number();
                if (DA.GetData(7, ref ghangle))
                {
                    if (GH_Convert.ToDouble(ghangle, out double angle, GH_Conversion.Both))
                        mem.Member.OrientationAngle = angle;
                }

                // 8 orientation node
                GH_Integer ghori = new GH_Integer();
                if (DA.GetData(8, ref ghori))
                {
                    if (GH_Convert.ToInt32(ghori, out int orient, GH_Conversion.Both))
                        mem.Member.OrientationNode = orient;
                }

                // 9 mesh size
                GH_Number ghmsz = new GH_Number();
                if (DA.GetData(9, ref ghmsz))
                {
                    if (GH_Convert.ToDouble(ghmsz, out double msz, GH_Conversion.Both))
                        mem.Member.MeshSize = msz;
                }

                // 10 mesh with others
                GH_Boolean ghbool = new GH_Boolean();
                if (DA.GetData(10, ref ghbool))
                {
                    if (GH_Convert.ToBoolean(ghbool, out bool mbool, GH_Conversion.Both))
                    {
                        //mem.member.MeshWithOthers
                    }
                }

                // 11 ID
                GH_Integer ghID = new GH_Integer();
                if (DA.GetData(11, ref ghID))
                {
                    if (GH_Convert.ToInt32(ghID, out int id, GH_Conversion.Both))
                        mem.ID = id;
                }

                // 12 name
                GH_String ghnm = new GH_String();
                if (DA.GetData(12, ref ghnm))
                {
                    if (GH_Convert.ToString(ghnm, out string name, GH_Conversion.Both))
                        mem.Member.Name = name;
                }

                // 13 Group
                GH_Integer ghgrp = new GH_Integer();
                if (DA.GetData(13, ref ghgrp))
                {
                    if (GH_Convert.ToInt32(ghgrp, out int grp, GH_Conversion.Both))
                        mem.Member.Group = grp;
                }

                // 14 Colour
                GH_Colour ghcol = new GH_Colour();
                if (DA.GetData(14, ref ghcol))
                {
                    if (GH_Convert.ToColor(ghcol, out System.Drawing.Color col, GH_Conversion.Both))
                        mem.Member.Colour = col;
                }

                // 15 Dummy
                GH_Boolean ghdum = new GH_Boolean();
                if (DA.GetData(15, ref ghdum))
                {
                    if (GH_Convert.ToBoolean(ghdum, out bool dum, GH_Conversion.Both))
                        mem.Member.IsDummy = dum;
                }

                // #### outputs ####
                DA.SetData(0, new GsaMember1dGoo(mem));
                DA.SetData(1, mem.PolyCurve);
                DA.SetData(2, mem.Section);

                DA.SetData(3, mem.Member.Type);

                DA.SetData(4, mem.Member.Type1D);

                GsaOffset gsaOffset = new GsaOffset
                {
                    X1 = mem.Member.Offset.X1,
                    X2 = mem.Member.Offset.X2,
                    Y = mem.Member.Offset.Y,
                    Z = mem.Member.Offset.Z
                };
                DA.SetData(5, new GsaOffsetGoo(gsaOffset));

                DA.SetData(6, mem.ReleaseStart);
                DA.SetData(7, mem.ReleaseEnd);

                DA.SetData(8, mem.Member.OrientationAngle);
                DA.SetData(9, mem.Member.OrientationNode);

                DA.SetData(10, mem.Member.MeshSize);
                //DA.SetData(11, mem.member.MeshSize); //mesh with others bool

                DA.SetData(12, mem.ID);
                DA.SetData(13, mem.Member.Name);
                DA.SetData(14, mem.Member.Group);
                DA.SetData(15, mem.Member.Colour);

                DA.SetData(16, mem.Member.IsDummy);
            }
        }
    }
}

