using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GsaGH.Parameters.Results;
using GsaGHTests.Helper;
using OasysUnits;
using Xunit;

namespace GsaGHTests.Parameters.Results {
  [Collection("GrasshopperFixture collection")]
  public class AssemblyDisplacementsTests {
    [Theory]
    [InlineData(ResultVector6.X)]
    [InlineData(ResultVector6.Y)]
    [InlineData(ResultVector6.Z)]
    [InlineData(ResultVector6.Xyz)]
    [InlineData(ResultVector6.Xx)]
    [InlineData(ResultVector6.Yy)]
    [InlineData(ResultVector6.Zz)]
    [InlineData(ResultVector6.Xxyyzz)]
    public void DisplacementsMaxFromAnalysisCaseTest(ResultVector6 component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.AssemblySimple, 1);
      double expected = ExpectedAnalysisCaseValues(component).Max();

      // Act
      AssemblyDisplacements resultSet = result.AssemblyDisplacements.ResultSubset(new Collection<int>() { 2 });

      // Assert 
      double max = TestsResultHelper.ResultsHelper(resultSet, component, true);
      Assert.Equal(expected, max, 1E-6);
    }

    [Theory]
    [InlineData(ResultVector6.X)]
    [InlineData(ResultVector6.Y)]
    [InlineData(ResultVector6.Z)]
    [InlineData(ResultVector6.Xyz)]
    [InlineData(ResultVector6.Xx)]
    [InlineData(ResultVector6.Yy)]
    [InlineData(ResultVector6.Zz)]
    [InlineData(ResultVector6.Xxyyzz)]
    public void AssemblyDisplacementsMaxFromCombinationCaseTest(ResultVector6 component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.AssemblySimple, 1);
      double expected = ExpectedCombinationCaseValues(component).Max();

      // Act
      AssemblyDisplacements resultSet = result.AssemblyDisplacements.ResultSubset(new Collection<int>() { 2 });

