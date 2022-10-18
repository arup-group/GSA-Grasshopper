using System;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaAPI;
using OasysGH;
using OasysGH.Parameters;
using OasysGH.Units;
using OasysUnits;

namespace GsaGH.Parameters
{
  /// <summary>
  /// Goo wrapper class, makes sure <see cref="GsaSection"/> can be used in Grasshopper.
  /// </summary>
  public class GsaSectionGoo : GH_OasysGoo<GsaSection>
  {
    public static string Name => "Section";
    public static string NickName => "PB";
    public static string Description => "GSA Beam Property";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    public GsaSectionGoo(GsaSection item) : base(item) { }

    public override IGH_Goo Duplicate() => new GsaSectionGoo(this.Value);

    public override bool CastTo<Q>(ref Q target)
    {
      if (base.CastTo<Q>(ref target))
        return true;

      else if (typeof(Q).IsAssignableFrom(typeof(GH_Integer)))
      {
        if (Value == null)
          target = default;
        else
        {
          GH_Integer ghint = new GH_Integer();
          if (GH_Convert.ToGHInteger(Value.ID, GH_Conversion.Both, ref ghint))
            target = (Q)(object)ghint;
          else
            target = default;
        }
        return true;
      }

      target = default;
      return false;
    }

    public override bool CastFrom(object source)
    {
      if (source == null)
        return false;

      if (base.CastFrom(source))
        return true;

      // Cast from string
      else if (GH_Convert.ToString(source, out string name, GH_Conversion.Both))
      {
        Value = new GsaSection(name);
        return true;
      }

      // Cast from integer
      else if (GH_Convert.ToInt32(source, out int idd, GH_Conversion.Both))
      {
        Value.ID = idd;
      }

      return false;
    }
  }
}
