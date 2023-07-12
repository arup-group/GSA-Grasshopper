using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Helpers.GH;
using GsaGH.Properties;
using OasysGH.Parameters;
using Rhino.DocObjects;
using Rhino;
using Rhino.Geometry;

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
    public bool IsBakeCapable => !m_data.IsEmpty;
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

    public void BakeGeometry(RhinoDoc doc, ObjectAttributes att, List<Guid> obj_ids) {
      var objs = new List<object>();
      objs.AddRange(m_data.Select(x => new GH_Brep(x.Value.Brep)));
      objs.AddRange(m_data.Select(x => new GH_Mesh(x.Value.Section3dPreview?.Mesh)));
      objs.AddRange(m_data.Select(x => x.Value.Section3dPreview?.Outlines.Select(y => new GH_Line(y))));
      var gH_BakeUtility = new GH_BakeUtility(OnPingDocument());
      gH_BakeUtility.BakeObjects(objs, att, doc);
      obj_ids.AddRange(gH_BakeUtility.BakedIds);
    }

    public void BakeGeometry(RhinoDoc doc, List<Guid> obj_ids) {
      BakeGeometry(doc, null, obj_ids);
    }
  }
}
