using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using GsaAPI;
using GsaGH.Parameters;
using OasysUnits;
using OasysUnits.Units;
using static System.Collections.Specialized.BitVector32;

namespace GsaGH.Helpers.Import {
  internal class MaterialQuantities {
    internal IDictionary<int, Mass> SteelQuantities { get; private set; }
      = new ConcurrentDictionary<int, Mass>();
    internal IDictionary<int, Mass> ConcreteQuantities { get; private set; }
      = new ConcurrentDictionary<int, Mass>();
    internal IDictionary<int, Mass> FrpQuantities { get; private set; }
      = new ConcurrentDictionary<int, Mass>();
    internal IDictionary<int, Mass> AluminiumQuantities { get; private set; }
      = new ConcurrentDictionary<int, Mass>();
    internal IDictionary<int, Mass> TimberQuantities { get; private set; }
      = new ConcurrentDictionary<int, Mass>();
    internal IDictionary<int, Mass> GlassQuantities { get; private set; }
      = new ConcurrentDictionary<int, Mass>();
    internal IDictionary<int, Mass> CustomMaterialQuantities { get; private set; }
      = new ConcurrentDictionary<int, Mass>();

    internal MaterialQuantities(GsaModel model, Layer layer, string list) {
      var propertyQuantities = new PropertyQuantities(model, layer, list);
      var steel = SteelQuantities as ConcurrentDictionary<int, Mass>;
      var concrete = ConcreteQuantities as ConcurrentDictionary<int, Mass>;
      var frp = FrpQuantities as ConcurrentDictionary<int, Mass>;
      var aluminium = AluminiumQuantities as ConcurrentDictionary<int, Mass>;
      var timber = TimberQuantities as ConcurrentDictionary<int, Mass>;
      var glass = GlassQuantities as ConcurrentDictionary<int, Mass>;
      var custom = CustomMaterialQuantities as ConcurrentDictionary<int, Mass>;
      ReadOnlyDictionary<int, GsaSectionGoo> sections = model.Sections;
      Parallel.ForEach(propertyQuantities.SectionQuantities, sectionKvp => {
        GsaSection section = sections[sectionKvp.Key].Value;
        MaterialType materialType = section.ApiSection.MaterialType;
        Volume volume = section.SectionProperties.Area * sectionKvp.Value;
        var density = new Density(section.Material.AnalysisMaterial.Density,
        DensityUnit.KilogramPerCubicMeter);
        Mass mass = volume * density;
        switch (section.Material.MaterialType) {
          case MatType.Steel:
            steel.AddOrUpdate(section.Material.Id, mass, (key, oldValue) => oldValue + mass);
            return;
          case MatType.Concrete:
            concrete.AddOrUpdate(section.Material.Id, mass, (key, oldValue) => oldValue + mass);
            return;
          case MatType.Frp:
            frp.AddOrUpdate(section.Material.Id, mass, (key, oldValue) => oldValue + mass);
            return;
          case MatType.Aluminium:
            aluminium.AddOrUpdate(section.Material.Id, mass, (key, oldValue) => oldValue + mass);
            return;
          case MatType.Timber:
            timber.AddOrUpdate(section.Material.Id, mass, (key, oldValue) => oldValue + mass);
            return;
          case MatType.Glass:
            glass.AddOrUpdate(section.Material.Id, mass, (key, oldValue) => oldValue + mass);
            return;
          case MatType.Custom:
            custom.AddOrUpdate(section.Material.Id, mass, (key, oldValue) => oldValue + mass);
            return;
        }
      });

      ReadOnlyDictionary<int, GsaProperty2dGoo> prop2ds = model.Prop2ds;
      Parallel.ForEach(propertyQuantities.Property2dQuantities, propKvp => {
        GsaProperty2d prop2d = prop2ds[propKvp.Key].Value;
        MaterialType materialType = prop2d.ApiProp2d.MaterialType;
        Volume volume = prop2d.Thickness * propKvp.Value;
        var density = new Density(prop2d.Material.AnalysisMaterial.Density,
        DensityUnit.KilogramPerCubicMeter);
        Mass mass = volume * density;
        switch (prop2d.Material.MaterialType) {
          case MatType.Steel:
            steel.AddOrUpdate(prop2d.Material.Id, mass, (key, oldValue) => oldValue + mass);
            return;
          case MatType.Concrete:
            concrete.AddOrUpdate(prop2d.Material.Id, mass, (key, oldValue) => oldValue + mass);
            return;
          case MatType.Frp:
            frp.AddOrUpdate(prop2d.Material.Id, mass, (key, oldValue) => oldValue + mass);
            return;
          case MatType.Aluminium:
            aluminium.AddOrUpdate(prop2d.Material.Id, mass, (key, oldValue) => oldValue + mass);
            return;
          case MatType.Timber:
            timber.AddOrUpdate(prop2d.Material.Id, mass, (key, oldValue) => oldValue + mass);
            return;
          case MatType.Glass:
            glass.AddOrUpdate(prop2d.Material.Id, mass, (key, oldValue) => oldValue + mass);
            return;
          case MatType.Custom:
            custom.AddOrUpdate(prop2d.Material.Id, mass, (key, oldValue) => oldValue + mass);
            return;
        }
      });
    }
  }
}
