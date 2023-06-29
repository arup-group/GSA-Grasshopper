using Grasshopper.Kernel;
using GsaGH.Helpers.GH;
using GsaGH.Properties;
using OasysGH.Parameters;
using Rhino.DocObjects;
using Rhino;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Grasshopper.Getters;
using Grasshopper.Kernel.Types;
using System.Linq;

namespace GsaGH.Parameters {
  /// <summary>
  ///   This class provides a parameter interface for the <see cref="GsaNodeGoo" /> type.
  /// </summary>
  public class GsaNodeParameter : GH_OasysPersistentGeometryParam<GsaNodeGoo>, 
    IGH_BakeAwareObject {
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
      var pt = new Point3d();
      if (!GH_Convert.ToPoint3d(data, ref pt, GH_Conversion.Both)) {
        this.AddRuntimeError($"Data conversion failed from {data.GetTypeName()} to Node");
        return new GsaNodeGoo(null);
      }

      var node = new GsaNode(pt);
      return new GsaNodeGoo(node);
    }

    public bool IsBakeCapable => !m_data.IsEmpty;
    public void BakeGeometry(RhinoDoc doc, ObjectAttributes att, List<Guid> obj_ids) {
      var gH_BakeUtility = new GH_BakeUtility(OnPingDocument());
      gH_BakeUtility.BakeObjects(m_data.Select(x => new GH_Point(x.Value.Point)), att, doc);
      obj_ids.AddRange(gH_BakeUtility.BakedIds);
    }

    public void BakeGeometry(RhinoDoc doc, List<Guid> obj_ids) {
      BakeGeometry(doc, null, obj_ids);
    }


    protected override ToolStripMenuItem Menu_CustomMultiValueItem() {
      return null;
    }

    protected override ToolStripMenuItem Menu_CustomSingleValueItem() {
      return null;
    }

    protected override GH_GetterResult Prompt_Singular(ref GsaNodeGoo value) {
      var getter = new GH_PointGetter();
      getter.RecreateSetup(GetFirst());
      getter.AcceptPreselected = true;
      GH_Point pt = getter.GetPoint();
      if (pt == null) {
        return GH_GetterResult.cancel;
      }

      value = new GsaNodeGoo(new GsaNode(pt.Value));
      return GH_GetterResult.success;
    }

    protected override GH_GetterResult Prompt_Plural(ref List<GsaNodeGoo> values) {
      var getter = new GH_PointGetter();
      getter.RecreateSetup(GetFirst());
      getter.AcceptPreselected = true;
      List<GH_Point> pts = getter.GetPoints();
      if (pts == null) {
        return GH_GetterResult.cancel;
      }

      values = pts.Select(p => new GsaNodeGoo(new GsaNode(p.Value))).ToList();
      return GH_GetterResult.success;
    }

    private GH_Point GetFirst() {
      GH_Point gH_Point = null;
      GsaNodeGoo node = m_data.get_FirstItem(filter_nulls: true);
      if (node != null) {
        gH_Point = new GH_Point(node.Value.Point);
      } else {
        gH_Point = new GH_Point(default(Point3d));
      }

      return gH_Point;
    }
  }
}
