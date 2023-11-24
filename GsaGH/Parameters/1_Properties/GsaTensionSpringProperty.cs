using System.Collections.Generic;
using GsaAPI;

namespace GsaGH.Parameters {
  public class GsaTensionSpringProperty : GsaSpringProperty {
    public GsaTensionSpringProperty(GsaTensionSpringProperty other) : base(other) { }

    internal GsaTensionSpringProperty(TensionSpringProperty property) : base(property) { }

    internal GsaTensionSpringProperty(KeyValuePair<int, TensionSpringProperty> property) {
      Id = property.Key;
      ApiProperty = property.Value;
      IsReferencedById = false;
    }
  }
}
