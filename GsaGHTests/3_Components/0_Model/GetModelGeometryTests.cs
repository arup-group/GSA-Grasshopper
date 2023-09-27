using Grasshopper.Kernel;
using GsaGH.Components;
using GsaGHTests.Helpers;
using System.IO;
using System;
using Xunit;
using System.Collections.Generic;
using GsaGH.Parameters;
using Rhino.Geometry;
using static GsaGHTests.Helpers.Export.AssembleModelTests;
using Grasshopper.Kernel.Types;
using OasysGH.Parameters;
using Rhino.Display;
using System.Drawing;
using System.Reflection;
using GsaGHTests.Components.Geometry;

namespace GsaGHTests.Model {
  [Collection("GrasshopperFixture collection")]
  public class GetModelGeometryTests {
    [Fact]
    public void GetModelGeometryNodeDrawViewportMeshesAndWiresTest() {
      var node = new GsaNode {
        Restraint = new GsaBool6(true, true, true, false, false, false),
        LocalAxis = new Plane(new Point3d(10, 10, 10), new Vector3d(10, 10, 10))
      };
      node.UpdatePreview();

      var node2 = new GsaNode(node);
      node2.ApiNode.Colour = Color.Blue;
      
      var modelGoo = (GsaModelGoo)ComponentTestHelper.GetOutput(
        CreateModelTest.CreateModelFromGeometry(new List<GsaNodeGoo>() {
          (GsaNodeGoo)ComponentTestHelper.GetOutput(CreateSupportTests.ComponentMother()),
          new GsaNodeGoo(node),
          new GsaNodeGoo(node2),
        }, null, null, null, null, null, ModelUnit.M));

      var comp = new GetModelGeometry();
      ComponentTestHelper.SetInput(comp, modelGoo);
      var output = (GsaNodeGoo)ComponentTestHelper.GetOutput(comp, 0);
      DrawViewportMeshesAndWiresTest(comp);
    }

    [Fact]
    public void GetModelGeometryElement1dDrawViewportMeshesAndWiresTest() {
      var modelGoo = (GsaModelGoo)ComponentTestHelper.GetOutput(
        CreateModelTest.CreateModelFromGeometry(null, new List<GsaElement1dGoo>() {
          (GsaElement1dGoo)ComponentTestHelper.GetOutput(CreateElement1dTests.ComponentMother()),
        }, null, null, null, null, ModelUnit.M));

      var comp = new GetModelGeometry();
      ComponentTestHelper.SetInput(comp, modelGoo);
      var output = (GsaElement1dGoo)ComponentTestHelper.GetOutput(comp, 1);
      DrawViewportMeshesAndWiresTest(comp);
    }

    [Fact]
    public void GetModelGeometryElement2dDrawViewportMeshesAndWiresTest() {
      var modelGoo = (GsaModelGoo)ComponentTestHelper.GetOutput(
        CreateModelTest.CreateModelFromGeometry(null, null, new List<GsaElement2dGoo>() {
          (GsaElement2dGoo)ComponentTestHelper.GetOutput(CreateElement2dTests.ComponentMother()),
        }, null, null, null, ModelUnit.M));

      var comp = new GetModelGeometry();
      ComponentTestHelper.SetInput(comp, modelGoo);
      var output = (GsaElement2dGoo)ComponentTestHelper.GetOutput(comp, 2);
      DrawViewportMeshesAndWiresTest(comp);
    }

    [Fact]
    public void GetModelGeometryMember1dDrawViewportMeshesAndWiresTest() {
      var modelGoo = (GsaModelGoo)ComponentTestHelper.GetOutput(
        CreateModelTest.CreateModelFromGeometry(null, null, null, new List<GsaMember1dGoo>() {
          (GsaMember1dGoo)ComponentTestHelper.GetOutput(CreateMember1dTests.ComponentMother()),
        }, null, null, ModelUnit.M));

      var comp = new GetModelGeometry();
      ComponentTestHelper.SetInput(comp, modelGoo);
      var output = (GsaMember1dGoo)ComponentTestHelper.GetOutput(comp, 4);
      DrawViewportMeshesAndWiresTest(comp);
    }

