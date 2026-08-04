using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using GsaGH.Components;
using GsaGH.Parameters;

using GsaGHTests.Helpers;

using OasysGH.Components;

using Xunit;

namespace GsaGHTests.Model {
  [Collection("GrasshopperFixture collection")]
  public class UnitsTests {

    public static GH_OasysComponent ComponentMother() {
      var comp = new ModelUnits();
      comp.CreateAttributes();

      ComponentTestHelper.SetInput(comp, ModelTests.GsaModelGooMother, 0);

      return comp;
    }

    [Fact]
    public void TestGetExistingUnits() {
      GH_OasysComponent comp = ComponentMother();
      int i = 1;
      string acceleration = ((GH_String)ComponentTestHelper.GetOutput(comp, i++)).Value;
      string angle = ((GH_String)ComponentTestHelper.GetOutput(comp, i++)).Value;
      string energy = ((GH_String)ComponentTestHelper.GetOutput(comp, i++)).Value;
      string force = ((GH_String)ComponentTestHelper.GetOutput(comp, i++)).Value;
      string lengthLarge = ((GH_String)ComponentTestHelper.GetOutput(comp, i++)).Value;
      string lengthSection = ((GH_String)ComponentTestHelper.GetOutput(comp, i++)).Value;
      string lengthSmall = ((GH_String)ComponentTestHelper.GetOutput(comp, i++)).Value;
      string mass = ((GH_String)ComponentTestHelper.GetOutput(comp, i++)).Value;
      string stress = ((GH_String)ComponentTestHelper.GetOutput(comp, i++)).Value;
      string strain = ((GH_String)ComponentTestHelper.GetOutput(comp, i++)).Value;
      string temperature = ((GH_String)ComponentTestHelper.GetOutput(comp, i++)).Value;
      string timeLong = ((GH_String)ComponentTestHelper.GetOutput(comp, i++)).Value;
      string timeMedium = ((GH_String)ComponentTestHelper.GetOutput(comp, i++)).Value;
      string timeShort = ((GH_String)ComponentTestHelper.GetOutput(comp, i++)).Value;
      string velocity = ((GH_String)ComponentTestHelper.GetOutput(comp, i++)).Value;
      Assert.Equal("MeterPerSecondSquared", acceleration);
      Assert.Equal("Joule", energy);
      Assert.Equal("Newton", force);
      Assert.Equal("Meter", lengthLarge);
      Assert.Equal("Meter", lengthSection);
      Assert.Equal("Meter", lengthSmall);
      Assert.Equal("Kilogram", mass);
      Assert.Equal("Pascal", stress);
      Assert.Equal("Ratio", strain);
      Assert.Equal("Celsius", temperature);
      Assert.Equal("Day", timeLong);
      Assert.Equal("Minute", timeMedium);
      Assert.Equal("Second", timeShort);
      Assert.Equal("MeterPerSecond", velocity);
    }

