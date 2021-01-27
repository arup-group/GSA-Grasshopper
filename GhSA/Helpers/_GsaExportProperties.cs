using GsaAPI;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using GhSA.Parameters;
using System.Linq;
using System.Collections.ObjectModel;

namespace GhSA.Util.Gsa.ToGSA
{
    class Sections
    {
        public static int ConvertSection(GsaSection section,
            ref Dictionary<int, Section> existingSections)
        {
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
                    outID = existingSections.Keys.Max() + 1;
                    existingSections.Add(outID, section.Section);
                }
            }

            return outID;
        }

        public static void ConvertSection(List<GsaSection> sections,
            ref Dictionary<int, Section> existingSections,
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

                            if (section.ID > 0) // if the ID is larger than 0 than means the ID has been set and we sent it to the known list
                            {
                                existingSections[section.ID] = apiSection;
                            }
                            else
                            {
                                existingSections.Add(sectionidcounter, apiSection);
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
        public static int ConvertProp2d(GsaProp2d prop2d, ref Dictionary<int, Prop2D> existingProp2Ds, ref int prop2didcounter)
        {
            int prop2dID = prop2d.ID;
            Prop2D apiProp2d = prop2d.Prop2d;

            if (prop2d.ID > 0) // if the ID is larger than 0 than means the ID has been set and we sent it to the known list
            {
                existingProp2Ds[prop2d.ID] = apiProp2d;
            }
            else
            {
                prop2dID = prop2didcounter;
                existingProp2Ds.Add(prop2didcounter, apiProp2d);
                prop2didcounter++;
            }
            return prop2dID;
        }

        public static void ConvertProp2d(List<GsaProp2d> prop2Ds,
            ref Dictionary<int, Prop2D> existingProp2Ds,
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
                            ConvertProp2d(prop2d, ref existingProp2Ds, ref prop2didcounter);
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
