using System;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaAPI;
using OasysGH;
using OasysGH.Parameters;

namespace GsaGH.Parameters
{
  /// <summary>
  /// Goo wrapper class, makes sure <see cref="GsaProp3d"/> can be used in Grasshopper.
  /// </summary>
  public class GsaProp3dGoo : GH_OasysGoo<GsaProp3d>
  {
    public static string Name => "Prop3D";
    public static string NickName => "PV";
    public static string Description => "GSA Volume Property";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    public GsaProp3dGoo(GsaProp3d item) : base(item) { }

    public override IGH_Goo Duplicate() => new GsaProp3dGoo(this.Value);

    public override bool CastTo<Q>(ref Q target)
    {
      if (typeof(Q).IsAssignableFrom(typeof(GsaProp3d)))
      {
        if (Value == null)
          target = default;
        else
          target = (Q)(object)Value.Duplicate();
        return true;
      }

      else if (typeof(Q).IsAssignableFrom(typeof(Prop3D)))
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
          if (GH_Convert.ToGHInteger(Value.ID, GH_Conversion.Both, ref ghint))
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
      if (typeof(GsaProp3d).IsAssignableFrom(source.GetType()))
      {
        Value = (GsaProp3d)source;
        return true;
      }

      // Cast from GsaAPI Prop2d
      else if (typeof(Prop3D).IsAssignableFrom(source.GetType()))
      {
        Value = new GsaProp3d();
        Value.API_Prop3d = (Prop3D)source;
        return true;
      }

      return base.CastFrom(source);
    }
  }
}
