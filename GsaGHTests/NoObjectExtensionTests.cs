using System.Collections.Generic;
using GsaAPI;
using GsaGH.Parameters;
using GsaGHTests.Helpers;
using OasysUnits.Units;
using Rhino.Geometry;
using Xunit;

namespace GsaGHTests
{
  [Collection("GrasshopperFixture collection")]
  public class NoObjectExtensionTests
  {
    [Fact]
    public void GsaModelEqualsTest()
    {
      GsaModel original = new GsaModel();
      GsaModel duplicate = original.Duplicate();
      Duplicates.AreEqual(original, duplicate);
    }

    [Fact]
    public void GsaBool6EqualsTest()
    {
      GsaBool6 original = new GsaBool6();
      GsaBool6 duplicate = original.Duplicate();

      Duplicates.AreEqual(original, duplicate);
    }

    [Fact]
    public void GsaMaterialEqualsTest()
    {
      GsaMaterial original = new GsaMaterial();
      original.MaterialType = GsaMaterial.MatType.ALUMINIUM;
      GsaMaterial duplicate = original.Duplicate();

      Duplicates.AreEqual(original, duplicate);
    }

    [Fact]
    public void GsaProp2dEqualsTest()
    {
      GsaProp2d original = new GsaProp2d();
      original.Name = "Name";
      original.Thickness = new OasysUnits.Length(200, LengthUnit.Millimeter);
      GsaProp2d duplicate = original.Duplicate();

      Duplicates.AreEqual(original, duplicate);
    }

    [Fact]
    public void GsaProp3dEqualsTest()
    {
      GsaProp3d original = new GsaProp3d(new GsaMaterial());
      original.Name = "Name";
      GsaProp3d duplicate = original.Duplicate();

      Duplicates.AreEqual(original, duplicate);
    }

    [Fact]
    public void GsaSectionEqualsTest()
    {
      GsaSection original = new GsaSection();
      original.Name = "Name";
      GsaSection duplicate = original.Duplicate();

      Duplicates.AreEqual(original, duplicate);
    }

    [Fact]
    public void GsaSectionModifierEqualsTest()
    {
      GsaSectionModifier original = new GsaSectionModifier();
      original.StressOption = GsaSectionModifier.StressOptionType.NoCalculation;
      GsaSectionModifier duplicate = original.Duplicate();

      Duplicates.AreEqual(original, duplicate);
    }

    [Fact]
    public void GsaElement1dEqualsTest()
    {
      GsaSection section = new GsaSection();
      section.Name = "Name";
      GsaElement1d original = new GsaElement1d(new Element(), new LineCurve(), 1, section, new GsaNode());
      original.Name = "Name";
      GsaElement1d duplicate = original.Duplicate();

      Duplicates.AreEqual(original, duplicate);
    }

    [Fact]
    public void GsaElement2dEqualsTest()
    {
      GsaElement2d original = new GsaElement2d(new Mesh());
      GsaElement2d duplicate = original.Duplicate();

      Duplicates.AreEqual(original, duplicate);
    }

    [Fact]
    public void GsaElement3dEqualsTest()
    {
      GsaElement3d original = new GsaElement3d(new Mesh());
      GsaElement3d duplicate = original.Duplicate();
      Duplicates.AreEqual(original, duplicate);
    }

    [Fact]
    public void GsaMember1dEqualsTest()
    {
      GsaSection section = new GsaSection();
      section.Name = "Name";
      GsaMember1d original = new GsaMember1d(new Member(), 1, new List<Point3d>(), new List<string>(), section, new GsaNode());
      original.Name = "Name";
      GsaMember1d duplicate = original.Duplicate();
      Duplicates.AreEqual(original, duplicate);
    }

    [Fact]
    public void GsaMember3dEqualsTest()
    {
      GsaProp3d prop = new GsaProp3d(new GsaMaterial());
      prop.Name = "Name";
      GsaMember3d original = new GsaMember3d(new Member(), 1, new Mesh(), prop);
      original.Name = "Name";
      GsaMember3d duplicate = original.Duplicate();

      Duplicates.AreEqual(original, duplicate);
    }

    [Fact]
    public void GsaNodeEqualsTest()
    {
      GsaNode original = new GsaNode();
      GsaNode duplicate = original.Duplicate();

      Duplicates.AreEqual(original, duplicate);
    }

    [Fact]
    public void GsaGridPlaneSurfaceEqualsTest()
    {
      GsaGridPlaneSurface original = new GsaGridPlaneSurface();
      GsaGridPlaneSurface duplicate = original.Duplicate();

      Duplicates.AreEqual(original, duplicate);
    }

    [Fact]
    public void GsaLoadEqualsTest()
    {
      GsaLoad original = new GsaLoad(new GsaBeamLoad());
      GsaLoad duplicate = original.Duplicate();

      Duplicates.AreEqual(original, duplicate);
    }

  }
}
