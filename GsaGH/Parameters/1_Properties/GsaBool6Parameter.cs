﻿using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using OasysGH.Parameters;

namespace GsaGH.Parameters
{
  /// <summary>
  /// This class provides a parameter interface for the <see cref="GsaBool6Goo"/> type.
  /// </summary>
  public class GsaBool6Parameter : GH_OasysPersistentParam<GsaBool6Goo>
  {
    public override string InstanceDescription => this.m_data.DataCount == 0 ? "Empty " + GsaBool6Goo.Name + " parameter" : base.InstanceDescription;
    public override string TypeName => this.SourceCount == 0 ? GsaBool6Goo.Name : base.TypeName;
    public override Guid ComponentGuid => new Guid("9bf01532-2035-4105-9c56-5e88b87f5220");
    public override GH_Exposure Exposure => GH_Exposure.secondary | GH_Exposure.obscure;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.Bool6Param;

    public GsaBool6Parameter() : base(new GH_InstanceDescription(
      GsaBool6Goo.Name,
      GsaBool6Goo.NickName,
      GsaBool6Goo.Description + " parameter",
      GsaGH.Components.Ribbon.CategoryName.Name(),
      GsaGH.Components.Ribbon.SubCategoryName.Cat9()))
    { }
  }
}
