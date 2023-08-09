using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Parameters.Enums;
using Rhino.Geometry;

namespace GsaGH.Parameters {
  
  public interface IGsaDiagram : IGH_GeometricGoo, IGH_PreviewData {
    public GsaDiagramType DiagramType { get; }
    public bool CastTo<TQ>(ref TQ target);
    public GeometryBase GetGeometry();
  }
}
