using System.Drawing;

using Grasshopper.Kernel.Types;

using GsaGH.Parameters;
using GsaGH.Helpers;

using GsaGHTests.GooWrappers;

using OasysGH.Parameters;

using OasysUnits.Units;

using Rhino.Geometry;
using Rhino.Geometry.Morphs;

using Xunit;
namespace GsaGHTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  public class GsaAnnotationGooTests {

    [Fact]
    public void CastAnnotation3dToUnitNumberTest() {
      var anno3d = new GsaAnnotationGoo(GsaAnnotation3dTests.Annotation3dMother());
      GH_UnitNumber un = null;
      anno3d.CastTo(ref un);
      Assert.NotNull(un);
      Assert.Equal(32.1, un.Value.As(LengthUnit.Millimeter));
    }

    [Fact]
    public void CastAnnotationDotToUnitNumberTest() {
      var annoDot = new GsaAnnotationGoo(GsaAnnotationDotTests.AnnotationDotMother());
      GH_UnitNumber un = null;
      annoDot.CastTo(ref un);
      Assert.NotNull(un);
      Assert.Equal(32.1, un.Value.As(LengthUnit.Millimeter));
    }

    [Fact]
    public void CastAnnotation3dToGHNumberTest() {
      var anno3d = new GsaAnnotationGoo(GsaAnnotation3dTests.Annotation3dMother());
      GH_Number number = null;
      anno3d.CastTo(ref number);
      Assert.NotNull(number);
      Assert.Equal(32.1, number.Value, DoubleComparer.Default);
      bool f = false;
      Assert.False(anno3d.CastTo(ref f));
    }

    [Fact]
    public void CastAnnotation3dToNumberTest() {
      var pln = new Plane(new Point3d(1, 1, 1), new Vector3d(1, 1, 1));
      Color col = Color.Teal;
      string text = "32.1";
      double height = 8.0;
      var anno3d = new GsaAnnotationGoo(new GsaAnnotation3d(pln, col, text, height));
      GH_Number number = null;
      anno3d.CastTo(ref number);
      Assert.NotNull(number);
      Assert.Equal(32.1, number.Value, DoubleComparer.Default);
    }

    [Fact]
    public void CastAnnotationDotToGHNumberTest() {
      var annoDot = new GsaAnnotationGoo(GsaAnnotationDotTests.AnnotationDotMother());
      GH_Number number = null;
      annoDot.CastTo(ref number);
      Assert.NotNull(number);
      Assert.Equal(32.1, number.Value, DoubleComparer.Default);
      bool f = false;
      Assert.False(annoDot.CastTo(ref f));
    }

    [Fact]
    public void CastAnnotationDotToNumberTest() {
      var pt = new Point3d(1, 1, 1);
      Color col = Color.Teal;
      string text = "32.1";
      var anno3d = new GsaAnnotationGoo(new GsaAnnotationDot(pt, col, text));
      GH_Number number = null;
      anno3d.CastTo(ref number);
      Assert.NotNull(number);
      Assert.Equal(32.1, number.Value, DoubleComparer.Default);
    }

    [Fact]
    public void DrawViewportMeshesAndWiresAnnotation3dTest() {
      var anno3d = new GsaAnnotationGoo(GsaAnnotation3dTests.Annotation3dMother());
      GH_OasysGeometryGooTests.DrawViewportMeshesAndWiresTest(anno3d);
    }

    [Fact]
    public void DrawViewportMeshesAndWiresAnnotationDotTest() {
      var anno3d = new GsaAnnotationGoo(GsaAnnotationDotTests.AnnotationDotMother());
      GH_OasysGeometryGooTests.DrawViewportMeshesAndWiresTest(anno3d);
    }

    [Fact]
    public void DuplicateAnnotation3dTest() {
      IGH_GeometricGoo anno3d = new GsaAnnotationGoo(GsaAnnotation3dTests.Annotation3dMother()).Duplicate();
      Assert.NotNull(anno3d);
    }

    [Fact]
    public void DuplicateAnnotationDotTest() {
      IGH_GeometricGoo anno3d = new GsaAnnotationGoo(GsaAnnotationDotTests.AnnotationDotMother()).Duplicate();
      Assert.NotNull(anno3d);
    }

