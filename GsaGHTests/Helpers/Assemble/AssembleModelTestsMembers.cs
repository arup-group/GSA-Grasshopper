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
  public partial class AssembleModelTests {

    [Fact]
    public void AssembleModelWithMember1dsTest() {
      string profile1 = "STD I 900 300 9 4";
      GsaSectionGoo section1 = Section(profile1, false);
      var crv1 = new GH_Curve(new Line(new Point3d(0, 0, 0), new Point3d(10, 0, 0)).ToNurbsCurve());
      GsaMember1dGoo mem1d1 = Member1d(crv1, section1);

      string profile2 = "STD R 400 600";
      GsaSectionGoo section2 = Section(profile2, true);
      var crv2 = new GH_Curve(new Line(new Point3d(0, 0, 0), new Point3d(0, 9, 0)).ToNurbsCurve());
      GsaMember1dGoo mem1d2 = Member1d(crv2, section2);

      var modelGoo = (GsaModelGoo)ComponentTestHelper.GetOutput(
        CreateModelTest.CreateModelFromGeometry(null, null, null, new List<GsaMember1dGoo>() {
          mem1d1,
          mem1d2,
        }, null, null, ModelUnit.Cm));

      TestMember1d(mem1d1.Value, LengthUnit.Centimeter, 1, modelGoo.Value);
      TestMember1d(mem1d2.Value, LengthUnit.Centimeter, 2, modelGoo.Value);
    }

    [Fact]
    public void AssembleModelWithMember1dsByRefTest() {
      string profile1 = "STD I 900 300 9 4";
      GsaSectionGoo section1 = Section(profile1, false);
      section1.Value.Id = 1;

      var crv1 = new GH_Curve(new Line(new Point3d(0, 0, 0), new Point3d(10, 0, 0)).ToNurbsCurve());
      GsaMember1dGoo mem1d1 = Member1d(crv1, section1.Value.Id);

      string profile2 = "STD R 400 600";
      GsaSectionGoo section2 = Section(profile2, true);
      section2.Value.Id = 2;

      var crv2 = new GH_Curve(new Line(new Point3d(0, 0, 0), new Point3d(0, 9, 0)).ToNurbsCurve());
      GsaMember1dGoo mem1d2 = Member1d(crv2, section2.Value.Id);

      OasysGH.Components.GH_OasysDropDownComponent comp =
        CreateModelTest.CreateModelFromGeometry(null, null, null, new List<GsaMember1dGoo>() {
          mem1d1,
          mem1d2,
        }, null, null, ModelUnit.Cm);
      ComponentTestHelper.SetInput(comp, section1, 1);
      ComponentTestHelper.SetInput(comp, section2, 1);

      var modelGoo = (GsaModelGoo)ComponentTestHelper.GetOutput(comp);

      TestMember1d(Member1d(crv1, section1).Value, LengthUnit.Centimeter, 1, modelGoo.Value);
      TestMember1d(Member1d(crv2, section2).Value, LengthUnit.Centimeter, 2, modelGoo.Value);
    }

    [Fact]
    public void AssembleModelWithModelsFromMember1dsByRefTest() {
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

      var crv1 = new GH_Curve(new Line(new Point3d(0, 0, 0), new Point3d(10, 0, 0)).ToNurbsCurve());
      GsaMember1dGoo mem1d1 = Member1d(crv1, section1.Value.Id);

      var crv2 = new GH_Curve(new Line(new Point3d(0, 0, 0), new Point3d(0, 9, 0)).ToNurbsCurve());
      GsaMember1dGoo mem1d2 = Member1d(crv2, section2.Value.Id);

      OasysGH.Components.GH_OasysDropDownComponent comp2 =
        CreateModelTest.CreateModelFromGeometry(null, null, null, new List<GsaMember1dGoo>() {
          mem1d1,
          mem1d2,
        }, null, null, ModelUnit.Cm);

      OasysGH.Components.GH_OasysDropDownComponent comp =
        CreateModelTest.CreateModelFromModels(new List<GsaModelGoo>() {
          (GsaModelGoo)ComponentTestHelper.GetOutput(comp1),
          (GsaModelGoo)ComponentTestHelper.GetOutput(comp2),
        });

      var modelGoo = (GsaModelGoo)ComponentTestHelper.GetOutput(comp);

      TestMember1d(Member1d(crv1, section1).Value, LengthUnit.Centimeter, 1, modelGoo.Value);
      TestMember1d(Member1d(crv2, section2).Value, LengthUnit.Centimeter, 2, modelGoo.Value);
    }

    [Fact]
    public void AssembleModelWithMember1dTest() {
      string profile = "STD I 900 300 9 4";
      GsaSectionGoo section = Section(profile, false);
      var crv = new GH_Curve(new Line(new Point3d(0, 0, 0), new Point3d(10, 0, 0)).ToNurbsCurve());
      GsaMember1dGoo mem1d = Member1d(crv, section);

      mem1d.Value.Offset = new GsaOffset(12, 15, 17, 19, LengthUnit.Centimeter);
      mem1d.Value.ApiMember.MeshSize = 0.9;
      mem1d.Value.ApiMember.IsIntersector = false;
      mem1d.Value.ApiMember.Colour = Color.Red;
      mem1d.Value.ApiMember.Group = 4;
      mem1d.Value.ApiMember.Name = "name Name Name";
      mem1d.Value.OrientationAngle = new Angle(45, AngleUnit.Degree);
      mem1d.Value.ReleaseEnd = new GsaBool6(true, true, true, false, false, true);
      mem1d.Value.ReleaseStart = new GsaBool6(false, false, false, false, false, false);
      mem1d.Value.ApiMember.Type = MemberType.COLUMN;
      mem1d.Value.ApiMember.Type1D = ElementType.BEAM;

      var modelGoo = (GsaModelGoo)ComponentTestHelper.GetOutput(
        CreateModelTest.CreateModelFromGeometry(null, null, null, new List<GsaMember1dGoo>() {
          mem1d,
        }, null, null, ModelUnit.M));

      TestMember1d(mem1d.Value, LengthUnit.Meter, 1, modelGoo.Value);
    }

    [Fact]
    public void AssembleModelWithMember2dsTest() {
      var thickness1 = new Length(200, LengthUnit.Millimeter);
      GsaProperty2dGoo prop1 = Prop2d(thickness1, true);
      var brep1 = new GH_Brep(Brep.CreateFromCornerPoints(new Point3d(0, 0, 0),
        new Point3d(10, 0, 0), new Point3d(10, 10, 0), new Point3d(0, 10, 0), 1));
      GsaMember2dGoo mem2d1 = Member2d(brep1, prop1);

      var thickness2 = new Length(2, LengthUnit.Centimeter);
      GsaProperty2dGoo prop2 = Prop2d(thickness2, false);
      var brep2 = new GH_Brep(Brep.CreateFromCornerPoints(new Point3d(0, 5, -5),
        new Point3d(10, 5, -5), new Point3d(10, 5, 5), new Point3d(0, 5, 5), 1));
      GsaMember2dGoo mem2d2 = Member2d(brep2, prop2);

      var modelGoo = (GsaModelGoo)ComponentTestHelper.GetOutput(
        CreateModelTest.CreateModelFromGeometry(null, null, null, null, new List<GsaMember2dGoo>() {
          mem2d1,
          mem2d2,
        }, null, ModelUnit.Ft));

      TestMember2d(mem2d1.Value, LengthUnit.Foot, 1, modelGoo.Value);
      TestMember2d(mem2d2.Value, LengthUnit.Foot, 2, modelGoo.Value);
    }

    [Fact]
    public void AssembleModelWithMember2dsByRefTest() {
      var thickness1 = new Length(200, LengthUnit.Millimeter);
      GsaProperty2dGoo prop1 = Prop2d(thickness1, true);
      prop1.Value.Id = 1;

      var brep1 = new GH_Brep(Brep.CreateFromCornerPoints(new Point3d(0, 0, 0),
        new Point3d(10, 0, 0), new Point3d(10, 10, 0), new Point3d(0, 10, 0), 1));
      GsaMember2dGoo mem2d1 = Member2d(brep1, prop1.Value.Id);

      var thickness2 = new Length(2, LengthUnit.Centimeter);
      GsaProperty2dGoo prop2 = Prop2d(thickness2, false);
      prop2.Value.Id = 2;

      var brep2 = new GH_Brep(Brep.CreateFromCornerPoints(new Point3d(0, 5, -5),
        new Point3d(10, 5, -5), new Point3d(10, 5, 5), new Point3d(0, 5, 5), 1));
      GsaMember2dGoo mem2d2 = Member2d(brep2, prop2.Value.Id);

      OasysGH.Components.GH_OasysDropDownComponent comp =
        CreateModelTest.CreateModelFromGeometry(null, null, null, null, new List<GsaMember2dGoo>() {
          mem2d1,
          mem2d2,
        }, null, ModelUnit.Ft);
      ComponentTestHelper.SetInput(comp, prop1, 1);
      ComponentTestHelper.SetInput(comp, prop2, 1);

      var modelGoo = (GsaModelGoo)ComponentTestHelper.GetOutput(comp);

      TestMember2d(Member2d(brep1, prop1).Value, LengthUnit.Foot, 1, modelGoo.Value);
      TestMember2d(Member2d(brep2, prop2).Value, LengthUnit.Foot, 2, modelGoo.Value);
    }

    [Fact]
    public void AssembleModelWithMember2dTest() {
      var thickness = new Length(200, LengthUnit.Millimeter);
      GsaProperty2dGoo prop = Prop2d(thickness, true);
      var brep = new GH_Brep(Brep.CreateFromCornerPoints(new Point3d(0, 0, 0),
        new Point3d(10, 0, 0), new Point3d(10, 10, 0), new Point3d(0, 10, 0), 1));
      GsaMember2dGoo mem2d = Member2d(brep, prop);

      var modelGoo = (GsaModelGoo)ComponentTestHelper.GetOutput(
        CreateModelTest.CreateModelFromGeometry(null, null, null, null, new List<GsaMember2dGoo>() {
          mem2d,
        }, null, ModelUnit.M));

      TestMember2d(mem2d.Value, LengthUnit.Meter, 1, modelGoo.Value);
    }

    [Fact]
    public void AssembleModelWithMember3dsTest() {
      GsaProperty3dGoo prop1 = Prop3d(true);
      Box box1 = Box.Empty;
      box1.X = new Interval(0, 10000);
      box1.Y = new Interval(0, 10000);
      box1.Z = new Interval(0, 10000);
      var brep1 = new GH_Box(box1);
      GsaMember3dGoo mem3d1 = Member3d(brep1, prop1);

      GsaProperty3dGoo prop2 = Prop3d(false);
      Box box2 = Box.Empty;
      box2.X = new Interval(5000, 15000);
      box2.Y = new Interval(5000, 15000);
      box2.Z = new Interval(5000, 15000);
      var brep2 = new GH_Box(box2);
      GsaMember3dGoo mem3d2 = Member3d(brep2, prop2);

      var modelGoo = (GsaModelGoo)ComponentTestHelper.GetOutput(
        CreateModelTest.CreateModelFromGeometry(null, null, null, null, null,
          new List<GsaMember3dGoo>() {
            mem3d1,
            mem3d2,
          }, ModelUnit.Mm));

      TestMember3d(mem3d1.Value, LengthUnit.Millimeter, 1, modelGoo.Value);
      TestMember3d(mem3d2.Value, LengthUnit.Millimeter, 2, modelGoo.Value);
    }

    [Fact]
    public void AssembleModelWithMember3dTest() {
      GsaProperty3dGoo prop = Prop3d(true);
      Box box = Box.Empty;
      box.X = new Interval(0, 10);
      box.Y = new Interval(0, 10);
      box.Z = new Interval(0, 10);
      var brep = new GH_Box(box);
      GsaMember3dGoo mem3d = Member3d(brep, prop);

      var modelGoo = (GsaModelGoo)ComponentTestHelper.GetOutput(
        CreateModelTest.CreateModelFromGeometry(null, null, null, null, null,
          new List<GsaMember3dGoo>() {
            mem3d,
          }, ModelUnit.M));

      TestMember3d(mem3d.Value, LengthUnit.Meter, 1, modelGoo.Value);
    }
  }
}
