using System;
using System.Drawing;

using Grasshopper.Kernel.Types;

using GsaAPI;

using GsaGH.Components;
using GsaGH.Parameters;

using GsaGHTests.Helpers;

using OasysGH.Components;

using Rhino.Geometry;

using Xunit;

namespace GsaGHTests.Components.Geometry {
  [Collection("GrasshopperFixture collection")]
  public class EditElement1dTests {

    public static GH_OasysComponent ComponentMother() {
      var comp = new Edit1dElement();
      comp.CreateAttributes();

      ComponentTestHelper.SetInput(comp,
        ComponentTestHelper.GetOutput(CreateElement1dTests.ComponentMother()), 0);

      return comp;
    }

    internal static void CompareRelease(GsaBool6 input, GsaBool6 output) {
      Assert.Equal(input.ToString(), output.ToString());
      Assert.Equal(input.ToString(), output.ToString());
      Assert.Equal(input.X, output.X);
      Assert.Equal(input.Y, output.Y);
      Assert.Equal(input.Z, output.Z);
      Assert.Equal(input.Xx, output.Xx);
      Assert.Equal(input.Yy, output.Yy);
      Assert.Equal(input.Zz, output.Zz);
    }

    [Fact]
    public void GetOutputsFromInputElements() {
      GH_OasysComponent comp = ComponentMother();

      var elem = (GsaElement1dGoo)ComponentTestHelper.GetOutput(comp, 0);
      var id = (GH_Integer)ComponentTestHelper.GetOutput(comp, 1);
      var line = (GH_Line)ComponentTestHelper.GetOutput(comp, 2);
      var section = (GsaPropertyGoo)ComponentTestHelper.GetOutput(comp, 3);
      var group = (GH_Integer)ComponentTestHelper.GetOutput(comp, 4);
      var type = (GH_String)ComponentTestHelper.GetOutput(comp, 5);
      var offset = (GsaOffsetGoo)ComponentTestHelper.GetOutput(comp, 6);
      var relStart = (GsaBool6Goo)ComponentTestHelper.GetOutput(comp, 7);
      var relEnd = (GsaBool6Goo)ComponentTestHelper.GetOutput(comp, 8);
      var angle = (GH_Number)ComponentTestHelper.GetOutput(comp, 9);
      var orientation = (GsaNodeGoo)ComponentTestHelper.GetOutput(comp, 10);
      var name = (GH_String)ComponentTestHelper.GetOutput(comp, 11);
      var colour = (GH_Colour)ComponentTestHelper.GetOutput(comp, 12);
      var dummy = (GH_Boolean)ComponentTestHelper.GetOutput(comp, 13);
      var parentMem = (GH_Integer)ComponentTestHelper.GetOutput(comp, 14);

      Assert.Equal(0, elem.Value.Line.PointAtStart.X, 6);
      Assert.Equal(-1, elem.Value.Line.PointAtStart.Y, 6);
      Assert.Equal(0, elem.Value.Line.PointAtStart.Z, 6);
      Assert.Equal(7, elem.Value.Line.PointAtEnd.X, 6);
      Assert.Equal(3, elem.Value.Line.PointAtEnd.Y, 6);
      Assert.Equal(1, elem.Value.Line.PointAtEnd.Z, 6);
      Assert.Equal("STD CH(ft) 1 2 3 4", elem.Value.Section.ApiSection.Profile);
      Assert.Equal(0, id.Value);
      Assert.Equal(0, line.Value.From.X, 6);
      Assert.Equal(-1, line.Value.From.Y, 6);
      Assert.Equal(0, line.Value.From.Z, 6);
      Assert.Equal(7, line.Value.To.X, 6);
      Assert.Equal(3, line.Value.To.Y, 6);
      Assert.Equal(1, line.Value.To.Z, 6);
      Assert.Equal("STD CH(ft) 1 2 3 4", ((GsaSection)section.Value).ApiSection.Profile);
      Assert.Equal(0, group.Value);
      Assert.Equal("Beam", type.Value);
      Assert.Equal(0, offset.Value.X1.Value, 6);
      Assert.Equal(0, offset.Value.X2.Value, 6);
      Assert.Equal(0, offset.Value.Y.Value, 6);
      Assert.Equal(0, offset.Value.Z.Value, 6);
      Assert.False(relStart.Value.X);
      Assert.False(relStart.Value.Y);
      Assert.False(relStart.Value.Z);
      Assert.False(relStart.Value.Xx);
      Assert.False(relStart.Value.Yy);
      Assert.False(relStart.Value.Zz);
      Assert.False(relEnd.Value.X);
      Assert.False(relEnd.Value.Y);
      Assert.False(relEnd.Value.Z);
      Assert.False(relEnd.Value.Xx);
      Assert.False(relEnd.Value.Yy);
      Assert.False(relEnd.Value.Zz);
      Assert.Equal(0, angle.Value, 6);
      Assert.Null(orientation.Value);
      Assert.Equal("", name.Value);
      Assert.Equal(0, colour.Value.R);
      Assert.Equal(0, colour.Value.G);
      Assert.Equal(0, colour.Value.B);
      Assert.False(dummy.Value);
      Assert.Equal(0, parentMem.Value);
    }

