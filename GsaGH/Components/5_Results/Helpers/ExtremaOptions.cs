using GsaGH.Parameters.Results;
using System.Collections.ObjectModel;

namespace GsaGH.Components.Helpers {
  internal class ExtremaOptions {
    internal static readonly ReadOnlyCollection<string> Vector6 = new ReadOnlyCollection<string>(new[] {
      "All",
      "Max Ux",
      "Max Uy",
      "Max Uz",
      "Max |U|",
      "Max Rxx",
      "Max Ryy",
      "Max Rzz",
      "Max |R|",
      "Min Ux",
      "Min Uy",
      "Min Uz",
      "Min |U|",
      "Min Rxx",
      "Min Ryy",
      "Min Rzz",
      "Min |R|",
    });

    internal static NodeExtremaKey NodeExtremaKey<T>(INodeResultSubset<T, ResultVector6<NodeExtremaKey>> resultSet, string key)
      where T : IResultItem {
      return key switch {
        "Max Ux" => resultSet.Max.X,
        "Max Uy" => resultSet.Max.Y,
        "Max Uz" => resultSet.Max.Z,
        "Max |U|" => resultSet.Max.Xyz,
        "Max Rxx" => resultSet.Max.Xx,
        "Max Ryy" => resultSet.Max.Yy,
        "Max Rzz" => resultSet.Max.Zz,
        "Max |R|" => resultSet.Max.Xxyyzz,
        "Min Ux" => resultSet.Min.X,
        "Min Uy" => resultSet.Min.Y,
        "Min Uz" => resultSet.Min.Z,
        "Min |U|" => resultSet.Min.Xyz,
        "Min Rxx" => resultSet.Min.Xx,
        "Min Ryy" => resultSet.Min.Yy,
        "Min Rzz" => resultSet.Min.Zz,
        "Min |R|" => resultSet.Min.Xxyyzz,
        _ => throw new System.ArgumentException("Extrema case not found"),
      };
    }
  }
}
