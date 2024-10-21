using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using OasysGH;
using OasysGH.Parameters;

using Rhino.Geometry;

namespace GsaGH.Parameters {
  /// <summary>
  ///   Goo wrapper class, makes sure <see cref="IGsaDiagram" /> can be used in Grasshopper.
  /// </summary>
  public class GsaDiagramGoo : GH_OasysGeometricGoo<IGsaDiagram> {
    public static string Description => "GSA Diagram";
    public static string Name => "Diagram";
    public static string NickName => "Dgm";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    public GsaDiagramGoo(IGsaDiagram item) : base(item) { }

    public override bool CastTo<TQ>(ref TQ target) {
      return Value.CastTo(ref target);
    }
    public override IGH_GeometricGoo Duplicate() {
      return new GsaDiagramGoo(Value);
    }

    public override void DrawViewportMeshes(GH_PreviewMeshArgs args) {
      Value.DrawViewportMeshes(args);
    }

    public override void DrawViewportWires(GH_PreviewWireArgs args) {
      Value.DrawViewportWires(args);
    }

    public override GeometryBase GetGeometry() {
      return Value.GetGeometry();
    }

    public override IGH_GeometricGoo Morph(SpaceMorph xmorph) {
      return Value.Morph(xmorph);
    }

    public override IGH_GeometricGoo Transform(Transform xform) {
      return Value.Transform(xform);
    }
  }
}
