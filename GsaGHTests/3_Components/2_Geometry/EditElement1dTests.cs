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

using Line = Rhino.Geometry.Line;

namespace GsaGHTests.Components.Geometry {
  [Collection("GrasshopperFixture collection")]
  public class EditElement1dTests_WithoutSettingInputs {
    public readonly EditElement1dTestsHelper _helper;

    public EditElement1dTests_WithoutSettingInputs() {
      _helper = new EditElement1dTestsHelper();
    }

    [Fact]
    public void ComponentReturnValidElementLinePointAtStartXValue() {
      GsaElement1D element = _helper.GetElementOutput();
      Assert.Equal(0, element.Line.PointAtStart.X, 6);
    }

    [Fact]
    public void ComponentReturnValidElementLinePointAtStartYValue() {
      GsaElement1D element = _helper.GetElementOutput();
      Assert.Equal(-1, element.Line.PointAtStart.Y, 6);
    }

    [Fact]
    public void ComponentReturnValidElementLinePointAtStartZValue() {
      GsaElement1D element = _helper.GetElementOutput();
      Assert.Equal(0, element.Line.PointAtStart.Z, 6);
    }

    [Fact]
    public void ComponentReturnValidElementLinePointAtEndXValue() {
      GsaElement1D element = _helper.GetElementOutput();
      Assert.Equal(7, element.Line.PointAtEnd.X, 6);
    }

    [Fact]
    public void ComponentReturnValidElementLinePointAtEndYValue() {
      GsaElement1D element = _helper.GetElementOutput();
      Assert.Equal(3, element.Line.PointAtEnd.Y, 6);
    }

    [Fact]
    public void ComponentReturnValidElementLinePointAtEndZValue() {
      GsaElement1D element = _helper.GetElementOutput();
      Assert.Equal(1, element.Line.PointAtEnd.Z, 6);
    }

    [Fact]
    public void ComponentReturnValidSectionProfile() {
      GsaElement1D element = _helper.GetElementOutput();
      Assert.Equal("STD CH(ft) 1 2 3 4", element.Section.ApiSection.Profile);
    }

    [Fact]
    public void ComponentReturnValidApiElementGroupValue() {
      GsaElement1D element = _helper.GetElementOutput();
      Assert.Equal(1, element.ApiElement.Group);
    }

    [Fact]
    public void ComponentReturnValidIdValue() {
      int id = _helper.GetIdOutput();
      Assert.Equal(0, id);
    }

    [Fact]
    public void ComponentReturnValidLineFromXValue() {
      Line line = _helper.GetLineOutput();
      Assert.Equal(0, line.From.X, 6);
    }

    [Fact]
    public void ComponentReturnValidLineFromYValue() {
      Line line = _helper.GetLineOutput();
      Assert.Equal(-1, line.From.Y, 6);
    }

    [Fact]
    public void ComponentReturnValidLineFromZValue() {
      Line line = _helper.GetLineOutput();
      Assert.Equal(0, line.From.Z, 6);
    }

    [Fact]
    public void ComponentReturnValidLineToXValue() {
      Line line = _helper.GetLineOutput();
      Assert.Equal(7, line.To.X, 6);
    }

    [Fact]
    public void ComponentReturnValidLineToYValue() {
      Line line = _helper.GetLineOutput();
      Assert.Equal(3, line.To.Y, 6);
    }

    [Fact]
    public void ComponentReturnValidLineToZValue() {
      Line line = _helper.GetLineOutput();
      Assert.Equal(1, line.To.Z, 6);
    }

    [Fact]
    public void ComponentReturnValidSectionProfileFromInput() {
      IGsaProperty section = _helper.GetSectionOutput();
      Assert.Equal("STD CH(ft) 1 2 3 4", ((GsaSection)section).ApiSection.Profile);
    }

    [Fact]
    public void ComponentReturnDefaultGroupValue() {
      int group = _helper.GetGroupOutput();
      Assert.Equal(1, group);
    }

    [Fact]
    public void ComponentReturnValidType() {
      string type = _helper.GetTypeOutput();
      Assert.Equal("Beam", type);
    }

