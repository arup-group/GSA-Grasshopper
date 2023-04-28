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

    internal static string GetRefElementIds(
      GsaGridPlaneSurface load, GsaGuidDictionary<AnalysisMaterial> apiMaterials,
      GsaGuidDictionary<Section> apiSections, GsaGuidDictionary<Prop2D> apiProp2ds,
      GsaGuidDictionary<Prop3D> apiProp3ds, GsaGuidIntListDictionary<Element> apiElements,
      GsaGuidDictionary<Member> apiMembers,
      ConcurrentDictionary<int, ConcurrentBag<int>> memberElementRelationship) {
      return GetReference(load._refObjectGuid, load._referenceType, apiMaterials, apiSections,
        apiProp2ds, apiProp3ds, apiElements, apiMembers, memberElementRelationship);
    }

    internal static string GetRefElementIds(
      GsaGravityLoad load, GsaGuidDictionary<AnalysisMaterial> apiMaterials,
      GsaGuidDictionary<Section> apiSections, GsaGuidDictionary<Prop2D> apiProp2ds,
      GsaGuidDictionary<Prop3D> apiProp3ds, GsaGuidIntListDictionary<Element> apiElements,
      GsaGuidDictionary<Member> apiMembers,
      ConcurrentDictionary<int, ConcurrentBag<int>> memberElementRelationship) {
      return GetReference(load._refObjectGuid, load._referenceType, apiMaterials, apiSections,
        apiProp2ds, apiProp3ds, apiElements, apiMembers, memberElementRelationship);
    }

    internal static string GetRefElementIds(
      GsaFaceLoad load, GsaGuidDictionary<AnalysisMaterial> apiMaterials,
      GsaGuidDictionary<Prop2D> apiProp2ds, GsaGuidIntListDictionary<Element> apiElements,
      GsaGuidDictionary<Member> apiMembers,
      ConcurrentDictionary<int, ConcurrentBag<int>> memberElementRelationship) {
      return GetReference(load._refObjectGuid, load._referenceType, apiMaterials, null,
        apiProp2ds, null, apiElements, apiMembers, memberElementRelationship);
    }

    internal static string GetRefElementIds(
      GsaBeamLoad load, GsaGuidDictionary<AnalysisMaterial> apiMaterials,
      GsaGuidDictionary<Section> apiSections, GsaGuidIntListDictionary<Element> apiElements,
      GsaGuidDictionary<Member> apiMembers,
      ConcurrentDictionary<int, ConcurrentBag<int>> memberElementRelationship) {
      return GetReference(load._refObjectGuid, load._referenceType, apiMaterials, apiSections,
        null, null, apiElements, apiMembers, memberElementRelationship);
    }

    private static string GetElementRef<T>(Guid guid, GsaGuidIntListDictionary<T> dictionary) {
      return dictionary.GuidDictionary.TryGetValue(guid, out Collection<int> ids) ?
        string.Join(" ", ids) : string.Empty;
    }

    private static string GetMemberChildElementsRef<T>(
      Guid guid, GsaGuidDictionary<T> dictionary,
      ref ConcurrentDictionary<int, ConcurrentBag<int>> memberElementRelationship) {
      return !dictionary.GuidDictionary.TryGetValue(guid, out int id) ?
        string.Empty :
        memberElementRelationship.TryGetValue(id, out ConcurrentBag<int> ids) ?
          string.Join(" ", ids) :
          string.Empty;
    }

    private static string GetRef<T>(Guid guid, GsaGuidDictionary<T> dictionary) {
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

    internal static string GetReference(
      Guid guid, ReferenceType referenceType, GsaGuidDictionary<AnalysisMaterial> apiMaterials,
      GsaGuidDictionary<Section> apiSections, GsaGuidDictionary<Prop2D> apiProp2ds,
      GsaGuidDictionary<Prop3D> apiProp3ds, GsaGuidIntListDictionary<Element> apiElements,
      GsaGuidDictionary<Member> apiMembers,
      ConcurrentDictionary<int, ConcurrentBag<int>> memberElementRelationship) {
      switch (referenceType) {
        case ReferenceType.Material:
          return GetRef(guid, apiMaterials);

        case ReferenceType.Section:
          return GetRef(guid, apiSections);

        case ReferenceType.Prop2d:
          return GetRef(guid, apiProp2ds);

        case ReferenceType.Prop3d:
          return GetRef(guid, apiProp3ds);

        case ReferenceType.Element:
          return GetElementRef(guid, apiElements);

        case ReferenceType.MemberChildElements:
          return GetMemberChildElementsRef(guid, apiMembers, ref memberElementRelationship);

        case ReferenceType.Member:
          return GetRef(guid, apiMembers);

        case ReferenceType.None:
        default:
          return string.Empty;
      }
    }
  }
}
