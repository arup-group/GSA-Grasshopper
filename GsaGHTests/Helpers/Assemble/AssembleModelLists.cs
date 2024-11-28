using GsaGH.Components;
using GsaGH.Parameters;

using Xunit;

namespace GsaGHTests.Helpers.Assemble {
  [Collection("GrasshopperFixture collection")]
  public class AssembleModelLists {
    private const int DefaultId = 1;
    private const string ExpectedName = "create list test";
    private const string ExpectedDefinition = "1 2 3";

    [Fact]
    public void ListShouldReturnDefinitionWhenIdIsSet() {
      const int ExpectedId = 2;
      GsaList gsaList = GsaList(ExpectedId);

      Assert.Equal(ExpectedId, gsaList.Id);
      Assert.Equal(ExpectedName, gsaList.Name);
      Assert.Equal(ExpectedDefinition, gsaList.Definition);
    }

    [Fact]
    public void ListShouldReturnDefinitionWhenIdIsNotSet() {
      GsaList gsaList = GsaList(DefaultId);

      Assert.Equal(DefaultId, gsaList.Id);
      Assert.Equal(ExpectedName, gsaList.Name);
      Assert.Equal(ExpectedDefinition, gsaList.Definition);
    }

    private static GsaList GsaList(int? id) {
      GsaListGoo listComponent = CreateListComponent(id);
      GsaModelGoo gsaModelGoo = CreateModelComponent(listComponent);
      return gsaModelGoo.Value.GetLists()[0];
    }

    private static GsaListGoo CreateListComponent(int? id) {
      var createListComponent = new CreateList();
      createListComponent.CreateAttributes();

      if (id != DefaultId) {
        ComponentTestHelper.SetInput(createListComponent, id, 0);
      }

      ComponentTestHelper.SetInput(createListComponent, ExpectedName, 1);
      ComponentTestHelper.SetInput(createListComponent, ExpectedDefinition, 2);

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
