using System;
using System.Drawing;
using System.Linq;
using GsaAPI;

namespace GsaGH.Parameters
{
  /// <summary>
  /// Prop2d class, this class defines the basic properties and methods for any Gsa Prop2d
  /// </summary>
  public class GsaProp3d
  {
    #region fields
    private int _idd = 0;
    private Guid _guid = Guid.NewGuid();
    private Prop3D _prop3d = new Prop3D();
    private GsaMaterial _material = new GsaMaterial();
    #endregion

    #region properties
    internal Prop3D API_Prop3d
    {
      get
      {
        return this._prop3d;
      }
      set
      {
        this._guid = Guid.NewGuid();
        this._prop3d = value;
        this._material = new GsaMaterial(this);
      }
    }
    public int ID
    {
      get
      {
        return this._idd;
      }
      set
      {
        this._guid = Guid.NewGuid();
        this._idd = value;
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
        this.CloneProperty();
        this._prop3d.MaterialType = Util.Gsa.ToGSA.Materials.ConvertType(this._material);
        this._prop3d.MaterialAnalysisProperty = this._material.AnalysisProperty;
        this._prop3d.MaterialGradeProperty = this._material.GradeProperty;
      }
    }
    #region GsaAPI members
    public string Name
    {
      get
      {
        return this._prop3d.Name;
      }
      set
      {
        this.CloneProperty();
        this._prop3d.Name = value;
      }
    }
    public int MaterialID
    {
      get
      {
        return this._prop3d.MaterialAnalysisProperty;
      }
      set
      {
        this.CloneProperty();
        this._prop3d.MaterialAnalysisProperty = value;
        this._material.AnalysisProperty = this._prop3d.MaterialAnalysisProperty;
      }
    }
    public int AxisProperty
    {
      get
      {
        return this._prop3d.AxisProperty;
      }
      set
      {
        this.CloneProperty();
        value = Math.Min(1, value);
        value = Math.Max(0, value);
        this._prop3d.AxisProperty = value * -1;
      }
    }
    public Color Colour
    {
      get
      {
        return (Color)this._prop3d.Colour;
      }
      set
      {
        this.CloneProperty();
        this._prop3d.Colour = value;
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
    public GsaProp3d()
    {
    }

    public GsaProp3d(int id)
    {
      this._idd = id;
    }

    public GsaProp3d(GsaMaterial material)
    {
      this.Material = material;
    }
    #endregion

    #region methods
    public GsaProp3d Duplicate()
    {
      GsaProp3d dup = new GsaProp3d();
      dup._prop3d = this._prop3d;
      dup._idd = this._idd;
      dup._material = this._material.Duplicate();
      dup._guid = new Guid(this._guid.ToString());
      return dup;
    }

    public override string ToString()
    {
      string type = Helpers.Mappings.materialTypeMapping.FirstOrDefault(x => x.Value == this.Material.MaterialType).Key;
      string pa = (this.ID > 0) ? "PV" + this.ID + " " : "";
      return pa + type;
    }

    private void CloneProperty()
    {
      Prop3D prop = new Prop3D
      {
        MaterialAnalysisProperty = this._prop3d.MaterialAnalysisProperty,
        MaterialGradeProperty = this._prop3d.MaterialGradeProperty,
        MaterialType = this._prop3d.MaterialType,
        Name = this._prop3d.Name.ToString(),
        AxisProperty = this._prop3d.AxisProperty
      };
      if ((Color)this._prop3d.Colour != Color.FromArgb(0, 0, 0)) // workaround to handle that Color is non-nullable type
        prop.Colour = this._prop3d.Colour;

      this._prop3d = prop;
      this._guid = Guid.NewGuid();
    }
    #endregion
  }
}
