using System.Collections.Generic;
using GsaAPI;
using GsaGH.Parameters;
using OasysUnits.Units;
using Rhino.Geometry;

namespace GsaGH.Util.Gsa.ToGSA
{
  partial class Members
  {
    #region member1d
    /// <summary>
    /// Method to create a API Member without bothering to find and using existing points.
    /// </summary>
    /// <param name="member1d"></param>
    /// <param name="nodes"></param>
    /// <param name="nodeidcounter"></param>
    /// <param name="unit"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Method to create a list of API members without bothering to find and using existing points.
    /// </summary>
    /// <param name="member1ds"></param>
    /// <param name="nodes"></param>
    /// <param name="nodeidcounter"></param>
    /// <param name="unit"></param>
    /// <returns></returns>
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

    #endregion

    #region member2d
    /// <summary>
    /// Method to create API member without bothering to find and using existing points.
    /// </summary>
    /// <param name="member2d"></param>
    /// <param name="nodes"></param>
    /// <param name="nodeidcounter"></param>
    /// <param name="unit">Model unit</param>
    /// <returns></returns>
    public static Member ConvertMember2D(GsaMember2d member2d, ref List<Node> nodes, ref int nodeidcounter, LengthUnit modelUnit)
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
        nodes.Add(Nodes.NodeFromPoint(pt, modelUnit));

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
            nodes.Add(Nodes.NodeFromPoint(pt, modelUnit));

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
            nodes.Add(Nodes.NodeFromPoint(pt, modelUnit));

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
          nodes.Add(Nodes.NodeFromPoint(pt, modelUnit));

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

    /// <summary>
    /// Method to create a list of API members without bothering to find and using existing points.
    /// </summary>
    /// <param name="member2ds"></param>
    /// <param name="nodes"></param>
    /// <param name="nodeidcounter"></param>
    /// <param name="modelUnit"></param>
    /// <returns></returns>
    public static List<Member> ConvertMember2D(List<GsaMember2d> member2ds, ref List<Node> nodes, ref int nodeidcounter, LengthUnit modelUnit)
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

              Member apiMember = Members.ConvertMember2D(member2d, ref nodes, ref nodeidcounter, modelUnit);

              mems.Add(apiMember);
            }
          }
        }
      }
      #endregion
      return mems;
    }

    #endregion

    #region member3d
    /// <summary>
    /// Method to create API member without bothering to find and using existing points.
    /// </summary>
    /// <param name="member3d"></param>
    /// <param name="nodes"></param>
    /// <param name="nodeidcounter"></param>
    /// <param name="unit"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Method to create a list of API members without bothering to find and using existing points.
    /// </summary>
    /// <param name="member3ds"></param>
    /// <param name="nodes"></param>
    /// <param name="nodeidcounter"></param>
    /// <param name="unit"></param>
    /// <returns></returns>
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
    #endregion
  }
}
