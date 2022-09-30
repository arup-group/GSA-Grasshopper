using Grasshopper.Kernel.Types;
using OasysGH;
using OasysGH.Parameters;

namespace GsaGH.Parameters
{
  /// <summary>
  /// Goo wrapper class, makes sure <see cref="GsaCombinationCase"/> can be used in Grasshopper.
  /// </summary>
  public class GsaCombinationCaseGoo : GH_OasysGoo<GsaCombinationCase>
  {
    public static string Name => "Combination Case";
    public static string NickName => "CC";
    public static string Description => "GSA Combination Case";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    public GsaCombinationCaseGoo(GsaCombinationCase item) : base(item) { }

    public override IGH_Goo Duplicate() => new GsaCombinationCaseGoo(this.Value);

    public override bool CastTo<Q>(ref Q target)
    {
      //if (typeof(Q).IsAssignableFrom(typeof(GH_Integer)))
      //{
      //    if (Value == null)
      //        target = default;
      //    else
      //    {
      //        GH_Integer ghint = new GH_Integer();
      //        if (GH_Convert.ToGHInteger(Value.ID, GH_Conversion.Both, ref ghint))
      //            target = (Q)(object)ghint;
      //        else
      //            target = default;
      //    }
      //    return true;
      //}
      return base.CastTo<Q>(ref target);
    }
    public override bool CastFrom(object source)
    {
      //Cast from string
      //if (GH_Convert.ToString(source, out string name, GH_Conversion.Both))
      //{
      //    Value = new GsaAnalysisCase();
      //    Value.Name = name;
      //    return true;
      //}

      ////Cast from string
      //if (GH_Convert.ToInt32(source, out int id, GH_Conversion.Both))
      //{
      //    Value = new GsaAnalysisCase();
      //    Value.ID = id;
      //    return true;
      //}
      return base.CastFrom(source);
    }
  }
}
