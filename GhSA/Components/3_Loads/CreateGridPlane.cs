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
    public class CreateGridPlane : GH_Component, IGH_VariableParameterComponent
    {
        #region Name and Ribbon Layout
        public CreateGridPlane()
            : base("Create Grid Plane", "GridPlane", "Create GSA Grid Plane",
                Ribbon.CategoryName.Name(),
                Ribbon.SubCategoryName.Cat3())
        { } 
        public override Guid ComponentGuid => new Guid("675fd47a-890d-45b8-bdde-fb2e8c1d9cca");
        public override GH_Exposure Exposure => GH_Exposure.tertiary;

        //protected override Bitmap Icon => Resources.CrossSections;
        #endregion

        #region Custom UI
        //This region overrides the typical component layout
        public override void CreateAttributes()
        {
            if (first)
            {
                selecteditem = _mode.ToString();
                first = false;
            }

            m_attributes = new UI.DropDownComponentUI(this, SetSelected, dropdownitems, selecteditem, "Type");
        }

        public void SetSelected(string selected)
        {
            selecteditem = selected;
            switch (selected)
            {
                case "General":
                    Mode1Clicked();
                    break;
                case "Storey":
                    Mode2Clicked();
                    break;
            }
        }
        #endregion

        #region Input and output
        readonly List<string> dropdownitems = new List<string>(new string[]
        {
            "General",
            "Storey"
        });

        string selecteditem;

        #endregion

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddPlaneParameter("Plane", "Pl", "Plane for Axis and Grid Plane definition. Note that an XY-plane will be created with an axis origin Z = 0 " +
                "and the height location will be controlled by Grid Plane elevation. For all none-XY plane inputs, the Grid Plane elevation will be 0", GH_ParamAccess.item);
            pManager.AddNumberParameter("Grid Elevation", "Ev", "Grid Elevation (Optional). Note that this value will be added to Plane origin location in the plane's normal axis direction.", GH_ParamAccess.item, 0);
            pManager.AddTextParameter("Name", "Na", "Name of Grid Plane", GH_ParamAccess.item);

            pManager[1].Optional = true;
            pManager[2].Optional = true;

            _mode = FoldMode.General;
        }
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Grid Plane", "GridPlane", "GSA Grid Plane", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // 0 Plane
            GH_Plane gh_pln = new GH_Plane();
            if (DA.GetData(0, ref gh_pln))
            {
                Plane pln = Plane.Unset;
                if (GH_Convert.ToPlane(gh_pln, ref pln, GH_Conversion.Both))
                {
                    // create gsa gridplanesurface from plane
                    GsaGridPlaneSurface gps = new GsaGridPlaneSurface(pln);

                    // 1 Grid elevation
                    GH_Number ghnum = new GH_Number();
                    if (DA.GetData(1, ref ghnum))
                    {
                        double elev = 0;
                        if (GH_Convert.ToDouble(ghnum, out elev, GH_Conversion.Both))
                        {
                            gps.GridPlane.Elevation = elev;
                            
                            // if elevation is set we want to move the plane in it's normal direction
                            Vector3d vec = pln.Normal;
                            vec.Unitize();
                            vec.X *= elev;
                            vec.Y *= elev;
                            vec.Z *= elev;
                            Transform xform = Transform.Translation(vec);
                            pln.Transform(xform);
                            gps.Plane = pln;
                            // note this wont move the Grid Plane Axis gps.Axis
                        }
                    }
                    
                    // 2 Name
                    GH_String ghtxt = new GH_String();
                    if (DA.GetData(2, ref ghtxt))
                    {
                        string name = "";
                        if (GH_Convert.ToString(ghtxt, out name, GH_Conversion.Both))
                            gps.GridPlane.Name = name;
                    }

                    // set is story
                    if (_mode == FoldMode.General)
                        gps.GridPlane.IsStoreyType = false;
                    else
                    {
                        gps.GridPlane.IsStoreyType = true;
                        
                        // 3 tolerance above
                        GH_Number ghtola = new GH_Number();
                        if (DA.GetData(3, ref ghtola))
                        {
                            double tola = 0;
                            if (GH_Convert.ToDouble(ghtola, out tola, GH_Conversion.Both))
                                gps.GridPlane.ToleranceAbove = tola;
                        }

                        // 4 tolerance above
                        GH_Number ghtolb = new GH_Number();
                        if (DA.GetData(4, ref ghtolb))
                        {
                            double tolb = 0;
                            if (GH_Convert.ToDouble(ghtolb, out tolb, GH_Conversion.Both))
                                gps.GridPlane.ToleranceBelow = tolb;
                        }
                    }

                    DA.SetData(0, new GsaGridPlaneSurfaceGoo(gps));
                }
            }
        }

        #region menu override
        private enum FoldMode
        {
            General,
            Storey
        }
        private bool first = true;
        private FoldMode _mode = FoldMode.General;

        private void Mode1Clicked()
        {
            if (_mode == FoldMode.General)
                return;

            RecordUndoEvent("General Parameters");
            _mode = FoldMode.General;

            //remove input parameters
            while (Params.Input.Count > 3)
                Params.UnregisterInputParameter(Params.Input[3], true);
            
            (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
            Params.OnParametersChanged();
            ExpireSolution(true);
        }
        private void Mode2Clicked()
        {
            if (_mode == FoldMode.Storey)
                return;

            RecordUndoEvent("Storey Parameters");
            _mode = FoldMode.Storey;

            //add input parameters
            Params.RegisterInputParam(new Param_Number());
            Params.RegisterInputParam(new Param_Number());

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
            if (_mode == FoldMode.Storey)
            {
                Params.Input[3].NickName = "tA";
                Params.Input[3].Name = "Tolerance Above (" + Util.GsaUnit.LengthLarge + ")";
                Params.Input[3].Description = "Tolerance Above Grid Plane";
                Params.Input[3].Access = GH_ParamAccess.item;
                Params.Input[3].Optional = true;

                Params.Input[4].NickName = "tB";
                Params.Input[4].Name = "Tolerance Below (" + Util.GsaUnit.LengthLarge + ")";
                Params.Input[4].Description = "Tolerance Above Grid Plane";
                Params.Input[4].Access = GH_ParamAccess.item;
                Params.Input[4].Optional = true;
            }
        }
        #endregion
    }
}
