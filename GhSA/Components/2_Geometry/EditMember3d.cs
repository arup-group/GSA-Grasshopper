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
    /// Component to edit a 3D Member
    /// </summary>
    public class EditMember3d : GH_Component, IGH_PreviewObject
    {
        #region Name and Ribbon Layout
        // This region handles how the component in displayed on the ribbon
        // including name, exposure level and icon
        public override Guid ComponentGuid => new Guid("955e573d-7608-4ac6-b436-54135f7714f6");
        public EditMember3d()
          : base("Edit 3D Member", "Mem3dEdit", "Modify GSA 3D Member",
                Ribbon.CategoryName.Name(),
                Ribbon.SubCategoryName.Cat2())
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.secondary | GH_Exposure.obscure;

        protected override System.Drawing.Bitmap Icon => GhSA.Properties.Resources.EditMem3D;
        #endregion

        #region Custom UI
        //This region overrides the typical component layout


        #endregion

        #region Input and output

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            
            pManager.AddGenericParameter("3D Member", "M3D", "GSA 3D Member to Modify", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Member3d Number", "ID", "Set Member Number. If ID is set it will replace any existing 3d Member in the model", GH_ParamAccess.item);
            pManager.AddGeometryParameter("Solid", "S", "Reposition Solid Geometry - Closed Brep or Mesh", GH_ParamAccess.item);
            pManager.AddGenericParameter("3D Property", "P3", "Change 3D Property", GH_ParamAccess.item);
            pManager.AddNumberParameter("Mesh Size", "Ms", "Set target mesh size", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Mesh With Others", "M/o", "Mesh with others?", GH_ParamAccess.item, true);
            pManager.AddTextParameter("Member3d Name", "Na", "Set Name of Member3d", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Member3d Group", "Gr", "Set Member 3d Group", GH_ParamAccess.item);
            pManager.AddColourParameter("Member3d Colour", "Co", "Set Member 3d Colour", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Dummy Member", "Dm", "Set Member to Dummy", GH_ParamAccess.item);

            for (int i = 1; i < pManager.ParamCount; i++)
                pManager[i].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("3D Member", "M3D", "Modified GSA 3D Member", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Member Number", "ID", "Get Member Number", GH_ParamAccess.item);
            pManager.AddMeshParameter("Solid Mesh", "M", "Member Solid Mesh", GH_ParamAccess.item);
            pManager.HideParameter(2);
            pManager.AddGenericParameter("3D Property", "P3", "Get 3D Property", GH_ParamAccess.item);
            pManager.AddNumberParameter("Mesh Size", "Ms", "Get Targe mesh size", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Mesh With Others", "M/o", "Get if to mesh with others", GH_ParamAccess.item);
            pManager.AddTextParameter("Member Name", "Na", "Get Name of Member", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Member Group", "Gr", "Get Member Group", GH_ParamAccess.item);
            pManager.AddColourParameter("Member Colour", "Co", "Get Member Colour", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Dummy Member", "Dm", "Get if Member is Dummy", GH_ParamAccess.item);
        }
        #endregion

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GsaMember3d gsaMember3d = new GsaMember3d();
            if (DA.GetData(0, ref gsaMember3d))
            {
                if (gsaMember3d == null) { AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Member3D input is null"); }
                GsaMember3d mem = gsaMember3d.Duplicate();

                // #### inputs ####

                // 1 ID
                GH_Integer ghID = new GH_Integer();
                if (DA.GetData(1, ref ghID))
                {
                    if (GH_Convert.ToInt32(ghID, out int id, GH_Conversion.Both))
                        mem.ID = id;
                }

                // 2 geometry
                GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
                if (DA.GetData(2, ref gh_typ))
                {
                    GsaMember3d tempMem = new GsaMember3d();
                    Brep brep = new Brep();
                    Mesh mesh = new Mesh();
                    if (GH_Convert.ToBrep(gh_typ.Value, ref brep, GH_Conversion.Both))
                        tempMem = new GsaMember3d(brep);
                    else if (GH_Convert.ToMesh(gh_typ.Value, ref mesh, GH_Conversion.Both))
                        tempMem = new GsaMember3d(mesh);
                    else
                    {
                        AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert Geometry input to a 3D Member");
                        return;
                    }
                    mem.SolidMesh = tempMem.SolidMesh;
                }

                // 3 prop3d -- to be implemented GsaAPI
                gh_typ = new GH_ObjectWrapper();
                if (DA.GetData(3, ref gh_typ))
                {
                    if (GH_Convert.ToInt32(gh_typ.Value, out int idd, GH_Conversion.Both))
                        mem.Member.Property = idd;
                    //GsaProp3d prop3d = new GsaProp3d();
                    //if (gh_typ.Value is GsaProp3dGoo)
                    //    gh_typ.CastTo(ref prop3d);
                    //else
                    //{
                    //    if (GH_Convert.ToInt32(gh_typ.Value, out int idd, GH_Conversion.Both))
                    //        prop3d.ID = idd;
                    //    else
                    //    {
                    //        AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert PA input to a 3D Property of reference integer");
                    //        return;
                    //    }
                    //}
                    //mem.Property = prop3d;
                }

                // 4 mesh size
                GH_Number ghmsz = new GH_Number();
                if (DA.GetData(4, ref ghmsz))
                {
                    if (GH_Convert.ToDouble(ghmsz, out double msz, GH_Conversion.Both))
                        mem.Member.MeshSize = msz;
                }

                // 5 mesh with others
                GH_Boolean ghbool = new GH_Boolean();
                if (DA.GetData(5, ref ghbool))
                {
                    if (GH_Convert.ToBoolean(ghbool, out bool mbool, GH_Conversion.Both))
                    {
                        //mem.member.MeshWithOthers
                    }
                }

                // 6 name
                GH_String ghnm = new GH_String();
                if (DA.GetData(6, ref ghnm))
                {
                    if (GH_Convert.ToString(ghnm, out string name, GH_Conversion.Both))
                        mem.Member.Name = name;
                }

                // 7 Group
                GH_Integer ghgrp = new GH_Integer();
                if (DA.GetData(7, ref ghgrp))
                {
                    if (GH_Convert.ToInt32(ghgrp, out int grp, GH_Conversion.Both))
                        mem.Member.Group = grp;
                }

                // 8 Colour
                GH_Colour ghcol = new GH_Colour();
                if (DA.GetData(8, ref ghcol))
                {
                    if (GH_Convert.ToColor(ghcol, out System.Drawing.Color col, GH_Conversion.Both))
                        mem.Member.Colour = col;
                }

                // 9 Dummy
                GH_Boolean ghdum = new GH_Boolean();
                if (DA.GetData(9, ref ghdum))
                {
                    if (GH_Convert.ToBoolean(ghdum, out bool dum, GH_Conversion.Both))
                        mem.Member.IsDummy = dum;
                }

                // #### outputs ####

                DA.SetData(0, new GsaMember3dGoo(mem));
                DA.SetData(1, mem.ID);
                DA.SetData(2, mem.SolidMesh);

                //DA.SetData(3, mem.Property);

                DA.SetData(4, mem.Member.MeshSize);
                //DA.SetData(5, mem.Member.MeshWithOthers);

                DA.SetData(6, mem.Member.Name);
                DA.SetData(7, mem.Member.Group);
                DA.SetData(8, mem.Member.Colour);
                DA.SetData(9, mem.Member.IsDummy);
            }
        }
    }
}

