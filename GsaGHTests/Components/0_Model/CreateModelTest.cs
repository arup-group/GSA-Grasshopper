using GsaGH.Components;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using Xunit;
using System.Collections.Generic;
using OasysGH.Components;
using static GsaGHTests.Helpers.Export.AssembleModelTests;

namespace GsaGHTests.Model
{
  [Collection("GrasshopperFixture collection")]
  public class CreateModelTest
  {
    public static GH_OasysDropDownComponent CreateModelFromGeometry(List<GsaElement1dGoo> elem1d, List<GsaElement2dGoo> elem2d, List<GsaMember1dGoo> mem1d, List<GsaMember2dGoo> mem2d, List<GsaMember3dGoo> mem3d, ModelUnit unit)
    {
      var comp = new CreateModel();
      comp.CreateAttributes();
      comp.SetSelected(0, (int)unit);
      if (elem1d != null)
        foreach (GsaElement1dGoo input in elem1d)
          ComponentTestHelper.SetInput(comp, input, 2);
      if (elem2d != null)
        foreach (GsaElement2dGoo input in elem2d)
          ComponentTestHelper.SetInput(comp, input, 2);
      if (mem1d != null)
        foreach (GsaMember1dGoo input in mem1d)
          ComponentTestHelper.SetInput(comp, input, 2);
      if (mem2d != null)
        foreach (GsaMember2dGoo input in mem2d)
          ComponentTestHelper.SetInput(comp, input, 2);
      if (mem3d != null)
        foreach (GsaMember3dGoo input in mem3d)
          ComponentTestHelper.SetInput(comp, input, 2);
      return comp;
    }
  }
}
