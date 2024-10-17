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

namespace GsaGH.Parameters {
  /// <summary>
  ///   This class provides a parameter interface for the <see cref="GsaDiagramGoo" /> type.
  /// </summary>
  public class GsaDiagramParameter : GH_OasysPersistentGeometryParam<GsaDiagramGoo>,
    IGH_BakeAwareObject {
    public override Guid ComponentGuid => new Guid("0a22aee0-7275-436f-98ac-dcd072c20cbc");
    public override GH_Exposure Exposure => GH_Exposure.hidden;
    public override string InstanceDescription
      => m_data.DataCount == 0 ? "Empty " + GsaDiagramGoo.Name + " parameter" :
        base.InstanceDescription;
    public bool IsBakeCapable => !m_data.IsEmpty;
    public override string TypeName => SourceCount == 0 ? GsaDiagramGoo.Name : base.TypeName;
    protected override Bitmap Icon => Resources.DiagramParam;

    public GsaDiagramParameter() : base(new GH_InstanceDescription(
      GsaDiagramGoo.Name + " parameter", GsaDiagramGoo.NickName,
      GsaDiagramGoo.Description, CategoryName.Name(),
      SubCategoryName.Cat9())) { }

    public void BakeGeometry(RhinoDoc doc, ObjectAttributes att, List<Guid> obj_ids) {
      var gH_BakeUtility = new GH_BakeUtility(OnPingDocument());
      att ??= doc.CreateDefaultAttributes();

      foreach (GsaDiagramGoo data in m_data.AllData(true).Cast<GsaDiagramGoo>()) {
        IGsaDiagram diagram = data.Value;
        switch (diagram) {
          case GsaVectorDiagram diagramVector:
            ObjectAttributes vectorAtt = att.Duplicate();
            vectorAtt.ObjectColor = diagramVector.Color;
            vectorAtt.ColorSource = ObjectColorSource.ColorFromObject;
            vectorAtt.ObjectDecoration = ObjectDecoration.EndArrowhead;
            gH_BakeUtility.BakeObject(new GH_Line(diagramVector.DisplayLine), vectorAtt, doc);
            break;

          case GsaLineDiagram diagramLine:
            ObjectAttributes lineAtt = att.Duplicate();
            lineAtt.ObjectColor = diagramLine.Color;
            lineAtt.ColorSource = ObjectColorSource.ColorFromObject;
            gH_BakeUtility.BakeObject(new GH_Line(diagramLine.Value), lineAtt, doc);
            break;

          case GsaArrowheadDiagram arrowhead:
            ObjectAttributes arrowheadAtt = att.Duplicate();
            arrowheadAtt.ColorSource = ObjectColorSource.ColorFromObject;
            gH_BakeUtility.BakeObject(new GH_Mesh(arrowhead.Value), arrowheadAtt, doc);
            break;
        }
      }

      obj_ids.AddRange(gH_BakeUtility.BakedIds);
    }

    public void BakeGeometry(RhinoDoc doc, List<Guid> obj_ids) {
      BakeGeometry(doc, null, obj_ids);
    }
  }
}
