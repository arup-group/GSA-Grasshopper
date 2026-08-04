using System;
using System.Collections.ObjectModel;
using System.Linq;

using GsaAPI;

using GsaGH.Parameters.Results;

using GsaGHTests.Helper;

using OasysUnits;

using Xunit;

namespace GsaGHTests.Parameters.Results {
  [Collection("GrasshopperFixture collection")]
  public partial class AssemblyDriftIndexTests {
    [Fact]
    public void TakePositionsThrowsExceptionTest() {
      // Assemble
      var result = (GsaResult)GsaResultTests.AnalysisCaseResult(GsaFile.AssemblySimple, 1);

      // Act
      AssemblyDriftIndices resultSet = result.AssemblyDriftIndices.ResultSubset(new Collection<int>() { 2 });

      // Assert
      Assert.Throws<NotImplementedException>(() => resultSet.Subset.FirstOrDefault().Value.FirstOrDefault().TakePositions(null));
    }
  }
}
