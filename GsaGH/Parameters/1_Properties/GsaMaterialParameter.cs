﻿using System;
using Grasshopper.Kernel;
using GsaGH.Helpers.GH;
using OasysGH.Parameters;

namespace GsaGH.Parameters
{
    /// <summary>
    /// This class provides a parameter interface for the <see cref="GsaMaterialGoo"/> type.
    /// </summary>
    public class GsaMaterialParameter : GH_OasysPersistentParam<GsaMaterialGoo>
  {
    public override string InstanceDescription => this.m_data.DataCount == 0 ? "Empty " + GsaMaterialGoo.Name + " parameter" : base.InstanceDescription;
    public override string TypeName => this.SourceCount == 0 ? GsaMaterialGoo.Name : base.TypeName;
    public override Guid ComponentGuid => new Guid("f13d079b-f7d1-4d8a-be7c-3b7e1e59c5ab");
    public override GH_Exposure Exposure => GH_Exposure.secondary | GH_Exposure.obscure;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.MaterialParam;

    public GsaMaterialParameter() : base(new GH_InstanceDescription(
      GsaMaterialGoo.Name,
      GsaMaterialGoo.NickName,
      GsaMaterialGoo.Description + " parameter",
      CategoryName.Name(),
      SubCategoryName.Cat9()))
    { }

    protected override GsaMaterialGoo PreferredCast(object data)
    {
      if (data.GetType() == typeof(GsaMaterial))
        return new GsaMaterialGoo((GsaMaterial)data);
      if (GH_Convert.ToInt32(data, out int id, GH_Conversion.Both))
      {
        GsaMaterial material = new GsaMaterial(id);
        return new GsaMaterialGoo(material);
      }
      return base.PreferredCast(data);
    }
  }
}
