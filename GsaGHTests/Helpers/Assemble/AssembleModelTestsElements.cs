using System.Collections.Generic;
using System.Drawing;

using Grasshopper.Kernel.Types;

using GsaAPI;

using GsaGH.Parameters;

using GsaGHTests.Model;

using OasysUnits;

using Rhino.Geometry;

using Xunit;

using AngleUnit = OasysUnits.Units.AngleUnit;
using LengthUnit = OasysUnits.Units.LengthUnit;
using Line = Rhino.Geometry.Line;

namespace GsaGHTests.Helpers.Export {
  [Collection("GrasshopperFixture collection")]
  public partial class AssembleModelTests {

    [Fact]
    public void AssembleModelWithElement1dsTest() {
      string profile1 = "STD I 900 300 9 4";
      GsaSectionGoo section1 = Section(profile1, false);
      var ln1 = new GH_Line(new Line(new Point3d(0, 0, 0), new Point3d(10, 0, 0)));
      GsaElement1dGoo elem1d1 = Element1d(ln1, section1);

      string profile2 = "STD R 400 600";
      GsaSectionGoo section2 = Section(profile2, true);
      var ln2 = new GH_Line(new Line(new Point3d(0, 0, 0), new Point3d(0, 9, 0)));
      GsaElement1dGoo elem1d2 = Element1d(ln2, section2);

      var modelGoo = (GsaModelGoo)ComponentTestHelper.GetOutput(
        CreateModelTest.CreateModelFromGeometry(null, new List<GsaElement1dGoo>() {
          elem1d1,
          elem1d2,
        }, null, null, null, null, ModelUnit.Ft));

      TestElement1d(elem1d1.Value, LengthUnit.Foot, 1, modelGoo.Value);
      TestElement1d(elem1d2.Value, LengthUnit.Foot, 2, modelGoo.Value);
    }

    [Fact]
    public void AssembleModelWithElement1dsByRefTest() {
      string profile1 = "STD I 900 300 9 4";
      GsaSectionGoo section1 = Section(profile1, false);
      section1.Value.Id = 1;
      var ln1 = new GH_Line(new Line(new Point3d(0, 0, 0), new Point3d(10, 0, 0)));
      GsaElement1dGoo elem1d1 = Element1d(ln1, section1.Value.Id);

      string profile2 = "STD R 400 600";
      GsaSectionGoo section2 = Section(profile2, true);
      section2.Value.Id = 2;
      var ln2 = new GH_Line(new Line(new Point3d(0, 0, 0), new Point3d(0, 9, 0)));
      GsaElement1dGoo elem1d2 = Element1d(ln2, section2.Value.Id);

      OasysGH.Components.GH_OasysDropDownComponent comp =
        CreateModelTest.CreateModelFromGeometry(null, new List<GsaElement1dGoo>() {
          elem1d1,
          elem1d2,
        }, null, null, null, null, ModelUnit.Ft);
      ComponentTestHelper.SetInput(comp, section1, 1);
      ComponentTestHelper.SetInput(comp, section2, 1);

      var modelGoo = (GsaModelGoo)ComponentTestHelper.GetOutput(comp);

      TestElement1d(Element1d(ln1, section1).Value, LengthUnit.Foot, 1, modelGoo.Value);
      TestElement1d(Element1d(ln2, section2).Value, LengthUnit.Foot, 2, modelGoo.Value);
    }

