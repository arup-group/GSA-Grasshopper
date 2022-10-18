using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using GsaAPI;
using OasysUnits;
using OasysUnits.Units;

namespace GsaGH.Parameters
{
  /// <summary>
  /// Prop2d class, this class defines the basic properties and methods for any <see cref="GsaAPI.Prop2D"/>
  /// </summary>
  public class GsaProp2d
  {
    #region fields
    private int _id = 0;
    private Guid _guid = Guid.NewGuid();
    private GsaMaterial _material = new GsaMaterial();
    private Prop2D _prop2d = new Prop2D();
    
    #endregion

    #region properties
    internal Prop2D API_Prop2d
    {
      get
      {
        return this._prop2d;
      }
      set
      {
        this._guid = Guid.NewGuid();
        this._prop2d = value;
        this._material = new GsaMaterial(this);
      }
    }
    public int ID
    {
      get
      {
        return this._id;
      }
      set
      {
        this._guid = Guid.NewGuid();
        this._id = value;
      }
    }
    public GsaMaterial Material
    {
      get
      {
        return this._material;
      }
      set
      {
        this._material = value;
        CloneProperty();
        this._prop2d.MaterialType = Util.Gsa.ToGSA.Materials.ConvertType(_material);
        this._prop2d.MaterialAnalysisProperty = this._material.AnalysisProperty;
        this._prop2d.MaterialGradeProperty = this._material.GradeProperty;
      }
    }
    #region GsaAPI members
    public string Name
    {
      get
      {
        return this._prop2d.Name;
      }
      set
      {
        this.CloneProperty();
        this._prop2d.Name = value;
      }
    }
    public int MaterialID
    {
      get
      {
        return
          this._prop2d.MaterialAnalysisProperty;
      }
      set
      {
        this.CloneProperty();
        this._prop2d.MaterialAnalysisProperty = value;
        this._material.AnalysisProperty = this._prop2d.MaterialAnalysisProperty;
      }
    }
    public Length Thickness
    {
      set
      {
        this.CloneProperty();
        IQuantity length = new Length(0, value.Unit);
        string unitAbbreviation = string.Concat(length.ToString().Where(char.IsLetter));
        this._prop2d.Description = value.Value.ToString() + "(" + unitAbbreviation + ")";
      }
      get
      {
        if (this._prop2d.Description.Length == 0)
          return Length.Zero;
        if (this._prop2d.Description.Last() == ')')
        {
          // thickness could be written as "30.33(in)"

          string unitAbbreviation = this._prop2d.Description.Split('(', ')')[1];
          LengthUnit unit = UnitParser.Default.Parse<LengthUnit>(unitAbbreviation);

          double val = double.Parse(this._prop2d.Description.Split('(')[0], System.Globalization.CultureInfo.InvariantCulture);

          return new Length(val, unit);
        }
        else
          return new Length(double.Parse(this._prop2d.Description, CultureInfo.InvariantCulture), LengthUnit.Millimeter);
      }
    }
    public string Description
    {
      get
      {
        return this._prop2d.Description.Replace("%", " ");
      }
      set
      {
        this.CloneProperty();
        this._prop2d.Description = value;
      }
    }
    public int AxisProperty
    {
      get
      {
        return this._prop2d.AxisProperty;
      }
      set
      {
        this.CloneProperty();
        value = Math.Min(1, value);
        value = Math.Max(0, value);
        this._prop2d.AxisProperty = value * -1;
      }
    }
    public Property2D_Type Type
    {
      get
      {
        return _prop2d.Type;
      }
      set
      {
        this.CloneProperty();
        this._prop2d.Type = value;
      }
    }
    public Color Colour
    {
      get
      {
        return (Color)_prop2d.Colour;
      }
      set
      {
        this.CloneProperty();
        this._prop2d.Colour = value;
      }
    }
    #endregion
    public Guid GUID
    {
      get
      {
        return this._guid;
      }
    }
    #endregion

    #region constructors
    public GsaProp2d()
    {
    }

    public GsaProp2d(int id)
    {
      this._id = id;
    }

    public GsaProp2d(Length thickness, int id = 0)
    {
      this.Thickness = thickness;
      this._id = id;
    }
    #endregion

    #region methods
    internal static Property2D_Type PropTypeFromString(string type)
    {
      if (Helpers.Mappings.prop2dTypeMapping.ContainsKey(type))
        return Helpers.Mappings.prop2dTypeMapping[type];
      else
      {
        type = type.Trim().Replace(" ", "_").ToUpper();
        type = type.Replace("PLANE", "PL");
        type = type.Replace("NUMBER", "NUM");
        type = type.Replace("AXIS_SYMMETRIC", "AXISYMMETRIC");
        type = type.Replace("LOAD_PANEL", "LOAD");
        return (Property2D_Type)Enum.Parse(typeof(Property2D_Type), type);
      }
    }

    public GsaProp2d Duplicate()
    {
      GsaProp2d dup = new GsaProp2d();
      dup._prop2d = this._prop2d;
      dup._id = this._id;
      dup._material = this._material.Duplicate();
      dup._guid = new Guid(this._guid.ToString());
      return dup;
    }

    public override string ToString()
    {
      string type = Helpers.Mappings.prop2dTypeMapping.FirstOrDefault(x => x.Value == this._prop2d.Type).Key;
      string desc = this.Description.Replace("(", string.Empty).Replace(")", string.Empty) + " ";
      string pa = (this.ID > 0) ? "PA" + this.ID + " " : "";
      return pa + desc + type;
    }

    private void CloneProperty()
    {
      Prop2D prop = new Prop2D
      {
        MaterialAnalysisProperty = this._prop2d.MaterialAnalysisProperty,
        MaterialGradeProperty = this._prop2d.MaterialGradeProperty,
        MaterialType = this._prop2d.MaterialType,
        Name = this._prop2d.Name.ToString(),
        Description = this._prop2d.Description.ToString(),
        Type = this._prop2d.Type, //GsaToModel.Prop2dType((int)m_prop2d.Type),
        AxisProperty = this._prop2d.AxisProperty
      };
      if ((Color)this._prop2d.Colour != Color.FromArgb(0, 0, 0)) // workaround to handle that System.Drawing.Color is non-nullable type
        prop.Colour = this._prop2d.Colour;

      this._prop2d = prop;
      this._guid = Guid.NewGuid();
    }
    #endregion
  }
}
