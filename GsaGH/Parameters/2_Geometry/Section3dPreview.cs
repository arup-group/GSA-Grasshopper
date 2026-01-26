using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using GsaAPI;

using GsaGH.Helpers.Assembly;
using GsaGH.Helpers.Graphics;
using GsaGH.Parameters.Results;

using OasysGH.Units;

using OasysUnits;

using Rhino;
using Rhino.DocObjects;
using Rhino.Geometry;

using LengthUnit = OasysUnits.Units.LengthUnit;
using Line = Rhino.Geometry.Line;

namespace GsaGH.Parameters {
  internal enum Layer {
    Analysis,
    Design
  }
  internal enum DimensionType {
    OneDimensional,
    TwoDimensional
  }

  public class Section3dPreview {
    public Mesh Mesh { get; set; }
    public IEnumerable<Line> Outlines { get; set; }

    public Section3dPreview(GsaElement1d elem) {
      Model model = AssembleTempModel(elem);
      CreateGraphics(model, Layer.Analysis, DimensionType.OneDimensional);
    }

    public Section3dPreview(GsaElement2d elem) {
      Model model = AssembleTempModel(elem);
      CreateGraphics(model, Layer.Analysis, DimensionType.TwoDimensional);
    }

    public Section3dPreview(GsaMember1d mem) {
      Model model = AssembleTempModel(mem);
      CreateGraphics(model, Layer.Design, DimensionType.OneDimensional);
    }

    public Section3dPreview(GsaMember2d mem) {
      Model model = AssembleTempModel(mem);
      CreateGraphics(model, Layer.Design, DimensionType.TwoDimensional);
    }

    public Section3dPreview(GsaResult res, string elementList, double scale) {
      GraphicSpecification spec = ResultSpec(res, elementList, scale);
      CreateGraphics(res.Model.ApiModel, spec);
    }

    internal Section3dPreview(GsaModel model, Layer layer) {
      GraphicSpecification spec = layer == Layer.Analysis ? AnalysisLayerSpec() : DesignLayerSpec();
      CreateGraphics(model.ApiModel, spec);
      Scale(model.ModelUnit);
    }

    internal Section3dPreview(Model model, LengthUnit unit, Layer layer) {
      GraphicSpecification spec = layer == Layer.Analysis ? AnalysisLayerSpec() : DesignLayerSpec();
      CreateGraphics(model, spec);
      Scale(unit);
    }

    private Section3dPreview() { }

    public Section3dPreview Duplicate() {
      return new Section3dPreview() {
        Mesh = Mesh.DuplicateMesh(),
        Outlines = Outlines.ToList(),
      };
    }

    public void BakeGeometry(ref GH_BakeUtility gH_BakeUtility, RhinoDoc doc, ObjectAttributes att) {
      att ??= doc.CreateDefaultAttributes();
      att.ColorSource = ObjectColorSource.ColorFromObject;
      ObjectAttributes meshAtt = att.Duplicate();
      gH_BakeUtility.BakeObject(new GH_Mesh(Mesh), meshAtt, doc);
      foreach (Line ln in Outlines) {
        ObjectAttributes lnAtt = att.Duplicate();
        gH_BakeUtility.BakeObject(new GH_Line(ln), lnAtt, doc);
      }
    }

    public void DrawViewportMeshes(GH_PreviewMeshArgs args) {
      args.Pipeline.DrawMeshFalseColors(Mesh);
    }

    public void DrawViewportWires(GH_PreviewWireArgs args) {
      if (args.Color == Colours.EntityIsNotSelected) {
        args.Pipeline.DrawLines(Outlines, Colours.Element1d);
      } else {
        args.Pipeline.DrawLines(Outlines, Colours.Element1dSelected);
      }
    }

    public void Morph(SpaceMorph xmorph) {
      xmorph.Morph(Mesh);
      var lns = new List<Line>();
      foreach (Line l in Outlines) {
        var line = new Line(xmorph.MorphPoint(l.From), xmorph.MorphPoint(l.To));
        lns.Add(line);
      }
      Outlines = lns;
    }

