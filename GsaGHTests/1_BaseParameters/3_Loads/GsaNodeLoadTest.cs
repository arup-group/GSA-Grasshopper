using GsaAPI;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using Xunit;

namespace GsaGHTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  public class GsaNodeLoadTest {
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public void ConstructorTest(int typeId) {
      var type = (NodeLoadType)typeId;
      var load = new GsaNodeLoad {
        Type = (GsaNodeLoad.NodeLoadType)type,
      };

      Assert.Equal(LoadType.Node, load.LoadType);
      Assert.Equal((NodeLoadType)type, (NodeLoadType)load.Type);
    }

    [Theory]
    [InlineData(0, 3)]
    [InlineData(1, 0)]
    [InlineData(2, 0)]
    [InlineData(3, 0)]
    [InlineData(4, 0)]
    public void DuplicateTest(int intType, int intDuplicateType) {
      var originalType = (GsaNodeLoad.NodeLoadType)intType;
      var duplicateType = (GsaNodeLoad.NodeLoadType)intDuplicateType;

      var original = new GsaNodeLoad {
        Type = (GsaNodeLoad.NodeLoadType)originalType,
        NodeLoad = {
          AxisProperty = 2,
          Case = 100,
          Direction = Direction.XY,
          Nodes = "all",
          Name = "name",
          Value = 97.5,
        },
      };

      var duplicate = (GsaNodeLoad)original.Duplicate();

      Duplicates.AreEqual(original, duplicate);

      duplicate.Type = duplicateType;
      duplicate.NodeLoad.AxisProperty = 3;
      duplicate.NodeLoad.Case = 99;
      duplicate.NodeLoad.Direction = Direction.YY;
      duplicate.NodeLoad.Nodes = "";
      duplicate.NodeLoad.Name = "";
      duplicate.NodeLoad.Value = -3.3;

      Assert.Equal(LoadType.Node, original.LoadType);
      Assert.Equal(originalType, original.Type);
      Assert.Equal(2, original.NodeLoad.AxisProperty);
      Assert.Equal(100, original.NodeLoad.Case);
      Assert.Equal(Direction.XY, original.NodeLoad.Direction);
      Assert.Equal("all", original.NodeLoad.Nodes);
      Assert.Equal("name", original.NodeLoad.Name);
      Assert.Equal(97.5, original.NodeLoad.Value);
    }

    [Fact]
    public void NodeLoadEmptyConstructorTest() {
      var load = new GsaNodeLoad();

      Assert.Equal(LoadType.Node, load.LoadType);
      Assert.Equal(GsaNodeLoad.NodeLoadType.NodeLoad, load.Type);
    }
  }
}
