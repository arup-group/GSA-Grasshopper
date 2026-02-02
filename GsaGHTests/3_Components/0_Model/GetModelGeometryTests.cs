using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Xml.Linq;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Geometry.SpatialTrees;

using GsaAPI;

using GsaGH.Components;
using GsaGH.Helpers.Import;
using GsaGH.Parameters;

using GsaGHTests.Components.Geometry;
using GsaGHTests.Helpers;

using Rhino.Geometry;

using Xunit;

using static GsaGHTests.Helpers.Export.AssembleModelTests;

namespace GsaGHTests.Model {
  [Collection("GrasshopperFixture collection")]
  public class GetModelGeometryTests {
    [Fact]
    public void GetModelGeometryNodeDrawViewportMeshesAndWiresTest() {
      var nodeWithSupportSymbol = new GsaNode {
        Restraint = new GsaBool6(true, true, true, false, false, false),
        LocalAxis = new Plane(new Point3d(10, 10, 10), new Vector3d(10, 10, 10))
      };
      nodeWithSupportSymbol.UpdatePreview();

      var nodeWithDefaultGsaColor = new GsaNode(nodeWithSupportSymbol);
      nodeWithDefaultGsaColor.ApiNode.Colour = Color.FromArgb(0, 0, 0);

      var NodeWithSupportText = new GsaNode(nodeWithSupportSymbol) {
        Restraint = new GsaBool6(true, false, true, false, true, false)
      };

      var modelGoo = (GsaModelGoo)ComponentTestHelper.GetOutput(
        CreateModelTest.CreateModelFromGeometry(new List<GsaNodeGoo>() {
          (GsaNodeGoo)ComponentTestHelper.GetOutput(CreateSupportTests.ComponentMother()),
          new GsaNodeGoo(nodeWithSupportSymbol),
          new GsaNodeGoo(nodeWithDefaultGsaColor),
          new GsaNodeGoo(NodeWithSupportText),
        }, null, null, null, null, null, ModelUnit.M));

      var component = new GetModelGeometry();
      ComponentTestHelper.SetInput(component, modelGoo);
      var output = (GsaNodeGoo)ComponentTestHelper.GetOutput(component, 0);
      ComponentTestHelper.DrawViewportMeshesAndWiresTest(component);
    }

