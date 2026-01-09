using System.Drawing;

using GsaGH.Parameters;
using GsaGH.Helpers;

using Rhino.Geometry;

using Xunit;

namespace GsaGHTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  public class GsaAnnotation3dTests {

    public static GsaAnnotation3d Annotation3dMother() {
      var pln = new Plane(new Point3d(1, 1, 1), new Vector3d(1, 1, 1));
      Color col = Color.Teal;
      string text = "32.1mm";
      double height = 8.0;
      return new GsaAnnotation3d(pln, col, text, height);
    }

    [Fact]
    public void ConstructorTest() {
      GsaAnnotation3d anno3d = Annotation3dMother();
      Assert.Equal(1, anno3d.Location.X, DoubleComparer.Default);
      Assert.Equal(1, anno3d.Location.Y, DoubleComparer.Default);
      Assert.Equal(1, anno3d.Location.Z, DoubleComparer.Default);
      Assert.Equal("Annotation3D", anno3d.TypeName);
      Assert.Equal("A GSA 3D Annotation.", anno3d.TypeDescription);
      Assert.Equal(Rhino.DocObjects.TextHorizontalAlignment.Center, anno3d.Value.HorizontalAlignment);
      Assert.Equal(Rhino.DocObjects.TextVerticalAlignment.Top, anno3d.Value.VerticalAlignment);
      Assert.Equal(8.0, anno3d.Value.Height);
      Assert.Equal("32.1mm", anno3d.Value.Text);
      Assert.Equal(Color.Teal, anno3d.Color);
    }

    [Fact]
    public void BoundingBoxTest() {
      GsaAnnotation3d anno3d = Annotation3dMother();
      Assert.Equal(-12.84, anno3d.ClippingBox.Corner(true, true, true).X, 2);
      Assert.Equal(-12.93, anno3d.ClippingBox.Corner(true, true, true).Y, 2);
      Assert.Equal(-5.68, anno3d.ClippingBox.Corner(true, true, true).Z, 2);
      Assert.Equal(18.20, anno3d.ClippingBox.Corner(false, false, false).X, 2);
      Assert.Equal(18.10, anno3d.ClippingBox.Corner(false, false, false).Y, 2);
      Assert.Equal(1.15, anno3d.ClippingBox.Corner(false, false, false).Z, 2);
    }

    [Fact]
    public void DuplicateGeometryTest() {
      var anno3d = (GsaAnnotation3d)Annotation3dMother().DuplicateGeometry();
      Assert.NotNull(anno3d);
    }

    [Fact]
    public void GetBoundingBoxTest() {
      GsaAnnotation3d anno3d = Annotation3dMother();
      var transform = Transform.Translation(new Vector3d(1, 1, 1));
      BoundingBox boundingBox = anno3d.GetBoundingBox(transform);
      Assert.Equal(-11.84, boundingBox.Corner(true, true, true).X, 2);
      Assert.Equal(-11.93, boundingBox.Corner(true, true, true).Y, 2);
      Assert.Equal(-4.68, boundingBox.Corner(true, true, true).Z, 2);
      Assert.Equal(19.20, boundingBox.Corner(false, false, false).X, 2);
      Assert.Equal(19.10, boundingBox.Corner(false, false, false).Y, 2);
      Assert.Equal(2.15, boundingBox.Corner(false, false, false).Z, 2);
    }

    [Fact]
    public void GetGeometryTest() {
      GeometryBase anno3d = Annotation3dMother().GetGeometry();
      Assert.NotNull(anno3d);
    }
  }
}
