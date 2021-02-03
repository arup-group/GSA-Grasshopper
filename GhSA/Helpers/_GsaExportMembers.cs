using GsaAPI;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using GhSA.Parameters;
using System.Linq;
using System.Collections.ObjectModel;

namespace GhSA.Util.Gsa.ToGSA
{
    class Members
    {
        #region member1d
        public static Member ConvertMember1D(GsaMember1d member1d, ref List<Node> nodes, ref int nodeidcounter)
        {
            // ensure node id is at least 1
            if (nodeidcounter < 1)
                nodeidcounter = 1;

            // take out api member
            Member apimember = member1d.Member;

            // create topology string to build
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

                topo += nodeidcounter++;

                if (j != member1d.Topology.Count - 1)
                    topo += " ";
            }
            // set topology in api member
            apimember.Topology = string.Copy(topo);

            return apimember;
        }

        public static void ConvertMember1D(GsaMember1d member1d,
            ref Dictionary<int, Member> existingMembers, ref int memberidcounter,
            ref Dictionary<int, Node> existingNodes, ref int nodeidcounter,
            ref Dictionary<int, Section> existingSections)
        {
            Member apiMember = member1d.Member;

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

                int id = Nodes.GetExistingNodeID(existingNodes, pt);
                if (id > 0)
                    topo += id;
                else
                {
                    existingNodes.Add(nodeidcounter, Nodes.NodeFromPoint(pt));
                    topo += nodeidcounter;
                    nodeidcounter++;
                }

                if (j != member1d.Topology.Count - 1)
                    topo += " ";
            }
            // set topology in api member
            apiMember.Topology = string.Copy(topo);

            // Section
            if (apiMember.Property == 0)
                apiMember.Property = Sections.ConvertSection(member1d.Section, ref existingSections);

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

        public static List<Member> ConvertMember1D(List<GsaMember1d> member1ds, ref List<Node> nodes, ref int nodeidcounter)
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

