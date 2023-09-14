﻿using GsaGH.Parameters;
using GsaGHTests.Helpers;
using Xunit;

namespace GsaGHTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  public class GsaProp3Tests {

    [Fact]
    public void DuplicateTest() {
      var original = new GsaProperty3d(new GsaMaterial()) {
      };

      var duplicate = new GsaProperty3d(original);

      Duplicates.AreEqual(original, duplicate, true);
    }

    [Fact]
    public void DuplicateReferenceTest() {
      var original = new GsaProperty3d(4);
      var duplicate = new GsaProperty3d(original);

      Assert.Equal(4, duplicate.Id);
      Assert.True(duplicate.IsReferencedById);
    }

    [Fact]
    public void DuplicateReferenceTest2() {
      var original = new GsaProperty3d(4);
      var duplicate = new GsaProperty3d(original);
      Duplicates.AreEqual(original, duplicate, true);
    }
  }
}
