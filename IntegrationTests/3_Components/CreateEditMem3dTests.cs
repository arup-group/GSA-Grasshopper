using System;
using System.IO;
using System.Reflection;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using GsaGH.Parameters;

using GsaGHTests.Helpers;

using Xunit;

namespace IntegrationTests.Components {
  [Collection("GrasshopperFixture collection")]
  public class CreateEditMem3dTests {
    public static GH_Document Document {
      get {
        if (document == null) {
          document = OpenDocument();
        }

        return document;
      }
    }
    private static GH_Document document = null;

    [Fact]
    public void CreateMember3dComponentTest() {
      GH_Document doc = Document;
      GH_Component comp = Helper.FindComponent(doc, "CreateMember3d");
      Assert.NotNull(comp);
      var output1 = (GsaMember3dGoo)ComponentTestHelper.GetOutput(comp, 0, 0, 0);
      var output2 = (GsaMember3dGoo)ComponentTestHelper.GetOutput(comp, 0, 0, 1);
      var output3 = (GsaMember3dGoo)ComponentTestHelper.GetOutput(comp, 0, 0, 2);

      Assert.Null(output1);
      Assert.NotNull(output2.Value);
      Assert.NotNull(output3.Value);

      Assert.Single(comp.RuntimeMessages(GH_RuntimeMessageLevel.Error));
    }

    [Fact]
    public void EditMember3dComponentTest() {
      GH_Document doc = Document;
      GH_Component comp = Helper.FindComponent(doc, "EditMember3d");
      Assert.NotNull(comp);
      var output1 = (GH_Mesh)ComponentTestHelper.GetOutput(comp, 2, 0, 0);
      var output2 = (GH_Mesh)ComponentTestHelper.GetOutput(comp, 2, 0, 1);
      var output3 = (GH_Mesh)ComponentTestHelper.GetOutput(comp, 2, 0, 2);

      Assert.Null(output1);
      Assert.NotNull(output2.Value);
      Assert.NotNull(output3.Value);

      Assert.Single(comp.RuntimeMessages(GH_RuntimeMessageLevel.Error));
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
  }
}
