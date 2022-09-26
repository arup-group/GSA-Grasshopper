using GsaAPI;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using GsaGH.Parameters;
using System.Linq;
using System.Threading.Tasks;
using OasysUnits.Units;
using OasysUnits;

namespace GsaGH.Util.Gsa.ToGSA
{
  class Nodes
  {
    /// <summary>
    /// Method to convert a GsaNode to GsaAPI.Node
    /// The method will look for existing nodes and avoid 
    /// creating duplicates. the inputs for existing 
    /// nodes, existing axes and the node id counter will
    /// be updated, input these with 'ref' keyword
    /// </summary>
    /// <param name="node">GsaNode to convert</param>
    /// <param name="existingNodes">Dictionary of existing GsaAPI nodes to add nodes to [in meters]</param>
    /// <param name="existingAxes">Dictionary of existing GsaAPI axes to add local axis to [in meters]</param>
    /// <param name="nodeidcounter">node id counter for </param>
    /// /// <param name="unit">UnitsNet LengthUnit of GsaNode node input</param>
    public static void ConvertNode(GsaNode node, ref Dictionary<int, Node> existingNodes,
        ref Dictionary<int, Axis> existingAxes, ref int nodeidcounter, LengthUnit unit)
    {
      Node apiNode = node.GetApiNodeToUnit(unit);

      // Add spring to model
      //if (node.Spring != null)
      //{
      // Spring not implemented in GsaAPI
      //node.Node.SpringProperty = gsa.AddSpring(node.Spring); // assuming this will send back the spring ID in the model
      //}

      // Add axis to model
      if (node.LocalAxis != null && node.LocalAxis.IsValid)
      {
        if (node.LocalAxis != Plane.WorldXY)
        {
          Axis ax = new Axis();
          Plane pln = node.LocalAxis;
          ax.Origin.X = (unit == LengthUnit.Meter) ? pln.OriginX : new Length(pln.OriginX, unit).Meters;
          ax.Origin.Y = (unit == LengthUnit.Meter) ? pln.OriginY : new Length(pln.OriginY, unit).Meters;
          ax.Origin.Z = (unit == LengthUnit.Meter) ? pln.OriginZ : new Length(pln.OriginZ, unit).Meters;

          ax.XVector.X = (unit == LengthUnit.Meter) ? pln.XAxis.X : new Length(pln.XAxis.X, unit).Meters;
          ax.XVector.Y = (unit == LengthUnit.Meter) ? pln.XAxis.Y : new Length(pln.XAxis.Y, unit).Meters;
          ax.XVector.Z = (unit == LengthUnit.Meter) ? pln.XAxis.Z : new Length(pln.XAxis.Z, unit).Meters;
          ax.XYPlane.X = (unit == LengthUnit.Meter) ? pln.YAxis.X : new Length(pln.YAxis.X, unit).Meters;
          ax.XYPlane.Y = (unit == LengthUnit.Meter) ? pln.YAxis.Y : new Length(pln.YAxis.Y, unit).Meters;
          ax.XYPlane.Z = (unit == LengthUnit.Meter) ? pln.YAxis.Z : new Length(pln.YAxis.Z, unit).Meters;

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
        ref Dictionary<int, Axis> existingAxes, LengthUnit lengthUnit)
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
            if (nodes[i] != null)
            {
              GsaNode node = nodes[i];

              // Add / Set node in dictionary
              ConvertNode(node, ref existingNodes, ref existingAxes, ref nodeidcounter, lengthUnit);
            }
          }
        }
      }
    }

    /// <summary>
    /// This method checks if the testNode is within tolerance of an existing node and returns the 
    /// node ID if found. Will return 0 if no existing node is found within the tolerance.
    /// </summary>
    /// <param name="existNodes">Dictionary of existing nodes to check against [in meters]</param>
    /// <param name="testNode">Node to test for [in meters]</param>
    /// <returns></returns>
    public static int GetExistingNodeID(IReadOnlyDictionary<int, Node> existNodes, Node testNode)
    {
      double tolerance = Units.Tolerance.Meters;
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
    /// <param name="existNodes">Dictionary of existing nodes to check against [in meters]</param>
    /// <param name="testPoint">Point to test for</param>
    /// /// <param name="unit">UnitsNet Length unit of testPoint</param>
    /// <returns></returns>
    public static int GetExistingNodeID(Dictionary<int, Node> existNodes, Point3d testPoint, LengthUnit unit)
    {
      Point3d pt = (unit == LengthUnit.Meter) ? testPoint :
          new Point3d(
              new Length(testPoint.X, unit).Meters,
              new Length(testPoint.Y, unit).Meters,
              new Length(testPoint.Z, unit).Meters
              );

      double tolerance = Units.Tolerance.Meters; // this method assumes everything is in meters
      foreach (int key in existNodes.Keys)
      {
        if (existNodes.TryGetValue(key, out Node gsaNode))
        {
          if (Math.Pow((pt.X - gsaNode.Position.X), 2)
              + Math.Pow((pt.Y - gsaNode.Position.Y), 2)
              + Math.Pow((pt.Z - gsaNode.Position.Z), 2)
              < Math.Pow(tolerance, 2))
          {
            return key;
          }
        }
      }
      return 0;
    }

    public static Node NodeFromPoint(Point3d point, LengthUnit unit)
    {
      Node gsaNode = new Node();

      gsaNode.Position.X = (unit == LengthUnit.Meter) ? point.X : new Length(point.X, unit).Meters;
      gsaNode.Position.Y = (unit == LengthUnit.Meter) ? point.Y : new Length(point.Y, unit).Meters;
      gsaNode.Position.Z = (unit == LengthUnit.Meter) ? point.Z : new Length(point.Z, unit).Meters;
      return gsaNode;
    }
  }
  class Axes
  {
    /// <summary>
    /// This method checks if the testAxis is within tolerance of an existing axis and returns the 
    /// axis ID if found. Will return 0 if no existing axis is found within the tolerance.
    /// </summary>
    /// <param name="existAxes">Dictionary of axis to check against [in meters]</param>
    /// <param name="testAxis">Axis to check for [in meters]</param>
    /// <returns></returns>
    public static int GetExistingAxisID(IReadOnlyDictionary<int, Axis> existAxes, Axis testAxis)
    {
      double tolerance = Units.Tolerance.Meters;
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
