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

      Assert.Equal(LoadType.Gravity, load.LoadType);
      Assert.Equal(0, load.GravityLoad.Factor.X);
      Assert.Equal(0, load.GravityLoad.Factor.Y);
      Assert.Equal(-1, load.GravityLoad.Factor.Z);
      Assert.Equal(1, load.GravityLoad.Case);
      Assert.Equal("all", load.GravityLoad.EntityList);
      Assert.Equal(GsaAPI.EntityType.Element, load.GravityLoad.EntityType);
      Assert.Equal("all", load.GravityLoad.Nodes);
    }

    [Fact]
    public void DuplicateTest() {
      var original = new GsaGravityLoad {
        GravityLoad = {
          Name = "name",
        },
      };
      var duplicate = (GsaGravityLoad)original.Duplicate();

      Duplicates.AreEqual(original, duplicate);

      duplicate.GravityLoad.Factor = new Vector3() {
        X = 1,
        Y = 1,
        Z = 1,
      };
      duplicate.GravityLoad.Case = 3;
      duplicate.GravityLoad.EntityList = "";
      duplicate.GravityLoad.EntityType = GsaAPI.EntityType.Member;
      duplicate.GravityLoad.Nodes = "";
      duplicate.GravityLoad.Name = "";

      Assert.Equal(LoadType.Gravity, original.LoadType);
      Assert.Equal(0, original.GravityLoad.Factor.X);
      Assert.Equal(0, original.GravityLoad.Factor.Y);
      Assert.Equal(-1, original.GravityLoad.Factor.Z);
      Assert.Equal(1, original.GravityLoad.Case);
      Assert.Equal("all", original.GravityLoad.EntityList);
      Assert.Equal(GsaAPI.EntityType.Element, original.GravityLoad.EntityType);
      Assert.Equal("all", original.GravityLoad.Nodes);
      Assert.Equal("name", original.GravityLoad.Name);
    }
  }
}
