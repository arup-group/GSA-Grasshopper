using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using GsaAPI;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using Rhino;
using GhSA.Util.Gsa;
using Grasshopper.Documentation;
using Rhino.Collections;

namespace GhSA.Parameters
{
    /// <summary>
    /// Individual load type classes holding GsaAPI load type along with any required geometry objects
    /// </summary>
    /// 

    #region internal load classes
    public class GsaGravityLoad
    {
        public GravityLoad GravityLoad
        {
            get { return m_gravityload; }
            set { m_gravityload = value; }
        }
        #region fields
        private GravityLoad m_gravityload;
        #endregion
        #region constructor
        public GsaGravityLoad()
        {
            m_gravityload = new GravityLoad();
        }
        #endregion
    }
    public class GsaNodeLoad
    {
        public NodeLoad NodeLoad
        {
            get { return m_nodeload; }
            set { m_nodeload = value; }
        }

        public NodeLoadTypes NodeLoadType;

        public enum NodeLoadTypes // direct copy from GSA API enums
        {
            NODE_LOAD = 0,
            APPLIED_DISP = 1,
            SETTLEMENT = 2,
            GRAVITY = 3,
            NUM_TYPES = 4
        }

        #region fields
        private NodeLoad m_nodeload;
        #endregion
        #region constructor
        public GsaNodeLoad()
        {
            m_nodeload = new NodeLoad();
            NodeLoadType = NodeLoadTypes.NODE_LOAD;
        }
        #endregion
    }
    public class GsaBeamLoad
    {
        public BeamLoad BeamLoad
        {
            get { return m_beamload; }
            set { m_beamload = value; }
        }
        public enum BeamLoadTypes // direct copy from GSA API enums
        {
            UNDEF = 0,
            POINT = 1,
            UNIFORM = 2,
            LINEAR = 3,
            PATCH = 4,
            TRILINEAR = 5
        }
        public BeamLoadTypes BeamLoadType;
        #region fields
        private BeamLoad m_beamload;
        #endregion
        #region constructor
        public GsaBeamLoad()
        {
            m_beamload = new BeamLoad();
            BeamLoadType = BeamLoadTypes.LINEAR;
        }
        #endregion
    }
    public class GsaFaceLoad
    {
        public FaceLoad FaceLoad
        {
            get { return m_faceload; }
            set { m_faceload = value; }
        }
        public enum FaceLoadTypes // direct copy from GSA API enums
        {
            UNDEF = 0,
            CONSTANT = 1,
            GENERAL = 2,
            POINT = 3
        }
        public FaceLoadTypes FaceLoadType;
        #region fields
        private FaceLoad m_faceload;
        #endregion
        #region constructor
        public GsaFaceLoad()
        {
            m_faceload = new FaceLoad();
            FaceLoadType = FaceLoadTypes.CONSTANT;
        }
        #endregion
    }
    public class GsaGridSurface
    {
        public GridSurface GridSurface
        {
            get { return m_gridsurface; }
            set { m_gridsurface = value; }
        }
        public GridPlane GridPlane
        {
            get { return m_gridplane; }
            set { m_gridplane = value; }
        }
        public Plane LocalAxis
        {
            get { return m_plane; }
            set { m_plane = value; }
        }
        #region fields
        private GridSurface m_gridsurface;
        private GridPlane m_gridplane;
        private Plane m_plane;
        #endregion
        #region constructor
        public GsaGridSurface()
        {
            m_gridsurface = new GridSurface();
            m_gridplane = new GridPlane();
            m_plane = new Plane();
        }
        #endregion
    }
    public class GsaGridPointLoad
    {
        public GridPointLoad GridPointLoad
        {
            get { return m_gridpointload; }
            set { m_gridpointload = value; }
        }
        public GsaGridSurface GridSurface
        {
            get { return m_gridsrf; }
            set { m_gridsrf = value; }
        }
        #region fields
        private GridPointLoad m_gridpointload;
        private GsaGridSurface m_gridsrf;
        #endregion
        #region constructor
        public GsaGridPointLoad()
        {
            m_gridpointload = new GridPointLoad();
            m_gridsrf = new GsaGridSurface();
        }
        #endregion
    }
    public class GsaGridLineLoad
    {
        public GridLineLoad GridLineLoad
        {
            get { return m_gridlineload; }
            set { m_gridlineload = value; }
        }
        
