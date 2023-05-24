using Grasshopper.Kernel;
using GsaGH.Helpers.GH;
using GsaGH.Properties;
using OasysGH.Parameters;
using Rhino.Geometry;
using System;
using System.Drawing;

namespace GsaGH.Parameters {
  /// <summary>
  ///   This class provides a parameter interface for the <see cref="GsaMember1dGoo" /> type.
  /// </summary>
  public class GsaMember1dParameter : GH_OasysPersistentGeometryParam<GsaMember1dGoo> {
    public override Guid ComponentGuid => new Guid("0392a5a0-7762-4214-8c30-fb395365056e");
    public override GH_Exposure Exposure => GH_Exposure.primary;
    public override string InstanceDescription
      => m_data.DataCount == 0 ? "Empty " + GsaMember1dGoo.Name + " parameter" :
        base.InstanceDescription;
    public override string TypeName => SourceCount == 0 ? GsaMember1dGoo.Name : base.TypeName;
    protected override Bitmap Icon => Resources.Mem1dParam;

    public GsaMember1dParameter() : base(new GH_InstanceDescription(GsaMember1dGoo.Name,
      GsaMember1dGoo.NickName, GsaMember1dGoo.Description + " parameter", CategoryName.Name(),
      SubCategoryName.Cat9())) { }

    protected override GsaMember1dGoo PreferredCast(object data) {
      Curve crv = null;
      if (GH_Convert.ToCurve(data, ref crv, GH_Conversion.Both)) {
        return new GsaMember1dGoo(new GsaMember1d(crv));
      }

      this.AddRuntimeError($"Data conversion failed from {data.GetTypeName()} to Member1d");
      return new GsaMember1dGoo(null);
    }
  }
}
