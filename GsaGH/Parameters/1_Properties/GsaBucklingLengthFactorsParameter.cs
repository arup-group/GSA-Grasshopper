﻿using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using OasysGH.Parameters;

namespace GsaGH.Parameters
{
  /// <summary>
  /// This class provides a parameter interface for the <see cref="GsaBucklingLengthFactorsGoo"/> type.
  /// </summary>
  public class GsaBucklingLengthFactorsParameter : GH_OasysPersistentParam<GsaBucklingLengthFactorsGoo>
  {
    public override string InstanceDescription => this.m_data.DataCount == 0 ? "Empty " + GsaBucklingLengthFactorsGoo.Name + " parameter" : base.InstanceDescription;
    public override string TypeName => this.SourceCount == 0 ? GsaBucklingLengthFactorsGoo.Name : base.TypeName;
    public override Guid ComponentGuid => new Guid("e2349b4f-1ebb-4661-99d9-07c6a3ef22b9");
    public override GH_Exposure Exposure => GH_Exposure.secondary | GH_Exposure.obscure;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.Bool6Param;

    public GsaBucklingLengthFactorsParameter() : base(new GH_InstanceDescription(
      GsaBucklingLengthFactorsGoo.Name,
      GsaBucklingLengthFactorsGoo.NickName,
      GsaBucklingLengthFactorsGoo.Description + " parameter",
      GsaGH.Components.Ribbon.CategoryName.Name(),
      GsaGH.Components.Ribbon.SubCategoryName.Cat9()))
    { }
  }
}
