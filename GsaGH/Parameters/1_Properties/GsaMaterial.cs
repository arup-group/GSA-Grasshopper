using System;
using System.Collections.Generic;
using System.Linq;
using GsaAPI;
using GsaAPI.Materials;
using GsaGH.Helpers.GsaApi;

namespace GsaGH.Parameters {
  /// <summary>
  ///   Material class, this class defines the basic properties and methods for any <see cref="GsaAPI.AnalysisMaterial" />
  /// </summary>
  public class GsaMaterial {
    public enum MatType {
      Undef = -2,
      None = -1,
      Generic = 0,
      Steel = 1,
      Concrete = 2,
      Aluminium = 3,
      Glass = 4,
      Frp = 5,
      Rebar = 6,
      Timber = 7,
      Fabric = 8,
      Soil = 9,
      NumMt = 10,
      Compound = 0x100,
      Bar = 0x1000,
      Tendon = 4352,
      Frpbar = 4608,
      Cfrp = 4864,
      Gfrp = 5120,
      Afrp = 5376,
      Argfrp = 5632,
      Barmat = 65280,
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

    public Guid Guid => _guid;
    public MatType MaterialType { get; set; } = MatType.Concrete;
    internal AnalysisMaterial AnalysisMaterial {
      get => _analysisMaterial;
      set {
        _analysisMaterial = value;
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

    /// <summary>
    ///   0 : Generic<br></br>
    ///   1 : Steel<br></br>
    ///   2 : Concrete<br></br>
    ///   3 : Aluminium<br></br>
    ///   4 : Glass<br></br>
    ///   5 : FRP<br></br>
    ///   7 : Timber<br></br>
    ///   8 : Fabric<br></br>
    /// </summary>
    /// <param name="typeId"></param>
    public GsaMaterial(int typeId) {
      MaterialType = (MatType)typeId;
    }

    internal GsaMaterial(GsaSection section, AnalysisMaterial analysisMaterial = null) {
      if (section?.ApiSection == null) {
        return;
      }

      if (section.Material != null) {
        if (analysisMaterial == null && section.Material.AnalysisMaterial != null) {
          analysisMaterial = section.Material.AnalysisMaterial;
        } else if (section.ApiSection.MaterialAnalysisProperty > 0 && section.Material != null
          && analysisMaterial == null) {
          analysisMaterial = section.Material.AnalysisMaterial;
        }
      }

      CreateFromApiObject(section.ApiSection.MaterialType,
        section.ApiSection.MaterialAnalysisProperty, section.ApiSection.MaterialGradeProperty,
        analysisMaterial);
    }

    internal GsaMaterial(GsaProp2d prop, AnalysisMaterial analysisMaterial = null) {
      if (prop?.ApiProp2d == null) {
        return;
      }

      if (prop.Material != null) {
        if (analysisMaterial == null && prop.Material.AnalysisMaterial != null) {
          analysisMaterial = prop.Material.AnalysisMaterial;
        } else if (prop.ApiProp2d.MaterialAnalysisProperty > 0 && analysisMaterial == null) {
          analysisMaterial = prop.Material.AnalysisMaterial;
        }
      }

      CreateFromApiObject(prop.ApiProp2d.MaterialType, prop.ApiProp2d.MaterialAnalysisProperty,
        prop.ApiProp2d.MaterialGradeProperty, analysisMaterial);
    }

    internal GsaMaterial(GsaProp3d prop, AnalysisMaterial analysisMaterial = null) {
      if (prop?.ApiProp3d == null) {
        return;
      }

      if (prop.Material != null) {
        if (analysisMaterial == null && prop.Material.AnalysisMaterial != null) {
          analysisMaterial = prop.Material.AnalysisMaterial;
        } else if (prop.ApiProp3d.MaterialAnalysisProperty > 0 && analysisMaterial == null) {
          analysisMaterial = prop.Material.AnalysisMaterial;
        }
      }

      CreateFromApiObject(prop.ApiProp3d.MaterialType, prop.ApiProp3d.MaterialAnalysisProperty,
        prop.ApiProp3d.MaterialGradeProperty, analysisMaterial);
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
          return;

        case MatType.Timber:
          _timberMaterial = m.CreateTimberMaterial(gradeName);
          return;

        default:
          throw new Exception($"Material type {type} does not have standard materials");
      }
    }

    public GsaMaterial Duplicate() {
      var dup = new GsaMaterial {
        MaterialType = MaterialType,
        _id = _id,
      };
      if (_analysisMaterial != null) {
        dup.AnalysisMaterial = new AnalysisMaterial() {
          CoefficientOfThermalExpansion = AnalysisMaterial.CoefficientOfThermalExpansion,
          Density = AnalysisMaterial.Density,
          ElasticModulus = AnalysisMaterial.ElasticModulus,
          PoissonsRatio = AnalysisMaterial.PoissonsRatio,
          Name = AnalysisMaterial.Name,
        };
      }

      dup._guid = new Guid(_guid.ToString());
      return dup;
    }

    public override string ToString() {
      string name = "";
      if (AnalysisMaterial == null || string.IsNullOrEmpty(AnalysisMaterial.Name)) {
        if (_id != 0) {
          name += "Custom ";
        }

        string type = Mappings.materialTypeMapping.FirstOrDefault(x => x.Value == MaterialType).Key;
        name += type.Trim() + " Material";
      } else {
        name = AnalysisMaterial.Name;
      }

      if (_id != 0) {
        return "ID:" + _id + " " + name;
      }

      string id = Id == 0 ? "" : "Grd:" + Id + " ";
      return (id + name).Trim();
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

        case MatType.Rebar:
          return new List<string>(m.GetStandardReinforcementMaterialNames());

        case MatType.Steel:
          return new List<string>(m.GetStandardSteelMaterialNames());

        case MatType.Timber:
          return new List<string>(m.GetStandardTimberMaterialNames());

        default:
          throw new Exception($"Material type {type} does not have standard materials");
      }
    }

    private static MatType GetType(MaterialType materialType) {
      MatType mType = MatType.Undef;

      switch (materialType) {
        case GsaAPI.MaterialType.GENERIC:
          mType = MatType.Generic;
          break;

        case GsaAPI.MaterialType.STEEL:
          mType = MatType.Steel;
          break;

        case GsaAPI.MaterialType.CONCRETE:
          mType = MatType.Concrete;
          break;

        case GsaAPI.MaterialType.TIMBER:
          mType = MatType.Timber;
          break;

        case GsaAPI.MaterialType.ALUMINIUM:
          mType = MatType.Aluminium;
          break;

        case GsaAPI.MaterialType.FRP:
          mType = MatType.Frp;
          break;

        case GsaAPI.MaterialType.GLASS:
          mType = MatType.Glass;
          break;

        case GsaAPI.MaterialType.FABRIC:
          mType = MatType.Fabric;
          break;
      }

      return mType;
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

    private void CreateFromApiObject(
    MaterialType materialType, int analysisProp, int gradeProp,
    AnalysisMaterial analysisMaterial) {
      MaterialType = GetType(materialType);
      Id = gradeProp;
      Id = analysisProp;
      if (!((Id != 0) & (analysisMaterial != null))) {
        return;
      }

      _guid = Guid.NewGuid();
      _analysisMaterial = new AnalysisMaterial() {
        CoefficientOfThermalExpansion = analysisMaterial.CoefficientOfThermalExpansion,
        Density = analysisMaterial.Density,
        ElasticModulus = analysisMaterial.ElasticModulus,
        PoissonsRatio = analysisMaterial.PoissonsRatio,
        Name = analysisMaterial.Name,
      };
    }
  }
}
