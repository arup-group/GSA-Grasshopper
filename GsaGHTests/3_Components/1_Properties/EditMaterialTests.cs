using Grasshopper.Kernel.Types;

using GsaGH.Components;
using GsaGH.Parameters;

using GsaGHTests.Components.Properties;
using GsaGHTests.Helpers;

using OasysGH.Components;

using Xunit;

namespace GsaGHTests.Properties {
  [Collection("GrasshopperFixture collection")]
  public class EditMaterialTests {

    public static GH_OasysComponent ComponentMother() {
      var comp = new EditMaterial();
      comp.CreateAttributes();
      ComponentTestHelper.SetInput(comp,
        (GsaMaterialGoo)ComponentTestHelper.GetOutput(CreateCustomMaterialTests.ComponentMother()));
      return comp;
    }

    [Theory]
    [InlineData(1, "myMat", "Concrete")]
    [InlineData(2, "myMat", "Steel")]
    [InlineData(2, "myMat", "Frp")]
    [InlineData(2, "myMat", "Aluminium")]
    [InlineData(2, "myMat", "Timber")]
    [InlineData(2, "myMat", "Glass")]
    [InlineData(2, "myMat", "Fabric")]
    [InlineData(2, "myMat", "Custom")]
    public void EditCustomMaterialStringInputTest(int id, string name, string type) {
      GH_OasysComponent comp = ComponentMother();
      ComponentTestHelper.SetInput(comp, id, 1);
      ComponentTestHelper.SetInput(comp, name, 2);
      ComponentTestHelper.SetInput(comp, type, 4);
      int i = 0;
      var materialGoo = (GsaMaterialGoo)ComponentTestHelper.GetOutput(comp, i++);
      var idGoo = (GH_Integer)ComponentTestHelper.GetOutput(comp, i++);
      var nameGoo = (GH_String)ComponentTestHelper.GetOutput(comp, i++);
      i++;
      var typeGoo = (GH_String)ComponentTestHelper.GetOutput(comp, i++);

      Assert.Equal(id, materialGoo.Value.Id);
      Assert.Equal(name, materialGoo.Value.Name);
      Assert.Equal(type, materialGoo.Value.MaterialType.ToString());
      Assert.Equal(id, idGoo.Value);
      Assert.Equal(name, nameGoo.Value);
      Assert.Equal(type, typeGoo.Value);
    }

    [Theory]
    [InlineData(1, "myMat", 2, "Concrete")]
    [InlineData(2, "myMat", 1, "Steel")]
    [InlineData(2, "myMat", 5, "Frp")]
    [InlineData(2, "myMat", 3, "Aluminium")]
    [InlineData(2, "myMat", 7, "Timber")]
    [InlineData(2, "myMat", 4, "Glass")]
    [InlineData(2, "myMat", 8, "Fabric")]
    [InlineData(2, "myMat", 0, "Custom")]
    public void EditCustomMaterialIntInputTest(int id, string name, int type, string expectedType) {
      GH_OasysComponent comp = ComponentMother();
      ComponentTestHelper.SetInput(comp, id, 1);
      ComponentTestHelper.SetInput(comp, name, 2);
      ComponentTestHelper.SetInput(comp, type, 4);
      int i = 0;
      var materialGoo = (GsaMaterialGoo)ComponentTestHelper.GetOutput(comp, i++);
      var idGoo = (GH_Integer)ComponentTestHelper.GetOutput(comp, i++);
      var nameGoo = (GH_String)ComponentTestHelper.GetOutput(comp, i++);
      i++;
      var typeGoo = (GH_String)ComponentTestHelper.GetOutput(comp, i++);

      Assert.Equal(id, materialGoo.Value.Id);
      Assert.Equal(name, materialGoo.Value.Name);
      Assert.Equal(expectedType, materialGoo.Value.MaterialType.ToString());
      Assert.Equal(id, idGoo.Value);
      Assert.Equal(name, nameGoo.Value);
      Assert.Equal(expectedType, typeGoo.Value);
    }

    [Fact]
    public void EditStandardMaterialTest() {
      var comp = new EditMaterial();
      comp.CreateAttributes();
      ComponentTestHelper.SetInput(comp,
        (GsaMaterialGoo)ComponentTestHelper.GetOutput(CreateMaterialTests.ComponentMother()));
      ComponentTestHelper.SetInput(comp, 2, 1);
      ComponentTestHelper.SetInput(comp, "myMat", 2);
      int i = 0;
      var materialGoo = (GsaMaterialGoo)ComponentTestHelper.GetOutput(comp, i++);
      var idGoo = (GH_Integer)ComponentTestHelper.GetOutput(comp, i++);
      var nameGoo = (GH_String)ComponentTestHelper.GetOutput(comp, i++);
      i++;
      var typeGoo = (GH_String)ComponentTestHelper.GetOutput(comp, i++);

      Assert.Equal(2, materialGoo.Value.Id);
      Assert.Equal("myMat", materialGoo.Value.Name);
      Assert.Equal(MatType.Concrete, materialGoo.Value.MaterialType);
      Assert.Equal(2, idGoo.Value);
      Assert.Equal("myMat", nameGoo.Value);
      Assert.Equal("Concrete", typeGoo.Value);
    }

    [Fact]
    public void EditStandardMaterialsAnalysisMaterialTest() {
      var comp = new EditMaterial();
      comp.CreateAttributes();
      ComponentTestHelper.SetInput(comp,
        (GsaMaterialGoo)ComponentTestHelper.GetOutput(CreateMaterialTests.ComponentMother()));
      ComponentTestHelper.SetInput(comp,
        (GsaMaterialGoo)ComponentTestHelper.GetOutput(CreateCustomMaterialTests.ComponentMother()), 3);
      var materialGoo = (GsaMaterialGoo)ComponentTestHelper.GetOutput(comp, 0);
      var customMaterialGoo = (GsaMaterialGoo)ComponentTestHelper.GetOutput(comp, 3);

      Assert.Equal("name", materialGoo.Value.AnalysisMaterial.Name);
      Assert.Equal("name", customMaterialGoo.Value.AnalysisMaterial.Name);
    }

    [Fact]
    public void EditStandardMaterialsAnalysisMaterialAndChangeTypeTest() {
      var comp = new EditMaterial();
      comp.CreateAttributes();
      ComponentTestHelper.SetInput(comp,
        (GsaMaterialGoo)ComponentTestHelper.GetOutput(CreateMaterialTests.ComponentMother()));
      ComponentTestHelper.SetInput(comp,
        (GsaMaterialGoo)ComponentTestHelper.GetOutput(CreateCustomMaterialTests.ComponentMother()), 3);
      ComponentTestHelper.SetInput(comp, "Timber", 4);
      var materialGoo = (GsaMaterialGoo)ComponentTestHelper.GetOutput(comp, 0);
      var customMaterialGoo = (GsaMaterialGoo)ComponentTestHelper.GetOutput(comp, 3);

      Assert.Equal("name", materialGoo.Value.AnalysisMaterial.Name);
      Assert.Equal("Created from Concrete C30/37", customMaterialGoo.Value.AnalysisMaterial.Name);
    }
  }
}
