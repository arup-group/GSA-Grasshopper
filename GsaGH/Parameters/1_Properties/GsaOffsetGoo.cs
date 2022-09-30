using System;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaAPI;
using OasysGH;
using OasysGH.Parameters;
using OasysGH.Units;
using OasysUnits;
using OasysUnits.Units;

namespace GsaGH.Parameters
{
  /// <summary>
  /// Goo wrapper class, makes sure <see cref="GsaOffset"/> can be used in Grasshopper.
  /// </summary>
  public class GsaOffsetGoo : GH_OasysGoo<GsaOffset>
  {
    public static string Name => "Offset";
    public static string NickName => "Off";
    public static string Description => "GSA Offset";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    public GsaOffsetGoo(GsaOffset item) : base(item) { }

    public override IGH_Goo Duplicate() => new GsaOffsetGoo(this.Value);

    public override bool CastTo<Q>(ref Q target)
    {
      if (typeof(Q).IsAssignableFrom(typeof(GsaOffset)))
      {
        if (Value == null)
          target = default;
        else
          target = (Q)(object)Value.Duplicate();
        return true;
      }

      else if (typeof(Q).IsAssignableFrom(typeof(Offset)))
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

      // Cast from GsaOffset
      if (typeof(GsaOffset).IsAssignableFrom(source.GetType()))
      {
        Value = (GsaOffset)source;
        return true;
      }

      // Cast from double
      if (GH_Convert.ToDouble(source, out double myval, GH_Conversion.Both))
      {
        Value.Z = new Length(myval, DefaultUnits.LengthUnitGeometry);
        // if input to parameter is a single number convert it to the most common Z-offset
        return true;
      }

      return base.CastFrom(source);
    }
  }
}