    [Fact]
    public void AssembleModelWithModelsFromElement1dsByRefTest() {
      string profile1 = "STD I 900 300 9 4";
      GsaSectionGoo section1 = Section(profile1, false);
      section1.Value.Id = 1;

      string profile2 = "STD R 400 600";
      GsaSectionGoo section2 = Section(profile2, true);
      section2.Value.Id = 2;

      OasysGH.Components.GH_OasysDropDownComponent comp1 =
        CreateModelTest.CreateModelFromProperties(new List<GsaSectionGoo>() {
          section1,
          section2,
        }, null, null, null);

      var ln1 = new GH_Line(new Line(new Point3d(0, 0, 0), new Point3d(10, 0, 0)));
      GsaElement1dGoo elem1d1 = Element1d(ln1, section1.Value.Id);
      elem1d1.Value.Id = 1;

      var ln2 = new GH_Line(new Line(new Point3d(0, 0, 0), new Point3d(0, 9, 0)));
      GsaElement1dGoo elem1d2 = Element1d(ln2, section2.Value.Id);
      elem1d2.Value.Id = 2;

      OasysGH.Components.GH_OasysDropDownComponent comp2 =
        CreateModelTest.CreateModelFromGeometry(null, new List<GsaElement1dGoo>() {
          elem1d1,
          elem1d2,
        }, null, null, null, null, ModelUnit.Ft);

      OasysGH.Components.GH_OasysDropDownComponent comp =
        CreateModelTest.CreateModelFromModels(new List<GsaModelGoo>() {
          (GsaModelGoo)ComponentTestHelper.GetOutput(comp1),
          (GsaModelGoo)ComponentTestHelper.GetOutput(comp2),
        });

      var modelGoo = (GsaModelGoo)ComponentTestHelper.GetOutput(comp);

      TestElement1d(Element1d(ln1, section1).Value, LengthUnit.Foot, 1, modelGoo.Value);
      TestElement1d(Element1d(ln2, section2).Value, LengthUnit.Foot, 2, modelGoo.Value);
    }

    [Fact]
    public void AssembleModelWithElement1dTest() {
      string profile = "STD I 900 300 9 4";
      GsaSectionGoo section = Section(profile, false);
      var ln = new GH_Line(new Line(new Point3d(0, 0, 0), new Point3d(10, 0, 0)));
      GsaElement1dGoo elem1d = Element1d(ln, section);

      elem1d.Value.Offset = new GsaOffset(12, 15, 17, 19, LengthUnit.Centimeter);
      elem1d.Value.ApiElement.Colour = Color.Red;
      elem1d.Value.ApiElement.Group = 4;
      elem1d.Value.ApiElement.Name = "name Name Name";
      elem1d.Value.OrientationAngle = new Angle(45, AngleUnit.Degree);
      elem1d.Value.ReleaseEnd = new GsaBool6(true, true, true, false, false, true);
      elem1d.Value.ReleaseStart = new GsaBool6(false, false, false, false, false, false);
      elem1d.Value.ApiElement.Type = ElementType.BEAM;

      var modelGoo = (GsaModelGoo)ComponentTestHelper.GetOutput(
        CreateModelTest.CreateModelFromGeometry(null, new List<GsaElement1dGoo>() {
          elem1d,
        }, null, null, null, null, ModelUnit.M));

      TestElement1d(elem1d.Value, LengthUnit.Meter, 1, modelGoo.Value);
    }

    [Fact]
    public void AssembleModelWithElement2dsTest() {
      var thickness1 = new Length(200, LengthUnit.Millimeter);
      GsaProperty2dGoo prop1 = Prop2d(thickness1, true);
      var m1 = new Mesh();
      m1.Vertices.Add(new Point3d(0, 0, 0));
      m1.Vertices.Add(new Point3d(1, 0, 0));
      m1.Vertices.Add(new Point3d(1, 1, 0));
      m1.Vertices.Add(new Point3d(0, 1, 0));
      m1.Vertices.Add(new Point3d(0, -1, 0));
      m1.Faces.AddFace(0, 1, 2, 3);
      m1.Faces.AddFace(0, 4, 1);
      var mesh1 = new GH_Mesh(m1);
      GsaElement2dGoo elem2d1 = Element2d(mesh1, prop1);

      var thickness2 = new Length(2, LengthUnit.Centimeter);
      GsaProperty2dGoo prop2 = Prop2d(thickness2, false);
      var m2 = new Mesh();
      m2.Vertices.Add(new Point3d(1, 0, 0));
      m2.Vertices.Add(new Point3d(2, 0, 0));
      m2.Vertices.Add(new Point3d(2, 2, 0));
      m2.Vertices.Add(new Point3d(1, 2, 0));
      m2.Vertices.Add(new Point3d(0, 0, -1));
      m2.Faces.AddFace(0, 1, 2, 3);
      m2.Faces.AddFace(0, 4, 1);
      var mesh2 = new GH_Mesh(m2);
      GsaElement2dGoo elem2d2 = Element2d(mesh2, prop2);

      var modelGoo = (GsaModelGoo)ComponentTestHelper.GetOutput(
        CreateModelTest.CreateModelFromGeometry(null, null, new List<GsaElement2dGoo>() {
          elem2d1,
          elem2d2,
        }, null, null, null, ModelUnit.Ft));

      TestElement2d(elem2d1.Value, LengthUnit.Foot, new List<int>() {
        1,
        2,
      }, modelGoo.Value);
      TestElement2d(elem2d2.Value, LengthUnit.Foot, new List<int>() {
        3,
        4,
      }, modelGoo.Value);
    }

