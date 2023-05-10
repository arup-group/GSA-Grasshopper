using System;
using System.Linq;
using System.Runtime.InteropServices;
using GsaAPI;
using GsaGH.Helpers.GsaApi;

namespace GsaGH.Parameters {
  public class GsaCustomMaterial : IGsaMaterial {
    
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
    public Guid Guid { get; set; } = Guid.NewGuid();
    public AnalysisMaterial AnalysisMaterial {
      get => _material;
      set {
        _material = value;
        Guid = Guid.NewGuid();
      }
    }
    public MaterialType Type { get; set; } = MaterialType.GENERIC;

    private int _id = 0;
    private AnalysisMaterial _material;
    private MaterialType _type = MaterialType.GENERIC;

    internal static AnalysisMaterial Duplicate(AnalysisMaterial material) {
      return new AnalysisMaterial() {
        CoefficientOfThermalExpansion =
            material.CoefficientOfThermalExpansion,
        Density = material.Density,
        ElasticModulus = material.ElasticModulus,
        PoissonsRatio = material.PoissonsRatio,
        Name = material.Name,
      };
    }

    public IGsaMaterial Duplicate() {
      
      return new GsaCustomMaterial() {
        AnalysisMaterial = Duplicate(_material),
        Id = _id,
        Guid = new Guid(Guid.ToString())
      };
    }

    public void ChangeType(MatType type) {
      Type = (MaterialType)Enum.Parse(typeof(MaterialType), type.ToString(), true);
      Guid = Guid.NewGuid();
    }

    public override string ToString() {
      string id = Id == 0 ? string.Empty : $"ID:{Id} ";
      return $"{id}Custom {Name}";
    }
  }
}
