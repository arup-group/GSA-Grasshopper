using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using GsaAPI;
using GsaGH.Components;
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
  public class GsaSpringProperty {
    public int Id { get; set; } = 0;
    public bool IsReferencedById { get; set; } = false;
    public SpringProperty ApiProperty { get; internal set; }

    public GsaSpringProperty(SpringProperty property) {
      ApiProperty = property;
    }

    /// <summary>
    /// Create a new instance with reference to an Id and no API object
    /// </summary>
    /// <param name="id"></param>
    public GsaSpringProperty(int id) {
      Id = id;
      IsReferencedById = true;
    }

    /// <summary>
    /// Create a duplicate instance from another instance
    /// </summary>
    /// <param name="other"></param>
    public GsaSpringProperty(GsaSpringProperty other) {
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
    internal GsaSpringProperty(KeyValuePair<int, Prop2D> prop2d) {
      Id = prop2d.Key;
      ApiProp2d = prop2d.Value;
      IsReferencedById = false;
    }

    public SpringProperty DuplicateApiObject() {
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
      //if (IsReferencedById) {
      //  return (Id > 0) ? $"{pa} (referenced)" : string.Empty; ;
      //}

      //string type = Mappings.prop2dTypeMapping.FirstOrDefault(x => x.Value == ApiProp2d.Type).Key;
      //string desc = ApiProp2d.Description.Replace("(", string.Empty).Replace(")", string.Empty);
      //if (ApiProp2d.Type != Property2D_Type.LOAD) {
      //  string mat = Material != null ? MaterialType
      //  : ApiProp2d.MaterialType.ToString().ToPascalCase();
      //  return string.Join(" ", pa, type, desc, mat).TrimSpaces();
      //}
      //string supportType = ApiProp2d.SupportType.ToString().ToSentenceCase();
      //string referenceEdge =
      //  ApiProp2d.SupportType != SupportType.Auto && ApiProp2d.SupportType != SupportType.AllEdges
      //  ? $"RefEdge:{ApiProp2d.ReferenceEdge}" : string.Empty;
      //return string.Join(" ", pa, type, supportType, referenceEdge, desc).TrimSpaces();

      return pa;
    }
  }
}
