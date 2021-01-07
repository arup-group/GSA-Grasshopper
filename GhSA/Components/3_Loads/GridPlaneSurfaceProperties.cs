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
        public override GH_Exposure Exposure => GH_Exposure.quarternary | GH_Exposure.obscure;

        protected override System.Drawing.Bitmap Icon => GhSA.Properties.Resources.GridPlaneProperties;
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
            pManager.AddPlaneParameter("Grid Plane", "P", "GSA Grid Plane (Axis + Elevation)", GH_ParamAccess.item); //0
            pManager.AddIntegerParameter("Grid Plane ID", "IdG", "GSA Grid Plane ID", GH_ParamAccess.item); //1
            pManager.AddTextParameter("Grid Plane Name", "NaP", "GSA Grid Plane Name", GH_ParamAccess.item); //2
            pManager.AddBooleanParameter("is Storey?", "St", "GSA Grid Plane is Storey type", GH_ParamAccess.item); //3
            pManager.AddPlaneParameter("Axis", "Ax", "GSA Grid Plane Axis as plane", GH_ParamAccess.item); //4
            pManager.AddIntegerParameter("Axis ID", "IdA", "GSA Axis ID", GH_ParamAccess.item); //5
            pManager.AddNumberParameter("Elevation", "Ev", "GSA Grid Plane Elevation", GH_ParamAccess.item); //6
            pManager.AddNumberParameter("Grid Plane Tolerance Above", "tA", "GSA Grid Plane Tolerance Above (for Storey Type)", GH_ParamAccess.item); //7
            pManager.AddNumberParameter("Grid Plane Tolerance Below", "tB", "GSA Grid Plane Tolerance Below (for Storey Type)", GH_ParamAccess.item); //8
            
            pManager.AddTextParameter("Grid Surface Name", "NaS", "GSA Grid Surface Name", GH_ParamAccess.item); //9
            pManager.AddIntegerParameter("Grid Surface ID", "IdS", "GSA Grid Surface ID", GH_ParamAccess.item); //10
            pManager.AddTextParameter("Elements", "El", "GSA Grid Surface Elements", GH_ParamAccess.item); //11
            pManager.AddTextParameter("Element Type", "Ty", "GSA Grid Surface Element Type", GH_ParamAccess.item); //12
            pManager.AddNumberParameter("Grid Surface Tolerance", "To", "GSA Grid Surface Tolerance", GH_ParamAccess.item); //13
            pManager.AddTextParameter("Span Type", "Sp", "GSA Grid Surface Span Type", GH_ParamAccess.item); //14
            pManager.AddNumberParameter("Span Direction", "Di", "GSA Grid Surface Span Direction", GH_ParamAccess.item); //15
            pManager.AddTextParameter("Expansion Type", "Ex", "GSA Grid Surface Expansion Type", GH_ParamAccess.item); //16
            pManager.AddBooleanParameter("Simplified Tributary Area", "Sf", "GSA Grid Surface Simplified Tributary Area", GH_ParamAccess.item); //17
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
                DA.SetData(1, gps.GridPlaneID);
                DA.SetData(2, gps.GridPlane.Name);
                DA.SetData(3, gps.GridPlane.IsStoreyType);
                Plane axis = new Plane(new Point3d(gps.Axis.Origin.X, gps.Axis.Origin.Y, gps.Axis.Origin.Z),
                    new Vector3d(gps.Axis.XVector.X, gps.Axis.XVector.Y, gps.Axis.XVector.Z),
                    new Vector3d(gps.Axis.XYPlane.X, gps.Axis.XYPlane.Y, gps.Axis.XYPlane.Z)
                    );
                DA.SetData(4, axis);
                DA.SetData(5, gps.AxisID);
                DA.SetData(6, gps.GridPlane.Elevation);
                DA.SetData(7, gps.GridPlane.ToleranceAbove);
                DA.SetData(8, gps.GridPlane.ToleranceBelow);
                
                DA.SetData(9, gps.GridSurface.Name);
                DA.SetData(10, gps.GridSurfaceID);
                DA.SetData(11, gps.GridSurface.Elements);
                string elemtype = gps.GridSurface.ElementType.ToString();
                DA.SetData(12, Char.ToUpper(elemtype[0]) + elemtype.Substring(1).ToLower().Replace("_", " "));
                DA.SetData(13, gps.GridSurface.Tolerance);
                string spantype = gps.GridSurface.SpanType.ToString();
                DA.SetData(14, Char.ToUpper(spantype[0]) + spantype.Substring(1).ToLower().Replace("_", " "));
                DA.SetData(15, gps.GridSurface.Direction);
                string expantype = gps.GridSurface.ExpansionType.ToString();
                DA.SetData(16, Char.ToUpper(expantype[0]) + expantype.Substring(1).ToLower().Replace("_", " "));
                bool simple = false;
                if (gps.GridSurface.SpanType == GridSurface.Span_Type.TWO_WAY_SIMPLIFIED_TRIBUTARY_AREAS)
                    simple = true;
                DA.SetData(17, simple);

            }
        }
    }
}
