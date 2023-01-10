﻿using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using GsaAPI;
using GsaGH.Helpers.GsaAPI;

namespace GsaGH.Parameters
{
  /// <summary>
  /// Prop2d class, this class defines the basic properties and methods for any <see cref="GsaAPI.Prop3D"/>
  /// </summary>
  public class GsaProp3d
  {
    #region fields
    private int _id = 0;
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
    public int Id
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
    internal bool IsReferencedByID { get; set; } = false;
    public GsaMaterial Material
    {
      get
      {
        return this._material;
      }
      set
      {
        this._material = value;
        if (this._prop3d == null)
          this._prop3d = new Prop3D();
        else
          this.CloneApiObject();

        this._prop3d.MaterialType = Helpers.Export.Materials.ConvertType(this._material);
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
        this.CloneApiObject();
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
        this.CloneApiObject();
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
        this.CloneApiObject();
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
        this.CloneApiObject();
        this._prop3d.Colour = value;
      }
    }
    #endregion
    public Guid Guid
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
      this._id = id;
      this.IsReferencedByID = true;
    }

    public GsaProp3d(GsaMaterial material)
    {
      this.Material = material;
    }

    internal GsaProp3d(ReadOnlyDictionary<int, Prop3D> pDict, int id, ReadOnlyDictionary<int, AnalysisMaterial> matDict) : this(id)
    {
      if (!pDict.ContainsKey(id))
        return;
      this._prop3d = pDict[id];
      this.IsReferencedByID = false;
      // material
      if (this._prop3d.MaterialAnalysisProperty != 0 && matDict.ContainsKey(this._prop3d.MaterialAnalysisProperty))
        this._material.AnalysisMaterial = matDict[this._prop3d.MaterialAnalysisProperty];
      this._material = new GsaMaterial(this);
    }
    #endregion

    #region methods
    public GsaProp3d Duplicate(bool cloneApiElement = false)
    {
      GsaProp3d dup = new GsaProp3d
      {
        _prop3d = this._prop3d,
        _id = this._id,
        _material = this._material.Duplicate(),
        _guid = new Guid(this._guid.ToString()),
        IsReferencedByID = this.IsReferencedByID
      };
      if (cloneApiElement)
        dup.CloneApiObject();
      return dup;
    }

    public override string ToString()
    {
      string type = Mappings.MaterialTypeMapping.FirstOrDefault(x => x.Value == this.Material.MaterialType).Key;
      string pa = (this.Id > 0) ? "PV" + this.Id + " " : "";
      return string.Join(" ", pa.Trim(), type.Trim()).Trim().Replace("  ", " ");
    }

    private void CloneApiObject()
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
