﻿using System;
using System.Collections;
using System.Collections.Generic;

using Rhino;
using Rhino.Geometry;

using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;

using System.IO;
using System.Linq;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Runtime.InteropServices;

using Rhino.DocObjects;
using Rhino.Collections;
using GH_IO;
using GH_IO.Serialization;
using Rhino.Display;

namespace GhSA.Parameters
{
    public class ResultPoint : GH_GeometricGoo<Point3d>, IGH_PreviewData
    {
        public ResultPoint(Point3d point, double result, Color colour, float size)
        : base(point) 
        { 
            m_result = result;
            m_size = size;
            m_colour = colour;
        }

        private double m_result;
        private float m_size;
        private Color m_colour;

        public override string ToString()
        {
            return string.Format("NodeResult: P:{0:0.0},{1:0.0},{2:0.0}, Val:{3:0.0}", Value.X, Value.Y, Value.Z, m_result);
        }
        public override string TypeName
        {
            get { return "Result Point"; }
        }
        public override string TypeDescription
        {
            get { return "A GSA result point type."; }
        }

        public override IGH_GeometricGoo DuplicateGeometry()
        {
            return new ResultPoint(Value, m_result, m_colour, m_size);
        }
        public override BoundingBox Boundingbox
        {
            get
            {
                BoundingBox box = new BoundingBox(Value, Value);
                box.Inflate(1);
                return box;
            }
        }
        public override BoundingBox GetBoundingBox(Transform xform)
        {
            Point3d point = Value;
            point.Transform(xform);
            BoundingBox box = new BoundingBox(point, point);
            box.Inflate(1);
            return box;
        }
        public override IGH_GeometricGoo Transform(Transform xform)
        {
            Point3d point = Value;
            point.Transform(xform);
            return new ResultPoint(point, m_result, m_colour, m_size);
        }
        public override IGH_GeometricGoo Morph(SpaceMorph xmorph)
        {
            Point3d point = xmorph.MorphPoint(Value);
            return new ResultPoint(point, m_result, m_colour, m_size);
        }

        public override object ScriptVariable()
        {
            return Value;
        }
        public override bool CastTo<TQ>(out TQ target)
        {
            if (typeof(TQ).IsAssignableFrom(typeof(Point3d)))
            {
                target = (TQ)(object)Value;
                return true;
            }

            if (typeof(TQ).IsAssignableFrom(typeof(GH_Point)))
            {
                target = (TQ)(object)new GH_Point(Value);
                return true;
            }

            target = default(TQ);
            return false;
        }
        public override bool CastFrom(object source)
        {
            if (source == null) return false;
            if (source is Point3d)
            {
                Value = (Point3d)source;
                return true;
            }
            GH_Point pointGoo = source as GH_Point;
            if (pointGoo != null)
            {
                Value = pointGoo.Value;
                return true;
            }

            Point3d point = Point3d.Unset;
            if (GH_Convert.ToPoint3d(source, ref point, GH_Conversion.Both))
            {
                Value = point;
                return true;
            }

            return false;
        }

        public BoundingBox ClippingBox
        {
            get { return Boundingbox; }
        }
        public void DrawViewportWires(GH_PreviewWireArgs args)
        {
            
            args.Pipeline.DrawPoint(Value, PointStyle.RoundSimple, m_size, m_colour);
            
        }
        public void DrawViewportMeshes(GH_PreviewMeshArgs args) { }
    }
}
