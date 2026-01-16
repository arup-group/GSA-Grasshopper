using System.Collections;


using Grasshopper.Kernel.Types;


using GsaGH.Components;

using GsaGH.Parameters;


using GsaGHTests.Helpers;


using OasysGH.Components;


using OasysUnits;

using OasysUnits.Units;


using GsaGH.Helpers;

using Xunit;
namespace GsaGHTests.Components.Geometry {
  [Collection("GrasshopperFixture collection")]
  public class GetAssemblyTests {
    [Fact]
    public void UpdateLengthTest() {
      var comp = new GetAssembly();
      comp.UpdateLength("mm");
      Assert.Equal("mm", comp.Message);
    }

    public static GH_OasysComponent GetByExplicitPositionsComponentMother() {
      var comp = new GetAssembly();
      comp.CreateAttributes();

      var output = (GsaAssemblyGoo)ComponentTestHelper.GetOutput(CreateAssemblyTests.ByExplicitPositionsComponentMother2());
      ComponentTestHelper.SetInput(comp, output);

      return comp;
    }

    public static GH_OasysComponent GetByNumberOfPointsComponentMother() {
      var comp = new GetAssembly();
      comp.CreateAttributes();

      var output = (GsaAssemblyGoo)ComponentTestHelper.GetOutput(CreateAssemblyTests.ByNumberOfPointsComponentMother2());
      ComponentTestHelper.SetInput(comp, output);

      return comp;
    }

    public static GH_OasysComponent GetBySpacingOfPointsComponentMother() {
      var comp = new GetAssembly();
      comp.CreateAttributes();

      var output = (GsaAssemblyGoo)ComponentTestHelper.GetOutput(CreateAssemblyTests.BySpacingOfPointsComponentMother2());
      ComponentTestHelper.SetInput(comp, output);

      return comp;
    }

    public static GH_OasysComponent GetByStoreyComponentMother() {
      var comp = new GetAssembly();
      comp.CreateAttributes();

      var output = (GsaAssemblyGoo)ComponentTestHelper.GetOutput(CreateAssemblyTests.ByStoreyComponentMother2());
      ComponentTestHelper.SetInput(comp, output);

      return comp;
    }

    [Fact]
    public void GetByExplicitPositionsComponent2() {
      GH_OasysComponent comp = GetByExplicitPositionsComponentMother();

      var name = (GH_String)ComponentTestHelper.GetOutput(comp, 0);
      var type = (GH_String)ComponentTestHelper.GetOutput(comp, 1);
      var assemblyEntities = (GsaListGoo)ComponentTestHelper.GetOutput(comp, 2);
      var topology1 = (GH_Integer)ComponentTestHelper.GetOutput(comp, 3);
      var topology2 = (GH_Integer)ComponentTestHelper.GetOutput(comp, 4);
      var orientationNode = (GH_Integer)ComponentTestHelper.GetOutput(comp, 5);
      var extentsY = (GH_ObjectWrapper)ComponentTestHelper.GetOutput(comp, 6);
      var extentsZ = (GH_ObjectWrapper)ComponentTestHelper.GetOutput(comp, 7);
      IList internalTopology = ComponentTestHelper.GetListOutput(comp, 8);
      var curveFit = (GH_Integer)ComponentTestHelper.GetOutput(comp, 9);
      IList positions = ComponentTestHelper.GetListOutput(comp, 10
        );

      Assert.Equal("Name", name.Value);
      Assert.Equal("By explicit positions", type.Value);
      Assert.Equal(EntityType.Member, assemblyEntities.Value.EntityType);
      Assert.Equal("1", assemblyEntities.Value.Definition);
      Assert.Equal(1, topology1.Value);
      Assert.Equal(2, topology2.Value);
      Assert.Equal(3, orientationNode.Value);
      Assert.Equal(1.0, ((Length)extentsY.Value).As(LengthUnit.Meter));
      Assert.Equal(-1.0, ((Length)extentsZ.Value).As(LengthUnit.Meter));
      Assert.Equal(3, internalTopology.Count);
      Assert.Equal(4, ((GH_Integer)internalTopology[0]).Value);
      Assert.Equal(5, ((GH_Integer)internalTopology[1]).Value);
      Assert.Equal(6, ((GH_Integer)internalTopology[2]).Value);
      Assert.Equal(1, curveFit.Value);
      Assert.Equal(2, positions.Count);
      Assert.Equal(7.7, ((GH_Number)positions[0]).Value, DoubleComparer.Default);
      Assert.Equal(8.8, ((GH_Number)positions[1]).Value, DoubleComparer.Default);
    }

