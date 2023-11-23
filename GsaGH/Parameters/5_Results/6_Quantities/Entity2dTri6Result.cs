﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using GsaAPI;

namespace GsaGH.Parameters.Results {
  public abstract class Entity2dTri6Result<T> : IEntity2dTri6Quantity<T>
    where T : IResultItem {
    public T Node1 { get; private set; }
    public T Node2 { get; private set; }
    public T Node3 { get; private set; }
    public T Node4 { get; private set; }
    public T Node5 { get; private set; }
    public T Node6 { get; private set; }
    public T Centre { get; private set; }

    internal Entity2dTri6Result(ReadOnlyCollection<Double6> apiResult, Func<Double6, T> constructor) {
      int i = 0;
      Centre = constructor(apiResult[i++]);
      Node1 = constructor(apiResult[i++]);
      Node2 = constructor(apiResult[i++]);
      Node3 = constructor(apiResult[i++]);
      Node4 = constructor(apiResult[i++]);
      Node5 = constructor(apiResult[i++]);
      Node6 = constructor(apiResult[i++]);
    }

    public IList<T> Results() {
      return new List<T>() {
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
