using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using Rhino.Geometry;

namespace GsaGH.Parameters {
  /// <summary>
  /// <para>An Annotation parameter can display IDs of objects or values of results.</para>
  /// <para>The Annotation parameter can be either a <see cref="GsaAnnotationDot"/> or a <see cref="GsaAnnotation3d"/> type.</para>
  /// </summary>
  public interface IGsaAnnotation : IGH_GeometricGoo, IGH_PreviewData {
    public string Text { get; }
    public Point3d Location { get; }
    public bool CastTo<TQ>(ref TQ target);
    public GeometryBase GetGeometry();
  }
}
