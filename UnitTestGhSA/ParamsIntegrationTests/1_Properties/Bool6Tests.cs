using System;
using NUnit.Framework;
using GsaGH;
using GsaGH.Parameters;
using Rhino.Geometry;
using GsaAPI;

namespace ParamsIntegrationTests
{
  public class Bool6Tests
  {
    [TestCase]
    public void TestCreateBool6()
    {
      // create new bool6
      GsaBool6 b6 = new GsaBool6();
      b6.X = true;
      b6.Y = true;
      b6.Z = true;
      b6.XX = true;
      b6.YY = true;
      b6.ZZ = true;

      Assert.IsTrue(b6.X);
      Assert.IsTrue(b6.Y);
      Assert.IsTrue(b6.Z);
      Assert.IsTrue(b6.XX);
      Assert.IsTrue(b6.YY);
      Assert.IsTrue(b6.ZZ);

      b6.X = false;
      b6.Y = false;
      b6.Z = false;
      b6.XX = false;
      b6.YY = false;
      b6.ZZ = false;

      Assert.IsFalse(b6.X);
      Assert.IsFalse(b6.Y);
      Assert.IsFalse(b6.Z);
      Assert.IsFalse(b6.XX);
      Assert.IsFalse(b6.YY);
      Assert.IsFalse(b6.ZZ);
    }

    [TestCase]
    public void TestDuplicateBool6()
    {
      // create new bool6
      GsaBool6 origB6 = new GsaBool6();
      origB6.X = true;
      origB6.Y = false;
      origB6.Z = true;
      origB6.XX = false;
      origB6.YY = true;
      origB6.ZZ = false;

      // duplicate
      GsaBool6 dup = origB6.Duplicate();

      // make some changes to original
      origB6.X = false;
      origB6.Y = true;
      origB6.Z = false;
      origB6.XX = true;
      origB6.YY = false;
      origB6.ZZ = true;

      Assert.IsTrue(dup.X);
      Assert.IsFalse(dup.Y);
      Assert.IsTrue(dup.Z);
      Assert.IsFalse(dup.XX);
      Assert.IsTrue(dup.YY);
      Assert.IsFalse(dup.ZZ);

      Assert.IsFalse(origB6.X);
      Assert.IsTrue(origB6.Y);
      Assert.IsFalse(origB6.Z);
      Assert.IsTrue(origB6.XX);
      Assert.IsFalse(origB6.YY);
      Assert.IsTrue(origB6.ZZ);
    }
  }
}