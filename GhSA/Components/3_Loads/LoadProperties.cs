using System;
using System.Linq;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaAPI;
using Rhino.Geometry;
using GhSA.Parameters;

namespace GhSA.Components
{
    public class LoadProp : GH_Component
    {
        #region Name and Ribbon Layout
        public LoadProp()
            : base("Load Properties", "LoadProp", "Load Properties",
                Ribbon.CategoryName.Name(),
                Ribbon.SubCategoryName.Cat3())
        { this.Hidden = true; } // sets the initial state of the component to hidden
        public override Guid ComponentGuid => new Guid("0df96bee-3440-4699-b08d-d805220d1f68");
        public override GH_Exposure Exposure => GH_Exposure.quarternary | GH_Exposure.obscure;

        protected override System.Drawing.Bitmap Icon => GSA.Properties.Resources.LoadProp;
        #endregion

        #region Custom UI
        //This region overrides the typical component layout
        #endregion

        #region input and output
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Load", "Load", "Load to get some info out of", GH_ParamAccess.item);
        }
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddIntegerParameter("Load case", "LC", "Load case number)", GH_ParamAccess.item);
            pManager.AddTextParameter("Name", "Na", "Load name", GH_ParamAccess.item);
            pManager.AddTextParameter("Elements or Nodes", "El", "Element or Node list", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Axis", "Ax", "Axis Property (0 : Global // -1 : Local", GH_ParamAccess.item);
            pManager.AddTextParameter("Direction", "Di", "Load direction", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Projected", "Pj", "Projected", GH_ParamAccess.item);
            pManager.AddNumberParameter("Load Value or Factor X (" + Util.GsaUnit.Force + " or " + Util.GsaUnit.Force + "/" + Util.GsaUnit.LengthLarge + ")", "V1", "Value at Start, Point 1 or Factor X (" + Util.GsaUnit.Force + " or " + Util.GsaUnit.Force + "/" + Util.GsaUnit.LengthLarge + ")", GH_ParamAccess.item);
            pManager.AddNumberParameter("Load Value or Factor Y (" + Util.GsaUnit.Force + " or " + Util.GsaUnit.Force + "/" + Util.GsaUnit.LengthLarge + ")", "V2", "Value at End, Point 2 or Factor Y (" + Util.GsaUnit.Force + " or " + Util.GsaUnit.Force + "/" + Util.GsaUnit.LengthLarge + ")", GH_ParamAccess.item);
            pManager.AddNumberParameter("Load Value or Factor Z (" + Util.GsaUnit.Force + " or " + Util.GsaUnit.Force + "/" + Util.GsaUnit.LengthLarge + ")", "V3", "Value at Point 3 or Factor Z (" + Util.GsaUnit.Force + " or " + Util.GsaUnit.Force + "/" + Util.GsaUnit.LengthLarge + ")", GH_ParamAccess.item);
            pManager.AddNumberParameter("Load Value (" + Util.GsaUnit.Force + " or " + Util.GsaUnit.Force + "/" + Util.GsaUnit.LengthLarge + ")", "V4", "Value at Point 4 (" + Util.GsaUnit.Force + " or " + Util.GsaUnit.Force + "/" + Util.GsaUnit.LengthLarge + ")", GH_ParamAccess.item);
            pManager.AddGenericParameter("Grid Plane Surface", "GPS", "Grid Plane Surface", GH_ParamAccess.item);
        }
        #endregion
        protected override void SolveInstance(IGH_DataAccess DA)
        {
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
                            DA.SetData(6, gsaload.NodeLoad.NodeLoad.Value / 1000);
                            return;
                        case GsaLoad.LoadTypes.Beam:
                            DA.SetData(0, gsaload.BeamLoad.BeamLoad.Case);
                            DA.SetData(1, gsaload.BeamLoad.BeamLoad.Name);
                            DA.SetData(2, gsaload.BeamLoad.BeamLoad.Elements);
                            DA.SetData(3, gsaload.BeamLoad.BeamLoad.AxisProperty);
                            DA.SetData(4, gsaload.BeamLoad.BeamLoad.Direction);
                            DA.SetData(5, gsaload.BeamLoad.BeamLoad.IsProjected);
                            DA.SetData(6, gsaload.BeamLoad.BeamLoad.Value(0) / 1000);
                            DA.SetData(7, gsaload.BeamLoad.BeamLoad.Value(1) / 1000);
                            return;
                        case GsaLoad.LoadTypes.Face:
                            DA.SetData(0, gsaload.FaceLoad.FaceLoad.Case);
                            DA.SetData(1, gsaload.FaceLoad.FaceLoad.Name);
                            DA.SetData(2, gsaload.FaceLoad.FaceLoad.Elements);
                            DA.SetData(3, gsaload.FaceLoad.FaceLoad.AxisProperty);
                            DA.SetData(4, gsaload.FaceLoad.FaceLoad.Direction);
                            DA.SetData(5, gsaload.FaceLoad.FaceLoad.IsProjected);
                            DA.SetData(6, gsaload.FaceLoad.FaceLoad.Value(0)/1000);
                            DA.SetData(7, gsaload.FaceLoad.FaceLoad.Value(1) / 1000);
                            DA.SetData(8, gsaload.FaceLoad.FaceLoad.Value(2) / 1000);
                            DA.SetData(9, gsaload.FaceLoad.FaceLoad.Value(3) / 1000);
                            return;
                        case GsaLoad.LoadTypes.GridPoint:
                            DA.SetData(0, gsaload.PointLoad.GridPointLoad.Case);
                            DA.SetData(1, gsaload.PointLoad.GridPointLoad.Name);
                            DA.SetData(2, "(" + gsaload.PointLoad.GridPointLoad.X + "," + gsaload.PointLoad.GridPointLoad.Y + ")");
                            DA.SetData(3, gsaload.PointLoad.GridPointLoad.AxisProperty);
                            DA.SetData(4, gsaload.PointLoad.GridPointLoad.Direction);
                            DA.SetData(6, gsaload.PointLoad.GridPointLoad.Value / 1000);
                            DA.SetData(10, new GsaGridPlaneSurfaceGoo(gsaload.PointLoad.GridPlaneSurface));
                            return;
                        case GsaLoad.LoadTypes.GridLine:
                            DA.SetData(0, gsaload.LineLoad.GridLineLoad.Case);
                            DA.SetData(1, gsaload.LineLoad.GridLineLoad.Name);
                            DA.SetData(2, gsaload.LineLoad.GridLineLoad.PolyLineDefinition);
                            DA.SetData(3, gsaload.LineLoad.GridLineLoad.AxisProperty);
                            DA.SetData(4, gsaload.LineLoad.GridLineLoad.Direction);
                            DA.SetData(6, gsaload.LineLoad.GridLineLoad.ValueAtStart / 1000);
                            DA.SetData(7, gsaload.LineLoad.GridLineLoad.ValueAtEnd / 1000);
                            DA.SetData(10, new GsaGridPlaneSurfaceGoo(gsaload.LineLoad.GridPlaneSurface));
                            return;
                        case GsaLoad.LoadTypes.GridArea:
                            DA.SetData(0, gsaload.AreaLoad.GridAreaLoad.Case);
                            DA.SetData(1, gsaload.AreaLoad.GridAreaLoad.Name);
                            DA.SetData(2, gsaload.AreaLoad.GridAreaLoad.PolyLineDefinition);
                            DA.SetData(3, gsaload.AreaLoad.GridAreaLoad.AxisProperty);
                            DA.SetData(4, gsaload.AreaLoad.GridAreaLoad.Direction);
                            DA.SetData(6, gsaload.AreaLoad.GridAreaLoad.Value / 1000);
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
    }
}
