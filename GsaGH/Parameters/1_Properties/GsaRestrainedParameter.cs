using System;
using System.Drawing;

using Grasshopper.Kernel;

using GsaGH.Helpers;
using GsaGH.Helpers.GH;
using GsaGH.Properties;

using OasysGH.Parameters;

namespace GsaGH.Parameters {
  /// <summary>
  ///   This class provides a parameter interface for the <see cref="GsaBool6Goo" /> type.
  /// </summary>
  public class GsaRestrainedParameter : GH_OasysPersistentParam<GsaRestraintGoo> {
    public override Guid ComponentGuid => new Guid("9bf01532-2035-4108-9c56-5e88b87f5220");
    public override GH_Exposure Exposure => GH_Exposure.secondary | GH_Exposure.obscure;
    public override string InstanceDescription => GsaRestraintGoo.Description;
    public override string TypeName => GsaRestraintGoo.Name;
    protected override Bitmap Icon => Resources.Bool6Param;

    public GsaRestrainedParameter() : base(new GH_InstanceDescription(GsaRestraintGoo.Name,
      GsaRestraintGoo.NickName, GsaRestraintGoo.Description + " parameter", CategoryName.Name(),
      SubCategoryName.Cat9())) { }

    protected override GsaRestraintGoo PreferredCast(object data) {
      try {
        if (data is GsaBool6Goo) {
          var outData = data as GsaBool6Goo;
          return new GsaRestraintGoo(outData.Value);
        } else {
          return new GsaRestraintGoo(StringExtension.ParseBool6(data));
        }
      } catch (Exception e) {
        this.AddRuntimeError(e.Message);
      }
      return new GsaRestraintGoo(null);
    }
  }
}
