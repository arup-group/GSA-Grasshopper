using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;

using GsaAPI;

using GsaGH.Helpers;
using GsaGH.Helpers.GsaApi;

using OasysUnits;

using Rhino.Geometry;

using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGH.Parameters {
  /// <summary>
  /// A 2D property is used by <see cref="GsaElement2d"/> and <see cref="GsaMember2d"/> and generally contains information about it's the Area Property's `Thickness` and <see cref="GsaMaterial"/>.
  /// <para>2D Properties can also be used to create LoadPanels, use the <see cref="Components.Create2dProperty"/> component and select `LoadPanel` from the dropdown list. </para>
  /// <para>Refer to <see href="https://docs.oasys-software.com/structural/gsa/references/hidr-data-pr-2d.html">2D Element Properties</see> to read more.</para>
  /// </summary>
  public class GsaProperty2d : Property {
    public Prop2D ApiProp2d { get; internal set; }
    public Plane LocalAxis { get; internal set; }

    public Length AdditionalOffsetZ {
      get => ApiProp2d == null ? Length.Zero
        : new Length(ApiProp2d.AdditionalOffsetZ, LengthUnit.Meter);
      set => ApiProp2d.AdditionalOffsetZ = value.As(LengthUnit.Meter);
    }

    public Length Thickness {
      get => ApiProp2d == null ? Length.Zero : ConvertDescriptionToLength(ApiProp2d.Description);
      set => ApiProp2d.Description = $"{value.Value}({Length.GetAbbreviation(value.Unit)})";
    }

    /// <summary>
    /// Empty constructor instantiating a new API object
    /// </summary>
    public GsaProperty2d() {
      ApiProp2d = new Prop2D();
    }

    /// <summary>
    /// Create a new instance with reference to an Id and no API object
    /// </summary>
    /// <param name="id"></param>
    public GsaProperty2d(int id) {
      Id = id;
      IsReferencedById = true;
    }

    /// <summary>
    /// Create new instance by casting from a Length corrosponding to the thickness
    /// </summary>
    /// <param name="thickness"></param>
    public GsaProperty2d(Length thickness) {
      ApiProp2d = new Prop2D();
      Thickness = thickness;
    }

    /// <summary>
    /// Create a duplicate instance from another instance
    /// </summary>
    /// <param name="other"></param>
    public GsaProperty2d(GsaProperty2d other) {
      Id = other.Id;
      IsReferencedById = other.IsReferencedById;
      if (!IsReferencedById) {
        ApiProp2d = other.DuplicateApiObject();
        Material = other.Material;
        if (other.LocalAxis != null) {
          LocalAxis = other.LocalAxis;
        }
      }
    }

    /// <summary>
    /// Create a new instance from an API object from an existing model
    /// </summary>
    /// <param name="prop2d"></param>
    internal GsaProperty2d(KeyValuePair<int, Prop2D> prop2d) {
      Id = prop2d.Key;
      ApiProp2d = prop2d.Value;
      IsReferencedById = false;
    }

    public Prop2D DuplicateApiObject() {
      var prop = new Prop2D {
        MaterialAnalysisProperty = ApiProp2d.MaterialAnalysisProperty,
        MaterialGradeProperty = ApiProp2d.MaterialGradeProperty,
        MaterialType = ApiProp2d.MaterialType,
        Name = ApiProp2d.Name,
        Description = ApiProp2d.Description,
        Type = ApiProp2d.Type,
        AxisProperty = ApiProp2d.AxisProperty,
        ReferenceSurface = ApiProp2d.ReferenceSurface,
        AdditionalOffsetZ = ApiProp2d.AdditionalOffsetZ,
      };

      if (ApiProp2d.Type == Property2D_Type.LOAD) {
        prop.SupportType = ApiProp2d.SupportType;
        if (ApiProp2d.SupportType != SupportType.Auto) {
          prop.ReferenceEdge = ApiProp2d.ReferenceEdge;
        }
      }

      prop.PropertyModifier.AdditionalMass = ApiProp2d.PropertyModifier.AdditionalMass;
      prop.PropertyModifier.Bending = ApiProp2d.PropertyModifier.Bending;
      prop.PropertyModifier.InPlane = ApiProp2d.PropertyModifier.InPlane;
      prop.PropertyModifier.Shear = ApiProp2d.PropertyModifier.Shear;
      prop.PropertyModifier.Volume = ApiProp2d.PropertyModifier.Volume;

      // workaround to handle that System.Drawing.Color is non-nullable type
      if ((Color)ApiProp2d.Colour != Color.FromArgb(0, 0, 0)) {
        prop.Colour = ApiProp2d.Colour;
      }

      return prop;
    }

    public override string ToString() {
      string pa = (Id > 0) ? "PA" + Id : string.Empty;
      if (IsReferencedById) {
        return (Id > 0) ? $"{pa} (referenced)" : string.Empty; ;
      }

      string type = Mappings._prop2dTypeMapping.FirstOrDefault(x => x.Value == ApiProp2d.Type).Key;
      string desc = ApiProp2d.Description.Replace("(", string.Empty).Replace(")", string.Empty);
      if (ApiProp2d.Type != Property2D_Type.LOAD) {
        string mat = Material != null ? MaterialType
        : ApiProp2d.MaterialType.ToString().ToPascalCase();
        return string.Join(" ", pa, type, desc, mat).TrimSpaces();
      }
      string supportType = ApiProp2d.SupportType.ToString().ToSentenceCase();
      string referenceEdge =
        ApiProp2d.SupportType != SupportType.Auto && ApiProp2d.SupportType != SupportType.AllEdges
        ? $"RefEdge:{ApiProp2d.ReferenceEdge}" : string.Empty;
      return string.Join(" ", pa, type, supportType, referenceEdge, desc).TrimSpaces();
    }

    internal static Property2D_Type PropTypeFromString(string type) {
      try {
        return Mappings.GetProperty2D_Type(type);
      } catch (ArgumentException) {
        type = type.TrimSpaces().Replace(" ", "_").ToUpper();
        type = type.Replace("PLANE", "PL");
        type = type.Replace("NUMBER", "NUM");
        type = type.Replace("AXIS_SYMMETRIC", "AXISYMMETRIC");
        type = type.Replace("LOAD_PANEL", "LOAD");
        return (Property2D_Type)Enum.Parse(typeof(Property2D_Type), type);
      }
    }

    internal void SetPlaneFromAxis(Axis axis) {
      LocalAxis = new Plane(new Point3d(axis.Origin.X, axis.Origin.Y, axis.Origin.Z),
            new Vector3d(axis.XVector.X, axis.XVector.Y, axis.XVector.Z),
            new Vector3d(axis.XYPlane.X, axis.XYPlane.Y, axis.XYPlane.Z));
    }

    internal Axis GetAxisFromPlane(LengthUnit unit) {
      var axis = new Axis();
      axis.Origin.X = (unit == LengthUnit.Meter) ? LocalAxis.OriginX :
        new Length(LocalAxis.OriginX, unit).Meters;
      axis.Origin.Y = (unit == LengthUnit.Meter) ? LocalAxis.OriginY :
        new Length(LocalAxis.OriginY, unit).Meters;
      axis.Origin.Z = (unit == LengthUnit.Meter) ? LocalAxis.OriginZ :
        new Length(LocalAxis.OriginZ, unit).Meters;

      axis.XVector.X = LocalAxis.XAxis.X;
      axis.XVector.Y = LocalAxis.XAxis.Y;
      axis.XVector.Z = LocalAxis.XAxis.Z;
      axis.XYPlane.X = LocalAxis.YAxis.X;
      axis.XYPlane.Y = LocalAxis.YAxis.Y;
      axis.XYPlane.Z = LocalAxis.YAxis.Z;

      return axis;
    }

    private static Length ConvertDescriptionToLength(string description) {
      if (description.Length == 0) {
        return Length.Zero;
      }

      if (description.Last() == ')') {
        // thickness could be written as "30.33(in)"
        string unitAbbreviation = description.Split('(', ')')[1];
        LengthUnit unit = OasysUnitsSetup.Default.UnitParser.Parse<LengthUnit>(unitAbbreviation);

        double val = double.Parse(description.Split('(')[0],
          CultureInfo.InvariantCulture);

        return new Length(val, unit);
      } else {
        return new Length(double.Parse(description, CultureInfo.InvariantCulture),
          LengthUnit.Millimeter);
      }
    }
  }
}
