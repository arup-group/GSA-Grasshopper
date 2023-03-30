using System;
using System.Drawing;
using Grasshopper.Kernel;
using GsaGH.Helpers.GH;
using GsaGH.Properties;
using OasysGH.Parameters;
using Rhino.Geometry;

namespace GsaGH.Parameters {
  /// <summary>
  ///   This class provides a parameter interface for the <see cref="GsaNodeGoo" /> type.
  /// </summary>
  public class GsaNodeParameter : GH_OasysPersistentGeometryParam<GsaNodeGoo> {
    public GsaNodeParameter() : base(new GH_InstanceDescription(GsaNodeGoo.Name,
      GsaNodeGoo.NickName,
      GsaNodeGoo.Description + " parameter",
      CategoryName.Name(),
      SubCategoryName.Cat9())) { }

    public override string InstanceDescription
      => m_data.DataCount == 0
        ? "Empty " + GsaNodeGoo.Name + " parameter"
        : base.InstanceDescription;

    public override string TypeName
      => SourceCount == 0
        ? GsaNodeGoo.Name
        : base.TypeName;

    public override Guid ComponentGuid => new Guid("8ebdc693-e882-494d-8177-b0bd9c3d84a3");
    public override GH_Exposure Exposure => GH_Exposure.primary;
    protected override Bitmap Icon => Resources.NodeParam;

    protected override GsaNodeGoo PreferredCast(object data) {
      if (data.GetType() == typeof(GsaNode))
        return new GsaNodeGoo((GsaNode)data);

      var pt = new Point3d();
      if (!GH_Convert.ToPoint3d(data, ref pt, GH_Conversion.Both))
        return base.PreferredCast(data);
      var node = new GsaNode(pt);
      return new GsaNodeGoo(node);

    }

    public override void DrawViewportMeshes(IGH_PreviewArgs args) {
    }
  }
}
