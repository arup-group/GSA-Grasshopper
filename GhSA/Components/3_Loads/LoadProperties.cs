using System;
using System.Linq;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Parameters;
using UnitsNet.Units;
using UnitsNet;
using UnitsNet.GH;

namespace GsaGH.Components
{
    public class LoadProp : GH_Component, IGH_VariableParameterComponent
    {
        #region Name and Ribbon Layout
        public LoadProp()
            : base("Load Properties", "LoadProp", "Load Properties",
                Ribbon.CategoryName.Name(),
                Ribbon.SubCategoryName.Cat3())
        { this.Hidden = true; } // sets the initial state of the component to hidden
        public override Guid ComponentGuid => new Guid("0df96bee-3440-4699-b08d-d805220d1f68");
        public override GH_Exposure Exposure => GH_Exposure.quarternary | GH_Exposure.obscure;

        protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.LoadInfo;
        #endregion

        #region Custom UI
        //This region overrides the typical component layout
        public override void CreateAttributes()
        {
            if (first)
            {
                dropdownitems = new List<List<string>>();
                dropdownitems.Add(Units.FilteredForceUnits);
                dropdownitems.Add(Units.FilteredLengthUnits);

                selecteditems = new List<string>();
                selecteditems.Add(Units.ForceUnit.ToString());
                selecteditems.Add(Units.LengthUnitGeometry.ToString());

                first = false;
            }

            m_attributes = new UI.MultiDropDownComponentUI(this, SetSelected, dropdownitems, selecteditems, spacerDescriptions);
        }

        public void SetSelected(int i, int j)
        {
            // change selected item
            selecteditems[i] = dropdownitems[i][j];

            switch (i)
            {
                case 0:
                    forceUnit = (ForceUnit)Enum.Parse(typeof(ForceUnit), selecteditems[0]);
                    break;
                case 1:
                    lengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), selecteditems[1]);
                    break;
            }

            (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
            
            // update input params
            ExpireSolution(true);
            Params.OnParametersChanged();
            this.OnDisplayExpired(true);
        }

