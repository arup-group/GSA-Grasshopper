using System;

using GsaAPI.Materials;

namespace GsaGH.Parameters {
  internal class GsaReferencedMaterial : GsaMaterial {
    public override AnalysisMaterial AnalysisMaterial { get => null; set => throw new ArgumentException("Can not set analysis material for a material that is referenced by ID"); }
    public override string Name {
      get => "Referenced by ID";
      set => AnalysisMaterial.Name = value;
    }

    internal GsaReferencedMaterial(int id, MatType type) {
      Id = id;
      MaterialType = type;
    }

    public override string ToString() {
      return $"{MaterialType} ID:{Id} (referenced)";
    }
  }
}
