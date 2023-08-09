using System;
using System.Collections.Concurrent;
using System.Linq;
using OasysUnits;

namespace GsaGH.Parameters {
  internal class GsaResultsValues {
    internal enum ResultType {
      Displacement,
      Force,
      Stress,
      Shear,
      StrainEnergy,
      Footfall,
    }

    internal IQuantity DmaxX { get; set; }
    internal IQuantity DmaxXx { get; set; }
    internal IQuantity DmaxXxyyzz { get; set; }
    internal IQuantity DmaxXyz { get; set; }
    internal IQuantity DmaxY { get; set; }
    internal IQuantity DmaxYy { get; set; }
    internal IQuantity DmaxZ { get; set; }
    internal IQuantity DmaxZz { get; set; }
    internal IQuantity DminX { get; set; }
    internal IQuantity DminXx { get; set; }
    internal IQuantity DminXxyyzz { get; set; }
    internal IQuantity DminXyz { get; set; }
    internal IQuantity DminY { get; set; }
    internal IQuantity DminYy { get; set; }
    internal IQuantity DminZ { get; set; }
    internal IQuantity DminZz { get; set; }
    internal ResultType Type { get; set; }
    internal ConcurrentDictionary<int, ConcurrentDictionary<int, GsaResultQuantity>>
      XxyyzzResults { get; set; }
      = new ConcurrentDictionary<int, ConcurrentDictionary<int, GsaResultQuantity>>();
    /// <summary>
    ///   Translation, forces, etc results
    ///   dictionary< key = node/ elementID, value= dictionary< key = position on element, value= value>>
    /// </summary>
    internal ConcurrentDictionary<int, ConcurrentDictionary<int, GsaResultQuantity>>
      XyzResults { get; set; }
      = new ConcurrentDictionary<int, ConcurrentDictionary<int, GsaResultQuantity>>();

    internal GsaResultsValues() { }

    internal void UpdateMinMax() {
      if (XyzResults.Count > 0) {
        DmaxX = XyzResults.AsParallel().Select(list => list.Value.Values.Select(res => res.X).Max())
         .Max();
        DmaxY = XyzResults.AsParallel().Select(list => list.Value.Values.Select(res => res.Y).Max())
         .Max();
        try {
          DmaxZ = XyzResults.AsParallel()
           .Select(list => list.Value.Values.Select(res => res.Z).Max()).Max();
        } catch (Exception) {
          // shear does not set this value
        }

        try {
          DmaxXyz = XyzResults.AsParallel()
           .Select(list => list.Value.Values.Select(res => res.Xyz).Max()).Max();
        } catch (Exception) {
          // resultant may not always be computed
        }

        DminX = XyzResults.AsParallel().Select(list => list.Value.Values.Select(res => res.X).Min())
         .Min();
        DminY = XyzResults.AsParallel().Select(list => list.Value.Values.Select(res => res.Y).Min())
         .Min();
        try {
          DminZ = XyzResults.AsParallel()
           .Select(list => list.Value.Values.Select(res => res.Z).Min()).Min();
        } catch (Exception) {
          // shear does not set this value
        }

        try {
          DminXyz = XyzResults.AsParallel()
           .Select(list => list.Value.Values.Select(res => res.Xyz).Min()).Min();
        } catch (Exception) {
          // resultant may not always be computed
        }
      }

      if (XxyyzzResults.Count <= 0) {
        return;
      }

      {
        try {
          DmaxXx = XxyyzzResults.AsParallel()
           .Select(list => list.Value.Values.Select(res => res.X).Max()).Max();
          DmaxYy = XxyyzzResults.AsParallel()
           .Select(list => list.Value.Values.Select(res => res.Y).Max()).Max();
          DmaxZz = XxyyzzResults.AsParallel()
           .Select(list => list.Value.Values.Select(res => res.Z).Max()).Max();
        } catch (Exception) {
          // some cases doesnt compute xxyyzz results at all
        }

        try {
          DmaxXxyyzz = XxyyzzResults.AsParallel()
           .Select(list => list.Value.Values.Select(res => res.Xyz).Max()).Max();
        } catch (Exception) {
          // resultant may not always be computed
        }

        try {
          DminXx = XxyyzzResults.AsParallel()
           .Select(list => list.Value.Values.Select(res => res.X).Min()).Min();
          DminYy = XxyyzzResults.AsParallel()
           .Select(list => list.Value.Values.Select(res => res.Y).Min()).Min();
          DminZz = XxyyzzResults.AsParallel()
           .Select(list => list.Value.Values.Select(res => res.Z).Min()).Min();
        } catch (Exception) {
          // some cases doesnt compute xxyyzz results at all
        }

        try {
          DminXxyyzz = XxyyzzResults.AsParallel()
           .Select(list => list.Value.Values.Select(res => res.Xyz).Min()).Min();
        } catch (Exception) {
          // resultant may not always be computed
        }
      }
    }
  }
}
