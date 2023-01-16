﻿//using System;
//using System.IO;
//using System.Reflection;
//using Grasshopper.Kernel;
//using GsaGH.Parameters;
//using GsaGHTests.Helpers;
//using Xunit;

//namespace IntegrationTests.Components
//{
//  [Collection("GrasshopperFixture collection")]
//  public class AssembleWithIDsTestIdSequenceTests
//  {
//    public static GH_Document Document
//    {
//      get
//      {
//        if (_document == null)
//          _document = OpenDocument();
//        return _document;
//      }
//    }
//    private static GH_Document _document = null;
//    private static GH_Document OpenDocument()
//    {
//      Type thisClass = MethodBase.GetCurrentMethod().DeclaringType;
//      string fileName = thisClass.Name + ".gh";
//      fileName = fileName.Replace(thisClass.Namespace, string.Empty).Replace("Tests", string.Empty);

//      string solutiondir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.Parent.FullName;
//      string path = Path.Combine(new string[] { solutiondir, "ExampleFiles", "Components" });

//      return Helper.CreateDocument(Path.Combine(path, fileName));
//    }

//    [Theory]
//    [InlineData("NPoints", 401)]
//    [InlineData("FixedId", 5)]
//    [InlineData("NodeSame", true)]
//    [InlineData("Elem1dIds", new int[] { 1, 2, 3, 14 })]
//    [InlineData("Elem1dSame", true)]
//    [InlineData("Mem1dIds", new int[] { 1, 2, 3, 14 })]
//    [InlineData("Mem1dSame", true)]
//    [InlineData("Mem2dIds", new int[] { 1, 2, 3 })]
//    [InlineData("Mem2dSame", true)]
//    public void Test(string groupIdentifier, object expected)
//    {
//      IGH_Param param = Helper.FindParameter(Document, groupIdentifier);
//      Helper.TestGHPrimitives(param, expected);
//    }

//    [Fact]
//    public void NoRuntimeErrorTest()
//    {
//      Helper.TestNoRuntimeMessagesInDocument(Document, GH_RuntimeMessageLevel.Error);
//      Helper.TestNoRuntimeMessagesInDocument(Document, GH_RuntimeMessageLevel.Warning);
//    }
//  }
//}

