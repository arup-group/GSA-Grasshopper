using GsaGH.Components;
using GsaGH.Parameters;

using GsaGHTests.Helpers;

using OasysGH.Components;

using OasysUnits.Units;

using Rhino.Geometry;

using Xunit;

namespace GsaGHTests.Components.Geometry {
  [Collection("GrasshopperFixture collection")]
  public class SectionAlignmentTests {
    public static GH_OasysComponent ComponentMother() {
      var comp = new SectionAlignment();
      comp.CreateAttributes();

      var member = new GsaMember1d(new LineCurve(new Point3d(0, 0, 0), new Point3d(0, 0, 10))) {
        Section = new GsaSection("CAT HE HE300.B"),
      };
      var goo = new GsaMember1dGoo(member);

      ComponentTestHelper.SetInput(comp, goo);

      return comp;
    }

    [Theory]
    [InlineData("Top-Left", 10, 10, -150, -150)]
    [InlineData("Top-Centre", -10, -10, 0, -150)]
    [InlineData("Top-Center", 2, 2, 0, -150)]
    [InlineData("Top", 3, 3, 0, -150)]
    [InlineData("Top-Right", 4, 4, 150, -150)]
    [InlineData("Mid-Left", 5, 5, -150, 0)]
    [InlineData("Left", 6, 6, -150, 0)]
    [InlineData("Centroid", 7, 7, 0, 0)]
    [InlineData("Mid-Right", 8, 8, 150, 0)]
    [InlineData("Right", 9, 9, 150, 0)]
    [InlineData("Bottom-Left", 10, 10, -150, 150)]
    [InlineData("Bottom-Centre", 11, 11, 0, 150)]
    [InlineData("Bottom-Center", 12, 12, 0, 150)]
    [InlineData("Bottom", 13, 13, 0, 150)]
    [InlineData("Bottom-Right", -14, -14, 150, 150)]
    public void AlignmentOffsetTest(
      string alignment, double y, double z, double expectedY, double expectedZ) {
      GH_OasysComponent comp = ComponentMother();

      ComponentTestHelper.SetInput(comp, alignment, 1);
      ComponentTestHelper.SetInput(comp,
        new GsaOffsetGoo(new GsaOffset(0, 0, y, z, LengthUnit.Millimeter)), 2);

      var output = (GsaOffsetGoo)ComponentTestHelper.GetOutput(comp, 1);

      Assert.Equal(expectedY + y, output.Value.Y.Millimeters, 6);
      Assert.Equal(expectedZ + z, output.Value.Z.Millimeters, 6);
    }

    [Theory]
    [InlineData("Top-Left", -150, -150)]
    [InlineData("Top-Centre", 0, -150)]
    [InlineData("Top-Center", 0, -150)]
    [InlineData("Top", 0, -150)]
    [InlineData("Top-Right", 150, -150)]
    [InlineData("Mid-Left", -150, 0)]
    [InlineData("Left", -150, 0)]
    [InlineData("Centroid", 0, 0)]
    [InlineData("Mid-Right", 150, 0)]
    [InlineData("Right", 150, 0)]
    [InlineData("Bottom-Left", -150, 150)]
    [InlineData("Bottom-Centre", 0, 150)]
    [InlineData("Bottom-Center", 0, 150)]
    [InlineData("Bottom", 0, 150)]
    [InlineData("Bottom-Right", 150, 150)]
    public void AlignmentTest(string alignment, double expectedY, double expectedZ) {
      GH_OasysComponent comp = ComponentMother();

      ComponentTestHelper.SetInput(comp, alignment, 1);

      var output = (GsaOffsetGoo)ComponentTestHelper.GetOutput(comp, 1);

      Assert.Equal(expectedY, output.Value.Y.Millimeters, 6);
      Assert.Equal(expectedZ, output.Value.Z.Millimeters, 6);
    }

