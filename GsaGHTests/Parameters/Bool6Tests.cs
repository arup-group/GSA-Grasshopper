//using GsaGH.Parameters;
//using Xunit;

//namespace ParamsIntegrationTests
//{
//  public class Bool6Tests
//  {
//    [Fact]
//    public void TestCreateBool6()
//    {
//      // create new bool6
//      GsaBool6 b6 = new GsaBool6();
//      b6.X = true;
//      b6.Y = true;
//      b6.Z = true;
//      b6.XX = true;
//      b6.YY = true;
//      b6.ZZ = true;

//      Assert.True(b6.X);
//      Assert.True(b6.Y);
//      Assert.True(b6.Z);
//      Assert.True(b6.XX);
//      Assert.True(b6.YY);
//      Assert.True(b6.ZZ);

//      b6.X = false;
//      b6.Y = false;
//      b6.Z = false;
//      b6.XX = false;
//      b6.YY = false;
//      b6.ZZ = false;

//      Assert.False(b6.X);
//      Assert.False(b6.Y);
//      Assert.False(b6.Z);
//      Assert.False(b6.XX);
//      Assert.False(b6.YY);
//      Assert.False(b6.ZZ);
//    }

//    [Fact]
//    public void TestDuplicateBool6()
//    {
//      // create new bool6
//      GsaBool6 origB6 = new GsaBool6();
//      origB6.X = true;
//      origB6.Y = false;
//      origB6.Z = true;
//      origB6.XX = false;
//      origB6.YY = true;
//      origB6.ZZ = false;

//      // duplicate
//      GsaBool6 dup = origB6.Duplicate();

//      // make some changes to original
//      origB6.X = false;
//      origB6.Y = true;
//      origB6.Z = false;
//      origB6.XX = true;
//      origB6.YY = false;
//      origB6.ZZ = true;

//      Assert.True(dup.X);
//      Assert.False(dup.Y);
//      Assert.True(dup.Z);
//      Assert.False(dup.XX);
//      Assert.True(dup.YY);
//      Assert.False(dup.ZZ);

//      Assert.False(origB6.X);
//      Assert.True(origB6.Y);
//      Assert.False(origB6.Z);
//      Assert.True(origB6.XX);
//      Assert.False(origB6.YY);
//      Assert.True(origB6.ZZ);
//    }
//  }
//}
