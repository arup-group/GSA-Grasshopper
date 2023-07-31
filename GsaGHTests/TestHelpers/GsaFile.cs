using System.IO;
using System.Linq;
using System.Net;

namespace GsaGHTests.Helper {
  internal static class GsaFile {
    internal static string SteelDesignComplex {
      get {
        if (steelDesignComplex == "") {
          steelDesignComplex = FilePath("Steel_Design_Complex.gwb");
          //= DownloadFile(
          //  "https://samples.oasys-software.com/gsa/10.1/Steel/Steel_Design_Complex.gwb");
        }

        return steelDesignComplex;
      }
    }
    internal static string SteelDesignSimple {
      get {
        if (steelDesignSimple == "") {
          steelDesignSimple = FilePath("Steel_Design_Simple.gwb");
            //= DownloadFile(
            //  "https://samples.oasys-software.com/gsa/10.1/Steel/Steel_Design_Simple.gwb");
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

    private static string FilePath(string fileName) {
      string solutiondir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent
        .FullName;
      return Path.Combine(new string[] {
        solutiondir,
        "TestHelpers",
        fileName
      });
    }
  }
}
