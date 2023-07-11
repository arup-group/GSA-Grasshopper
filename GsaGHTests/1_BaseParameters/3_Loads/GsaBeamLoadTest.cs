using System;
using GsaAPI;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using Xunit;

namespace GsaGHTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  public class GsaBeamLoadTest {
    [Fact]
    public void ConstructorTest() {
      var load = new GsaBeamLoad();

      Assert.Equal(LoadType.Beam, load.LoadType);
    }

    [Theory]
    [InlineData("UNDEF", "UNIFORM")]
    [InlineData("POINT", "UNDEF")]
    [InlineData("UNIFORM", "UNDEF")]
    [InlineData("LINEAR", "UNDEF")]
    [InlineData("PATCH", "UNDEF")]
    [InlineData("TRILINEAR", "UNDEF")]
    public void DuplicateTest(string originalTypeString, string duplicateTypeString) {
      var originalType = (BeamLoadType)Enum.Parse(typeof(BeamLoadType), originalTypeString);
      var duplicateType = (BeamLoadType)Enum.Parse(typeof(BeamLoadType), duplicateTypeString);

      var original = new GsaBeamLoad {
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

      var duplicate = (GsaBeamLoad)original.Duplicate();

      Duplicates.AreEqual(original, duplicate);

      duplicate.BeamLoad.Type = duplicateType;
      duplicate.BeamLoad.AxisProperty = 1;
      duplicate.BeamLoad.Case = 1;
      duplicate.BeamLoad.Direction = Direction.XX;
      duplicate.BeamLoad.Elements = "";
      duplicate.BeamLoad.Name = "";
      duplicate.BeamLoad.IsProjected = false;
      duplicate.BeamLoad.SetPosition(0, 99);
      duplicate.BeamLoad.SetValue(0, 99);
      duplicate.BeamLoad.SetPosition(1, 99);
      duplicate.BeamLoad.SetValue(1, 99);

      Assert.Equal(LoadType.Beam, original.LoadType);
      Assert.Equal(originalType, original.BeamLoad.Type);
      Assert.Equal(5, original.BeamLoad.AxisProperty);
      Assert.Equal(6, original.BeamLoad.Case);
      Assert.Equal(Direction.ZZ, original.BeamLoad.Direction);
      Assert.Equal("all", original.BeamLoad.Elements);
      Assert.Equal("name", original.BeamLoad.Name);
      Assert.True(original.BeamLoad.IsProjected);
      switch (original.BeamLoad.Type) {
        case BeamLoadType.POINT:
          Assert.Equal(0, original.BeamLoad.Position(0));
          Assert.Equal(0, original.BeamLoad.Value(0));
          break;

        case BeamLoadType.UNIFORM:
          Assert.Equal(0, original.BeamLoad.Value(0));
          break;

        case BeamLoadType.LINEAR:
          Assert.Equal(0, original.BeamLoad.Position(0));
          Assert.Equal(0, original.BeamLoad.Value(1));
          break;

        case BeamLoadType.PATCH:
          Assert.Equal(0, original.BeamLoad.Position(0));
          Assert.Equal(0, original.BeamLoad.Position(1));
          Assert.Equal(0, original.BeamLoad.Value(0));
          Assert.Equal(0, original.BeamLoad.Value(1));
          break;

        case BeamLoadType.TRILINEAR:
          Assert.Equal(0, original.BeamLoad.Position(0));
          Assert.Equal(0, original.BeamLoad.Position(1));
          Assert.Equal(0, original.BeamLoad.Value(0));
          Assert.Equal(0, original.BeamLoad.Value(1));
          break;
      }
    }
  }
}
