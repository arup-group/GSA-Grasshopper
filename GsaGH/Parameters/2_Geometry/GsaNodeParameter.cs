using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Rhino.Geometry;
using OasysGH.Parameters;
using GsaGH.Helpers.GH;

namespace GsaGH.Parameters
{
    /// <summary>
    /// This class provides a parameter interface for the <see cref="GsaNodeGoo"/> type.
    /// </summary>
    public class GsaNodeParameter : GH_OasysPersistentGeometryParam<GsaNodeGoo>
  {
    public override string InstanceDescription => this.m_data.DataCount == 0 ? "Empty " + GsaNodeGoo.Name + " parameter" : base.InstanceDescription;
    public override string TypeName => this.SourceCount == 0 ? GsaNodeGoo.Name : base.TypeName;
    public override Guid ComponentGuid => new Guid("8ebdc693-e882-494d-8177-b0bd9c3d84a3");
    public override GH_Exposure Exposure => GH_Exposure.primary;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.NodeParam;

    public GsaNodeParameter() : base(new GH_InstanceDescription(
      GsaNodeGoo.Name,
      GsaNodeGoo.NickName,
      GsaNodeGoo.Description + " parameter",
      CategoryName.Name(),
      SubCategoryName.Cat9()))
    { }

    protected override GsaNodeGoo PreferredCast(object data)
    {
      if (data.GetType() == typeof(GsaNode))
        return new GsaNodeGoo((GsaNode)data);

      Point3d pt = new Point3d();
      if (GH_Convert.ToPoint3d(data, ref pt, GH_Conversion.Both))
      {
        GsaNode node = new GsaNode(pt);
        return new GsaNodeGoo(node);
      }

      return base.PreferredCast(data);
    }

    public override void DrawViewportMeshes(IGH_PreviewArgs args)
    {
      //Meshes aren't drawn for points.
    }
  }
}
