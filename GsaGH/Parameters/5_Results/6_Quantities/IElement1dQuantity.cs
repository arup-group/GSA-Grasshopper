﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using GsaAPI;

namespace GsaGH.Parameters.Results {
  public interface IElement1dQuantity<T> where T : IResultItem {
    IDictionary<double, T> Results { get; }
  }
}
