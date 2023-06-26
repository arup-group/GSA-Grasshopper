using System.Collections.Generic;
using System.Linq;
using GsaGH.Parameters;

namespace GsaGH.Helpers.Export {
  internal class Sections {
    internal static void ConvertSections(List<GsaSection> sections, ref Properties apiProperties) {
      if (sections == null) {
        return;
      }

      sections = sections.OrderByDescending(s => s.Id).ToList();
      foreach (GsaSection section in sections.Where(section => section != null)) {
        ConvertSection(section, ref apiProperties);
      }
    }

    internal static int ConvertSection(
      GsaSection section, ref Properties apiProperties) {
      if (section == null) {
        return 0;
      }

      if (section.IsReferencedById || section.ApiSection == null) {
        return section.Id;
      }

      return AddSection(section, ref apiProperties);
    }

    internal static int AddSection(GsaSection section, ref Properties apiProperties) {
      Materials.AddMaterial(ref section, ref apiProperties.Materials);

      int outId;
      if (section.Id > 0) {
        apiProperties.Sections.SetValue(section.Id, section.Guid, section.ApiSection);
        outId = section.Id;
      } else {
        outId = apiProperties.Sections.AddValue(section.Guid, section.ApiSection);
      }

      if (section.Modifier != null && section.Modifier.IsModified) {
        apiProperties.SecionModifiers.SetValue(outId, section.Modifier._sectionModifier);
      }

      return outId;
    }
  }
}