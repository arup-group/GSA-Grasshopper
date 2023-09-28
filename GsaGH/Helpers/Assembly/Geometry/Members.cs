﻿using System;
using System.Collections.Generic;
using System.Linq;
using GsaAPI;
using GsaGH.Parameters;
using OasysUnits;
using Rhino.Collections;
using Rhino.Geometry;

namespace GsaGH.Helpers.Assembly {
  internal partial class ModelAssembly {
    private void AddMember(int id, Guid guid, Member apiMember) {
      if (id > 0) {
        _members.SetValue(id, guid, apiMember);
      } else {
        _members.AddValue(guid, apiMember);
      }
    }

    private void ConvertMember1d(GsaMember1d member1d) {
      Member apiMember = member1d.DuplicateApiObject();
      apiMember.MeshSize = new Length(member1d.ApiMember.MeshSize, _unit).Meters;

      string topo = CreateTopology(member1d.Topology, member1d.TopologyType);
      if (topo != string.Empty) {
        try {
          apiMember.Topology = string.Copy(topo.TrimSpaces());
        } catch (Exception) {
          var errors = member1d.Topology.Select(t => "{" + t.ToString() + "}").ToList();
          string error = string.Join(", ", errors);
          throw new Exception(" Invalid topology for Member1d: " + topo + " with original points: "
            + error);
        }
      }

      if (member1d.OrientationNode != null) {
        apiMember.OrientationNode = AddNode(member1d.OrientationNode.Point);
      }

      apiMember.Property = ConvertSection(member1d.Section);

      AddMember(member1d.Id, member1d.Guid, apiMember);
    }

    private void ConvertMember1ds(List<GsaMember1d> member1ds) {
      if (member1ds == null) {
        return;
      }

      member1ds = member1ds.OrderByDescending(x => x.Id).ToList();
      foreach (GsaMember1d member in member1ds.Where(member => member != null)) {
        ConvertMember1d(member);
      }
    }

    private void ConvertMember2d(GsaMember2d member2d) {
      Member apiMember = member2d.DuplicateApiObject();
      apiMember.MeshSize = new Length(member2d.ApiMember.MeshSize, _unit).Meters;

      string topo
        = CreateTopology(member2d.Topology, member2d.TopologyType);

      if (member2d.VoidTopology != null) {
        for (int i = 0; i < member2d.VoidTopology.Count; i++) {
          topo += " V(" + CreateTopology(member2d.VoidTopology[i], member2d.VoidTopologyType[i]) + ")";
        }
      }

      if (member2d.InclusionLinesTopology != null) {
        for (int i = 0; i < member2d.InclusionLinesTopology.Count; i++) {
          topo += " L(" + CreateTopology(member2d.InclusionLinesTopology[i],
            member2d.InclusionLinesTopologyType[i]) + ")";
        }
      }

      if (member2d.InclusionPoints != null) {
        topo += " P(" + CreateTopology(member2d.InclusionPoints, null)
          + ")";
      }

      try {
        apiMember.Topology = string.Copy(topo.Replace("( ", "(").TrimSpaces());
      } catch (Exception) {
        var errors = member2d.Topology.Select(t => "{" + t.ToString() + "}").ToList();
        string error = string.Join(", ", errors);
        throw new Exception(" Invalid topology for Member2d: " + topo + " with original points: "
          + error);
      }

      apiMember.Property = ConvertProp2d(member2d.Prop2d);

      AddMember(member2d.Id, member2d.Guid, apiMember);
    }

    private void ConvertMember2ds(List<GsaMember2d> member2ds) {
      if (member2ds == null) {
        return;
      }

      member2ds = member2ds.OrderByDescending(x => x.Id).ToList();
      foreach (GsaMember2d member2d in member2ds.Where(member2d => member2d != null)) {
        ConvertMember2d(member2d);
      }
    }

    private void ConvertMember3d(GsaMember3d member3d) {
      Member apiMember = member3d.DuplicateApiObject();
      apiMember.MeshSize = new Length(member3d.ApiMember.MeshSize, _unit).Meters;

      var topos = new List<string>();

      var topoInts = new List<int>();
      foreach (Point3d verticy in member3d.SolidMesh.TopologyVertices) {
        topoInts.Add(AddNode(verticy));
      }

      for (int j = 0; j < member3d.SolidMesh.Faces.Count; j++) {
        topos.Add(string.Join(" ", topoInts[member3d.SolidMesh.Faces[j].A],
          topoInts[member3d.SolidMesh.Faces[j].B], topoInts[member3d.SolidMesh.Faces[j].C]));
      }

      string topo = string.Join("; ", topos);
      apiMember.Topology = string.Copy(topo);

      apiMember.Property = ConvertProp3d(member3d.Prop3d);

      AddMember(member3d.Id, member3d.Guid, apiMember);
    }

    private void ConvertMember3ds(List<GsaMember3d> member3ds) {
      if (member3ds == null) {
        return;
      }

      member3ds = member3ds.OrderByDescending(x => x.Id).ToList();
      foreach (GsaMember3d member3d in member3ds.Where(member3d => member3d != null)) {
        ConvertMember3d(member3d);
      }
    }

    private string CreateTopology(Point3dList topology, IReadOnlyList<string> topoType) {
      string topo = string.Empty;
      if (topology == null) {
        return topo;
      }

      for (int j = 0; j < topology.Count; j++) {
        if (topoType != null) {
          if (j > 0) {
            string topologyType = topoType[j];
            if (topologyType.Trim() == string.Empty) {
              topo += " ";
            } else {
              topo += topologyType.ToLower()
                + " "; // add topology type (nothing or "a") in front of node id
            }
          }
        }

        topo += AddNode(topology[j]) + " ";
      }

      return topo.Trim();
    }
  }
}
