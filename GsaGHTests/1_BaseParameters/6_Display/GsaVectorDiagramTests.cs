using System.Drawing;

using Grasshopper.Kernel.Types;

using GsaGH.Helpers;
using GsaGH.Helpers.Graphics;
using GsaGH.Parameters;

using OasysUnits;

using Rhino.Collections;
using Rhino.Geometry;
using Rhino.Geometry.Morphs;

using Xunit;

using ForceUnit = OasysUnits.Units.ForceUnit;
using Line = Rhino.Geometry.Line;

namespace GsaGHTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  public class GsaVectorDiagramTests {

    [Fact]
    public void WhenCreateInstance_ThenObject_ShouldNotBeNull() {
      var obj = new GsaVectorDiagram(Point3d.Unset, Vector3d.Unset, false, Color.Empty);
      Assert.NotNull(obj);
    }

    [Fact]
    public void WhenCreateInstanceWithDefaultPointAndVector_ThenVariables_ShouldBeValid() {
      var expectedPoint = new Point3d();
      var expectedVector = new Vector3d();
      string expectedForceValue = new Force(0, ForceUnit.Kilonewton).ToString();
      var expectedReactionForceLine = new Line(expectedPoint * -1, expectedVector);
      var expectedValue = new GH_Vector(new Vector3d(
        expectedReactionForceLine.ToX - expectedReactionForceLine.FromX,
        expectedReactionForceLine.ToY - expectedReactionForceLine.FromY,
        expectedReactionForceLine.ToZ - expectedReactionForceLine.FromZ));

      var obj = new GsaVectorDiagram(expectedPoint, expectedVector, false, Color.Empty);

      Assert.Equal(expectedPoint, obj.AnchorPoint);
      Assert.Equal(expectedVector, obj.Direction);
      Assert.Equal(expectedValue.Value, obj.Value);
      Assert.Equal(Colours.GsaDarkPurple, obj.Color);
    }

    [Fact]
    public void WhenCreateInstanceWithNullAndUnsetArgs_ThenVariables_ShouldBeValid() {
      Point3d expectedPoint = Point3d.Unset;
      Vector3d expectedVector = Vector3d.Zero;
      var expectedReactionForceLine = new Line(expectedPoint * -1, expectedVector);
      var expectedValue = new GH_Vector(new Vector3d(
        expectedReactionForceLine.ToX - expectedReactionForceLine.FromX,
        expectedReactionForceLine.ToY - expectedReactionForceLine.FromY,
        expectedReactionForceLine.ToZ - expectedReactionForceLine.FromZ));

      var obj = new GsaVectorDiagram(expectedPoint, expectedVector, false, Color.Empty);

      Assert.Equal(expectedPoint, obj.AnchorPoint);
      Assert.Equal(expectedVector, obj.Direction);
      Assert.Equal(expectedValue.Value, obj.Value);
      Assert.Equal(Colours.GsaDarkPurple, obj.Color);
    }

    [Fact]
    public void WhenCreateInstanceWithValidArguments_ThenVariables_ShouldBeValid() {
      var expectedPoint = new Point3d(1, 11, 111);
      var expectedVector = new Vector3d(2, 22, 222);
      string expectedForceValue = new Force(33, ForceUnit.Kilonewton).ToString();
      var expectedReactionForceLine = new Line(expectedPoint * -1, expectedVector);
      var expectedValue = new GH_Vector(new Vector3d(
        expectedReactionForceLine.ToX - expectedReactionForceLine.FromX,
        expectedReactionForceLine.ToY - expectedReactionForceLine.FromY,
        expectedReactionForceLine.ToZ - expectedReactionForceLine.FromZ));
      var obj = new GsaVectorDiagram(expectedPoint, expectedVector, true, Color.Empty);

      Assert.Equal(expectedPoint, obj.AnchorPoint);
      Assert.Equal(expectedVector, obj.Direction);
      Assert.Equal(expectedValue.Value, obj.Value);
      Assert.Equal(Colours.GsaGold, obj.Color);
    }

