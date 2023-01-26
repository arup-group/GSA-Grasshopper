using Grasshopper.Documentation;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using OasysGH;
using OasysGH.Parameters;

namespace GsaGH.Parameters
{
  /// <summary>
  /// Goo wrapper class, makes sure <see cref="GsaBool6"/> can be used in Grasshopper.
  /// </summary>
  public class GsaBool6Goo : GH_OasysGoo<GsaBool6>
  {
    public static string Name => "Bool6";
    public static string NickName => "B6";
    public static string Description => "GSA Bool6 (to set releases and restraints)";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    public GsaBool6Goo(GsaBool6 item) : base(item) { }

    public override IGH_Goo Duplicate() => new GsaBool6Goo(this.Value);

    public override bool CastFrom(object source)
    {
      if (source == null)
        return false;

      if (base.CastFrom(source))
        return true;

      // Cast from Bool
      else if (GH_Convert.ToBoolean(source, out bool mybool, GH_Conversion.Both))
      {
        Value.X = mybool;
        Value.Y = mybool;
        Value.Z = mybool;
        Value.XX = mybool;
        Value.YY = mybool;
        Value.ZZ = mybool;
        return true;
      }

      // Cast from string
      else if (GH_Convert.ToString(source, out string mystring, GH_Conversion.Both))
      {
        mystring = mystring.Trim().ToLower();

        if (mystring == "free")
        {
          Value.X = false;
          Value.Y = false;
          Value.Z = false;
          Value.XX = false;
          Value.YY = false;
          Value.ZZ = false;
          return true;
        }
        else if (mystring == "pin" | mystring == "pinned")
        {
          Value.X = true;
          Value.Y = true;
          Value.Z = true;
          Value.XX = false;
          Value.YY = false;
          Value.ZZ = false;
          return true;
        }
        else if (mystring == "fix" | mystring == "fixed")
        {
          Value.X = true;
          Value.Y = true;
          Value.Z = true;
          Value.XX = true;
          Value.YY = true;
          Value.ZZ = true;
          return true;
        }
        else if (mystring == "release" | mystring == "released" | mystring == "hinge" | mystring == "hinged" | mystring == "charnier")
        {
          Value.X = false;
          Value.Y = false;
          Value.Z = false;
          Value.XX = false;
          Value.YY = true;
          Value.ZZ = true;
          return true;
        }
        else if (mystring.Length == 6)
        {
          if (mystring[0] == 'f')
            Value.X = false;
          else if (mystring[0] == 'r')
            Value.X = true;
          else
            return false;

          if (mystring[1] == 'f')
            Value.Y = false;
          else if (mystring[1] == 'r')
            Value.Y = true;
          else
            return false;

          if (mystring[2] == 'f')
            Value.Z = false;
          else if (mystring[2] == 'r')
            Value.Z = true;
          else
            return false;

          if (mystring[3] == 'f')
            Value.XX = false;
          else if (mystring[3] == 'r')
            Value.XX = true;
          else
            return false;

          if (mystring[4] == 'f')
            Value.YY = false;
          else if (mystring[4] == 'r')
            Value.YY = true;
          else
            return false;

          if (mystring[5] == 'f')
            Value.ZZ = false;
          else if (mystring[5] == 'r')
            Value.ZZ = true;
          else
            return false;
        }
        return false;
      }
      
      return false;
    }
  }
}
