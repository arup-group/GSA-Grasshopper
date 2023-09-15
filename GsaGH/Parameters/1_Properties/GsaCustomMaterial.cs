using GsaAPI.Materials;

namespace GsaGH.Parameters {
  public class GsaCustomMaterial : GsaMaterial {
    public override AnalysisMaterial AnalysisMaterial { get; set; }
    public override string Name {
      get => AnalysisMaterial.Name;
      set => AnalysisMaterial.Name = value;
    }

    internal GsaCustomMaterial(AnalysisMaterial analysisMaterial, int id, MatType type = MatType.Custom) {
      AnalysisMaterial = analysisMaterial;
      Id = id;
      IsUserDefined = true;
      MaterialType = type;
    }

    public GsaCustomMaterial(GsaCustomMaterial other) : base(other) {
      AnalysisMaterial = new AnalysisMaterial();
      DuplicateAnalysisMaterial(other);
      Name = other.Name;
    }
  }
}
