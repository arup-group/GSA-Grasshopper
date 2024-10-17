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
  /// This class provides a parameter interface for the <see cref="GsaGridLineGoo" /> type.
  /// </summary>
  public class GsaGridLineParameter : GH_OasysPersistentGeometryParam<GsaGridLineGoo>, IGH_BakeAwareObject {
    public override Guid ComponentGuid => new Guid("e5e50621-12c9-4ac4-abb0-926d60414ea7");
    public override GH_Exposure Exposure => GH_Exposure.primary | GH_Exposure.obscure;
    public override string InstanceDescription
      => m_data.DataCount == 0 ? "Empty " + GsaGridLineGoo.Name + " parameter" :
        base.InstanceDescription;
    public bool IsBakeCapable => !m_data.IsEmpty;
    public override string TypeName => SourceCount == 0 ? GsaGridLineGoo.Name : base.TypeName;
    protected override Bitmap Icon => Resources.GridLineParam;

    public GsaGridLineParameter() : base(new GH_InstanceDescription(GsaGridLineGoo.Name,
      GsaGridLineGoo.NickName, GsaGridLineGoo.Description + " parameter", CategoryName.Name(),
      SubCategoryName.Cat9())) { }

    protected override GsaGridLineGoo PreferredCast(object data) {
      this.AddRuntimeError($"Data conversion failed from {data.GetTypeName()} to Grid Line");
      return new GsaGridLineGoo(null);
    }

    public void BakeGeometry(RhinoDoc doc, ObjectAttributes att, List<Guid> obj_ids) {
      var gH_BakeUtility = new GH_BakeUtility(OnPingDocument());
      att ??= doc.CreateDefaultAttributes();
      att.ColorSource = ObjectColorSource.ColorFromObject;
      foreach (GsaGridLineGoo goo in m_data.AllData(true).Cast<GsaGridLineGoo>()) {
        ObjectAttributes objAtt = att.Duplicate();
        objAtt.ObjectColor = Color.Black;
        objAtt.ColorSource = ObjectColorSource.ColorFromObject;
        objAtt.LinetypeSource = ObjectLinetypeSource.LinetypeFromObject;
        objAtt.LinetypeIndex = doc.Linetypes.Find("Center");
        gH_BakeUtility.BakeObject(new GH_Curve(goo.Value.Curve), objAtt, doc);
      }
      obj_ids.AddRange(gH_BakeUtility.BakedIds);
    }

    public void BakeGeometry(RhinoDoc doc, List<Guid> obj_ids) {
      BakeGeometry(doc, null, obj_ids);
    }
  }
}
