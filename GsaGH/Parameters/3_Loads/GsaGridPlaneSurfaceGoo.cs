using System;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

namespace GsaGH.Parameters
{
  /// <summary>
  /// Goo wrapper class, makes sure <see cref="GsaGridPlaneSurface"/> can be used in Grasshopper.
  /// </summary>
  public class GsaGridPlaneSurfaceGoo : GH_GeometricGoo<GsaGridPlaneSurface>, IGH_PreviewData
  {
    public static string Name => "GridPlaneSurface";
    public static string NickName => "GPS";
    public static string Description => "GSA Grid Plane Surface";

    #region constructors
    public GsaGridPlaneSurfaceGoo()
    {
      this.Value = new GsaGridPlaneSurface();
    }

    public GsaGridPlaneSurfaceGoo(GsaGridPlaneSurface gridplane)
    {
      if (gridplane == null)
        this.Value = null;
      else
        this.Value = gridplane; //gridplane.Duplicate();
    }

    public override IGH_GeometricGoo DuplicateGeometry()
    {
      return DuplicateGsaNode();
    }

    public GsaGridPlaneSurfaceGoo DuplicateGsaNode()
    {
      return new GsaGridPlaneSurfaceGoo(Value == null ? new GsaGridPlaneSurface() : Value); //Value.Duplicate());
    }
    #endregion

    #region properties
    public override bool IsValid
    {
      get
      {
        if (Value == null) { return false; }
        return Value.IsValid;
      }
    }

    public override string IsValidWhyNot
    {
      get
      {
        if (Value.IsValid) { return string.Empty; }
        return Value.Plane.IsValid.ToString(); //Todo: beef this up to be more informative.
      }
    }

    public override string ToString()
    {
      if (Value == null)
        return "Null GridPlane";
      else
        return Value.ToString();
    }

    public override string TypeName
    {
      get { return ("GridPlane"); }
    }

    public override string TypeDescription
    {
      get { return ("GSA Grid Plane"); }
    }

    public override BoundingBox Boundingbox
    {
      get
      {
        if (Value == null) { return BoundingBox.Empty; }
        if (Value.Plane == null) { return BoundingBox.Empty; }
        Point3d pt1 = Value.Plane.Origin;
        pt1.Z += 10;
        Point3d pt2 = Value.Plane.Origin;
        pt2.Z += -10;
        Line ln = new Line(pt1, pt2);
        LineCurve crv = new LineCurve(ln);
        return crv.GetBoundingBox(false);
      }
    }
    public override BoundingBox GetBoundingBox(Transform xform)
    {
      if (Value == null) { return BoundingBox.Empty; }
      if (Value.Plane.Origin == null) { return BoundingBox.Empty; }
      Point3d pt = Value.Plane.Origin;
      pt.Z += 0.001;
      Line ln = new Line(Value.Plane.Origin, pt);
      LineCurve crv = new LineCurve(ln);
      return crv.GetBoundingBox(xform); //BoundingBox.Empty; //Value.point.GetBoundingBox(xform);
    }
    #endregion

    #region casting methods
    public override bool CastTo<Q>(out Q target)
    {
      // This function is called when Grasshopper needs to convert this 
      // instance of GsaGridPlane into some other type Q.            


      if (typeof(Q).IsAssignableFrom(typeof(GsaGridPlaneSurface)))
      {
        if (Value == null)
          target = default;
        else
          target = (Q)(object)Value.Duplicate();
        return true;
      }

      if (typeof(Q).IsAssignableFrom(typeof(GH_Plane)))
      {
        if (Value == null)
          target = default;
        else
        {
          GH_Plane pln = new GH_Plane();
          GH_Convert.ToGHPlane(Value.Plane, GH_Conversion.Both, ref pln);
          target = (Q)(object)pln;
        }
        return true;
      }

      target = default;
      return false;
    }
    public override bool CastFrom(object source)
    {
      // This function is called when Grasshopper needs to convert other data 
      // into GsaGridPlane.


      if (source == null) { return false; }

      //Cast from GsaGridPlane
      if (typeof(GsaGridPlaneSurface).IsAssignableFrom(source.GetType()))
      {
        Value = (GsaGridPlaneSurface)source;
        return true;
      }

      //Cast from Plane
      Plane pln = new Plane();

      if (GH_Convert.ToPlane(source, ref pln, GH_Conversion.Both))
      {
        GsaGridPlaneSurface gridplane = new GsaGridPlaneSurface(pln);
        this.Value = gridplane;
        return true;
      }

      return false;
    }
    #endregion

    #region transformation methods
    public override IGH_GeometricGoo Transform(Transform xform)
    {
      if (Value == null) { return null; }
      if (Value.Plane == null) { return null; }
      GsaGridPlaneSurface dup = Value.Duplicate();
      Plane pln = dup.Plane;
      pln.Transform(xform);
      GsaGridPlaneSurface gridplane = new GsaGridPlaneSurface(pln);
      return new GsaGridPlaneSurfaceGoo(gridplane);
    }

    public override IGH_GeometricGoo Morph(SpaceMorph xmorph)
    {
      if (Value == null) { return null; }
      if (Value.Plane == null) { return null; }
      GsaGridPlaneSurface dup = Value.Duplicate();
      Plane pln = dup.Plane;
      xmorph.Morph(ref pln);
      GsaGridPlaneSurface gridplane = new GsaGridPlaneSurface(pln);
      return new GsaGridPlaneSurfaceGoo(gridplane);
    }

    #endregion

    #region drawing methods
    public BoundingBox ClippingBox
    {
      get { return Boundingbox; }
    }
    public void DrawViewportMeshes(GH_PreviewMeshArgs args)
    {
      //No meshes are drawn.   
    }
    public void DrawViewportWires(GH_PreviewWireArgs args)
    {
      if (Value == null) { return; }


      if (Value.Plane.IsValid)
      {
        if (args.Color == System.Drawing.Color.FromArgb(255, 150, 0, 0)) // this is a workaround to change colour between selected and not
        {
          GH_Plane.DrawPlane(args.Pipeline, Value.Plane, 16, 16, System.Drawing.Color.Gray, System.Drawing.Color.Red, System.Drawing.Color.Green);
          args.Pipeline.DrawPoint(Value.Plane.Origin, Rhino.Display.PointStyle.RoundSimple, 3, UI.Colour.Node);
        }
        else
        {
          GH_Plane.DrawPlane(args.Pipeline, Value.Plane, 16, 16, System.Drawing.Color.LightGray, System.Drawing.Color.Red, System.Drawing.Color.Green);
          args.Pipeline.DrawPoint(Value.Plane.Origin, Rhino.Display.PointStyle.RoundControlPoint, 3, UI.Colour.NodeSelected);
        }
      }
    }
    #endregion
  }
}
