using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GsaAPI;
using GsaGH.Parameters;
using Rhino.Geometry;
using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGH.Helpers.Export.Load {
  internal class NodeLoads {
    internal List<NodeLoad> Nodes;
    internal List<NodeLoad> Displacements;
    internal List<NodeLoad> Settlements;
    public NodeLoads() {
      Nodes = new List<NodeLoad>();
      Displacements = new List<NodeLoad>();
      Settlements = new List<NodeLoad>();
    }

    internal void Assemble(ref Model apiModel) {
      apiModel.AddNodeLoads(NodeLoadType.APPL_DISP, new ReadOnlyCollection<NodeLoad>(Displacements));
      apiModel.AddNodeLoads(NodeLoadType.NODE_LOAD, new ReadOnlyCollection<NodeLoad>(Nodes));
      apiModel.AddNodeLoads(NodeLoadType.SETTLEMENT, new ReadOnlyCollection<NodeLoad>(Settlements));
    }

    internal static void ConvertNodeLoads(
      List<IGsaLoad> loads, ref NodeLoads nodeloads, ref GsaIntDictionary<Node> apiNodes,
      ref GsaGuidDictionary<EntityList> apiLists, LengthUnit unit) {
      if (loads == null) {
        return;
      }

      foreach (IGsaLoad load in loads.Where(load => load != null)
       .Where(load => load.LoadType == LoadType.Node)) {
        ConvertNodeLoad((GsaNodeLoad)load, ref nodeloads, ref apiNodes, ref apiLists, unit);
      }
    }

    internal static void ConvertNodeLoad(
      GsaNodeLoad load,
      ref NodeLoads loads,
      ref GsaIntDictionary<Node> apiNodes,
      ref GsaGuidDictionary<EntityList> apiLists,
      LengthUnit unit) {
      if (load._refPoint != Point3d.Unset) {
        load.NodeLoad.Nodes
          = Export.Nodes.AddNode(ref apiNodes, load._refPoint, unit).ToString();
      }

      if (load.ReferenceList != null) {
        load.NodeLoad.Nodes = Lists.GetNodeList(
          load.ReferenceList, ref apiLists, ref apiNodes, unit);
      }

      switch (load.Type) {
        case GsaNodeLoad.NodeLoadType.AppliedDisp:
          loads.Displacements.Add(load.NodeLoad);
          break;

        case GsaNodeLoad.NodeLoadType.NodeLoad:
          loads.Nodes.Add(load.NodeLoad);
          break;

        case GsaNodeLoad.NodeLoadType.Settlement:
          loads.Settlements.Add(load.NodeLoad);
          break;
      }

      PostHog.Load(load._refPoint != Point3d.Unset, load.Type.ToString());
    }
  }
}
