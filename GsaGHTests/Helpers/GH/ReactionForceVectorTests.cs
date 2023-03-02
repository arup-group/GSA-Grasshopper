using GsaGH.Helpers.GH;
using OasysUnits;
using OasysUnits.Units;
using Rhino.Geometry;
using Xunit;

namespace GsaGHTests.Helpers.GH
{
  [Collection("GrasshopperFixture collection")]
  public class ReactionForceVectorTests
  {
    [Fact]
    public void WhenCreateInstance_ThenObject_ShouldNotBeNull()
    {
      var obj = new ReactionForceVector(0, Point3d.Unset, Vector3d.Unset, null, ForceType.Force);
      Assert.NotNull(obj);
    }

    [Fact]
    public void WhenCreateInstanceWithValidArguments_ThenVariables_ShouldBeValid()
    {
      var expectedId = 11;
      var expectedPoint = new Point3d(1, 11, 111);
      var expectedVector = new Vector3d(2, 22, 222);
      var expectedForceValue = new Force(33, ForceUnit.Kilonewton);
      var expectedForceType = ForceType.Force;
      var expectedReactionForceLine = new Line(expectedPoint *-1, expectedVector);

      var obj = new ReactionForceVector(expectedId, expectedPoint, expectedVector, expectedForceValue, expectedForceType);

      Assert.Equal(expectedId, obj.Id);
      Assert.Equal(expectedPoint, obj.StartingPoint);
      Assert.Equal(expectedVector, obj.Direction);
      Assert.Equal(expectedForceValue, obj.ForceValue);
      Assert.Equal(expectedForceType, obj.GetForceType());
      Assert.Equal(expectedReactionForceLine, obj.GetReactionForceLine());
    }

    [Fact]
    public void WhenCreateInstanceWithDefaultPointAndVector_ThenVariables_ShouldBeValid()
    {
      var expectedId = 11;
      var expectedPoint = new Point3d();
      var expectedVector = new Vector3d();
      var expectedForceValue = new Force(0, ForceUnit.Kilonewton);
      var expectedForceType = ForceType.Moment;
      var expectedReactionForceLine = new Line(expectedPoint * -1, expectedVector);

      var obj = new ReactionForceVector(expectedId, expectedPoint, expectedVector, expectedForceValue, expectedForceType);

      Assert.Equal(expectedId, obj.Id);
      Assert.Equal(expectedPoint, obj.StartingPoint);
      Assert.Equal(expectedVector, obj.Direction);
      Assert.Equal(expectedForceValue, obj.ForceValue);
      Assert.Equal(expectedForceType, obj.GetForceType());
      Assert.Equal(expectedReactionForceLine, obj.GetReactionForceLine());
    }
    
    [Fact]
    public void WhenCreateInstanceWithNullAndUnsetArgs_ThenVariables_ShouldBeValid()
    {
      var expectedId = 11;
      var expectedPoint = Point3d.Origin;
      var expectedVector = Vector3d.Zero;
      IQuantity expectedForceValue = null;
      var expectedForceType = ForceType.Moment;
      var expectedReactionForceLine = new Line(expectedPoint * -1, expectedVector);

      var obj = new ReactionForceVector(expectedId, Point3d.Unset, Vector3d.Unset, null, expectedForceType);

      Assert.Equal(expectedId, obj.Id);
      Assert.Equal(expectedPoint, obj.StartingPoint);
      Assert.Equal(expectedVector, obj.Direction);
      Assert.Equal(expectedForceValue, obj.ForceValue);
      Assert.Equal(expectedForceType, obj.GetForceType());
      Assert.Equal(expectedReactionForceLine, obj.GetReactionForceLine());
    }

    [Fact]
    public void WhenCalculatingOffsets_AndLineLengthIsZero_ThenPointPosition_ShouldBeValid()
    {
      var obj = new ReactionForceVector(1, Point3d.Unset, Vector3d.Unset, null, ForceType.Moment);
      var actualEndOffsetPosition = obj.CalculateExtraEndOffsetPoint(2000, 30);
      var actualStartOffsetPosition = obj.CalculateExtraStartOffsetPoint(2000, 30);

      Assert.Equal(new Line(), obj.GetReactionForceLine());
      Assert.Equal(Point3d.Origin, actualStartOffsetPosition);
      Assert.Equal(Point3d.Origin, actualEndOffsetPosition);
    }

    [Fact]
    public void WhenCalculatingOffsets_AndLineLengthIsNonZero_ThenPointPosition_ShouldBeValid()
    {
      var obj = new ReactionForceVector(1, new Point3d(1,11,1), new Vector3d(2,22,2), null, ForceType.Moment);
      var actualEndOffsetPosition = obj.CalculateExtraEndOffsetPoint(2000, 30);
      var actualStartOffsetPosition = obj.CalculateExtraStartOffsetPoint(2000, 30);

      var expectedReactionForceLine = new Line(new Point3d(1, 11, 1) * -1, new Vector3d(2, 22, 2));

      Assert.Equal(expectedReactionForceLine, obj.GetReactionForceLine());
      Assert.Equal(new Point3d(0.99864749554799881, 10.985122451027987, 0.99864749554799881), actualStartOffsetPosition);
      Assert.Equal(new Point3d(-1.0013525044520011, -11.014877548972013, -1.0013525044520011), actualEndOffsetPosition);
    }

    [Fact]
    public void WhenCalculatingOffsets_AndArgsAreZero_ThenPointPosition_ShouldBeTheSameAsPointPosition()
    {
      var obj = new ReactionForceVector(1, new Point3d(1, 11, 1), new Vector3d(2, 22, 2), null, ForceType.Moment);
      var actualEndOffsetPosition = obj.CalculateExtraEndOffsetPoint(0, 0);
      var actualStartOffsetPosition = obj.CalculateExtraStartOffsetPoint(0, 0);

      var expectedReactionForceLine = new Line(new Point3d(1, 11, 1) * -1, new Vector3d(2, 22, 2));

      Assert.Equal(expectedReactionForceLine, obj.GetReactionForceLine());
      Assert.Equal(new Point3d(1, 11, 1), actualStartOffsetPosition);
      Assert.Equal(new Point3d(-1,-11,-1), actualEndOffsetPosition);
    }
  }
}
