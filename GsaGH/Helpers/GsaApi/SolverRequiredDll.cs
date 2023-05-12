using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GsaGH.Helpers.GsaApi {
  public static class SolverRequiredDll {
    internal static string LoadedFromPath { get; private set; }
    private static bool canAnalyse;
    private static bool loaded;

    public static bool IsCorrectVersionLoaded() {
      if (!loaded || !canAnalyse) {
        ProcessModuleCollection dlls = Process.GetCurrentProcess().Modules;
        foreach (ProcessModule module in dlls) {
          if (module.ModuleName != "libiomp5md.dll") {
            continue;
          }

          loaded = true;
          string gsaVersion = FileVersionInfo
           .GetVersionInfo(AddReferencePriority.InstallPath + "\\libiomp5md.dll").FileVersion;
          if (FileVersionInfo.GetVersionInfo(module.FileName).FileVersion == gsaVersion) {
            canAnalyse = true;
          } else {
            canAnalyse = false;
            LoadedFromPath = module.FileName;
          }

          break;
        }
      }

      return !loaded || canAnalyse;
    }
  }
}
