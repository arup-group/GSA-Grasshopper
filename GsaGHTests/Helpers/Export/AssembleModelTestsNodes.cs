using GsaGH.Components;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using Grasshopper.Kernel.Types;
using Xunit;
using OasysGH.Components;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Rhino.Geometry;
using GsaGHTests.Model;
using GsaGHTests.Components.Properties;
using GsaAPI;
using OasysUnits.Units;
using OasysUnits;
using System.Drawing;

namespace GsaGHTests.Helpers.Export
{
  public partial class AssembleModelTests
  {
    [Fact]
    public void AssembleModelWithNodeTest()
    {
      GsaNodeGoo node = new GsaNodeGoo(new GsaNode());
      node.Value.Colour = Color.Red;
      node.Value.Name = "name Name Name";
      node.Value.Restraint = new GsaBool6(true, true, true, false, false, true);
      node.Value.Point = new Point3d(4, 66, -10.0802);
      node.Value.LocalAxis = new Plane(new Point3d(1, 2, 3), new Vector3d(4, 3, 1));
      node.Value.DamperProperty = 4;
      node.Value.MassProperty = 3;
      node.Value.SpringProperty = 2;
      node.Value.Id = 42;

      GsaModelGoo modelGoo = (GsaModelGoo)ComponentTestHelper.GetOutput(
        CreateModelTest.CreateModelFromGeometry(new List<GsaNodeGoo>() { node }, null, null, null, null, null, ModelUnit.inch));

      TestNode(node.Value, LengthUnit.Inch, 42, modelGoo.Value);
    }

    [Fact]
    public void AssembleModelWithNodesTest()
    {
      GsaNodeGoo node1 = new GsaNodeGoo(new GsaNode());
      node1.Value.Colour = Color.Red;
      node1.Value.Name = "name Name Name";
      node1.Value.Restraint = new GsaBool6(true, true, true, false, false, true);
      node1.Value.Point = new Point3d(1, 1, 1);
      node1.Value.Id = 2;

      GsaNodeGoo node2 = new GsaNodeGoo(new GsaNode());
      node2.Value.Restraint = new GsaBool6(true, true, true, false, false, true);
      node2.Value.Point = new Point3d(-0.5, 1, -1);

      GsaModelGoo modelGoo = (GsaModelGoo)ComponentTestHelper.GetOutput(
        CreateModelTest.CreateModelFromGeometry(new List<GsaNodeGoo>() { node1, node2 }, null, null, null, null, null, ModelUnit.m));

      TestNode(node1.Value, LengthUnit.Meter, 2, modelGoo.Value);
      TestNode(node2.Value, LengthUnit.Meter, 3, modelGoo.Value);
    }
  }
}
