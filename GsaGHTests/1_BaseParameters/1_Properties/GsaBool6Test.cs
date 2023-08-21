﻿using System.Collections.Generic;
using GsaGH.Helpers.Export;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using OasysUnits;
using OasysUnits.Units;
using Rhino.Geometry;
using Xunit;

namespace GsaGHTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  public class GsaBool6Test {

    [Fact]
    public void AssembleWithElementTest() {
      var e1d = new GsaElement1d(new LineCurve(new Point3d(0, 0, 0), new Point3d(10, 0, 0))) {
        ReleaseStart = new GsaBool6(true, false, true, false, true, false),
        ReleaseEnd = new GsaBool6(false, true, false, true, false, true),
      };

      var assembled = new GsaModel {
        Model = Assemble.AssembleForLocalAxis(e1d),
      };

      var startAssembled = new GsaBool6(assembled.Model.Elements()[1].Release(0));
      var endAssembled = new GsaBool6(assembled.Model.Elements()[1].Release(1));

      _ = Duplicates.AreEqual(e1d.ReleaseStart, startAssembled);
      _ = Duplicates.AreEqual(e1d.ReleaseEnd, endAssembled);
    }

    [Fact]
    public void AssembleWitMemberTest() {
      var m1d = new GsaMember1d(new LineCurve(new Point3d(0, 0, 0), new Point3d(10, 0, 0))) {
        ReleaseStart = new GsaBool6(true, false, true, false, true, false),
        ReleaseEnd = new GsaBool6(false, true, false, true, false, true),
      };

      var assembled = new GsaModel {
        Model = Assemble.AssembleForLocalAxis(m1d),
      };

      var startAssembled = new GsaBool6(assembled.Model.Members()[1].GetEndRelease(0).Releases);
      var endAssembled = new GsaBool6(assembled.Model.Members()[1].GetEndRelease(1).Releases);

      _ = Duplicates.AreEqual(m1d.ReleaseStart, startAssembled);
      _ = Duplicates.AreEqual(m1d.ReleaseEnd, endAssembled);
    }

    [Theory]
    [InlineData(true, true, true, true, true, true)]
    [InlineData(false, false, false, false, false, false)]
    public void ConstructorTest(bool x, bool y, bool z, bool xx, bool yy, bool zz) {
      var b6 = new GsaBool6(x, y, z, xx, yy, zz);

      Assert.Equal(x, b6.X);
      Assert.Equal(y, b6.Y);
      Assert.Equal(z, b6.Z);
      Assert.Equal(xx, b6.Xx);
      Assert.Equal(yy, b6.Yy);
      Assert.Equal(zz, b6.Zz);
    }

    [Theory]
    [InlineData(true, true, true, true, true, true)]
    [InlineData(false, false, false, false, false, false)]
    public void DuplicateTest(bool x, bool y, bool z, bool xx, bool yy, bool zz) {
      var original = new GsaBool6(x, y, z, xx, yy, zz);

      GsaBool6 duplicate = original.Duplicate();

      _ = Duplicates.AreEqual(original, duplicate);

      duplicate.X = false;
      duplicate.Y = true;
      duplicate.Z = false;
      duplicate.Xx = true;
      duplicate.Yy = false;
      duplicate.Zz = true;

      Assert.Equal(x, original.X);
      Assert.Equal(y, original.Y);
      Assert.Equal(z, original.Z);
      Assert.Equal(xx, original.Xx);
      Assert.Equal(yy, original.Yy);
      Assert.Equal(zz, original.Zz);
    }
  }
}
