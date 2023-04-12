using System.IO;
using System.Linq;
using System.Net;

namespace GsaGHTests.Helper {
  internal static class GsaFile {
    internal static string SteelDesignComplex {
      get {
        if (s_steelDesignComplex == "")
          s_steelDesignComplex
            = DownloadFile(
              "https://samples.oasys-software.com/gsa/10.1/Steel/Steel_Design_Complex.gwb");
        return s_steelDesignComplex;
      }
    }
    internal static string SteelDesignSimple {
      get {
        if (s_steelDesignSimple == "")
          s_steelDesignSimple
            = DownloadFile(
              "https://samples.oasys-software.com/gsa/10.1/Steel/Steel_Design_Simple.gwb");
        return s_steelDesignSimple;
      }
    }
    private static string s_steelDesignComplex = "";
    private static string s_steelDesignSimple = "";
    private static string DownloadFile(string url) {
      string path = Path.GetTempPath();
      string fileName = url.Split('/')
        .Last();
      var webClient = new WebClient();
      webClient.DownloadFile(url, path + fileName);
      return path + fileName;
    }
  }
}
