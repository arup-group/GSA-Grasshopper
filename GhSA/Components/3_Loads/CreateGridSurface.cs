using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel.Parameters;
using Rhino.Geometry;
using GsaAPI;
using GhSA.Parameters;

namespace GhSA.Components
{
    public class CreateGridSurface : GH_Component, IGH_VariableParameterComponent
    {
        #region Name and Ribbon Layout
        public CreateGridSurface()
            : base("Create Grid Surface", "GridSurface", "Create GSA Grid Surface",
                Ribbon.CategoryName.Name(),
                Ribbon.SubCategoryName.Cat3())
        { } // sets the initial state of the component to hidden
        public override Guid ComponentGuid => new Guid("1052955c-cf97-4378-81d3-8491e0defad0");
        public override GH_Exposure Exposure => GH_Exposure.tertiary;

        //protected override Bitmap Icon => Resources.CrossSections;
        #endregion

        #region Custom UI
        //This region overrides the typical component layout
        public override void CreateAttributes()
        {
            if (first)
            {
                selecteditem = "1D, One-way span";
                
            }

            m_attributes = new UI.DropDownComponentUI(this, SetSelected, dropdownitems, selecteditem, "Type");
        }

        public void SetSelected(string selected)
        {
            selecteditem = selected;
            switch (selected)
            {
                case "1D, One-way span":
                    Mode1Clicked();
                    break;
                case "1D, Two-way span":
                    Mode2Clicked();
                    break;
                case "2D":
                    Mode3Clicked();
                    break;
            }
        }
        #endregion

        #region Input and output
        readonly List<string> dropdownitems = new List<string>(new string[]
        {
            "1D, One-way span",
            "1D, Two-way span",
            "2D"
        });

        string selecteditem;

        #endregion

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Grid Plane", "GPl", "Grid Plane. If no input, Global XY-plane will be used", GH_ParamAccess.item);
            pManager.AddTextParameter("Elements", "El", "Elements for which the load should be expanded to. Default all", GH_ParamAccess.item, "all");
            pManager.AddTextParameter("Name", "Na", "Name of Grid Surface", GH_ParamAccess.item);
            pManager.AddNumberParameter("Tolerance (" + Util.GsaUnit.LengthSmall + ")", "Tol", "Tolerance for Load Expansion (default 10mm)", GH_ParamAccess.item, 10);

            pManager[0].Optional = true;
            pManager[1].Optional = true;
            pManager[2].Optional = true;
            pManager[3].Optional = true;

            if (first)
            {
                first = false;
                _mode = FoldMode.One_Dimensional_One_Way;
                pManager.AddNumberParameter("Span Direction", "Dir", "Span Direction (" + Util.GsaUnit.Angle + ") between -180 and 180 degrees", GH_ParamAccess.item, 0);
                pManager[4].Optional = true;
            }

            
        }
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Grid Surface", "GridSurface", "GSA Grid Surface", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // 0 Plane
            GsaGridPlaneSurface gps;
            Plane pln = Plane.Unset;
            GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
            if (DA.GetData(0, ref gh_typ))
            {
                if (gh_typ.Value is GsaGridPlaneSurface)
                {
                    GsaGridPlaneSurface temppln = new GsaGridPlaneSurface();
                    gh_typ.CastTo(ref temppln);
                    gps = temppln.Duplicate();
                }
                else
                {
                    gh_typ.CastTo(ref pln);
                    gps = new GsaGridPlaneSurface(pln);
                }
            }
            else
            {
                pln = Plane.WorldXY;
                gps = new GsaGridPlaneSurface(pln);
            }

            // 1 Elements
            GH_String ghelem = new GH_String();
            if (DA.GetData(1, ref ghelem))
            {
                string elem = "";
                if (GH_Convert.ToString(ghelem, out elem, GH_Conversion.Both))
                    gps.GridSurface.Elements = elem;
            }

            // 2 Name
            GH_String ghtxt = new GH_String();
            if (DA.GetData(2, ref ghtxt))
            {
                string name = "";
                if (GH_Convert.ToString(ghtxt, out name, GH_Conversion.Both))
                    gps.GridSurface.Name = name;
            }

            // 3 Tolerance
            GH_Number ghtol = new GH_Number();
            if (DA.GetData(3, ref ghtol))
            {
                double tol = 10;
                if (GH_Convert.ToDouble(ghtol, out tol, GH_Conversion.Both))
                gps.GridSurface.Tolerance = tol;
            }

