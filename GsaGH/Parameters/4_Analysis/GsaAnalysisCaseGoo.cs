using System;
using System.Text.RegularExpressions;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using OasysGH;
using OasysGH.Parameters;

namespace GsaGH.Parameters
{
  /// <summary>
  /// Goo wrapper class, makes sure <see cref="GsaAnalysisCase"/> can be used in Grasshopper.
  /// </summary>
  public class GsaAnalysisCaseGoo : GH_OasysGoo<GsaAnalysisCase>
  {
    public static string Name => "Analysis Case";
    public static string NickName => "ΣC";
    public static string Description => "GSA Analysis Case (Load Case or Combination)";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    public GsaAnalysisCaseGoo(GsaAnalysisCase item) : base(item) { }

    public override IGH_Goo Duplicate() => new GsaAnalysisCaseGoo(this.Value);

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
      return false;
    }

    public override bool CastFrom(object source)
    {
      if (source == null)
        return false;

      if (base.CastFrom(source))
        return true;

      // Cast from string
      if (GH_Convert.ToString(source, out string input, GH_Conversion.Both))
      {
        Regex re = new Regex(@"([a-zA-Z]+)(\d+)");
        Match result = re.Match(input);

        string name = result.Groups[1].Value;
        Int32.TryParse(result.Groups[2].Value, out int id);

        Value = new GsaAnalysisCase(id, name);
        return true;
      }

      // Cast from int
      else if (GH_Convert.ToInt32(source, out int id, GH_Conversion.Both))
      {
        Value = new GsaAnalysisCase(id, id.ToString());
        return true;
      }

      return false;
    }
  }
}