      // Assert
      double max = TestsResultHelper.ResultsHelper(resultSet, component, true);
      Assert.Equal(expected, max, 1E-6);
    }

    [Theory]
    [InlineData(ResultVector6.X)]
    [InlineData(ResultVector6.Y)]
    [InlineData(ResultVector6.Z)]
    [InlineData(ResultVector6.Xyz)]
    [InlineData(ResultVector6.Xx)]
    [InlineData(ResultVector6.Yy)]
    [InlineData(ResultVector6.Zz)]
    [InlineData(ResultVector6.Xxyyzz)]
    public void AssemblyDisplacementsMinFromAnalysisCaseTest(ResultVector6 component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.AssemblySimple, 1);
      double expected = ExpectedAnalysisCaseValues(component).Min();

      // Act
      AssemblyDisplacements resultSet = result.AssemblyDisplacements.ResultSubset(new Collection<int>() { 2 });

      // Assert
      double min = TestsResultHelper.ResultsHelper(resultSet, component, false);
      Assert.Equal(expected, min, 1E-6);
    }

    [Theory]
    [InlineData(ResultVector6.X)]
    [InlineData(ResultVector6.Y)]
    [InlineData(ResultVector6.Z)]
    [InlineData(ResultVector6.Xyz)]
    [InlineData(ResultVector6.Xx)]
    [InlineData(ResultVector6.Yy)]
    [InlineData(ResultVector6.Zz)]
    [InlineData(ResultVector6.Xxyyzz)]
    public void AssemblyDisplacementsMinFromcombinationCaseTest(ResultVector6 component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.AssemblySimple, 1);
      double expected = ExpectedCombinationCaseValues(component).Min();

      // Act
      AssemblyDisplacements resultSet = result.AssemblyDisplacements.ResultSubset(new Collection<int>() { 2 });

      // Assert
      double min = TestsResultHelper.ResultsHelper(resultSet, component, false);
      Assert.Equal(expected, min, 1E-6);
    }

    [Theory]
    [InlineData(ResultVector6.X)]
    [InlineData(ResultVector6.Y)]
    [InlineData(ResultVector6.Z)]
    [InlineData(ResultVector6.Xyz)]
    [InlineData(ResultVector6.Xx)]
    [InlineData(ResultVector6.Yy)]
    [InlineData(ResultVector6.Zz)]
    [InlineData(ResultVector6.Xxyyzz)]
    public void AssemblyDisplacementValuesFromAnalysisCaseTest(ResultVector6 component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.AssemblySimple, 1);
      List<double> expected = ExpectedAnalysisCaseValues(component);

      // Act
      AssemblyDisplacements resultSet = result.AssemblyDisplacements.ResultSubset(new Collection<int>() { 2 });

      // Assert
      IList<IAssemblyQuantity<IDisplacement>> displacementQuantity = resultSet.Subset[2];
      Assert.Single(displacementQuantity);

      int position = 0;
      foreach (IDisplacement displacement in displacementQuantity[0].Results.Values) {
        double x = TestsResultHelper.ResultsHelper(displacement, component);
        Assert.Equal(expected[position++], x, 1E-6);
      }
    }

    [Theory]
    [InlineData(ResultVector6.X)]
    [InlineData(ResultVector6.Y)]
    [InlineData(ResultVector6.Z)]
    [InlineData(ResultVector6.Xyz)]
    [InlineData(ResultVector6.Xx)]
    [InlineData(ResultVector6.Yy)]
    [InlineData(ResultVector6.Zz)]
    [InlineData(ResultVector6.Xxyyzz)]
    public void AssemblyDisplacementValuesFromCombinationCaseTest(ResultVector6 component) {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.AssemblySimple, 1);
      List<double> expected = ExpectedCombinationCaseValues(component);

      // Act
      AssemblyDisplacements resultSet = result.AssemblyDisplacements.ResultSubset(new Collection<int>() { 2 });

      // Assert
      IList<IAssemblyQuantity<IDisplacement>> displacementQuantity = resultSet.Subset[2];
      Assert.Single(displacementQuantity);

      int position = 0;
      foreach (IDisplacement displacement in displacementQuantity[0].Results.Values) {
        double x = TestsResultHelper.ResultsHelper(displacement, component);
        Assert.Equal(expected[position++], x, 1E-6);
      }
    }

    private List<double> ExpectedAnalysisCaseValues(ResultVector6 component) {
      switch (component) {
        case ResultVector6.X: return AssemblyDisplacementsA1.XInMillimeter();
        case ResultVector6.Y: return AssemblyDisplacementsA1.YInMillimeter();
        case ResultVector6.Z: return AssemblyDisplacementsA1.ZInMillimeter();
        case ResultVector6.Xyz: return AssemblyDisplacementsA1.XyzInMillimeter();
        case ResultVector6.Xx: return AssemblyDisplacementsA1.XxInRadian();
        case ResultVector6.Yy: return AssemblyDisplacementsA1.YyInRadian();
        case ResultVector6.Zz: return AssemblyDisplacementsA1.ZzInRadian();
        case ResultVector6.Xxyyzz: return AssemblyDisplacementsA1.XxyyzzInRadian();
      }

      throw new NotImplementedException();
    }

    private List<double> ExpectedCombinationCaseValues(ResultVector6 component) {
      switch (component) {
        //case ResultVector6.X: return Element1dDisplacementsC4p1.XInMillimeter();
        //case ResultVector6.Y: return Element1dDisplacementsC4p1.YInMillimeter();
        //case ResultVector6.Z: return Element1dDisplacementsC4p1.ZInMillimeter();
        //case ResultVector6.Xyz: return Element1dDisplacementsC4p1.XyzInMillimeter();
        //case ResultVector6.Xx: return Element1dDisplacementsC4p1.XxInRadian();
        //case ResultVector6.Yy: return Element1dDisplacementsC4p1.YyInRadian();
        //case ResultVector6.Zz: return Element1dDisplacementsC4p1.ZzInRadian();
        //case ResultVector6.Xxyyzz: return Element1dDisplacementsC4p1.XxyyzzInRadian();
      }

      throw new NotImplementedException();
    }
  }
}
