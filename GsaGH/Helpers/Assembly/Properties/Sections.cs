using System.Collections.Generic;
using System.Linq;

using GsaAPI;

using GsaGH.Parameters;

namespace GsaGH.Helpers.Assembly {
  internal partial class ModelAssembly {
    private int AddSection(GsaSection section) {
      Section api = section.DuplicateApiObject();
      AddMaterial(section.Material, ref api);

      int outId;
      if (section.Id > 0) {
        _sections.SetValue(section.Id, section.Guid, api);
        outId = section.Id;
      } else {
        outId = _sections.AddValue(section.Guid, api);
      }

      if (section.Modifier != null && section.Modifier.IsModified) {
        _secionModifiers.SetValue(outId, section.Modifier.DuplicateApiObject());
      }

      return outId;
    }

    private int ConvertSection(GsaSection section) {
      if (section == null) {
        return 0;
      }

      if (section.IsReferencedById || section.ApiSection == null) {
        return section.Id;
      }

      return AddSection(section);
    }

    private void ConvertSections(List<GsaSection> sections) {
      if (sections == null) {
        return;
      }

      sections = sections.OrderByDescending(s => s.Id).ToList();
      foreach (GsaSection section in sections.Where(section => section != null)) {
        ConvertSection(section);
      }
    }
  }
}
