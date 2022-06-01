using System;
using GsaGH;
using GsaGH.Parameters;
using GsaAPI;
using NUnit.Framework;
using Rhino.Geometry;
using UnitsNet;
using UnitsNet.Units;

namespace ParamsIntegrationTests
{
  public class OffsetTests
  {
    [TestCase]
    public void TestCreateOffset()
    {
      // create new offset
      GsaOffset offset = new GsaOffset();
      offset.X1 = new Length(1.57, LengthUnit.Meter);
      offset.X2 = new Length(-2.5, LengthUnit.Meter);
      offset.Y = new Length(4.2, LengthUnit.Meter);
      offset.Z = new Length(-10.5, LengthUnit.Meter);

      Assert.AreEqual(1.57, offset.X1);
      Assert.AreEqual(-2.5, offset.X2);
      Assert.AreEqual(4.2, offset.Y);
      Assert.AreEqual(-10.5, offset.Z);
    }

    [TestCase]
    public void TestDuplicateOffset()
    {
      // create new offset
      GsaOffset offset = new GsaOffset();
      offset.X1 = new Length(-1.57, LengthUnit.Meter);
      offset.X2 = new Length(2.5, LengthUnit.Meter);
      offset.Y = new Length(-4.2, LengthUnit.Meter);
      offset.Z = new Length(10.5, LengthUnit.Meter);

      // duplicate original
      GsaOffset dup = offset.Duplicate();

      // make some changes to original
      offset.X1 = new Length(-1000, LengthUnit.Meter);
      offset.X2 = new Length(0.0025, LengthUnit.Meter);
      offset.Y = new Length(42, LengthUnit.Meter);
      offset.Z = new Length(0, LengthUnit.Meter);

      Assert.AreEqual(-1.57, dup.X1);
      Assert.AreEqual(2.5, dup.X2);
      Assert.AreEqual(-4.2, dup.Y);
      Assert.AreEqual(10.5, dup.Z);

      Assert.AreEqual(-1000, offset.X1);
      Assert.AreEqual(0.0025, offset.X2);
      Assert.AreEqual(42, offset.Y);
      Assert.AreEqual(0, offset.Z);
    }
  }
}