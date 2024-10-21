using System.Drawing;

using Grasshopper.Kernel.Types;

using GsaAPI;

using GsaGH.Components;
using GsaGH.Parameters;

using GsaGHTests.Helpers;

using OasysGH.Components;

using OasysUnits;

using Rhino.Collections;
using Rhino.Geometry;
using GsaGHTests.Components.Geometry;
using Xunit;

using LengthUnit = OasysUnits.Units.LengthUnit;
using Line = Rhino.Geometry.Line;

namespace GsaGHTests.Components.Geometry {
  [Collection("GrasshopperFixture collection")]
  public class EditElement2dTests {

    public static GH_OasysComponent ComponentMother() {
      var component = new Edit2dElement();
      component.CreateAttributes();

      return component;
    }
    [Fact]
    public void SettingOffsetToLoadPanelWillThrowRunTimeError() {

      var mesh = Mesh.CreateFromPlanarBoundary(CreateElement2dTests.Get2dPolyline(),
        MeshingParameters.DefaultAnalysisMesh, 0.001);

      var fe2dElement = new GsaElement2d(mesh);
      var fe2dLoadPanel = new GsaElement2d(CreateElement2dTests.Get2dPolyline());

      GH_OasysComponent feComponet = ComponentMother();

      ComponentTestHelper.SetInput(feComponet, new GsaElement2dGoo(fe2dElement), 0);
      ComponentTestHelper.SetInput(feComponet, new GsaOffsetGoo(new GsaOffset(1, 2, 3, 4)), 4);
      var offset = (GsaOffsetGoo)ComponentTestHelper.GetOutput(feComponet, 6);
      Assert.Equal(1, offset.Value.X1.Value);
      Assert.Equal(2, offset.Value.X2.Value);
      Assert.Equal(3, offset.Value.Y.Value);
      Assert.Equal(4, offset.Value.Z.Value);

      GH_OasysComponent loadPanelComponet = ComponentMother();
      ComponentTestHelper.SetInput(loadPanelComponet, new GsaElement2dGoo(fe2dLoadPanel), 0);
      ComponentTestHelper.SetInput(loadPanelComponet, new GsaOffsetGoo(new GsaOffset(1, 2, 3, 4)), 4);
      System.Collections.Generic.List<IGH_Goo> output = ComponentTestHelper.GetResultOutputAllData(loadPanelComponet, 6);
      Assert.Contains("One runtime error", loadPanelComponet.InstanceDescription);
      Assert.Empty(output);
    }

  }
}
