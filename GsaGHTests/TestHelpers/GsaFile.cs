using System.IO;

namespace GsaGHTests.Helper {
  internal static class GsaFile {
    private static string _steelDesignComplex = string.Empty;
    private static string _steelDesignSimple = string.Empty;
    private static string _element2dSimple = string.Empty;
    private static string _element3dSimple = string.Empty;
    private static string _springForces = string.Empty;
    private static string _steelFootfall = string.Empty;
    private static string _element2dMultiPropsFromParentMember = string.Empty;
    private static string _basicFrame = string.Empty;
    private static string _assemblySimple = string.Empty;
    private static string _assemblyByStorey = string.Empty;

    internal static string SteelDesignComplex {
      get {
        if (_steelDesignComplex == string.Empty) {
          _steelDesignComplex = FilePath("Steel_Design_Complex.gwb");
        }

        return _steelDesignComplex;
      }
    }

    internal static string SteelDesignSimple {
      get {
        if (_steelDesignSimple == string.Empty) {
          _steelDesignSimple = FilePath("Steel_Design_Simple.gwb");
        }

        return _steelDesignSimple;
      }
    }

    internal static string Element2dSimple {
      get {
        if (_element2dSimple == string.Empty) {
          _element2dSimple = FilePath("Element2d_Simple.gwb");
        }

        return _element2dSimple;
      }
    }

    internal static string Element2dMultiPropsParentMember {
      get {
        if (_element2dMultiPropsFromParentMember == string.Empty) {
          _element2dMultiPropsFromParentMember = FilePath("Element2dMultiPropsFromParentMember.gwb");
        }

        return _element2dMultiPropsFromParentMember;
      }
    }

    internal static string Element3dSimple {
      get {
        if (_element3dSimple == string.Empty) {
          _element3dSimple = FilePath("Element3d_Simple.gwb");
        }

        return _element3dSimple;
      }
    }
    internal static string SpringForces {
      get {
        if (_springForces == string.Empty) {
          _springForces = FilePath("spring-reaction-forces.gwb");
        }

        return _springForces;
      }
    }
    internal static string SteelFootfall {
      get {
        if (_steelFootfall == string.Empty) {
          _steelFootfall = FilePath("footfall_steel.gwb");
        }

        return _steelFootfall;
      }
    }
    internal static string BasicFrame {
      get {
        if (_basicFrame == string.Empty) {
          _basicFrame = FilePath("basicFrame.gwb");
        }

        return _basicFrame;
      }
    }

    internal static string AssemblySimple {
      get {
        if (_assemblySimple == string.Empty) {
          _assemblySimple = FilePath("assembly-simple.gwb");
        }

        return _assemblySimple;
      }
    }

    internal static string AssemblyByStorey {
      get {
        if (_assemblyByStorey == string.Empty) {
          _assemblyByStorey = FilePath("assembly-by-storey.gwb");
        }

        return _assemblyByStorey;
      }
    }

    internal static string FabricMaterialModel {
      get {
        if (_assemblyByStorey == string.Empty) {
          _assemblyByStorey = FilePath("fabric_material_model.gwb");
        }

        return _assemblyByStorey;
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
