using GsaAPI;
using System;
using System.Collections.Generic;
using GsaGH.Parameters;
using System.Linq;

namespace GsaGH.Helpers.Export
{
  internal class Prop2ds
  {
    internal static int ConvertProp2d(GsaProp2d prop2d,
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

    internal static void ConvertProp2d(List<GsaProp2d> prop2Ds,
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
}
