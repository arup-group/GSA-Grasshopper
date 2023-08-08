using System;
using System.Collections.Generic;
using System.Drawing;
using GH_IO.Serialization;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;
using OasysGH;
using OasysGH.Components;
using OasysGH.UI;
using Rhino.Geometry;

namespace GsaGH.Components {
  public class AnnotateDetailed : GH_OasysDropDownComponent {
    public override Guid ComponentGuid => new Guid("8784697d-56de-4349-a2b1-639910254f53");
    public override GH_Exposure Exposure => GH_Exposure.primary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.AnnotateDetailed;
    private GH_Structure<GsaAnnotationGoo> _annotations = new GH_Structure<GsaAnnotationGoo>();
    private Color _color = Color.Empty;
    private GH_Structure<GH_Point> _points = new GH_Structure<GH_Point>();
    private GH_Structure<GH_String> _texts = new GH_Structure<GH_String>();
    private bool _showId = false;
    private bool _showName = true;
    private bool _showProperty = true;
    private bool _showMaterial = false;
    private bool _text3d = false;
    private List<bool> _initialCheckState;
    private readonly List<string> _checkboxTexts = new List<string>() {
      "Object ID", "Object Name", "Property Info", "Material Info"
    };

    public AnnotateDetailed() : base("Annotate Detailed", "An+",
      "Show the detailed information of Element or Member parameters",
      CategoryName.Name(), SubCategoryName.Cat6()) { }

    public override void CreateAttributes() {
      if (!_isInitialised) {
        InitialiseDropdowns();
      }

      m_attributes = new DropDownCheckBoxesComponentAttributes(this, SetSelected, _dropDownItems,
        _selectedItems, SetCheckBoxes, _initialCheckState, _checkboxTexts, _spacerDescriptions);
    }

    public override bool Read(GH_IReader reader) {
      _showId = reader.GetBoolean("id");
      _showName = reader.GetBoolean("name");
      _showProperty = reader.GetBoolean("property");
      _showMaterial = reader.GetBoolean("material");
      _text3d = reader.GetBoolean("text3d");
      return base.Read(reader);
    }

    public void SetCheckBoxes(List<bool> selection) {
      _showId = selection[0];
      _showName = selection[1];
      _showProperty = selection[2];
      _showMaterial = selection[3];
      base.UpdateUI();
    }


    public override bool Write(GH_IWriter writer) {
      writer.SetBoolean("id", _showId);
      writer.SetBoolean("name", _showName);
      writer.SetBoolean("property", _showProperty);
      writer.SetBoolean("material", _showMaterial);
      writer.SetBoolean("text3d", _text3d);
      return base.Write(writer);
    }

    public override void SetSelected(int i, int j) {
      _selectedItems[i] = _dropDownItems[i][j];
      if (j == 0) {
        _text3d = false;
      } else {
        _text3d = true;
      }
    }

    protected override void InitialiseDropdowns() {
      _spacerDescriptions = new List<string>(new[] {
        "Type",
        "Settings",
      });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      _dropDownItems.Add(new List<string>() {
        "Regular Dot",
        "TextTag 3D"
      });
      _selectedItems.Add(_dropDownItems[0][0]);

      _initialCheckState = new List<bool>() {
        _showId,
        _showName,
        _showProperty,
        _showMaterial,
      };

      _isInitialised = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddGenericParameter("Element/Member", "Geo",
        "Element or Member to annotate details of.", GH_ParamAccess.tree);
      pManager.AddNumberParameter("Size", "S", "Size of annotation", GH_ParamAccess.item);
      pManager.AddColourParameter("Colour", "C", "Optional colour of annotation",
        GH_ParamAccess.item);
      pManager[1].Optional = true;
      pManager[2].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaAnnotationParameter(), "Annotations",
        "An", "Annotations for the GSA object", GH_ParamAccess.tree);
      pManager.AddPointParameter("Position", "P", "The (centre/mid) location(s) of the object(s)",
        GH_ParamAccess.tree);
      pManager.HideParameter(1);
      pManager.AddTextParameter("Text", "T", "The objects ID(s) or the result/diagram value(s)",
        GH_ParamAccess.tree);
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      if (!da.GetDataTree(0, out GH_Structure<IGH_Goo> tree)) {
        return;
      }

      double size = 1;
      if (!da.GetData(1, ref size)) {
        size = _text3d ? 1 : 14;
      }

      if (!da.GetData(2, ref _color)) {
        _color = Color.Empty;
      }

      _annotations = new GH_Structure<GsaAnnotationGoo>();
      _points = new GH_Structure<GH_Point>();
      _texts = new GH_Structure<GH_String>();

