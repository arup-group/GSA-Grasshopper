using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using OasysGH;
using OasysGH.Parameters;

namespace GsaGH.Parameters {

  /// <summary>
  /// Goo wrapper class, makes sure <see cref="GsaBool6"/> can be used in Grasshopper.
  /// </summary>
  public class GsaBool6Goo : GH_OasysGoo<GsaBool6> {

    #region Properties + Fields
    public static string Description => "GSA Bool6 (A 6-character string to describe the restraint condition (F = Fixed, R = Released) for each degree of freedom)";
    public static string Name => "Bool6";
    public static string NickName => "B6";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    #endregion Properties + Fields

    #region Public Constructors
    public GsaBool6Goo(GsaBool6 item) : base(item) {
    }

    #endregion Public Constructors

    #region Public Methods
    public override bool CastFrom(object source) {
      if (source == null)
        return false;

      if (base.CastFrom(source))
        return true;
      else if (GH_Convert.ToBoolean(source, out bool mybool, GH_Conversion.Both)) {
        Value.X = mybool;
        Value.Y = mybool;
        Value.Z = mybool;
        Value.Xx = mybool;
        Value.Yy = mybool;
        Value.Zz = mybool;
        return true;
      }
      else if (GH_Convert.ToString(source, out string mystring, GH_Conversion.Both)) {
        mystring = mystring.Trim().ToLower();

        if (mystring == "free") {
          Value.X = false;
          Value.Y = false;
          Value.Z = false;
          Value.Xx = false;
          Value.Yy = false;
          Value.Zz = false;
          return true;
        }
        else if (mystring == "pin" | mystring == "pinned") {
          Value.X = true;
          Value.Y = true;
          Value.Z = true;
          Value.Xx = false;
          Value.Yy = false;
          Value.Zz = false;
          return true;
        }
        else if (mystring == "fix" | mystring == "fixed") {
          Value.X = true;
          Value.Y = true;
          Value.Z = true;
          Value.Xx = true;
          Value.Yy = true;
          Value.Zz = true;
          return true;
        }
        else if (mystring == "release" | mystring == "released" | mystring == "hinge" | mystring == "hinged" | mystring == "charnier") {
          Value.X = false;
          Value.Y = false;
          Value.Z = false;
          Value.Xx = false;
          Value.Yy = true;
          Value.Zz = true;
          return true;
        }
        else if (mystring.Length == 6) {
          switch (mystring[0]) {
            case 'r':
              Value.X = false;
              break;

            case 'f':
              Value.X = true;
              break;

            default:
              return false;
          }

          switch (mystring[1]) {
            case 'r':
              Value.Y = false;
              break;

            case 'f':
              Value.Y = true;
              break;

            default:
              return false;
          }

          switch (mystring[2]) {
            case 'r':
              Value.Z = false;
              break;

            case 'f':
              Value.Z = true;
              break;

            default:
              return false;
          }

          switch (mystring[3]) {
            case 'r':
              Value.Xx = false;
              break;

            case 'f':
              Value.Xx = true;
              break;

            default:
              return false;
          }

          switch (mystring[4]) {
            case 'r':
              Value.Yy = false;
              break;

            case 'f':
              Value.Yy = true;
              break;

            default:
              return false;
          }

          switch (mystring[5]) {
            case 'r':
              Value.Zz = false;
              break;

            case 'f':
              Value.Zz = true;
              break;

            default:
              return false;
          }
          return true;
        }
        return false;
      }
      return false;
    }

    public override IGH_Goo Duplicate() => new GsaBool6Goo(Value);

    #endregion Public Methods
  }
}
