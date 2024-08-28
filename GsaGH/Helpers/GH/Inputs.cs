using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using GsaAPI;

using GsaGH.Helpers.Assembly;
using GsaGH.Parameters;
using GsaGH.Parameters.Results;

using EntityType = GsaGH.Parameters.EntityType;

namespace GsaGH.Helpers.GH {
  internal class Inputs {
    internal static List<object> GetGooObjectsForLists(
      GH_Component owner, IGH_DataAccess DA, int inputid, EntityType type) {
      // Get Geometry input
      var gh_types = new List<GH_ObjectWrapper>();
      var list = new List<object>();
      if (DA.GetDataList(inputid, gh_types)) {
        for (int i = 0; i < gh_types.Count; i++) {
          GH_ObjectWrapper gh_typ = gh_types[i];
          if (gh_typ == null) {
            owner.AddRuntimeWarning("Input (index: " + i + ") is null and has been ignored");
            continue;
          }

          if (gh_typ.Value is GH_String txt) {
            list.Add(txt.Value);
            continue;
          }

          if (gh_typ.Value is GH_Integer integer) {
            list.Add(integer.Value.ToString());
            continue;
          }

          if (gh_typ.Value is GH_Number number) {
            string val = number.Value.ToString();
            if (!val.Contains(".")) {
              list.Add(val);
              continue;
            }
          }

          switch (type) {
            case EntityType.Node:
              if (gh_typ.Value is GsaNodeGoo nodeGoo) {
                list.Add(nodeGoo);
              } else if (gh_typ.Value is GH_Point ghPoint) {
                var node = new GsaNode(ghPoint.Value);
                list.Add(new GsaNodeGoo(node));
              } else {
                owner.AddRuntimeError("Unable to convert " + owner.Params.Input[inputid].NickName
                  + "  input (index: " + i + ") input parameter of type "
                  + gh_typ.Value.GetType().Name.Replace("Goo", string.Empty) + Environment.NewLine
                  + " to Node and has been ignored");
                continue;
              }

              break;

            case EntityType.Element:
              switch (gh_typ.Value) {
                case GsaMaterialGoo materialGoo:
                  switch (materialGoo.Value.MaterialType) {
                    case MatType.Custom:
                    case MatType.Steel:
                    case MatType.Concrete:
                    case MatType.Frp:
                      list.Add(materialGoo);
                      break;

                    default:
                      owner.AddRuntimeError($"Unable to reference Material of type " +
                        $"{materialGoo.Value.MaterialType} in a list.");
                      continue;
                  }
                  break;

                case GsaSectionGoo sectionGoo:
                  list.Add(sectionGoo);
                  break;

                case GsaProperty2dGoo prop2dGoo:
                  list.Add(prop2dGoo);
                  break;

                case GsaProperty3dGoo prop3dGoo:
                  list.Add(prop3dGoo);
                  break;

                case GsaElement1dGoo element1dGoo:
                  list.Add(element1dGoo);
                  break;

                case GsaElement2dGoo element2dGoo:
                  list.Add(element2dGoo);
                  break;

                case GsaElement3dGoo element3dGoo:
                  list.Add(element3dGoo);
                  break;

                case GsaMember1dGoo member1dGoo:
                  list.Add(member1dGoo);
                  break;

                case GsaMember2dGoo member2dGoo:
                  list.Add(member2dGoo);
                  break;

                case GsaMember3dGoo member3dGoo:
                  list.Add(member3dGoo);
                  break;

                default:
                  owner.AddRuntimeError("Unable to convert " + owner.Params.Input[inputid].NickName
                      + " input (index: " + i + ") input parameter of type "
                      + gh_typ.Value.GetType().Name.Replace("Goo", string.Empty)
                      + Environment.NewLine
                      + " to Element, Member child, Material, or Property and has been ignored");
                  continue;
              }
              break;

            case EntityType.Member:
              switch (gh_typ.Value) {
                case GsaMaterialGoo materialGoo:
                  switch (materialGoo.Value.MaterialType) {
                    case MatType.Custom:
                    case MatType.Steel:
                    case MatType.Concrete:
                    case MatType.Frp:
                      list.Add(materialGoo);
                      break;

                    default:
                      owner.AddRuntimeError($"Unable to reference Material of type " +
                        $"{materialGoo.Value.MaterialType} in a list.");
                      continue;
                  }
                  break;

                case GsaSectionGoo sectionGoo:
                  list.Add(sectionGoo);
                  break;

                case GsaProperty2dGoo prop2dGoo:
                  list.Add(prop2dGoo);
                  break;

                case GsaProperty3dGoo prop3dGoo:
                  owner.AddRuntimeError($"Unable to reference 3D property in a Member list.");
                  continue;

                case GsaMember1dGoo member1dGoo:
                  list.Add(member1dGoo);
                  break;

                case GsaMember2dGoo member2dGoo:
                  list.Add(member2dGoo);
                  break;

                case GsaMember3dGoo member3dGoo:
                  list.Add(member3dGoo);
                  break;

                default:
                  owner.AddRuntimeError("Unable to convert " + owner.Params.Input[inputid].NickName
                      + " input (index: " + i + ") input parameter of type "
                      + gh_typ.Value.GetType().Name.Replace("Goo", string.Empty)
                      + Environment.NewLine + " to Member or Property and has been ignored");
                  continue;
              }
              break;

            case EntityType.Case:
              int caseId = 0;
              if (!GH_Convert.ToInt32(gh_typ.Value, out caseId, GH_Conversion.Both)) {
                owner.AddRuntimeError("Unable to convert " + owner.Params.Input[inputid].NickName
                  + " input (index: " + i + ") input parameter of type "
                  + gh_typ.Value.GetType().Name.Replace("Goo", string.Empty) + Environment.NewLine
                  + " to Integer and has been ignored");
                continue;
              }

              list.Add(caseId);
              break;
          }
        }
      }

      return list;
    }

