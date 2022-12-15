using System;
using Grasshopper.Kernel;
using GsaGH.Helpers.GH;
using OasysGH.Parameters;

namespace GsaGH.Parameters
{
  /// <summary>
  /// This class provides a parameter interface for the <see cref="GsaSectionGoo"/> type.
  /// </summary>
  public class GsaSectionParameter : GH_OasysPersistentParam<GsaSectionGoo>
  {
    public override string InstanceDescription => this.m_data.DataCount == 0 ? "Empty " + GsaSectionGoo.Name + " parameter" : base.InstanceDescription;
    public override string TypeName => this.SourceCount == 0 ? GsaSectionGoo.Name : base.TypeName;
    public override Guid ComponentGuid => new Guid("8500f335-fad7-46a0-b1be-bdad22ab1474");
    public override GH_Exposure Exposure => GH_Exposure.secondary;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.SectionParam;

    public GsaSectionParameter() : base(new GH_InstanceDescription(
      GsaSectionGoo.Name,
      GsaSectionGoo.NickName,
      GsaSectionGoo.Description + " parameter",
      CategoryName.Name(),
      SubCategoryName.Cat9()))
    { }

    protected override GsaSectionGoo PreferredCast(object data)
    {
      if (data.GetType() == typeof(GsaSection))
        return new GsaSectionGoo((GsaSection)data);

      if (GH_Convert.ToInt32(data, out int id, GH_Conversion.Both))
      {
        GsaSection section = new GsaSection(id);
        return new GsaSectionGoo(section);
      }
      if (GH_Convert.ToString(data, out string profile, GH_Conversion.Both))
      {
        if (GsaSection.ValidProfile(profile))
        {
          GsaSection section = new GsaSection(profile);
          return new GsaSectionGoo(section);
        }
      }
      return base.PreferredCast(data);
    }
  }
}
