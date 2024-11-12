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
  ///   This class provides a parameter interface for the <see cref="GsaBool6Goo" /> type.
  /// </summary>
  public class GsaReleaseParameter : GsaBool6Parameter {
    public override Guid ComponentGuid => new Guid("9bf01532-2035-4107-9c56-5e88b87f5220");
    public override GH_Exposure Exposure => GH_Exposure.secondary | GH_Exposure.obscure;
    public override string InstanceDescription => GsaReleaseParameterInfo.Description;
    public override string TypeName => GsaReleaseParameterInfo.Name;
    protected override Bitmap Icon => Resources.ReleaseParam;

    public GsaReleaseParameter() {
      Name = GsaReleaseParameterInfo.Name;
      NickName = GsaReleaseParameterInfo.NickName;
      Description = GsaReleaseParameterInfo.Description;
      Category = CategoryName.Name();
      SubCategory = SubCategoryName.Cat9();
    }

    protected override GsaBool6Goo PreferredCast(object data) {
      return new GsaBool6Goo(StringExtension.ParseRelease(data));
    }
  }

  public class GsaReleaseParameterInfo {
    public static string Description => "GSA releases containing six booleans representing the status";
    public static string Name => "Release";
    public static string NickName => "Rel";
  }
}
