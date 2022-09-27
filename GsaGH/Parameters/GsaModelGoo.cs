using System;
using System.IO;
using Grasshopper.Kernel.Types;
using GsaAPI;

namespace GsaGH.Parameters
{
  /// <summary>
  /// Model class, this class defines the basic properties and methods for any Gsa Model
  /// </summary>
  [Serializable]
  public class GsaModel
  {
    public Model Model
    {
      get { return m_model; }
      set { m_model = value; }
    }

    public string FileName
    {
      get { return m_filename; }
      set { m_filename = value; }
    }
    public Guid GUID
    {
      get { return m_guid; }
    }

    #region fields
    private Model m_model;
    private string m_filename = "";
    private Guid m_guid = Guid.NewGuid();

    #endregion

    #region constructors
    public GsaModel()
    {
      m_model = new Model();
    }

    /// <summary>
    /// Clones this model so we can make changes safely
    /// </summary>
    /// <returns>Returns a clone of this model with a new GUID</returns>
    public GsaModel Clone()
    {
      GsaModel clone = new GsaModel();
      clone.Model = m_model.Clone();
      clone.FileName = m_filename;
      clone.m_guid = Guid.NewGuid();
      return clone;
    }

    public GsaModel Duplicate()
    {
      //duplicate the incoming model ### 
      if (m_model != null)
      {
        GsaModel dup = new GsaModel();
        dup.Model = m_model;
        dup.FileName = m_filename.ToString();
        dup.m_guid = new Guid(m_guid.ToString());
        return dup;
      }
      return null;
    }
    #endregion

    #region properties
    public bool IsValid
    {
      get
      {
        if (m_model == null)
          return false;
        return true;
      }
    }


    #endregion

    #region methods
    public override string ToString()
    {
      //Could add detailed description of model content here
      string s = "";
      if (FileName != null)
      {
        if (FileName != "" && FileName.Length > 4)
        {
          s = Path.GetFileName(FileName);
          s = s.Substring(0, s.Length - 4);
          s = " (" + s + ")";
        }
      }

      return "GSA Model" + s;
    }

    #endregion
  }

  /// <summary>
  /// Goo wrapper class, makes sure <see cref="GsaModel"/> can be used in Grasshopper.
  /// </summary>
  public class GsaModelGoo : GH_Goo<GsaModel>
  {
    #region constructors
    public GsaModelGoo()
    {
      this.Value = new GsaModel();
    }
    public GsaModelGoo(GsaModel model)
    {
      if (model == null)
        model = new GsaModel();
      this.Value = model; //model.Duplicate();
    }

    public override IGH_Goo Duplicate()
    {
      return DuplicateGsaModel();
    }
    public GsaModelGoo DuplicateGsaModel()
    {
      return new GsaModelGoo(Value == null ? new GsaModel() : Value); //Value.Duplicate());
    }
    #endregion

    #region properties
    public override bool IsValid
    {
      get
      {
        if (Value.Model == null) { return false; }
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
        return "Null GSA Model";
      else
        return Value.ToString();
    }
    public override string TypeName
    {
      get { return ("GSA Model"); }
    }
    public override string TypeDescription
    {
      get { return ("GSA Model"); }
    }


    #endregion

    #region casting methods
    public override bool CastTo<Q>(ref Q target)
    {
      // This function is called when Grasshopper needs to convert this 
      // instance of GsaModel into some other type Q.            

      if (typeof(Q).IsAssignableFrom(typeof(GsaModel)))
      {
        if (Value == null)
          target = default;
        else
          target = (Q)(object)Value.Duplicate();
        return true;
      }

      if (typeof(Q).IsAssignableFrom(typeof(Model)))
      {
        if (Value == null)
          target = default;
        else
          target = (Q)(object)Value.Model;
        return true;
      }


      target = default;
      return false;
    }
    public override bool CastFrom(object source)
    {
      // This function is called when Grasshopper needs to convert other data 
      // into GsaModel.


      if (source == null) { return false; }

      //Cast from GsaModel
      if (typeof(GsaModel).IsAssignableFrom(source.GetType()))
      {
        Value = (GsaModel)source;
        return true;
      }

      if (typeof(Model).IsAssignableFrom(source.GetType()))
      {
        Value.Model = (Model)source;
        return true;
      }


      return false;
    }
    #endregion
  }
}
