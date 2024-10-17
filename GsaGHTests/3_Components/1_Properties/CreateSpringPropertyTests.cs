using GsaAPI;

using GsaGH.Components;
using GsaGH.Parameters;

using GsaGHTests.Helpers;

using OasysGH.Components;

using Xunit;

namespace GsaGHTests.Components.Properties {
  [Collection("GrasshopperFixture collection")]
  public class CreateSpringPropertyTests {
    public static GH_OasysDropDownComponent AxialComponentMother() {
      var comp = new CreateSpringProperty();
      comp.CreateAttributes();

      comp.SetSelected(0, 0);
      comp.SetSelected(1, 2); // N/m
      ComponentTestHelper.SetInput(comp, "Name", 0);
      ComponentTestHelper.SetInput(comp, 1.2, 1);
      ComponentTestHelper.SetInput(comp, 0.1, 2);

      return comp;
    }

    public static GH_OasysDropDownComponent CompressionOnlyComponentMother() {
      var comp = new CreateSpringProperty();
      comp.CreateAttributes();

      comp.SetSelected(0, 1);
      comp.SetSelected(1, 2); // N/m
      ComponentTestHelper.SetInput(comp, "Name", 0);
      ComponentTestHelper.SetInput(comp, 1.2, 1);
      ComponentTestHelper.SetInput(comp, 0.1, 2);

      return comp;
    }

    public static GH_OasysDropDownComponent ConnectorComponentMother() {
      var comp = new CreateSpringProperty();
      comp.CreateAttributes();

      comp.SetSelected(0, 2);
      ComponentTestHelper.SetInput(comp, "Name", 0);
      ComponentTestHelper.SetInput(comp, 0.1, 1);

      return comp;
    }

    public static GH_OasysDropDownComponent FrictionComponentMother() {
      var comp = new CreateSpringProperty();
      comp.CreateAttributes();

      comp.SetSelected(0, 3);
      comp.SetSelected(1, 2); // N/m
      ComponentTestHelper.SetInput(comp, "Name", 0);
      ComponentTestHelper.SetInput(comp, 1.2, 1);
      ComponentTestHelper.SetInput(comp, 1.3, 2);
      ComponentTestHelper.SetInput(comp, 1.4, 3);
      ComponentTestHelper.SetInput(comp, 0.5, 4);
      ComponentTestHelper.SetInput(comp, 0.1, 5);

      return comp;
    }

    public static GH_OasysDropDownComponent GapComponentMother() {
      var comp = new CreateSpringProperty();
      comp.CreateAttributes();

      comp.SetSelected(0, 4);
      comp.SetSelected(1, 2); // N/m
      ComponentTestHelper.SetInput(comp, "Name", 0);
      ComponentTestHelper.SetInput(comp, 1.2, 1);
      ComponentTestHelper.SetInput(comp, 0.1, 2);

      return comp;
    }

    public static GH_OasysDropDownComponent GeneralComponentMother1() {
      var comp = new CreateSpringProperty();
      comp.CreateAttributes();

      comp.SetSelected(0, 5);
      comp.SetSelected(1, 2); // N/m
      comp.SetSelected(2, 1); // Nm/rad
      ComponentTestHelper.SetInput(comp, "Name", 0);
      ComponentTestHelper.SetInput(comp, 1, 1);
      ComponentTestHelper.SetInput(comp, 2, 3);
      ComponentTestHelper.SetInput(comp, 3, 5);
      ComponentTestHelper.SetInput(comp, 4, 7);
      ComponentTestHelper.SetInput(comp, 5, 9);
      ComponentTestHelper.SetInput(comp, 6, 11);
      ComponentTestHelper.SetInput(comp, 0.1, 13);

      return comp;
    }

    public static GH_OasysDropDownComponent GeneralComponentMother2() {
      var comp = new CreateSpringProperty();
      comp.CreateAttributes();

      comp.SetSelected(0, 5);
      comp.SetSelected(1, 2); // N/m
      comp.SetSelected(2, 1); // Nm/rad
      ComponentTestHelper.SetInput(comp, "Name", 0);
      ComponentTestHelper.SetInput(comp, 1.2, 2);
      ComponentTestHelper.SetInput(comp, 1.3, 4);
      ComponentTestHelper.SetInput(comp, 1.4, 6);
      ComponentTestHelper.SetInput(comp, 1.5, 8);
      ComponentTestHelper.SetInput(comp, 1.6, 10);
      ComponentTestHelper.SetInput(comp, 1.7, 12);
      ComponentTestHelper.SetInput(comp, 0.1, 13);

      return comp;
    }

