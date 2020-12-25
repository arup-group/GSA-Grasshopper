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
        { this.Hidden = true; } // sets the initial state of the component to hidden
        public override GH_Exposure Exposure => GH_Exposure.tertiary | GH_Exposure.obscure;

        protected override System.Drawing.Bitmap Icon => GhSA.Properties.Resources.EditUnits;
        #endregion

        #region Custom UI
        //This region overrides the typical component layout


        #endregion

        #region Input and output

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Force Unit", "Force", "Set unit for Forces", GH_ParamAccess.item);
            pManager.AddTextParameter("Length Large Unit", "Geometry", "Set unit for modelled geometry. By default Rhino Document Unit is used", GH_ParamAccess.item);
            pManager.AddTextParameter("Length Small Unit", "Deflections", "Set unit for Cross-Sections", GH_ParamAccess.item);
            pManager.AddTextParameter("Section Length Unit", "Section", "Set unit for Cross-Sections", GH_ParamAccess.item);
            pManager.AddTextParameter("Mass unit", "Mass", "Set unit for Mass", GH_ParamAccess.item);
            pManager.AddTextParameter("Temperature unit", "Temperature", "Set unit for Temperature", GH_ParamAccess.item);
            pManager.AddTextParameter("Stress unit", "Stress", "Set unit for Stress", GH_ParamAccess.item);
            pManager.AddTextParameter("Strain unit", "Strain", "Set unit for Strain", GH_ParamAccess.item);
            pManager.AddTextParameter("Velocity unit", "Velocity", "Set unit for Velocity", GH_ParamAccess.item);
            pManager.AddTextParameter("Acceleration unit", "Acceleration", "Set unit for Acceleration", GH_ParamAccess.item);
            pManager.AddTextParameter("Energy unit", "Energy", "Set unit for Energy", GH_ParamAccess.item);
            pManager.AddTextParameter("Angle unit", "Angle", "Set unit for Angle", GH_ParamAccess.item);
            pManager.AddTextParameter("Time Short unit", "Time S", "Set unit for Time - short", GH_ParamAccess.item);
            pManager.AddTextParameter("Time Medium unit", "Time M", "Set unit for Time - medium", GH_ParamAccess.item);
            pManager.AddTextParameter("Time Long unit", "Time L", "Set unit for Time - Long", GH_ParamAccess.item);
            pManager.AddNumberParameter("Tolerance", "Tolerance", "Set Tolerance (using Length Large Unit)", GH_ParamAccess.item);
            for (int i = 0; i < pManager.ParamCount; i++)
                pManager[i].Optional = true;
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
            bool update = false;

            GH_String ghstr = new GH_String();
            if (DA.GetData(0, ref ghstr))
            {
                if (GH_Convert.ToString(ghstr, out string unit, GH_Conversion.Both))
                {
                    Util.GsaUnit.Force = unit;
                    update = true;
                }
            }

            ghstr = new GH_String();
            if (DA.GetData(1, ref ghstr))
            {
                if (GH_Convert.ToString(ghstr, out string unit, GH_Conversion.Both))
                {
                    Util.GsaUnit.LengthLarge = unit;
                    update = true;
                }
            }
            ghstr = new GH_String();
            if (DA.GetData(2, ref ghstr))
            {
                if (GH_Convert.ToString(ghstr, out string unit, GH_Conversion.Both))
                {
                    Util.GsaUnit.LengthSmall = unit;
                    update = true;
                }
            }
            ghstr = new GH_String();
            if (DA.GetData(3, ref ghstr))
            {
                if (GH_Convert.ToString(ghstr, out string unit, GH_Conversion.Both))
                {
                    Util.GsaUnit.LengthSection = unit;
                    update = true;
                }
            }
            ghstr = new GH_String();
            if (DA.GetData(4, ref ghstr))
            {
                if (GH_Convert.ToString(ghstr, out string unit, GH_Conversion.Both))
                {
                    Util.GsaUnit.Mass = unit;
                    update = true;
                }
            }
            ghstr = new GH_String();
            if (DA.GetData(5, ref ghstr))
            {
                if (GH_Convert.ToString(ghstr, out string unit, GH_Conversion.Both))
                {
                    Util.GsaUnit.Temperature = unit;
                    update = true;
                }
            }
            ghstr = new GH_String();
            if (DA.GetData(6, ref ghstr))
            {
                if (GH_Convert.ToString(ghstr, out string unit, GH_Conversion.Both))
                {
                    Util.GsaUnit.Stress = unit;
                    update = true;
                }
            }
            ghstr = new GH_String();
            if (DA.GetData(7, ref ghstr))
            {
                if (GH_Convert.ToString(ghstr, out string unit, GH_Conversion.Both))
                {
                    Util.GsaUnit.Strain = unit;
                    update = true;
                }
            }
            ghstr = new GH_String();
            if (DA.GetData(8, ref ghstr))
            {
                if (GH_Convert.ToString(ghstr, out string unit, GH_Conversion.Both))
                {
                    Util.GsaUnit.Velocity = unit;
                    update = true;
                }
            }
            ghstr = new GH_String();
            if (DA.GetData(9, ref ghstr))
            {
                if (GH_Convert.ToString(ghstr, out string unit, GH_Conversion.Both))
                {
                    Util.GsaUnit.Acceleration = unit;
                    update = true;
                }
            }
            ghstr = new GH_String();
            if (DA.GetData(10, ref ghstr))
            {
                if (GH_Convert.ToString(ghstr, out string unit, GH_Conversion.Both))
                {
                    Util.GsaUnit.Energy = unit;
                    update = true;
                }
            }
            ghstr = new GH_String();
            if (DA.GetData(11, ref ghstr))
            {
                if (GH_Convert.ToString(ghstr, out string unit, GH_Conversion.Both))
                {
                    Util.GsaUnit.Angle = unit;
                    update = true;
                }
            }
            ghstr = new GH_String();
            if (DA.GetData(12, ref ghstr))
            {
                if (GH_Convert.ToString(ghstr, out string unit, GH_Conversion.Both))
                {
                    Util.GsaUnit.TimeShort = unit;
                    update = true;
                }
            }
            ghstr = new GH_String();
            if (DA.GetData(13, ref ghstr))
            {
                if (GH_Convert.ToString(ghstr, out string unit, GH_Conversion.Both))
                {
                    Util.GsaUnit.TimeMedium = unit;
                    update = true;
                }
            }
            ghstr = new GH_String();
            if (DA.GetData(14, ref ghstr))
            {
                if (GH_Convert.ToString(ghstr, out string unit, GH_Conversion.Both))
                {
                    Util.GsaUnit.TimeLong = unit;
                    update = true;
                }
            }
            GH_Number ghnum = new GH_Number();
            if (DA.GetData(15, ref ghnum))
            {
                if (GH_Convert.ToDouble(ghnum, out double tol, GH_Conversion.Both))
                {
                    Util.GsaUnit.Tolerance = tol;
                    update = true;
                }
            }
            List<string> units = new List<string>
            {
                "Force: " + Util.GsaUnit.Force,
                "Length Large: " + Util.GsaUnit.LengthLarge
                + ((Util.GsaUnit.LengthLarge == Util.GsaUnit.RhinoDocUnit) ? "" : System.Environment.NewLine + "NB: Not similar to Rhino Document units!"),
                "Length Small: " + Util.GsaUnit.LengthSmall,
                "Length Section: " + Util.GsaUnit.LengthSection,
                "Mass: " + Util.GsaUnit.Mass,
                "Temperature: " + Util.GsaUnit.Temperature,
                "Stress: " + Util.GsaUnit.Stress,
                "Strain: " + Util.GsaUnit.Strain,
                "Velocity: " + Util.GsaUnit.Velocity,
                "Acceleration: " + Util.GsaUnit.Acceleration,
                "Energy: " + Util.GsaUnit.Energy,
                "Angle: " + Util.GsaUnit.Angle,
                "Time - short: " + Util.GsaUnit.TimeShort,
                "Time - medium: " + Util.GsaUnit.TimeMedium,
                "Time - long: " + Util.GsaUnit.TimeLong,
                "Tolerance: " + Util.GsaUnit.Tolerance
            };
            if (update)
                UpdateCanvas();
            DA.SetDataList(0, units);
            
        }
    }
}

