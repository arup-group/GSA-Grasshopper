using GsaAPI;
using GsaAPI.Materials;

namespace GsaGH.Parameters {
  public class GsaAluminiumMaterial : GsaMaterial, IGsaStandardMaterial {

    public override AnalysisMaterial AnalysisMaterial {
      get => _aluminiumMaterial.AnalysisMaterial;
      set {
        _aluminiumMaterial.AnalysisMaterial = value;
        IsUserDefined = true;
      }
    }
    public override string Name {
      get => _aluminiumMaterial.Name;
      set => _aluminiumMaterial.Name = value;
    }
    public object StandardMaterial => _aluminiumMaterial;

    private AluminiumMaterial _aluminiumMaterial;

    internal GsaAluminiumMaterial(AluminiumMaterial aluminiumMaterial, bool fromApi = false,
      string concreteDesignCodeName = "", string steelDesignCodeName = "") {
      ConcreteDesignCodeName = concreteDesignCodeName;
      IsFromApi = fromApi;
      MaterialType = MatType.Aluminium;
      SteelDesignCodeName = steelDesignCodeName;
      _aluminiumMaterial = aluminiumMaterial;
    }

    internal GsaAluminiumMaterial(GsaAluminiumMaterial other) : base(other) {
      Model model = GsaModel.CreateModelFromCodes(ConcreteDesignCodeName, SteelDesignCodeName);
      _aluminiumMaterial = model.CreateAluminiumMaterial(other.Name);

      DuplicateAnalysisMaterial(other);
    }
  }
}
