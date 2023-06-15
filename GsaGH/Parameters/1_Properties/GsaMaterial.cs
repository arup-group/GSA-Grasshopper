﻿using System;
using System.Collections.Generic;
using GsaAPI;
using GsaAPI.Materials;

namespace GsaGH.Parameters {
  /// <summary>
  ///   Material class, this class defines the basic properties and methods for any <see cref="GsaAPI.AnalysisMaterial" />
  /// </summary>
  public class GsaMaterial {
    public enum MatType {
      Generic,
      Steel,
      Concrete,
      Aluminium,
      Glass,
      Frp,
      Timber,
      Fabric,
    }

    public int Id {
      get => _id;
      set {
        _id = value;
        if (_id == 0) {
          return;
        }

        _guid = Guid.NewGuid();
      }
    }
    public string ConcreteDesignCodeName { get; private set; } = string.Empty;
    public string SteelDesignCodeName { get; private set; } = string.Empty;
    public string Name {
      get {
        switch (MaterialType) {
          case MatType.Aluminium:
            return _aluminiumMaterial.Name;

          case MatType.Concrete:
            return _concreteMaterial.Name;

          case MatType.Frp:
            return _frpMaterial.Name;

          case MatType.Glass:
            return _glassMaterial.Name;

          case MatType.Steel:
            return _steelMaterial.Name;

          case MatType.Timber:
            return _timberMaterial.Name;

          case MatType.Fabric:
            return _fabricMaterial.Name;

          default:
            return _analysisMaterial.Name;
        }
      }
      set {
        _gradeName = Name;
        switch (MaterialType) {
          case MatType.Aluminium:
            _aluminiumMaterial.Name = value;
            break;

          case MatType.Concrete:
            _concreteMaterial.Name = value;
            break;

          case MatType.Frp:
            _frpMaterial.Name = value;
            break;

          case MatType.Glass:
            _glassMaterial.Name = value;
            break;

          case MatType.Steel:
            _steelMaterial.Name = value;
            break;

          case MatType.Timber:
            _timberMaterial.Name = value;
            break;

          case MatType.Fabric:
            _fabricMaterial.Name = value;
            break;

          default:
            _analysisMaterial.Name = value;
            break;
        }
        _guid = Guid.NewGuid();
      }
    }
    public Guid Guid => _guid;
    public MatType MaterialType { get; private set; } = MatType.Generic;
    
    internal AnalysisMaterial AnalysisMaterial {
      get {
        switch (MaterialType) {
          case MatType.Aluminium:
            return _aluminiumMaterial.AnalysisMaterial;

          case MatType.Concrete:
            return _concreteMaterial.AnalysisMaterial;

          case MatType.Frp:
            return _frpMaterial.AnalysisMaterial;

          case MatType.Glass:
            return _glassMaterial.AnalysisMaterial;

          case MatType.Steel:
            return _steelMaterial.AnalysisMaterial;

          case MatType.Timber:
            return _timberMaterial.AnalysisMaterial;

          case MatType.Fabric:
            throw new Exception("Cannot edit Analysis material for Fabric");

          default:
            return _analysisMaterial;
        }
      }
      set {
        if (MaterialType != MatType.Generic) {
          _gradeName = Name;
        }

        switch (MaterialType) {
          case MatType.Aluminium:
            _aluminiumMaterial.AnalysisMaterial = value;
            _aluminiumMaterial.Name = value.Name;
            break;

          case MatType.Concrete:
            _concreteMaterial.AnalysisMaterial = value;
            _concreteMaterial.Name = value.Name;
            break;

          case MatType.Frp:
            _frpMaterial.AnalysisMaterial = value;
            _frpMaterial.Name = value.Name;
            break;

          case MatType.Glass:
            _glassMaterial.AnalysisMaterial = value;
            _glassMaterial.Name = value.Name;
            break;

          case MatType.Steel:
            _steelMaterial.AnalysisMaterial = value;
            _steelMaterial.Name = value.Name;
            break;

          case MatType.Timber:
            _timberMaterial.AnalysisMaterial = value;
            _timberMaterial.Name = value.Name;
            break;

          case MatType.Fabric:
            throw new Exception("Fabric material is not isotropic");

          default:
            _analysisMaterial = value;
            break;
        }
        _guid = Guid.NewGuid();
      }
    }
    internal object StandardMaterial {
      get {
        switch (MaterialType) {
          case MatType.Aluminium:
            return _aluminiumMaterial
              ?? throw new Exception("Material is not a standard material.");

          case MatType.Concrete:
            return _concreteMaterial
              ?? throw new Exception("Material is not a standard material.");

          case MatType.Fabric:
            return _fabricMaterial ?? throw new Exception("Material is not a standard material.");

          case MatType.Frp:
            return _frpMaterial ?? throw new Exception("Material is not a standard material.");

          case MatType.Glass:
            return _glassMaterial ?? throw new Exception("Material is not a standard material.");

          case MatType.Steel:
            return _steelMaterial ?? throw new Exception("Material is not a standard material.");

          case MatType.Timber:
            return _timberMaterial ?? throw new Exception("Material is not a standard material.");

          default:
            throw new Exception("Material is not a standard material.");
        }
      }
    }
    