    [Fact]
    public void ComponentReturnValidOffsetX1() {
      GsaOffset offset = _helper.GetOffsetOutput();
      Assert.Equal(0, offset.X1.Value, 6);
    }

    [Fact]
    public void ComponentReturnValidOffsetX2() {
      GsaOffset offset = _helper.GetOffsetOutput();
      Assert.Equal(0, offset.X2.Value, 6);
    }

    [Fact]
    public void ComponentReturnValidOffsetY() {
      GsaOffset offset = _helper.GetOffsetOutput();
      Assert.Equal(0, offset.Y.Value, 6);
    }

    [Fact]
    public void ComponentReturnValidOffsetZ() {
      GsaOffset offset = _helper.GetOffsetOutput();
      Assert.Equal(0, offset.Z.Value, 6);
    }

    [Fact]
    public void ComponentReturnValidReleaseStartX() {
      GsaBool6 startRelease = _helper.GetStartReleaseOutput();
      Assert.False(startRelease.X);
    }

    [Fact]
    public void ComponentReturnValidReleaseStartY() {
      GsaBool6 startRelease = _helper.GetStartReleaseOutput();
      Assert.False(startRelease.Y);
    }

    [Fact]
    public void ComponentReturnValidReleaseStartZ() {
      GsaBool6 startRelease = _helper.GetStartReleaseOutput();
      Assert.False(startRelease.Z);
    }

    [Fact]
    public void ComponentReturnValidReleaseStartXx() {
      GsaBool6 startRelease = _helper.GetStartReleaseOutput();
      Assert.False(startRelease.Xx);
    }

    [Fact]
    public void ComponentReturnValidReleaseStartYy() {
      GsaBool6 startRelease = _helper.GetStartReleaseOutput();
      Assert.False(startRelease.Yy);
    }

    [Fact]
    public void ComponentReturnValidReleaseStartZz() {
      GsaBool6 startRelease = _helper.GetStartReleaseOutput();
      Assert.False(startRelease.Zz);
    }

    [Fact]
    public void ComponentReturnValidReleaseEndX() {
      GsaBool6 endRelease = _helper.GetEndReleaseOutput();
      Assert.False(endRelease.X);
    }

    [Fact]
    public void ComponentReturnValidReleaseEndY() {
      GsaBool6 endRelease = _helper.GetEndReleaseOutput();
      Assert.False(endRelease.Y);
    }

    [Fact]
    public void ComponentReturnValidReleaseEndZ() {
      GsaBool6 endRelease = _helper.GetEndReleaseOutput();
      Assert.False(endRelease.Z);
    }

    [Fact]
    public void ComponentReturnValidReleaseEndXx() {
      GsaBool6 endRelease = _helper.GetEndReleaseOutput();
      Assert.False(endRelease.Xx);
    }

    [Fact]
    public void ComponentReturnValidReleaseEndYy() {
      GsaBool6 endRelease = _helper.GetEndReleaseOutput();
      Assert.False(endRelease.Yy);
    }

    [Fact]
    public void ComponentReturnValidReleaseEndZz() {
      GsaBool6 endRelease = _helper.GetEndReleaseOutput();
      Assert.False(endRelease.Zz);
    }

    [Fact]
    public void ComponentReturnDefaultAngleValue() {
      double angle = _helper.GetAngleOutput();
      Assert.Equal(0, angle);
    }

    [Fact]
    public void ComponentReturnNullOrientationValue() {
      GsaNode orientation = _helper.GetOrientationOutput();
      Assert.Null(orientation);
    }

    [Fact]
    public void ComponentReturnEmptyNameValue() {
      string name = _helper.GetNameOutput();
      Assert.Empty(name);
    }

    [Fact]
    public void ComponentReturnDefaultColorValue() {
      Color colour = _helper.GetColorOutput();
      Assert.Equal("ff000000", colour.Name);
    }

    [Fact]
    public void ComponentReturnDefaultDummyValue() {
      bool dummy = _helper.GetDummyOutput();
      Assert.False(dummy);
    }

    [Fact]
    public void ComponentShouldReturnParentMemberDefaultValue() {
      int parentMember = _helper.GetParentMemberOutput();
      Assert.Equal(0, parentMember);
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
      GsaElement1D element = _helper.GetElementOutput();
      Assert.Equal(0, element.Line.PointAtStart.X);
    }

