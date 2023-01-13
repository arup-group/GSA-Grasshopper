using System;
using System.Collections.Generic;
using System.Linq;
using GsaAPI;
using GsaGH.Helpers.Import;
using GsaGH.Parameters;
using OasysUnits;
using OasysUnits.Units;
using Rhino.Geometry;

namespace GsaGH.Helpers.Export
{
    internal class Members
  {
    #region topologylist
    private static string CreateTopology(List<Point3d> topology, List<string> topoType, ref GsaIntDictionary<Node> existingNodes, LengthUnit unit)
    {
      string topo = "";
      if (topology == null)
        return topo;
      for (int j = 0; j < topology.Count; j++)
      {
        // add the topology type to the topo string first
        if (topoType != null)
        {
          if (j > 0)
          {
            string topologyType = topoType[j];
            if (topologyType == "" | topologyType == " ")
              topo += " ";
            else
              topo += topologyType.ToLower() + " "; // add topology type (nothing or "a") in front of node id
          }
        }
        topo += Nodes.AddNode(ref existingNodes, topology[j], unit) + " ";
      }
      return topo.Trim();
    }
    #endregion

    private static void AddMember(int id, Guid guid, Member apiMember, ref GsaGuidDictionary<Member> apiMembers)
    {
      if (id > 0)
        apiMembers.SetValue(id, guid, apiMember);
      else
        apiMembers.AddValue(guid, apiMember);
    }

    #region member1d
    internal static void ConvertMember1D(GsaMember1d member1d,
        ref GsaGuidDictionary<Member> apiMembers, ref GsaIntDictionary<Node> existingNodes, LengthUnit unit,
        ref GsaGuidDictionary<Section> apiSections, ref GsaIntDictionary<SectionModifier> apiSectionModifiers, ref GsaGuidDictionary<AnalysisMaterial> apiMaterials)
    {
      Member apiMember = member1d.GetAPI_MemberClone();
      apiMember.MeshSize = new Length(member1d.MeshSize, unit).Meters;

      string topo = CreateTopology(member1d.Topology, member1d.TopologyType, ref existingNodes, unit);
      if (topo!= "")
      {
        try //GsaAPI will perform check on topology list
        {
          apiMember.Topology = string.Copy(topo.Replace("  ", " "));
        }
        catch (Exception)
        {
          List<string> errors = member1d.Topology.Select(t => "{" + t.ToString() + "}").ToList();
          string error = string.Join(", ", errors);
          throw new Exception(" Invalid topology for Member1d: " + topo + " with original points: " + error);
        }
      }

      if (member1d.OrientationNode != null)
        apiMember.OrientationNode = Nodes.AddNode(ref existingNodes, member1d.OrientationNode.Point, unit);

      apiMember.Property = Sections.ConvertSection(member1d.Section, ref apiSections, ref apiSectionModifiers, ref apiMaterials);

      AddMember(member1d.Id, member1d.Guid, apiMember, ref apiMembers);
    }

    internal static void ConvertMember1D(List<GsaMember1d> member1ds,
        ref GsaGuidDictionary<Member> apiMembers, ref GsaIntDictionary<Node> existingNodes, LengthUnit unit,
        ref GsaGuidDictionary<Section> apiSections, ref GsaIntDictionary<SectionModifier> apiSectionModifiers, ref GsaGuidDictionary<AnalysisMaterial> apiMaterials)
    {
      if (member1ds != null)
      {
        member1ds = member1ds.OrderByDescending(x => x.Id).ToList();
        for (int i = 0; i < member1ds.Count; i++)
          if (member1ds[i] != null)
            ConvertMember1D(member1ds[i], ref apiMembers, ref existingNodes, unit, ref apiSections, ref apiSectionModifiers, ref apiMaterials);
      }
    }
    #endregion

    #region member2d
    
