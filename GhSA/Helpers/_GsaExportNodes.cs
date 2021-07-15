using GsaAPI;
using Rhino.Geometry;
using Rhino.Collections;
using System;
using System.Collections.Generic;
using GhSA.Parameters;
using System.Linq;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace GhSA.Util.Gsa.ToGSA
{
    class Nodes
    {
        /// <summary>
        /// Method to convert a GhSA GsaNode to GsaAPI.Node
        /// The method will look for existing nodes and avoid 
        /// creating duplicates. the inputs for existing 
        /// nodes, existing axes and the node id counter will
        /// be updated, input these with 'ref' keyword
        /// </summary>
        /// <param name="node">GsaNode to convert</param>
        /// <param name="existingNodes">Dictionary of existing GsaAPI nodes to add nodes to</param>
        /// <param name="existingAxes">Dictionary of existing GsaAPI axes to add local axis to</param>
        /// <param name="nodeidcounter">node id counter for </param>
        public static void ConvertNode(GsaNode node, ref Dictionary<int, Node> existingNodes, 
            ref Dictionary<int, Axis> existingAxes, ref int nodeidcounter)
        {
            Node apiNode = node.API_Node;

            // Add spring to model
            if (node.Spring != null)
            {
                // Spring not implemented in GsaAPI
                //node.Node.SpringProperty = gsa.AddSpring(node.Spring); // assuming this will send back the spring ID in the model
            }

            // Add axis to model
            if (node.LocalAxis != null)
            {
                if (node.LocalAxis != Plane.WorldXY)
                {
                    Axis ax = new Axis();
                    Plane pln = node.LocalAxis;
                    ax.Origin.X = pln.OriginX;
                    ax.Origin.Y = pln.OriginY;
                    ax.Origin.Z = pln.OriginZ;

                    ax.XVector.X = pln.XAxis.X;
                    ax.XVector.Y = pln.XAxis.Y;
                    ax.XVector.Z = pln.XAxis.Z;
                    ax.XYPlane.X = pln.YAxis.X;
                    ax.XYPlane.Y = pln.YAxis.Y;
                    ax.XYPlane.Z = pln.YAxis.Z;

                    // set Axis property in node
                    apiNode.AxisProperty = 1;
                    if (existingAxes.Count > 0)
                        apiNode.AxisProperty = existingAxes.Keys.Max() + 1;
                    existingAxes.Add(apiNode.AxisProperty, ax);
                }
            }
            if (node.ID > 0) // if the ID is larger than 0 than means the ID has been set and we sent it to the known list
            {
                existingNodes[node.ID] = apiNode;
            }
            else
            {
                // get existing node id if any:
                int id = GetExistingNodeID(existingNodes, apiNode);
                if (id > 0) // if within tolerance of existing node
                {
                    // get GSA node
                    Node gsaNode = new Node();
                    existingNodes.TryGetValue(id, out gsaNode);

                    // combine restraints always picking true
                    if (gsaNode.Restraint.X)
                        apiNode.Restraint.X = true;
                    if (gsaNode.Restraint.Y)
                        apiNode.Restraint.Y = true;
                    if (gsaNode.Restraint.Z)
                        apiNode.Restraint.Z = true;
                    if (gsaNode.Restraint.XX)
                        apiNode.Restraint.XX = true;
                    if (gsaNode.Restraint.YY)
                        apiNode.Restraint.YY = true;
                    if (gsaNode.Restraint.ZZ)
                        apiNode.Restraint.ZZ = true;

                    // set local axis if it is set in GH node
                    if (node.LocalAxis != null)
                    {
                        if (gsaNode.SpringProperty > 0)
                            apiNode.SpringProperty = gsaNode.SpringProperty;
                    }
                    // replace existing node with new merged node
                    existingNodes[id] = apiNode;
                }
                else
                {
                    existingNodes.Add(nodeidcounter, apiNode);
                    nodeidcounter++;
                }
            }
        }
        
        public static void ConvertNode(List<GsaNode> nodes, ref Dictionary<int, Node> existingNodes,
            ref Dictionary<int, Axis> existingAxes,
            GrasshopperAsyncComponent.WorkerInstance workerInstance = null,
            Action<string, double> ReportProgress = null)
        {
            int nodeidcounter = (existingNodes.Count > 0) ? existingNodes.Keys.Max() + 1 : 1;

            // Add/Set Nodes
            if (nodes != null)
            {
                if (nodes.Count > 0)
                {
                    // update counter if new nodes have set ID higher than existing max
                    int existingNodeMaxID = nodes.Max(x => x.ID); // max ID in new nodes
                    if (existingNodeMaxID > nodeidcounter)
                        nodeidcounter = existingNodeMaxID + 1;

                    for (int i = 0; i < nodes.Count; i++)
                    {
                        // if method is called by a Async component check for cancellation and report progress
                        if (workerInstance != null)
                        {
                            if (workerInstance.CancellationToken.IsCancellationRequested) return;
                            ReportProgress("Nodes ", (double)i / (nodes.Count - 1));
                        }

                        if (nodes[i] != null)
                        {
                            GsaNode node = nodes[i];

                            // Add / Set node in dictionary
                            ConvertNode(node, ref existingNodes, ref existingAxes, ref nodeidcounter);
                        }
                    }
                }
            }
            if (workerInstance != null)
            {
                ReportProgress("Nodes assembled", -2);
            }
        }

        /// <summary>
        /// This method checks if the testNode is within tolerance of an existing node and returns the 
        /// node ID if found. Will return 0 if no existing node is found within the tolerance.
        /// </summary>
        /// <param name="existNodes">Dictionary of existing nodes to check against</param>
        /// <param name="testNode">Node to test for</param>
        /// <returns></returns>
        public static int GetExistingNodeID(IReadOnlyDictionary<int, Node> existNodes, Node testNode)
        {
            // ## the method below is slightly faster (46.5s vs 49.2s) but does not include tolerance
            //List<Node> pts = existNodes.Values.ToList();
            //List<int> keys = existNodes.Keys.ToList();
            //Point3dList existingPts = new Point3dList(pts.ConvertAll(n => new Point3d(n.Position.X, n.Position.Y, n.Position.Z)));

            //Point3d testPoint = new Point3d(testNode.Position.X, testNode.Position.Y, testNode.Position.Z);

            //if (existingPts.Contains(testPoint))
            //    return 0;
            //else
            //{
            //    return keys[existingPts.ClosestIndex(testPoint)];
            //}


            //double tolerance = Units.Tolerance;

            //foreach (int key in existNodes.Keys)
            //{
            //    if (existNodes.TryGetValue(key, out Node gsaNode))
            //    {
            //        if (Math.Pow((testNode.Position.X - gsaNode.Position.X), 2)
            //            + Math.Pow((testNode.Position.Y - gsaNode.Position.Y), 2)
            //            + Math.Pow((testNode.Position.Z - gsaNode.Position.Z), 2)
            //            < Math.Pow(tolerance, 2))
            //        {
            //            return key;
            //        }
            //    }
            //}
            //return 0;


            double tolerance = Units.Tolerance;
            int existingNodeID = 0;
            
            Parallel.ForEach(existNodes.Keys, (key, state) =>
            {
                if (existNodes.TryGetValue(key, out Node gsaNode))
                {
                    if (Math.Pow((testNode.Position.X - gsaNode.Position.X), 2)
                        + Math.Pow((testNode.Position.Y - gsaNode.Position.Y), 2)
                        + Math.Pow((testNode.Position.Z - gsaNode.Position.Z), 2)
                        < Math.Pow(tolerance, 2))
                    {
                        existingNodeID = key;
                        state.Break();
                    }
                }
            });
                
            return existingNodeID;
        }

        /// <summary>
        /// This method checks if the testNode is within tolerance of an existing node and returns the 
        /// node ID if found. Will return 0 if no existing node is found within the tolerance.
        /// </summary>
        /// <param name="existNodes">Dictionary of existing nodes to check against</param>
        /// <param name="testPoint">Point to test for</param>
        /// <returns></returns>
        public static int GetExistingNodeID(Dictionary<int, Node> existNodes, Point3d testPoint)
        {
            // ## the method below is slightly faster (46.5s vs 49.2s) but does not include tolerance
            //List<Node> pts = existNodes.Values.ToList();
            //List<int> keys = existNodes.Keys.ToList();
            //Point3dList existingPts = new Point3dList(pts.ConvertAll(n => new Point3d(n.Position.X, n.Position.Y, n.Position.Z)));

            //if (existingPts.Contains(testPoint))
            //    return 0;
            //else
            //{
            //    return keys[existingPts.ClosestIndex(testPoint)];
            //}
            
            double tolerance = Units.Tolerance;
            foreach (int key in existNodes.Keys)
            {
                if (existNodes.TryGetValue(key, out Node gsaNode))
                {
                    if (Math.Pow((testPoint.X - gsaNode.Position.X), 2)
                        + Math.Pow((testPoint.Y - gsaNode.Position.Y), 2)
                        + Math.Pow((testPoint.Z - gsaNode.Position.Z), 2)
                        < Math.Pow(tolerance, 2))
                    {
                        return key;
                    }
                }
            }
            return 0;
        }

        public static Node NodeFromPoint(Point3d point)
        {
            Node gsaNode = new Node();
            gsaNode.Position.X = point.X;
            gsaNode.Position.Y = point.Y;
            gsaNode.Position.Z = point.Z;
            return gsaNode;
        }
    }
    class Axes
    {
        /// <summary>
        /// This method checks if the testAxis is within tolerance of an existing axis and returns the 
        /// axis ID if found. Will return 0 if no existing axis is found within the tolerance.
        /// </summary>
        /// <param name="existAxes">Dictionary of axis to check against</param>
        /// <param name="testAxis">Axis to check for</param>
        /// <returns></returns>
        public static int GetExistingAxisID(IReadOnlyDictionary<int, Axis> existAxes, Axis testAxis)
        {
            double tolerance = Units.Tolerance;
            foreach (int key in existAxes.Keys)
            {
                if (existAxes.TryGetValue(key, out Axis gsaAxis))
                {
                    if (Math.Abs(testAxis.Origin.X - gsaAxis.Origin.X) <= tolerance &
                        Math.Abs(testAxis.Origin.Y - gsaAxis.Origin.Y) <= tolerance &
                        Math.Abs(testAxis.Origin.Z - gsaAxis.Origin.Z) <= tolerance &
                        Math.Abs(testAxis.XVector.X - gsaAxis.XVector.X) <= tolerance &
                        Math.Abs(testAxis.XVector.Y - gsaAxis.XVector.Y) <= tolerance &
                        Math.Abs(testAxis.XVector.Z - gsaAxis.XVector.Z) <= tolerance &
                        Math.Abs(testAxis.XYPlane.X - gsaAxis.XYPlane.X) <= tolerance &
                        Math.Abs(testAxis.XYPlane.Y - gsaAxis.XYPlane.Y) <= tolerance &
                        Math.Abs(testAxis.XYPlane.Z - gsaAxis.XYPlane.Z) <= tolerance
                        )
                        return key;
                }
            }
            return 0;
        }
    }
}
