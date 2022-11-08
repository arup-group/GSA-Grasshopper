﻿using GsaGH.Parameters;
using GsaGHTests.Helpers;
using OasysGH.Components;
using OasysUnits;
using OasysUnits.Units;
using Xunit;

namespace GsaGHTests.Components
{
  [Collection("GrasshopperFixture collection")]
  public class CreateOffsetTests
  {
    public static GH_OasysDropDownComponent ComponentMother()
    {
      var comp = new GsaGH.Components.CreateOffset();
      comp.CreateAttributes();
      return comp;
    }

    [Fact]
    public void CreateComponent()
    {
      var comp = ComponentMother();

      ComponentTestHelper.SetInput(comp, new Length(0.5, LengthUnit.Meter));
      ComponentTestHelper.SetInput(comp, new Length(-0.75, LengthUnit.Meter), 1);
      ComponentTestHelper.SetInput(comp, new Length(1.99, LengthUnit.Meter), 2);
      ComponentTestHelper.SetInput(comp, new Length(0.99, LengthUnit.Meter), 3);

      // set output data to Gsa-goo type
      GsaOffsetGoo output = (GsaOffsetGoo)ComponentTestHelper.GetOutput(comp);

      // cast from -goo to Gsa-GH data type
      GsaOffset offset = new GsaOffset();
      output.CastTo(ref offset);

      Assert.Equal(0.5, offset.X1.Value);
      Assert.Equal(-0.75, offset.X2.Value);
      Assert.Equal(1.99, offset.Y.Value);
      Assert.Equal(0.99, offset.Z.Value);
    }
  }
}
