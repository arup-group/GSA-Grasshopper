using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Grasshopper.Kernel;
using GsaAPI;
using GsaGH.Parameters;
using OasysUnits.Units;

namespace GsaGH.Helpers.Export {
  internal class Lists {
    internal static string GetNodeList(GsaList list, ref GsaGuidDictionary<EntityList> apiLists,
      ref GsaIntDictionary<Node> apiNodes, LengthUnit unit) {
      if (apiLists.GuidDictionary.TryGetValue(list.Guid, out int id)) {
        return "\"" + apiLists.ReadOnlyDictionary[id].Name + "\"";
      }

      AddNodeList(list, ref apiLists, ref apiNodes, unit);
      
      return "\"" + apiLists.ReadOnlyDictionary[apiLists.GuidDictionary[list.Guid]].Name + "\"";
    }

    internal static void ConvertNodeList(
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
      var ids = new Collection<int>();

      foreach (GsaNodeGoo node in list._nodes.Where(x => x != null && x.Value != null)) {
        ids.Add(Nodes.AddNode(ref apiNodes, node.Value.GetApiNodeToUnit(unit)));
      }

      copyList._definition = string.Join(" ", ids);

      AddList(copyList, ref apiLists);
    }

    internal static string GetElementList(GsaList list, ref GsaGuidDictionary<EntityList> apiLists,
      GsaGuidDictionary<AnalysisMaterial> apiMaterials, GsaGuidDictionary<Section> apiSections,
      GsaGuidDictionary<Prop2D> apiProp2ds, GsaGuidDictionary<Prop3D> apiProp3ds,
      GsaGuidIntListDictionary<Element> apiElements, GsaGuidDictionary<Member> apiMembers,
      ConcurrentDictionary<int, ConcurrentBag<int>> memberElementRelationship, GH_Component owner) {
      if (apiLists.GuidDictionary.TryGetValue(list.Guid, out int id)) {
        return "\"" + apiLists.ReadOnlyDictionary[id].Name + "\"";
      }

      if (list.EntityType == Parameters.EntityType.Member) {
        AddMemberList(list.Duplicate(), ref apiLists, apiMembers, owner);
        string name = apiLists.ReadOnlyDictionary[apiLists.GuidDictionary[list.Guid]].Name;
        list._name = "Children of '" + name + "'";
        list.EntityType = Parameters.EntityType.Element;
      }

      GsaList copyList = AddPropertiesList(list, apiMaterials, apiSections, apiProp2ds, apiProp3ds, owner);
      AddElementList(copyList, ref apiLists, apiElements, apiMembers, memberElementRelationship, owner);

      return "\"" + apiLists.ReadOnlyDictionary[apiLists.GuidDictionary[copyList.Guid]].Name + "\"";
    }

    internal static void ConvertList(
      List<GsaList> lists, ref GsaGuidDictionary<EntityList> apiLists,
      GsaGuidDictionary<AnalysisMaterial> apiMaterials, GsaGuidDictionary<Section> apiSections,
      GsaGuidDictionary<Prop2D> apiProp2ds, GsaGuidDictionary<Prop3D> apiProp3ds,
      GsaGuidIntListDictionary<Element> apiElements, GsaGuidDictionary<Member> apiMembers,
      ConcurrentDictionary<int, ConcurrentBag<int>> memberElementRelationship, GH_Component owner) {
      if (lists == null) {
        return;
      }

      lists = lists.OrderByDescending(x => x.Id).ToList();
      foreach (GsaList list in lists.Where(list => list != null)) {
        ConvertList(list, ref apiLists, apiMaterials, apiSections, apiProp2ds, apiProp3ds,
          apiElements, apiMembers, memberElementRelationship, owner);
      }
    }

