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
  public class GsaBool6Parameter : GH_OasysPersistentParam<GsaBool6Goo> {
    public override Guid ComponentGuid => new Guid("9bf01532-2035-4105-9c56-5e88b87f5220");
    public override GH_Exposure Exposure => GH_Exposure.secondary | GH_Exposure.obscure;
    public override string InstanceDescription
      => m_data.DataCount == 0 ? "Empty " + GsaBool6Goo.Name + " parameter" :
        GsaBool6Goo.Description;
    public override string TypeName => SourceCount == 0 ? GsaBool6Goo.Name : base.TypeName;
    protected override Bitmap Icon => Resources.Bool6Param;

    public GsaBool6Parameter() : base(new GH_InstanceDescription(GsaBool6Goo.Name,
      GsaBool6Goo.NickName, GsaBool6Goo.Description + " parameter", CategoryName.Name(),
      SubCategoryName.Cat9())) { }

    protected override GsaBool6Goo PreferredCast(object data) {
      if (data is GsaRestrainedGoo) {
        var outData = data as GsaRestrainedGoo;
        return new GsaBool6Goo(outData.Value);
      } else if (data is GsaReleaseGoo) {
        var outData = data as GsaReleaseGoo;
        return new GsaBool6Goo(outData.Value);
      } else {
        var bool6 = new GsaBool6();
        if (GH_Convert.ToBoolean(data, out bool mybool, GH_Conversion.Both)) {
          bool6.X = mybool;
          bool6.Y = mybool;
          bool6.Z = mybool;
          bool6.Xx = mybool;
          bool6.Yy = mybool;
          bool6.Zz = mybool;
          return new GsaBool6Goo(bool6);
        }
        if (GH_Convert.ToString(data, out string mystring, GH_Conversion.Both)
          && StringExtension.ParseBool6(mystring, ref bool6)) {
          new GsaBool6Goo(bool6);
        }
      }
      throw new InvalidCastException($"Data conversion failed from {data.GetTypeName()} to Bool6");
    }
  }
}
