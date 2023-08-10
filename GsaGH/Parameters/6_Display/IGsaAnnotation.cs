using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Parameters.Enums;
using Rhino.Geometry;

namespace GsaGH.Parameters {
  
  public interface IGsaAnnotation : IGH_GeometricGoo, IGH_PreviewData {
    public GsaAnnotationType AnnotationType { get; }
    public string Text { get; }
    public Point3d Location { get; }
    public bool CastTo<TQ>(ref TQ target);
    public GeometryBase GetGeometry();
  }
}