    public static GH_OasysDropDownComponent LockupComponentMother() {
      var comp = new CreateSpringProperty();
      comp.CreateAttributes();

      comp.SetSelected(0, 6);
      comp.SetSelected(1, 2); // N/m
      comp.SetSelected(2, 2); // m
      ComponentTestHelper.SetInput(comp, "Name", 0);
      ComponentTestHelper.SetInput(comp, 1.2, 1);
      ComponentTestHelper.SetInput(comp, 1.3, 2);
      ComponentTestHelper.SetInput(comp, 1.4, 3);
      ComponentTestHelper.SetInput(comp, 0.1, 4);

      return comp;
    }

    public static GH_OasysDropDownComponent MatrixComponentMother() {
      var comp = new CreateSpringProperty();
      comp.CreateAttributes();

      comp.SetSelected(0, 7);
      ComponentTestHelper.SetInput(comp, "Name", 0);
      ComponentTestHelper.SetInput(comp, 2, 1);
      ComponentTestHelper.SetInput(comp, 0.1, 2);

      return comp;
    }

    public static GH_OasysDropDownComponent TensionOnlyComponentMother() {
      var comp = new CreateSpringProperty();
      comp.CreateAttributes();

      comp.SetSelected(0, 8);
      comp.SetSelected(1, 2); // N/m
      ComponentTestHelper.SetInput(comp, "Name", 0);
      ComponentTestHelper.SetInput(comp, 1.2, 1);
      ComponentTestHelper.SetInput(comp, 0.1, 2);

      return comp;
    }

    public static GH_OasysDropDownComponent TorsionalComponentMother() {
      var comp = new CreateSpringProperty();
      comp.CreateAttributes();

      comp.SetSelected(0, 9);
      comp.SetSelected(1, 1); // Nm/rad
      ComponentTestHelper.SetInput(comp, "Name", 0);
      ComponentTestHelper.SetInput(comp, 1.2, 1);
      ComponentTestHelper.SetInput(comp, 0.1, 2);

      return comp;
    }