    [Fact]
    public void EditElementShouldReturnValidElementLinePointAtStartValueY() {
      GsaElement1D element = _helper.GetElementOutput();
      Assert.Equal(0, element.Line.PointAtStart.Y);
    }

    [Fact]
    public void EditElementShouldReturnValidElementLinePointAtStartValueZ() {
      GsaElement1D element = _helper.GetElementOutput();
      Assert.Equal(0, element.Line.PointAtStart.Z);
    }

    [Fact]
    public void EditElementShouldReturnValidElementLinePointAtEndValueX() {
      GsaElement1D element = _helper.GetElementOutput();
      Assert.Equal(1, element.Line.PointAtEnd.X);
    }

    [Fact]
    public void EditElementShouldReturnValidElementLinePointAtEndValueY() {
      GsaElement1D element = _helper.GetElementOutput();
      Assert.Equal(2, element.Line.PointAtEnd.Y);
    }

    [Fact]
    public void EditElementShouldReturnValidElementLinePointAtEndValueZ() {
      GsaElement1D element = _helper.GetElementOutput();
      Assert.Equal(3, element.Line.PointAtEnd.Z);
    }

    [Fact]
    public void EditElementShouldReturnValidSectionProfileFromElement() {
      GsaElement1D element = _helper.GetElementOutput();
      Assert.Equal(_helper.DefaultSection, element.Section.ApiSection.Profile);
    }

    [Fact]
    public void EditElementShouldReturnValidGroupValueFromElement() {
      GsaElement1D element = _helper.GetElementOutput();
      Assert.Equal(7, element.ApiElement.Group);
    }

    [Fact]
    public void EditElementShouldReturnValidId() {
      int output = _helper.GetIdOutput();
      Assert.Equal(1, output);
    }

    [Fact]
    public void EditElementShouldReturnValidLineFromXValue() {
      Line output = _helper.GetLineOutput();
      Assert.Equal(0, output.From.X);
    }

    [Fact]
    public void EditElementShouldReturnValidLineFromYValue() {
      Line output = _helper.GetLineOutput();
      Assert.Equal(0, output.From.Y);
    }

    [Fact]
    public void EditElementShouldReturnValidLineFromZValue() {
      Line output = _helper.GetLineOutput();
      Assert.Equal(0, output.From.Z);
    }

    [Fact]
    public void EditElementShouldReturnValidLineToXValue() {
      Line output = _helper.GetLineOutput();
      Assert.Equal(1, output.To.X);
    }

    [Fact]
    public void EditElementShouldReturnValidLineToYValue() {
      Line output = _helper.GetLineOutput();
      Assert.Equal(2, output.To.Y);
    }

    [Fact]
    public void EditElementShouldReturnValidLineToZValue() {
      Line output = _helper.GetLineOutput();
      Assert.Equal(3, output.To.Z);
    }

    [Fact]
    public void EditElementShouldReturnValidSectionProfileValue() {
      IGsaProperty output = _helper.GetSectionOutput();
      Assert.Equal(_helper.DefaultSection, ((GsaSection)output).ApiSection.Profile);
    }

    [Fact]
    public void EditElementShouldReturnValidGroupValue() {
      int output = _helper.GetGroupOutput();
      Assert.Equal(7, output);
    }

    [Fact]
    public void EditElementShouldReturnValidTypeValue() {
      string output = _helper.GetTypeOutput();
      Assert.Equal("Beam", output);
    }

    [Fact]
    public void EditElementShouldReturnValidOffsetX1Value() {
      GsaOffset output = _helper.GetOffsetOutput();
      Assert.Equal(1, output.X1.Value);
    }

    [Fact]
    public void EditElementShouldReturnValidOffsetX2Value() {
      GsaOffset output = _helper.GetOffsetOutput();
      Assert.Equal(2, output.X2.Value);
    }

    [Fact]
    public void EditElementShouldReturnValidOffsetYValue() {
      GsaOffset output = _helper.GetOffsetOutput();
      Assert.Equal(3, output.Y.Value);
    }

