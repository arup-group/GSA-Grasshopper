using System.Collections.Generic;
using System.Drawing;

using GsaAPI;

using GsaGH.Helpers;

using OasysUnits;

using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGH.Parameters {
  /// <summary>
  /// A Section is used by <see cref="GsaElement1d"/> and <see cref="GsaMember1d"/> and generally contains information about it's `Profile` and <see cref="GsaMaterial"/>.
  /// <para>Use the <see cref="Components.CreateProfile"/> component to create Catalogue and custom profiles.</para>
  /// <para>Refer to <see href="https://docs.oasys-software.com/structural/gsa/references/hidr-data-sect-lib.html">Sections</see> to read more.</para>
  /// </summary>
  public class GsaSection : Property {
    public Section ApiSection { get; internal set; }
    public GsaSectionModifier Modifier { get; set; }
    public GsaSectionProperties SectionProperties
      => new GsaSectionProperties(ApiSection.Properties());
    public Length AdditionalOffsetY {
      get => ApiSection == null ? Length.Zero
        : new Length(ApiSection.AdditionalOffsetY, LengthUnit.Meter);
      set => ApiSection.AdditionalOffsetY = value.Meters;
    }
    public Length AdditionalOffsetZ {
      get => ApiSection == null ? Length.Zero
        : new Length(ApiSection.AdditionalOffsetZ, LengthUnit.Meter);
      set => ApiSection.AdditionalOffsetZ = value.Meters;
    }

    /// <summary>
    /// Empty constructor instantiating a new API object
    /// </summary>
    public GsaSection() {
      ApiSection = new Section();
    }

    /// <summary>
    /// Create a new instance with reference to an Id and no API object
    /// </summary>
    /// <param name="id"></param>
    public GsaSection(int id) {
      Id = id;
      IsReferencedById = true;
    }

    /// <summary>
    /// Create new instance by casting from a Profile string
    /// </summary>
    /// <param name="profile"></param>
    public GsaSection(string profile) {
      ApiSection = new Section {
        Profile = profile
      };
    }

    /// <summary>
    /// Create a duplicate instance from another instance
    /// </summary>
    /// <param name="other"></param>
    public GsaSection(GsaSection other) {
      Id = other.Id;
      IsReferencedById = other.IsReferencedById;
      if (!IsReferencedById) {
        ApiSection = other.DuplicateApiObject();
        Material = other.Material;
        Modifier = other.Modifier;
      }
    }

    /// <summary>
    /// Create a new instance from an API object from an existing model
    /// </summary>
    /// <param name="section"></param>
    internal GsaSection(KeyValuePair<int, Section> section) {
      Id = section.Key;
      ApiSection = section.Value;
      IsReferencedById = false;
    }

    public Section DuplicateApiObject() {
      var sec = new Section() {
        MaterialAnalysisProperty = ApiSection.MaterialAnalysisProperty,
        MaterialGradeProperty = ApiSection.MaterialGradeProperty,
        MaterialType = ApiSection.MaterialType,
        Name = ApiSection.Name.ToString(),
        BasicOffset = ApiSection.BasicOffset,
        AdditionalOffsetY = ApiSection.AdditionalOffsetY,
        AdditionalOffsetZ = ApiSection.AdditionalOffsetZ,
        Pool = ApiSection.Pool,
        Profile = ApiSection.Profile,
      };
      // workaround to handle that Color is non-nullable type
      if ((Color)ApiSection.Colour != Color.FromArgb(0, 0, 0)) {
        sec.Colour = ApiSection.Colour;
      }

      return sec;
    }

    public override string ToString() {
      string pb = Id > 0 ? $"PB{Id}" : string.Empty;
      if (IsReferencedById) {
        return (Id > 0) ? $"{pb} (referenced)" : string.Empty;
      }

      string prof = ApiSection.Profile.Replace("%", " ");
      string mat = Material != null ? MaterialType
        : ApiSection.MaterialType.ToString().ToPascalCase();
      string mod = (Modifier != null && Modifier.IsModified) ? "modified" : string.Empty;
      return string.Join(" ", pb, prof, mat, mod).TrimSpaces();
    }

    internal static bool IsValidProfile(string profile) {
      var test = new Section {
        Profile = profile,
      };
      return test.Properties().Area != 0;
    }
  }
}
