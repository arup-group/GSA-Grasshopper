using System;
using GsaAPI;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using Rhino.Geometry;
using Xunit;
using static GsaAPI.GridLineLoad;
using static GsaGH.Parameters.GsaLoad;
using static GsaGH.Parameters.GsaNodeLoad;

namespace GsaGHTests.Parameters {

  [Collection("GrasshopperFixture collection")]
  public class GsaLoadTest {

    #region Public Methods
    [Fact]
    public void BeamLoadConstructorTest() {
      var beamLoad = new GsaBeamLoad();
      var load = new GsaLoad(beamLoad);

      Assert.Equal(LoadTypes.Beam, load.LoadType);
    }

    [Theory]
    [InlineData("UNDEF", "UNIFORM")]
    [InlineData("POINT", "UNDEF")]
    [InlineData("UNIFORM", "UNDEF")]
    [InlineData("LINEAR", "UNDEF")]
    [InlineData("PATCH", "UNDEF")]
    [InlineData("TRILINEAR", "UNDEF")]
    public void BeamLoadDuplicateTest(string originalTypeString, string duplicateTypeString) {
      var originalType = (BeamLoadType)Enum.Parse(typeof(BeamLoadType), originalTypeString);
      var duplicateType = (BeamLoadType)Enum.Parse(typeof(BeamLoadType), duplicateTypeString);

      var beamLoad = new GsaBeamLoad {
        BeamLoad = {
          Type = originalType,
          AxisProperty = 5,
          Case = 6,
          Direction = Direction.ZZ,
          Elements = "all",
          Name = "name",
          IsProjected = true,
        },
      };
      beamLoad.BeamLoad.IsProjected = true;
      var original = new GsaLoad(beamLoad);

      GsaLoad duplicate = original.Duplicate();

      Duplicates.AreEqual(original, duplicate);

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

      Assert.Equal(LoadTypes.Beam, original.LoadType);
      Assert.Equal(originalType, original.BeamLoad.BeamLoad.Type);
      Assert.Equal(5, original.BeamLoad.BeamLoad.AxisProperty);
      Assert.Equal(6, original.BeamLoad.BeamLoad.Case);
      Assert.Equal(Direction.ZZ, original.BeamLoad.BeamLoad.Direction);
      Assert.Equal("all", original.BeamLoad.BeamLoad.Elements);
      Assert.Equal("name", original.BeamLoad.BeamLoad.Name);
      Assert.True(original.BeamLoad.BeamLoad.IsProjected);
      switch (original.BeamLoad.BeamLoad.Type) {
        case BeamLoadType.POINT:
          Assert.Equal(0, original.BeamLoad.BeamLoad.Position(0));
          Assert.Equal(0, original.BeamLoad.BeamLoad.Value(0));
          break;

        case BeamLoadType.UNIFORM:
          Assert.Equal(0, original.BeamLoad.BeamLoad.Value(0));
          break;

        case BeamLoadType.LINEAR:
          Assert.Equal(0, original.BeamLoad.BeamLoad.Position(0));
          Assert.Equal(0, original.BeamLoad.BeamLoad.Value(1));
          break;

        case BeamLoadType.PATCH:
          Assert.Equal(0, original.BeamLoad.BeamLoad.Position(0));
          Assert.Equal(0, original.BeamLoad.BeamLoad.Position(1));
          Assert.Equal(0, original.BeamLoad.BeamLoad.Value(0));
          Assert.Equal(0, original.BeamLoad.BeamLoad.Value(1));
          break;

        case BeamLoadType.TRILINEAR:
          Assert.Equal(0, original.BeamLoad.BeamLoad.Position(0));
          Assert.Equal(0, original.BeamLoad.BeamLoad.Position(1));
          Assert.Equal(0, original.BeamLoad.BeamLoad.Value(0));
          Assert.Equal(0, original.BeamLoad.BeamLoad.Value(1));
          break;
      }
    }

    [Fact]
    public void ConstructorTest() {
      var load = new GsaLoad();

      Assert.Equal(LoadTypes.Gravity, load.LoadType);
      Assert.Equal(0, load.GravityLoad.GravityLoad.Factor.X);
      Assert.Equal(0, load.GravityLoad.GravityLoad.Factor.Y);
      Assert.Equal(-1, load.GravityLoad.GravityLoad.Factor.Z);
      Assert.Equal(1, load.GravityLoad.GravityLoad.Case);
      Assert.Equal("all", load.GravityLoad.GravityLoad.Elements);
      Assert.Equal("all", load.GravityLoad.GravityLoad.Nodes);
    }

