using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

using Grasshopper.Kernel;

using GsaGH.Components;
using GsaGH.Parameters;

using GsaGHTests.Components.Geometry;
using GsaGHTests.Helpers;

using Rhino.Geometry;

using Xunit;

using static GsaGHTests.Helpers.Export.AssembleModelTests;

namespace GsaGHTests.Model {
  [Collection("GrasshopperFixture collection")]
  public class GetModelGeometryTests_OBSOLETE {
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

      var comp = new GetModelGeometry_OBSOLETE();
      ComponentTestHelper.SetInput(comp, modelGoo);
      var output = (GsaNodeGoo)ComponentTestHelper.GetOutput(comp, 0);
      ComponentTestHelper.DrawViewportMeshesAndWiresTest(comp);
    }

    [Fact]
    public void GetModelGeometryElement1dDrawViewportMeshesAndWiresTest() {
      var modelGoo = (GsaModelGoo)ComponentTestHelper.GetOutput(
        CreateModelTest.CreateModelFromGeometry(null, new List<GsaElement1dGoo>() {
          (GsaElement1dGoo)ComponentTestHelper.GetOutput(CreateElement1dTests.ComponentMother()),
        }, null, null, null, null, ModelUnit.M));

      var comp = new GetModelGeometry_OBSOLETE();
      ComponentTestHelper.SetInput(comp, modelGoo);
      var output = (GsaElement1dGoo)ComponentTestHelper.GetOutput(comp, 1);
      ComponentTestHelper.DrawViewportMeshesAndWiresTest(comp);
    }

    [Fact]
    public void GetModelGeometryElement2dDrawViewportMeshesAndWiresTest() {
      var modelGoo = (GsaModelGoo)ComponentTestHelper.GetOutput(
        CreateModelTest.CreateModelFromGeometry(null, null, new List<GsaElement2dGoo>() {
          (GsaElement2dGoo)ComponentTestHelper.GetOutput(CreateElement2dTests.ComponentMother()),
        }, null, null, null, ModelUnit.M));

      var comp = new GetModelGeometry_OBSOLETE();
      ComponentTestHelper.SetInput(comp, modelGoo);
      var output = (GsaElement2dGoo)ComponentTestHelper.GetOutput(comp, 2);
      ComponentTestHelper.DrawViewportMeshesAndWiresTest(comp);
    }

    [Fact]
    public void GetModelGeometryMember1dDrawViewportMeshesAndWiresTest() {
      var modelGoo = (GsaModelGoo)ComponentTestHelper.GetOutput(
        CreateModelTest.CreateModelFromGeometry(null, null, null, new List<GsaMember1dGoo>() {
          (GsaMember1dGoo)ComponentTestHelper.GetOutput(CreateMember1dTests.ComponentMother()),
        }, null, null, ModelUnit.M));

      var comp = new GetModelGeometry_OBSOLETE();
      ComponentTestHelper.SetInput(comp, modelGoo);
      var output = (GsaMember1dGoo)ComponentTestHelper.GetOutput(comp, 4);
      ComponentTestHelper.DrawViewportMeshesAndWiresTest(comp);
    }

    [Fact]
    public void GetModelGeometryMember2dDrawViewportMeshesAndWiresTest() {
      var modelGoo = (GsaModelGoo)ComponentTestHelper.GetOutput(
        CreateModelTest.CreateModelFromGeometry(null, null, null, null, new List<GsaMember2dGoo>() {
          (GsaMember2dGoo)ComponentTestHelper.GetOutput(CreateMember2dTests.ComponentMother()),
        }, null, ModelUnit.M));

      var comp = new GetModelGeometry_OBSOLETE();
      ComponentTestHelper.SetInput(comp, modelGoo);
      var output = (GsaMember2dGoo)ComponentTestHelper.GetOutput(comp, 5);
      ComponentTestHelper.DrawViewportMeshesAndWiresTest(comp);
    }

    [Fact]
    public void GetModelGeometryMember3dDrawViewportMeshesAndWiresTest() {
      var modelGoo = (GsaModelGoo)ComponentTestHelper.GetOutput(
        CreateModelTest.CreateModelFromGeometry(null, null, null, null, null,
        new List<GsaMember3dGoo>() {
          (GsaMember3dGoo)ComponentTestHelper.GetOutput(CreateMember3dTests.ComponentMother()),
        }, ModelUnit.M));

      var comp = new GetModelGeometry_OBSOLETE();
      ComponentTestHelper.SetInput(comp, modelGoo);
      var output = (GsaMember3dGoo)ComponentTestHelper.GetOutput(comp, 6);
      ComponentTestHelper.DrawViewportMeshesAndWiresTest(comp);
    }

    [Fact]
    public void CreateGetGeometryTest() {
      var comp = new GetModelGeometry_OBSOLETE();
      comp.CreateAttributes();
      ChangeDropDownTest(comp);
    }

    [Fact]
    public void ModeClickedTest() {
      var comp = new GetModelGeometry_OBSOLETE();

      Assert.True(string.IsNullOrEmpty(comp.Message));
      comp.GraftModeClicked(null, null);
      Assert.Equal("Graft by Property", comp.Message);
      comp.ListModeClicked(null, null);
      Assert.Equal("Import as List", comp.Message);
    }

    private static void ChangeDropDownTest(
      GetModelGeometry_OBSOLETE comp, bool ignoreSpacerDescriptionsCount = false) {
      Assert.True(comp._isInitialised);
      if (!ignoreSpacerDescriptionsCount) {
        Assert.Equal(comp._dropDownItems.Count, comp._spacerDescriptions.Count);
      }

      Assert.Equal(comp._dropDownItems.Count, comp._selectedItems.Count);

      for (int i = 0; i < comp._dropDownItems.Count; i++) {
        comp.SetSelected(i, 0);

        for (int j = 0; j < comp._dropDownItems[i].Count; j++) {
          comp.SetSelected(i, j);
          TestDeserialize(comp);
          Assert.Equal(comp._selectedItems[i], comp._dropDownItems[i][j]);
        }
      }
    }

    private static void TestDeserialize(GetModelGeometry_OBSOLETE comp, string customIdentifier = "") {
      comp.CreateAttributes();

      var doc = new GH_Document();
      doc.AddObject(comp, true);

      var serialize = new GH_DocumentIO {
        Document = doc,
      };
      var originalComponent = (GH_Component)serialize.Document.Objects[0];
      originalComponent.Attributes.PerformLayout();
      originalComponent.ExpireSolution(true);
      originalComponent.Params.Output[0].CollectData();

      string path = Path.Combine(Environment.CurrentDirectory, "GH-Test-Files");
      Directory.CreateDirectory(path);
      Type myType = comp.GetType();
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
  }
}