    private int _id = 0;
    private string _gradeName;
    private AnalysisMaterial _analysisMaterial;
    private Guid _guid = Guid.NewGuid();
    private AluminiumMaterial _aluminiumMaterial;
    private ConcreteMaterial _concreteMaterial;
    private FabricMaterial _fabricMaterial;
    private FrpMaterial _frpMaterial;
    private GlassMaterial _glassMaterial;
    private SteelMaterial _steelMaterial;
    private TimberMaterial _timberMaterial;

    public GsaMaterial() { }

    internal GsaMaterial(AnalysisMaterial apiMaterial, int id) {
      MaterialType = MatType.Generic;
      _analysisMaterial = apiMaterial;
      _id = id;
    }

    internal GsaMaterial(AluminiumMaterial apiMaterial, int id) {
      MaterialType = MatType.Aluminium;
      _aluminiumMaterial = apiMaterial;
      _id = id;
    }

    internal GsaMaterial(ConcreteMaterial apiMaterial, int id, string codeName) {
      MaterialType = MatType.Concrete;
      _concreteMaterial = apiMaterial;
      _id = id;
      ConcreteDesignCodeName = codeName;
    }

    internal GsaMaterial(FabricMaterial apiMaterial, int id) {
      MaterialType = MatType.Fabric;
      _fabricMaterial = apiMaterial;
      _id = id;
    }

    internal GsaMaterial(FrpMaterial apiMaterial, int id) {
      MaterialType = MatType.Frp;
      _frpMaterial = apiMaterial;
      _id = id;
    }

    internal GsaMaterial(GlassMaterial apiMaterial, int id) {
      MaterialType = MatType.Glass;
      _glassMaterial = apiMaterial;
      _id = id;
    }

    internal GsaMaterial(SteelMaterial apiMaterial, int id, string codeName) {
      MaterialType = MatType.Steel;
      _steelMaterial = apiMaterial;
      _id = id;
      SteelDesignCodeName = codeName;
    }

    internal GsaMaterial(TimberMaterial apiMaterial, int id) {
      MaterialType = MatType.Timber;
      _timberMaterial = apiMaterial;
      _id = id;
    }

    internal GsaMaterial(MatType type, string gradeName,
      string steelDesignCode = "", string concreteDesignCode = "") {
      Model m = CreateMaterialModel(steelDesignCode, concreteDesignCode);
      MaterialType = type;
      switch (type) {
        case MatType.Aluminium:
          _aluminiumMaterial = m.CreateAluminiumMaterial(gradeName);
          return;

        case MatType.Concrete:
          _concreteMaterial = m.CreateConcreteMaterial(gradeName);
          ConcreteDesignCodeName = concreteDesignCode;
          return;

        case MatType.Fabric:
          _fabricMaterial = m.CreateFabricMaterial(gradeName);
          return;

        case MatType.Frp:
          _frpMaterial = m.CreateFrpMaterial(gradeName);
          return;

        case MatType.Glass:
          _glassMaterial = m.CreateGlassMaterial(gradeName);
          return;

        case MatType.Steel:
          _steelMaterial = m.CreateSteelMaterial(gradeName);
          SteelDesignCodeName = steelDesignCode;
          return;

        case MatType.Timber:
          _timberMaterial = m.CreateTimberMaterial(gradeName);
          return;

        default:
          throw new Exception($"Material type {type} does not have standard materials");
      }
    }

