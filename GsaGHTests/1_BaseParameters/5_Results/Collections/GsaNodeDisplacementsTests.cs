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
using Rhino.Commands;
using Xunit;
using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGHTests.Parameters.Results {
  [Collection("GrasshopperFixture collection")]
  public partial class GsaNodeDisplacementsTests {
    public enum GsaNodeDisplacementComponent {
      X, 
      Y, 
      Z, 
      Xyz, 
      Xx, 
      Yy, 
      Zz, 
      Xxyyzz
    }

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
    public void GsaNodeDisplacementsNodeIdsFromcombinationCaseTest() {
      // Assemble
      var result = (GsaResult2)GsaResult2Tests.CombinationCaseResult(GsaFile.SteelDesignComplex, 4);

      // Act
      GsaNodeDisplacements resultSet = result.NodeDisplacementValues(NodeList);

      // Assert node IDs
      var expectedIds = result.Model.Model.Nodes(NodeList).Keys.ToList();
      Assert.Equal(expectedIds, resultSet.Ids);
    }

    [Theory]
    [InlineData(GsaNodeDisplacementComponent.X)]
    [InlineData(GsaNodeDisplacementComponent.Y)]
    [InlineData(GsaNodeDisplacementComponent.Z)]
    [InlineData(GsaNodeDisplacementComponent.Xyz)]
    [InlineData(GsaNodeDisplacementComponent.Xx)]
    [InlineData(GsaNodeDisplacementComponent.Yy)]
    [InlineData(GsaNodeDisplacementComponent.Zz)]
    [InlineData(GsaNodeDisplacementComponent.Xxyyzz)]
    public void GsaNodeDisplacementsMaxFromAnalysisCaseTest(GsaNodeDisplacementComponent component) {
      // Assemble
      var result = (GsaResult2)GsaResult2Tests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);
      double expected = ExpectedAnalysisCaseValues(component).Max();

      // Act
      GsaNodeDisplacements resultSet = result.NodeDisplacementValues(NodeList);

      // Assert Max in set
      double max = ResultsHelper(resultSet.Max, component);
      Assert.Equal(expected, max);
    }

    [Theory]
    [InlineData(GsaNodeDisplacementComponent.X)]
    [InlineData(GsaNodeDisplacementComponent.Y)]
    [InlineData(GsaNodeDisplacementComponent.Z)]
    [InlineData(GsaNodeDisplacementComponent.Xyz)]
    [InlineData(GsaNodeDisplacementComponent.Xx)]
    [InlineData(GsaNodeDisplacementComponent.Yy)]
    [InlineData(GsaNodeDisplacementComponent.Zz)]
    [InlineData(GsaNodeDisplacementComponent.Xxyyzz)]
    public void GsaNodeDisplacementsMaxFromCombinationCaseTest(GsaNodeDisplacementComponent component) {
      // Assemble
      var result = (GsaResult2)GsaResult2Tests.CombinationCaseResult(GsaFile.SteelDesignComplex, 4);
      double expected = Math.Max(
        ExpectedCombinationCaseC4p1Values(component).Max(),
        ExpectedCombinationCaseC4p2Values(component).Max());

      // Act
      GsaNodeDisplacements resultSet = result.NodeDisplacementValues(NodeList);

      // Assert Max in set
      double max = ResultsHelper(resultSet.Max, component);
      Assert.Equal(expected, max);
    }

    [Theory]
    [InlineData(GsaNodeDisplacementComponent.X)]
    [InlineData(GsaNodeDisplacementComponent.Y)]
    [InlineData(GsaNodeDisplacementComponent.Z)]
    [InlineData(GsaNodeDisplacementComponent.Xyz)]
    [InlineData(GsaNodeDisplacementComponent.Xx)]
    [InlineData(GsaNodeDisplacementComponent.Yy)]
    [InlineData(GsaNodeDisplacementComponent.Zz)]
    [InlineData(GsaNodeDisplacementComponent.Xxyyzz)]
    public void GsaNodeDisplacementsMinFromAnalysisCaseTest(GsaNodeDisplacementComponent component) {
      // Assemble
      var result = (GsaResult2)GsaResult2Tests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);
      double expected = ExpectedAnalysisCaseValues(component).Min();

      // Act
      GsaNodeDisplacements resultSet = result.NodeDisplacementValues(NodeList);

      // Assert Max in set
      double min = ResultsHelper(resultSet.Min, component);
      Assert.Equal(expected, min);
    }

    [Theory]
    [InlineData(GsaNodeDisplacementComponent.X)]
    [InlineData(GsaNodeDisplacementComponent.Y)]
    [InlineData(GsaNodeDisplacementComponent.Z)]
    [InlineData(GsaNodeDisplacementComponent.Xyz)]
    [InlineData(GsaNodeDisplacementComponent.Xx)]
    [InlineData(GsaNodeDisplacementComponent.Yy)]
    [InlineData(GsaNodeDisplacementComponent.Zz)]
    [InlineData(GsaNodeDisplacementComponent.Xxyyzz)]
    public void GsaNodeDisplacementsMinFromcombinationCaseTest(GsaNodeDisplacementComponent component) {
      // Assemble
      var result = (GsaResult2)GsaResult2Tests.CombinationCaseResult(GsaFile.SteelDesignComplex, 4);
      double expected = Math.Min(
        ExpectedCombinationCaseC4p1Values(component).Min(),
        ExpectedCombinationCaseC4p2Values(component).Min());

      // Act
      GsaNodeDisplacements resultSet = result.NodeDisplacementValues(NodeList);

      // Assert Max in set
      double min = ResultsHelper(resultSet.Min, component);
      Assert.Equal(expected, min);
    }

    [Theory]
    [InlineData(GsaNodeDisplacementComponent.X)]
    [InlineData(GsaNodeDisplacementComponent.Y)]
    [InlineData(GsaNodeDisplacementComponent.Z)]
    [InlineData(GsaNodeDisplacementComponent.Xyz)]
    [InlineData(GsaNodeDisplacementComponent.Xx)]
    [InlineData(GsaNodeDisplacementComponent.Yy)]
    [InlineData(GsaNodeDisplacementComponent.Zz)]
    [InlineData(GsaNodeDisplacementComponent.Xxyyzz)]
    public void GsaNodeDisplacementsValuesFromAnalysisCaseTest(GsaNodeDisplacementComponent component) {
      // Assemble
      var result = (GsaResult2)GsaResult2Tests.AnalysisCaseResult(GsaFile.SteelDesignComplex, 1);
      List<double> expected = ExpectedAnalysisCaseValues(component);

      // Act
      GsaNodeDisplacements resultSet = result.NodeDisplacementValues(NodeList);

      // Assert result values
      int i = 0;
      foreach (int id in resultSet.Ids) {
        Collection<IDisplacement> displacementQuantity = resultSet.Results[id];
        
        // for analysis case results we expect only one value in the collection
        Assert.Single(displacementQuantity);

        double x = ResultsHelper(displacementQuantity[0], component);
        Assert.Equal(expected[i++], x);
      }
    }

    [Theory]
    [InlineData(GsaNodeDisplacementComponent.X)]
    [InlineData(GsaNodeDisplacementComponent.Y)]
    [InlineData(GsaNodeDisplacementComponent.Z)]
    [InlineData(GsaNodeDisplacementComponent.Xyz)]
    [InlineData(GsaNodeDisplacementComponent.Xx)]
    [InlineData(GsaNodeDisplacementComponent.Yy)]
    [InlineData(GsaNodeDisplacementComponent.Zz)]
    [InlineData(GsaNodeDisplacementComponent.Xxyyzz)]
    public void GsaNodeDisplacementsValuesFromCombinationCaseTest(GsaNodeDisplacementComponent component) {
      // Assemble
      var result = (GsaResult2)GsaResult2Tests.CombinationCaseResult(GsaFile.SteelDesignComplex, 4);
      List<double> expectedP1 = ExpectedCombinationCaseC4p1Values(component);
      List<double> expectedP2 = ExpectedCombinationCaseC4p2Values(component);

      // Act
      GsaNodeDisplacements resultSet = result.NodeDisplacementValues(NodeList);

      // Assert result values
      int i = 0;
      foreach (int id in resultSet.Ids) {
        Collection<IDisplacement> displacementQuantity = resultSet.Results[id];

        // for C4 case results we expect two permutations in the collection
        Assert.Equal(2, displacementQuantity.Count);

        double perm1 = ResultsHelper(displacementQuantity[0], component);
        Assert.Equal(expectedP1[i], perm1);
        double perm2 = ResultsHelper(displacementQuantity[1], component);
        Assert.Equal(expectedP2[i++], perm2);
      }
    }

    private List<double> ExpectedAnalysisCaseValues(GsaNodeDisplacementComponent component) {
      switch (component) {
        case GsaNodeDisplacementComponent.X:
          return NodeDisplacementsA1.XInMillimeter();

        case GsaNodeDisplacementComponent.Y:
          return NodeDisplacementsA1.YInMillimeter();

        case GsaNodeDisplacementComponent.Z:
          return NodeDisplacementsA1.ZInMillimeter();

        case GsaNodeDisplacementComponent.Xyz:
          return NodeDisplacementsA1.XyzInMillimeter();

        case GsaNodeDisplacementComponent.Xx:
          return NodeDisplacementsA1.XxInRadian();

        case GsaNodeDisplacementComponent.Yy:
          return NodeDisplacementsA1.YyInRadian();

        case GsaNodeDisplacementComponent.Zz:
          return NodeDisplacementsA1.ZzInRadian();

        case GsaNodeDisplacementComponent.Xxyyzz:
          return NodeDisplacementsA1.XxyyzzInRadian();
      }
      throw new NotImplementedException();
    }

    private List<double> ExpectedCombinationCaseC4p1Values(GsaNodeDisplacementComponent component) {
      switch (component) {
        case GsaNodeDisplacementComponent.X:
          return NodeDisplacementsC4p1.XInMillimeter();

        case GsaNodeDisplacementComponent.Y:
          return NodeDisplacementsC4p1.YInMillimeter();

        case GsaNodeDisplacementComponent.Z:
          return NodeDisplacementsC4p1.ZInMillimeter();

        case GsaNodeDisplacementComponent.Xyz:
          return NodeDisplacementsC4p1.XyzInMillimeter();

        case GsaNodeDisplacementComponent.Xx:
          return NodeDisplacementsC4p1.XxInRadian();

        case GsaNodeDisplacementComponent.Yy:
          return NodeDisplacementsC4p1.YyInRadian();

        case GsaNodeDisplacementComponent.Zz:
          return NodeDisplacementsC4p1.ZzInRadian();

        case GsaNodeDisplacementComponent.Xxyyzz:
          return NodeDisplacementsC4p1.XxyyzzInRadian();
      }
      throw new NotImplementedException();
    }

    private List<double> ExpectedCombinationCaseC4p2Values(GsaNodeDisplacementComponent component) {
      switch (component) {
        case GsaNodeDisplacementComponent.X:
          return NodeDisplacementsC4p2.XInMillimeter();

        case GsaNodeDisplacementComponent.Y:
          return NodeDisplacementsC4p2.YInMillimeter();

        case GsaNodeDisplacementComponent.Z:
          return NodeDisplacementsC4p2.ZInMillimeter();

        case GsaNodeDisplacementComponent.Xyz:
          return NodeDisplacementsC4p2.XyzInMillimeter();

        case GsaNodeDisplacementComponent.Xx:
          return NodeDisplacementsC4p2.XxInRadian();

        case GsaNodeDisplacementComponent.Yy:
          return NodeDisplacementsC4p2.YyInRadian();

        case GsaNodeDisplacementComponent.Zz:
          return NodeDisplacementsC4p2.ZzInRadian();

        case GsaNodeDisplacementComponent.Xxyyzz:
          return NodeDisplacementsC4p2.XxyyzzInRadian();
      }
      throw new NotImplementedException();
    }

    private double ResultsHelper(IDisplacement result, GsaNodeDisplacementComponent component) {
      double d = 0;
      switch (component) {
        case GsaNodeDisplacementComponent.X:
          d = result.X.Millimeters;
          break;

        case GsaNodeDisplacementComponent.Y:
          d = result.Y.Millimeters;
          break;

        case GsaNodeDisplacementComponent.Z:
          d = result.Z.Millimeters;
          break;

        case GsaNodeDisplacementComponent.Xyz:
          d = result.Xyz.Millimeters;
          break;

        case GsaNodeDisplacementComponent.Xx:
          d = result.Xx.Radians;
          break;

        case GsaNodeDisplacementComponent.Yy:
          d = result.Yy.Radians;
          break;

        case GsaNodeDisplacementComponent.Zz:
          d = result.Zz.Radians;
          break;

        case GsaNodeDisplacementComponent.Xxyyzz:
          d = result.Xxyyzz.Radians;
          break;
      }

      return ResultHelper.RoundToSignificantDigits(d, 4);
    }

    private static readonly string NodeList = "442 to 468";
  }
}
