using System;

using GsaAPI.Materials;

namespace GsaGH.Parameters {
  /// <summary>
  /// <para>A Material is used by <see cref="GsaSection"/>s, <see cref="GsaProperty2d"/>s and <see cref="GsaProperty3d"/>s. It is only possible to work with elastic isotropic material types. A Material can either be created as a Standard Material from design code and grade using the <see cref="Components.CreateMaterial"/> component, or as a custom material using the <see cref="Components.CreateCustomMaterial"/> component.</para>
  /// <para>Use the <see cref="Components.GetModelMaterials"/> to get all materials in a <see cref="GsaModel"/> and then use <see cref="Components.EditMaterial"/> in combination with <see cref="Components.MaterialProperties"/> to get information about material properties.</para>
  /// <para>Refer to <see href="https://docs.oasys-software.com/structural/gsa/references/hidr-data-mat-steel.html">Materials</see> to read more.</para>
  /// </summary>
  public abstract class GsaMaterial {
    public virtual AnalysisMaterial AnalysisMaterial { get; set; }
    public string ConcreteDesignCodeName { get; protected set; } = string.Empty;
    public Guid Guid { get; protected set; } = Guid.NewGuid();
    public int Id { get; set; }
    public bool IsFromApi { get; protected set; } = false;
    public MatType MaterialType { get; set; }
    public virtual string Name { get; set; }
    public string SteelDesignCodeName { get; protected set; } = string.Empty;
    public bool IsUserDefined { get; set; }

    protected GsaMaterial() { }

    public GsaMaterial(GsaMaterial other) {
      ConcreteDesignCodeName = other.ConcreteDesignCodeName;
      Guid = System.Guid.NewGuid();
      Id = other.Id;
      MaterialType = other.MaterialType;
      SteelDesignCodeName = other.SteelDesignCodeName;
      IsUserDefined = other.IsUserDefined;
    }

    public override string ToString() {
      string code = string.Empty;
      if (MaterialType == MatType.Concrete && !IsUserDefined) {
        code = ConcreteDesignCodeName;
      }

      if (MaterialType == MatType.Steel && !IsUserDefined) {
        code = SteelDesignCodeName;
      }

      if (MaterialType == MatType.Custom) {
        code = "Custom";
      }

      string id = Id == 0 ? string.Empty : (MaterialType == MatType.Custom) ? $" ID:{Id}" : $" Grd:{Id}";
      return (code + " " + MaterialType + id + " " + (Name ?? string.Empty)).Trim();
    }

    internal void DuplicateAnalysisMaterial(GsaMaterial other) {
      AnalysisMaterial.Name = other.AnalysisMaterial.Name;
      if (IsUserDefined) {
        AnalysisMaterial.CoefficientOfThermalExpansion = other.AnalysisMaterial.CoefficientOfThermalExpansion;
        AnalysisMaterial.Density = other.AnalysisMaterial.Density;
        AnalysisMaterial.ElasticModulus = other.AnalysisMaterial.ElasticModulus;
        AnalysisMaterial.PoissonsRatio = other.AnalysisMaterial.PoissonsRatio;
      }
    }
  }
}
