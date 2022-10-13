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
    public override bool CastTo<Q>(out Q target)
    {
      // This function is called when Grasshopper needs to convert this 
      // instance of GsaElement2D into some other type Q.            


      if (typeof(Q).IsAssignableFrom(typeof(GsaElement2d)))
      {
        if (Value == null)
          target = default;
        else
          target = (Q)(object)Value.Duplicate();
        return true;
      }

      if (typeof(Q).IsAssignableFrom(typeof(List<Element>)))
      {
        if (Value == null)
          target = default;
        else
          target = (Q)(object)Value.API_Elements;
        return true;
      }

      //Cast to Mesh
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

      if (typeof(Q).IsAssignableFrom(typeof(List<GH_Integer>)))
      {
        if (Value == null)
          target = default;
        else
        {
          List<GH_Integer> ints = new List<GH_Integer>();

          for (int i = 0; i < Value.Id.Count; i++)
          {
            GH_Integer ghint = new GH_Integer();
            if (GH_Convert.ToGHInteger(Value.Id, GH_Conversion.Both, ref ghint))
              ints.Add(ghint);
          }
          target = (Q)(object)ints;
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

      //Cast from GsaElement
      if (typeof(GsaElement2d).IsAssignableFrom(source.GetType()))
      {
        Value = (GsaElement2d)source;
        return true;
      }

      //Cast from GsaAPI Member
      // we shouldnt provide auto-convertion from GsaAPI.Element
      // as this cannot alone be used to create a line....
      //if (typeof(List<Element>).IsAssignableFrom(source.GetType()))
      //{
      //    Value.Elements = (List<Element>)source;
      //    return true;
      //}

      if (typeof(Element).IsAssignableFrom(source.GetType()))
      {
        if (Value.API_Elements.Count > 1) { return false; } // we cannot convert a list on the fly
        Value.API_Elements[0] = (Element)source; //If someone should want to just test if they can convert a Mesh face
        return true;
      }

      //Cast from Mesh
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
      dup.Id = new List<int>(new int[dup.Mesh.Faces.Count()]);
      Mesh xMs = dup.Mesh.DuplicateMesh();
      xMs.Transform(xform);
      return new GsaElement2dGoo(dup.UpdateGeometry(xMs));
    }

    public override IGH_GeometricGoo Morph(SpaceMorph xmorph)
    {
      if (Value == null) { return null; }
      if (Value.Mesh == null) { return null; }

      GsaElement2d dup = Value.Duplicate(true);
      dup.Id = new List<int>(new int[dup.Mesh.Faces.Count()]);
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
        args.Pipeline.DrawMeshShaded(Value.Mesh, UI.Colour.Element2dFace);
      }
      else
        args.Pipeline.DrawMeshShaded(Value.Mesh, UI.Colour.Element2dFaceSelected);
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
          args.Pipeline.DrawMeshWires(Value.Mesh, UI.Colour.Element2dEdge, 1);
        }
        else
        {
          args.Pipeline.DrawMeshWires(Value.Mesh, UI.Colour.Element2dEdgeSelected, 2);
        }
      }
    }
    #endregion
  }
}
