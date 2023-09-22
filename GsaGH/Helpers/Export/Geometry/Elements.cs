using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GsaAPI;
using GsaGH.Parameters;
using Rhino.Collections;
using Rhino.Geometry;

namespace GsaGH.Helpers.Export {
  internal partial class ModelAssembly {
    internal void ConvertElement1D(GsaElement1d element1d) {
      LineCurve line = element1d.Line;
      Element apiElement = element1d.DuplicateApiObject();
      var topo = new List<int> {
        AddNode( line.PointAtStart),
        AddNode(line.PointAtEnd),
      };
      apiElement.Topology = new ReadOnlyCollection<int>(topo);
      if (element1d.OrientationNode != null) {
        apiElement.OrientationNode
          = AddNode(element1d.OrientationNode.Point);
      }

      apiElement.Property = ConvertSection(element1d.Section);
      AddElement(element1d.Id, element1d.Guid, apiElement, true);
    }

    internal void ConvertElement1ds(List<GsaElement1d> element1ds) {
      if (element1ds == null) {
        return;
      }

      element1ds = element1ds.OrderByDescending(x => x.Id).ToList();
      foreach (GsaElement1d element1d in element1ds.Where(element1d => element1d != null)) {
        ConvertElement1D(element1d);
      }
    }

    internal void ConvertElement2D(GsaElement2d element2d) {
      Point3dList meshVerticies = element2d.Topology;
      List<Element> apiElems = element2d.DuplicateApiObjects();
      for (int i = 0; i < apiElems.Count; i++) {
        Element apiMeshElement = apiElems[i];
        List<int> meshVertexIndex = element2d.TopoInt[i];

        var topo = new List<int>();
        foreach (int mesh in meshVertexIndex) {
          topo.Add(AddNode(meshVerticies[mesh]));
        }

        apiMeshElement.Topology = new ReadOnlyCollection<int>(topo);
        if (!element2d.Prop2ds.IsNullOrEmpty()) {
          GsaProperty2d prop = (i > element2d.Prop2ds.Count - 1) ? element2d.Prop2ds.Last() :
          element2d.Prop2ds[i];
          apiMeshElement.Property = ConvertProp2d(prop);
        }

        AddElement(element2d.Ids[i], element2d.Guid, apiMeshElement, false);
      }
    }

    internal void ConvertElement2ds(List<GsaElement2d> element2ds) {
      if (element2ds == null) {
        return;
      }

      element2ds = element2ds.OrderByDescending(e => e.Ids.First()).ToList();
      foreach (GsaElement2d element2d in element2ds) {
        if (element2d != null) {
          ConvertElement2D(element2d);
        }
      }
    }

    internal void ConvertElement3D(GsaElement3d element3d) {
      Point3dList meshVerticies = element3d.Topology;
      List<Element> apiElems = element3d.DuplicateApiObjects();
      for (int i = 0; i < apiElems.Count; i++) {
        Element apiMeshElement = apiElems[i];
        List<int> meshVertexIndex = element3d.TopoInt[i];
        var topo = new List<int>();
        foreach (int mesh in meshVertexIndex) {
          topo.Add(AddNode(meshVerticies[mesh]));
        }

        apiMeshElement.Topology = new ReadOnlyCollection<int>(topo);
        if (!element3d.Prop3ds.IsNullOrEmpty()) {
          GsaProperty3d prop = (i > element3d.Prop3ds.Count - 1) ? element3d.Prop3ds.Last() :
          element3d.Prop3ds[i];
          apiMeshElement.Property = ConvertProp3d(prop);
        }

        AddElement(element3d.Ids[i], element3d.Guid, apiMeshElement, false);
      }
    }

    internal void ConvertElement3ds(
      List<GsaElement3d> element3ds) {
      if (element3ds == null) {
        return;
      }

      element3ds = element3ds.OrderByDescending(e => e.Ids.First()).ToList();
      foreach (GsaElement3d element3d in element3ds) {
        if (element3d != null) {
          ConvertElement3D(element3d);
        }
      }
    }

    private void AddElement(int id, Guid guid, Element apiElement, bool overwrite) {
      if (id > 0) {
        Elements.SetValue(id, guid, apiElement, overwrite);
      } else {
        Elements.AddValue(guid, apiElement);
      }
    }
  }
}
