using System;
using GsaAPI;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using Xunit;
using LoadCase = GsaGH.Parameters.Enums.LoadCase;

namespace GsaGHTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  public class GsaBeamLoadTest {
    [Fact]
    public void ConstructorTest() {
      var load = new GsaBeamLoad();

      Assert.Equal(LoadType.Beam, load.LoadType);
    }

    [Fact]
    public void LoadCaseTest() {
      var load = new GsaBeamLoad();
      Assert.Null(load.LoadCase);
      load.LoadCase = new GsaLoadCase(99);
      Assert.Equal(99, load.LoadCase.Id);
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
          EntityList = "all",
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
      duplicate.BeamLoad.EntityList = "";
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
      Assert.Equal("all", original.BeamLoad.EntityList);
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

    [Fact]
    public void DuplicateLoadCaseTest() {
      var load = new GsaBeamLoad();
      Assert.Null(load.LoadCase);
      var duplicate = (GsaBeamLoad)load.Duplicate();
      Assert.Null(duplicate.LoadCase);

      load.LoadCase = new GsaLoadCase(99);

      duplicate = (GsaBeamLoad)load.Duplicate();
      Assert.Equal(99, duplicate.LoadCase.Id);

      duplicate.LoadCase = new GsaLoadCase(1, LoadCase.LoadCaseType.Dead, "DeadLoad");
      Assert.Equal(99, load.LoadCase.Id);
      Assert.Equal(1, duplicate.LoadCase.Id);
      Assert.Equal("Dead", duplicate.LoadCase.LoadCase.CaseType.ToString());
      Assert.Equal("DeadLoad", duplicate.LoadCase.LoadCase.Name);
    }
  }
}
