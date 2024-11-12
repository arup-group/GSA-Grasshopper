using System;
using System.Drawing;

using Grasshopper.Kernel;
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
    private readonly GH_OasysComponent _component;

    public EditElement1dTests() {
      _component = ComponentMother();
    }

    public static GH_OasysComponent ComponentMother() {
      var comp = new Edit1dElement();
      comp.CreateAttributes();

      ComponentTestHelper.SetInput(comp,
        ComponentTestHelper.GetOutput(CreateElement1dTests.ComponentMother()), 0);

      return comp;
    }

    [Fact]
    public void ComponentReturnValidLinePointAtStartXValue() {
      GsaElement1dGoo element = GetElementOutput();
      Assert.Equal(0, element.Value.Line.PointAtStart.X, 6);
    }

    [Fact]
    public void ComponentReturnValidLinePointAtStartYValue() {
      GsaElement1dGoo element = GetElementOutput();
      Assert.Equal(-1, element.Value.Line.PointAtStart.Y, 6);
    }

    [Fact]
    public void ComponentReturnValidLinePointAtStartZValue() {
      GsaElement1dGoo element = GetElementOutput();
      Assert.Equal(0, element.Value.Line.PointAtStart.Z, 6);
    }

    [Fact]
    public void ComponentReturnValidLinePointAtEndXValue() {
      GsaElement1dGoo element = GetElementOutput();
      Assert.Equal(7, element.Value.Line.PointAtStart.X, 6);
    }

    [Fact]
    public void ComponentReturnValidLinePointAtEndYValue() {
      GsaElement1dGoo element = GetElementOutput();
      Assert.Equal(3, element.Value.Line.PointAtStart.Y, 6);
    }

    [Fact]
    public void ComponentReturnValidLinePointAtEndZValue() {
      GsaElement1dGoo element = GetElementOutput();
      Assert.Equal(1, element.Value.Line.PointAtStart.Z, 6);
    }

    [Fact]
    public void ComponentReturnValidSectionProfile() {
      GsaElement1dGoo element = GetElementOutput();
      Assert.Equal("STD CH(ft) 1 2 3 4", element.Value.Section.ApiSection.Profile);
    }

    [Fact]
    public void ComponentReturnValidApiElementGroupValue() {
      GsaElement1dGoo element = GetElementOutput();
      Assert.Equal(1, element.Value.ApiElement.Group);
    }

    [Fact]
    public void ComponentReturnValidIdValue() {
      GH_Integer id = GetIdOutput();
      Assert.Equal(0, id.Value);
    }

    [Fact]
    public void ComponentReturnValidLineFromXValue() {
      GH_Line line = GetLineOutput();
      Assert.Equal(0, line.Value.From.X, 6);
    }

    [Fact]
    public void ComponentReturnValidLineFromYValue() {
      GH_Line line = GetLineOutput();
      Assert.Equal(-1, line.Value.From.Y, 6);
    }

    [Fact]
    public void ComponentReturnValidLineFromZValue() {
      GH_Line line = GetLineOutput();
      Assert.Equal(0, line.Value.From.Z, 6);
    }

    [Fact]
    public void ComponentReturnValidLineToXValue() {
      GH_Line line = GetLineOutput();
      Assert.Equal(7, line.Value.To.X, 6);
    }

    [Fact]
    public void ComponentReturnValidLineToYValue() {
      GH_Line line = GetLineOutput();
      Assert.Equal(3, line.Value.To.Y, 6);
    }

    [Fact]
    public void ComponentReturnValidLineToZValue() {
      GH_Line line = GetLineOutput();
      Assert.Equal(1, line.Value.To.Z, 6);
    }

    [Fact]
    public void ComponentReturnValidSectionProfileFromInput() {
      GsaPropertyGoo section = GetSectionOutput();
      Assert.Equal("STD CH(ft) 1 2 3 4", ((GsaSection)section.Value).ApiSection.Profile);
    }

    [Fact]
    public void ComponentReturnDefaultGroupValue() {
      GH_Integer group = GetGroupOutput();
      Assert.Equal(1, group.Value);
    }

    [Fact]
    public void ComponentReturnValidType() {
      GH_String type = GetTypeOutput();
      Assert.Equal("Beam", type.Value);
    }

    [Fact]
    public void ComponentReturnValidOffsetX1() {
      GsaOffsetGoo offset = GetOffsetOutput();
      Assert.Equal(0, offset.Value.X1.Value, 6);
    }

    [Fact]
    public void ComponentReturnValidOffsetX2() {
      GsaOffsetGoo offset = GetOffsetOutput();
      Assert.Equal(0, offset.Value.X2.Value, 6);
    }

    [Fact]
    public void ComponentReturnValidOffsetY() {
      GsaOffsetGoo offset = GetOffsetOutput();
      Assert.Equal(0, offset.Value.Y.Value, 6);
    }

    [Fact]
    public void ComponentReturnValidOffsetZ() {
      GsaOffsetGoo offset = GetOffsetOutput();
      Assert.Equal(0, offset.Value.Z.Value, 6);
    }

    [Fact]
    public void ComponentReturnValidReleaseStartX() {
      GsaBool6Goo startRelease = GetStartReleaseOutput();
      Assert.False(startRelease.Value.X);
    }

    [Fact]
    public void ComponentReturnValidReleaseStartY() {
      GsaBool6Goo startRelease = GetStartReleaseOutput();
      Assert.False(startRelease.Value.Y);
    }

    [Fact]
    public void ComponentReturnValidReleaseStartZ() {
      GsaBool6Goo startRelease = GetStartReleaseOutput();
      Assert.False(startRelease.Value.Z);
    }

    [Fact]
    public void ComponentReturnValidReleaseStartXx() {
      GsaBool6Goo startRelease = GetStartReleaseOutput();
      Assert.False(startRelease.Value.Xx);
    }

    [Fact]
    public void ComponentReturnValidReleaseStartYy() {
      GsaBool6Goo startRelease = GetStartReleaseOutput();
      Assert.False(startRelease.Value.Yy);
    }

    [Fact]
    public void ComponentReturnValidReleaseStartZz() {
      GsaBool6Goo startRelease = GetStartReleaseOutput();
      Assert.False(startRelease.Value.Zz);
    }

    [Fact]
    public void ComponentReturnValidReleaseEndX() {
      GsaBool6Goo endRelease = GetEndReleaseOutput();
      Assert.False(endRelease.Value.X);
    }

    [Fact]
    public void ComponentReturnValidReleaseEndY() {
      GsaBool6Goo endRelease = GetEndReleaseOutput();
      Assert.False(endRelease.Value.Y);
    }

    [Fact]
    public void ComponentReturnValidReleaseEndZ() {
      GsaBool6Goo endRelease = GetEndReleaseOutput();
      Assert.False(endRelease.Value.Z);
    }

    [Fact]
    public void ComponentReturnValidReleaseEndXx() {
      GsaBool6Goo endRelease = GetEndReleaseOutput();
      Assert.False(endRelease.Value.Xx);
    }

    [Fact]
    public void ComponentReturnValidReleaseEndYy() {
      GsaBool6Goo endRelease = GetEndReleaseOutput();
      Assert.False(endRelease.Value.Yy);
    }

    [Fact]
    public void ComponentReturnValidReleaseEndZz() {
      GsaBool6Goo endRelease = GetEndReleaseOutput();
      Assert.False(endRelease.Value.Zz);
    }

    [Fact]
    public void ComponentReturnDefaultAngleValue() {
      GH_Number angle = GetAngleOutput();
      Assert.Equal(0, angle.Value);
    }

    [Fact]
    public void ComponentReturnNullOrientationValue() {
      GsaNodeGoo orientation = GetOrientationOutput();
      Assert.Null(orientation.Value);
    }

    [Fact]
    public void ComponentReturnEmptyNameValue() {
      GH_String name = GetNameOutput();
      Assert.Empty(name.Value);
    }

    [Fact]
    public void ComponentReturnDefaultColorValue() {
      GH_Colour colour = GetColorOutput();
      Assert.Equal("ff000000", colour.Value.Name);
    }

    [Fact]
    public void ComponentReturnDefaultDummyValue() {
      GH_Boolean dummy = GetDummyOutput();
      Assert.False(dummy.Value);
    }

    [Fact]
    public void ComponentShouldReturnParentMemberDefaultValue() {
      GH_Integer parentMember = GetParentMemberOutput();
      Assert.Equal(0, parentMember.Value);
    }

    [Fact]
    public void EditElementByInputs() {
      ComponentTestHelper.SetInput(_component, 1, 1);
      ComponentTestHelper.SetInput(_component, new LineCurve(new Point3d(0, 0, 0), new Point3d(1, 2, 3)), 2);
      ComponentTestHelper.SetInput(_component, "STD CH 10 20 30 40", 3);
      ComponentTestHelper.SetInput(_component, 7, 4);
      ComponentTestHelper.SetInput(_component, "Beam", 5);
      ComponentTestHelper.SetInput(_component, new GsaOffsetGoo(new GsaOffset(1, 2, 3, 4)), 6);
      ComponentTestHelper.SetInput(_component, new GsaBool6Goo(new GsaBool6(true, true, true, true, true, true)), 7);
      ComponentTestHelper.SetInput(_component, new GsaBool6Goo(new GsaBool6(true, true, true, true, true, true)), 8);
      ComponentTestHelper.SetInput(_component, Math.PI, 9);
      var node = new GsaNode(new Point3d(1, 2, 3)) {
        Id = 99
      };
      ComponentTestHelper.SetInput(_component, new GsaNodeGoo(node), 10);
      ComponentTestHelper.SetInput(_component, "name", 11);
      ComponentTestHelper.SetInput(_component, new GH_Colour(Color.White), 12);
      ComponentTestHelper.SetInput(_component, true, 13);

      var element = (GsaElement1dGoo)ComponentTestHelper.GetOutput(_component, 0);
      var id = (GH_Integer)ComponentTestHelper.GetOutput(_component, 1);
      var line = (GH_Line)ComponentTestHelper.GetOutput(_component, 2);
      var section = (GsaPropertyGoo)ComponentTestHelper.GetOutput(_component, 3);
      var group = (GH_Integer)ComponentTestHelper.GetOutput(_component, 4);
      var type = (GH_String)ComponentTestHelper.GetOutput(_component, 5);
      var offset = (GsaOffsetGoo)ComponentTestHelper.GetOutput(_component, 6);
      var startRelease = (GsaBool6Goo)ComponentTestHelper.GetOutput(_component, 7);
      var relEnd = (GsaBool6Goo)ComponentTestHelper.GetOutput(_component, 8);
      var angle = (GH_Number)ComponentTestHelper.GetOutput(_component, 9);
      var orientation = (GsaNodeGoo)ComponentTestHelper.GetOutput(_component, 10);
      var name = (GH_String)ComponentTestHelper.GetOutput(_component, 11);
      var colour = (GH_Colour)ComponentTestHelper.GetOutput(_component, 12);
      var dummy = (GH_Boolean)ComponentTestHelper.GetOutput(_component, 13);
      var parentMem = (GH_Integer)ComponentTestHelper.GetOutput(_component, 14);

      Assert.Equal(0, element.Value.Line.PointAtStart.X);
      Assert.Equal(0, element.Value.Line.PointAtStart.Y);
      Assert.Equal(0, element.Value.Line.PointAtStart.Z);
      Assert.Equal(1, element.Value.Line.PointAtEnd.X);
      Assert.Equal(2, element.Value.Line.PointAtEnd.Y);
      Assert.Equal(3, element.Value.Line.PointAtEnd.Z);
      Assert.Equal("STD CH 10 20 30 40", element.Value.Section.ApiSection.Profile);
      Assert.Equal(7, element.Value.ApiElement.Group);
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
      Assert.True(startRelease.Value.X);
      Assert.True(startRelease.Value.Y);
      Assert.True(startRelease.Value.Z);
      Assert.True(startRelease.Value.Xx);
      Assert.True(startRelease.Value.Yy);
      Assert.True(startRelease.Value.Zz);
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
      ComponentTestHelper.SetInput(_component, "1", 5); // set beam type by int
      var type = (GH_String)ComponentTestHelper.GetOutput(_component, 5, 0, 0, true);
      Assert.Equal("Bar", type.Value);
    }

    [Fact]
    public void ChangeToSpringElement() {
      var property = new AxialSpringProperty {
        Stiffness = 3.0,
      };
      ComponentTestHelper.SetInput(_component, new GsaPropertyGoo(new GsaSpringProperty(property)), 3);
      ComponentTestHelper.SetInput(_component, "Spring", 5);

      var element = (GsaElement1dGoo)ComponentTestHelper.GetOutput(_component, 0);
      var id = (GH_Integer)ComponentTestHelper.GetOutput(_component, 1);
      var line = (GH_Line)ComponentTestHelper.GetOutput(_component, 2);
      var springProperty = (GsaPropertyGoo)ComponentTestHelper.GetOutput(_component, 3);
      var group = (GH_Integer)ComponentTestHelper.GetOutput(_component, 4);
      var type = (GH_String)ComponentTestHelper.GetOutput(_component, 5);
      var offset = (GsaOffsetGoo)ComponentTestHelper.GetOutput(_component, 6);
      var startRelease = (GsaBool6Goo)ComponentTestHelper.GetOutput(_component, 7);
      var relEnd = (GsaBool6Goo)ComponentTestHelper.GetOutput(_component, 8);
      var angle = (GH_Number)ComponentTestHelper.GetOutput(_component, 9);
      var orientation = (GsaNodeGoo)ComponentTestHelper.GetOutput(_component, 10);
      var name = (GH_String)ComponentTestHelper.GetOutput(_component, 11);
      var colour = (GH_Colour)ComponentTestHelper.GetOutput(_component, 12);
      var dummy = (GH_Boolean)ComponentTestHelper.GetOutput(_component, 13);
      var parentMem = (GH_Integer)ComponentTestHelper.GetOutput(_component, 14);

      Assert.Equal(0, element.Value.Line.PointAtStart.X, 6);
      Assert.Equal(-1, element.Value.Line.PointAtStart.Y, 6);
      Assert.Equal(0, element.Value.Line.PointAtStart.Z, 6);
      Assert.Equal(7, element.Value.Line.PointAtEnd.X, 6);
      Assert.Equal(3, element.Value.Line.PointAtEnd.Y, 6);
      Assert.Equal(1, element.Value.Line.PointAtEnd.Z, 6);
      Assert.Null(element.Value.Section);
      Assert.Equal(1, element.Value.ApiElement.Group);
      Assert.Equal(0, id.Value);
      Assert.Equal(0, line.Value.From.X, 6);
      Assert.Equal(-1, line.Value.From.Y, 6);
      Assert.Equal(0, line.Value.From.Z, 6);
      Assert.Equal(7, line.Value.To.X, 6);
      Assert.Equal(3, line.Value.To.Y, 6);
      Assert.Equal(1, line.Value.To.Z, 6);
      Assert.NotNull(((GsaSpringProperty)springProperty.Value).ApiProperty);
      Assert.Equal(1, group.Value);
      Assert.Equal("Spring", type.Value);
      Assert.Equal(0, offset.Value.X1.Value, 6);
      Assert.Equal(0, offset.Value.X2.Value, 6);
      Assert.Equal(0, offset.Value.Y.Value, 6);
      Assert.Equal(0, offset.Value.Z.Value, 6);
      Assert.False(startRelease.Value.X);
      Assert.False(startRelease.Value.Y);
      Assert.False(startRelease.Value.Z);
      Assert.False(startRelease.Value.Xx);
      Assert.False(startRelease.Value.Yy);
      Assert.False(startRelease.Value.Zz);
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
      var property = new AxialSpringProperty {
        Stiffness = 3.0,
      };
      ComponentTestHelper.SetInput(_component, new GsaSpringPropertyGoo(new GsaSpringProperty(property)), 3);
      ComponentTestHelper.SetInput(_component, "Beam", 5);

      _component.Params.Output[0].ExpireSolution(true);
      _component.Params.Output[0].CollectData();
      Assert.Empty(_component.RuntimeMessages(GH_RuntimeMessageLevel.Error));
    }

    [Fact]
    public void InvalidPropertyElementTypeCombination2() {
      ComponentTestHelper.SetInput(_component, "STD CH 10 20 30 40", 3);
      ComponentTestHelper.SetInput(_component, "Spring", 5);

      _component.Params.Output[0].ExpireSolution(true);
      _component.Params.Output[0].CollectData();
      Assert.Single(_component.RuntimeMessages(GH_RuntimeMessageLevel.Error));
    }

    [Fact]
    public void InvalidPropertyElementTypeCombination3() {
      var comp = new Edit1dElement();
      comp.CreateAttributes();
      ComponentTestHelper.SetInput(_component, "Spring", 5);

      comp.Params.Output[0].ExpireSolution(true);
      comp.Params.Output[0].CollectData();
      Assert.Empty(comp.RuntimeMessages(GH_RuntimeMessageLevel.Error));
    }

    private GsaElement1dGoo GetElementOutput() {
      var element = (GsaElement1dGoo)ComponentTestHelper.GetOutput(_component, 0);
      return element;
    }

    private GH_Integer GetIdOutput() {
      var id = (GH_Integer)ComponentTestHelper.GetOutput(_component, 1);
      return id;
    }

    private GH_Line GetLineOutput() {
      var line = (GH_Line)ComponentTestHelper.GetOutput(_component, 2);
      return line;
    }

    private GsaPropertyGoo GetSectionOutput() {
      var section = (GsaPropertyGoo)ComponentTestHelper.GetOutput(_component, 3);
      return section;
    }

    private GH_Integer GetGroupOutput() {
      var group = (GH_Integer)ComponentTestHelper.GetOutput(_component, 4);
      return group;
    }

    private GH_String GetTypeOutput() {
      var type = (GH_String)ComponentTestHelper.GetOutput(_component, 5);
      return type;
    }

    private GsaOffsetGoo GetOffsetOutput() {
      var offset = (GsaOffsetGoo)ComponentTestHelper.GetOutput(_component, 6);
      return offset;
    }

    private GsaBool6Goo GetStartReleaseOutput() {
      var startRelease = (GsaBool6Goo)ComponentTestHelper.GetOutput(_component, 7);
      return startRelease;
    }

    private GsaBool6Goo GetEndReleaseOutput() {
      var endRelease = (GsaBool6Goo)ComponentTestHelper.GetOutput(_component, 8);
      return endRelease;
    }

    private GH_Number GetAngleOutput() {
      var angle = (GH_Number)ComponentTestHelper.GetOutput(_component, 9);
      return angle;
    }

    private GsaNodeGoo GetOrientationOutput() {
      var orientation = (GsaNodeGoo)ComponentTestHelper.GetOutput(_component, 10);
      return orientation;
    }

    private GH_String GetNameOutput() {
      var name = (GH_String)ComponentTestHelper.GetOutput(_component, 11);
      return name;
    }

    private GH_Colour GetColorOutput() {
      var colour = (GH_Colour)ComponentTestHelper.GetOutput(_component, 12);
      return colour;
    }

    private GH_Boolean GetDummyOutput() {
      var dummy = (GH_Boolean)ComponentTestHelper.GetOutput(_component, 13);
      return dummy;
    }

    private GH_Integer GetParentMemberOutput() {
      var parentMember = (GH_Integer)ComponentTestHelper.GetOutput(_component, 14);
      return parentMember;
    }
  }
}
