using System;
using System.Drawing;
using Grasshopper.Kernel;
using GsaGH.Helpers.GH;
using GsaGH.Properties;
using OasysGH.Parameters;
using OasysUnits;
using Rhino.Geometry;

namespace GsaGH.Parameters {
  /// <summary>
  ///   This class provides a parameter interface for the <see cref="GsaMember3dGoo" /> type.
  /// </summary>
  public class GsaMember3dParameter : GH_OasysPersistentGeometryParam<GsaMember3dGoo> {
    public override Guid ComponentGuid => new Guid("7608a5a0-7762-4214-8c30-fb395365056e");
    public override GH_Exposure Exposure => GH_Exposure.primary | GH_Exposure.obscure;
    public override string InstanceDescription
      => m_data.DataCount == 0 ? "Empty " + GsaMember3dGoo.Name + " parameter" :
        base.InstanceDescription;
    public override string TypeName => SourceCount == 0 ? GsaMember3dGoo.Name : base.TypeName;
    protected override Bitmap Icon => Resources.Mem3dParam;

    public GsaMember3dParameter() : base(new GH_InstanceDescription(GsaMember3dGoo.Name,
      GsaMember3dGoo.NickName, GsaMember3dGoo.Description + " parameter", CategoryName.Name(),
      SubCategoryName.Cat9())) { }

    protected override GsaMember3dGoo PreferredCast(object data) {
      if (data.GetType() == typeof(GsaMember3d)) {
        return new GsaMember3dGoo((GsaMember3d)data);
      }

      var brep = new Brep();
      if (GH_Convert.ToBrep(data, ref brep, GH_Conversion.Both)) {
        return new GsaMember3dGoo(new GsaMember3d(brep));
      }

      var mesh = new Mesh();
      if (GH_Convert.ToMesh(data, ref mesh, GH_Conversion.Both)) {
        return new GsaMember3dGoo(new GsaMember3d(mesh));
      }

      AddRuntimeMessage(GH_RuntimeMessageLevel.Error,
        $"Data conversion failed from {data.GetTypeName()} to Member3d");
      return new GsaMember3dGoo(null);
    }
  }
}
