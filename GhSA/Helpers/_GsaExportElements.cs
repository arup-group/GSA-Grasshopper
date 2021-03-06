﻿using GsaAPI;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using GhSA.Parameters;
using System.Linq;
using System.Collections.ObjectModel;

namespace GhSA.Util.Gsa.ToGSA
{
    class Elements
    {
        #region element1d
        public static void ConvertElement1D(GsaElement1d element1d,
            ref Dictionary<int, Element> existingElements, ref int elementidcounter,
            ref Dictionary<int, Node> existingNodes, ref int nodeidcounter,
            ref Dictionary<int, Section> existingSections, ref Dictionary<Guid, int> sections_guid)
        {
            LineCurve line = element1d.Line;
            Element apiElement = element1d.Element;

            // update topology list to fit model nodes
            List<int> topo = new List<int>();
            //Start node
            int id = Nodes.GetExistingNodeID(existingNodes, line.PointAtStart);
            if (id > 0)
                topo.Add(id);
            else
            {
                existingNodes.Add(nodeidcounter, Nodes.NodeFromPoint(line.PointAtStart));
                topo.Add(nodeidcounter);
                nodeidcounter++;
            }

            //End node
            id = Nodes.GetExistingNodeID(existingNodes, line.PointAtEnd);
            if (id > 0)
                topo.Add(id);
            else
            {
                existingNodes.Add(nodeidcounter, Nodes.NodeFromPoint(line.PointAtEnd));
                topo.Add(nodeidcounter);
                nodeidcounter++;
            }
            // update topology in Element
            apiElement.Topology = new ReadOnlyCollection<int>(topo.ToList());

            // section
            if (apiElement.Property == 0)
                apiElement.Property = Sections.ConvertSection(element1d.Section, ref existingSections, ref sections_guid);

            // set apielement in dictionary
            if (element1d.ID > 0) // if the ID is larger than 0 than means the ID has been set and we sent it to the known list
            {
                existingElements[element1d.ID] = apiElement;
            }
            else
            {
                existingElements.Add(elementidcounter, apiElement);
                elementidcounter++;
            }
        }

        public static void ConvertElement1D(List<GsaElement1d> element1ds,
            ref Dictionary<int, Element> existingElements, ref int elementidcounter,
            ref Dictionary<int, Node> existingNodes,
            ref Dictionary<int, Section> existingSections, ref Dictionary<Guid, int> sections_guid,
            GrasshopperAsyncComponent.WorkerInstance workerInstance = null,
            Action<string, double> ReportProgress = null)
        {
            int nodeidcounter = (existingNodes.Count > 0) ? existingNodes.Keys.Max() + 1 : 1;

            // Elem1ds
            if (element1ds != null)
            {
                for (int i = 0; i < element1ds.Count; i++)
                {
                    if (workerInstance != null)
                    {
                        if (workerInstance.CancellationToken.IsCancellationRequested) return;
                        ReportProgress("Elem1D ", (double)i / (element1ds.Count - 1));
                    }


                    if (element1ds[i] != null)
                    {
                        GsaElement1d element1d = element1ds[i];

                        // Add/set element
                        ConvertElement1D(element1d, ref existingElements, ref elementidcounter,
                            ref existingNodes, ref nodeidcounter, ref existingSections, ref sections_guid);
                    }
                }
            }
            if (workerInstance != null)
            {
                ReportProgress("Elem1D assembled", -2);
            }
        }
        #endregion

        #region element2d

