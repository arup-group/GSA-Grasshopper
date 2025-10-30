using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;

using GsaAPI;

using Rhino.Geometry;

using Line = Rhino.Geometry.Line;

namespace GsaGH.Parameters {
  public class AssemblyPreview {
    public IEnumerable<Line> Outlines { get; set; }

    internal AssemblyPreview(Assembly assembly, Node topology1, Node topology2, Node orientationNode, ReadOnlyCollection<GridPlane> gridPlanes = null, string definition = "all") {
      var model = new Model();
      model.AddNode(topology1);
      model.AddNode(topology2);
      model.AddNode(orientationNode);

      foreach (GridPlane plane in gridPlanes) {
        if (plane.IsStoreyType) {
          model.AddGridPlane(plane);
        }
      }

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

      temp.EntityList = assembly.EntityList;
      temp.EntityType = assembly.EntityType;
      temp.ExtentY = assembly.ExtentY;
      temp.ExtentZ = assembly.ExtentZ;

      model.AddAssembly(temp);
      string assemblyName = assembly.Name + " test.gwb";
      model.SaveAs(assemblyName);

      var spec = new GraphicSpecification() {
        Entities = new EntityList() {
          Definition = definition,
          Name = "Assembly",
          Type = GsaAPI.EntityType.Assembly
        }
      };
      CreateGraphics(model, spec);
      File.Delete(assemblyName);
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

    private void CreateGraphics(Model model, GraphicSpecification spec) {
      GraphicDrawResult graphic = model.Draw(spec);
      Outlines = CreateOutlines(graphic.Lines);
    }
  }
}
