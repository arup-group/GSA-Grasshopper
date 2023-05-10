﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using GsaAPI;
using GsaGH.Helpers.Export;
using GsaGH.Helpers.GH;
using GsaGH.Helpers.GsaApi;

namespace GsaGH.Parameters {
  /// <summary>
  ///   Prop2d class, this class defines the basic properties and methods for any <see cref="GsaAPI.Prop3D" />
  /// </summary>
  public class GsaProp3d {
    public int AxisProperty {
      get => _prop3d.AxisProperty;
      set {
        CloneApiObject();
        value = Math.Min(1, value);
        value = Math.Max(0, value);
        _prop3d.AxisProperty = value * -1;
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
    public IGsaMaterial Material {
      get => _material;
      set {
        _material = value;
        if (_prop3d == null) {
          _prop3d = new Prop3D();
        } else {
          CloneApiObject();
        }

        _prop3d.MaterialType = (MaterialType)Enum.Parse(typeof(MaterialType),
          _material.Type.ToString(), true);
        _prop3d.MaterialAnalysisProperty = 0;
        _prop3d.MaterialGradeProperty = 0;
        IsReferencedById = false;
      }
    }
    public int MaterialId {
      get => _prop3d.MaterialAnalysisProperty;
      set {
        CloneApiObject();
        _prop3d.MaterialAnalysisProperty = value;
        _material.Id = _prop3d.MaterialAnalysisProperty;
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
        _material = null;
        IsReferencedById = false;
      }
    }
    internal bool IsReferencedById { get; set; } = false;
    private Guid _guid = Guid.NewGuid();
    private int _id;
    private IGsaMaterial _material;
    private Prop3D _prop3d = new Prop3D();

    public GsaProp3d() { }

    public GsaProp3d(int id) {
      _id = id;
      IsReferencedById = true;
    }

    public GsaProp3d(IGsaMaterial material) {
      Material = material;
    }

    internal GsaProp3d(
      IReadOnlyDictionary<int, Prop3D> pDict, int id,
      Helpers.Import.Materials matDict) : this(id) {
      if (!pDict.ContainsKey(id)) {
        return;
      }

      _prop3d = pDict[id];
      IsReferencedById = false;

      _material = matDict.GetMaterial(this);
    }

    public GsaProp3d Duplicate(bool cloneApiElement = false) {
      var dup = new GsaProp3d {
        _prop3d = _prop3d,
        _id = _id,
        _material = _material.Duplicate(),
        _guid = new Guid(_guid.ToString()),
        IsReferencedById = IsReferencedById,
      };
      if (cloneApiElement) {
        dup.CloneApiObject();
      }

      return dup;
    }

    public override string ToString() {
      string type = Material.Type.ToString().ToPascalCase();
      string pa = (Id > 0) ? "PV" + Id + " " : "";
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
