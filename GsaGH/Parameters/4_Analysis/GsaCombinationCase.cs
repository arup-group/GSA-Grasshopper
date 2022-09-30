using Grasshopper.Kernel.Types;
using OasysGH;
using OasysGH.Components;
using OasysGH.Parameters;

namespace GsaGH.Parameters
{
  public class GsaCombinationCase
  {
    public string Name { get; set; }
    public string Description { get; set; }
    internal int ID { get; set; } = 0;
    public GsaCombinationCase()
    { }
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
    public GsaCombinationCase Duplicate()
    {
      return new GsaCombinationCase(ID, Name, Description);
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
      return "GSA Combination Case '" + Name.ToString() + "' {" + Description.ToString() + "}";
    }

    #endregion
  }
}
