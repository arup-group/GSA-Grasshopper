using GsaAPI;
using GsaAPI.Materials;

using GsaGH.Helpers.GsaApi;

namespace GsaGH.Parameters {
  public class GsaFrpMaterial : GsaMaterial, IGsaStandardMaterial {
    public override AnalysisMaterial AnalysisMaterial {
      get => _frpMaterial.AnalysisMaterial;
      set {
        _frpMaterial.AnalysisMaterial = value;
        IsUserDefined = true;
      }
    }
    public override string Name {
      get => _frpMaterial.Name;
      set => _frpMaterial.Name = value;
    }
    public object StandardMaterial => _frpMaterial;

    private FrpMaterial _frpMaterial;

    internal GsaFrpMaterial(FrpMaterial frpMaterial, bool fromApi = false,
      string concreteDesignCodeName = "", string steelDesignCodeName = "") {
      ConcreteDesignCodeName = concreteDesignCodeName;
      IsFromApi = fromApi;
      MaterialType = MatType.Frp;
      SteelDesignCodeName = steelDesignCodeName;
      _frpMaterial = frpMaterial;
    }

    public GsaFrpMaterial(GsaFrpMaterial other) : base(other) {
      Model model = ModelFactory.CreateModelFromCodes(ConcreteDesignCodeName, SteelDesignCodeName);
      _frpMaterial = model.CreateFrpMaterial(other.Name);

      DuplicateAnalysisMaterial(other);
    }
  }
}
