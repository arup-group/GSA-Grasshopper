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
    /// Component to create new 3d Member
    /// </summary>
    public class CreateMember3d : GH_Component, IGH_PreviewObject
    {
        #region Name and Ribbon Layout
        // This region handles how the component in displayed on the ribbon
        // including name, exposure level and icon
        public override Guid ComponentGuid => new Guid("df0c7608-9e46-4500-ab63-0c4162a580d4");
        public CreateMember3d()
          : base("Create 3D Member", "Mem3D", "Create GSA Member 3D",
                Ribbon.CategoryName.Name(),
                Ribbon.SubCategoryName.Cat2())
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.primary;

        protected override System.Drawing.Bitmap Icon => GhSA.Properties.Resources.CreateMem3D;
        #endregion

        #region Custom UI
        //This region overrides the typical component layout


        #endregion

        #region Input and output

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGeometryParameter("Solid", "S", "Solid Geometry - Closed Brep or Mesh", GH_ParamAccess.item);
            pManager.AddGenericParameter("3D Prop", "P3", "3D Property", GH_ParamAccess.item);
            pManager.AddNumberParameter("Mesh Size", "Ms", "Targe mesh size", GH_ParamAccess.item, 0);

            pManager[1].Optional = true;
            pManager[2].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("3D Member", "M3D", "GSA 3D Member", GH_ParamAccess.item);
        }
        #endregion

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
            if (DA.GetData(0, ref gh_typ))
            {
                if (gh_typ == null) { AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Solid input is null"); }
                GsaMember3d mem = new GsaMember3d();
                Brep brep = new Brep();
                Mesh mesh = new Mesh();
                if (GH_Convert.ToBrep(gh_typ.Value, ref brep, GH_Conversion.Both))
                    mem = new GsaMember3d(brep);
                else if (GH_Convert.ToMesh(gh_typ.Value, ref mesh, GH_Conversion.Both))
                    mem = new GsaMember3d(mesh);
                else
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert Geometry input to a 3D Member");
                    return;
                }

                // 1 prop3d to be implemented GsaAPI
                gh_typ = new GH_ObjectWrapper();
                //GsaProp2d prop2d = new GsaProp2d();
                if (DA.GetData(1, ref gh_typ))
                {
                    //if (gh_typ.Value is GsaProp2dGoo)
                    //{
                    //    gh_typ.CastTo(ref prop2d);
                    //    mem.Property = prop2d;
                    //}
                    //else
                    //{
                    if (GH_Convert.ToInt32(gh_typ.Value, out int idd, GH_Conversion.Both))
                        mem.PropertyID = idd; //new GsaProp3d(idd);
                    else
                    {
                        AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert PA input to a 2D Property of reference integer");
                        return;
                    }
                    //}
                }

                // 2 mesh size
                GH_Number ghmsz = new GH_Number();
                if (DA.GetData(2, ref ghmsz))
                {
                    GH_Convert.ToDouble(ghmsz, out double m_size, GH_Conversion.Both);
                    mem.MeshSize = m_size;
                }

                DA.SetData(0, new GsaMember3dGoo(mem));
            }
        }
    }
}

