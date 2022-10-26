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
      if (base.CastTo<Q>(ref target))
        return true;

      if (typeof(Q).IsAssignableFrom(typeof(GH_Integer)))
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

      if (source.GetType().IsAssignableFrom(typeof(GsaMaterial)))
      {
        Value = new GsaProp3d((GsaMaterial)source);
        return true;
      }
      if (source.GetType().IsAssignableFrom(typeof(GsaMaterialGoo)))
      {
        Value = new GsaProp3d(((GsaMaterialGoo)source).Value);
        return true;
      }

      return false;
    }
  }
}
