using GsaAPI;
using GsaAPI.Materials;

using GsaGH.Helpers.GsaApi;

namespace GsaGH.Parameters {
  public class GsaSteelMaterial : GsaMaterial, IGsaStandardMaterial {
    public override AnalysisMaterial AnalysisMaterial {
      get => _steelMaterial.AnalysisMaterial;
      set {
        _steelMaterial.AnalysisMaterial = value;
        IsUserDefined = true;
      }
    }
    public override string Name {
      get => _steelMaterial.Name;
      set => _steelMaterial.Name = value;
    }
    public object StandardMaterial => _steelMaterial;

    private SteelMaterial _steelMaterial;

    internal GsaSteelMaterial(SteelMaterial steelMaterial, string steelDesignCodeName = "") {
      MaterialType = MatType.Steel;
      SteelDesignCodeName = steelDesignCodeName;
      _steelMaterial = steelMaterial;
    }

    internal GsaSteelMaterial(SteelMaterial steelMaterial, bool fromApi = false,
      string concreteDesignCodeName = "", string steelDesignCodeName = "") {
      ConcreteDesignCodeName = concreteDesignCodeName;
      IsFromApi = fromApi;
      MaterialType = MatType.Steel;
      SteelDesignCodeName = steelDesignCodeName;
      _steelMaterial = steelMaterial;
    }

    public GsaSteelMaterial(GsaSteelMaterial other) : base(other) {
      Model model = ModelFactory.CreateModelFromCodes(ConcreteDesignCodeName, SteelDesignCodeName);
      _steelMaterial = model.CreateSteelMaterial(other.Name);

      DuplicateAnalysisMaterial(other);
    }
  }
}