    [Fact]
    public void WhenDuplicateGeometry_ThenMethod_ShouldReturnDuplicatedObject() {
      var obj = new GsaVectorDiagram(Point3d.Origin, Vector3d.Zero, false, Color.Empty);

      var expectedObj = new GsaVectorDiagram(Point3d.Origin, Vector3d.Zero, false, Color.Empty);

      IGH_GeometricGoo actualObj = obj.DuplicateGeometry();

      Assert.Equal(expectedObj.ReferenceID, actualObj.ReferenceID);
      Assert.Equal(expectedObj.Boundingbox, actualObj.Boundingbox);
      Assert.Equal(expectedObj.TypeName, actualObj.TypeName);
      Assert.Equal(expectedObj.TypeDescription, actualObj.TypeDescription);
      Assert.Equal(expectedObj.ToString(), actualObj.ToString());
      Assert.Equal(Colours.GsaDarkPurple, obj.Color);
    }

    [Fact]
    public void WhenGeClippingBox_Then_ShouldReturnBoundingBox() {
      var startingPoint = new Point3d(3, 3, 3);
      var vector3d = new Vector3d(2, 2, 2);
      string force = new Force(4, ForceUnit.Kilonewton).ToString();
      var obj = new GsaVectorDiagram(startingPoint, vector3d, false, Color.Empty);

      BoundingBox expectedBoundingBox = obj.Boundingbox;
      BoundingBox actualResult = obj.ClippingBox;

      Assert.Equal(expectedBoundingBox, actualResult);
    }

    [Fact]
    public void WhenGetBoundingBox_ThenMethod_ShouldReturnValidValue() {
      var obj = new GsaVectorDiagram(
        new Point3d(3, 3, 3), new Vector3d(3, 3, 3), false, Color.Empty);

      BoundingBox actualBoundingBox = obj.Boundingbox;
      var expectedBoundingBox = new BoundingBox(new Point3dList() {
        Point3d.Origin,
        new Point3d(3, 3, 3),
      });

      Assert.Equal(expectedBoundingBox.Area, actualBoundingBox.Area);
      Assert.Equal(expectedBoundingBox.Center, actualBoundingBox.Center);
      Assert.Equal(expectedBoundingBox.Diagonal, actualBoundingBox.Diagonal);
      Assert.Equal(expectedBoundingBox.IsValid, actualBoundingBox.IsValid);
      Assert.Equal(expectedBoundingBox.Max, actualBoundingBox.Max);
      Assert.Equal(expectedBoundingBox.Min, actualBoundingBox.Min);
      Assert.Equal(expectedBoundingBox.Volume, actualBoundingBox.Volume);
    }

    [Fact]
    public void WhenGetBoundingBoxWithTransform_ThenMethod_ShouldReturnValidValue() {
      var startingPoint = new Point3d(3, 3, 3);
      var vector3d = new Vector3d(2, 2, 2);
      var obj = new GsaVectorDiagram(startingPoint, vector3d, false, Color.Empty);

      BoundingBox actualBoundingBox = obj.Boundingbox;
      var expectedBoundingBox = new BoundingBox(new Point3dList() {
        new Point3d(1, 1, 1),
        new Point3d(3, 3, 3),
      });

      Assert.Equal(expectedBoundingBox.Area, actualBoundingBox.Area, 12);
      Assert.Equal(expectedBoundingBox.Center, actualBoundingBox.Center);
      Assert.Equal(expectedBoundingBox.Diagonal, actualBoundingBox.Diagonal);
      Assert.Equal(expectedBoundingBox.IsValid, actualBoundingBox.IsValid);
      Assert.Equal(expectedBoundingBox.Max, actualBoundingBox.Max);
      Assert.Equal(expectedBoundingBox.Min, actualBoundingBox.Min);
      Assert.Equal(expectedBoundingBox.Volume, actualBoundingBox.Volume, 12);
    }

    [Fact]
    public void WhenGetTypeDescription_ThenShouldReturnValidString() {
      var obj = new GsaVectorDiagram(Point3d.Origin, Vector3d.Zero, false, Color.Empty);

      string expectedString = "A GSA vector diagram.";

      Assert.Equal(expectedString, obj.TypeDescription);
    }

