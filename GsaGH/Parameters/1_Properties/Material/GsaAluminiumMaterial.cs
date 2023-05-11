using System;
using GsaAPI;

namespace GsaGH.Parameters {
  public class GsaAluminiumMaterial : IGsaStandardMaterial<AluminiumMaterial> {
    public MatType Type => MatType.Aluminium;
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
    public AluminiumMaterial StandardMaterial {
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
    private AluminiumMaterial _material;

    public IGsaMaterial Duplicate() {
      try {
        var model = new Model(ConcreteDesignCodeName, SteelDesignCodeName);
        AluminiumMaterial mat = model.CreateAluminiumMaterial(Name);
        return new GsaAluminiumMaterial() {
          StandardMaterial = mat,
          Id = _id,
          SteelDesignCodeName = SteelDesignCodeName,
          ConcreteDesignCodeName = ConcreteDesignCodeName,
          Guid = new Guid(Guid.ToString()),
        };
      } catch (Exception) {
        return new GsaAluminiumMaterial() {
          StandardMaterial = new AluminiumMaterial() {
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
