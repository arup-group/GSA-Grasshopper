using GsaAPI;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using System;
using Xunit;

namespace GsaGHTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  public class GsaFaceLoadTest {
    [Fact]
    public void ConstructorTest() {
      var load = new GsaFaceLoad();

      Assert.Equal(LoadType.Face, load.LoadType);
      Assert.Equal(FaceLoadType.CONSTANT, load.FaceLoad.Type);
    }

    [Theory]
    [InlineData("UNDEF", "CONSTANT")]
    [InlineData("CONSTANT", "UNDEF")]
    [InlineData("GENERAL", "UNDEF")]
    [InlineData("POINT", "UNDEF")]
    public void DuplicateTest(string originalTypeString, string duplicateTypeString) {
      var originalType = (FaceLoadType)Enum.Parse(typeof(FaceLoadType), originalTypeString);
      var duplicateType = (FaceLoadType)Enum.Parse(typeof(FaceLoadType), duplicateTypeString);

      var original = new GsaFaceLoad {
        FaceLoad = {
          AxisProperty = 5,
          Case = 6,
          Direction = Direction.ZZ,
          EntityList = "all",
          Name = "name",
          Type = originalType,
        },
      };
      var duplicate = (GsaFaceLoad)original.Duplicate();

      Duplicates.AreEqual(original, duplicate);

      duplicate.FaceLoad.Type = duplicateType;
      duplicate.FaceLoad.AxisProperty = 1;
      duplicate.FaceLoad.Case = 1;
      duplicate.FaceLoad.Direction = Direction.XX;
      duplicate.FaceLoad.EntityList = "";
      duplicate.FaceLoad.Name = "";
      duplicate.FaceLoad.IsProjected = true;
      duplicate.FaceLoad.SetValue(0, 99);
      duplicate.FaceLoad.SetValue(1, 99);
      duplicate.FaceLoad.SetValue(2, 99);
      duplicate.FaceLoad.SetValue(3, 99);

      Assert.Equal(LoadType.Face, original.LoadType);
      Assert.Equal(originalType, original.FaceLoad.Type);
      Assert.Equal(5, original.FaceLoad.AxisProperty);
      Assert.Equal(6, original.FaceLoad.Case);
      Assert.Equal(Direction.ZZ, original.FaceLoad.Direction);
      Assert.Equal("all", original.FaceLoad.EntityList);
      Assert.Equal("name", original.FaceLoad.Name);

      switch (original.FaceLoad.Type) {
        case FaceLoadType.CONSTANT:
          Assert.False(original.FaceLoad.IsProjected);
          Assert.Equal(0, original.FaceLoad.Value(0));
          break;

        case FaceLoadType.GENERAL:
          Assert.False(original.FaceLoad.IsProjected);
          Assert.Equal(0, original.FaceLoad.Value(0));
          Assert.Equal(0, original.FaceLoad.Value(1));
          Assert.Equal(0, original.FaceLoad.Value(2));
          Assert.Equal(0, original.FaceLoad.Value(3));
          break;

        case FaceLoadType.POINT:
          Assert.False(original.FaceLoad.IsProjected);
          Assert.Equal(0, original.FaceLoad.Value(0));
          Assert.Equal(0, original.FaceLoad.Position.X);
          Assert.Equal(0, original.FaceLoad.Position.Y);
          break;
      }
    }
  }
}
