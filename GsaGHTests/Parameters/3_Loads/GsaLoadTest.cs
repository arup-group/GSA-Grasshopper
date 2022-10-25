using GsaAPI;
using GsaGH.Parameters;
using GsaGH.Util.Gsa.ToGSA;
using GsaGHTests.Helpers;
using Xunit;
using static GsaGH.Parameters.GsaLoad;
using static GsaGH.Parameters.GsaNodeLoad;

namespace GsaGHTests.Parameters
{
  [Collection("GrasshopperFixture collection")]
  public class GsaLoadTest
  {
    [Fact]
    public void ConstructorTest()
    {
      // Act
      GsaLoad load = new GsaLoad();

      // Assert
      Assert.Equal(LoadTypes.Gravity, load.LoadType);
      Assert.Equal(0, load.GravityLoad.GravityLoad.Factor.X);
      Assert.Equal(0, load.GravityLoad.GravityLoad.Factor.Y);
      Assert.Equal(-1, load.GravityLoad.GravityLoad.Factor.Z);
      Assert.Equal(1, load.GravityLoad.GravityLoad.Case);
      Assert.Equal("all", load.GravityLoad.GravityLoad.Elements);
      Assert.Equal("all", load.GravityLoad.GravityLoad.Nodes);
    }

    [Fact]
    public void GravityLoadConstructorTest()
    {
      // Act
      GsaLoad load = new GsaLoad(new GsaGravityLoad());

      // Assert
      Assert.Equal(LoadTypes.Gravity, load.LoadType);
      Assert.Equal(0, load.GravityLoad.GravityLoad.Factor.X);
      Assert.Equal(0, load.GravityLoad.GravityLoad.Factor.Y);
      Assert.Equal(-1, load.GravityLoad.GravityLoad.Factor.Z);
      Assert.Equal(1, load.GravityLoad.GravityLoad.Case);
      Assert.Equal("all", load.GravityLoad.GravityLoad.Elements);
      Assert.Equal("all", load.GravityLoad.GravityLoad.Nodes);
    }

    [Fact]
    public void NodeLoadEmptyConstructorTest()
    {
      // Act
      GsaNodeLoad nodeLoad = new GsaNodeLoad();
      GsaLoad load = new GsaLoad(nodeLoad);

      // Assert
      Assert.Equal(LoadTypes.Node, load.LoadType);
      Assert.Equal(NodeLoadTypes.NODE_LOAD, load.NodeLoad.Type);
    }

    [Theory]
    [InlineData(NodeLoadTypes.NODE_LOAD)]
    [InlineData(NodeLoadTypes.APPLIED_DISP)]
    [InlineData(NodeLoadTypes.SETTLEMENT)]
    [InlineData(NodeLoadTypes.GRAVITY)]
    [InlineData(NodeLoadTypes.NUM_TYPES)]
    public void NodeLoadConstructorTest(NodeLoadTypes type)
    {
      // Act
      GsaNodeLoad nodeLoad = new GsaNodeLoad();
      nodeLoad.Type = type;
      GsaLoad load = new GsaLoad(nodeLoad);

      // Assert
      Assert.Equal(LoadTypes.Node, load.LoadType);
      Assert.Equal(type, load.NodeLoad.Type);
    }

    [Fact]
    public void BeamLoadConstructorTest()
    {
      // Act
      GsaBeamLoad beamLoad = new GsaBeamLoad();
      GsaLoad load = new GsaLoad(beamLoad);

      // Assert
      Assert.Equal(LoadTypes.Beam, load.LoadType);
    }

    [Fact]
    public void FaceLoadConstructorTest()
    {
      // Act
      GsaFaceLoad faceLoad = new GsaFaceLoad();
      GsaLoad load = new GsaLoad(faceLoad);

      // Assert
      Assert.Equal(LoadTypes.Face, load.LoadType);
      Assert.Equal(FaceLoadType.CONSTANT, load.FaceLoad.FaceLoad.Type);
    }

