using System;
using Grasshopper.Kernel;
using OasysGH.Parameters;

namespace GsaGH.Parameters
{
  /// <summary>
  /// This class provides a parameter interface for the <see cref="GsaElement1dGoo"/> type.
  /// </summary>
  public class GsaElement1dParameter : GH_OasysPersistentGeometryParam<GsaElement1dGoo>
  {
    public override string InstanceDescription => this.m_data.DataCount == 0 ? "Empty " + GsaElement1dGoo.Name + " parameter" : base.InstanceDescription;
    public override string TypeName => this.SourceCount == 0 ? GsaElement1dGoo.Name : base.TypeName;
    public override Guid ComponentGuid => new Guid("9c045214-cab6-47d9-a158-ae1f4f494b66");
    public override GH_Exposure Exposure => GH_Exposure.primary | GH_Exposure.obscure;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.Elem1dParam;

    public GsaElement1dParameter() : base(new GH_InstanceDescription(
      GsaElement1dGoo.Name,
      GsaElement1dGoo.NickName,
      GsaElement1dGoo.Description + " parameter",
      GsaGH.Components.Ribbon.CategoryName.Name(),
      GsaGH.Components.Ribbon.SubCategoryName.Cat9()))
    { }

    protected override GsaElement1dGoo PreferredCast(object data)
    {
      if (data.GetType() == typeof(GsaElement1d))
        return new GsaElement1dGoo((GsaElement1d)data);

      return base.PreferredCast(data);
    }

    public override void DrawViewportMeshes(IGH_PreviewArgs args)
    {
      // Meshes aren't drawn for lines/curves.
    }
  }
}
