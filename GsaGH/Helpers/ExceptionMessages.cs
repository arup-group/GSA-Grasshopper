using System;
using System.Runtime.Remoting.Messaging;

using Grasshopper.Kernel.Types;

using GsaGH.Parameters;

using Rhino.Geometry;

namespace GsaGH.Helpers {

  public static class InvalidGeometryForProperty {
    public const string NotSupportedType = "Input geometry is not supported to create a 2D element";
    public const string CouldNotBeConvertedToPolyline
      = "Polyline could not be extracted from the given curve geometry";
    public const string WrongGeometryTypeForAnalysis = "FE element require mesh geometry as the input parameter";
    public const string WrongGeometryTypeForLoadPanel = "Load panel element require curve as the input parameter";

    public static string GetMessage(IGH_Goo inputParameter, bool isLoadPanel, bool isProp2dAssigned) {
      if (isProp2dAssigned) {
        switch (inputParameter) {
          case GH_Mesh _:
            if (isLoadPanel) {
              return WrongGeometryTypeForLoadPanel;
            }

            break;
          case GH_Curve curve: {
            if (!isLoadPanel) {
              return WrongGeometryTypeForAnalysis;
            }

            if (!curve.Value.TryGetPolyline(out Polyline _)) {
              return CouldNotBeConvertedToPolyline;
            }
          }
            break;
          default: {
            return NotSupportedType;
          }
        }
      }

      return string.Empty;
    }
  }
}