        private void UpdateUIFromSelectedItems()
        {
            forceUnit = (ForceUnit)Enum.Parse(typeof(ForceUnit), selecteditems[1]);
            lengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), selecteditems[2]);

            CreateAttributes();
            ExpireSolution(true);
            Params.OnParametersChanged();
            this.OnDisplayExpired(true);
        }
        #endregion

        #region Input and output
        // list of lists with all dropdown lists conctent
        List<List<string>> dropdownitems;
        // list of selected items
        List<string> selecteditems;
        // list of descriptions 
        List<string> spacerDescriptions = new List<string>(new string[]
        {
            "Unit"
        });

        private ForcePerLengthUnit forcePerLengthUnit;
        private ForceUnit forceUnit = Units.ForceUnit;
        private LengthUnit lengthUnit = Units.LengthUnitGeometry;
        bool first = true;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Load", "Ld", "Load to get some info out of", GH_ParamAccess.item);
        }
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            IQuantity force = new Force(0, forceUnit);
            string forceUnitAbbreviation = string.Concat(force.ToString().Where(char.IsLetter));
            IQuantity length = new Length(0, lengthUnit);
            string lengthUnitAbbreviation = string.Concat(length.ToString().Where(char.IsLetter));
            string unitAbbreviation = forceUnitAbbreviation + "/" + lengthUnitAbbreviation;

            pManager.AddIntegerParameter("Load case", "LC", "Load case number)", GH_ParamAccess.item);
            pManager.AddTextParameter("Name", "Na", "Load name", GH_ParamAccess.item);
            pManager.AddGenericParameter("Elements/Nodes/Definition", "De", "Element/Node list that load is applied to or Grid point / polygon definition", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Axis", "Ax", "Axis Property (0 : Global // -1 : Local", GH_ParamAccess.item);
            pManager.AddTextParameter("Direction", "Di", "Load direction", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Projected", "Pj", "Projected", GH_ParamAccess.item);
            pManager.AddNumberParameter("Load Value or Factor X [" + forceUnitAbbreviation + ", " + unitAbbreviation + "]", "V1", "Value at Start, Point 1 or Factor X", GH_ParamAccess.item);
            pManager.AddNumberParameter("Load Value or Factor Y [" + forceUnitAbbreviation + ", " + unitAbbreviation + "]", "V2", "Value at End, Point 2 or Factor Y", GH_ParamAccess.item);
            pManager.AddNumberParameter("Load Value or Factor Z [" + forceUnitAbbreviation + ", " + unitAbbreviation + "]", "V3", "Value at Point 3 or Factor Z", GH_ParamAccess.item);
            pManager.AddNumberParameter("Load Value [" + forceUnitAbbreviation + ", " + unitAbbreviation + "]", "V4", "Value at Point 4", GH_ParamAccess.item);
            pManager.AddGenericParameter("Grid Plane Surface", "GPS", "Grid Plane Surface", GH_ParamAccess.item);
        }
        #endregion
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Force kN = Force.From(1, forceUnit);
            Length m = Length.From(1, lengthUnit);
            ForcePerLength kNperM = kN / m;
            forcePerLengthUnit = kNperM.Unit;
            PressureUnit pressureUnit = (kN / (Length.From(1, lengthUnit) * Length.From(1, lengthUnit))).Unit;
            // Get Loads input
            GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
            if (DA.GetData(0, ref gh_typ))
            {
                GsaLoad gsaload = null;
                if (gh_typ.Value is GsaLoadGoo)
                {
                    gh_typ.CastTo(ref gsaload);
                    switch (gsaload.LoadType)
                    {
                        case GsaLoad.LoadTypes.Gravity:
                            DA.SetData(0, gsaload.GravityLoad.GravityLoad.Case);
                            DA.SetData(1, gsaload.GravityLoad.GravityLoad.Name);
                            DA.SetData(2, gsaload.GravityLoad.GravityLoad.Elements);
                            DA.SetData(6, gsaload.GravityLoad.GravityLoad.Factor.X);
                            DA.SetData(7, gsaload.GravityLoad.GravityLoad.Factor.Y);
                            DA.SetData(8, gsaload.GravityLoad.GravityLoad.Factor.Z);
                            return;
                        case GsaLoad.LoadTypes.Node:
                            DA.SetData(0, gsaload.NodeLoad.NodeLoad.Case);
                            DA.SetData(1, gsaload.NodeLoad.NodeLoad.Name);
                            DA.SetData(2, gsaload.NodeLoad.NodeLoad.Nodes);
                            DA.SetData(3, gsaload.NodeLoad.NodeLoad.AxisProperty);
                            DA.SetData(4, gsaload.NodeLoad.NodeLoad.Direction);
                            Force apiNodeForce = new Force(gsaload.NodeLoad.NodeLoad.Value, ForceUnit.Newton);
                            Force outNodeForce = new Force(apiNodeForce.As(forceUnit), forceUnit);
                            DA.SetData(6, new GH_UnitNumber(outNodeForce));
                            return;
                        case GsaLoad.LoadTypes.Beam:
                            DA.SetData(0, gsaload.BeamLoad.BeamLoad.Case);
                            DA.SetData(1, gsaload.BeamLoad.BeamLoad.Name);
                            DA.SetData(2, gsaload.BeamLoad.BeamLoad.Elements);
                            DA.SetData(3, gsaload.BeamLoad.BeamLoad.AxisProperty);
                            DA.SetData(4, gsaload.BeamLoad.BeamLoad.Direction);
                            DA.SetData(5, gsaload.BeamLoad.BeamLoad.IsProjected);
                            ForcePerLength apiBeamForce1 = new ForcePerLength(gsaload.BeamLoad.BeamLoad.Value(0), ForcePerLengthUnit.NewtonPerMeter);
                            ForcePerLength outBeamForce1 = new ForcePerLength(apiBeamForce1.As(forcePerLengthUnit), forcePerLengthUnit);
                            DA.SetData(6, new GH_UnitNumber(outBeamForce1));
                            ForcePerLength apiBeamForce2 = new ForcePerLength(gsaload.BeamLoad.BeamLoad.Value(1), ForcePerLengthUnit.NewtonPerMeter);
                            ForcePerLength outBeamForce2 = new ForcePerLength(apiBeamForce2.As(forcePerLengthUnit), forcePerLengthUnit);
                            DA.SetData(7, new GH_UnitNumber(outBeamForce2));
                            return;
                        case GsaLoad.LoadTypes.Face:
                            DA.SetData(0, gsaload.FaceLoad.FaceLoad.Case);
                            DA.SetData(1, gsaload.FaceLoad.FaceLoad.Name);
                            DA.SetData(2, gsaload.FaceLoad.FaceLoad.Elements);
                            DA.SetData(3, gsaload.FaceLoad.FaceLoad.AxisProperty);
                            DA.SetData(4, gsaload.FaceLoad.FaceLoad.Direction);
                            DA.SetData(5, gsaload.FaceLoad.FaceLoad.IsProjected);
                            Pressure apiFaceForce1 = new Pressure(gsaload.FaceLoad.FaceLoad.Value(0), PressureUnit.NewtonPerSquareMeter);
                            Pressure outFaceForce1 = new Pressure(apiFaceForce1.As(pressureUnit), pressureUnit);
                            DA.SetData(6, new GH_UnitNumber(outFaceForce1));
                            Pressure apiFaceForce2 = new Pressure(gsaload.FaceLoad.FaceLoad.Value(1), PressureUnit.NewtonPerSquareMeter);
                            Pressure outFaceForce2 = new Pressure(apiFaceForce2.As(pressureUnit), pressureUnit);
                            DA.SetData(7, new GH_UnitNumber(outFaceForce2));
                            Pressure apiFaceForce3 = new Pressure(gsaload.FaceLoad.FaceLoad.Value(2), PressureUnit.NewtonPerSquareMeter);
                            Pressure outFaceForce3 = new Pressure(apiFaceForce3.As(pressureUnit), pressureUnit);
                            DA.SetData(8, new GH_UnitNumber(outFaceForce3));
                            Pressure apiFaceForce4 = new Pressure(gsaload.FaceLoad.FaceLoad.Value(3), PressureUnit.NewtonPerSquareMeter);
                            Pressure outFaceForce4 = new Pressure(apiFaceForce4.As(pressureUnit), pressureUnit);
                            DA.SetData(9, new GH_UnitNumber(outFaceForce4));
                            return;
                        case GsaLoad.LoadTypes.GridPoint:
                            DA.SetData(0, gsaload.PointLoad.GridPointLoad.Case);
                            DA.SetData(1, gsaload.PointLoad.GridPointLoad.Name);
                            DA.SetData(2, "(" + gsaload.PointLoad.GridPointLoad.X + "," + gsaload.PointLoad.GridPointLoad.Y + ")");
                            DA.SetData(3, gsaload.PointLoad.GridPointLoad.AxisProperty);
                            DA.SetData(4, gsaload.PointLoad.GridPointLoad.Direction);
                            Force apiPointForce = new Force(gsaload.NodeLoad.NodeLoad.Value, ForceUnit.Newton);
                            Force outPointForce = new Force(apiPointForce.As(forceUnit), forceUnit);
                            DA.SetData(6, new GH_UnitNumber(outPointForce));
                            DA.SetData(10, new GsaGridPlaneSurfaceGoo(gsaload.PointLoad.GridPlaneSurface));
                            return;
                        case GsaLoad.LoadTypes.GridLine:
                            DA.SetData(0, gsaload.LineLoad.GridLineLoad.Case);
                            DA.SetData(1, gsaload.LineLoad.GridLineLoad.Name);
                            DA.SetData(2, gsaload.LineLoad.GridLineLoad.PolyLineDefinition);
                            DA.SetData(3, gsaload.LineLoad.GridLineLoad.AxisProperty);
                            DA.SetData(4, gsaload.LineLoad.GridLineLoad.Direction);
                            ForcePerLength apiLineForce1 = new ForcePerLength(gsaload.LineLoad.GridLineLoad.ValueAtStart, ForcePerLengthUnit.NewtonPerMeter);
                            ForcePerLength outLineForce1 = new ForcePerLength(apiLineForce1.As(forcePerLengthUnit), forcePerLengthUnit);
                            DA.SetData(6, new GH_UnitNumber(outLineForce1));
                            ForcePerLength apiLineForce2 = new ForcePerLength(gsaload.LineLoad.GridLineLoad.ValueAtEnd, ForcePerLengthUnit.NewtonPerMeter);
                            ForcePerLength outLineForce2 = new ForcePerLength(apiLineForce2.As(forcePerLengthUnit), forcePerLengthUnit);
                            DA.SetData(7, new GH_UnitNumber(outLineForce2));
                            DA.SetData(10, new GsaGridPlaneSurfaceGoo(gsaload.LineLoad.GridPlaneSurface));
                            return;
                        case GsaLoad.LoadTypes.GridArea:
                            DA.SetData(0, gsaload.AreaLoad.GridAreaLoad.Case);
                            DA.SetData(1, gsaload.AreaLoad.GridAreaLoad.Name);
                            DA.SetData(2, gsaload.AreaLoad.GridAreaLoad.PolyLineDefinition);
                            DA.SetData(3, gsaload.AreaLoad.GridAreaLoad.AxisProperty);
                            DA.SetData(4, gsaload.AreaLoad.GridAreaLoad.Direction);
                            Pressure apiAreaForce = new Pressure(gsaload.AreaLoad.GridAreaLoad.Value, PressureUnit.NewtonPerSquareMeter);
                            Pressure outAreaForce = new Pressure(apiAreaForce.As(pressureUnit), pressureUnit);
                            DA.SetData(6, new GH_UnitNumber(outAreaForce));
                            DA.SetData(10, new GsaGridPlaneSurfaceGoo(gsaload.AreaLoad.GridPlaneSurface));
                            return;
                    }
                }
                else
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Error converting input to GSA Load");
                    return;
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
            try // this will fail if user has an old version of the component
            {
                Util.GH.DeSerialization.readDropDownComponents(ref reader, ref dropdownitems, ref selecteditems, ref spacerDescriptions);
            }
            catch (Exception) // we set the stored values like first initation of component
            {
                dropdownitems = new List<List<string>>();
                dropdownitems.Add(Units.FilteredForceUnits);
                dropdownitems.Add(Units.FilteredLengthUnits);

                selecteditems = new List<string>();
                selecteditems.Add(ForceUnit.Kilonewton.ToString());
                selecteditems.Add(LengthUnit.Meter.ToString());
            }
            first = false;

            UpdateUIFromSelectedItems();
            return base.Read(reader);
        }

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
        #endregion
        #region IGH_VariableParameterComponent null implementation
        void IGH_VariableParameterComponent.VariableParameterMaintenance()
        {
            IQuantity force = new Force(0, forceUnit);
            string forceUnitAbbreviation = string.Concat(force.ToString().Where(char.IsLetter));
            IQuantity length = new Length(0, lengthUnit);
            string lengthUnitAbbreviation = string.Concat(length.ToString().Where(char.IsLetter));
            string unitAbbreviation = forceUnitAbbreviation + "/" + lengthUnitAbbreviation;

            Params.Output[6].NickName = "V";
            Params.Output[6].Name = "Value [" + unitAbbreviation + "]";
            Params.Output[6].Description = "Load Value";
            Params.Output[6].Access = GH_ParamAccess.item;

            Params.Output[7].NickName = "t";
            Params.Output[7].Name = "Position (%)";
            Params.Output[7].Description = "Line parameter where point load act (between 0.0 and 1.0)";
            Params.Output[7].Access = GH_ParamAccess.item;
        }
        #endregion
    }
}
