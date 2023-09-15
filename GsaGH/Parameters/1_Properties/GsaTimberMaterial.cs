using GsaAPI;
using GsaAPI.Materials;

namespace GsaGH.Parameters {
  public class GsaTimberMaterial : GsaMaterial, IGsaStandardMaterial {
    public override AnalysisMaterial AnalysisMaterial {
      get => _timberMaterial.AnalysisMaterial;
      set {
        _timberMaterial.AnalysisMaterial = value;
        IsUserDefined = true;
      }
    }
    public override string Name {
      get => _timberMaterial.Name;
      set => _timberMaterial.Name = value;
    }
    public object StandardMaterial => _timberMaterial;

    private TimberMaterial _timberMaterial;

    internal GsaTimberMaterial(TimberMaterial timberMaterial, bool fromApi = false,
      string concreteDesignCodeName = "", string steelDesignCodeName = "") {
      ConcreteDesignCodeName = concreteDesignCodeName;
      IsFromApi = fromApi;
      MaterialType = MatType.Timber;
      SteelDesignCodeName = steelDesignCodeName;
      _timberMaterial = timberMaterial;
    }

    internal GsaTimberMaterial(GsaTimberMaterial other) : base(other) {
      Model model = GsaModel.CreateModelFromCodes(ConcreteDesignCodeName, SteelDesignCodeName);
      _timberMaterial = model.CreateTimberMaterial(other.Name);

      DuplicateAnalysisMaterial(other);
    }
  }
}
