using System.Collections.Generic;
using System.Linq;
using GsaAPI;
using GsaGH.Parameters;

namespace GsaGH.Helpers.Export {
  internal class Sections {
    internal static int AddSection(GsaSection section, ref GsaGuidDictionary<Section> apiSections, ref GsaIntDictionary<SectionModifier> apiSectionModifiers, ref GsaGuidDictionary<AnalysisMaterial> apiMaterials) {
      Materials.AddMaterial(ref section, ref apiMaterials);

      int outId;
      if (section.Id > 0) {
        apiSections.SetValue(section.Id, section.Guid, section.API_Section);
        outId = section.Id;
      }
      else
        outId = apiSections.AddValue(section.Guid, section.API_Section);

      if (section.Modifier != null && section.Modifier.IsModified)
        apiSectionModifiers.SetValue(outId, section.Modifier._sectionModifier);

      return outId;
    }

    internal static int ConvertSection(GsaSection section,
        ref GsaGuidDictionary<Section> apiSections,
        ref GsaIntDictionary<SectionModifier> apiSectionModifiers,
        ref GsaGuidDictionary<AnalysisMaterial> apiMaterials) {
      if (section == null) { return 0; }
      if (section.IsReferencedByID || section.API_Section == null) { return section.Id; }
      return AddSection(section, ref apiSections, ref apiSectionModifiers, ref apiMaterials);
    }

    internal static void ConvertSection(List<GsaSection> sections,
       ref GsaGuidDictionary<Section> apiSections,
        ref GsaIntDictionary<SectionModifier> apiSectionModifiers,
        ref GsaGuidDictionary<AnalysisMaterial> apiMaterials) {
      if (sections == null) {
        return;
      }

      sections = sections.OrderByDescending(s => s.Id).ToList();
      foreach (GsaSection section in sections.Where(section => section != null))
        ConvertSection(section, ref apiSections, ref apiSectionModifiers, ref apiMaterials);
    }
  }
}
