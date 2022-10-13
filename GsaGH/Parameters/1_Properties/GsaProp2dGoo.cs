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
  /// Goo wrapper class, makes sure <see cref="GsaProp2d"/> can be used in Grasshopper.
  /// </summary>
  public class GsaProp2dGoo : GH_OasysGoo<GsaProp2d>
  {
    public static string Name => "Prop2D";
    public static string NickName => "PA";
    public static string Description => "GSA Area Property";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    public GsaProp2dGoo(GsaProp2d item) : base(item) { }

    public override IGH_Goo Duplicate() => new GsaProp2dGoo(this.Value);

    public override bool CastTo<Q>(ref Q target)
    {
      if (typeof(Q).IsAssignableFrom(typeof(GsaProp2d)))
      {
        if (Value == null)
          target = default;
        else
          target = (Q)(object)Value.Duplicate();
        return true;
      }

      else if (typeof(Q).IsAssignableFrom(typeof(Prop2D)))
      {
        if (Value == null)
          target = default;
        else
          target = (Q)(object)Value;
        return true;
      }

      else if (typeof(Q).IsAssignableFrom(typeof(GH_Integer)))
      {
        if (Value == null)
          target = default;
        else
        {
          GH_Integer ghint = new GH_Integer();
          if (GH_Convert.ToGHInteger(Value.Id, GH_Conversion.Both, ref ghint))
            target = (Q)(object)ghint;
          else
            target = default;
        }
        return true;
      }

      return base.CastTo(ref target);
    }

    public override bool CastFrom(object source)
    {
      if (source == null)
        return false;

      // Cast from GsaProp2d
      if (typeof(GsaProp2d).IsAssignableFrom(source.GetType()))
      {
        Value = (GsaProp2d)source;
        return true;
      }

      // Cast from GsaAPI Prop2d
      else if (typeof(Prop2D).IsAssignableFrom(source.GetType()))
      {
        Value = new GsaProp2d();
        Value.API_Prop2d = (Prop2D)source;
        return true;
      }

      // Cast from double
      else if (GH_Convert.ToDouble(source, out double thk, GH_Conversion.Both))
      {
        Value = new GsaProp2d(new Length(thk, DefaultUnits.LengthUnitSection));
      }
      return base.CastFrom(source);
    }
  }
}