    [Fact]
    public void WhenGetTypeName_ThenShouldReturnValidString() {
      var obj = new GsaVectorDiagram(Point3d.Origin, Vector3d.Zero, false, Color.Empty);

      string expectedString = "Vector Diagram";

      Assert.Equal(expectedString, obj.TypeName);
    }

    [Fact]
    public void WhenSetColor_ThenColorOfDiagramShoouldBeChanged() {
      var obj = new GsaVectorDiagram(Point3d.Unset, Vector3d.Unset, false, Color.Aqua);
      Assert.Equal(Color.Aqua, obj.Color);
    }

    [Fact]
    public void WhenMorph_ThenMethod_ShouldNothingChange() {
      var startingPoint = new Point3d(3, 3, 3);
      var vector3d = new Vector3d(2, 2, 2);
      string force = new Force(4, ForceUnit.Kilonewton).ToString();
      var obj = new GsaVectorDiagram(startingPoint, vector3d, false, Color.Empty);

      IGH_GeometricGoo actualObject = obj.Morph(new TwistSpaceMorph());
      var expectedObject = new GsaVectorDiagram(startingPoint, vector3d, false, Color.Empty);

      Assert.Equal(expectedObject.Boundingbox.Diagonal, actualObject.Boundingbox.Diagonal);
      Assert.Equal(expectedObject.Boundingbox.Max, actualObject.Boundingbox.Max);
      Assert.Equal(expectedObject.Boundingbox.Min, actualObject.Boundingbox.Min);
    }

    [Fact]
    public void WhenScriptVariable_ThenMethod_ShouldReturnGhVector() {
      var startingPoint = new Point3d(3, 3, 3);
      var vector3d = new Vector3d(2, 2, 2);
      string force = new Force(4, ForceUnit.Kilonewton).ToString();
      var obj = new GsaVectorDiagram(startingPoint, vector3d, false, Color.Empty);

      object actualObject = obj.ScriptVariable();

      Assert.IsType<Vector3d>(actualObject);
    }

    [Fact]
    public void WhenToString_ThenMethod_ShouldReturnValidString() {
      var obj = new GsaVectorDiagram(Point3d.Origin, Vector3d.Zero, false, Color.Empty);

      var pt = new GH_Point(Point3d.Origin);
      var vec = new GH_Vector(Vector3d.Zero);
      string expectedString
        = $"Diagram Vector: Anchor {pt}, Direction {vec}";

      Assert.Equal(expectedString, obj.ToString());
    }

    [Fact]
    public void WhenTransform_ThenMethod_ShouldReturnValidObject() {
      var startingPoint = new Point3d(3, 3, 3);
      var vector3d = new Vector3d(2, 2, 2);
      var obj = new GsaVectorDiagram(startingPoint, vector3d, false, Color.Empty);

      IGH_GeometricGoo actualObject
        = obj.Transform(new Transform(Transform.Mirror(startingPoint, vector3d)));
      var expectedObject = new Vector3d(2, 2, 2);

      Assert.Equal(expectedObject.X, actualObject.Boundingbox.Diagonal.X, DoubleComparer.Default);
      Assert.Equal(expectedObject.Y, actualObject.Boundingbox.Diagonal.Y, DoubleComparer.Default);
      Assert.Equal(expectedObject.Z, actualObject.Boundingbox.Diagonal.Z, DoubleComparer.Default);
    }

    [Fact]
    public void GetBoundingBoxTest() {
      var startingPoint = new Point3d(3, 3, 3);
      var vector3d = new Vector3d(2, 2, 2);
      var obj = new GsaVectorDiagram(startingPoint, vector3d, false, Color.Empty);
      var transform = Transform.Translation(new Vector3d(1, 1, 1));
      BoundingBox boundingBox = obj.GetBoundingBox(transform);
      Assert.Equal(2, boundingBox.Corner(true, true, true).X, 2);
      Assert.Equal(2, boundingBox.Corner(true, true, true).Y, 2);
      Assert.Equal(2, boundingBox.Corner(true, true, true).Z, 2);
      Assert.Equal(4, boundingBox.Corner(false, false, false).X, 2);
      Assert.Equal(4, boundingBox.Corner(false, false, false).Y, 2);
      Assert.Equal(4, boundingBox.Corner(false, false, false).Z, 2);
    }
  }
}
