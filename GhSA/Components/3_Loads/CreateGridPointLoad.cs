using System;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaAPI;
using Rhino.Geometry;
using GhSA.Parameters;
using UnitsNet.Units;
using System.Collections.Generic;
using UnitsNet;
using System.Linq;

namespace GhSA.Components
{
    public class CreateGridPointLoad : GH_Component, IGH_VariableParameterComponent
    {
        #region Name and Ribbon Layout
        public CreateGridPointLoad()
            : base("Create Grid Point Load", "PointLoad", "Create GSA Grid Point Load",
                Ribbon.CategoryName.Name(),
                Ribbon.SubCategoryName.Cat3())
        { this.Hidden = true; } // sets the initial state of the component to hidden
        public override Guid ComponentGuid => new Guid("076f03c6-67ba-49d3-9462-cd4a4b5aff92");
        public override GH_Exposure Exposure => GH_Exposure.secondary;

        protected override System.Drawing.Bitmap Icon => GhSA.Properties.Resources.PointLoad;
        #endregion

        #region Custom UI
        //This region overrides the typical component layout
        public override void CreateAttributes()
        {
            if (first)
            {
                dropdownitems = new List<List<string>>();
                dropdownitems.Add(Units.FilteredForceUnits);

                selecteditems = new List<string>();
                selecteditems.Add(Units.ForceUnit.ToString());

                first = false;
            }

            m_attributes = new UI.MultiDropDownComponentUI(this, SetSelected, dropdownitems, selecteditems, spacerDescriptions);
        }
        public void SetSelected(int i, int j)
        {
            // change selected item
            selecteditems[i] = dropdownitems[i][j];

            forceUnit = (ForceUnit)Enum.Parse(typeof(ForceUnit), selecteditems[0]);

            (this as IGH_VariableParameterComponent).VariableParameterMaintenance();

            // update input params
            ExpireSolution(true);
            Params.OnParametersChanged();
            this.OnDisplayExpired(true);
        }
        private void UpdateUIFromSelectedItems()
        {
            forceUnit = (ForceUnit)Enum.Parse(typeof(ForceUnit), selecteditems[0]);

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
            "Unit",
        });

        private ForceUnit forceUnit = Units.ForceUnit;
        bool first = true;
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            IQuantity force = new Force(0, forceUnit);
            string forceUnitAbbreviation = string.Concat(force.ToString().Where(char.IsLetter));

            pManager.AddIntegerParameter("Load case", "LC", "Load case number (default 1)", GH_ParamAccess.item, 1);
            pManager.AddPointParameter("Point", "Pt", "Point. If you input grid plane below only x and y coordinates will be used from this point, but if not a new Grid Plane Surface (xy-plane) will be created at the z-elevation of this point.", GH_ParamAccess.item);
            pManager.AddGenericParameter("Grid Plane Surface", "GPS", "Grid Plane Surface or Plane (optional). If no input here then the point's z-coordinate will be used for an xy-plane at that elevation.", GH_ParamAccess.item);
            pManager.AddTextParameter("Direction", "Di", "Load direction (default z)." +
                    System.Environment.NewLine + "Accepted inputs are:" +
                    System.Environment.NewLine + "x" +
                    System.Environment.NewLine + "y" +
                    System.Environment.NewLine + "z", GH_ParamAccess.item, "z");
            pManager.AddIntegerParameter("Axis", "Ax", "Load axis (default Global). " +
                    System.Environment.NewLine + "Accepted inputs are:" +
                    System.Environment.NewLine + "0 : Global" +
                    System.Environment.NewLine + "-1 : Local", GH_ParamAccess.item, 0);
            pManager.AddTextParameter("Name", "Na", "Load Name", GH_ParamAccess.item);
            pManager.AddGenericParameter("Value [" + forceUnitAbbreviation + "]", "V", "Load Value", GH_ParamAccess.item);

