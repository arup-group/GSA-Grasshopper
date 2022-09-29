using ComposGHTests.Helpers;
using GsaGH.Parameters;
using Xunit;

namespace GsaGHTests
{
  [Collection("GrasshopperFixture collection")]
  public class ObjectExtensionTests
  {
    [Fact]
    public void GsaBool6EqualsTest()
    {
      GsaBool6 original = new GsaBool6();
      GsaBool6 duplicate = original.Duplicate();

      Duplicates.AreEqual(original, duplicate);
    }

    [Fact]
    public void GsaMaterialEqualsTest()
    {
      GsaMaterial original = new GsaMaterial();
      original.MaterialType = GsaMaterial.MatType.ALUMINIUM;
      GsaMaterial duplicate = original.Duplicate();

      Duplicates.AreEqual(original, duplicate);
    }

    [Fact]
    public void GsaOffsetEqualsTest()
    {
      GsaOffset original = new GsaOffset();
      GsaOffset duplicate = original.Duplicate();

      Duplicates.AreEqual(original, duplicate);
    }

  }
}