    [Fact]
    public void TestSetUnits() {
      GH_OasysComponent comp = ComponentMother();
      int i = 1;
      ComponentTestHelper.SetInput(comp, "CentimeterPerSecondSquared", i++);
      ComponentTestHelper.SetInput(comp, "Degree", i++);
      ComponentTestHelper.SetInput(comp, "Megajoule", i++);
      ComponentTestHelper.SetInput(comp, "KiloNewton", i++);
      ComponentTestHelper.SetInput(comp, "Centimeter", i++);
      ComponentTestHelper.SetInput(comp, "Millimeter", i++);
      ComponentTestHelper.SetInput(comp, "Inch", i++);
      ComponentTestHelper.SetInput(comp, "Slug", i++);
      ComponentTestHelper.SetInput(comp, "Gigapascal", i++);
      ComponentTestHelper.SetInput(comp, "Permille", i++);
      ComponentTestHelper.SetInput(comp, "Kelvin", i++);
      ComponentTestHelper.SetInput(comp, "Day", i++);
      ComponentTestHelper.SetInput(comp, "Hour", i++);
      ComponentTestHelper.SetInput(comp, "Minute", i++);
      ComponentTestHelper.SetInput(comp, "KilometerPerHour", i++);

      // test that items have been set into API model
      var output = (GsaModelGoo)ComponentTestHelper.GetOutput(comp);
      GsaAPI.UiUnits units = output.Value.ApiModel.UiUnits();
      Assert.Equal("CentimeterPerSecondSquared", units.Acceleration.ToString());
      Assert.Equal("Degree", units.Angle.ToString());
      Assert.Equal("Megajoule", units.Energy.ToString());
      Assert.Equal("KiloNewton", units.Force.ToString());
      Assert.Equal("Centimeter", units.LengthLarge.ToString());
      Assert.Equal("Millimeter", units.LengthSections.ToString());
      Assert.Equal("Inch", units.LengthSmall.ToString());
      Assert.Equal("Slug", units.Mass.ToString());
      Assert.Equal("Gigapascal", units.Stress.ToString());
      Assert.Equal("Permille", units.Strain.ToString());
      Assert.Equal("Kelvin", units.Temperature.ToString());
      Assert.Equal("Day", units.TimeLong.ToString());
      Assert.Equal("Hour", units.TimeMedium.ToString());
      Assert.Equal("Minute", units.TimeShort.ToString());
      Assert.Equal("KilometerPerHour", units.Velocity.ToString());

      i = 1;
      string acceleration = ((GH_String)ComponentTestHelper.GetOutput(comp, i++)).Value;
      string angle = ((GH_String)ComponentTestHelper.GetOutput(comp, i++)).Value;
      string energy = ((GH_String)ComponentTestHelper.GetOutput(comp, i++)).Value;
      string force = ((GH_String)ComponentTestHelper.GetOutput(comp, i++)).Value;
      string lengthLarge = ((GH_String)ComponentTestHelper.GetOutput(comp, i++)).Value;
      string lengthSection = ((GH_String)ComponentTestHelper.GetOutput(comp, i++)).Value;
      string lengthSmall = ((GH_String)ComponentTestHelper.GetOutput(comp, i++)).Value;
      string mass = ((GH_String)ComponentTestHelper.GetOutput(comp, i++)).Value;
      string stress = ((GH_String)ComponentTestHelper.GetOutput(comp, i++)).Value;
      string strain = ((GH_String)ComponentTestHelper.GetOutput(comp, i++)).Value;
      string temperature = ((GH_String)ComponentTestHelper.GetOutput(comp, i++)).Value;
      string timeLong = ((GH_String)ComponentTestHelper.GetOutput(comp, i++)).Value;
      string timeMedium = ((GH_String)ComponentTestHelper.GetOutput(comp, i++)).Value;
      string timeShort = ((GH_String)ComponentTestHelper.GetOutput(comp, i++)).Value;
      string velocity = ((GH_String)ComponentTestHelper.GetOutput(comp, i++)).Value;
      Assert.Equal("CentimeterPerSecondSquared", acceleration);
      Assert.Equal("Degree", angle);
      Assert.Equal("Megajoule", energy);
      Assert.Equal("KiloNewton", force);
      Assert.Equal("Centimeter", lengthLarge);
      Assert.Equal("Millimeter", lengthSection);
      Assert.Equal("Inch", lengthSmall);
      Assert.Equal("Slug", mass);
      Assert.Equal("Gigapascal", stress);
      Assert.Equal("Permille", strain);
      Assert.Equal("Kelvin", temperature);
      Assert.Equal("Day", timeLong);
      Assert.Equal("Hour", timeMedium);
      Assert.Equal("Minute", timeShort);
      Assert.Equal("KilometerPerHour", velocity);
    }

    [Theory]
    [InlineData(1)] // acceleration
    [InlineData(2)] // angle
    [InlineData(3)] // energy
    [InlineData(4)] // force
    [InlineData(5)] // lengthLarge
    [InlineData(6)] // lengthSection
    [InlineData(7)] // lengthSmall
    [InlineData(8)] // mass
    [InlineData(9)] // stress
    [InlineData(10)] // strain
    [InlineData(11)] // temperature
    [InlineData(12)] // timeLong
    [InlineData(13)] // timeMedium
    [InlineData(14)] // timeShort
    [InlineData(15)] // velocity
    public void TestErrorFromWrongInput(int id) {
      GH_OasysComponent comp = ComponentMother();
      ComponentTestHelper.SetInput(comp, "invalid", id);

      // test that items have been set into API model
      var output = (GsaModelGoo)ComponentTestHelper.GetOutput(comp);

      Assert.Equal(GH_RuntimeMessageLevel.Error, comp.RuntimeMessageLevel);
      Assert.Single(comp.RuntimeMessages(GH_RuntimeMessageLevel.Error));
      Assert.Single(comp.RuntimeMessages(GH_RuntimeMessageLevel.Remark));
    }
  }
}
