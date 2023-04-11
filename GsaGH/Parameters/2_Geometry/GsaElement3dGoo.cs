using System.Drawing;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Helpers.Graphics;
using OasysGH;
using OasysGH.Parameters;
using Rhino.Geometry;

namespace GsaGH.Parameters {
  /// <summary>
  ///   Goo wrapper class, makes sure <see cref="GsaElement3d" /> can be used in Grasshopper.
  /// </summary>
  public class GsaElement3dGoo : GH_OasysGeometricGoo<GsaElement3d>,
    IGH_PreviewData {
    public GsaElement3dGoo(GsaElement3d item) : base(item) { }

    internal GsaElement3dGoo(GsaElement3d item, bool duplicate) : base(null)
      => Value = duplicate
        ? item.Duplicate()
        : item;

    public static string Name => "Element3D";
    public static string NickName => "E3D";
    public static string Description => "GSA 3D Element(s)";

    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    public override IGH_GeometricGoo Duplicate() => new GsaElement3dGoo(Value);

    public override GeometryBase GetGeometry() => Value.DisplayMesh;

    #region casting methods

    public override bool CastTo<TQ>(ref TQ target) {
      // This function is called when Grasshopper needs to convert this 
      // instance of GsaElement3D into some other type Q.            
      if (base.CastTo(ref target))
        return true;

      if (typeof(TQ).IsAssignableFrom(typeof(Mesh))) {
        target = Value == null
          ? default
          : (TQ)(object)Value.DisplayMesh;
        return true;
      }

      if (typeof(TQ).IsAssignableFrom(typeof(GH_Mesh))) {
        target = Value == null
          ? default
          : (TQ)(object)new GH_Mesh(Value.DisplayMesh);

        return true;
      }

      target = default;
      return false;
    }

    #endregion

    #region transformation methods

    public override IGH_GeometricGoo Transform(Transform xform)
      => new GsaElement3dGoo(Value.Transform(xform));

    public override IGH_GeometricGoo Morph(SpaceMorph xmorph)
      => new GsaElement3dGoo(Value.Morph(xmorph));

    #endregion

    #region drawing methods

    public override void DrawViewportMeshes(GH_PreviewMeshArgs args) => args.Pipeline.DrawMeshShaded(Value.DisplayMesh,
        args.Material.Diffuse == Color.FromArgb(255, 150, 0, 0) // this is a workaround to change colour between selected and not
          ? Colours.Element3dFace
          : Colours.Element2dFaceSelected);

    public override void DrawViewportWires(GH_PreviewWireArgs args) {
      if (Value == null
        || CentralSettings.PreviewMeshEdges == false
        || Value.NgonMesh == null)
        return;

      if (args.Color == Color.FromArgb(255, 150, 0, 0)) // this is a workaround to change colour between selected and not
        args.Pipeline.DrawMeshWires(Value.DisplayMesh, Colours.Element2dEdge, 1);
      else
        args.Pipeline.DrawMeshWires(Value.DisplayMesh, Colours.Element2dEdgeSelected, 2);
    }

    #endregion
  }
}
