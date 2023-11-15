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

    internal static EntityList GetElementOrMemberList(
      GH_Component owner, IGH_DataAccess da, int inputid) {
      // to-do GSAGH-350
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
        owner.AddRuntimeWarning("List must be of either Element or Member type to apply to " +
          "element filter");
        return list;
      }

      return listGoo.Value.GetApiList();
    }

    internal static string GetElementListDefinition(
      GH_Component owner, IGH_DataAccess da, int inputid, GsaModel model) {
      // to-do GSAGH-350
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
        owner.AddRuntimeWarning("List must be of either Element or Member type to apply to " +
          "element filter");
        return string.Empty;
      }

      if (listGoo.Value.EntityType == EntityType.Element) {
        if (model.Model.Lists().Values.Where(
          x => x.Type == GsaAPI.EntityType.Element && x.Name == listGoo.Value.Name).Any()) {
          return "\"" + listGoo.Value.Name + "\"";
        }

        return listGoo.Value.Definition;
      }

      // list is Member list
      ConcurrentDictionary<int, ConcurrentBag<int>> memberElementRelationship
        = ModelAssembly.GetMemberElementRelationship(model.Model);

      // try find existing list of same name in model
      if (listGoo.Value.Name != null && listGoo.Value.Name != string.Empty) {
        if (model.Model.Lists().Values.Where(
          x => x.Type == GsaAPI.EntityType.Element
          && x.Name == $"Children of '{listGoo.Value.Name}'").Any()) {
          owner.AddRuntimeRemark($"Element definition was derived from Children of " +
            $"'{listGoo.Value.Name}' List");
          return "\"" + listGoo.Value.Name + "\"";
        }

        foreach (EntityList list in model.Model.Lists().Values) {
          if (list.Type != GsaAPI.EntityType.Member || list.Name != listGoo.Value.Name) {
            continue;
          }

          ReadOnlyCollection<int> memberIds = model.Model.ExpandList(list);

          var elementIds = new List<int>();
          var warnings = new List<int>(); ;
          foreach (int memberId in memberIds) {
            if (!memberElementRelationship.ContainsKey(memberId)) {
              continue;
            }

            elementIds.AddRange(memberElementRelationship[memberId]);
          }

          if (warnings.Count > 0) {
            string warningIds = GsaList.CreateListDefinition(warnings);
            owner.AddRuntimeWarning($"No child elements found for Members {warningIds}");
          }

          owner.AddRuntimeRemark($"Element definition was derived from Elements with Parent " +
            $"Member included in '{listGoo.Value.Name}' List");
          return GsaList.CreateListDefinition(elementIds);
        }
      }

      // try convert Member list to child elements
      EntityList tempList = listGoo.Value.GetApiList();
      if (string.IsNullOrEmpty(tempList.Name)) {
        tempList.Name = "List";
      }
      ReadOnlyCollection<int> memberIds2 = model.Model.ExpandList(tempList);

      var elementIds2 = new List<int>();
      var warnings2 = new List<int>(); ;
      foreach (int memberId in memberIds2) {
        if (!memberElementRelationship.ContainsKey(memberId)) {
          continue;
        }

        elementIds2.AddRange(memberElementRelationship[memberId]);
      }

      if (elementIds2.Count > 0) {
        if (warnings2.Count > 0) {
          string warningIds = GsaList.CreateListDefinition(warnings2);
          owner.AddRuntimeWarning($"No child elements found for Members {warningIds}");
        }

        owner.AddRuntimeRemark($"Element definition was derived from Elements with Parent "
          + $"Member included in '{listGoo.Value.Name}' List");
        return GsaList.CreateListDefinition(elementIds2);
      }

      return string.Empty;
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

          if (model.Model.Lists().Values.Where(
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

          if (model.Model.Lists().Values.Where(
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
  }
}
