using System.Collections.Generic;
using System.Drawing;
using GsaGH.Parameters;
using GsaGHTests.Model;
using OasysUnits.Units;
using Rhino.Geometry;
using Xunit;

namespace GsaGHTests.Helpers.Export {
  public partial class AssembleModelTests {
    [Fact]
    public void AssembleModelWithNodesTest() {
      var node1 = new GsaNodeGoo(new GsaNode());
      node1.Value.Colour = Color.Red;
      node1.Value.Name = "name Name Name";
      node1.Value.Restraint = new GsaBool6(true,
        true,
        true,
        false,
        false,
        true);
      node1.Value.Point = new Point3d(1, 1, 1);
      node1.Value.Id = 2;

      var node2 = new GsaNodeGoo(new GsaNode());
      node2.Value.Restraint = new GsaBool6(true,
        true,
        true,
        false,
        false,
        true);
      node2.Value.Point = new Point3d(-0.5, 1, -1);

      var modelGoo = (GsaModelGoo)ComponentTestHelper.GetOutput(
        CreateModelTest.CreateModelFromGeometry(new List<GsaNodeGoo>() {
            node1,
            node2,
          },
          null,
          null,
          null,
          null,
          null,
          ModelUnit.M));

      TestNode(node1.Value, LengthUnit.Meter, 2, modelGoo.Value);
      TestNode(node2.Value, LengthUnit.Meter, 1, modelGoo.Value);
    }

    [Fact]
    public void AssembleModelWithNodeTest() {
      var node = new GsaNodeGoo(new GsaNode());
      node.Value.Colour = Color.Red;
      node.Value.Name = "name Name Name";
      node.Value.Restraint = new GsaBool6(true,
        true,
        true,
        false,
        false,
        true);
      node.Value.Point = new Point3d(4, 66, -10.0802);
      node.Value.LocalAxis = new Plane(new Point3d(1, 2, 3), new Vector3d(4, 3, 1));
      node.Value.DamperProperty = 4;
      node.Value.MassProperty = 3;
      node.Value.SpringProperty = 2;
      node.Value.Id = 42;

      var modelGoo = (GsaModelGoo)ComponentTestHelper.GetOutput(
        CreateModelTest.CreateModelFromGeometry(new List<GsaNodeGoo>() {
            node,
          },
          null,
          null,
          null,
          null,
          null,
          ModelUnit.Inch));

      TestNode(node.Value, LengthUnit.Inch, 42, modelGoo.Value);
    }
  }
}
