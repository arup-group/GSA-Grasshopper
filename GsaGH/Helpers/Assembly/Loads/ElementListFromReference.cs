using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GsaAPI;
using GsaGH.Parameters;
using GsaGH.Parameters.Enums;

namespace GsaGH.Helpers.Assembly {
  internal partial class ModelAssembly {
    internal static ConcurrentDictionary<int, ConcurrentBag<int>> GetMemberElementRelationship(Model model) {
      var relationships = new ConcurrentDictionary<int, ConcurrentBag<int>>();
      Parallel.ForEach(model.Elements(),
        item => relationships.GetOrAdd(item.Value.ParentMember.Member, new ConcurrentBag<int>())
         .Add(item.Key));
      return relationships;
    }

    private string GetElementsReferenceDefinition(Guid guid) {
      return _elements.GuidDictionary.TryGetValue(guid, out Collection<int> ids) ?
        GsaList.CreateListDefinition(ids.ToList()) : string.Empty;
    }

    private string GetMemberChildElementsReferenceDefinition(Guid guid) {
      return !_members.GuidDictionary.TryGetValue(guid, out int id) ?
        string.Empty :
        GetMemberChildElementReferenceIdsDefinition(id);
    }

    private string GetMemberChildElementReferenceIdsDefinition(int memberId) {
      return memberElementRelationship.TryGetValue(memberId, out ConcurrentBag<int> ids) ?
          GsaList.CreateListDefinition(ids.ToList()) : string.Empty;
    }

    private string GetLoadReferenceDefinition(IGsaLoad load) {
      return GetReferenceDefinition(load.RefObjectGuid, load.ReferenceType);
    }

    private string GetReferenceDefinition(Guid guid, ReferenceType referenceType) {
      switch (referenceType) {
        case ReferenceType.Property:
          return GetPropertyReferenceDefinition(guid);

        case ReferenceType.Element:
          return GetElementsReferenceDefinition(guid);

        case ReferenceType.MemberChildElements:
          return GetMemberChildElementsReferenceDefinition(guid);

        case ReferenceType.Member:
          return _members.GuidDictionary.TryGetValue(guid, out int id)
            ? id.ToString() : string.Empty;

        case ReferenceType.None:
        default:
          return string.Empty;
      }
    }

    private string GetReferenceElementIdsDefinition(GsaGridPlaneSurface load) {
      return GetReferenceDefinition(load._refObjectGuid, load._referenceType);
    }
  }
}
