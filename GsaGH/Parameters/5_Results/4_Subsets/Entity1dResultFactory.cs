using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace GsaGH.Parameters.Results {
  internal static partial class Entity1dResultsFactory {
    internal static IList<IEntity1dQuantity<QuantityResult>> CreateResults<ApiResultType, QuantityResult>(
      ReadOnlyCollection<ApiResultType> results, ReadOnlyCollection<double> positions,
        Func<ReadOnlyCollection<ApiResultType>, ReadOnlyCollection<double>,
        IEntity1dQuantity<QuantityResult>> constructor)
      where QuantityResult : IResultItem {
      return new List<IEntity1dQuantity<QuantityResult>> {
              constructor(results, positions)
             };
    }

    internal static IList<IEntity1dQuantity<QuantityResult>> CreateResults<ApiResultType, QuantityResult>(
      ReadOnlyCollection<ReadOnlyCollection<ApiResultType>> results, ReadOnlyCollection<double> positions,
        Func<ReadOnlyCollection<ApiResultType>, ReadOnlyCollection<double>,
        IEntity1dQuantity<QuantityResult>> constructor)
      where QuantityResult : IResultItem {
      var permutations = new List<IEntity1dQuantity<QuantityResult>>();
      foreach (ReadOnlyCollection<ApiResultType> permutation in results) {
        permutations.Add(constructor(permutation, positions));
      }
      return permutations;
    }

    internal static IList<IEntity1dQuantity<QuantityResult>> AddMissingPositions<ApiResultType, QuantityResult>(
      this IList<IEntity1dQuantity<QuantityResult>> existing,
      ReadOnlyCollection<ApiResultType> results, ReadOnlyCollection<double> positions,
      Func<ApiResultType, QuantityResult> constructor)
      where QuantityResult : IResultItem {
      for (int i = 0; i < positions.Count; i++) {
        if (!existing[0].Results.ContainsKey(positions[i])) {
          existing[0].Results.Add(positions[i], constructor(results[i]));
        }
      }

      return existing;
    }

    internal static IList<IEntity1dQuantity<QuantityResult>> AddMissingPositions<ApiResultType, QuantityResult>(
      this IList<IEntity1dQuantity<QuantityResult>> existing,
      ReadOnlyCollection<ReadOnlyCollection<ApiResultType>> results, ReadOnlyCollection<double> positions,
      Func<ApiResultType, QuantityResult> constructor)
      where QuantityResult : IResultItem {
      for (int i = 0; i < existing.Count; i++) {
        for (int j = 0; j < positions.Count; j++) {
          if (!existing[i].Results.ContainsKey(positions[j])) {
            existing[i].Results.Add(positions[j], constructor(results[i][j]));
          }
        }
      }

      return existing;
    }
  }
}