    [Fact]
    public void EditElementShouldReturnValidOffsetZValue() {
      GsaOffset output = _helper.GetOffsetOutput();
      Assert.Equal(4, output.Z.Value);
    }

    [Fact]
    public void EditElementShouldReturnValidStartReleaseXValue() {
      GsaBool6 output = _helper.GetStartReleaseOutput();
      Assert.True(output.X);
    }

    [Fact]
    public void EditElementShouldReturnValidStartReleaseYValue() {
      GsaBool6 output = _helper.GetStartReleaseOutput();
      Assert.True(output.Y);
    }

    [Fact]
    public void EditElementShouldReturnValidStartReleaseZValue() {
      GsaBool6 output = _helper.GetStartReleaseOutput();
      Assert.True(output.X);
    }

    [Fact]
    public void EditElementShouldReturnValidStartReleaseXxValue() {
      GsaBool6 output = _helper.GetStartReleaseOutput();
      Assert.True(output.Xx);
    }

    [Fact]
    public void EditElementShouldReturnValidStartReleaseYyValue() {
      GsaBool6 output = _helper.GetStartReleaseOutput();
      Assert.True(output.Yy);
    }

    [Fact]
    public void EditElementShouldReturnValidStartReleaseZzValue() {
      GsaBool6 output = _helper.GetStartReleaseOutput();
      Assert.True(output.Zz);
    }

    [Fact]
    public void EditElementShouldReturnValidEndReleaseXValue() {
      GsaBool6 output = _helper.GetEndReleaseOutput();
      Assert.True(output.X);
    }

    [Fact]
    public void EditElementShouldReturnValidEndReleaseYValue() {
      GsaBool6 output = _helper.GetEndReleaseOutput();
      Assert.True(output.Y);
    }

    [Fact]
    public void EditElementShouldReturnValidEndReleaseZValue() {
      GsaBool6 output = _helper.GetEndReleaseOutput();
      Assert.True(output.X);
    }

    [Fact]
    public void EditElementShouldReturnValidEndReleaseXxValue() {
      GsaBool6 output = _helper.GetEndReleaseOutput();
      Assert.True(output.Xx);
    }

    [Fact]
    public void EditElementShouldReturnValidEndReleaseYyValue() {
      GsaBool6 output = _helper.GetEndReleaseOutput();
      Assert.True(output.Yy);
    }

    [Fact]
    public void EditElementShouldReturnValidEndtReleaseZzValue() {
      GsaBool6 output = _helper.GetEndReleaseOutput();
      Assert.True(output.Zz);
    }

    [Fact]
    public void EditElementShouldReturnValidAngleValue() {
      double output = _helper.GetAngleOutput();
      Assert.Equal(Math.PI, output);
    }

    [Fact]
    public void EditElementShouldReturnValidOrientationYValue() {
      GsaNode output = _helper.GetOrientationOutput();
      Assert.Equal(2, output.Point.Y);
    }

    [Fact]
    public void EditElementShouldReturnValidOrientationZValue() {
      GsaNode output = _helper.GetOrientationOutput();
      Assert.Equal(3, output.Point.Z);
    }

    [Fact]
    public void EditElementShouldReturnValidOrientationIdValue() {
      GsaNode output = _helper.GetOrientationOutput();
      Assert.Equal(99, output.Id);
    }

    [Fact]
    public void EditElementShouldReturnValidNameValue() {
      string output = _helper.GetNameOutput();
      Assert.Equal("name", output);
    }

    [Fact]
    public void EditElementShouldReturnValidColorValue() {
      Color output = _helper.GetColorOutput();
      Assert.Equal("ffffffff", output.Name);
    }

    [Fact]
    public void EditElementShouldReturnValidDummyValue() {
      bool output = _helper.GetDummyOutput();
      Assert.True(output);
    }

    [Fact]
    public void EditElementShouldReturnValidParentMemberValue() {
      int output = _helper.GetParentMemberOutput();
      Assert.Equal(0, output);
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
      GsaElement1D output = _helper.GetElementOutput();
      Assert.Equal(0, output.Line.PointAtStart.X, 6);
    }

    [Fact]
    public void EditElementShouldReturnValidLinePointAtStartYValue() {
      GsaElement1D output = _helper.GetElementOutput();
      Assert.Equal(-1, output.Line.PointAtStart.Y, 6);
    }

