using System;
using System.Collections.Generic;
using System.Linq;
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
    public bool IsCustom { get; private set; } = false;
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
        if (IsCustom) {
          return _analysisMaterial.Name;
        }

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
            return string.Empty;
        }
      }
      set {
        if (IsCustom) {
          _analysisMaterial.Name = value;
        } else {
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
        }
        _guid = Guid.NewGuid();
      }
    }
    public Guid Guid => _guid;
    public MatType MaterialType { get; private set; } = MatType.Generic;

    internal AnalysisMaterial AnalysisMaterial {
      get {
        if (IsCustom) {
          return _analysisMaterial;
        }

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
        if (IsCustom) {
          _analysisMaterial = DuplicateAnalysisMaterial(value);
        } else {
          _gradeName = Name;
          switch (MaterialType) {
            case MatType.Aluminium:
              _aluminiumMaterial.AnalysisMaterial = DuplicateAnalysisMaterial(value);
              _aluminiumMaterial.Name = value.Name;
              break;

            case MatType.Concrete:
              _concreteMaterial.AnalysisMaterial = DuplicateAnalysisMaterial(value);
              _concreteMaterial.Name = value.Name;
              break;

            case MatType.Frp:
              _frpMaterial.AnalysisMaterial = DuplicateAnalysisMaterial(value);
              _frpMaterial.Name = value.Name;
              break;

            case MatType.Glass:
              _glassMaterial.AnalysisMaterial = DuplicateAnalysisMaterial(value);
              _glassMaterial.Name = value.Name;
              break;

            case MatType.Steel:
              _steelMaterial.AnalysisMaterial = DuplicateAnalysisMaterial(value);
              _steelMaterial.Name = value.Name;
              break;

            case MatType.Timber:
              _timberMaterial.AnalysisMaterial = DuplicateAnalysisMaterial(value);
              _timberMaterial.Name = value.Name;
              break;

            case MatType.Fabric:
              throw new Exception("Fabric material is not isotropic");

            default:
              _analysisMaterial = value;
              break;
          }
        }
        _guid = Guid.NewGuid();
      }
    }
    internal object StandardMaterial {
      get {
        if (IsCustom) {
          throw new Exception("Material is not a standard material.");
        }

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

    internal GsaMaterial(AnalysisMaterial apiMaterial, int id, MatType type = MatType.Generic) {
      MaterialType = type;
      _analysisMaterial = apiMaterial;
      _id = id;
      IsCustom = true;
    }

    internal GsaMaterial(AluminiumMaterial apiMaterial, int id, string concreteCodeName, string steelCodeName) {
      MaterialType = MatType.Aluminium;
      _aluminiumMaterial = apiMaterial;
      _id = id;
      ConcreteDesignCodeName = concreteCodeName;
      SteelDesignCodeName = steelCodeName;
      ValidateApiMaterial();
    }

    internal GsaMaterial(ConcreteMaterial apiMaterial, int id, string concreteCodeName, string steelCodeName) {
      MaterialType = MatType.Concrete;
      _concreteMaterial = apiMaterial;
      _id = id;
      ConcreteDesignCodeName = concreteCodeName;
      SteelDesignCodeName = steelCodeName;
      ValidateApiMaterial();
    }

    internal GsaMaterial(FabricMaterial apiMaterial, int id, string concreteCodeName, string steelCodeName) {
      MaterialType = MatType.Fabric;
      _fabricMaterial = apiMaterial;
      _id = id;
      ConcreteDesignCodeName = concreteCodeName;
      SteelDesignCodeName = steelCodeName;
      ValidateApiMaterial();
    }

    internal GsaMaterial(FrpMaterial apiMaterial, int id, string concreteCodeName, string steelCodeName) {
      MaterialType = MatType.Frp;
      _frpMaterial = apiMaterial;
      _id = id;
      ConcreteDesignCodeName = concreteCodeName;
      SteelDesignCodeName = steelCodeName;
    }

    internal GsaMaterial(GlassMaterial apiMaterial, int id, string concreteCodeName, string steelCodeName) {
      MaterialType = MatType.Glass;
      _glassMaterial = apiMaterial;
      _id = id;
      ConcreteDesignCodeName = concreteCodeName;
      SteelDesignCodeName = steelCodeName;
      ValidateApiMaterial();
    }

    internal GsaMaterial(SteelMaterial apiMaterial, int id, string concreteCodeName, string steelCodeName) {
      MaterialType = MatType.Steel;
      _steelMaterial = apiMaterial;
      _id = id;
      ConcreteDesignCodeName = concreteCodeName;
      SteelDesignCodeName = steelCodeName;
      ValidateApiMaterial();
    }

    internal GsaMaterial(TimberMaterial apiMaterial, int id, string concreteCodeName, string steelCodeName) {
      MaterialType = MatType.Timber;
      _timberMaterial = apiMaterial;
      _id = id;
      ConcreteDesignCodeName = concreteCodeName;
      SteelDesignCodeName = steelCodeName;
      ValidateApiMaterial();
    }

    internal GsaMaterial(MatType type, string gradeName,
      string steelDesignCode = "", string concreteDesignCode = "") {
      Model m = GsaModel.CreateModelFromCodes(concreteDesignCode, steelDesignCode);
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

    public GsaMaterial Clone() {
      var dup = new GsaMaterial {
        MaterialType = MaterialType,
        _id = _id,
        ConcreteDesignCodeName = ConcreteDesignCodeName,
        SteelDesignCodeName = SteelDesignCodeName,
        IsCustom = IsCustom,
      };

      if (IsCustom) {
        dup._analysisMaterial = DuplicateAnalysisMaterial(AnalysisMaterial);
      } else {
        dup.RecreateForDesignCode(_gradeName ?? Name);

        if (_gradeName != null) {
          dup.AnalysisMaterial = DuplicateAnalysisMaterial(AnalysisMaterial);
          dup.Name = Name;
          dup._gradeName = _gradeName;
        }
      }

      dup._guid = Guid.NewGuid();
      return dup;
    }

    public GsaMaterial Duplicate() {
      return this;
    }

    internal void RecreateForDesignCode(Model m) {
      RecreateForDesignCode(m, _gradeName ?? Name);
    }

    internal void RecreateForDesignCode(string gradeName) {
      Model m = GsaModel.CreateModelFromCodes(ConcreteDesignCodeName, SteelDesignCodeName);
      RecreateForDesignCode(m, gradeName);
    }

    private void RecreateForDesignCode(Model m, string gradeName) {
      switch (MaterialType) {
        case MatType.Aluminium:
          AnalysisMaterial tempAluminium = DuplicateAnalysisMaterial(AnalysisMaterial);
          _aluminiumMaterial = m.CreateAluminiumMaterial(gradeName);
          if (_gradeName != null || !AnalysisMaterialsAreEqual(tempAluminium, AnalysisMaterial)) {
            _aluminiumMaterial.AnalysisMaterial = tempAluminium;
            _aluminiumMaterial.Name = Name;
          }
          break;

        case MatType.Concrete:
          AnalysisMaterial tempConcrete = DuplicateAnalysisMaterial(AnalysisMaterial);
          _concreteMaterial = m.CreateConcreteMaterial(gradeName);
          if (_gradeName != null || !AnalysisMaterialsAreEqual(tempConcrete, AnalysisMaterial)) {
            _concreteMaterial.AnalysisMaterial = tempConcrete;
            _concreteMaterial.Name = Name;
          }
          break;

        case MatType.Fabric:
          _fabricMaterial = m.CreateFabricMaterial(gradeName);
          if (_gradeName != null) {
            _fabricMaterial.Name = Name;
          }
          break;

        case MatType.Frp:
          AnalysisMaterial tempFrp = DuplicateAnalysisMaterial(AnalysisMaterial);
          _frpMaterial = m.CreateFrpMaterial(gradeName);
          if (_gradeName != null || !AnalysisMaterialsAreEqual(tempFrp, AnalysisMaterial)) {
            _frpMaterial.AnalysisMaterial = tempFrp;
            _frpMaterial.Name = Name;
          }
          break;

        case MatType.Glass:
          AnalysisMaterial tempGlass = DuplicateAnalysisMaterial(AnalysisMaterial);
          _glassMaterial = m.CreateGlassMaterial(gradeName);
          if (_gradeName != null || !AnalysisMaterialsAreEqual(tempGlass, AnalysisMaterial)) {
            _glassMaterial.AnalysisMaterial = tempGlass;
            _glassMaterial.Name = Name;
          }
          break;

        case MatType.Steel:
          AnalysisMaterial tempSteel = DuplicateAnalysisMaterial(AnalysisMaterial);
          _steelMaterial = m.CreateSteelMaterial(gradeName);
          if (_gradeName != null || !AnalysisMaterialsAreEqual(tempSteel, AnalysisMaterial)) {
            _steelMaterial.AnalysisMaterial = tempSteel;
            _steelMaterial.Name = Name;
          }
          break;

        case MatType.Timber:
          AnalysisMaterial tempTimber = DuplicateAnalysisMaterial(AnalysisMaterial);
          _timberMaterial = m.CreateTimberMaterial(gradeName);
          if (_gradeName != null || !AnalysisMaterialsAreEqual(tempTimber, AnalysisMaterial)) {
            _timberMaterial.AnalysisMaterial = tempTimber;
            _timberMaterial.Name = Name;
          }
          break;

        default:
          _analysisMaterial = DuplicateAnalysisMaterial(AnalysisMaterial);
          break;
      }
    }

    public override string ToString() {
      string code = string.Empty;

      if (MaterialType == MatType.Concrete && _gradeName == null) {
        code = ConcreteDesignCodeName;
      }

      if (MaterialType == MatType.Steel && _gradeName == null) {
        code = SteelDesignCodeName;
      }

      if (IsCustom) {
        code = "Custom";
      }

      string id = Id == 0 ? string.Empty : " Grd:" + Id;
      return (code + " " + MaterialType + id + " " + (Name ?? string.Empty)).Trim();
    }

    internal static List<string> GetGradeNames(
      MatType type, string steelDesignCode = "", string concreteDesignCode = "") {
      Model m = GsaModel.CreateModelFromCodes(concreteDesignCode, steelDesignCode);
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

    private void ValidateApiMaterial() {
      List<string> gradeNames = GetGradeNames(
        MaterialType, SteelDesignCodeName, ConcreteDesignCodeName);
      if (gradeNames.Contains(Name)) {
        AnalysisMaterial a = CreateCodeAnalysisMaterial(Name);
        if (AnalysisMaterialsAreEqual(a, AnalysisMaterial)) {
          return;
        }

        _gradeName = Name;
      } else {
        var dict = new Dictionary<string, double>();
        foreach (string gradeName in gradeNames) {
          AnalysisMaterial a = CreateCodeAnalysisMaterial(gradeName);
          if (AnalysisMaterialsAreEqual(a, AnalysisMaterial)) {
            _gradeName = gradeName;
            return;
          }

          dict[gradeName] = AnalysisMaterialsSimilarity(a, AnalysisMaterial);
        }

        _gradeName = dict.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
      }
    }

    private double AnalysisMaterialsSimilarity(AnalysisMaterial a, AnalysisMaterial b) {
      if (a == null || b == null) {
        return 0;
      }

      double similarity = 0;
      if (Math.Round(a.CoefficientOfThermalExpansion, 11) ==
        Math.Round(b.CoefficientOfThermalExpansion, 11)) {
        similarity += 1;
      } else {
        similarity += Math.Abs((a.CoefficientOfThermalExpansion - b.CoefficientOfThermalExpansion)
          / b.CoefficientOfThermalExpansion);
      }

      if (Math.Round(a.Density, 11) == Math.Round(b.Density, 11)) {
        similarity += 1;
      } else {
        similarity += Math.Abs((a.Density - b.Density) / b.Density);
      }

      if (Math.Round(a.ElasticModulus, 11) == Math.Round(b.ElasticModulus, 11)) {
        similarity += 1;
      } else {
        similarity += Math.Abs((a.ElasticModulus - b.ElasticModulus) / b.ElasticModulus);
      }

      if (Math.Round(a.PoissonsRatio, 11) == Math.Round(b.PoissonsRatio, 11)) {
        similarity += 1;
      } else {
        similarity += Math.Abs((a.PoissonsRatio - b.PoissonsRatio)/ b.PoissonsRatio);
      }

      return similarity;
    }

    private bool AnalysisMaterialsAreEqual(AnalysisMaterial a, AnalysisMaterial b) {
      if (a == null || b == null) {
        return false;
      }

      if (Math.Round(a.CoefficientOfThermalExpansion, 11) != 
        Math.Round(b.CoefficientOfThermalExpansion, 11)) {
        return false;
      }

      if (Math.Round(a.Density, 11) != Math.Round(b.Density, 11)) {
        return false;
      }

      if (Math.Round(a.ElasticModulus, 11) != Math.Round(b.ElasticModulus, 11)) {
        return false;
      }

      if (Math.Round(a.PoissonsRatio, 11) != Math.Round(b.PoissonsRatio, 11)) {
        return false;
      }

      return true;
    }

    private AnalysisMaterial CreateCodeAnalysisMaterial(string name) {
      Model m = GsaModel.CreateModelFromCodes(ConcreteDesignCodeName, SteelDesignCodeName);
      switch (MaterialType) {
        case MatType.Aluminium:
          return m.CreateAluminiumMaterial(name).AnalysisMaterial;

        case MatType.Concrete:
          return m.CreateConcreteMaterial(name).AnalysisMaterial;

        case MatType.Frp:
          return m.CreateFrpMaterial(name).AnalysisMaterial;

        case MatType.Glass:
          return m.CreateGlassMaterial(name).AnalysisMaterial;

        case MatType.Steel:
          return m.CreateSteelMaterial(name).AnalysisMaterial;

        case MatType.Timber:
          return m.CreateTimberMaterial(name).AnalysisMaterial;
      }
      return null;
    }

    private static AnalysisMaterial DuplicateAnalysisMaterial(AnalysisMaterial original) {
      return new AnalysisMaterial() {
        CoefficientOfThermalExpansion = original.CoefficientOfThermalExpansion,
        Density = original.Density,
        ElasticModulus = original.ElasticModulus,
        PoissonsRatio = original.PoissonsRatio,
        Name = original.Name,
      };
    }
  }
}