    internal static void ConvertList(GsaList list, ref GsaGuidDictionary<EntityList> apiLists,
      GsaGuidDictionary<AnalysisMaterial> apiMaterials, GsaGuidDictionary<Section> apiSections,
      GsaGuidDictionary<Prop2D> apiProp2ds, GsaGuidDictionary<Prop3D> apiProp3ds,
      GsaGuidIntListDictionary<Element> apiElements, GsaGuidDictionary<Member> apiMembers,
      ConcurrentDictionary<int, ConcurrentBag<int>> memberElementRelationship, GH_Component owner) {
      if (list == null) {
        return;
      }
      GsaList copyList;
      switch (list.EntityType) {
        case Parameters.EntityType.Element:
          copyList = AddPropertiesList(list, apiMaterials, apiSections, apiProp2ds, apiProp3ds, owner);
          AddElementList(copyList, ref apiLists, apiElements, apiMembers, memberElementRelationship, owner);
          break;

        case Parameters.EntityType.Member:
          copyList = AddPropertiesList(list, apiMaterials, apiSections, apiProp2ds, apiProp3ds, owner);
          AddMemberList(copyList, ref apiLists, apiMembers, owner);
          break;

        case Parameters.EntityType.Case:
          copyList = list.Duplicate();
          list._definition += string.Join(" ", list._cases);
          AddList(list, ref apiLists);
          break;
      }
    }

    private static GsaList AddPropertiesList(GsaList list,
      GsaGuidDictionary<AnalysisMaterial> apiMaterials, GsaGuidDictionary<Section> apiSections,
      GsaGuidDictionary<Prop2D> apiProp2ds, GsaGuidDictionary<Prop3D> apiProp3ds,
      GH_Component owner) {
      GsaList copyList = list.Duplicate();
      if (copyList._properties == (null, null, null, null) || (copyList._properties.materials.Count == 0
        && copyList._properties.sections.Count == 0 && copyList._properties.prop2ds.Count == 0
        && copyList._properties.prop3ds.Count == 0)) {
        return copyList;
      }

      var ids = new Collection<string>();

      foreach (GsaMaterialGoo material in copyList._properties.materials
        .Where(x => x != null && x.Value != null)) {
        string id = ElementListFromReference.GetReference(material.Value.Guid,
          ReferenceType.Material, apiMaterials, null, null, null, null, null, null);
        if (id == "") {
          owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning,
            "Issue adding List " + copyList.Name + " to Model:" + System.Environment.NewLine
            + "Material " + material.Value.ToString() + " not found in Model");
        }

        ids.Add(id);
      }

      foreach (GsaSectionGoo section in copyList._properties.sections
        .Where(x => x != null && x.Value != null)) {
        string id = ElementListFromReference.GetReference(section.Value.Guid,
          ReferenceType.Section, null, apiSections, null, null, null, null, null);
        if (id == "") {
          owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning,
            "Issue adding List " + copyList.Name + " to Model:" + Environment.NewLine
            + section.Value.ToString() + " not found in Model");
        }

        ids.Add(id);
      }

      foreach (GsaProp2dGoo prop2d in copyList._properties.prop2ds
        .Where(x => x != null && x.Value != null)) {
        string id = ElementListFromReference.GetReference(prop2d.Value.Guid,
          ReferenceType.Prop2d, null, null, apiProp2ds, null, null, null, null);
        if (id == "") {
          owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning,
            "Issue adding List " + copyList.Name + " to Model:" + Environment.NewLine
            + prop2d.Value.ToString() + " not found in Model");
        }