        public GsaGridSurface GridSurface
        {
            get { return m_gridsrf; }
            set { m_gridsrf = value; }
        }
        public enum PolyLineTypes // direct copy from GSA API enums
        {
            EXPLICIT_POLYLINE = 0,
            POLYLINE_REFERENCE = 1
        }
        public PolyLineTypes PolyLineType;
        #region fields
        private GridLineLoad m_gridlineload;
        private GsaGridSurface m_gridsrf;
        #endregion
        #region constructor
        public GsaGridLineLoad()
        {
            m_gridlineload = new GridLineLoad();
            PolyLineType = PolyLineTypes.EXPLICIT_POLYLINE;
            m_gridsrf = new GsaGridSurface();
        }
        #endregion
    }
    public class GsaGridAreaLoad
    {
        public GridAreaLoad GridAreaLoad
        {
            get { return m_gridareaload; }
            set { m_gridareaload = value; }
        }
        public GsaGridSurface GridSurface
        {
            get { return m_gridsrf; }
            set { m_gridsrf = value; }
        }
        public enum PolyLineTypes // direct copy from GSA API enums
        {
            PLANE = 1,
            POLYREF = 2,
            POLYGON = 3
        }
        public PolyLineTypes PolyLineType;
        #region fields
        private GridAreaLoad m_gridareaload;
        private GsaGridSurface m_gridsrf;
        #endregion
        #region constructor
        public GsaGridAreaLoad()
        {
            m_gridareaload = new GridAreaLoad();
            PolyLineType = PolyLineTypes.PLANE;
            m_gridsrf = new GsaGridSurface();
        }
        #endregion
    }
    #endregion
    /// <summary>
    /// GsaLoad class holding all load types
    /// </summary>
    public class GsaLoad
    {
        public enum LoadTypes
        {
            [Description("Gravity Load")] Gravity,
            [Description("Node Load")] Node,
            [Description("Beam Load")] Beam,
            [Description("Face Load")] Face,
            [Description("Grid Point Load")] GridPoint,
            [Description("Grid Line Load")] GridLine,
            [Description("Grid Area Load")] GridArea,
        }
        public LoadTypes LoadType;

        public GsaGravityLoad GravityLoad
        {
            get { return m_gravity; }
            set { m_gravity = value; }
        }
        public GsaNodeLoad NodeLoad
        {
            get { return m_node; }
            set { m_node = value; }
        }
        public GsaBeamLoad BeamLoad
        {
            get { return m_beam; }
            set { m_beam = value; }
        }
        public GsaFaceLoad FaceLoad
        {
            get { return m_face; }
            set { m_face = value; }
        }
        public GsaGridPointLoad PointLoad
        {
            get { return m_gridpoint; }
            set { m_gridpoint = value; }
        }
        public GsaGridLineLoad LineLoad
        {
            get { return m_gridline; }
            set { m_gridline = value; }
        }
        public GsaGridAreaLoad AreaLoad
        {
            get { return m_gridarea; }
            set { m_gridarea = value; }
        }
        #region fields
        private GsaGravityLoad m_gravity;
        private GsaNodeLoad m_node;
        private GsaBeamLoad m_beam;
        private GsaFaceLoad m_face;
        private GsaGridPointLoad m_gridpoint;
        private GsaGridLineLoad m_gridline;
        private GsaGridAreaLoad m_gridarea;
        #endregion