    [Fact]
    public void DuplicateTest()
    {
      // Arrange
      GsaLoad original = new GsaLoad();

      // Act
      GsaLoad duplicate = original.Duplicate();

      // Assert
      Duplicates.AreEqual(original, duplicate);

      // make some changes to duplicate
      duplicate.LoadType = LoadTypes.GridLine;
      duplicate.GravityLoad.GravityLoad.Factor = new Vector3() { X = 1, Y = 1, Z = 1 };
      duplicate.GravityLoad.GravityLoad.Case = 3;
      duplicate.GravityLoad.GravityLoad.Elements = "not_all";
      duplicate.GravityLoad.GravityLoad.Nodes = "some";

      // Assert
      Assert.Equal(LoadTypes.Gravity, original.LoadType);
      Assert.Equal(0, original.GravityLoad.GravityLoad.Factor.X);
      Assert.Equal(0, original.GravityLoad.GravityLoad.Factor.Y);
      Assert.Equal(-1, original.GravityLoad.GravityLoad.Factor.Z);
      Assert.Equal(1, original.GravityLoad.GravityLoad.Case);
      Assert.Equal("all", original.GravityLoad.GravityLoad.Elements);
      Assert.Equal("all", original.GravityLoad.GravityLoad.Nodes);
    }

    [Fact]
    public void GravityLoadDuplicateTest()
    {
      // Arrange
      GsaGravityLoad gravityLoad = new GsaGravityLoad();
      gravityLoad.GravityLoad.Name = "name";
      GsaLoad original = new GsaLoad(gravityLoad);

      // Act
      GsaLoad duplicate = original.Duplicate();

      // Assert
      Duplicates.AreEqual(original, duplicate);

      // make some changes to duplicate
      duplicate.LoadType = LoadTypes.GridLine;
      duplicate.GravityLoad.GravityLoad.Factor = new Vector3() { X = 1, Y = 1, Z = 1 };
      duplicate.GravityLoad.GravityLoad.Case = 3;
      duplicate.GravityLoad.GravityLoad.Elements = "not_all";
      duplicate.GravityLoad.GravityLoad.Nodes = "some";
      duplicate.GravityLoad.GravityLoad.Name = "";

      // Assert
      Assert.Equal(LoadTypes.Gravity, original.LoadType);
      Assert.Equal(0, original.GravityLoad.GravityLoad.Factor.X);
      Assert.Equal(0, original.GravityLoad.GravityLoad.Factor.Y);
      Assert.Equal(-1, original.GravityLoad.GravityLoad.Factor.Z);
      Assert.Equal(1, original.GravityLoad.GravityLoad.Case);
      Assert.Equal("all", original.GravityLoad.GravityLoad.Elements);
      Assert.Equal("all", original.GravityLoad.GravityLoad.Nodes);
      Assert.Equal("name", original.GravityLoad.GravityLoad.Name);
    }

    [Theory]
    [InlineData(NodeLoadTypes.NODE_LOAD, NodeLoadTypes.GRAVITY)]
    [InlineData(NodeLoadTypes.APPLIED_DISP, NodeLoadTypes.NODE_LOAD)]
    [InlineData(NodeLoadTypes.SETTLEMENT, NodeLoadTypes.NODE_LOAD)]
    [InlineData(NodeLoadTypes.GRAVITY, NodeLoadTypes.NODE_LOAD)]
    [InlineData(NodeLoadTypes.NUM_TYPES, NodeLoadTypes.NODE_LOAD)]
    public void NodeLoadDuplicateTest(NodeLoadTypes originalType, NodeLoadTypes duplicateType)
    {
      // Arrange
      GsaNodeLoad nodeLoad = new GsaNodeLoad();
      nodeLoad.Type = originalType;
      nodeLoad.NodeLoad.AxisProperty = 2;
      nodeLoad.NodeLoad.Case = 100;
      nodeLoad.NodeLoad.Direction = Direction.XY;
      nodeLoad.NodeLoad.Nodes = "all";
      nodeLoad.NodeLoad.Name = "name";
      nodeLoad.NodeLoad.Value = 97.5;
      GsaLoad original = new GsaLoad(nodeLoad);

      // Act
      GsaLoad duplicate = original.Duplicate();

      // Assert
      Duplicates.AreEqual(original, duplicate);

      // make some changes to duplicate
      duplicate.NodeLoad.Type = duplicateType;
      duplicate.NodeLoad.NodeLoad.AxisProperty = 3;
      duplicate.NodeLoad.NodeLoad.Case = 99;
      duplicate.NodeLoad.NodeLoad.Direction = Direction.YY;
      duplicate.NodeLoad.NodeLoad.Nodes = "some";
      duplicate.NodeLoad.NodeLoad.Name = "none";
      duplicate.NodeLoad.NodeLoad.Value = -3.3;

      // Assert
      Assert.Equal(LoadTypes.Node, original.LoadType);
      Assert.Equal(originalType, original.NodeLoad.Type);
      Assert.Equal(2, original.NodeLoad.NodeLoad.AxisProperty);
      Assert.Equal(100, original.NodeLoad.NodeLoad.Case);
      Assert.Equal(Direction.XY, original.NodeLoad.NodeLoad.Direction);
      Assert.Equal("all", original.NodeLoad.NodeLoad.Nodes);
      Assert.Equal("name", original.NodeLoad.NodeLoad.Name);
      Assert.Equal(97.5, original.NodeLoad.NodeLoad.Value);
    }

