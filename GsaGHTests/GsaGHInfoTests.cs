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
      Assert.Contains(GsaGhInfo.ProductName, info.Name);
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

    [Theory]
    [InlineData("1.0.0", "1.1.1", 100, 111)]
    [InlineData("1.01", "222", 101, 222)]
    [InlineData("102", "", 102, 0)]
    public void TryoCalculateVersionsShouldProvideNumbers(string s1, string s2, int expectedInt1, int expectedInt2) {
      var addReferencePriority = new AddReferencePriority();
      addReferencePriority.TryCalculateVersions(s1, s2, out int dll1, out int dll2);
      Assert.Equal(expectedInt1, dll1);
      Assert.Equal(expectedInt2, dll2);
    }

    [Theory]
    [InlineData(1, 2)]
    [InlineData(2, 1)]
    public void CheckGsaUpdateIsRequiredShouldReturnValidOutput(int ver1, int ver2) {
      if (ver1 > ver2) {
        Assert.False(AddReferencePriority.CheckGsaUpdateIsRequired(ver1, ver2));
      } else {
        Assert.True(AddReferencePriority.CheckGsaUpdateIsRequired(ver1, ver2));
      }
    }
  }
}