            switch (_mode)
            {
                case FoldMode.One_Dimensional_One_Way:
                    gps.GridSurface.ElementType = GridSurface.Element_Type.ONE_DIMENSIONAL;
                    gps.GridSurface.SpanType = GridSurface.Span_Type.ONE_WAY;
                    
                    // 4 span direction
                    GH_Number ghdir = new GH_Number();
                    if (DA.GetData(3, ref ghdir))
                    {
                        double dir = 0;
                        if (GH_Convert.ToDouble(ghdir, out dir, GH_Conversion.Both))
                            if (dir > 180 || dir < -180)
                                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Angle value must be between -180 and 180 degrees");
                        gps.GridSurface.Direction = dir;
                    }
                    
                    break;

                case FoldMode.One_Dimensional_Two_Way:
                    gps.GridSurface.ElementType = GridSurface.Element_Type.ONE_DIMENSIONAL;
                    
                    // 4 expansion method
                    int exp = 0;
                    GH_Integer ghexp = new GH_Integer();
                    if (DA.GetData(4, ref ghexp))
                        GH_Convert.ToInt32_Primary(ghexp, ref exp);
                    gps.GridSurface.ExpansionType = GridSurfaceExpansionType.PLANE_CORNER;
                    if (exp == 1)
                        gps.GridSurface.ExpansionType = GridSurfaceExpansionType.PLANE_SMOOTH;
                    if (exp == 2)
                        gps.GridSurface.ExpansionType = GridSurfaceExpansionType.PLANE_ASPECT;
                    if (exp == 3)
                        gps.GridSurface.ExpansionType = GridSurfaceExpansionType.LEGACY;

                    // 5 simplify tributary area
                    bool simple = true;
                    GH_Boolean ghsim = new GH_Boolean();
                    if (DA.GetData(5, ref ghsim))
                        GH_Convert.ToBoolean(ghsim, out simple, GH_Conversion.Both);
                    if (simple)
                        gps.GridSurface.SpanType = GridSurface.Span_Type.TWO_WAY_SIMPLIFIED_TRIBUTARY_AREAS;
                    else
                        gps.GridSurface.SpanType = GridSurface.Span_Type.TWO_WAY;

                    break;

                case FoldMode.Two_Dimensional:
                    gps.GridSurface.ElementType = GridSurface.Element_Type.TWO_DIMENSIONAL;
                    break;

            }

            DA.SetData(0, new GsaGridPlaneSurfaceGoo(gps));
        }

        #region menu override
        private enum FoldMode
        {
            One_Dimensional_One_Way,
            One_Dimensional_Two_Way,
            Two_Dimensional
        }
        private bool first = true;
        private FoldMode _mode = FoldMode.One_Dimensional_One_Way;

        private void Mode1Clicked()
        {
            if (_mode == FoldMode.One_Dimensional_One_Way)
                return;

            RecordUndoEvent("1D, one-way Parameters");
            _mode = FoldMode.One_Dimensional_One_Way;

            //remove input parameters
            while (Params.Input.Count > 4)
                Params.UnregisterInputParameter(Params.Input[4], true);

            //add input parameters
            Params.RegisterInputParam(new Param_Number());

            (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
            Params.OnParametersChanged();
            ExpireSolution(true);
        }
        private void Mode2Clicked()
        {
            if (_mode == FoldMode.One_Dimensional_Two_Way)
                return;

            RecordUndoEvent("1D, two-way Parameters");
            _mode = FoldMode.One_Dimensional_Two_Way;

            //remove input parameters
            while (Params.Input.Count > 4)
                Params.UnregisterInputParameter(Params.Input[4], true);

            //add input parameters
            Params.RegisterInputParam(new Param_Integer());
            Params.RegisterInputParam(new Param_Boolean());

            (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
            Params.OnParametersChanged();
            ExpireSolution(true);
        }
        private void Mode3Clicked()
        {
            if (_mode == FoldMode.Two_Dimensional)
                return;

            RecordUndoEvent("2D Parameters");
            _mode = FoldMode.Two_Dimensional;

            //remove input parameters
            while (Params.Input.Count > 4)
                Params.UnregisterInputParameter(Params.Input[4], true);

            (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
            Params.OnParametersChanged();
            ExpireSolution(true);
        }

        #endregion

        #region (de)serialization
        public override bool Write(GH_IO.Serialization.GH_IWriter writer)
        {
            writer.SetInt32("Mode", (int)_mode);
            writer.SetString("select", selecteditem);
            return base.Write(writer);
        }
        public override bool Read(GH_IO.Serialization.GH_IReader reader)
        {
            _mode = (FoldMode)reader.GetInt32("Mode");
            selecteditem = reader.GetString("select");
            this.CreateAttributes();
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
            if (_mode == FoldMode.One_Dimensional_One_Way)
            {
                Params.Input[4].NickName = "Dir";
                Params.Input[4].Name = "Span Direction";
                Params.Input[4].Description = "Span Direction (" + Util.GsaUnit.Angle + ") between -180 and 180 degrees";
                Params.Input[4].Access = GH_ParamAccess.item;
                Params.Input[4].Optional = true;
            }
            if (_mode == FoldMode.One_Dimensional_Two_Way)
            {
                Params.Input[4].NickName = "Exp";
                Params.Input[4].Name = "Load Expansion";
                Params.Input[4].Description = "Load Expansion: " + System.Environment.NewLine + "Accepted inputs are:" +
                    System.Environment.NewLine + "0 : Corner (plane)" +
                    System.Environment.NewLine + "1 : Smooth (plane)" +
                    System.Environment.NewLine + "2 : Plane" +
                    System.Environment.NewLine + "3 : Legacy";
                Params.Input[4].Access = GH_ParamAccess.item;
                Params.Input[4].Optional = true;

                Params.Input[5].NickName = "Sim";
                Params.Input[5].Name = "Simplify";
                Params.Input[5].Description = "Simplify Tributary Area (default: True)";
                Params.Input[5].Access = GH_ParamAccess.item;
                Params.Input[5].Optional = true;
            }
        }
        #endregion
    }
}
