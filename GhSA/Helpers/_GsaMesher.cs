using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GsaAPI;
using Rhino.Geometry;
using Grasshopper;
using GhSA.Parameters;

namespace GhSA.Util
{
    public class GsaMesher
    {
        public static Model GsaReMesh(List<GsaMember3d> member3Ds = null, List<GsaMember2d> member2Ds = null, List<GsaMember1d> member1Ds = null)
        {
            // temporary model to set members in
            Model gsa = new Model();

            // list of topology nodes
            List<Node> nodes = new List<Node>();

            // counter for creating nodes
            int id = 1;

            // List to set members in
            List<Member> mems = new List<Member>();

            #region member1d
            // member1Ds
            if (member1Ds != null)
            {
                if (member1Ds.Count > 0)
                {
                    for (int i = 0; i < member1Ds.Count; i++)
                    {
                        if (member1Ds[i] != null)
                        {
                            GsaMember1d member1d = member1Ds[i];
                            Member apiMember = member1d.Member;

                            // update topology list to fit model nodes
                            string topo = "";

                            // Loop through the topology list
                            for (int j = 0; j < member1d.Topology.Count; j++)
                            {


                                string topologyType = member1d.TopologyType[j];
                                if (j > 0)
                                {
                                    if (topologyType == "" | topologyType == " ")
                                        topo += " ";
                                    else
                                        topo += topologyType.ToLower() + " "; // add topology type (nothing or "a") in front of node id
                                }

                                Point3d pt = member1d.Topology[j];
                                Node node = new Node();
                                node.Position.X = pt.X;
                                node.Position.Y = pt.Y;
                                node.Position.Z = pt.Z;
                                nodes.Add(node);

                                topo += id++;

                                if (j != member1d.Topology.Count - 1)
                                    topo += " ";
                            }
                            // set topology in api member
                            apiMember.Topology = string.Copy(topo);

                            mems.Add(apiMember);
                        }
                    }
                }
            }
            #endregion

            #region member2d
            // member2Ds
            if (member2Ds != null)
            {
                if (member2Ds.Count > 0)
                {
                    for (int i = 0; i < member2Ds.Count; i++)
                    {
                        if (member2Ds[i] != null)
                        {
                            GsaMember2d member2d = member2Ds[i];
                            Member apimember = member2d.Member;

                            #region topology
                            // update topology list to fit model nodes
                            string topo = "";

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
                                Node node = new Node();
                                node.Position.X = pt.X;
                                node.Position.Y = pt.Y;
                                node.Position.Z = pt.Z;
                                nodes.Add(node);

                                topo += id++;

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
                                    for (int k = 0; k < member2Ds[i].VoidTopology[j].Count; k++)
                                    {
                                        string voidtopologytype = member2d.VoidTopologyType[j][k];

                                        if (k == 0)
                                            topo += " V(";
                                        if (voidtopologytype == "" | voidtopologytype == " ")
                                            topo += " ";
                                        else
                                            topo += voidtopologytype.ToLower() + " "; // add topology type (nothing or "a") in front of node id

                                        Point3d pt = member2d.VoidTopology[j][k];
                                        Node node = new Node();
                                        node.Position.X = pt.X;
                                        node.Position.Y = pt.Y;
                                        node.Position.Z = pt.Z;
                                        nodes.Add(node);

                                        topo += id++;

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
                                        Node node = new Node();
                                        node.Position.X = pt.X;
                                        node.Position.Y = pt.Y;
                                        node.Position.Z = pt.Z;
                                        nodes.Add(node);

                                        topo += id++;

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
                                    Node node = new Node();
                                    node.Position.X = pt.X;
                                    node.Position.Y = pt.Y;
                                    node.Position.Z = pt.Z;
                                    nodes.Add(node);

                                    topo += id++;

                                    if (j != member2d.InclusionPoints.Count - 1)
                                        topo += " ";
                                    else
                                        topo += ")";
                                }
                            }
                            #endregion

                            // update topology for api member
                            apimember.Topology = string.Copy(topo);

                            mems.Add(apimember);
                        }
                    }
                }
            }
            #endregion

            #region member3d
            // member3Ds
            if (member3Ds != null)
            {
                if (member3Ds.Count > 0)
                {
                    for (int i = 0; i < member3Ds.Count; i++)
                    {
                        if (member3Ds[i] != null)
                        {
                            GsaMember3d member3d = member3Ds[i];
                            Member apiMember = member3d.Member;

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
                                    Node node = new Node();
                                    node.Position.X = pt.X;
                                    node.Position.Y = pt.Y;
                                    node.Position.Z = pt.Z;
                                    nodes.Add(node);

                                    // add space if we are not in first iteration
                                    if (k > 0)
                                        topo += " ";

                                    topo += id++;
                                }
                                // add ";" between face lists, unless we are in final iteration
                                if (j != member3d.SolidMesh.Faces.Count - 1)
                                    topo += "; ";
                            }
                            // set topology in api member
                            apiMember.Topology = string.Copy(topo);

                            mems.Add(apiMember);
                        }
                    }
                }
            }
            #endregion

            #region create model
            Dictionary<int, Node> nodeDic = nodes
                .Select((s, index) => new { s, index })
                .ToDictionary(x => x.index + 1, x => x.s);
            ReadOnlyDictionary<int, Node> setnodes = new ReadOnlyDictionary<int, Node>(nodeDic);
            gsa.SetNodes(setnodes);

            Dictionary<int, Member> memDic = mems
                .Select((s, index) => new { s, index })
                .ToDictionary(x => x.index + 1, x => x.s);
            ReadOnlyDictionary<int, Member> setmem = new ReadOnlyDictionary<int, Member>(memDic);
            gsa.SetMembers(setmem);
            #endregion

            return gsa;
        }
    }
}
