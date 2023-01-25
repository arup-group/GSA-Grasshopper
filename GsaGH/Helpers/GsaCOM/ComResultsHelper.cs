using GsaAPI;
using GsaGH.Parameters;
using OasysUnits;
using OasysUnits.Units;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GsaGH.Parameters.GsaResult;

namespace GsaGH.Helpers.GsaAPI
{
  internal enum FootfallResultType
  {
    Resonant,
    Transient
  }
  internal partial class ResultHelper
  {
    internal static GsaResultsValues GetNodeFootfallResultValues(string nodelist, GsaModel model, FootfallResultType type, int caseId)
    {
      if (model == null) { return null; }

      Interop.Gsa_10_1.ComAuto GSA = GsaComHelper.GetGsaComModel(model);

      ReadOnlyDictionary<int, Node> nodes = model.Model.Nodes(nodelist);

      int dataRef = 12009001;
      if (type == FootfallResultType.Transient)
        dataRef = 12009101;

      string aCase = "A";

      var check = GSA.Output_Init(0, "default", aCase + caseId, dataRef, 0);

      GsaResultsValues r = new GsaResultsValues();
      r.Type = GsaResultsValues.ResultType.Footfall;

      foreach (int nodeID in nodes.Keys)
      {
        ConcurrentDictionary<int, GsaResultQuantity> xyzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
        Ratio ff = new Ratio((double)GSA.Output_Extract(nodeID, 0), RatioUnit.DecimalFraction);
        GsaResultQuantity res = new GsaResultQuantity()
        {
          X = ff
        };
        xyzRes.TryAdd(0, res);
        r.xyzResults.TryAdd(nodeID, xyzRes);
      }
      r.UpdateMinMax();

      return r;
    }

    internal static GsaResultsValues GetElement1DFootfallResultValues(string elemList, GsaModel model, GsaResultsValues nodeFootfallResultValues)
    {
      return GetElementFootfallResults(elemList, model, nodeFootfallResultValues, ElementDimension._1D);
    }

    internal static GsaResultsValues GetElement2DFootfallResultValues(string elemList, GsaModel model, GsaResultsValues nodeFootfallResultValues)
    {
      return GetElementFootfallResults(elemList, model, nodeFootfallResultValues, ElementDimension._2D);
    }
    private enum ElementDimension
    {
      _1D,
      _2D
    }
    private static GsaResultsValues GetElementFootfallResults(string elemList, GsaModel model, GsaResultsValues nodeFootfallResultValues, ElementDimension typ)
    {
      if (model == null) { return null; }

      ReadOnlyDictionary<int, Element> elements = model.Model.Elements(elemList);

      GsaResultsValues r = new GsaResultsValues();
      r.Type = GsaResultsValues.ResultType.Footfall;

      foreach (int elemID in elements.Keys)
      {
        int startint = 0;
        if (typ == ElementDimension._1D)
        {
          if (elements[elemID].Topology.Count > 2)
            continue;
        }
        else
        {
          startint = 1;
          if (elements[elemID].Topology.Count < 3)
            continue;
        }

        ConcurrentDictionary<int, GsaResultQuantity> xyzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
        for (int i = startint; i < elements[elemID].Topology.Count; i++)
        {
          int nodeID = elements[elemID].Topology[i];
          GsaResultQuantity nodeResult = nodeFootfallResultValues.xyzResults[nodeID][0];
          xyzRes.TryAdd(i, nodeResult);
        }
        if (typ == ElementDimension._2D)
        {
          int nodeID = elements[elemID].Topology[0];
          GsaResultQuantity nodeResult = nodeFootfallResultValues.xyzResults[nodeID][0];
          xyzRes.TryAdd(elements[elemID].Topology.Count, nodeResult);// add centre point last
        }
        r.xyzResults.TryAdd(elemID, xyzRes);
      }
      r.UpdateMinMax();

      return r;
    }
  }
}
