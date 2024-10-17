using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using Rhino.Geometry;

namespace GsaGH.Parameters {
  /// <summary>
  /// <para>A Diagram parameter is used to visualise loading or results.</para>
  /// <para>The diagram geometry consist of either a <see cref="GsaArrowheadDiagram"/>,
  /// <see cref="GsaLineDiagram"/> or <see cref="GsaVectorDiagram"/>.</para>
  /// </summary>
  public interface IGsaDiagram : IGH_GeometricGoo, IGH_PreviewData {
    public bool CastTo<TQ>(ref TQ target);
    public GeometryBase GetGeometry();
  }
}
