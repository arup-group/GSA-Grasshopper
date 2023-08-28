using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using GsaAPI;
using GsaAPI.Materials;
using GsaGH.Helpers.Export;
using GsaGH.Helpers.GsaApi;

namespace GsaGH.Parameters {
  /// <summary>
  /// A 3D Property is used by <see cref="GsaElement3d"/> and <see cref="GsaMember3d"/> and simply contain information about <see cref="GsaMaterial"/>.
  /// <para>Refer to <see href="https://docs.oasys-software.com/structural/gsa/references/hidr-data-pr-3d.html">3D Element Properties</see> to read more.</para>
  /// </summary>
  public class GsaProperty3d {
    public int AxisProperty {
      get => _prop3d.AxisProperty;
      set {
        CloneApiObject();
        _prop3d.AxisProperty = value;
        IsReferencedById = false;
      }
    }
    public Color Colour {
      get => (Color)_prop3d.Colour;
      set {
        CloneApiObject();
        _prop3d.Colour = value;
        IsReferencedById = false;
      }
    }
    public Guid Guid => _guid;
    public int Id {
      get => _id;
      set {
        _guid = Guid.NewGuid();
        _id = value;
      }
    }
    public GsaMaterial Material {
      get => _material;
      set {
        _material = value;
        if (_prop3d == null) {
          _prop3d = new Prop3D();
        } else {
          CloneApiObject();
        }

        _prop3d.MaterialType = Materials.GetMaterialType(_material);
        if (_material.IsCustom) {
          _prop3d.MaterialAnalysisProperty = _material.Id;
        } else {
          _prop3d.MaterialGradeProperty = _material.Id;
        }
        IsReferencedById = false;
      }
    }
    public string Name {
      get => _prop3d.Name;
      set {
        CloneApiObject();
        _prop3d.Name = value;
        IsReferencedById = false;
      }
    }
    internal Prop3D ApiProp3d {
      get => _prop3d;
      set {
        _guid = Guid.NewGuid();
        _prop3d = value;
        _material = Material.Duplicate();
        IsReferencedById = false;
      }
    }
    internal bool IsReferencedById { get; set; } = false;
    private Guid _guid = Guid.NewGuid();
    private int _id;
    private GsaMaterial _material = new GsaMaterial();
    private Prop3D _prop3d = new Prop3D();

    public GsaProperty3d() { }

    public GsaProperty3d(int id) {
      _id = id;
      IsReferencedById = true;
    }

    public GsaProperty3d(GsaMaterial material) {
      Material = material;
    }

    internal GsaProperty3d(
      IReadOnlyDictionary<int, Prop3D> pDict, int id,
      IReadOnlyDictionary<int, AnalysisMaterial> matDict) : this(id) {
      if (!pDict.ContainsKey(id)) {
        return;
      }

      _prop3d = pDict[id];
      IsReferencedById = false;
      // material
      if (_prop3d.MaterialAnalysisProperty != 0
        && matDict.ContainsKey(_prop3d.MaterialAnalysisProperty)) {
        _material.AnalysisMaterial = matDict[_prop3d.MaterialAnalysisProperty];
      }

      _material = Material.Duplicate();
    }

    public GsaProperty3d Clone() {
      var dup = new GsaProperty3d {
        _prop3d = _prop3d,
        _id = _id,
        _material = _material.Duplicate(),
        IsReferencedById = IsReferencedById,
      };
      dup.CloneApiObject();
      return dup;
    }

    public GsaProperty3d Duplicate() {
      return this;
    }

    public override string ToString() {
      string type = Mappings.materialTypeMapping
       .FirstOrDefault(x => x.Value == Material.MaterialType).Key;
      string pa = (Id > 0) ? "PV" + Id + " " : string.Empty;
      return string.Join(" ", pa.Trim(), type.Trim()).Trim().Replace("  ", " ");
    }

    private void CloneApiObject() {
      var prop = new Prop3D {
        MaterialAnalysisProperty = _prop3d.MaterialAnalysisProperty,
        MaterialGradeProperty = _prop3d.MaterialGradeProperty,
        MaterialType = _prop3d.MaterialType,
        Name = _prop3d.Name.ToString(),
        AxisProperty = _prop3d.AxisProperty,
      };
      if ((Color)_prop3d.Colour
        != Color.FromArgb(0, 0, 0)) // workaround to handle that Color is non-nullable type
      {
        prop.Colour = _prop3d.Colour;
      }

      _prop3d = prop;
      _guid = Guid.NewGuid();
    }
  }
}
