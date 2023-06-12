using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using GsaAPI;
using GsaGH.Parameters;

namespace GsaGH.Helpers.Export {
  internal class ElementListFromReference {

    internal static ConcurrentDictionary<int, ConcurrentBag<int>> GetMemberElementRelationship(
      Model model) {
      var relationships = new ConcurrentDictionary<int, ConcurrentBag<int>>();
      Parallel.ForEach(model.Elements(),
        item => relationships.GetOrAdd(item.Value.ParentMember.Member, new ConcurrentBag<int>())
         .Add(item.Key));
      return relationships;
    }

    internal static string GetReferenceElementIdsDefinition(
      GsaGridPlaneSurface load, GsaGuidDictionary<AnalysisMaterial> apiMaterials,
      GsaGuidDictionary<Section> apiSections, GsaGuidDictionary<Prop2D> apiProp2ds,
      GsaGuidDictionary<Prop3D> apiProp3ds, GsaGuidIntListDictionary<Element> apiElements,
      GsaGuidDictionary<Member> apiMembers,
      ConcurrentDictionary<int, ConcurrentBag<int>> memberElementRelationship) {
      return GetReferenceDefinition(load._refObjectGuid, load._referenceType, apiMaterials, apiSections,
        apiProp2ds, apiProp3ds, apiElements, apiMembers, memberElementRelationship);
    }

    internal static string GetReferenceElementIdsDefinition(
      GsaGravityLoad load, GsaGuidDictionary<AnalysisMaterial> apiMaterials,
      GsaGuidDictionary<Section> apiSections, GsaGuidDictionary<Prop2D> apiProp2ds,
      GsaGuidDictionary<Prop3D> apiProp3ds, GsaGuidIntListDictionary<Element> apiElements,
      GsaGuidDictionary<Member> apiMembers,
      ConcurrentDictionary<int, ConcurrentBag<int>> memberElementRelationship) {
      return GetReferenceDefinition(load._refObjectGuid, load._referenceType, apiMaterials, apiSections,
        apiProp2ds, apiProp3ds, apiElements, apiMembers, memberElementRelationship);
    }

    internal static string GetReferenceElementIdsDefinition(
      GsaFaceLoad load, GsaGuidDictionary<AnalysisMaterial> apiMaterials,
      GsaGuidDictionary<Prop2D> apiProp2ds, GsaGuidIntListDictionary<Element> apiElements,
      GsaGuidDictionary<Member> apiMembers,
      ConcurrentDictionary<int, ConcurrentBag<int>> memberElementRelationship) {
      return GetReferenceDefinition(load._refObjectGuid, load._referenceType, apiMaterials, null,
        apiProp2ds, null, apiElements, apiMembers, memberElementRelationship);
    }

    internal static string GetReferenceElementIdsDefinition(
      GsaBeamLoad load, GsaGuidDictionary<AnalysisMaterial> apiMaterials,
      GsaGuidDictionary<Section> apiSections, GsaGuidIntListDictionary<Element> apiElements,
      GsaGuidDictionary<Member> apiMembers,
      ConcurrentDictionary<int, ConcurrentBag<int>> memberElementRelationship) {
      return GetReferenceDefinition(load._refObjectGuid, load._referenceType, apiMaterials, apiSections,
        null, null, apiElements, apiMembers, memberElementRelationship);
    }

    internal static string GetMemberChildElementReferenceIdsDefinition(
      int memberId, ConcurrentDictionary<int, ConcurrentBag<int>> memberElementRelationship) {
      return memberElementRelationship.TryGetValue(memberId, out ConcurrentBag<int> ids) ?
          string.Join(" ", ids) :
          string.Empty;
    }

    private static string GetElementsReferenceDefinition<T>(Guid guid, GsaGuidIntListDictionary<T> dictionary) {
      return dictionary.GuidDictionary.TryGetValue(guid, out Collection<int> ids) ?
        string.Join(" ", ids) : string.Empty;
    }

    private static string GetMemberChildElementsReferenceDefinition<T>(
      Guid guid, GsaGuidDictionary<T> dictionary,
      ConcurrentDictionary<int, ConcurrentBag<int>> memberElementRelationship) {
      return !dictionary.GuidDictionary.TryGetValue(guid, out int id) ?
        string.Empty :
        GetMemberChildElementReferenceIdsDefinition(id, memberElementRelationship);
    }
    

    private static string GetReferenceType<T>(Guid guid, GsaGuidDictionary<T> dictionary) {
      if (dictionary.GuidDictionary.TryGetValue(guid, out int id)) {
        string t = string.Empty;
        if (typeof(T) == typeof(Section)) {
          t = "PB";
        }

        if (typeof(T) == typeof(Prop2D)) {
          t = "PA";
        }

        if (typeof(T) == typeof(Prop3D)) {
          t = "PV";
        }

        if (typeof(T) == typeof(AnalysisMaterial)) {
          t = "M";
        }

        return t + id;
      } else {
        return string.Empty;
      }
    }

    internal static string GetReferenceDefinition(
      Guid guid, ReferenceType referenceType, GsaGuidDictionary<AnalysisMaterial> apiMaterials,
      GsaGuidDictionary<Section> apiSections, GsaGuidDictionary<Prop2D> apiProp2ds,
      GsaGuidDictionary<Prop3D> apiProp3ds, GsaGuidIntListDictionary<Element> apiElements,
      GsaGuidDictionary<Member> apiMembers,
      ConcurrentDictionary<int, ConcurrentBag<int>> memberElementRelationship) {
      switch (referenceType) {
        case ReferenceType.Material:
          return GetReferenceType(guid, apiMaterials);

        case ReferenceType.Section:
          return GetReferenceType(guid, apiSections);

        case ReferenceType.Prop2d:
          return GetReferenceType(guid, apiProp2ds);

        case ReferenceType.Prop3d:
          return GetReferenceType(guid, apiProp3ds);

        case ReferenceType.Element:
          return GetElementsReferenceDefinition(guid, apiElements);

        case ReferenceType.MemberChildElements:
          return GetMemberChildElementsReferenceDefinition(guid, apiMembers, memberElementRelationship);

        case ReferenceType.Member:
          return GetReferenceType(guid, apiMembers);

        case ReferenceType.None:
        default:
          return string.Empty;
      }
    }
  }
}
