using System;
using System.IO;
using System.Reflection;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;

using GsaGH.Components;

using Xunit;

namespace IntegrationTests.Components {
  [Collection("GrasshopperFixture collection")]
  public class QuantitiesTests {

    [Theory]
    [InlineData(9.25, 0.0)]
    [InlineData(15.0, 1.2)]
    public void PropertyQuantitiesTest(double lengthInM, double offsetsInM) {
      GH_Document doc = OpenDocument();
      SetParamInput(lengthInM, "Length", doc);
      SetParamInput(offsetsInM, "Offsets", doc);
      var comp = (PropertyQuantities)Helper.FindComponent(doc, "PropertyQuantities");
      double expectedLengthInM = lengthInM - (2 * offsetsInM);
      double expectedAreaInM2 = lengthInM * lengthInM;
      comp.SetSelected(0, 0); // analysis layer
      IGH_Param PBQ = Helper.FindParameter(doc, "PBQ");
      Helper.TestGhPrimitives(PBQ, expectedLengthInM);
      IGH_Param PBA = Helper.FindParameter(doc, "PAQ");
      Helper.TestGhPrimitives(PBA, expectedAreaInM2);
      Helper.TestNoRuntimeMessagesInDocument(doc, GH_RuntimeMessageLevel.Error);
      Helper.TestNoRuntimeMessagesInDocument(doc, GH_RuntimeMessageLevel.Warning);

      comp.SetSelected(0, 1); // design layer
      PBQ = Helper.FindParameter(doc, "PBQ");
      Helper.TestGhPrimitives(PBQ, expectedLengthInM);
      PBA = Helper.FindParameter(doc, "PAQ");
      Helper.TestGhPrimitives(PBA, expectedAreaInM2);
      Helper.TestNoRuntimeMessagesInDocument(doc, GH_RuntimeMessageLevel.Error);
      Helper.TestNoRuntimeMessagesInDocument(doc, GH_RuntimeMessageLevel.Warning);
    }

    [Theory]
    [InlineData(10, 1.0, 100.0, 100.0, 1000, 0, false)]
    [InlineData(9.25, 0.0, 55.0, 25.0, 320, 0, false)]
    [InlineData(7.25, 0.5, 65.0, 35.0, 280, 1, false)]
    [InlineData(4.5, 0.1, 5.0, 3.0, 80, 2, false)]
    [InlineData(6.75, 0.2, 50.0, 30.0, 200, 3, false)]
    [InlineData(12.0, 0.3, 40.0, 30.0, 180, 4, false)]
    [InlineData(15.5, 0.4, 30.0, 30.0, 220, 5, false)]
    [InlineData(8, 0.1, 10.0, 30.0, 70, 0, true)]
    public void MaterialQuantitiesTest(double lengthInM, double offsetsInM, double depthInCm,
      double widthInCm, double thicknessInMm, int standardMatType, bool customMaterial) {
      GH_Document doc = OpenDocument();
      SetParamInput(lengthInM, "Length", doc);
      SetParamInput(offsetsInM, "Offsets", doc);
      SetParamInput(depthInCm, "Depth", doc);
      SetParamInput(widthInCm, "Width", doc);
      SetParamInput(thicknessInMm, "Thickness", doc);
      SetParamInput(customMaterial, "MaterialPicker", doc);
      var stdMat = (CreateMaterial)Helper.FindComponent(doc, "StandardMaterial");
      stdMat.SetSelected(0, standardMatType); // material type

      double expectedLengthInM = lengthInM - (2 * offsetsInM);
      double expectedVolPbInM3 = depthInCm * widthInCm * 0.0001 * expectedLengthInM;
      double expectedAreaInM2 = lengthInM * lengthInM;
      double expectedVolPaInM3 = thicknessInMm * 0.001 * expectedAreaInM2;
      double expectedVolume = expectedVolPbInM3 + expectedVolPaInM3;
      IGH_Param density = Helper.FindParameter(doc, "MatDensity");
      double expectedMass = expectedVolume * ((GH_Number)density.VolatileData.get_Branch(0)[0]).Value;

      var comp = (MaterialQuantities)Helper.FindComponent(doc, "MatQuantities");
      if (customMaterial) {
        standardMatType = 6;
      }

      for (int i = 0; i < 7; i++) {
        if (i != standardMatType) {
          comp.Params.Output[i].CollectData();
          Assert.Empty(comp.Params.Output[i].VolatileData.AllData(false));
        }
      }

      IGH_Param quantity = Helper.FindParameter(doc, "MatQ");
      Helper.TestGhPrimitives(quantity, expectedMass);

      comp.SetSelected(0, 1); // design layer
      for (int i = 0; i < 7; i++) {
        if (i != standardMatType) {
          comp.Params.Output[i].CollectData();
          Assert.Empty(comp.Params.Output[i].VolatileData.AllData(false));
        }
      }

      quantity = Helper.FindParameter(doc, "MatQ");
      Helper.TestGhPrimitives(quantity, expectedMass);
    }

    private static GH_Document OpenDocument() {
      Type thisClass = MethodBase.GetCurrentMethod().DeclaringType;
      string fileName = thisClass.Name + ".gh";
      fileName = fileName.Replace(thisClass.Namespace, string.Empty).Replace("Tests", string.Empty);

      string solutiondir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent
       .Parent.FullName;
      string path = Path.Combine(new string[] {
        solutiondir,
        "ExampleFiles",
        "Components",
      });

      return Helper.CreateDocument(Path.Combine(path, fileName));
    }

    private void SetParamInput(double value, string name, GH_Document doc) {
      IGH_Param param = Helper.FindParameter(doc, name);
      param.VolatileData.ClearData();
      var path = new GH_Path(0);
      param.AddVolatileData(path, 0, new GH_Number(value));
    }

    private void SetParamInput(bool value, string name, GH_Document doc) {
      IGH_Param param = Helper.FindParameter(doc, name);
      param.VolatileData.ClearData();
      var path = new GH_Path(0);
      param.AddVolatileData(path, 0, new GH_Boolean(value));
    }
  }
}
