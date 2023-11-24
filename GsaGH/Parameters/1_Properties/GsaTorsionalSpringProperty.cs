using System.Collections.Generic;
using GsaAPI;

namespace GsaGH.Parameters {
  public class GsaTorsionalSpringProperty : GsaSpringProperty {
    public GsaTorsionalSpringProperty(GsaTorsionalSpringProperty other) : base(other) { }

    internal GsaTorsionalSpringProperty(TorsionalSpringProperty property) : base(property) { }

    internal GsaTorsionalSpringProperty(KeyValuePair<int, TorsionalSpringProperty> property) {
      Id = property.Key;
      ApiProperty = property.Value;
      IsReferencedById = false;
    }
  }
}
