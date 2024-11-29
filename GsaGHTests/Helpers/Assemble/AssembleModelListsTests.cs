using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

using GsaAPI;

using GsaGH.Components;
using GsaGH.Parameters;

using Rhino.Geometry;

using Xunit;

namespace GsaGHTests.Helpers.Assemble {
  [Collection("GrasshopperFixture collection")]
  public class AssembleModelListsTests {
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
    public void ListWithEmptyNodesShouldReturnDefinitionWhenIdIsSet() {
      const int ExpectedId = 2;
      GsaList gsaList = GsaList(ExpectedId);
      gsaList._nodes = new ConcurrentBag<GsaNodeGoo>();

      Assert.Equal(ExpectedId, gsaList.Id);
      Assert.Equal(ExpectedName, gsaList.Name);
      Assert.Equal(ExpectedDefinition, gsaList.Definition);
    }

    [Fact]
    public void ListShouldReturnDefinitionWhenIdIsNotSet() {
      GsaList gsaList = GsaList(null);

      Assert.Equal(DefaultId, gsaList.Id);
      Assert.Equal(ExpectedName, gsaList.Name);
      Assert.Equal(ExpectedDefinition, gsaList.Definition);
    }

    [Fact]
    public void ListWithNodesShouldIgnoreNulls() {
      var gsaList = GetNodeList();

      List<object> gooObjects = GetDummyNodes();
      gooObjects.Add(null);
      gooObjects.Add(new GsaNodeGoo(null));
      gsaList.SetListGooObjects(gooObjects);

      var gridLineOutput = new GsaListGoo(gsaList);
      GsaModelGoo gsaModelGoo = CreateModelComponent(gridLineOutput);

      var lists = gsaModelGoo.Value.GetLists();
      Assert.Single(lists);
      Assert.Equal("1 2", lists[0].Definition);
    }

    [Fact]
    public void ListWithNodesShouldProduceValidLists() {
      var gsaList = GetNodeList();

      gsaList.SetListGooObjects(GetDummyNodes());

      var gridLineOutput = new GsaListGoo(gsaList);
      GsaModelGoo gsaModelGoo = CreateModelComponent(gridLineOutput);

      var lists = gsaModelGoo.Value.GetLists();
      Assert.Single(lists);
      Assert.Equal("1 2", lists[0].Definition);
    }

    private static GsaList GetNodeList() {
      return new GsaList {
        EntityType = GsaGH.Parameters.EntityType.Node,
      };
    }

    private static List<object> GetDummyNodes() {
      var gsaNodeGoo1 = new GsaNodeGoo(new GsaNode(new Point3d(1, 2, 3)));
      var gsaNodeGoo2 = new GsaNodeGoo(new GsaNode(new Point3d(4, 5, 6)));
      var gooObjects = new List<object> {
        gsaNodeGoo1,
        gsaNodeGoo2
      };
      return gooObjects;
    }

    [Fact]
    public void nullIdShouldReturnDefault() {
      GsaList gsaList = GsaList(null);

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

      ComponentTestHelper.SetInput(createListComponent, id, 0);
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
