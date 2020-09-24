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
    public class gsaElement2dInfo : GH_Component, IGH_PreviewObject
    {
        #region Name and Ribbon Layout
        // This region handles how the component in displayed on the ribbon
        // including name, exposure level and icon
        public override Guid ComponentGuid => new Guid("7e02da84-87ab-46ec-abd0-9946c48fee68");
        public gsaElement2dInfo()
          : base("GSA 2D Element Info", "Elem2dInfo", "Get GSA Element2d Info",
                Ribbon.CategoryName.name(),
                Ribbon.SubCategoryName.cat2())
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.hidden;

        //protected override Bitmap Icon => Resources.CrossSections;
        #endregion

        #region Custom UI
        //This region overrides the typical component layout
        

        #endregion

        #region Input and output
        
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("GSA Element2d", "Elem2d", "GSA 2D Element to get a bit more info out of", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("GSA 2D Element", "Elem2d", "GSA 2D Element", GH_ParamAccess.item);
            pManager.HideParameter(0);
            pManager.AddMeshParameter("Analysis Mesh", "Mesh", "Get Analysis Mesh", GH_ParamAccess.item);
            pManager.AddGenericParameter("Section", "Section", "Get Section Property", GH_ParamAccess.list);
            pManager.AddGenericParameter("Offset", "Offset", "Get Element Offset", GH_ParamAccess.list);
            pManager.AddIntegerParameter("2D Analysis Type", "Typ2D", "Get Element 2d Analysis Type", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Element2d Number", "ID", "Get Element Number", GH_ParamAccess.list);
            pManager.AddTextParameter("Element2d Name", "Name", "Set Name of Element2d", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Element2d Group", "Group", "Get Element 2d Group", GH_ParamAccess.list);
            pManager.AddColourParameter("Element2d Colour", "Colour", "Get Element 2d Colour", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Parent Members", "ParentM", "Get Parent Member IDs in Model that Element was created from", GH_ParamAccess.list);
        }
        #endregion

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GsaElement2d gsaElement2d = new GsaElement2d();
            if (DA.GetData(0, ref gsaElement2d))
            {
                if (gsaElement2d != null)
                {
                    DA.SetData(0, new GsaElement2dGoo(gsaElement2d));

                    DA.SetData(1, gsaElement2d.Mesh);

                    List<int> sections = new List<int>();
                    List<GsaOffset> offsets = new List<GsaOffset>();
                    //List<int> anal = new List<int>();
                    List<int> ids = new List<int>();
                    List<string> names = new List<string>();
                    List<int> groups = new List<int>();
                    List<System.Drawing.Color> colours = new List<System.Drawing.Color>();
                    List<int> pmems = new List<int>();
                    for (int i = 0; i < gsaElement2d.Elements.Count; i++)
                    {
                        sections.Add(gsaElement2d.Elements[i].Property);
                        GsaOffset offset = new GsaOffset();
                        offset.Z = gsaElement2d.Elements[i].Offset.Z;
                        offsets.Add(offset);
                        //anal.Add(gsaElement2d.Elements[i].Type);
                        ids.Add(gsaElement2d.Elements[i].Property);
                        names.Add(gsaElement2d.Elements[i].Name);
                        groups.Add(gsaElement2d.Elements[i].Group);
                        colours.Add((System.Drawing.Color)gsaElement2d.Elements[i].Colour);
                        try { pmems.Add(gsaElement2d.Elements[i].ParentMember.Member); } catch (Exception) { pmems.Add(0); }
                        ;
                    }
                    DA.SetDataList(2, sections); //section property to be added
                    DA.SetDataList(3, offsets);
                    //DA.SetDataList(4, anal);
                    DA.SetDataList(5, ids);
                    DA.SetDataList(6, names);
                    DA.SetDataList(7, groups);
                    DA.SetDataList(8, colours);
                    DA.SetDataList(9, pmems);
                }
            }
        }
    }
}

