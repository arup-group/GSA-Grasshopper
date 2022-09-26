using GsaAPI;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using GsaGH.Parameters;
using System.Linq;
using UnitsNet.Units;
using UnitsNet;

namespace GsaGH.Util.Gsa.ToGSA
{
  partial class Members
  {
    #region topologylist

    private static string CreateTopology(List<Point3d> topology, List<string> topoType, ref Dictionary<int, Node> existingNodes, ref int nodeidcounter, LengthUnit unit)
    {
      // create a topology string with node IDs as they are set in the model
      string topo = "";

      int prevID = -1;

      // Loop through the topology list
      for (int j = 0; j < topology.Count; j++)
      {
        Point3d pt = topology[j];
        int id = Nodes.GetExistingNodeID(existingNodes, pt, unit);

        // return if we have two nodes close to each other that they have the same ID
        if (id == prevID)
          continue;

        // add the topology type to the topo string first
        if (topoType != null)
        {
          string topologyType = topoType[j];
          if (j > 0)
          {
            if (topologyType == "" | topologyType == " ")
              topo += " ";
            else
              topo += topologyType.ToLower() + " "; // add topology type (nothing or "a") in front of node id
          }
        }
        else if (j > 0)
          topo += " ";

        // add the topology ID next
        if (id > 0)
        {
          topo += id;
          prevID = id;
        }
        else
        {
          existingNodes.Add(nodeidcounter, Nodes.NodeFromPoint(pt, unit));
          topo += nodeidcounter;
          prevID = nodeidcounter;
          nodeidcounter++;
        }

        // add a space unless we are at the end of the list
        if (j != topology.Count - 1)
          topo += " ";
      }

      return topo;
    }

    #endregion

    #region member1d
    /// <summary>
    /// Method to create API Member by using existing points when within tolerance.
    /// </summary>
    /// <param name="member1d"></param>
    /// <param name="existingMembers"></param>
    /// <param name="memberidcounter"></param>
    /// <param name="existingNodes"></param>
    /// <param name="nodeidcounter"></param>
    /// <param name="unit"></param>
    /// <param name="existingSections"></param>
    /// <param name="sections_guid"></param>
    /// <param name="existingSectionModifiers"></param>
    /// <param name="existingMaterials"></param>
    /// <param name="materials_guid"></param>
    /// <exception cref="Exception"></exception>
    public static void ConvertMember1D(GsaMember1d member1d,
        ref Dictionary<int, Member> existingMembers, ref int memberidcounter,
        ref Dictionary<int, Node> existingNodes, ref int nodeidcounter, LengthUnit unit,
        ref Dictionary<int, Section> existingSections, ref Dictionary<Guid, int> sections_guid,
        ref Dictionary<int, SectionModifier> existingSectionModifiers,
        ref Dictionary<int, AnalysisMaterial> existingMaterials, ref Dictionary<Guid, int> materials_guid)
    {
      Member apiMember = member1d.GetAPI_MemberClone();

      string topo = CreateTopology(member1d.Topology, member1d.TopologyType, ref existingNodes, ref nodeidcounter, unit);

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

      //Orientation node
      if (member1d.OrientationNode != null)
      {
        int id = Nodes.GetExistingNodeID(existingNodes, member1d.OrientationNode.Point, unit);
        if (id > 0)
          apiMember.OrientationNode = id;
        else
        {
          existingNodes.Add(nodeidcounter, Nodes.NodeFromPoint(member1d.OrientationNode.Point, unit));
          apiMember.OrientationNode = nodeidcounter;
          nodeidcounter++;
        }
      }

      // Section
      apiMember.Property = Sections.ConvertSection(member1d.Section, ref existingSections, ref sections_guid, ref existingSectionModifiers, ref existingMaterials, ref materials_guid);

      // set apimember in dictionary
      if (member1d.ID > 0) // if the ID is larger than 0 than means the ID has been set and we sent it to the known list
      {
        existingMembers[member1d.ID] = apiMember;
      }
      else
      {
        existingMembers.Add(memberidcounter, apiMember);
        memberidcounter++;
      }
    }
    /// <summary>
    /// Method to create a list of API Members by using existing points when within tolerance.
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
    public static void ConvertMember1D(List<GsaMember1d> member1ds,
        ref Dictionary<int, Member> existingMembers, ref int memberidcounter,
        ref Dictionary<int, Node> existingNodes, LengthUnit unit,
        ref Dictionary<int, Section> existingSections, ref Dictionary<Guid, int> sections_guid,
        ref Dictionary<int, SectionModifier> existingSectionModifiers,
        ref Dictionary<int, AnalysisMaterial> existingMaterials, ref Dictionary<Guid, int> materials_guid)
    {
      int nodeidcounter = (existingNodes.Count > 0) ? existingNodes.Keys.Max() + 1 : 1;

      // Mem1ds
      if (member1ds != null)
      {
        for (int i = 0; i < member1ds.Count; i++)
        {
          if (member1ds[i] != null)
          {
            GsaMember1d member1d = member1ds[i];

            ConvertMember1D(member1d, ref existingMembers, ref memberidcounter, ref existingNodes, ref nodeidcounter, unit, ref existingSections, ref sections_guid, ref existingSectionModifiers, ref existingMaterials, ref materials_guid);
          }
        }
      }
    }

