using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using GsaAPI;
using GsaGH.Helpers.GsaApi;

namespace GsaGH.Parameters {
  /// <summary>
  /// Prop2d class, this class defines the basic properties and methods for any <see cref="GsaAPI.Prop3D"/>
  /// </summary>
  public class GsaProp3d {
    public int AxisProperty {
      get {
        return _prop3d.AxisProperty;
      }
      set {
        CloneApiObject();
        value = Math.Min(1, value);
        value = Math.Max(0, value);
        _prop3d.AxisProperty = value * -1;
        IsReferencedById = false;
      }
    }
    public Color Colour {
      get {
        return (Color)_prop3d.Colour;
      }
      set {
        CloneApiObject();
        _prop3d.Colour = value;
        IsReferencedById = false;
      }
    }
    public Guid Guid {
      get {
        return _guid;
      }
    }
    public int Id {
      get {
        return _id;
      }
      set {
        _guid = Guid.NewGuid();
        _id = value;
      }
    }
    public GsaMaterial Material {
      get {
        return _material;
      }
      set {
        _material = value;
        if (_prop3d == null)
          _prop3d = new Prop3D();
        else
          CloneApiObject();

        _prop3d.MaterialType = Helpers.Export.Materials.ConvertType(_material);
        _prop3d.MaterialAnalysisProperty = _material.AnalysisProperty;
        _prop3d.MaterialGradeProperty = _material.GradeProperty;
        IsReferencedById = false;
      }
    }
    public int MaterialId {
      get {
        return _prop3d.MaterialAnalysisProperty;
      }
      set {
        CloneApiObject();
        _prop3d.MaterialAnalysisProperty = value;
        _material.AnalysisProperty = _prop3d.MaterialAnalysisProperty;
        IsReferencedById = false;
      }
    }
    public string Name {
      get {
        return _prop3d.Name;
      }
      set {
        CloneApiObject();
        _prop3d.Name = value;
        IsReferencedById = false;
      }
    }
    public GsaProp3d() {
    }

    public GsaProp3d(int id) {
      _id = id;
      IsReferencedById = true;
    }

    public GsaProp3d(GsaMaterial material) {
      Material = material;
    }

    public GsaProp3d Duplicate(bool cloneApiElement = false) {
      var dup = new GsaProp3d {
        _prop3d = _prop3d,
        _id = _id,
        _material = _material.Duplicate(),
        _guid = new Guid(_guid.ToString()),
        IsReferencedById = IsReferencedById,
      };
      if (cloneApiElement)
        dup.CloneApiObject();
      return dup;
    }

    public override string ToString() {
      string type = Mappings.s_materialTypeMapping.FirstOrDefault(x => x.Value == Material.MaterialType).Key;
      string pa = (Id > 0) ? "PV" + Id + " " : "";
      return string.Join(" ", pa.Trim(), type.Trim()).Trim().Replace("  ", " ");
    }

    internal Prop3D ApiProp3d {
      get {
        return _prop3d;
      }
      set {
        _guid = Guid.NewGuid();
        _prop3d = value;
        _material = new GsaMaterial(this);
        IsReferencedById = false;
      }
    }
    internal bool IsReferencedById { get; set; } = false;
    internal GsaProp3d(IReadOnlyDictionary<int, Prop3D> pDict, int id, IReadOnlyDictionary<int, AnalysisMaterial> matDict) : this(id) {
      if (!pDict.ContainsKey(id))
        return;
      _prop3d = pDict[id];
      IsReferencedById = false;
      // material
      if (_prop3d.MaterialAnalysisProperty != 0 && matDict.ContainsKey(_prop3d.MaterialAnalysisProperty))
        _material.AnalysisMaterial = matDict[_prop3d.MaterialAnalysisProperty];
      _material = new GsaMaterial(this);
    }

    private Guid _guid = Guid.NewGuid();
    private int _id;
    private GsaMaterial _material = new GsaMaterial();
    private Prop3D _prop3d = new Prop3D();
    private void CloneApiObject() {
      var prop = new Prop3D {
        MaterialAnalysisProperty = _prop3d.MaterialAnalysisProperty,
        MaterialGradeProperty = _prop3d.MaterialGradeProperty,
        MaterialType = _prop3d.MaterialType,
        Name = _prop3d.Name.ToString(),
        AxisProperty = _prop3d.AxisProperty,
      };
      if ((Color)_prop3d.Colour != Color.FromArgb(0, 0, 0)) // workaround to handle that Color is non-nullable type
        prop.Colour = _prop3d.Colour;

      _prop3d = prop;
      _guid = Guid.NewGuid();
    }
  }
}
