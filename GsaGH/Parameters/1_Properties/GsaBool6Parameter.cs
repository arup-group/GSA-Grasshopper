using System;
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
        GsaBool6Goo.Description;
    public override string TypeName => SourceCount == 0 ? GsaBool6Goo.Name : base.TypeName;
    protected override Bitmap Icon => Resources.Bool6Param;

    public GsaBool6Parameter() : base(new GH_InstanceDescription(GsaBool6Goo.Name,
      GsaBool6Goo.NickName, GsaBool6Goo.Description + " parameter", CategoryName.Name(),
      SubCategoryName.Cat9())) { }

    protected override GsaBool6Goo PreferredCast(object data) {
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

      if (GH_Convert.ToString(data, out string mystring, GH_Conversion.Both)) {
        mystring = mystring.Trim().ToLower();

        if (mystring == "free") {
          bool6.X = false;
          bool6.Y = false;
          bool6.Z = false;
          bool6.Xx = false;
          bool6.Yy = false;
          bool6.Zz = false;
          return new GsaBool6Goo(bool6);
        } else if (mystring == "pin" || mystring == "pinned") {
          bool6.X = true;
          bool6.Y = true;
          bool6.Z = true;
          bool6.Xx = false;
          bool6.Yy = false;
          bool6.Zz = false;
          return new GsaBool6Goo(bool6);
        } else if (mystring == "fix" || mystring == "fixed") {
          bool6.X = true;
          bool6.Y = true;
          bool6.Z = true;
          bool6.Xx = true;
          bool6.Yy = true;
          bool6.Zz = true;
          return new GsaBool6Goo(bool6);
        } else if (mystring == "release" || mystring == "released" || mystring == "hinge"
          || mystring == "hinged" || mystring == "charnier") {
          bool6.X = false;
          bool6.Y = false;
          bool6.Z = false;
          bool6.Xx = false;
          bool6.Yy = true;
          bool6.Zz = true;
          return new GsaBool6Goo(bool6);
        } else if (mystring.Length == 6) {
          return new GsaBool6Goo(ConvertString(mystring));
        }
      }

      this.AddRuntimeError($"Data conversion failed from {data.GetTypeName()} to Bool6");
      return new GsaBool6Goo(null);
    }

    private GsaBool6 ConvertString(string txt) {
      int i = 0;
      return new GsaBool6() {
        X = ConvertChar(txt[i++]),
        Y = ConvertChar(txt[i++]),
        Z = ConvertChar(txt[i++]),
        Xx = ConvertChar(txt[i++]),
        Yy = ConvertChar(txt[i++]),
        Zz = ConvertChar(txt[i++]),
      };
    }
    private bool ConvertChar(char rel) {
      return rel switch {
        'r' => false,
        'f' => true,
        _ => throw new ArgumentException(
                    $"Unable to convert string to Bool6, character {rel} not recognised"),
      };
    }
  }
}