        #region constructors
        public GsaLoad(GsaGravityLoad gravityload)
        {
            GravityLoad = gravityload;
            LoadType = LoadTypes.Gravity;
        }
        public GsaLoad(GsaNodeLoad nodeload)
        {
            NodeLoad = nodeload;
            LoadType = LoadTypes.Node;
        }
        public GsaLoad(GsaBeamLoad beamload)
        {
            BeamLoad = beamload;
            LoadType = LoadTypes.Beam;
        }
        public GsaLoad(GsaFaceLoad faceload)
        {
            FaceLoad = faceload;
            LoadType = LoadTypes.Face;
        }
        public GsaLoad(GsaGridPointLoad gridpointload)
        {
            PointLoad = gridpointload;
            LoadType = LoadTypes.GridPoint;
        }
        public GsaLoad(GsaGridLineLoad gridlineload)
        {
            LineLoad = gridlineload;
            LoadType = LoadTypes.GridLine;
        }
        public GsaLoad(GsaGridAreaLoad gridareaload)
        {
            AreaLoad = gridareaload;
            LoadType = LoadTypes.GridArea;
        }
        public GsaLoad Duplicate()
        {
            GsaLoad dup;
            switch (LoadType)
            {
                case LoadTypes.Gravity:
                    dup = new GsaLoad(m_gravity);
                    return dup;
                case LoadTypes.Node:
                    dup = new GsaLoad(m_node);
                    return dup;
                case LoadTypes.Beam:
                    dup = new GsaLoad(m_beam);
                    return dup;
                case LoadTypes.Face:
                    dup = new GsaLoad(m_face);
                    return dup;
                case LoadTypes.GridPoint:
                    dup = new GsaLoad(m_gridpoint);
                    return dup;
                case LoadTypes.GridLine:
                    dup = new GsaLoad(m_gridline);
                    return dup;
                case LoadTypes.GridArea:
                    dup = new GsaLoad(m_gridarea);
                    return dup;
            }
            return default;
        }
        #endregion

        #region properties
        public bool IsValid
        {
            get
            {
                return true;
            }
        }


        #endregion

        #region methods
        public override string ToString()
        {
            string str = LoadType.ToString();
            return "GSA Load " + str;
        }

        #endregion
    }

    /// <summary>
    /// GsaLoad Goo wrapper class, makes sure GsaLoad can be used in Grasshopper.
    /// </summary>
    public class GsaLoadGoo : GH_Goo<GsaLoad>
    {
        #region constructors
        public GsaLoadGoo()
        {
            this.Value = null; // new GsaLoad();
        }
        public GsaLoadGoo(GsaLoad load)
        {
            this.Value = load.Duplicate();
        }

        public override IGH_Goo Duplicate()
        {
            return DuplicateGsaLoad();
        }
        public GsaLoadGoo DuplicateGsaLoad()
        {
            return new GsaLoadGoo(Value == null ? null : Value.Duplicate());
        }
        #endregion

        #region properties
        public override bool IsValid
        {
            get
            {
                if (Value == null) { return false; }
                return true;
            }
        }
        public override string IsValidWhyNot
        {
            get
            {
                //if (Value == null) { return "No internal GsaMember instance"; }
                if (Value.IsValid) { return string.Empty; }
                return Value.IsValid.ToString(); //Todo: beef this up to be more informative.
            }
        }
        public override string ToString()
        {
            if (Value == null)
                return "Null GSA Load";
            else
                return Value.ToString();
        }
        public override string TypeName
        {
            get { return ("GSA Load"); }
        }
        public override string TypeDescription
        {
            get { return ("GSA Load containing different load types"); }
        }


        #endregion

