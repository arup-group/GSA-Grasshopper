using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Helpers.Graphics;
using OasysUnits;
using Rhino.Geometry;

namespace GsaGH.Parameters {
  public class MeshResultGoo : GH_GeometricGoo<Mesh>, IGH_PreviewData {
    public override BoundingBox Boundingbox => Value.GetBoundingBox(false);
    public override string TypeDescription => "A GSA result mesh type.";
    public override string TypeName => "Result Mesh";
    public Mesh ValidMesh {
      get {
        if (!_finalised) {
          Finalise();
        }

        var m = new Mesh();
        Mesh x = Value;

        m.Vertices.AddVertices(x.Vertices.ToList());
        m.VertexColors.SetColors(Value.VertexColors.ToArray());

        var ngons = x.GetNgonAndFacesEnumerable().ToList();

        foreach (int faceId in ngons
         .Select(ngon => ngon.FaceIndexList().Select(u => (int)u).ToList())
         .SelectMany(faceIndex => faceIndex)) {
          m.Faces.AddFace(x.Faces[faceId]);
        }

        m.RebuildNormals();
        return m;
      }
    }
    public readonly List<int> ElementIds;
    public readonly List<List<IQuantity>> ResultValues;
    public readonly List<List<Point3d>> Vertices;
    private bool _finalised;
    private List<Mesh> _tempMeshes = new List<Mesh>();

    public MeshResultGoo(
      Mesh mesh, List<List<IQuantity>> results, List<List<Point3d>> vertices,
      List<int> ids) : base(mesh) {
      ResultValues = results;
      Vertices = vertices;
      ElementIds = ids;
    }

    public BoundingBox ClippingBox => Boundingbox;

    public void DrawViewportMeshes(GH_PreviewMeshArgs args) {
      args.Pipeline.DrawMeshFalseColors(Value);
    }

    public void DrawViewportWires(GH_PreviewWireArgs args) {
      if (Value == null) {
        return;
      }

      if (CentralSettings.PreviewMeshEdges == false) {
        return;
      }

      Color color
        = args.Color
        == Color.FromArgb(255, 150, 0,
          0) // this is a workaround to change colour between selected and not
          ? Colours.Element2dEdge : Colours.Element2dEdgeSelected;

      if (Value.Ngons.Count > 0) {
        for (int i = 0; i < Value.TopologyEdges.Count; i++) {
          args.Pipeline.DrawLine(Value.TopologyEdges.EdgeLine(i), color, 1);
        }
      }

      args.Pipeline.DrawMeshWires(Value, color, 1);
    }

    public void Add(Mesh tempMesh, List<IQuantity> results, List<Point3d> vertices, int id) {
      _tempMeshes.Add(tempMesh);
      ResultValues.Add(results);
      Vertices.Add(vertices);
      ElementIds.Add(id);
      _finalised = false;
    }

    public void Add(
      List<Mesh> tempMesh, List<List<IQuantity>> results, List<List<Point3d>> vertices,
      List<int> ids) {
      _tempMeshes.AddRange(tempMesh);
      ResultValues.AddRange(results);
      Vertices.AddRange(vertices);
      ElementIds.AddRange(ids);
      Finalise();
    }

    public override bool CastTo<TQ>(out TQ target) {
      if (typeof(TQ).IsAssignableFrom(typeof(GH_Mesh))) {
        target = Value.IsValid ? (TQ)(object)new GH_Mesh(Value) :
          (TQ)(object)new GH_Mesh(ValidMesh);
        return true;
      }

      target = default;
      return false;
    }

    public override IGH_GeometricGoo DuplicateGeometry() {
      return new MeshResultGoo(Value, ResultValues, Vertices, ElementIds);
    }

    public void Finalise() {
      if (_finalised) {
        return;
      }

      Value = new Mesh();
      Value.Append(_tempMeshes);
      Value.RebuildNormals();
      Value.Compact();
      _tempMeshes = new List<Mesh>();
      _finalised = true;
    }

    public override BoundingBox GetBoundingBox(Transform xform) {
      Mesh m = Value;
      m.Transform(xform);
      return m.GetBoundingBox(false);
    }

    public override IGH_GeometricGoo Morph(SpaceMorph xmorph) {
      Mesh m = Value.DuplicateMesh();
      xmorph.Morph(m);
      var vertices = Vertices.Select(vertex => vertex.Select(point => new Point3d(point))
       .Select(xmorph.MorphPoint).ToList()).ToList();

      return new MeshResultGoo(m, ResultValues, Vertices, ElementIds);
    }

    public override object ScriptVariable() {
      return Value;
    }

    public override string ToString() {
      return
        $"MeshResult: V:{Value.Vertices.Count:0}, F:{Value.Faces.Count:0}, R:{ResultValues.Count:0}";
    }

    public override IGH_GeometricGoo Transform(Transform xform) {
      Mesh m = Value.DuplicateMesh();
      m.Transform(xform);
      var vertices = new List<List<Point3d>>();
      foreach (List<Point3d> vertex in Vertices) {
        var duplicates = new List<Point3d>();
        foreach (Point3d dup in vertex.Select(point => new Point3d(point))) {
          dup.Transform(xform);
          duplicates.Add(dup);
        }

        vertices.Add(duplicates);
      }

      return new MeshResultGoo(m, ResultValues, vertices, ElementIds);
    }
  }
}
