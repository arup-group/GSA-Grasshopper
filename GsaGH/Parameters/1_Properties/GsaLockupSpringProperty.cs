using System.Collections.Generic;
using GsaAPI;

namespace GsaGH.Parameters {
  public class GsaLockupSpringProperty : GsaSpringProperty {
    public GsaLockupSpringProperty(GsaConnectorSpringProperty other) : base(other) { }

    internal GsaLockupSpringProperty(LockupSpringProperty property) : base(property) { }

    internal GsaLockupSpringProperty(KeyValuePair<int, LockupSpringProperty> property) {
      Id = property.Key;
      ApiProperty = property.Value;
      IsReferencedById = false;
    }
  }
}
