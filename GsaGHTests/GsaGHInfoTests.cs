using System;
using System.Reflection;

using GsaGH;

using OasysGH;

using Xunit;

using static GsaGH.GsaGhInfo;

namespace GsaGHTests {
  [Collection("GrasshopperFixture collection")]
  public class GsaGHInfoTests {
    [Fact]
    public void GsaVersionRequiredStructContainsStaticFields() {
      Assert.Equal(GsaVersionRequired.MainVersion,
        $"{GsaVersionRequired.MajorVersion}.{GsaVersionRequired.MinorVersion}");
      Assert.Equal(GsaVersionRequired.FullVersion,
        $"{GsaVersionRequired.MajorVersion}.{GsaVersionRequired.MinorVersion}.{GsaVersionRequired.BuildVersion}");
    }

    [Fact]
    public void GsaGhInfoStaticFieldsTest() {
      var info = new GsaGhInfo();
      Assert.Equal(Contact, info.AuthorContact);
      Assert.Equal(Company, info.AuthorName);
      Assert.Contains(Copyright, info.Description);
      Assert.Contains(TermsConditions, info.Description);
      Assert.Contains(ProductName, info.Name);
      Assert.NotNull(info.Icon);
    }

    [Fact]
    public void GsaGhInfoGuidIsNotChangedTest() {
      Assert.Equal("a3b08c32-f7de-4b00-b415-f8b466f05e9f", guid.ToString());
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void CheckVersionIsValidWhenBetaIsSetTo(bool isBeta) {
      GsaGhInfo.isBeta = isBeta;
      var info = new GsaGhInfo();
      string expectedString = isBeta ? GrasshopperVersion + "-beta" : GrasshopperVersion;

      Assert.Equal(expectedString, info.Version);
    }

    [Fact]
    public void GsaGhInfoVersionIsAsInDllTest() {
      var info = new GsaGhInfo();
      var assembly = Assembly.LoadFrom("GsaGH.dll");
      Version ver = assembly.GetName().Version;
      string expectedVersion = $"{ver.Major}.{ver.Minor}.{ver.Build}";
      if (isBeta) {
        expectedVersion += "-beta";
      }
      Assert.Equal(expectedVersion, info.Version);
    }

    [Fact]
    public void CheckGsaUpdateIsRequiredShouldReturnTrueWhenNeededVersionIsHigher() {
      string ver1 = "1.1.1";
      string ver2 = "1.1.2";
      Assert.True(AddReferencePriority.CheckGsaUpdateIsRequired(ver1, ver2));
    }

    [Theory]
    [InlineData("1.1.2", "1.1.1")]
    [InlineData("1.1.3", "1.1.3")]
    public void CheckGsaUpdateIsRequiredShouldReturnFalseWhenNeededVersionIsLowerOrEqual(string ver1, string ver2) {
      Assert.False(AddReferencePriority.CheckGsaUpdateIsRequired(ver1, ver2));
    }

    [Fact]
    public void ShouldHavePosthogSetupWhenWeLoad() {
      var analyticsSpy = new AnalyticsSpy();
      var addReferencePriority = new AddReferencePriority(analyticsSpy, true);
      addReferencePriority.PriorityLoad();
      Assert.True(analyticsSpy.loadedCalled);
    }

    class AnalyticsSpy: IAnalytics {

      public bool loadedCalled = false;

      public void PluginLoaded(OasysPluginInfo pluginInfo, string error = "") { loadedCalled = true; }
    }
  }
}
