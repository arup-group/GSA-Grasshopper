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
  ///   This class provides a parameter interface for the <see cref="GsaElement2dGoo" /> type.
  /// </summary>
  public class GsaElement2dParameter : GH_OasysPersistentGeometryParam<GsaElement2dGoo>,
    IGH_BakeAwareObject {
    public override Guid ComponentGuid => new Guid("bfaa6912-77b0-40b1-aa78-54e2b28614d0");
    public override GH_Exposure Exposure => GH_Exposure.tertiary | GH_Exposure.obscure;
    public override string InstanceDescription
      => m_data.DataCount == 0 ? "Empty " + GsaElement2dGoo.Name + " parameter" :
        base.InstanceDescription;
    public bool IsBakeCapable => !m_data.IsEmpty;
    public override string TypeName => SourceCount == 0 ? GsaElement2dGoo.Name : base.TypeName;
    protected override Bitmap Icon => Resources.Element2dParam;

    public GsaElement2dParameter() : base(new GH_InstanceDescription(GsaElement2dGoo.Name,
      GsaElement2dGoo.NickName, GsaElement2dGoo.Description + " parameter", CategoryName.Name(),
      SubCategoryName.Cat9())) { }

    protected override GsaElement2dGoo PreferredCast(object data) {
      var mesh = new Mesh();
      if (GH_Convert.ToMesh(data, ref mesh, GH_Conversion.Both)) {
        return new GsaElement2dGoo(new GsaElement2d(mesh));
      }

      this.AddRuntimeError($"Data conversion failed from {data.GetTypeName()} to Element2d");
      return new GsaElement2dGoo(null);
    }

    public void BakeGeometry(RhinoDoc doc, ObjectAttributes att, List<Guid> obj_ids) {
      var gH_BakeUtility = new GH_BakeUtility(OnPingDocument());
      att ??= doc.CreateDefaultAttributes();
      att.ColorSource = ObjectColorSource.ColorFromObject;
      foreach (GsaElement2dGoo goo in m_data.AllData(true).Cast<GsaElement2dGoo>()) {
        if (goo.Value.IsLoadPanel) {
          gH_BakeUtility.BakeObject(new GH_Curve(goo.Value.Curve), att, doc);
        } else {
          gH_BakeUtility.BakeObject(new GH_Mesh(goo.Value.Mesh), att, doc);
          goo.Value.Section3dPreview?.BakeGeometry(ref gH_BakeUtility, doc, att);
        }
      }
      obj_ids.AddRange(gH_BakeUtility.BakedIds);
    }

    public void BakeGeometry(RhinoDoc doc, List<Guid> obj_ids) {
      BakeGeometry(doc, null, obj_ids);
    }
  }
}
