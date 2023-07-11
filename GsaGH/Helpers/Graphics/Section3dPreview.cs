using System.Collections.Generic;
using System.Collections.ObjectModel;
using GsaAPI;
using GsaGH.Parameters;
using OasysGH.Units;
using Rhino.Geometry;
using Line = Rhino.Geometry.Line;

namespace GsaGH.Helpers.Graphics {
  internal class Section3dPreview {
    internal static (Mesh, IEnumerable<Line>) CreatePreview(GsaElement1d elem) {
      Model model = AssembleTempModel(
        elem.Line.PointAtStart, elem.Line.PointAtEnd, elem.GetApiElementClone(), elem.Section.ApiSection);
      var spec = new GraphicSpecification() {
        Entities = new EntityList() { Definition = "all", Name = "AllElements", Type = GsaAPI.EntityType.Element },
        Cases = new EntityList() { Definition = "all", Name = "case", Type = GsaAPI.EntityType.Case },
        EntityDisplayMethod = new EntityDisplayMethod { 
          for1D = DisplayMethodFor1D.OutLineFilled,
          for2D = DisplayMethodFor2D.Off,
          for3D = DisplayMethodFor3D.Off
        },
        ScaleFactor = 1.0,
        IsNormalised = true
      };
      GraphicDrawResult graphic = model.Draw(spec);
      Mesh m = CreateMeshFromTriangles(graphic.Triangles);
      List<Line> lines = CreateOutlines(graphic.Lines);
      return (m, lines);
    }

    internal static (Mesh, IEnumerable<Line>) CreatePreview(GsaElement2d elem) {
      Model model = AssembleTempModel(elem);
      
      var spec = new GraphicSpecification() {
        Entities = new EntityList() { Definition = "all", Name = "AllElements", Type = GsaAPI.EntityType.Element },
        Cases = new EntityList() { Definition = "all", Name = "case", Type = GsaAPI.EntityType.Case },
        EntityDisplayMethod = new EntityDisplayMethod {
          for1D = DisplayMethodFor1D.Off,
          for2D = DisplayMethodFor2D.Solid | DisplayMethodFor2D.Thickness | DisplayMethodFor2D.Edge,
          for3D = DisplayMethodFor3D.Off
        },
        ScaleFactor = 1.0,
        IsNormalised = true
      };
      GraphicDrawResult graphic = model.Draw(spec);
      Mesh m = CreateMeshFromTriangles(graphic.Triangles);
      List<Line> lines = CreateOutlines(graphic.Lines);
      return (m, lines);
    }

    private static List<Line> CreateOutlines(ReadOnlyCollection<GsaAPI.Line> lines) {
      var lns = new List<Line>();
      foreach (GsaAPI.Line line in lines) {
        var start = new Point3d(line.Start.X, line.Start.Y, line.Start.Z);
        var end = new Point3d(line.End.X, line.End.Y, line.End.Z);
        lns.Add(new Line(start, end));
      }
      return lns;
    }

    private static Mesh CreateMeshFromTriangles(ReadOnlyCollection<Triangle> triangles) {
      var mesh = new Mesh();
      foreach (Triangle tri in triangles) {
        var face = new Mesh();
        foreach (Double3 verticy in tri.Vertices) {
          face.Vertices.Add(verticy.X, verticy.Y, verticy.Z);
          face.VertexColors.Add((System.Drawing.Color)tri.Colour);
        }
        
        face.Faces.AddFace(0, 1, 2);
        mesh.Append(face);
      }

      mesh.RebuildNormals();
      mesh.Weld(0.0001);
      mesh.Compact();
      return mesh;
    }

    private static Model AssembleTempModel(Point3d start, Point3d end, Element elem1d, Section section) {
      var model = new Model();
      OasysUnits.Units.LengthUnit unit = DefaultUnits.LengthUnitGeometry;
      var topo = new List<int> {
        model.AddNode(Export.Nodes.NodeFromPoint(start, unit)),
        model.AddNode(Export.Nodes.NodeFromPoint(end, unit))
      };
      elem1d.Topology = new ReadOnlyCollection<int>(topo);
      elem1d.Property = model.AddSection(section);
      model.AddElement(elem1d);
      return model;
    }

    private static Model AssembleTempModel(GsaElement2d elem) {
      var model = new Model();
      OasysUnits.Units.LengthUnit unit = DefaultUnits.LengthUnitGeometry;
      for (int i = 0; i < elem.ApiElements.Count; i++) {
        var topo = new List<int>();
        foreach (int id in elem.TopoInt[i]) {
          topo.Add(model.AddNode(
            Export.Nodes.NodeFromPoint(elem.Topology[id], unit)));
        };
        Element element = elem.GetApiObjectClone(i);
        element.Topology = new ReadOnlyCollection<int>(topo);
        element.Property = model.AddProp2D(elem.Prop2ds[i].ApiProp2d);
        model.AddElement(element);
      }

      return model;
    }

    private static Model AssembleTempModel(List<Point3d> pts, Element elem3d, Prop3D prop) {
      var model = new Model();
      OasysUnits.Units.LengthUnit unit = DefaultUnits.LengthUnitGeometry;
      var topo = new List<int>();
      foreach (Point3d pt in pts) {
        topo.Add(model.AddNode(Export.Nodes.NodeFromPoint(pt, unit)));
      };
      elem3d.Topology = new ReadOnlyCollection<int>(topo);
      elem3d.Property = model.AddProp3D(prop);
      model.AddElement(elem3d);
      return model;
    }
  }
}
