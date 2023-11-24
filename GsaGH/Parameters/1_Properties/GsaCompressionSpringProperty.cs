using System.Collections.Generic;
using GsaAPI;

namespace GsaGH.Parameters {
  public class GsaCompressionSpringProperty : GsaSpringProperty {
    public GsaCompressionSpringProperty(GsaCompressionSpringProperty other) : base(other) { }

    internal GsaCompressionSpringProperty(CompressionSpringProperty property) : base(property) { }

    internal GsaCompressionSpringProperty(KeyValuePair<int, CompressionSpringProperty> property) {
      Id = property.Key;
      ApiProperty = property.Value;
      IsReferencedById = false;
    }
  }
}
