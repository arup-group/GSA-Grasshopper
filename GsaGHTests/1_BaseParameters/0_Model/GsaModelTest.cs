using System;
using System.IO;
using GsaAPI;
using GsaGH.Helpers.Export;
using GsaGH.Parameters;
using GsaGHTests.Helper;
using GsaGHTests.Helpers;
using OasysUnits;
using OasysUnits.Units;
using Rhino.Geometry;
using Xunit;

namespace GsaGHTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  public class GsaModelTest {
    [Fact]
    public void GsaModelEqualsTest() {
      var original = new GsaModel();
      GsaModel duplicate = original.Duplicate();

      Duplicates.AreEqual(original, duplicate);
    }

    [Fact]
    public void TestSaveModel() {
      var m = new GsaModel();
      string file = GsaFile.Steel_Design_Simple;
      m.Model.Open(file);

      string tempfilename = Path.GetTempPath() + "GSA-Grasshopper_temp.gwb";
      ReturnValue returnValue = m.Model.SaveAs(tempfilename);

      Assert.Same(ReturnValue.GS_OK.ToString(), returnValue.ToString());
    }

    [Fact]
    public void TestDuplicateModel() {
      var m = new GsaModel();

      Guid originalGuid = m.Guid;
      GsaModel clone = m.Clone();
      Guid cloneGuid = clone.Guid;
      Assert.NotEqual(cloneGuid, originalGuid);
      GsaModel dup = m.Duplicate();
      Guid dupGuid = dup.Guid;
      Assert.Equal(dupGuid, originalGuid);
    }

    [Fact]
    public void TestCreateModelFromModel() {
      var original = new GsaModel();
      original.Model.Open(GsaFile.Steel_Design_Simple);

      var assembled = new GsaModel {
        Model = AssembleModel.Assemble(original,
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        LengthUnit.Meter,
        Length.Zero,
        false,
        null),
      };

      Duplicates.AreEqual(original, assembled, true);
    }

    [Theory]
    [InlineData(LengthUnit.Meter, 12800.0)]
    [InlineData(LengthUnit.Foot, 452027.734035)]
    public void TestGetBoundingBox(LengthUnit modelUnit, double expectedVolume) {
      var model = new GsaModel();
      model.Model.Open(GsaFile.Steel_Design_Complex);
      model.ModelUnit = modelUnit;
      BoundingBox bbox = model.BoundingBox;

      Assert.Equal(expectedVolume, bbox.Volume, 6);
    }
  }
}
