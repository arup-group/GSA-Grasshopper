using System;
using System.Collections.Generic;
using System.Linq;
using GsaAPI;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using OasysUnits;
using OasysUnits.Units;
using OasysGH.Units;
using OasysGH;
using OasysGH.Parameters;

namespace GsaGH.Parameters
{
  /// <summary>
  /// Goo wrapper class, makes sure <see cref="GsaSectionModifier"/> can be used in Grasshopper.
  /// </summary>
  public class GsaSectionModifierGoo : GH_OasysGoo<GsaSectionModifier>
  {
    public static string Name => "Section Modifier";
    public static string NickName => "PBM";
    public static string Description => "GSA Section Modifier";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    public GsaSectionModifierGoo(GsaSectionModifier item) : base(item) { }

    public override IGH_Goo Duplicate() => new GsaSectionModifierGoo(this.Value);

    public override bool CastTo<Q>(ref Q target)
    {
      if (typeof(Q).IsAssignableFrom(typeof(GsaSectionModifier)))
      {
        if (Value == null)
          target = default;
        else
          target = (Q)(object)Value.Duplicate();
        return true;
      }

      else if (typeof(Q).IsAssignableFrom(typeof(SectionModifier)))
      {
        if (Value == null)
          target = default;
        else
          target = (Q)(object)Value;
        return true;
      }

      return base.CastTo(ref target);
    }

    public override bool CastFrom(object source)
    {
      if (source == null)
        return false;

      // Cast from other Section Modifier
      else if (typeof(GsaSectionModifier).IsAssignableFrom(source.GetType()))
      {
        Value = (GsaSectionModifier)source;
        return true;
      }

      return base.CastFrom(source);
    }
  }
}