    [Fact]
    public void CreateCompressionOnlyComponent() {
      GH_OasysDropDownComponent comp = CompressionOnlyComponentMother();

      var output = (GsaSpringPropertyGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal("Name", output.Value.ApiProperty.Name);
      Assert.Equal(0.1, output.Value.ApiProperty.DampingRatio);
      Assert.Equal(1.2, ((CompressionSpringProperty)output.Value.ApiProperty).Stiffness, 8);
    }

    [Fact]
    public void CreateConnectorComponent() {
      GH_OasysDropDownComponent comp = ConnectorComponentMother();

      var output = (GsaSpringPropertyGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal("Name", output.Value.ApiProperty.Name);
      Assert.Equal(0.1, output.Value.ApiProperty.DampingRatio);
    }

    [Fact]
    public void CreateFrictionComponent() {
      GH_OasysDropDownComponent comp = FrictionComponentMother();

      var output = (GsaSpringPropertyGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal("Name", output.Value.ApiProperty.Name);
      Assert.Equal(0.1, output.Value.ApiProperty.DampingRatio);
      Assert.Equal(1.2, ((FrictionSpringProperty)output.Value.ApiProperty).StiffnessX, 8);
      Assert.Equal(1.3, ((FrictionSpringProperty)output.Value.ApiProperty).StiffnessY, 8);
      Assert.Equal(1.4, ((FrictionSpringProperty)output.Value.ApiProperty).StiffnessZ, 8);
      Assert.Equal(0.5, ((FrictionSpringProperty)output.Value.ApiProperty).FrictionCoefficient, 8);
    }


    [Fact]
    public void CreateGapComponent() {
      GH_OasysDropDownComponent comp = GapComponentMother();

      var output = (GsaSpringPropertyGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal("Name", output.Value.ApiProperty.Name);
      Assert.Equal(0.1, output.Value.ApiProperty.DampingRatio);
      Assert.Equal(1.2, ((GapSpringProperty)output.Value.ApiProperty).Stiffness, 8);
    }

    [Fact]
    public void CreateGeneralComponent1() {
      GH_OasysDropDownComponent comp = GeneralComponentMother1();

      var output = (GsaSpringPropertyGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal("Name", output.Value.ApiProperty.Name);
      Assert.Equal(0.1, output.Value.ApiProperty.DampingRatio);
      Assert.Equal(1, ((GeneralSpringProperty)output.Value.ApiProperty).SpringCurveX);
      Assert.Equal(2, ((GeneralSpringProperty)output.Value.ApiProperty).SpringCurveY);
      Assert.Equal(3, ((GeneralSpringProperty)output.Value.ApiProperty).SpringCurveZ);
      Assert.Equal(4, ((GeneralSpringProperty)output.Value.ApiProperty).SpringCurveXX);
      Assert.Equal(5, ((GeneralSpringProperty)output.Value.ApiProperty).SpringCurveYY);
      Assert.Equal(6, ((GeneralSpringProperty)output.Value.ApiProperty).SpringCurveZZ);
    }

    [Fact]
    public void CreateGeneralComponent2() {
      GH_OasysDropDownComponent comp = GeneralComponentMother2();

      var output = (GsaSpringPropertyGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal("Name", output.Value.ApiProperty.Name);
      Assert.Equal(0.1, output.Value.ApiProperty.DampingRatio);
      Assert.Equal(1.2, ((GeneralSpringProperty)output.Value.ApiProperty).StiffnessX);
      Assert.Equal(1.3, ((GeneralSpringProperty)output.Value.ApiProperty).StiffnessY);
      Assert.Equal(1.4, ((GeneralSpringProperty)output.Value.ApiProperty).StiffnessZ);
      Assert.Equal(1.5, ((GeneralSpringProperty)output.Value.ApiProperty).StiffnessXX);
      Assert.Equal(1.6, ((GeneralSpringProperty)output.Value.ApiProperty).StiffnessYY);
      Assert.Equal(1.7, ((GeneralSpringProperty)output.Value.ApiProperty).StiffnessZZ);
    }

    [Fact]
    public void CreateLockupComponent() {
      GH_OasysDropDownComponent comp = LockupComponentMother();

      var output = (GsaSpringPropertyGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal("Name", output.Value.ApiProperty.Name);
      Assert.Equal(0.1, output.Value.ApiProperty.DampingRatio);
      Assert.Equal(1.2, ((LockupSpringProperty)output.Value.ApiProperty).Stiffness, 8);
      Assert.Equal(1.3, ((LockupSpringProperty)output.Value.ApiProperty).NegativeLockup, 8);
      Assert.Equal(1.4, ((LockupSpringProperty)output.Value.ApiProperty).PositiveLockup, 8);
    }


    [Fact]
    public void CreateMatrixComponent() {
      GH_OasysDropDownComponent comp = MatrixComponentMother();

      var output = (GsaSpringPropertyGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal("Name", output.Value.ApiProperty.Name);
      Assert.Equal(0.1, output.Value.ApiProperty.DampingRatio);
      Assert.Equal(2, ((MatrixSpringProperty)output.Value.ApiProperty).SpringMatrix);
    }

    [Fact]
    public void CreateTensionOnlyComponent() {
      GH_OasysDropDownComponent comp = TensionOnlyComponentMother();

      var output = (GsaSpringPropertyGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal("Name", output.Value.ApiProperty.Name);
      Assert.Equal(0.1, output.Value.ApiProperty.DampingRatio);
      Assert.Equal(1.2, ((TensionSpringProperty)output.Value.ApiProperty).Stiffness, 8);
    }


    [Fact]
    public void CreateTorsionalComponent() {
      GH_OasysDropDownComponent comp = TorsionalComponentMother();

      var output = (GsaSpringPropertyGoo)ComponentTestHelper.GetOutput(comp);
      Assert.Equal("Name", output.Value.ApiProperty.Name);
      Assert.Equal(0.1, output.Value.ApiProperty.DampingRatio);
      Assert.Equal(1.2, ((TorsionalSpringProperty)output.Value.ApiProperty).Stiffness, 8);
    }

    [Fact]
    public void ParseSpringMatrixErrorTest() {
      GH_OasysDropDownComponent comp = MatrixComponentMother();
      ComponentTestHelper.SetInput(comp, "matrix", 1);

      comp.Params.Output[0].ExpireSolution(true);
      comp.Params.Output[0].CollectData();
      Assert.Equal(2, comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Error).Count);
    }

    [Fact]
    public void ParseFrictionCoefficientErrorTest() {
      GH_OasysDropDownComponent comp = FrictionComponentMother();
      ComponentTestHelper.SetInput(comp, "friction", 4);

      comp.Params.Output[0].ExpireSolution(true);
      comp.Params.Output[0].CollectData();
      Assert.Equal(2, comp.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Error).Count);
    }
  }
}