      foreach (GH_Path path in tree.Paths) {
        foreach (IGH_Goo goo in tree.get_Branch(path)) {
          switch (goo) {
            case GsaElement2dGoo e2d:
              for (int i = 0; i < e2d.Value.Mesh.Faces.Count; i++) {
                if (_text3d) {
                  if (e2d.Value.Mesh.FaceNormals.Count == 0) {
                    e2d.Value.Mesh.RebuildNormals();
                  }
                  AddAnnotation3d(
                    new Plane(e2d.Value.Mesh.Faces.GetFaceCenter(i), e2d.Value.Mesh.FaceNormals[i]),
                    CreateText(e2d, path, i), e2d.Value.Colours[i], size, path);
                } else {
                  AddAnnotationDot(e2d.Value.Mesh.Faces.GetFaceCenter(i), CreateText(e2d, path, i),
                  e2d.Value.Colours[i], size, path);
                }
              }
              continue;

            case GsaElement3dGoo e3d:
              for (int i = 0; i < e3d.Value.NgonMesh.Ngons.Count; i++) {
                if (_text3d) {
                  AddAnnotation3d(new Plane(e3d.Value.NgonMesh.Ngons.GetNgonCenter(i), Vector3d.ZAxis),
                    CreateText(e3d, path, i), e3d.Value.Colours[i], size, path);
                } else {
                  AddAnnotationDot(e3d.Value.NgonMesh.Ngons.GetNgonCenter(i), CreateText(e3d, path, i),
                  e3d.Value.Colours[i], size, path);
                }
              }
              continue;

            case GsaElement1dGoo e1d:
              if (_text3d) {
                AddAnnotation3d(
                  TangentToNormal(e1d.Value.Line, size), CreateText(e1d, path), e1d.Value.Colour, size, path);
              } else {
                AddAnnotationDot(e1d.Value.Line.PointAtNormalizedLength(0.5),
                CreateText(e1d, path), e1d.Value.Colour, size, path);
              }
              break;

            case GsaMember1dGoo m1d:
              if (_text3d) {
                AddAnnotation3d(
                  TangentToNormal(m1d.Value.PolyCurve, size), CreateText(m1d, path), m1d.Value.Colour,
                  size, path);
              } else {
                AddAnnotationDot(m1d.Value.PolyCurve.PointAtNormalizedLength(0.5),
                CreateText(m1d, path), m1d.Value.Colour, size, path);
              }
              break;

            case GsaMember2dGoo m2d:
              m2d.Value.PolyCurve.TryGetPolyline(out Polyline pl);
              if (_text3d) {
                Plane.FitPlaneToPoints(pl, out Plane pln);
                pln.Origin = pl.CenterPoint();
                AddAnnotation3d(pln, CreateText(m2d, path), m2d.Value.Colour, size, path);
              } else {
                AddAnnotationDot(pl.CenterPoint(), CreateText(m2d, path), m2d.Value.Colour, size, path);
              }
              break;
            case GsaMember3dGoo m3d:
              if (_text3d) {
                AddAnnotation3d(m3d.Value.SolidMesh.GetBoundingBox(false).Center,
                  CreateText(m3d, path), m3d.Value.Colour,
                  size, path);
              } else {
                AddAnnotationDot(m3d.Value.SolidMesh.GetBoundingBox(false).Center,
                  CreateText(m3d, path), m3d.Value.Colour, size, path);
              }
              break;

            default:
              this.AddRuntimeWarning("Unable to convert " + goo.TypeName
                + " to Node, Element (1D/2D/3D), Member (1D/2D/3D) or Point/Line/Mesh result.");
              _annotations.Append(null, path);
              _points.Append(null, path);
              _texts.Append(null, path);
              break;
          }
        }
      }

      da.SetDataTree(0, _annotations);
      da.SetDataTree(1, _points);
      da.SetDataTree(2, _texts);
    }
    private void AddAnnotation3d(Point3d origin, string txt, Color color, double size, GH_Path path) {
      var pln = new Plane(origin, Vector3d.XAxis, Vector3d.YAxis);
      AddAnnotation3d(pln, txt, color, size, path);
    }

    private void AddAnnotation3d(Point3d origin, Vector3d xAxis, Vector3d yAxis, string txt, Color color, double size, GH_Path path) {
      var pln = new Plane(origin, xAxis, yAxis);
      AddAnnotation3d(pln, txt, color, size, path);
    }
    private void AddAnnotation3d(Plane pln, string txt, Color color, double size, GH_Path path) {
      if (_color != Color.Empty) {
        color = _color;
      }

      _annotations.Append(new GsaAnnotationGoo(
        new GsaAnnotation3d(pln, color == Color.Empty ? _color : color, txt, size)), path);
      _points.Append(new GH_Point(pln.Origin), path);
    }

