using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using GsaGH.Helpers.GH;
using GsaGH.Properties;

using OasysGH.Parameters;

using Rhino;
using Rhino.DocObjects;
using Rhino.Geometry;

namespace GsaGH.Parameters {
  /// <summary>
  ///   This class provides a parameter interface for the <see cref="GsaNodeGoo" /> type.
  /// </summary>
  public class GsaNodeParameter : GH_OasysPersistentGeometryParam<GsaNodeGoo>,
    IGH_BakeAwareObject {
    public override Guid ComponentGuid => new Guid("8ebdc693-e882-494d-8177-b0bd9c3d84a3");
    public override GH_Exposure Exposure => GH_Exposure.tertiary;
    public override string InstanceDescription
      => m_data.DataCount == 0 ? "Empty " + GsaNodeGoo.Name + " parameter" :
        base.InstanceDescription;
    public bool IsBakeCapable => !m_data.IsEmpty;
    public override string TypeName => SourceCount == 0 ? GsaNodeGoo.Name : base.TypeName;
    protected override Bitmap Icon => Resources.NodeParam;

    public GsaNodeParameter() : base(new GH_InstanceDescription(GsaNodeGoo.Name,
      GsaNodeGoo.NickName, GsaNodeGoo.Description + " parameter", CategoryName.Name(),
      SubCategoryName.Cat9())) { }

    public override void DrawViewportMeshes(IGH_PreviewArgs args) { }

    protected override GsaNodeGoo PreferredCast(object data) {
      var pt = new Point3d();
      if (!GH_Convert.ToPoint3d(data, ref pt, GH_Conversion.Both)) {
        this.AddRuntimeError($"Data conversion failed from {data.GetTypeName()} to Node");
        return new GsaNodeGoo(null);
      }

      var node = new GsaNode(pt);
      return new GsaNodeGoo(node);
    }

    public void BakeGeometry(RhinoDoc doc, ObjectAttributes att, List<Guid> obj_ids) {
      var gH_BakeUtility = new GH_BakeUtility(OnPingDocument());
      att ??= doc.CreateDefaultAttributes();
      att.ColorSource = ObjectColorSource.ColorFromObject;
      foreach (GsaNodeGoo goo in m_data.AllData(true).Cast<GsaNodeGoo>()) {
        ObjectAttributes objAtt = att.Duplicate();
        objAtt.ObjectColor = (Color)goo.Value.ApiNode.Colour;
        gH_BakeUtility.BakeObject(new GH_Point(goo.Value.Point), objAtt, doc);
        goo.Value.SupportPreview?.BakeGeometry(ref gH_BakeUtility, ref obj_ids, doc, att);
      }
      obj_ids.AddRange(gH_BakeUtility.BakedIds);
    }

    public void BakeGeometry(RhinoDoc doc, List<Guid> obj_ids) {
      BakeGeometry(doc, null, obj_ids);
    }
  }
}
