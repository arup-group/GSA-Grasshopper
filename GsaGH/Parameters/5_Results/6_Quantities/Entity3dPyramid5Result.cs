using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using GsaAPI;

namespace GsaGH.Parameters.Results {
  public abstract class Entity3dPyramid5Result<ApiResultType, QuantityResult> : IEntity3dPyramid5Quantity<QuantityResult>
    where QuantityResult : IResultItem {
    public QuantityResult Node1 { get; private set; }
    public QuantityResult Node2 { get; private set; }
    public QuantityResult Node3 { get; private set; }
    public QuantityResult Node4 { get; private set; }
    public QuantityResult Node5 { get; private set; }
    public QuantityResult Centre { get; private set; }
    public QuantityResult Face1Centre { get; private set; }
    public QuantityResult Face2Centre { get; private set; }
    public QuantityResult Face3Centre { get; private set; }
    public QuantityResult Face4Centre { get; private set; }
    public QuantityResult Face5Centre { get; private set; }

    internal Entity3dPyramid5Result(ReadOnlyCollection<ApiResultType> apiResult, Func<ApiResultType, QuantityResult> constructor) {
      int i = 0;
      Centre = constructor(apiResult[i++]);
      Node1 = constructor(apiResult[i++]);
      Node2 = constructor(apiResult[i++]);
      Node3 = constructor(apiResult[i++]);
      Node4 = constructor(apiResult[i++]);
      Node5 = constructor(apiResult[i++]);
      Face1Centre = constructor(apiResult[i++]);
      Face2Centre = constructor(apiResult[i++]);
      Face3Centre = constructor(apiResult[i++]);
      Face4Centre = constructor(apiResult[i++]);
      Face5Centre = constructor(apiResult[i++]);
    }

    public IList<QuantityResult> Results() {
      return new List<QuantityResult>() {
        Node1,
        Node2,
        Node3,
        Node4,
        Node5,
        Centre
      };
    }
  }
}
