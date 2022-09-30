using System;
using System.IO;
using Grasshopper.Kernel.Types;
using GsaAPI;
using OasysGH;
using OasysGH.Parameters;

namespace GsaGH.Parameters
{
  /// <summary>
  /// Goo wrapper class, makes sure <see cref="GsaModel"/> can be used in Grasshopper.
  /// </summary>
  public class GsaModelGoo : GH_OasysGoo<GsaModel>
  {
    public static string Name => "Model";
    public static string NickName => "M";
    public static string Description => "GSA Model";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    public GsaModelGoo(GsaModel item) : base(item) { }

    public override IGH_Goo Duplicate() => new GsaModelGoo(this.Value);

    public override bool CastTo<Q>(ref Q target)
    {
      if (typeof(Q).IsAssignableFrom(typeof(GsaModel)))
      {
        if (Value == null)
          target = default;
        else
          target = (Q)(object)Value.Duplicate();
        return true;
      }

      else if (typeof(Q).IsAssignableFrom(typeof(Model)))
      {
        if (Value == null)
          target = default;
        else
          target = (Q)(object)Value.Model;
        return true;
      }

      return CastTo(ref target);
    }

    public override bool CastFrom(object source)
    {
      if (source == null)
        return false;

      // Cast from GsaModel
      if (typeof(GsaModel).IsAssignableFrom(source.GetType()))
      {
        Value = (GsaModel)source;
        return true;
      }

      if (typeof(Model).IsAssignableFrom(source.GetType()))
      {
        Value.Model = (Model)source;
        return true;
      }

      return base.CastFrom(source);
    }
  }
}
