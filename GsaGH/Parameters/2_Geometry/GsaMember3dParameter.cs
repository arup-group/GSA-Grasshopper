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
  ///   This class provides a parameter interface for the <see cref="GsaMember3dGoo" /> type.
  /// </summary>
  public class GsaMember3dParameter : GH_OasysPersistentGeometryParam<GsaMember3dGoo>,
    IGH_BakeAwareObject {
    public override Guid ComponentGuid => new Guid("7608a5a0-7762-4214-8c30-fb395365056e");
    public override GH_Exposure Exposure => GH_Exposure.primary | GH_Exposure.obscure;
    public override string InstanceDescription
      => m_data.DataCount == 0 ? "Empty " + GsaMember3dGoo.Name + " parameter" :
        base.InstanceDescription;
    public override string TypeName => SourceCount == 0 ? GsaMember3dGoo.Name : base.TypeName;
    protected override Bitmap Icon => Resources.Mem3dParam;

    public GsaMember3dParameter() : base(new GH_InstanceDescription(GsaMember3dGoo.Name,
      GsaMember3dGoo.NickName, GsaMember3dGoo.Description + " parameter", CategoryName.Name(),
      SubCategoryName.Cat9())) { }

    protected override GsaMember3dGoo PreferredCast(object data) {
      var brep = new Brep();
      if (GH_Convert.ToBrep(data, ref brep, GH_Conversion.Both)) {
        return new GsaMember3dGoo(new GsaMember3d(brep));
      }

      var mesh = new Mesh();
      if (GH_Convert.ToMesh(data, ref mesh, GH_Conversion.Both)) {
        return new GsaMember3dGoo(new GsaMember3d(mesh));
      }

      this.AddRuntimeError($"Data conversion failed from {data.GetTypeName()} to Member3d");
      return new GsaMember3dGoo(null);
    }

    public bool IsBakeCapable => !m_data.IsEmpty;
    public void BakeGeometry(RhinoDoc doc, ObjectAttributes att, List<Guid> obj_ids) {
      var gH_BakeUtility = new GH_BakeUtility(OnPingDocument());
      gH_BakeUtility.BakeObjects(m_data.Select(x => new GH_Mesh(x.Value.SolidMesh)), att, doc);
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

    protected override GH_GetterResult Prompt_Singular(ref GsaMember3dGoo value) {
      GH_Brep brep = GH_BrepGetter.GetBrep();
      if (brep == null) {
        return GH_GetterResult.cancel;
      }

      value = new GsaMember3dGoo(new GsaMember3d(brep.Value));
      return GH_GetterResult.success;
    }

    protected override GH_GetterResult Prompt_Plural(ref List<GsaMember3dGoo> values) {
      List<GH_Brep> breps = GH_BrepGetter.GetBreps();
      if (breps == null || breps.Count == 0) {
        return GH_GetterResult.cancel;
      }

      values = breps.Select(brep =>
        new GsaMember3dGoo(new GsaMember3d(brep.Value))).ToList();
      return GH_GetterResult.success;
    }
  }
}
