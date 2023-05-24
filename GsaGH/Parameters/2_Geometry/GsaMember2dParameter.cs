using Grasshopper.Kernel;
using GsaGH.Helpers.GH;
using GsaGH.Properties;
using OasysGH.Parameters;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace GsaGH.Parameters {
  /// <summary>
  ///   This class provides a parameter interface for the <see cref="GsaMember2dGoo" /> type.
  /// </summary>
  public class GsaMember2dParameter : GH_OasysPersistentGeometryParam<GsaMember2dGoo> {
    public override Guid ComponentGuid => new Guid("fa512c2d-4767-49f1-a574-32bf66a66568");
    public override GH_Exposure Exposure => GH_Exposure.primary;
    public override string InstanceDescription
      => m_data.DataCount == 0 ? "Empty " + GsaMember2dGoo.Name + " parameter" :
        base.InstanceDescription;
    public override string TypeName => SourceCount == 0 ? GsaMember2dGoo.Name : base.TypeName;
    protected override Bitmap Icon => Resources.Mem2dParam;

    public GsaMember2dParameter() : base(new GH_InstanceDescription(GsaMember2dGoo.Name,
      GsaMember2dGoo.NickName, GsaMember2dGoo.Description + " parameter", CategoryName.Name(),
      SubCategoryName.Cat9())) { }

    protected override GsaMember2dGoo PreferredCast(object data) {
      if (data.GetType() == typeof(GsaMember2d)) {
        return new GsaMember2dGoo((GsaMember2d)data);
      }

      var brep = new Brep();
      if (GH_Convert.ToBrep(data, ref brep, GH_Conversion.Both)) {
        return new GsaMember2dGoo(new GsaMember2d(brep, new List<Curve>(), new List<Point3d>()));
      }

      this.AddRuntimeError($"Data conversion failed from {data.GetTypeName()} to Member2d");
      return new GsaMember2dGoo(null);
    }
  }
}