    public void Scale(LengthUnit unit) {
      if (unit != LengthUnit.Meter) {
        double unitScaleFactor = UnitConverter.Convert(1, LengthUnit.Meter, unit);
        var scalar = Rhino.Geometry.Transform.Scale(new Point3d(0, 0, 0), unitScaleFactor);
        Transform(scalar);
      }
      ZoomToBoundingBox();
    }

    private void ZoomToBoundingBox() {
      BoundingBox bbox = Mesh.GetBoundingBox(true);
      foreach (Line line in Outlines) {
        bbox = BoundingBox.Union(bbox, new BoundingBox(line.From, line.To));
      }
      Rhino.Display.RhinoView view = RhinoDoc.ActiveDoc?.Views.ActiveView;
      if (view != null) {
        view.ActiveViewport.ZoomBoundingBox(bbox);
        view.Redraw();
      }
    }

    public void Transform(Transform xform) {
      Mesh.Transform(xform);
      var lns = new List<Line>();
      foreach (Line l in Outlines) {
        var line = new Line(l.From, l.To);
        line.Transform(xform);
        lns.Add(line);
      }
      Outlines = lns;
    }

    private static Model AssembleTempModel(GsaElement1d elem) {
      var model = new Model();
      LengthUnit unit = DefaultUnits.LengthUnitGeometry;
      var topo = new List<int> {
        model.AddNode(ModelAssembly.NodeFromPoint(elem.Line.Line.From, unit)),
        model.AddNode(ModelAssembly.NodeFromPoint(elem.Line.Line.To, unit))
      };
      GSAElement elem1d = elem.DuplicateApiObject();
      elem1d.Topology = new ReadOnlyCollection<int>(topo);
      elem1d.Property = elem.Section.Id;
      if (elem.Section.ApiSection != null ) {
        elem1d.Property = model.AddSection(elem.Section.ApiSection);
      }
      model.AddElement(elem1d.Element);
      return model;
    }

    private static Model AssembleTempModel(GsaMember1d mem) {
      var model = new Model();
      LengthUnit unit = DefaultUnits.LengthUnitGeometry;
      string topo = string.Empty;
      for (int i = 0; i < mem.Topology.Count; i++) {
        int id = model.AddNode(
          ModelAssembly.NodeFromPoint(mem.Topology[i], unit));
        topo += $" {mem.TopologyType[i]}{id}";
      };
      Member mem1d = mem.DuplicateApiObject();
      mem1d.Topology = topo.Trim();
      mem1d.Property = mem.Section.Id;
      if (mem.Section.ApiSection != null) {
        mem1d.Property = model.AddSection(mem.Section.ApiSection);
      }
      model.AddMember(mem1d);
      return model;
    }

    private static Model AssembleTempModel(GsaElement2d elem) {
      var model = new Model();
      LengthUnit unit = DefaultUnits.LengthUnitGeometry;
      for (int i = 0; i < elem.ApiElements.Count; i++) {
        var topo = new List<int>();
        foreach (int id in elem.TopoInt[i]) {
          topo.Add(model.AddNode(ModelAssembly.NodeFromPoint(elem.Topology[id], unit)));
        };
        GSAElement element = elem.ApiElements[i];
        element.Topology = new ReadOnlyCollection<int>(topo);
        element.Property = elem.Prop2ds[i].Id;
        if (elem.Prop2ds[i].ApiProp2d != null) {
          element.Property = model.AddProp2D(elem.Prop2ds[i].ApiProp2d);
        }
        if (element.IsLoadPanel) {
          model.AddLoadPanelElement(element.LoadPanelElement);
        } else {
          model.AddElement(element.Element);
        }

      }

      return model;
    }