    #endregion

    #region member2d
    /// <summary>
    /// Method to create API Member by using existing points when within tolerance.
    /// </summary>
    /// <param name="member2d"></param>
    /// <param name="existingMembers"></param>
    /// <param name="memberidcounter"></param>
    /// <param name="existingNodes"></param>
    /// <param name="nodeidcounter"></param>
    /// <param name="unit"></param>
    /// <param name="existingProp2Ds"></param>
    /// <param name="prop2d_guid"></param>
    /// <param name="existingMaterials"></param>
    /// <param name="materials_guid"></param>
    public static void ConvertMember2D(GsaMember2d member2d,
        ref Dictionary<int, Member> existingMembers, ref int memberidcounter,
        ref Dictionary<int, Node> existingNodes, ref int nodeidcounter, LengthUnit unit,
        ref Dictionary<int, Prop2D> existingProp2Ds, ref Dictionary<Guid, int> prop2d_guid,
        ref Dictionary<int, AnalysisMaterial> existingMaterials, ref Dictionary<Guid, int> materials_guid)
    {
      Member apiMember = member2d.GetAPI_MemberClone();

      // update topology list to fit model nodes
      string topo = CreateTopology(member2d.Topology, member2d.TopologyType, ref existingNodes, ref nodeidcounter, unit);

      // Loop through the voidtopology list
      if (member2d.VoidTopology != null)
      {
        for (int i = 0; i < member2d.VoidTopology.Count; i++)
        {
          string voidTopo = " V(";
          voidTopo += CreateTopology(member2d.VoidTopology[i], member2d.VoidTopologyType[i], ref existingNodes, ref nodeidcounter, unit);
          voidTopo += ")";

          topo += voidTopo;
        }
      }

      // Loop through the inclusion lines topology list  
      if (member2d.IncLinesTopology != null)
      {
        for (int i = 0; i < member2d.IncLinesTopology.Count; i++)
        {
          string inclLineTopo = " L(";
          inclLineTopo += CreateTopology(member2d.IncLinesTopology[i], member2d.IncLinesTopologyType[i], ref existingNodes, ref nodeidcounter, unit);
          inclLineTopo += ")";

          topo += inclLineTopo;
        }
      }

      // Loop through the inclucion point topology list
      if (member2d.InclusionPoints != null)
      {
        string inclPtTopo = " P(";
        inclPtTopo += CreateTopology(member2d.InclusionPoints, null, ref existingNodes, ref nodeidcounter, unit);
        inclPtTopo += ")";

        topo += inclPtTopo;
      }

      // update topology for api member
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

      // Prop2d
      apiMember.Property = Prop2ds.ConvertProp2d(member2d.Property, ref existingProp2Ds, ref prop2d_guid, ref existingMaterials, ref materials_guid);

      // set apimember in dictionary
      if (member2d.ID > 0) // if the ID is larger than 0 than means the ID has been set and we sent it to the known list
      {
        existingMembers[member2d.ID] = apiMember;
      }
      else
      {
        existingMembers.Add(memberidcounter, apiMember);
        memberidcounter++;
      }
    }
    /// <summary>
    /// Method to create a list of API Members by using existing points when within tolerance.
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
    public static void ConvertMember2D(List<GsaMember2d> member2ds,
        ref Dictionary<int, Member> existingMembers, ref int memberidcounter,
        ref Dictionary<int, Node> existingNodes, LengthUnit unit,
        ref Dictionary<int, Prop2D> existingProp2Ds, ref Dictionary<Guid, int> prop2d_guid,
        ref Dictionary<int, AnalysisMaterial> existingMaterials, ref Dictionary<Guid, int> materials_guid)
    {
      // create a counter for creating new elements, nodes and properties
      int nodeidcounter = (existingNodes.Count > 0) ? existingNodes.Keys.Max() + 1 : 1;

      if (member2ds != null)
      {

        for (int i = 0; i < member2ds.Count; i++)
        {
          if (member2ds[i] != null)
          {
            GsaMember2d member2d = member2ds[i];

            ConvertMember2D(member2d,
               ref existingMembers, ref memberidcounter,
               ref existingNodes, ref nodeidcounter, unit,
               ref existingProp2Ds, ref prop2d_guid,
               ref existingMaterials, ref materials_guid);
          }
        }
      }
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
    public static void ConvertMember3D(GsaMember3d member3d,
        ref Dictionary<int, Member> existingMembers, ref int memberidcounter,
        ref Dictionary<int, Node> existingNodes, ref int nodeidcounter, LengthUnit unit)
    {
      Member apiMember = member3d.GetAPI_MemberClone();

      // update topology list to fit model nodes
      string topo = "";

      // Loop through the face list
      for (int j = 0; j < member3d.SolidMesh.Faces.Count; j++)
      {
        for (int k = 0; k < 3; k++)
        {
          int faceint = 0;
          if (k == 0)
            faceint = member3d.SolidMesh.Faces[j].A;
          if (k == 1)
            faceint = member3d.SolidMesh.Faces[j].B;
          if (k == 2)
            faceint = member3d.SolidMesh.Faces[j].C;

          // vertex point of current face corner
          Point3d pt = member3d.SolidMesh.Vertices[faceint];

          // add space if we are not in first iteration
          if (k > 0)
            topo += " ";

          // check point against existing nodes in model
          int id = Nodes.GetExistingNodeID(existingNodes, pt, unit);
          if (id > 0)
            topo += id;
          else
          {
            existingNodes.Add(nodeidcounter, Nodes.NodeFromPoint(pt, unit));
            topo += nodeidcounter;
            nodeidcounter++;
          }
        }
        // add ";" between face lists, unless we are in final iteration
        if (j != member3d.SolidMesh.Faces.Count - 1)
          topo += "; ";
      }
      // set topology in api member
      apiMember.Topology = string.Copy(topo);

      // Section
      if (apiMember.Property == 0)
        // to be done

        // set apimember in dictionary
        if (member3d.ID > 0) // if the ID is larger than 0 than means the ID has been set and we sent it to the known list
        {
          existingMembers[member3d.ID] = apiMember;
        }
        else
        {
          existingMembers.Add(memberidcounter, apiMember);
          memberidcounter++;
        }
    }
    /// <summary>
    /// Method to create a list of API Members by using existing points when within tolerance.
    /// </summary>
    /// <param name="member3ds"></param>
    /// <param name="existingMembers"></param>
    /// <param name="memberidcounter"></param>
    /// <param name="existingNodes"></param>
    /// <param name="unit"></param>
    public static void ConvertMember3D(List<GsaMember3d> member3ds,
    ref Dictionary<int, Member> existingMembers, ref int memberidcounter,
    ref Dictionary<int, Node> existingNodes, LengthUnit unit)
    {
      // create a counter for creating new elements, nodes and properties
      int nodeidcounter = (existingNodes.Count > 0) ? existingNodes.Keys.Max() + 1 : 1;

      // Mem3ds
      if (member3ds != null)
      {
        for (int i = 0; i < member3ds.Count; i++)
        {
          if (member3ds[i] != null)
          {
            GsaMember3d member3d = member3ds[i];

            ConvertMember3D(member3d, ref existingMembers, ref memberidcounter, ref existingNodes, ref nodeidcounter, unit);
          }
        }
      }
    }
    #endregion
  }
}
