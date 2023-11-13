using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using GsaAPI;
using GsaGH.Parameters;
using Interop.Gsa_10_2;
using OasysUnits;
using OasysUnits.Units;

namespace GsaGH.Helpers.GsaApi {
  internal enum FootfallResultType {
    Resonant,
    Transient,
  }

  internal partial class ResultHelper {
    private enum ElementDimension {
      _1D,
      _2D,
    }

    internal static GsaResultsValues GetElement1DFootfallResultValues(
      string elemList, GsaModel model, GsaResultsValues nodeFootfallResultValues) {
      return GetElementFootfallResults(elemList, model, nodeFootfallResultValues,
        ElementDimension._1D);
    }

    internal static GsaResultsValues GetElement2DFootfallResultValues(
      string elemList, GsaModel model, GsaResultsValues nodeFootfallResultValues) {
      return GetElementFootfallResults(elemList, model, nodeFootfallResultValues,
        ElementDimension._2D);
    }

    internal static GsaResultsValues GetNodeFootfallResultValues(
      string nodelist, GsaModel model, FootfallResultType type, int caseId) {
      if (model == null) {
        return null;
      }

      ComAuto gsa = GsaComHelper.GetGsaComModel(model);

      ReadOnlyDictionary<int, Node> nodes = model.Model.Nodes(nodelist);

      int dataRef = 12009001;
      if (type == FootfallResultType.Transient) {
        dataRef = 12009101;
      }

      const string aCase = "A";

      gsa.Output_Init(0, "default", aCase + caseId, dataRef, 0);

      var r = new GsaResultsValues {
        Type = GsaResultsValues.ResultType.Footfall,
      };

      foreach (int nodeId in nodes.Keys) {
        var xyzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
        try {
          var ff = new Ratio((double)gsa.Output_Extract(nodeId, 0), RatioUnit.DecimalFraction);
          var res = new GsaResultQuantity() {
            X = ff,
          };
          xyzRes.TryAdd(0, res);
          r.XyzResults.TryAdd(nodeId, xyzRes);
        } catch (System.ArgumentException) {
          // skip
        }
      }

      r.UpdateMinMax();

      return r;
    }

    private static GsaResultsValues GetElementFootfallResults(
      string elemList, GsaModel model, GsaResultsValues nodeFootfallResultValues,
      ElementDimension typ) {
      if (model == null) {
        return null;
      }

      ReadOnlyDictionary<int, Element> elements = model.Model.Elements(elemList);

      var r = new GsaResultsValues {
        Type = GsaResultsValues.ResultType.Footfall,
      };

      foreach (int elemId in elements.Keys) {
        if (typ == ElementDimension._1D) {
          if (elements[elemId].Topology.Count > 2) {
            continue;
          }
        } else {
          if (elements[elemId].Topology.Count < 3) {
            continue;
          }
        }

        var xyzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
        for (int i = 0; i < elements[elemId].Topology.Count; i++) {
          int nodeId = elements[elemId].Topology[i];
          GsaResultQuantity nodeResult = nodeFootfallResultValues.XyzResults[nodeId][0];
          xyzRes.TryAdd(i, nodeResult);
        }

        if (typ == ElementDimension._2D) {
          var average = new Ratio(0, RatioUnit.DecimalFraction);
          foreach (int key in xyzRes.Keys) {
            average += (Ratio)xyzRes[key].X;
          }

          average /= elements[elemId].Topology.Count;
          xyzRes.TryAdd(elements[elemId].Topology.Count, new GsaResultQuantity() {
            X = average,
          });
        }

        r.XyzResults.TryAdd(elemId, xyzRes);
      }

      r.UpdateMinMax();

      return r;
    }
  }
}
