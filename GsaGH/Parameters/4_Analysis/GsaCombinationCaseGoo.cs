﻿using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using OasysGH;
using OasysGH.Parameters;

namespace GsaGH.Parameters {
  /// <summary>
  ///   Goo wrapper class, makes sure <see cref="GsaCombinationCase" /> can be used in Grasshopper.
  /// </summary>
  public class GsaCombinationCaseGoo : GH_OasysGoo<GsaCombinationCase> {
    public static string Description => "GSA Combination Case";
    public static string Name => "Combination Case";
    public static string NickName => "ΣC";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    public GsaCombinationCaseGoo(GsaCombinationCase item) : base(item) { }
    public override bool CastTo<TQ>(ref TQ target) {
      if (base.CastTo(ref target)) {
        return true;
      }

      if (typeof(TQ).IsAssignableFrom(typeof(GH_Integer))) {
        if (Value != null) {
          var ghint = new GH_Integer();
          GH_Convert.ToGHInteger(Value.Id, GH_Conversion.Both, ref ghint);
          target = (TQ)(object)ghint;
          return true;
        }
      }

      target = default;
      return false;
    }
    public override IGH_Goo Duplicate() {
      return new GsaCombinationCaseGoo(Value);
    }
  }
}