    internal static EntityList GetAssemblyList(GH_Component owner, IGH_DataAccess da, int inputid) {
      var list = new EntityList() {
        Definition = "All",
        Type = GsaAPI.EntityType.Assembly
      };

      var ghType = new GH_ObjectWrapper();
      if (!da.GetData(inputid, ref ghType)) {
        return list;
      }

      if (!(ghType.Value is GsaListGoo listGoo)) {
        GH_Convert.ToString(ghType.Value, out string definition, GH_Conversion.Both);
        if (string.IsNullOrEmpty(definition) || definition.ToLower() == "all") {
          definition = "All";
        }

        list.Definition = definition;
        return list;
      }

      if (listGoo.Value.EntityType != EntityType.Assembly) {
        owner.AddRuntimeWarning("List must be of type Assembly to apply to assembly filter");
        return list;
      }

      return listGoo.Value.GetApiList();
    }

    internal static EntityList GetElementOrMemberList(
    GH_Component owner, IGH_DataAccess da, int inputid) {
      var list = new EntityList() {
        Definition = "All",
        Type = GsaAPI.EntityType.Element
      };

      var ghType = new GH_ObjectWrapper();
      if (!da.GetData(inputid, ref ghType)) {
        return list;
      }

      if (!(ghType.Value is GsaListGoo listGoo)) {
        GH_Convert.ToString(ghType.Value, out string definition, GH_Conversion.Both);
        if (string.IsNullOrEmpty(definition) || definition.ToLower() == "all") {
          definition = "All";
        }

        list.Definition = definition;
        return list;
      }

      if (listGoo.Value.EntityType != EntityType.Element
        && listGoo.Value.EntityType != EntityType.Member) {
        owner.AddRuntimeWarning("List must be of either type Element or Member to apply to " +
          "element filter");
        return list;
      }

      return listGoo.Value.GetApiList();
    }

    internal static string GetAssemblyListDefinition(GH_Component owner, IGH_DataAccess da, int inputid, GsaModel model) {
      string assemblyList = "All";
      var ghType = new GH_ObjectWrapper();
      if (!da.GetData(inputid, ref ghType)) {
        return assemblyList;
      }

      if (!(ghType.Value is GsaListGoo listGoo)) {
        GH_Convert.ToString(ghType.Value, out assemblyList, GH_Conversion.Both);
        if (string.IsNullOrEmpty(assemblyList) || assemblyList.ToLower() == "all") {
          assemblyList = "All";
        }

        return assemblyList;
      }

      if (listGoo.Value.EntityType != EntityType.Assembly) {
        owner.AddRuntimeWarning("List must be of either type Assembly to apply to assembly filter");
        return string.Empty;
      }

      if (model.ApiModel.Lists().Values.Where(
        x => x.Type == GsaAPI.EntityType.Assembly && x.Name == listGoo.Value.Name).Any()) {
        return "\"" + listGoo.Value.Name + "\"";
      }

      return listGoo.Value.Definition;
    }