    [Fact]
    public void EditElementByInputs() {
      GH_OasysComponent comp = ComponentMother();
      ComponentTestHelper.SetInput(comp, 1, 1);
      ComponentTestHelper.SetInput(comp, new LineCurve(new Point3d(0, 0, 0), new Point3d(1, 2, 3)),
        2);
      ComponentTestHelper.SetInput(comp, "STD CH 10 20 30 40", 3);
      ComponentTestHelper.SetInput(comp, 7, 4);
      ComponentTestHelper.SetInput(comp, "Beam", 5);
      ComponentTestHelper.SetInput(comp, new GsaOffsetGoo(new GsaOffset(1, 2, 3, 4)), 6);
      ComponentTestHelper.SetInput(comp,
        new GsaBool6Goo(new GsaBool6(true, true, true, true, true, true)), 7);
      ComponentTestHelper.SetInput(comp,
        new GsaBool6Goo(new GsaBool6(true, true, true, true, true, true)), 8);
      ComponentTestHelper.SetInput(comp, Math.PI, 9);
      var node = new GsaNode(new Point3d(1, 2, 3)) {
        Id = 99
      };
      ComponentTestHelper.SetInput(comp, new GsaNodeGoo(node), 10);
      ComponentTestHelper.SetInput(comp, "name", 11);
      ComponentTestHelper.SetInput(comp, new GH_Colour(Color.White), 12);
      ComponentTestHelper.SetInput(comp, true, 13);

      var elem = (GsaElement1dGoo)ComponentTestHelper.GetOutput(comp, 0);
      var id = (GH_Integer)ComponentTestHelper.GetOutput(comp, 1);
      var line = (GH_Line)ComponentTestHelper.GetOutput(comp, 2);
      var section = (GsaPropertyGoo)ComponentTestHelper.GetOutput(comp, 3);
      var group = (GH_Integer)ComponentTestHelper.GetOutput(comp, 4);
      var type = (GH_String)ComponentTestHelper.GetOutput(comp, 5);
      var offset = (GsaOffsetGoo)ComponentTestHelper.GetOutput(comp, 6);
      var relStart = (GsaBool6Goo)ComponentTestHelper.GetOutput(comp, 7);
      var relEnd = (GsaBool6Goo)ComponentTestHelper.GetOutput(comp, 8);
      var angle = (GH_Number)ComponentTestHelper.GetOutput(comp, 9);
      var orientation = (GsaNodeGoo)ComponentTestHelper.GetOutput(comp, 10);
      var name = (GH_String)ComponentTestHelper.GetOutput(comp, 11);
      var colour = (GH_Colour)ComponentTestHelper.GetOutput(comp, 12);
      var dummy = (GH_Boolean)ComponentTestHelper.GetOutput(comp, 13);
      var parentMem = (GH_Integer)ComponentTestHelper.GetOutput(comp, 14);

      Assert.Equal(0, elem.Value.Line.PointAtStart.X);
      Assert.Equal(0, elem.Value.Line.PointAtStart.Y);
      Assert.Equal(0, elem.Value.Line.PointAtStart.Z);
      Assert.Equal(1, elem.Value.Line.PointAtEnd.X);
      Assert.Equal(2, elem.Value.Line.PointAtEnd.Y);
      Assert.Equal(3, elem.Value.Line.PointAtEnd.Z);
      Assert.Equal("STD CH 10 20 30 40", elem.Value.Section.ApiSection.Profile);
      Assert.Equal(1, id.Value);
      Assert.Equal(0, line.Value.From.X);
      Assert.Equal(0, line.Value.From.Y);
      Assert.Equal(0, line.Value.From.Z);
      Assert.Equal(1, line.Value.To.X);
      Assert.Equal(2, line.Value.To.Y);
      Assert.Equal(3, line.Value.To.Z);
      Assert.Equal("STD CH 10 20 30 40", ((GsaSection)section.Value).ApiSection.Profile);
      Assert.Equal(7, group.Value);
      Assert.Equal("Beam", type.Value);
      Assert.Equal(1, offset.Value.X1.Value);
      Assert.Equal(2, offset.Value.X2.Value);
      Assert.Equal(3, offset.Value.Y.Value);
      Assert.Equal(4, offset.Value.Z.Value);
      Assert.True(relStart.Value.X);
      Assert.True(relStart.Value.Y);
      Assert.True(relStart.Value.Z);
      Assert.True(relStart.Value.Xx);
      Assert.True(relStart.Value.Yy);
      Assert.True(relStart.Value.Zz);
      Assert.True(relEnd.Value.X);
      Assert.True(relEnd.Value.Y);
      Assert.True(relEnd.Value.Z);
      Assert.True(relEnd.Value.Xx);
      Assert.True(relEnd.Value.Yy);
      Assert.True(relEnd.Value.Zz);
      Assert.Equal(Math.PI, angle.Value);
      Assert.Equal(1, orientation.Value.Point.X);
      Assert.Equal(2, orientation.Value.Point.Y);
      Assert.Equal(3, orientation.Value.Point.Z);
      Assert.Equal(99, orientation.Value.Id);
      Assert.Equal("name", name.Value);
      Assert.Equal(255, colour.Value.R);
      Assert.Equal(255, colour.Value.G);
      Assert.Equal(255, colour.Value.B);
      Assert.True(dummy.Value);
      Assert.Equal(0, parentMem.Value);
    }

