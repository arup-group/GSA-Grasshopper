using System.IO;
using System.Linq;
using System.Net;

namespace GsaGHTests.Helper {
  internal static class GsaFile {
    internal static string SteelDesignComplex {
      get {
        if (steelDesignComplex == "") {
          steelDesignComplex
            = DownloadFile(
              "https://samples.oasys-software.com/gsa/10.2/Steel/Steel_Design_Complex.gwb");
        }

        return steelDesignComplex;
      }
    }
    internal static string SteelDesignSimple {
      get {
        if (steelDesignSimple == "") {
          steelDesignSimple
            = DownloadFile(
              "https://samples.oasys-software.com/gsa/10.2/Steel/Steel_Design_Simple.gwb");
        }

        return steelDesignSimple;
      }
    }
    private static string steelDesignComplex = "";
    private static string steelDesignSimple = "";

    private static string DownloadFile(string url) {
      string path = Path.GetTempPath();
      string fileName = url.Split('/').Last();
      var webClient = new WebClient();
      webClient.DownloadFile(url, path + fileName);
      return path + fileName;
    }
  }
}
