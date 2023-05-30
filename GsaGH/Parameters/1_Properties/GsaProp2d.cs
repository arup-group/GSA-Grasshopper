using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using GsaAPI;
using GsaGH.Helpers.Export;
using GsaGH.Helpers.GsaApi;
using OasysUnits;
using OasysUnits.Units;
using Rhino.Geometry;

namespace GsaGH.Parameters {
  /// <summary>
  ///   Prop2d class, this class defines the basic properties and methods for any <see cref="GsaAPI.Prop2D" />
  /// </summary>
  public class GsaProp2d {
    public int AxisProperty {
      get => _prop2d.AxisProperty;
      set {
        CloneApiObject();
        _prop2d.AxisProperty = value;
        IsReferencedById = false;
      }
    }
    public Color Colour {
      get => (Color)_prop2d.Colour;
      set {
        CloneApiObject();
        _prop2d.Colour = value;
        IsReferencedById = false;
      }
    }
    public string Description {
      get => _prop2d.Description.Replace("%", " ");
      set {
        CloneApiObject();
        _prop2d.Description = value;
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
    public Plane LocalAxis {
      get => _localAxis;
      set {
        _localAxis = value;
        CloneApiObject();
        _prop2d.AxisProperty = -2;
        IsReferencedById = false;
      }
    }
    public GsaMaterial Material {
      get => _material;
      set {
        _material = value;
        if (_prop2d == null) {
          _prop2d = new Prop2D();
        } else {
          CloneApiObject();
        }

        _prop2d.MaterialType = Materials.ConvertType(_material);
        _prop2d.MaterialAnalysisProperty = _material.AnalysisProperty;
        _prop2d.MaterialGradeProperty = _material.GradeProperty;
        IsReferencedById = false;
      }
    }
    public int MaterialId {
      get => _prop2d.MaterialAnalysisProperty;
      set {
        CloneApiObject();
        IsReferencedById = false;
        _prop2d.MaterialAnalysisProperty = value;
        _material.AnalysisProperty = _prop2d.MaterialAnalysisProperty;
      }
    }
    public string Name {
      get => _prop2d.Name;
      set {
        CloneApiObject();
        _prop2d.Name = value;
        IsReferencedById = false;
      }
    }
    public int ReferenceEdge {
      get => _prop2d.ReferenceEdge;
      set {
        CloneApiObject();
        _prop2d.ReferenceEdge = value;
        IsReferencedById = false;
      }
    }
    public ReferenceSurface ReferenceSurface {
      get => _prop2d.ReferenceSurface;
      set {
        CloneApiObject();
        _prop2d.ReferenceSurface = value;
      }
    }
    public Length AdditionalOffsetZ {
      get => new Length(_prop2d.AdditionalOffsetZ, LengthUnit.Meter);
      set {
        CloneApiObject();
        _prop2d.AdditionalOffsetZ = value.As(LengthUnit.Meter);
      }
    }
    public SupportType SupportType {
      get => _prop2d.SupportType;
      set {
        CloneApiObject();
        _prop2d.SupportType = value;
        IsReferencedById = false;
      }
    }
    public Length Thickness {
      get {
        if (_prop2d.Description.Length == 0) {
          return Length.Zero;
        }

        if (_prop2d.Description.Last() == ')') {
          // thickness could be written as "30.33(in)"
          string unitAbbreviation = _prop2d.Description.Split('(', ')')[1];
          LengthUnit unit = UnitParser.Default.Parse<LengthUnit>(unitAbbreviation);

          double val = double.Parse(_prop2d.Description.Split('(')[0],
            CultureInfo.InvariantCulture);

          return new Length(val, unit);
        } else {
          return new Length(double.Parse(_prop2d.Description, CultureInfo.InvariantCulture),
            LengthUnit.Millimeter);
        }
      }
      set {
        CloneApiObject();
        IQuantity length = new Length(0, value.Unit);
        string unitAbbreviation = string.Concat(length.ToString().Where(char.IsLetter));
        _prop2d.Description = value.Value + "(" + unitAbbreviation + ")";
        IsReferencedById = false;
      }
    }
    public Property2D_Type Type {
      get => _prop2d.Type;
      set {
        CloneApiObject();
        _prop2d.Type = value;
        IsReferencedById = false;
      }
    }
    internal Prop2D ApiProp2d {
      get => _prop2d;
      set {
        _guid = Guid.NewGuid();
        _prop2d = value;
        _material = new GsaMaterial(this);
        IsReferencedById = false;
      }
    }
    internal bool IsReferencedById { get; set; } = false;
    private Guid _guid = Guid.NewGuid();
    private int _id;
    private Plane _localAxis = Plane.Unset;
    private GsaMaterial _material = new GsaMaterial();
    private Prop2D _prop2d = new Prop2D();

    public GsaProp2d() { }

    public GsaProp2d(int id) {
      _id = id;
      IsReferencedById = true;
    }

    public GsaProp2d(Length thickness, int id = 0) {
      Thickness = thickness;
      _id = id;
    }

    internal GsaProp2d(
      IReadOnlyDictionary<int, Prop2D> pDict, int id,
      IReadOnlyDictionary<int, AnalysisMaterial> matDict, IReadOnlyDictionary<int, Axis> axDict,
      LengthUnit unit) : this(id) {
      if (!pDict.ContainsKey(id)) {
        return;
      }

      _prop2d = pDict[id];
      IsReferencedById = false;
      // material
      if (_prop2d.MaterialAnalysisProperty != 0
        && matDict.ContainsKey(_prop2d.MaterialAnalysisProperty)) {
        _material.AnalysisMaterial = matDict[_prop2d.MaterialAnalysisProperty];
      }

      if (_prop2d.AxisProperty > 0) {
        if (axDict != null && axDict.ContainsKey(_prop2d.AxisProperty)) {
          Axis ax = axDict[_prop2d.AxisProperty];
          LocalAxis = new Plane(
            new Point3d(new Length(ax.Origin.X, LengthUnit.Meter).As(unit),
              new Length(ax.Origin.Y, LengthUnit.Meter).As(unit),
              new Length(ax.Origin.Z, LengthUnit.Meter).As(unit)),
            new Vector3d(ax.XVector.X, ax.XVector.Y, ax.XVector.Z),
            new Vector3d(ax.XYPlane.X, ax.XYPlane.Y, ax.XYPlane.Z));
        }
      }

      _material = new GsaMaterial(this);
    }

    public GsaProp2d Duplicate(bool cloneApiElement = false) {
      var dup = new GsaProp2d {
        _prop2d = _prop2d,
        _id = _id,
        _material = _material.Duplicate(),
        _guid = new Guid(_guid.ToString()),
        _localAxis = new Plane(_localAxis),
        IsReferencedById = IsReferencedById,
      };
      if (cloneApiElement) {
        dup.CloneApiObject();
      }

      return dup;
    }

    public override string ToString() {
      string type = Mappings.prop2dTypeMapping.FirstOrDefault(x => x.Value == _prop2d.Type).Key
        + " ";
      string desc = Description.Replace("(", string.Empty).Replace(")", string.Empty) + " ";
      string mat = Type != Property2D_Type.LOAD ?
        Mappings.materialTypeMapping.FirstOrDefault(x => x.Value == Material.MaterialType).Key
        + " " : string.Empty;
      string pa = (Id > 0) ? "PA" + Id + " " : string.Empty;
      string supportType = Type == Property2D_Type.LOAD ? $"{SupportType}" : string.Empty;
      string referenceEdge
        = Type == Property2D_Type.LOAD && SupportType != SupportType.Auto
        && SupportType != SupportType.AllEdges ? $"RefEdge:{ReferenceEdge}" : string.Empty;
      return string
       .Join(" ", pa.Trim(), type.Trim(), supportType.Trim(), referenceEdge.Trim(), desc.Trim(),
          mat.Trim()).Trim().Replace("  ", " ");
    }

    internal static Property2D_Type PropTypeFromString(string type) {
      try {
        return Mappings.GetProperty2D_Type(type);
      } catch (ArgumentException) {
        type = type.Trim().Replace(" ", "_").ToUpper();
        type = type.Replace("PLANE", "PL");
        type = type.Replace("NUMBER", "NUM");
        type = type.Replace("AXIS_SYMMETRIC", "AXISYMMETRIC");
        type = type.Replace("LOAD_PANEL", "LOAD");
        return (Property2D_Type)Enum.Parse(typeof(Property2D_Type), type);
      }
    }

    private void CloneApiObject() {
      _prop2d = GetApiObject();
      _guid = Guid.NewGuid();
    }

    private Prop2D GetApiObject() {
      if (_prop2d == null) {
        return new Prop2D();
      }

      var prop = new Prop2D {
        MaterialAnalysisProperty = _prop2d.MaterialAnalysisProperty,
        MaterialGradeProperty = _prop2d.MaterialGradeProperty,
        MaterialType = _prop2d.MaterialType,
        Name = _prop2d.Name,
        Description = _prop2d.Description,
        Type = _prop2d.Type,
        AxisProperty = _prop2d.AxisProperty,
        ReferenceSurface = _prop2d.ReferenceSurface,
        AdditionalOffsetZ = _prop2d.AdditionalOffsetZ,
      };
      if (_prop2d.Type == Property2D_Type.LOAD) {
        prop.SupportType = _prop2d.SupportType;
        if (_prop2d.SupportType != SupportType.Auto) {
          prop.ReferenceEdge = _prop2d.ReferenceEdge;
        }
      }
      // workaround to handle that System.Drawing.Color is non-nullable type
      if ((Color)_prop2d.Colour != Color.FromArgb(0, 0, 0)) {
        prop.Colour = _prop2d.Colour;
      }

      return prop;
    }
  }
}
