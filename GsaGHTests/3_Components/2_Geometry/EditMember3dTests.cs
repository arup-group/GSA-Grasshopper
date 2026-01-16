using System.Drawing;

using Grasshopper.Kernel.Types;

using GsaGH.Components;
using GsaGH.Parameters;

using GsaGHTests.Helpers;

using OasysGH.Components;

using Rhino.Geometry;

using Xunit;

namespace GsaGHTests.Components.Geometry {
  [Collection("GrasshopperFixture collection")]
  public class EditMember3dTests_WithoutSettingInputs {
    private readonly EditMember3dTestsHelper _helper;

    public EditMember3dTests_WithoutSettingInputs() {
      _helper = new EditMember3dTestsHelper();
    }

    [Fact]
    public void ComponentReturnValidMaterialTypeForMember() {
      GsaMember3d output = _helper.GetMemberOutput();
      Assert.Equal(MatType.Concrete, output.Prop3d.Material.MaterialType);
    }

    [Fact]
    public void ComponentReturnValidMeshSizeForMember() {
      GsaMember3d output = _helper.GetMemberOutput();
      Assert.Equal(0.5, output.ApiMember.MeshSize);
    }

    [Fact]
    public void ComponentReturnValiGroupValueForMember() {
      GsaMember3d output = _helper.GetMemberOutput();
      Assert.Equal(1, output.ApiMember.Group);
    }

    [Fact]
    public void ComponentReturnValiId() {
      int output = _helper.GetIdOutput();
      Assert.Equal(0, output);
    }

    [Fact]
    public void ComponentReturnNotNullGeometry() {
      Mesh output = _helper.GetSolidGeometryOutput();
      Assert.NotNull(output);
    }

    [Fact]
    public void ComponentReturnConcreteMaterialType() {
      GsaProperty3d output = _helper.Get3dPropertyOutput();
      Assert.Equal(MatType.Concrete, output.Material.MaterialType);
    }

    [Fact]
    public void ComponentReturnValidMeshSize() {
      double output = _helper.GetMeshSizeOutput();
      Assert.Equal(0.5, output);
    }

    [Fact]
    public void ComponentReturnValidIntersectorValue() {
      bool output = _helper.GetIntersectorOutput();
      Assert.True(output);
    }

    [Fact]
    public void ComponentReturnValidName() {
      string output = _helper.GetNameOutput();
      Assert.Empty(output);
    }

    [Fact]
    public void ComponentReturnValidGroupValue() {
      int output = _helper.GetGroupOutput();
      Assert.Equal(1, output);
    }

    [Fact]
    public void ComponentReturnValidColor() {
      Color output = _helper.GetColorOutput();
      Assert.Equal("ff000000", output.Name);
    }

    [Fact]
    public void ComponentReturnValidDummyValue() {
      bool output = _helper.GetDummyOutput();
      Assert.False(output);
    }

    [Fact]
    public void ComponentReturnValidTopology() {
      string output = _helper.GetTopologyOutput();
      Assert.Empty(output);
    }
  }

  [Collection("GrasshopperFixture collection")]
  public class EditMember3dTests_ForInputsSet {
    private readonly EditMember3dTestsHelper _helper;

    public EditMember3dTests_ForInputsSet() {
      _helper = new EditMember3dTestsHelper();
      _helper.SetIdInput(7);
      _helper.SetMeshSizeInput(0.7);
      _helper.SetIntersectorInput(false);
      _helper.SetNameInput("name");
      _helper.SetGroupInput(1);
      _helper.SetColorInput(new GH_Colour(Color.White));
      _helper.SetDummyInput(true);
    }

    [Fact]
    public void EditMember3dReturnValidMaterialTypeForMember() {
      GsaMember3d output = _helper.GetMemberOutput();
      Assert.Equal(MatType.Concrete, output.Prop3d.Material.MaterialType);
    }

    [Fact]
    public void EditMember3dReturnValidMeshSizeForMember() {
      GsaMember3d output = _helper.GetMemberOutput();
      Assert.Equal(0.7, output.ApiMember.MeshSize);
    }

    [Fact]
    public void EditMember3dReturnValiGroupValueForMember() {
      GsaMember3d output = _helper.GetMemberOutput();
      Assert.Equal(1, output.ApiMember.Group);
    }

