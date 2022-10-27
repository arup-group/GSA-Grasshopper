﻿using System.IO;
using System.Reflection;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Xunit;

namespace IntegrationTests.Parameters
{
  [Collection("GrasshopperFixture collection")]
  public class CreateEditBucklingFactorsTest
  {
    public static GH_Document Document()
    {
      string fileName = MethodBase.GetCurrentMethod().DeclaringType + ".gh";
      fileName = fileName.Replace("IntegrationTests.Parameters.", string.Empty);
      fileName = fileName.Replace("Test", string.Empty);

      string solutiondir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.Parent.FullName;
      string path = Path.Combine(new string[] { solutiondir, "ExampleFiles", "Parameters", "1_Properties" });
      GH_DocumentIO io = new GH_DocumentIO();
      Assert.True(File.Exists(Path.Combine(path, fileName)));
      Assert.True(io.Open(Path.Combine(path, fileName)));
      io.Document.NewSolution(true);
      return io.Document;
    }

    [Fact]
    public void OutputTest()
    {
      GH_Document doc = CreateEditBucklingFactorsTest.Document();

      IGH_Param y = Helper.FindParameter(doc, "TestLsy");
      Assert.NotNull(y);
      y.CollectData();
      GH_Number outputY = (GH_Number)y.VolatileData.get_Branch(0)[0];
      Assert.Equal(10, outputY.Value);

      IGH_Param z = Helper.FindParameter(doc, "TestLsz");
      Assert.NotNull(z);
      z.CollectData();
      GH_Number outputZ = (GH_Number)z.VolatileData.get_Branch(0)[0];
      Assert.Equal(15, outputZ.Value);

      IGH_Param lt = Helper.FindParameter(doc, "TestLtb");
      Assert.NotNull(lt);
      lt.CollectData();
      GH_Number outputLTB = (GH_Number)lt.VolatileData.get_Branch(0)[0];
      Assert.Equal(20, outputLTB.Value);
    }

    [Fact]
    public void NoRuntimeErrorTest()
    {
      Helper.TestNoRuntimeMessagesInDocument(CreateEditBucklingFactorsTest.Document(), GH_RuntimeMessageLevel.Error);
    }
  }
}
