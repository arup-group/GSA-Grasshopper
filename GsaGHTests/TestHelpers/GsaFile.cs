using System.IO;
using System.Linq;
using System.Net;

namespace GsaGHTests.Helper {
  internal static class GsaFile {
    private static string steelDesignComplex = string.Empty;
    private static string steelDesignSimple = string.Empty;
    internal static string SteelDesignComplex {
      get {
        if (string.IsNullOrEmpty(steelDesignComplex)) {
          steelDesignComplex
            = DownloadFile(
              "https://samples.oasys-software.com/gsa/10.1/Steel/Steel_Design_Complex.gwb");
        }

        return steelDesignComplex;
      }
    }
    internal static string SteelDesignSimple {
      get {
        if (string.IsNullOrEmpty(steelDesignSimple)) {
          steelDesignSimple
            = DownloadFile(
              "https://samples.oasys-software.com/gsa/10.1/Steel/Steel_Design_Simple.gwb");
        }

        return steelDesignSimple;
      }
    }

    private static string DownloadFile(string url) {
      string path = Path.GetTempPath();
      string fileName = url.Split('/').Last();
      var webClient = new WebClient();
      webClient.DownloadFile(url, path + fileName);
      return path + fileName;
    }
  }
}
