using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using Grasshopper.Kernel.Types;
using GsaGH.Helpers.Graphics;
using GsaGH.Parameters;
using OasysUnits;
using OasysUnits.Units;
using Rhino.Geometry;
using Rhino.Geometry.Morphs;
using Xunit;

namespace GsaGHTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  public class DiagramGooTests {

    [Fact]
    public void WhenCreateInstance_ThenObject_ShouldNotBeNull() {
      var obj = new DiagramGoo(Point3d.Unset, Vector3d.Unset, null, ArrowMode.NoArrow);
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

      var obj = new DiagramGoo(expectedPoint, expectedVector, expectedForceValue,
        ArrowMode.NoArrow);

      Assert.Equal(expectedPoint, obj.StartingPoint);
      Assert.Equal(expectedVector, obj.Direction);
      Assert.Equal(expectedForceValue, obj.ForceValue);
      Assert.Equal(expectedValue.Value, obj.Value.Value);
      Assert.Equal(ArrowMode.NoArrow, obj.ArrowMode);
      Assert.Equal(Colours.GsaDarkPurple, obj.Color);
    }

    [Fact]
    public void WhenCreateInstanceWithNullAndUnsetArgs_ThenVariables_ShouldBeValid() {
      Point3d expectedPoint = Point3d.Origin;
      Vector3d expectedVector = Vector3d.Zero;
      var expectedReactionForceLine = new Line(expectedPoint * -1, expectedVector);
      var expectedValue = new GH_Vector(new Vector3d(
        expectedReactionForceLine.ToX - expectedReactionForceLine.FromX,
        expectedReactionForceLine.ToY - expectedReactionForceLine.FromY,
        expectedReactionForceLine.ToZ - expectedReactionForceLine.FromZ));

      var obj = new DiagramGoo(Point3d.Unset, Vector3d.Unset, null, ArrowMode.OneArrow);

      Assert.Equal(expectedPoint, obj.StartingPoint);
      Assert.Equal(expectedVector, obj.Direction);
      Assert.Null(obj.ForceValue);
      Assert.Equal(expectedValue.Value, obj.Value.Value);
      Assert.Equal(ArrowMode.OneArrow, obj.ArrowMode);
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
      var obj = new DiagramGoo(expectedPoint, expectedVector, expectedForceValue,
        ArrowMode.DoubleArrow);

      Assert.Equal(expectedPoint, obj.StartingPoint);
      Assert.Equal(expectedVector, obj.Direction);
      Assert.Equal(expectedForceValue, obj.ForceValue);
      Assert.Equal(expectedValue.Value, obj.Value.Value);
      Assert.Equal(ArrowMode.DoubleArrow, obj.ArrowMode);
      Assert.Equal(Colours.GsaDarkPurple, obj.Color);
    }

    [Fact]
    public void WhenDuplicateGeometry_ThenMethod_ShouldReturnDuplicatedObject() {
      var obj = new DiagramGoo(Point3d.Origin, Vector3d.Zero,
        new Force(4, ForceUnit.Kilonewton).ToString(), ArrowMode.NoArrow);

      var expectedObj = new DiagramGoo(Point3d.Origin, Vector3d.Zero,
        new Force(4, ForceUnit.Kilonewton).ToString(), ArrowMode.NoArrow);

      IGH_GeometricGoo actualObj = obj.DuplicateGeometry();

      Assert.Equal(expectedObj.ReferenceID, actualObj.ReferenceID);
      Assert.Equal(expectedObj.Boundingbox, actualObj.Boundingbox);
      Assert.Equal(expectedObj.TypeName, actualObj.TypeName);
      Assert.Equal(expectedObj.TypeDescription, actualObj.TypeDescription);
      Assert.Equal(expectedObj.ToString(), actualObj.ToString());
      Assert.Equal(ArrowMode.NoArrow, obj.ArrowMode);
      Assert.Equal(Colours.GsaDarkPurple, obj.Color);
    }

    [Fact]
    public void WhenGeClippingBox_Then_ShouldReturnBoundingBox() {
      var startingPoint = new Point3d(3, 3, 3);
      var vector3d = new Vector3d(2, 2, 2);
      string force = new Force(4, ForceUnit.Kilonewton).ToString();
      var obj = new DiagramGoo(startingPoint, vector3d, force, ArrowMode.NoArrow);

      BoundingBox expectedBoundingBox = obj.Boundingbox;
      BoundingBox actualResult = obj.ClippingBox;

      Assert.Equal(expectedBoundingBox, actualResult);
    }

