using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GsaAPI;
using GsaGH.Components;
using GsaGH.Helpers.GsaApi;
using GsaGH.Parameters;
using GsaGH.Parameters.Results;
using GsaGHTests.Helper;
using OasysUnits;
using Xunit;
using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGHTests.Parameters.Results {
  [Collection("GrasshopperFixture collection")]
  public partial class GsaNodeDisplacementsTests {
    [Fact]
    public void GsaNodeDisplacementsNodeIdsFromAnalysisCaseTest() {
      // Assemble
      var result = (GsaResult2)GsaResult2Tests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);

      // Act
      GsaNodeDisplacements resultSet = result.NodeDisplacementValues(NodeList);

      // Assert node IDs
      var expectedIds = result.Model.Model.Nodes(NodeList).Keys.ToList();
      Assert.Equal(expectedIds, resultSet.Ids);
    }

    [Fact]
    public void GsaNodeDisplacementsXValuesFromAnalysisCaseTest() {
      // Assemble
      var result = (GsaResult2)GsaResult2Tests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);

      // Act
      GsaNodeDisplacements resultSet = result.NodeDisplacementValues(NodeList);

      // Assert result values
      List<double> expectedX = ExpectedDisplacementXInMillimeter();
      int i = 0;
      foreach (int id in resultSet.Ids) {
        Collection<IDisplacement> displacementQuantity = resultSet.Results[id];
        // for analysis case results we expect only one value in the collection
        Assert.Single(displacementQuantity);
        double x = ResultHelper.RoundToSignificantDigits(
          displacementQuantity[0].X.Millimeters, 4);
        Assert.Equal(expectedX[i++], x);
      }
    }

    [Fact]
    public void GsaNodeDisplacementsMaxMinXFromAnalysisCaseTest() {
      // Assemble
      var result = (GsaResult2)GsaResult2Tests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);

      // Act
      GsaNodeDisplacements resultSet = result.NodeDisplacementValues(NodeList);

      // Assert Max/Min in set
      Assert.Equal(6.426, resultSet.Max.X.Millimeters, 3);
      Assert.Equal(-0.1426, resultSet.Min.X.Millimeters, 3);
    }


    private static string NodeList = "442 to 468";
    // these are regression tests, the values are taken directly from GSA results
    // "Steel_Design_Complex.gwb"
    private static List<double> ExpectedDisplacementXInMillimeter() {
      return new List<double>() {
        1.108,
0.9107,
0.6290,
0.2078,
0.1886,
0.5299,
1.140,
1.923,
2.786,
3.646,
4.429,
5.087,
5.601,
5.973,
6.214,
6.349,
6.409,
6.426,
6.423,
6.419,
6.417,
0.3396,
0.2390,
0.01895,
-0.1426,
0.01656,
0.4770
      };
    }
  }
}