    internal static string GetElementListDefinition(
    GH_Component owner, IGH_DataAccess da, int inputid, GsaModel model) {
      string elementlist = "All";
      var ghType = new GH_ObjectWrapper();
      if (!da.GetData(inputid, ref ghType)) {
        return elementlist;
      }

      if (!(ghType.Value is GsaListGoo listGoo)) {
        GH_Convert.ToString(ghType.Value, out elementlist, GH_Conversion.Both);
        if (string.IsNullOrEmpty(elementlist) || elementlist.ToLower() == "all") {
          elementlist = "All";
        }

        return elementlist;
      }

      if (listGoo.Value.EntityType != EntityType.Element
        && listGoo.Value.EntityType != EntityType.Member) {
        owner.AddRuntimeWarning("List must be of either type Element or Member to apply to " +
          "element filter");
        return string.Empty;
      }

      if (listGoo.Value.EntityType == EntityType.Element) {
        if (model.ApiModel.Lists().Values.Where(
          x => x.Type == GsaAPI.EntityType.Element && x.Name == listGoo.Value.Name).Any()) {
          return "\"" + listGoo.Value.Name + "\"";
        }

        return listGoo.Value.Definition;
      }

      // list is Member list
      ConcurrentDictionary<int, ConcurrentBag<int>> memberElementRelationship
        = ModelAssembly.GetMemberElementRelationship(model.ApiModel);

      // try find existing list of same name in model
      if (listGoo.Value.Name != null && listGoo.Value.Name != string.Empty) {
        if (model.ApiModel.Lists().Values.Where(
          x => x.Type == GsaAPI.EntityType.Element
          && x.Name == $"Children of '{listGoo.Value.Name}'").Any()) {
          owner.AddRuntimeRemark($"Element definition was derived from Children of " +
            $"'{listGoo.Value.Name}' List");
          return "\"" + listGoo.Value.Name + "\"";
        }

        foreach (EntityList list in model.ApiModel.Lists().Values) {
          if (list.Type != GsaAPI.EntityType.Member || list.Name != listGoo.Value.Name) {
            continue;
          }

          ReadOnlyCollection<int> memberIds = model.ApiModel.ExpandList(list);
          var elementIds = new List<int>();
          var warnings = new List<int>(); ;
          foreach (int memberId in memberIds) {
            if (!memberElementRelationship.ContainsKey(memberId)) {
              warnings.Add(memberId);
              continue;
            }

            elementIds.AddRange(memberElementRelationship[memberId]);
          }

          if (warnings.Count > 0) {
            string warningIds = GsaList.CreateListDefinition(warnings);
            owner.AddRuntimeWarning($"No child elements found for Members {warningIds}");
          }

          string listName = string.Empty;
          if (!string.IsNullOrEmpty(listGoo.Value.Name)) {
            listName = $"'{listGoo.Value.Name}' ";
          }
          owner.AddRuntimeRemark($"Element definition was derived from Elements with parent " +
            $"Members included in {listName}List");
          return GsaList.CreateListDefinition(elementIds);
        }
      }

      // try convert Member list to child elements
      EntityList tempList = listGoo.Value.GetApiList();
      if (string.IsNullOrEmpty(tempList.Name)) {
        tempList.Name = "List";
      }
      ReadOnlyCollection<int> memberIds2 = model.ApiModel.ExpandList(tempList);

      var elementIds2 = new List<int>();
      var warnings2 = new List<int>(); ;
      foreach (int memberId in memberIds2) {
        if (!memberElementRelationship.ContainsKey(memberId)) {
          warnings2.Add(memberId);
          continue;
        }

        elementIds2.AddRange(memberElementRelationship[memberId]);
      }

      if (elementIds2.Count > 0) {
        string listName = string.Empty;
        if (!string.IsNullOrEmpty(listGoo.Value.Name)) {
          listName = $"'{listGoo.Value.Name}' ";
        }
        owner.AddRuntimeRemark($"Element definition was derived from Elements with parent "
          + $"Members included in {listName}List");
        return GsaList.CreateListDefinition(elementIds2);
      }

      if (warnings2.Count > 0) {
        string warningIds = GsaList.CreateListDefinition(warnings2);
        owner.AddRuntimeWarning($"No child elements found for Members {warningIds}");
      }

      return string.Empty;
    }

