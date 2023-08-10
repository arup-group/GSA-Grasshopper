﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Xunit;

namespace IntegrationTests.ExampleFiles {
  [Collection("GrasshopperFixture collection")]
  [SuppressMessage("ReSharper", "InconsistentNaming")]
  public class Example6_3dBoxes_Test {

    public static GH_Document Document() {
      Type thisClass = MethodBase.GetCurrentMethod().DeclaringType;
      string fileName = "GsaGH_" + thisClass.Name + ".gh";
      fileName = fileName.Replace(thisClass.Namespace, string.Empty).Replace("_Test", string.Empty);

      string solutiondir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent
       .Parent.FullName;
      string path = Path.Combine(solutiondir, "ExampleFiles");

      return Helper.CreateDocument(Path.Combine(path, fileName));
    }

    [Fact]
    public void MaxDeflectionTest() {
      IGH_Param param1 = Helper.FindParameter(Document(), "TestMaxValue");
      var output1 = (GH_Number)param1.VolatileData.get_Branch(0)[0];

      IGH_Param param2 = Helper.FindParameter(Document(), "TestDeflectedShape");
      var output2 = (GH_Number)param2.VolatileData.get_Branch(0)[0];

      Assert.Equal(output1.Value, output2.Value, 2);
      Assert.NotEqual(0.0, output1.Value);
    }

    [Fact]
    public void Stress3dTest() {
      IGH_Param param = Helper.FindParameter(Document(), "3dStress");
      var output1 = (GH_Number)param.VolatileData.get_Branch(0)[0];
      var output2 = (GH_Number)param.VolatileData.get_Branch(0)[1];
      var output3 = (GH_Number)param.VolatileData.get_Branch(0)[2];
      var output4 = (GH_Number)param.VolatileData.get_Branch(0)[3];
      var output5 = (GH_Number)param.VolatileData.get_Branch(0)[4];
      var output6 = (GH_Number)param.VolatileData.get_Branch(0)[5];
      Assert.Equal(3.03, output1.Value, 2);
      Assert.Equal(3.03, output2.Value, 2);
      Assert.Equal(11.46, output3.Value, 2);
      Assert.Equal(0, output4.Value, 2);
      Assert.Equal(1.29, output5.Value, 2);
      Assert.Equal(4.01, output6.Value, 2);
    }

    [Fact]
    public void NoRuntimeErrorsTest() {
      Helper.TestNoRuntimeMessagesInDocument(Document(), GH_RuntimeMessageLevel.Error);
    }
  }
}
