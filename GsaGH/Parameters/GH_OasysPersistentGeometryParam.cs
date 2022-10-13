using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using OasysGH;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OasysGH.Parameters
{
  public abstract class GH_OasysPersistentGeometryParam<T> : GH_OasysPersistentParam<T>, IGH_PreviewObject where T : class, IGH_GeometricGoo
  {
    protected GH_OasysPersistentGeometryParam(GH_InstanceDescription nTag) : base(nTag)
    {
    }

    public virtual BoundingBox ClippingBox
    {
      get
      {
        return Preview_ComputeClippingBox();
      }
    }
    public virtual void DrawViewportMeshes(IGH_PreviewArgs args)
    {
      Preview_DrawMeshes(args);
    }
    public virtual void DrawViewportWires(IGH_PreviewArgs args)
    {
      Preview_DrawWires(args);
    }

    public override bool Hidden
    {
      get { return m_hidden; }
      set { m_hidden = value; }
    }
    private bool m_hidden = false;

    public override bool IsPreviewCapable
    {
      get { return true; }
    }
  }
}
