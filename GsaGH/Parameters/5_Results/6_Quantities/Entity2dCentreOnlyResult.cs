using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using GsaAPI;

namespace GsaGH.Parameters.Results {
  public abstract class Entity2dCentreOnlyResult<ApiResultType, QuantityResult> : IEntity2dQuantity<QuantityResult>
    where QuantityResult : IResultItem {
    public QuantityResult Centre { get; private set; }

    internal Entity2dCentreOnlyResult(
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