    [Fact]
    public void GetGeometryAnnotation3dTest() {
      GeometryBase anno3d = new GsaAnnotationGoo(GsaAnnotation3dTests.Annotation3dMother()).GetGeometry();
      Assert.NotNull(anno3d);
    }

    [Fact]
    public void GetGeometryAnnotationDotTest() {
      GeometryBase anno3d = new GsaAnnotationGoo(GsaAnnotationDotTests.AnnotationDotMother()).GetGeometry();
      Assert.NotNull(anno3d);
    }

    [Fact]
    public void TransformAnnotation3dTest() {
      var transform = Transform.Translation(new Vector3d(1, 1, 1));
      var anno3d = new GsaAnnotationGoo(GsaAnnotation3dTests.Annotation3dMother());
      IGsaAnnotation transformed = ((GsaAnnotationGoo)anno3d.Transform(transform)).Value;
      Assert.NotNull(transformed);
      Assert.Equal(2, transformed.Location.X, 2);
      Assert.Equal(2, transformed.Location.Y, 2);
      Assert.Equal(2, transformed.Location.Z, 2);
      Assert.Equal(1, anno3d.Value.Location.X, DoubleComparer.Default);
      Assert.Equal(1, anno3d.Value.Location.Y, DoubleComparer.Default);
      Assert.Equal(1, anno3d.Value.Location.Z, DoubleComparer.Default);
    }

    [Fact]
    public void TransformAnnotationDotTest() {
      var transform = Transform.Translation(new Vector3d(1, 1, 1));
      var annoDot = new GsaAnnotationGoo(GsaAnnotationDotTests.AnnotationDotMother());
      IGsaAnnotation transformed = ((GsaAnnotationGoo)annoDot.Transform(transform)).Value;
      Assert.NotNull(transformed);
      Assert.Equal(2, transformed.Location.X, 2);
      Assert.Equal(2, transformed.Location.Y, 2);
      Assert.Equal(2, transformed.Location.Z, 2);
      Assert.Equal(1, annoDot.Value.Location.X, DoubleComparer.Default);
      Assert.Equal(1, annoDot.Value.Location.Y, DoubleComparer.Default);
      Assert.Equal(1, annoDot.Value.Location.Z, DoubleComparer.Default);
    }

    [Fact]
    public void MorphAnnotation3dTest() {
      var morph = new StretchSpaceMorph(new Point3d(0, 0, 0), new Point3d(10, 10, 10), 10);
      var anno3d = new GsaAnnotationGoo(GsaAnnotation3dTests.Annotation3dMother());
      IGsaAnnotation morphed = ((GsaAnnotationGoo)anno3d.Morph(morph)).Value;
      Assert.NotNull(morphed);
      Assert.Equal(0.88, morphed.Location.X, 2);
      Assert.Equal(0.88, morphed.Location.Y, 2);
      Assert.Equal(0.88, morphed.Location.Z, 2);
      Assert.Equal(1, anno3d.Value.Location.X, DoubleComparer.Default);
      Assert.Equal(1, anno3d.Value.Location.Y, DoubleComparer.Default);
      Assert.Equal(1, anno3d.Value.Location.Z, DoubleComparer.Default);
    }

    [Fact]
    public void MorphAnnotationDotTest() {
      var morph = new StretchSpaceMorph(new Point3d(0, 0, 0), new Point3d(10, 10, 10), 10);
      var annoDot = new GsaAnnotationGoo(GsaAnnotationDotTests.AnnotationDotMother());
      IGsaAnnotation morphed = ((GsaAnnotationGoo)annoDot.Morph(morph)).Value;
      Assert.NotNull(morphed);
      Assert.Equal(0.88, morphed.Location.X, 2);
      Assert.Equal(0.88, morphed.Location.Y, 2);
      Assert.Equal(0.88, morphed.Location.Z, 2);
      Assert.Equal(1, annoDot.Value.Location.X, DoubleComparer.Default);
      Assert.Equal(1, annoDot.Value.Location.Y, DoubleComparer.Default);
      Assert.Equal(1, annoDot.Value.Location.Z, DoubleComparer.Default);
    }
  }
}