    private void AddAnnotationDot(Point3d pt, string txt, Color color, double size, GH_Path path) {
      if (_color != Color.Empty) {
        color = _color;
      }
      var dot = new GsaAnnotationDot(pt, color == Color.Empty ? _color : color, txt);
      dot.Value.FontHeight = (int)size;
      _annotations.Append(new GsaAnnotationGoo(dot), path);
      _points.Append(new GH_Point(pt), path);
    }

    private Plane TangentToNormal(LineCurve line, double size) {
      var tangent = new Vector3d(
        line.PointAtEnd.X - line.PointAtStart.X,
        line.PointAtEnd.Y - line.PointAtStart.Y,
        line.PointAtEnd.Z - line.PointAtStart.Z);
      Plane pln = TangentToNormal(tangent);
      pln.Origin = line.PointAtNormalizedLength(0.5);
      var offset = new Vector3d(pln.Normal);
      offset.Unitize();
      offset *= size;
      var transform = Transform.Translation(offset);
      pln.Transform(transform);
      return pln;
    }

    private Plane TangentToNormal(PolyCurve crv, double size) {
      Plane pln = TangentToNormal(crv.TangentAt(0.5));
      pln.Origin = crv.PointAtNormalizedLength(0.5);
      var offset = new Vector3d(pln.Normal);
      offset.Unitize();
      offset *= size;
      var transform = Transform.Translation(offset);
      pln.Transform(transform);
      return pln;
    }

    private Plane TangentToNormal(Vector3d tangent) {
      tangent.Unitize();
      Vector3d dotVec = Math.Round(tangent.Z, 6) == 1.0 ? Vector3d.XAxis : Vector3d.ZAxis;
      var dotVec2 = Vector3d.CrossProduct(dotVec, tangent);
      var dotVec3 = Vector3d.CrossProduct(tangent, dotVec2);
      return new Plane(Point3d.Origin, tangent, dotVec3);
    }

    private string CreateText(IGH_Goo goo, GH_Path path, int i = 0) {
      int id = 0;
      string name = string.Empty;
      string prop = string.Empty;
      string mat = string.Empty;
      switch (goo) {
        case GsaElement2dGoo e2d:
          id = e2d.Value.Ids[i];
          name = e2d.Value.Names[i];
          prop = e2d.Value.Prop2ds[i].ToString();
          mat = e2d.Value.Prop2ds[i].Material.ToString();
          break;

        case GsaElement3dGoo e3d:
          id = e3d.Value.Ids[i];
          name = e3d.Value.Names[i];
          prop = e3d.Value.Prop3ds[i].ToString();
          mat = e3d.Value.Prop3ds[i].Material.ToString();
          break;

        case GsaElement1dGoo e1d:
          id = e1d.Value.Id;
          name = e1d.Value.Name;
          prop = e1d.Value.Section.ToString();
          mat = e1d.Value.Section.Material.ToString();
          break;

        case GsaMember1dGoo m1d:
          id = m1d.Value.Id;
          name = m1d.Value.Name;
          prop = m1d.Value.Section.ToString();
          mat = m1d.Value.Section.Material.ToString();
          break;

        case GsaMember2dGoo m2d:
          id = m2d.Value.Id;
          name = m2d.Value.Name;
          prop = m2d.Value.Prop2d.ToString();
          mat = m2d.Value.Prop2d.Material.ToString();
          break;
        case GsaMember3dGoo m3d:
          id = m3d.Value.Id;
          name = m3d.Value.Name;
          prop = m3d.Value.Prop3d.ToString();
          mat = m3d.Value.Prop3d.Material.ToString();
          break;

        default:
          break;
      }

      string spacer = " -- ";
      string outputText =
        (_showId ? $"ID: {id}{spacer}" : string.Empty) +
        (_showName ? $"Name: {name}{spacer}" : string.Empty) +
        (_showProperty ? $"Property: {prop}{spacer}" : string.Empty) +
        (_showMaterial ? $"Material: {mat}" : string.Empty);

      if (_text3d) {
        _texts.Append(new GH_String(outputText), path);
      }

      return 
        (_showId ? $"{id}\n" : string.Empty) +
        (_showName ? $"{name}\n" : string.Empty) +
        (_showProperty ? $"{prop}\n" : string.Empty) +
        (_showMaterial ? $"{mat}" : string.Empty);
    }
  }
}