    [Fact]
    public void dest()
    {
      // Arrange
      GsaBeamLoad beamLoad = new GsaBeamLoad();
      beamLoad.BeamLoad.Type = BeamLoadType.POINT;
      beamLoad.BeamLoad.AxisProperty = 5;
      beamLoad.BeamLoad.Case = 6;
      beamLoad.BeamLoad.Direction = Direction.ZZ;
      beamLoad.BeamLoad.Elements = "elements";
      beamLoad.BeamLoad.Name = "name";
      beamLoad.BeamLoad.IsProjected = true;
      beamLoad.BeamLoad.IsProjected = true;
      GsaLoad original = new GsaLoad(beamLoad);

      // Act
      GsaLoad duplicate = original.Duplicate();

      // Assert
      Duplicates.AreEqual(original, duplicate);

      // make some changes to duplicate
      duplicate.BeamLoad.BeamLoad.Type = BeamLoadType.UNDEF;
      duplicate.BeamLoad.BeamLoad.AxisProperty = 1;
      duplicate.BeamLoad.BeamLoad.Case = 1;
      duplicate.BeamLoad.BeamLoad.Direction = Direction.XX;
      duplicate.BeamLoad.BeamLoad.Elements = "none";
      duplicate.BeamLoad.BeamLoad.Name = "";
      duplicate.BeamLoad.BeamLoad.IsProjected = false;
      duplicate.BeamLoad.BeamLoad.SetPosition(0, 99);
      duplicate.BeamLoad.BeamLoad.SetValue(0, 99);
      duplicate.BeamLoad.BeamLoad.SetPosition(1, 99);
      duplicate.BeamLoad.BeamLoad.SetValue(1, 99);

      // Assert
      Assert.Equal(LoadTypes.Beam, original.LoadType);
      Assert.Equal(BeamLoadType.POINT, original.BeamLoad.BeamLoad.Type);
      Assert.Equal(5, original.BeamLoad.BeamLoad.AxisProperty);
      Assert.Equal(6, original.BeamLoad.BeamLoad.Case);
      Assert.Equal(Direction.ZZ, original.BeamLoad.BeamLoad.Direction);
      Assert.Equal("elements", original.BeamLoad.BeamLoad.Elements);
      Assert.Equal("name", original.BeamLoad.BeamLoad.Name);
      Assert.True(original.BeamLoad.BeamLoad.IsProjected);
      if (original.BeamLoad.BeamLoad.Type == BeamLoadType.POINT)
      {
        Assert.Equal(0, original.BeamLoad.BeamLoad.Position(0));
        Assert.Equal(0, original.BeamLoad.BeamLoad.Value(0));
      }
      else if (original.BeamLoad.BeamLoad.Type == BeamLoadType.UNIFORM)
      {
        Assert.Equal(0, original.BeamLoad.BeamLoad.Value(0));
      }
      else if (original.BeamLoad.BeamLoad.Type == BeamLoadType.LINEAR)
      {
        Assert.Equal(0, original.BeamLoad.BeamLoad.Position(0));
        Assert.Equal(1, original.BeamLoad.BeamLoad.Value(1));
      }
      else if (original.BeamLoad.BeamLoad.Type == BeamLoadType.PATCH)
      {
        Assert.Equal(0, original.BeamLoad.BeamLoad.Position(0));
        Assert.Equal(1, original.BeamLoad.BeamLoad.Position(1));
        Assert.Equal(0, original.BeamLoad.BeamLoad.Value(0));
        Assert.Equal(1, original.BeamLoad.BeamLoad.Value(1));
      }
      else if (original.BeamLoad.BeamLoad.Type == BeamLoadType.TRILINEAR)
      {
        Assert.Equal(0, original.BeamLoad.BeamLoad.Position(0));
        Assert.Equal(1, original.BeamLoad.BeamLoad.Position(1));
        Assert.Equal(0, original.BeamLoad.BeamLoad.Value(0));
        Assert.Equal(1, original.BeamLoad.BeamLoad.Value(1));
      }
    }