    [Fact]
    public void DuplicateTest() {
      var original = new GsaLoad();

      GsaLoad duplicate = original.Duplicate();

      Duplicates.AreEqual(original, duplicate);

      duplicate.LoadType = LoadTypes.GridLine;
      duplicate.GravityLoad.GravityLoad.Factor = new Vector3() {
        X = 1,
        Y = 1,
        Z = 1,
      };
      duplicate.GravityLoad.GravityLoad.Case = 3;
      duplicate.GravityLoad.GravityLoad.Elements = "";
      duplicate.GravityLoad.GravityLoad.Nodes = "";

      Assert.Equal(LoadTypes.Gravity, original.LoadType);
      Assert.Equal(0, original.GravityLoad.GravityLoad.Factor.X);
      Assert.Equal(0, original.GravityLoad.GravityLoad.Factor.Y);
      Assert.Equal(-1, original.GravityLoad.GravityLoad.Factor.Z);
      Assert.Equal(1, original.GravityLoad.GravityLoad.Case);
      Assert.Equal("all", original.GravityLoad.GravityLoad.Elements);
      Assert.Equal("all", original.GravityLoad.GravityLoad.Nodes);
    }

    [Fact]
    public void FaceLoadConstructorTest() {
      var faceLoad = new GsaFaceLoad();
      var load = new GsaLoad(faceLoad);

      Assert.Equal(LoadTypes.Face, load.LoadType);
      Assert.Equal(FaceLoadType.CONSTANT, load.FaceLoad.FaceLoad.Type);
    }

    [Theory]
    [InlineData("UNDEF", "CONSTANT")]
    [InlineData("CONSTANT", "UNDEF")]
    [InlineData("GENERAL", "UNDEF")]
    [InlineData("POINT", "UNDEF")]
    public void FaceLoadDuplicateTest(string originalTypeString, string duplicateTypeString) {
      var originalType = (FaceLoadType)Enum.Parse(typeof(FaceLoadType), originalTypeString);
      var duplicateType = (FaceLoadType)Enum.Parse(typeof(FaceLoadType), duplicateTypeString);

      var faceLoad = new GsaFaceLoad {
        FaceLoad = {
          AxisProperty = 5,
          Case = 6,
          Direction = Direction.ZZ,
          Elements = "all",
          Name = "name",
          Type = originalType,
        },
      };
      var original = new GsaLoad(faceLoad);

      GsaLoad duplicate = original.Duplicate();

      Duplicates.AreEqual(original, duplicate);

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

      Assert.Equal(LoadTypes.Face, original.LoadType);
      Assert.Equal(originalType, original.FaceLoad.FaceLoad.Type);
      Assert.Equal(5, original.FaceLoad.FaceLoad.AxisProperty);
      Assert.Equal(6, original.FaceLoad.FaceLoad.Case);
      Assert.Equal(Direction.ZZ, original.FaceLoad.FaceLoad.Direction);
      Assert.Equal("all", original.FaceLoad.FaceLoad.Elements);
      Assert.Equal("name", original.FaceLoad.FaceLoad.Name);

      switch (original.FaceLoad.FaceLoad.Type) {
        case FaceLoadType.CONSTANT:
          Assert.False(original.FaceLoad.FaceLoad.IsProjected);
          Assert.Equal(0, original.FaceLoad.FaceLoad.Value(0));
          break;

        case FaceLoadType.GENERAL:
          Assert.False(original.FaceLoad.FaceLoad.IsProjected);
          Assert.Equal(0, original.FaceLoad.FaceLoad.Value(0));
          Assert.Equal(0, original.FaceLoad.FaceLoad.Value(1));
          Assert.Equal(0, original.FaceLoad.FaceLoad.Value(2));
          Assert.Equal(0, original.FaceLoad.FaceLoad.Value(3));
          break;

        case FaceLoadType.POINT:
          Assert.False(original.FaceLoad.FaceLoad.IsProjected);
          Assert.Equal(0, original.FaceLoad.FaceLoad.Value(0));
          Assert.Equal(0, original.FaceLoad.FaceLoad.Position.X);
          Assert.Equal(0, original.FaceLoad.FaceLoad.Position.Y);
          break;
      }
    }

