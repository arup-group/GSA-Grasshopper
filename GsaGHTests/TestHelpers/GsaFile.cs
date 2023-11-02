using System.IO;

namespace GsaGHTests.Helper {
  internal static class GsaFile {
    private static string steelDesignComplex = "";
    private static string steelDesignSimple = "";
    private static string element2dSimple = "";
    private static string element3dSimple = "";
    private static string springForces = "";
    internal static string SteelDesignComplex {
      get {
        if (steelDesignComplex == "") {
          steelDesignComplex = FilePath("Steel_Design_Complex.gwb");
        }

        return steelDesignComplex;
      }
    }

    internal static string SteelDesignSimple {
      get {
        if (steelDesignSimple == "") {
          steelDesignSimple = FilePath("Steel_Design_Simple.gwb");
        }

        return steelDesignSimple;
      }
    }

    internal static string Element2dSimple {
      get {
        if (element2dSimple == "") {
          element2dSimple = FilePath("Element2d_Simple.gwb");
        }

        return element2dSimple;
      }
    }

    internal static string Element3dSimple {
      get {
        if (element3dSimple == "") {
          element3dSimple = FilePath("Element3d_Simple.gwb");
        }

        return element3dSimple;
      }
    }
    internal static string SpringForces {
      get {
        if (springForces == "") {
          springForces = FilePath("spring-reaction-forces.gwb");
        }

        return springForces;
      }
    }

    private static string FilePath(string fileName) {
      string solutiondir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent
       .FullName;
      return Path.Combine(new string[] {
        solutiondir,
        "TestHelpers",
        fileName,
      });
    }
  }
}
