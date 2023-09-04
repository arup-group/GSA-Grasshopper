using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using GsaAPI;
using GsaAPI.Materials;
using GsaGH.Helpers.GsaApi;

namespace GsaGH.Parameters {
  /// <summary>
  /// A 3D Property is used by <see cref="GsaElement3d"/> and <see cref="GsaMember3d"/> and simply contain information about <see cref="GsaMaterial"/>.
  /// <para>Refer to <see href="https://docs.oasys-software.com/structural/gsa/references/hidr-data-pr-3d.html">3D Element Properties</see> to read more.</para>
  /// </summary>
  public class GsaProperty3d : GsaProperty {
    public Prop3D ApiProp3d = new Prop3D();

    public GsaProperty3d() { }
    public GsaProperty3d(int id) {
      Id = id;
      IsReferencedById = true;
    }

    public GsaProperty3d(GsaMaterial material) {
      Material = material;
    }

    public override string ToString() {
      string type = Mappings.materialTypeMapping
       .FirstOrDefault(x => x.Value == Material.MaterialType).Key;
      string pa = (Id > 0) ? "PV" + Id + " " : string.Empty;
      return string.Join(" ", pa.Trim(), type.Trim()).Trim().Replace("  ", " ");
    }

    public override void DuplicateApiObject() {
      var prop = new Prop3D {
        MaterialAnalysisProperty = ApiProp3d.MaterialAnalysisProperty,
        MaterialGradeProperty = ApiProp3d.MaterialGradeProperty,
        MaterialType = ApiProp3d.MaterialType,
        Name = ApiProp3d.Name.ToString(),
        AxisProperty = ApiProp3d.AxisProperty,
      };
      // workaround to handle that Color is non-nullable type
      if ((Color)ApiProp3d.Colour != Color.FromArgb(0, 0, 0)) {
        prop.Colour = ApiProp3d.Colour;
      }

      ApiProp3d = prop;
    }

    public override IGsaObject Clone() {
      var dup = (GsaProperty3d)MemberwiseClone();
      dup.DuplicateApiObject();
      return dup;
    }
  }
}
