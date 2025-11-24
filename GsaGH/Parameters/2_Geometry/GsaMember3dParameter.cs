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
  ///   This class provides a parameter interface for the <see cref="GsaMember3dGoo" /> type.
  /// </summary>
  public class GsaMember3dParameter : GH_OasysPersistentGeometryParam<GsaMember3dGoo>,
    IGH_BakeAwareObject {
    public override Guid ComponentGuid => new Guid("7608a5a0-7762-4214-8c30-fb395365056e");
    public override GH_Exposure Exposure => GH_Exposure.tertiary | GH_Exposure.obscure;
    public override string InstanceDescription
      => m_data.DataCount == 0 ? "Empty " + GsaMember3dGoo.Name + " parameter" :
        base.InstanceDescription;
    public bool IsBakeCapable => !m_data.IsEmpty;
    public override string TypeName => SourceCount == 0 ? GsaMember3dGoo.Name : base.TypeName;
    protected override Bitmap Icon => Resources.Member3dParam;

    public GsaMember3dParameter() : base(new GH_InstanceDescription(
      GsaMember3dGoo.Name, GsaMember3dGoo.NickName,
      GsaMember3dGoo.Description + " parameter", CategoryName.Name(),
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

    public void BakeGeometry(RhinoDoc doc, ObjectAttributes att, List<Guid> obj_ids) {
      var gH_BakeUtility = new GH_BakeUtility(OnPingDocument());
      att ??= doc.CreateDefaultAttributes();
      att.ColorSource = ObjectColorSource.ColorFromObject;
      foreach (GsaMember3dGoo goo in m_data.AllData(true).Cast<GsaMember3dGoo>()) {
        ObjectAttributes objAtt = att.Duplicate();
        objAtt.ObjectColor = (Color)goo.Value.ApiMember.Colour;
        gH_BakeUtility.BakeObject(new GH_Mesh(goo.Value.SolidMesh), objAtt, doc);
      }
      obj_ids.AddRange(gH_BakeUtility.BakedIds);
    }

    public void BakeGeometry(RhinoDoc doc, List<Guid> obj_ids) {
      BakeGeometry(doc, null, obj_ids);
    }
  }
}
