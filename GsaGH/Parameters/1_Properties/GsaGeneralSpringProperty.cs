using System.Collections.Generic;
using GsaAPI;

namespace GsaGH.Parameters {
  public class GsaGeneralSpringProperty : GsaSpringProperty {
    public GsaGeneralSpringProperty(GsaGeneralSpringProperty other) : base(other) { }

    internal GsaGeneralSpringProperty(GeneralSpringProperty property) : base(property) { }

    internal GsaGeneralSpringProperty(KeyValuePair<int, GeneralSpringProperty> property) {
      Id = property.Key;
      ApiProperty = property.Value;
      IsReferencedById = false;
    }
  }
}