    [Fact]
    public void AssembleModelWithElement2dsByRefTest() {
      var thickness1 = new Length(200, LengthUnit.Millimeter);
      GsaProperty2dGoo prop1 = Prop2d(thickness1, true);
      prop1.Value.Id = 1;

      var m1 = new Mesh();
      m1.Vertices.Add(new Point3d(0, 0, 0));
      m1.Vertices.Add(new Point3d(1, 0, 0));
      m1.Vertices.Add(new Point3d(1, 1, 0));
      m1.Vertices.Add(new Point3d(0, 1, 0));
      m1.Vertices.Add(new Point3d(0, -1, 0));
      m1.Faces.AddFace(0, 1, 2, 3);
      m1.Faces.AddFace(0, 4, 1);
      var mesh1 = new GH_Mesh(m1);
      GsaElement2dGoo elem2d1 = Element2d(mesh1, prop1.Value.Id);

      var thickness2 = new Length(2, LengthUnit.Centimeter);
      GsaProperty2dGoo prop2 = Prop2d(thickness2, false);
      prop2.Value.Id = 2;

      var m2 = new Mesh();
      m2.Vertices.Add(new Point3d(1, 0, 0));
      m2.Vertices.Add(new Point3d(2, 0, 0));
      m2.Vertices.Add(new Point3d(2, 2, 0));
      m2.Vertices.Add(new Point3d(1, 2, 0));
      m2.Vertices.Add(new Point3d(0, 0, -1));
      m2.Faces.AddFace(0, 1, 2, 3);
      m2.Faces.AddFace(0, 4, 1);
      var mesh2 = new GH_Mesh(m2);
      GsaElement2dGoo elem2d2 = Element2d(mesh2, prop2.Value.Id);

      OasysGH.Components.GH_OasysDropDownComponent comp =
        CreateModelTest.CreateModelFromGeometry(null, null, new List<GsaElement2dGoo>() {
          elem2d1,
          elem2d2,
        }, null, null, null, ModelUnit.Ft);
      ComponentTestHelper.SetInput(comp, prop1, 1);
      ComponentTestHelper.SetInput(comp, prop2, 1);

      var modelGoo = (GsaModelGoo)ComponentTestHelper.GetOutput(comp);

      TestElement2d(Element2d(mesh1, prop1).Value, LengthUnit.Foot, new List<int>() {
        1,
        2,
      }, modelGoo.Value);
      TestElement2d(Element2d(mesh2, prop2).Value, LengthUnit.Foot, new List<int>() {
        3,
        4,
      }, modelGoo.Value);
    }

