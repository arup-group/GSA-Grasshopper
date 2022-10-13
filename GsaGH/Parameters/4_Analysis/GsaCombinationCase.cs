namespace GsaGH.Parameters
{
  public class GsaCombinationCase
  {
    internal int ID { get; set; } = 0;
    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsValid
    {
      get
      {
        return true;
      }
    }

    public GsaCombinationCase()
    {
    }

    internal GsaCombinationCase(int id, string name, string description)
    {
      this.ID = id;
      this.Name = name;
      this.Description = description;
    }

    public GsaCombinationCase(string name, string description)
    {
      this.Name = name;
      this.Description = description;
    }

    #region methods
    public GsaCombinationCase Duplicate()
    {
      return new GsaCombinationCase(ID, Name, Description);
    }

    public override string ToString()
    {
      string s = "GSA Combination Case";
      if (this.Name != null)
        s += " '" + this.Name.ToString() + "'";
      if (this.Description != null)
        s += " { " + this.Description.ToString() + " }";
      return s;
    }
    #endregion
  }
}
