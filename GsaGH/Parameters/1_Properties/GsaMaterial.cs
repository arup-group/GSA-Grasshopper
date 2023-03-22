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
      get {
        return _grade;
      }
      set {
        _grade = value;
        if (_grade > 0)
          _analProp = 0;
        _guid = Guid.NewGuid();
      }
    }
    public int AnalysisProperty {
      get {
        return _analProp;
      }
      set {
        _analProp = value;
        if (_analProp != 0) {
          _guid = Guid.NewGuid();
          _grade = 0;
        }
      }
    }
    public Guid Guid {
      get {
        return _guid;
      }
    }
    internal AnalysisMaterial AnalysisMaterial {
      get {
        return _analysisMaterial;
      }
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
    /// 0 : Generic
    /// 1 : Steel
    /// 2 : Concrete
    /// 3 : Aluminium
    /// 4 : Glass
    /// 5 : FRP
    /// 7 : Timber
    /// 8 : Fabric
    /// </summary>
    /// <param name="material_id"></param>
    public GsaMaterial(int material_id) {
      MaterialType = (MatType)material_id;
    }

    internal GsaMaterial(GsaSection section, AnalysisMaterial analysisMaterial = null) {
      if (section == null || section.API_Section == null)
        return;
      if (section.Material != null) {
        if (analysisMaterial == null && section.Material.AnalysisMaterial != null)
          analysisMaterial = section.Material.AnalysisMaterial;
        else if (section.API_Section.MaterialAnalysisProperty > 0 && section.Material != null && analysisMaterial == null)
          analysisMaterial = section.Material.AnalysisMaterial;
      }

      CreateFromApiObject(section.API_Section.MaterialType, section.API_Section.MaterialAnalysisProperty, section.API_Section.MaterialGradeProperty, analysisMaterial);
    }

    internal GsaMaterial(GsaProp2d prop, AnalysisMaterial analysisMaterial = null) {
      if (prop == null || prop.API_Prop2d == null)
        return;
      if (prop.Material != null) {
        if (analysisMaterial == null && prop.Material.AnalysisMaterial != null)
          analysisMaterial = prop.Material.AnalysisMaterial;
        else if (prop.API_Prop2d.MaterialAnalysisProperty > 0 && analysisMaterial == null)
          analysisMaterial = prop.Material.AnalysisMaterial;
      }

      CreateFromApiObject(prop.API_Prop2d.MaterialType, prop.API_Prop2d.MaterialAnalysisProperty, prop.API_Prop2d.MaterialGradeProperty, analysisMaterial);
    }

    internal GsaMaterial(GsaProp3d prop, AnalysisMaterial analysisMaterial = null) {
      if (prop == null || prop.API_Prop3d == null)
        return;

      if (prop.Material != null) {
        if (analysisMaterial == null && prop.Material.AnalysisMaterial != null)
          analysisMaterial = prop.Material.AnalysisMaterial;
        else if (prop.API_Prop3d.MaterialAnalysisProperty > 0 && analysisMaterial == null)
          analysisMaterial = prop.Material.AnalysisMaterial;
      }
      CreateFromApiObject(prop.API_Prop3d.MaterialType, prop.API_Prop3d.MaterialAnalysisProperty, prop.API_Prop3d.MaterialGradeProperty, analysisMaterial);
    }
    #endregion

    #region methods
    public GsaMaterial Duplicate() {
      var dup = new GsaMaterial();
      dup.MaterialType = MaterialType;
      dup._grade = _grade;
      dup._analProp = _analProp;
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
      else {
        string id = GradeProperty == 0 ? "" : "Grd:" + GradeProperty + " ";
        return (id + type).Trim();
      }
    }

    private void CreateFromApiObject(MaterialType materialType, int analysisProp, int gradeProp, AnalysisMaterial analysisMaterial) {
      MaterialType = GetType(materialType);
      GradeProperty = gradeProp;
      AnalysisProperty = analysisProp;
      if (AnalysisProperty != 0 & analysisMaterial != null) {
        _guid = Guid.NewGuid();
        _analysisMaterial = new AnalysisMaterial() {
          CoefficientOfThermalExpansion = analysisMaterial.CoefficientOfThermalExpansion,
          Density = analysisMaterial.Density,
          ElasticModulus = analysisMaterial.ElasticModulus,
          PoissonsRatio = analysisMaterial.PoissonsRatio
        };
      }
    }

    private MatType GetType(MaterialType materialType) {
      MatType m_type = MatType.Undef; // custom material

      if (materialType == GsaAPI.MaterialType.GENERIC)
        m_type = MatType.Generic;
      if (materialType == GsaAPI.MaterialType.STEEL)
        m_type = MatType.Steel;
      if (materialType == GsaAPI.MaterialType.CONCRETE)
        m_type = MatType.Concrete;
      if (materialType == GsaAPI.MaterialType.TIMBER)
        m_type = MatType.Timber;
      if (materialType == GsaAPI.MaterialType.ALUMINIUM)
        m_type = MatType.Aluminium;
      if (materialType == GsaAPI.MaterialType.FRP)
        m_type = MatType.Frp;
      if (materialType == GsaAPI.MaterialType.GLASS)
        m_type = MatType.Glass;
      if (materialType == GsaAPI.MaterialType.FABRIC)
        m_type = MatType.Fabric;
      return m_type;
    }
    #endregion
  }
}