    [Fact]
    public void GetModelGeometryMember2dDrawViewportMeshesAndWiresTest() {
      var modelGoo = (GsaModelGoo)ComponentTestHelper.GetOutput(
        CreateModelTest.CreateModelFromGeometry(null, null, null, null, new List<GsaMember2dGoo>() {
          (GsaMember2dGoo)ComponentTestHelper.GetOutput(CreateMember2dTests.ComponentMother()),
        }, null, ModelUnit.M));

      var comp = new GetModelGeometry();
      ComponentTestHelper.SetInput(comp, modelGoo);
      var output = (GsaMember2dGoo)ComponentTestHelper.GetOutput(comp, 5);
      DrawViewportMeshesAndWiresTest(comp);
    }

    [Fact]
    public void GetModelGeometryMember3dDrawViewportMeshesAndWiresTest() {
      var modelGoo = (GsaModelGoo)ComponentTestHelper.GetOutput(
        CreateModelTest.CreateModelFromGeometry(null, null, null, null, null, 
        new List<GsaMember3dGoo>() {
          (GsaMember3dGoo)ComponentTestHelper.GetOutput(CreateMember3dTests.ComponentMother()),
        }, ModelUnit.M));

      var comp = new GetModelGeometry();
      ComponentTestHelper.SetInput(comp, modelGoo);
      var output = (GsaMember3dGoo)ComponentTestHelper.GetOutput(comp, 6);
      DrawViewportMeshesAndWiresTest(comp);
    }

    [Fact]
    public void CreateGetGeometryTest() {
      var comp = new GetModelGeometry();
      comp.CreateAttributes();
      ChangeDropDownTest(comp);
    }

    private static void ChangeDropDownTest(
      GetModelGeometry comp, bool ignoreSpacerDescriptionsCount = false) {
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

    private static void DrawViewportMeshesAndWiresTest(GetModelGeometry component) {
      var rhinoViewPort = new RhinoViewport();
      var rhinoDocument = Rhino.RhinoDoc.CreateHeadless(null);
      rhinoDocument.Views.DefaultViewLayout();
      DisplayPipeline displayPipeline = rhinoDocument.Views.ActiveView.DisplayPipeline;
      var grasshopperDocument = new GH_Document();
      Grasshopper.CentralSettings.PreviewMeshEdges = true;
      MeshingParameters mp = grasshopperDocument.PreviewCurrentMeshParameters();
      var notSelectedMaterial = new DisplayMaterial {
        Diffuse = Color.FromArgb(255, 150, 0, 0),
        Emission = Color.FromArgb(50, 190, 190, 190),
        Transparency = 0.1,
      };
      var selectedMaterial = new DisplayMaterial {
        Diffuse = Color.FromArgb(255, 150, 0, 1),
        Emission = Color.FromArgb(50, 190, 190, 190),
        Transparency = 0.1,
      };

      object[] parameters = {
        grasshopperDocument,
        displayPipeline, 
        rhinoViewPort, 
        3,
        Color.FromArgb(255, 150, 0, 0), 
        Color.FromArgb(255, 150, 0, 1),
        notSelectedMaterial,
        selectedMaterial,
        mp
      };

      // create GH_PreviewArgs with reflection as constructor is internal in GH
      BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;
      var args = (GH_PreviewArgs)Activator.CreateInstance(
        typeof(GH_PreviewArgs), flags, null, parameters, null);

      component.Attributes.Selected = false;
      component.DrawViewportMeshes(args);
      component.DrawViewportWires(args);
      component.Attributes.Selected = true;
      component.DrawViewportMeshes(args);
      component.DrawViewportWires(args);

      Assert.True(true);

      rhinoDocument.Dispose();
      grasshopperDocument.Dispose();
    }

    private static void TestDeserialize(GetModelGeometry comp, string customIdentifier = "") {
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