    [Fact]
    public void GravityLoadConstructorTest() {
      var load = new GsaLoad(new GsaGravityLoad());

      Assert.Equal(LoadTypes.Gravity, load.LoadType);
      Assert.Equal(0, load.GravityLoad.GravityLoad.Factor.X);
      Assert.Equal(0, load.GravityLoad.GravityLoad.Factor.Y);
      Assert.Equal(-1, load.GravityLoad.GravityLoad.Factor.Z);
      Assert.Equal(1, load.GravityLoad.GravityLoad.Case);
      Assert.Equal("all", load.GravityLoad.GravityLoad.Elements);
      Assert.Equal("all", load.GravityLoad.GravityLoad.Nodes);
    }

    [Fact]
    public void GravityLoadDuplicateTest() {
      var gravityLoad = new GsaGravityLoad {
        GravityLoad = {
          Name = "name",
        },
      };
      var original = new GsaLoad(gravityLoad);

      GsaLoad duplicate = original.Duplicate();

      Duplicates.AreEqual(original, duplicate);

      duplicate.LoadType = LoadTypes.GridLine;
      duplicate.GravityLoad.GravityLoad.Factor = new Vector3() {
        X = 1,
        Y = 1,
        Z = 1,
      };
      duplicate.GravityLoad.GravityLoad.Case = 3;
      duplicate.GravityLoad.GravityLoad.Elements = "";
      duplicate.GravityLoad.GravityLoad.Nodes = "";
      duplicate.GravityLoad.GravityLoad.Name = "";

      Assert.Equal(LoadTypes.Gravity, original.LoadType);
      Assert.Equal(0, original.GravityLoad.GravityLoad.Factor.X);
      Assert.Equal(0, original.GravityLoad.GravityLoad.Factor.Y);
      Assert.Equal(-1, original.GravityLoad.GravityLoad.Factor.Z);
      Assert.Equal(1, original.GravityLoad.GravityLoad.Case);
      Assert.Equal("all", original.GravityLoad.GravityLoad.Elements);
      Assert.Equal("all", original.GravityLoad.GravityLoad.Nodes);
      Assert.Equal("name", original.GravityLoad.GravityLoad.Name);
    }

    [Fact]
    public void GridAreaLoadConstructorTest() {
      var gridAreaLoad = new GsaGridAreaLoad();
      var load = new GsaLoad(gridAreaLoad);

      Assert.Equal(LoadTypes.GridArea, load.LoadType);
      Assert.Equal(GridAreaPolyLineType.PLANE, load.AreaLoad.GridAreaLoad.Type);
    }