    private static Model AssembleTempModel(GsaMember2d mem) {
      var model = new Model();
      LengthUnit unit = DefaultUnits.LengthUnitGeometry;
      string topo = string.Empty;
      for (int i = 0; i < mem.Topology.Count; i++) {
        int id = model.AddNode(ModelAssembly.NodeFromPoint(mem.Topology[i], unit));
        topo += $" {mem.TopologyType[i]}{id}";
      };
      Member mem2d = mem.DuplicateApiObject();
      mem2d.Topology = topo.Trim();
      mem2d.Property = mem.Prop2d.Id;
      if (mem.Prop2d.ApiProp2d != null) {
        mem2d.Property = model.AddProp2D(mem.Prop2d.ApiProp2d);
      }
      model.AddMember(mem2d);
      return model;
    }

    private static ConcurrentBag<Line> CreateOutlines(ReadOnlyCollection<GsaAPI.Line> lines) {
      var lns = new ConcurrentBag<Line>();
      Parallel.ForEach(lines, line => {
        var start = new Point3d(line.Start.X, line.Start.Y, line.Start.Z);
        var end = new Point3d(line.End.X, line.End.Y, line.End.Z);
        lns.Add(new Line(start, end));
      });
      return lns;
    }

    private static Mesh CreateMeshFromTriangles(ReadOnlyCollection<Triangle> triangles) {
      var faces = new ConcurrentBag<Mesh>();
      Parallel.ForEach(triangles, tri => {
        var face = new Mesh();
        var col = (Color)tri.Colour;
        if (col.Name == "ff464646" || col.Name == "ff000000") {
          col = Colours.Preview3dMeshDefault;
        }

        foreach (Vector3 verticy in tri.Vertices) {
          face.Vertices.Add(verticy.X, verticy.Y, verticy.Z);
          face.VertexColors.Add(col);
        }

        face.Faces.AddFace(0, 1, 2);
        faces.Add(face);
      });
      var mesh = new Mesh();
      mesh.Append(faces);
      mesh.Vertices.CombineIdentical(true, false);
      mesh.Faces.ConvertTrianglesToQuads(1, 0.75);
      return mesh;
    }

    private void CreateGraphics(Model model, Layer layer, DimensionType type, string definition = "all") {
      GraphicDrawResult graphic = model.Draw(Specification(layer, definition, type));
      Mesh = CreateMeshFromTriangles(graphic.Triangles);
      Outlines = CreateOutlines(graphic.Lines);
    }

    private void CreateGraphics(Model model, GraphicSpecification spec) {
      GraphicDrawResult graphic = model.Draw(spec);
      Mesh = CreateMeshFromTriangles(graphic.Triangles);
      Outlines = CreateOutlines(graphic.Lines);
    }

    private GraphicSpecification Specification(Layer layer, string definition, DimensionType type) {
      if (layer == Layer.Analysis) {
        return AnalysisLayerSpec(definition, type);
      } else {
        return DesignLayerSpec(definition, type);
      }
    }

    private static GraphicSpecification AnalysisLayerSpec(string definition, DimensionType type) {
      return new GraphicSpecification() {
        Entities = new EntityList() {
          Definition = definition,
          Name = "Name",
          Type = GsaAPI.EntityType.Element
        },
        Cases = new EntityList() {
          Definition = "none",
          Name = "case",
          Type = GsaAPI.EntityType.Case
        },
        EntityDisplayMethod = new EntityDisplayMethod {
          For1d = type == DimensionType.OneDimensional
            ? DisplayMethodFor1d.OutLineFilled : DisplayMethodFor1d.Off,
          For2d = type == DimensionType.OneDimensional ? DisplayMethodFor2d.Off
            : DisplayMethodFor2d.Solid
            | DisplayMethodFor2d.Thickness
            | DisplayMethodFor2d.Edge,
          For3d = DisplayMethodFor3d.Off
        },
        DeformationScaleFactor = 0,
        IsDeformationNormalised = false,
        DrawInitialState = false,
        DrawDeformedShape = false
      };
    }

