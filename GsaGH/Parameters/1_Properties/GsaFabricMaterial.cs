using System;

using GsaAPI;
using GsaAPI.Materials;

using GsaGH.Helpers.GsaApi;

namespace GsaGH.Parameters {
  public class GsaFabricMaterial : GsaMaterial, IGsaStandardMaterial {
    public override AnalysisMaterial AnalysisMaterial { get => null; set => throw new ArgumentException("Can not set analysis material for a fabric material"); }
    public override string Name {
      get => _fabricMaterial.Name;
      set => _fabricMaterial.Name = value;
    }
    public new bool IsUserDefined = false;
    public object StandardMaterial => _fabricMaterial;

    private FabricMaterial _fabricMaterial;

    internal GsaFabricMaterial(FabricMaterial fabricMaterial, bool fromApi = false,
      string concreteDesignCodeName = "", string steelDesignCodeName = "") {
      ConcreteDesignCodeName = concreteDesignCodeName;
      IsFromApi = fromApi;
      MaterialType = MatType.Fabric;
      SteelDesignCodeName = steelDesignCodeName;
      _fabricMaterial = fabricMaterial;
    }

    public GsaFabricMaterial(GsaFabricMaterial other) : base(other) {
      Model model = ModelFactory.CreateModelFromCodes(ConcreteDesignCodeName, SteelDesignCodeName);
      _fabricMaterial = model.CreateFabricMaterial(other.Name);
    }
  }
}
