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
    /// Dummy component with dropdown functionality, showing how custom UI class UI.DropDownComponentUI shall be called
    /// </summary>
    public class DropDownTest2 : GH_Component, IGH_PreviewObject
    {
        #region Name and Ribbon Layout
        // This region handles how the component in displayed on the ribbon
        // including name, exposure level and icon
        public override Guid ComponentGuid => new Guid("d73be4e3-887f-4507-8400-99687460f31c");
        public DropDownTest2()
          : base("DropDownSingle", "DropDown1", "Create GSA Node Support",
                Ribbon.CategoryName.name(),
                Ribbon.SubCategoryName.cat8())
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.primary;

        //protected override Bitmap Icon => Resources.CrossSections;
        #endregion

        #region (de)serialization 
        public override bool Write(GH_IO.Serialization.GH_IWriter writer) 
        {
            // we need to save all the items that we want to reappear when a GH file is saved and re-opened
            writer.SetString("selected", selecteditem);
            return base.Write(writer);
        }
        public override bool Read(GH_IO.Serialization.GH_IReader reader)
        {
            // when a GH file is opened we need to read in the data that was previously set by user
            selecteditem = (string)reader.GetString("selected");
            
            // we need to recreate the custom UI again as this is created before this read IO is called
            // otherwise the component will not display the selected item on the canvas
            this.CreateAttributes(); 
            return base.Read(reader);
        }
        #endregion

        #region Custom UI
        //This region overrides the typical component layout

        
        public override void CreateAttributes()
        {
            m_attributes = new UI.DropDownComponentUI(this, setSelected, dropdownitems, selecteditem, "Concrete Grade");
        }

        public void setSelected(string selected)
        {
            selecteditem = selected;
        }
        #endregion

        #region Input and output
        List<string> dropdownitems = new List<string>(new string[]
        {
            "C20/25", "C30/37", "C40/50"
        });

        string selecteditem;

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("Point", "Pt", "Point (x, y, z) location of support", GH_ParamAccess.item);
            pManager.AddPlaneParameter("Plane", "Pl", "(Optional) Plane for local axis", GH_ParamAccess.item, Plane.WorldXY);
            pManager.AddGenericParameter("Bool6", "Re", "(Optional) Restraint in Bool6 form", GH_ParamAccess.item);
            pManager.AddGenericParameter("Spring", "Sp", "(Optional) GSA Spring", GH_ParamAccess.item);

            pManager[1].Optional = true;
            pManager.HideParameter(1);
            pManager[2].Optional = true;
            pManager[3].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("GSA Node", "Node", "GSA Node with Restraint", GH_ParamAccess.list);
            
        }
        #endregion

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            
        }

        #region UI override

        #endregion
    }
}

