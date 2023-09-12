using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Parameters;

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
                  list.Add(materialGoo);
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
                  list.Add(materialGoo);
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

    internal static string GetElementListNameForResults(
      GH_Component owner, IGH_DataAccess da, int inputid, GsaModel model) {
      // to-do GSAGH-350
      string elementlist = "All";
      var ghType = new GH_ObjectWrapper();
      if (da.GetData(inputid, ref ghType)) {
        if (ghType.Value is GsaListGoo listGoo) {
          if (listGoo.Value.EntityType != EntityType.Element
            && listGoo.Value.EntityType != EntityType.Member) {
            owner.AddRuntimeWarning("List must be of type Element to apply to element filter");
            return string.Empty;
          }

          if (listGoo.Value.Name == null || listGoo.Value.Name == string.Empty) {
            return listGoo.Value.Definition;
          }

          if (model.Model.Lists().Values.Where(
            x => x.Type == GsaAPI.EntityType.Element && x.Name == listGoo.Value.Name).Any()) {
            return "\"" + listGoo.Value.Name + "\"";
          }

          if (model.Model.Lists().Values.Where(
            x => x.Type == GsaAPI.EntityType.Member && x.Name == listGoo.Value.Name).Any()) {
            return "\"" + "Children of '" + listGoo.Value.Name + "'\"";
          }
          
          return listGoo.Value.Definition;
          
        } else {
          GH_Convert.ToString(ghType.Value, out elementlist, GH_Conversion.Both);
        }
      }

      if (string.IsNullOrEmpty(elementlist) || elementlist.ToLower() == "all") {
        elementlist = "All";
      }

      return elementlist;
    }

    internal static string GetNodeListNameForResults(
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
