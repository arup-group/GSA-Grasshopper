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
  ///   This class provides a parameter interface for the <see cref="GsaMember2dGoo" /> type.
  /// </summary>
  public class GsaMember2dParameter : GH_OasysPersistentGeometryParam<GsaMember2dGoo>,
    IGH_BakeAwareObject {
    public override Guid ComponentGuid => new Guid("fa512c2d-4767-49f1-a574-32bf66a66568");
    public override GH_Exposure Exposure => GH_Exposure.primary;
    public override string InstanceDescription
      => m_data.DataCount == 0 ? "Empty " + GsaMember2dGoo.Name + " parameter" :
        base.InstanceDescription;
    public override string TypeName => SourceCount == 0 ? GsaMember2dGoo.Name : base.TypeName;
    protected override Bitmap Icon => Resources.Mem2dParam;

    public GsaMember2dParameter() : base(new GH_InstanceDescription(GsaMember2dGoo.Name,
      GsaMember2dGoo.NickName, GsaMember2dGoo.Description + " parameter", CategoryName.Name(),
      SubCategoryName.Cat9())) { }

    protected override GsaMember2dGoo PreferredCast(object data) {
      var brep = new Brep();
      if (GH_Convert.ToBrep(data, ref brep, GH_Conversion.Both)) {
        return new GsaMember2dGoo(new GsaMember2d(brep, new List<Curve>(), new List<Point3d>()));
      }

      this.AddRuntimeError($"Data conversion failed from {data.GetTypeName()} to Member2d");
      return new GsaMember2dGoo(null);
    }

    public bool IsBakeCapable => !m_data.IsEmpty;
    public void BakeGeometry(RhinoDoc doc, ObjectAttributes att, List<Guid> obj_ids) {
      var gH_BakeUtility = new GH_BakeUtility(OnPingDocument());
      gH_BakeUtility.BakeObjects(m_data.Select(x => new GH_Brep(x.Value.Brep)), att, doc);
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

    protected override GH_GetterResult Prompt_Singular(ref GsaMember2dGoo value) {
      GH_Brep brep = GH_BrepGetter.GetBrep();
      if (brep == null) {
        return GH_GetterResult.cancel;
      }

      value = new GsaMember2dGoo(new GsaMember2d(brep.Value));
      return GH_GetterResult.success;
    }

    protected override GH_GetterResult Prompt_Plural(ref List<GsaMember2dGoo> values) {
      List<GH_Brep> breps = GH_BrepGetter.GetBreps();
      if (breps == null || breps.Count == 0) {
        return GH_GetterResult.cancel;
      }

      values = breps.Select(brep =>
        new GsaMember2dGoo(new GsaMember2d(brep.Value))).ToList();
      return GH_GetterResult.success;
    }
  }
}
