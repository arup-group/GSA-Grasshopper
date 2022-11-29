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
using System.Drawing;

namespace GsaGHTests.Helpers.Export
{
  public partial class AssembleModelTests
  {
    [Fact]
    public void AssembleModelWithMember1dTest()
    {
      string profile = "STD I 900 300 9 4";
      GsaSectionGoo section = Section(profile, false);
      GH_Curve crv = new GH_Curve(new Line(new Point3d(0, 0, 0), new Point3d(10, 0, 0)).ToNurbsCurve());
      GsaMember1dGoo mem1d = Member1d(crv, section);

      mem1d.Value.Offset = new GsaOffset(12, 15, 17, 19, LengthUnit.Centimeter);
      mem1d.Value.MeshSize = 0.9;
      mem1d.Value.MeshWithOthers = false;
      mem1d.Value.Colour = Color.Red;
      mem1d.Value.Group = 4;
      mem1d.Value.Name = "name Name Name";
      mem1d.Value.OrientationAngle = new Angle(45, AngleUnit.Degree);
      mem1d.Value.ReleaseEnd = new GsaBool6(true, true, true, false, false, true);
      mem1d.Value.ReleaseStart = new GsaBool6(false, false, false, false, false, false);
      mem1d.Value.Type = MemberType.COLUMN;
      mem1d.Value.Type1D = ElementType.BEAM;

      GsaModelGoo modelGoo = (GsaModelGoo)ComponentTestHelper.GetOutput(
        CreateModelTest.CreateModelFromGeometry(null, null, new List<GsaMember1dGoo>() { mem1d }, null, null, ModelUnit.m));

      TestMember1d(mem1d.Value, LengthUnit.Meter, 1, modelGoo.Value);
    }

    [Fact]
    public void AssembleModelWithMember1dsTest()
    {
      string profile1 = "STD I 900 300 9 4";
      GsaSectionGoo section1 = Section(profile1, false);
      GH_Curve crv1 = new GH_Curve(new Line(new Point3d(0, 0, 0), new Point3d(10, 0, 0)).ToNurbsCurve());
      GsaMember1dGoo mem1d1 = Member1d(crv1, section1);

      string profile2 = "STD R 400 600";
      GsaSectionGoo section2 = Section(profile2, true);
      GH_Curve crv2 = new GH_Curve(new Line(new Point3d(0, 0, 0), new Point3d(0, 9, 0)).ToNurbsCurve());
      GsaMember1dGoo mem1d2 = Member1d(crv2, section2);

      GsaModelGoo modelGoo = (GsaModelGoo)ComponentTestHelper.GetOutput(
        CreateModelTest.CreateModelFromGeometry(null, null, new List<GsaMember1dGoo>() { mem1d1, mem1d2 }, null, null, ModelUnit.cm));

      TestMember1d(mem1d1.Value, LengthUnit.Centimeter, 1, modelGoo.Value);
      TestMember1d(mem1d2.Value, LengthUnit.Centimeter, 2, modelGoo.Value);
    }

    [Fact]
    public void AssembleModelWithMember2dTest()
    {
      Length thickness = new Length(200, LengthUnit.Millimeter);
      GsaProp2dGoo prop = Prop2d(thickness, true);
      GH_Brep brep = new GH_Brep(Brep.CreateFromCornerPoints(new Point3d(0, 0, 0), new Point3d(10, 0, 0), new Point3d(10, 10, 0), new Point3d(0, 10, 0), 1));
      GsaMember2dGoo mem2d = Member2d(brep, prop);

      GsaModelGoo modelGoo = (GsaModelGoo)ComponentTestHelper.GetOutput(
        CreateModelTest.CreateModelFromGeometry(null, null, null, new List<GsaMember2dGoo>() { mem2d }, null, ModelUnit.m));

      TestMember2d(mem2d.Value, LengthUnit.Meter, 1, modelGoo.Value);
    }

    [Fact]
    public void AssembleModelWithMember2dsTest()
    {
      Length thickness1 = new Length(200, LengthUnit.Millimeter);
      GsaProp2dGoo prop1 = Prop2d(thickness1, true);
      GH_Brep brep1 = new GH_Brep(Brep.CreateFromCornerPoints(new Point3d(0, 0, 0), new Point3d(10, 0, 0), new Point3d(10, 10, 0), new Point3d(0, 10, 0), 1));
      GsaMember2dGoo mem2d1 = Member2d(brep1, prop1);

      Length thickness2 = new Length(2, LengthUnit.Centimeter);
      GsaProp2dGoo prop2 = Prop2d(thickness2, false);
      GH_Brep brep2 = new GH_Brep(Brep.CreateFromCornerPoints(new Point3d(0, 5, -5), new Point3d(10, 5, -5), new Point3d(10, 5, 5), new Point3d(0, 5, 5), 1));
      GsaMember2dGoo mem2d2 = Member2d(brep2, prop2);

      GsaModelGoo modelGoo = (GsaModelGoo)ComponentTestHelper.GetOutput(
        CreateModelTest.CreateModelFromGeometry(null, null, null, new List<GsaMember2dGoo>() { mem2d1, mem2d2 }, null, ModelUnit.ft));

      TestMember2d(mem2d1.Value, LengthUnit.Foot, 1, modelGoo.Value);
      TestMember2d(mem2d2.Value, LengthUnit.Foot, 2, modelGoo.Value);
    }

    [Fact]
    public void AssembleModelWithMember3dTest()
    {
      GsaProp3dGoo prop = Prop3d(true);
      Box box = Box.Empty;
      box.X = new Interval(0, 10);
      box.Y = new Interval(0, 10);
      box.Z = new Interval(0, 10);
      GH_Box brep = new GH_Box(box);
      GsaMember3dGoo mem3d = Member3d(brep, prop);

      GsaModelGoo modelGoo = (GsaModelGoo)ComponentTestHelper.GetOutput(
        CreateModelTest.CreateModelFromGeometry(null, null, null, null, new List<GsaMember3dGoo>() { mem3d }, ModelUnit.m));

      TestMember3d(mem3d.Value, LengthUnit.Meter, 1, modelGoo.Value);
    }

    [Fact]
    public void AssembleModelWithMember3dsTest()
    {
      GsaProp3dGoo prop1 = Prop3d(true);
      Box box1 = Box.Empty;
      box1.X = new Interval(0, 10000);
      box1.Y = new Interval(0, 10000);
      box1.Z = new Interval(0, 10000);
      GH_Box brep1 = new GH_Box(box1);
      GsaMember3dGoo mem3d1 = Member3d(brep1, prop1);

      GsaProp3dGoo prop2 = Prop3d(false);
      Box box2 = Box.Empty;
      box2.X = new Interval(5000, 15000);
      box2.Y = new Interval(5000, 15000);
      box2.Z = new Interval(5000, 15000);
      GH_Box brep2 = new GH_Box(box2);
      GsaMember3dGoo mem3d2 = Member3d(brep2, prop2);

      GsaModelGoo modelGoo = (GsaModelGoo)ComponentTestHelper.GetOutput(
        CreateModelTest.CreateModelFromGeometry(null, null, null, null, new List<GsaMember3dGoo>() { mem3d1, mem3d2 }, ModelUnit.mm));

      TestMember3d(mem3d1.Value, LengthUnit.Millimeter, 1, modelGoo.Value);
      TestMember3d(mem3d2.Value, LengthUnit.Millimeter, 2, modelGoo.Value);
    }
  }
}
