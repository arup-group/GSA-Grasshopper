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
  public class EditElement1dTests_WithoutSettingInputs {
    public readonly EditElement1dTestsHelper _helper;

    public EditElement1dTests_WithoutSettingInputs() {
      _helper = new EditElement1dTestsHelper();
    }

    [Fact]
    public void ComponentReturnValidElementLinePointAtStartXValue() {
      GsaElement1dGoo element = _helper.GetElementOutput();
      Assert.Equal(0, element.Value.Line.PointAtStart.X, 6);
    }

    [Fact]
    public void ComponentReturnValidElementLinePointAtStartYValue() {
      GsaElement1dGoo element = _helper.GetElementOutput();
      Assert.Equal(-1, element.Value.Line.PointAtStart.Y, 6);
    }

    [Fact]
    public void ComponentReturnValidElementLinePointAtStartZValue() {
      GsaElement1dGoo element = _helper.GetElementOutput();
      Assert.Equal(0, element.Value.Line.PointAtStart.Z, 6);
    }

    [Fact]
    public void ComponentReturnValidElementLinePointAtEndXValue() {
      GsaElement1dGoo element = _helper.GetElementOutput();
      Assert.Equal(7, element.Value.Line.PointAtEnd.X, 6);
    }

    [Fact]
    public void ComponentReturnValidElementLinePointAtEndYValue() {
      GsaElement1dGoo element = _helper.GetElementOutput();
      Assert.Equal(3, element.Value.Line.PointAtEnd.Y, 6);
    }

    [Fact]
    public void ComponentReturnValidElementLinePointAtEndZValue() {
      GsaElement1dGoo element = _helper.GetElementOutput();
      Assert.Equal(1, element.Value.Line.PointAtEnd.Z, 6);
    }

    [Fact]
    public void ComponentReturnValidSectionProfile() {
      GsaElement1dGoo element = _helper.GetElementOutput();
      Assert.Equal("STD CH(ft) 1 2 3 4", element.Value.Section.ApiSection.Profile);
    }

    [Fact]
    public void ComponentReturnValidApiElementGroupValue() {
      GsaElement1dGoo element = _helper.GetElementOutput();
      Assert.Equal(1, element.Value.ApiElement.Group);
    }

    [Fact]
    public void ComponentReturnValidIdValue() {
      GH_Integer id = _helper.GetIdOutput();
      Assert.Equal(0, id.Value);
    }

    [Fact]
    public void ComponentReturnValidLineFromXValue() {
      GH_Line line = _helper.GetLineOutput();
      Assert.Equal(0, line.Value.From.X, 6);
    }

    [Fact]
    public void ComponentReturnValidLineFromYValue() {
      GH_Line line = _helper.GetLineOutput();
      Assert.Equal(-1, line.Value.From.Y, 6);
    }

    [Fact]
    public void ComponentReturnValidLineFromZValue() {
      GH_Line line = _helper.GetLineOutput();
      Assert.Equal(0, line.Value.From.Z, 6);
    }

    [Fact]
    public void ComponentReturnValidLineToXValue() {
      GH_Line line = _helper.GetLineOutput();
      Assert.Equal(7, line.Value.To.X, 6);
    }

    [Fact]
    public void ComponentReturnValidLineToYValue() {
      GH_Line line = _helper.GetLineOutput();
      Assert.Equal(3, line.Value.To.Y, 6);
    }

    [Fact]
    public void ComponentReturnValidLineToZValue() {
      GH_Line line = _helper.GetLineOutput();
      Assert.Equal(1, line.Value.To.Z, 6);
    }

    [Fact]
    public void ComponentReturnValidSectionProfileFromInput() {
      GsaPropertyGoo section = _helper.GetSectionOutput();
      Assert.Equal("STD CH(ft) 1 2 3 4", ((GsaSection)section.Value).ApiSection.Profile);
    }

    [Fact]
    public void ComponentReturnDefaultGroupValue() {
      GH_Integer group = _helper.GetGroupOutput();
      Assert.Equal(1, group.Value);
    }

    [Fact]
    public void ComponentReturnValidType() {
      GH_String type = _helper.GetTypeOutput();
      Assert.Equal("Beam", type.Value);
    }

    [Fact]
    public void ComponentReturnValidOffsetX1() {
      GsaOffsetGoo offset = _helper.GetOffsetOutput();
      Assert.Equal(0, offset.Value.X1.Value, 6);
    }

    [Fact]
    public void ComponentReturnValidOffsetX2() {
      GsaOffsetGoo offset = _helper.GetOffsetOutput();
      Assert.Equal(0, offset.Value.X2.Value, 6);
    }

    [Fact]
    public void ComponentReturnValidOffsetY() {
      GsaOffsetGoo offset = _helper.GetOffsetOutput();
      Assert.Equal(0, offset.Value.Y.Value, 6);
    }

    [Fact]
    public void ComponentReturnValidOffsetZ() {
      GsaOffsetGoo offset = _helper.GetOffsetOutput();
      Assert.Equal(0, offset.Value.Z.Value, 6);
    }

    [Fact]
    public void ComponentReturnValidReleaseStartX() {
      GsaBool6Goo startRelease = _helper.GetStartReleaseOutput();
      Assert.False(startRelease.Value.X);
    }

    [Fact]
    public void ComponentReturnValidReleaseStartY() {
      GsaBool6Goo startRelease = _helper.GetStartReleaseOutput();
      Assert.False(startRelease.Value.Y);
    }

    [Fact]
    public void ComponentReturnValidReleaseStartZ() {
      GsaBool6Goo startRelease = _helper.GetStartReleaseOutput();
      Assert.False(startRelease.Value.Z);
    }

    [Fact]
    public void ComponentReturnValidReleaseStartXx() {
      GsaBool6Goo startRelease = _helper.GetStartReleaseOutput();
      Assert.False(startRelease.Value.Xx);
    }

    [Fact]
    public void ComponentReturnValidReleaseStartYy() {
      GsaBool6Goo startRelease = _helper.GetStartReleaseOutput();
      Assert.False(startRelease.Value.Yy);
    }

    [Fact]
    public void ComponentReturnValidReleaseStartZz() {
      GsaBool6Goo startRelease = _helper.GetStartReleaseOutput();
      Assert.False(startRelease.Value.Zz);
    }

    [Fact]
    public void ComponentReturnValidReleaseEndX() {
      GsaBool6Goo endRelease = _helper.GetEndReleaseOutput();
      Assert.False(endRelease.Value.X);
    }

    [Fact]
    public void ComponentReturnValidReleaseEndY() {
      GsaBool6Goo endRelease = _helper.GetEndReleaseOutput();
      Assert.False(endRelease.Value.Y);
    }

    [Fact]
    public void ComponentReturnValidReleaseEndZ() {
      GsaBool6Goo endRelease = _helper.GetEndReleaseOutput();
      Assert.False(endRelease.Value.Z);
    }

    [Fact]
    public void ComponentReturnValidReleaseEndXx() {
      GsaBool6Goo endRelease = _helper.GetEndReleaseOutput();
      Assert.False(endRelease.Value.Xx);
    }

    [Fact]
    public void ComponentReturnValidReleaseEndYy() {
      GsaBool6Goo endRelease = _helper.GetEndReleaseOutput();
      Assert.False(endRelease.Value.Yy);
    }

    [Fact]
    public void ComponentReturnValidReleaseEndZz() {
      GsaBool6Goo endRelease = _helper.GetEndReleaseOutput();
      Assert.False(endRelease.Value.Zz);
    }

    [Fact]
    public void ComponentReturnDefaultAngleValue() {
      GH_Number angle = _helper.GetAngleOutput();
      Assert.Equal(0, angle.Value);
    }

    [Fact]
    public void ComponentReturnNullOrientationValue() {
      GsaNodeGoo orientation = _helper.GetOrientationOutput();
      Assert.Null(orientation.Value);
    }

    [Fact]
    public void ComponentReturnEmptyNameValue() {
      GH_String name = _helper.GetNameOutput();
      Assert.Empty(name.Value);
    }

    [Fact]
    public void ComponentReturnDefaultColorValue() {
      GH_Colour colour = _helper.GetColorOutput();
      Assert.Equal("ff000000", colour.Value.Name);
    }

    [Fact]
    public void ComponentReturnDefaultDummyValue() {
      GH_Boolean dummy = _helper.GetDummyOutput();
      Assert.False(dummy.Value);
    }

    [Fact]
    public void ComponentShouldReturnParentMemberDefaultValue() {
      GH_Integer parentMember = _helper.GetParentMemberOutput();
      Assert.Equal(0, parentMember.Value);
    }
  }

  [Collection("GrasshopperFixture collection")]
  public class EditElement1dTests_ForInputsSet {
    public readonly EditElement1dTestsHelper _helper;

    public EditElement1dTests_ForInputsSet() {
      _helper = new EditElement1dTestsHelper();
      _helper.SetIdInput();
      _helper.SetLineInput();
      _helper.SetSectionInput();
      _helper.SetGroupInput(7);
      _helper.SetTypeInput("Beam");
      _helper.SetOffsetInput(new GsaOffsetGoo(new GsaOffset(1, 2, 3, 4)));
      _helper.SetStartReleaseInput(new GsaBool6Goo(new GsaBool6(true, true, true, true, true, true)));
      _helper.SetEndReleaseInput(new GsaBool6Goo(new GsaBool6(true, true, true, true, true, true)));
      _helper.SetAngleInput(Math.PI);
      _helper.SetOrientationInput(new GsaNodeGoo(new GsaNode(new Point3d(1, 2, 3)) {
        Id = 99,
      }));
      _helper.SetNameInput("name");
      _helper.SetColorInput(new GH_Colour(Color.White));
      _helper.SetDummyInput(true);
    }

    [Fact]
    public void EditElementShouldReturnValidElementLinePointAtStartValueX() {
      GsaElement1dGoo element = _helper.GetElementOutput();
      Assert.Equal(0, element.Value.Line.PointAtStart.X);
    }

    [Fact]
    public void EditElementShouldReturnValidElementLinePointAtStartValueY() {
      GsaElement1dGoo element = _helper.GetElementOutput();
      Assert.Equal(0, element.Value.Line.PointAtStart.Y);
    }

    [Fact]
    public void EditElementShouldReturnValidElementLinePointAtStartValueZ() {
      GsaElement1dGoo element = _helper.GetElementOutput();
      Assert.Equal(0, element.Value.Line.PointAtStart.Z);
    }

    [Fact]
    public void EditElementShouldReturnValidElementLinePointAtEndValueX() {
      GsaElement1dGoo element = _helper.GetElementOutput();
      Assert.Equal(1, element.Value.Line.PointAtEnd.X);
    }

    [Fact]
    public void EditElementShouldReturnValidElementLinePointAtEndValueY() {
      GsaElement1dGoo element = _helper.GetElementOutput();
      Assert.Equal(2, element.Value.Line.PointAtEnd.Y);
    }

    [Fact]
    public void EditElementShouldReturnValidElementLinePointAtEndValueZ() {
      GsaElement1dGoo element = _helper.GetElementOutput();
      Assert.Equal(3, element.Value.Line.PointAtEnd.Z);
    }

    [Fact]
    public void EditElementShouldReturnValidSectionProfileFromElement() {
      GsaElement1dGoo element = _helper.GetElementOutput();
      Assert.Equal("STD CH 10 20 30 40", element.Value.Section.ApiSection.Profile);
    }

    [Fact]
    public void EditElementShouldReturnValidGroupValueFromElement() {
      GsaElement1dGoo element = _helper.GetElementOutput();
      Assert.Equal(7, element.Value.ApiElement.Group);
    }

    [Fact]
    public void EditElementShouldReturnValidId() {
      GH_Integer output = _helper.GetIdOutput();
      Assert.Equal(1, output.Value);
    }

    [Fact]
    public void EditElementShouldReturnValidLineFromXValue() {
      GH_Line output = _helper.GetLineOutput();
      Assert.Equal(0, output.Value.From.X);
    }

    [Fact]
    public void EditElementShouldReturnValidLineFromYValue() {
      GH_Line output = _helper.GetLineOutput();
      Assert.Equal(0, output.Value.From.Y);
    }

    [Fact]
    public void EditElementShouldReturnValidLineFromZValue() {
      GH_Line output = _helper.GetLineOutput();
      Assert.Equal(0, output.Value.From.Z);
    }

    [Fact]
    public void EditElementShouldReturnValidLineToXValue() {
      GH_Line output = _helper.GetLineOutput();
      Assert.Equal(1, output.Value.To.X);
    }

    [Fact]
    public void EditElementShouldReturnValidLineToYValue() {
      GH_Line output = _helper.GetLineOutput();
      Assert.Equal(2, output.Value.To.Y);
    }

    [Fact]
    public void EditElementShouldReturnValidLineToZValue() {
      GH_Line output = _helper.GetLineOutput();
      Assert.Equal(3, output.Value.To.Z);
    }

    [Fact]
    public void EditElementShouldReturnValidSectionProfileValue() {
      GsaPropertyGoo output = _helper.GetSectionOutput();
      Assert.Equal("STD CH 10 20 30 40", ((GsaSection)output.Value).ApiSection.Profile);
    }

    [Fact]
    public void EditElementShouldReturnValidGroupValue() {
      GH_Integer output = _helper.GetGroupOutput();
      Assert.Equal(7, output.Value);
    }

    [Fact]
    public void EditElementShouldReturnValidTypeValue() {
      GH_String output = _helper.GetTypeOutput();
      Assert.Equal("Beam", output.Value);
    }

    [Fact]
    public void EditElementShouldReturnValidOffsetX1Value() {
      GsaOffsetGoo output = _helper.GetOffsetOutput();
      Assert.Equal(1, output.Value.X1.Value);
    }

    [Fact]
    public void EditElementShouldReturnValidOffsetX2Value() {
      GsaOffsetGoo output = _helper.GetOffsetOutput();
      Assert.Equal(2, output.Value.X2.Value);
    }

    [Fact]
    public void EditElementShouldReturnValidOffsetYValue() {
      GsaOffsetGoo output = _helper.GetOffsetOutput();
      Assert.Equal(3, output.Value.Y.Value);
    }

    [Fact]
    public void EditElementShouldReturnValidOffsetZValue() {
      GsaOffsetGoo output = _helper.GetOffsetOutput();
      Assert.Equal(4, output.Value.Z.Value);
    }

    [Fact]
    public void EditElementShouldReturnValidStartReleaseXValue() {
      GsaBool6Goo output = _helper.GetStartReleaseOutput();
      Assert.True(output.Value.X);
    }

    [Fact]
    public void EditElementShouldReturnValidStartReleaseYValue() {
      GsaBool6Goo output = _helper.GetStartReleaseOutput();
      Assert.True(output.Value.Y);
    }

    [Fact]
    public void EditElementShouldReturnValidStartReleaseZValue() {
      GsaBool6Goo output = _helper.GetStartReleaseOutput();
      Assert.True(output.Value.X);
    }

    [Fact]
    public void EditElementShouldReturnValidStartReleaseXxValue() {
      GsaBool6Goo output = _helper.GetStartReleaseOutput();
      Assert.True(output.Value.Xx);
    }

    [Fact]
    public void EditElementShouldReturnValidStartReleaseYyValue() {
      GsaBool6Goo output = _helper.GetStartReleaseOutput();
      Assert.True(output.Value.Yy);
    }

    [Fact]
    public void EditElementShouldReturnValidStartReleaseZzValue() {
      GsaBool6Goo output = _helper.GetStartReleaseOutput();
      Assert.True(output.Value.Zz);
    }

    [Fact]
    public void EditElementShouldReturnValidEndReleaseXValue() {
      GsaBool6Goo output = _helper.GetEndReleaseOutput();
      Assert.True(output.Value.X);
    }

    [Fact]
    public void EditElementShouldReturnValidEndReleaseYValue() {
      GsaBool6Goo output = _helper.GetEndReleaseOutput();
      Assert.True(output.Value.Y);
    }

    [Fact]
    public void EditElementShouldReturnValidEndReleaseZValue() {
      GsaBool6Goo output = _helper.GetEndReleaseOutput();
      Assert.True(output.Value.X);
    }

    [Fact]
    public void EditElementShouldReturnValidEndReleaseXxValue() {
      GsaBool6Goo output = _helper.GetEndReleaseOutput();
      Assert.True(output.Value.Xx);
    }

    [Fact]
    public void EditElementShouldReturnValidEndReleaseYyValue() {
      GsaBool6Goo output = _helper.GetEndReleaseOutput();
      Assert.True(output.Value.Yy);
    }

    [Fact]
    public void EditElementShouldReturnValidEndtReleaseZzValue() {
      GsaBool6Goo output = _helper.GetEndReleaseOutput();
      Assert.True(output.Value.Zz);
    }

    [Fact]
    public void EditElementShouldReturnValidAngleValue() {
      GH_Number output = _helper.GetAngleOutput();
      Assert.Equal(Math.PI, output.Value);
    }

    [Fact]
    public void EditElementShouldReturnValidOrientationYValue() {
      GsaNodeGoo output = _helper.GetOrientationOutput();
      Assert.Equal(2, output.Value.Point.Y);
    }

    [Fact]
    public void EditElementShouldReturnValidOrientationZValue() {
      GsaNodeGoo output = _helper.GetOrientationOutput();
      Assert.Equal(3, output.Value.Point.Z);
    }

    [Fact]
    public void EditElementShouldReturnValidOrientationIdValue() {
      GsaNodeGoo output = _helper.GetOrientationOutput();
      Assert.Equal(99, output.Value.Id);
    }

    [Fact]
    public void EditElementShouldReturnValidNameValue() {
      GH_String output = _helper.GetNameOutput();
      Assert.Equal("name", output.Value);
    }

    [Fact]
    public void EditElementShouldReturnValidColorValue() {
      GH_Colour output = _helper.GetColorOutput();
      Assert.Equal("ffffffff", output.Value.Name);
    }

    [Fact]
    public void EditElementShouldReturnValidDummyValue() {
      GH_Boolean output = _helper.GetDummyOutput();
      Assert.True(output.Value);
    }

    [Fact]
    public void EditElementShouldReturnValidParentMemberValue() {
      GH_Integer output = _helper.GetParentMemberOutput();
      Assert.Equal(0, output.Value);
    }
  }

  [Collection("GrasshopperFixture collection")]
  public class EditElement1dTests_ForSpringPropertySet {
    private readonly EditElement1dTestsHelper _helper;

    public EditElement1dTests_ForSpringPropertySet() {
      _helper = new EditElement1dTestsHelper();
      var property = new AxialSpringProperty {
        Stiffness = 3.0,
      };
      _helper.SetSpringPropertyInput(new GsaPropertyGoo(new GsaSpringProperty(property)));
      _helper.SetTypeInput("Spring");
    }

    [Fact]
    public void EditElementShouldReturnValidLinePointAtStartXValue() {
      GsaElement1dGoo output = _helper.GetElementOutput();
      Assert.Equal(0, output.Value.Line.PointAtStart.X, 6);
    }

    [Fact]
    public void EditElementShouldReturnValidLinePointAtStartYValue() {
      GsaElement1dGoo output = _helper.GetElementOutput();
      Assert.Equal(-1, output.Value.Line.PointAtStart.Y, 6);
    }

    [Fact]
    public void EditElementShouldReturnValidLinePointAtStartZValue() {
      GsaElement1dGoo output = _helper.GetElementOutput();
      Assert.Equal(0, output.Value.Line.PointAtStart.Z, 6);
    }

    [Fact]
    public void EditElementShouldReturnValidLinePointAtEndXValue() {
      GsaElement1dGoo output = _helper.GetElementOutput();
      Assert.Equal(7, output.Value.Line.PointAtEnd.X, 6);
    }

    [Fact]
    public void EditElementShouldReturnValidLinePointAtEndYValue() {
      GsaElement1dGoo output = _helper.GetElementOutput();
      Assert.Equal(3, output.Value.Line.PointAtEnd.Y, 6);
    }

    [Fact]
    public void EditElementShouldReturnValidLinePointAtEndZValue() {
      GsaElement1dGoo output = _helper.GetElementOutput();
      Assert.Equal(1, output.Value.Line.PointAtEnd.Z, 6);
    }

    [Fact]
    public void EditElement1dShouldReturnNullElementSection() {
      GsaElement1dGoo output = _helper.GetElementOutput();
      Assert.Null(output.Value.Section);
    }

    [Fact]
    public void EditElement1dShouldReturnDefaultElementGrupForElement() {
      GsaElement1dGoo output = _helper.GetElementOutput();
      Assert.Equal(1, output.Value.ApiElement.Group);
    }

    [Fact]
    public void EditElement1dShouldReturnValidId() {
      GH_Integer output = _helper.GetIdOutput();
      Assert.Equal(0, output.Value);
    }

    [Fact]
    public void EditElement1dShouldReturnValidLineFromXValue() {
      GH_Line output = _helper.GetLineOutput();
      Assert.Equal(0, output.Value.From.X, 6);
    }

    [Fact]
    public void EditElement1dShouldReturnValidLineFromYValue() {
      GH_Line output = _helper.GetLineOutput();
      Assert.Equal(-1, output.Value.From.Y, 6);
    }

    [Fact]
    public void EditElement1dShouldReturnValidLineFromZValue() {
      GH_Line output = _helper.GetLineOutput();
      Assert.Equal(0, output.Value.From.Z, 6);
    }

    [Fact]
    public void EditElement1dShouldReturnValidLineToXValue() {
      GH_Line output = _helper.GetLineOutput();
      Assert.Equal(7, output.Value.To.X, 6);
    }

    [Fact]
    public void EditElement1dShouldReturnValidLineToYValue() {
      GH_Line output = _helper.GetLineOutput();
      Assert.Equal(3, output.Value.To.Y, 6);
    }

    [Fact]
    public void EditElement1dShouldReturnValidLineToZValue() {
      GH_Line output = _helper.GetLineOutput();
      Assert.Equal(1, output.Value.To.Z, 6);
    }

    [Fact]
    public void EditElement1dShouldReturnNotNullProperty() {
      GsaPropertyGoo output = _helper.GetSectionOutput();
      Assert.NotNull(((GsaSpringProperty)output.Value).ApiProperty);
    }

    [Fact]
    public void EditElement1dShouldReturnDefaultGroupValue() {
      GH_Integer output = _helper.GetGroupOutput();
      Assert.Equal(1, output.Value);
    }

    [Fact]
    public void EditElement1dShouldReturnValidType() {
      GH_String output = _helper.GetTypeOutput();
      Assert.Equal("Spring", output.Value);
    }

    [Fact]
    public void EditElement1dShouldReturnValidOffsetX1Value() {
      GsaOffsetGoo output = _helper.GetOffsetOutput();
      Assert.Equal(0, output.Value.X1.Value, 6);
    }

    [Fact]
    public void EditElement1dShouldReturnValidOffsetX2Value() {
      GsaOffsetGoo output = _helper.GetOffsetOutput();
      Assert.Equal(0, output.Value.X2.Value, 6);
    }

    [Fact]
    public void EditElement1dShouldReturnValidOffsetYValue() {
      GsaOffsetGoo output = _helper.GetOffsetOutput();
      Assert.Equal(0, output.Value.Y.Value, 6);
    }

    [Fact]
    public void EditElement1dShouldReturnValidOffsetZValue() {
      GsaOffsetGoo output = _helper.GetOffsetOutput();
      Assert.Equal(0, output.Value.Z.Value, 6);
    }

    [Fact]
    public void EditElement1dShouldReturnValidStartReleaseXValue() {
      GsaBool6Goo output = _helper.GetStartReleaseOutput();
      Assert.False(output.Value.X);
    }

    [Fact]
    public void EditElement1dShouldReturnValidStartReleaseYValue() {
      GsaBool6Goo output = _helper.GetStartReleaseOutput();
      Assert.False(output.Value.Y);
    }

    [Fact]
    public void EditElement1dShouldReturnValidStartReleaseZValue() {
      GsaBool6Goo output = _helper.GetStartReleaseOutput();
      Assert.False(output.Value.Z);
    }

    [Fact]
    public void EditElement1dShouldReturnValidStartReleaseXxValue() {
      GsaBool6Goo output = _helper.GetStartReleaseOutput();
      Assert.False(output.Value.Xx);
    }

    [Fact]
    public void EditElement1dShouldReturnValidStartReleaseYyValue() {
      GsaBool6Goo output = _helper.GetStartReleaseOutput();
      Assert.False(output.Value.Yy);
    }

    [Fact]
    public void EditElement1dShouldReturnValidStartReleaseZzValue() {
      GsaBool6Goo output = _helper.GetStartReleaseOutput();
      Assert.False(output.Value.Zz);
    }

    [Fact]
    public void EditElement1dShouldReturnValidEndReleaseXValue() {
      GsaBool6Goo output = _helper.GetEndReleaseOutput();
      Assert.False(output.Value.X);
    }

    [Fact]
    public void EditElement1dShouldReturnValidEndReleaseYValue() {
      GsaBool6Goo output = _helper.GetEndReleaseOutput();
      Assert.False(output.Value.Y);
    }

    [Fact]
    public void EditElement1dShouldReturnValidEndReleaseZValue() {
      GsaBool6Goo output = _helper.GetEndReleaseOutput();
      Assert.False(output.Value.Z);
    }

    [Fact]
    public void EditElement1dShouldReturnValidEndReleaseXxValue() {
      GsaBool6Goo output = _helper.GetEndReleaseOutput();
      Assert.False(output.Value.Xx);
    }

    [Fact]
    public void EditElement1dShouldReturnValidEndReleaseYyValue() {
      GsaBool6Goo output = _helper.GetEndReleaseOutput();
      Assert.False(output.Value.Yy);
    }

    [Fact]
    public void EditElement1dShouldReturnValidEndReleaseZzValue() {
      GsaBool6Goo output = _helper.GetEndReleaseOutput();
      Assert.False(output.Value.Zz);
    }

    [Fact]
    public void EditElement1dShouldReturnValidAngleValue() {
      GH_Number output = _helper.GetAngleOutput();
      Assert.Equal(0, output.Value, 6);
    }

    [Fact]
    public void EditElement1dShouldReturnNullOrientation() {
      GsaNodeGoo output = _helper.GetOrientationOutput();
      Assert.Null(output.Value);
    }

    [Fact]
    public void EditElement1dShouldReturnValidName() {
      GH_String output = _helper.GetNameOutput();
      Assert.Equal("", output.Value);
    }

    [Fact]
    public void EditElement1dShouldReturnValidColor() {
      GH_Colour output = _helper.GetColorOutput();
      Assert.Equal("ff000000", output.Value.Name);
    }

    [Fact]
    public void EditElement1dShouldReturnFalseDummy() {
      GH_Boolean output = _helper.GetDummyOutput();
      Assert.False(output.Value);
    }

    [Fact]
    public void EditElement1dShouldReturnValidParentMember() {
      GH_Integer output = _helper.GetParentMemberOutput();
      Assert.Equal(0, output.Value);
    }
  }

  [Collection("GrasshopperFixture collection")]
  public class EditElement1dTests_ErrorsHandling {
    private EditElement1dTestsHelper _helper;

    public EditElement1dTests_ErrorsHandling() {
      _helper = new EditElement1dTestsHelper();
    }

    [Fact]
    public void InvalidPropertyElementTypeCombination1() {
      var property = new AxialSpringProperty {
        Stiffness = 3.0,
      };
      _helper.SetSpringPropertyInput(new GsaPropertyGoo(new GsaSpringProperty(property)));
      _helper.SetTypeInput("Beam");

      _helper.GetComponent().Params.Output[0].ExpireSolution(true);
      _helper.GetComponent().Params.Output[0].CollectData();
      Assert.Empty(_helper.GetComponent().RuntimeMessages(GH_RuntimeMessageLevel.Error));
    }

    [Fact]
    public void InvalidPropertyElementTypeCombination2() {
      _helper.SetSectionInput();
      _helper.SetTypeInput("Spring");

      _helper.GetComponent().Params.Output[0].ExpireSolution(true);
      _helper.GetComponent().Params.Output[0].CollectData();
      Assert.Single(_helper.GetComponent().RuntimeMessages(GH_RuntimeMessageLevel.Error));
    }

    [Fact]
    public void InvalidPropertyElementTypeCombination3() {
      var comp = new Edit1dElement();
      comp.CreateAttributes();
      ComponentTestHelper.SetInput(comp, "Spring", 5);

      comp.Params.Output[0].ExpireSolution(true);
      comp.Params.Output[0].CollectData();
      Assert.Empty(comp.RuntimeMessages(GH_RuntimeMessageLevel.Error));
    }
  }

  [Collection("GrasshopperFixture collection")]
  public class EditElement1dTests_Extras {
    private EditElement1dTestsHelper _helper;

    public EditElement1dTests_Extras() {
      _helper = new EditElement1dTestsHelper();
    }

    [Fact]
    public void EditElementShouldReturnValidTypeWhenInputTypeIsInt() {
      _helper.SetTypeInput("1");
      GH_String output = _helper.GetTypeOutput();
      Assert.Equal("Bar", output.Value);
    }
  }

  public class EditElement1dTestsHelper {
    private readonly GH_OasysComponent _component;

    public EditElement1dTestsHelper() {
      _component = ComponentMother();
    }

    public GH_OasysComponent GetComponent() {
      return _component;
    }

    public static GH_OasysComponent ComponentMother() {
      var comp = new Edit1dElement();
      comp.CreateAttributes();

      ComponentTestHelper.SetInput(comp, ComponentTestHelper.GetOutput(CreateElement1dTests.ComponentMother()), 0);

      return comp;
    }

    public GsaElement1dGoo GetElementOutput() {
      var element = (GsaElement1dGoo)ComponentTestHelper.GetOutput(_component, 0);
      return element;
    }

    public GH_Integer GetIdOutput() {
      var id = (GH_Integer)ComponentTestHelper.GetOutput(_component, 1);
      return id;
    }

    public GH_Line GetLineOutput() {
      var line = (GH_Line)ComponentTestHelper.GetOutput(_component, 2);
      return line;
    }

    public GsaPropertyGoo GetSectionOutput() {
      var section = (GsaPropertyGoo)ComponentTestHelper.GetOutput(_component, 3);
      return section;
    }

    public GH_Integer GetGroupOutput() {
      var group = (GH_Integer)ComponentTestHelper.GetOutput(_component, 4);
      return group;
    }

    public GH_String GetTypeOutput() {
      var type = (GH_String)ComponentTestHelper.GetOutput(_component, 5);
      return type;
    }

    public GsaOffsetGoo GetOffsetOutput() {
      var offset = (GsaOffsetGoo)ComponentTestHelper.GetOutput(_component, 6);
      return offset;
    }

    public GsaBool6Goo GetStartReleaseOutput() {
      var startRelease = (GsaBool6Goo)ComponentTestHelper.GetOutput(_component, 7);
      return startRelease;
    }

    public GsaBool6Goo GetEndReleaseOutput() {
      var endRelease = (GsaBool6Goo)ComponentTestHelper.GetOutput(_component, 8);
      return endRelease;
    }

    public GH_Number GetAngleOutput() {
      var angle = (GH_Number)ComponentTestHelper.GetOutput(_component, 9);
      return angle;
    }

    public GsaNodeGoo GetOrientationOutput() {
      var orientation = (GsaNodeGoo)ComponentTestHelper.GetOutput(_component, 10);
      return orientation;
    }

    public GH_String GetNameOutput() {
      var name = (GH_String)ComponentTestHelper.GetOutput(_component, 11);
      return name;
    }

    public GH_Colour GetColorOutput() {
      var colour = (GH_Colour)ComponentTestHelper.GetOutput(_component, 12);
      return colour;
    }

    public GH_Boolean GetDummyOutput() {
      var dummy = (GH_Boolean)ComponentTestHelper.GetOutput(_component, 13);
      return dummy;
    }

    public GH_Integer GetParentMemberOutput() {
      var parentMember = (GH_Integer)ComponentTestHelper.GetOutput(_component, 14);
      return parentMember;
    }

    public void SetIdInput(int id = 1) {
      ComponentTestHelper.SetInput(_component, id, 1);
    }

    public void SetLineInput() {
      ComponentTestHelper.SetInput(_component, new LineCurve(new Point3d(0, 0, 0), new Point3d(1, 2, 3)), 2);
    }

    public void SetSectionInput(string profile = "STD CH 10 20 30 40") {
      ComponentTestHelper.SetInput(_component, profile, 3);
    }

    public void SetSpringPropertyInput(GsaPropertyGoo property) {
      ComponentTestHelper.SetInput(_component, property, 3);
    }

    public void SetGroupInput(int id) {
      ComponentTestHelper.SetInput(_component, id, 4);
    }

    public void SetTypeInput(string type) {
      ComponentTestHelper.SetInput(_component, type, 5);
    }

    public void SetOffsetInput(GsaOffsetGoo offset) {
      ComponentTestHelper.SetInput(_component, offset, 6);
    }

    public void SetStartReleaseInput(GsaBool6Goo release) {
      ComponentTestHelper.SetInput(_component, release, 7);
    }

    public void SetEndReleaseInput(GsaBool6Goo release) {
      ComponentTestHelper.SetInput(_component, release, 8);
    }

    public void SetAngleInput(double angle) {
      ComponentTestHelper.SetInput(_component, angle, 9);
    }

    public void SetOrientationInput(GsaNodeGoo orientation) {
      ComponentTestHelper.SetInput(_component, orientation, 10);
    }

    public void SetNameInput(string name) {
      ComponentTestHelper.SetInput(_component, name, 11);
    }

    public void SetColorInput(GH_Colour color) {
      ComponentTestHelper.SetInput(_component, color, 12);
    }

    public void SetDummyInput(bool dummy) {
      ComponentTestHelper.SetInput(_component, dummy, 13);
    }
  }
}
