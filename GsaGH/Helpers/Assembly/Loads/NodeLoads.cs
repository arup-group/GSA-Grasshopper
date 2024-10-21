using System.Collections.Generic;
using System.Linq;

using GsaAPI;

using GsaGH.Parameters;

using Rhino.Geometry;

using NodeLoadType = GsaGH.Parameters.NodeLoadType;

namespace GsaGH.Helpers.Assembly {
  internal partial class ModelAssembly {
    private List<NodeLoad> _nodeLoads;
    private List<NodeLoad> _displacements;
    private List<NodeLoad> _settlements;

    private void ConvertNodeLoads(List<IGsaLoad> loads) {
      if (!loads.IsNullOrEmpty()) {
        _deleteResults = true;
      }
      if (loads == null) {
        return;
      }

      foreach (IGsaLoad load in loads.Where(load => load != null)
       .Where(load => load is GsaNodeLoad)) {
        ConvertNodeLoad((GsaNodeLoad)load);
      }
    }

    private void ConvertNodeLoad(GsaNodeLoad load) {
      if (load._refPoint != Point3d.Unset) {
        load.ApiLoad.Nodes = AddNode(load._refPoint).ToString();
      }

      if (load.ReferenceList != null) {
        load.ApiLoad.Nodes = GetNodeList(load.ReferenceList);
      }

      if (load.LoadCase != null) {
        load.CaseId = load.LoadCase.Id;
      }

      switch (load.Type) {
        case NodeLoadType.AppliedDisp:
          _displacements.Add(load.ApiLoad);
          break;

        case NodeLoadType.NodeLoad:
          _nodeLoads.Add(load.ApiLoad);
          break;

        case NodeLoadType.Settlement:
          _settlements.Add(load.ApiLoad);
          break;
      }

      PostHog.Load(load._refPoint != Point3d.Unset, load.Type.ToString());
    }
  }
}