    [Theory]
    [InlineData(BeamLoadType.UNDEF, BeamLoadType.UNIFORM)]
    [InlineData(BeamLoadType.POINT, BeamLoadType.UNDEF)]
    [InlineData(BeamLoadType.UNIFORM, BeamLoadType.UNDEF)]
    [InlineData(BeamLoadType.LINEAR, BeamLoadType.UNDEF)]
    [InlineData(BeamLoadType.PATCH, BeamLoadType.UNDEF)]
    [InlineData(BeamLoadType.TRILINEAR, BeamLoadType.UNDEF)]
    public void BeamLoadDuplicateTest(BeamLoadType originalType, BeamLoadType duplicateType)
    {
      // Arrange
      GsaBeamLoad beamLoad = new GsaBeamLoad();
      beamLoad.BeamLoad.Type = originalType;
      beamLoad.BeamLoad.AxisProperty = 5;
      beamLoad.BeamLoad.Case = 6;
      beamLoad.BeamLoad.Direction = Direction.ZZ;
      beamLoad.BeamLoad.Elements = "elements";
      beamLoad.BeamLoad.Name = "name";
      beamLoad.BeamLoad.IsProjected = true;
      beamLoad.BeamLoad.IsProjected = true;
      GsaLoad original = new GsaLoad(beamLoad);

      // Act
      GsaLoad duplicate = original.Duplicate();

      // Assert
      Duplicates.AreEqual(original, duplicate);

      // make some changes to duplicate
      duplicate.BeamLoad.BeamLoad.Type = duplicateType;
      duplicate.BeamLoad.BeamLoad.AxisProperty = 1;
      duplicate.BeamLoad.BeamLoad.Case = 1;
      duplicate.BeamLoad.BeamLoad.Direction = Direction.XX;
      duplicate.BeamLoad.BeamLoad.Elements = "none";
      duplicate.BeamLoad.BeamLoad.Name = "";
      duplicate.BeamLoad.BeamLoad.IsProjected = false;
      duplicate.BeamLoad.BeamLoad.SetPosition(0, 99);
      duplicate.BeamLoad.BeamLoad.SetValue(0, 99);
      duplicate.BeamLoad.BeamLoad.SetPosition(1, 99);
      duplicate.BeamLoad.BeamLoad.SetValue(1, 99);

      // Assert
      Assert.Equal(LoadTypes.Beam, original.LoadType);
      Assert.Equal(originalType, original.BeamLoad.BeamLoad.Type);
      Assert.Equal(5, original.BeamLoad.BeamLoad.AxisProperty);
      Assert.Equal(6, original.BeamLoad.BeamLoad.Case);
      Assert.Equal(Direction.ZZ, original.BeamLoad.BeamLoad.Direction);
      Assert.Equal("elements", original.BeamLoad.BeamLoad.Elements);
      Assert.Equal("name", original.BeamLoad.BeamLoad.Name);
      Assert.True(original.BeamLoad.BeamLoad.IsProjected);
      if (original.BeamLoad.BeamLoad.Type == BeamLoadType.POINT)
      {
        Assert.Equal(0, original.BeamLoad.BeamLoad.Position(0));
        Assert.Equal(0, original.BeamLoad.BeamLoad.Value(0));
      }
      else if (original.BeamLoad.BeamLoad.Type == BeamLoadType.UNIFORM)
      {
        Assert.Equal(0, original.BeamLoad.BeamLoad.Value(0));
      }
      else if (original.BeamLoad.BeamLoad.Type == BeamLoadType.LINEAR)
      {
        Assert.Equal(0, original.BeamLoad.BeamLoad.Position(0));
        Assert.Equal(1, original.BeamLoad.BeamLoad.Value(1));
      }
      else if (original.BeamLoad.BeamLoad.Type == BeamLoadType.PATCH)
      {
        Assert.Equal(0, original.BeamLoad.BeamLoad.Position(0));
        Assert.Equal(1, original.BeamLoad.BeamLoad.Position(1));
        Assert.Equal(0, original.BeamLoad.BeamLoad.Value(0));
        Assert.Equal(1, original.BeamLoad.BeamLoad.Value(1));
      }
      else if (original.BeamLoad.BeamLoad.Type == BeamLoadType.TRILINEAR)
      {
        Assert.Equal(0, original.BeamLoad.BeamLoad.Position(0));
        Assert.Equal(1, original.BeamLoad.BeamLoad.Position(1));
        Assert.Equal(0, original.BeamLoad.BeamLoad.Value(0));
        Assert.Equal(1, original.BeamLoad.BeamLoad.Value(1));
      }
    }

