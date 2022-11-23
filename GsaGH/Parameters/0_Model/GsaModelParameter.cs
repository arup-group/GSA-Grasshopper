﻿using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using GsaGH.Helpers.GH;
using OasysGH.Parameters;

namespace GsaGH.Parameters
{
    /// <summary>
    /// This class provides a parameter interface for the <see cref="GsaModelGoo"/> type.
    /// </summary>
    public class GsaModelParameter : GH_OasysPersistentParam<GsaModelGoo>
  {
    public override string InstanceDescription => this.m_data.DataCount == 0 ? "Empty " + GsaModelGoo.Name + " parameter" : base.InstanceDescription;
    public override string TypeName => this.SourceCount == 0 ? GsaModelGoo.Name : base.TypeName;
    public override Guid ComponentGuid => new Guid("43eb8fb6-d469-4c3b-ab3c-e8d6ad378d9a");
    public override GH_Exposure Exposure => GH_Exposure.primary;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.ModelParam;

    public GsaModelParameter() : base(new GH_InstanceDescription(
      GsaModelGoo.Name,
      GsaModelGoo.NickName,
      GsaModelGoo.Description + " parameter",
      CategoryName.Name(),
      SubCategoryName.Cat9()))
    { }

    protected override GsaModelGoo PreferredCast(object data)
    {
      if (data.GetType() == typeof(GsaModel))
        return new GsaModelGoo((GsaModel)data);
      return base.PreferredCast(data);
    }
  }
}
