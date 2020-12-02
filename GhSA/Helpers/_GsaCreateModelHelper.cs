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
    class ModelAxis
    {
        /// <summary>
        /// This method checks if the testAxis is within tolerance of an existing axis and returns the 
        /// axis ID if found. Will return 0 if no existing axis is found within the tolerance.
        /// </summary>
        /// <param name="existNodes"></param>
        /// <param name="testNode"></param>
        /// <param name="tolerance"></param>
        /// <param name="nodesToBeSet"></param>
        /// <returns></returns>
        public static int ExistingAxis(IReadOnlyDictionary<int, Axis> existAxes, Axis testAxis)
        {
            Axis gsaAxis;
            double tolerance = Util.GsaUnit.Tolerance;
            foreach (int key in existAxes.Keys)
            {
                if (existAxes.TryGetValue(key, out gsaAxis))
                {
                    if (Math.Pow((testAxis.Origin.X - testAxis.Origin.X), 2)
                        + Math.Pow((testAxis.Origin.Y - testAxis.Origin.Y), 2)
                        + Math.Pow((testAxis.Origin.Z - testAxis.Origin.Z), 2)
                        < Math.Pow(tolerance, 2))
                    {
                        if (Math.Pow((testAxis.XVector.X - testAxis.XVector.X), 2)
                        + Math.Pow((testAxis.XVector.Y - testAxis.XVector.Y), 2)
                        + Math.Pow((testAxis.XVector.Z - testAxis.XVector.Z), 2)
                        < Math.Pow(tolerance, 2))
                        {
                            if (Math.Pow((testAxis.XYPlane.X - testAxis.XYPlane.X), 2)
                            + Math.Pow((testAxis.XYPlane.Y - testAxis.XYPlane.Y), 2)
                            + Math.Pow((testAxis.XYPlane.Z - testAxis.XYPlane.Z), 2)
                            < Math.Pow(tolerance, 2))
                            {
                                return key;
                            }
                        }
                    }
                }
            }
            return 0;
        }
    }
}
