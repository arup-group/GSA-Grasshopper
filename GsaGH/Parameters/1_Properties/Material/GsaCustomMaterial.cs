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
    public MatType Type {
      get => _type;
      set {
        _type = value;
        Guid = Guid.NewGuid();
      }
    }

    private int _id = 0;
    private AnalysisMaterial _material;
    private MatType _type = MatType.Generic;

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

    public override string ToString() {
      string id = Id == 0 ? string.Empty : $"ID:{Id} ";
      return $"{id}Custom {Name}";
    }
  }
}
