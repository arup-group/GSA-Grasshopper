using System;
using System.IO;
using System.Reflection;
using Grasshopper.Kernel;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using Xunit;

namespace IntegrationTests.Components
{
  [Collection("GrasshopperFixture collection")]
  public class LoadsFromReferenceObjectsTests
  {
    public static GH_Document Document()
    {
      Type thisClass = MethodBase.GetCurrentMethod().DeclaringType;
      string fileName = thisClass.Name + ".gh";
      fileName = fileName.Replace(thisClass.Namespace, string.Empty).Replace("Tests", string.Empty);

      string solutiondir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.Parent.FullName;
      string path = Path.Combine(new string[] { solutiondir, "ExampleFiles", "Components"});
      
      return Helper.CreateDocument(Path.Combine(path, fileName));
    }

    [Theory]
    [InlineData("BeamLoadFromElementTest", true)]
    [InlineData("FaceLoadFromElementTest", 0)]
    [InlineData("BeamLoadFromMemberTest", 0)]
    [InlineData("BeamLoadFromSectionTest", "PB7")]
    [InlineData("FaceLoadFromMemberTest", 0)]
    [InlineData("FaceLoadFromProp2dTest", "PA19")]
    [InlineData("GravityLoadFromSectionTest", "PB7")]
    [InlineData("GravityLoadFromProp2dTest", "PA19")]
    [InlineData("GravityLoadFromElem1dTest", 42)]
    [InlineData("GravityLoadFromElem2dTest", 0)]
    [InlineData("GravityLoadFromMember1dTest", 0)]
    [InlineData("GravityLoadFromMember2dTest", 0)]
    [InlineData("GridPlnSrfFromSection", "PB7")]
    [InlineData("GridPlnSrfFromProp2d", "PA19")]
    [InlineData("GridPlnSrfFromElem2d", 0)]
    [InlineData("GridPlnSrfFromMem1d", 0)]
    [InlineData("GridPlnSrfFromMem2d", 0)]
    public void Test(string groupIdentifier, object expected)
    {
      IGH_Param param = Helper.FindParameter(Document(), groupIdentifier);
      Helper.TestGHPrimitives(param, expected);
    }
  }
}
