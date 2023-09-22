using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Grasshopper.Kernel;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Parameters.Enums;
using EntityType = GsaGH.Parameters.EntityType;

namespace GsaGH.Helpers.Export {
  internal partial class ModelAssembly {
    internal string GetNodeList(GsaList list) {
      if (Lists.GuidDictionary.TryGetValue(list.Guid, out int id)) {
        return $"\"{Lists.ReadOnlyDictionary[id].Name}\"";
      }

      AddNodeList(list);

      return $"\"{Lists.ReadOnlyDictionary[Lists.GuidDictionary[list.Guid]].Name}\"";
    }

    internal void ConvertNodeLists(List<GsaList> lists) {
      if (lists == null) {
        return;
      }

      lists = lists.OrderByDescending(x => x.Id).ToList();
      foreach (GsaList list in lists.Where(list => list != null
      && list.EntityType == EntityType.Node)) {
        AddNodeList(list);
      }
    }

    private void AddNodeList(GsaList list) {
      if (list._nodes == null || list._nodes.Count == 0) {
        AddList(list);
      }
      GsaList copyList = list.Duplicate();
      var ids = new List<int>();

      foreach (GsaNodeGoo node in list._nodes.Where(x => x != null && x.Value != null)) {
        ids.Add(AddNode(node.Value.GetApiNodeToUnit(Unit)));
      }

      copyList.Definition = GsaList.CreateListDefinition(ids);

      AddList(copyList);
    }

    internal string GetElementList(GsaList list, GH_Component owner) {
      if (list.EntityType == EntityType.Element
        && Lists.GuidDictionary.TryGetValue(list.Guid, out int id)) {
        return $"\"{Lists.ReadOnlyDictionary[id].Name}\"";
      }

      if (list.EntityType == EntityType.Member) {
        AddMemberList(list.Duplicate(), owner);
        string name = Lists.ReadOnlyDictionary[Lists.GuidDictionary[list.Guid]].Name;
        list.Name = $"Children of '{name}'";
        list.EntityType = EntityType.Element;
      }

      GsaList copyList = AddPropertiesList(list, owner);
      AddElementList(copyList, owner);

      return $"\"{Lists.ReadOnlyDictionary[Lists.GuidDictionary[copyList.Guid]].Name}\"";
    }

    internal string GetElementOrMemberList(GsaList list, GH_Component owner) {
      if (Lists.GuidDictionary.TryGetValue(list.Guid, out int id)) {
        return $"\"{Lists.ReadOnlyDictionary[id].Name}\"";
      }

      GsaList copyList = AddPropertiesList(list, owner);
      switch (list.EntityType) {
        case EntityType.Element:
          AddElementList(copyList, owner);
          break;

        case EntityType.Member:
          AddMemberList(copyList, owner);
          break;
      }

      return $"\"{Lists.ReadOnlyDictionary[Lists.GuidDictionary[copyList.Guid]].Name}\"";
    }

    internal void ConvertList(List<GsaList> lists, GH_Component owner) {
      if (lists == null) {
        return;
      }

      lists = lists.OrderByDescending(x => x.Id).ToList();
      foreach (GsaList list in lists.Where(list => list != null)) {
        ConvertList(list, owner);
      }
    }

    internal void ConvertList(GsaList list, GH_Component owner) {
      if (list == null) {
        return;
      }
      GsaList copyList;
      switch (list.EntityType) {
        case EntityType.Element:
          copyList = AddPropertiesList(list, owner);
          AddElementList(copyList, owner);
          break;

        case EntityType.Member:
          copyList = AddPropertiesList(list, owner);
          AddMemberList(copyList, owner);
          break;

        case EntityType.Case:
          copyList = list.Duplicate();
          list.Definition += GsaList.CreateListDefinition(list._cases);
          AddList(list);
          break;
      }
    }

    private GsaList AddPropertiesList(GsaList list, GH_Component owner) {
      GsaList copyList = list.Duplicate();
      if (copyList._properties == (null, null, null, null) || (copyList._properties.materials.Count == 0
        && copyList._properties.sections.Count == 0 && copyList._properties.prop2ds.Count == 0
        && copyList._properties.prop3ds.Count == 0)) {
        return copyList;
      }

      var ids = new Collection<string>();

      foreach (GsaMaterialGoo material in copyList._properties.materials
        .Where(x => x != null && x.Value != null)) {
        string id = GetReferenceDefinition1(material.Value.Guid, ReferenceType.Property);
        if (id == string.Empty) {
          owner.AddRuntimeWarning($"Issue adding List {copyList.Name} to Model:{Environment.NewLine}Material {material.Value} not found in Model");
        }

        ids.Add(id);
      }

      foreach (GsaSectionGoo section in copyList._properties.sections
        .Where(x => x != null && x.Value != null)) {
        string id = GetReferenceDefinition1(section.Value.Guid, ReferenceType.Property);
        if (id == string.Empty) {
          owner.AddRuntimeWarning("Issue adding List " + copyList.Name + " to Model:" + Environment.NewLine
            + section.Value.ToString() + " not found in Model");
        }

        ids.Add(id);
      }

      foreach (GsaProperty2dGoo prop2d in copyList._properties.prop2ds
        .Where(x => x != null && x.Value != null)) {
        string id = GetReferenceDefinition1(prop2d.Value.Guid, ReferenceType.Property);
        if (id == string.Empty) {
          owner.AddRuntimeWarning("Issue adding List " + copyList.Name + " to Model:" + Environment.NewLine
            + prop2d.Value.ToString() + " not found in Model");
        }

        ids.Add(id);
      }

      foreach (GsaProperty3dGoo prop3d in copyList._properties.prop3ds
        .Where(x => x != null && x.Value != null)) {
        string id = GetReferenceDefinition1(prop3d.Value.Guid, ReferenceType.Property);
        if (id == string.Empty) {
          owner.AddRuntimeWarning("Issue adding List " + copyList.Name + " to Model:" + Environment.NewLine
            + prop3d.Value.ToString() + " not found in Model");
        }

        ids.Add(id);
      }

      copyList.Definition = string.Join(" ", ids);
      return copyList;
    }

