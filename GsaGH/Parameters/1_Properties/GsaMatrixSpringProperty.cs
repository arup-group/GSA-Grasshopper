using System.Collections.Generic;
using GsaAPI;

namespace GsaGH.Parameters {
  public class GsaMatrixSpringProperty : GsaSpringProperty {
    public GsaMatrixSpringProperty(GsaAxialSpringProperty other) : base(other) { }

    internal GsaMatrixSpringProperty(MatrixSpringProperty property) : base(property) { }

    internal GsaMatrixSpringProperty(KeyValuePair<int, MatrixSpringProperty> property) {
      Id = property.Key;
      ApiProperty = property.Value;
      IsReferencedById = false;
    }
  }
}
