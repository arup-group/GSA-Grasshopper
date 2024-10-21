using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using Grasshopper.Kernel;

using GsaGH.Helpers.GH;
using GsaGH.Properties;

using OasysGH.Parameters;

using Rhino;
using Rhino.DocObjects;

namespace GsaGH.Parameters {
  /// <summary>
  ///   This class provides a parameter interface for the <see cref="GsaAnnotationGoo" /> type.
  /// </summary>
  public class GsaAnnotationParameter : GH_OasysPersistentGeometryParam<GsaAnnotationGoo>,
    IGH_BakeAwareObject {
    public override Guid ComponentGuid => new Guid("68b51e31-9c78-4e5b-940c-55aef3bac2b7");
    public override GH_Exposure Exposure => GH_Exposure.hidden;
    public override string InstanceDescription
      => m_data.DataCount == 0 ? "Empty " + GsaAnnotationGoo.Name + " parameter" :
        base.InstanceDescription;
    public bool IsBakeCapable => !m_data.IsEmpty;
    public override string TypeName => SourceCount == 0 ? GsaAnnotationGoo.Name : base.TypeName;
    protected override Bitmap Icon => Resources.AnnotationParam;

    public GsaAnnotationParameter() : base(new GH_InstanceDescription(
      GsaAnnotationGoo.Name + " parameter", GsaAnnotationGoo.NickName,
      GsaAnnotationGoo.Description, CategoryName.Name(),
      SubCategoryName.Cat9())) { }

    public void BakeGeometry(RhinoDoc doc, ObjectAttributes att, List<Guid> obj_ids) {
      att ??= doc.CreateDefaultAttributes();
      ObjectAttributes objAtt = att.Duplicate();

      foreach (GsaAnnotationGoo data in m_data.AllData(true).Cast<GsaAnnotationGoo>()) {
        IGsaAnnotation annotation = data.Value;
        switch (annotation) {
          case GsaAnnotationDot dot:
            objAtt.ColorSource = ObjectColorSource.ColorFromObject;
            objAtt.ObjectColor = dot.Color;
            obj_ids.Add(doc.Objects.AddTextDot(dot.Text, dot.Location, objAtt));
            break;

          case GsaAnnotation3d text3d:
            objAtt.ColorSource = ObjectColorSource.ColorFromObject;
            objAtt.ObjectColor = text3d.Color;
            obj_ids.Add(doc.Objects.AddText(text3d.Value, objAtt));
            break;
        }
      }
    }

    public void BakeGeometry(RhinoDoc doc, List<Guid> obj_ids) {
      BakeGeometry(doc, null, obj_ids);
    }
  }
}
