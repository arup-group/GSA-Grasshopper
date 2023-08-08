using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

namespace GsaGH.Parameters {
  public enum AnnotationType {
    TextDot,
    Text3d
  }
  public interface IGsaAnnotation : IGH_GeometricGoo, IGH_PreviewData {
    public AnnotationType AnnotationType { get; }
    public string Text { get; }
    public Point3d Location { get; }
    public GeometryBase GetGeometry();
  }
}
