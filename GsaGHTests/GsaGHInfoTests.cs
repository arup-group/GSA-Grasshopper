using System;
using System.Reflection;

using GsaGH;

using Xunit;

namespace GsaGHTests {
  [Collection("GrasshopperFixture collection")]
  public class GsaGHInfoTests {
    [Fact]
    public void GsaGhInfoStaticFieldsTest() {
      var info = new GsaGhInfo();
      Assert.Equal(GsaGhInfo.Contact, info.AuthorContact);
      Assert.Equal(GsaGhInfo.Company, info.AuthorName);
      Assert.Contains(GsaGhInfo.Copyright, info.Description);
      Assert.Contains(GsaGhInfo.TermsConditions, info.Description);
      Assert.NotNull(info.Icon);
    }

    [Fact]
    public void GsaGhInfoGuidIsNotChangedTest() {
      Assert.Equal("a3b08c32-f7de-4b00-b415-f8b466f05e9f", GsaGhInfo.guid.ToString());
    }

    [Fact]
    public void GsaGhInfoVersionIsAsInDllTest() {
      var info = new GsaGhInfo();
      var assembly = Assembly.LoadFrom("GsaGH.dll");
      Version ver = assembly.GetName().Version;
      string expectedVersion = $"{ver.Major}.{ver.Minor}.{ver.Build}";
      if (GsaGhInfo.isBeta) {
        expectedVersion += "-beta";
      }
      Assert.Equal(expectedVersion, info.Version);
    }
  }
}
