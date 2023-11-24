using System.Collections.Generic;
using GsaAPI;

namespace GsaGH.Parameters {
  public class GsaAxialSpringProperty : GsaSpringProperty {
    public GsaAxialSpringProperty(GsaAxialSpringProperty other) : base(other) { }

    internal GsaAxialSpringProperty(AxialSpringProperty property) : base(property) { }

    internal GsaAxialSpringProperty(KeyValuePair<int, AxialSpringProperty> property) {
      Id = property.Key;
      ApiProperty = property.Value;
      IsReferencedById = false;
    }
  }
}