    public GsaMaterial Duplicate(bool clone = false) {
      if (!clone) {
        return this;
      }

      var dup = new GsaMaterial {
        MaterialType = MaterialType,
        _id = _id,
        ConcreteDesignCodeName = ConcreteDesignCodeName,
        SteelDesignCodeName = SteelDesignCodeName,
      };

      if (_gradeName != null) {
        dup._gradeName = _gradeName;
      }

      Model m = CreateMaterialModel(SteelDesignCodeName, ConcreteDesignCodeName);
      switch (MaterialType) {
        case MatType.Aluminium:
          dup._aluminiumMaterial = m.CreateAluminiumMaterial(_gradeName ?? Name);
          if (_gradeName != null) {
            dup._aluminiumMaterial.AnalysisMaterial = DuplicateAnalysisMaterial(AnalysisMaterial);
            dup._aluminiumMaterial.Name = Name;
          }
          break;

        case MatType.Concrete:
          dup._concreteMaterial = m.CreateConcreteMaterial(_gradeName ?? Name);
          if (_gradeName != null) {
            dup._concreteMaterial.AnalysisMaterial = DuplicateAnalysisMaterial(AnalysisMaterial);
            dup._concreteMaterial.Name = Name;
          }
          break;

        case MatType.Fabric:
          dup._fabricMaterial = m.CreateFabricMaterial(_gradeName ?? Name);
          if (_gradeName != null) {
            dup._fabricMaterial.Name = Name;
          }
          break;

        case MatType.Frp:
          dup._frpMaterial = m.CreateFrpMaterial(_gradeName ?? Name);
          if (_gradeName != null) {
            dup._frpMaterial.AnalysisMaterial = DuplicateAnalysisMaterial(AnalysisMaterial);
            dup._frpMaterial.Name = Name;
          }
          break;

        case MatType.Glass:
          dup._glassMaterial = m.CreateGlassMaterial(_gradeName ?? Name);
          if (_gradeName != null) {
            dup._glassMaterial.AnalysisMaterial = DuplicateAnalysisMaterial(AnalysisMaterial);
            dup._glassMaterial.Name = Name;
          }
          break;

        case MatType.Steel:
          dup._steelMaterial = m.CreateSteelMaterial(_gradeName ?? Name);
          if (_gradeName != null) {
            dup._steelMaterial.AnalysisMaterial = DuplicateAnalysisMaterial(AnalysisMaterial);
            dup._steelMaterial.Name = Name;
          }
          break;

        case MatType.Timber:
          dup._timberMaterial = m.CreateTimberMaterial(_gradeName ?? Name);
          if (_gradeName != null) {
            dup._timberMaterial.AnalysisMaterial = DuplicateAnalysisMaterial(AnalysisMaterial);
            dup._timberMaterial.Name = Name;
          }
          break;

        default:
          dup._analysisMaterial = DuplicateAnalysisMaterial(_analysisMaterial); 
          break;
      }

      dup._guid = new Guid(_guid.ToString());
      return dup;
    }

    public override string ToString() {
      string code = string.Empty;

      if (MaterialType == MatType.Concrete && _gradeName == null) {
        code = ConcreteDesignCodeName;
      }

      if (MaterialType == MatType.Steel && _gradeName == null) {
        code = SteelDesignCodeName;
      }

      string id = Id == 0 ? string.Empty : " Grd:" + Id;
      return (code + " " + MaterialType + id + " " + (Name ?? string.Empty)).Trim();
    }

    internal static List<string> GetGradeNames(
      MatType type, string steelDesignCode = "", string concreteDesignCode = "") {
      Model m = CreateMaterialModel(steelDesignCode, concreteDesignCode);
      switch (type) {
        case MatType.Aluminium:
          return new List<string>(m.GetStandardAluminumMaterialNames());

        case MatType.Concrete:
          return new List<string>(m.GetStandardConcreteMaterialNames());

        case MatType.Fabric:
          return new List<string>(m.GetStandardFabricMaterialNames());

        case MatType.Frp:
          return new List<string>(m.GetStandardFrpMaterialNames());

        case MatType.Glass:
          return new List<string>(m.GetStandardGlassMaterialNames());

        case MatType.Steel:
          return new List<string>(m.GetStandardSteelMaterialNames());

        case MatType.Timber:
          return new List<string>(m.GetStandardTimberMaterialNames());

        default:
          throw new Exception($"Material type {type} does not have standard materials");
      }
    }

    private AnalysisMaterial DuplicateAnalysisMaterial(AnalysisMaterial original) {
      return new AnalysisMaterial() {
        CoefficientOfThermalExpansion = original.CoefficientOfThermalExpansion,
        Density = original.Density,
        ElasticModulus = original.ElasticModulus,
        PoissonsRatio = original.PoissonsRatio,
        Name = original.Name,
      };
    }

    private static Model CreateMaterialModel(
      string steelDesignCode = "", string concreteDesignCode = "") {
      if (steelDesignCode == string.Empty) {
        steelDesignCode = DesignCode.GetSteelDesignCodeNames()[0];
      }

      if (concreteDesignCode == string.Empty) {
        concreteDesignCode = DesignCode.GetConcreteDesignCodeNames()[0];
      }

      return new Model(concreteDesignCode, steelDesignCode);
    }
  }
}
