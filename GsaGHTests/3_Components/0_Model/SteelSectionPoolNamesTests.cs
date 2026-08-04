using System.Collections.Generic;
using System.Collections.ObjectModel;

using Grasshopper.Kernel.Types;

using GsaGH.Components;
using GsaGH.Parameters;

using GsaGHTests.Helpers;

using OasysGH.Components;

using Xunit;

namespace GsaGHTests.Model {
  [Collection("GrasshopperFixture collection")]
  public class SteelSectionPoolNamesTests {
    public static GH_OasysComponent ComponentMother() {
      var comp = new SteelSectionPoolNames();
      comp.CreateAttributes();

      ComponentTestHelper.SetInput(comp, ModelTests.GsaModelGooMother, 0);

      return comp;
    }

    [Fact]
    public void GetExistingSteelSectionPoolNamesTest() {
      GH_OasysComponent comp = ComponentMother();

      var id = (GH_Integer)ComponentTestHelper.GetOutput(comp, 1);
      var name = (GH_String)ComponentTestHelper.GetOutput(comp, 2);
      var section = (GsaSectionGoo)ComponentTestHelper.GetOutput(comp, 3);
      Assert.Equal(1, id.Value);
      Assert.Equal("Steel pool 1", name.Value);
      Assert.Equal("PB1 CAT UB UB457x191x89 19990407 Custom", section.Value.ToString());
    }

    [Fact]
    public void SetSteelSectionPoolNames() {
      GH_OasysComponent comp = ComponentMother();

      ComponentTestHelper.SetListInput(comp, new List<object>() { 1, 2, 4 }, 1);
      ComponentTestHelper.SetListInput(comp, new List<object>() { "Steel pool 1", "Steel pool 2", "Steel pool 4" }, 2);

      var output = (List<GsaModelGoo>)ComponentTestHelper.GetListOutput(comp, 0);
      ReadOnlyDictionary<int, string> sectionPools = output[0].Value.ApiModel.SteelSectionPools();
      Assert.Equal("Steel pool 1", sectionPools[1]);
      Assert.Equal("Steel pool 2", sectionPools[2]);
      Assert.Equal("Steel pool 4", sectionPools[4]);

      var ids = (List<GH_Integer>)ComponentTestHelper.GetListOutput(comp, 1);
      var names = (List<GH_String>)ComponentTestHelper.GetListOutput(comp, 2);
      var sections = (List<GsaSectionGoo>)ComponentTestHelper.GetListOutput(comp, 3);

      Assert.Equal(1, ids[0].Value);
      Assert.Equal(2, ids[1].Value);
      Assert.Equal(4, ids[2].Value);
      Assert.Equal("Steel pool 1", names[0].Value);
      Assert.Equal("Steel pool 2", names[1].Value);
      Assert.Equal("Steel pool 4", names[2].Value);
      Assert.Single(sections);
      Assert.Equal("PB1 CAT UB UB457x191x89 19990407 Custom", sections[0].Value.ToString());
    }

    [Fact]
    public void ListSizeErrorTest() {
      GH_OasysComponent comp = ComponentMother();

      ComponentTestHelper.SetListInput(comp, new List<object>() { 1 }, 1);
      ComponentTestHelper.SetListInput(comp, new List<object>() { "Steel pool 1", "Steel pool 2" }, 2);

      var output = (List<GsaModelGoo>)ComponentTestHelper.GetListOutput(comp, 0);
      comp.Params.Output[0].ExpireSolution(true);
      comp.Params.Output[0].CollectData();
      Assert.Single(comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Error));
    }

    [Fact]
    public void ListSizeWarningTest() {
      GH_OasysComponent comp = ComponentMother();

      ComponentTestHelper.SetListInput(comp, new List<object>() { 1, 2 }, 1);
      ComponentTestHelper.SetListInput(comp, new List<object>() { "Steel pool 1" }, 2);

      var output = (List<GsaModelGoo>)ComponentTestHelper.GetListOutput(comp, 0);
      comp.Params.Output[0].ExpireSolution(true);
      comp.Params.Output[0].CollectData();
      Assert.Single(comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Warning));
    }
  }
}
