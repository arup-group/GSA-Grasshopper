﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace GsaGH.Parameters.Results {
  public abstract class Entity2dTri6Result<ApiResultType, QuantityResult> : IEntity2dTri6Quantity<QuantityResult>
    where QuantityResult : IResultItem {
    public QuantityResult Node1 { get; private set; }
    public QuantityResult Node2 { get; private set; }
    public QuantityResult Node3 { get; private set; }
    public QuantityResult Node4 { get; private set; }
    public QuantityResult Node5 { get; private set; }
    public QuantityResult Node6 { get; private set; }
    public QuantityResult Centre { get; private set; }

    internal Entity2dTri6Result(
      ReadOnlyCollection<ApiResultType> apiResult, Func<ApiResultType, QuantityResult> constructor) {
      int i = 0;
      Centre = constructor(apiResult[i++]);
      Node1 = constructor(apiResult[i++]);
      Node2 = constructor(apiResult[i++]);
      Node3 = constructor(apiResult[i++]);
      Node4 = constructor(apiResult[i++]);
      Node5 = constructor(apiResult[i++]);
      Node6 = constructor(apiResult[i++]);
    }

    public IList<QuantityResult> Results() {
      return new List<QuantityResult>() {
        Node1,
        Node2,
        Node3,
        Node4,
        Node5,
        Node6,
        Centre
      };
    }
  }
}
