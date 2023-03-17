using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using OasysUnits;
using Rhino.Geometry;

namespace GsaGH.Parameters
{
  public class MeshResultGoo : GH_GeometricGoo<Mesh>, IGH_PreviewData
  {
    public readonly List<List<IQuantity>> ResultValues = new List<List<IQuantity>>();
    public readonly List<List<Point3d>> Vertices = new List<List<Point3d>>();
    public readonly List<int> ElementIds = new List<int>();
    private List<Mesh> _tempMeshes = new List<Mesh>();
    private bool _finalised = false;

    public MeshResultGoo(Mesh mesh, List<List<IQuantity>> results, List<List<Point3d>> vertices, List<int> ids)
    : base(mesh)
    {
      this.ResultValues = results;
      this.Vertices = vertices;
      this.ElementIds = ids;
    }

    public Mesh ValidMesh
    {
      get
      {
        if (!_finalised)
          Finalise();

        var m = new Mesh();
        Mesh x = Value;

        m.Vertices.AddVertices(x.Vertices.ToList());
        m.VertexColors.SetColors(Value.VertexColors.ToArray());

        List<MeshNgon> ngons = x.GetNgonAndFacesEnumerable().ToList();

        foreach (var faceId in ngons.Select(ngon => ngon.FaceIndexList().Select(u => (int)u).ToList()).SelectMany(faceIndex => faceIndex))
        {
          m.Faces.AddFace(x.Faces[faceId]);
        }
        m.RebuildNormals();
        return m;
      }
    }

    public void Add(Mesh tempMesh, List<IQuantity> results, List<Point3d> vertices, int id)
    {
      _tempMeshes.Add(tempMesh);
      ResultValues.Add(results);
      Vertices.Add(vertices);
      ElementIds.Add(id);
      _finalised = false;
    }
    public void Add(List<Mesh> tempMesh, List<List<IQuantity>> results, List<List<Point3d>> vertices, List<int> ids)
    {
      _tempMeshes.AddRange(tempMesh);
      ResultValues.AddRange(results);
      Vertices.AddRange(vertices);
      ElementIds.AddRange(ids);
      Finalise();
    }
    public void Finalise()
    {
      if (_finalised == true) { return; }
      this.Value = new Mesh();
      this.Value.Append(_tempMeshes);
      this.Value.RebuildNormals();
      this.Value.Compact();
      _tempMeshes = new List<Mesh>();
      _finalised = true;
    }

    public override string ToString() => $"MeshResult: V:{Value.Vertices.Count:0}, F:{Value.Faces.Count:0}, R:{ResultValues.Count:0}";
    
    public override string TypeName => "Result Mesh";

    public override string TypeDescription => "A GSA result mesh type.";

    public override IGH_GeometricGoo DuplicateGeometry() => new MeshResultGoo(Value, ResultValues, Vertices, ElementIds);
    
    public override BoundingBox Boundingbox => Value.GetBoundingBox(false);

    public override BoundingBox GetBoundingBox(Transform xform)
    {
      Mesh m = Value;
      m.Transform(xform);
      return m.GetBoundingBox(false);
    }
    public override IGH_GeometricGoo Transform(Transform xform)
    {
      Mesh m = Value.DuplicateMesh();
      m.Transform(xform);
      var vertices = new List<List<Point3d>>();
      foreach (var vertex in this.Vertices) 
      {
        var duplicates = new List<Point3d>();
        foreach (var point in vertex) 
        { 
          var dup = new Point3d(point);
          dup.Transform(xform);
          duplicates.Add(dup);
        }
        vertices.Add(duplicates);
      }

      return new MeshResultGoo(m, this.ResultValues, vertices, ElementIds);
    }
    public override IGH_GeometricGoo Morph(SpaceMorph xmorph)
    {
      Mesh m = Value.DuplicateMesh();
      xmorph.Morph(m);
      var vertices = new List<List<Point3d>>();
      foreach (var vertex in this.Vertices)
      {
        var duplicates = new List<Point3d>();
        foreach (var point in vertex)
        {
          var dup = new Point3d(point);
          duplicates.Add(xmorph.MorphPoint(dup));
        }
        vertices.Add(duplicates);
      }
      return new MeshResultGoo(m, ResultValues, Vertices, ElementIds);
    }

    public override object ScriptVariable() => Value;
    
    public override bool CastTo<TQ>(out TQ target)
    {
      if (typeof(TQ).IsAssignableFrom(typeof(Mesh)))
      {
        if (Value.IsValid)
          target = (TQ)(object)Value;
        else
          target = (TQ)(object)ValidMesh;
        return true;
      }

      if (typeof(TQ).IsAssignableFrom(typeof(GH_Mesh)))
      {
        if (Value.IsValid)
          target = (TQ)(object)new GH_Mesh(Value);
        else
          target = (TQ)(object)new GH_Mesh(ValidMesh);
        return true;
      }

      target = default(TQ);
      return false;
    }
    public override bool CastFrom(object source)
    {
      if (source == null) return false;
      if (source is Mesh)
      {
        Value = (Mesh)source;
        return true;
      }
      GH_Mesh meshGoo = source as GH_Mesh;
      if (meshGoo != null)
      {
        Value = meshGoo.Value;
        return true;
      }

      Mesh m = new Mesh();
      if (GH_Convert.ToMesh(source, ref m, GH_Conversion.Both))
      {
        Value = m;
        return true;
      }

      return false;
    }

    public BoundingBox ClippingBox => Boundingbox;

    public void DrawViewportWires(GH_PreviewWireArgs args)
    {
      if (Value == null) { return; }
      if (Grasshopper.CentralSettings.PreviewMeshEdges == false) { return; }

      Color color = args.Color == System.Drawing.Color.FromArgb(255, 150, 0, 0) // this is a workaround to change colour between selected and not
          ? Helpers.Graphics.Colours.Element2dEdge
          : Helpers.Graphics.Colours.Element2dEdgeSelected;

      if (Value.Ngons.Count > 0)
      {
        for(int i = 0; i < Value.TopologyEdges.Count; i++)
          args.Pipeline.DrawLine(Value.TopologyEdges.EdgeLine(i), color, 1);
      }

      args.Pipeline.DrawMeshWires(Value, color, 1);
    }

    public void DrawViewportMeshes(GH_PreviewMeshArgs args)
    {
      args.Pipeline.DrawMeshFalseColors(Value);
    }
  }
}
