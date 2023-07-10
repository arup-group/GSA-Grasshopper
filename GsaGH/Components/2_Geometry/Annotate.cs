﻿using System;
using System.Drawing;
using System.Security.Cryptography;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;
using OasysGH;
using OasysGH.Components;
using Rhino.Geometry;
using Rhino.Input.Custom;

namespace GsaGH.Components {
  public class Annotate : GH_OasysComponent {
    public override Guid ComponentGuid => new Guid("fcad844d-a044-4064-8c6e-f3ea47553941");
    public override GH_Exposure Exposure => GH_Exposure.quinary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.ShowID;
    private GH_Structure<AnnotationGoo> _annotations = new GH_Structure<AnnotationGoo>();
    private Color _color = Color.Empty;
    private GH_Structure<GH_Point> _points = new GH_Structure<GH_Point>();
    private GH_Structure<GH_String> _texts = new GH_Structure<GH_String>();

    public Annotate() : base("Annotate", "A",
      "Show the ID of a Node, Element, or Member parameters, or get Result or Diagram values",
      CategoryName.Name(), SubCategoryName.Cat2()) { }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddGenericParameter("Node/Element/Member/Load/Result/Diagram", "Geo",
        "Node, Element, Member, Point/Line/Mesh result, Result or Load diagram or to get ID for.", GH_ParamAccess.tree);
      pManager.AddColourParameter("Colour", "Co", "[Optional] Colour to override default colour",
        GH_ParamAccess.item);
      pManager[1].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddGenericParameter("Annotations", "Val", "Annotations for the GSA object",
        GH_ParamAccess.tree);
      pManager.AddPointParameter("Position", "P", "The (centre/mid) location(s) of the object(s)",
        GH_ParamAccess.tree);
      pManager.HideParameter(1);
      pManager.AddTextParameter("Text", "T", "The objects ID(s) or the result/diagram value(s)",
        GH_ParamAccess.tree);
    }

    private void AddAnnotation(Point3d pt, string txt, Color color, GH_Path path) {
      if (_color != Color.Empty) {
        color = _color;
      }

      _annotations.Append(new AnnotationGoo(pt, color == Color.Empty ? _color : color, txt), path);
      _points.Append(new GH_Point(pt), path);
      _texts.Append(new GH_String(txt), path);
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      if (!da.GetDataTree(0, out GH_Structure<IGH_Goo> tree)) {
        return;
      }

      if (!da.GetData(1, ref _color)) {
        _color = Color.Empty;
      }

      _annotations = new GH_Structure<AnnotationGoo>();
      _points = new GH_Structure<GH_Point>();
      _texts = new GH_Structure<GH_String>();

      foreach (GH_Path path in tree.Paths) {
        foreach (IGH_Goo goo in tree.get_Branch(path)) {
          switch (goo) {
            case AnnotationGoo annotationGoo:
              AddAnnotation(annotationGoo.Value.Point, annotationGoo.Value.Text,
                annotationGoo.Color, path);
              break;

            case GsaElement2dGoo e2d:
              for (int i = 0; i < e2d.Value.Mesh.Faces.Count; i++) {
                AddAnnotation(e2d.Value.Mesh.Faces.GetFaceCenter(i), e2d.Value.Ids[i].ToString(),
                  Color.Empty, path);
              }

              continue;

            case GsaElement3dGoo e3d:
              for (int i = 0; i < e3d.Value.NgonMesh.Ngons.Count; i++) {
                AddAnnotation(e3d.Value.NgonMesh.Ngons.GetNgonCenter(i),
                  e3d.Value.Ids[i].ToString(), Color.Empty, path);
              }

              continue;

            case MeshResultGoo resMesh:
              for (int i = 0; i < resMesh.ElementIds.Count; i++) {
                if (resMesh.Value.Ngons.Count > 0) {
                  AddAnnotation(resMesh.Value.Ngons.GetNgonCenter(i),
                    resMesh.ElementIds[i].ToString(), Color.Empty, path);
                } else {
                  AddAnnotation(resMesh.Value.Faces.GetFaceCenter(i),
                    resMesh.ElementIds[i].ToString(), Color.Empty, path);
                }
              }

              continue;

            case GsaNodeGoo node:
              AddAnnotation(node.Value.Point, node.Value.Id.ToString(), Color.Empty, path);
              break;

            case GsaElement1dGoo e1d:
              AddAnnotation(e1d.Value.Line.PointAtNormalizedLength(0.5),
                e1d.Value.Id.ToString(), Color.Empty, path);
              break;

            case GsaMember1dGoo m1d:
              AddAnnotation(m1d.Value.PolyCurve.PointAtNormalizedLength(0.5),
                m1d.Value.Id.ToString(), Color.Empty, path);
              break;

            case GsaMember2dGoo m2d:
              m2d.Value.PolyCurve.TryGetPolyline(out Polyline pl);
              AddAnnotation(pl.CenterPoint(), m2d.Value.Id.ToString(), Color.Empty, path);
              break;
            case GsaMember3dGoo m3d:
              AddAnnotation(m3d.Value.SolidMesh.GetBoundingBox(false).Center,
                m3d.Value.Id.ToString(), Color.Empty, path);
              break;

            case PointResultGoo resPoint:
              AddAnnotation(resPoint.Value, resPoint.NodeId.ToString(),
                Color.Empty, path);
              break;

            case LineResultGoo resLine:
              AddAnnotation(resLine.Value.PointAt(0.5), resLine.ElementId.ToString(), 
                Color.Empty, path);
              break;

            case DiagramGoo resVector:
              AddAnnotation(resVector.StartingPoint, string.Empty, Color.Empty, path);
              break;

            default:
              this.AddRuntimeWarning("Unable to convert " + goo.TypeName
                + " to Node, Element (1D/2D/3D), Member (1D/2D/3D) or Point/Line/Mesh result.");
              break;
          }
        }
      }

      da.SetDataTree(0, _annotations);
      da.SetDataTree(1, _points);
      da.SetDataTree(2, _texts);
    }
  }
}
