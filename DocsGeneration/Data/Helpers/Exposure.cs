using Grasshopper.Kernel;

namespace DocsGeneration.Data.Helpers {
  public class Exposure {
    public static int GetExposure(GH_Exposure exposure) {
      if (exposure.HasFlag(GH_Exposure.hidden)) {
        return -1;
      }

      if (exposure.HasFlag(GH_Exposure.primary)) {
        return 1;
      }

      if (exposure.HasFlag(GH_Exposure.secondary)) {
        return 2;
      }

      if (exposure.HasFlag(GH_Exposure.tertiary)) {
        return 3;
      }

      if (exposure.HasFlag(GH_Exposure.quarternary)) {
        return 4;
      }

      if (exposure.HasFlag(GH_Exposure.quinary)) {
        return 5;
      }

      if (exposure.HasFlag(GH_Exposure.senary)) {
        return 6;
      }

      if (exposure.HasFlag(GH_Exposure.septenary)) {
        return 7;
      }

      return -1;
    }
  }
}
