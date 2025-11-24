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
using Rhino.Collections;
using Rhino.DocObjects;
using Rhino.Geometry;

namespace GsaGH.Parameters {
  /// <summary>
  ///   This class provides a parameter interface for the <see cref="GsaMember2dGoo" /> type.
  /// </summary>
  public class GsaMember2dParameter : GH_OasysPersistentGeometryParam<GsaMember2dGoo>,
    IGH_BakeAwareObject {
    public override Guid ComponentGuid => new Guid("fa512c2d-4767-49f1-a574-32bf66a66568");
    public override GH_Exposure Exposure => GH_Exposure.tertiary;
    public override string InstanceDescription
      => m_data.DataCount == 0 ? "Empty " + GsaMember2dGoo.Name + " parameter" :
        base.InstanceDescription;
    public bool IsBakeCapable => !m_data.IsEmpty;
    public override string TypeName => SourceCount == 0 ? GsaMember2dGoo.Name : base.TypeName;
    protected override Bitmap Icon => Resources.Member2dParam;

    public GsaMember2dParameter() : base(new GH_InstanceDescription(
      GsaMember2dGoo.Name, GsaMember2dGoo.NickName,
      GsaMember2dGoo.Description + " parameter", CategoryName.Name(),
      SubCategoryName.Cat9())) { }

    protected override GsaMember2dGoo PreferredCast(object data) {
      var brep = new Brep();
      if (GH_Convert.ToBrep(data, ref brep, GH_Conversion.Both)) {
        return new GsaMember2dGoo(new GsaMember2D(brep, new List<Curve>(), new Point3dList()));
      }

      this.AddRuntimeError($"Data conversion failed from {data.GetTypeName()} to Member2d");
      return new GsaMember2dGoo(null);
    }

    public void BakeGeometry(RhinoDoc doc, ObjectAttributes att, List<Guid> obj_ids) {
      var gH_BakeUtility = new GH_BakeUtility(OnPingDocument());
      att ??= doc.CreateDefaultAttributes();
      att.ColorSource = ObjectColorSource.ColorFromObject;
      foreach (GsaMember2dGoo goo in m_data.AllData(true).Cast<GsaMember2dGoo>()) {
        ObjectAttributes objAtt = att.Duplicate();
        objAtt.ObjectColor = (Color)goo.Value.ApiMember.Colour;
        gH_BakeUtility.BakeObject(new GH_Brep(goo.Value.Brep), objAtt, doc);
        goo.Value.Section3dPreview?.BakeGeometry(ref gH_BakeUtility, doc, att);
      }

      obj_ids.AddRange(gH_BakeUtility.BakedIds);
    }

    public void BakeGeometry(RhinoDoc doc, List<Guid> obj_ids) {
      BakeGeometry(doc, null, obj_ids);
    }
  }
}
