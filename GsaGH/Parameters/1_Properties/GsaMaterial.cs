using System;
using System.Collections.Generic;
using GsaAPI;
using GsaAPI.Materials;

namespace GsaGH.Parameters {
  /// <summary>
  /// "A Material is used by <see cref="GsaSection"/>s, <see cref="GsaProp2d"/>s and <see cref="GsaProp3d"/>s. It is only possible to work with elastic isotropic material types. A Material can either be created as a Standard Material (<see cref="Components.CreateMaterial"/>) from design code and grade, or as a custom material <see cref="Components.CreateCustomMaterial"/>).
  /// Use the <see cref="Components.GetMaterials"/> to get all materials in a <see cref="GsaModel"/> and then use <see cref="Components.EditMaterial"/> in combination with <see cref="Components.GetMaterialProperties"/> to get information about material properties.
  /// Refer to <see href="/references/hidr-data-mat-steel.html">Materials</see> to read more.
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
    public bool AnalysisMaterialsModified { get; private set; } = false;
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
          AnalysisMaterialsModified = true;
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
          AnalysisMaterialsModified = true;
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

    internal GsaMaterial(AluminiumMaterial apiMaterial, int id, Model model) {
      MaterialType = MatType.Aluminium;
      _aluminiumMaterial = apiMaterial;
      _id = id;
      SetCodeNames(model);
      CheckIfAnalysisMaterialIsModified(model);
      SimplifyCodeNames();
    }

    internal GsaMaterial(ConcreteMaterial apiMaterial, int id, Model model) {
      MaterialType = MatType.Concrete;
      _concreteMaterial = apiMaterial;
      _id = id;
      SetCodeNames(model);
      CheckIfAnalysisMaterialIsModified(model);
      SimplifyCodeNames();
    }

    internal GsaMaterial(FabricMaterial apiMaterial, int id, Model model) {
      MaterialType = MatType.Fabric;
      _fabricMaterial = apiMaterial;
      _id = id;
      SetCodeNames(model);
      SimplifyCodeNames();
    }

    internal GsaMaterial(FrpMaterial apiMaterial, int id, Model model) {
      MaterialType = MatType.Frp;
      _frpMaterial = apiMaterial;
      _id = id;
      SetCodeNames(model);
      CheckIfAnalysisMaterialIsModified(model);
      SimplifyCodeNames();
    }

    internal GsaMaterial(GlassMaterial apiMaterial, int id, Model model) {
      MaterialType = MatType.Glass;
      _glassMaterial = apiMaterial;
      _id = id;
      SetCodeNames(model);
      CheckIfAnalysisMaterialIsModified(model);
      SimplifyCodeNames();
    }

    internal GsaMaterial(SteelMaterial apiMaterial, int id, Model model) {
      MaterialType = MatType.Steel;
      _steelMaterial = apiMaterial;
      _id = id;
      SetCodeNames(model);
      CheckIfAnalysisMaterialIsModified(model);
      SimplifyCodeNames();
    }

    internal GsaMaterial(TimberMaterial apiMaterial, int id, Model model) {
      MaterialType = MatType.Timber;
      _timberMaterial = apiMaterial;
      _id = id;
      SetCodeNames(model);
      CheckIfAnalysisMaterialIsModified(model);
      SimplifyCodeNames();
    }

