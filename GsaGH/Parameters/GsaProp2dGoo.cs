using System;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaAPI;
using OasysGH;
using OasysGH.Parameters;
using OasysGH.Units;
using OasysUnits;
using OasysUnits.Units;

namespace GsaGH.Parameters
{
  /// <summary>
  /// Prop2d class, this class defines the basic properties and methods for any Gsa Prop2d
  /// </summary>
  public class GsaProp2d
  {
    internal Prop2D API_Prop2d
    {
      get { return m_prop2d; }
      set
      {
        m_guid = Guid.NewGuid();
        m_prop2d = value;
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
        if (m_prop2d == null)
          m_prop2d = new Prop2D();
        else
          CloneProperty();
        m_prop2d.MaterialType = Util.Gsa.ToGSA.Materials.ConvertType(m_material);
        m_prop2d.MaterialAnalysisProperty = m_material.AnalysisProperty;
        m_prop2d.MaterialGradeProperty = m_material.GradeProperty;
      }
    }
    #region GsaAPI members
    public string Name
    {
      get { return m_prop2d.Name; }
      set
      {
        CloneProperty();
        m_prop2d.Name = value;
      }
    }
    public int MaterialID
    {
      get { return m_prop2d.MaterialAnalysisProperty; }
      set
      {
        CloneProperty();
        m_prop2d.MaterialAnalysisProperty = value;
        m_material.AnalysisProperty = m_prop2d.MaterialAnalysisProperty;
      }
    }
    public Length Thickness
    {
      set
      {
        CloneProperty();
        IQuantity length = new Length(0, value.Unit);
        string unitAbbreviation = string.Concat(length.ToString().Where(char.IsLetter));
        m_prop2d.Description = value.Value.ToString() + "(" + unitAbbreviation + ")";
      }
      get
      {
        if (m_prop2d.Description.Last() == ')')
        {
          // thickness could be written as "30.33(in)"

          string unitAbbreviation = m_prop2d.Description.Split('(', ')')[1];
          LengthUnit unit = UnitParser.Default.Parse<LengthUnit>(unitAbbreviation);

          double val = double.Parse(m_prop2d.Description.Split('(')[0], System.Globalization.CultureInfo.InvariantCulture);

          return new Length(val, unit);
        }
        else
          return new Length(
              double.Parse(m_prop2d.Description, System.Globalization.CultureInfo.InvariantCulture),
              LengthUnit.Millimeter);
      }
    }
    public string Description
    {
      get { return m_prop2d.Description.Replace("%", " "); }
      set
      {
        CloneProperty();
        m_prop2d.Description = value;
      }
    }
    public int AxisProperty
    {
      get { return m_prop2d.AxisProperty; }
      set
      {
        CloneProperty();
        value = Math.Min(1, value);
        value = Math.Max(0, value);
        m_prop2d.AxisProperty = value * -1;
      }
    }
    public Property2D_Type Type
    {
      get { return m_prop2d.Type; }
      set
      {
        CloneProperty();
        m_prop2d.Type = value;
      }
    }
    public System.Drawing.Color Colour
    {
      get { return (System.Drawing.Color)m_prop2d.Colour; }
      set
      {
        CloneProperty();
        m_prop2d.Colour = value;
      }
    }
    private void CloneProperty()
    {
      if (m_prop2d == null)
      {
        m_prop2d = new Prop2D();
        m_guid = Guid.NewGuid();
        return;
      }
      Prop2D prop = new Prop2D
      {
        MaterialAnalysisProperty = m_prop2d.MaterialAnalysisProperty,
        MaterialGradeProperty = m_prop2d.MaterialGradeProperty,
        MaterialType = m_prop2d.MaterialType,
        Name = m_prop2d.Name.ToString(),
        Description = m_prop2d.Description.ToString(),
        Type = m_prop2d.Type, //GsaToModel.Prop2dType((int)m_prop2d.Type),
        AxisProperty = m_prop2d.AxisProperty
      };
      if ((System.Drawing.Color)m_prop2d.Colour != System.Drawing.Color.FromArgb(0, 0, 0)) // workaround to handle that System.Drawing.Color is non-nullable type
        prop.Colour = m_prop2d.Colour;

      m_prop2d = prop;
      m_guid = Guid.NewGuid();
    }
    #endregion
    public Guid GUID
    {
      get { return m_guid; }
    }

    #region fields
    Prop2D m_prop2d;
    int m_idd;
    GsaMaterial m_material = null;
    private Guid m_guid;
    #endregion

    #region constructors
    public GsaProp2d()
    {
      m_prop2d = null;
      m_guid = Guid.Empty;
      m_idd = 0;
    }
    public GsaProp2d(int id)
    {
      m_prop2d = null;
      m_guid = Guid.Empty;
      m_idd = id;
    }
    public GsaProp2d(Length thickness, int ID = 0)
    {
      m_prop2d = new Prop2D();
      m_material = new GsaMaterial();
      Thickness = thickness;
      m_idd = ID;
      m_guid = Guid.NewGuid();
    }
    public GsaProp2d Duplicate()
    {
      if (this == null) { return null; }
      GsaProp2d dup = new GsaProp2d();
      if (m_prop2d != null)
        dup.m_prop2d = m_prop2d;
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
      if (m_prop2d != null)
      {
        str = m_prop2d.Type.ToString();
        str = Char.ToUpper(str[0]) + str.Substring(1).ToLower().Replace("_", " ");
      }
      string pa = (ID > 0) ? "PA" + ID + " " : "";
      return "GSA 2D Property " + ((ID > 0) ? pa : "") + ((m_prop2d == null) ? "" : str);
    }

    #endregion
  }

  /// <summary>
  /// Goo wrapper class, makes sure <see cref="GsaProp2d"/> can be used in Grasshopper.
  /// </summary>
  public class GsaProp2dGoo : GH_OasysGoo<GsaProp2d>
  {
    public static string Name => "2D Property";
    public static string NickName => "P2D";
    public static string Description => "GSA 2D Property";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    public GsaProp2dGoo(GsaProp2d item) : base(item) { }

    public override IGH_Goo Duplicate() => new GsaProp2dGoo(this.Value);

    public override bool CastTo<Q>(ref Q target)
    {
      if (typeof(Q).IsAssignableFrom(typeof(GsaProp2d)))
      {
        if (Value == null)
          target = default;
        else
          target = (Q)(object)Value.Duplicate();
        return true;
      }

      else if (typeof(Q).IsAssignableFrom(typeof(Prop2D)))
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
      if (typeof(GsaProp2d).IsAssignableFrom(source.GetType()))
      {
        Value = (GsaProp2d)source;
        return true;
      }

      // Cast from GsaAPI Prop2d
      else if (typeof(Prop2D).IsAssignableFrom(source.GetType()))
      {
        Value = new GsaProp2d();
        Value.API_Prop2d = (Prop2D)source;
        return true;
      }

      // Cast from double
      else if (GH_Convert.ToDouble(source, out double thk, GH_Conversion.Both))
      {
        Value = new GsaProp2d(new Length(thk, DefaultUnits.LengthUnitSection));
      }
      return base.CastFrom(source);
    }
  }
}