    [Theory]
    [InlineData(FaceLoadType.UNDEF, FaceLoadType.CONSTANT)]
    [InlineData(FaceLoadType.CONSTANT, FaceLoadType.UNDEF)]
    [InlineData(FaceLoadType.GENERAL, FaceLoadType.UNDEF)]
    [InlineData(FaceLoadType.POINT, FaceLoadType.UNDEF)]
    public void FaceLoadDuplicateTest(FaceLoadType originalType, FaceLoadType duplicateType)
    {
      // Arrange
      GsaFaceLoad faceLoad = new GsaFaceLoad();
      faceLoad.FaceLoad.AxisProperty = 5;
      faceLoad.FaceLoad.Case = 6;
      faceLoad.FaceLoad.Direction = Direction.ZZ;
      faceLoad.FaceLoad.Elements = "elements";
      faceLoad.FaceLoad.Name = "name";
      faceLoad.FaceLoad.Type = originalType;
      GsaLoad original = new GsaLoad(faceLoad);

      // Act
      GsaLoad duplicate = original.Duplicate();

      // Assert
      Duplicates.AreEqual(original, duplicate);

      // make some changes to duplicate
      duplicate.FaceLoad.FaceLoad.Type = duplicateType;
      duplicate.FaceLoad.FaceLoad.AxisProperty = 1;
      duplicate.FaceLoad.FaceLoad.Case = 1;
      duplicate.FaceLoad.FaceLoad.Direction = Direction.XX;
      duplicate.FaceLoad.FaceLoad.Elements = "none";
      duplicate.FaceLoad.FaceLoad.IsProjected = true;
      duplicate.FaceLoad.FaceLoad.SetValue(0, 99);
      duplicate.FaceLoad.FaceLoad.SetValue(1, 99);
      duplicate.FaceLoad.FaceLoad.SetValue(2, 99);
      duplicate.FaceLoad.FaceLoad.SetValue(3, 99);
      //duplicate.FaceLoad.FaceLoad.Position = new Vector2(); // not yet implemented in Gsa API

      // Assert
      Assert.Equal(LoadTypes.Face, original.LoadType);
      Assert.Equal(originalType, original.FaceLoad.FaceLoad.Type);
      Assert.Equal(5, original.BeamLoad.BeamLoad.AxisProperty);
      Assert.Equal(6, original.BeamLoad.BeamLoad.Case);
      Assert.Equal(Direction.ZZ, original.BeamLoad.BeamLoad.Direction);
      Assert.Equal("elements", original.BeamLoad.BeamLoad.Elements);
      Assert.Equal("name", original.BeamLoad.BeamLoad.Name);


      if (original.FaceLoad.FaceLoad.Type == FaceLoadType.CONSTANT)
      {
        Assert.False(original.FaceLoad.FaceLoad.IsProjected);
        Assert.Equal(0, original.FaceLoad.FaceLoad.Value(0));
      }
      else if (original.FaceLoad.FaceLoad.Type == FaceLoadType.GENERAL)
      {
        Assert.False(original.FaceLoad.FaceLoad.IsProjected);
        Assert.Equal(0, original.FaceLoad.FaceLoad.Value(0));
        Assert.Equal(1, original.FaceLoad.FaceLoad.Value(1));
        Assert.Equal(2, original.FaceLoad.FaceLoad.Value(2));
        Assert.Equal(3, original.FaceLoad.FaceLoad.Value(3));
      }
      else if (original.FaceLoad.FaceLoad.Type == FaceLoadType.POINT)
      {
        Assert.False(original.FaceLoad.FaceLoad.IsProjected);
        Assert.Equal(0, original.FaceLoad.FaceLoad.Value(0));
        Assert.Equal(1, original.FaceLoad.FaceLoad.Position.X);
        Assert.Equal(1, original.FaceLoad.FaceLoad.Position.Y);
      }
    }
  }
}