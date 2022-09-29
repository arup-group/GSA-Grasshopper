using Grasshopper.Kernel.Types;
using OasysGH;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GsaGH.Parameters
{
  public abstract class GH_OasysGeometricGoo<T> : GH_GeometricGoo<T>
  {
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

    public abstract OasysPluginInfo PluginInfo { get; }

    public override string TypeName => typeof(T).Name.TrimStart('I');

    public override string TypeDescription => PluginInfo.ProductName + " " + TypeName + " Parameter";

    public override bool IsValid
    {
      get
      {
        if (this.Value != null)
          return true;
        return false;
      }
    }

    public override string IsValidWhyNot
    {
      get
      {
        if (this.IsValid)
          return string.Empty;
        return this.IsValid.ToString();
      }
    }

    public GH_OasysGeometricGoo(T item)
    {
      if (item == null)
        this.Value = item;
      else
        this.Value = (T)item.Duplicate();
    }

    public abstract GeometryBase GetGeometry();

    public override string ToString()
    {
      if (this.Value == null)
        return "Null";
      return PluginInfo.ProductName + " " + this.TypeName + " {" + this.Value.ToString() + "}";
    }

    public override bool CastTo<Q>(ref Q target)
    {
      if (typeof(Q).IsAssignableFrom(typeof(T)))
      {
        if (this.Value == null)
          target = default;
        else
          target = (Q)(object)this.Value;
        return true;
      }
      target = default;
      return false;
    }

    public override bool CastFrom(object source)
    {
      if (source == null)
        return false;
      if (typeof(T).IsAssignableFrom(source.GetType()))
      {
        this.Value = (T)source;
        return true;
      }
      return false;
    }

    public override IGH_GeometricGoo DuplicateGeometry()
    {
      throw new NotImplementedException();
    }

    public override BoundingBox GetBoundingBox(Transform xform)
    {

      Transform transform = new Transform();

      GeometryBase geom = this.GetGeometry();
      if (geom == null)
        return BoundingBox.Empty;
      return geom.GetBoundingBox(Rhino.Geometry.Transform.ZeroTransformation);
    }

    public override IGH_GeometricGoo Morph(SpaceMorph xmorph)
    {
      throw new NotImplementedException();
    }

    public override IGH_GeometricGoo Transform(Transform xform)
    {
      throw new NotImplementedException();
    }
  }
}
