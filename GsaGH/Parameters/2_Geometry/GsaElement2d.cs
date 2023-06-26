using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using Grasshopper;
using Grasshopper.Kernel.Data;
using GsaAPI;
using GsaGH.Helpers.GH;
using GsaGH.Helpers.GsaApi;
using OasysUnits;
using Rhino.Geometry;
using AngleUnit = OasysUnits.Units.AngleUnit;
using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGH.Parameters {
  /// <summary>
  ///   Element2d class, this class defines the basic properties and methods for any Gsa Element 2d
  /// </summary>
  public class GsaElement2d {
    private enum ApiObjectMember {
      All,
      Group,
      Dummy,
      Name,
      OrientationAngle,
      Offset,
      Property,
      Type,
      Colour,
    }

    public List<Color> Colours {
      get {
        var cols = new List<Color>();
        for (int i = 0; i < ApiElements.Count; i++) {
          if ((Color)ApiElements[i].Colour == Color.FromArgb(0, 0, 0)) {
            ApiElements[i].Colour = Color.FromArgb(50, 150, 150, 150);
          }

          cols.Add((Color)ApiElements[i].Colour);

          Mesh.VertexColors.SetColor(i, (Color)ApiElements[i].Colour);
        }

        return cols;
      }
      set
        => CloneApiElements(ApiObjectMember.Colour, null, null, null, null, null, null, null,
          value);
    }
    public int Count => ApiElements.Count;
    public List<int> Groups {
      get => (from element in ApiElements where element != null select element.Group).ToList();
      set => CloneApiElements(ApiObjectMember.Group, value);
    }
    public Guid Guid => _guid;
    public List<int> Ids { get; set; } = new List<int>();
    public List<bool> IsDummies {
      get => (from element in ApiElements where element != null select element.IsDummy).ToList();
      set => CloneApiElements(ApiObjectMember.Dummy, null, value);
    }
    public Mesh Mesh { get; private set; } = new Mesh();
    public List<string> Names {
      get => (from element in ApiElements where element != null select element.Name).ToList();
      set => CloneApiElements(ApiObjectMember.Name, null, null, value);
    }
    public List<GsaOffset> Offsets {
      get
        => (from element in ApiElements where element != null
            select new GsaOffset(element.Offset.X1, element.Offset.X2, element.Offset.Y,
              element.Offset.Z)).ToList();
      set => CloneApiElements(ApiObjectMember.Offset, null, null, null, null, value);
    }
    public List<Angle> OrientationAngles {
      get
        => (from element in ApiElements where element != null
            select new Angle(element.OrientationAngle, AngleUnit.Degree).ToUnit(AngleUnit.Radian))
         .ToList();
      set => CloneApiElements(ApiObjectMember.OrientationAngle, null, null, null, value);
    }
    public List<int> ParentMembers {
      get {
        var pMems = new List<int>();
        foreach (Element element in ApiElements) {
          try {
            pMems.Add(element.ParentMember.Member);
          } catch (Exception) {
            pMems.Add(0);
          }
        }

        return pMems;
      }
    }
    public List<GsaProp2d> Prop2ds { get; set; } = new List<GsaProp2d>();
    public List<List<int>> TopoInt { get; private set; }
    public List<Point3d> Topology { get; private set; }
    public DataTree<int> TopologyIDs {
      get {
        var topos = new DataTree<int>();
        for (int i = 0; i < ApiElements.Count; i++) {
          if (ApiElements[i] != null) {
            topos.AddRange(ApiElements[i].Topology.ToList(), new GH_Path(Ids[i]));
          }
        }

        return topos;
      }
    }
    public List<ElementType> Types {
      get => (from t in ApiElements where t != null select t.Type).ToList();
      set => CloneApiElements(ApiObjectMember.Type, null, null, null, null, null, null, value);
    }
    internal List<Element> ApiElements { get; set; } = new List<Element>();

    private Guid _guid = Guid.NewGuid();

    public GsaElement2d() { }

    public GsaElement2d(Mesh mesh, int prop = 0) {
      Mesh = mesh.DuplicateMesh();
      Mesh.Compact();
      Tuple<List<Element>, List<Point3d>, List<List<int>>> convertMesh
        = RhinoConversions.ConvertMeshToElem2d(Mesh, prop);
      ApiElements = convertMesh.Item1;
      Topology = convertMesh.Item2;
      TopoInt = convertMesh.Item3;
      Ids = new List<int>(new int[Mesh.Faces.Count]);
      var singleProp = new GsaProp2d(prop);
      for (int i = 0; i < Mesh.Faces.Count; i++) {
        Prop2ds.Add(singleProp.Duplicate());
      }
    }

    public GsaElement2d(
      Brep brep, List<Curve> curves, List<Point3d> points, double meshSize,
      List<GsaMember1d> mem1ds, List<GsaNode> nodes, LengthUnit unit, Length tolerance,
      int prop = 0) {
      Mesh = RhinoConversions.ConvertBrepToMesh(brep,
        points, nodes, curves, null, mem1ds, meshSize, unit, tolerance).Item1;
      Tuple<List<Element>, List<Point3d>, List<List<int>>> convertMesh
        = RhinoConversions.ConvertMeshToElem2d(Mesh, prop, true);
      ApiElements = convertMesh.Item1;
      Topology = convertMesh.Item2;
      TopoInt = convertMesh.Item3;
      Ids = new List<int>(new int[Mesh.Faces.Count]);
      var singleProp = new GsaProp2d(prop);
      for (int i = 0; i < Mesh.Faces.Count; i++) {
        Prop2ds.Add(singleProp.Duplicate());
      }
    }

    internal GsaElement2d(
      ConcurrentDictionary<int, Element> elements, Mesh mesh, List<GsaProp2d> prop2ds) {
      Mesh = mesh;
      Topology = new List<Point3d>(mesh.Vertices.ToPoint3dArray());
      TopoInt = RhinoConversions.ConvertMeshToElem2d(Mesh);
      ApiElements = elements.Values.ToList();
      Ids = elements.Keys.ToList();
      Prop2ds = prop2ds;
    }

    public static Tuple<GsaElement2d, List<GsaNode>, List<GsaElement1d>> GetElement2dFromBrep(
      Brep brep, List<Point3d> points, List<GsaNode> nodes, List<Curve> curves,
      List<GsaElement1d> elem1ds, List<GsaMember1d> mem1ds, double meshSize, LengthUnit unit,
      Length tolerance) {
      var gsaElement2D = new GsaElement2d();
      Tuple<Mesh, List<GsaNode>, List<GsaElement1d>> tuple
        = RhinoConversions.ConvertBrepToMesh(brep, points, nodes, curves, elem1ds, mem1ds, meshSize,
          unit, tolerance);
      gsaElement2D.Mesh = tuple.Item1;
      Tuple<List<Element>, List<Point3d>, List<List<int>>> convertMesh
        = RhinoConversions.ConvertMeshToElem2d(gsaElement2D.Mesh, 0, true);
      gsaElement2D.ApiElements = convertMesh.Item1;
      gsaElement2D.Topology = convertMesh.Item2;
      gsaElement2D.TopoInt = convertMesh.Item3;
      gsaElement2D.Ids = new List<int>(new int[gsaElement2D.Mesh.Faces.Count]);

      return new Tuple<GsaElement2d, List<GsaNode>, List<GsaElement1d>>(gsaElement2D, tuple.Item2,
        tuple.Item3);
    }

    public GsaElement2d Clone() {
      var dup = new GsaElement2d {
        ApiElements = ApiElements,
      };
      dup.CloneApiElements();

      dup.Ids = Ids.ToList();
      dup.Mesh = (Mesh)Mesh.DuplicateShallow();
      dup.Prop2ds = Prop2ds.ConvertAll(x => x.Duplicate());
      dup.Topology = Topology;
      dup.TopoInt = TopoInt;
      return dup;
    }

    public GsaElement2d Duplicate() {
      return this;
    }

    public GsaElement2d Morph(SpaceMorph xmorph) {
      if (Mesh == null) {
        return null;
      }

      GsaElement2d dup = Clone();
      dup.Ids = new List<int>(new int[dup.Mesh.Faces.Count]);

      Mesh xMs = dup.Mesh.DuplicateMesh();
      xmorph.Morph(xMs);

      return dup.UpdateGeometry(xMs);
    }

    public override string ToString() {
      if (!Mesh.IsValid) {
        return "Null";
      }

      string type = Mappings.elementTypeMapping.FirstOrDefault(x => x.Value == Types.First()).Key
        + " ";
      string info = "N:" + Mesh.Vertices.Count + " E:" + ApiElements.Count;
      return string.Join(" ", type.Trim(), info.Trim()).Trim().Replace("  ", " ");
    }

    public GsaElement2d Transform(Transform xform) {
      if (Mesh == null) {
        return null;
      }

      GsaElement2d dup = Clone();
      dup.Ids = new List<int>(new int[dup.Mesh.Faces.Count]);

      Mesh xMs = dup.Mesh.DuplicateMesh();
      xMs.Transform(xform);

      return dup.UpdateGeometry(xMs);
    }

    public GsaElement2d UpdateGeometry(Mesh newMesh) {
      if (Mesh.Faces.Count != ApiElements.Count) {
        return null; // the logic below assumes the number of elements is equal to number of faces
      }

      GsaElement2d dup = Clone();
      Mesh = newMesh;
      Tuple<List<Element>, List<Point3d>, List<List<int>>> convertMesh
        = RhinoConversions.ConvertMeshToElem2d(Mesh, 0);
      ApiElements = convertMesh.Item1;
      Topology = convertMesh.Item2;
      TopoInt = convertMesh.Item3;
      return dup;
    }

    internal void CloneApiElements() {
      CloneApiElements(ApiObjectMember.All);
      _guid = Guid.NewGuid();
    }

    internal Element GetApiObjectClone(int i) {
      return new Element() {
        Group = ApiElements[i].Group,
        IsDummy = ApiElements[i].IsDummy,
        Name = ApiElements[i].Name.ToString(),
        OrientationNode = ApiElements[i].OrientationNode,
        OrientationAngle = ApiElements[i].OrientationAngle,
        Offset = ApiElements[i].Offset,
        ParentMember = ApiElements[i].ParentMember,
        Property = ApiElements[i].Property,
        Topology = new ReadOnlyCollection<int>(ApiElements[i].Topology.ToList()),
        Type = ApiElements[i].Type,
      };
    }

    private void CloneApiElements(
      ApiObjectMember memType, IList<int> grp = null, IList<bool> dum = null,
      IList<string> nm = null, IList<Angle> oriA = null, IList<GsaOffset> off = null,
      IList<int> prop = null, IList<ElementType> typ = null, IList<Color> col = null) {
      var elems = new List<Element>();
      for (int i = 0; i < ApiElements.Count; i++) {
        elems.Add(new Element() {
          Group = ApiElements[i].Group,
          IsDummy = ApiElements[i].IsDummy,
          Name = ApiElements[i].Name.ToString(),
          OrientationNode = ApiElements[i].OrientationNode,
          OrientationAngle = ApiElements[i].OrientationAngle,
          Offset = ApiElements[i].Offset,
          ParentMember = ApiElements[i].ParentMember,
          Property = ApiElements[i].Property,
          Type = ApiElements[i].Type,
        });
        elems[i].Topology = new ReadOnlyCollection<int>(ApiElements[i].Topology.ToList());

        if (memType == ApiObjectMember.All) {
          continue;
        }

        switch (memType) {
          case ApiObjectMember.Group:
            elems[i].Group = grp.Count > i ? grp[i] : grp.Last();
            break;

          case ApiObjectMember.Dummy:
            elems[i].IsDummy = dum.Count > i ? dum[i] : dum.Last();
            break;

          case ApiObjectMember.Name:
            elems[i].Name = nm.Count > i ? nm[i] : nm.Last();
            break;

          case ApiObjectMember.OrientationAngle:
            elems[i].OrientationAngle = oriA.Count > i ? oriA[i].Degrees : oriA.Last().Degrees;
            break;

          case ApiObjectMember.Offset:
            if (off.Count > i) {
              elems[i].Offset.X1 = off[i].X1.Meters;
              elems[i].Offset.X2 = off[i].X2.Meters;
              elems[i].Offset.Y = off[i].Y.Meters;
              elems[i].Offset.Z = off[i].Z.Meters;
            } else {
              elems[i].Offset.X1 = off.Last().X1.Meters;
              elems[i].Offset.X2 = off.Last().X2.Meters;
              elems[i].Offset.Y = off.Last().Y.Meters;
              elems[i].Offset.Z = off.Last().Z.Meters;
            }

            break;

          case ApiObjectMember.Property:
            elems[i].Property = prop.Count > i ? prop[i] : prop.Last();
            break;

          case ApiObjectMember.Type:
            elems[i].Type = typ.Count > i ? typ[i] : typ.Last();
            break;

          case ApiObjectMember.Colour:
            elems[i].Colour = col.Count > i ? col[i] : (ValueType)col.Last();

            Mesh.VertexColors.SetColor(i, (Color)elems[i].Colour);
            break;
        }
      }

      ApiElements = elems;
    }
  }
}