    [Theory]
    [InlineData("PLANE")]
    [InlineData("POLYREF")]
    [InlineData("POLYGON")]
    public void GridAreaLoadDuplicateTest(string gridAreaPolyLineTypeString) {
      var type = (GridAreaPolyLineType)Enum.Parse(typeof(GridAreaPolyLineType),
        gridAreaPolyLineTypeString);

      var gridAreaLoad = new GsaGridAreaLoad {
        GridAreaLoad = {
          AxisProperty = 5,
          Case = 6,
          Direction = Direction.ZZ,
          GridSurface = 7,
          IsProjected = true,
          Name = "name",
          PolyLineDefinition = "",
          PolyLineReference = 1,
          Type = type,
          Value = 10,
        },
      };
      var originalGridPlaneSurface = new GsaGridPlaneSurface();
      gridAreaLoad.GridPlaneSurface = originalGridPlaneSurface;
      var original = new GsaLoad(gridAreaLoad);

      GsaLoad duplicate = original.Duplicate();

      Duplicates.AreEqual(original, duplicate);

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

    [Fact]
    public void GridLineLoadConstructorTest() {
      var gridLineLoad = new GsaGridLineLoad();
      var load = new GsaLoad(gridLineLoad);

      Assert.Equal(LoadTypes.GridLine, load.LoadType);
      Assert.Equal(0, load.LineLoad.GridLineLoad.PolyLineReference);
    }

    [Theory]
    [InlineData("EXPLICIT_POLYLINE")]
    [InlineData("POLYLINE_REFERENCE")]
    public void GridLineLoadDuplicateTest(string polyLineTypeString) {
      var type = (PolyLineType)Enum.Parse(typeof(PolyLineType), polyLineTypeString);

      var gridLineLoad = new GsaGridLineLoad {
        GridLineLoad = {
          AxisProperty = 5,
          Case = 6,
          Direction = Direction.ZZ,
          GridSurface = 7,
          IsProjected = true,
          Name = "name",
          PolyLineDefinition = "",
          PolyLineReference = 1,
          Type = type,
          ValueAtStart = 10,
          ValueAtEnd = 20,
        },
      };
      var original = new GsaLoad(gridLineLoad);

      GsaLoad duplicate = original.Duplicate();

      Duplicates.AreEqual(original, duplicate);

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

    [Fact]
    public void GridPointLoadConstructorTest() {
      var gridPointLoad = new GsaGridPointLoad();
      var load = new GsaLoad(gridPointLoad);

      Assert.Equal(LoadTypes.GridPoint, load.LoadType);
    }

    [Fact]
    public void GridPointLoadDuplicateTest() {
      var gridPointLoad = new GsaGridPointLoad {
        GridPointLoad = {
          AxisProperty = 5,
          Case = 6,
          Direction = Direction.ZZ,
          GridSurface = 7,
          Name = "name",
          X = 5,
          Y = 6,
          Value = 10,
        },
      };
      var original = new GsaLoad(gridPointLoad);

      GsaLoad duplicate = original.Duplicate();

      Duplicates.AreEqual(original, duplicate);

      duplicate.PointLoad.GridPointLoad.AxisProperty = 1;
      duplicate.PointLoad.GridPointLoad.Case = 1;
      duplicate.PointLoad.GridPointLoad.Direction = Direction.XX;
      duplicate.PointLoad.GridPointLoad.GridSurface = 1;
      duplicate.PointLoad.GridPointLoad.Name = "";
      duplicate.PointLoad.GridPointLoad.X = 0;
      duplicate.PointLoad.GridPointLoad.Y = 0;
      duplicate.PointLoad.GridPointLoad.Value = 0;

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
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public void NodeLoadConstructorTest(int typeId) {
      var type = (NodeLoadTypes)typeId;
      var nodeLoad = new GsaNodeLoad {
        Type = type,
      };
      var load = new GsaLoad(nodeLoad);

      Assert.Equal(LoadTypes.Node, load.LoadType);
      Assert.Equal(type, load.NodeLoad.Type);
    }

    [Theory]
    [InlineData(0, 3)]
    [InlineData(1, 0)]
    [InlineData(2, 0)]
    [InlineData(3, 0)]
    [InlineData(4, 0)]
    public void NodeLoadDuplicateTest(int intType, int intDuplicateType) {
      var originalType = (NodeLoadTypes)intType;
      var duplicateType = (NodeLoadTypes)intDuplicateType;

      var nodeLoad = new GsaNodeLoad {
        Type = originalType,
        NodeLoad = {
          AxisProperty = 2,
          Case = 100,
          Direction = Direction.XY,
          Nodes = "all",
          Name = "name",
          Value = 97.5,
        },
      };
      var original = new GsaLoad(nodeLoad);

      GsaLoad duplicate = original.Duplicate();

      Duplicates.AreEqual(original, duplicate);

      duplicate.NodeLoad.Type = duplicateType;
      duplicate.NodeLoad.NodeLoad.AxisProperty = 3;
      duplicate.NodeLoad.NodeLoad.Case = 99;
      duplicate.NodeLoad.NodeLoad.Direction = Direction.YY;
      duplicate.NodeLoad.NodeLoad.Nodes = "";
      duplicate.NodeLoad.NodeLoad.Name = "";
      duplicate.NodeLoad.NodeLoad.Value = -3.3;

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
    public void NodeLoadEmptyConstructorTest() {
      var nodeLoad = new GsaNodeLoad();
      var load = new GsaLoad(nodeLoad);

      Assert.Equal(LoadTypes.Node, load.LoadType);
      Assert.Equal(NodeLoadTypes.NodeLoad, load.NodeLoad.Type);
    }

    #endregion Public Methods
  }
}
