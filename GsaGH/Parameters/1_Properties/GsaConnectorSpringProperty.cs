using System.Collections.Generic;
using GsaAPI;

namespace GsaGH.Parameters {
  public class GsaConnectorSpringProperty : GsaSpringProperty {
    public GsaConnectorSpringProperty(GsaConnectorSpringProperty other) : base(other) { }

    internal GsaConnectorSpringProperty(ConnectorSpringProperty property) : base(property) { }

    internal GsaConnectorSpringProperty(KeyValuePair<int, ConnectorSpringProperty> property) {
      Id = property.Key;
      ApiProperty = property.Value;
      IsReferencedById = false;
    }
  }
}
