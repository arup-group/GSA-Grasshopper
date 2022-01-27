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
using UnitsNet;
using System.Linq;

namespace GhSA.Components
{
    /// <summary>
    /// Component to retrieve non-geometric objects from a GSA model
    /// </summary>
    public class GetLoads : GH_Component, IGH_VariableParameterComponent
    {
        #region Name and Ribbon Layout
        // This region handles how the component in displayed on the ribbon
        // including name, exposure level and icon
        public override Guid ComponentGuid => new Guid("87ff28e5-a1a6-4d78-ba71-e930e01dca13");
        public GetLoads()
          : base("Get Model Loads", "GetLoads", "Get Loads and Grid Planes/Surfaces from GSA model",
                Ribbon.CategoryName.Name(),
                Ribbon.SubCategoryName.Cat0())
        { this.Hidden = true; } // sets the initial state of the component to hidden
        public override GH_Exposure Exposure => GH_Exposure.secondary | GH_Exposure.obscure;

        protected override System.Drawing.Bitmap Icon => GhSA.Properties.Resources.GetLoads;
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

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("GSA Model", "GSA", "GSA model containing some loads", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            IQuantity length = new Length(0, lengthUnit);
            unitAbbreviation = string.Concat(length.ToString().Where(char.IsLetter));

            pManager.AddGenericParameter("Gravity Loads", "Gr", "Gravity Loads from GSA Model", GH_ParamAccess.list);
            pManager.AddGenericParameter("Node Loads", "No", "Node Loads from GSA Model", GH_ParamAccess.list);
            pManager.AddGenericParameter("Beam Loads", "Be", "Beam Loads from GSA Model", GH_ParamAccess.list);
            pManager.AddGenericParameter("Face Loads", "Fa", "Face Loads from GSA Model", GH_ParamAccess.list);
            pManager.AddGenericParameter("Grid Point Loads [" + unitAbbreviation + "]", "Pt", "Grid Point Loads from GSA Model", GH_ParamAccess.list);
            pManager.AddGenericParameter("Grid Line Loads [" + unitAbbreviation + "]", "Ln", "Grid Line Loads from GSA Model", GH_ParamAccess.list);
            pManager.AddGenericParameter("Grid Area Loads [" + unitAbbreviation + "]", "Ar", "Grid Area Loads from GSA Model", GH_ParamAccess.list);
            pManager.AddGenericParameter("Grid Plane Surfaces [" + unitAbbreviation + "]", "GPS", "Grid Plane Surfaces from GSA Model", GH_ParamAccess.list);
            pManager.HideParameter(7);
        }
        #endregion

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GsaModel gsaModel = new GsaModel();
            if (DA.GetData(0, ref gsaModel))
            {
                Model model = new Model();
                model = gsaModel.Model;

                List<GsaLoadGoo> gravity = Util.Gsa.FromGSA.GetGravityLoads(model.GravityLoads());
                List<GsaLoadGoo> node = Util.Gsa.FromGSA.GetNodeLoads(model);
                List<GsaLoadGoo> beam = Util.Gsa.FromGSA.GetBeamLoads(model.BeamLoads());
                List<GsaLoadGoo> face = Util.Gsa.FromGSA.GetFaceLoads(model.FaceLoads());

                IReadOnlyDictionary<int, GridSurface> srfDict = model.GridSurfaces();
                IReadOnlyDictionary<int, GridPlane> plnDict = model.GridPlanes();
                IReadOnlyDictionary<int, Axis> axDict = model.Axes();
                List<GsaLoadGoo> point = Util.Gsa.FromGSA.GetGridPointLoads(model.GridPointLoads(), srfDict, plnDict, axDict, lengthUnit);
                List<GsaLoadGoo> line = Util.Gsa.FromGSA.GetGridLineLoads(model.GridLineLoads(), srfDict, plnDict, axDict, lengthUnit);
                List<GsaLoadGoo> area = Util.Gsa.FromGSA.GetGridAreaLoads(model.GridAreaLoads(), srfDict, plnDict, axDict, lengthUnit);
                
                List<GsaGridPlaneSurfaceGoo> gps = new List<GsaGridPlaneSurfaceGoo>();
                
                foreach (int key in srfDict.Keys)
                    gps.Add(new GsaGridPlaneSurfaceGoo(Util.Gsa.FromGSA.GetGridPlaneSurface(srfDict, plnDict, axDict, key, lengthUnit)));

                DA.SetDataList(0, gravity);
                DA.SetDataList(1, node);
                DA.SetDataList(2, beam);
                DA.SetDataList(3, face);
                DA.SetDataList(4, point);
                DA.SetDataList(5, line);
                DA.SetDataList(6, area);
                DA.SetDataList(7, gps);
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
            int i = 4;
            Params.Input[i++].Name = "Grid Point Loads [" + unitAbbreviation + "]";
            Params.Input[i++].Name = "Grid Line Loads [" + unitAbbreviation + "]";
            Params.Input[i++].Name = "Grid Area Loads [" + unitAbbreviation + "]";
            Params.Input[i++].Name = "Grid Plane Surfaces [" + unitAbbreviation + "]";
        }
        #endregion  
    }
}