    [Fact]
    public void GetByNumberOfPointsComponent2() {
      GH_OasysComponent comp = GetByNumberOfPointsComponentMother();

      var name = (GH_String)ComponentTestHelper.GetOutput(comp, 0);
      var type = (GH_String)ComponentTestHelper.GetOutput(comp, 1);
      var assemblyEntities = (GsaListGoo)ComponentTestHelper.GetOutput(comp, 2);
      var topology1 = (GH_Integer)ComponentTestHelper.GetOutput(comp, 3);
      var topology2 = (GH_Integer)ComponentTestHelper.GetOutput(comp, 4);
      var orientationNode = (GH_Integer)ComponentTestHelper.GetOutput(comp, 5);
      var extentsY = (GH_ObjectWrapper)ComponentTestHelper.GetOutput(comp, 6);
      var extentsZ = (GH_ObjectWrapper)ComponentTestHelper.GetOutput(comp, 7);
      IList internalTopology = ComponentTestHelper.GetListOutput(comp, 8);
      var curveFit = (GH_Integer)ComponentTestHelper.GetOutput(comp, 9);
      var number = (GH_Integer)ComponentTestHelper.GetOutput(comp, 10);

      Assert.Equal("Name", name.Value);
      Assert.Equal("By number of points", type.Value);
      Assert.Equal(EntityType.Member, assemblyEntities.Value.EntityType);
      Assert.Equal("1", assemblyEntities.Value.Definition);
      Assert.Equal(1, topology1.Value);
      Assert.Equal(2, topology2.Value);
      Assert.Equal(3, orientationNode.Value);
      Assert.Equal(1.0, ((Length)extentsY.Value).As(LengthUnit.Meter));
      Assert.Equal(-1.0, ((Length)extentsZ.Value).As(LengthUnit.Meter));
      Assert.Equal(3, internalTopology.Count);
      Assert.Equal(4, ((GH_Integer)internalTopology[0]).Value);
      Assert.Equal(5, ((GH_Integer)internalTopology[1]).Value);
      Assert.Equal(6, ((GH_Integer)internalTopology[2]).Value);
      Assert.Equal(1, curveFit.Value);
      Assert.Equal(7, number.Value);
    }

    [Fact]
    public void GetBySpacingOfPointsComponent2() {
      GH_OasysComponent comp = GetBySpacingOfPointsComponentMother();

      var name = (GH_String)ComponentTestHelper.GetOutput(comp, 0);
      var type = (GH_String)ComponentTestHelper.GetOutput(comp, 1);
      var assemblyEntities = (GsaListGoo)ComponentTestHelper.GetOutput(comp, 2);
      var topology1 = (GH_Integer)ComponentTestHelper.GetOutput(comp, 3);
      var topology2 = (GH_Integer)ComponentTestHelper.GetOutput(comp, 4);
      var orientationNode = (GH_Integer)ComponentTestHelper.GetOutput(comp, 5);
      var extentsY = (GH_ObjectWrapper)ComponentTestHelper.GetOutput(comp, 6);
      var extentsZ = (GH_ObjectWrapper)ComponentTestHelper.GetOutput(comp, 7);
      IList internalTopology = ComponentTestHelper.GetListOutput(comp, 8);
      var curveFit = (GH_Integer)ComponentTestHelper.GetOutput(comp, 9);
      var spacing = (GH_ObjectWrapper)ComponentTestHelper.GetOutput(comp, 10);

      Assert.Equal("Name", name.Value);
      Assert.Equal("By spacing of points", type.Value);
      Assert.Equal(EntityType.Member, assemblyEntities.Value.EntityType);
      Assert.Equal("1", assemblyEntities.Value.Definition);
      Assert.Equal(1, topology1.Value);
      Assert.Equal(2, topology2.Value);
      Assert.Equal(3, orientationNode.Value);
      Assert.Equal(1.0, ((Length)extentsY.Value).As(LengthUnit.Meter));
      Assert.Equal(-1.0, ((Length)extentsZ.Value).As(LengthUnit.Meter));
      Assert.Equal(3, internalTopology.Count);
      Assert.Equal(4, ((GH_Integer)internalTopology[0]).Value);
      Assert.Equal(5, ((GH_Integer)internalTopology[1]).Value);
      Assert.Equal(6, ((GH_Integer)internalTopology[2]).Value);
      Assert.Equal(1, curveFit.Value);
      Assert.Equal(7.7, ((Length)spacing.Value).As(LengthUnit.Meter));
    }

    [Fact]
    public void GetByStoreyComponent2() {
      GH_OasysComponent comp = GetByStoreyComponentMother();

      var name = (GH_String)ComponentTestHelper.GetOutput(comp, 0);
      var type = (GH_String)ComponentTestHelper.GetOutput(comp, 1);
      var assemblyEntities = (GsaListGoo)ComponentTestHelper.GetOutput(comp, 2);
      var topology1 = (GH_Integer)ComponentTestHelper.GetOutput(comp, 3);
      var topology2 = (GH_Integer)ComponentTestHelper.GetOutput(comp, 4);
      var orientationNode = (GH_Integer)ComponentTestHelper.GetOutput(comp, 5);
      var extentsY = (GH_ObjectWrapper)ComponentTestHelper.GetOutput(comp, 6);
      var extentsZ = (GH_ObjectWrapper)ComponentTestHelper.GetOutput(comp, 7);
      IList internalTopology = ComponentTestHelper.GetListOutput(comp, 8);
      IList curveFit = ComponentTestHelper.GetListOutput(comp, 9);
      var storeyList = (GH_String)ComponentTestHelper.GetOutput(comp, 10);

      Assert.Equal("Name", name.Value);
      Assert.Equal("By storey", type.Value);
      Assert.Equal(EntityType.Member, assemblyEntities.Value.EntityType);
      Assert.Equal("1", assemblyEntities.Value.Definition);
      Assert.Equal(1, topology1.Value);
      Assert.Equal(2, topology2.Value);
      Assert.Equal(3, orientationNode.Value);
      Assert.Equal(1.0, ((Length)extentsY.Value).As(LengthUnit.Meter));
      Assert.Equal(-1.0, ((Length)extentsZ.Value).As(LengthUnit.Meter));
      Assert.Empty(internalTopology);
      Assert.Empty(curveFit);
      Assert.Equal("7", storeyList.Value);
    }
  }
}
