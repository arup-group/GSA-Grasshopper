using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using GsaAPI;
using GsaGH.Parameters;
using OasysUnits.Units;

namespace GsaGH.Helpers.Export {
  internal class Lists {
    internal static string GetElementList(GsaList list, ref GsaGuidDictionary<EntityList> apiLists,
      GsaGuidDictionary<AnalysisMaterial> apiMaterials, GsaGuidDictionary<Section> apiSections,
      GsaGuidDictionary<Prop2D> apiProp2ds, GsaGuidDictionary<Prop3D> apiProp3ds,
      GsaGuidIntListDictionary<Element> apiElements, GsaGuidDictionary<Member> apiMembers,
      ConcurrentDictionary<int, ConcurrentBag<int>> memberElementRelationship, GH_Component owner) {
      if (apiLists.GuidDictionary.TryGetValue(list.Guid, out int id)) {
        return "\"" + apiLists.ReadOnlyDictionary[id].Name + "\"";
      }

      if (list.EntityType == Parameters.EntityType.Member) {
        list = list.Duplicate();
        list.EntityType = Parameters.EntityType.Element;
      }

      AddPropertiesList(ref list, apiMaterials, apiSections, apiProp2ds, apiProp3ds, owner);
      AddElementList(list, ref apiLists, apiElements, apiMembers, memberElementRelationship, owner);

      return "\"" + list.Name + "\"";
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

      switch (list.EntityType) {
        case Parameters.EntityType.Element:
          AddPropertiesList(ref list, apiMaterials, apiSections, apiProp2ds, apiProp3ds, owner);
          AddElementList(list, ref apiLists, apiElements, apiMembers, memberElementRelationship, owner);
          break;

        case Parameters.EntityType.Member:
          AddPropertiesList(ref list, apiMaterials, apiSections, apiProp2ds, apiProp3ds, owner);
          AddMemberList(list, ref apiLists, apiMembers, owner);
          break;

        case Parameters.EntityType.Case:
          list._definition += string.Join(" ", list._cases);
          AddList(list, ref apiLists);
          break;
      }
    }

    private static void AddNodeList(GsaList list, ref GsaGuidDictionary<EntityList> apiLists,
      ref GsaIntDictionary<Node> apiNodes, LengthUnit unit) {
      if (list._nodes == null || list._nodes.Count == 0) {
        AddList(list, ref apiLists);
      }

      foreach (GsaNodeGoo node in list._nodes.Where(x => x != null && x.Value != null)) {
        list._definition +=
          " " + Nodes.AddNode(ref apiNodes, node.Value.GetApiNodeToUnit(unit)).ToString();
      }

      list._definition.Trim();

      AddList(list, ref apiLists);
    }

    private static void AddPropertiesList(ref GsaList list,
      GsaGuidDictionary<AnalysisMaterial> apiMaterials, GsaGuidDictionary<Section> apiSections,
      GsaGuidDictionary<Prop2D> apiProp2ds, GsaGuidDictionary<Prop3D> apiProp3ds,
      GH_Component owner) {
      // the list is passed in as 'ref' to simply update the '_definition' in this method
      // and let either AddElementList or AddMemberList add the list in the apiList dictionary

      if (list._properties == (null, null, null, null) || list._properties.materials.Count == 0
        || list._properties.sections.Count == 0 || list._properties.prop2ds.Count == 0
        || list._properties.prop3ds.Count == 0) {
        return;
      }

      foreach (GsaMaterialGoo material in list._properties.materials
        .Where(x => x != null && x.Value != null)) {
        string id = ElementListFromReference.GetReference(material.Value.Guid,
          ReferenceType.Material, apiMaterials, null, null, null, null, null, null);
        if (id == "") {
          owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning,
            "Issue adding List " + list.Name + " to Model:" + System.Environment.NewLine
            + "Material " + material.Value.ToString() + " not found in Model");
        }

        list._definition += " " + id;
      }

      foreach (GsaSectionGoo section in list._properties.sections
        .Where(x => x != null && x.Value != null)) {
        string id = ElementListFromReference.GetReference(section.Value.Guid,
          ReferenceType.Section, null, apiSections, null, null, null, null, null);
        if (id == "") {
          owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning,
            "Issue adding List " + list.Name + " to Model:" + Environment.NewLine
            + section.Value.ToString() + " not found in Model");
        }

        list._definition += " " + id;
      }

      foreach (GsaProp2dGoo prop2d in list._properties.prop2ds
        .Where(x => x != null && x.Value != null)) {
        string id = ElementListFromReference.GetReference(prop2d.Value.Guid,
          ReferenceType.Prop2d, null, null, apiProp2ds, null, null, null, null);
        if (id == "") {
          owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning,
            "Issue adding List " + list.Name + " to Model:" + Environment.NewLine
            + prop2d.Value.ToString() + " not found in Model");
        }

