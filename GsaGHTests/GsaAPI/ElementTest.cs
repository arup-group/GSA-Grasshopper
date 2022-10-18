using GsaAPI;
using GsaGH;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using Rhino.Geometry;
using Xunit;

namespace GsaGHTests.GsaAPI
{
  [Collection("GrasshopperFixture collection")]
  public class ElementTest
  {
    [Fact]
    public void DuplicateTest()
    {
      // Arrange
      Element element = new Element();
      GsaSection section = new GsaSection();
      section.Name = "Name";
      GsaElement1d element1d = new GsaElement1d(element, new LineCurve(), 1, section, new GsaNode());
      element1d.Name = "Name";
      Element original = element1d.API_Element;

      // Act
      //Element duplicate = (Element)original.Duplicate();

      // Assert
      //Duplicates.AreEqual(original, duplicate);
    }
  }
}