    [Fact]
    public void WhenGetBoundingBox_ThenMethod_ShouldReturnValidValue() {
      var obj = new DiagramGoo(new Point3d(3, 3, 3), new Vector3d(3, 3, 3),
        new Force(4, ForceUnit.Kilonewton).ToString(), ArrowMode.NoArrow);

      BoundingBox actualBoundingBox = obj.Boundingbox;
      var expectedBoundingBox = new BoundingBox(new List<Point3d>() {
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
      var obj = new DiagramGoo(startingPoint, vector3d,
        new Force(4, ForceUnit.Kilonewton).ToString(), ArrowMode.NoArrow);

      BoundingBox actualBoundingBox
        = obj.GetBoundingBox(new Transform(Transform.Mirror(startingPoint, vector3d)));
      var expectedBoundingBox = new BoundingBox(new List<Point3d>() {
        new Point3d(1, 1, 1),
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
    public void WhenGetTypeDescription_ThenShouldReturnValidString() {
      var obj = new DiagramGoo(Point3d.Origin, Vector3d.Zero,
        new Force(4, ForceUnit.Kilonewton).ToString(), ArrowMode.NoArrow);

      string expectedString = "A GSA result diagram type.";

      Assert.Equal(expectedString, obj.TypeDescription);
    }

    [Fact]
    public void WhenGetTypeName_ThenShouldReturnValidString() {
      var obj = new DiagramGoo(Point3d.Origin, Vector3d.Zero,
        new Force(4, ForceUnit.Kilonewton).ToString(), ArrowMode.NoArrow);

      string expectedString = "Diagram Vector";

      Assert.Equal(expectedString, obj.TypeName);
    }

    [Fact]
    public void WhenInitialised_ThenClassContainSetColorMethod() {
      var obj = new DiagramGoo(Point3d.Unset, Vector3d.Unset, new Force().ToString(),
        ArrowMode.NoArrow);
      MethodInfo[] actualMethods = obj.GetType().GetMethods();

      Assert.Contains(actualMethods, info => info.Name.Equals("SetColor"));

      MethodInfo setColorMethodInfo
        = actualMethods.FirstOrDefault(method => method.Name.Equals("SetColor"));

      Assert.True(setColorMethodInfo?.IsPublic);
      Assert.True(setColorMethodInfo?.GetParameters().Length == 1);
      Assert.True(setColorMethodInfo?.GetParameters()[0].ParameterType == typeof(Color));
      Assert.IsType<DiagramGoo>(obj.SetColor(Color.Red));
    }

    [Fact]
    public void WhenSetColor_ThenColorOfDiagramShoouldBeChanged() {
      var obj = new DiagramGoo(Point3d.Unset, Vector3d.Unset, new Force().ToString(),
        ArrowMode.NoArrow);
      Assert.Equal(Colours.GsaDarkPurple, obj.Color);

      obj.SetColor(Color.Aqua);

      Assert.Equal(Color.Aqua, obj.Color);
    }

    /// <summary>
    ///   ToDO remove when showText will be deleted
    /// </summary>
    [Fact]
    public void WhenInitialised_ThenClassContainShowTextMethod() {
      var obj = new DiagramGoo(Point3d.Unset, Vector3d.Unset, new Force().ToString(),
        ArrowMode.NoArrow);
      MethodInfo[] actualMethods = obj.GetType().GetMethods();

      Assert.Contains(actualMethods, info => info.Name.Equals("ShowText"));

      MethodInfo showTextMethodInfo
        = actualMethods.FirstOrDefault(method => method.Name.Equals("ShowText"));

      Assert.True(showTextMethodInfo?.IsPublic);
      Assert.True(showTextMethodInfo?.GetParameters().Length == 1);
      Assert.True(showTextMethodInfo?.GetParameters()[0].ParameterType == typeof(bool));
      Assert.True(showTextMethodInfo?.ReturnParameter?.ParameterType.Name == "Void");
    }

    [Fact]
    public void WhenMorph_ThenMethod_ShouldNothingChange() {
      var startingPoint = new Point3d(3, 3, 3);
      var vector3d = new Vector3d(2, 2, 2);
      string force = new Force(4, ForceUnit.Kilonewton).ToString();
      var obj = new DiagramGoo(startingPoint, vector3d, force, ArrowMode.NoArrow);

      IGH_GeometricGoo actualObject = obj.Morph(new TwistSpaceMorph());
      var expectedObject = new DiagramGoo(startingPoint, vector3d, force, ArrowMode.NoArrow);

      Assert.Equal(expectedObject.Boundingbox.Diagonal, actualObject.Boundingbox.Diagonal);
      Assert.Equal(expectedObject.Boundingbox.Max, actualObject.Boundingbox.Max);
      Assert.Equal(expectedObject.Boundingbox.Min, actualObject.Boundingbox.Min);
    }

    [Fact]
    public void WhenScriptVariable_ThenMethod_ShouldReturnGhVector() {
      var startingPoint = new Point3d(3, 3, 3);
      var vector3d = new Vector3d(2, 2, 2);
      string force = new Force(4, ForceUnit.Kilonewton).ToString();
      var obj = new DiagramGoo(startingPoint, vector3d, force, ArrowMode.NoArrow);

      object actualObject = obj.ScriptVariable();

      Assert.IsType<GH_Vector>(actualObject);
    }

    [Fact]
    public void WhenToString_ThenMethod_ShouldReturnValidString() {
      var obj = new DiagramGoo(Point3d.Origin, Vector3d.Zero,
        new Force(4, ForceUnit.Kilonewton).ToString(), ArrowMode.NoArrow);

      string expectedString
        = $"Diagram Result: Starting point: {Point3d.Origin}, Direction:{Vector3d.Zero}, Force:4 kN";

      Assert.Equal(expectedString, obj.ToString());
    }

    [Fact]
    public void WhenTransform_ThenMethod_ShouldReturnValidObject() {
      var startingPoint = new Point3d(3, 3, 3);
      var vector3d = new Vector3d(2, 2, 2);
      var obj = new DiagramGoo(startingPoint, vector3d,
        new Force(4, ForceUnit.Kilonewton).ToString(), ArrowMode.NoArrow);

      IGH_GeometricGoo actualObject
        = obj.Transform(new Transform(Transform.Mirror(startingPoint, vector3d)));
      var expectedObject = new Vector3d(2, 2, 2);

      Assert.Equal(expectedObject.X, actualObject.Boundingbox.Diagonal.X, 6);
      Assert.Equal(expectedObject.Y, actualObject.Boundingbox.Diagonal.Y, 6);
      Assert.Equal(expectedObject.Z, actualObject.Boundingbox.Diagonal.Z, 6);
    }
  }
}
