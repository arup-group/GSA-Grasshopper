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
  ///   This class provides a parameter interface for the <see cref="GsaElement3dGoo" /> type.
  /// </summary>
  public class GsaElement3dParameter : GH_OasysPersistentGeometryParam<GsaElement3dGoo>,
    IGH_BakeAwareObject {
    public override Guid ComponentGuid => new Guid("e7326f8e-c8e5-40d9-b8e4-6912ccf80b92");
    public override GH_Exposure Exposure => GH_Exposure.tertiary | GH_Exposure.obscure;
    public override string InstanceDescription
      => m_data.DataCount == 0 ? "Empty " + GsaElement3dGoo.Name + " parameter" :
        base.InstanceDescription;
    public bool IsBakeCapable => !m_data.IsEmpty;
    public override string TypeName => SourceCount == 0 ? GsaElement3dGoo.Name : base.TypeName;
    protected override Bitmap Icon => Resources.Element3dParam;

    public GsaElement3dParameter() : base(new GH_InstanceDescription(GsaElement3dGoo.Name,
      GsaElement3dGoo.NickName, GsaElement3dGoo.Description + " parameter", CategoryName.Name(),
      SubCategoryName.Cat9())) { }

    protected override GsaElement3dGoo PreferredCast(object data) {
      var mesh = new Mesh();
      string mes = string.Empty;
      if (GH_Convert.ToMesh(data, ref mesh, GH_Conversion.Both)) {
        if (mesh.IsClosed) {
          var ngons = mesh.GetNgonAndFacesEnumerable().ToList();
          if (ngons.Count != ngons.Select(f => f.FaceCount).Sum()) {
            return new GsaElement3dGoo(new GsaElement3D(mesh));
          }
        }
        mes = "Mesh must be a closed (solid) Ngon Mesh";
      }

      if (!string.IsNullOrEmpty(mes)) {
        mes = "." + Environment.NewLine + mes;
      }

      this.AddRuntimeError($"Data conversion failed from {data.GetTypeName()} to Element3d" + mes);
      return new GsaElement3dGoo(null);
    }

    public void BakeGeometry(RhinoDoc doc, ObjectAttributes att, List<Guid> obj_ids) {
      var gH_BakeUtility = new GH_BakeUtility(OnPingDocument());
      att ??= doc.CreateDefaultAttributes();
      att.ColorSource = ObjectColorSource.ColorFromObject;
      gH_BakeUtility.BakeObjects(m_data.Select(x => new GH_Mesh(x.Value.DisplayMesh)), att, doc);
      obj_ids.AddRange(gH_BakeUtility.BakedIds);
    }

    public void BakeGeometry(RhinoDoc doc, List<Guid> obj_ids) {
      BakeGeometry(doc, null, obj_ids);
    }
  }
}
