using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using GsaAPI;

namespace GsaGH.Parameters.Results {
  public abstract class Entity2dTriResult<ApiResultType, QuantityResult> : IEntity2dTriQuantity<QuantityResult>
    where QuantityResult : IResultItem {
    public QuantityResult Node1 { get; private set; }
    public QuantityResult Node2 { get; private set; }
    public QuantityResult Node3 { get; private set; }
    public QuantityResult Centre { get; private set; }

    internal Entity2dTriResult(
      ReadOnlyCollection<ApiResultType> apiResult, Func<ApiResultType, QuantityResult> constructor) {
      int i = 0;
      Centre = constructor(apiResult[i++]);
      Node1 = constructor(apiResult[i++]);
      Node2 = constructor(apiResult[i++]);
      Node3 = constructor(apiResult[i++]);
    }

    public IList<QuantityResult> Results() {
      return new List<QuantityResult>() {
        Node1,
        Node2,
        Node3,
        Centre
      };
    }
  }
}
