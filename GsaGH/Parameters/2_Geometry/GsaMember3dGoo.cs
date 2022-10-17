using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaAPI;
using OasysGH;
using OasysGH.Parameters;
using Rhino.Geometry;

namespace GsaGH.Parameters
{
  /// <summary>
  /// Goo wrapper class, makes sure <see cref="GsaMember3d"/> can be used in Grasshopper.
  /// </summary>
  public class GsaMember3dGoo : GH_OasysGeometricGoo<GsaMember3d>, IGH_PreviewData
  {
    public static string Name => "Member3D";
    public static string NickName => "M3D";
    public static string Description => "GSA 3D Member";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    public GsaMember3dGoo(GsaMember3d item) : base(item) { }
    public override IGH_GeometricGoo Duplicate() => new GsaMember3dGoo(this.Value);
    public override GeometryBase GetGeometry() => this.Value.SolidMesh;

    #region casting methods
    public override bool CastTo<Q>(ref Q target)
    {
      // This function is called when Grasshopper needs to convert this 
      // instance of GsaMember into some other type Q.            
      if (base.CastTo<Q>(ref target))
        return true;

      //Cast to Mesh
      if (typeof(Q).IsAssignableFrom(typeof(Mesh)))
      {
        if (Value == null)
          target = default;
        else
          target = (Q)(object)Value.SolidMesh;
        return true;
      }
      if (typeof(Q).IsAssignableFrom(typeof(GH_Mesh)))
      {
        if (Value == null)
          target = default;
        else
        {
          target = (Q)(object)new GH_Mesh(Value.SolidMesh);
          if (Value.SolidMesh == null)
            return false;
        }

        return true;
      }

      if (typeof(Q).IsAssignableFrom(typeof(GH_Integer)))
      {
        if (Value == null)
          target = default;
        else
        {
          GH_Integer ghint = new GH_Integer();
          if (GH_Convert.ToGHInteger(Value.ID, GH_Conversion.Both, ref ghint))
            target = (Q)(object)ghint;
          else
            target = default;
        }
        return true;
      }

      target = default;
      return false;
    }
    public override bool CastFrom(object source)
    {
      // This function is called when Grasshopper needs to convert other data 
      // into GsaMember.
      if (source == null) { return false; }

      if (base.CastFrom(source))
        return true;

      //Cast from Brep
      Brep brep = new Brep();
      if (GH_Convert.ToBrep(source, ref brep, GH_Conversion.Both))
      {
        GsaMember3d member = new GsaMember3d(brep);
        this.Value = member;
        return true;
      }

      //Cast from Mesh
      Mesh mesh = new Mesh();

      if (GH_Convert.ToMesh(source, ref mesh, GH_Conversion.Both))
      {
        GsaMember3d member = new GsaMember3d(mesh);
        this.Value = member;
        return true;
      }

      return false;
    }
    #endregion

    #region transformation methods
    public override IGH_GeometricGoo Transform(Transform xform)
    {
      if (Value == null) { return null; }
      if (Value.SolidMesh == null) { return null; }

      GsaMember3d dup = Value.Duplicate(true);
      dup.ID = 0;
      dup.SolidMesh.Transform(xform);

      return new GsaMember3dGoo(dup);
    }

    public override IGH_GeometricGoo Morph(SpaceMorph xmorph)
    {
      if (Value == null) { return null; }
      if (Value.SolidMesh == null) { return null; }

      GsaMember3d dup = Value.Duplicate(true);
      dup.ID = 0;
      xmorph.Morph(dup.SolidMesh.Duplicate());

      return new GsaMember3dGoo(dup);
    }

    #endregion

    #region drawing methods
    public override void DrawViewportMeshes(GH_PreviewMeshArgs args)
    {
      //Draw shape.
      if (Value.SolidMesh != null)
      {
        if (!Value.IsDummy)
        {
          if (args.Material.Diffuse == System.Drawing.Color.FromArgb(255, 150, 0, 0)) // this is a workaround to change colour between selected and not
            args.Pipeline.DrawMeshShaded(Value.SolidMesh, UI.Colour.Element2dFace); //UI.Colour.Member2dFace
          else
            args.Pipeline.DrawMeshShaded(Value.SolidMesh, UI.Colour.Element2dFaceSelected);
        }
        else
          args.Pipeline.DrawMeshShaded(Value.SolidMesh, UI.Colour.Dummy2D);
      }
    }
    public override void DrawViewportWires(GH_PreviewWireArgs args)
    {
      if (Value == null) { return; }

      //Draw lines
      if (Value.SolidMesh != null)
      {
        // Draw edges
        if (Value.IsDummy)
        {
          for (int i = 0; i < Value.previewEdgeLines.Count; i++)
          {
            if (args.Color == System.Drawing.Color.FromArgb(255, 150, 0, 0)) // this is a workaround to change colour between selected and not
            {
              args.Pipeline.DrawDottedLine(Value.previewEdgeLines[i], UI.Colour.Dummy1D);
            }
            else
            {
              args.Pipeline.DrawDottedLine(Value.previewEdgeLines[i], UI.Colour.Member2dEdgeSelected);
            }
          }
        }
        else
        {
          for (int i = 0; i < Value.previewEdgeLines.Count; i++)
          {
            if (args.Color == System.Drawing.Color.FromArgb(255, 150, 0, 0)) // this is a workaround to change colour between selected and not
            {
              if ((System.Drawing.Color)Value.Colour != System.Drawing.Color.FromArgb(0, 0, 0))
                args.Pipeline.DrawLine(Value.previewEdgeLines[i], (System.Drawing.Color)Value.Colour, 2);
              else
              {
                System.Drawing.Color col = UI.Colour.Member2dEdge;
                args.Pipeline.DrawLine(Value.previewEdgeLines[i], col, 2);
              }
            }
            else
              args.Pipeline.DrawLine(Value.previewEdgeLines[i], UI.Colour.Element2dEdgeSelected, 2);
          }

          for (int i = 0; i < Value.previewHiddenLines.Count; i++)
          {
            args.Pipeline.DrawDottedPolyline(Value.previewHiddenLines[i], UI.Colour.Dummy1D, false);
          }
        }
        // draw points
        for (int i = 0; i < Value.previewPts.Count; i++)
        {
          if (args.Color == System.Drawing.Color.FromArgb(255, 150, 0, 0)) // this is a workaround to change colour between selected and not
            args.Pipeline.DrawPoint(Value.previewPts[i], Rhino.Display.PointStyle.RoundSimple, 2, (Value.IsDummy) ? UI.Colour.Dummy1D : UI.Colour.Member1dNode);
          else
            args.Pipeline.DrawPoint(Value.previewPts[i], Rhino.Display.PointStyle.RoundControlPoint, 3, UI.Colour.Member1dNodeSelected);
        }
      }
    }
    #endregion
  }
}
