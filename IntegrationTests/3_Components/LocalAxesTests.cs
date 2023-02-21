using System;
using System.IO;
using System.Reflection;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using Xunit;

namespace IntegrationTests.Components
{
  [Collection("GrasshopperFixture collection")]
  public class LocalAxesTests
  {
    public static GH_Document Document
    {
      get
      {
        if (_document == null)
          _document = OpenDocument();
        return _document;
      }
    }
    private static GH_Document _document = null;
    private static GH_Document OpenDocument()
    {
      Type thisClass = MethodBase.GetCurrentMethod().DeclaringType;
      string fileName = thisClass.Name + ".gh";
      fileName = fileName.Replace(thisClass.Namespace, string.Empty).Replace("Tests", string.Empty);

      string solutiondir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.Parent.FullName;
      string path = Path.Combine(new string[] { solutiondir, "ExampleFiles", "Components" });

      return Helper.CreateDocument(Path.Combine(path, fileName));
    }

    [Fact]
    public void LocalAxesTest()
    {
      GH_Document doc = Document;

      IGH_Param paramLine = Helper.FindParameter(doc, "line");
      IGH_Param paramX1 = Helper.FindParameter(doc, "X1");
      IGH_Param paramY1 = Helper.FindParameter(doc, "Y1");
      IGH_Param paramZ1 = Helper.FindParameter(doc, "Z1");
      IGH_Param paramX2 = Helper.FindParameter(doc, "X2");
      IGH_Param paramY2 = Helper.FindParameter(doc, "Y2");
      IGH_Param paramZ2 = Helper.FindParameter(doc, "Z2");
      for (int i = 0; i < 20; i++)
      {
        GH_Line outputLine = (GH_Line)paramLine.VolatileData.get_Branch(0)[i];
        double x = outputLine.Value.ToX - outputLine.Value.FromX;
        double y = outputLine.Value.ToY - outputLine.Value.FromY;
        double z = outputLine.Value.ToZ - outputLine.Value.FromZ;
        Vector3d vector = new Vector3d(x, y, z);
        vector.Unitize();

        GH_Vector outputX1 = (GH_Vector)paramX1.VolatileData.get_Branch(0)[i];
        GH_Vector outputY1 = (GH_Vector)paramY1.VolatileData.get_Branch(0)[i];
        GH_Vector outputZ1 = (GH_Vector)paramZ1.VolatileData.get_Branch(0)[i];
        GH_Vector outputX2 = (GH_Vector)paramX2.VolatileData.get_Branch(0)[i];
        GH_Vector outputY2 = (GH_Vector)paramY2.VolatileData.get_Branch(0)[i];
        GH_Vector outputZ2 = (GH_Vector)paramZ2.VolatileData.get_Branch(0)[i];

        // is x vector in direction of the line?
        Assert.Equal(outputX1.Value.X, vector.X, 6);
        Assert.Equal(outputX1.Value.Y, vector.Y, 6);
        Assert.Equal(outputX1.Value.Z, vector.Z, 6);

        Assert.Equal(outputX1.Value.X, outputX2.Value.X, 6);
        Assert.Equal(outputX1.Value.Y, outputX2.Value.Y, 6);
        Assert.Equal(outputX1.Value.Z, outputX2.Value.Z, 6);
        Assert.Equal(outputY1.Value.X, outputY2.Value.X, 6);
        Assert.Equal(outputY1.Value.Y, outputY2.Value.Y, 6);
        Assert.Equal(outputY1.Value.Z, outputY2.Value.Z, 6);
        Assert.Equal(outputZ1.Value.X, outputZ2.Value.X, 6);
        Assert.Equal(outputZ1.Value.Y, outputZ2.Value.Y, 6);
        Assert.Equal(outputZ1.Value.Z, outputZ2.Value.Z, 6);
      }

      IGH_Param paramX3 = Helper.FindParameter(doc, "X3");
      IGH_Param paramY3 = Helper.FindParameter(doc, "Y3");
      IGH_Param paramZ3 = Helper.FindParameter(doc, "Z3");
      IGH_Param paramX4 = Helper.FindParameter(doc, "X4");
      IGH_Param paramY4 = Helper.FindParameter(doc, "Y4");
      IGH_Param paramZ4 = Helper.FindParameter(doc, "Z4");
      for (int i = 0; i < 20; i++)
      {
        GH_Vector outputX3 = (GH_Vector)paramX3.VolatileData.get_Branch(0)[i];
        GH_Vector outputY3 = (GH_Vector)paramY3.VolatileData.get_Branch(0)[i];
        GH_Vector outputZ3 = (GH_Vector)paramZ3.VolatileData.get_Branch(0)[i];
        GH_Vector outputX4 = (GH_Vector)paramX4.VolatileData.get_Branch(0)[i];
        GH_Vector outputY4 = (GH_Vector)paramY4.VolatileData.get_Branch(0)[i];
        GH_Vector outputZ4 = (GH_Vector)paramZ4.VolatileData.get_Branch(0)[i];

        Assert.Equal(outputX3.Value.X, outputX4.Value.X, 6);
        Assert.Equal(outputX3.Value.Y, outputX4.Value.Y, 6);
        Assert.Equal(outputX3.Value.Z, outputX4.Value.Z, 6);
        Assert.Equal(outputY3.Value.X, outputY4.Value.X, 6);
        Assert.Equal(outputY3.Value.Y, outputY4.Value.Y, 6);
        Assert.Equal(outputY3.Value.Z, outputY4.Value.Z, 6);
        Assert.Equal(outputZ3.Value.X, outputZ4.Value.X, 6);
        Assert.Equal(outputZ3.Value.Y, outputZ4.Value.Y, 6);
        Assert.Equal(outputZ3.Value.Z, outputZ4.Value.Z, 6);
      }
    }
  }
}
