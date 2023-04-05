using System;
using System.Linq;
using GsaAPI;
using GsaGH.Helpers.GsaAPI;

namespace GsaGH.Parameters {
  /// <summary>
  /// Material class, this class defines the basic properties and methods for any <see cref="GsaAPI.AnalysisMaterial"/>
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

    #region fields
    private int _grade = 1;
    private Guid _guid = Guid.NewGuid();
    private int _analProp = 0;
    private AnalysisMaterial _analysisMaterial;
    #endregion

    #region properties
    public MatType MaterialType { get; set; } = MatType.Undef;
    public int GradeProperty {
      get => _grade;
      set {
        _grade = value;
        if (_grade > 0)
          _analProp = 0;
        _guid = Guid.NewGuid();
      }
    }
    public int AnalysisProperty {
      get => _analProp;
      set {
        _analProp = value;
        if (_analProp == 0) {
          return;
        }

        _guid = Guid.NewGuid();
        _grade = 0;
      }
    }
    public Guid Guid => _guid;
    internal AnalysisMaterial AnalysisMaterial {
      get => _analysisMaterial;
      set {
        _analysisMaterial = value;
        _guid = Guid.NewGuid();
      }
    }
    #endregion

    #region constructors
    public GsaMaterial() {
    }

    /// <summary>
    /// 0 : Generic<br></br>
    /// 1 : Steel<br></br>
    /// 2 : Concrete<br></br>
    /// 3 : Aluminium<br></br>
    /// 4 : Glass<br></br>
    /// 5 : FRP<br></br>
    /// 7 : Timber<br></br>
    /// 8 : Fabric<br></br>
    /// </summary>
    /// <param name="materialId"></param>
    public GsaMaterial(int materialId) => MaterialType = (MatType)materialId;

    internal GsaMaterial(GsaSection section, AnalysisMaterial analysisMaterial = null) {
      if (section?.ApiSection == null)
        return;
      if (section.Material != null) {
        if (analysisMaterial == null && section.Material.AnalysisMaterial != null)
          analysisMaterial = section.Material.AnalysisMaterial;
        else if (section.ApiSection.MaterialAnalysisProperty > 0 && section.Material != null && analysisMaterial == null)
          analysisMaterial = section.Material.AnalysisMaterial;
      }

      CreateFromApiObject(section.ApiSection.MaterialType, section.ApiSection.MaterialAnalysisProperty, section.ApiSection.MaterialGradeProperty, analysisMaterial);
    }

    internal GsaMaterial(GsaProp2d prop, AnalysisMaterial analysisMaterial = null) {
      if (prop?.ApiProp2d == null)
        return;
      if (prop.Material != null) {
        if (analysisMaterial == null && prop.Material.AnalysisMaterial != null)
          analysisMaterial = prop.Material.AnalysisMaterial;
        else if (prop.ApiProp2d.MaterialAnalysisProperty > 0 && analysisMaterial == null)
          analysisMaterial = prop.Material.AnalysisMaterial;
      }

      CreateFromApiObject(prop.ApiProp2d.MaterialType, prop.ApiProp2d.MaterialAnalysisProperty, prop.ApiProp2d.MaterialGradeProperty, analysisMaterial);
    }

    internal GsaMaterial(GsaProp3d prop, AnalysisMaterial analysisMaterial = null) {
      if (prop?.ApiProp3d == null)
        return;

      if (prop.Material != null) {
        if (analysisMaterial == null && prop.Material.AnalysisMaterial != null)
          analysisMaterial = prop.Material.AnalysisMaterial;
        else if (prop.ApiProp3d.MaterialAnalysisProperty > 0 && analysisMaterial == null)
          analysisMaterial = prop.Material.AnalysisMaterial;
      }
      CreateFromApiObject(prop.ApiProp3d.MaterialType, prop.ApiProp3d.MaterialAnalysisProperty, prop.ApiProp3d.MaterialGradeProperty, analysisMaterial);
    }
    #endregion

    #region methods
    public GsaMaterial Duplicate() {
      var dup = new GsaMaterial {
        MaterialType = MaterialType,
        _grade = _grade,
        _analProp = _analProp,
      };
      if (_analProp != 0 && _analysisMaterial != null) {
        dup.AnalysisMaterial = new AnalysisMaterial() {
          CoefficientOfThermalExpansion = AnalysisMaterial.CoefficientOfThermalExpansion,
          Density = AnalysisMaterial.Density,
          ElasticModulus = AnalysisMaterial.ElasticModulus,
          PoissonsRatio = AnalysisMaterial.PoissonsRatio
        };
      }
      dup._guid = new Guid(_guid.ToString());
      return dup;
    }

    public override string ToString() {
      string type = Mappings.s_materialTypeMapping.FirstOrDefault(x => x.Value == MaterialType).Key;
      if (_analProp != 0)
        return "ID:" + _analProp + " Custom " + type.Trim() + " Material";
      string id = GradeProperty == 0 ? "" : "Grd:" + GradeProperty + " ";
      return (id + type).Trim();
    }

    private void CreateFromApiObject(MaterialType materialType, int analysisProp, int gradeProp, AnalysisMaterial analysisMaterial) {
      MaterialType = GetType(materialType);
      GradeProperty = gradeProp;
      AnalysisProperty = analysisProp;
      if (!(AnalysisProperty != 0 & analysisMaterial != null)) {
        return;
      }

      _guid = Guid.NewGuid();
      _analysisMaterial = new AnalysisMaterial() {
        CoefficientOfThermalExpansion = analysisMaterial.CoefficientOfThermalExpansion,
        Density = analysisMaterial.Density,
        ElasticModulus = analysisMaterial.ElasticModulus,
        PoissonsRatio = analysisMaterial.PoissonsRatio
      };
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
    #endregion
  }
}
