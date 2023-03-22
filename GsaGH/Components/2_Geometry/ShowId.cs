using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using OasysGH;
using OasysGH.Components;
using Rhino.Geometry;

namespace GsaGH.Components {
  public class ShowId : GH_OasysComponent {
    private List<Point3d> _pts;
    private List<string> _txts;

    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("e01fde68-b591-4ada-b590-9506fc962114");
    public override GH_Exposure Exposure => GH_Exposure.quinary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Properties.Resources.ShowID;

    public ShowId() : base("ShowID",
      "ID",
      "Show the ID of a Node, Element, Member geometry or Result parameters",
      CategoryName.Name(),
      SubCategoryName.Cat2()) { }
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddGenericParameter("Node/Element/Member/Result", "Geo", "Node, Element, Member or Point/Line/Mesh result to get ID for.", GH_ParamAccess.tree);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddPointParameter("Position", "P", "The (centre/mid) location(s) of the object(s)", GH_ParamAccess.tree);
      pManager.HideParameter(0);
      pManager.AddIntegerParameter("Index", "ID", "The objects ID(s)", GH_ParamAccess.tree);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess da) {
      _pts = new List<Point3d>();
      _txts = new List<string>();

      if (!da.GetDataTree(0, out GH_Structure<IGH_Goo> tree)) {
        return;
      }

      var ids = new GH_Structure<GH_Integer>();
      var ghPts = new GH_Structure<GH_Point>();

      foreach (GH_Path path in tree.Paths) {
        foreach (IGH_Goo goo in tree.get_Branch(path)) {
          int id = 0;
          Point3d pt = Point3d.Unset;

          switch (goo) {
            case GsaElement2dGoo e2d:
              for (int i = 0; i < e2d.Value.Mesh.Faces.Count; i++) {
                _txts.Add(e2d.Value.Ids[i].ToString());
                ids.Append(new GH_Integer(e2d.Value.Ids[i]), path);
                _pts.Add(e2d.Value.Mesh.Faces.GetFaceCenter(i));
                ghPts.Append(new GH_Point(e2d.Value.Mesh.Faces.GetFaceCenter(i)), path);
              }
              continue;

            case GsaElement3dGoo e3d:
              for (int i = 0; i < e3d.Value.NgonMesh.Ngons.Count; i++) {
                _txts.Add(e3d.Value.Ids[i].ToString());
                ids.Append(new GH_Integer(e3d.Value.Ids[i]), path);
                _pts.Add(e3d.Value.NgonMesh.Ngons.GetNgonCenter(i));
                ghPts.Append(new GH_Point(e3d.Value.NgonMesh.Ngons.GetNgonCenter(i)), path);
              }
              continue;

            case MeshResultGoo resMesh:
              for (int i = 0; i < resMesh.ElementIds.Count; i++) {
                _txts.Add(resMesh.ElementIds[i].ToString());
                ids.Append(new GH_Integer(resMesh.ElementIds[i]), path);
                if (resMesh.Value.Ngons.Count > 0) {
                  _pts.Add(resMesh.Value.Ngons.GetNgonCenter(i));
                  ghPts.Append(new GH_Point(resMesh.Value.Ngons.GetNgonCenter(i)), path);
                }
                else {
                  _pts.Add(resMesh.Value.Faces.GetFaceCenter(i));
                  ghPts.Append(new GH_Point(resMesh.Value.Faces.GetFaceCenter(i)), path);
                }
              }
              continue;

            case GsaNodeGoo node:
              id = node.Value.Id;
              pt = node.Value.Point;
              break;

            case GsaElement1dGoo e1d:
              id = e1d.Value.Id;
              pt = e1d.Value.Line.PointAtNormalizedLength(0.5);
              break;

            case GsaMember1dGoo m1d:
              id = m1d.Value.Id;
              pt = m1d.Value.PolyCurve.PointAtNormalizedLength(0.5);
              break;

            case GsaMember2dGoo m2d:
              id = m2d.Value.Id;
              m2d.Value.PolyCurve.TryGetPolyline(out Polyline pl);
              pt = pl.CenterPoint();
              break;

            case GsaMember3dGoo m3d:
              id = m3d.Value.Id;
              pt = m3d.Value.SolidMesh.GetBoundingBox(false).Center;
              break;

            case PointResultGoo resPoint:
              id = resPoint.NodeId;
              pt = resPoint.Value;
              break;

            case LineResultGoo resLine:
              id = resLine.ElementId;
              pt = resLine.Value.PointAt(0.5);
              break;

            case VectorResultGoo resVector:
              id = resVector.NodeId;
              pt = resVector.StartingPoint;
              break;

            default:
              this.AddRuntimeWarning("Unable to convert " + goo.TypeName + " to Node, Element (1D/2D/3D), Member (1D/2D/3D) or Point/Line/Mesh result.");
              break;
          }

          _txts.Add(id.ToString());
          ids.Append(new GH_Integer(id), path);
          _pts.Add(pt);
          ghPts.Append(new GH_Point(pt), path);
        }
      }

      da.SetDataTree(0, ghPts);
      da.SetDataTree(1, ids);
    }

    public override void DrawViewportWires(IGH_PreviewArgs args) {
      base.DrawViewportWires(args);

      if (_txts == null) {
        return;
      }

      for (int i = 0; i < _txts.Count; i++) {
        Point2d positionOnTheScreen = args.Viewport.WorldToClient(_pts[i]);
        args.Display.Draw2dText(_txts[i], Attributes.Selected ? args.WireColour_Selected : args.WireColour, positionOnTheScreen, true);
      }
    }

    public override BoundingBox ClippingBox => _pts != null ? new BoundingBox(_pts) : base.ClippingBox;
  }
}
