using GsaGH.Components;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using Grasshopper.Kernel.Types;
using Xunit;
using OasysGH.Components;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Rhino.Geometry;
using GsaGHTests.Model;
using GsaGHTests.Components.Properties;
using GsaAPI;
using OasysUnits.Units;
using OasysUnits;

namespace GsaGHTests.Helpers.Export
{
  [Collection("GrasshopperFixture collection")]
  public partial class AssembleModelTests
  {
    [Fact]
    public void AssembleModelWithElement1dTest()
    {
      string profile = "STD I 900 300 9 4";
      GsaSectionGoo section = Section(profile, false);
      GH_Line ln = new GH_Line(new Line(new Point3d(0, 0, 0), new Point3d(10, 0, 0)));
      GsaElement1dGoo elem1d = Element1d(ln, section);

      GsaModelGoo modelGoo = (GsaModelGoo)ComponentTestHelper.GetOutput(
        CreateModelTest.CreateModelFromGeometry(new List<GsaElement1dGoo>() { elem1d }, null, null, null, null, ModelUnit.m));

      TestElement1d(elem1d.Value, LengthUnit.Meter, 1, modelGoo.Value);
    }

    [Fact]
    public void AssembleModelWithElement1dsTest()
    {
      string profile1 = "STD I 900 300 9 4";
      GsaSectionGoo section1 = Section(profile1, false);
      GH_Line ln1 = new GH_Line(new Line(new Point3d(0, 0, 0), new Point3d(10, 0, 0)));
      GsaElement1dGoo elem1d1 = Element1d(ln1, section1);

      string profile2 = "STD R 400 600";
      GsaSectionGoo section2 = Section(profile2, true);
      GH_Line ln2 = new GH_Line(new Line(new Point3d(0, 0, 0), new Point3d(0, 9, 0)));
      GsaElement1dGoo elem1d2 = Element1d(ln2, section2);

      GsaModelGoo modelGoo = (GsaModelGoo)ComponentTestHelper.GetOutput(
        CreateModelTest.CreateModelFromGeometry(new List<GsaElement1dGoo>() { elem1d1, elem1d2 }, null, null, null, null, ModelUnit.ft));

      TestElement1d(elem1d1.Value, LengthUnit.Foot, 1, modelGoo.Value);
      TestElement1d(elem1d2.Value, LengthUnit.Foot, 2, modelGoo.Value);
    }


    [Fact]
    public void AssembleModelWithElement2dTest()
    {
      Length thickness = new Length(200, LengthUnit.Millimeter);
      GsaProp2dGoo prop = Prop2d(thickness, true);
      Mesh m = new Mesh();
      m.Vertices.Add(new Point3d(0, 0, 0));
      m.Vertices.Add(new Point3d(1, 0, 0));
      m.Vertices.Add(new Point3d(1, 1, 0));
      m.Vertices.Add(new Point3d(0, 1, 0));
      m.Vertices.Add(new Point3d(0, -1, 0));
      m.Faces.AddFace(0, 1, 2, 3);
      m.Faces.AddFace(0, 4, 1);
      GH_Mesh mesh = new GH_Mesh(m);
      GsaElement2dGoo elem2d = Element2d(mesh, prop);
      GsaModelGoo modelGoo = (GsaModelGoo)ComponentTestHelper.GetOutput(
        CreateModelTest.CreateModelFromGeometry(null, new List<GsaElement2dGoo>() { elem2d }, null, null, null, ModelUnit.cm));

      TestElement2d(elem2d.Value, LengthUnit.Centimeter, new List<int>() { 1, 2 }, modelGoo.Value);
    }
  }
}
