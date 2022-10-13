using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using GsaAPI;
using OasysUnits;
using OasysUnits.Units;

namespace GsaGH.Parameters
{
  /// <summary>
  /// Prop2d class, this class defines the basic properties and methods for any Gsa Prop2d
  /// </summary>
  public class GsaProp2d
  {
    #region fields
    private int m_idd = 0;
    private GsaMaterial m_material = new GsaMaterial();
    private Prop2D m_prop2d = new Prop2D();
    private Guid m_guid = Guid.NewGuid();
    #endregion

    #region properties
    internal Prop2D API_Prop2d
    {
      get
      {
        return this.m_prop2d;
      }
      set
      {
        this.m_guid = Guid.NewGuid();
        this.m_prop2d = value;
        this.m_material = new GsaMaterial(this);
      }
    }
    public int ID
    {
      get
      {
        return this.m_idd;
      }
      set
      {
        this.m_guid = Guid.NewGuid();
        this.m_idd = value;
      }
    }
    public GsaMaterial Material
    {
      get
      {
        return this.m_material;
      }
      set
      {
        this.m_material = value;
        CloneProperty();
        this.m_prop2d.MaterialType = Util.Gsa.ToGSA.Materials.ConvertType(m_material);
        this.m_prop2d.MaterialAnalysisProperty = this.m_material.AnalysisProperty;
        this.m_prop2d.MaterialGradeProperty = this.m_material.GradeProperty;
      }
    }
    #region GsaAPI members
    public string Name
    {
      get
      {
        return this.m_prop2d.Name;
      }
      set
      {
        this.CloneProperty();
        this.m_prop2d.Name = value;
      }
    }
    public int MaterialID
    {
      get
      {
        return
          this.m_prop2d.MaterialAnalysisProperty;
      }
      set
      {
        this.CloneProperty();
        this.m_prop2d.MaterialAnalysisProperty = value;
        this.m_material.AnalysisProperty = this.m_prop2d.MaterialAnalysisProperty;
      }
    }
    public Length Thickness
    {
      set
      {
        this.CloneProperty();
        IQuantity length = new Length(0, value.Unit);
        string unitAbbreviation = string.Concat(length.ToString().Where(char.IsLetter));
        this.m_prop2d.Description = value.Value.ToString() + "(" + unitAbbreviation + ")";
      }
      get
      {
        if (this.m_prop2d.Description.Length == 0)
          return Length.Zero;
        if (this.m_prop2d.Description.Last() == ')')
        {
          // thickness could be written as "30.33(in)"

          string unitAbbreviation = this.m_prop2d.Description.Split('(', ')')[1];
          LengthUnit unit = UnitParser.Default.Parse<LengthUnit>(unitAbbreviation);

          double val = double.Parse(this.m_prop2d.Description.Split('(')[0], System.Globalization.CultureInfo.InvariantCulture);

          return new Length(val, unit);
        }
        else
          return new Length(double.Parse(this.m_prop2d.Description, CultureInfo.InvariantCulture), LengthUnit.Millimeter);
      }
    }
    public string Description
    {
      get
      {
        return this.m_prop2d.Description.Replace("%", " ");
      }
      set
      {
        this.CloneProperty();
        this.m_prop2d.Description = value;
      }
    }
    public int AxisProperty
    {
      get
      {
        return this.m_prop2d.AxisProperty;
      }
      set
      {
        this.CloneProperty();
        value = Math.Min(1, value);
        value = Math.Max(0, value);
        this.m_prop2d.AxisProperty = value * -1;
      }
    }
    public Property2D_Type Type
    {
      get
      {
        return m_prop2d.Type;
      }
      set
      {
        this.CloneProperty();
        this.m_prop2d.Type = value;
      }
    }
    public Color Colour
    {
      get
      {
        return (Color)m_prop2d.Colour;
      }
      set
      {
        this.CloneProperty();
        this.m_prop2d.Colour = value;
      }
    }
    #endregion
    public Guid GUID
    {
      get
      {
        return this.m_guid;
      }
    }
    public bool IsValid
    {
      get
      {
        return true;
      }
    }
    #endregion

    #region constructors
    public GsaProp2d()
    {
    }

    public GsaProp2d(int id)
    {
      this.m_idd = id;
    }

    public GsaProp2d(Length thickness, int id = 0)
    {
      this.Thickness = thickness;
      this.m_idd = id;
    }
    #endregion

    #region methods
    internal static Property2D_Type PropTypeFromString(string type)
    {
      type = type.Trim().Replace(" ", "_").ToUpper();
      type = type.Replace("PLANE", "PL");
      type = type.Replace("NUMBER", "NUM");
      type = type.Replace("AXIS_SYMMETRIC", "AXISYMMETRIC");
      type = type.Replace("LOAD_PANEL", "LOAD");
      return (Property2D_Type)Enum.Parse(typeof(Property2D_Type), type);
    }

    public GsaProp2d Duplicate()
    {
      GsaProp2d dup = new GsaProp2d();
      dup.m_prop2d = this.m_prop2d;
      dup.m_idd = this.m_idd;
      dup.m_material = this.m_material.Duplicate();
      dup.m_guid = new Guid(this.m_guid.ToString());
      return dup;
    }

    public override string ToString()
    {
      string str = this.m_prop2d.Type.ToString();
      str = Char.ToUpper(str[0]) + str.Substring(1).ToLower().Replace("_", " ");
      string pa = (this.ID > 0) ? "PA" + this.ID + " " : "";
      return "GSA 2D Property " + ((this.ID > 0) ? pa : "") + ((this.m_prop2d == null) ? "" : str);
    }

    private void CloneProperty()
    {
      Prop2D prop = new Prop2D
      {
        MaterialAnalysisProperty = this.m_prop2d.MaterialAnalysisProperty,
        MaterialGradeProperty = this.m_prop2d.MaterialGradeProperty,
        MaterialType = this.m_prop2d.MaterialType,
        Name = this.m_prop2d.Name.ToString(),
        Description = this.m_prop2d.Description.ToString(),
        Type = this.m_prop2d.Type, //GsaToModel.Prop2dType((int)m_prop2d.Type),
        AxisProperty = this.m_prop2d.AxisProperty
      };
      if ((Color)this.m_prop2d.Colour != Color.FromArgb(0, 0, 0)) // workaround to handle that System.Drawing.Color is non-nullable type
        prop.Colour = this.m_prop2d.Colour;

      this.m_prop2d = prop;
      this.m_guid = Guid.NewGuid();
    }
    #endregion
  }
}
