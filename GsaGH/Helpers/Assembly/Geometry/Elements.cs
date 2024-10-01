using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using GsaAPI;

using GsaGH.Parameters;

namespace GsaGH.Helpers.Assembly {
  internal partial class ModelAssembly {
    private void AddElement(int id, Guid guid, GSAElement apiElement, bool overwrite) {
      if (id > 0) {
        _elements.SetValue(id, guid, apiElement, overwrite);
      } else {
        _elements.AddValue(guid, apiElement);
      }
    }

    private void ConvertElement1d(GsaElement1d element1d) {
      GSAElement apiElement = element1d.DuplicateApiObject();
      var topo = new List<int> {
        AddNode(element1d.Line.PointAtStart),
        AddNode(element1d.Line.PointAtEnd),
      };
      apiElement.Topology = new ReadOnlyCollection<int>(topo);
      if (element1d.OrientationNode != null) {
        apiElement.OrientationNode
          = AddNode(element1d.OrientationNode.Point);
      }

      if (element1d.ApiElement.Type != ElementType.SPRING) {
        apiElement.Property = ConvertSection(element1d.Section);
      } else {
        apiElement.Property = ConvertSpringProp(element1d.SpringProperty);
      }

      AddElement(element1d.Id, element1d.Guid, apiElement, true);
    }

    private void ConvertElement1ds(List<GsaElement1d> element1ds) {
      if (element1ds == null) {
        return;
      }

      element1ds = element1ds.OrderByDescending(x => x.Id).ToList();
      foreach (GsaElement1d element1d in element1ds.Where(element1d => element1d != null)) {
        ConvertElement1d(element1d);
      }
    }

    private void ConvertElement2d(GsaElement2d element2d) {
      List<GSAElement> apiElems = element2d.DuplicateApiObjects();
      for (int i = 0; i < apiElems.Count; i++) {
        GSAElement apiMeshElement = apiElems[i];
        List<int> meshVertexIndex = element2d.TopoInt[i];

        var topo = new List<int>();
        foreach (int mesh in meshVertexIndex) {
          topo.Add(AddNode(element2d.Topology[mesh]));
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

    private void ConvertElement2ds(List<GsaElement2d> element2ds) {
      if (element2ds == null) {
        return;
      }

      element2ds = element2ds.OrderByDescending(e => e.Ids.First()).ToList();
      foreach (GsaElement2d element2d in element2ds) {
        if (element2d != null) {
          ConvertElement2d(element2d);
        }
      }
    }

    private void ConvertElement3d(GsaElement3d element3d) {
      List<GSAElement> apiElems = element3d.DuplicateApiObjects();
      for (int i = 0; i < apiElems.Count; i++) {
        GSAElement apiMeshElement = apiElems[i];
        List<int> meshVertexIndex = element3d.TopoInt[i];
        var topo = new List<int>();
        foreach (int mesh in meshVertexIndex) {
          topo.Add(AddNode(element3d.Topology[mesh]));
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

    private void ConvertElement3ds(List<GsaElement3d> element3ds) {
      if (element3ds == null) {
        return;
      }

      element3ds = element3ds.OrderByDescending(e => e.Ids.First()).ToList();
      foreach (GsaElement3d element3d in element3ds) {
        if (element3d != null) {
          ConvertElement3d(element3d);
        }
      }
    }
  }
}
