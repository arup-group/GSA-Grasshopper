using System;
using System.IO;
using Grasshopper.Kernel.Types;
using GsaAPI;
using OasysGH;
using OasysGH.Parameters;

namespace GsaGH.Parameters
{
  /// <summary>
  /// Model class, this class defines the basic properties and methods for any Gsa Model
  /// </summary>
  [Serializable]
  public class GsaModel
  {
    #region properties
    public Model Model { get; set; } = new Model();
    public string FileName { get; set; }
    public Guid Guid { get; set; } = Guid.NewGuid();
    
    internal GsaAPI.Titles Titles
    {
      get
      {
        return Model.Titles();
      }
    }
    #endregion

    #region constructors
    public GsaModel()
    {
    }
    #endregion

    #region methods
    /// <summary>
    /// Clones this model so we can make changes safely
    /// </summary>
    /// <returns>Returns a clone of this model with a new GUID</returns>
    public GsaModel Clone()
    {
      GsaModel clone = new GsaModel();
      clone.Model = this.Model.Clone();
      clone.FileName = this.FileName;
      clone.Guid = Guid.NewGuid();
      return clone;
    }

    public GsaModel Duplicate(bool copy = false)
    {
      if (copy)
        return this.Clone();

      // create shallow copy
      GsaModel dup = new GsaModel();
      dup.Model = this.Model;
      if (this.FileName != null)
        dup.FileName = this.FileName.ToString();
      dup.Guid = new Guid(this.Guid.ToString());
      return dup;
    }

    public override string ToString()
    {
      string s = "Invalid";
      if (this.Model != null && this.Titles != null)
      {
        s = Titles.Title;
        if (s == "" && this.FileName != null)
        {
          if (this.FileName != "" && this.FileName.EndsWith(".gwb"))
          {
            s = Path.GetFileName(this.FileName);
            s = s.Substring(0, s.Length - 4);
          }
          else
          {
            s = "Nameless";
          }
        }
        else
          s = "New GsaGH Model";
      }
      return s;
    }
    #endregion
  }
}
