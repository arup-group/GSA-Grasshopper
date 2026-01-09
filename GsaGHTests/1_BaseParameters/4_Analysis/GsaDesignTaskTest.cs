using System.Linq;

using GsaGH.Parameters;

using Xunit;

namespace GsaGHTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  public class GsaDesignTaskTest {

    [Fact]
    public void ConstructorTest() {
      var designTask = new GsaSteelDesignTask("myTask");
      Assert.NotNull(designTask);
      Assert.Equal(0, designTask.Id);
      Assert.Null(designTask.List);
      Assert.Equal("myTask", designTask.Name);
    }

    [Fact]
    public void ImportFromExistingModelTest() {
      var model = new GsaAPI.Model();
      var apiTask = new GsaAPI.SteelDesignTask("myTask") {
        ListDefinition = "All",
        CombinationCaseId = 1,
      };
      model.AddCombinationCase(new GsaAPI.CombinationCase("combo", "1.5A1"));
      model.AddDesignTask(apiTask);

      var designTask = new GsaSteelDesignTask(
        model.SteelDesignTasks().First(), new GsaModel(model));
      Assert.NotNull(designTask);
      Assert.Equal(1, designTask.Id);
      Assert.Equal("myTask", designTask.Name);
      Assert.NotNull(designTask.List);
      Assert.Equal("all", designTask.List.Definition);
    }

    [Fact]
    public void ImportFromExistingModelWithListTest() {
      var model = new GsaAPI.Model();
      var apiTask = new GsaAPI.SteelDesignTask("myTask") {
        ListDefinition = "\"my list\"",
        CombinationCaseId = 1,
      };
      model.AddCombinationCase(new GsaAPI.CombinationCase("combo", "1.5A1"));
      var apiList = new GsaAPI.EntityList() {
        Definition = "1 3",
        Name = "my list",
        Type = GsaAPI.EntityType.Member
      };
      model.AddList(apiList);
      model.AddDesignTask(apiTask);

      var designTask = new GsaSteelDesignTask(
        model.SteelDesignTasks().First(), new GsaModel(model));
      Assert.NotNull(designTask);
      Assert.Equal(1, designTask.Id);
      Assert.Equal("myTask", designTask.Name);
      Assert.NotNull(designTask.List);
      Assert.Equal("my list", designTask.List.Name);
      Assert.Equal("\"my list\"", designTask.ApiTask.ListDefinition);
    }

    [Fact]
    public void ToStringTest1() {
      var designTask = new GsaSteelDesignTask("myTask");
      designTask.ApiTask.ListDefinition = "All";
      designTask.ApiTask.CombinationCaseId = 2;
      designTask.ApiTask.UpperTargetUtilisationLimit = 0.5;
      designTask.ApiTask.PrimaryObjective = GsaAPI.SteelDesignObjective.MinWeight;
      Assert.Equal("(Steel) myTask Case:C2 Def:all Target:0.5 Obj:MinWeight", designTask.ToString());
    }

    [Fact]
    public void ToStringTest2() {
      var designTask = new GsaSteelDesignTask("myTask") {
        List = new GsaList() {
          Definition = "1 3",
          EntityType = EntityType.Member,
          Name = "myList"
        }
      };
      designTask.ApiTask.CombinationCaseId = 99;
      designTask.ApiTask.UpperTargetUtilisationLimit = 0.8;
      designTask.ApiTask.PrimaryObjective = GsaAPI.SteelDesignObjective.MinDepth;
      designTask.ApiTask.GroupSectionsByPool = true;
      Assert.Equal("(Steel) myTask Case:C99 Def:\"myList\" Target:0.8 Obj:MinDepth Grouped",
        designTask.ToString());
    }
  }
}