    [Theory]
    [InlineData("STD A(cm) 20 20 1.5 1.5", 100, 100)]
    [InlineData("STD CH(cm) 20 20 1.5 1.5", 100, 100)]
    [InlineData("STD CHS(cm) 20 1.5", 100, 100)]
    [InlineData("STD C(cm) 20", 100, 100)]
    [InlineData("STD X(cm) 20 20 1.5 1.5", 100, 100)]
    [InlineData("STD OVAL(cm) 20 20 1.5", 100, 100)]
    [InlineData("STD E(cm) 20 20 2", 100, 100)]
    [InlineData("STD GC(cm) 20 20 1.5 1.5", 100, 100)]
    [InlineData("STD GZ(cm) 20 20 20 1.5 1.5 1.5", 100, 100)]
    [InlineData("STD GI(cm) 20 20 20 1.5 1.5 1.5", 100, 100)]
    [InlineData("STD CB(cm) 20 20 20 1.5 1.5 1.5", 100, 100)]
    [InlineData("STD I(cm) 20 20 1.5 1.5", 100, 100)]
    [InlineData("STD RHS(cm) 20 20 1.5 1.5", 100, 100)]
    [InlineData("STD R(cm) 20 20", 100, 100)]
    [InlineData("STD RE 200 200 150 120 2", 100, 100)]
    [InlineData("STD RC(cm) 20 20", 100, 100)]
    [InlineData("STD SP(cm) 20 10 2", 150, 100)]
    [InlineData("STD SPW(cm) 20 10 2", 100, 100)]
    [InlineData("STD SHT(cm) 20 20 5 5 2 1", 100, 100)]
    [InlineData("STD TR(cm) 20 10 20", 100, 100)]
    [InlineData("STD T(cm) 20 20 2 2", 100, 100)]
    public void AlignmentProfileTest(string profile, double expectedY, double expectedZ) {
      var member = new GsaMember1d(new LineCurve(new Point3d(0, 0, 0), new Point3d(0, 0, 10))) {
        Section = new GsaSection(profile),
      };
      var goo = new GsaMember1dGoo(member);
      var comp = new SectionAlignment();
      comp.CreateAttributes();
      ComponentTestHelper.SetInput(comp, goo, 0);
      ComponentTestHelper.SetInput(comp, "Bottom-Right", 1);

      var output = (GsaOffsetGoo)ComponentTestHelper.GetOutput(comp, 1);

      Assert.Equal(expectedY, output.Value.Y.Millimeters, 6);
      Assert.Equal(expectedZ, output.Value.Z.Millimeters, 6);
    }

    [Fact]
    public void SectionAlignmentElement1dTest() {
      var comp = new SectionAlignment();
      comp.CreateAttributes();

      var element = new GsaElement1d(new LineCurve(new Point3d(0, 0, 0), new Point3d(0, 0, 10))) {
        Section = new GsaSection("CAT HE HE300.B"),
      };
      var goo = new GsaElement1dGoo(element);

      ComponentTestHelper.SetInput(comp, goo);
      ComponentTestHelper.SetInput(comp, "Top-Left", 1);
      var output = (GsaOffsetGoo)ComponentTestHelper.GetOutput(comp, 1);

      Assert.Equal(-150, output.Value.Y.Millimeters, 6);
      Assert.Equal(-150, output.Value.Z.Millimeters, 6);
    }

    [Fact]
    public void SectionAlignmentElement2dTest() {
      var comp = new SectionAlignment();
      comp.CreateAttributes();

      var goo = (GsaElement2dGoo)ComponentTestHelper.GetOutput(
        CreateElement2dTests.ComponentMother());

      ComponentTestHelper.SetInput(comp, goo);
      ComponentTestHelper.SetInput(comp, "Top-Left", 1);
      var output = (GsaOffsetGoo)ComponentTestHelper.GetOutput(comp, 1);

      Assert.Equal(-7, output.Value.Z.Inches, 6);
    }

    [Fact]
    public void SectionAlignmentMember2dTest() {
      var comp = new SectionAlignment();
      comp.CreateAttributes();

      var goo = (GsaMember2dGoo)ComponentTestHelper.GetOutput(
        CreateMember2dTests.ComponentMother());

      ComponentTestHelper.SetInput(comp, goo);
      ComponentTestHelper.SetInput(comp, "Top-Left", 1);
      var output = (GsaOffsetGoo)ComponentTestHelper.GetOutput(comp, 1);

      Assert.Equal(-7, output.Value.Z.Inches, 6);
    }

    [Fact]
    public void SectionAlignmentMember1dNullErrorTest() {
      var comp = new SectionAlignment();
      comp.CreateAttributes();
      var goo = new GsaMember1dGoo(null);
      ComponentTestHelper.SetInput(comp, goo);
      ComponentTestHelper.SetInput(comp, "Top-Left", 1);

      comp.Params.Output[0].ExpireSolution(true);
      comp.Params.Output[0].CollectData();

      Assert.Single(comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Error));
    }

