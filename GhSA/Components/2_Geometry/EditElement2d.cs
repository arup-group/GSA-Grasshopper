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
    /// Component to edit a 2D Element
    /// </summary>
    public class EditElement2d : GH_Component, IGH_PreviewObject
    {
        #region Name and Ribbon Layout
        // This region handles how the component in displayed on the ribbon
        // including name, exposure level and icon
        public override Guid ComponentGuid => new Guid("e9611aa7-88c1-4b5b-83d6-d9629e21ad8a");
        public EditElement2d()
          : base("Edit 2D Element", "Elem2dEdit", "Modify GSA 2D Element",
                Ribbon.CategoryName.Name(),
                Ribbon.SubCategoryName.Cat2())
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.secondary | GH_Exposure.obscure;

        protected override System.Drawing.Bitmap Icon => GhSA.Properties.Resources.EditElem2D;
        #endregion

        #region Custom UI
        //This region overrides the typical component layout


        #endregion

        #region Input and output

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            
            pManager.AddGenericParameter("2D Element", "E2D", "GSA 2D Element to Modify", GH_ParamAccess.item);
            pManager.AddGenericParameter("2D Property", "PA", "Change 2D Property. Input either a GSA 2D Property or an Integer to use a Section already defined in model", GH_ParamAccess.list);
            pManager.AddGenericParameter("Offset", "Of", "Set Element Offset", GH_ParamAccess.list);
            pManager.AddIntegerParameter("2D Analysis Type", "Ty", "Set Element 2D Analysis Type", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Element2d Number", "ID", "Set Element Number. If ID is set it will replace any existing 2d Element in the model", GH_ParamAccess.list);
            pManager.AddTextParameter("Element2d Name", "Na", "Set Name of Element", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Element2d Group", "Gr", "Set Element Group", GH_ParamAccess.list);
            pManager.AddColourParameter("Element2d Colour", "Co", "Set Element Colour", GH_ParamAccess.list);

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
            pManager.AddGenericParameter("2D Element", "E2D", "Modified GSA 2d Element", GH_ParamAccess.item);
            pManager.AddMeshParameter("Analysis Mesh", "M", "Get Analysis Mesh", GH_ParamAccess.item);
            pManager.AddGenericParameter("2D Property", "PA", "Get 2D Property. Input either a GSA 2D Property or an Integer to use a Section already defined in model", GH_ParamAccess.list);
            pManager.AddGenericParameter("Offset", "Of", "Get Element Offset", GH_ParamAccess.list);
            pManager.AddIntegerParameter("2D Analysis Type", "Ty", "Get Element 2D Analysis Type", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Number", "ID", "Get Element Number", GH_ParamAccess.list);
            pManager.AddTextParameter("Name", "Na", "Set Element Name", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Group", "Gr", "Get Element Group", GH_ParamAccess.list);
            pManager.AddColourParameter("Colour", "Co", "Get Element Colour", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Parent Members", "pM", "Get Parent Member IDs in Model that Element was created from", GH_ParamAccess.list);
        }
        #endregion

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GsaElement2d gsaElement2d = new GsaElement2d();
            if (DA.GetData(0, ref gsaElement2d))
            {
                GsaElement2d elem = gsaElement2d.Clone();

                // #### inputs ####

                // no good way of updating location of mesh on the fly // 
                // suggest users re-create from scratch //

                // 1 section
                List<GH_ObjectWrapper> gh_types = new List<GH_ObjectWrapper>();
                if (DA.GetDataList(2, gh_types))
                {
                    for (int i = 0; i< gh_types.Count; i++)
                    {
                        GH_ObjectWrapper gh_typ = gh_types[i];
                        GsaProp2d prop2d = new GsaProp2d();
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
                        List<GsaProp2d> prop2Ds = new List<GsaProp2d>();
                        for (int j = 0; j < elem.Elements.Count; j++)
                            prop2Ds.Add(prop2d);
                        elem.Properties = prop2Ds;
                    }
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
                    for (int i = 0; i < ghinteg.Count; i++)
                    {
                        if (GH_Convert.ToInt32(ghinteg[i], out int type, GH_Conversion.Both))
                        {
                            //elem.Elements[i].Type = Util.Gsa.GsaToModel.Element2dType(type); Note: Type on 2D element should be analysis order - GsaAPI bug?
                        }
                    }
                }

                // 4 ID
                List<GH_Integer> ghID = new List<GH_Integer>();
                if (DA.GetDataList(4, ghID))
                {
                    for (int i = 0; i < ghID.Count; i++)
                    {
                        if (GH_Convert.ToInt32(ghID[i], out int id, GH_Conversion.Both))
                            elem.ID[i] = id;
                    }
                }

                // 5 name
                List<GH_String> ghnm = new List<GH_String>();
                if (DA.GetDataList(5, ghnm))
                {
                    for (int i = 0; i < ghnm.Count; i++)
                    {
                        if (GH_Convert.ToString(ghnm[i], out string name, GH_Conversion.Both))
                            elem.Elements[i].Name = name;
                    }
                }

                // 6 Group
                List<GH_Integer> ghgrp = new List<GH_Integer>();
                if (DA.GetDataList(6, ghgrp))
                {
                    for (int i = 0; i < ghgrp.Count; i++)
                    {
                        if (GH_Convert.ToInt32(ghgrp[i], out int grp, GH_Conversion.Both))
                            elem.Elements[i].Group = grp;
                    }
                }

                // 7 Colour
                List<GH_Colour> ghcol = new List<GH_Colour>();
                if (DA.GetDataList(7, ghcol))
                {
                    for (int i = 0; i < ghcol.Count; i++)
                    {
                        if (GH_Convert.ToColor(ghcol[i], out System.Drawing.Color col, GH_Conversion.Both))
                            elem.Elements[i].Colour = col;
                    }
                }


                // #### outputs ####

                DA.SetData(0, new GsaElement2dGoo(elem));
                DA.SetData(1, elem.Mesh);

                List<GsaOffset> offsets = new List<GsaOffset>();
                //List<int> anal = new List<int>();
                List<string> names = new List<string>();
                List<int> groups = new List<int>();
                List<System.Drawing.Color> colours = new List<System.Drawing.Color>();
                List<int> pmems = new List<int>();
                for (int i = 0; i < elem.Elements.Count; i++)
                {
                    GsaOffset offset1 = new GsaOffset
                    {
                        Z = elem.Elements[i].Offset.Z
                    };
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