                            Member apiMember = Members.ConvertMember1D(member1d, ref nodes, ref nodeidcounter);

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
            ref Dictionary<int, Node> existingNodes,
            ref Dictionary<int, Section> existingSections,
            GrasshopperAsyncComponent.WorkerInstance workerInstance = null,
            Action<string, double> ReportProgress = null)
        {
            int nodeidcounter = (existingNodes.Count > 0) ? existingNodes.Keys.Max() + 1 : 1;

            // Mem1ds
            if (member1ds != null)
            {
                for (int i = 0; i < member1ds.Count; i++)
                {
                    if (workerInstance != null)
                    {
                        if (workerInstance.CancellationToken.IsCancellationRequested) return;
                        ReportProgress("Mem1D ", (double)i / (member1ds.Count - 1));
                    }


                    if (member1ds[i] != null)
                    {
                        GsaMember1d member1d = member1ds[i];

                        ConvertMember1D(member1d, ref existingMembers, ref memberidcounter, ref existingNodes, ref nodeidcounter, ref existingSections);
                    }
                }
            }
            if (workerInstance != null)
            {
                ReportProgress("Mem1D assembled", -2);
            }
        }
        
        #endregion

        #region member2d
        public static Member ConvertMember2D(GsaMember2d member2d, ref List<Node> nodes, ref int nodeidcounter)
        {
            // take out api member
            Member apimember = member2d.Member;

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
                Node node = new Node();
                node.Position.X = pt.X;
                node.Position.Y = pt.Y;
                node.Position.Z = pt.Z;
                nodes.Add(node);

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
                        Node node = new Node();
                        node.Position.X = pt.X;
                        node.Position.Y = pt.Y;
                        node.Position.Z = pt.Z;
                        nodes.Add(node);

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
                        Node node = new Node();
                        node.Position.X = pt.X;
                        node.Position.Y = pt.Y;
                        node.Position.Z = pt.Z;
                        nodes.Add(node);

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
                    Node node = new Node();
                    node.Position.X = pt.X;
                    node.Position.Y = pt.Y;
                    node.Position.Z = pt.Z;
                    nodes.Add(node);

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
            ref Dictionary<int, Node> existingNodes, ref int nodeidcounter,
            ref Dictionary<int, Prop2D> existingProp2Ds, ref int prop2didcounter)
        {
            Member apiMember = member2d.Member;

            // update topology list to fit model nodes
            string topo = "";

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

                int id = Nodes.GetExistingNodeID(existingNodes, pt);
                if (id > 0)
                    topo += id;
                else
                {
                    existingNodes.Add(nodeidcounter, Nodes.NodeFromPoint(pt));
                    topo += nodeidcounter;
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

                        int id = Nodes.GetExistingNodeID(existingNodes, pt);
                        if (id > 0)
                            topo += id;
                        else
                        {
                            existingNodes.Add(nodeidcounter, Nodes.NodeFromPoint(pt));
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

                        int id = Nodes.GetExistingNodeID(existingNodes, pt);
                        if (id > 0)
                            topo += id;
                        else
                        {
                            existingNodes.Add(nodeidcounter, Nodes.NodeFromPoint(pt));
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

                    int id = Nodes.GetExistingNodeID(existingNodes, pt);
                    if (id > 0)
                        topo += id;
                    else
                    {
                        existingNodes.Add(nodeidcounter, Nodes.NodeFromPoint(pt));
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

            // section
            if (apiMember.Property == 0)
                apiMember.Property = Prop2ds.ConvertProp2d(member2d.Property, ref existingProp2Ds, ref prop2didcounter);

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

            public static List<Member> ConvertMember2D(List<GsaMember2d> member2ds, ref List<Node> nodes, ref int nodeidcounter)
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

                            Member apiMember = Members.ConvertMember2D(member2d, ref nodes, ref nodeidcounter);

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
            ref Dictionary<int, Node> existingNodes,
            ref Dictionary<int, Prop2D> existingProp2Ds,
            GrasshopperAsyncComponent.WorkerInstance workerInstance = null,
            Action<string, double> ReportProgress = null)
        {
            // create a counter for creating new elements, nodes and properties
            int nodeidcounter = (existingNodes.Count > 0) ? existingNodes.Keys.Max() + 1 : 1;
            int prop2didcounter = (existingProp2Ds.Count > 0) ? existingProp2Ds.Keys.Max() + 1 : 1; //checking the existing model Mem2ds

            if (member2ds != null)
            {

                for (int i = 0; i < member2ds.Count; i++)
                {
                    if (workerInstance != null)
                    {
                        if (workerInstance.CancellationToken.IsCancellationRequested) return;
                        ReportProgress("Mem2D ", (double)i / (member2ds.Count - 1));
                    }


                    if (member2ds[i] != null)
                    {
                        GsaMember2d member2d = member2ds[i];

                        ConvertMember2D(member2d,
                           ref existingMembers, ref memberidcounter,
                           ref existingNodes, ref nodeidcounter,
                           ref existingProp2Ds, ref prop2didcounter);
                    }
                }
            }
            if (workerInstance != null)
            {
                ReportProgress("Mem2D assembled", -2);
            }
        }
            #endregion

            #region member3d

        public static Member ConvertMember3D(GsaMember3d member3d, ref List<Node> nodes, ref int nodeidcounter)
        {
            // ensure node id is at least 1
            if (nodeidcounter < 1)
                nodeidcounter = 1;

            // take out api member
            Member apimember = member3d.Member;

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
                    Node node = new Node();
                    node.Position.X = pt.X;
                    node.Position.Y = pt.Y;
                    node.Position.Z = pt.Z;
                    nodes.Add(node);

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
            ref Dictionary<int, Node> existingNodes, ref int nodeidcounter)
        {
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

                    // add space if we are not in first iteration
                    if (k > 0)
                        topo += " ";

                    // check point against existing nodes in model
                    int id = Nodes.GetExistingNodeID(existingNodes, pt);
                    if (id > 0)
                        topo += id;
                    else
                    {
                        existingNodes.Add(nodeidcounter, Nodes.NodeFromPoint(pt));
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

        public static List<Member> ConvertMember3D(List<GsaMember3d> member3ds, ref List<Node> nodes, ref int nodeidcounter)
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

                            Member apiMember = Members.ConvertMember3D(member3d, ref nodes, ref nodeidcounter);

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
        ref Dictionary<int, Node> existingNodes,
        GrasshopperAsyncComponent.WorkerInstance workerInstance = null,
        Action<string, double> ReportProgress = null)
        {
            // create a counter for creating new elements, nodes and properties
            int nodeidcounter = (existingNodes.Count > 0) ? existingNodes.Keys.Max() + 1 : 1;

            // Mem3ds
            if (member3ds != null)
            {
                for (int i = 0; i < member3ds.Count; i++)
                {
                    if (workerInstance != null)
                    {
                        if (workerInstance.CancellationToken.IsCancellationRequested) return;
                        ReportProgress("Mem3D ", (double)i / (member3ds.Count - 1));
                    }


                    if (member3ds[i] != null)
                    {
                        GsaMember3d member3d = member3ds[i];

                        ConvertMember3D(member3d, ref existingMembers, ref memberidcounter, ref existingNodes, ref nodeidcounter);
                    }
                }
            }
            if (workerInstance != null)
            {
                ReportProgress("Mem3D assembled", -2);
            }
        }
            #endregion
    }
}
