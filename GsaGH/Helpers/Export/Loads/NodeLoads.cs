using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GsaAPI;
using GsaGH.Parameters;
using GsaGH.Parameters.Enums;
using OasysUnits;
using Rhino.Geometry;
using LengthUnit = OasysUnits.Units.LengthUnit;
using NodeLoadType = GsaGH.Parameters.NodeLoadType;

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
      apiModel.AddNodeLoads(GsaAPI.NodeLoadType.APPL_DISP, new ReadOnlyCollection<NodeLoad>(Displacements));
      apiModel.AddNodeLoads(GsaAPI.NodeLoadType.NODE_LOAD, new ReadOnlyCollection<NodeLoad>(Nodes));
      apiModel.AddNodeLoads(GsaAPI.NodeLoadType.SETTLEMENT, new ReadOnlyCollection<NodeLoad>(Settlements));
    }

    internal static void ConvertNodeLoads(
      List<IGsaLoad> loads, ref NodeLoads nodeloads, ref GsaIntDictionary<Node> apiNodes,
      ref GsaGuidDictionary<EntityList> apiLists, LengthUnit unit) {
      if (loads == null) {
        return;
      }

      foreach (IGsaLoad load in loads.Where(load => load != null)
       .Where(load => load is GsaNodeLoad)) {
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
        load.ApiLoad.Nodes
          = Export.Nodes.AddNode(ref apiNodes, load._refPoint, unit).ToString();
      }

      if (load.ReferenceList != null) {
        load.ApiLoad.Nodes = Lists.GetNodeList(
          load.ReferenceList, ref apiLists, ref apiNodes, unit);
      }

      load.CaseId = load.LoadCase.Id;

      switch (load.Type) {
        case NodeLoadType.AppliedDisp:
          loads.Displacements.Add(load.ApiLoad);
          break;

        case NodeLoadType.NodeLoad:
          loads.Nodes.Add(load.ApiLoad);
          break;

        case NodeLoadType.Settlement:
          loads.Settlements.Add(load.ApiLoad);
          break;
      }

      PostHog.Load(load._refPoint != Point3d.Unset, load.Type.ToString());
    }
  }
}
