using System.Text.RegularExpressions;
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

    public override bool CastFrom(object source) {
      if (source == null)
        return false;

      if (base.CastFrom(source))
        return true;

      if (GH_Convert.ToString(source, out string input, GH_Conversion.Both)) {
        var re = new Regex(@"([a-zA-Z]+)(\d+)");
        Match result = re.Match(input);

        string name = result.Groups[1]
          .Value;
        int.TryParse(result.Groups[2]
            .Value,
          out int id);

        Value = new GsaAnalysisCase(id, name);
        return true;
      }
      else if (GH_Convert.ToInt32(source, out int id, GH_Conversion.Both)) {
        Value = new GsaAnalysisCase(id, id.ToString());
        return true;
      }

      return false;
    }

    public override bool CastTo<TQ>(ref TQ target) {
      if (base.CastTo(ref target))
        return true;

      if (!typeof(TQ).IsAssignableFrom(typeof(GH_Integer)))
        return false;
      if (Value == null)
        target = default;
      else {
        var ghint = new GH_Integer();
        target = GH_Convert.ToGHInteger(Value.Id, GH_Conversion.Both, ref ghint)
          ? (TQ)(object)ghint
          : default;
      }

      return true;
    }

    public override IGH_Goo Duplicate() => new GsaAnalysisCaseGoo(Value);
  }
}
