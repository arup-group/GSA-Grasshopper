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
    /// Component to get geometric properties of a section
    /// </summary>
    public class GetSectionProperties : GH_Component, IGH_PreviewObject
    {
        #region Name and Ribbon Layout
        // This region handles how the component in displayed on the ribbon
        // including name, exposure level and icon
        public override Guid ComponentGuid => new Guid("6504a99f-a4e2-4e30-8251-de31ea83e8cb");
        public GetSectionProperties()
          : base("Section Properties", "SectProp", "Get GSA Section Properties",
                Ribbon.CategoryName.Name(),
                Ribbon.SubCategoryName.Cat1())
        { this.Hidden = true; } // sets the initial state of the component to hidden
        public override GH_Exposure Exposure => GH_Exposure.quarternary | GH_Exposure.obscure;

        protected override System.Drawing.Bitmap Icon => GhSA.Properties.Resources.SectionProperties;
        #endregion

        #region Custom UI
        //This region overrides the typical component layout
        public override void CreateAttributes()
        {
            if (first)
            {
                dropdownitems = new List<List<string>>();
                selecteditems = new List<string>();

                // length
                //dropdownitems.Add(Enum.GetNames(typeof(UnitsNet.Units.LengthUnit)).ToList());
                dropdownitems.Add(Units.FilteredLengthUnits);
                selecteditems.Add(lengthUnit.ToString());

                IQuantity quantity = new Length(0, lengthUnit);
                unitAbbreviation = string.Concat(quantity.ToString().Where(char.IsLetter));

                first = false;
            }
            m_attributes = new UI.MultiDropDownComponentUI(this, SetSelected, dropdownitems, selecteditems, spacerDescriptions);
        }
        public void SetSelected(int i, int j)
        {
            // change selected item
            selecteditems[i] = dropdownitems[i][j];

            lengthUnit = (UnitsNet.Units.LengthUnit)Enum.Parse(typeof(UnitsNet.Units.LengthUnit), selecteditems[i]);

            // update name of inputs (to display unit on sliders)
            (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
            ExpireSolution(true);
            Params.OnParametersChanged();
            this.OnDisplayExpired(true);
        }
        private void UpdateUIFromSelectedItems()
        {
            lengthUnit = (UnitsNet.Units.LengthUnit)Enum.Parse(typeof(UnitsNet.Units.LengthUnit), selecteditems[0]);

            CreateAttributes();
            (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
            ExpireSolution(true);
            Params.OnParametersChanged();
            this.OnDisplayExpired(true);
        }
        // list of lists with all dropdown lists conctent
        List<List<string>> dropdownitems;
        // list of selected items
        List<string> selecteditems;
        // list of descriptions 
        List<string> spacerDescriptions = new List<string>(new string[]
        {
            "Unit"
        });
        private bool first = true;
        private UnitsNet.Units.LengthUnit lengthUnit = Units.LengthUnitGeometry;
        string unitAbbreviation;

        #endregion

        #region Input and output

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Section", "PB", "Profile or GSA Section to get a bit more info out of", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("Area", "A", "GSA Section Area (" + Units.LengthUnitSection + "\xB2)", GH_ParamAccess.item);
            pManager.AddNumberParameter("Moment of Inertia y-y", "Iyy", "GSA Section Moment of Intertia around local y-y axis (" + Units.LengthUnitSection + "\x2074)", GH_ParamAccess.item);
            pManager.AddNumberParameter("Moment of Inertia z-z", "Izz", "GSA Section Moment of Intertia around local z-z axis (" + Units.LengthUnitSection + "\x2074)", GH_ParamAccess.item);
            pManager.AddNumberParameter("Moment of Inertia y-z", "Iyz", "GSA Section Moment of Intertia around local y-z axis (" + Units.LengthUnitSection + "\x2074)", GH_ParamAccess.item);
            pManager.AddNumberParameter("Torsion constant", "J", "GSA Section Torsion constant J (" + Units.LengthUnitSection + "\x2074)", GH_ParamAccess.item);
            pManager.AddNumberParameter("Shear Area Factor in y", "Ky", "GSA Section Shear Area Factor in local y-direction (-)", GH_ParamAccess.item);
            pManager.AddNumberParameter("Shear Area Factor in z", "Kz", "GSA Section Shear Area Factor in local z-direction (-)", GH_ParamAccess.item);
            pManager.AddNumberParameter("Surface A/Length", "S/L", "GSA Section Surface Area per Unit Length (" + Units.LengthUnitSection + "\xB2/"+ Units.LengthUnitGeometry + ")", GH_ParamAccess.item);
            pManager.AddNumberParameter("Volume/Length", "V/L", "GSA Section Volume per Unit Length (" + Units.LengthUnitSection + "\xB3/"+ Units.LengthUnitGeometry + ")", GH_ParamAccess.item);
        }
        #endregion

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GsaSection gsaSection = new GsaSection();
            GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
            if (DA.GetData(0, ref gh_typ))
            {
                if (gh_typ.Value is GsaSectionGoo)
                    gh_typ.CastTo(ref gsaSection);
                else
                {
                    string profile = "";
                    gh_typ.CastTo(ref profile);
                    gsaSection = new GsaSection(profile);
                }
            }
            if (gsaSection != null)
            {
                double conversionfactor = 1;
                if (Units.LengthUnitSection != "m")
                {
                    switch (Units.LengthUnitSection)
                    {
                        case "mm":
                            conversionfactor = 1000;
                            break;
                        case "cm":
                            conversionfactor = 100;
                            break;
                        case "in":
                            conversionfactor = 1000 / 25.4;
                            break;
                        case "ft":
                            conversionfactor = 1000 / (12 * 25.4);
                            break;
                    }
                }
                DA.SetData(0, gsaSection.Area * Math.Pow(conversionfactor, 2));
                DA.SetData(1, gsaSection.Iyy * Math.Pow(conversionfactor, 4));
                DA.SetData(2, gsaSection.Izz * Math.Pow(conversionfactor, 4));
                DA.SetData(3, gsaSection.Iyz * Math.Pow(conversionfactor, 4));
                DA.SetData(4, gsaSection.J * Math.Pow(conversionfactor, 4));
                DA.SetData(5, gsaSection.Ky);
                DA.SetData(6, gsaSection.Kz);
                DA.SetData(7, gsaSection.SurfaceAreaPerLength * Math.Pow(conversionfactor, 2));
                DA.SetData(8, gsaSection.VolumePerLength * Math.Pow(conversionfactor, 3));
            }
        }
    }
    #region (de)serialization
    public override bool Write(GH_IO.Serialization.GH_IWriter writer)
    {
        Util.GH.DeSerialization.writeDropDownComponents(ref writer, dropdownitems, selecteditems, spacerDescriptions);
        return base.Write(writer);
    }
    
    public override bool Read(GH_IO.Serialization.GH_IReader reader)
    {
        try // if users has an old versopm of this component then dropdown menu wont read
        {
            Util.GH.DeSerialization.readDropDownComponents(ref reader, ref dropdownitems, ref selecteditems, ref spacerDescriptions);
        }
        catch (Exception) // we create the dropdown menu with our chosen default
        {
            dropdownitems = new List<List<string>>();
            selecteditems = new List<string>();

            // set length to meters as this was the only option for old components
            lengthUnit = UnitsNet.Units.LengthUnit.Meter;

            dropdownitems.Add(Units.FilteredLengthUnits);
            selecteditems.Add(lengthUnit.ToString());

            IQuantity quantity = new Length(0, lengthUnit);
            unitAbbreviation = string.Concat(quantity.ToString().Where(char.IsLetter));

            first = false;
        }

        UpdateUIFromSelectedItems();

        first = false;

        return base.Read(reader);
    }
    #endregion
    #region IGH_VariableParameterComponent null implementation
    bool IGH_VariableParameterComponent.CanInsertParameter(GH_ParameterSide side, int index)
    {
        return false;
    }
    bool IGH_VariableParameterComponent.CanRemoveParameter(GH_ParameterSide side, int index)
    {
        return false;
    }
    IGH_Param IGH_VariableParameterComponent.CreateParameter(GH_ParameterSide side, int index)
    {
        return null;
    }
    bool IGH_VariableParameterComponent.DestroyParameter(GH_ParameterSide side, int index)
    {
        return false;
    }
    void IGH_VariableParameterComponent.VariableParameterMaintenance()
    {
        IQuantity length = new Length(0, lengthUnit);
        unitAbbreviation = string.Concat(length.ToString().Where(char.IsLetter));

        int i = 0;
        Params.Input[i++].Name = "Nodes [" + unitAbbreviation + "]";
        Params.Input[i++].Name = "1D Members [" + unitAbbreviation + "]";
        Params.Input[i++].Name = "2D Members [" + unitAbbreviation + "]";
        Params.Input[i++].Name = "3D Members [" + unitAbbreviation + "]";

    }
    #endregion
}