    [Fact]
    public void EditElementShouldReturnValidLinePointAtStartZValue() {
      GsaElement1D output = _helper.GetElementOutput();
      Assert.Equal(0, output.Line.PointAtStart.Z, 6);
    }

    [Fact]
    public void EditElementShouldReturnValidLinePointAtEndXValue() {
      GsaElement1D output = _helper.GetElementOutput();
      Assert.Equal(7, output.Line.PointAtEnd.X, 6);
    }

    [Fact]
    public void EditElementShouldReturnValidLinePointAtEndYValue() {
      GsaElement1D output = _helper.GetElementOutput();
      Assert.Equal(3, output.Line.PointAtEnd.Y, 6);
    }

    [Fact]
    public void EditElementShouldReturnValidLinePointAtEndZValue() {
      GsaElement1D output = _helper.GetElementOutput();
      Assert.Equal(1, output.Line.PointAtEnd.Z, 6);
    }

    [Fact]
    public void EditElement1dShouldReturnNullElementSection() {
      GsaElement1D output = _helper.GetElementOutput();
      Assert.Null(output.Section);
    }

    [Fact]
    public void EditElement1dShouldReturnDefaultElementGrupForElement() {
      GsaElement1D output = _helper.GetElementOutput();
      Assert.Equal(1, output.ApiElement.Group);
    }

    [Fact]
    public void EditElement1dShouldReturnValidId() {
      int output = _helper.GetIdOutput();
      Assert.Equal(0, output);
    }

    [Fact]
    public void EditElement1dShouldReturnValidLineFromXValue() {
      Line output = _helper.GetLineOutput();
      Assert.Equal(0, output.From.X, 6);
    }

    [Fact]
    public void EditElement1dShouldReturnValidLineFromYValue() {
      Line output = _helper.GetLineOutput();
      Assert.Equal(-1, output.From.Y, 6);
    }

    [Fact]
    public void EditElement1dShouldReturnValidLineFromZValue() {
      Line output = _helper.GetLineOutput();
      Assert.Equal(0, output.From.Z, 6);
    }

    [Fact]
    public void EditElement1dShouldReturnValidLineToXValue() {
      Line output = _helper.GetLineOutput();
      Assert.Equal(7, output.To.X, 6);
    }

    [Fact]
    public void EditElement1dShouldReturnValidLineToYValue() {
      Line output = _helper.GetLineOutput();
      Assert.Equal(3, output.To.Y, 6);
    }

    [Fact]
    public void EditElement1dShouldReturnValidLineToZValue() {
      Line output = _helper.GetLineOutput();
      Assert.Equal(1, output.To.Z, 6);
    }

    [Fact]
    public void EditElement1dShouldReturnNotNullProperty() {
      IGsaProperty output = _helper.GetSectionOutput();
      Assert.NotNull(((GsaSpringProperty)output).ApiProperty);
    }

    [Fact]
    public void EditElement1dShouldReturnDefaultGroupValue() {
      int output = _helper.GetGroupOutput();
      Assert.Equal(1, output);
    }

    [Fact]
    public void EditElement1dShouldReturnValidType() {
      string output = _helper.GetTypeOutput();
      Assert.Equal("Spring", output);
    }

    [Fact]
    public void EditElement1dShouldReturnValidOffsetX1Value() {
      GsaOffset output = _helper.GetOffsetOutput();
      Assert.Equal(0, output.X1.Value, 6);
    }

    [Fact]
    public void EditElement1dShouldReturnValidOffsetX2Value() {
      GsaOffset output = _helper.GetOffsetOutput();
      Assert.Equal(0, output.X2.Value, 6);
    }

    [Fact]
    public void EditElement1dShouldReturnValidOffsetYValue() {
      GsaOffset output = _helper.GetOffsetOutput();
      Assert.Equal(0, output.Y.Value, 6);
    }

    [Fact]
    public void EditElement1dShouldReturnValidOffsetZValue() {
      GsaOffset output = _helper.GetOffsetOutput();
      Assert.Equal(0, output.Z.Value, 6);
    }