    [Fact]
    public void EditMember3dReturnValiId() {
      int output = _helper.GetIdOutput();
      Assert.Equal(7, output);
    }

    [Fact]
    public void EditMember3dReturnNotNullGeometry() {
      Mesh output = _helper.GetSolidGeometryOutput();
      Assert.NotNull(output);
    }

    [Fact]
    public void EditMember3dReturnConcreteMaterialType() {
      GsaProperty3d output = _helper.Get3dPropertyOutput();
      Assert.Equal(MatType.Concrete, output.Material.MaterialType);
    }

    [Fact]
    public void EditMember3dReturnValidMeshSize() {
      double output = _helper.GetMeshSizeOutput();
      Assert.Equal(0.7, output);
    }

    [Fact]
    public void EditMember3dReturnValidIntersectorValue() {
      bool output = _helper.GetIntersectorOutput();
      Assert.False(output);
    }

    [Fact]
    public void EditMember3dReturnValidName() {
      string output = _helper.GetNameOutput();
      Assert.Equal("name", output);
    }

    [Fact]
    public void EditMember3dReturnValidGroupValue() {
      int output = _helper.GetGroupOutput();
      Assert.Equal(1, output);
    }

    [Fact]
    public void EditMember3dReturnValidColor() {
      Color output = _helper.GetColorOutput();
      Assert.Equal("ffffffff", output.Name);
    }

    [Fact]
    public void EditMember3dReturnValidDummyValue() {
      bool output = _helper.GetDummyOutput();
      Assert.True(output);
    }

    [Fact]
    public void EditMember3dReturnValidTopology() {
      string output = _helper.GetTopologyOutput();
      Assert.Empty(output);
    }
  }

  public class EditMember3dTestsHelper {
    private readonly GH_OasysComponent _component;

    public EditMember3dTestsHelper() {
      _component = ComponentMother();
    }

    private GH_OasysComponent ComponentMother() {
      var comp = new Edit3dMember();
      comp.CreateAttributes();

      ComponentTestHelper.SetInput(comp, ComponentTestHelper.GetOutput(CreateMember3dTests.ComponentMother()), 0);

      return comp;
    }

    public GsaMember3d GetMemberOutput() {
      return ComponentTestHelper.GetMember3dOutput(_component, 0);
    }

    public int GetIdOutput() {
      return ComponentTestHelper.GetIntOutput(_component, 1);
    }

    public Mesh GetSolidGeometryOutput() {
      return ComponentTestHelper.GetMeshOutput(_component, 2);
    }

    public GsaProperty3d Get3dPropertyOutput() {
      return ComponentTestHelper.Get3dPropertyOutput(_component, 3);
    }

    public double GetMeshSizeOutput() {
      return ComponentTestHelper.GetNumberOutput(_component, 4);
    }

    public bool GetIntersectorOutput() {
      return ComponentTestHelper.GetBoolOutput(_component, 5);
    }

    public string GetNameOutput() {
      return ComponentTestHelper.GetStringOutput(_component, 6);
    }

    public int GetGroupOutput() {
      return ComponentTestHelper.GetIntOutput(_component, 7);
    }

    public Color GetColorOutput() {
      return ComponentTestHelper.GetColorOutput(_component, 8);
    }

    public bool GetDummyOutput() {
      return ComponentTestHelper.GetBoolOutput(_component, 9);
    }

    public string GetTopologyOutput() {
      return ComponentTestHelper.GetStringOutput(_component, 10);
    }

    public void SetIdInput(int input) {
      ComponentTestHelper.SetInput(_component, input, 1);
    }

    public void SetMeshSizeInput(double input) {
      ComponentTestHelper.SetInput(_component, input, 4);
    }

    public void SetIntersectorInput(bool input) {
      ComponentTestHelper.SetInput(_component, input, 5);
    }

    public void SetNameInput(string input) {
      ComponentTestHelper.SetInput(_component, input, 6);
    }

    public void SetGroupInput(int input) {
      ComponentTestHelper.SetInput(_component, input, 7);
    }

    public void SetColorInput(GH_Colour input) {
      ComponentTestHelper.SetInput(_component, input, 8);
    }

    public void SetDummyInput(bool input) {
      ComponentTestHelper.SetInput(_component, input, 9);
    }
  }
}
