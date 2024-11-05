using System;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;

using Grasshopper.Kernel;

using GsaGH.Helpers;
using GsaGH.Helpers.GH;
using GsaGH.Properties;

using OasysGH.Parameters;

namespace GsaGH.Parameters {
  /// <summary>
  ///   This class provides a parameter interface for the <see cref="GsaReleaseGoo" /> type.
  /// </summary>
  public class GsaReleaseParameter : GH_OasysPersistentParam<GsaReleaseGoo> {
    public override Guid ComponentGuid => new Guid("9bf01532-2035-4107-9c56-5e88b87f5220");
    public override GH_Exposure Exposure => GH_Exposure.secondary | GH_Exposure.obscure;
    public override string InstanceDescription => GsaReleaseGoo.Description;
    public override string TypeName => GsaReleaseGoo.Name;
    protected override Bitmap Icon => Resources.Bool6Param;

    public GsaReleaseParameter() : base(new GH_InstanceDescription(GsaReleaseGoo.Name,
      GsaReleaseGoo.NickName, GsaReleaseGoo.Description + " parameter", CategoryName.Name(),
      SubCategoryName.Cat9())) { }

    protected override GsaReleaseGoo PreferredCast(object data) {
      try {
        if (data is GsaBool6Goo) {
          var outData = data as GsaBool6Goo;
          return new GsaReleaseGoo(outData.Value);
        } else {
          return new GsaReleaseGoo(StringExtension.ParseBool6(data, true));
        }
      } catch (Exception e) {
        this.AddRuntimeError(e.Message);
      }
      return new GsaReleaseGoo(null);
    }
  }
}
