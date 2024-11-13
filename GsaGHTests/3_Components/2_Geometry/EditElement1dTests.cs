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

      ComponentTestHelper.SetInput(comp, ComponentTestHelper.GetOutput(CreateElement1dTests.ComponentMother()), 0);

      return comp;
    }

    [Fact]
    public void ComponentReturnValidElementLinePointAtStartXValue() {
      GsaElement1dGoo element = GetElementOutput();
      Assert.Equal(0, element.Value.Line.PointAtStart.X, 6);
    }

    [Fact]
    public void ComponentReturnValidElementLinePointAtStartYValue() {
      GsaElement1dGoo element = GetElementOutput();
      Assert.Equal(-1, element.Value.Line.PointAtStart.Y, 6);
    }

    [Fact]
    public void ComponentReturnValidElementLinePointAtStartZValue() {
      GsaElement1dGoo element = GetElementOutput();
      Assert.Equal(0, element.Value.Line.PointAtStart.Z, 6);
    }

    [Fact]
    public void ComponentReturnValidElementLinePointAtEndXValue() {
      GsaElement1dGoo element = GetElementOutput();
      Assert.Equal(7, element.Value.Line.PointAtEnd.X, 6);
    }

    [Fact]
    public void ComponentReturnValidElementLinePointAtEndYValue() {
      GsaElement1dGoo element = GetElementOutput();
      Assert.Equal(3, element.Value.Line.PointAtEnd.Y, 6);
    }

    [Fact]
    public void ComponentReturnValidElementLinePointAtEndZValue() {
      GsaElement1dGoo element = GetElementOutput();
      Assert.Equal(1, element.Value.Line.PointAtEnd.Z, 6);
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
    public void EditElementShouldReturnValidElementLinePointAtStartValueX() {
      SetInputForEditElement1d();
      GsaElement1dGoo element = GetElementOutput();
      Assert.Equal(0, element.Value.Line.PointAtStart.X);
    }

    [Fact]
    public void EditElementShouldReturnValidElementLinePointAtStartValueY() {
      SetInputForEditElement1d();
      GsaElement1dGoo element = GetElementOutput();
      Assert.Equal(0, element.Value.Line.PointAtStart.Y);
    }

    [Fact]
    public void EditElementShouldReturnValidElementLinePointAtStartValueZ() {
      SetInputForEditElement1d();
      GsaElement1dGoo element = GetElementOutput();
      Assert.Equal(0, element.Value.Line.PointAtStart.Z);
    }

    [Fact]
    public void EditElementShouldReturnValidElementLinePointAtEndValueX() {
      SetInputForEditElement1d();
      GsaElement1dGoo element = GetElementOutput();
      Assert.Equal(1, element.Value.Line.PointAtEnd.X);
    }

    [Fact]
    public void EditElementShouldReturnValidElementLinePointAtEndValueY() {
      SetInputForEditElement1d();
      GsaElement1dGoo element = GetElementOutput();
      Assert.Equal(2, element.Value.Line.PointAtEnd.Y);
    }

    [Fact]
    public void EditElementShouldReturnValidElementLinePointAtEndValueZ() {
      SetInputForEditElement1d();
      GsaElement1dGoo element = GetElementOutput();
      Assert.Equal(3, element.Value.Line.PointAtEnd.Z);
    }

    [Fact]
    public void EditElementShouldReturnValidSectionProfileFromElement() {
      SetInputForEditElement1d();
      GsaElement1dGoo element = GetElementOutput();
      Assert.Equal("STD CH 10 20 30 40", element.Value.Section.ApiSection.Profile);
    }

    [Fact]
    public void EditElementShouldReturnValidGroupValueFromElement() {
      SetInputForEditElement1d();
      GsaElement1dGoo element = GetElementOutput();
      Assert.Equal(7, element.Value.ApiElement.Group);
    }

    [Fact]
    public void EditElementShouldReturnValidId() {
      SetInputForEditElement1d();
      GH_Integer output = GetIdOutput();
      Assert.Equal(1, output.Value);
    }

    [Fact]
    public void EditElementShouldReturnValidLineFromXValue() {
      SetInputForEditElement1d();
      GH_Line output = GetLineOutput();
      Assert.Equal(0, output.Value.From.X);
    }

    [Fact]
    public void EditElementShouldReturnValidLineFromYValue() {
      SetInputForEditElement1d();
      GH_Line output = GetLineOutput();
      Assert.Equal(0, output.Value.From.Y);
    }

    [Fact]
    public void EditElementShouldReturnValidLineFromZValue() {
      SetInputForEditElement1d();
      GH_Line output = GetLineOutput();
      Assert.Equal(0, output.Value.From.Z);
    }

    [Fact]
    public void EditElementShouldReturnValidLineToXValue() {
      SetInputForEditElement1d();
      GH_Line output = GetLineOutput();
      Assert.Equal(1, output.Value.To.X);
    }

    [Fact]
    public void EditElementShouldReturnValidLineToYValue() {
      SetInputForEditElement1d();
      GH_Line output = GetLineOutput();
      Assert.Equal(2, output.Value.To.Y);
    }

    [Fact]
    public void EditElementShouldReturnValidLineToZValue() {
      SetInputForEditElement1d();
      GH_Line output = GetLineOutput();
      Assert.Equal(3, output.Value.To.Z);
    }

    [Fact]
    public void EditElementShouldReturnValidSectionProfileValue() {
      SetInputForEditElement1d();
      GsaPropertyGoo output = GetSectionOutput();
      Assert.Equal("STD CH 10 20 30 40", ((GsaSection)output.Value).ApiSection.Profile);
    }

    [Fact]
    public void EditElementShouldReturnValidGroupValue() {
      SetInputForEditElement1d();
      GH_Integer output = GetGroupOutput();
      Assert.Equal(7, output.Value);
    }

    [Fact]
    public void EditElementShouldReturnValidTypeValue() {
      SetInputForEditElement1d();
      GH_String output = GetTypeOutput();
      Assert.Equal("Beam", output.Value);
    }

    [Fact]
    public void EditElementShouldReturnValidOffsetX1Value() {
      SetInputForEditElement1d();
      GsaOffsetGoo output = GetOffsetOutput();
      Assert.Equal(1, output.Value.X1.Value);
    }

    [Fact]
    public void EditElementShouldReturnValidOffsetX2Value() {
      SetInputForEditElement1d();
      GsaOffsetGoo output = GetOffsetOutput();
      Assert.Equal(2, output.Value.X2.Value);
    }

    [Fact]
    public void EditElementShouldReturnValidOffsetYValue() {
      SetInputForEditElement1d();
      GsaOffsetGoo output = GetOffsetOutput();
      Assert.Equal(3, output.Value.Y.Value);
    }

    [Fact]
    public void EditElementShouldReturnValidOffsetZValue() {
      SetInputForEditElement1d();
      GsaOffsetGoo output = GetOffsetOutput();
      Assert.Equal(4, output.Value.Z.Value);
    }

    [Fact]
    public void EditElementShouldReturnValidStartReleaseXValue() {
      SetInputForEditElement1d();
      GsaBool6Goo output = GetStartReleaseOutput();
      Assert.True(output.Value.X);
    }

    [Fact]
    public void EditElementShouldReturnValidStartReleaseYValue() {
      SetInputForEditElement1d();
      GsaBool6Goo output = GetStartReleaseOutput();
      Assert.True(output.Value.Y);
    }

    [Fact]
    public void EditElementShouldReturnValidStartReleaseZValue() {
      SetInputForEditElement1d();
      GsaBool6Goo output = GetStartReleaseOutput();
      Assert.True(output.Value.X);
    }

    [Fact]
    public void EditElementShouldReturnValidStartReleaseXxValue() {
      SetInputForEditElement1d();
      GsaBool6Goo output = GetStartReleaseOutput();
      Assert.True(output.Value.Xx);
    }

    [Fact]
    public void EditElementShouldReturnValidStartReleaseYyValue() {
      SetInputForEditElement1d();
      GsaBool6Goo output = GetStartReleaseOutput();
      Assert.True(output.Value.Yy);
    }

    [Fact]
    public void EditElementShouldReturnValidStartReleaseZzValue() {
      SetInputForEditElement1d();
      GsaBool6Goo output = GetStartReleaseOutput();
      Assert.True(output.Value.Zz);
    }

    [Fact]
    public void EditElementShouldReturnValidEndReleaseXValue() {
      SetInputForEditElement1d();
      GsaBool6Goo output = GetEndReleaseOutput();
      Assert.True(output.Value.X);
    }

    [Fact]
    public void EditElementShouldReturnValidEndReleaseYValue() {
      SetInputForEditElement1d();
      GsaBool6Goo output = GetEndReleaseOutput();
      Assert.True(output.Value.Y);
    }

    [Fact]
    public void EditElementShouldReturnValidEndReleaseZValue() {
      SetInputForEditElement1d();
      GsaBool6Goo output = GetEndReleaseOutput();
      Assert.True(output.Value.X);
    }

    [Fact]
    public void EditElementShouldReturnValidEndReleaseXxValue() {
      SetInputForEditElement1d();
      GsaBool6Goo output = GetEndReleaseOutput();
      Assert.True(output.Value.Xx);
    }

    [Fact]
    public void EditElementShouldReturnValidEndReleaseYyValue() {
      SetInputForEditElement1d();
      GsaBool6Goo output = GetEndReleaseOutput();
      Assert.True(output.Value.Yy);
    }

    [Fact]
    public void EditElementShouldReturnValidEndtReleaseZzValue() {
      SetInputForEditElement1d();
      GsaBool6Goo output = GetEndReleaseOutput();
      Assert.True(output.Value.Zz);
    }

    [Fact]
    public void EditElementShouldReturnValidAngleValue() {
      SetInputForEditElement1d();
      GH_Number output = GetAngleOutput();
      Assert.Equal(Math.PI, output.Value);
    }

    [Fact]
    public void EditElementShouldReturnValidOrientationYValue() {
      SetInputForEditElement1d();
      GsaNodeGoo output = GetOrientationOutput();
      Assert.Equal(2, output.Value.Point.Y);
    }

    [Fact]
    public void EditElementShouldReturnValidOrientationZValue() {
      SetInputForEditElement1d();
      GsaNodeGoo output = GetOrientationOutput();
      Assert.Equal(3, output.Value.Point.Z);
    }

    [Fact]
    public void EditElementShouldReturnValidOrientationIdValue() {
      SetInputForEditElement1d();
      GsaNodeGoo output = GetOrientationOutput();
      Assert.Equal(99, output.Value.Id);
    }

    [Fact]
    public void EditElementShouldReturnValidNameValue() {
      SetInputForEditElement1d();
      GH_String output = GetNameOutput();
      Assert.Equal("name", output.Value);
    }

    [Fact]
    public void EditElementShouldReturnValidColorValue() {
      SetInputForEditElement1d();
      GH_Colour output = GetColorOutput();
      Assert.Equal("ffffffff", output.Value.Name);
    }

    [Fact]
    public void EditElementShouldReturnValidDummyValue() {
      SetInputForEditElement1d();
      GH_Boolean output = GetDummyOutput();
      Assert.True(output.Value);
    }

    [Fact]
    public void EditElementShouldReturnValidParentMemberValue() {
      SetInputForEditElement1d();
      GH_Integer output = GetParentMemberOutput();
      Assert.Equal(0, output.Value);
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

    private void SetInputForEditElement1d() {
      SetIdInput();
      SetLineInput();
      SetSectionInput();
      SetGroupInput(7);
      SetTypeInput("Beam");
      SetOffsetInput(new GsaOffsetGoo(new GsaOffset(1, 2, 3, 4)));
      SetStartReleaseInput(new GsaBool6Goo(new GsaBool6(true, true, true, true, true, true)));
      SetEndReleaseInput(new GsaBool6Goo(new GsaBool6(true, true, true, true, true, true)));
      SetAngleInput(Math.PI);
      SetOrientationInput(new GsaNodeGoo(new GsaNode(new Point3d(1, 2, 3)) {
        Id = 99,
      }));
      SetNameInput("name");
      SetColorInput(new GH_Colour(Color.White));
      SetDummyInput(true);
    }

    #region helper methods: get output

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

    #endregion

    #region helper methods: set input

    private void SetIdInput(int id = 1) {
      ComponentTestHelper.SetInput(_component, id, 1);
    }

    private void SetLineInput() {
      ComponentTestHelper.SetInput(_component, new LineCurve(new Point3d(0, 0, 0), new Point3d(1, 2, 3)), 2);
    }

    private void SetSectionInput(string profile = "STD CH 10 20 30 40") {
      ComponentTestHelper.SetInput(_component, profile, 3);
    }

    private void SetGroupInput(int id = 7) {
      ComponentTestHelper.SetInput(_component, id, 4);
    }

    private void SetTypeInput(string type) {
      ComponentTestHelper.SetInput(_component, type, 5);
    }

    private void SetOffsetInput(GsaOffsetGoo offset) {
      ComponentTestHelper.SetInput(_component, offset, 6);
    }

    private void SetStartReleaseInput(GsaBool6Goo release) {
      ComponentTestHelper.SetInput(_component, release, 7);
    }

    private void SetEndReleaseInput(GsaBool6Goo release) {
      ComponentTestHelper.SetInput(_component, release, 8);
    }

    private void SetAngleInput(double angle) {
      ComponentTestHelper.SetInput(_component, angle, 9);
    }

    private void SetOrientationInput(GsaNodeGoo orientation) {
      ComponentTestHelper.SetInput(_component, orientation, 10);
    }

    private void SetNameInput(string name) {
      ComponentTestHelper.SetInput(_component, name, 11);
    }

    private void SetColorInput(GH_Colour color) {
      ComponentTestHelper.SetInput(_component, color, 12);
    }

    private void SetDummyInput(bool dummy) {
      ComponentTestHelper.SetInput(_component, dummy, 13);
    }

    #endregion

  }
}
