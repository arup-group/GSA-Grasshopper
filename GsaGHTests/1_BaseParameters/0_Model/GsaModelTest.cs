using GsaAPI;
using GsaGH.Components;
using GsaGH.Parameters;
using GsaGHTests.Helper;
using GsaGHTests.Helpers;
using OasysUnits;
using OasysUnits.Units;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using Xunit;

namespace GsaGHTests.Parameters
{
  [Collection("GrasshopperFixture collection")]
  public class GsaModelTest
  {
    [Fact]
    public void GsaModelEqualsTest()
    {
      // Arrange
      GsaModel original = new GsaModel();

      // Act
      GsaModel duplicate = original.Duplicate();

      // Assert
      Duplicates.AreEqual(original, duplicate);
    }

    [Fact]
    public void TestSaveModel()
    {
      // create new GH-GSA model 
      GsaModel m = new GsaModel();

      string file = GsaFile.Steel_Design_Simple;
      m.Model.Open(file);

      // save file to temp location
      string tempfilename = System.IO.Path.GetTempPath() + "GSA-Grasshopper_temp.gwb";
      ReturnValue returnValue = m.Model.SaveAs(tempfilename);

      Assert.Same(ReturnValue.GS_OK.ToString(), returnValue.ToString());
    }

    [Fact]
    public void TestDuplicateModel()
    {
      // create new GH-GSA model 
      GsaModel m = new GsaModel();

      // get the GSA install path
      string installPath = GsaGH.Helpers.GsaAPI.InstallationFolder.GetPath;

      // open existing GSA model (steel design sample)
      ReturnValue returnValue = m.Model.Open(installPath + "\\Samples\\Steel\\Steel_Design_Simple.gwb");

      // get original GUID
      Guid originalGUID = m.Guid;

      // clone model
      GsaModel clone = m.Clone();

      // get clone GUID
      Guid cloneGUID = clone.Guid;
      Assert.NotEqual(cloneGUID, originalGUID);

      // duplicate model
      GsaModel dup = m.Duplicate();

      // get duplicate GUID
      Guid dupGUID = dup.Guid;
      Assert.Equal(dupGUID, originalGUID);
    }

    [Fact]
    public void TestCreateModelFromModel()
    {
      GsaModel original = new GsaModel();
      original.Model.Open(GsaFile.Steel_Design_Simple);

      GsaModel assembled = new GsaModel();
      assembled.Model = GsaGH.Helpers.Export.AssembleModel.Assemble(original, null, null, null, null, null, null, null, null, null, null, null, null, null, null, LengthUnit.Meter, Length.Zero, false, null);

      // Assert
      Duplicates.AreEqual(original, assembled, true);
    }

    [Theory]
    [InlineData(LengthUnit.Meter, 12800.0)]
    [InlineData(LengthUnit.Foot, 452027.734035)]
    public void TestGetBoundingBox(LengthUnit modelUnit, double expectedVolume)
    {
      GsaModel model = new GsaModel();
      model.Model.Open(GsaFile.Steel_Design_Complex);

      model.ModelUnit = modelUnit;
      BoundingBox bbox = model.BoundingBox;

      // Assert
      Assert.Equal(expectedVolume, bbox.Volume, 6);
    }
  }
}
