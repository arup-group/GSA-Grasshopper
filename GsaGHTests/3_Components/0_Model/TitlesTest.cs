using Grasshopper.Kernel.Types;

using GsaGH.Components;
using GsaGH.Parameters;

using GsaGHTests.Helpers;

using OasysGH.Components;

using Xunit;

namespace GsaGHTests.Model {
  [Collection("GrasshopperFixture collection")]
  public class TitlesTests {

    public static GH_OasysComponent ComponentMother() {
      var comp = new ModelTitles();
      comp.CreateAttributes();

      ComponentTestHelper.SetInput(comp, ModelTests.GsaModelGooMother, 0);

      return comp;
    }

    [Fact]
    public void TestGetExistingTitles() {
      GH_OasysComponent comp = ComponentMother();

      var jn = (GH_String)ComponentTestHelper.GetOutput(comp, 1);
      var initials = (GH_String)ComponentTestHelper.GetOutput(comp, 2);
      var title = (GH_String)ComponentTestHelper.GetOutput(comp, 3);
      var subtitle = (GH_String)ComponentTestHelper.GetOutput(comp, 4);
      var header = (GH_String)ComponentTestHelper.GetOutput(comp, 5);
      var notes = (GH_String)ComponentTestHelper.GetOutput(comp, 6);
      Assert.Equal("77107/75", jn.Value);
      Assert.Equal("PTAM", initials.Value);
      Assert.Equal("Steel Design Sample File", title.Value);
      Assert.Equal("Single beam", subtitle.Value);
      Assert.Equal(string.Empty, header.Value);
      Assert.Equal(string.Empty, notes.Value);
    }

    [Fact]
    public void TestSetTitles() {
      GH_OasysComponent comp = ComponentMother();

      ComponentTestHelper.SetInput(comp, "123456-78", 1);
      ComponentTestHelper.SetInput(comp, "KPN", 2);
      ComponentTestHelper.SetInput(comp, "Test this Title", 3);
      ComponentTestHelper.SetInput(comp, "Sub title sub test", 4);
      ComponentTestHelper.SetInput(comp, "Calcuation header test", 5);
      ComponentTestHelper.SetInput(comp, "Here to take notes", 6);

      // test that items have been set into API model
      var output = (GsaModelGoo)ComponentTestHelper.GetOutput(comp);
      GsaAPI.Titles titles = output.Value.ApiModel.Titles();
      Assert.Equal("123456-78", titles.JobNumber);
      Assert.Equal("KPN", titles.Initials);
      Assert.Equal("Test this Title", titles.Title);
      Assert.Equal("Sub title sub test", titles.SubTitle);
      Assert.Equal("Calcuation header test", titles.Calculation);
      Assert.Equal("Here to take notes", titles.Notes);

      // test that the component outputs the new titles
      var jn = (GH_String)ComponentTestHelper.GetOutput(comp, 1);
      var initials = (GH_String)ComponentTestHelper.GetOutput(comp, 2);
      var title = (GH_String)ComponentTestHelper.GetOutput(comp, 3);
      var subtitle = (GH_String)ComponentTestHelper.GetOutput(comp, 4);
      var header = (GH_String)ComponentTestHelper.GetOutput(comp, 5);
      var notes = (GH_String)ComponentTestHelper.GetOutput(comp, 6);
      Assert.Equal("123456-78", jn.Value);
      Assert.Equal("KPN", initials.Value);
      Assert.Equal("Test this Title", title.Value);
      Assert.Equal("Sub title sub test", subtitle.Value);
      Assert.Equal("Calcuation header test", header.Value);
      Assert.Equal("Here to take notes", notes.Value);
    }
  }
}
