using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaAPI;
using OasysGH;
using OasysGH.Parameters;
using Rhino.Geometry;

namespace GsaGH.Parameters
{
  /// <summary>
  /// Goo wrapper class, makes sure <see cref="GsaElement2d"/> can be used in Grasshopper.
  /// </summary>
  public class GsaElement2dGoo : GH_OasysGeometricGoo<GsaElement2d>, IGH_PreviewData
  {
    public static string Name => "Element2D";
    public static string NickName => "E2D";
    public static string Description => "GSA 2D Element(s)";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    public GsaElement2dGoo(GsaElement2d item) : base(item) { }
    public override IGH_GeometricGoo Duplicate() => new GsaElement2dGoo(this.Value);
    public override GeometryBase GetGeometry() => this.Value.Mesh;

    #region casting methods
    public override bool CastTo<Q>(ref Q target)
    {
      // This function is called when Grasshopper needs to convert this 
      // instance of GsaElement2D into some other type Q.            
      if (base.CastTo<Q>(ref target))
        return true;

      if (typeof(Q).IsAssignableFrom(typeof(Mesh)))
      {
        if (Value == null)
          target = default;
        else
          target = (Q)(object)Value.Mesh;
        return true;
      }

      if (typeof(Q).IsAssignableFrom(typeof(GH_Mesh)))
      {
        if (Value == null)
          target = default;
        else
        {
          target = (Q)(object)new GH_Mesh(Value.Mesh);
        }

        return true;
      }

      target = default;
      return false;
    }
    public override bool CastFrom(object source)
    {
      // This function is called when Grasshopper needs to convert other data 
      // into GsaElement.
      if (source == null) { return false; }

      if (base.CastFrom(source))
        return true;

      if (typeof(Element).IsAssignableFrom(source.GetType()))
      {
        if (Value.API_Elements.Count > 1) { return false; } // we cannot convert a list on the fly
        Value.API_Elements[0] = (Element)source; //If someone should want to just test if they can convert a Mesh face
        return true;
      }

      Mesh mesh = new Mesh();
      if (GH_Convert.ToMesh(source, ref mesh, GH_Conversion.Both))
      {
        GsaElement2d elem = new GsaElement2d(mesh);
        this.Value = elem;
        return true;
      }

      return false;
    }
    #endregion

    #region transformation methods
    public override IGH_GeometricGoo Transform(Transform xform)
    {
      if (Value == null) { return null; }
      if (Value.Mesh == null) { return null; }

      GsaElement2d dup = Value.Duplicate(true);
      dup.Ids = new List<int>(new int[dup.Mesh.Faces.Count()]);
      Mesh xMs = dup.Mesh.DuplicateMesh();
      xMs.Transform(xform);
      return new GsaElement2dGoo(dup.UpdateGeometry(xMs));
    }

    public override IGH_GeometricGoo Morph(SpaceMorph xmorph)
    {
      if (Value == null) { return null; }
      if (Value.Mesh == null) { return null; }

      GsaElement2d dup = Value.Duplicate(true);
      dup.Ids = new List<int>(new int[dup.Mesh.Faces.Count()]);
      Mesh xMs = dup.Mesh.DuplicateMesh();
      xmorph.Morph(xMs);
      return new GsaElement2dGoo(dup.UpdateGeometry(xMs));
    }

    #endregion

    #region drawing methods
    public override void DrawViewportMeshes(GH_PreviewMeshArgs args)
    {
      //Draw shape.
      if (args.Material.Diffuse == System.Drawing.Color.FromArgb(255, 150, 0, 0)) // this is a workaround to change colour between selected and not
      {
        args.Pipeline.DrawMeshShaded(Value.Mesh, Helpers.Graphics.Colours.Element2dFace);
      }
      else
        args.Pipeline.DrawMeshShaded(Value.Mesh, Helpers.Graphics.Colours.Element2dFaceSelected);
    }
    public override void DrawViewportWires(GH_PreviewWireArgs args)
    {
      if (Value == null) { return; }
      if (Grasshopper.CentralSettings.PreviewMeshEdges == false) { return; }

      //Draw lines
      if (Value.Mesh != null)
      {
        if (args.Color == System.Drawing.Color.FromArgb(255, 150, 0, 0)) // this is a workaround to change colour between selected and not
        {
          args.Pipeline.DrawMeshWires(Value.Mesh, Helpers.Graphics.Colours.Element2dEdge, 1);
        }
        else
        {
          args.Pipeline.DrawMeshWires(Value.Mesh, Helpers.Graphics.Colours.Element2dEdgeSelected, 2);
        }
      }
    }
    #endregion
  }
}
