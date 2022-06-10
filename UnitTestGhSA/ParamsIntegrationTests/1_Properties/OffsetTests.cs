using System;
using NUnit.Framework;
using GsaGH;
using GsaGH.Parameters;
using Rhino.Geometry;
using GsaAPI;

namespace ParamsIntegrationTests
{
  public class OffsetTests
  {
    [TestCase]
    public void TestCreateOffset()
    {
      // create new offset
      GsaOffset offset = new GsaOffset();
      offset.X1 = 1.57;
      offset.X2 = -2.5;
      offset.Y = 4.2;
      offset.Z = -10.5;

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
      offset.X1 = -1.57;
      offset.X2 = 2.5;
      offset.Y = -4.2;
      offset.Z = 10.5;

      // duplicate original
      GsaOffset dup = offset.Duplicate();

      // make some changes to original
      offset.X1 = -1000;
      offset.X2 = 0.0025;
      offset.Y = 42;
      offset.Z = 0;

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