﻿using System;
using System.Drawing;
using Grasshopper.Kernel;
using GsaGH.Helpers.GH;
using GsaGH.Properties;
using OasysGH.Parameters;

namespace GsaGH.Parameters {
  /// <summary>
  ///   This class provides a parameter interface for the <see cref="GsaBool6Goo" /> type.
  /// </summary>
  public class GsaBool6Parameter : GH_OasysPersistentParam<GsaBool6Goo> {
    public override Guid ComponentGuid => new Guid("9bf01532-2035-4105-9c56-5e88b87f5220");
    public override GH_Exposure Exposure => GH_Exposure.secondary | GH_Exposure.obscure;
    public override string InstanceDescription
      => m_data.DataCount == 0 ? "Empty " + GsaBool6Goo.Name + " parameter" :
        base.InstanceDescription;
    public override string TypeName => SourceCount == 0 ? GsaBool6Goo.Name : base.TypeName;
    protected override Bitmap Icon => Resources.Bool6Param;

    public GsaBool6Parameter() : base(new GH_InstanceDescription(GsaBool6Goo.Name,
      GsaBool6Goo.NickName, GsaBool6Goo.Description + " parameter", CategoryName.Name(),
      SubCategoryName.Cat9())) { }

    protected override GsaBool6Goo PreferredCast(object data) {
      if (data.GetType() == typeof(GsaBool6)) {
        return new GsaBool6Goo((GsaBool6)data);
      }

      var goo = new GsaBool6Goo(new GsaBool6());
      if (goo.CastFrom(data)) {
        return goo;
      }

      AddRuntimeMessage(GH_RuntimeMessageLevel.Error,
      $"Data conversion failed from {data.GetTypeName()} to Bool6");
      return new GsaBool6Goo(null);
    }
  }
}