        ids.Add(id);
      }

      foreach (GsaProp3dGoo prop3d in copyList._properties.prop3ds
        .Where(x => x != null && x.Value != null)) {
        string id = ElementListFromReference.GetReference(prop3d.Value.Guid,
          ReferenceType.Prop3d, null, null, null, apiProp3ds, null, null, null);
        if (id == "") {
          owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning,
            "Issue adding List " + copyList.Name + " to Model:" + Environment.NewLine
            + prop3d.Value.ToString() + " not found in Model");
        }

        ids.Add(id);
      }

      copyList._definition = string.Join(" ", ids);
      return copyList;
    }

    private static void AddElementList(GsaList list, ref GsaGuidDictionary<EntityList> apiLists,
      GsaGuidIntListDictionary<Element> apiElements, GsaGuidDictionary<Member> apiMembers,
      ConcurrentDictionary<int, ConcurrentBag<int>> memberElementRelationship, GH_Component owner) {
      GsaList copyList = list.Duplicate();

      var ids = new Collection<string>();

      if (copyList._elements != (null, null, null)) {
        foreach (GsaElement1dGoo element1d in copyList._elements.e1d
          .Where(x => x != null && x.Value != null)) {
          string id = ElementListFromReference.GetReference(element1d.Value.Guid,
            ReferenceType.Element, null, null, null, null, apiElements, null, null);
          if (id == "") {
            owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning,
              "Issue adding List " + copyList.Name + " to Model:" + Environment.NewLine
              + element1d.Value.ToString() + " not found in Model");
          }

          ids.Add(id);
        }

        foreach (GsaElement2dGoo element2d in copyList._elements.e2d
          .Where(x => x != null && x.Value != null)) {
          string id = ElementListFromReference.GetReference(element2d.Value.Guid,
            ReferenceType.Element, null, null, null, null, apiElements, null, null);
          if (id == "") {
            owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning,
              "Issue adding List " + copyList.Name + " to Model:" + Environment.NewLine
              + element2d.Value.ToString() + " not found in Model");
          }

          ids.Add(id);
        }

        foreach (GsaElement3dGoo element3d in copyList._elements.e3d
          .Where(x => x != null && x.Value != null)) {
          string id = ElementListFromReference.GetReference(element3d.Value.Guid,
            ReferenceType.Element, null, null, null, null, apiElements, null, null);
          if (id == "") {
            owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning,
              "Issue adding List " + copyList.Name + " to Model:" + Environment.NewLine
              + element3d.Value.ToString() + " not found in Model");
          }

          ids.Add(id);
        }
      }

      if (copyList._members != (null, null, null)) {
        foreach (GsaMember1dGoo member1d in copyList._members.m1d
          .Where(x => x != null && x.Value != null)) {
          string id = ElementListFromReference.GetReference(member1d.Value.Guid,
            ReferenceType.MemberChildElements, null, null, null, null, null, apiMembers,
            memberElementRelationship);
          if (id == "") {
            owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning,
              "Issue adding List " + copyList.Name + " to Model:" + Environment.NewLine
              + member1d.Value.ToString() + " not found in Model");
          }

          ids.Add(id);
        }

        foreach (GsaMember2dGoo member2d in copyList._members.m2d
          .Where(x => x != null && x.Value != null)) {
          string id = ElementListFromReference.GetReference(member2d.Value.Guid,
            ReferenceType.MemberChildElements, null, null, null, null, null, apiMembers,
            memberElementRelationship);
          if (id == "") {
            owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning,
              "Issue adding List " + copyList.Name + " to Model:" + Environment.NewLine
              + member2d.Value.ToString() + " not found in Model");
          }

          ids.Add(id);
        }

        foreach (GsaMember3dGoo member3d in copyList._members.m3d
          .Where(x => x != null && x.Value != null)) {
          string id = ElementListFromReference.GetReference(member3d.Value.Guid,
            ReferenceType.MemberChildElements, null, null, null, null, null, apiMembers,
            memberElementRelationship);
          if (id == "") {
            owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning,
              "Issue adding List " + copyList.Name + " to Model:" + Environment.NewLine
              + member3d.Value.ToString() + " not found in Model");
          }

          ids.Add(id);
        }
      }

      copyList._definition += string.Join(" ", ids);

      AddList(copyList, ref apiLists);
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
        string id = ElementListFromReference.GetReference(member1d.Value.Guid,
          ReferenceType.Member, null, null, null, null, null, apiMembers, null);
        if (id == "") {
          owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning,
            "Issue adding List " + copyList.Name + " to Model:" + Environment.NewLine
            + member1d.Value.ToString() + " not found in Model");
        }

        ids.Add(id);
      }

      foreach (GsaMember2dGoo member2d in copyList._members.m2d
        .Where(x => x != null && x.Value != null)) {
        string id = ElementListFromReference.GetReference(member2d.Value.Guid,
          ReferenceType.Member, null, null, null, null, null, apiMembers, null);
        if (id == "") {
          owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning,
            "Issue adding List " + copyList.Name + " to Model:" + Environment.NewLine
            + member2d.Value.ToString() + " not found in Model");
        }

        ids.Add(id);
      }

      foreach (GsaMember3dGoo member3d in copyList._members.m3d
        .Where(x => x != null && x.Value != null)) {
        string id = ElementListFromReference.GetReference(member3d.Value.Guid,
          ReferenceType.Member, null, null, null, null, null, apiMembers, null);
        if (id == "") {
          owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning,
            "Issue adding List " + copyList.Name + " to Model:" + Environment.NewLine
            + member3d.Value.ToString() + " not found in Model");
        }

        ids.Add(id);
      }

      copyList._definition += string.Join(" ", ids);

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
