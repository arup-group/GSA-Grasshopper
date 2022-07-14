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
  class Members
  {
    #region member1d
    public static Member ConvertMember1D(GsaMember1d member1d, ref List<Node> nodes, ref int nodeidcounter, LengthUnit unit)
    {
      // ensure node id is at least 1
      if (nodeidcounter < 1)
        nodeidcounter = 1;

      // take out api member
      Member apimember = member1d.GetAPI_MemberClone();

      // create topology string to build
      string topo = "";

      // Loop through the topology list
      for (int i = 0; i < member1d.Topology.Count; i++)
      {
        string topologyType = member1d.TopologyType[i];
        if (i > 0)
        {
          if (topologyType == "" | topologyType == " ")
            topo += " ";
          else
            topo += topologyType.ToLower() + " "; // add topology type (nothing or "a") in front of node id
        }

        Point3d pt = member1d.Topology[i];
        nodes.Add(Nodes.NodeFromPoint(pt, unit));

        topo += nodeidcounter++;

        if (i != member1d.Topology.Count - 1)
          topo += " ";
      }
      // set topology in api member
      apimember.Topology = string.Copy(topo);

      return apimember;
    }

    public static void ConvertMember1D(GsaMember1d member1d,
        ref Dictionary<int, Member> existingMembers, ref int memberidcounter,
        ref Dictionary<int, Node> existingNodes, ref int nodeidcounter, LengthUnit unit,
        ref Dictionary<int, Section> existingSections, ref Dictionary<Guid, int> sections_guid,
        ref Dictionary<int, SectionModifier> existingSectionModifiers,
        ref Dictionary<int, AnalysisMaterial> existingMaterials, ref Dictionary<Guid, int> materials_guid)
    {
      Member apiMember = member1d.GetAPI_MemberClone();

      // update topology list to fit model nodes
      string topo = "";

      // Loop through the topology list
      for (int j = 0; j < member1d.Topology.Count; j++)
      {
        string topologyType = member1d.TopologyType[j];
        Point3d pt = member1d.Topology[j];
        if (j > 0)
        {
          if (topologyType == "" | topologyType == " ")
            topo += " ";
          else
            topo += topologyType.ToLower() + " "; // add topology type (nothing or "a") in front of node id
        }

        int id = Nodes.GetExistingNodeID(existingNodes, pt, unit);
        if (id > 0)
          topo += id;
        else
        {
          existingNodes.Add(nodeidcounter, Nodes.NodeFromPoint(pt, unit));
          topo += nodeidcounter;
          nodeidcounter++;
        }

        if (j != member1d.Topology.Count - 1)
          topo += " ";
      }
      // set topology in api member
      try
      {
        apiMember.Topology = string.Copy(topo);
      }
      catch (Exception e)
      {
        List<string> errors = member1d.Topology.Select(t => "{" + t.ToString() + "}").ToList();
        string error = string.Join(", ", errors);
        throw new Exception(" Invalid topology for Member1D: " + topo + " for original points: " + error);
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

    public static List<Member> ConvertMember1D(List<GsaMember1d> member1ds, ref List<Node> nodes, ref int nodeidcounter, LengthUnit unit)
    {
      // List to set members in
      List<Member> mems = new List<Member>();

      #region member1d
      // member1Ds
      if (member1ds != null)
      {
        if (member1ds.Count > 0)
        {
          for (int i = 0; i < member1ds.Count; i++)
          {
            if (member1ds[i] != null)
            {
              GsaMember1d member1d = member1ds[i];

              Member apiMember = ConvertMember1D(member1d, ref nodes, ref nodeidcounter, unit);

              mems.Add(apiMember);
            }
          }
        }
      }
      return mems;
      #endregion
    }

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
    public static Member ConvertMember2D(GsaMember2d member2d, ref List<Node> nodes, ref int nodeidcounter, LengthUnit unit)
    {
      // take out api member
      Member apimember = member2d.GetAPI_MemberClone();

      // create string to build topology
      string topo = "";

      #region outline topology
      // Loop through the topology list
      for (int j = 0; j < member2d.Topology.Count; j++)
      {
        string topologyType = member2d.TopologyType[j];

        if (j > 0)
        {
          if (topologyType == "" | topologyType == " ")
            topo += " ";
          else
            topo += topologyType.ToLower() + " "; // add topology type (nothing or "a") in front of node id
        }

        Point3d pt = member2d.Topology[j];
        nodes.Add(Nodes.NodeFromPoint(pt, unit));

        topo += nodeidcounter++;

        if (j != member2d.Topology.Count - 1)
          topo += " ";
      }
      #endregion

      #region voids
      // Loop through the voidtopology list
      if (member2d.VoidTopology != null)
      {
        for (int j = 0; j < member2d.VoidTopology.Count; j++)
        {
          for (int k = 0; k < member2d.VoidTopology[j].Count; k++)
          {
            string voidtopologytype = member2d.VoidTopologyType[j][k];

            if (k == 0)
              topo += " V(";
            if (voidtopologytype == "" | voidtopologytype == " ")
              topo += " ";
            else
              topo += voidtopologytype.ToLower() + " "; // add topology type (nothing or "a") in front of node id

            Point3d pt = member2d.VoidTopology[j][k];
            nodes.Add(Nodes.NodeFromPoint(pt, unit));

            topo += nodeidcounter++;

            if (k != member2d.VoidTopology[j].Count - 1)
              topo += " ";
            else
              topo += ")";
          }
        }
      }
      #endregion

      #region inclusion lines
      // Loop through the inclusion lines topology list  
      if (member2d.IncLinesTopology != null)
      {
        for (int j = 0; j < member2d.IncLinesTopology.Count; j++)
        {
          for (int k = 0; k < member2d.IncLinesTopology[j].Count; k++)
          {
            string inclineTopologytype = member2d.IncLinesTopologyType[j][k];

            if (k == 0)
              topo += " L(";
            if (inclineTopologytype == "" | inclineTopologytype == " ")
              topo += " ";
            else
              topo += inclineTopologytype.ToLower() + " "; // add topology type (nothing or "a") in front of node id

            Point3d pt = member2d.IncLinesTopology[j][k];
            nodes.Add(Nodes.NodeFromPoint(pt, unit));

            topo += nodeidcounter++;

            if (k != member2d.IncLinesTopology[j].Count - 1)
              topo += " ";
            else
              topo += ")";
          }
        }
      }
      #endregion

      #region inclusion points
      // Loop through the inclucion point topology list
      if (member2d.InclusionPoints != null)
      {
        for (int j = 0; j < member2d.InclusionPoints.Count; j++)
        {
          if (j == 0)
            topo += " P(";

          Point3d pt = member2d.InclusionPoints[j];
          nodes.Add(Nodes.NodeFromPoint(pt, unit));

          topo += nodeidcounter++;

          if (j != member2d.InclusionPoints.Count - 1)
            topo += " ";
          else
            topo += ")";
        }
      }
      #endregion

      // update topology for api member
      apimember.Topology = string.Copy(topo);

      return apimember;
    }

    public static void ConvertMember2D(GsaMember2d member2d,
        ref Dictionary<int, Member> existingMembers, ref int memberidcounter,
        ref Dictionary<int, Node> existingNodes, ref int nodeidcounter, LengthUnit unit,
        ref Dictionary<int, Prop2D> existingProp2Ds, ref Dictionary<Guid, int> prop2d_guid,
        ref Dictionary<int, AnalysisMaterial> existingMaterials, ref Dictionary<Guid, int> materials_guid)
    {
      Member apiMember = member2d.GetAPI_MemberClone();

      // update topology list to fit model nodes
      string topo = "";

      int lastId = int.MinValue;
      // Loop through the topology list
      for (int i = 0; i < member2d.Topology.Count; i++)
      {
        Point3d pt = member2d.Topology[i];
        string topologyType = member2d.TopologyType[i];

        if (i > 0)
        {
          if (topologyType == "" | topologyType == " ")
            topo += " ";
          else
            topo += topologyType.ToLower() + " "; // add topology type (nothing or "a") in front of node id
        }

        int id = Nodes.GetExistingNodeID(existingNodes, pt, unit);
        if (id > 0)
        {
          // check if node id has just been added
          if (id != lastId)
            topo += id;
          lastId = id;
        }
        else
        {
          existingNodes.Add(nodeidcounter, Nodes.NodeFromPoint(pt, unit));
          topo += nodeidcounter;
          lastId = nodeidcounter;
          nodeidcounter++;
        }

        if (i != member2d.Topology.Count - 1)
          topo += " ";
      }

      // Loop through the voidtopology list
      if (member2d.VoidTopology != null)
      {
        for (int i = 0; i < member2d.VoidTopology.Count; i++)
        {
          for (int j = 0; j < member2d.VoidTopology[i].Count; j++)
          {
            Point3d pt = member2d.VoidTopology[i][j];
            string voidtopologytype = member2d.VoidTopologyType[i][j];

            if (j == 0)
              topo += " V(";
            if (voidtopologytype == "" | voidtopologytype == " ")
              topo += " ";
            else
              topo += voidtopologytype.ToLower() + " "; // add topology type (nothing or "a") in front of node id

            int id = Nodes.GetExistingNodeID(existingNodes, pt, unit);
            if (id > 0)
              topo += id;
            else
            {
              existingNodes.Add(nodeidcounter, Nodes.NodeFromPoint(pt, unit));
              topo += nodeidcounter;
              nodeidcounter++;
            }

            if (j != member2d.VoidTopology[i].Count - 1)
              topo += " ";
            else
              topo += ")";
          }
        }
      }

      // Loop through the inclusion lines topology list  
      if (member2d.IncLinesTopology != null)
      {
        for (int i = 0; i < member2d.IncLinesTopology.Count; i++)
        {
          for (int j = 0; j < member2d.IncLinesTopology[i].Count; j++)
          {
            Point3d pt = member2d.IncLinesTopology[i][j];
            string inclineTopologytype = member2d.IncLinesTopologyType[i][j];

            if (j == 0)
              topo += " L(";
            if (inclineTopologytype == "" | inclineTopologytype == " ")
              topo += " ";
            else
              topo += inclineTopologytype.ToLower() + " "; // add topology type (nothing or "a") in front of node id

            int id = Nodes.GetExistingNodeID(existingNodes, pt, unit);
            if (id > 0)
              topo += id;
            else
            {
              existingNodes.Add(nodeidcounter, Nodes.NodeFromPoint(pt, unit));
              topo += nodeidcounter;
              nodeidcounter++;
            }

            if (j != member2d.IncLinesTopology[i].Count - 1)
              topo += " ";
            else
              topo += ")";
          }
        }
      }

      // Loop through the inclucion point topology list
      if (member2d.InclusionPoints != null)
      {
        for (int i = 0; i < member2d.InclusionPoints.Count; i++)
        {
          Point3d pt = member2d.InclusionPoints[i];
          if (i == 0)
            topo += " P(";

          int id = Nodes.GetExistingNodeID(existingNodes, pt, unit);
          if (id > 0)
            topo += id;
          else
          {
            existingNodes.Add(nodeidcounter, Nodes.NodeFromPoint(pt, unit));
            topo += nodeidcounter;
            nodeidcounter++;
          }

          if (i != member2d.InclusionPoints.Count - 1)
            topo += " ";
          else
            topo += ")";
        }
      }

      // update topology for api member
      apiMember.Topology = string.Copy(topo);

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

    public static List<Member> ConvertMember2D(List<GsaMember2d> member2ds, ref List<Node> nodes, ref int nodeidcounter, LengthUnit unit)
    {
      // ensure node id is at least 1
      if (nodeidcounter < 1)
        nodeidcounter = 1;

      // List to set members in
      List<Member> mems = new List<Member>();

      #region member2d
      // member2Ds
      if (member2ds != null)
      {
        if (member2ds.Count > 0)
        {
          for (int i = 0; i < member2ds.Count; i++)
          {
            if (member2ds[i] != null)
            {
              GsaMember2d member2d = member2ds[i];

              Member apiMember = Members.ConvertMember2D(member2d, ref nodes, ref nodeidcounter, unit);

              mems.Add(apiMember);
            }
          }
        }
      }
      #endregion
      return mems;
    }

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

    public static Member ConvertMember3D(GsaMember3d member3d, ref List<Node> nodes, ref int nodeidcounter, LengthUnit unit)
    {
      // ensure node id is at least 1
      if (nodeidcounter < 1)
        nodeidcounter = 1;

      // take out api member
      Member apimember = member3d.GetAPI_MemberClone();

      // create string to build topology list
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
          nodes.Add(Nodes.NodeFromPoint(pt, unit));

          // add space if we are not in first iteration
          if (k > 0)
            topo += " ";

          topo += nodeidcounter++;
        }
        // add ";" between face lists, unless we are in final iteration
        if (j != member3d.SolidMesh.Faces.Count - 1)
          topo += "; ";
      }
      // set topology in api member
      apimember.Topology = string.Copy(topo);

      return apimember;
    }

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

    public static List<Member> ConvertMember3D(List<GsaMember3d> member3ds, ref List<Node> nodes, ref int nodeidcounter, LengthUnit unit)
    {
      // List to set members in
      List<Member> mems = new List<Member>();

      #region member3d
      // member3Ds
      if (member3ds != null)
      {
        if (member3ds.Count > 0)
        {
          for (int i = 0; i < member3ds.Count; i++)
          {
            if (member3ds[i] != null)
            {
              GsaMember3d member3d = member3ds[i];

              Member apiMember = Members.ConvertMember3D(member3d, ref nodes, ref nodeidcounter, unit);

              mems.Add(apiMember);
            }
          }
        }
      }
      #endregion
      return mems;
    }

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
