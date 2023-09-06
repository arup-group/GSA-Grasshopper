using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Grasshopper.Kernel;
using GsaAPI;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGH.Helpers.Export {
  internal class Lists {
    internal static string GetNodeList(GsaList list, ref GsaGuidDictionary<EntityList> apiLists,
      ref GsaIntDictionary<Node> apiNodes, LengthUnit unit) {
      if (apiLists.GuidDictionary.TryGetValue(list.Guid, out int id)) {
        return $"\"{apiLists.ReadOnlyDictionary[id].Name}\"";
      }

      AddNodeList(list, ref apiLists, ref apiNodes, unit);

      return $"\"{apiLists.ReadOnlyDictionary[apiLists.GuidDictionary[list.Guid]].Name}\"";
    }

    internal static void ConvertNodeLists( 
    List<GsaList> lists, ref GsaGuidDictionary<EntityList> apiLists,
    ref GsaIntDictionary<Node> apiNodes, LengthUnit modelUnit) {
      if (lists == null) {
        return;
      }

      lists = lists.OrderByDescending(x => x.Id).ToList();
      foreach (GsaList list in lists.Where(list => list != null
      && list.EntityType == Parameters.EntityType.Node)) {
        AddNodeList(list, ref apiLists, ref apiNodes, modelUnit);
      }
    }

    private static void AddNodeList(GsaList list, ref GsaGuidDictionary<EntityList> apiLists,
      ref GsaIntDictionary<Node> apiNodes, LengthUnit unit) {
      if (list._nodes == null || list._nodes.Count == 0) {
        AddList(list, ref apiLists);
      }
      GsaList copyList = list.Duplicate();
      var ids = new List<int>();

      foreach (GsaNodeGoo node in list._nodes.Where(x => x != null && x.Value != null)) {
        ids.Add(Nodes.AddNode(ref apiNodes, node.Value.GetApiNodeToUnit(unit)));
      }

      copyList._definition = GsaList.CreateListDefinition(ids);

      AddList(copyList, ref apiLists);
    }

    internal static string GetElementList(GsaList list, ref ModelAssembly model, GH_Component owner) {
      if (list.EntityType == Parameters.EntityType.Element 
        && model.Lists.GuidDictionary.TryGetValue(list.Guid, out int id)) {
        return $"\"{model.Lists.ReadOnlyDictionary[id].Name}\"";
      }

      if (list.EntityType == Parameters.EntityType.Member) {
        AddMemberList(list.Duplicate(), ref model.Lists, model.Members, owner);
        string name = model.Lists.ReadOnlyDictionary[model.Lists.GuidDictionary[list.Guid]].Name;
        list._name = $"Children of '{name}'";
        list.EntityType = Parameters.EntityType.Element;
      }

      GsaList copyList = AddPropertiesList(list, model.Properties, owner);
      AddElementList(copyList, ref model, owner);

      return $"\"{model.Lists.ReadOnlyDictionary[model.Lists.GuidDictionary[copyList.Guid]].Name}\"";
    }

    internal static void ConvertList(
      List<GsaList> lists, ref ModelAssembly model, GH_Component owner) {
      if (lists == null) {
        return;
      }

      lists = lists.OrderByDescending(x => x.Id).ToList();
      foreach (GsaList list in lists.Where(list => list != null)) {
        ConvertList(list, ref model, owner);
      }
    }

    internal static void ConvertList(GsaList list, ref ModelAssembly model, GH_Component owner) {
      if (list == null) {
        return;
      }
      GsaList copyList;
      switch (list.EntityType) {
        case Parameters.EntityType.Element:
          copyList = AddPropertiesList(list, model.Properties, owner);
          AddElementList(copyList, ref model, owner);
          break;

        case Parameters.EntityType.Member:
          copyList = AddPropertiesList(list, model.Properties, owner);
          AddMemberList(copyList, ref model.Lists, model.Members, owner);
          break;

        case Parameters.EntityType.Case:
          copyList = list.Duplicate();
          list._definition += GsaList.CreateListDefinition(list._cases);
          AddList(list, ref model.Lists);
          break;
      }
    }

    private static GsaList AddPropertiesList(
      GsaList list, Properties apiProperties, GH_Component owner) {
      GsaList copyList = list.Duplicate();
      if (copyList._properties == (null, null, null, null) || (copyList._properties.materials.Count == 0
        && copyList._properties.sections.Count == 0 && copyList._properties.prop2ds.Count == 0
        && copyList._properties.prop3ds.Count == 0)) {
        return copyList;
      }

      var ids = new Collection<string>();

      foreach (GsaMaterialGoo material in copyList._properties.materials
        .Where(x => x != null && x.Value != null)) {
        string id = ElementListFromReference.GetReferenceDefinition(material.Value.Guid,
          ReferenceType.Property, apiProperties, null, null, null);
        if (id == string.Empty) {
          owner.AddRuntimeWarning($"Issue adding List {copyList.Name} to Model:{Environment.NewLine}Material {material.Value} not found in Model");
        }

        ids.Add(id);
      }

      foreach (GsaSectionGoo section in copyList._properties.sections
        .Where(x => x != null && x.Value != null)) {
        string id = ElementListFromReference.GetReferenceDefinition(section.Value.Guid,
          ReferenceType.Property, apiProperties, null, null, null);
        if (id == string.Empty) {
          owner.AddRuntimeWarning("Issue adding List " + copyList.Name + " to Model:" + Environment.NewLine
            + section.Value.ToString() + " not found in Model");
        }

        ids.Add(id);
      }

      foreach (GsaProperty2dGoo prop2d in copyList._properties.prop2ds
        .Where(x => x != null && x.Value != null)) {
        string id = ElementListFromReference.GetReferenceDefinition(prop2d.Value.Guid,
          ReferenceType.Property, apiProperties, null, null, null);
        if (id == string.Empty) {
          owner.AddRuntimeWarning("Issue adding List " + copyList.Name + " to Model:" + Environment.NewLine
            + prop2d.Value.ToString() + " not found in Model");
        }

        ids.Add(id);
      }

      foreach (GsaProperty3dGoo prop3d in copyList._properties.prop3ds
        .Where(x => x != null && x.Value != null)) {
        string id = ElementListFromReference.GetReferenceDefinition(prop3d.Value.Guid,
          ReferenceType.Property, apiProperties, null, null, null);
        if (id == string.Empty) {
          owner.AddRuntimeWarning("Issue adding List " + copyList.Name + " to Model:" + Environment.NewLine
            + prop3d.Value.ToString() + " not found in Model");
        }

        ids.Add(id);
      }

      copyList._definition = string.Join(" ", ids);
      return copyList;
    }

    private static void AddElementList(GsaList list, ref ModelAssembly model, GH_Component owner) {
      GsaList copyList = list.Duplicate();

      var ids = new List<string>();

      if (copyList._elements != (null, null, null)) {
        foreach (GsaElement1dGoo element1d in copyList._elements.e1d
          .Where(x => x != null && x.Value != null)) {
          string id = ElementListFromReference.GetReferenceDefinition(element1d.Value.Guid,
            ReferenceType.Element, null, model.Elements, null, null);
          if (id == string.Empty && element1d.Value.Id != 0) {
            id = element1d.Value.Id.ToString();
          }

          if (id == string.Empty) {
            owner.AddRuntimeWarning("Issue adding List " + copyList.Name + " to Model:" + Environment.NewLine
              + element1d.Value.ToString() + " not found in Model");
          }

          ids.Add(id);
        }

        foreach (GsaElement2dGoo element2d in copyList._elements.e2d
          .Where(x => x != null && x.Value != null)) {
          string id = ElementListFromReference.GetReferenceDefinition(element2d.Value.Guid,
            ReferenceType.Element, null, model.Elements, null, null);
          if (id == string.Empty && element2d.Value.Ids.Count != 0) {
            id = string.Join(" ", element2d.Value.Ids.Where(x => x != 0));
          }

          if (id == string.Empty) {
            owner.AddRuntimeWarning("Issue adding List " + copyList.Name + " to Model:" + Environment.NewLine
              + element2d.Value.ToString() + " not found in Model");
          }

          ids.Add(id);
        }

        foreach (GsaElement3dGoo element3d in copyList._elements.e3d
          .Where(x => x != null && x.Value != null)) {
          string id = ElementListFromReference.GetReferenceDefinition(element3d.Value.Guid,
            ReferenceType.Element, null, model.Elements, null, null);
          if (id == string.Empty && element3d.Value.Ids.Count != 0) {
            id = string.Join(" ", element3d.Value.Ids.Where(x => x != 0));
          }

          if (id == string.Empty) {
            owner.AddRuntimeWarning("Issue adding List " + copyList.Name + " to Model:" + Environment.NewLine
              + element3d.Value.ToString() + " not found in Model");
          }

          ids.Add(id);
        }
      }

      if (copyList._members != (null, null, null)) {
        foreach (GsaMember1dGoo member1d in copyList._members.m1d
          .Where(x => x != null && x.Value != null)) {
          string id = ElementListFromReference.GetReferenceDefinition(
            member1d.Value.Guid,
            ReferenceType.MemberChildElements, 
            null, 
            null, 
            model.Members,
            model.MemberElementRelationship);
          if (id == string.Empty && member1d.Value.Id != 0) {
            id = ElementListFromReference.GetMemberChildElementReferenceIdsDefinition(
              member1d.Value.Id, model.MemberElementRelationship);
          }

          if (id == string.Empty) {
            owner.AddRuntimeWarning("Issue adding List " + copyList.Name + " to Model:" + Environment.NewLine
              + member1d.Value.ToString() + " not found in Model");
          }

          ids.Add(id);
        }

        foreach (GsaMember2dGoo member2d in copyList._members.m2d
          .Where(x => x != null && x.Value != null)) {
          string id = ElementListFromReference.GetReferenceDefinition(
            member2d.Value.Guid,
            ReferenceType.MemberChildElements, 
            null, 
            null, 
            model.Members,
            model.MemberElementRelationship);
          if (id == string.Empty && member2d.Value.Id != 0) {
            id = ElementListFromReference.GetMemberChildElementReferenceIdsDefinition(
              member2d.Value.Id, model.MemberElementRelationship);
          }

          if (id == string.Empty) {
            owner.AddRuntimeWarning("Issue adding List " + copyList.Name + " to Model:" + Environment.NewLine
              + member2d.Value.ToString() + " not found in Model");
          }

          ids.Add(id);
        }

        foreach (GsaMember3dGoo member3d in copyList._members.m3d
          .Where(x => x != null && x.Value != null)) {
          string id = ElementListFromReference.GetReferenceDefinition(
            member3d.Value.Guid,
            ReferenceType.MemberChildElements, 
            null, 
            null, 
            model.Members,
            model.MemberElementRelationship);
          if (id == string.Empty && member3d.Value.Id != 0) {
            id = ElementListFromReference.GetMemberChildElementReferenceIdsDefinition(
              member3d.Value.Id, model.MemberElementRelationship);
          }

          if (id == string.Empty) {
            owner.AddRuntimeWarning("Issue adding List " + copyList.Name + " to Model:" + Environment.NewLine
              + member3d.Value.ToString() + " not found in Model");
          }

          ids.Add(id);
        }
      }

      copyList._definition += " " + string.Join(" ", ids);

      AddList(copyList, ref model.Lists);
    }

    private static void AddMemberList(GsaList list, ref GsaGuidDictionary<EntityList> apiLists,
      GsaGuidDictionary<Member> apiMembers, GH_Component owner) {
      GsaList copyList = list.Duplicate();

      if (copyList._members == (null, null, null) && (copyList._members.m1d.Count == 0
        || copyList._members.m2d.Count == 0 || copyList._members.m3d.Count == 0)) {
        AddList(copyList, ref apiLists);
      }
      var ids = new Collection<string>();
      foreach (GsaMember1dGoo member1d in copyList._members.m1d
        .Where(x => x != null && x.Value != null)) {
        string id = ElementListFromReference.GetReferenceDefinition(member1d.Value.Guid,
          ReferenceType.Member, null, null, apiMembers, null);
        if (id == string.Empty && member1d.Value.Id != 0) {
          id = member1d.Value.Id.ToString();
        }

        if (id == string.Empty) {
          owner.AddRuntimeWarning("Issue adding List " + copyList.Name + " to Model:" + Environment.NewLine
            + member1d.Value.ToString() + " not found in Model");
        }

        ids.Add(id);
      }

      foreach (GsaMember2dGoo member2d in copyList._members.m2d
        .Where(x => x != null && x.Value != null)) {
        string id = ElementListFromReference.GetReferenceDefinition(member2d.Value.Guid,
          ReferenceType.Member, null, null, apiMembers, null);
        if (id == string.Empty && member2d.Value.Id != 0) {
          id = member2d.Value.Id.ToString();
        }

        if (id == string.Empty) {
          owner.AddRuntimeWarning("Issue adding List " + copyList.Name + " to Model:" + Environment.NewLine
            + member2d.Value.ToString() + " not found in Model");
        }

        ids.Add(id);
      }

      foreach (GsaMember3dGoo member3d in copyList._members.m3d
        .Where(x => x != null && x.Value != null)) {
        string id = ElementListFromReference.GetReferenceDefinition(member3d.Value.Guid,
          ReferenceType.Member, null, null, apiMembers, null);
        if (id == string.Empty && member3d.Value.Id != 0) {
          id = member3d.Value.Id.ToString();
        }

        if (id == string.Empty) {
          owner.AddRuntimeWarning("Issue adding List " + copyList.Name + " to Model:" + Environment.NewLine
            + member3d.Value.ToString() + " not found in Model");
        }

        ids.Add(id);
      }

      copyList._definition += " " + string.Join(" ", ids);

      AddList(copyList, ref apiLists);
    }

    private static void AddList(GsaList list, ref GsaGuidDictionary<EntityList> apiLists) {
      if (list.Id > 0) {
        if (list.Name == null || list.Name.Length == 0) {
          list._name = list.EntityType.ToString() + " List [" + list.Id + "]";
        }
        apiLists.SetValue(list.Id, list.Guid, list.GetApiList());
      } else {
        if (list.Name == null || list.Name.Length == 0) {
          list._name = list.EntityType.ToString() + " List [" + (apiLists.Count + 1) + "]";
        }
        apiLists.AddValue(list.Guid, list.GetApiList());
      }
    }
  }
}
