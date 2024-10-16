using GsaGH.Components;

using GsaGHTests.Helpers;

using Xunit;

namespace GsaGHTests.Components.Properties {
  [Collection("GrasshopperFixture collection")]
  public class CreateProp2dTests {

    public static Create2dProperty ComponentMother(bool isLoadType) {
      if (isLoadType) {
        return GetLoadPanelProperty();
      }

      return GetFlatPlateProperty();
    }

    public static Create2dProperty GetFlatPlateProperty() {
      var component = new Create2dProperty();
      component.CreateAttributes();
      component.SetSelected(0, 2); // 2-"Flat Plate"
      component.SetSelected(1, 3); // "in"
      ComponentTestHelper.SetInput(component, 14.0, 0); // 14 inch thickness
      ComponentTestHelper.SetInput(component,
        ComponentTestHelper.GetOutput(CreateCustomMaterialTests.ComponentMother()), 1);
      return component;
    }

    public static Create2dProperty GetLoadPanelProperty() {
      var component = new Create2dProperty();
      component.CreateAttributes();
      component.SetSelected(0, 5); // 5-"Load"
      component.SetSelected(1, 3); // "TwoEdges"
      ComponentTestHelper.SetInput(component, 2, 0);
      return component;
    }
  }

}
