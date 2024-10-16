﻿using GsaAPI;
using GsaGH.Components;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using OasysGH.Components;
using OasysUnits;
using Xunit;
using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGHTests.Components.Properties {
  [Collection("GrasshopperFixture collection")]
  public class CreateProp2dTests {

    public static GH_OasysDropDownComponent ComponentMother(bool isLoadType) {
      var comp = new Create2dProperty();
      comp.CreateAttributes();

      comp.SetSelected(0, isLoadType ? 5 : 2); // set type to 2-"Flat Plate" or 5-"Load"
      comp.SetSelected(1, 3); // set measure to "in" or for load "TwoEdges"

      if (isLoadType) {
        ComponentTestHelper.SetInput(comp, 2, 0);
      } else {
        ComponentTestHelper.SetInput(comp, 14.0, 0); // 14 inch thickness
        ComponentTestHelper.SetInput(comp,
          ComponentTestHelper.GetOutput(CreateCustomMaterialTests.ComponentMother()), 1);
      }

      return comp;
    }

    [Fact]
    public void CreateFlatComponent() {
      GH_OasysDropDownComponent comp = ComponentMother(false);

      var output = (GsaProperty2dGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(Property2D_Type.PLATE, output.Value.ApiProp2d.Type);
      Assert.Equal(new Length(14, LengthUnit.Inch), output.Value.Thickness);
    }

    [Fact]
    public void CreateLoadComponent() {
      GH_OasysDropDownComponent comp = ComponentMother(true);

      var output = (GsaProperty2dGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(Property2D_Type.LOAD, output.Value.ApiProp2d.Type);
      Assert.Equal(SupportType.TwoEdges, output.Value.ApiProp2d.SupportType);
      Assert.Equal(2, output.Value.ApiProp2d.ReferenceEdge);
    }
  }
}
