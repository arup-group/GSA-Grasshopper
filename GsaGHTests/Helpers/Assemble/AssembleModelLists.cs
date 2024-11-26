using GsaGH.Components;
using GsaGH.Parameters;

using Xunit;

namespace GsaGHTests.Helpers.Assemble {
  [Collection("GrasshopperFixture collection")]
  public class AssembleModelLists {
    private static readonly int DefaultId = 1;
    private static readonly string DefaultName = "create list test";
    private static readonly string DefaultDefinition = "1 2 3";

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ListShouldReturnDefinitionWhenIdIsSet(bool withId) {
      GsaList gsaList = GsaList(withId);

      Assert.Equal(DefaultId, gsaList.Id);
      Assert.Equal(DefaultName, gsaList.Name);
      Assert.Equal(DefaultDefinition, gsaList.Definition);
    }

    private static GsaList GsaList(bool withId) {
      GsaListGoo listComponent = CreateListComponent(withId);
      GsaModelGoo gsaModelGoo = CreateModelComponent(listComponent);
      return gsaModelGoo.Value.GetLists()[0];
    }

    private static GsaListGoo CreateListComponent(bool withId) {
      var createListComponent = new CreateList();
      createListComponent.CreateAttributes();

      if (withId) {
        ComponentTestHelper.SetInput(createListComponent, DefaultId, 0);
      }

      ComponentTestHelper.SetInput(createListComponent, DefaultName, 1);
      ComponentTestHelper.SetInput(createListComponent, DefaultDefinition, 2);

      var gridLineOutput = (GsaListGoo)ComponentTestHelper.GetOutput(createListComponent);

      return gridLineOutput;
    }

    private static GsaModelGoo CreateModelComponent(GsaListGoo gsaListGoo) {
      var createModelComponent = new CreateModel();
      createModelComponent.CreateAttributes();

      ComponentTestHelper.SetInput(createModelComponent, gsaListGoo, 0);
      var gsaModelGoo = (GsaModelGoo)ComponentTestHelper.GetOutput(createModelComponent);

      return gsaModelGoo;
    }
  }
}