        #region casting methods
        public override bool CastTo<Q>(ref Q target)
        {
            // This function is called when Grasshopper needs to convert this 
            // instance of GsaLoad into some other type Q.            


            if (typeof(Q).IsAssignableFrom(typeof(GsaLoad)))
            {
                if (Value == null)
                    target = default;
                else
                    target = (Q)(object)Value;
                return true;
            }

            if (typeof(Q).IsAssignableFrom(typeof(GH_Plane)))
            {
                if (Value == null)
                    target = default;
                else
                {
                    if (Value.LoadType == GsaLoad.LoadTypes.GridArea)
                    {
                        GH_Plane ghpln = new GH_Plane();
                        GH_Convert.ToGHPlane(Value.AreaLoad.GridSurface.LocalAxis, GH_Conversion.Both, ref ghpln);
                        target = (Q)(object)ghpln;
                        return true;
                    }
                    if (Value.LoadType == GsaLoad.LoadTypes.GridLine)
                    {
                        GH_Plane ghpln = new GH_Plane();
                        GH_Convert.ToGHPlane(Value.LineLoad.GridSurface.LocalAxis, GH_Conversion.Both, ref ghpln);
                        target = (Q)(object)ghpln;
                        return true;
                    }
                    if (Value.LoadType == GsaLoad.LoadTypes.GridPoint)
                    {
                        GH_Plane ghpln = new GH_Plane();
                        GH_Convert.ToGHPlane(Value.PointLoad.GridSurface.LocalAxis, GH_Conversion.Both, ref ghpln);
                        target = (Q)(object)ghpln;
                        return true;
                    }
                }
                return false;
            }
            if (typeof(Q).IsAssignableFrom(typeof(GH_Point)))
            {
                if (Value == null)
                    target = default;
                else
                {
                    if (Value.LoadType == GsaLoad.LoadTypes.GridPoint)
                    {
                        Point3d point = new Point3d();
                        point.X = Value.PointLoad.GridPointLoad.X;
                        point.Y = Value.PointLoad.GridPointLoad.Y;
                        point.X = Value.PointLoad.GridSurface.LocalAxis.OriginZ;
                        GH_Point ghpt = new GH_Point();
                        GH_Convert.ToGHPoint(point, GH_Conversion.Both, ref ghpt);
                        target = (Q)(object)ghpt;
                        return true;
                    }
                }
                return false;
            }
            if (typeof(Q).IsAssignableFrom(typeof(GH_Curve)))
            {
                if (Value == null)
                    target = default;
                else
                {
                    if (Value.LoadType == GsaLoad.LoadTypes.GridLine)
                    {
                        List<Point3d> pts = new List<Point3d>();
                        string def = Value.LineLoad.GridLineLoad.PolyLineDefinition; //implement converter
                        // to be done
                        //target = (Q)(object)ghpt;
                        //return true;
                    }
                }
                return false;
            }

            target = default;
            return false;
        }
        public override bool CastFrom(object source)
        {
            // This function is called when Grasshopper needs to convert other data 
            // into GsaLoad.


            if (source == null) { return false; }

            //Cast from GsaLoad
            if (typeof(GsaLoad).IsAssignableFrom(source.GetType()))
            {
                Value = (GsaLoad)source;
                return true;
            }


            return false;
        }
        #endregion


    }

    /// <summary>
    /// This class provides a Parameter interface for the Data_GsaLoad type.
    /// </summary>
    public class GsaLoadParameter : GH_PersistentParam<GsaLoadGoo>
    {
        public GsaLoadParameter()
          : base(new GH_InstanceDescription("GSA Load", "Load", "GSA Load", GhSA.Components.Ribbon.CategoryName.Name(), GhSA.Components.Ribbon.SubCategoryName.Cat9()))
        {
        }

        public override Guid ComponentGuid => new Guid("2833ef04-c595-4b05-8db3-622c75fa9a25");

        public override GH_Exposure Exposure => GH_Exposure.secondary;

        //protected override Bitmap Icon => Resources.CrossSections;

        protected override GH_GetterResult Prompt_Plural(ref List<GsaLoadGoo> values)
        {
            return GH_GetterResult.cancel;
        }
        protected override GH_GetterResult Prompt_Singular(ref GsaLoadGoo value)
        {
            return GH_GetterResult.cancel;
        }
        protected override System.Windows.Forms.ToolStripMenuItem Menu_CustomSingleValueItem()
        {
            System.Windows.Forms.ToolStripMenuItem item = new System.Windows.Forms.ToolStripMenuItem
            {
                Text = "Not available",
                Visible = false
            };
            return item;
        }
        protected override System.Windows.Forms.ToolStripMenuItem Menu_CustomMultiValueItem()
        {
            System.Windows.Forms.ToolStripMenuItem item = new System.Windows.Forms.ToolStripMenuItem
            {
                Text = "Not available",
                Visible = false
            };
            return item;
        }

        #region preview methods

        public bool Hidden
        {
            get { return true; }
            //set { m_hidden = value; }
        }
        public bool IsPreviewCapable
        {
            get { return false; }
        }
        #endregion
    }
}
