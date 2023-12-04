﻿using Grasshopper.Kernel.Types;
using OasysGH;
using OasysGH.Parameters;

namespace GsaGH.Parameters {
  /// <summary>
  ///   Goo wrapper class, makes sure <see cref="GsaSection" /> and <see cref="GsaSpringProperty" />can be used in Grasshopper.
  /// </summary>
  public class GsaPropertyGoo : GH_OasysGoo<IGsaProperty> {
    public static string Description => "GSA Section Property (Beam) or Spring Property";
    public static string Name => "Section";
    public static string NickName => "PB";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    public GsaPropertyGoo(IGsaProperty item) : base(item) { }

    public override bool CastTo<TQ>(ref TQ target) {
      if (typeof(TQ).IsAssignableFrom(typeof(GH_Integer))) {
        if (Value != null) {
          target = (TQ)(object)new GH_Integer(Value.Id);
          return true;
        }
      }

      target = default;
      return false;
    }

    public override IGH_Goo Duplicate() {
      return new GsaPropertyGoo(Value);
    }
  }
}
