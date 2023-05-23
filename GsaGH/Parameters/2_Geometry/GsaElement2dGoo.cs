using System.Drawing;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GsaGH.Helpers.Graphics;
using OasysGH;
using OasysGH.Parameters;
using Rhino.Geometry;

namespace GsaGH.Parameters {
  /// <summary>
  ///   Goo wrapper class, makes sure <see cref="GsaElement2d" /> can be used in Grasshopper.
  /// </summary>
  public class GsaElement2dGoo : GH_OasysGeometricGoo<GsaElement2d>, IGH_PreviewData {
    public static string Description => "GSA 2D Element(s)";
    public static string Name => "Element2D";
    public static string NickName => "E2D";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    public GsaElement2dGoo(GsaElement2d item) : base(item) { }

    internal GsaElement2dGoo(GsaElement2d item, bool duplicate) : base(null) {
      Value = duplicate ? item.Duplicate() : item;
    }

    public override bool CastTo<TQ>(ref TQ target) {
      // This function is called when Grasshopper needs to convert this
      // instance of GsaElement2D into some other type Q.
      if (base.CastTo(ref target)) {
        return true;
      }

      if (typeof(TQ).IsAssignableFrom(typeof(Mesh))) {
        target = Value == null ? default : (TQ)(object)Value.Mesh;
        return true;
      }

      if (typeof(TQ).IsAssignableFrom(typeof(GH_Mesh))) {
        target = Value == null ? default : (TQ)(object)new GH_Mesh(Value.Mesh);

        return true;
      }

      target = default;
      return false;
    }

    public override void DrawViewportMeshes(GH_PreviewMeshArgs args) {
      if (Value == null || Value.Mesh == null) {
        return;
      }
      args.Pipeline.DrawMeshShaded(Value.Mesh,
        args.Material.Diffuse
        == Color.FromArgb(255, 150, 0,
          0) // this is a workaround to change colour between selected and not
          ? Colours.Element2dFace : Colours.Element2dFaceSelected);
    }

    public override void DrawViewportWires(GH_PreviewWireArgs args) {
      if (Value == null || CentralSettings.PreviewMeshEdges == false || Value.Mesh == null) {
        return;
      }

      if (args.Color
        == Color.FromArgb(255, 150, 0,
          0)) // this is a workaround to change colour between selected and not
      {
        args.Pipeline.DrawMeshWires(Value.Mesh, Colours.Element2dEdge, 1);
      } else {
        args.Pipeline.DrawMeshWires(Value.Mesh, Colours.Element2dEdgeSelected, 2);
      }
    }

    public override IGH_GeometricGoo Duplicate() {
      return new GsaElement2dGoo(Value);
    }

    public override GeometryBase GetGeometry() {
      return Value == null ? null : (GeometryBase)Value.Mesh;
    }

    public override IGH_GeometricGoo Morph(SpaceMorph xmorph) {
      return new GsaElement2dGoo(Value.Morph(xmorph));
    }

    public override IGH_GeometricGoo Transform(Transform xform) {
      return new GsaElement2dGoo(Value.Transform(xform));
    }
  }
}
