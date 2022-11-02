using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Parameters;
using Rhino.UI;
using Xunit;

namespace IntegrationTests.Parameters
{
  [Collection("GrasshopperFixture collection")]
  public class GetGeometry_TrGen_10_Test
  {
    public static GH_Document Document()
    {
      string fileName = MethodBase.GetCurrentMethod().DeclaringType + ".gh";
      fileName = fileName.Replace("IntegrationTests.Parameters.", string.Empty);
      fileName = fileName.Replace("_Test", string.Empty);

      string solutiondir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.Parent.FullName;
      string path = Path.Combine(new string[] { solutiondir, "ExampleFiles", "Parameters", "2_Geometry" });

      return Helper.CreateDocument(Path.Combine(path, fileName));
    }

    [Theory]
    [InlineData("NodeCount", 3027)]
    [InlineData("Elem1dCount", 98)]
    [InlineData("Elem1dIDs", false, true)]
    [InlineData("Elem1dType", "Beam")]
    [InlineData("Elem1dGrp", 4)]
    [InlineData("RotationAngle", 30.0)]
    [InlineData("Elem2dCount", 3)]
    [InlineData("Elem2dsCount", 574, 288, 87)]
    [InlineData("Elem2dType", "QUAD8")]
    [InlineData("Elem2dTopo", 87)]
    [InlineData("Mem1dCount", 94)]
    [InlineData("Mem1dGrp", 3)]
    [InlineData("Mem1dType", "Generic 1D")]
    [InlineData("Mem1dElemType", "Beam")]
    [InlineData("Mem1dTopo", "1 13")]
    [InlineData("Mem2dCount", 3)]
    [InlineData("Mem2dType", "Generic 2D")]
    [InlineData("Mem2dAnalysisType", "Linear")]
    [InlineData("Mem2dTopo", "13 16 19 22 23 24 21 18 15 14 V(59 60 61 62) L(19 59) L(22 60)")]
    public void Test(string groupIdentifier, object expected, object expected2 = null, object expected3 = null, object expected4 = null, object expected5 = null)
    {
      IGH_Param param = Helper.FindParameter(Document(), groupIdentifier);
      Assert.NotNull(param);
      param.CollectData();

      if (expected2 == null)
      {
        if (expected.GetType() == typeof(string))
        {
          GH_String valOut = (GH_String)param.VolatileData.get_Branch(0)[0];
          Assert.Equal(expected, valOut.Value);
        }
        else if (expected.GetType() == typeof(int))
        {
          GH_Integer valOut = (GH_Integer)param.VolatileData.get_Branch(0)[0];
          Assert.Equal(expected, valOut.Value);
        }
        else if (expected.GetType() == typeof(double))
        {
          GH_Number valOut = (GH_Number)param.VolatileData.get_Branch(0)[0];
          Assert.Equal((double)expected, valOut.Value, 6);
        }
        else if (expected.GetType() == typeof(bool))
        {
          GH_Boolean valOut = (GH_Boolean)param.VolatileData.get_Branch(0)[0];
          Assert.Equal(expected, valOut.Value);
        }
      }
      else if (expected.GetType() == typeof(string))
      {
        List<string> expectedList = new List<string>();
        expectedList.Add((string)expected);
        expectedList.Add((string)expected2);
        if (expected3 != null)
          expectedList.Add((string)expected3);
        if (expected4 != null)
          expectedList.Add((string)expected4);
        if (expected5 != null)
          expectedList.Add((string)expected5);

        for (int i = 0; i < expectedList.Count; i++)
        {
          GH_String valOut = (GH_String)param.VolatileData.get_Branch(0)[i];
          Assert.Equal(expectedList[i], valOut.Value);
        }
      }
      else if (expected.GetType() == typeof(int))
      {
        List<int> expectedList = new List<int>();
        expectedList.Add((int)expected);
        expectedList.Add((int)expected2);
        if (expected3 != null)
          expectedList.Add((int)expected3);
        if (expected4 != null)
          expectedList.Add((int)expected4);
        if (expected5 != null)
          expectedList.Add((int)expected5);

        for (int i = 0; i < expectedList.Count; i++)
        {
          GH_Integer valOut = (GH_Integer)param.VolatileData.get_Branch(0)[i];
          Assert.Equal(expectedList[i], valOut.Value);
        }
      }
      else if (expected.GetType() == typeof(double))
      {
        List<double> expectedList = new List<double>();
        expectedList.Add((double)expected);
        expectedList.Add((double)expected2);
        if (expected3 != null)
          expectedList.Add((double)expected3);
        if (expected4 != null)
          expectedList.Add((double)expected4);
        if (expected5 != null)
          expectedList.Add((double)expected5);

        for (int i = 0; i < expectedList.Count; i++)
        {
          GH_Number valOut = (GH_Number)param.VolatileData.get_Branch(0)[i];
          Assert.Equal((double)expectedList[i], valOut.Value, 6);
        }
      }
      else if (expected.GetType() == typeof(bool))
      {
        List<bool> expectedList = new List<bool>();
        expectedList.Add((bool)expected);
        expectedList.Add((bool)expected2);
        if (expected3 != null)
          expectedList.Add((bool)expected3);
        if (expected4 != null)
          expectedList.Add((bool)expected4);
        if (expected5 != null)
          expectedList.Add((bool)expected5);

        for (int i = 0; i < expectedList.Count; i++)
        {
          GH_Boolean valOut = (GH_Boolean)param.VolatileData.get_Branch(0)[i];
          Assert.Equal(expectedList[i], valOut.Value);
        }
      }
      else
        Assert.True(false, "Expected type not found!");
    }

    [Fact]
    public void NoRuntimeErrorTest()
    {
      Helper.TestNoRuntimeMessagesInDocument(Document(), GH_RuntimeMessageLevel.Error);
    }
  }
}
