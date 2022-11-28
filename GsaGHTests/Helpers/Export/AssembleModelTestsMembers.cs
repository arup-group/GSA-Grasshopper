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
    

  }
}