    [Fact]
    public void EditElement1dShouldReturnValidStartReleaseXValue() {
      GsaBool6 output = _helper.GetStartReleaseOutput();
      Assert.False(output.X);
    }

    [Fact]
    public void EditElement1dShouldReturnValidStartReleaseYValue() {
      GsaBool6 output = _helper.GetStartReleaseOutput();
      Assert.False(output.Y);
    }

    [Fact]
    public void EditElement1dShouldReturnValidStartReleaseZValue() {
      GsaBool6 output = _helper.GetStartReleaseOutput();
      Assert.False(output.Z);
    }

    [Fact]
    public void EditElement1dShouldReturnValidStartReleaseXxValue() {
      GsaBool6 output = _helper.GetStartReleaseOutput();
      Assert.False(output.Xx);
    }

    [Fact]
    public void EditElement1dShouldReturnValidStartReleaseYyValue() {
      GsaBool6 output = _helper.GetStartReleaseOutput();
      Assert.False(output.Yy);
    }

    [Fact]
    public void EditElement1dShouldReturnValidStartReleaseZzValue() {
      GsaBool6 output = _helper.GetStartReleaseOutput();
      Assert.False(output.Zz);
    }

    [Fact]
    public void EditElement1dShouldReturnValidEndReleaseXValue() {
      GsaBool6 output = _helper.GetEndReleaseOutput();
      Assert.False(output.X);
    }

    [Fact]
    public void EditElement1dShouldReturnValidEndReleaseYValue() {
      GsaBool6 output = _helper.GetEndReleaseOutput();
      Assert.False(output.Y);
    }

    [Fact]
    public void EditElement1dShouldReturnValidEndReleaseZValue() {
      GsaBool6 output = _helper.GetEndReleaseOutput();
      Assert.False(output.Z);
    }

    [Fact]
    public void EditElement1dShouldReturnValidEndReleaseXxValue() {
      GsaBool6 output = _helper.GetEndReleaseOutput();
      Assert.False(output.Xx);
    }

    [Fact]
    public void EditElement1dShouldReturnValidEndReleaseYyValue() {
      GsaBool6 output = _helper.GetEndReleaseOutput();
      Assert.False(output.Yy);
    }

    [Fact]
    public void EditElement1dShouldReturnValidEndReleaseZzValue() {
      GsaBool6 output = _helper.GetEndReleaseOutput();
      Assert.False(output.Zz);
    }

    [Fact]
    public void EditElement1dShouldReturnValidAngleValue() {
      double output = _helper.GetAngleOutput();
      Assert.Equal(0, output, 6);
    }

    [Fact]
    public void EditElement1dShouldReturnNullOrientation() {
      GsaNode output = _helper.GetOrientationOutput();
      Assert.Null(output);
    }

    [Fact]
    public void EditElement1dShouldReturnValidName() {
      string output = _helper.GetNameOutput();
      Assert.Equal("", output);
    }

    [Fact]
    public void EditElement1dShouldReturnValidColor() {
      Color output = _helper.GetColorOutput();
      Assert.Equal("ff000000", output.Name);
    }

    [Fact]
    public void EditElement1dShouldReturnFalseDummy() {
      bool output = _helper.GetDummyOutput();
      Assert.False(output);
    }

    [Fact]
    public void EditElement1dShouldReturnValidParentMember() {
      int output = _helper.GetParentMemberOutput();
      Assert.Equal(0, output);
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
      string output = _helper.GetTypeOutput();
      Assert.Equal("Bar", output);
    }

    [Fact]
    public void ElementStartReleasesAreCorrectWhenBoolValuesAssignedAsInput() {
      var releaseInput = new GsaBool6(true, true, true, false, false, false);
      _helper.SetStartReleaseInput(new GsaBool6Goo(releaseInput));
      GsaBool6 releaseOutput = _helper.GetStartReleaseOutput();
      Assert.Equal(releaseInput, releaseOutput);
    }

    [Fact]
    public void ElementEndReleasesAreCorrectWhenBoolValuesAssignedAsInput() {
      var releaseInput = new GsaBool6(true, true, true, false, false, false);
      _helper.SetEndReleaseInput(new GsaBool6Goo(releaseInput));
      GsaBool6 releaseOutput = _helper.GetEndReleaseOutput();
      Assert.Equal(releaseInput, releaseOutput);
    }

