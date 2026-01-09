using GsaAPI;

using GsaGH.Parameters;

using GsaGHTests.Helpers;

using Xunit;

using LoadCaseType = GsaGH.Parameters.LoadCaseType;
using NodeLoadType = GsaGH.Parameters.NodeLoadType;
using GsaGH.Helpers;
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
        Type = type,
      };

      Assert.Equal(type, load.Type);
    }

    [Fact]
    public void LoadCaseTest() {
      var load = new GsaNodeLoad();
      Assert.Null(load.LoadCase);
      load.LoadCase = new GsaLoadCase(99);
      Assert.Equal(99, load.LoadCase.Id);
    }

    [Theory]
    [InlineData(0, 3)]
    [InlineData(1, 0)]
    [InlineData(2, 0)]
    [InlineData(3, 0)]
    [InlineData(4, 0)]
    public void DuplicateTest(int intType, int intDuplicateType) {
      var originalType = (NodeLoadType)intType;
      var duplicateType = (NodeLoadType)intDuplicateType;

      var original = new GsaNodeLoad {
        Type = originalType,
        ApiLoad = {
          AxisProperty = 2,
          Case = 100,
          Direction = Direction.X,
          Nodes = "all",
          Name = "name",
          Value = 97.5,
        },
      };

      var duplicate = (GsaNodeLoad)original.Duplicate();

      Duplicates.AreEqual(original, duplicate);

      duplicate.Type = duplicateType;
      duplicate.ApiLoad.AxisProperty = 3;
      duplicate.ApiLoad.Case = 99;
      duplicate.ApiLoad.Direction = Direction.Y;
      duplicate.ApiLoad.Nodes = "";
      duplicate.ApiLoad.Name = "";
      duplicate.ApiLoad.Value = -3.3;

      Assert.Equal(originalType, original.Type);
      Assert.Equal(2, original.ApiLoad.AxisProperty);
      Assert.Equal(100, original.ApiLoad.Case);
      Assert.Equal(Direction.X, original.ApiLoad.Direction);
      Assert.Equal("all", original.ApiLoad.Nodes);
      Assert.Equal("name", original.ApiLoad.Name);
      Assert.Equal(97.5, original.ApiLoad.Value, DoubleComparer.Default);
    }

    [Fact]
    public void DuplicateLoadCaseTest() {
      var load = new GsaNodeLoad();
      Assert.Null(load.LoadCase);
      var duplicate = (GsaNodeLoad)load.Duplicate();
      Assert.Null(duplicate.LoadCase);

      load.LoadCase = new GsaLoadCase(99);

      duplicate = (GsaNodeLoad)load.Duplicate();
      Assert.Equal(99, duplicate.LoadCase.Id);

      duplicate.LoadCase = new GsaLoadCase(1, LoadCaseType.Dead, "DeadLoad");

      Assert.Equal(99, load.LoadCase.Id);
      Assert.Equal(1, duplicate.LoadCase.Id);
      Assert.Equal("Dead", duplicate.LoadCase.LoadCase.CaseType.ToString());
      Assert.Equal("DeadLoad", duplicate.LoadCase.LoadCase.Name);
    }

    [Fact]
    public void NodeLoadEmptyConstructorTest() {
      var load = new GsaNodeLoad();

      Assert.Equal(NodeLoadType.NodeLoad, load.Type);
    }
  }
}
