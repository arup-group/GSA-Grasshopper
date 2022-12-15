﻿using Grasshopper.Kernel;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using OasysUnits;
using OasysUnits.Units;
using Rhino.Geometry;
using System.Collections.Generic;
using Xunit;

namespace GsaGHTests.Parameters
{
  [Collection("GrasshopperFixture collection")]
  public class GsaBool6Test
  {
    [Theory]
    [InlineData(true, true, true, true, true, true)]
    [InlineData(false, false, false, false, false, false)]
    public void ConstructorTest(bool x, bool y, bool z, bool xx, bool yy, bool zz)
    {
      // Act
      GsaBool6 b6 = new GsaBool6(x, y, z, xx, yy, zz);

      // Assert
      Assert.Equal(x, b6.X);
      Assert.Equal(y, b6.Y);
      Assert.Equal(z, b6.Z);
      Assert.Equal(xx, b6.XX);
      Assert.Equal(yy, b6.YY);
      Assert.Equal(zz, b6.ZZ);
    }

    [Theory]
    [InlineData(true, true, true, true, true, true)]
    [InlineData(false, false, false, false, false, false)]
    public void DuplicateTest(bool x, bool y, bool z, bool xx, bool yy, bool zz)
    {
      // Arrange
      GsaBool6 original = new GsaBool6(x, y, z, xx, yy, zz);

      // Act
      GsaBool6 duplicate = original.Duplicate();

      // Assert
      Duplicates.AreEqual(original, duplicate);

      // make some changes to duplicate
      duplicate.X = false;
      duplicate.Y = true;
      duplicate.Z = false;
      duplicate.XX = true;
      duplicate.YY = false;
      duplicate.ZZ = true;

      Assert.Equal(x, original.X);
      Assert.Equal(y, original.Y);
      Assert.Equal(z, original.Z);
      Assert.Equal(xx, original.XX);
      Assert.Equal(yy, original.YY);
      Assert.Equal(zz, original.ZZ);
    }

    [Fact]
    public void AssembleWithElementTest()
    {
      GsaElement1d e1d = new GsaElement1d(new LineCurve(new Point3d(0, 0, 0), new Point3d(10, 0, 0)));
      e1d.ReleaseStart = new GsaBool6(true, false, true, false, true, false);
      e1d.ReleaseEnd = new GsaBool6(false, true, false, true, false, true);

      GsaModel assembled = new GsaModel();
      assembled.Model = GsaGH.Helpers.Export.AssembleModel.Assemble(null, null, new List<GsaElement1d>() { e1d }, null, null, null, null, null, null, null, null, null, null, null, null, LengthUnit.Meter, -1, false);

      GsaBool6 startAssembled = new GsaBool6(assembled.Model.Elements()[1].Release(0));
      GsaBool6 endAssembled = new GsaBool6(assembled.Model.Elements()[1].Release(1));

      Duplicates.AreEqual(e1d.ReleaseStart, startAssembled);
      Duplicates.AreEqual(e1d.ReleaseEnd, endAssembled);
    }

    [Fact]
    public void AssembleWitMemberTest()
    {
      GsaMember1d m1d = new GsaMember1d(new LineCurve(new Point3d(0, 0, 0), new Point3d(10, 0, 0)));
      m1d.ReleaseStart = new GsaBool6(true, false, true, false, true, false);
      m1d.ReleaseEnd = new GsaBool6(false, true, false, true, false, true);

      GsaModel assembled = new GsaModel();
      assembled.Model = GsaGH.Helpers.Export.AssembleModel.Assemble(null, null, null, null, null, new List<GsaMember1d>() { m1d }, null, null, null, null, null, null, null, null, null, LengthUnit.Meter, -1, false);

      GsaBool6 startAssembled = new GsaBool6(assembled.Model.Members()[1].GetEndRelease(0).Releases);
      GsaBool6 endAssembled = new GsaBool6(assembled.Model.Members()[1].GetEndRelease(1).Releases);

      Duplicates.AreEqual(m1d.ReleaseStart, startAssembled);
      Duplicates.AreEqual(m1d.ReleaseEnd, endAssembled);
    }
  }
}
