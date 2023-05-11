using System;
using GsaAPI;

namespace GsaGH.Parameters {
  public class GsaFabricMaterial : IGsaStandardMaterial<FabricMaterial> {
    public MatType Type => MatType.Fabric;
    public string SteelDesignCodeName { get; set; }
    public string ConcreteDesignCodeName { get; set; }
    public Guid Guid { get; set; } = Guid.NewGuid(); 
    public string Name {
      get => _material.Name;
      set {
        _material.Name = value;
        Guid = Guid.NewGuid();
      }
    }
    public int Id {
      get => _id;
      set {
        _id = value;
        Guid = Guid.NewGuid();
      }
    }
    public FabricMaterial StandardMaterial {
      get => _material;
      set {
        _material = value;
        Guid = Guid.NewGuid();
      }
    }
    public AnalysisMaterial AnalysisMaterial {
      get => _material.AnalysisMaterial;
      set {
        _material.AnalysisMaterial = value;
        Guid = Guid.NewGuid();
      }
    }

    private int _id = 0;
    private FabricMaterial _material;

    public IGsaMaterial Duplicate() {
      try {
        var model = new Model(ConcreteDesignCodeName, SteelDesignCodeName);
        FabricMaterial mat = model.CreateFabricMaterial(Name);
        return new GsaFabricMaterial() {
          StandardMaterial = mat,
          Id = _id,
          SteelDesignCodeName = SteelDesignCodeName,
          ConcreteDesignCodeName = ConcreteDesignCodeName,
          Guid = new Guid(Guid.ToString()),
        };
      } catch (Exception) {
        return new GsaFabricMaterial() {
          StandardMaterial = new FabricMaterial() {
            AnalysisMaterial = GsaCustomMaterial.Duplicate(_material.AnalysisMaterial),
            Name = _material.Name,
          },
          Id = _id,
          SteelDesignCodeName = SteelDesignCodeName,
          ConcreteDesignCodeName = ConcreteDesignCodeName,
          Guid = new Guid(Guid.ToString()),
        };
      }
    }
    public override string ToString() {
      string id = Id == 0 ? string.Empty : $"ID:{Id} ";
      return $"{id}{Type} {Name}";
    }
  }
}
