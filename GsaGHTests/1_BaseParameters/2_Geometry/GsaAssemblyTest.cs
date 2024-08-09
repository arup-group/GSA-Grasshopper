using System.Collections.Generic;
using System.Linq;

using GsaAPI;

using GsaGH.Parameters;

using GsaGHTests.Helpers;

using Xunit;

namespace GsaGHTests.Parameters {
  [Collection("GrasshopperFixture collection")]
  public class GsaAssemblyTest {
    [Fact]
    public void DuplicateByExplicitPositionsTest() {
      var original = new GsaAssembly {
        ApiAssembly = new AssemblyByExplicitPositions("Explicit positions", 1, 2, 3, new List<int>() { 4, 5 }, CurveFit.LagrangeInterpolation) {
          Positions = new SortedSet<double>() { 6, 7 }
        }
      };

      var duplicate = new GsaAssembly(original);

      Duplicates.AreEqual(original, duplicate, new List<string>() { "Guid" });

      duplicate.ApiAssembly = new AssemblyByExplicitPositions("", 0, 0, 0, new List<int>() { }, CurveFit.CircularArc);

      Assert.Equal(0, original.Id);
      Assert.Equal("Explicit positions", original.ApiAssembly.Name);
      Assert.Equal(1, original.ApiAssembly.Topology1);
      Assert.Equal(2, original.ApiAssembly.Topology2);
      Assert.Equal(3, original.ApiAssembly.OrientationNode);
      Assert.Equal(2, ((AssemblyByExplicitPositions)original.ApiAssembly).InternalTopology.Count);
      Assert.Equal(4, ((AssemblyByExplicitPositions)original.ApiAssembly).InternalTopology[0]);
      Assert.Equal(5, ((AssemblyByExplicitPositions)original.ApiAssembly).InternalTopology[1]);
      Assert.Equal(CurveFit.LagrangeInterpolation, ((AssemblyByExplicitPositions)original.ApiAssembly).CurveFit);
      Assert.Equal(2, ((AssemblyByExplicitPositions)original.ApiAssembly).Positions.Count);
      Assert.Equal(6, ((AssemblyByExplicitPositions)original.ApiAssembly).Positions.First());
      Assert.Equal(7, ((AssemblyByExplicitPositions)original.ApiAssembly).Positions.Last());
    }

    [Fact]
    public void DuplicateByNumberOfPointsTest() {
      var original = new GsaAssembly {
        ApiAssembly = new AssemblyByNumberOfPoints("Number of points", 1, 2, 3, new List<int>() { 4, 5 }, CurveFit.CircularArc) {
          NumberOfPoints = 10
        }
      };

      var duplicate = new GsaAssembly(original);

      Duplicates.AreEqual(original, duplicate, new List<string>() { "Guid" });

      duplicate.ApiAssembly = new AssemblyByNumberOfPoints("", 0, 0, 0, new List<int>() { }, CurveFit.CircularArc);

      Assert.Equal(0, original.Id);
      Assert.Equal("Number of points", original.ApiAssembly.Name);
      Assert.Equal(1, original.ApiAssembly.Topology1);
      Assert.Equal(2, original.ApiAssembly.Topology2);
      Assert.Equal(3, original.ApiAssembly.OrientationNode);
      Assert.Equal(2, ((AssemblyByNumberOfPoints)original.ApiAssembly).InternalTopology.Count);
      Assert.Equal(4, ((AssemblyByNumberOfPoints)original.ApiAssembly).InternalTopology[0]);
      Assert.Equal(5, ((AssemblyByNumberOfPoints)original.ApiAssembly).InternalTopology[1]);
      Assert.Equal(CurveFit.CircularArc, ((AssemblyByNumberOfPoints)original.ApiAssembly).CurveFit);
      Assert.Equal(10, ((AssemblyByNumberOfPoints)original.ApiAssembly).NumberOfPoints);
    }

    [Fact]
    public void DuplicateBySpacingOfPointsTest() {
      var original = new GsaAssembly {
        ApiAssembly = new AssemblyBySpacingOfPoints("Number of points", 1, 2, 3, new List<int>() { 4, 5 }, CurveFit.CircularArc) {
          Spacing = 7.0
        }
      };

      var duplicate = new GsaAssembly(original);

      Duplicates.AreEqual(original, duplicate, new List<string>() { "Guid" });

      duplicate.ApiAssembly = new AssemblyBySpacingOfPoints("", 0, 0, 0, new List<int>() { }, CurveFit.CircularArc);

      Assert.Equal(0, original.Id);
      Assert.Equal("Number of points", original.ApiAssembly.Name);
      Assert.Equal(1, original.ApiAssembly.Topology1);
      Assert.Equal(2, original.ApiAssembly.Topology2);
      Assert.Equal(3, original.ApiAssembly.OrientationNode);
      Assert.Equal(2, ((AssemblyBySpacingOfPoints)original.ApiAssembly).InternalTopology.Count);
      Assert.Equal(4, ((AssemblyBySpacingOfPoints)original.ApiAssembly).InternalTopology[0]);
      Assert.Equal(5, ((AssemblyBySpacingOfPoints)original.ApiAssembly).InternalTopology[1]);
      Assert.Equal(CurveFit.CircularArc, ((AssemblyBySpacingOfPoints)original.ApiAssembly).CurveFit);
      Assert.Equal(7.0, ((AssemblyBySpacingOfPoints)original.ApiAssembly).Spacing);
    }

    [Fact]
    public void DuplicateByStoreyTest() {
      var original = new GsaAssembly {
        ApiAssembly = new AssemblyByStorey("Number of points", 1, 2, 3) {
          StoreyList = "all"
        }
      };

      var duplicate = new GsaAssembly(original);

      Duplicates.AreEqual(original, duplicate, new List<string>() { "Guid" });

      duplicate.ApiAssembly = new AssemblyByStorey("", 0, 0, 0);

      Assert.Equal(0, original.Id);
      Assert.Equal("Number of points", original.ApiAssembly.Name);
      Assert.Equal(1, original.ApiAssembly.Topology1);
      Assert.Equal(2, original.ApiAssembly.Topology2);
      Assert.Equal(3, original.ApiAssembly.OrientationNode);
      Assert.Equal("all", ((AssemblyByStorey)original.ApiAssembly).StoreyList);
    }
  }
}