    internal GsaMaterial(MatType type, string gradeName,
      string codeName = "") {
      string concreteDesignCode = type == MatType.Concrete ? codeName : string.Empty;
      string steelDesignCode = type == MatType.Steel ? codeName : string.Empty;
      Model m = GsaModel.CreateModelFromCodes(concreteDesignCode, steelDesignCode);
      MaterialType = type;
      switch (type) {
        case MatType.Aluminium:
          _aluminiumMaterial = m.CreateAluminiumMaterial(gradeName);
          return;

        case MatType.Concrete:
          _concreteMaterial = m.CreateConcreteMaterial(gradeName);
          ConcreteDesignCodeName = codeName;
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
          SteelDesignCodeName = codeName;
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
        AnalysisMaterialsModified = AnalysisMaterialsModified,
      };

      if (IsCustom) {
        dup._analysisMaterial = DuplicateAnalysisMaterial(AnalysisMaterial);
      } else {
        dup.RecreateForDesignCode(AnalysisMaterialsModified ? _gradeName : Name);

        if (AnalysisMaterialsModified) {
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
      RecreateForDesignCode(m, AnalysisMaterialsModified ? _gradeName : Name);
    }

    internal void RecreateForDesignCode(string gradeName) {
      Model m = GsaModel.CreateModelFromCodes(ConcreteDesignCodeName, SteelDesignCodeName);
      RecreateForDesignCode(m, gradeName);
    }

    private void RecreateForDesignCode(Model m, string gradeName) {
      switch (MaterialType) {
        case MatType.Aluminium:
          _aluminiumMaterial = m.CreateAluminiumMaterial(gradeName);
          if (AnalysisMaterialsModified) {
            _aluminiumMaterial.AnalysisMaterial = DuplicateAnalysisMaterial(AnalysisMaterial);
            _aluminiumMaterial.Name = Name;
          }
          break;

        case MatType.Concrete:
          _concreteMaterial = m.CreateConcreteMaterial(gradeName);
          if (AnalysisMaterialsModified) {
            _concreteMaterial.AnalysisMaterial = DuplicateAnalysisMaterial(AnalysisMaterial);
            _concreteMaterial.Name = Name;
          }
          break;

        case MatType.Fabric:
          _fabricMaterial = m.CreateFabricMaterial(gradeName);
          if (AnalysisMaterialsModified) {
            _fabricMaterial.Name = Name;
          }
          break;

        case MatType.Frp:
          _frpMaterial = m.CreateFrpMaterial(gradeName);
          if (AnalysisMaterialsModified) {
            _frpMaterial.AnalysisMaterial = DuplicateAnalysisMaterial(AnalysisMaterial);
            _frpMaterial.Name = Name;
          }
          break;

        case MatType.Glass:
          _glassMaterial = m.CreateGlassMaterial(gradeName);
          if (AnalysisMaterialsModified) {
            _glassMaterial.AnalysisMaterial = DuplicateAnalysisMaterial(AnalysisMaterial);
            _glassMaterial.Name = Name;
          }
          break;

        case MatType.Steel:
          _steelMaterial = m.CreateSteelMaterial(gradeName);
          if (AnalysisMaterialsModified) {
            _steelMaterial.AnalysisMaterial = DuplicateAnalysisMaterial(AnalysisMaterial);
            _steelMaterial.Name = Name;
          }
          break;

        case MatType.Timber:
          _timberMaterial = m.CreateTimberMaterial(gradeName);
          if (AnalysisMaterialsModified) {
            _timberMaterial.AnalysisMaterial = DuplicateAnalysisMaterial(AnalysisMaterial);
            _timberMaterial.Name = Name;
          }
          break;

        default:
          _analysisMaterial = DuplicateAnalysisMaterial(AnalysisMaterial);
          break;
      }
    }

    public override string ToString() {
      string code = GetCodeName();
      string id = Id == 0 ? string.Empty : " Grd:" + Id;
      return (code + " " + MaterialType + id + " " + (Name ?? string.Empty)).Trim();
    }

    internal string GetCodeName() {
      string code = string.Empty;
      if (MaterialType == MatType.Concrete && !AnalysisMaterialsModified) {
        code = ConcreteDesignCodeName;
      }

      if (MaterialType == MatType.Steel && !AnalysisMaterialsModified) {
        code = SteelDesignCodeName;
      }

      if (IsCustom) {
        code = "Custom";
      }
      return code;
    }

    internal static List<string> GetGradeNames(
      MatType type, string concreteDesignCode = "", string steelDesignCode = "") {
      Model m = GsaModel.CreateModelFromCodes(concreteDesignCode, steelDesignCode);
      return GetGradeNames(type, m);
    }

    private static List<string> GetGradeNames(MatType type, Model m) {
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

    private void CheckIfAnalysisMaterialIsModified(Model model) {
      List<string> gradeNames = GetGradeNames(MaterialType, model);
      if (gradeNames.Contains(Name)) {
        AnalysisMaterial a = CreateCodeAnalysisMaterial(Name);
        if (AnalysisMaterialsAreEqual(a, AnalysisMaterial)) {
          AnalysisMaterialsModified = false;
          return;
        }

        _gradeName = Name;
      }

      AnalysisMaterialsModified = true;
    }

    private bool AnalysisMaterialsAreEqual(AnalysisMaterial a, AnalysisMaterial b) {
      if (a == null || b == null) {
        return false;
      }

      return Math.Round(a.CoefficientOfThermalExpansion, 11) ==
        Math.Round(b.CoefficientOfThermalExpansion, 11)
        && Math.Round(a.Density, 11) == Math.Round(b.Density, 11)
        && Math.Round(a.ElasticModulus, 11) == Math.Round(b.ElasticModulus, 11)
        && Math.Round(a.PoissonsRatio, 11) == Math.Round(b.PoissonsRatio, 11);
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

    private void SetCodeNames(Model model) {
      ConcreteDesignCodeName = model.ConcreteDesignCode();
      SteelDesignCodeName = model.SteelDesignCode();
    }

    private void SimplifyCodeNames() {
      if (!AnalysisMaterialsModified) {
        switch (MaterialType) {
          case MatType.Concrete:
            SteelDesignCodeName = string.Empty;
            return;
          case MatType.Steel:
            ConcreteDesignCodeName = string.Empty;
            return;

          default:
            SteelDesignCodeName = string.Empty;
            ConcreteDesignCodeName = string.Empty;
            return;
        }
      }
    }
  }
}