        public static void ConvertElement2D(GsaElement2d element2d,
            ref Dictionary<int, Element> existingElements, ref int elementidcounter,
            ref Dictionary<int, Node> existingNodes, ref int nodeidcounter,
            ref Dictionary<int, Prop2D> existingProp2Ds, ref Dictionary<Guid, int> prop2d_guid)
        {
            List<Point3d> meshVerticies = element2d.Topology;

            //Loop through all faces in mesh to update topology list to fit model nodes
            for (int i = 0; i < element2d.Elements.Count; i++)
            {
                Element apiMeshElement = element2d.Elements[i];
                List<int> meshVertexIndex = element2d.TopoInt[i];

                List<int> topo = new List<int>(); // temp topologylist

                //Loop through topology
                for (int j = 0; j < meshVertexIndex.Count; j++)
                {
                    int id = Nodes.GetExistingNodeID(existingNodes, meshVerticies[meshVertexIndex[j]]);
                    if (id > 0)
                        topo.Add(id);
                    else
                    {
                        existingNodes.Add(nodeidcounter, Nodes.NodeFromPoint(meshVerticies[meshVertexIndex[j]]));
                        topo.Add(nodeidcounter);
                        nodeidcounter++;
                    }
                }
                //update topology in Element
                apiMeshElement.Topology = new ReadOnlyCollection<int>(topo.ToList());

                // section
                if (apiMeshElement.Property == 0)
                    apiMeshElement.Property = Prop2ds.ConvertProp2d(element2d.Properties[i], ref existingProp2Ds, ref prop2d_guid);
                

                // set api element in dictionary
                if (element2d.ID[i] > 0) // if the ID is larger than 0 than means the ID has been set and we sent it to the known list
                {
                    existingElements[element2d.ID[i]] = apiMeshElement;
                }
                else
                {
                    existingElements.Add(elementidcounter, apiMeshElement);
                    elementidcounter++;
                }
            }
        }
        public static void ConvertElement2D(List<GsaElement2d> element2ds,
            ref Dictionary<int, Element> existingElements, ref int elementidcounter,
            ref Dictionary<int, Node> existingNodes,
            ref Dictionary<int, Prop2D> existingProp2Ds, ref Dictionary<Guid, int> prop2d_guid,
            GrasshopperAsyncComponent.WorkerInstance workerInstance = null,
            Action<string, double> ReportProgress = null)
        {
            // create a counter for creating new elements, nodes and properties
            int nodeidcounter = (existingNodes.Count > 0) ? existingNodes.Keys.Max() + 1 : 1;
            int prop2didcounter = (existingProp2Ds.Count > 0) ? existingProp2Ds.Keys.Max() + 1 : 1; //checking the existing model

            // Elem2ds
            if (element2ds != null)
            {
                for (int i = 0; i < element2ds.Count; i++)
                {
                    if (workerInstance != null)
                    {
                        if (workerInstance.CancellationToken.IsCancellationRequested) return;
                        ReportProgress("Elem2D ", (double)i / (element2ds.Count - 1));
                    }


                    if (element2ds[i] != null)
                    {
                        GsaElement2d element2d = element2ds[i];

                        ConvertElement2D(element2d, 
                            ref existingElements, ref elementidcounter, 
                            ref existingNodes, ref nodeidcounter, 
                            ref existingProp2Ds, ref prop2d_guid);

                    }
                }
            }
            if (workerInstance != null)
            {
                ReportProgress("Elem2D assembled", -2);
            }
        }
        #endregion

        #region element3d

        public static void ConvertElement3D(GsaElement3d element3d,
            ref Dictionary<int, Element> existingElements, ref int elementidcounter,
            ref Dictionary<int, Node> existingNodes, ref int nodeidcounter
            )
        {
            List<Point3d> meshVerticies = element3d.Topology;

            //Loop through all faces in mesh to update topology list to fit model nodes
            for (int i = 0; i < element3d.Elements.Count; i++)
            {
                Element apiMeshElement = element3d.Elements[i];
                List<int> meshVertexIndex = element3d.TopoInt[i];

                List<int> topo = new List<int>(); // temp topologylist

                //Loop through topology
                for (int j = 0; j < meshVertexIndex.Count; j++)
                {
                    int id = Nodes.GetExistingNodeID(existingNodes, meshVerticies[meshVertexIndex[j]]);
                    if (id > 0)
                        topo.Add(id);
                    else
                    {
                        existingNodes.Add(nodeidcounter, Nodes.NodeFromPoint(meshVerticies[meshVertexIndex[j]]));
                        topo.Add(nodeidcounter);
                        nodeidcounter++;
                    }
                }
                //update topology in Element
                apiMeshElement.Topology = new ReadOnlyCollection<int>(topo.ToList());

                // section
                //if (apiMeshElement.Property == 0)
                //    apiMeshElement.Property = Prop2ds.ConvertProp2d(element2d.Properties[i], ref existingProp2Ds, ref prop2didcounter);


                // set api element in dictionary
                if (element3d.ID[i] > 0) // if the ID is larger than 0 than means the ID has been set and we sent it to the known list
                {
                    existingElements[element3d.ID[i]] = apiMeshElement;
                }
                else
                {
                    existingElements.Add(elementidcounter, apiMeshElement);
                    elementidcounter++;
                }
            }
        }
        public static void ConvertElement3D(List<GsaElement3d> element3ds,
            ref Dictionary<int, Element> existingElements, ref int elementidcounter,
            ref Dictionary<int, Node> existingNodes,
            GrasshopperAsyncComponent.WorkerInstance workerInstance = null,
            Action<string, double> ReportProgress = null)
        {
            // create a counter for creating new elements, nodes and properties
            int nodeidcounter = (existingNodes.Count > 0) ? existingNodes.Keys.Max() + 1 : 1;
            //int prop2didcounter = (existingProp2Ds.Count > 0) ? existingProp2Ds.Keys.Max() + 1 : 1; //checking the existing model

            // Elem3ds
            if (element3ds != null)
            {
                for (int i = 0; i < element3ds.Count; i++)
                {
                    if (workerInstance != null)
                    {
                        if (workerInstance.CancellationToken.IsCancellationRequested) return;
                        ReportProgress("Elem3D ", (double)i / (element3ds.Count - 1));
                    }


                    if (element3ds[i] != null)
                    {
                        GsaElement3d element3d = element3ds[i];

                        ConvertElement3D(element3d,
                            ref existingElements, ref elementidcounter,
                            ref existingNodes, ref nodeidcounter);

                    }
                }
            }
            if (workerInstance != null)
            {
                ReportProgress("Elem3D assembled", -2);
            }
        }
        #endregion
    }
}
