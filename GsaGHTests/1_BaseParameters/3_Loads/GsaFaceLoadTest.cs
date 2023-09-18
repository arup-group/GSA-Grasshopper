using GsaAPI;
using GsaGH.Parameters;
using GsaGH.Parameters.Enums;
using GsaGHTests.Helpers;
using System;
using Xunit;
using LoadCaseType = GsaGH.Parameters.LoadCaseType;

namespace GsaGHTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  public class GsaFaceLoadTest {
    [Fact]
    public void ConstructorTest() {
      var load = new GsaFaceLoad();

      Assert.Equal(FaceLoadType.CONSTANT, load.ApiLoad.Type);
    }

    [Fact]
    public void LoadCaseTest() {
      var load = new GsaFaceLoad();
      Assert.Null(load.LoadCase);
      load.LoadCase = new GsaLoadCase(99);
      Assert.Equal(99, load.LoadCase.Id);
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
        ApiLoad = {
          AxisProperty = 5,
          Case = 6,
          Direction = Direction.ZZ,
          EntityList = "all",
          EntityType = GsaAPI.EntityType.Element,
          Name = "name",
          Type = originalType,
        },
      };
      var duplicate = (GsaFaceLoad)original.Duplicate();

      Duplicates.AreEqual(original, duplicate);

      duplicate.ApiLoad.Type = duplicateType;
      duplicate.ApiLoad.AxisProperty = 1;
      duplicate.ApiLoad.Case = 1;
      duplicate.ApiLoad.Direction = Direction.XX;
      duplicate.ApiLoad.EntityList = "";
      duplicate.ApiLoad.Name = "";
      duplicate.ApiLoad.IsProjected = true;
      duplicate.ApiLoad.SetValue(0, 99);
      duplicate.ApiLoad.SetValue(1, 99);
      duplicate.ApiLoad.SetValue(2, 99);
      duplicate.ApiLoad.SetValue(3, 99);

      Assert.Equal(originalType, original.ApiLoad.Type);
      Assert.Equal(5, original.ApiLoad.AxisProperty);
      Assert.Equal(6, original.ApiLoad.Case);
      Assert.Equal(Direction.ZZ, original.ApiLoad.Direction);
      Assert.Equal("all", original.ApiLoad.EntityList);
      Assert.Equal(GsaAPI.EntityType.Element, original.ApiLoad.EntityType);
      Assert.Equal("name", original.ApiLoad.Name);

      switch (original.ApiLoad.Type) {
        case FaceLoadType.CONSTANT:
          Assert.False(original.ApiLoad.IsProjected);
          Assert.Equal(0, original.ApiLoad.Value(0));
          break;

        case FaceLoadType.GENERAL:
          Assert.False(original.ApiLoad.IsProjected);
          Assert.Equal(0, original.ApiLoad.Value(0));
          Assert.Equal(0, original.ApiLoad.Value(1));
          Assert.Equal(0, original.ApiLoad.Value(2));
          Assert.Equal(0, original.ApiLoad.Value(3));
          break;

        case FaceLoadType.POINT:
          Assert.False(original.ApiLoad.IsProjected);
          Assert.Equal(0, original.ApiLoad.Value(0));
          Assert.Equal(0, original.ApiLoad.Position.X);
          Assert.Equal(0, original.ApiLoad.Position.Y);
          break;
      }
    }

    [Fact]
    public void DuplicateLoadCaseTest() {
      var load = new GsaFaceLoad();
      Assert.Null(load.LoadCase);
      var duplicate = (GsaFaceLoad)load.Duplicate();
      Assert.Null(duplicate.LoadCase);

      load.LoadCase = new GsaLoadCase(99);

      duplicate = (GsaFaceLoad)load.Duplicate();
      Assert.Equal(99, duplicate.LoadCase.Id);

      duplicate.LoadCase = new GsaLoadCase(1, LoadCaseType.Dead, "DeadLoad");

      Assert.Equal(99, load.LoadCase.Id);
      Assert.Equal(1, duplicate.LoadCase.Id);
      Assert.Equal("Dead", duplicate.LoadCase.LoadCase.CaseType.ToString());
      Assert.Equal("DeadLoad", duplicate.LoadCase.LoadCase.Name);
    }
  }
}
