using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using OasysGH;
using Rhino.Geometry;
using System;

namespace OasysGH.Parameters
{
  public abstract class GH_OasysGeometricGoo<T> : GH_GeometricGoo<T>, IGH_PreviewData
  {
    abstract public OasysPluginInfo PluginInfo { get; }

    public override string TypeName => typeof(T).Name.TrimStart('I').TrimStart('G', 's', 'a').TrimStart('A', 'd', 'S', 'e', 'c');
    public override string TypeDescription => PluginInfo.ProductName + " " + this.TypeName + " Parameter";
    public override bool IsValid => (this.Value == null) ? false : true;
    public override string IsValidWhyNot
    {
      get
      {
        if (IsValid)
          return string.Empty;
        else
          return IsValid.ToString();
      }
    }

    public GH_OasysGeometricGoo(T item)
    {
      if (item == null)
        this.Value = item;
      else
        this.Value = (T)item.Duplicate();
    }

    public override string ToString()
    {
      if (this.Value == null)
        return "Null";
      else
        return this.PluginInfo.ProductName + " " + this.TypeName + " {" + this.Value.ToString() + "}";
    }

    #region casting methods
    public override bool CastTo<Q>(ref Q target)
    {
      // This function is called when Grasshopper needs to convert this 
      // instance of our custom class into some other type Q.            

      if (typeof(Q).IsAssignableFrom(typeof(T)))
      {
        if (Value == null)
          target = default;
        else
          target = (Q)(object)Value;
        return true;
      }

      target = default;
      return false;
    }

    public override bool CastFrom(object source)
    {
      // This function is called when Grasshopper needs to convert other data 
      // into our custom class.

      if (source == null)
        return false;

      //Cast from this type
      if (typeof(T).IsAssignableFrom(source.GetType()))
      {
        Value = (T)source;
        return true;
      }

      return false;
    }
    #endregion

    #region geometry
    public override IGH_GeometricGoo DuplicateGeometry()
    {
      return this.Duplicate();
    }
    public abstract new IGH_GeometricGoo Duplicate();

    public abstract GeometryBase GetGeometry();

    public override BoundingBox Boundingbox
    {
      get
      {
        if (!m_BoundingBox.IsValid)
        {
          m_BoundingBox = this.GetBoundingBox(Rhino.Geometry.Transform.ZeroTransformation);
        }
        return m_BoundingBox;
      }
    }
    private BoundingBox m_BoundingBox;
    public override BoundingBox GetBoundingBox(Transform xform)
    {
      GeometryBase geom = this.GetGeometry();
      if (geom == null)
        return BoundingBox.Empty;
      return geom.GetBoundingBox(Rhino.Geometry.Transform.ZeroTransformation);
    }
    #endregion

    #region transformation methods
    public abstract override IGH_GeometricGoo Morph(SpaceMorph xmorph);

    public abstract override IGH_GeometricGoo Transform(Transform xform);
    #endregion

    #region drawing methods
    public virtual BoundingBox ClippingBox
    {
      get { return Boundingbox; }
    }
    public abstract void DrawViewportMeshes(GH_PreviewMeshArgs args);

    public abstract void DrawViewportWires(GH_PreviewWireArgs args);
    #endregion
  }
}
