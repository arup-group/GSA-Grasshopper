using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using GsaAPI;
using GsaGH.Parameters;
using OasysUnits.Units;
using System.Collections.Concurrent;

namespace GsaGH.Helpers.Import
{
  /// <summary>
  /// Class containing functions to import various object types from GSA
  /// </summary>
  internal class Lists
  {
    internal static List<GsaList> GetLists(GsaModel gsaModel, LengthUnit unit)
    {
      List<GsaList> lists = new List<GsaList>();
      foreach (KeyValuePair<int, EntityList> apiList in gsaModel.Model.Lists())
      {
        GsaList list = null;
        switch (apiList.Value.Type)
        {
          case GsaAPI.EntityType.Node:
            list = new GsaList(apiList.Key, apiList.Value,
              Nodes.GetNodes(gsaModel.Model.Nodes(apiList.Value.Definition), unit, gsaModel.Model.Axes()));
            break;
          
          case GsaAPI.EntityType.Element:
            Dictionary<int, ReadOnlyCollection<double>> elementLocalAxesDict = new Dictionary<int, ReadOnlyCollection<double>>();
            foreach (int id in gsaModel.Model.Elements(apiList.Value.Definition).Keys)
              elementLocalAxesDict.Add(id, gsaModel.Model.ElementDirectionCosine(id));
            list = new GsaList(apiList.Key, apiList.Value,
              Elements.GetElements(
                gsaModel.Model.Elements(apiList.Value.Definition),
                gsaModel.Model.Nodes(), gsaModel.Model.Sections(), gsaModel.Model.Prop2Ds(), gsaModel.Model.Prop3Ds(),
                gsaModel.Model.AnalysisMaterials(), gsaModel.Model.SectionModifiers(), elementLocalAxesDict, gsaModel.Model.Axes(), unit, false));
            break;

          case GsaAPI.EntityType.Member:
            Dictionary<int, ReadOnlyCollection<double>> memberLocalAxesDict = new Dictionary<int, ReadOnlyCollection<double>>();
            foreach (int id in gsaModel.Model.Members(apiList.Value.Definition).Keys)
              memberLocalAxesDict.Add(id, gsaModel.Model.MemberDirectionCosine(id));
            list = new GsaList(apiList.Key, apiList.Value,
              Members.GetMembers(
                gsaModel.Model.Members(apiList.Value.Definition),
                gsaModel.Model.Nodes(), gsaModel.Model.Sections(), gsaModel.Model.Prop2Ds(), gsaModel.Model.Prop3Ds(),
                gsaModel.Model.AnalysisMaterials(), gsaModel.Model.SectionModifiers(), memberLocalAxesDict, gsaModel.Model.Axes(), unit, false));
            break;

          case GsaAPI.EntityType.Case:
            ReadOnlyCollection<int> cases = gsaModel.Model.ExpandList(apiList.Value);
            list = new GsaList(apiList.Key, apiList.Value, new List<int>(cases));
            break;

          case GsaAPI.EntityType.Undefined:
          default:
            list = new GsaList(apiList.Key, apiList.Value);
            break;
        }
        lists.Add(list);
      }
      return lists;
    }
  }
}
