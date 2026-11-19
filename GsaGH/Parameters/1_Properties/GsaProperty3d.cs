using System.Collections.Generic;
using System.Drawing;

using GsaAPI;

using GsaGH.Helpers;

namespace GsaGH.Parameters {
  /// <summary>
  /// A 3D Property is used by <see cref="GsaElement3d"/> and <see cref="GsaMember3d"/> and simply contains information about <see cref="GsaMaterial"/>.
  /// <para>Refer to <see href="https://docs.oasys-software.com/structural/gsa/references/hidr-data-pr-3d.html">3D Element Properties</see> to read more.</para>
  /// </summary>
  public class GsaProperty3d : Property {
    public Prop3D ApiProp3d { get; internal set; }

    /// <summary>
    /// Empty constructor instantiating a new API object
    /// </summary>
    public GsaProperty3d() {
      ApiProp3d = new Prop3D();
    }

    /// <summary>
    /// Create a new instance with reference to an Id and no API object
    /// </summary>
    /// <param name="id"></param>
    public GsaProperty3d(int id) {
      Id = id;
      IsReferencedById = true;
    }

    /// <summary>
    /// Create new instance by casting from a Material
    /// </summary>
    /// <param name="material"></param>
    public GsaProperty3d(GsaMaterial material) {
      ApiProp3d = new Prop3D();
      Material = material;
    }

    /// <summary>
    /// Create a duplicate instance from another instance
    /// </summary>
    /// <param name="other"></param>
    public GsaProperty3d(GsaProperty3d other) {
      Id = other.Id;
      IsReferencedById = other.IsReferencedById;
      if (!IsReferencedById) {
        ApiProp3d = other.DuplicateApiObject();
        Material = other.Material;
      }
    }

    /// <summary>
    /// Create a new instance from an API object from an existing model
    /// </summary>
    /// <param name="prop3d"></param>
    internal GsaProperty3d(KeyValuePair<int, Prop3D> prop3d) {
      Id = prop3d.Key;
      ApiProp3d = DuplicateFrom(prop3d.Value);
      IsReferencedById = false;
    }

    public Prop3D DuplicateApiObject() {
      return DuplicateFrom(ApiProp3d);
    }

    private static Prop3D DuplicateFrom(Prop3D source) {
      var prop = new Prop3D {
        MaterialAnalysisProperty = source.MaterialAnalysisProperty,
        MaterialGradeProperty = source.MaterialGradeProperty,
        MaterialType = source.MaterialType,
        Name = source.Name.ToString(),
        AxisProperty = source.AxisProperty,
      };
      // workaround to handle that Color is non-nullable type
      if ((Color)source.Colour != Color.FromArgb(0, 0, 0)) {
        prop.Colour = source.Colour;
      }

      return prop;
    }

    public override string ToString() {
      string pv = (Id > 0) ? $"PV{Id}" : string.Empty;
      if (IsReferencedById) {
        return (Id > 0) ? $"{pv} (referenced)" : string.Empty; ;
      }

      string mat = Material != null ? MaterialType
        : ApiProp3d.MaterialType.ToString().ToPascalCase();
      return string.Join(" ", pv, mat).TrimSpaces();
    }
  }
}
