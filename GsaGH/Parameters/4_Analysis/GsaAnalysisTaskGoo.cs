using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using OasysGH;
using OasysGH.Parameters;

namespace GsaGH.Parameters
{
  /// <summary>
  /// Goo wrapper class, makes sure <see cref="GsaAnalysisTask"/> can be used in Grasshopper.
  /// </summary>
  public class GsaAnalysisTaskGoo : GH_OasysGoo<GsaAnalysisTask>
  {
    public static string Name => "Analysis Task";
    public static string NickName => "AT";
    public static string Description => "GSA Analysis Task";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    public GsaAnalysisTaskGoo(GsaAnalysisTask item) : base(item) { }

    public override IGH_Goo Duplicate() => new GsaAnalysisTaskGoo(this.Value);

    public override bool CastTo<Q>(ref Q target)
    {
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
      return base.CastTo(ref target);
    }

    public override bool CastFrom(object source)
    {
      if (source == null)
        return false;

      // Cast from string
      if (GH_Convert.ToString(source, out string name, GH_Conversion.Both))
      {
        Value = new GsaAnalysisTask();
        Value.Name = name;
        try
        {
          Value.Type = (GsaAnalysisTask.AnalysisType)Enum.Parse(typeof(GsaAnalysisTask.AnalysisType), name);
        }
        catch (Exception)
        {
          return false;
        }
        return true;
      }
      
      return base.CastFrom(source);
    }
  }
}
