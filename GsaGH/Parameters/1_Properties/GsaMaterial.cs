using System;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaAPI;
using OasysGH;
using OasysGH.Parameters;

namespace GsaGH.Parameters
{
  /// <summary>
  /// Section class, this class defines the basic properties and methods for any Gsa Section
  /// </summary>
  public class GsaMaterial
  {
    public enum MatType
    {
      UNDEF = -2,
      NONE = -1,
      GENERIC = 0,
      STEEL = 1,
      CONCRETE = 2,
      ALUMINIUM = 3,
      GLASS = 4,
      FRP = 5,
      REBAR = 6,
      TIMBER = 7,
      FABRIC = 8,
      SOIL = 9,
      NUM_MT = 10,
      COMPOUND = 256,
      BAR = 4096,
      TENDON = 4352,
      FRPBAR = 4608,
      CFRP = 4864,
      GFRP = 5120,
      AFRP = 5376,
      ARGFRP = 5632,
      BARMAT = 65280
    }

    public int GradeProperty
    {
      get { return m_grade; }
      set
      {
        m_grade = value;
      }
    }
    public int AnalysisProperty
    {
      get { return m_analProp; }
      set
      {
        m_analProp = value;
        if (m_analProp != 0)
          m_guid = Guid.NewGuid();
      }
    }
    public Guid GUID
    {
      get { return m_guid; }
    }
    internal AnalysisMaterial AnalysisMaterial
    {
      get { return m_AnalysisMaterial; }
      set
      {
        m_AnalysisMaterial = value;
        m_guid = Guid.NewGuid();
      }
    }
    #region fields
    public MatType MaterialType { get; set; } = MatType.UNDEF;
    private AnalysisMaterial m_AnalysisMaterial;
    //int m_idd = 0;
    int m_grade = 1;
    int m_analProp = 0;
    private Guid m_guid;
    #endregion

    #region constructors
    public GsaMaterial()
    {
    }

    private MatType getType(MaterialType materialType)
    {
      MatType m_type = MatType.UNDEF; // custom material

      if (materialType == GsaAPI.MaterialType.GENERIC)
        m_type = MatType.GENERIC;
      if (materialType == GsaAPI.MaterialType.STEEL)
        m_type = MatType.STEEL;
      if (materialType == GsaAPI.MaterialType.CONCRETE)
        m_type = MatType.CONCRETE;
      if (materialType == GsaAPI.MaterialType.TIMBER)
        m_type = MatType.TIMBER;
      if (materialType == GsaAPI.MaterialType.ALUMINIUM)
        m_type = MatType.ALUMINIUM;
      if (materialType == GsaAPI.MaterialType.FRP)
        m_type = MatType.FRP;
      if (materialType == GsaAPI.MaterialType.GLASS)
        m_type = MatType.GLASS;
      if (materialType == GsaAPI.MaterialType.FABRIC)
        m_type = MatType.FABRIC;
      return m_type;
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
    public GsaMaterial(int material_id)
    {
      MaterialType = (MatType)material_id;
    }

    internal GsaMaterial(GsaSection section, AnalysisMaterial analysisMaterial = null)
    {
      if (section == null || section.API_Section == null)
        return;
      if (section.Material != null)
      {
        if (analysisMaterial == null && section.Material.AnalysisMaterial != null)
          analysisMaterial = section.Material.AnalysisMaterial;
        else if (section.API_Section.MaterialAnalysisProperty > 0 && section.Material != null && analysisMaterial == null)
          analysisMaterial = section.Material.AnalysisMaterial;
      }
        
      CreateFromAPI(section.API_Section.MaterialType, section.API_Section.MaterialAnalysisProperty, section.API_Section.MaterialGradeProperty, analysisMaterial);
    }
    internal GsaMaterial(GsaProp2d prop, AnalysisMaterial analysisMaterial = null)
    {
      if (prop == null || prop.API_Prop2d == null)
        return;
      if (prop.Material != null)
      {
        if (analysisMaterial == null && prop.Material.AnalysisMaterial != null)
          analysisMaterial = prop.Material.AnalysisMaterial;
        else if (prop.API_Prop2d.MaterialAnalysisProperty > 0 && analysisMaterial == null)
          analysisMaterial = prop.Material.AnalysisMaterial;
      }
      
      CreateFromAPI(prop.API_Prop2d.MaterialType, prop.API_Prop2d.MaterialAnalysisProperty, prop.API_Prop2d.MaterialGradeProperty, analysisMaterial);
    }
    internal GsaMaterial(GsaProp3d prop, AnalysisMaterial analysisMaterial = null)
    {
      if (prop == null || prop.API_Prop3d == null)
        return;

      if (prop.Material != null)
      {
        if (analysisMaterial == null && prop.Material.AnalysisMaterial != null)
          analysisMaterial = prop.Material.AnalysisMaterial;
        else if (prop.API_Prop3d.MaterialAnalysisProperty > 0 && analysisMaterial == null)
          analysisMaterial = prop.Material.AnalysisMaterial;
      }
      CreateFromAPI(prop.API_Prop3d.MaterialType, prop.API_Prop3d.MaterialAnalysisProperty, prop.API_Prop3d.MaterialGradeProperty, analysisMaterial);
    }
    private void CreateFromAPI(MaterialType materialType, int analysisProp, int gradeProp, AnalysisMaterial analysisMaterial)
    {
      MaterialType = getType(materialType);
      GradeProperty = gradeProp;
      AnalysisProperty = analysisProp;
      if (AnalysisProperty != 0 & analysisMaterial != null)
      {
        m_guid = Guid.NewGuid();
        m_AnalysisMaterial = new AnalysisMaterial()
        {
          CoefficientOfThermalExpansion = analysisMaterial.CoefficientOfThermalExpansion,
          Density = analysisMaterial.Density,
          ElasticModulus = analysisMaterial.ElasticModulus,
          PoissonsRatio = analysisMaterial.PoissonsRatio
        };
      }
    }

    public GsaMaterial Duplicate()
    {
      if (this == null) { return null; }
      GsaMaterial dup = new GsaMaterial();
      dup.MaterialType = MaterialType;
      dup.GradeProperty = m_grade;
      dup.AnalysisProperty = m_analProp;
      if (m_analProp != 0)
      {
        dup.m_guid = Guid.NewGuid();
        dup.AnalysisMaterial = new AnalysisMaterial()
        {
          CoefficientOfThermalExpansion = AnalysisMaterial.CoefficientOfThermalExpansion,
          Density = AnalysisMaterial.Density,
          ElasticModulus = AnalysisMaterial.ElasticModulus,
          PoissonsRatio = AnalysisMaterial.PoissonsRatio
        };
      }
      return dup;
    }
    #endregion

    #region methods
    public override string ToString()
    {
      if (MaterialType == MatType.UNDEF)
      {
        return "Custom Elastic Isotropic";
      }
      string mate = MaterialType.ToString();
      return Char.ToUpper(mate[0]) + mate.Substring(1).ToLower().Replace("_", " ");
    }

    #endregion
  }
}