    [Fact]
    public void SectionAlignmentMember1dNoProfileErrorTest() {
      var comp = new SectionAlignment();
      comp.CreateAttributes();
      var member = new GsaMember1d(new LineCurve(new Point3d(0, 0, 0), new Point3d(0, 0, 10)));
      var goo = new GsaMember1dGoo(member);
      ComponentTestHelper.SetInput(comp, goo);
      ComponentTestHelper.SetInput(comp, "Top-Left", 1);

      comp.Params.Output[0].ExpireSolution(true);
      comp.Params.Output[0].CollectData();

      Assert.Single(comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Error));
    }

    [Fact]
    public void SectionAlignmentElement1dNullErrorTest() {
      var comp = new SectionAlignment();
      comp.CreateAttributes();
      var goo = new GsaElement1dGoo(null);
      ComponentTestHelper.SetInput(comp, goo);
      ComponentTestHelper.SetInput(comp, "Top-Left", 1);

      comp.Params.Output[0].ExpireSolution(true);
      comp.Params.Output[0].CollectData();

      Assert.Single(comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Error));
    }

    [Fact]
    public void SectionAlignmentElement1dNoProfileErrorTest() {
      var comp = new SectionAlignment();
      comp.CreateAttributes();
      var element = new GsaElement1d(new LineCurve(new Point3d(0, 0, 0), new Point3d(0, 0, 10)));
      var goo = new GsaElement1dGoo(element);
      ComponentTestHelper.SetInput(comp, goo);
      ComponentTestHelper.SetInput(comp, "Top-Left", 1);

      comp.Params.Output[0].ExpireSolution(true);
      comp.Params.Output[0].CollectData();

      Assert.Single(comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Error));
    }

    [Fact]
    public void SectionAlignmentMember2dNullErrorTest() {
      var comp = new SectionAlignment();
      comp.CreateAttributes();
      var goo = new GsaMember2dGoo(null);
      ComponentTestHelper.SetInput(comp, goo);
      ComponentTestHelper.SetInput(comp, "Top-Left", 1);

      comp.Params.Output[0].ExpireSolution(true);
      comp.Params.Output[0].CollectData();

      Assert.Single(comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Error));
    }

    [Fact]
    public void SectionAlignmentMember2dNoProfileErrorTest() {
      var comp = new SectionAlignment();
      comp.CreateAttributes();
      var brep = Brep.CreateFromCornerPoints(new Point3d(0, 0, 0), new Point3d(10, 0, 0),
          new Point3d(10, 10, 0), new Point3d(0, 10, 0), 1);
      var member = new GsaMember2d(brep);
      var goo = new GsaMember2dGoo(member);
      ComponentTestHelper.SetInput(comp, goo);
      ComponentTestHelper.SetInput(comp, "Top-Left", 1);

      comp.Params.Output[0].ExpireSolution(true);
      comp.Params.Output[0].CollectData();

      Assert.Single(comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Error));
    }

    [Fact]
    public void SectionAlignmentElement2dNullErrorTest() {
      var comp = new SectionAlignment();
      comp.CreateAttributes();
      var goo = new GsaElement2dGoo(null);
      ComponentTestHelper.SetInput(comp, goo);
      ComponentTestHelper.SetInput(comp, "Top-Left", 1);

      comp.Params.Output[0].ExpireSolution(true);
      comp.Params.Output[0].CollectData();

      Assert.Single(comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Error));
    }

    [Fact]
    public void SectionAlignmentElement2dNoProfileErrorTest() {
      var comp = new SectionAlignment();
      comp.CreateAttributes();
      var mesh = new Mesh();
      mesh.Vertices.Add(new Point3d(0, 0, 0));
      mesh.Vertices.Add(new Point3d(1, 0, 0));
      mesh.Vertices.Add(new Point3d(1, 1, 0));
      mesh.Vertices.Add(new Point3d(0, 1, 0));
      mesh.Faces.AddFace(0, 1, 2, 3);
      var element = new GsaElement2d(mesh);
      var goo = new GsaElement2dGoo(element);
      ComponentTestHelper.SetInput(comp, goo);
      ComponentTestHelper.SetInput(comp, "Top-Left", 1);

      comp.Params.Output[0].ExpireSolution(true);
      comp.Params.Output[0].CollectData();

      Assert.Single(comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Error));
    }
  }
}
