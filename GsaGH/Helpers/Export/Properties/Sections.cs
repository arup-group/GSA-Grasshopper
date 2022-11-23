using GsaAPI;
using System;
using System.Collections.Generic;
using GsaGH.Parameters;
using System.Linq;

namespace GsaGH.Helpers.Export
{
  internal class Sections
  {
    internal static int ConvertSection(GsaSection section,
        ref Dictionary<int, Section> existingSections, ref Dictionary<Guid, int> sections_guid,
        ref Dictionary<int, SectionModifier> existingSectionModifiers,
        ref Dictionary<int, AnalysisMaterial> existingMaterials, ref Dictionary<Guid, int> materials_guid)
    {
      if (section == null) { return 0; }
      if (section.API_Section == null) { return section.Id; }

      if (sections_guid.ContainsKey(section.GUID))
      {
        sections_guid.TryGetValue(section.GUID, out int sID);
        // if guid exist in our dictionary it has been added to the model 
        return sID;
      }

      int outID = section.Id;

      // set material
      if (section.API_Section.MaterialAnalysisProperty != 0 && section.Material != null && section.Material.AnalysisMaterial != null)
        section.API_Section.MaterialAnalysisProperty = Materials.ConvertCustomMaterial(section.Material, ref existingMaterials, ref materials_guid);

      // section
      if (section.Id > 0)
      {
        if (section.API_Section != null) // section can refer to an ID only, meaning that the section must already exist in the model. Else we set it in the model:
          existingSections[section.Id] = section.API_Section;
        else
          return outID; // return without setting the GUID in the dictionary
      }
      else
      {
        if (section.API_Section != null)
        {
          if (existingSections.Count > 0)
            outID = existingSections.Keys.Max() + 1;
          else
            outID = 1;

          existingSections.Add(outID, section.API_Section);
        }
      }

      // set guid in dictionary
      sections_guid.Add(section.GUID, outID);

      // set modifier
      if (section.Modifier != null && section.Modifier.IsModified)
      {
        if (existingSectionModifiers.ContainsKey(outID))
          existingSectionModifiers[outID] = section.Modifier._sectionModifier;
        else
          existingSectionModifiers.Add(outID, section.Modifier._sectionModifier);
      }

      return outID;
    }

    internal static void ConvertSection(List<GsaSection> sections,
        ref Dictionary<int, Section> existingSections, ref Dictionary<Guid, int> sections_guid, ref Dictionary<int, SectionModifier> apimodifiers,
        ref Dictionary<int, AnalysisMaterial> existingMaterials, ref Dictionary<Guid, int> materials_guid)
    {
      // create a counter for creating new sections
      int sectionidcounter = (existingSections.Count > 0) ? existingSections.Keys.Max() + 1 : 1; //checking the existing model

      // Add/Set sections
      if (sections != null)
      {
        if (sections.Count != 0)
        {
          // update counter if new sections have set ID higher than existing max
          int existingSectionsMaxID = sections.Max(x => x.Id); // max ID in new 
          if (existingSectionsMaxID > sectionidcounter)
            sectionidcounter = existingSectionsMaxID + 1;

          for (int i = 0; i < sections.Count; i++)
          {
            if (sections[i] != null)
            {
              GsaSection section = sections[i];
              Section apiSection = section.API_Section;

              // set modifier
              if (section.Modifier != null && section.Modifier.IsModified)
              {
                if (apimodifiers.ContainsKey(i))
                  apimodifiers[i] = section.Modifier._sectionModifier;
                else
                  apimodifiers.Add(i, section.Modifier._sectionModifier);
              }

              // set material
              if (section.API_Section.MaterialAnalysisProperty != 0 && section.Material != null && section.Material.AnalysisMaterial != null)
                section.API_Section.MaterialAnalysisProperty = Materials.ConvertCustomMaterial(section.Material, ref existingMaterials, ref materials_guid);

              if (sections_guid.ContainsKey(section.GUID))
              {
                sections_guid.TryGetValue(section.GUID, out int sID);
                // if guid exist in our dictionary it has been added to the model 
                continue;
              }

              if (section.Id > 0) // if the ID is larger than 0 than means the ID has been set and we sent it to the known list
              {
                existingSections[section.Id] = apiSection;
                // set guid in dictionary
                sections_guid.Add(section.GUID, section.Id);
              }
              else
              {
                existingSections.Add(sectionidcounter, apiSection);
                // set guid in dictionary
                sections_guid.Add(section.GUID, sectionidcounter);
                sectionidcounter++;
              }
            }
          }
        }
      }
    }
  }
}
