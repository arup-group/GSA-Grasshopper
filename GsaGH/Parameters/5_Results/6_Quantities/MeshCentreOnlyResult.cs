using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using GsaAPI;

namespace GsaGH.Parameters.Results {
  public abstract class MeshCentreOnlyResult<ApiResultType, QuantityResult> : IMeshQuantity<QuantityResult>
    where QuantityResult : IResultItem {
    public QuantityResult Centre { get; private set; }

    internal MeshCentreOnlyResult(
      ReadOnlyCollection<ApiResultType> apiResult, Func<ApiResultType, QuantityResult> constructor) {
      Centre = constructor(apiResult[0]);
    }

    public IList<QuantityResult> Results() {
      return new List<QuantityResult>() {
        Centre
      };
    }
  }
}