            pManager[0].Optional = true;
            pManager[2].Optional = true;
            pManager[3].Optional = true;
            pManager[4].Optional = true;
            pManager[5].Optional = true;
        }
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Grid Point Load", "Ld", "GSA Grid Point Load", GH_ParamAccess.item);
        }
        #endregion
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GsaGridPointLoad gridpointload = new GsaGridPointLoad();

            // 0 Load case
            int lc = 1;
            GH_Integer gh_lc = new GH_Integer();
            if (DA.GetData(0, ref gh_lc))
                GH_Convert.ToInt32(gh_lc, out lc, GH_Conversion.Both);
            gridpointload.GridPointLoad.Case = lc;

            // 1 Point
            Point3d pt = new Point3d();
            GH_Point gh_pt = new GH_Point();
            if (DA.GetData(1, ref gh_pt))
                GH_Convert.ToPoint3d(gh_pt, ref pt, GH_Conversion.Both);
            gridpointload.GridPointLoad.X = pt.X;
            gridpointload.GridPointLoad.Y = pt.Y;

            // 2 Plane
            GsaGridPlaneSurface grdplnsrf;
            Plane pln = Plane.WorldXY;
            GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
            if (DA.GetData(2, ref gh_typ))
            {
                if (gh_typ.Value is GsaGridPlaneSurfaceGoo)
                {
                    GsaGridPlaneSurface temppln = new GsaGridPlaneSurface();
                    gh_typ.CastTo(ref temppln);
                    grdplnsrf = temppln.Duplicate();
                    gridpointload.GridPlaneSurface = grdplnsrf;
                }
                else if (gh_typ.Value is Plane)
                {
                    gh_typ.CastTo(ref pln);
                    grdplnsrf = new GsaGridPlaneSurface(pln);
                    gridpointload.GridPlaneSurface = grdplnsrf;
                }
                else
                {
                    int id = 0;
                    if (GH_Convert.ToInt32(gh_typ.Value, out id, GH_Conversion.Both))
                    {
                        gridpointload.GridPointLoad.GridSurface = id;
                        gridpointload.GridPlaneSurface = null;
                    }
                    else
                    {
                        AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Error in GPS input. Accepted inputs are Grid Plane Surface or Plane. " +
                            System.Environment.NewLine + "If no input here then the point's z-coordinate will be used for an xy-plane at that elevation");
                        return;
                    }
                }
            }
            else
            {
                pln = Plane.WorldXY;
                pln.Origin = pt;
                grdplnsrf = new GsaGridPlaneSurface(pln);
                gridpointload.GridPlaneSurface = grdplnsrf;
            }

            // 3 direction
            string dir = "Z";
            Direction direc = Direction.Z;

            GH_String gh_dir = new GH_String();
            if (DA.GetData(3, ref gh_dir))
                GH_Convert.ToString(gh_dir, out dir, GH_Conversion.Both);
            dir = dir.ToUpper();
            if (dir == "X")
                direc = Direction.X;
            if (dir == "Y")
                direc = Direction.Y;

            gridpointload.GridPointLoad.Direction = direc;

            // 4 Axis
            int axis = 0;
            gridpointload.GridPointLoad.AxisProperty = 0; 
            GH_Integer gh_ax = new GH_Integer();
            if (DA.GetData(4, ref gh_ax))
            {
                GH_Convert.ToInt32(gh_ax, out axis, GH_Conversion.Both);
                if (axis == 0 || axis == -1)
                    gridpointload.GridPointLoad.AxisProperty = axis;
            }

            // 5 Name
            string name = "";
            GH_String gh_name = new GH_String();
            if (DA.GetData(5, ref gh_name))
            {
                if (GH_Convert.ToString(gh_name, out name, GH_Conversion.Both))
                    gridpointload.GridPointLoad.Name = name;
            }

            // 6 load value
            gridpointload.GridPointLoad.Value = GetInput.Force(this, DA, 6, forceUnit).Newtons;

            // convert to goo
            GsaLoad gsaLoad = new GsaLoad(gridpointload);
            DA.SetData(0, new GsaLoadGoo(gsaLoad));
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

                selecteditems = new List<string>();
                selecteditems.Add(ForceUnit.Kilonewton.ToString());
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
            Params.Input[6].Name = "Value [" + forceUnitAbbreviation + "]";
        }
        #endregion
    }
}
