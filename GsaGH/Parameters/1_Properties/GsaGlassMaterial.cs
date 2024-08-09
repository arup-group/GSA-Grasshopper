using GsaAPI;
using GsaAPI.Materials;

using GsaGH.Helpers.GsaApi;

namespace GsaGH.Parameters {
  public class GsaGlassMaterial : GsaMaterial, IGsaStandardMaterial {
    public override AnalysisMaterial AnalysisMaterial {
      get => _glassMaterial.AnalysisMaterial;
      set {
        _glassMaterial.AnalysisMaterial = value;
        IsUserDefined = true;
      }
    }
    public override string Name {
      get => _glassMaterial.Name;
      set => _glassMaterial.Name = value;
    }
    public object StandardMaterial => _glassMaterial;

    private GlassMaterial _glassMaterial;

    internal GsaGlassMaterial(GlassMaterial glassMaterial, bool fromApi = false,
      string concreteDesignCodeName = "", string steelDesignCodeName = "") {
      ConcreteDesignCodeName = concreteDesignCodeName;
      IsFromApi = fromApi;
      MaterialType = MatType.Glass;
      SteelDesignCodeName = steelDesignCodeName;
      _glassMaterial = glassMaterial;
    }

    public GsaGlassMaterial(GsaGlassMaterial other) : base(other) {
      Model model = ModelFactory.CreateModelFromCodes(ConcreteDesignCodeName, SteelDesignCodeName);
      _glassMaterial = model.CreateGlassMaterial(other.Name);

      DuplicateAnalysisMaterial(other);
    }
  }
}
