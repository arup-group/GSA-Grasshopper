using GsaAPI;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using GhSA.Parameters;
using System.Linq;
using System.Collections.ObjectModel;

namespace GhSA.Util.Gsa.ToGSA
{
    class Materials
    {
        public static MaterialType ConvertType(GsaMaterial material)
        {
            MaterialType matType = GsaAPI.MaterialType.NONE;
            int typ = (int)material.Type;
            if (typ == 1)
                matType = MaterialType.STEEL;
            if (typ == 2)
                matType = MaterialType.CONCRETE;
            if (typ == 5)
                matType = MaterialType.FRP;
            if (typ == 3)
                matType = MaterialType.ALUMINIUM;
            if (typ == 7)
                matType = MaterialType.TIMBER;
            if (typ == 4)
                matType = MaterialType.GLASS;
            if (typ == 8)
                matType = MaterialType.FABRIC;
            if (typ == 0)
                matType = MaterialType.GENERIC;
            
            return matType;
        }

    }
    class Sections
    {
        public static int ConvertSection(GsaSection section,
            ref Dictionary<int, Section> existingSections,
            ref Dictionary<Guid, int> sections_guid)
        {
            if (section == null) { return 0; }
            if (sections_guid.ContainsKey(section.GUID))
            {
                sections_guid.TryGetValue(section.GUID, out int sID);
                // if guid exist in our dictionary it has been added to the model 
                return sID;
            }

            int outID = section.ID;

            // section
            if (section.ID > 0)
            {
                if (section.Section != null) // section can refer to an ID only, meaning that the section must already exist in the model. Else we set it in the model:
                    existingSections[section.ID] = section.Section;
            }
            else
            {
                if (section.Section != null)
                {
                    if (existingSections.Count > 0)
                        outID = existingSections.Keys.Max() + 1;
                    else
                        outID = 1;
                    
                    existingSections.Add(outID, section.Section);
                }
            }

            // set guid in dictionary
            sections_guid.Add(section.GUID, outID);

            return outID;   
        }

        public static void ConvertSection(List<GsaSection> sections,
            ref Dictionary<int, Section> existingSections, ref Dictionary<Guid, int> sections_guid,
            GrasshopperAsyncComponent.WorkerInstance workerInstance = null,
            Action<string, double> ReportProgress = null)
        {
            // create a counter for creating new nodes
            int sectionidcounter = (existingSections.Count > 0) ? existingSections.Keys.Max() + 1 : 1; //checking the existing model

            // Add/Set Nodes
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
                        if (workerInstance != null)
                        {
                            if (workerInstance.CancellationToken.IsCancellationRequested) return;
                            ReportProgress("Sections ", (double)i / (sections.Count - 1));
                        }


                        if (sections[i] != null)
                        {
                            GsaSection section = sections[i];
                            Section apiSection = section.Section;

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
            if (workerInstance != null)
            {
                ReportProgress("Sections assembled", -2);
            }
        }
    }

    class Prop2ds
    {
        //public static int ConvertProp2d(GsaProp2d prop2d, ref Dictionary<int, Prop2D> existingProp2Ds, ref int prop2didcounter)
        //{
        //    if (prop2d == null) { return 0; }
        //
        //    int prop2dID = prop2d.ID;
        //
        //    if (prop2d.ID > 0) // if the ID is larger than 0 than means the ID has been set and we sent it to the known list
        //    {
        //        if (prop2d.Prop2d != null)
        //            existingProp2Ds[prop2d.ID] = prop2d.Prop2d; 
        //    }
        //    else
        //    {
        //        if (prop2d.Prop2d != null)
        //        {
        //            prop2dID = prop2didcounter;
        //            existingProp2Ds.Add(prop2didcounter, prop2d.Prop2d);
        //            prop2didcounter++;
        //        }
        //    }
        //    return prop2dID;
        //}

        public static int ConvertProp2d(GsaProp2d prop2d,
            ref Dictionary<int, Prop2D> existingProp2Ds,
            ref Dictionary<Guid, int> prop2d_guid)
        {
            if (prop2d == null) { return 0; }
            if (prop2d_guid.ContainsKey(prop2d.GUID))
            {
                prop2d_guid.TryGetValue(prop2d.GUID, out int sID);
                // if guid exist in our dictionary it has been added to the model 
                return sID;
            }

            int outID = prop2d.ID;

            // section
            if (prop2d.ID > 0)
            {
                if (prop2d.Prop2d != null) // section can refer to an ID only, meaning that the section must already exist in the model. Else we set it in the model:
                    existingProp2Ds[prop2d.ID] = prop2d.Prop2d;
            }
            else
            {
                if (prop2d.Prop2d != null)
                {
                    if (existingProp2Ds.Count > 0)
                        outID = existingProp2Ds.Keys.Max() + 1;
                    else
                        outID = 1;

                    existingProp2Ds.Add(outID, prop2d.Prop2d);
                }
            }

            // set guid in dictionary
            prop2d_guid.Add(prop2d.GUID, outID);

            return outID;
        }

        public static void ConvertProp2d(List<GsaProp2d> prop2Ds,
            ref Dictionary<int, Prop2D> existingProp2Ds, ref Dictionary<Guid, int> prop2d_guid,
            GrasshopperAsyncComponent.WorkerInstance workerInstance = null,
            Action<string, double> ReportProgress = null)
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
                        if (workerInstance != null)
                        {
                            if (workerInstance.CancellationToken.IsCancellationRequested) return;
                            ReportProgress("Prop2D ", (double)i / (prop2Ds.Count - 1));
                        }

                        if (prop2Ds[i] != null)
                        {
                            GsaProp2d prop2d = prop2Ds[i];
                            Prop2D apiProp2d = prop2d.Prop2d;

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
            if (workerInstance != null)
            {
                ReportProgress("Prop2D assembled", -2);
            }
        }
    }
}
