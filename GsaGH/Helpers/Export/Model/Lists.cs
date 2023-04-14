using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using GsaAPI;
using GsaGH.Helpers.Import;
using GsaGH.Parameters;
using OasysUnits.Units;

namespace GsaGH.Helpers.Export {
  internal class Lists {
    internal static void ConvertList(
      List<GsaList> lists, ref GsaGuidDictionary<EntityList> apiLists,
      ref GsaGuidDictionary<AnalysisMaterial> apiMaterials,
      ref GsaGuidDictionary<Section> apiSections,
      ref GsaIntDictionary<SectionModifier> apiSectionModifiers,
      ref GsaGuidDictionary<Prop2D> apiProp2ds,
      ref GsaGuidDictionary<Prop3D> apiProp3ds,
      ref GsaIntDictionary<Node> apiNodes,
      ref GsaGuidIntListDictionary<Element> apiElements,
      ref GsaGuidDictionary<Member> apiMembers, LengthUnit modelUnit) {
      if (lists == null) {
        return;
      }

      lists = lists.OrderByDescending(x => x.Id).ToList();
      foreach (GsaList list in lists.Where(list => list != null)) {
        ConvertList(list, ref apiLists, ref apiMaterials, ref apiSections, ref apiSectionModifiers, ref apiProp2ds, ref apiProp3ds, ref apiNodes, ref apiElements, ref apiMembers, modelUnit);
      }
    }

    internal static void ConvertList(
      GsaList list, ref GsaGuidDictionary<EntityList> apiLists,
      ref GsaGuidDictionary<AnalysisMaterial> apiMaterials,
      ref GsaGuidDictionary<Section> apiSections,
      ref GsaIntDictionary<SectionModifier> apiSectionModifiers,
      ref GsaGuidDictionary<Prop2D> apiProp2ds,
      ref GsaGuidDictionary<Prop3D> apiProp3ds,
      ref Dictionary<int, Axis> apiAxes,
      ref GsaIntDictionary<Node> apiNodes,
      ref GsaGuidIntListDictionary<Element> apiElements,
      ref GsaGuidDictionary<Member> apiMembers, LengthUnit modelUnit) {
      if (list == null) {
        return;
      }

      switch (list.EntityType) {
        case Parameters.EntityType.Node:
          AddNodeList(list, ref apiLists, ref apiNodes, modelUnit);
          return;

        case Parameters.EntityType.Element:
          AddPropertiesList(ref list, ref apiMaterials, ref apiSections,
            ref apiSectionModifiers, ref apiProp2ds, ref apiProp3ds, ref apiAxes, modelUnit);
          AddElementList(list, apiLists, )
          break;

        case Parameters.EntityType.Member:
          AddPropertiesList(ref list, ref apiMaterials, ref apiSections,
            ref apiSectionModifiers, ref apiProp2ds, ref apiProp3ds, ref apiAxes, modelUnit);

          break;

        case Parameters.EntityType.Case:
          if (_cases != null) {
            dup._cases = new List<int>(_cases);
          }

          break;
      }
    }

    private static void AddNodeList(GsaList list, ref GsaGuidDictionary<EntityList> apiLists, 
      ref GsaIntDictionary<Node> apiNodes, LengthUnit unit) {
      if (list._nodes == null || list._nodes.Count == 0) { 
        AddList(list, ref apiLists);
      }

      foreach (GsaNodeGoo node in list._nodes.Where(x => x != null && x.Value != null)) {
        list._definition += " " + Nodes.AddNode(ref apiNodes, node.Value.GetApiNodeToUnit(unit)).ToString();
      }
      
      list._definition.Trim();
      
      AddList(list, ref apiLists);
    }

    private static void AddPropertiesList(ref GsaList list,
      ref GsaGuidDictionary<AnalysisMaterial> apiMaterials, 
      ref GsaGuidDictionary<Section> apiSections, 
      ref GsaIntDictionary<SectionModifier> apiSectionModifiers, 
      ref GsaGuidDictionary<Prop2D> apiProp2ds, ref GsaGuidDictionary<Prop3D> apiProp3ds,
      ref Dictionary<int, Axis> apiAxes, LengthUnit unit) {
      // the list is passed in as 'ref' to simply update the '_definition' in this method
      // and let either AddElementList or AddMemberList add the list in the apiList dictionary

      if (list._properties == (null, null, null, null) || list._properties.materials.Count == 0
        || list._properties.sections.Count == 0 || list._properties.prop2ds.Count == 0
        || list._properties.prop3ds.Count == 0) {
        return;
      }

      foreach (GsaMaterialGoo material in list._properties.materials
        .Where(x => x != null && x.Value != null)) {
        list._definition += " M" + Materials.AddMaterial(
          material.Value, ref apiMaterials).ToString();
      }

      foreach (GsaSectionGoo section in list._properties.sections
        .Where(x => x != null && x.Value != null)) {
        list._definition += " PB" + Sections.AddSection(
          section.Value, ref apiSections, ref apiSectionModifiers, ref apiMaterials).ToString();
      }

      foreach (GsaProp2dGoo prop2d in list._properties.prop2ds
        .Where(x => x != null && x.Value != null)) {
        list._definition += " PA" + Prop2ds.AddProp2d(
          prop2d.Value, ref apiProp2ds, ref apiMaterials, ref apiAxes, unit).ToString();
      }

      foreach (GsaProp3dGoo prop3d in list._properties.prop3ds
        .Where(x => x != null && x.Value != null)) {
        list._definition += " PV" + Prop3ds.AddProp3d(
          prop3d.Value, ref apiProp3ds, ref apiMaterials).ToString();
      }

      list._definition.Trim();
    }

    private static void AddElementList(GsaList list, ref GsaGuidDictionary<EntityList> apiLists, ref GsaGuidIntListDictionary<Element> apiElements,
      ref GsaIntDictionary<Node> existingNodes, LengthUnit unit,
      ref GsaGuidDictionary<Section> apiSections,
      ref GsaIntDictionary<SectionModifier> apiSectionModifiers,
      ref GsaGuidDictionary<AnalysisMaterial> apiMaterials) {
      if (list._elements == (null, null, null) || list._elements.e1d.Count == 0
        || list._elements.e2d.Count == 0 || list._elements.e3d.Count == 0) {
        AddList(list, ref apiLists);
      }

      foreach (GsaElement1dGoo element1d in list._elements.e1d
        .Where(x => x != null && x.Value != null)) {
        if ()
        list._definition += " " + Elements.Ad.AddNode(ref apiNodes, node.Value.GetApiNodeToUnit(unit)).ToString();
      }

      list._definition.Trim();

      AddList(list, ref apiLists);
    }

    private static void AddList(GsaList list, ref GsaGuidDictionary<EntityList> apiLists) {
      if (list.Id > 0) {
        apiLists.SetValue(list.Id, list.Guid, list.GetApiList());
      } else {
        apiLists.AddValue(list.Guid, list.GetApiList());
      }
    }
  }
}
