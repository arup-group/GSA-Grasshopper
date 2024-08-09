using System;
using System.Collections.Generic;
using System.Drawing;

using GH_IO.Serialization;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;

using GsaAPI;

using GsaGH.Helpers;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;

using OasysGH;
using OasysGH.Components;
using OasysGH.UI;

using Rhino.Collections;
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
      _initialCheckState = new List<bool>() {
        _showId,
        _showName,
        _showProperty,
        _showMaterial,
      };
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

    protected override void SolveInternal(IGH_DataAccess da) {
      da.GetDataTree(0, out GH_Structure<IGH_Goo> tree);
      double size = 1;
      if (!da.GetData(1, ref size)) {
        size = _text3d ? 0.1 : 14;
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
              Point3dList points = e2d.Value.GetCenterPoints();
              int faceIndex = 0;
              for (int i = 0; i < e2d.Value.ApiElements.Count; i++) {
                if (_text3d) {
                  if (e2d.Value.Mesh.FaceNormals.Count == 0) {
                    e2d.Value.Mesh.RebuildNormals();
                  }
                  AddAnnotation3d(new Plane(points[i], e2d.Value.Mesh.FaceNormals[faceIndex]),
                    CreateText(e2d, path, i), (Color)e2d.Value.ApiElements[i].Colour, size, path);
                } else {
                  AddAnnotationDot(points[i], CreateText(e2d, path, i), (Color)e2d.Value.ApiElements[i].Colour, size, path);
                }

                switch (e2d.Value.ApiElements[i].Type) {
                  case ElementType.QUAD8:
                    faceIndex += 8;
                    break;

                  case ElementType.TRI6:
                    faceIndex += 4;
                    break;

                  default:
                    faceIndex++;
                    break;
                }
              }
              continue;

            case GsaElement3dGoo e3d:
              for (int i = 0; i < e3d.Value.NgonMesh.Ngons.Count; i++) {
                if (_text3d) {
                  AddAnnotation3d(
                    new Plane(e3d.Value.NgonMesh.Ngons.GetNgonCenter(i), Vector3d.ZAxis),
                    CreateText(e3d, path, i),
                    (Color)e3d.Value.ApiElements[i].Colour, size, path);
                } else {
                  AddAnnotationDot(
                    e3d.Value.NgonMesh.Ngons.GetNgonCenter(i),
                    CreateText(e3d, path, i),
                    (Color)e3d.Value.ApiElements[i].Colour, size, path);
                }
              }
              continue;

            case GsaElement1dGoo e1d:
              if (_text3d) {
                AddAnnotation3d(
                  CreateLocalAxis(e1d.Value.Line),
                  CreateText(e1d, path),
                  (Color)e1d.Value.ApiElement.Colour, size, path);
              } else {
                AddAnnotationDot(
                  e1d.Value.Line.PointAtNormalizedLength(0.5),
                  CreateText(e1d, path),
                  (Color)e1d.Value.ApiElement.Colour, size, path);
              }
              break;

            case GsaMember1dGoo m1d:
              if (_text3d) {
                AddAnnotation3d(
                  CreateLocalAxis(m1d.Value.PolyCurve),
                  CreateText(m1d, path),
                  (Color)m1d.Value.ApiMember.Colour, size, path);
              } else {
                AddAnnotationDot(
                  m1d.Value.PolyCurve.PointAtNormalizedLength(0.5),
                  CreateText(m1d, path),
                  (Color)m1d.Value.ApiMember.Colour, size, path);
              }
              break;

            case GsaMember2dGoo m2d:
              m2d.Value.PolyCurve.TryGetPolyline(out Rhino.Geometry.Polyline pl);
              if (_text3d) {
                Plane.FitPlaneToPoints(pl, out Plane pln);
                pln.Origin = pl.CenterPoint();
                AddAnnotation3d(
                  pln,
                  CreateText(m2d, path),
                  (Color)m2d.Value.ApiMember.Colour, size, path);
              } else {
                AddAnnotationDot(
                  pl.CenterPoint(),
                  CreateText(m2d, path),
                  (Color)m2d.Value.ApiMember.Colour, size, path);
              }
              break;
            case GsaMember3dGoo m3d:
              if (_text3d) {
                AddAnnotation3d(
                  m3d.Value.SolidMesh.GetBoundingBox(false).Center,
                  CreateText(m3d, path),
                  (Color)m3d.Value.ApiMember.Colour, size, path);
              } else {
                AddAnnotationDot(
                  m3d.Value.SolidMesh.GetBoundingBox(false).Center,
                  CreateText(m3d, path),
                  (Color)m3d.Value.ApiMember.Colour, size, path);
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

    private Plane CreateLocalAxis(LineCurve line) {
      var tangent = new Vector3d(
        line.PointAtEnd.X - line.PointAtStart.X,
        line.PointAtEnd.Y - line.PointAtStart.Y,
        line.PointAtEnd.Z - line.PointAtStart.Z);
      Plane pln = CreateLocalAxis(tangent);
      pln.Origin = line.PointAtNormalizedLength(0.5);
      return pln;
    }

    private Plane CreateLocalAxis(PolyCurve crv) {
      Plane pln = CreateLocalAxis(crv.TangentAt(0.5));
      pln.Origin = crv.PointAtNormalizedLength(0.5);
      return pln;
    }

    private Plane CreateLocalAxis(Vector3d tangent) {
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
          name = GeometryToString(e2d.Value.ApiElements[i].Name, e2d.Value.ApiElements[i].TypeAsString());
          prop = Prop2dToString(e2d.Value.Prop2ds?[i]);
          mat = MaterialToString(e2d.Value.Prop2ds?[i].Material);
          break;

        case GsaElement3dGoo e3d:
          id = e3d.Value.Ids[i];
          name = GeometryToString(e3d.Value.ApiElements[i].Name, e3d.Value.ApiElements[i].TypeAsString());
          prop = Prop3dToString(e3d.Value.Prop3ds?[i]);
          mat = MaterialToString(e3d.Value.Prop3ds?[i].Material);
          break;

        case GsaElement1dGoo e1d:
          id = e1d.Value.Id;
          name = GeometryToString(e1d.Value.ApiElement.Name, e1d.Value.ApiElement.TypeAsString());
          prop = SectionToString(e1d.Value.Section);
          mat = MaterialToString(e1d.Value.Section?.Material);
          break;

        case GsaMember1dGoo m1d:
          id = m1d.Value.Id;
          name = GeometryToString(m1d.Value.ApiMember.Name, m1d.Value.ApiMember.TypeAsString());
          prop = SectionToString(m1d.Value.Section);
          mat = MaterialToString(m1d.Value.Section?.Material);
          break;

        case GsaMember2dGoo m2d:
          id = m2d.Value.Id;
          name = GeometryToString(m2d.Value.ApiMember.Name, m2d.Value.ApiMember.TypeAsString());
          prop = Prop2dToString(m2d.Value.Prop2d);
          mat = MaterialToString(m2d.Value.Prop2d?.Material);
          break;
        case GsaMember3dGoo m3d:
          id = m3d.Value.Id;
          name = GeometryToString(m3d.Value.ApiMember.Name, string.Empty);
          prop = Prop3dToString(m3d.Value.Prop3d);
          mat = MaterialToString(m3d.Value.Prop3d?.Material);
          break;

        default:
          break;
      }

      string spacer = " -- ";
      string outputText =
        (_showId ? $"ID: {id}" : string.Empty) +
        (_showName ? $"{spacer}Name: {name}" : string.Empty) +
        (_showProperty ? $"{spacer}Property: {prop}" : string.Empty) +
        (_showMaterial ? $"{spacer}Material: {mat}" : string.Empty);

      if (_text3d) {
        _texts.Append(new GH_String(outputText), path);
      }

      return "\n" +
        (_showId ? $"{id}\n" : string.Empty) +
        (_showName ? $"{name}\n" : string.Empty) +
        (_showProperty ? $"{prop}\n" : string.Empty) +
        (_showMaterial ? $"{mat}" : string.Empty);
    }

    private static string MaterialToString(GsaMaterial mat) {
      if (mat == null) {
        return string.Empty;
      }

      string s = string.Empty;
      s += mat.MaterialType.ToString();
      AddSeparator(ref s);
      s += mat.Id == 0 ? string.Empty : $"Grd:{mat.Id}";
      AddSeparator(ref s);
      s += mat.Name ?? string.Empty;
      return s.Trim();
    }

    private static string GeometryToString(string name, object type) {
      string s = string.Empty;
      s += name;
      AddSeparator(ref s);
      s += type.ToString().ToPascalCase();
      return s.Trim();
    }

    private static string Prop2dToString(GsaProperty2d prop) {
      if (prop == null) {
        return string.Empty;
      }

      string s = string.Empty;
      s += prop.Id > 0 ? $"PA{prop.Id}" : string.Empty;
      if (prop.IsReferencedById) {
        return s;
      }
      AddSeparator(ref s);
      s += prop.ApiProp2d.Name;
      AddSeparator(ref s);
      s += prop.ApiProp2d.Description;
      if (prop.ApiProp2d.Type != GsaAPI.Property2D_Type.SHELL) {
        AddSeparator(ref s);
        s += prop.ApiProp2d.Type.ToString().ToPascalCase();
      }
      return s.Trim();
    }

    private static string Prop3dToString(GsaProperty3d prop) {
      if (prop == null) {
        return string.Empty;
      }

      string s = string.Empty;
      s += prop.Id > 0 ? $"PV{prop.Id}" : string.Empty;
      AddSeparator(ref s);
      s += prop.ApiProp3d == null ? string.Empty : prop.ApiProp3d.Name;
      return s.Trim();
    }

    private static string SectionToString(GsaSection section) {
      if (section == null) {
        return string.Empty;
      }

      string s = string.Empty;
      s += section.Id > 0 ? $"PB{section.Id}" : string.Empty;
      AddSeparator(ref s);
      s += section.ApiSection.Name;
      AddSeparator(ref s);
      s += section.ApiSection.Profile;
      return s.Trim();
    }

    private static string SpringPropertyToString(GsaSpringProperty springProperty) {
      if (springProperty == null) {
        return string.Empty;
      }

      string s = string.Empty;
      s += springProperty.Id > 0 ? $"PS{springProperty.Id}" : string.Empty;
      AddSeparator(ref s);
      s += springProperty.ApiProperty.Name;
      return s.Trim();
    }

    private static void AddSeparator(ref string s) {
      if (s.Length == 0 || s[s.Length - 1] == ' ') {
        return;
      }

      s += " ";
    }
  }
}
