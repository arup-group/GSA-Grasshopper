﻿using Grasshopper.Kernel;
using GsaGH.Helpers.GH;
using GsaGH.Properties;
using OasysGH.Parameters;
using Rhino.DocObjects;
using Rhino;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Grasshopper.Getters;
using Grasshopper.Kernel.Types;
using System.Linq;

namespace GsaGH.Parameters {
  /// <summary>
  ///   This class provides a parameter interface for the <see cref="GsaNodeGoo" /> type.
  /// </summary>
  public class GsaNodeParameter : GH_OasysPersistentGeometryParam<GsaNodeGoo>, 
    IGH_BakeAwareObject {
    public override Guid ComponentGuid => new Guid("8ebdc693-e882-494d-8177-b0bd9c3d84a3");
    public override GH_Exposure Exposure => GH_Exposure.primary;
    public override string InstanceDescription
      => m_data.DataCount == 0 ? "Empty " + GsaNodeGoo.Name + " parameter" :
        base.InstanceDescription;
    public override string TypeName => SourceCount == 0 ? GsaNodeGoo.Name : base.TypeName;
    protected override Bitmap Icon => Resources.NodeParam;

    public GsaNodeParameter() : base(new GH_InstanceDescription(GsaNodeGoo.Name,
      GsaNodeGoo.NickName, GsaNodeGoo.Description + " parameter", CategoryName.Name(),
      SubCategoryName.Cat9())) { }

    public override void DrawViewportMeshes(IGH_PreviewArgs args) { }

    protected override GsaNodeGoo PreferredCast(object data) {
      var pt = new Point3d();
      if (!GH_Convert.ToPoint3d(data, ref pt, GH_Conversion.Both)) {
        this.AddRuntimeError($"Data conversion failed from {data.GetTypeName()} to Node");
        return new GsaNodeGoo(null);
      }

      var node = new GsaNode(pt);
      return new GsaNodeGoo(node);
    }

    public bool IsBakeCapable => !m_data.IsEmpty;
    public void BakeGeometry(RhinoDoc doc, ObjectAttributes att, List<Guid> obj_ids) {
      var gH_BakeUtility = new GH_BakeUtility(OnPingDocument());
      gH_BakeUtility.BakeObjects(m_data.Select(x => new GH_Point(x.Value.Point)), att, doc);
      obj_ids.AddRange(gH_BakeUtility.BakedIds);
    }

    public void BakeGeometry(RhinoDoc doc, List<Guid> obj_ids) {
      BakeGeometry(doc, null, obj_ids);
    }
  }
}
