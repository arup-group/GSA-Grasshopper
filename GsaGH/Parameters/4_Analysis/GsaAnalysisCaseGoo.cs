﻿using System.Text.RegularExpressions;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using OasysGH;
using OasysGH.Parameters;

namespace GsaGH.Parameters {
  /// <summary>
  ///   Goo wrapper class, makes sure <see cref="GsaAnalysisCase" /> can be used in Grasshopper.
  /// </summary>
  public class GsaAnalysisCaseGoo : GH_OasysGoo<GsaAnalysisCase> {
    public static string Description => "GSA Analysis Case (Load Case or Combination)";
    public static string Name => "Analysis Case";
    public static string NickName => "ΣA";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    public GsaAnalysisCaseGoo(GsaAnalysisCase item) : base(item) { }
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
      return new GsaAnalysisCaseGoo(Value);
    }
  }
}
