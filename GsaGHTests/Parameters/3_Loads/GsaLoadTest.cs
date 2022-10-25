using System;
using GsaAPI;
using GsaGH.Parameters;
using GsaGH.Util.Gsa.ToGSA;
using GsaGHTests.Helpers;
using Rhino.Geometry;
using Xunit;
using static GsaAPI.GridLineLoad;
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
    public void GridPointLoadConstructorTest()
    {
      // Act
      GsaGridPointLoad gridPointLoad = new GsaGridPointLoad();
      GsaLoad load = new GsaLoad(gridPointLoad);

      // Assert
      Assert.Equal(LoadTypes.GridPoint, load.LoadType);
    }

    [Fact]
    public void GridLineLoadConstructorTest()
    {
      // Act
      GsaGridLineLoad gridLineLoad = new GsaGridLineLoad();
      GsaLoad load = new GsaLoad(gridLineLoad);

      // Assert
      Assert.Equal(LoadTypes.GridLine, load.LoadType);
      Assert.Equal(0, load.LineLoad.GridLineLoad.PolyLineReference);
    }

    [Fact]
    public void GridAreaLoadConstructorTest()
    {
      // Act
      GsaGridAreaLoad gridAreaLoad = new GsaGridAreaLoad();
      GsaLoad load = new GsaLoad(gridAreaLoad);

      // Assert
      Assert.Equal(LoadTypes.GridArea, load.LoadType);
      Assert.Equal(GridAreaPolyLineType.PLANE, load.AreaLoad.GridAreaLoad.Type);
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
      duplicate.GravityLoad.GravityLoad.Elements = "";
      duplicate.GravityLoad.GravityLoad.Nodes = "";

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
      duplicate.GravityLoad.GravityLoad.Elements = "";
      duplicate.GravityLoad.GravityLoad.Nodes = "";
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
      duplicate.NodeLoad.NodeLoad.Nodes = "";
      duplicate.NodeLoad.NodeLoad.Name = "";
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

    [Theory]
    [InlineData("UNDEF", "UNIFORM")]
    [InlineData("POINT", "UNDEF")]
    [InlineData("UNIFORM", "UNDEF")]
    [InlineData("LINEAR", "UNDEF")]
    [InlineData("PATCH", "UNDEF")]
    [InlineData("TRILINEAR", "UNDEF")]
    public void BeamLoadDuplicateTest(string originalTypeString, string duplicateTypeString)
    {
      // Arrange
      BeamLoadType originalType = (BeamLoadType)Enum.Parse(typeof(BeamLoadType), originalTypeString);
      BeamLoadType duplicateType = (BeamLoadType)Enum.Parse(typeof(BeamLoadType), duplicateTypeString);

      GsaBeamLoad beamLoad = new GsaBeamLoad();
      beamLoad.BeamLoad.Type = originalType;
      beamLoad.BeamLoad.AxisProperty = 5;
      beamLoad.BeamLoad.Case = 6;
      beamLoad.BeamLoad.Direction = Direction.ZZ;
      beamLoad.BeamLoad.Elements = "all";
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
      duplicate.BeamLoad.BeamLoad.Elements = "";
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
      Assert.Equal("all", original.BeamLoad.BeamLoad.Elements);
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
        Assert.Equal(0, original.BeamLoad.BeamLoad.Value(1));
      }
      else if (original.BeamLoad.BeamLoad.Type == BeamLoadType.PATCH)
      {
        Assert.Equal(0, original.BeamLoad.BeamLoad.Position(0));
        Assert.Equal(0, original.BeamLoad.BeamLoad.Position(1));
        Assert.Equal(0, original.BeamLoad.BeamLoad.Value(0));
        Assert.Equal(0, original.BeamLoad.BeamLoad.Value(1));
      }
      else if (original.BeamLoad.BeamLoad.Type == BeamLoadType.TRILINEAR)
      {
        Assert.Equal(0, original.BeamLoad.BeamLoad.Position(0));
        Assert.Equal(0, original.BeamLoad.BeamLoad.Position(1));
        Assert.Equal(0, original.BeamLoad.BeamLoad.Value(0));
        Assert.Equal(0, original.BeamLoad.BeamLoad.Value(1));
      }
    }

    [Theory]
    [InlineData("UNDEF", "CONSTANT")]
    [InlineData("CONSTANT", "UNDEF")]
    [InlineData("GENERAL", "UNDEF")]
    [InlineData("POINT", "UNDEF")]
    public void FaceLoadDuplicateTest(string originalTypeString, string duplicateTypeString)
    {
      // Arrange
      FaceLoadType originalType = (FaceLoadType)Enum.Parse(typeof(FaceLoadType), originalTypeString);
      FaceLoadType duplicateType = (FaceLoadType)Enum.Parse(typeof(FaceLoadType), duplicateTypeString);

      GsaFaceLoad faceLoad = new GsaFaceLoad();
      faceLoad.FaceLoad.AxisProperty = 5;
      faceLoad.FaceLoad.Case = 6;
      faceLoad.FaceLoad.Direction = Direction.ZZ;
      faceLoad.FaceLoad.Elements = "all";
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
      duplicate.FaceLoad.FaceLoad.Elements = "";
      duplicate.FaceLoad.FaceLoad.Name = "";
      duplicate.FaceLoad.FaceLoad.IsProjected = true;
      duplicate.FaceLoad.FaceLoad.SetValue(0, 99);
      duplicate.FaceLoad.FaceLoad.SetValue(1, 99);
      duplicate.FaceLoad.FaceLoad.SetValue(2, 99);
      duplicate.FaceLoad.FaceLoad.SetValue(3, 99);
      //duplicate.FaceLoad.FaceLoad.Position = new Vector2(); // not yet implemented in Gsa API

      // Assert
      Assert.Equal(LoadTypes.Face, original.LoadType);
      Assert.Equal(originalType, original.FaceLoad.FaceLoad.Type);
      Assert.Equal(5, original.FaceLoad.FaceLoad.AxisProperty);
      Assert.Equal(6, original.FaceLoad.FaceLoad.Case);
      Assert.Equal(Direction.ZZ, original.FaceLoad.FaceLoad.Direction);
      Assert.Equal("all", original.FaceLoad.FaceLoad.Elements);
      Assert.Equal("name", original.FaceLoad.FaceLoad.Name);

      if (original.FaceLoad.FaceLoad.Type == FaceLoadType.CONSTANT)
      {
        Assert.False(original.FaceLoad.FaceLoad.IsProjected);
        Assert.Equal(0, original.FaceLoad.FaceLoad.Value(0));
      }
      else if (original.FaceLoad.FaceLoad.Type == FaceLoadType.GENERAL)
      {
        Assert.False(original.FaceLoad.FaceLoad.IsProjected);
        Assert.Equal(0, original.FaceLoad.FaceLoad.Value(0));
        Assert.Equal(0, original.FaceLoad.FaceLoad.Value(1));
        Assert.Equal(0, original.FaceLoad.FaceLoad.Value(2));
        Assert.Equal(0, original.FaceLoad.FaceLoad.Value(3));
      }
      else if (original.FaceLoad.FaceLoad.Type == FaceLoadType.POINT)
      {
        Assert.False(original.FaceLoad.FaceLoad.IsProjected);
        Assert.Equal(0, original.FaceLoad.FaceLoad.Value(0));
        Assert.Equal(0, original.FaceLoad.FaceLoad.Position.X);
        Assert.Equal(0, original.FaceLoad.FaceLoad.Position.Y);
      }
    }

    [Fact]
    public void GridPointLoadDuplicateTest()
    {
      // Arrange
      GsaGridPointLoad gridPointLoad = new GsaGridPointLoad();
      gridPointLoad.GridPointLoad.AxisProperty = 5;
      gridPointLoad.GridPointLoad.Case = 6;
      gridPointLoad.GridPointLoad.Direction = Direction.ZZ;
      gridPointLoad.GridPointLoad.GridSurface = 7;
      gridPointLoad.GridPointLoad.Name = "name";
      gridPointLoad.GridPointLoad.X = 5;
      gridPointLoad.GridPointLoad.Y = 6;
      gridPointLoad.GridPointLoad.Value = 10;
      GsaLoad original = new GsaLoad(gridPointLoad);

      // Act
      GsaLoad duplicate = original.Duplicate();

      // Assert
      Duplicates.AreEqual(original, duplicate);

      // make some changes to duplicate
      duplicate.PointLoad.GridPointLoad.AxisProperty = 1;
      duplicate.PointLoad.GridPointLoad.Case = 1;
      duplicate.PointLoad.GridPointLoad.Direction = Direction.XX;
      duplicate.PointLoad.GridPointLoad.GridSurface = 1;
      duplicate.PointLoad.GridPointLoad.Name = "";
      duplicate.PointLoad.GridPointLoad.X = 0;
      duplicate.PointLoad.GridPointLoad.Y = 0;
      duplicate.PointLoad.GridPointLoad.Value = 0;

      // Assert
      Assert.Equal(LoadTypes.GridPoint, original.LoadType);
      Assert.Equal(5, original.PointLoad.GridPointLoad.AxisProperty);
      Assert.Equal(6, original.PointLoad.GridPointLoad.Case);
      Assert.Equal(Direction.ZZ, original.PointLoad.GridPointLoad.Direction);
      Assert.Equal(7, original.PointLoad.GridPointLoad.GridSurface);
      Assert.Equal("name", original.PointLoad.GridPointLoad.Name);
      Assert.Equal(5, original.PointLoad.GridPointLoad.X);
      Assert.Equal(6, original.PointLoad.GridPointLoad.Y);
      Assert.Equal(10, original.PointLoad.GridPointLoad.Value);
    }

    [Theory]
    [InlineData("EXPLICIT_POLYLINE")]
    [InlineData("POLYLINE_REFERENCE")]
    public void GridLineLoadDuplicateTest(string polyLineTypeString)
    {
      // Arrange
      PolyLineType type = (PolyLineType)Enum.Parse(typeof(PolyLineType), polyLineTypeString);

      GsaGridLineLoad gridLineLoad = new GsaGridLineLoad();
      gridLineLoad.GridLineLoad.AxisProperty = 5;
      gridLineLoad.GridLineLoad.Case = 6;
      gridLineLoad.GridLineLoad.Direction = Direction.ZZ;
      gridLineLoad.GridLineLoad.GridSurface = 7;
      gridLineLoad.GridLineLoad.IsProjected = true;
      gridLineLoad.GridLineLoad.Name = "name";
      gridLineLoad.GridLineLoad.PolyLineDefinition = ""; // insert valid definition here
      gridLineLoad.GridLineLoad.PolyLineReference = 1;
      gridLineLoad.GridLineLoad.Type = type;
      gridLineLoad.GridLineLoad.ValueAtStart = 10;
      gridLineLoad.GridLineLoad.ValueAtEnd = 20;
      GsaLoad original = new GsaLoad(gridLineLoad);

      // Act
      GsaLoad duplicate = original.Duplicate();

      // Assert
      Duplicates.AreEqual(original, duplicate);

      // make some changes to duplicate
      duplicate.LineLoad.GridLineLoad.AxisProperty = 1;
      duplicate.LineLoad.GridLineLoad.Case = 1;
      duplicate.LineLoad.GridLineLoad.Direction = Direction.XX;
      duplicate.LineLoad.GridLineLoad.GridSurface = 1;
      duplicate.LineLoad.GridLineLoad.IsProjected = false;
      duplicate.LineLoad.GridLineLoad.Name = "";
      duplicate.LineLoad.GridLineLoad.PolyLineDefinition = "";
      duplicate.LineLoad.GridLineLoad.PolyLineReference = 0;
      duplicate.LineLoad.GridLineLoad.Type = PolyLineType.EXPLICIT_POLYLINE;
      duplicate.LineLoad.GridLineLoad.ValueAtStart = 0;
      duplicate.LineLoad.GridLineLoad.ValueAtEnd = 0;

      // Assert
      Assert.Equal(LoadTypes.GridLine, original.LoadType);
      Assert.Equal(5, original.LineLoad.GridLineLoad.AxisProperty);
      Assert.Equal(6, original.LineLoad.GridLineLoad.Case);
      Assert.Equal(Direction.ZZ, original.LineLoad.GridLineLoad.Direction);
      Assert.Equal(7, original.LineLoad.GridLineLoad.GridSurface);
      Assert.True(original.LineLoad.GridLineLoad.IsProjected);
      Assert.Equal("name", original.LineLoad.GridLineLoad.Name);
      Assert.Equal("", original.LineLoad.GridLineLoad.PolyLineDefinition);
      Assert.Equal(1, original.LineLoad.GridLineLoad.PolyLineReference);
      Assert.Equal(type, original.LineLoad.GridLineLoad.Type);
      Assert.Equal(10, original.LineLoad.GridLineLoad.ValueAtStart);
      Assert.Equal(20, original.LineLoad.GridLineLoad.ValueAtEnd);
    }

    [Theory]
    [InlineData("PLANE")]
    [InlineData("POLYREF")]
    [InlineData("POLYGON")]
    public void GridAreaLoadDuplicateTest(string gridAreaPolyLineTypeString)
    {
      // Arrange
      GridAreaPolyLineType type = (GridAreaPolyLineType)Enum.Parse(typeof(GridAreaPolyLineType), gridAreaPolyLineTypeString);

      GsaGridAreaLoad gridAreaLoad = new GsaGridAreaLoad();
      gridAreaLoad.GridAreaLoad.AxisProperty = 5;
      gridAreaLoad.GridAreaLoad.Case = 6;
      gridAreaLoad.GridAreaLoad.Direction = Direction.ZZ;
      gridAreaLoad.GridAreaLoad.GridSurface = 7;
      gridAreaLoad.GridAreaLoad.IsProjected = true;
      gridAreaLoad.GridAreaLoad.Name = "name";
      gridAreaLoad.GridAreaLoad.PolyLineDefinition = ""; // insert valid definition here
      gridAreaLoad.GridAreaLoad.PolyLineReference = 1;
      gridAreaLoad.GridAreaLoad.Type = type;
      gridAreaLoad.GridAreaLoad.Value = 10;
      GsaGridPlaneSurface originalGridPlaneSurface = new GsaGridPlaneSurface();
      gridAreaLoad.GridPlaneSurface = originalGridPlaneSurface;
      GsaLoad original = new GsaLoad(gridAreaLoad);

      // Act
      GsaLoad duplicate = original.Duplicate();

      // Assert
      Duplicates.AreEqual(original, duplicate);

      // make some changes to duplicate
      duplicate.AreaLoad.GridAreaLoad.AxisProperty = 1;
      duplicate.AreaLoad.GridAreaLoad.Case = 1;
      duplicate.AreaLoad.GridAreaLoad.Direction = Direction.XX;
      duplicate.AreaLoad.GridAreaLoad.GridSurface = 1;
      duplicate.AreaLoad.GridAreaLoad.IsProjected = false;
      duplicate.AreaLoad.GridAreaLoad.Name = "";
      duplicate.AreaLoad.GridAreaLoad.PolyLineDefinition = "";
      duplicate.AreaLoad.GridAreaLoad.PolyLineReference = 0;
      duplicate.AreaLoad.GridAreaLoad.Type = GridAreaPolyLineType.POLYGON;
      duplicate.AreaLoad.GridAreaLoad.Value = 0;
      duplicate.AreaLoad.GridPlaneSurface = new GsaGridPlaneSurface(new Plane());

      // Assert
      Assert.Equal(LoadTypes.GridArea, original.LoadType);
      Assert.Equal(5, original.AreaLoad.GridAreaLoad.AxisProperty);
      Assert.Equal(6, original.AreaLoad.GridAreaLoad.Case);
      Assert.Equal(Direction.ZZ, original.AreaLoad.GridAreaLoad.Direction);
      Assert.Equal(7, original.AreaLoad.GridAreaLoad.GridSurface);
      Assert.True(original.AreaLoad.GridAreaLoad.IsProjected);
      Assert.Equal("name", original.AreaLoad.GridAreaLoad.Name);
      Assert.Equal("", original.AreaLoad.GridAreaLoad.PolyLineDefinition);
      Assert.Equal(1, original.AreaLoad.GridAreaLoad.PolyLineReference);
      Assert.Equal(type, original.AreaLoad.GridAreaLoad.Type);
      Assert.Equal(10, original.AreaLoad.GridAreaLoad.Value);
      Assert.Equal(originalGridPlaneSurface, original.AreaLoad.GridPlaneSurface);
    }
  }
}