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
using System.Windows.Forms;

namespace GsaGH.Parameters {
  /// <summary>
  ///   This class provides a parameter interface for the <see cref="GsaElement1dGoo" /> type.
  /// </summary>
  public class GsaElement1dParameter : GH_OasysPersistentGeometryParam<GsaElement1dGoo>,
    IGH_BakeAwareObject {
    public override Guid ComponentGuid => new Guid("9c045214-cab6-47d9-a158-ae1f4f494b66");
    public override GH_Exposure Exposure => GH_Exposure.primary | GH_Exposure.obscure;
    public override string InstanceDescription
      => m_data.DataCount == 0 ? "Empty " + GsaElement1dGoo.Name + " parameter" :
        base.InstanceDescription;
    public bool IsBakeCapable => !m_data.IsEmpty;
    public override string TypeName => SourceCount == 0 ? GsaElement1dGoo.Name : base.TypeName;
    protected override Bitmap Icon => Resources.Elem1dParam;

    public GsaElement1dParameter() : base(new GH_InstanceDescription(GsaElement1dGoo.Name,
      GsaElement1dGoo.NickName, GsaElement1dGoo.Description + " parameter", CategoryName.Name(),
      SubCategoryName.Cat9())) { }

    protected override GsaElement1dGoo PreferredCast(object data) {
      var ln = new Line();
      if (GH_Convert.ToLine(data, ref ln, GH_Conversion.Both)) {
        return new GsaElement1dGoo(new GsaElement1d(new LineCurve(ln)));
      }

      this.AddRuntimeError($"Data conversion failed from {data.GetTypeName()} to Element1d");
      return new GsaElement1dGoo(null);
    }

    public void BakeGeometry(RhinoDoc doc, ObjectAttributes att, List<Guid> obj_ids) {
      var gH_BakeUtility = new GH_BakeUtility(OnPingDocument());
      att ??= doc.CreateDefaultAttributes();
      att.ColorSource = ObjectColorSource.ColorFromObject;
      foreach (GsaElement1dGoo goo in m_data.AllData(true).Cast<GsaElement1dGoo>()) {
        ObjectAttributes objAtt = att.Duplicate();
        objAtt.ObjectColor = goo.Value.Colour;
        gH_BakeUtility.BakeObject(new GH_Line(goo.Value.Line.Line), objAtt, doc);
        if (Value.Section3dPreview != null) {
          ObjectAttributes meshAtt = att.Duplicate();
          gH_BakeUtility.BakeObject(new GH_Line(goo.Value.Section3dPreview.Mesh), meshAtt, doc);
          foreach (Line ln in goo.Value.Section3dPreview.Outlines) {
            ObjectAttributes lnAtt = att.Duplicate();
            gH_BakeUtility.BakeObject(new GH_Line(ln), lnAtt, doc);
          }
        }
      }
      obj_ids.AddRange(gH_BakeUtility.BakedIds);
    }

    public void BakeGeometry(RhinoDoc doc, List<Guid> obj_ids) {
      BakeGeometry(doc, null, obj_ids);
    }
  }
}