    [Fact]
    public void EditElementByInputsOverride() {
      GH_OasysComponent comp = ComponentMother();
      ComponentTestHelper.SetInput(comp, "1", 5); // set beam type by int
      var type = (GH_String)ComponentTestHelper.GetOutput(comp, 5, 0, 0, true);
      Assert.Equal("Bar", type.Value);
    }

    [Fact]
    public void ChangeToSpringElement() {
      GH_OasysComponent comp = ComponentMother();
      var property = new AxialSpringProperty {
        Stiffness = 3.0
      };
      ComponentTestHelper.SetInput(comp, new GsaPropertyGoo(new GsaSpringProperty(property)), 3);
      ComponentTestHelper.SetInput(comp, "Spring", 5);

      var elem = (GsaElement1dGoo)ComponentTestHelper.GetOutput(comp, 0);
      var id = (GH_Integer)ComponentTestHelper.GetOutput(comp, 1);
      var line = (GH_Line)ComponentTestHelper.GetOutput(comp, 2);
      var springProperty = (GsaPropertyGoo)ComponentTestHelper.GetOutput(comp, 3);
      var group = (GH_Integer)ComponentTestHelper.GetOutput(comp, 4);
      var type = (GH_String)ComponentTestHelper.GetOutput(comp, 5);
      var offset = (GsaOffsetGoo)ComponentTestHelper.GetOutput(comp, 6);
      var relStart = (GsaBool6Goo)ComponentTestHelper.GetOutput(comp, 7);
      var relEnd = (GsaBool6Goo)ComponentTestHelper.GetOutput(comp, 8);
      var angle = (GH_Number)ComponentTestHelper.GetOutput(comp, 9);
      var orientation = (GsaNodeGoo)ComponentTestHelper.GetOutput(comp, 10);
      var name = (GH_String)ComponentTestHelper.GetOutput(comp, 11);
      var colour = (GH_Colour)ComponentTestHelper.GetOutput(comp, 12);
      var dummy = (GH_Boolean)ComponentTestHelper.GetOutput(comp, 13);
      var parentMem = (GH_Integer)ComponentTestHelper.GetOutput(comp, 14);

      Assert.Equal(0, elem.Value.Line.PointAtStart.X, 6);
      Assert.Equal(-1, elem.Value.Line.PointAtStart.Y, 6);
      Assert.Equal(0, elem.Value.Line.PointAtStart.Z, 6);
      Assert.Equal(7, elem.Value.Line.PointAtEnd.X, 6);
      Assert.Equal(3, elem.Value.Line.PointAtEnd.Y, 6);
      Assert.Equal(1, elem.Value.Line.PointAtEnd.Z, 6);
      Assert.Null(elem.Value.Section);
      Assert.Equal(0, id.Value);
      Assert.Equal(0, line.Value.From.X, 6);
      Assert.Equal(-1, line.Value.From.Y, 6);
      Assert.Equal(0, line.Value.From.Z, 6);
      Assert.Equal(7, line.Value.To.X, 6);
      Assert.Equal(3, line.Value.To.Y, 6);
      Assert.Equal(1, line.Value.To.Z, 6);
      Assert.NotNull(((GsaSpringProperty)springProperty.Value).ApiProperty);
      Assert.Equal(0, group.Value);
      Assert.Equal("Spring", type.Value);
      Assert.Equal(0, offset.Value.X1.Value, 6);
      Assert.Equal(0, offset.Value.X2.Value, 6);
      Assert.Equal(0, offset.Value.Y.Value, 6);
      Assert.Equal(0, offset.Value.Z.Value, 6);
      Assert.False(relStart.Value.X);
      Assert.False(relStart.Value.Y);
      Assert.False(relStart.Value.Z);
      Assert.False(relStart.Value.Xx);
      Assert.False(relStart.Value.Yy);
      Assert.False(relStart.Value.Zz);
      Assert.False(relEnd.Value.X);
      Assert.False(relEnd.Value.Y);
      Assert.False(relEnd.Value.Z);
      Assert.False(relEnd.Value.Xx);
      Assert.False(relEnd.Value.Yy);
      Assert.False(relEnd.Value.Zz);
      Assert.Equal(0, angle.Value, 6);
      Assert.Null(orientation.Value);
      Assert.Equal("", name.Value);
      Assert.Equal(0, colour.Value.R);
      Assert.Equal(0, colour.Value.G);
      Assert.Equal(0, colour.Value.B);
      Assert.False(dummy.Value);
      Assert.Equal(0, parentMem.Value);
    }

