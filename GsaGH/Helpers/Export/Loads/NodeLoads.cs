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
      List<GsaLoad> loads, ref NodeLoads nodeloads, ref GsaIntDictionary<Node> apiNodes,
      ref GsaGuidDictionary<EntityList> apiLists, LengthUnit unit) {
      if (loads == null) {
        return;
      }

      foreach (GsaLoad load in loads.Where(load => load != null)
       .Where(load => load.LoadType == GsaLoad.LoadTypes.Node)) {
        ConvertNodeLoad(load, ref nodeloads, ref apiNodes, ref apiLists, unit);
      }
    }

    internal static void ConvertNodeLoad(
      GsaLoad load, 
      ref NodeLoads loads, 
      ref GsaIntDictionary<Node> apiNodes,
      ref GsaGuidDictionary<EntityList> apiLists, 
      LengthUnit unit) {
      if (load.NodeLoad._refPoint != Point3d.Unset) {
        load.NodeLoad.NodeLoad.Nodes
          = Export.Nodes.AddNode(ref apiNodes, load.NodeLoad._refPoint, unit).ToString();
      }

      if (load.NodeLoad._refList != null) {
        load.NodeLoad.NodeLoad.Nodes = Lists.GetNodeList(
          load.NodeLoad._refList, ref apiLists, ref apiNodes, unit);
      }

      switch (load.NodeLoad.Type) {
        case GsaNodeLoad.NodeLoadTypes.AppliedDisp:
          loads.Displacements.Add(load.NodeLoad.NodeLoad);
          break;

        case GsaNodeLoad.NodeLoadTypes.NodeLoad:
          loads.Nodes.Add(load.NodeLoad.NodeLoad);
          break;

        case GsaNodeLoad.NodeLoadTypes.Settlement:
          loads.Settlements.Add(load.NodeLoad.NodeLoad);
          break;
      }

      PostHog.Load(load.NodeLoad._refPoint != Point3d.Unset, load.NodeLoad.Type.ToString());
    }
  }
}
