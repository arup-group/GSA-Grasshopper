using System.Collections.Generic;
using OasysUnits;

namespace GsaGH.Parameters.Results {
  public static partial class ExtremaKeyUtility {
    public static (ResultVector6<Entity1dExtremaKey> Max, ResultVector6<Entity1dExtremaKey> Min) GetResultVector6AssemblyExtremaKeys<T, U>(
      this IDictionary<int, IList<T>> subset)
      where T : IAssemblyQuantity<U> where U : IResultItem {

      var maxValue = new ResultVector6<double?>(double.MinValue);
      var minValue = new ResultVector6<double?>(double.MaxValue);

      var maxKey = new ResultVector6<Entity1dExtremaKey>();
      var minKey = new ResultVector6<Entity1dExtremaKey>();

      foreach (int elementId in subset.Keys) {
        IList<T> values = subset[elementId];
        for (int permutation = 0; permutation < values.Count; permutation++) {
          foreach (double position in values[permutation].Results.Keys) {
            switch (values[permutation]) {
              case IAssemblyQuantity<IDisplacement> displacement:
                UpdateExtrema<IDisplacement, Length, Angle>(displacement.Results[position],
                  elementId, permutation, position,
                  ref maxValue, ref minValue, ref maxKey, ref minKey);
                break;

              case IAssemblyQuantity<IInternalForce> force:
                UpdateExtrema<IInternalForce, Force, Moment>(force.Results[position],
                  elementId, permutation, position,
                  ref maxValue, ref minValue, ref maxKey, ref minKey);
                break;

            }
          }
        }
      }

      return (maxKey, minKey);
    }
  }
}