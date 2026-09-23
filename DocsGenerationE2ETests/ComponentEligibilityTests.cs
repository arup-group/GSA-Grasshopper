using System;

using DocsGeneration.Data;

using Grasshopper.Kernel;

using Xunit;

namespace DocsGenerationE2ETests {
  public class ComponentEligibilityTests {
    [Fact]
    public void IsDocumentableComponent_ReturnsTrue_ForConcreteGrasshopperComponents() {
      Assert.True(Component.IsDocumentableComponent(typeof(DocumentableComponent)));
    }

    [Fact]
    public void IsDocumentableComponent_ReturnsFalse_ForAbstractGrasshopperComponents() {
      Assert.False(Component.IsDocumentableComponent(typeof(AbstractComponent)));
    }

    [Fact]
    public void IsDocumentableComponent_ReturnsFalse_ForNonComponentTypes() {
      Assert.False(Component.IsDocumentableComponent(typeof(NonComponentType)));
    }

    private class DocumentableComponent : GH_Component {
      public DocumentableComponent() : base("Test", "Test", "Test component", "Test", "Test") {
      }

      public override Guid ComponentGuid => Guid.Empty;

      protected override void RegisterInputParams(GH_InputParamManager pManager) {
      }

      protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      }

      protected override void SolveInstance(IGH_DataAccess da) {
      }
    }

    private abstract class AbstractComponent : GH_Component {
    }

    private class NonComponentType {
    }
  }
}
