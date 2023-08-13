using GsaAPI;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using System;
using Xunit;
using LoadCase = GsaGH.Parameters.Enums.LoadCase;

namespace GsaGHTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  public class GsaFaceLoadTest {
    [Fact]
    public void ConstructorTest() {
      var load = new GsaFaceLoad();

      Assert.Equal(LoadType.Face, load.LoadType);
      Assert.Equal(FaceLoadType.CONSTANT, load.FaceLoad.Type);
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
        FaceLoad = {
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
      Assert.Equal(GsaAPI.EntityType.Element, original.FaceLoad.EntityType);
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

    [Fact]
    public void DuplicateLoadCaseTest() {
      var load = new GsaFaceLoad();
      Assert.Null(load.LoadCase);
      var duplicate = (GsaFaceLoad)load.Duplicate();
      Assert.Null(duplicate.LoadCase);

      load.LoadCase = new GsaLoadCase(99);

      duplicate = (GsaFaceLoad)load.Duplicate();
      Assert.Equal(99, duplicate.LoadCase.Id);

      duplicate.LoadCase = new GsaLoadCase(1, LoadCase.LoadCaseType.Dead, "DeadLoad");
      Assert.Equal(99, load.LoadCase.Id);
      Assert.Equal(1, duplicate.LoadCase.Id);
      Assert.Equal("Dead", duplicate.LoadCase.LoadCase.CaseType.ToString());
      Assert.Equal("DeadLoad", duplicate.LoadCase.LoadCase.Name);
    }
  }
}
