using System.Collections.Generic;

using GsaGH.Parameters;

namespace GsaGH.Helpers.Import {
  internal class GsaLoading {
    internal List<GsaGridPlaneSurface> GridPlaneSurfaces { get; set; } = new List<GsaGridPlaneSurface>();
    internal List<IGsaLoad> Loads { get; set; } = new List<IGsaLoad>();
    internal List<GsaLoadCase> LoadCases { get; set; } = new List<GsaLoadCase>();

    internal GsaLoading() { }

    internal bool IsNullOrEmpty() {
      return GridPlaneSurfaces.IsNullOrEmpty() & Loads.IsNullOrEmpty() & LoadCases.IsNullOrEmpty();
    }
  }
}
