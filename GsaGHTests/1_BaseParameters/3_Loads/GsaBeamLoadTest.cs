using System;

using GsaAPI;

using GsaGH.Parameters;

using GsaGHTests.Helpers;

using Xunit;

using LoadCaseType = GsaGH.Parameters.LoadCaseType;

namespace GsaGHTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  public class GsaBeamLoadTest {
    [Fact]
    public void LoadCaseTest() {
      var load = new GsaBeamLoad();
      Assert.Null(load.LoadCase);
      load.LoadCase = new GsaLoadCase(99);
      Assert.Equal(99, load.LoadCase.Id);
    }

    [Theory]
    [InlineData("POINT", "UNIFORM")]
    [InlineData("UNIFORM", "POINT")]
    [InlineData("LINEAR", "POINT")]
    [InlineData("PATCH", "POINT")]
    [InlineData("TRILINEAR", "POINT")]
    public void DuplicateTest(string originalTypeString, string duplicateTypeString) {
      var originalType = (BeamLoadType)Enum.Parse(typeof(BeamLoadType), originalTypeString);
      var duplicateType = (BeamLoadType)Enum.Parse(typeof(BeamLoadType), duplicateTypeString);

      var original = new GsaBeamLoad {
        ApiLoad = {
          Type = originalType,
          AxisProperty = 5,
          Case = 6,
          Direction = Direction.ZZ,
          EntityList = "all",
          EntityType = GsaAPI.EntityType.Element,
          Name = "name",
          IsProjected = true,
        },
      };

      var duplicate = (GsaBeamLoad)original.Duplicate();

      Duplicates.AreEqual(original, duplicate);

      duplicate.ApiLoad.Type = duplicateType;
      duplicate.ApiLoad.AxisProperty = 1;
      duplicate.ApiLoad.Case = 1;
      duplicate.ApiLoad.Direction = Direction.XX;
      duplicate.ApiLoad.EntityList = "";
      duplicate.ApiLoad.Name = "";
      duplicate.ApiLoad.IsProjected = false;
      duplicate.ApiLoad.SetPosition(0, 99);
      duplicate.ApiLoad.SetValue(0, 99);
      duplicate.ApiLoad.SetPosition(1, 99);
      duplicate.ApiLoad.SetValue(1, 99);

      Assert.Equal(originalType, original.ApiLoad.Type);
      Assert.Equal(5, original.ApiLoad.AxisProperty);
      Assert.Equal(6, original.ApiLoad.Case);
      Assert.Equal(Direction.ZZ, original.ApiLoad.Direction);
      Assert.Equal("all", original.ApiLoad.EntityList);
      Assert.Equal(GsaAPI.EntityType.Element, original.ApiLoad.EntityType);
      Assert.Equal("name", original.ApiLoad.Name);
      Assert.True(original.ApiLoad.IsProjected);
      switch (original.ApiLoad.Type) {
        case BeamLoadType.POINT:
          Assert.Equal(0, original.ApiLoad.Position(0));
          Assert.Equal(0, original.ApiLoad.Value(0));
          break;

        case BeamLoadType.UNIFORM:
          Assert.Equal(0, original.ApiLoad.Value(0));
          break;

        case BeamLoadType.LINEAR:
          Assert.Equal(0, original.ApiLoad.Position(0));
          Assert.Equal(0, original.ApiLoad.Value(1));
          break;

        case BeamLoadType.PATCH:
          Assert.Equal(0, original.ApiLoad.Position(0));
          Assert.Equal(0, original.ApiLoad.Position(1));
          Assert.Equal(0, original.ApiLoad.Value(0));
          Assert.Equal(0, original.ApiLoad.Value(1));
          break;

        case BeamLoadType.TRILINEAR:
          Assert.Equal(0, original.ApiLoad.Position(0));
          Assert.Equal(0, original.ApiLoad.Position(1));
          Assert.Equal(0, original.ApiLoad.Value(0));
          Assert.Equal(0, original.ApiLoad.Value(1));
          break;
      }
    }

    [Fact]
    public void DuplicateLoadCaseTest() {
      var load = new GsaBeamLoad();
      Assert.Null(load.LoadCase);
      var duplicate = (GsaBeamLoad)load.Duplicate();
      Assert.Null(duplicate.LoadCase);

      load.LoadCase = new GsaLoadCase(99);

      duplicate = (GsaBeamLoad)load.Duplicate();
      Assert.Equal(99, duplicate.LoadCase.Id);


      duplicate.LoadCase = new GsaLoadCase(1, LoadCaseType.Dead, "DeadLoad");

      Assert.Equal(99, load.LoadCase.Id);
      Assert.Equal(1, duplicate.LoadCase.Id);
      Assert.Equal("Dead", duplicate.LoadCase.LoadCase.CaseType.ToString());
      Assert.Equal("DeadLoad", duplicate.LoadCase.LoadCase.Name);
    }
  }
}