    [Fact]
    public void InvalidPropertyElementTypeCombination1() {
      GH_OasysComponent comp = ComponentMother();
      var property = new AxialSpringProperty {
        Stiffness = 3.0
      };
      ComponentTestHelper.SetInput(comp, new GsaSpringPropertyGoo(new GsaSpringProperty(property)), 3);
      ComponentTestHelper.SetInput(comp, "Beam", 5);

      comp.Params.Output[0].ExpireSolution(true);
      comp.Params.Output[0].CollectData();
      Assert.Empty(comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Error));
    }

    [Fact]
    public void InvalidPropertyElementTypeCombination2() {
      GH_OasysComponent comp = ComponentMother();
      ComponentTestHelper.SetInput(comp, "STD CH 10 20 30 40", 3);
      ComponentTestHelper.SetInput(comp, "Spring", 5);

      comp.Params.Output[0].ExpireSolution(true);
      comp.Params.Output[0].CollectData();
      Assert.Single(comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Error));
    }

    [Fact]
    public void InvalidPropertyElementTypeCombination3() {
      var comp = new Edit1dElement();
      comp.CreateAttributes();
      ComponentTestHelper.SetInput(comp, "Spring", 5);

      comp.Params.Output[0].ExpireSolution(true);
      comp.Params.Output[0].CollectData();
      Assert.Empty(comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Error));
    }

    [Fact]
    public void ElementReleasesAreCorrectWhenBoolValuesAssignedAsInput() {
      GH_OasysComponent comp = ComponentMother();
      var startReleaseInput = new GsaBool6(true, true, true, false, false, false);
      var endReleaseInput = new GsaBool6(false, false, false, true, true, true);
      ComponentTestHelper.SetInput(comp, new GsaBool6Goo(startReleaseInput), 7);
      ComponentTestHelper.SetInput(comp, new GsaBool6Goo(endReleaseInput), 8);
      GsaBool6 startReleaseOutput = ((GsaBool6Goo)ComponentTestHelper.GetOutput(comp, 7)).Value;
      GsaBool6 endReleaseOutput = ((GsaBool6Goo)ComponentTestHelper.GetOutput(comp, 8)).Value;
      CompareRelease(startReleaseInput, startReleaseOutput);
      CompareRelease(endReleaseInput, endReleaseOutput);
    }

    [Fact]
    public void ElementReleasesAreCorrectWhenStringAssignedAsInput() {
      GH_OasysComponent comp = ComponentMother();
      var startReleaseInput = new GsaBool6(true, true, true, false, false, false);
      var endReleaseInput = new GsaBool6(false, false, false, true, true, true);
      ComponentTestHelper.SetInput(comp, "RRRFFF", 7);
      ComponentTestHelper.SetInput(comp, "FFFRRR", 8);
      GsaBool6 startReleaseOutput = ((GsaBool6Goo)ComponentTestHelper.GetOutput(comp, 7)).Value;
      GsaBool6 endReleaseOutput = ((GsaBool6Goo)ComponentTestHelper.GetOutput(comp, 8)).Value;
      CompareRelease(startReleaseInput, startReleaseOutput);
      CompareRelease(endReleaseInput, endReleaseOutput);
    }


  }
}
