using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using OasysGH;
using OasysGH.Parameters;

namespace GsaGH.Parameters
{
  public class GsaAnalysisCase
  {
    public string Name { get; set; }
    public string Description { get; set; }
    internal int ID { get; set; } = 0;
    public GsaAnalysisCase()
    { }
    internal GsaAnalysisCase(int id, string name, string description)
    {
      this.ID = id;
      this.Name = name;
      this.Description = description;
    }
    public GsaAnalysisCase(string name, string description)
    {
      this.Name = name;
      this.Description = description;
    }
    public GsaAnalysisCase Duplicate()
    {
      return new GsaAnalysisCase(ID, Name, Description);
    }

    #region properties
    public bool IsValid
    {
      get
      {
        return true;
      }
    }
    #endregion

    #region methods
    public override string ToString()
    {
      return "GSA Analysis Case '" + Name.ToString() + "' {" + Description.ToString() + "}";
    }

    #endregion
  }

  /// <summary>
  /// Goo wrapper class, makes sure <see cref="GsaAnalysisCase"/> can be used in Grasshopper.
  /// </summary>
  public class GsaAnalysisCaseGoo : GH_OasysGoo<GsaAnalysisCase>
  {
    public static string Name => "Analysis Case";
    public static string NickName => "CreateCase";
    public static string Description => "GSA Analysis Case (Load Case or Combination)";
    public override IGH_Goo Duplicate() => new GsaAnalysisCaseGoo(this.Value);
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    public GsaAnalysisCaseGoo(GsaAnalysisCase item) : base(item) { }

    public override bool CastTo<Q>(ref Q target)
    {
      if (Value != null)
      {
        if (typeof(Q).IsAssignableFrom(typeof(GH_Integer)))
        {
          GH_Integer ghint = new GH_Integer();
          if (GH_Convert.ToGHInteger(Value.ID, GH_Conversion.Both, ref ghint))
            target = (Q)(object)ghint;
          else
            target = default;
          return true;
        }
      }
      return base.CastTo(ref target);
    }

    public override bool CastFrom(object source)
    {
      if (source != null)
      {
        // Cast from string
        if (GH_Convert.ToString(source, out string name, GH_Conversion.Both))
        {
          Value = new GsaAnalysisCase();
          Value.Name = name;
          return true;
        }
        // Cast from int
        else if (GH_Convert.ToInt32(source, out int id, GH_Conversion.Both))
        {
          Value = new GsaAnalysisCase();
          Value.ID = id;
          return true;
        }
      }
      return base.CastFrom(source);
    }
  }
}
