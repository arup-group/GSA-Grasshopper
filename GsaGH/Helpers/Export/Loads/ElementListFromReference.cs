using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using GsaAPI;
using GsaGH.Parameters;

namespace GsaGH.Helpers.Export {
  internal class ElementListFromReference {
    internal static ConcurrentDictionary<int, ConcurrentBag<int>> GetMemberElementRelationship(Model model) {
      var relationships = new ConcurrentDictionary<int, ConcurrentBag<int>>();
      Parallel.ForEach(model.Elements(), item => relationships.GetOrAdd(item.Value.ParentMember.Member, new ConcurrentBag<int>()).Add(item.Key));
      return relationships;
    }

    internal static string GetRefElementIds(GsaGridPlaneSurface load, GsaGuidDictionary<Section> apiSections, GsaGuidDictionary<Prop2D> apiProp2ds, GsaGuidDictionary<Prop3D> apiProp3ds, GsaGuidIntListDictionary<Element> apiElements, GsaGuidDictionary<Member> apiMembers, ConcurrentDictionary<int, ConcurrentBag<int>> memberElementRelationship) => GetReference(load._refObjectGuid, load._referenceType, apiSections, apiProp2ds, apiProp3ds, apiElements,
         apiMembers, memberElementRelationship);

    internal static string GetRefElementIds(GsaGravityLoad load, GsaGuidDictionary<Section> apiSections, GsaGuidDictionary<Prop2D> apiProp2ds, GsaGuidDictionary<Prop3D> apiProp3ds, GsaGuidIntListDictionary<Element> apiElements, GsaGuidDictionary<Member> apiMembers, ConcurrentDictionary<int, ConcurrentBag<int>> memberElementRelationship) => GetReference(load._refObjectGuid, load._referenceType, apiSections, apiProp2ds, apiProp3ds, apiElements,
         apiMembers, memberElementRelationship);

    internal static string GetRefElementIds(GsaFaceLoad load, GsaGuidDictionary<Prop2D> apiProp2ds, GsaGuidIntListDictionary<Element> apiElements, GsaGuidDictionary<Member> apiMembers, ConcurrentDictionary<int, ConcurrentBag<int>> memberElementRelationship) => GetReference(load._refObjectGuid, load._referenceType, null, apiProp2ds, null, apiElements,
         apiMembers, memberElementRelationship);

    internal static string GetRefElementIds(GsaBeamLoad load, GsaGuidDictionary<Section> apiSections, GsaGuidIntListDictionary<Element> apiElements, GsaGuidDictionary<Member> apiMembers, ConcurrentDictionary<int, ConcurrentBag<int>> memberElementRelationship) => GetReference(load._refObjectGuid, load._referenceType, apiSections, null, null, apiElements,
         apiMembers, memberElementRelationship);

    private static string GetElementRef<T>(Guid guid, GsaGuidIntListDictionary<T> dictionary) => dictionary.GuidDictionary.TryGetValue(guid, out Collection<int> ids)
        ? string.Join(" ", ids)
        : "";

    private static string GetMemberRef<T>(Guid guid, GsaGuidDictionary<T> dictionary, ConcurrentDictionary<int, ConcurrentBag<int>> memberElementRelationship) => !dictionary.GuidDictionary.TryGetValue(guid, out int id)
        ? ""
        : memberElementRelationship.TryGetValue(id, out ConcurrentBag<int> ids)
        ? string.Join(" ", ids)
        : "";

    private static string GetRef<T>(Guid guid, GsaGuidDictionary<T> dictionary) {
      if (dictionary.GuidDictionary.TryGetValue(guid, out int id)) {
        string t = string.Empty;
        if (typeof(T) == typeof(Section))
          t = "PB";
        if (typeof(T) == typeof(Prop2D))
          t = "PA";
        if (typeof(T) == typeof(Prop3D))
          t = "PV";
        return t + id;
      }
      else
        return "";
    }

    private static string GetReference(Guid guid, ReferenceType referenceType, GsaGuidDictionary<Section> apiSections, GsaGuidDictionary<Prop2D> apiProp2ds, GsaGuidDictionary<Prop3D> apiProp3ds, GsaGuidIntListDictionary<Element> apiElements,
                    GsaGuidDictionary<Member> apiMembers, ConcurrentDictionary<int, ConcurrentBag<int>> memberElementRelationship) {
      switch (referenceType) {
        case ReferenceType.Section:
          return GetRef(guid, apiSections);

        case ReferenceType.Prop2d:
          return GetRef(guid, apiProp2ds);

        case ReferenceType.Prop3d:
          return GetRef(guid, apiProp3ds);

        case ReferenceType.Element:
          return GetElementRef(guid, apiElements);

        case ReferenceType.Member:
          return GetMemberRef(guid, apiMembers, memberElementRelationship);

        default:
        case ReferenceType.None:
          return "";
      }
    }
  }
}
