using System;
using System.Runtime.Remoting.Messaging;

using Grasshopper.Kernel.Types;

using GsaGH.Parameters;

using Rhino.Geometry;

namespace GsaGH.Helpers {

  public class InvalidGeometryForProperty {

    public static string GetMessage(IGH_Goo inputParameter, bool isLoadPanel, bool isProp2dAssigned) {
      if (isProp2dAssigned) {
        switch (inputParameter) {
          case GH_Mesh mesh:
            if (isLoadPanel) {
              return "Load panel element require curve as the input parameter";
            }
            break;
          case GH_Curve curve: {
              if (!isLoadPanel) {
                return "FE element require mesh geometry as the input parameter";
              }

              if (curve.Value.TryGetPolyline(out Rhino.Geometry.Polyline polyline)) {
                if (polyline.ToArray().Length < 3) {
                  return "A minimum of three points are required to create a 2D load panel";
                }
              } else {
                return "Polyline could not be extracted from the given curve geometry";
              }
            }
            break;
          default: {
              return "Input geometry is not supported to create a 2D element";
            }
        }
      }
      return "";
    }
  }
}
