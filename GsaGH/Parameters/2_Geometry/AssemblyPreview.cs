using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Threading.Tasks;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GsaGH.Helpers.Graphics;
using Rhino;
using Rhino.DocObjects;
using Rhino.Geometry;
using Line = Rhino.Geometry.Line;

namespace GsaGH.Parameters {
  public class AssemblyPreview {
    public Mesh Mesh { get; set; }
    public IEnumerable<Line> Outlines { get; set; }

    internal AssemblyPreview(Assembly assembly, Node topology1, Node topology2, Node orientationNode, string definition = "all") {
      var model = new Model();
      model.AddNode(topology1);
      model.AddNode(topology2);
      model.AddNode(orientationNode);

      Assembly temp = null;
      switch (assembly) {
        case AssemblyByExplicitPositions explicitPositions:
          temp = new AssemblyByExplicitPositions(assembly.Name, 1, 2, 3, explicitPositions.InternalTopology, explicitPositions.CurveFit) {
            Positions = explicitPositions.Positions
          };
          break;

        case AssemblyByNumberOfPoints numberOfPoints:
          temp = new AssemblyByNumberOfPoints(assembly.Name, 1, 2, 3, numberOfPoints.InternalTopology, numberOfPoints.CurveFit) {
            NumberOfPoints = numberOfPoints.NumberOfPoints
          };
          break;

        case AssemblyBySpacingOfPoints spacingOfPoints:
          temp = new AssemblyBySpacingOfPoints(assembly.Name, 1, 2, 3, spacingOfPoints.InternalTopology, spacingOfPoints.CurveFit) {
            Spacing = spacingOfPoints.Spacing
          };
          break;

        case AssemblyByStorey byStorey:
          temp = new AssemblyByStorey(assembly.Name, 1, 2, 3) {
            StoreyList = byStorey.StoreyList
          };
          break;
      }

      model.AddAssembly(temp);

      var spec = new GraphicSpecification() {
        Entities = new EntityList() {
          Definition = definition,
          Name = "Assembly",
          Type = GsaAPI.EntityType.Assembly
        }
      };
      CreateGraphics(model, spec);
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
      if (args.Color == Color.FromArgb(255, 150, 0, 0)) {
        args.Pipeline.DrawLines(Outlines, Colours.Element1d);
      } else {
        args.Pipeline.DrawLines(Outlines, Colours.Element1dSelected);
      }
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

    private void CreateGraphics(Model model, GraphicSpecification spec) {
      GraphicDrawResult graphic = model.Draw(spec);
      Mesh = CreateMeshFromTriangles(graphic.Triangles);
      Outlines = CreateOutlines(graphic.Lines);
    }
  }
}
