using System.Collections.Generic;
using System.Linq;
using GH_IO.Serialization;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using OasysUnits;
using Rhino.Geometry;

namespace GsaGH.Parameters
{
  public class MeshResultGoo : GH_GeometricGoo<Mesh>, IGH_PreviewData
  {
    internal List<List<IQuantity>> ResultValues = new List<List<IQuantity>>();
    internal List<List<Point3d>> Vertices = new List<List<Point3d>>();
    internal List<int> _ids = new List<int>();
    private List<Mesh> _tempMeshes = new List<Mesh>();
    private bool _finalised = false;

    public MeshResultGoo(Mesh mesh, List<List<IQuantity>> results, List<List<Point3d>> vertices, List<int> ids)
    : base(mesh)
    {
      this.ResultValues = results;
      this.Vertices = vertices;
      this._ids = ids;
    }

    public Mesh ValidMesh
    {
      get
      {
        if (!_finalised)
          Finalise();

        Mesh m = new Mesh();
        Mesh x = Value;

        m.Vertices.AddVertices(x.Vertices.ToList());
        m.VertexColors.SetColors(Value.VertexColors.ToArray());

        List<MeshNgon> ngons = x.GetNgonAndFacesEnumerable().ToList();

        for (int i = 0; i < ngons.Count; i++)
        {
          List<int> faceindex = ngons[i].FaceIndexList().Select(u => (int)u).ToList();
          for (int j = 0; j < faceindex.Count; j++)
          {
            m.Faces.AddFace(x.Faces[faceindex[j]]);
          }
        }
        m.RebuildNormals();
        return m;
      }
    }

    public void Add(Mesh temp_mesh, List<IQuantity> results, List<Point3d> vertices, int id)
    {
      _tempMeshes.Add(temp_mesh);
      ResultValues.Add(results);
      Vertices.Add(vertices);
      _ids.Add(id);
      _finalised = false;
    }
    public void Add(List<Mesh> temp_mesh, List<List<IQuantity>> results, List<List<Point3d>> vertices, List<int> ids)
    {
      _tempMeshes.AddRange(temp_mesh);
      ResultValues.AddRange(results);
      Vertices.AddRange(vertices);
      _ids.AddRange(ids);
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

    public override string ToString()
    {
      return string.Format("MeshResult: V:{0:0}, F:{1:0}, R:{2:0}", Value.Vertices.Count, Value.Faces.Count, ResultValues.Count);
    }
    public override string TypeName
    {
      get { return "Result Mesh"; }
    }
    public override string TypeDescription
    {
      get { return "A GSA result mesh type."; }
    }

    public override IGH_GeometricGoo DuplicateGeometry()
    {
      return new MeshResultGoo(Value, ResultValues, Vertices, _ids);
    }
    public override BoundingBox Boundingbox
    {
      get
      {
        return Value.GetBoundingBox(false);
      }
    }
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
      List<List<Point3d>> vertices = new List<List<Point3d>>();
      foreach (List<Point3d> vertex in this.Vertices) 
      {
        List<Point3d> duplicates = new List<Point3d>();
        foreach (Point3d point in vertex) 
        { 
          Point3d dup = new Point3d(point);
          dup.Transform(xform);
          duplicates.Add(dup);
        }
        vertices.Add(duplicates);
      }

      return new MeshResultGoo(m, this.ResultValues, vertices, _ids);
    }
    public override IGH_GeometricGoo Morph(SpaceMorph xmorph)
    {
      Mesh m = Value.DuplicateMesh();
      xmorph.Morph(m);
      List<List<Point3d>> vertices = new List<List<Point3d>>();
      foreach (List<Point3d> vertex in this.Vertices)
      {
        List<Point3d> duplicates = new List<Point3d>();
        foreach (Point3d point in vertex)
        {
          Point3d dup = new Point3d(point);
          duplicates.Add(xmorph.MorphPoint(dup));
        }
        vertices.Add(duplicates);
      }
      return new MeshResultGoo(m, ResultValues, Vertices, _ids);
    }

    public override object ScriptVariable()
    {
      return Value;
    }
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

    public BoundingBox ClippingBox
    {
      get { return Boundingbox; }
    }
    public void DrawViewportWires(GH_PreviewWireArgs args)
    {
      if (Value == null) { return; }
      if (Grasshopper.CentralSettings.PreviewMeshEdges == false) { return; }

      if (args.Color == System.Drawing.Color.FromArgb(255, 150, 0, 0)) // this is a workaround to change colour between selected and not
        args.Pipeline.DrawMeshWires(Value, Helpers.Graphics.Colours.Element2dEdge, 1);
      else
        args.Pipeline.DrawMeshWires(Value, Helpers.Graphics.Colours.Element2dEdgeSelected, 1);
    }

    public void DrawViewportMeshes(GH_PreviewMeshArgs args)
    {
      args.Pipeline.DrawMeshFalseColors(Value);
    }
  }
}
