using GsaGH.Components;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using Grasshopper.Kernel.Types;
using Xunit;
using OasysGH.Components;
using GsaGHTests.Components.Properties;
using OasysUnits;
using OasysGH.Parameters;
using Rhino.Geometry;

namespace GsaGHTests.Helpers.Export
{
  public partial class AssembleModelTests
  {
    public enum ModelUnit
    {
      mm, 
      cm,
      m,
      inch,
      ft
    }

    public static GsaMaterialGoo CustomMaterialSteel()
    {
      var comp = new CreateCustomMaterial();
      comp.CreateAttributes();
      comp.SetSelected(0, 1); // set material type to "Steel"
      comp.SetSelected(1, 3); // set stress unit to "GPa"
      comp.SetSelected(2, 5); // set density unit to "kg/m^3"
      comp.SetSelected(3, 1); // set temperature unit to "K"
      ComponentTestHelper.SetInput(comp, 1, 0); // ID = 1
      ComponentTestHelper.SetInput(comp, 205, 1); // E = 205 GPa
      ComponentTestHelper.SetInput(comp, 0.5, 2); // v = 0.5
      ComponentTestHelper.SetInput(comp, 7850, 3); // p = 7850 kg/m^3
      return (GsaMaterialGoo)ComponentTestHelper.GetOutput(comp);
    }

    public static GsaMaterialGoo CustomMaterialConcrete()
    {
      var comp = new CreateCustomMaterial();
      comp.CreateAttributes();
      comp.SetSelected(0, 2); // set material type to "Concrete"
      comp.SetSelected(1, 3); // set stress unit to "GPa"
      comp.SetSelected(2, 5); // set density unit to "kg/m^3"
      comp.SetSelected(3, 1); // set temperature unit to "K"
      ComponentTestHelper.SetInput(comp, 2, 0); // ID = 2
      ComponentTestHelper.SetInput(comp, 30, 1); // E = 30 GPa
      ComponentTestHelper.SetInput(comp, 0.3, 2); // v = 0.3
      ComponentTestHelper.SetInput(comp, 2500, 3); // p = 2500 kg/m^3
      return (GsaMaterialGoo)ComponentTestHelper.GetOutput(comp);
    }

    public static GsaSectionGoo Section(string profile, bool useConcrete)
    {
      var comp = new CreateSection();
      comp.CreateAttributes();
      GsaMaterialGoo material = useConcrete? CustomMaterialConcrete() : CustomMaterialSteel();
      ComponentTestHelper.SetInput(comp, profile, 0);
      ComponentTestHelper.SetInput(comp, material, 1);
      return (GsaSectionGoo)ComponentTestHelper.GetOutput(comp);
    }

    public static GsaProp2dGoo Prop2d(Length thickness, bool useConcrete)
    {
      var comp = new CreateProp2d();
      comp.CreateAttributes();
      GsaMaterialGoo material = useConcrete ? CustomMaterialConcrete() : CustomMaterialSteel();
      ComponentTestHelper.SetInput(comp, new GH_UnitNumber(thickness), 0);
      ComponentTestHelper.SetInput(comp, material, 1);
      return (GsaProp2dGoo)ComponentTestHelper.GetOutput(comp);
    }

    public static GsaProp3dGoo Prop3d(bool useConcrete)
    {
      var comp = new CreateProp3d();
      comp.CreateAttributes();
      GsaMaterialGoo material = useConcrete ? CustomMaterialConcrete() : CustomMaterialSteel();
      ComponentTestHelper.SetInput(comp, material, 0);
      return (GsaProp3dGoo)ComponentTestHelper.GetOutput(comp);
    }

    public static GsaElement1dGoo Element1d(GH_Line line, GsaSectionGoo section)
    {
      var comp = new CreateElement1d();
      comp.CreateAttributes();
      ComponentTestHelper.SetInput(comp, line, 0);
      ComponentTestHelper.SetInput(comp, section, 1);
      return (GsaElement1dGoo)ComponentTestHelper.GetOutput(comp);
    }

    public static GsaMember1dGoo Member1d(GH_Curve crv, GsaSectionGoo section)
    {
      var comp = new CreateMember1d();
      comp.CreateAttributes();
      ComponentTestHelper.SetInput(comp, crv, 0);
      ComponentTestHelper.SetInput(comp, section, 1);
      return (GsaMember1dGoo)ComponentTestHelper.GetOutput(comp);
    }

    public static GsaElement2dGoo Element2d(GH_Mesh mesh, GsaProp2dGoo prop)
    {
      var comp = new CreateElement2d();
      comp.CreateAttributes();
      ComponentTestHelper.SetInput(comp, mesh, 0);
      ComponentTestHelper.SetInput(comp, prop, 1);
      return (GsaElement2dGoo)ComponentTestHelper.GetOutput(comp);
    }

    public static GsaMember2dGoo Member2d(GH_Brep brep, GsaProp2dGoo prop)
    {
      var comp = new CreateMember2d();
      comp.CreateAttributes();
      ComponentTestHelper.SetInput(comp, brep, 0);
      ComponentTestHelper.SetInput(comp, prop, 3);
      return (GsaMember2dGoo)ComponentTestHelper.GetOutput(comp);
    }

    public static GsaMember3dGoo Member3d(GH_Box box, GsaProp3dGoo prop)
    {
      var comp = new CreateMember3d();
      comp.CreateAttributes();
      ComponentTestHelper.SetInput(comp, box, 0);
      ComponentTestHelper.SetInput(comp, prop, 1);
      return (GsaMember3dGoo)ComponentTestHelper.GetOutput(comp);
    }
  }
}
