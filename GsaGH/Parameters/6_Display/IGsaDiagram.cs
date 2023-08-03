using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

namespace GsaGH.Parameters {
  public enum DiagramType {
    Vector,
    Line,
    ArrowHead,
    Load
  }
  public interface IGsaDiagram : IGH_GeometricGoo, IGH_PreviewData {
    public DiagramType DiagramType { get; }

    public GeometryBase GetGeometry();
  }
}
