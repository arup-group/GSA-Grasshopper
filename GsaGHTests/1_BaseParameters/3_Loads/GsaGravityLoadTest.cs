using GsaAPI;

using GsaGH.Parameters;

using GsaGHTests.Helpers;

using Xunit;

namespace GsaGHTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  public class GsaGravityLoadTest {
    [Fact]
    public void ConstructorTest() {
      var load = new GsaGravityLoad();

      Assert.Equal(0, load.ApiLoad.Factor.X);
      Assert.Equal(0, load.ApiLoad.Factor.Y);
      Assert.Equal(-1, load.ApiLoad.Factor.Z);
      Assert.Equal(1, load.ApiLoad.Case);
      Assert.Equal("all", load.ApiLoad.EntityList);
      Assert.Equal(GsaAPI.EntityType.Element, load.ApiLoad.EntityType);
      Assert.Equal("all", load.ApiLoad.Nodes);
    }

    [Fact]
    public void LoadCaseTest() {
      var load = new GsaGravityLoad();
      Assert.Null(load.LoadCase);
      load.LoadCase = new GsaLoadCase(99);
      Assert.Equal(99, load.LoadCase.Id);
    }

    [Fact]
    public void DuplicateTest() {
      var original = new GsaGravityLoad {
        ApiLoad = {
          Name = "name",
        },
      };
      var duplicate = (GsaGravityLoad)original.Duplicate();

      Duplicates.AreEqual(original, duplicate);

      duplicate.ApiLoad.Factor = new Vector3() {
        X = 1,
        Y = 1,
        Z = 1,
      };
      duplicate.ApiLoad.Case = 3;
      duplicate.ApiLoad.EntityList = "";
      duplicate.ApiLoad.EntityType = GsaAPI.EntityType.Member;
      duplicate.ApiLoad.Nodes = "";
      duplicate.ApiLoad.Name = "";

      Assert.Equal(0, original.ApiLoad.Factor.X);
      Assert.Equal(0, original.ApiLoad.Factor.Y);
      Assert.Equal(-1, original.ApiLoad.Factor.Z);
      Assert.Equal(1, original.ApiLoad.Case);
      Assert.Equal("all", original.ApiLoad.EntityList);
      Assert.Equal(GsaAPI.EntityType.Element, original.ApiLoad.EntityType);
      Assert.Equal("all", original.ApiLoad.Nodes);
      Assert.Equal("name", original.ApiLoad.Name);
    }

    [Fact]
    public void DuplicateLoadCaseTest() {
      var load = new GsaGravityLoad();
      Assert.Null(load.LoadCase);
      var duplicate = (GsaGravityLoad)load.Duplicate();
      Assert.Null(duplicate.LoadCase);

      load.LoadCase = new GsaLoadCase(99);

      duplicate = (GsaGravityLoad)load.Duplicate();
      Assert.Equal(99, duplicate.LoadCase.Id);

      duplicate.LoadCase = new GsaLoadCase(1, GsaGH.Parameters.LoadCaseType.Dead, "DeadLoad");

      Assert.Equal(99, load.LoadCase.Id);
      Assert.Equal(1, duplicate.LoadCase.Id);
      Assert.Equal("Dead", duplicate.LoadCase.LoadCase.CaseType.ToString());
      Assert.Equal("DeadLoad", duplicate.LoadCase.LoadCase.Name);
    }
  }
}
