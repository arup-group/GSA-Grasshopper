﻿using System;
using System.IO;
using System.Reflection;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Xunit;

namespace IntegrationTests.Components {
  [Collection("GrasshopperFixture collection")]
  public class NodeReactionForceResultTests {

    public static GH_Document Document() {
      Type thisClass = MethodBase.GetCurrentMethod()
        .DeclaringType;
      string fileName = thisClass.Name + ".gh";
      fileName = fileName.Replace(thisClass.Namespace, string.Empty)
        .Replace("Tests", string.Empty);

      string solutiondir = Directory.GetParent(Directory.GetCurrentDirectory())
        .Parent.Parent.Parent.Parent.FullName;
      string path = Path.Combine(new string[] {
        solutiondir,
        "ExampleFiles",
        "Components",
      });

      return Helper.CreateDocument(Path.Combine(path, fileName));
    }

    [Fact]
    public void FAssert() {
      IGH_Param param = Helper.FindParameter(Document(), "F");
      var output1 = (GH_Number)param.VolatileData.get_Branch(0)[0];
      var output2 = (GH_Number)param.VolatileData.get_Branch(0)[1];
      var output3 = (GH_Number)param.VolatileData.get_Branch(0)[2];
      var output4 = (GH_Number)param.VolatileData.get_Branch(0)[3];
      Assert.Equal(209.900, output1.Value, 1);
      Assert.Equal(205.700, output2.Value, 1);
      Assert.Equal(209.900, output3.Value, 1);
      Assert.Equal(205.700, output4.Value, 1);
    }

    [Fact]
    public void FxAssert() {
      IGH_Param param = Helper.FindParameter(Document(), "Fx");
      var output1 = (GH_Number)param.VolatileData.get_Branch(0)[0];
      var output2 = (GH_Number)param.VolatileData.get_Branch(0)[1];
      var output3 = (GH_Number)param.VolatileData.get_Branch(0)[2];
      var output4 = (GH_Number)param.VolatileData.get_Branch(0)[3];
      Assert.Equal(-0.0007855, output1.Value, 7);
      Assert.Equal(0.1297, output2.Value, 4);
      Assert.Equal(0.0007855, output3.Value, 7);
      Assert.Equal(-0.1297, output4.Value, 4);
    }

    [Fact]
    public void FyAssert() {
      IGH_Param param = Helper.FindParameter(Document(), "Fy");
      var output1 = (GH_Number)param.VolatileData.get_Branch(0)[0];
      var output2 = (GH_Number)param.VolatileData.get_Branch(0)[1];
      var output3 = (GH_Number)param.VolatileData.get_Branch(0)[2];
      var output4 = (GH_Number)param.VolatileData.get_Branch(0)[3];
      Assert.Equal(-205.200, output1.Value, 1);
      Assert.Equal(205.200, output2.Value, 1);
      Assert.Equal(-205.200, output3.Value, 1);
      Assert.Equal(205.200, output4.Value, 1);
    }

    [Fact]
    public void FzAssert() {
      IGH_Param param = Helper.FindParameter(Document(), "Fz");
      var output1 = (GH_Number)param.VolatileData.get_Branch(0)[0];
      var output2 = (GH_Number)param.VolatileData.get_Branch(0)[1];
      var output3 = (GH_Number)param.VolatileData.get_Branch(0)[2];
      var output4 = (GH_Number)param.VolatileData.get_Branch(0)[3];
      Assert.Equal(44.160, output1.Value, 2);
      Assert.Equal(13.700, output2.Value, 1);
      Assert.Equal(44.160, output3.Value, 2);
      Assert.Equal(13.700, output4.Value, 1);
    }

    [Fact]
    public void MAssert() {
      IGH_Param param = Helper.FindParameter(Document(), "M");
      var output1 = (GH_Number)param.VolatileData.get_Branch(0)[0];
      var output2 = (GH_Number)param.VolatileData.get_Branch(0)[1];
      var output3 = (GH_Number)param.VolatileData.get_Branch(0)[2];
      var output4 = (GH_Number)param.VolatileData.get_Branch(0)[3];
      Assert.Equal(51.740, output1.Value, 2);
      Assert.Equal(42.250, output2.Value, 2);
      Assert.Equal(51.740, output3.Value, 2);
      Assert.Equal(42.250, output4.Value, 2);
    }

    [Fact]
    public void MxxAssert() {
      IGH_Param param = Helper.FindParameter(Document(), "Mxx");
      var output1 = (GH_Number)param.VolatileData.get_Branch(0)[0];
      var output2 = (GH_Number)param.VolatileData.get_Branch(0)[1];
      var output3 = (GH_Number)param.VolatileData.get_Branch(0)[2];
      var output4 = (GH_Number)param.VolatileData.get_Branch(0)[3];
      Assert.Equal(51.740, output1.Value, 2);
      Assert.Equal(42.250, output2.Value, 2);
      Assert.Equal(51.740, output3.Value, 2);
      Assert.Equal(42.250, output4.Value, 2);
    }

    [Fact]
    public void MyyAssert() {
      IGH_Param param = Helper.FindParameter(Document(), "Myy");
      var output1 = (GH_Number)param.VolatileData.get_Branch(0)[0];
      var output2 = (GH_Number)param.VolatileData.get_Branch(0)[1];
      var output3 = (GH_Number)param.VolatileData.get_Branch(0)[2];
      var output4 = (GH_Number)param.VolatileData.get_Branch(0)[3];
      Assert.Equal(-0.1376, output1.Value, 4);
      Assert.Equal(-0.00001342, output2.Value, 8);
      Assert.Equal(0.1376, output3.Value, 4);
      Assert.Equal(0.00001342, output4.Value, 8);
    }

    [Fact]
    public void MzzAssert() {
      IGH_Param param = Helper.FindParameter(Document(), "Mzz");
      var output1 = (GH_Number)param.VolatileData.get_Branch(0)[0];
      var output2 = (GH_Number)param.VolatileData.get_Branch(0)[1];
      var output3 = (GH_Number)param.VolatileData.get_Branch(0)[2];
      var output4 = (GH_Number)param.VolatileData.get_Branch(0)[3];
      Assert.Equal(0.00007976, output1.Value, 8);
      Assert.Equal(-0.4818, output2.Value, 4);
      Assert.Equal(-0.00007976, output3.Value, 8);
      Assert.Equal(0.4818, output4.Value, 4);
    }

    [Fact]
    public void NoRuntimeErrorsTest()
      => Helper.TestNoRuntimeMessagesInDocument(Document(), GH_RuntimeMessageLevel.Error);
  }
}
