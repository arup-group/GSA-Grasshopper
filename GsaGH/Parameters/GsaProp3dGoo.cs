using System;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaAPI;
using OasysGH;
using OasysGH.Parameters;

namespace GsaGH.Parameters
{
  /// <summary>
  /// Prop2d class, this class defines the basic properties and methods for any Gsa Prop2d
  /// </summary>
  public class GsaProp3d
  {
    internal Prop3D API_Prop3d
    {
      get { return m_prop3d; }
      set
      {
        m_guid = Guid.NewGuid();
        m_prop3d = value;
        m_material = new GsaMaterial(this);
      }
    }
    public int ID
    {
      get { return m_idd; }
      set
      {
        m_guid = Guid.NewGuid();
        m_idd = value;
      }
    }
    public GsaMaterial Material
    {
      get { return m_material; }
      set
      {
        m_material = value;
        if (m_prop3d == null)
          m_prop3d = new Prop3D();
        else
          CloneProperty();
        m_prop3d.MaterialType = Util.Gsa.ToGSA.Materials.ConvertType(m_material);
        m_prop3d.MaterialAnalysisProperty = m_material.AnalysisProperty;
        m_prop3d.MaterialGradeProperty = m_material.GradeProperty;
      }
    }
    #region GsaAPI members
    public string Name
    {
      get { return m_prop3d.Name; }
      set
      {
        CloneProperty();
        m_prop3d.Name = value;
      }
    }
    public int MaterialID
    {
      get { return m_prop3d.MaterialAnalysisProperty; }
      set
      {
        CloneProperty();
        m_prop3d.MaterialAnalysisProperty = value;
        m_material.AnalysisProperty = m_prop3d.MaterialAnalysisProperty;
      }
    }


    public int AxisProperty
    {
      get { return m_prop3d.AxisProperty; }
      set
      {
        CloneProperty();
        value = Math.Min(1, value);
        value = Math.Max(0, value);
        m_prop3d.AxisProperty = value * -1;
      }
    }

    public System.Drawing.Color Colour
    {
      get { return (System.Drawing.Color)m_prop3d.Colour; }
      set
      {
        CloneProperty();
        m_prop3d.Colour = value;
      }
    }
    private void CloneProperty()
    {
      if (m_prop3d == null)
      {
        m_prop3d = new Prop3D();
        m_guid = Guid.NewGuid();
        return;
      }
      Prop3D prop = new Prop3D
      {
        MaterialAnalysisProperty = m_prop3d.MaterialAnalysisProperty,
        MaterialGradeProperty = m_prop3d.MaterialGradeProperty,
        MaterialType = m_prop3d.MaterialType,
        Name = m_prop3d.Name.ToString(),
        AxisProperty = m_prop3d.AxisProperty
      };
      if ((System.Drawing.Color)m_prop3d.Colour != System.Drawing.Color.FromArgb(0, 0, 0)) // workaround to handle that System.Drawing.Color is non-nullable type
        prop.Colour = m_prop3d.Colour;

      m_prop3d = prop;
      m_guid = Guid.NewGuid();
    }
    #endregion
    public Guid GUID
    {
      get { return m_guid; }
    }

    #region fields
    Prop3D m_prop3d;
    int m_idd;
    GsaMaterial m_material = null;
    private Guid m_guid;
    #endregion

    #region constructors
    public GsaProp3d()
    {
      m_prop3d = null;
      m_guid = Guid.Empty;
      m_idd = 0;
    }
    public GsaProp3d(int id)
    {
      m_prop3d = null;
      m_guid = Guid.Empty;
      m_idd = id;
    }
    public GsaProp3d(GsaMaterial material)
    {
      m_prop3d = new Prop3D();
      m_guid = Guid.Empty;
      m_idd = 0;
      this.Material = material;
    }


    public GsaProp3d Duplicate()
    {
      if (this == null) { return null; }
      GsaProp3d dup = new GsaProp3d();
      if (m_prop3d != null)
        dup.m_prop3d = m_prop3d;
      dup.m_idd = m_idd;
      if (m_material != null)
        dup.m_material = m_material.Duplicate();
      dup.m_guid = new Guid(m_guid.ToString());
      return dup;
    }
    #endregion

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
      string str = "";
      if (m_prop3d != null)
      {
        str = m_prop3d.MaterialType.ToString();
        str = Char.ToUpper(str[0]) + str.Substring(1).ToLower().Replace("_", " ");
      }
      string pa = (ID > 0) ? "PV" + ID + " " : "";
      return "GSA 3D Property " + ((ID > 0) ? pa : "") + ((m_prop3d == null) ? "" : str);
    }

    #endregion
  }

  /// <summary>
  /// Goo wrapper class, makes sure <see cref="GsaProp3d"/> can be used in Grasshopper.
  /// </summary>
  public class GsaProp3dGoo : GH_OasysGoo<GsaProp3d>
  {
    public static string Name => "3D Property";
    public static string NickName => "3dP";
    public static string Description => "GSA 3D Property";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    public GsaProp3dGoo(GsaProp3d item) : base(item) { }

    public override IGH_Goo Duplicate() => new GsaProp3dGoo(this.Value);

    public override bool CastTo<Q>(ref Q target)
    {
      if (typeof(Q).IsAssignableFrom(typeof(GsaProp3d)))
      {
        if (Value == null)
          target = default;
        else
          target = (Q)(object)Value.Duplicate();
        return true;
      }

      else if (typeof(Q).IsAssignableFrom(typeof(Prop3D)))
      {
        if (Value == null)
          target = default;
        else
          target = (Q)(object)Value;
        return true;
      }

      else if (typeof(Q).IsAssignableFrom(typeof(GH_Integer)))
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

      // Cast from GsaProp2d
      if (typeof(GsaProp3d).IsAssignableFrom(source.GetType()))
      {
        Value = (GsaProp3d)source;
        return true;
      }

      // Cast from GsaAPI Prop2d
      else if (typeof(Prop3D).IsAssignableFrom(source.GetType()))
      {
        Value = new GsaProp3d();
        Value.API_Prop3d = (Prop3D)source;
        return true;
      }

      return base.CastFrom(source);
    }
  }
}
