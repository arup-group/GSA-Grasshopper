using System.Collections.Generic;

using Grasshopper.Kernel;

using GsaGH.Components;
using GsaGH.Parameters;

using GsaGHTests.Helpers;

using OasysGH.Components;

using Xunit;

using static GsaGHTests.Helpers.Export.AssembleModelTests;

namespace GsaGHTests.Model {
  [Collection("GrasshopperFixture collection")]
  public class CreateModelTest {

    public static GH_OasysDropDownComponent CreateModelFromGeometry(
      List<GsaNodeGoo> node, List<GsaElement1dGoo> elem1d, List<GsaElement2dGoo> elem2d,
      List<GsaMember1dGoo> mem1d, List<GsaMember2dGoo> mem2d, List<GsaMember3dGoo> mem3d,
      ModelUnit unit) {
      var comp = new CreateModel();
      comp.CreateAttributes();
      comp.Params.Input[2].DataMapping = GH_DataMapping.Flatten;
      comp.SetSelected(0, (int)unit);
      if (node != null) {
        foreach (GsaNodeGoo input in node) {
          ComponentTestHelper.SetInput(comp, input, 2);
        }
      }

      if (elem1d != null) {
        foreach (GsaElement1dGoo input in elem1d) {
          ComponentTestHelper.SetInput(comp, input, 2);
        }
      }

      if (elem2d != null) {
        foreach (GsaElement2dGoo input in elem2d) {
          ComponentTestHelper.SetInput(comp, input, 2);
        }
      }

      if (mem1d != null) {
        foreach (GsaMember1dGoo input in mem1d) {
          ComponentTestHelper.SetInput(comp, input, 2);
        }
      }

      if (mem2d != null) {
        foreach (GsaMember2dGoo input in mem2d) {
          ComponentTestHelper.SetInput(comp, input, 2);
        }
      }

      if (mem3d != null) {
        foreach (GsaMember3dGoo input in mem3d) {
          ComponentTestHelper.SetInput(comp, input, 2);
        }
      }

      return comp;
    }

    public static GH_OasysDropDownComponent CreateModelFromLoads(
      List<IGsaLoad> loads, List<GsaGridPlaneSurface> gridPlaneSurfaces) {
      var comp = new CreateModel();
      comp.CreateAttributes();
      comp.Params.Input[3].DataMapping = GH_DataMapping.Flatten;
      if (loads != null) {
        foreach (IGsaLoad input in loads) {
          ComponentTestHelper.SetInput(comp, new GsaLoadGoo(input), 3);
        }
      }

      if (gridPlaneSurfaces != null) {
        foreach (GsaGridPlaneSurface input in gridPlaneSurfaces) {
          ComponentTestHelper.SetInput(comp, input, 3);
        }
      }

      return comp;
    }

    public static GH_OasysDropDownComponent CreateModelFromModels(List<GsaModelGoo> models) {
      var comp = new CreateModel();
      comp.CreateAttributes();
      comp.Params.Input[0].DataMapping = GH_DataMapping.Flatten;
      if (models == null) {
        return comp;
      }

      foreach (GsaModelGoo input in models) {
        ComponentTestHelper.SetInput(comp, input, 0);
      }

      return comp;
    }

    public static GH_OasysDropDownComponent CreateModelFromProperties(List<GsaSectionGoo> sections,
      List<GsaProperty2dGoo> prop2ds, List<GsaProperty3dGoo> prop3ds, List<GsaSpringPropertyGoo> springProps) {
      var comp = new CreateModel();
      comp.CreateAttributes();
      comp.Params.Input[1].DataMapping = GH_DataMapping.Flatten;
      if (sections != null) {
        foreach (GsaSectionGoo input in sections) {
          ComponentTestHelper.SetInput(comp, input, 1);
        }
      }

      if (prop2ds != null) {
        foreach (GsaProperty2dGoo input in prop2ds) {
          ComponentTestHelper.SetInput(comp, input, 1);
        }
      }

      if (prop3ds != null) {
        foreach (GsaProperty3dGoo input in prop3ds) {
          ComponentTestHelper.SetInput(comp, input, 1);
        }
      }

      if (springProps != null) {
        foreach (GsaSpringPropertyGoo input in springProps) {
          ComponentTestHelper.SetInput(comp, input, 1);
        }
      }

      return comp;
    }

    public static GsaModel GetModel(GH_OasysDropDownComponent comp) {
      var modelGoo = (GsaModelGoo)ComponentTestHelper.GetOutput(comp);
      return modelGoo.Value;
    }
  }
}
