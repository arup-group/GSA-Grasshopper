using System;
using System.Drawing;
using Grasshopper.Kernel;
using GsaGH.Helpers.GH;
using GsaGH.Properties;
using OasysGH.Parameters;
using Rhino.Geometry;

namespace GsaGH.Parameters {
  /// <summary>
  ///   This class provides a parameter interface for the <see cref="GsaElement1dGoo" /> type.
  /// </summary>
  public class GsaElement1dParameter : GH_OasysPersistentGeometryParam<GsaElement1dGoo> {
    public override Guid ComponentGuid => new Guid("9c045214-cab6-47d9-a158-ae1f4f494b66");
    public override GH_Exposure Exposure => GH_Exposure.primary | GH_Exposure.obscure;
    public override string InstanceDescription
      => m_data.DataCount == 0 ? $"Empty {GsaElement1dGoo.Name} parameter" :
        base.InstanceDescription;
    public override string TypeName => SourceCount == 0 ? GsaElement1dGoo.Name : base.TypeName;
    protected override Bitmap Icon => Resources.Elem1dParam;

    public GsaElement1dParameter() : base(new GH_InstanceDescription(GsaElement1dGoo.Name,
      GsaElement1dGoo.NickName, $"{GsaElement1dGoo.Description} parameter", CategoryName.Name(),
      SubCategoryName.Cat9())) { }

    protected override GsaElement1dGoo PreferredCast(object data) {
      var ln = new Line();
      if (GH_Convert.ToLine(data, ref ln, GH_Conversion.Both)) {
        return new GsaElement1dGoo(new GsaElement1d(new LineCurve(ln)));
      }

      this.AddRuntimeError($"Data conversion failed from {data.GetTypeName()} to Element1d");
      return new GsaElement1dGoo(null);
    }
  }
}
