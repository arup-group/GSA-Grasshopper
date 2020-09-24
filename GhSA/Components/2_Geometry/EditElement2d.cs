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
    public class gsaElement2dEdit : GH_Component, IGH_PreviewObject
    {
        #region Name and Ribbon Layout
        // This region handles how the component in displayed on the ribbon
        // including name, exposure level and icon
        public override Guid ComponentGuid => new Guid("e9611aa7-88c1-4b5b-83d6-d9629e21ad8a");
        public gsaElement2dEdit()
          : base("Edit 2D Element", "Elem2dEdit", "Modify GSA 2D Element",
                Ribbon.CategoryName.name(),
                Ribbon.SubCategoryName.cat2())
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.secondary | GH_Exposure.obscure;

        //protected override Bitmap Icon => Resources.CrossSections;
        #endregion

        #region Custom UI
        //This region overrides the typical component layout
        

        #endregion

        #region Input and output
        
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            
            pManager.AddGenericParameter("2D Element", "Elem2d", "GSA 2D Element to Modify", GH_ParamAccess.item);
            pManager.AddGenericParameter("2D Property", "PA", "Change 2D Property. Input either a GSA 2D Property or an Integer to use a Section already defined in model", GH_ParamAccess.list);
            pManager.AddGenericParameter("Offset", "Off", "Set Element Offset", GH_ParamAccess.list);
            pManager.AddIntegerParameter("2D Analysis Type", "Typ", "Set Element 2D Analysis Type", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Element2d Number", "ID", "Set Element Number. If ID is set it will replace any existing 2d Element in the model", GH_ParamAccess.list);
            pManager.AddTextParameter("Element2d Name", "Na", "Set Name of Element", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Element2d Group", "Grp", "Set Element Group", GH_ParamAccess.list);
            pManager.AddColourParameter("Element2d Colour", "Col", "Set Element Colour", GH_ParamAccess.list);

            pManager[1].Optional = true;
            pManager[2].Optional = true;
            pManager[3].Optional = true;
            pManager[4].Optional = true;
            pManager[5].Optional = true;
            pManager[6].Optional = true;
            pManager[7].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("2D Element", "Elem2d", "Modified GSA 2d Element", GH_ParamAccess.item);
            pManager.AddMeshParameter("Analysis Mesh", "M", "Get Analysis Mesh", GH_ParamAccess.item);
            pManager.AddGenericParameter("2D Property", "PA", "Get 2D Property. Input either a GSA 2D Property or an Integer to use a Section already defined in model", GH_ParamAccess.list);
            pManager.AddGenericParameter("Offset", "Off", "Get Element Offset", GH_ParamAccess.list);
            pManager.AddIntegerParameter("2D Analysis Type", "Typ", "Get Element 2D Analysis Type", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Number", "ID", "Get Element Number", GH_ParamAccess.list);
            pManager.AddTextParameter("Name", "Na", "Set Element Name", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Group", "Grp", "Get Element Group", GH_ParamAccess.list);
            pManager.AddColourParameter("Colour", "Col", "Get Element Colour", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Parent Members", "ParM", "Get Parent Member IDs in Model that Element was created from", GH_ParamAccess.list);
        }
        #endregion

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GsaElement2d gsaElement2d = new GsaElement2d();
            if (DA.GetData(0, ref gsaElement2d))
            {
                GsaElement2d elem = new GsaElement2d();
                elem = gsaElement2d.Duplicate();

                // #### inputs ####

                // no good way of updating location of mesh on the fly // 
                // suggest users re-create from scratch //

                // 1 section
                List<GsaProp2d> prop2ds = new List<GsaProp2d>();
                List<GH_Integer> gh_sec_idd = new List<GH_Integer>();
                if (DA.GetDataList(1, prop2ds))
                {
                    elem.Properties = prop2ds;
                }
                else if (DA.GetDataList(1, gh_sec_idd))
                {
                    int idd = 0;
                    for (int i = 0; i < gh_sec_idd.Count; i++)
                    {
                        if (GH_Convert.ToInt32(gh_sec_idd[i], out idd, GH_Conversion.Both))
                        {
                            GsaProp2d prop2d = new GsaProp2d();
                            prop2d.ID = idd;
                            prop2ds.Add(prop2d);
                        }
                    }
                    elem.Properties = prop2ds;
                }

                // 2 offset
                List<GsaOffset> offset = new List<GsaOffset>();
                if (DA.GetDataList(2, offset))
                {
                    for (int i = 0; i < offset.Count; i++)
                        elem.Elements[i].Offset.Z = offset[i].Z;
                }

                // 3 element type / analysis order
                List<GH_Integer> ghinteg = new List<GH_Integer>();
                if (DA.GetDataList(3, ghinteg))
                {
                    int type = new int();
                    for (int i = 0; i < ghinteg.Count; i++)
                    {
                        if (GH_Convert.ToInt32(ghinteg[i], out type, GH_Conversion.Both))
                        {
                            //elem.Elements[i].Type = Util.Gsa.GsaToModel.Element2dType(type); Note: Type on 2D element should be analysis order - GsaAPI bug?
                        }
                    }
                }

                // 4 ID
                List<GH_Integer> ghID = new List<GH_Integer>();
                if (DA.GetDataList(4, ghID))
                {
                    int id = new int();
                    for (int i = 0; i < ghID.Count; i++)
                    {
                        if (GH_Convert.ToInt32(ghID[i], out id, GH_Conversion.Both))
                        {
                            elem.ID[i] = id;
                        }
                    }
                        
                }

                // 5 name
                List<GH_String> ghnm = new List<GH_String>();
                if (DA.GetDataList(5, ghnm))
                {
                    string name = "";
                    for (int i = 0; i < ghnm.Count; i++)
                    {
                        if (GH_Convert.ToString(ghnm[i], out name, GH_Conversion.Both))
                        {
                            elem.Elements[i].Name = name;
                        }
                    }
                }

                // 6 Group
                List<GH_Integer> ghgrp = new List<GH_Integer>();
                if (DA.GetDataList(6, ghgrp))
                {
                    int grp = new int();
                    for (int i = 0; i < ghgrp.Count; i++)
                    {
                        if (GH_Convert.ToInt32(ghgrp[i], out grp, GH_Conversion.Both))
                        {
                            elem.Elements[i].Group = grp;
                        }
                    }
                }

                // 7 Colour
                List<GH_Colour> ghcol = new List<GH_Colour>();
                if (DA.GetDataList(7, ghcol))
                {
                    System.Drawing.Color col = new System.Drawing.Color();
                    for (int i = 0; i < ghcol.Count; i++)
                    {
                        if (GH_Convert.ToColor(ghcol[i], out col, GH_Conversion.Both))
                        {
                            elem.Elements[i].Colour = col;
                        }
                    }
                }


                // #### outputs ####

                DA.SetData(0, new GsaElement2dGoo(elem));
                DA.SetData(1, elem.Mesh);

                List<GsaOffset> offsets = new List<GsaOffset>();
                //List<int> anal = new List<int>();
                List<int> ids = new List<int>();
                List<string> names = new List<string>();
                List<int> groups = new List<int>();
                List<System.Drawing.Color> colours = new List<System.Drawing.Color>();
                List<int> pmems = new List<int>();
                for (int i = 0; i < elem.Elements.Count; i++)
                {
                    GsaOffset offset1 = new GsaOffset();
                    offset1.Z = elem.Elements[i].Offset.Z;
                    offsets.Add(offset1);
                    //anal.Add(gsaElement2d.Elements[i].Type);
                    names.Add(elem.Elements[i].Name);
                    groups.Add(elem.Elements[i].Group);
                    colours.Add((System.Drawing.Color)elem.Elements[i].Colour);
                    try { pmems.Add(elem.Elements[i].ParentMember.Member); } catch (Exception) { pmems.Add(0); }
                    ;
                }
                DA.SetDataList(2, elem.Properties); 
                DA.SetDataList(3, offsets);
                //DA.SetDataList(4, anal);
                DA.SetDataList(5, elem.ID);
                DA.SetDataList(6, names);
                DA.SetDataList(7, groups);
                DA.SetDataList(8, colours);
                DA.SetDataList(9, pmems);
            }
        }
    }
}

