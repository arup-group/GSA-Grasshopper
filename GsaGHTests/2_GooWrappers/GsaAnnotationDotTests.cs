using System.Drawing;

using GsaGH.Parameters;
using GsaGH.Helpers;

using Rhino.Geometry;

using Xunit;

namespace GsaGHTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  public class GsaAnnotationDotTests {

    public static GsaAnnotationDot AnnotationDotMother() {
      var pt = new Point3d(1, 1, 1);
      Color col = Color.Teal;
      string text = "32.1mm";
      return new GsaAnnotationDot(pt, col, text);
    }

    [Fact]
    public void ConstructorTest() {
      GsaAnnotationDot anno3d = AnnotationDotMother();
      Assert.Equal(1, anno3d.Location.X, DoubleComparer.Default);
      Assert.Equal(1, anno3d.Location.Y, DoubleComparer.Default);
      Assert.Equal(1, anno3d.Location.Z, DoubleComparer.Default);
      Assert.Equal("Annotation", anno3d.TypeName);
      Assert.Equal("A GSA Annotation.", anno3d.TypeDescription);
      Assert.Equal("32.1mm", anno3d.Value.Text);
      Assert.Equal(Color.Teal, anno3d.Color);
    }

    [Fact]
    public void BoundingBoxTest() {
      GsaAnnotationDot anno3d = AnnotationDotMother();
      Assert.Equal(1, anno3d.ClippingBox.Corner(true, true, true).X, 2);
      Assert.Equal(1, anno3d.ClippingBox.Corner(true, true, true).Y, 2);
      Assert.Equal(1, anno3d.ClippingBox.Corner(true, true, true).Z, 2);
      Assert.Equal(1, anno3d.ClippingBox.Corner(false, false, false).X, 2);
      Assert.Equal(1, anno3d.ClippingBox.Corner(false, false, false).Y, 2);
      Assert.Equal(1, anno3d.ClippingBox.Corner(false, false, false).Z, 2);
    }

    [Fact]
    public void DuplicateGeometryTest() {
      var anno3d = (GsaAnnotationDot)AnnotationDotMother().DuplicateGeometry();
      Assert.NotNull(anno3d);
    }

    [Fact]
    public void GetBoundingBoxTest() {
      GsaAnnotationDot annoDot = AnnotationDotMother();
      var transform = Transform.Translation(new Vector3d(1, 1, 1));
      BoundingBox boundingBox = annoDot.GetBoundingBox(transform);
      Assert.Equal(2, boundingBox.Corner(true, true, true).X, 2);
      Assert.Equal(2, boundingBox.Corner(true, true, true).Y, 2);
      Assert.Equal(2, boundingBox.Corner(true, true, true).Z, 2);
      Assert.Equal(2, boundingBox.Corner(false, false, false).X, 2);
      Assert.Equal(2, boundingBox.Corner(false, false, false).Y, 2);
      Assert.Equal(2, boundingBox.Corner(false, false, false).Z, 2);
    }

    [Fact]
    public void GetGeometryTest() {
      GeometryBase anno3d = AnnotationDotMother().GetGeometry();
      Assert.NotNull(anno3d);
    }
  }
}
