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
    public class GridPlaneSurfaceProperties : GH_Component
    {
        #region Name and Ribbon Layout
        public GridPlaneSurfaceProperties()
            : base("Grid Plane Surface Properties", "GridPlaneSurfaceProp", "Get GSA Grid Plane Surface Properties",
                Ribbon.CategoryName.Name(),
                Ribbon.SubCategoryName.Cat3())
        { } 
        public override Guid ComponentGuid => new Guid("cb5c1d72-e414-447b-b5db-ce18d76e2f4d");
        public override GH_Exposure Exposure => GH_Exposure.tertiary;

        //protected override Bitmap Icon => Resources.CrossSections;
        #endregion

        #region Custom UI
        //This region overrides the typical component layout
        #endregion

        #region Input and output
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Grid Plane Surface", "GPS", "Grid Plane Surface to get a bit more info out of", GH_ParamAccess.item);

        }
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            //pManager.AddGenericParameter("Grid Plane", "GridPlane", "GSA Grid Plane", GH_ParamAccess.item);
            pManager.AddPlaneParameter("Grid Plane", "Pln", "GSA Grid Plane (Axis + Elevation)", GH_ParamAccess.item);
            pManager.AddTextParameter("Grid Plane Name", "NaP", "GSA Grid Plane Name", GH_ParamAccess.item);
            pManager.AddBooleanParameter("is Storey?", "St", "GSA Grid Plane is Storey type", GH_ParamAccess.item);
            pManager.AddPlaneParameter("Axis", "Axis", "GSA Grid Plane Axis as plane", GH_ParamAccess.item);
            pManager.AddNumberParameter("Elevation", "Elv", "GSA Grid Plane Elevation", GH_ParamAccess.item);
            pManager.AddNumberParameter("Grid Plane Tolerance Above", "tolA", "GSA Grid Plane Tolerance Above (for Storey Type)", GH_ParamAccess.item);
            pManager.AddNumberParameter("Grid Plane Tolerance Below", "tolB", "GSA Grid Plane Tolerance Below (for Storey Type)", GH_ParamAccess.item);
            pManager.AddTextParameter("Grid Surface Name", "NaS", "GSA Grid Surface Name", GH_ParamAccess.item);
            pManager.AddTextParameter("Elements", "Elem", "GSA Grid Surface Elements", GH_ParamAccess.item);
            pManager.AddTextParameter("Element Type", "Typ", "GSA Grid Surface Element Type", GH_ParamAccess.item);
            pManager.AddNumberParameter("Grid Surface Tolerance", "tol", "GSA Grid Surface Tolerance", GH_ParamAccess.item);
            pManager.AddTextParameter("Span Type", "Span", "GSA Grid Surface Span Type", GH_ParamAccess.item);
            pManager.AddNumberParameter("Span Direction", "Dir", "GSA Grid Surface Span Direction", GH_ParamAccess.item);
            pManager.AddTextParameter("Expansion Type", "Exp", "GSA Grid Surface Expansion Type", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Simplified Tributary Area", "Simp", "GSA Grid Surface Simplified Tributary Area", GH_ParamAccess.item);
        }
        #endregion
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            GsaGridPlaneSurface gps = new GsaGridPlaneSurface();
            if(DA.GetData(0, ref gps))
            {
                //GsaGridPlaneSurface gridplane = new GsaGridPlaneSurface();
                //gridplane.GridPlane = gps.GridPlane;

                //DA.SetData(0, gridplane);
                DA.SetData(0, gps.Plane);
                DA.SetData(1, gps.GridPlane.Name);
                DA.SetData(2, gps.GridPlane.IsStoreyType);
                Plane axis = new Plane(new Point3d(gps.Axis.Origin.X, gps.Axis.Origin.Y, gps.Axis.Origin.Z),
                    new Vector3d(gps.Axis.XVector.X, gps.Axis.XVector.Y, gps.Axis.XVector.Z),
                    new Vector3d(gps.Axis.XYPlane.X, gps.Axis.XYPlane.Y, gps.Axis.XYPlane.Z)
                    );
                DA.SetData(3, axis);
                DA.SetData(4, gps.GridPlane.Elevation);
                DA.SetData(5, gps.GridPlane.ToleranceAbove);
                DA.SetData(6, gps.GridPlane.ToleranceBelow);
                DA.SetData(7, gps.GridSurface.Name);
                DA.SetData(8, gps.GridSurface.Elements);
                DA.SetData(9, gps.GridSurface.ElementType.ToString());
                DA.SetData(10, gps.GridSurface.Tolerance);
                DA.SetData(11, gps.GridSurface.SpanType.ToString());
                DA.SetData(12, gps.GridSurface.Direction);
                DA.SetData(13, gps.GridSurface.ExpansionType.ToString());
                bool simple = false;
                if (gps.GridSurface.SpanType == GridSurface.Span_Type.TWO_WAY_SIMPLIFIED_TRIBUTARY_AREAS)
                    simple = true;
                DA.SetData(14, simple);

            }
        }
    }
}