    internal static string GetNodeListDefinition(
      GH_Component owner, IGH_DataAccess da, int inputid, GsaModel model) {
      string nodeList = "All";
      var ghType = new GH_ObjectWrapper();
      if (da.GetData(inputid, ref ghType)) {
        if (ghType.Value is GsaListGoo listGoo) {
          if (listGoo.Value.EntityType != EntityType.Node) {
            owner.AddRuntimeWarning("List must be of type Node to apply to node filter");
          }

          if (listGoo.Value.Name == null || listGoo.Value.Name == string.Empty) {
            return listGoo.Value.Definition;
          }

          if (model.ApiModel.Lists().Values.Where(
            x => x.Type == GsaAPI.EntityType.Node && x.Name == listGoo.Value.Name).Any()) {
            return "\"" + listGoo.Value.Name + "\"";
          }

          return listGoo.Value.Definition;

        } else {
          GH_Convert.ToString(ghType.Value, out nodeList, GH_Conversion.Both);
        }
      }

      if (string.IsNullOrEmpty(nodeList) || nodeList.ToLower() == "all") {
        nodeList = "All";
      }

      return nodeList;
    }

    internal static string GetMemberListDefinition(
      GH_Component owner, IGH_DataAccess da, int inputid, GsaModel model) {
      string memberList = "All";
      var ghType = new GH_ObjectWrapper();
      if (da.GetData(inputid, ref ghType)) {
        if (ghType.Value is GsaListGoo listGoo) {
          if (listGoo.Value.EntityType != EntityType.Member) {
            owner.AddRuntimeWarning("List must be of type Member to apply to member filter");
          }

          if (listGoo.Value.Name == null || listGoo.Value.Name == string.Empty) {
            return listGoo.Value.Definition;
          }

          if (model.ApiModel.Lists().Values.Where(
            x => x.Type == GsaAPI.EntityType.Member && x.Name == listGoo.Value.Name).Any()) {
            return "\"" + listGoo.Value.Name + "\"";
          }

          return listGoo.Value.Definition;

        } else {
          GH_Convert.ToString(ghType.Value, out memberList, GH_Conversion.Both);
        }
      }

      if (string.IsNullOrEmpty(memberList) || memberList.ToLower() == "all") {
        memberList = "All";
      }

      return memberList;
    }

    internal static GsaResult GetResultInput(GH_Component owner, GH_ObjectWrapper ghTyp) {
      var goo = (GsaResultGoo)ghTyp.Value;
      var result = (GsaResult)goo.Value;
      if (result == null) {
        string errMsg = owner.RuntimeMessages(GH_RuntimeMessageLevel.Error)[0];
        if (errMsg.StartsWith("Use 'SelectResults'")) {
          var connect = new Guid(
            errMsg.Split(new string[] { "Connect" }, StringSplitOptions.None).Last());
          owner.Params.Input[0].RemoveAllSources();
          IGH_Param newInput = owner.OnPingDocument().FindParameter(connect);
          owner.Params.Input[0].AddSource(newInput);
        }

        return null;
      }

      return result;
    }

    internal static bool IsResultCaseEnveloped(
      GH_Component owner, GsaResult result, ref string caseTxt, EnvelopeMethod envelope) {
      switch (result.CaseType) {
        case CaseType.CombinationCase when result.SelectedPermutationIds.Count > 1:
          owner.AddRuntimeRemark("Combination Case " + result.CaseId + " contains "
            + result.SelectedPermutationIds.Count
            + $" permutations which have been enveloped using {envelope} method."
            + Environment.NewLine
            + "Change the enveloping method by right-clicking the component.");
          owner.Message = $"{owner.Message} \n{envelope}";
          caseTxt = $"Case C{result.CaseId} ({result.SelectedPermutationIds.Count} perm.)" +
            "\n" + ResultsUtility.EnvelopeMethodAbbreviated(envelope);
          return true;
        case CaseType.CombinationCase:
          caseTxt = "Case C" + result.CaseId + " P" + result.SelectedPermutationIds[0];
          return result.Permutations != 1;

        case CaseType.AnalysisCase:
        default:
          caseTxt = "Case A" + result.CaseId + Environment.NewLine + result.CaseName;
          return false;
      }
    }
  }
}
