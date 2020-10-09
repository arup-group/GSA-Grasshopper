using GsaAPI;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GhSA.Util.Gsa
{
    class ModelNodes
    {
        /// <summary>
        /// This method checks if the testNode is within tolerance of an existing node and returns the 
        /// node ID if found. Will return 0 if no existing node is found within the tolerance.
        /// </summary>
        /// <param name="existNodes"></param>
        /// <param name="testNode"></param>
        /// <param name="tolerance"></param>
        /// <param name="nodesToBeSet"></param>
        /// <returns></returns>
        public static int ExistingNode(IReadOnlyDictionary<int, Node> existNodes, Node testNode)
        {
            Node gsaNode;
            double tolerance = Util.GsaUnit.Tolerance;
            foreach (int key in existNodes.Keys)
            {
                if (existNodes.TryGetValue(key, out gsaNode))
                {
                    if (Math.Pow((testNode.Position.X - gsaNode.Position.X), 2)
                        + Math.Pow((testNode.Position.Y - gsaNode.Position.Y), 2)
                        + Math.Pow((testNode.Position.Z - gsaNode.Position.Z), 2)
                        < Math.Pow(tolerance, 2))
                    {
                        return key;
                    }
                }
            }
            return 0;
        }
        /// <summary>
        /// This method checks if the testNode is within tolerance of an existing node and returns the 
        /// node ID if found. Will return 0 if no existing node is found within the tolerance.
        /// </summary>
        /// <param name="existNodes"></param>
        /// <param name="testNode"></param>
        /// <param name="tolerance"></param>
        /// <param name="nodesToBeSet"></param>
        /// <returns></returns>
        public static int ExistingNodePoint(Dictionary<int, Node> existNodes, Point3d testPoint)
        {
            Node gsaNode;
            double tolerance = Util.GsaUnit.Tolerance;
            foreach (int key in existNodes.Keys)
            {
                if (existNodes.TryGetValue(key, out gsaNode))
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
}