    [Fact]
    public void ElementStartReleasesAreCorrectWhenStringAssignedAsInput() {
      _helper.SetStartReleaseInput("RRRFFF");
      var releaseInput = new GsaBool6(true, true, true, false, false, false);
      GsaBool6 releaseOutput = _helper.GetStartReleaseOutput();
      Assert.Equal(releaseInput, releaseOutput);
    }

    [Fact]
    public void ElementEndReleasesAreCorrectWhenStringAssignedAsInput() {
      _helper.SetEndReleaseInput("RRRFFF");
      var releaseInput = new GsaBool6(true, true, true, false, false, false);
      GsaBool6 releaseOutput = _helper.GetEndReleaseOutput();
      Assert.Equal(releaseInput, releaseOutput);
    }
  }

  public class EditElement1dTestsHelper {
    public readonly string DefaultSection = "STD CH 10 20 30 40";
    public readonly int DefaultId = 1;
    private readonly GH_OasysComponent _component;

    public EditElement1dTestsHelper() {
      _component = ComponentMother();
    }

    public GH_OasysComponent GetComponent() {
      return _component;
    }

    private static GH_OasysComponent ComponentMother() {
      var comp = new Edit1dElement();
      comp.CreateAttributes();

      ComponentTestHelper.SetInput(comp, ComponentTestHelper.GetOutput(CreateElement1dTests.ComponentMother()), 0);

      return comp;
    }

    public GsaElement1D GetElementOutput() {
      return ComponentTestHelper.GetElement1dOutput(_component, 0);
    }

    public int GetIdOutput() {
      return ComponentTestHelper.GetIntOutput(_component, 1);
    }

    public Line GetLineOutput() {
      return ComponentTestHelper.GetLineOutput(_component, 2);
    }

    public IGsaProperty GetSectionOutput() {
      return ComponentTestHelper.GetPropertyOutput(_component, 3);
    }

    public int GetGroupOutput() {
      return ComponentTestHelper.GetIntOutput(_component, 4);
    }

    public string GetTypeOutput() {
      return ComponentTestHelper.GetStringOutput(_component, 5);
    }

    public GsaOffset GetOffsetOutput() {
      return ComponentTestHelper.GetOffsetOutput(_component, 6);
    }

    public GsaBool6 GetStartReleaseOutput() {
      return ComponentTestHelper.GetBool6Output(_component, 7);
    }

    public GsaBool6 GetEndReleaseOutput() {
      return ComponentTestHelper.GetBool6Output(_component, 8);
    }

    public double GetAngleOutput() {
      return ComponentTestHelper.GetNumberOutput(_component, 9);
    }

    public GsaNode GetOrientationOutput() {
      return ComponentTestHelper.GetNodeOutput(_component, 10);
    }

    public string GetNameOutput() {
      return ComponentTestHelper.GetStringOutput(_component, 11);
    }

    public Color GetColorOutput() {
      return ComponentTestHelper.GetColorOutput(_component, 12);
    }

    public bool GetDummyOutput() {
      return ComponentTestHelper.GetBoolOutput(_component, 13);
    }

    public int GetParentMemberOutput() {
      return ComponentTestHelper.GetIntOutput(_component, 14);
    }

    public void SetIdInput() {
      SetIdInput(DefaultId);
    }

    public void SetSectionInput() {
      SetSectionInput(DefaultSection);
    }

    public void SetIdInput(int id) {
      ComponentTestHelper.SetInput(_component, id, 1);
    }

    public void SetLineInput() {
      ComponentTestHelper.SetInput(_component, new LineCurve(new Point3d(0, 0, 0), new Point3d(1, 2, 3)), 2);
    }

    public void SetSectionInput(string profile) {
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

    public void SetStartReleaseInput(string release) {
      ComponentTestHelper.SetInput(_component, release, 7);
    }

    public void SetEndReleaseInput(GsaBool6Goo release) {
      ComponentTestHelper.SetInput(_component, release, 8);
    }

    public void SetEndReleaseInput(string release) {
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
