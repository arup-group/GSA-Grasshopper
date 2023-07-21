using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using GsaAPI;
using GsaGH.Helpers.Export;
using GsaGH.Helpers.Graphics;
using OasysGH.Units;
using OasysUnits;
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
  
  public class GsaSection3dPreview {
    public Mesh Mesh { get; set; }
    public IEnumerable<Line> Outlines { get; set; }

    public GsaSection3dPreview(GsaElement1d elem) {
      Model model = AssembleTempModel(elem);
      CreateGraphics(model, Layer.Analysis, DimensionType.OneDimensional);
    }
    public GsaSection3dPreview(GsaElement2d elem) {
      Model model = AssembleTempModel(elem);
      CreateGraphics(model, Layer.Analysis, DimensionType.TwoDimensional);
    }
    public GsaSection3dPreview(GsaMember1d mem) {
      Model model = AssembleTempModel(mem);
      CreateGraphics(model, Layer.Design, DimensionType.OneDimensional);
    }
    public GsaSection3dPreview(GsaMember2d mem) {
      Model model = AssembleTempModel(mem);
      CreateGraphics(model, Layer.Design, DimensionType.TwoDimensional);
    }

    internal static GsaSection3dPreview CreateFromApi(
      GsaModel model, Layer layer) {
      var section3dPreview = new GsaSection3dPreview();
      GraphicSpecification spec = layer == Layer.Analysis ? AnalysisLayerSpec() : DesignLayerSpec();
      section3dPreview.CreateGraphics(model.Model, spec);
      if (model.ModelUnit != LengthUnit.Meter) {
        double unitScaleFactor = UnitConverter.Convert(1, LengthUnit.Meter, model.ModelUnit);
        var scalar = Rhino.Geometry.Transform.Scale(new Point3d(0, 0, 0), unitScaleFactor);
        section3dPreview.Transform(scalar);
      }

      return section3dPreview;
    }
    private GsaSection3dPreview() { }

    public GsaSection3dPreview Transform(Transform xform) {
      Mesh m = Mesh.DuplicateMesh();
      m.Transform(xform);
      IEnumerable<Line> lns = Outlines.Select(l => new Line(l.From, l.To));
      lns.Select(l => l.Transform(xform));
      var dup = new GsaSection3dPreview() {
        Mesh = m,
        Outlines = lns,
      };
      return dup;
    }

    public GsaSection3dPreview Morph(SpaceMorph xmorph) {
      Mesh m = Mesh.DuplicateMesh();
      xmorph.Morph(m);
      IEnumerable<Line> lns = Outlines.Select(
        l => new Line(xmorph.MorphPoint(l.From), xmorph.MorphPoint(l.To)));
      var dup = new GsaSection3dPreview() {
        Mesh = m,
        Outlines = lns,
      };
      return dup;
    }

    private static Model AssembleTempModel(GsaElement1d elem) {
      var model = new Model();
      OasysUnits.Units.LengthUnit unit = DefaultUnits.LengthUnitGeometry;
      var topo = new List<int> {
        model.AddNode(Nodes.NodeFromPoint(elem.Line.Line.From, unit)),
        model.AddNode(Nodes.NodeFromPoint(elem.Line.Line.To, unit))
      };
      Element elem1d = elem.GetApiElementClone();
      elem1d.Topology = new ReadOnlyCollection<int>(topo);
      elem1d.Property = model.AddSection(elem.Section.ApiSection);
      model.AddElement(elem1d);
      return model;
    }

    private static Model AssembleTempModel(GsaMember1d mem) {
      var model = new Model();
      LengthUnit unit = DefaultUnits.LengthUnitGeometry;
      string topo = string.Empty;
      for (int i = 0; i < mem.Topology.Count; i++) {
        int id = model.AddNode(
          Nodes.NodeFromPoint(mem.Topology[i], unit));
        topo += $" {mem.TopologyType[i]}{id}";
      };
      Member mem1d = mem.GetAPI_MemberClone();
      mem1d.Topology = topo.Trim();
      mem1d.Property = model.AddSection(mem.Section.ApiSection);
      model.AddMember(mem1d);
      return model;
    }

    private static Model AssembleTempModel(GsaElement2d elem) {
      var model = new Model();
      LengthUnit unit = DefaultUnits.LengthUnitGeometry;
      for (int i = 0; i < elem.ApiElements.Count; i++) {
        var topo = new List<int>();
        foreach (int id in elem.TopoInt[i]) {
          topo.Add(model.AddNode(Nodes.NodeFromPoint(elem.Topology[id], unit)));
        };
        Element element = elem.GetApiObjectClone(i);
        element.Topology = new ReadOnlyCollection<int>(topo);
        element.Property = model.AddProp2D(elem.Prop2ds[i].ApiProp2d);
        model.AddElement(element);
      }

      return model;
    }

    private static Model AssembleTempModel(GsaMember2d mem) {
      var model = new Model();
      OasysUnits.Units.LengthUnit unit = DefaultUnits.LengthUnitGeometry;
      string topo = string.Empty;
      for (int i = 0; i < mem.Topology.Count; i++) {
        int id = model.AddNode(Nodes.NodeFromPoint(mem.Topology[i], unit));
        topo += $" {mem.TopologyType[i]}{id}";
      };
      Member mem2d = mem.GetAPI_MemberClone();
      mem2d.Topology = topo.Trim();
      mem2d.Property = model.AddProp2D(mem.Prop2d.ApiProp2d);
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
        
        foreach (Double3 verticy in tri.Vertices) {
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
        ScaleFactor = 1.0,
        IsNormalised = true
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
        ScaleFactor = 1.0,
        IsNormalised = true
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
        ScaleFactor = 1.0,
        IsNormalised = true
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
        ScaleFactor = 1.0,
        IsNormalised = true
      };
    }
  }
}
