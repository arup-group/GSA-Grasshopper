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
    public override Guid ComponentGuid => new Guid("8ebdc693-e882-494d-8177-b0bd9c3d84a3");
    public override GH_Exposure Exposure => GH_Exposure.primary;
    public override string InstanceDescription
      => m_data.DataCount == 0 ? "Empty " + GsaNodeGoo.Name + " parameter" :
        base.InstanceDescription;
    public override string TypeName => SourceCount == 0 ? GsaNodeGoo.Name : base.TypeName;
    protected override Bitmap Icon => Resources.NodeParam;

    public GsaNodeParameter() : base(new GH_InstanceDescription(GsaNodeGoo.Name,
      GsaNodeGoo.NickName, GsaNodeGoo.Description + " parameter", CategoryName.Name(),
      SubCategoryName.Cat9())) { }

    public override void DrawViewportMeshes(IGH_PreviewArgs args) { }

    protected override GsaNodeGoo PreferredCast(object data) {
      if (data.GetType() == typeof(GsaNode)) {
        return new GsaNodeGoo((GsaNode)data);
      }

      var pt = new Point3d();
      if (!GH_Convert.ToPoint3d(data, ref pt, GH_Conversion.Both)) {
        AddRuntimeMessage(GH_RuntimeMessageLevel.Error,
          $"Data conversion failed from {data.GetTypeName()} to Node");
        return new GsaNodeGoo(null);
      }

      var node = new GsaNode(pt);
      return new GsaNodeGoo(node);
    }
  }
}