    private void AddElementList(GsaList list, GH_Component owner) {
      GsaList copyList = list.Duplicate();

      var ids = new List<string>();

      if (copyList._elements != (null, null, null)) {
        foreach (GsaElement1dGoo element1d in copyList._elements.e1d
          .Where(x => x != null && x.Value != null)) {
          string id = GetReferenceDefinition1(element1d.Value.Guid, ReferenceType.Element);
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
          string id = GetReferenceDefinition1(element2d.Value.Guid, ReferenceType.Element);
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
          string id = GetReferenceDefinition1(element3d.Value.Guid, ReferenceType.Element);
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
          string id = GetReferenceDefinition1(member1d.Value.Guid, ReferenceType.MemberChildElements);
          if (id == string.Empty && member1d.Value.Id != 0) {
            id = GetMemberChildElementReferenceIdsDefinition(member1d.Value.Id);
          }

          if (id == string.Empty) {
            owner.AddRuntimeWarning("Issue adding List " + copyList.Name + " to Model:" + Environment.NewLine
              + member1d.Value.ToString() + " not found in Model");
          }

          ids.Add(id);
        }

        foreach (GsaMember2dGoo member2d in copyList._members.m2d
          .Where(x => x != null && x.Value != null)) {
          string id = GetReferenceDefinition1(
            member2d.Value.Guid,
            ReferenceType.MemberChildElements);
          if (id == string.Empty && member2d.Value.Id != 0) {
            id = GetMemberChildElementReferenceIdsDefinition(member2d.Value.Id);
          }

          if (id == string.Empty) {
            owner.AddRuntimeWarning("Issue adding List " + copyList.Name + " to Model:" + Environment.NewLine
              + member2d.Value.ToString() + " not found in Model");
          }

          ids.Add(id);
        }

        foreach (GsaMember3dGoo member3d in copyList._members.m3d
          .Where(x => x != null && x.Value != null)) {
          string id = GetReferenceDefinition1(member3d.Value.Guid, ReferenceType.MemberChildElements);
          if (id == string.Empty && member3d.Value.Id != 0) {
            id = GetMemberChildElementReferenceIdsDefinition(member3d.Value.Id);
          }

          if (id == string.Empty) {
            owner.AddRuntimeWarning("Issue adding List " + copyList.Name + " to Model:" + Environment.NewLine
              + member3d.Value.ToString() + " not found in Model");
          }

          ids.Add(id);
        }
      }

      copyList.Definition += " " + string.Join(" ", ids);
      AddList(copyList);
    }

    private void AddMemberList(GsaList list, GH_Component owner) {
      GsaList copyList = list.Duplicate();

      if (copyList._members == (null, null, null) && (copyList._members.m1d.Count == 0
        || copyList._members.m2d.Count == 0 || copyList._members.m3d.Count == 0)) {
        AddList(copyList);
      }
      var ids = new Collection<string>();
      foreach (GsaMember1dGoo member1d in copyList._members.m1d
        .Where(x => x != null && x.Value != null)) {
        string id = GetReferenceDefinition1(member1d.Value.Guid, ReferenceType.Member);
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
        string id = GetReferenceDefinition1(member2d.Value.Guid, ReferenceType.Member);
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
        string id = GetReferenceDefinition1(member3d.Value.Guid, ReferenceType.Member);
        if (id == string.Empty && member3d.Value.Id != 0) {
          id = member3d.Value.Id.ToString();
        }

        if (id == string.Empty) {
          owner.AddRuntimeWarning("Issue adding List " + copyList.Name + " to Model:" + Environment.NewLine
            + member3d.Value.ToString() + " not found in Model");
        }

        ids.Add(id);
      }

      copyList.Definition += " " + string.Join(" ", ids);
      AddList(copyList);
    }

    private void AddList(GsaList list) {
      if (list.Id > 0) {
        if (list.Name == null || list.Name.Length == 0) {
          list.Name = list.EntityType.ToString() + " List [" + list.Id + "]";
        }
        Lists.SetValue(list.Id, list.Guid, list.GetApiList());
      } else {
        if (list.Name == null || list.Name.Length == 0) {
          list.Name = list.EntityType.ToString() + " List [" + (Lists.Count + 1) + "]";
        }
        Lists.AddValue(list.Guid, list.GetApiList());
      }
    }
  }
}
