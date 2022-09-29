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
  public class GsaModelGoo : GH_OasysGoo<GsaModel>
  {
    public static string Name => "Model";
    public static string NickName => "M";
    public static string Description => "GSA Model";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    public GsaModelGoo(GsaModel item) : base(item) { }

    public override IGH_Goo Duplicate() => new GsaModelGoo(this.Value);

    public override bool CastTo<Q>(ref Q target)
    {
      if (typeof(Q).IsAssignableFrom(typeof(GsaModel)))
      {
        if (Value == null)
          target = default;
        else
          target = (Q)(object)Value.Duplicate();
        return true;
      }

      else if (typeof(Q).IsAssignableFrom(typeof(Model)))
      {
        if (Value == null)
          target = default;
        else
          target = (Q)(object)Value.Model;
        return true;
      }

      return CastTo(ref target);
    }

    public override bool CastFrom(object source)
    {
      if (source == null)
        return false;

      // Cast from GsaModel
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

      return base.CastFrom(source);
    }
  }
}
