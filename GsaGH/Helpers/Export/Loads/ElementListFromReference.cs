using GsaAPI;
using GsaGH.Parameters;
using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

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
      GsaGridPlaneSurface load, ModelAssembly model) {
      return GetReferenceDefinition(
        load._refObjectGuid,
        load._referenceType,
        model.Properties,
        model.Elements,
        model.Members,
        model.MemberElementRelationship);
    }

    internal static string GetReferenceElementIdsDefinition(
      GsaGravityLoad load, ModelAssembly model) {
      return GetReferenceDefinition(
        load._refObjectGuid,
        load._referenceType,
        model.Properties,
        model.Elements,
        model.Members,
        model.MemberElementRelationship);
    }

    internal static string GetReferenceElementIdsDefinition(
      GsaFaceLoad load, ModelAssembly model) {
      return GetReferenceDefinition(
        load._refObjectGuid,
        load._referenceType,
        model.Properties,
        model.Elements,
        model.Members,
        model.MemberElementRelationship);
    }

    internal static string GetReferenceElementIdsDefinition(
      GsaBeamLoad load, ModelAssembly model) {
      return GetReferenceDefinition(
        load._refObjectGuid,
        load._referenceType,
        model.Properties,
        model.Elements,
        model.Members,
        model.MemberElementRelationship);
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

    internal static string GetReferenceDefinition(
      Guid guid,
      ReferenceType referenceType,
      Properties apiProperties,
      GsaGuidIntListDictionary<Element> apiElements,
      GsaGuidDictionary<Member> apiMembers,
      ConcurrentDictionary<int, ConcurrentBag<int>> memberElementRelationship) {
      switch (referenceType) {
        case ReferenceType.Property:
          return apiProperties.GetReferenceDefinition(guid);

        case ReferenceType.Element:
          return GetElementsReferenceDefinition(guid, apiElements);

        case ReferenceType.MemberChildElements:
          return GetMemberChildElementsReferenceDefinition(guid, apiMembers, memberElementRelationship);

        case ReferenceType.Member:
          return apiMembers.GuidDictionary.TryGetValue(guid, out int id)
            ? id.ToString() : string.Empty;

        case ReferenceType.None:
        default:
          return string.Empty;
      }
    }
  }
}