    [Fact]
    public void GetModelGeometryElement1dDrawViewportMeshesAndWiresTest() {
      var modelGoo = (GsaModelGoo)ComponentTestHelper.GetOutput(
        CreateModelTest.CreateModelFromGeometry(null, new List<GsaElement1dGoo>() {
          (GsaElement1dGoo)ComponentTestHelper.GetOutput(CreateElement1dTests.ComponentMother()),
        }, null, null, null, null, ModelUnit.M));

      var component = new GetModelGeometry();
      ComponentTestHelper.SetInput(component, modelGoo);
      var output = (GsaElement1dGoo)ComponentTestHelper.GetOutput(component, 1);
      ComponentTestHelper.DrawViewportMeshesAndWiresTest(component);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void GetModelGeometryElement2dDrawViewportMeshesAndWiresTest(bool isLoadPanel) {
      var modelGoo = (GsaModelGoo)ComponentTestHelper.GetOutput(
        CreateModelTest.CreateModelFromGeometry(null, null, new List<GsaElement2dGoo>() {
          (GsaElement2dGoo)ComponentTestHelper.GetOutput(CreateElement2dTests.ComponentMother(isLoadPanel)),
        }, null, null, null, ModelUnit.M));

      var component = new GetModelGeometry();
      ComponentTestHelper.SetInput(component, modelGoo);
      var output = (GsaElement2dGoo)ComponentTestHelper.GetOutput(component, 2);
      ComponentTestHelper.DrawViewportMeshesAndWiresTest(component);
    }

    [Fact]
    public void GetModelGeometryMember1dDrawViewportMeshesAndWiresTest() {
      var modelGoo = (GsaModelGoo)ComponentTestHelper.GetOutput(
        CreateModelTest.CreateModelFromGeometry(null, null, null, new List<GsaMember1dGoo>() {
          (GsaMember1dGoo)ComponentTestHelper.GetOutput(CreateMember1dTests.ComponentMother()),
        }, null, null, ModelUnit.M));

      var component = new GetModelGeometry();
      ComponentTestHelper.SetInput(component, modelGoo);
      var output = (GsaMember1dGoo)ComponentTestHelper.GetOutput(component, 4);
      ComponentTestHelper.DrawViewportMeshesAndWiresTest(component);
    }

    [Fact]
    public void GetModelGeometryMember2dDrawViewportMeshesAndWiresTest() {
      var modelGoo = (GsaModelGoo)ComponentTestHelper.GetOutput(
        CreateModelTest.CreateModelFromGeometry(null, null, null, null, new List<GsaMember2dGoo>() {
          (GsaMember2dGoo)ComponentTestHelper.GetOutput(CreateMember2dTests.ComponentMother()),
        }, null, ModelUnit.M));

      var component = new GetModelGeometry();
      ComponentTestHelper.SetInput(component, modelGoo);
      var output = (GsaMember2dGoo)ComponentTestHelper.GetOutput(component, 5);
      ComponentTestHelper.DrawViewportMeshesAndWiresTest(component);
    }

    [Fact]
    public void GetModelGeometryMember3dDrawViewportMeshesAndWiresTest() {
      var modelGoo = (GsaModelGoo)ComponentTestHelper.GetOutput(
        CreateModelTest.CreateModelFromGeometry(null, null, null, null, null,
        new List<GsaMember3dGoo>() {
          (GsaMember3dGoo)ComponentTestHelper.GetOutput(CreateMember3dTests.ComponentMother()),
        }, ModelUnit.M));

      var component = new GetModelGeometry();
      ComponentTestHelper.SetInput(component, modelGoo);
      var output = (GsaMember3dGoo)ComponentTestHelper.GetOutput(component, 6);
      ComponentTestHelper.DrawViewportMeshesAndWiresTest(component);
    }

    [Fact]
    public void CreateGetGeometryTest() {
      var component = new GetModelGeometry();
      component.CreateAttributes();
      ChangeDropDownTest(component);
    }

    [Fact]
    public void ModeClickedTest() {
      var component = new GetModelGeometry();

      Assert.True(string.IsNullOrEmpty(component.Message));
      component.GraftModeClicked(null, null);
      Assert.Equal("Graft by Property", component.Message);
      component.ListModeClicked(null, null);
      Assert.Equal("Import as List", component.Message);
    }

    private static void ChangeDropDownTest(
      GetModelGeometry component, bool ignoreSpacerDescriptionsCount = false) {
      Assert.True(component._isInitialised);
      if (!ignoreSpacerDescriptionsCount) {
        Assert.Equal(component._dropDownItems.Count, component._spacerDescriptions.Count);
      }

      Assert.Equal(component._dropDownItems.Count, component._selectedItems.Count);

      for (int i = 0; i < component._dropDownItems.Count; i++) {
        component.SetSelected(i, 0);

        for (int j = 0; j < component._dropDownItems[i].Count; j++) {
          component.SetSelected(i, j);
          TestDeserialize(component);
          Assert.Equal(component._selectedItems[i], component._dropDownItems[i][j]);
        }
      }
    }

    private static void TestDeserialize(GetModelGeometry component, string customIdentifier = "") {
      component.CreateAttributes();

      var doc = new GH_Document();
      doc.AddObject(component, true);

      var serialize = new GH_DocumentIO {
        Document = doc,
      };
      var originalComponent = (GH_Component)serialize.Document.Objects[0];
      originalComponent.Attributes.PerformLayout();
      originalComponent.ExpireSolution(true);
      originalComponent.Params.Output[0].CollectData();

      string path = Path.Combine(Environment.CurrentDirectory, "GH-Test-Files");
      Directory.CreateDirectory(path);
      Type myType = component.GetType();
      string pathFileName = Path.Combine(path, myType.Name) + customIdentifier + ".gh";
      Assert.True(serialize.SaveQuiet(pathFileName));

      var deserialize = new GH_DocumentIO();
      Assert.True(deserialize.Open(pathFileName));

      var deserializedComponent = (GH_Component)deserialize.Document.Objects[0];
      deserializedComponent.Attributes.PerformLayout();
      deserializedComponent.ExpireSolution(true);
      deserializedComponent.Params.Output[0].CollectData();

      Duplicates.AreEqual(originalComponent, deserializedComponent, new List<string>() { "Guid" });
    }

    [Fact]
    public void NoExceptionWhenAssignedModelIsNull() {
      var component = new GetModelGeometry();
      var parameter = new GsaModelParameter();
      parameter.AddVolatileData(new Grasshopper.Kernel.Data.GH_Path(0), 0, null);
      component.Params.Input[0].AddSource(parameter);
      ComponentTestHelper.ComputeOutput(component);
      Assert.Empty(component.RuntimeMessages(Grasshopper.Kernel.GH_RuntimeMessageLevel.Error));
    }

    [Fact]
    public void GetModelGeometryCanParseSpringElement() {
      var gsaModel = new GsaModel(ModeElement1D(out Element element));
      var elem1dDict = new ConcurrentDictionary<int, GSAElement>();
      elem1dDict.TryAdd(1, new GSAElement(element));
      ConcurrentBag<GsaElement1dGoo> elements = GsaElementFactory.CreateElement1dFromApi(elem1dDict, gsaModel);
      Assert.Single(elements);
      Assert.Equal(ElementType.SPRING, elements.First().Value.ApiElement.Type);
    }

    private static GsaAPI.Model ModeElement1D(out Element element) {
      var model = new GsaAPI.Model();
      var node1 = new Node();
      int node1Id = model.AddNode(node1);

      var node2 = new Node();
      node2.Position.X = 1;
      int node2Id = model.AddNode(node2);

      var prop = new AxialSpringProperty {
        Stiffness = 100
      };
      model.AddSpringProperty(prop);

      element = new Element() {
        Topology = new ReadOnlyCollection<int>(
            new int[] { node1Id, node2Id }
        ),
        Property = 1,
        Type = ElementType.SPRING
      };
      model.AddElement(element);
      return model;
    }
  }
}
