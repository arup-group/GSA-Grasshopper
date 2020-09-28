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
using Grasshopper.Documentation;

namespace GhSA.Components
{
    public class EditGsaUnits : GH_Component
    {
        #region Name and Ribbon Layout
        // This region handles how the component in displayed on the ribbon
        // including name, exposure level and icon
        public override Guid ComponentGuid => new Guid("7804c8d8-07b9-43f2-af07-1a056dce8c6d");
        public EditGsaUnits()
          : base("Edit GSA Units", "Unit", "Set GSA Units for this document",
                Ribbon.CategoryName.Name(),
                Ribbon.SubCategoryName.Cat0())
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.tertiary;

        //protected override Bitmap Icon => Resources.CrossSections;
        #endregion

        #region Custom UI
        //This region overrides the typical component layout
        

        #endregion

        #region Input and output
        
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Section Unist", "SecU", "Set units for Sections", GH_ParamAccess.item);
            pManager[0].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Units", "Units", "List of all units in document", GH_ParamAccess.list);
            
        }
        #endregion
        private void UpdateCanvas()
        {
            //none of these below will update the text. 
            //to be implemented
            //method to redraw component descriptions/names when units are updated

            //Instances.ActiveCanvas.Document.DestroyAttributeCache();
            //IGH_Component comp1 = Instances.ActiveCanvas.Document.FindComponent(new Guid("6504a99f-a4e2-4e30-8251-de31ea83e8cb"));
            //comp1.OnAttributesChanged();

            for (int i = 0; i < Instances.ActiveCanvas.Document.Attributes.Count; i++)
            {
                if (Instances.ActiveCanvas.Document.Attributes[i].DocObject.NickName == "Profile") //set nickname here
                {
                if (Instances.ActiveCanvas.Document.Attributes[i] is GH_Component comp)
                {
                    comp.ExpireSolution(true);
                    for (int j = 0; j < comp.Params.Input.Count; j++)
                    {
                        for (int k = 0; k < comp.Params.Input[j].Sources.Count; k++)
                            comp.Params.Input[j].Sources[k].Attributes.PerformLayout();
                    }
                }



                //Instances.ActiveCanvas.Document.Attributes[i].DocObject.ExpireSolution(true);


                //Instances.ActiveCanvas.Document.Attributes[i].DocObject.Attributes.PerformLayout();
                //Instances.ActiveCanvas.Document.Attributes[i].PerformLayout();
                //Instances.ActiveCanvas.Document.Attributes[i].DocObject.Attributes.ExpireLayout();
                //Instances.ActiveCanvas.Document.Attributes[i].ExpireLayout();
                //Instances.ActiveCanvas.Document.Attributes[i].DocObject.CreateAttributes();

                //Instances.ActiveCanvas.Document.Attributes[i].DocObject.ExpirePreview(true);
                //Instances.ActiveCanvas.Document.Attributes[i].DocObject.OnAttributesChanged();
                //Instances.ActiveCanvas.Document.Attributes[i].DocObject.OnDisplayExpired(true);
                //Instances.ActiveCanvas.Document.Attributes[i].DocObject.OnObjectChanged(GH_ObjectEventType.Layout);
                //Instances.ActiveCanvas.Document.Attributes[i].DocObject.OnObjectChanged(GH_ObjectEventType.PersistentData);
                //Instances.ActiveCanvas.Document.Attributes[i].DocObject.OnPingDocument();
                //Instances.ActiveCanvas.Document.Attributes[i].DocObject.OnPreviewExpired(true);
            }
            }
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GH_String ghnm = new GH_String();
            if (DA.GetData(0, ref ghnm))
            {
                if (GH_Convert.ToString(ghnm, out string SectU, GH_Conversion.Both))
                {
                    Util.Unit.LengthSection = SectU;
                    UpdateCanvas();
                }
            }

            List<string> units = new List<string>
            {
                "Length Large: " + Util.Unit.LengthLarge,
                "Length Small: " + Util.Unit.LengthSmall,
                "Length Section: " + Util.Unit.LengthSection,
                "Rhino document unit: " + Util.Unit.RhinoDocUnit,
                "Rhino unit conversion to meter: " + Util.Unit.RhinoDocFactorToMeter.ToString()
            };

            DA.SetDataList(0, units);
        }
    }
}

