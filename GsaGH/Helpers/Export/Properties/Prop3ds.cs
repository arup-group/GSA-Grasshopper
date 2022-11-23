using GsaAPI;
using System;
using System.Collections.Generic;
using GsaGH.Parameters;
using System.Linq;

namespace GsaGH.Helpers.Export
{
  internal class Prop3ds
  {
    internal static int ConvertProp3d(GsaProp3d prop3d,
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

    internal static void ConvertProp3d(List<GsaProp3d> prop3Ds,
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
