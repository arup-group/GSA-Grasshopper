using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaAPI;
using Rhino.Geometry;

namespace GsaGH.Parameters
{
  /// <summary>
  /// Goo wrapper class, makes sure <see cref="GsaElement3d"/> can be used in Grasshopper.
  /// </summary>
  public class GsaElement3dGoo : GH_GeometricGoo<GsaElement3d>, IGH_PreviewData
  {
    public static string Name => "3D Element";
    public static string NickName => "E3D";
    public static string Description => "GSA 3D Element";

    #region constructors
    public GsaElement3dGoo()
    {
      this.Value = new GsaElement3d();
    }
    public GsaElement3dGoo(GsaElement3d element)
    {
      if (element == null)
        element = new GsaElement3d();
      this.Value = element; //element.Duplicate();
    }

    public override IGH_GeometricGoo DuplicateGeometry()
    {
      return DuplicateGsaElement3d();
    }
    public GsaElement3dGoo DuplicateGsaElement3d()
    {
      return new GsaElement3dGoo(Value == null ? new GsaElement3d() : Value); //Value.Duplicate());
    }
    #endregion

    #region properties
    public override bool IsValid
    {
      get
      {
        if (Value == null) { return false; }
        if (Value.NgonMesh == null) { return false; }
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
        return "Null Element3D";
      else
        return Value.ToString();
    }
    public override string TypeName
    {
      get { return ("Element 3D"); }
    }
    public override string TypeDescription
    {
      get { return ("GSA 3D Element"); }
    }

    public override BoundingBox Boundingbox
    {
      get
      {
        if (Value == null) { return BoundingBox.Empty; }
        if (Value.DisplayMesh == null) { return BoundingBox.Empty; }
        return Value.DisplayMesh.GetBoundingBox(false);
      }
    }
    public override BoundingBox GetBoundingBox(Transform xform)
    {
      if (Value == null) { return BoundingBox.Empty; }
      if (Value.DisplayMesh == null) { return BoundingBox.Empty; }
      return Value.DisplayMesh.GetBoundingBox(xform);
    }
    #endregion

    #region casting methods
    public override bool CastTo<Q>(out Q target)
    {
      // This function is called when Grasshopper needs to convert this 
      // instance of GsaElement3D into some other type Q.            


      if (typeof(Q).IsAssignableFrom(typeof(GsaElement3d)))
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
          target = (Q)(object)Value.DisplayMesh;
        return true;
      }

      if (typeof(Q).IsAssignableFrom(typeof(GH_Mesh)))
      {
        if (Value == null)
          target = default;
        else
        {
          target = (Q)(object)new GH_Mesh(Value.DisplayMesh);
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

          for (int i = 0; i < Value.ID.Count; i++)
          {
            GH_Integer ghint = new GH_Integer();
            if (GH_Convert.ToGHInteger(Value.ID, GH_Conversion.Both, ref ghint))
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
      if (typeof(GsaElement3d).IsAssignableFrom(source.GetType()))
      {
        Value = (GsaElement3d)source;
        return true;
      }

      ////Cast from GsaAPI Member
      //if (typeof(List<Element>).IsAssignableFrom(source.GetType()))
      //{
      //    Value.API_Elements = (List<Element>)source;
      //    return true;
      //}

      //if (typeof(Element).IsAssignableFrom(source.GetType()))
      //{
      //    Value.Elements[0] = (Element)source; //If someone should want to just test if they can convert a Mesh face
      //    return true;
      //}

      //Cast from Mesh
      //Mesh mesh = new Mesh();

      //if (GH_Convert.ToMesh(source, ref mesh, GH_Conversion.Both))
      //{
      //    GsaElement3d elem = new GsaElement3d(mesh);
      //    this.Value = elem;
      //    return true;
      //}

      return false;
    }
    #endregion

    #region transformation methods
    public override IGH_GeometricGoo Transform(Transform xform)
    {
      if (Value == null) { return null; }
      if (Value.NgonMesh == null) { return null; }

      GsaElement3d dup = Value.Duplicate();
      dup.ID = new List<int>(new int[dup.NgonMesh.Faces.Count()]);
      Mesh xMs = dup.NgonMesh.DuplicateMesh();
      xMs.Transform(xform);
      return new GsaElement3dGoo(dup.UpdateGeometry(xMs));
    }

    public override IGH_GeometricGoo Morph(SpaceMorph xmorph)
    {
      if (Value == null) { return null; }
      if (Value.NgonMesh == null) { return null; }

      GsaElement3d dup = Value.Duplicate();
      dup.ID = new List<int>(new int[dup.NgonMesh.Faces.Count()]);
      Mesh xMs = dup.NgonMesh.DuplicateMesh();
      xmorph.Morph(xMs);
      return new GsaElement3dGoo(dup.UpdateGeometry(xMs));
    }

    #endregion

    #region drawing methods
    public BoundingBox ClippingBox
    {
      get { return Boundingbox; }
    }
    public void DrawViewportMeshes(GH_PreviewMeshArgs args)
    {
      //Draw shape.
      if (args.Material.Diffuse == System.Drawing.Color.FromArgb(255, 150, 0, 0)) // this is a workaround to change colour between selected and not
      {
        args.Pipeline.DrawMeshShaded(Value.DisplayMesh, UI.Colour.Element3dFace);
      }
      else
        args.Pipeline.DrawMeshShaded(Value.DisplayMesh, UI.Colour.Element2dFaceSelected);
    }
    public void DrawViewportWires(GH_PreviewWireArgs args)
    {
      if (Value == null) { return; }
      if (Grasshopper.CentralSettings.PreviewMeshEdges == false) { return; }

      //Draw lines
      if (Value.NgonMesh != null)
      {
        if (args.Color == System.Drawing.Color.FromArgb(255, 150, 0, 0)) // this is a workaround to change colour between selected and not
        {
          args.Pipeline.DrawMeshWires(Value.DisplayMesh, UI.Colour.Element2dEdge, 1);
        }
        else
        {
          args.Pipeline.DrawMeshWires(Value.DisplayMesh, UI.Colour.Element2dEdgeSelected, 2);
        }
      }
    }
    #endregion
  }
}
