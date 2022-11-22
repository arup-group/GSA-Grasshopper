﻿using Grasshopper.Kernel.Types;
using GsaGH.Components;
using GsaGH.Parameters;
using GsaGHTests.Components.Properties;
using GsaGHTests.Helpers;
using OasysGH.Components;
using OasysUnits.Units;
using OasysUnits;
using Rhino.Geometry;
using Xunit;
using static GsaGH.Parameters.GsaMaterial;

namespace GsaGHTests.Components.Geometry
{
  [Collection("GrasshopperFixture collection")]
  public class CreateMember3dTests
  {
    public static GH_OasysComponent ComponentMother()
    {
      var comp = new CreateMember3d();
      comp.CreateAttributes();

      Box box = Box.Empty;
      box.X = new Interval(0, 10);
      box.Y = new Interval(0, 10);
      box.Z = new Interval(0, 10);
      ComponentTestHelper.SetInput(comp, box, 0);
      ComponentTestHelper.SetInput(comp, ComponentTestHelper.GetOutput(CreateProp3dTests.ComponentMother()), 1);
      ComponentTestHelper.SetInput(comp, 0.5, 2);

      return comp;
    }

    [Fact]
    public void CreateComponentTest()
    {
      // Arrange & Act
      var comp = ComponentMother();

      // Assert
      GsaMember3dGoo output = (GsaMember3dGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal(MatType.CONCRETE, output.Value.Property.Material.MaterialType);
      Assert.Equal(0.5, output.Value.MeshSize);
    }
  }
}
