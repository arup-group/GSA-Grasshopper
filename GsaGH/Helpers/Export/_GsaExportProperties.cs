using GsaAPI;
using System;
using System.Collections.Generic;
using GsaGH.Parameters;
using System.Linq;

namespace GsaGH.Util.Gsa.ToGSA
{
  class Materials
  {
    public static MaterialType ConvertType(GsaMaterial material)
    {
      MaterialType matType = GsaAPI.MaterialType.NONE;
      
      if (material != null)
        matType = (MaterialType)(int)material.MaterialType;

      return matType;
    }

    public static int ConvertCustomMaterial(GsaMaterial material,
        ref Dictionary<int, AnalysisMaterial> existingMaterials,
        ref Dictionary<Guid, int> materials_guid)
    {
      if (material == null) { return 0; }

      int outID = material.AnalysisProperty;

      // test if material has already bee added to the dictionary
      if (materials_guid.ContainsKey(material.Guid))
      {
        materials_guid.TryGetValue(material.Guid, out int sID);
        // if guid exist in our dictionary it has been added to the model 
        return sID;
      }

      // material
      if (material.AnalysisProperty > 0)
      {
        if (material.AnalysisMaterial != null) // material can refer to an ID only, meaning that the material must already exist in the model. Else we set it in the model:
          existingMaterials[material.AnalysisProperty] = material.AnalysisMaterial;
        else
          return outID; // return without setting the GUID in the dictionary
      }
      else
      {
        if (material.AnalysisMaterial != null)
        {
          if (existingMaterials.Count > 0)
            outID = existingMaterials.Keys.Max() + 1;
          else
            outID = 1;

          existingMaterials.Add(outID, material.AnalysisMaterial);
        }
      }

      // set guid in dictionary
      materials_guid.Add(material.Guid, outID);

      return outID;
    }
  }
  class Sections
  {
    public static int ConvertSection(GsaSection section,
        ref Dictionary<int, Section> existingSections, ref Dictionary<Guid, int> sections_guid,
        ref Dictionary<int, SectionModifier> existingSectionModifiers,
        ref Dictionary<int, AnalysisMaterial> existingMaterials, ref Dictionary<Guid, int> materials_guid)
    {
      if (section == null) { return 0; }
      if (section.API_Section == null) { return section.ID; }

      if (sections_guid.ContainsKey(section.GUID))
      {
        sections_guid.TryGetValue(section.GUID, out int sID);
        // if guid exist in our dictionary it has been added to the model 
        return sID;
      }

      int outID = section.ID;

      // set material
      if (section.API_Section.MaterialAnalysisProperty != 0 && section.Material != null && section.Material.AnalysisMaterial != null)
        section.API_Section.MaterialAnalysisProperty = Materials.ConvertCustomMaterial(section.Material, ref existingMaterials, ref materials_guid);

      // section
      if (section.ID > 0)
      {
        if (section.API_Section != null) // section can refer to an ID only, meaning that the section must already exist in the model. Else we set it in the model:
          existingSections[section.ID] = section.API_Section;
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

    public static void ConvertSection(List<GsaSection> sections,
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
          int existingSectionsMaxID = sections.Max(x => x.ID); // max ID in new 
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

              if (section.ID > 0) // if the ID is larger than 0 than means the ID has been set and we sent it to the known list
              {
                existingSections[section.ID] = apiSection;
                // set guid in dictionary
                sections_guid.Add(section.GUID, section.ID);
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

  class Prop2ds
  {
    public static int ConvertProp2d(GsaProp2d prop2d,
        ref Dictionary<int, Prop2D> existingProp2Ds, ref Dictionary<Guid, int> prop2d_guid,
        ref Dictionary<int, AnalysisMaterial> existingMaterials, ref Dictionary<Guid, int> materials_guid)
    {
      if (prop2d == null) { return 0; }
      if (prop2d.API_Prop2d == null) { return prop2d.ID; }
      if (prop2d_guid.ContainsKey(prop2d.GUID))
      {
        prop2d_guid.TryGetValue(prop2d.GUID, out int sID);
        // if guid exist in our dictionary it has been added to the model 
        return sID;
      }

      int outID = prop2d.ID;

      // set material
      if (prop2d.API_Prop2d.MaterialAnalysisProperty != 0 && prop2d.Material != null && prop2d.Material.AnalysisMaterial != null)
        prop2d.API_Prop2d.MaterialAnalysisProperty = Materials.ConvertCustomMaterial(prop2d.Material, ref existingMaterials, ref materials_guid);


      // section
      if (prop2d.ID > 0)
      {
        if (prop2d.API_Prop2d != null) // section can refer to an ID only, meaning that the section must already exist in the model. Else we set it in the model:
          existingProp2Ds[prop2d.ID] = prop2d.API_Prop2d;
        else
          return outID; // return without setting the GUID in the dictionary
      }
      else
      {
        if (prop2d.API_Prop2d != null)
        {
          if (existingProp2Ds.Count > 0)
            outID = existingProp2Ds.Keys.Max() + 1;
          else
            outID = 1;

          existingProp2Ds.Add(outID, prop2d.API_Prop2d);
        }
      }

      // set guid in dictionary
      prop2d_guid.Add(prop2d.GUID, outID);

      return outID;
    }

    public static void ConvertProp2d(List<GsaProp2d> prop2Ds,
        ref Dictionary<int, Prop2D> existingProp2Ds, ref Dictionary<Guid, int> prop2d_guid,
        ref Dictionary<int, AnalysisMaterial> existingMaterials, ref Dictionary<Guid, int> materials_guid)
    {
      // create a counter for creating new properties
      int prop2didcounter = (existingProp2Ds.Count > 0) ? existingProp2Ds.Keys.Max() + 1 : 1; //checking the existing model

      // Prop2Ds
      if (prop2Ds != null)
      {
        if (prop2Ds.Count != 0)
        {
          // update counter if new prop2ds have set ID higher than existing max
          int existingProp2dMaxID = prop2Ds.Max(x => x.ID); // max ID in new 
          if (existingProp2dMaxID > prop2didcounter)
            prop2didcounter = existingProp2dMaxID + 1;

          for (int i = 0; i < prop2Ds.Count; i++)
          {
            if (prop2Ds[i] != null)
            {
              GsaProp2d prop2d = prop2Ds[i];
              Prop2D apiProp2d = prop2d.API_Prop2d;

              // set material
              if (prop2d.API_Prop2d.MaterialAnalysisProperty != 0 && prop2d.Material != null && prop2d.Material.AnalysisMaterial != null)
                prop2d.API_Prop2d.MaterialAnalysisProperty = Materials.ConvertCustomMaterial(prop2d.Material, ref existingMaterials, ref materials_guid);


              if (prop2d_guid.ContainsKey(prop2d.GUID))
              {
                prop2d_guid.TryGetValue(prop2d.GUID, out int sID);
                // if guid exist in our dictionary it has been added to the model 
                continue;
              }

              if (prop2d.ID > 0) // if the ID is larger than 0 than means the ID has been set and we sent it to the known list
              {
                existingProp2Ds[prop2d.ID] = apiProp2d;
                // set guid in dictionary
                prop2d_guid.Add(prop2d.GUID, prop2d.ID);
              }
              else
              {
                existingProp2Ds.Add(prop2didcounter, apiProp2d);
                // set guid in dictionary
                prop2d_guid.Add(prop2d.GUID, prop2didcounter);
                prop2didcounter++;
              }
            }
          }
        }
      }
    }
  }
  class Prop3ds
  {
    public static int ConvertProp3d(GsaProp3d prop3d,
        ref Dictionary<int, Prop3D> existingProp3Ds, ref Dictionary<Guid, int> prop3d_guid,
        ref Dictionary<int, AnalysisMaterial> existingMaterials, ref Dictionary<Guid, int> materials_guid)
    {
      if (prop3d == null) { return 0; }
      if (prop3d.API_Prop3d == null) { return prop3d.ID; }
      if (prop3d_guid.ContainsKey(prop3d.GUID))
      {
        prop3d_guid.TryGetValue(prop3d.GUID, out int sID);
        // if guid exist in our dictionary it has been added to the model 
        return sID;
      }

      int outID = prop3d.ID;

      // set material
      if (prop3d.API_Prop3d.MaterialAnalysisProperty != 0 && prop3d.Material != null && prop3d.Material.AnalysisMaterial != null)
        prop3d.API_Prop3d.MaterialAnalysisProperty = Materials.ConvertCustomMaterial(prop3d.Material, ref existingMaterials, ref materials_guid);


      // section
      if (prop3d.ID > 0)
      {
        if (prop3d.API_Prop3d != null) // section can refer to an ID only, meaning that the section must already exist in the model. Else we set it in the model:
          existingProp3Ds[prop3d.ID] = prop3d.API_Prop3d;
        else
          return outID; // return without setting the GUID in the dictionary
      }
      else
      {
        if (prop3d.API_Prop3d != null)
        {
          if (existingProp3Ds.Count > 0)
            outID = existingProp3Ds.Keys.Max() + 1;
          else
            outID = 1;

          existingProp3Ds.Add(outID, prop3d.API_Prop3d);
        }
      }

      // set guid in dictionary
      prop3d_guid.Add(prop3d.GUID, outID);

      return outID;
    }

    public static void ConvertProp3d(List<GsaProp3d> prop3Ds,
        ref Dictionary<int, Prop3D> existingProp3Ds, ref Dictionary<Guid, int> prop3d_guid,
        ref Dictionary<int, AnalysisMaterial> existingMaterials, ref Dictionary<Guid, int> materials_guid)
    {
      // create a counter for creating new properties
      int prop3didcounter = (existingProp3Ds.Count > 0) ? existingProp3Ds.Keys.Max() + 1 : 1; //checking the existing model

      // Prop2Ds
      if (prop3Ds != null)
      {
        if (prop3Ds.Count != 0)
        {
          // update counter if new prop2ds have set ID higher than existing max
          int existingProp2dMaxID = prop3Ds.Max(x => x.ID); // max ID in new 
          if (existingProp2dMaxID > prop3didcounter)
            prop3didcounter = existingProp2dMaxID + 1;

          for (int i = 0; i < prop3Ds.Count; i++)
          {
            if (prop3Ds[i] != null)
            {
              GsaProp3d prop3d = prop3Ds[i];
              Prop3D apiProp3d = prop3d.API_Prop3d;

              // set material
              if (prop3d.API_Prop3d.MaterialAnalysisProperty != 0 && prop3d.Material != null && prop3d.Material.AnalysisMaterial != null)
                prop3d.API_Prop3d.MaterialAnalysisProperty = Materials.ConvertCustomMaterial(prop3d.Material, ref existingMaterials, ref materials_guid);


              if (prop3d_guid.ContainsKey(prop3d.GUID))
              {
                prop3d_guid.TryGetValue(prop3d.GUID, out int sID);
                // if guid exist in our dictionary it has been added to the model 
                continue;
              }

              if (prop3d.ID > 0) // if the ID is larger than 0 than means the ID has been set and we sent it to the known list
              {
                existingProp3Ds[prop3d.ID] = apiProp3d;
                // set guid in dictionary
                prop3d_guid.Add(prop3d.GUID, prop3d.ID);
              }
              else
              {
                existingProp3Ds.Add(prop3didcounter, apiProp3d);
                // set guid in dictionary
                prop3d_guid.Add(prop3d.GUID, prop3didcounter);
                prop3didcounter++;
              }
            }
          }
        }
      }
    }
  }
}
