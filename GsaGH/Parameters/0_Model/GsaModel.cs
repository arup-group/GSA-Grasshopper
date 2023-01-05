using System;
using System.IO;
using GsaAPI;
using OasysUnits;
using OasysUnits.Units;

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
    public string FileNameAndPath { get; set; }
    public Guid Guid { get; set; } = Guid.NewGuid();
    public LengthUnit ModelUnit { get; set; } = LengthUnit.Undefined;
    
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
      clone.FileNameAndPath = this.FileNameAndPath;
      clone.ModelUnit = this.ModelUnit;
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
      if (this.FileNameAndPath != null)
        dup.FileNameAndPath = this.FileNameAndPath.ToString();
      dup.Guid = new Guid(this.Guid.ToString());
      dup.ModelUnit = this.ModelUnit;
      return dup;
    }

    public override string ToString()
    {
      string s = "Invalid";
      if (this.Model != null && this.Titles != null)
      {
        s = Titles.Title;
        if (s == "" && this.FileNameAndPath != null)
        {
          if (this.FileNameAndPath != "" && this.FileNameAndPath.EndsWith(".gwb"))
          {
            s = Path.GetFileName(this.FileNameAndPath);
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
      if (this.ModelUnit != LengthUnit.Undefined)
        s += " [" + Length.GetAbbreviation(this.ModelUnit) + "]";
      return s;
    }
    #endregion
  }
}