    [Fact]
    public void AssembleModelWithElement2dTest() {
      var thickness = new Length(200, LengthUnit.Millimeter);
      GsaProperty2dGoo prop = Prop2d(thickness, true);
      var m = new Mesh();
      m.Vertices.Add(new Point3d(0, 0, 0));
      m.Vertices.Add(new Point3d(10, 0, 0));
      m.Vertices.Add(new Point3d(10, 10, 0));
      m.Vertices.Add(new Point3d(0, 10, 0));
      m.Vertices.Add(new Point3d(0, -10, 0));
      m.Faces.AddFace(0, 1, 2, 3);
      m.Faces.AddFace(0, 4, 1);
      var mesh = new GH_Mesh(m);
      GsaElement2dGoo elem2d = Element2d(mesh, prop);
      var modelGoo = (GsaModelGoo)ComponentTestHelper.GetOutput(
        CreateModelTest.CreateModelFromGeometry(null, null, new List<GsaElement2dGoo>() {
          elem2d,
        }, null, null, null, ModelUnit.Cm));

      TestElement2d(elem2d.Value, LengthUnit.Centimeter, new List<int>() {
        1,
        2,
      }, modelGoo.Value);
    }

    [Fact]
    public void AssembleModelWithElementsTest() {
      string profile1 = "STD I 900 300 9 4";
      GsaSectionGoo section1 = Section(profile1, false);
      var ln1 = new GH_Line(new Line(new Point3d(100, 0, 0), new Point3d(100, 0, -100)));
      GsaElement1dGoo elem1d1 = Element1d(ln1, section1);

      string profile2 = "STD R 400 600";
      GsaSectionGoo section2 = Section(profile2, true);
      var ln2 = new GH_Line(new Line(new Point3d(0, 0, 0), new Point3d(0, 0, -100)));
      GsaElement1dGoo elem1d2 = Element1d(ln2, section2);

      var thickness1 = new Length(200, LengthUnit.Millimeter);
      GsaProperty2dGoo prop1 = Prop2d(thickness1, true);
      var m1 = new Mesh();
      m1.Vertices.Add(new Point3d(0, 0, 0));
      m1.Vertices.Add(new Point3d(100, 0, 0));
      m1.Vertices.Add(new Point3d(100, 100, 0));
      m1.Vertices.Add(new Point3d(0, 100, 0));
      m1.Vertices.Add(new Point3d(0, -100, 0));
      m1.Faces.AddFace(0, 1, 2, 3);
      m1.Faces.AddFace(0, 4, 1);
      var mesh1 = new GH_Mesh(m1);
      GsaElement2dGoo elem2d1 = Element2d(mesh1, prop1);

      var thickness2 = new Length(2, LengthUnit.Centimeter);
      GsaProperty2dGoo prop2 = Prop2d(thickness2, false);
      var m2 = new Mesh();
      m2.Vertices.Add(new Point3d(100, 0, 0));
      m2.Vertices.Add(new Point3d(200, 0, 0));
      m2.Vertices.Add(new Point3d(200, 200, 0));
      m2.Vertices.Add(new Point3d(100, 200, 0));
      m2.Vertices.Add(new Point3d(0, 0, -100));
      m2.Faces.AddFace(0, 1, 2, 3);
      m2.Faces.AddFace(0, 4, 1);
      var mesh2 = new GH_Mesh(m2);
      GsaElement2dGoo elem2d2 = Element2d(mesh2, prop2);

      var modelGoo = (GsaModelGoo)ComponentTestHelper.GetOutput(
        CreateModelTest.CreateModelFromGeometry(null, new List<GsaElement1dGoo>() {
          elem1d1,
          elem1d2,
        }, new List<GsaElement2dGoo>() {
          elem2d1,
          elem2d2,
        }, null, null, null, ModelUnit.Inch));

      TestElement1d(elem1d1.Value, LengthUnit.Inch, 1, modelGoo.Value);
      TestElement1d(elem1d2.Value, LengthUnit.Inch, 2, modelGoo.Value);
      TestElement2d(elem2d1.Value, LengthUnit.Inch, new List<int>() {
        3,
        4,
      }, modelGoo.Value);
      TestElement2d(elem2d2.Value, LengthUnit.Inch, new List<int>() {
        5,
        6,
      }, modelGoo.Value);
    }
  }
}
