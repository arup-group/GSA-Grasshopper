using Grasshopper.Getters;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Helpers.GH;
using GsaGH.Properties;
using OasysGH.Parameters;
using Rhino.DocObjects;
using Rhino;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace GsaGH.Parameters {
  /// <summary>
  ///   This class provides a parameter interface for the <see cref="GsaMember1dGoo" /> type.
  /// </summary>
  public class GsaMember1dParameter : GH_OasysPersistentGeometryParam<GsaMember1dGoo>,
    IGH_BakeAwareObject {
    public override Guid ComponentGuid => new Guid("0392a5a0-7762-4214-8c30-fb395365056e");
    public override GH_Exposure Exposure => GH_Exposure.primary;
    public override string InstanceDescription
      => m_data.DataCount == 0 ? "Empty " + GsaMember1dGoo.Name + " parameter" :
        base.InstanceDescription;
    public override string TypeName => SourceCount == 0 ? GsaMember1dGoo.Name : base.TypeName;
    protected override Bitmap Icon => Resources.Mem1dParam;

    public GsaMember1dParameter() : base(new GH_InstanceDescription(GsaMember1dGoo.Name,
      GsaMember1dGoo.NickName, GsaMember1dGoo.Description + " parameter", CategoryName.Name(),
      SubCategoryName.Cat9())) { }

    protected override GsaMember1dGoo PreferredCast(object data) {
      Curve crv = null;
      if (GH_Convert.ToCurve(data, ref crv, GH_Conversion.Both)) {
        return new GsaMember1dGoo(new GsaMember1d(crv));
      }

      this.AddRuntimeError($"Data conversion failed from {data.GetTypeName()} to Member1d");
      return new GsaMember1dGoo(null);
    }

    public bool IsBakeCapable => !m_data.IsEmpty;
    public void BakeGeometry(RhinoDoc doc, ObjectAttributes att, List<Guid> obj_ids) {
      var gH_BakeUtility = new GH_BakeUtility(OnPingDocument());
      gH_BakeUtility.BakeObjects(m_data.Select(x => new GH_Curve(x.Value.PolyCurve)), att, doc);
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

    protected override GH_GetterResult Prompt_Singular(ref GsaMember1dGoo value) {
      GH_Curve crv = GH_CurveGetter.GetCurve();
      if (crv == null || !crv.IsValid) {
        return GH_GetterResult.cancel;
      }

      value = new GsaMember1dGoo(new GsaMember1d(crv.Value));
      return GH_GetterResult.success;
    }

    protected override GH_GetterResult Prompt_Plural(ref List<GsaMember1dGoo> values) {
      List<GH_Curve> crvs = GH_CurveGetter.GetCurves();
      if (crvs == null || crvs.Count == 0) {
        return GH_GetterResult.cancel;
      }

      values = crvs.Select(crv =>
        new GsaMember1dGoo(new GsaMember1d(crv.Value))).ToList();
      return GH_GetterResult.success;
    }
  }
}