        list._definition += " " + id;
      }

      foreach (GsaProp3dGoo prop3d in list._properties.prop3ds
        .Where(x => x != null && x.Value != null)) {
        string id = ElementListFromReference.GetReference(prop3d.Value.Guid,
          ReferenceType.Prop3d, null, null, null, apiProp3ds, null, null, null);
        if (id == "") {
          owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning,
            "Issue adding List " + list.Name + " to Model:" + Environment.NewLine
            + prop3d.Value.ToString() + " not found in Model");
        }

        list._definition += " " + id;
      }

      list._definition.Trim();
    }

    private static void AddElementList(GsaList list, ref GsaGuidDictionary<EntityList> apiLists,
      GsaGuidIntListDictionary<Element> apiElements, GsaGuidDictionary<Member> apiMembers,
      ConcurrentDictionary<int, ConcurrentBag<int>> memberElementRelationship, GH_Component owner) {
      if (list._elements != (null, null, null)) {
        foreach (GsaElement1dGoo element1d in list._elements.e1d
          .Where(x => x != null && x.Value != null)) {
          string id = ElementListFromReference.GetReference(element1d.Value.Guid,
            ReferenceType.Element, null, null, null, null, apiElements, null, null);
          if (id == "") {
            owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning,
              "Issue adding List " + list.Name + " to Model:" + Environment.NewLine
              + element1d.Value.ToString() + " not found in Model");
          }

          list._definition += " " + id;
        }

        foreach (GsaElement2dGoo element2d in list._elements.e2d
          .Where(x => x != null && x.Value != null)) {
          string id = ElementListFromReference.GetReference(element2d.Value.Guid,
            ReferenceType.Element, null, null, null, null, apiElements, null, null);
          if (id == "") {
            owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning,
              "Issue adding List " + list.Name + " to Model:" + Environment.NewLine
              + element2d.Value.ToString() + " not found in Model");
          }

          list._definition += " " + id;
        }

        foreach (GsaElement3dGoo element3d in list._elements.e3d
          .Where(x => x != null && x.Value != null)) {
          string id = ElementListFromReference.GetReference(element3d.Value.Guid,
            ReferenceType.Element, null, null, null, null, apiElements, null, null);
          if (id == "") {
            owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning,
              "Issue adding List " + list.Name + " to Model:" + Environment.NewLine
              + element3d.Value.ToString() + " not found in Model");
          }

          list._definition += " " + id;
        }
      }

      if (list._members != (null, null, null)) {
        foreach (GsaMember1dGoo member1d in list._members.m1d
          .Where(x => x != null && x.Value != null)) {
          string id = ElementListFromReference.GetReference(member1d.Value.Guid,
            ReferenceType.MemberChildElements, null, null, null, null, null, apiMembers,
            memberElementRelationship);
          if (id == "") {
            owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning,
              "Issue adding List " + list.Name + " to Model:" + Environment.NewLine
              + member1d.Value.ToString() + " not found in Model");
          }

          list._definition += " " + id;
        }

        foreach (GsaMember2dGoo member2d in list._members.m2d
          .Where(x => x != null && x.Value != null)) {
          string id = ElementListFromReference.GetReference(member2d.Value.Guid,
            ReferenceType.MemberChildElements, null, null, null, null, null, apiMembers,
            memberElementRelationship);
          if (id == "") {
            owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning,
              "Issue adding List " + list.Name + " to Model:" + Environment.NewLine
              + member2d.Value.ToString() + " not found in Model");
          }

          list._definition += " " + id;
        }

        foreach (GsaMember3dGoo member3d in list._members.m3d
          .Where(x => x != null && x.Value != null)) {
          string id = ElementListFromReference.GetReference(member3d.Value.Guid,
            ReferenceType.MemberChildElements, null, null, null, null, null, apiMembers,
            memberElementRelationship);
          if (id == "") {
            owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning,
              "Issue adding List " + list.Name + " to Model:" + Environment.NewLine
              + member3d.Value.ToString() + " not found in Model");
          }

          list._definition += " " + id;
        }
      }

      list._definition.Trim();

      AddList(list, ref apiLists);
    }

    private static void AddMemberList(GsaList list, ref GsaGuidDictionary<EntityList> apiLists,
      GsaGuidDictionary<Member> apiMembers, GH_Component owner) {
      if (list._members == (null, null, null) || list._members.m1d.Count == 0
        || list._members.m2d.Count == 0 || list._members.m3d.Count == 0) {
        AddList(list, ref apiLists);
      }

      foreach (GsaMember1dGoo member1d in list._members.m1d
        .Where(x => x != null && x.Value != null)) {
        string id = ElementListFromReference.GetReference(member1d.Value.Guid,
          ReferenceType.Member, null, null, null, null, null, apiMembers, null);
        if (id == "") {
          owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning,
            "Issue adding List " + list.Name + " to Model:" + Environment.NewLine
            + member1d.Value.ToString() + " not found in Model");
        }

        list._definition += " " + id;
      }

      foreach (GsaMember2dGoo member2d in list._members.m2d
        .Where(x => x != null && x.Value != null)) {
        string id = ElementListFromReference.GetReference(member2d.Value.Guid,
          ReferenceType.Member, null, null, null, null, null, apiMembers, null);
        if (id == "") {
          owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning,
            "Issue adding List " + list.Name + " to Model:" + Environment.NewLine
            + member2d.Value.ToString() + " not found in Model");
        }

        list._definition += " " + id;
      }

      foreach (GsaMember3dGoo member3d in list._members.m3d
        .Where(x => x != null && x.Value != null)) {
        string id = ElementListFromReference.GetReference(member3d.Value.Guid,
          ReferenceType.Member, null, null, null, null, null, apiMembers, null);
        if (id == "") {
          owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning,
            "Issue adding List " + list.Name + " to Model:" + Environment.NewLine
            + member3d.Value.ToString() + " not found in Model");
        }

        list._definition += " " + id;
      }

      list._definition.Trim();

      AddList(list, ref apiLists);
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
