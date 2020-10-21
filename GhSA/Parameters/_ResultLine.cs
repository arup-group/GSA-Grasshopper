using System;
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
    public class ResultLine : GH_GeometricGoo<Line>, IGH_PreviewData
    {
        public ResultLine(Line line, double result1, double result2, Color colour1, Color colour2, float size1, float size2)
        : base(line) 
        { 
            m_result1 = result1;
            m_result2 = result2;
            m_size1 = size1;
            m_size2 = size2;
            m_colour1 = colour1;
            m_colour2 = colour2;
        }

        private double m_result1;
        private double m_result2;
        private float m_size1;
        private float m_size2;
        private Color m_colour1;
        private Color m_colour2;

        public override string ToString()
        {
            return string.Format("LineResult: L:{0:0.0}, Val1:{1:0.0}, Val2:{2:0.0}", Value.Length, m_result1, m_result2);
        }
        public override string TypeName
        {
            get { return "Result Line"; }
        }
        public override string TypeDescription
        {
            get { return "A GSA result line type."; }
        }

        public override IGH_GeometricGoo DuplicateGeometry()
        {
            return new ResultLine(Value, m_result1, m_result2, m_colour1, m_colour2, m_size1, m_size2);
        }
        public override BoundingBox Boundingbox
        {
            get
            {
                return Value.BoundingBox;
            }
        }
        public override BoundingBox GetBoundingBox(Transform xform)
        {
            Line ln = Value;
            ln.Transform(xform);
            return ln.BoundingBox;
        }
        public override IGH_GeometricGoo Transform(Transform xform)
        {
            Line ln = Value;
            ln.Transform(xform);
            return new ResultLine(ln, m_result1, m_result2, m_colour1, m_colour2, m_size1, m_size2);
        }
        public override IGH_GeometricGoo Morph(SpaceMorph xmorph)
        {
            Point3d start = xmorph.MorphPoint(Value.From);
            Point3d end = xmorph.MorphPoint(Value.To);
            Line ln = new Line(start, end);
            return new ResultLine(ln, m_result1, m_result2, m_colour1, m_colour2, m_size1, m_size2);
        }

        public override object ScriptVariable()
        {
            return Value;
        }
        public override bool CastTo<TQ>(out TQ target)
        {
            if (typeof(TQ).IsAssignableFrom(typeof(Line)))
            {
                target = (TQ)(object)Value;
                return true;
            }

            if (typeof(TQ).IsAssignableFrom(typeof(GH_Line)))
            {
                target = (TQ)(object)new GH_Line(Value);
                return true;
            }

            target = default(TQ);
            return false;
        }
        public override bool CastFrom(object source)
        {
            if (source == null) return false;
            if (source is Line)
            {
                Value = (Line)source;
                return true;
            }
            GH_Line lineGoo = source as GH_Line;
            if (lineGoo != null)
            {
                Value = lineGoo.Value;
                return true;
            }

            Line line = new Line();
            if (GH_Convert.ToLine(source, ref line, GH_Conversion.Both))
            {
                Value = line;
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
            int numDiv = 40;

            Grasshopper.GUI.Gradient.GH_Gradient gH_Gradient = new Grasshopper.GUI.Gradient.GH_Gradient();
            gH_Gradient.AddGrip(0, m_colour1);
            gH_Gradient.AddGrip(1, m_colour2);

            for (int i = 0; i < numDiv + 1; i++)
            {
                double t = (double)i / numDiv;
                Color col = gH_Gradient.ColourAt(t);
                int thk = (int)Math.Abs(((m_size2 - m_size1) * t + m_size1));
                Line ln = new Line(Value.PointAt((double)i / (numDiv - 1)), Value.PointAt((double)(i + 1) / (numDiv - 1)));
                args.Pipeline.DrawLine(ln, col, thk);
            }
            
        }
        public void DrawViewportMeshes(GH_PreviewMeshArgs args) { }
    }
}
