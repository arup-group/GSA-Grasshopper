using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GsaAPI;
using GsaGH.Parameters;
using GsaGH.Parameters.Enums;

namespace GsaGH.Helpers.Export {
  internal partial class ModelAssembly {

    internal static ConcurrentDictionary<int, ConcurrentBag<int>> GetMemberElementRelationship(
      Model model) {
      var relationships = new ConcurrentDictionary<int, ConcurrentBag<int>>();
      Parallel.ForEach(model.Elements(),
        item => relationships.GetOrAdd(item.Value.ParentMember.Member, new ConcurrentBag<int>())
         .Add(item.Key));
      return relationships;
    }

    internal string GetReferenceElementIdsDefinition(GsaGridPlaneSurface load) {
      return GetReferenceDefinition1(load._refObjectGuid, load._referenceType);
    }

    internal string GetMemberChildElementReferenceIdsDefinition(int memberId) {
      return MemberElementRelationship.TryGetValue(memberId, out ConcurrentBag<int> ids) ?
          GsaList.CreateListDefinition(ids.ToList()) : string.Empty;
    }

    private string GetElementsReferenceDefinition4(Guid guid) {
      return Elements.GuidDictionary.TryGetValue(guid, out Collection<int> ids) ?
        GsaList.CreateListDefinition(ids.ToList()) : string.Empty;
    }

    private string GetMemberChildElementsReferenceDefinition(Guid guid) {
      return !Members.GuidDictionary.TryGetValue(guid, out int id) ?
        string.Empty :
        GetMemberChildElementReferenceIdsDefinition(id);
    }

    internal string GetReferenceDefinition2(IGsaLoad load) {
      return GetReferenceDefinition1(load.RefObjectGuid, load.ReferenceType);
    }

    internal string GetReferenceDefinition1(
      Guid guid,
      ReferenceType referenceType) {
      switch (referenceType) {
        case ReferenceType.Property:
          return GetReferenceDefinition3(guid);

        case ReferenceType.Element:
          return GetElementsReferenceDefinition4(guid);

        case ReferenceType.MemberChildElements:
          return GetMemberChildElementsReferenceDefinition(guid);

        case ReferenceType.Member:
          return Members.GuidDictionary.TryGetValue(guid, out int id)
            ? id.ToString() : string.Empty;

        case ReferenceType.None:
        default:
          return string.Empty;
      }
    }
  }
}
