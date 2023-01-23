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

      var check = GSA.Output_Init(0, "global", aCase + caseId, dataRef, 0);

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

    internal static GsaResultsValues GetElement1DFootfallResultValues(string elemList, int positionsCount, GsaModel model, FootfallResultType type, int caseId)
    {
      if (model == null) { return null; }

      Interop.Gsa_10_1.ComAuto GSA = GsaComHelper.GetGsaComModel(model);

      ReadOnlyDictionary<int, Element> elements = model.Model.Elements(elemList);

      int dataRef = 12509001;
      if (type == FootfallResultType.Transient)
        dataRef = 12509101;
      
      string aCase = "A";

      var check = GSA.Output_Init(0, "global", aCase + caseId, dataRef, positionsCount);

      GsaResultsValues r = new GsaResultsValues();
      r.Type = GsaResultsValues.ResultType.Footfall;

      foreach (int elemID in elements.Keys)
      {
        ConcurrentDictionary<int, GsaResultQuantity> xyzRes = new ConcurrentDictionary<int, GsaResultQuantity>();
        for (int i = 0; i < positionsCount; i++)
        {
          Ratio ff = new Ratio((double)GSA.Output_Extract(elemID, i), RatioUnit.DecimalFraction);
          GsaResultQuantity res = new GsaResultQuantity()
          {
            X = ff
          };
          xyzRes.TryAdd(i, res);
        }
        r.xyzResults.TryAdd(elemID, xyzRes);
      }
      r.UpdateMinMax();

      return r;
    }
  }
}