    internal static void ConvertMember2D(GsaMember2d member2d,
        ref GsaGuidDictionary<Member> apiMembers, ref GsaIntDictionary<Node> existingNodes, LengthUnit unit,
        ref GsaGuidDictionary<Prop2D> apiProp2ds, ref GsaGuidDictionary<AnalysisMaterial> apiMaterials)
    {
      Member apiMember = member2d.GetAPI_MemberClone();
      apiMember.MeshSize = new Length(member2d.MeshSize, unit).Meters;

      string topo = CreateTopology(member2d.Topology, member2d.TopologyType, ref existingNodes, unit);

      if (member2d.VoidTopology != null)
        for (int i = 0; i < member2d.VoidTopology.Count; i++)
          topo += " V(" + CreateTopology(member2d.VoidTopology[i], member2d.VoidTopologyType[i], ref existingNodes, unit) + ")";

      if (member2d.IncLinesTopology != null)
        for (int i = 0; i < member2d.IncLinesTopology.Count; i++)
          topo += " L(" + CreateTopology(member2d.IncLinesTopology[i], member2d.IncLinesTopologyType[i], ref existingNodes, unit) + ")"; ;

      if (member2d.InclusionPoints != null)
        topo += " P(" + CreateTopology(member2d.InclusionPoints, null, ref existingNodes, unit) + ")"; ;

      try
      {
        apiMember.Topology = string.Copy(topo.Replace("( ", "(").Replace("  ", " "));
      }
      catch (Exception)
      {
        List<string> errors = member2d.Topology.Select(t => "{" + t.ToString() + "}").ToList();
        string error = string.Join(", ", errors);
        throw new Exception(" Invalid topology for Member2d: " + topo + " with original points: " + error);
      }

      apiMember.Property = Prop2ds.ConvertProp2d(member2d.Property, ref apiProp2ds, ref apiMaterials);

      AddMember(member2d.Id, member2d.Guid, apiMember, ref apiMembers);
    }

    internal static void ConvertMember2D(List<GsaMember2d> member2ds,
        ref GsaGuidDictionary<Member> apiMembers, ref GsaIntDictionary<Node> existingNodes, LengthUnit unit,
        ref GsaGuidDictionary<Prop2D> apiProp2ds, ref GsaGuidDictionary<AnalysisMaterial> apiMaterials)
    {
      if (member2ds != null)
      {
        member2ds = member2ds.OrderByDescending(x => x.Id).ToList();
        for (int i = 0; i < member2ds.Count; i++)
          if (member2ds[i] != null)
            ConvertMember2D(member2ds[i], ref apiMembers, ref existingNodes, unit, ref apiProp2ds, ref apiMaterials);
      }
    }
    #endregion

    #region member3d
    internal static void ConvertMember3D(GsaMember3d member3d,
        ref GsaGuidDictionary<Member> apiMembers, ref GsaIntDictionary<Node> existingNodes, LengthUnit unit,
        ref GsaGuidDictionary<Prop3D> apiProp3ds, ref GsaGuidDictionary<AnalysisMaterial> apiMaterials)
    {
      Member apiMember = member3d.GetAPI_MemberClone();
      apiMember.MeshSize = new Length(member3d.MeshSize, unit).Meters;

      // update topology list to fit model nodes

      List<string> topos = new List<string>();
      // Loop through the face list
      
      List<int> topoInts = new List<int>();
      foreach (Point3d verticy in member3d.SolidMesh.TopologyVertices)
        topoInts.Add(Nodes.AddNode(ref existingNodes, verticy, unit));

      for (int j = 0; j < member3d.SolidMesh.Faces.Count; j++)
        topos.Add(string.Join(" ", topoInts[member3d.SolidMesh.Faces[j].A], topoInts[member3d.SolidMesh.Faces[j].B], topoInts[member3d.SolidMesh.Faces[j].C]));

      string topo = string.Join("; ", topos);
      apiMember.Topology = string.Copy(topo);

      apiMember.Property = Prop3ds.ConvertProp3d(member3d.Prop3d, ref apiProp3ds, ref apiMaterials);

      AddMember(member3d.Id, member3d.Guid, apiMember, ref apiMembers);
    }

    internal static void ConvertMember3D(List<GsaMember3d> member3ds,
        ref GsaGuidDictionary<Member> apiMembers, ref GsaIntDictionary<Node> existingNodes, LengthUnit unit,
        ref GsaGuidDictionary<Prop3D> apiProp3ds, ref GsaGuidDictionary<AnalysisMaterial> apiMaterials)
    {
      if (member3ds != null)
      {
        member3ds = member3ds.OrderByDescending(x => x.Id).ToList();
        for (int i = 0; i < member3ds.Count; i++)
          if (member3ds[i] != null)
            ConvertMember3D(member3ds[i], ref apiMembers, ref existingNodes, unit, ref apiProp3ds, ref apiMaterials);
      }
    }
    #endregion
  }
}
