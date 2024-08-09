using GsaAPI;
using GsaAPI.Materials;

using GsaGH.Helpers.GsaApi;

namespace GsaGH.Parameters {
  public class GsaConcreteMaterial : GsaMaterial, IGsaStandardMaterial {
    public override AnalysisMaterial AnalysisMaterial {
      get => _concreteMaterial.AnalysisMaterial;
      set {
        _concreteMaterial.AnalysisMaterial = value;
        IsUserDefined = true;
      }
    }
    public override string Name {
      get => _concreteMaterial.Name;
      set => _concreteMaterial.Name = value;
    }
    public object StandardMaterial => _concreteMaterial;

    private readonly ConcreteMaterial _concreteMaterial;

    internal GsaConcreteMaterial(ConcreteMaterial concreteMaterial, string concreteDesignCodeName = "") {
      ConcreteDesignCodeName = concreteDesignCodeName;
      MaterialType = MatType.Concrete;
      _concreteMaterial = concreteMaterial;
    }

    internal GsaConcreteMaterial(ConcreteMaterial concreteMaterial, bool fromApi = false,
      string concreteDesignCodeName = "", string steelDesignCodeName = "") {
      ConcreteDesignCodeName = concreteDesignCodeName;
      IsFromApi = fromApi;
      MaterialType = MatType.Concrete;
      SteelDesignCodeName = steelDesignCodeName;
      _concreteMaterial = concreteMaterial;
    }

    public GsaConcreteMaterial(GsaConcreteMaterial other) : base(other) {
      Model model = ModelFactory.CreateModelFromCodes(ConcreteDesignCodeName, SteelDesignCodeName);
      _concreteMaterial = model.CreateConcreteMaterial(other.Name);

      DuplicateAnalysisMaterial(other);
    }
  }
}
