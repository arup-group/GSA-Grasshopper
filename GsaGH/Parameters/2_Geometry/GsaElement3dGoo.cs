using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaAPI;
using OasysGH;
using OasysGH.Parameters;
using Rhino.Geometry;

namespace GsaGH.Parameters
{
  /// <summary>
  /// Goo wrapper class, makes sure <see cref="GsaElement3d"/> can be used in Grasshopper.
  /// </summary>
  public class GsaElement3dGoo : GH_OasysGeometricGoo<GsaElement3d>, IGH_PreviewData
  {
    public static string Name => "Element3D";
    public static string NickName => "E3D";
    public static string Description => "GSA 3D Element(s)";

    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    public GsaElement3dGoo(GsaElement3d item) : base(item) { }
    internal GsaElement3dGoo(GsaElement3d item, bool duplicate) : base(null)
    {
      if (duplicate)
        this.Value = item.Duplicate();
      else
        this.Value = item;
    }

    public override IGH_GeometricGoo Duplicate() => new GsaElement3dGoo(this.Value);

    public override GeometryBase GetGeometry() => this.Value.DisplayMesh;

    #region casting methods
    public override bool CastTo<Q>(ref Q target)
    {
      // This function is called when Grasshopper needs to convert this 
      // instance of GsaElement3D into some other type Q.            
      if (base.CastTo<Q>(ref target))
        return true;

      //Cast to Mesh
      if (typeof(Q).IsAssignableFrom(typeof(Mesh)))
      {
        if (Value == null)
          target = default;
        else
          target = (Q)(object)Value.DisplayMesh;
        return true;
      }

      if (typeof(Q).IsAssignableFrom(typeof(GH_Mesh)))
      {
        if (Value == null)
          target = default;
        else
        {
          target = (Q)(object)new GH_Mesh(Value.DisplayMesh);
        }

        return true;
      }

      target = default;
      return false;
    }
    #endregion

    #region transformation methods
    public override IGH_GeometricGoo Transform(Transform xform)
    {
      return new GsaElement3dGoo(Value.Transform(xform));
    }

    public override IGH_GeometricGoo Morph(SpaceMorph xmorph)
    {
      return new GsaElement3dGoo(Value.Morph(xmorph));
    }
    #endregion

    #region drawing methods
    public override void DrawViewportMeshes(GH_PreviewMeshArgs args)
    {
      //Draw shape.
      if (args.Material.Diffuse == System.Drawing.Color.FromArgb(255, 150, 0, 0)) // this is a workaround to change colour between selected and not
      {
        args.Pipeline.DrawMeshShaded(Value.DisplayMesh, Helpers.Graphics.Colours.Element3dFace);
      }
      else
        args.Pipeline.DrawMeshShaded(Value.DisplayMesh, Helpers.Graphics.Colours.Element2dFaceSelected);
    }
    public override void DrawViewportWires(GH_PreviewWireArgs args)
    {
      if (Value == null) { return; }
      if (Grasshopper.CentralSettings.PreviewMeshEdges == false) { return; }

      //Draw lines
      if (Value.NgonMesh != null)
      {
        if (args.Color == System.Drawing.Color.FromArgb(255, 150, 0, 0)) // this is a workaround to change colour between selected and not
        {
          args.Pipeline.DrawMeshWires(Value.DisplayMesh, Helpers.Graphics.Colours.Element2dEdge, 1);
        }
        else
        {
          args.Pipeline.DrawMeshWires(Value.DisplayMesh, Helpers.Graphics.Colours.Element2dEdgeSelected, 2);
        }
      }
    }
    #endregion
  }
}
