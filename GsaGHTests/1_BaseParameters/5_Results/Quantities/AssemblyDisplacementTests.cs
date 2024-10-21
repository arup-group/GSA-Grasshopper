using System;
using System.Collections.ObjectModel;
using System.Linq;

using GsaGH.Parameters.Results;

using GsaGHTests.Helper;

using Xunit;

namespace GsaGHTests.Parameters.Results {
  [Collection("GrasshopperFixture collection")]
  public partial class AssemblyDiscplacementTests {
    [Fact]
    public void TakePositionsThrowsExceptionTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.AssemblySimple, 1);

      // Act
      AssemblyDisplacements resultSet = result.AssemblyDisplacements.ResultSubset(new Collection<int>() { 2 });

      // Assert
      Assert.Throws<NotImplementedException>(() => resultSet.Subset.FirstOrDefault().Value.FirstOrDefault().TakePositions(null));
    }
  }
}
