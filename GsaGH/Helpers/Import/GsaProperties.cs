using System.Collections.Generic;

using GsaGH.Parameters;

namespace GsaGH.Helpers.Import {
  internal class GsaProperties {
    internal List<GsaMaterial> Materials { get; set; } = new List<GsaMaterial>();
    internal List<GsaProperty2d> Property2ds { get; set; } = new List<GsaProperty2d>();
    internal List<GsaProperty3d> Property3ds { get; set; } = new List<GsaProperty3d>();
    internal List<GsaSection> Sections { get; set; } = new List<GsaSection>();
    internal List<GsaSpringProperty> SpringProperties { get; set; } = new List<GsaSpringProperty>();

    internal GsaProperties() { }

    internal bool IsNullOrEmpty() {
      return Materials.IsNullOrEmpty() & Property2ds.IsNullOrEmpty() & Property3ds.IsNullOrEmpty() & Sections.IsNullOrEmpty() & SpringProperties.IsNullOrEmpty();
    }
  }
}