    private static GraphicSpecification DesignLayerSpec(string definition, DimensionType type) {
      return new GraphicSpecification() {
        Entities = new EntityList() {
          Definition = definition,
          Name = "Name",
          Type = GsaAPI.EntityType.Member
        },
        Cases = new EntityList() {
          Definition = "none",
          Name = "case",
          Type = GsaAPI.EntityType.Case
        },
        EntityDisplayMethod = new EntityDisplayMethod {
          For1d = type == DimensionType.OneDimensional
            ? DisplayMethodFor1d.OutLineFilled : DisplayMethodFor1d.Off,
          For2d = type == DimensionType.OneDimensional ? DisplayMethodFor2d.Off
            : DisplayMethodFor2d.Solid
            | DisplayMethodFor2d.Thickness
            | DisplayMethodFor2d.Edge,
          For3d = DisplayMethodFor3d.Off
        },
        DeformationScaleFactor = 0,
        IsDeformationNormalised = false,
        DrawInitialState = false,
        DrawDeformedShape = false
      };
    }

    private static GraphicSpecification DesignLayerSpec() {
      return new GraphicSpecification() {
        Entities = new EntityList() {
          Definition = "all",
          Name = "Name",
          Type = GsaAPI.EntityType.Member
        },
        Cases = new EntityList() {
          Definition = "none",
          Name = "case",
          Type = GsaAPI.EntityType.Case
        },
        EntityDisplayMethod = new EntityDisplayMethod {
          For1d = DisplayMethodFor1d.OutLineFilled,
          For2d = DisplayMethodFor2d.Solid
                | DisplayMethodFor2d.Thickness
                | DisplayMethodFor2d.Edge,
          For3d = DisplayMethodFor3d.Off
        },
        DeformationScaleFactor = 0,
        IsDeformationNormalised = false,
        DrawInitialState = false,
        DrawDeformedShape = false
      };
    }

    private static GraphicSpecification AnalysisLayerSpec() {
      return new GraphicSpecification() {
        Entities = new EntityList() {
          Definition = "all",
          Name = "Name",
          Type = GsaAPI.EntityType.Element
        },
        Cases = new EntityList() {
          Definition = "none",
          Name = "case",
          Type = GsaAPI.EntityType.Case
        },
        EntityDisplayMethod = new EntityDisplayMethod {
          For1d = DisplayMethodFor1d.OutLineFilled,
          For2d = DisplayMethodFor2d.Solid
                | DisplayMethodFor2d.Thickness
                | DisplayMethodFor2d.Edge,
          For3d = DisplayMethodFor3d.Off
        },
        DeformationScaleFactor = 0,
        IsDeformationNormalised = false,
        DrawInitialState = false,
        DrawDeformedShape = false
      };
    }

    private static GraphicSpecification ResultSpec(
      GsaResult res, string elementList = "all", double scaleFactor = 1.0) {
      string caseType = res.CaseType == CaseType.AnalysisCase ? "A" : "C";
      string caseDefinition = $"{caseType}{res.CaseId}";
      return new GraphicSpecification() {
        Entities = new EntityList() {
          Definition = elementList,
          Name = "Name",
          Type = GsaAPI.EntityType.Element
        },
        Cases = new EntityList() {
          Definition = caseDefinition,
          Name = "case",
          Type = GsaAPI.EntityType.Case
        },
        EntityDisplayMethod = new EntityDisplayMethod {
          For1d = DisplayMethodFor1d.OutLineFilled,
          For2d = DisplayMethodFor2d.Solid
                | DisplayMethodFor2d.Thickness
                | DisplayMethodFor2d.Edge,
          For3d = DisplayMethodFor3d.Solid
                | DisplayMethodFor3d.Edge
        },
        DeformationScaleFactor = scaleFactor,
        IsDeformationNormalised = false,
        DrawInitialState = false,
        DrawDeformedShape = true
      };
    }
  }
}
