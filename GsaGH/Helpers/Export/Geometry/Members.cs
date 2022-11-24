using System;
using System.Collections.Generic;
using System.Linq;
using GsaAPI;
using GsaGH.Parameters;
using OasysUnits;
using OasysUnits.Units;
using Rhino.Geometry;

namespace GsaGH.Helpers.Export
{
    internal class Members
  {
    #region topologylist
    private static string CreateTopology(List<Point3d> topology, List<string> topoType, ref GsaIntKeyDictionary<Node> existingNodes, LengthUnit unit)
    {
      string topo = "";
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

    #region member1d
    /// <summary>
    /// Method to create API Member
    /// </summary>
    /// <param name="member1d"></param>
    /// <param name="existingMembers"></param>
    /// <param name="memberidcounter"></param>
    /// <param name="existingNodes"></param>
    /// <param name="unit"></param>
    /// <param name="existingSections"></param>
    /// <param name="sections_guid"></param>
    /// <param name="existingSectionModifiers"></param>
    /// <param name="existingMaterials"></param>
    /// <param name="materials_guid"></param>
    /// <exception cref="Exception"></exception>
    internal static void ConvertMember1D(GsaMember1d member1d,
        ref Dictionary<int, Member> existingMembers, ref int memberidcounter,
        ref GsaIntKeyDictionary<Node> existingNodes, LengthUnit unit,
        ref Dictionary<int, Section> existingSections, ref Dictionary<Guid, int> sections_guid,
        ref Dictionary<int, SectionModifier> existingSectionModifiers,
        ref Dictionary<int, AnalysisMaterial> existingMaterials, ref Dictionary<Guid, int> materials_guid, LengthUnit modelUnit)
    {
      Member apiMember = member1d.GetAPI_MemberClone();
      apiMember.MeshSize = new Length(member1d.MeshSize, modelUnit).Meters;

      string topo = CreateTopology(member1d.Topology, member1d.TopologyType, ref existingNodes, unit);

      try
      {
        apiMember.Topology = string.Copy(topo.Replace("  ", " "));
      }
      catch (Exception)
      {
        List<string> errors = member1d.Topology.Select(t => "{" + t.ToString() + "}").ToList();
        string error = string.Join(", ", errors);
        throw new Exception(" Invalid topology for Member1d: " + topo + " with original points: " + error);
      }

      if (member1d.OrientationNode != null)
        apiMember.OrientationNode = Nodes.AddNode(ref existingNodes, member1d.OrientationNode.Point, unit);

      apiMember.Property = Sections.ConvertSection(member1d.Section, ref existingSections, ref sections_guid, ref existingSectionModifiers, ref existingMaterials, ref materials_guid);

      if (member1d.Id > 0) // if the ID is larger than 0 than means the ID has been set and we sent it to the known list
        existingMembers[member1d.Id] = apiMember;
      else
        existingMembers.Add(memberidcounter++, apiMember);
    }

    /// <summary>
    /// Method to create a list of API Members
    /// </summary>
    /// <param name="member1ds"></param>
    /// <param name="existingMembers"></param>
    /// <param name="memberidcounter"></param>
    /// <param name="existingNodes"></param>
    /// <param name="unit"></param>
    /// <param name="existingSections"></param>
    /// <param name="sections_guid"></param>
    /// <param name="existingSectionModifiers"></param>
    /// <param name="existingMaterials"></param>
    /// <param name="materials_guid"></param>
    internal static void ConvertMember1D(List<GsaMember1d> member1ds,
        ref Dictionary<int, Member> existingMembers, ref int memberidcounter,
        ref GsaIntKeyDictionary<Node> existingNodes, LengthUnit unit,
        ref Dictionary<int, Section> existingSections, ref Dictionary<Guid, int> sections_guid,
        ref Dictionary<int, SectionModifier> existingSectionModifiers,
        ref Dictionary<int, AnalysisMaterial> existingMaterials, ref Dictionary<Guid, int> materials_guid, LengthUnit modelUnit)
    {
      if (member1ds != null)
        for (int i = 0; i < member1ds.Count; i++)
          if (member1ds[i] != null)
            ConvertMember1D(member1ds[i], ref existingMembers, ref memberidcounter, ref existingNodes, unit, ref existingSections, ref sections_guid, ref existingSectionModifiers, ref existingMaterials, ref materials_guid, modelUnit);
    }
    #endregion

    #region member2d
    /// <summary>
    /// Method to create API Member
    /// </summary>
    /// <param name="member2d"></param>
    /// <param name="existingMembers"></param>
    /// <param name="memberidcounter"></param>
    /// <param name="existingNodes"></param>
    /// <param name="unit"></param>
    /// <param name="existingProp2Ds"></param>
    /// <param name="prop2d_guid"></param>
    /// <param name="existingMaterials"></param>
    /// <param name="materials_guid"></param>
    internal static void ConvertMember2D(GsaMember2d member2d,
        ref Dictionary<int, Member> existingMembers, ref int memberidcounter,
        ref GsaIntKeyDictionary<Node> existingNodes, LengthUnit unit,
        ref Dictionary<int, Prop2D> existingProp2Ds, ref Dictionary<Guid, int> prop2d_guid,
        ref Dictionary<int, AnalysisMaterial> existingMaterials, ref Dictionary<Guid, int> materials_guid, LengthUnit modelUnit)
    {
      Member apiMember = member2d.GetAPI_MemberClone();
      apiMember.MeshSize = new Length(member2d.MeshSize, modelUnit).Meters;

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

      apiMember.Property = Prop2ds.ConvertProp2d(member2d.Property, ref existingProp2Ds, ref prop2d_guid, ref existingMaterials, ref materials_guid);

      if (member2d.Id > 0) // if the ID is larger than 0 than means the ID has been set and we sent it to the known list
        existingMembers[member2d.Id] = apiMember;
      else
        existingMembers.Add(memberidcounter++, apiMember);
    }

    /// <summary>
    /// Method to create a list of API Members
    /// </summary>
    /// <param name="member2ds"></param>
    /// <param name="existingMembers"></param>
    /// <param name="memberidcounter"></param>
    /// <param name="existingNodes"></param>
    /// <param name="unit"></param>
    /// <param name="existingProp2Ds"></param>
    /// <param name="prop2d_guid"></param>
    /// <param name="existingMaterials"></param>
    /// <param name="materials_guid"></param>
    internal static void ConvertMember2D(List<GsaMember2d> member2ds,
        ref Dictionary<int, Member> existingMembers, ref int memberidcounter,
        ref GsaIntKeyDictionary<Node> existingNodes, LengthUnit unit,
        ref Dictionary<int, Prop2D> existingProp2Ds, ref Dictionary<Guid, int> prop2d_guid,
        ref Dictionary<int, AnalysisMaterial> existingMaterials, ref Dictionary<Guid, int> materials_guid, LengthUnit modelUnit)
    {
      if (member2ds != null)
        for (int i = 0; i < member2ds.Count; i++)
          if (member2ds[i] != null)
            ConvertMember2D(member2ds[i],
               ref existingMembers, ref memberidcounter,
               ref existingNodes, unit,
               ref existingProp2Ds, ref prop2d_guid,
               ref existingMaterials, ref materials_guid, modelUnit);
    }
    #endregion

    #region member3d
    /// <summary>
    /// Method to create API Member by using existing points when within tolerance.
    /// </summary>
    /// <param name="member3d"></param>
    /// <param name="existingMembers"></param>
    /// <param name="memberidcounter"></param>
    /// <param name="existingNodes"></param>
    /// <param name="nodeidcounter"></param>
    /// <param name="unit"></param>
    internal static void ConvertMember3D(GsaMember3d member3d,
        ref Dictionary<int, Member> existingMembers, ref int memberidcounter,
        ref GsaIntKeyDictionary<Node> existingNodes, LengthUnit modelUnit,
        ref Dictionary<int, Prop3D> existingProp3Ds, ref Dictionary<Guid, int> prop3d_guid,
        ref Dictionary<int, AnalysisMaterial> existingMaterials, ref Dictionary<Guid, int> materials_guid)
    {
      Member apiMember = member3d.GetAPI_MemberClone();
      apiMember.MeshSize = new Length(member3d.MeshSize, modelUnit).Meters;

      // update topology list to fit model nodes

      List<string> topos = new List<string>();
      // Loop through the face list
      for (int j = 0; j < member3d.SolidMesh.Faces.Count; j++)
        topos.Add(string.Join(" ",
          Nodes.AddNode(ref existingNodes, member3d.SolidMesh.Vertices[member3d.SolidMesh.Faces[j].A], modelUnit),
          Nodes.AddNode(ref existingNodes, member3d.SolidMesh.Vertices[member3d.SolidMesh.Faces[j].B], modelUnit),
          Nodes.AddNode(ref existingNodes, member3d.SolidMesh.Vertices[member3d.SolidMesh.Faces[j].C], modelUnit)));

      string topo = string.Join("; ", topos);
      apiMember.Topology = string.Copy(topo);

      apiMember.Property = Prop3ds.ConvertProp3d(member3d.Property, ref existingProp3Ds, ref prop3d_guid, ref existingMaterials, ref materials_guid);

      if (member3d.Id > 0) // if the ID is larger than 0 than means the ID has been set and we sent it to the known list
        existingMembers[member3d.Id] = apiMember;
      else
        existingMembers.Add(memberidcounter++, apiMember);
    }

    /// <summary>
    /// Method to create a list of API Members by using existing points when within tolerance.
    /// </summary>
    /// <param name="member3ds"></param>
    /// <param name="existingMembers"></param>
    /// <param name="memberidcounter"></param>
    /// <param name="existingNodes"></param>
    /// <param name="unit"></param>
    internal static void ConvertMember3D(List<GsaMember3d> member3ds,
    ref Dictionary<int, Member> existingMembers, ref int memberidcounter,
    ref GsaIntKeyDictionary<Node> existingNodes, LengthUnit unit,
    ref Dictionary<int, Prop3D> existingProp3Ds, ref Dictionary<Guid, int> prop3d_guid,
        ref Dictionary<int, AnalysisMaterial> existingMaterials, ref Dictionary<Guid, int> materials_guid)
    {
      if (member3ds != null)
        for (int i = 0; i < member3ds.Count; i++)
          if (member3ds[i] != null)
            ConvertMember3D(member3ds[i], 
              ref existingMembers, ref memberidcounter, 
              ref existingNodes, unit,
              ref existingProp3Ds, ref prop3d_guid,
              ref existingMaterials, ref materials_guid);
    }
    #endregion
  }
}
