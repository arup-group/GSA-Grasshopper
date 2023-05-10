using System.Drawing;
using Grasshopper.Kernel.Types;
using GsaGH.Components;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using OasysGH.Components;
using Xunit;
using static GsaGH.Parameters.GsaMaterial;

namespace GsaGHTests.Components.Geometry {
  [Collection("GrasshopperFixture collection")]
  public class EditMember3dTests {

    public static GH_OasysComponent ComponentMother() {
      var comp = new EditMember3d();
      comp.CreateAttributes();

      ComponentTestHelper.SetInput(comp,
        ComponentTestHelper.GetOutput(CreateMember3dTests.ComponentMother()), 0);

      return comp;
    }

    [Fact]
    public void CreateComponentTest1() {
      GH_OasysComponent comp = ComponentMother();

      var output0 = (GsaMember3dGoo)ComponentTestHelper.GetOutput(comp, 0);
      var output1 = (GH_Integer)ComponentTestHelper.GetOutput(comp, 1);
      var output2 = (GH_Mesh)ComponentTestHelper.GetOutput(comp, 2);
      var output3 = (GsaProp3dGoo)ComponentTestHelper.GetOutput(comp, 3);
      var output4 = (GH_Number)ComponentTestHelper.GetOutput(comp, 4);
      var output5 = (GH_Boolean)ComponentTestHelper.GetOutput(comp, 5);
      var output6 = (GH_String)ComponentTestHelper.GetOutput(comp, 6);
      var output7 = (GH_Integer)ComponentTestHelper.GetOutput(comp, 7);
      var output8 = (GH_Colour)ComponentTestHelper.GetOutput(comp, 8);
      var output9 = (GH_Boolean)ComponentTestHelper.GetOutput(comp, 9);
      var output10 = (GH_String)ComponentTestHelper.GetOutput(comp, 10);

      Assert.Equal(MaterialType.Concrete, output0.Value.Prop3d.Material.MaterialType);
      Assert.Equal(0.5, output0.Value.MeshSize);
      Assert.Equal(0, output1.Value);
      Assert.NotNull(output2.Value);
      Assert.Equal(MaterialType.Concrete, output3.Value.Material.MaterialType);
      Assert.Equal(0.5, output4.Value);
      Assert.True(output5.Value);
      Assert.Equal("", output6.Value);
      Assert.Equal(0, output7.Value);
      Assert.Equal(0, output8.Value.R);
      Assert.Equal(0, output8.Value.G);
      Assert.Equal(0, output8.Value.B);
      Assert.False(output9.Value);
      Assert.Equal("", output10.Value);
    }

    [Fact]
    public void CreateComponentTest2() {
      GH_OasysComponent comp = ComponentMother();
      ComponentTestHelper.SetInput(comp, 7, 1);
      ComponentTestHelper.SetInput(comp, 0.7, 4);
      ComponentTestHelper.SetInput(comp, false, 5);
      ComponentTestHelper.SetInput(comp, "name", 6);
      ComponentTestHelper.SetInput(comp, 1, 7);
      ComponentTestHelper.SetInput(comp, new GH_Colour(Color.White), 8);
      ComponentTestHelper.SetInput(comp, true, 9);

      var output0 = (GsaMember3dGoo)ComponentTestHelper.GetOutput(comp, 0);
      var output1 = (GH_Integer)ComponentTestHelper.GetOutput(comp, 1);
      var output2 = (GH_Mesh)ComponentTestHelper.GetOutput(comp, 2);
      var output3 = (GsaProp3dGoo)ComponentTestHelper.GetOutput(comp, 3);
      var output4 = (GH_Number)ComponentTestHelper.GetOutput(comp, 4);
      var output5 = (GH_Boolean)ComponentTestHelper.GetOutput(comp, 5);
      var output6 = (GH_String)ComponentTestHelper.GetOutput(comp, 6);
      var output7 = (GH_Integer)ComponentTestHelper.GetOutput(comp, 7);
      var output8 = (GH_Colour)ComponentTestHelper.GetOutput(comp, 8);
      var output9 = (GH_Boolean)ComponentTestHelper.GetOutput(comp, 9);
      var output10 = (GH_String)ComponentTestHelper.GetOutput(comp, 10);

      Assert.Equal(MaterialType.Concrete, output0.Value.Prop3d.Material.MaterialType);
      Assert.Equal(0.7, output0.Value.MeshSize);
      Assert.Equal(7, output1.Value);
      Assert.NotNull(output2.Value);
      Assert.Equal(MaterialType.Concrete, output3.Value.Material.MaterialType);
      Assert.Equal(0.7, output4.Value);
      Assert.True(output5.Value);
      Assert.Equal("name", output6.Value);
      Assert.Equal(1, output7.Value);
      Assert.Equal(255, output8.Value.R);
      Assert.Equal(255, output8.Value.G);
      Assert.Equal(255, output8.Value.B);
      Assert.True(output9.Value);
      Assert.Equal("", output10.Value);
    }
  }
}
