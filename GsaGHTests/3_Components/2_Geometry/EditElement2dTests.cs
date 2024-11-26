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
using System.Collections.ObjectModel;
using GsaGH.Helpers.Import;
using System.Linq;
using Grasshopper;
using System.Collections.Generic;
using Grasshopper.Kernel;

namespace GsaGHTests.Components.Geometry {
  [Collection("GrasshopperFixture collection")]
  public class EditElement2dTests {

    public static GsaAPI.Model GsaModelWithLoadPanelElement() {
      var model = new GsaAPI.Model();
      for (int i = 0; i < 2; i++) {
        var node1 = new Node();
        node1.Position.X = i;
        int node1Id = model.AddNode(node1);

        var node2 = new Node();
        node2.Position.X = 1 + i;
        int node2Id = model.AddNode(node2);

        var node3 = new Node();
        node3.Position.X = 1 + i;
        node3.Position.Y = 1;
        int node3Id = model.AddNode(node3);

        var node4 = new Node();
        node3.Position.X = i;
        node4.Position.Y = 1;
        int node4Id = model.AddNode(node4);

        var element = new LoadPanelElement() {
          Topology = new ReadOnlyCollection<int>(
            new int[] { node1Id, node2Id, node3Id, node4Id }
        )
        };
        int elementId = 3 + i;
        model.SetLoadPanelElement(elementId, element);
      }
      return model;
    }

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

    [Fact]
    public void GettingCorrectTreeOutputFor2dElement() {
      var elements = new Elements(new GsaModel(GsaModelWithLoadPanelElement()), "all");
      GH_OasysComponent component = ComponentMother();
      ComponentTestHelper.SetListInput(component, elements.Element2ds.Cast<object>().ToList());
      IList<System.Collections.IList> topologyTree = ComponentTestHelper.GetBranchOutput(component, 12);
      Assert.Equal(2, topologyTree.Count);
      int nodeId = 0;
      foreach (System.Collections.IList tree in topologyTree) {
        foreach (GH_Integer subTree in tree) {
          Assert.Equal(++nodeId, subTree.Value);
        }
      }
    }
  }
}
