using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

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
  /// GsaSection Goo wrapper class, makes sure GsaSection can be used in Grasshopper.
  /// </summary>
  public class GsaAnalysisCaseGoo : GH_Goo<GsaAnalysisCase>
  {
    #region constructors
    public GsaAnalysisCaseGoo()
    {
      this.Value = new GsaAnalysisCase();
    }
    public GsaAnalysisCaseGoo(GsaAnalysisCase anal)
    {
      if (anal == null)
        anal = new GsaAnalysisCase();
      this.Value = anal; //section.Duplicate();
    }
    public override IGH_Goo Duplicate()
    {
      return DuplicateGsaAnalysisCase();
    }
    public GsaAnalysisCaseGoo DuplicateGsaAnalysisCase()
    {
      return new GsaAnalysisCaseGoo(Value == null ? new GsaAnalysisCase() : Value.Duplicate());
    }
    #endregion

    #region properties
    public override bool IsValid
    {
      get
      {
        if (Value == null) { return false; }
        return true;
      }
    }
    public override string IsValidWhyNot
    {
      get
      {
        //if (Value == null) { return "No internal GsaMember instance"; }
        if (Value.IsValid) { return string.Empty; }
        return Value.IsValid.ToString(); //Todo: beef this up to be more informative.
      }
    }
    public override string ToString()
    {
      if (Value == null)
        return "Null GSA Analysis Case";
      else
        return Value.ToString();
    }
    public override string TypeName
    {
      get { return ("GSA Analysis Case"); }
    }
    public override string TypeDescription
    {
      get { return ("GSA Analysis Case"); }
    }


    #endregion

    #region casting methods
    public override bool CastTo<Q>(ref Q target)
    {
      // This function is called when Grasshopper needs to convert this 
      // instance into some other type Q.            

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

      target = default;
      return false;
    }
    public override bool CastFrom(object source)
    {
      // This function is called when Grasshopper needs to convert other data 
      // into GsaSection.


      if (source == null) { return false; }

      //Cast from string
      if (GH_Convert.ToString(source, out string name, GH_Conversion.Both))
      {
        Value = new GsaAnalysisCase();
        Value.Name = name;
        return true;
      }

      //Cast from string
      if (GH_Convert.ToInt32(source, out int id, GH_Conversion.Both))
      {
        Value = new GsaAnalysisCase();
        Value.ID = id;
        return true;
      }

      return false;
    }
    #endregion

  }
}
