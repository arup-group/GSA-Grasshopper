using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using OasysGH;
using OasysGH.Parameters;

using Rhino.Geometry;

namespace GsaGH.Parameters {
  /// <summary>
  ///   Goo wrapper class, makes sure <see cref="IGsaAnnotation" /> can be used in Grasshopper.
  /// </summary>
  public class GsaAnnotationGoo : GH_OasysGeometricGoo<IGsaAnnotation> {
    public static string Description => "GSA Annotation";

    public static string Name => "Annotation";
    public static string NickName => "An";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    public GsaAnnotationGoo(IGsaAnnotation item) : base(item) { }

    public override bool CastTo<TQ>(ref TQ target) {
      return Value.CastTo(ref target);
    }

    public override IGH_GeometricGoo Duplicate() {
      return new GsaAnnotationGoo(Value);
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
