﻿using System;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using OasysGH;
using OasysGH.Parameters;

namespace GsaGH.Parameters {
  /// <summary>
  ///   Goo wrapper class, makes sure <see cref="GsaAnalysisTask" /> can be used in Grasshopper.
  /// </summary>
  public class GsaAnalysisTaskGoo : GH_OasysGoo<GsaAnalysisTask> {
    public static string Description => "GSA Analysis Task";
    public static string Name => "Analysis Task";
    public static string NickName => "ΣT";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    public GsaAnalysisTaskGoo(GsaAnalysisTask item) : base(item) { }

    public override bool CastFrom(object source) {
      if (source == null) {
        return false;
      }

      if (base.CastFrom(source)) {
        return true;
      }

      if (!GH_Convert.ToString(source, out string name, GH_Conversion.Both)) {
        return false;
      }

      Value = new GsaAnalysisTask {
        Name = name,
      };
      try {
        Value.Type
          = (GsaAnalysisTask.AnalysisType)Enum.Parse(typeof(GsaAnalysisTask.AnalysisType), name);
      }
      catch (Exception) {
        return false;
      }

      return true;
    }

    public override bool CastTo<TQ>(ref TQ target) {
      if (base.CastTo(ref target)) {
        return true;
      }

      if (!typeof(TQ).IsAssignableFrom(typeof(GH_Integer))) {
        return base.CastTo(ref target);
      }

      if (Value == null) {
        target = default;
      }
      else {
        var ghint = new GH_Integer();
        target = GH_Convert.ToGHInteger(Value.Id, GH_Conversion.Both, ref ghint) ? (TQ)(object)ghint : default;
      }

      return true;
    }

    public override IGH_Goo Duplicate() {
      return new GsaAnalysisTaskGoo(Value);
    }
  }
}
