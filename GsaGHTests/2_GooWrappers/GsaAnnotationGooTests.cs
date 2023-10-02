using System.Drawing;
using Grasshopper.Kernel.Types;
using GsaGH.Helpers.Graphics;
using GsaGH.Parameters;
using GsaGHTests.GooWrappers;
using OasysGH.Parameters;
using OasysUnits;
using OasysUnits.Units;
using Rhino.Collections;
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
      Assert.Equal(32.1, number.Value);
    }

    [Fact]
    public void CastAnnotationDotToGHNumberTest() {
      var annoDot = new GsaAnnotationGoo(GsaAnnotationDotTests.AnnotationDotMother());
      GH_Number number = null;
      annoDot.CastTo(ref number);
      Assert.NotNull(number);
      Assert.Equal(32.1, number.Value);
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
      var transformed = (IGsaAnnotation)anno3d.Transform(transform);
      Assert.NotNull(transformed);
      Assert.Equal(2, transformed.Location.X, 2);
      Assert.Equal(2, transformed.Location.Y, 2);
      Assert.Equal(2, transformed.Location.Z, 2);
      Assert.Equal(1, anno3d.Value.Location.X);
      Assert.Equal(1, anno3d.Value.Location.Y);
      Assert.Equal(1, anno3d.Value.Location.Z);
    }

    [Fact]
    public void TransformAnnotationDotTest() {
      var transform = Transform.Translation(new Vector3d(1, 1, 1));
      var annoDot = new GsaAnnotationGoo(GsaAnnotationDotTests.AnnotationDotMother());
      var transformed = (IGsaAnnotation)annoDot.Transform(transform);
      Assert.NotNull(transformed);
      Assert.Equal(2, transformed.Location.X, 2);
      Assert.Equal(2, transformed.Location.Y, 2);
      Assert.Equal(2, transformed.Location.Z, 2);
      Assert.Equal(1, annoDot.Value.Location.X);
      Assert.Equal(1, annoDot.Value.Location.Y);
      Assert.Equal(1, annoDot.Value.Location.Z);
    }

    [Fact]
    public void MorphAnnotation3dTest() {
      var morph = new StretchSpaceMorph(new Point3d(0, 0, 0), new Point3d(10, 10, 10), 10);
      var anno3d = new GsaAnnotationGoo(GsaAnnotation3dTests.Annotation3dMother());
      var morphed = (IGsaAnnotation)anno3d.Morph(morph);
      Assert.NotNull(morphed);
      Assert.Equal(0.88, morphed.Location.X, 2);
      Assert.Equal(0.88, morphed.Location.Y, 2);
      Assert.Equal(0.88, morphed.Location.Z, 2);
      Assert.Equal(1, anno3d.Value.Location.X);
      Assert.Equal(1, anno3d.Value.Location.Y);
      Assert.Equal(1, anno3d.Value.Location.Z);
    }

    [Fact]
    public void MorphAnnotationDotTest() {
      var morph = new StretchSpaceMorph(new Point3d(0, 0, 0), new Point3d(10, 10, 10), 10);
      var annoDot = new GsaAnnotationGoo(GsaAnnotationDotTests.AnnotationDotMother());
      var morphed = (IGsaAnnotation)annoDot.Morph(morph);
      Assert.NotNull(morphed);
      Assert.Equal(0.88, morphed.Location.X, 2);
      Assert.Equal(0.88, morphed.Location.Y, 2);
      Assert.Equal(0.88, morphed.Location.Z, 2);
      Assert.Equal(1, annoDot.Value.Location.X);
      Assert.Equal(1, annoDot.Value.Location.Y);
      Assert.Equal(1, annoDot.Value.Location.Z);
    }
  }
}
