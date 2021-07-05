using System;
using System.Linq;
using System.Drawing;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using GsaAPI;
using Rhino.Geometry;
using GhSA.Parameters;

namespace GhSA.Util.Gsa
{
    /// <summary>
    /// Class containing functions to import various object types from GSA
    /// </summary>
    public class FromGSA
    {
        #region nodes
        /// <summary>
        /// Method to import Nodes from a GSA model.
        /// Will output a list of GhSA GsaNodeGoos.
        /// Input node dictionary pre-filtered for selected nodes to import;
        /// </summary>
        /// <param name="nDict">Dictionary of GSA Nodes pre-filtered for nodes to import</param>
        /// <param name="model">GSA Model, only used in case node refers to a local axis</param>
        /// <returns></returns>
        public static List<GsaNodeGoo> GetNodes(IReadOnlyDictionary<int, Node> nDict, Model model)
        {
            List<GsaNodeGoo> nodes = new List<GsaNodeGoo>();

            // Placeholder in case we need to get local axis
            IReadOnlyDictionary<int, GsaAPI.Axis> axDict = null;
            bool getAxes = true;

            foreach (KeyValuePair<int, Node> item in nDict)
            {
                // create new node with basic Position and ID values
                GsaNode gsaNode = new GsaNode
                {
                    Node = item.Value,
                    Point = new Point3d
                    { X = item.Value.Position.X, Y = item.Value.Position.Y, Z = item.Value.Position.Z },
                    ID = item.Key
                };
                // add local axis if node has Axis property
                if (item.Value.AxisProperty > 0)
                {
                    if (getAxes)
                    {
                        axDict = model.Axes();
                        getAxes = false;
                    }
                    axDict.TryGetValue(item.Value.AxisProperty, out GsaAPI.Axis axis);
                    gsaNode.LocalAxis = AxisToPlane(axis);
                }
                else
                {
                    switch (item.Value.AxisProperty)
                    {
                        case 0:
                            // local axis = Global
                            break;
                        case -11:
                            // local axis = X-elevation
                            gsaNode.LocalAxis = Plane.WorldYZ;
                            break;
                        case -12:
                            // local axis = X-elevation
                            gsaNode.LocalAxis = Plane.WorldZX;
                            break;
                        case -13:
                            // local axis = vertical
                            gsaNode.LocalAxis = Plane.WorldXY;
                            break;
                        case -14:
                            // local axis = global cylindric
                            gsaNode.LocalAxis = Plane.Unset;
                            break;
                    }
                }
                nodes.Add(new GsaNodeGoo(gsaNode.Duplicate()));
            }
            return nodes;
        }

        /// <summary>
        /// Method to create a Rhino Plane from a GSA Axis
        /// </summary>
        /// <param name="axis">GSA Axis to create plane from</param>
        /// <returns></returns>
        public static Plane AxisToPlane(GsaAPI.Axis axis)
        {
            if (axis == null) { return Plane.Unset; }

            // origin point from GSA Axis
            Point3d origin = new Point3d(axis.Origin.X, axis.Origin.Y, axis.Origin.Z);

            // X-axis from GSA Axis
            Vector3d xAxis = new Vector3d
            {
                X = axis.XVector.X,
                Y = axis.XVector.Y,
                Z = axis.XVector.Z
            };
            // check if vector is zero-length
            if (xAxis.IsZero) { return Plane.Unset; }
            // create unitised vector
            Vector3d xUnit = new Vector3d(xAxis);
            xUnit.Unitize();

            // Y-axis from GSA Axis
            Vector3d yAxis = new Vector3d
            {
                X = axis.XYPlane.X,
                Y = axis.XYPlane.Y,
                Z = axis.XYPlane.Z
            };
            // check if vector is zero-length
            if (yAxis.IsZero) { return Plane.Unset; }
            // create unitised vector
            Vector3d yUnit = new Vector3d(yAxis);
            yUnit.Unitize();

            // check if x and y unitised are not the same
            if (xUnit.Equals(yUnit)) { return Plane.Unset; }


            Plane pln = new Plane(origin, xAxis, yAxis);

            return pln;
        }
        #endregion

        #region elements
        /// <summary>
        /// /// Method to convert Elements to GhSA Element 1D and Element 2D
        /// Element 3Ds to be implemented
        /// Will output a tuple containing a:
        /// 1. List of GsaElement1dGoos and 
        /// 2. List of GsaElement2dGoos
        /// </summary>
        /// <param name="eDict">Dictionary of Elements to import</param>
        /// <param name="nDict">Dictionary of Nodes that elements refers to by topology. Include all Nodes unless you are sure what you are doing</param>
        /// <param name="sDict">Dictionary of Sections (for 1D elements)</param>
        /// <param name="pDict">Dictionary of 2D Properties (for 2D elements)</param>
        /// <returns></returns>
        public static Tuple<List<GsaElement1dGoo>, List<GsaElement2dGoo>, List<GsaElement3dGoo>> 
            GetElements(IReadOnlyDictionary<int, Element> eDict, IReadOnlyDictionary<int, Node> nDict,
            IReadOnlyDictionary<int, Section> sDict, IReadOnlyDictionary<int, Prop2D> pDict)
        {
            // Create lists for Rhino lines and meshes
            List<GsaElement1dGoo> elem1ds = new List<GsaElement1dGoo>();
            List<GsaElement2dGoo> elem2ds = new List<GsaElement2dGoo>();
            List<GsaElement3dGoo> elem3ds = new List<GsaElement3dGoo>();

            Dictionary<int, Element> elem2dDict = new Dictionary<int, Element>();
            Dictionary<int, Element> elem3dDict = new Dictionary<int, Element>();

            foreach (KeyValuePair<int, Element> item in eDict)
            {
                // find type of element, 1D, 2D or 3D:
                int elemDimension = 1; // default assume 1D element
                
                // get element type
                ElementType type = item.Value.Type;

                // change to 2D if type is one of these
                if (type == ElementType.TRI3 || type == ElementType.TRI6 ||
                    type == ElementType.QUAD4 || type == ElementType.QUAD8 ||
                    type == ElementType.TWO_D || type == ElementType.TWO_D_FE ||
                    type == ElementType.TWO_D_LOAD)
                    elemDimension = 2;
                // change to 3D if type is one of these
                if (type == ElementType.BRICK8 || type == ElementType.WEDGE6 ||
                    type == ElementType.PYRAMID5 || type == ElementType.TETRA4 ||
                    type == ElementType.THREE_D)
                    elemDimension = 3;

                switch (elemDimension)
                {
                    case 1:
                        // create new GhSA element from api element;
                        elem1ds.Add(
                            new GsaElement1dGoo(
                                ConvertToElement1D(
                                    item.Value, item.Key, nDict, sDict)));
                        break;

                    case 2:
                        // add 2D element to dictionary to bulk create and combine
                        // meshes in one go
                        elem2dDict.Add(item.Key, item.Value);
                        break;

                    case 3:
                        // add 3D element to dictionary to bulk create and combine
                        // meshes in one go
                        elem3dDict.Add(item.Key, item.Value);
                        break;
                }
            }

            // if import found any 2D elements add the in one go.
            // GhSA GsaElement2d consist of a list of 2D elements in order
            // to display a combined mesh: each 2D element is a mesh face
            if (elem2dDict.Count > 0)
                elem2ds = ConvertToElement2Ds(elem2dDict, nDict, pDict);

            if (elem3dDict.Count > 0)
                elem3ds = ConvertToElement3Ds(elem3dDict, nDict);

            return new Tuple<List<GsaElement1dGoo>, List<GsaElement2dGoo>, List<GsaElement3dGoo>>(elem1ds, elem2ds, elem3ds);
        }
        
        /// <summary>
        /// Method to get convert an Element to a Mesh with one face (tri, quad or ngon)
        /// </summary>
        /// <param name="element">Element to get mesh face from</param>
        /// <param name="nodes">Dictionary of nodes that includes nodes for the topology which the element.Topology refers to. Typically use all nodes from a GSA model</param>
        /// <returns></returns>
        public static Mesh ConvertElement2D(Element element, IReadOnlyDictionary<int, Node> nodes)
        {
            // get element's topology
            ReadOnlyCollection<int> topo = element.Topology;

            // check if element is 2D
            if (topo.Count < 3 ||
                element.Type == ElementType.THREE_D ||
                element.Type == ElementType.BRICK8 ||
                element.Type == ElementType.WEDGE6 ||
                element.Type == ElementType.PYRAMID5 ||
                element.Type == ElementType.TETRA4)
                return null;

            Mesh outMesh = new Mesh();

            // Get verticies:
            for (int k = 0; k < topo.Count; k++)
            {
                if (nodes.TryGetValue(topo[k], out Node node))
                {
                    {
                        var p = node.Position;
                        outMesh.Vertices.Add(new Point3d(p.X, p.Y, p.Z));
                    }
                }
            }

            // Create mesh face (Tri- or Quad):
            if (topo.Count == 3)
                outMesh.Faces.AddFace(0, 1, 2);
            else if (topo.Count == 4)
                outMesh.Faces.AddFace(0, 1, 2, 3);
            else if (topo.Count > 4)
            {
                // so we introduce the average middle point and create more faces

                if (topo.Count == 6)
                {
                    outMesh.Faces.AddFace(0, 3, 5);
                    outMesh.Faces.AddFace(1, 4, 3);
                    outMesh.Faces.AddFace(2, 5, 4);
                    outMesh.Faces.AddFace(3, 4, 5);
                    List<int> tri6Vert = new List<int>() { 0, 3, 1, 4, 2, 5 };
                    List<int> tri6Face = new List<int>() { 0, 1, 2, 3 };
                    MeshNgon meshGon = MeshNgon.Create(tri6Vert, tri6Face);

                    outMesh.Ngons.AddNgon(meshGon);
                }

                if (topo.Count == 8)
                {
                    Point3d ave = new Point3d();
                    ave.X = 0;
                    ave.Y = 0;
                    ave.Z = 0;
                    for (int k = 0; k < topo.Count; k++)
                    {
                        ave.X += outMesh.Vertices[k].X;
                        ave.Y += outMesh.Vertices[k].Y;
                        ave.Z += outMesh.Vertices[k].Z;
                    }
                    ave.X = ave.X / topo.Count;
                    ave.Y = ave.Y / topo.Count;
                    ave.Z = ave.Z / topo.Count;

                    outMesh.Vertices.Add(ave);

                    outMesh.Faces.AddFace(0, 4, 8);
                    outMesh.Faces.AddFace(1, 8, 4);
                    outMesh.Faces.AddFace(1, 5, 8);
                    outMesh.Faces.AddFace(2, 8, 5);
                    outMesh.Faces.AddFace(2, 6, 8);
                    outMesh.Faces.AddFace(3, 8, 6);
                    outMesh.Faces.AddFace(3, 7, 8);
                    outMesh.Faces.AddFace(0, 8, 7);
                    List<int> quad8vert = new List<int>() { 0, 4, 1, 5, 2, 6, 3, 7 };
                    List<int> quad8Face = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7 };

                    MeshNgon meshGon = MeshNgon.Create(quad8vert, quad8Face);

                    outMesh.Ngons.AddNgon(meshGon);
                }
            }
            return outMesh;
        }

        /// <summary>
        /// Method to bulk convert 2D Elements to GhSA Element 2Ds
        /// Will output a list of GsaElement2dGoos
        /// </summary>
        /// <param name="elements">Dictionary of 2D Elements</param>
        /// <param name="nodes">Dictionary of Nodes that elements refers to by topology. Include all Nodes unless you are sure what you are doing</param>
        /// <param name="properties">Dictionary of 2D Properties</param>
        /// <returns></returns>
        public static List<GsaElement2dGoo> ConvertToElement2Ds(Dictionary<int, Element> elements, IReadOnlyDictionary<int, Node> nodes, IReadOnlyDictionary<int, Prop2D> properties)
        {
            List<List<Element>> apielements = new List<List<Element>>();
            List<List<int>> IDs = new List<List<int>>();
            List<int> hosts = new List<int>();
            
            // loop through incoming elements and write them to lists with ID and ParentMember number
            // to sort elements in lists containing elements with same parent
            foreach (KeyValuePair<int, Element> elem in elements)
            {
                Element item = DuplicateElement(elem.Value);

                // get parent member
                int host = item.ParentMember.Member;

                // place in list to add elements
                int i;

                // check if the host list is not empty and already contains the host id
                if (hosts.Count > 0 && hosts.Contains(host))
                {
                    // if we already have an element with the same parent then add to list
                    i = hosts.IndexOf(host);
                }
                else
                {
                    // if we dont have any elements with this parent add the list and add element to list    
                    hosts.Add(host);
                    apielements.Add(new List<Element>());
                    IDs.Add(new List<int>());
                    i = hosts.Count - 1;
                }
                apielements[i].Add(item);
                IDs[i].Add(elem.Key);
            }

            // loop through list of elements and create Meshes and Element2Ds 
            List<GsaElement2dGoo> elem2dGoos = new List<GsaElement2dGoo>();
            for (int i = 0; i < apielements.Count; i++)
            {
                // list of elements with same parent
                List<Element> elems = apielements[i];

                // create list of Prop2Ds
                List<GsaProp2d> prop2Ds = new List<GsaProp2d>();

                // create list of meshes
                List<Mesh> mList = new List<Mesh>();

                // loop through elements in list
                for (int j = 0; j < elems.Count; j++)
                {
                    Mesh faceMesh = ConvertElement2D(elems[j], nodes);
                    if (faceMesh == null) { continue; }
                    mList.Add(faceMesh);

                    // get prop2d (if it exist)
                    GsaProp2d prop = new GsaProp2d();
                    prop.ID = elems[j].Property;
                    if (properties.TryGetValue(elems[j].Property, out Prop2D apiprop))
                        prop.Prop2d = apiprop;
                    else
                        prop.Prop2d = null;

                    prop2Ds.Add(prop);
                    
                    // set elemeent prop to 0 to force export to lookup GsaProp2d
                    elems[j].Property = 0;
                }
                // new mesh to merge existing into
                Mesh m = new Mesh();
                // create one large mesh from single mesh face using
                // append list of meshes (faster than appending each mesh one by one)
                m.Append(mList);

                // if parent member value is 0 then no parent member exist for element
                // we can therefore not be sure all elements with parent member = 0 are
                // connected in one mesh.
                if (hosts[i] == 0 && m.DisjointMeshCount > 1)
                {
                    // revert back to list of meshes instead of the joined one
                    for (int j = 0; j < mList.Count; j++)
                    {
                        // create new element from mesh (takes care of topology lists etc)
                        GsaElement2d singleelement2D = new GsaElement2d(mList[j]);

                        // set elements list of IDs
                        singleelement2D.ID = new List<int>();
                        singleelement2D.ID.Add(IDs[i][j]);

                        // set elements list of properties
                        singleelement2D.Properties = new List<GsaProp2d>();
                        singleelement2D.Properties.Add(prop2Ds[j]);

                        // add element
                        singleelement2D.Elements = new List<Element>();
                        singleelement2D.Elements.Add(elems[j]);

                        // add the element to list of goo 2d elements
                        elem2dGoos.Add(new GsaElement2dGoo(singleelement2D));

                    }
                }
                else
                {
                    // create new element from mesh (takes care of topology lists etc)
                    GsaElement2d element2D = new GsaElement2d(m);

                    // set elements list of IDs
                    //if (!ngon) // we only set this if faces are tri or quad
                    //{
                        element2D.ID = IDs[i];

                        // set elements list of properties
                        element2D.Properties = prop2Ds;

                        // add the element to list of goo 2d elements

                    //}
                    element2D.Elements = elems;

                    while (element2D.Elements.Count != element2D.Properties.Count)
                    {
                        if (element2D.Elements.Count > element2D.Properties.Count)
                            element2D.Properties.Add(element2D.Properties[element2D.Properties.Count - 1]);
                        else
                            element2D.Properties.RemoveAt(element2D.Properties.Count - 1);
                    }
                    
                    elem2dGoos.Add(new GsaElement2dGoo(element2D));
                }
            }
            return elem2dGoos;
        }

        public static Mesh ConvertElement3D(Element element, IReadOnlyDictionary<int, Node> nodes)
        {
            // get element's topology
            ReadOnlyCollection<int> topo = element.Topology;

            // check if element is 3D
            List<bool> check3d = new List<bool>
                    {
                        element.Type == ElementType.THREE_D,
                        element.Type == ElementType.BRICK8,
                        element.Type == ElementType.WEDGE6,
                        element.Type == ElementType.PYRAMID5,
                        element.Type == ElementType.TETRA4
                    };
            if (!check3d.Contains(true))
                return null;

            Mesh outMesh = new Mesh();

            // Get verticies:
            for (int k = 0; k < topo.Count; k++)
            {
                if (nodes.TryGetValue(topo[k], out Node node))
                {
                    {
                        var p = node.Position;
                        outMesh.Vertices.Add(new Point3d(p.X, p.Y, p.Z));
                    }
                }
            }

            // Create 3D element
            switch (topo.Count)
            {
                case 4:
                    // tetrahedron element
                    outMesh.Faces.AddFace(0, 2, 1); //bottom
                    outMesh.Faces.AddFace(0, 1, 3); //side 1
                    outMesh.Faces.AddFace(1, 2, 3); //side 2
                    outMesh.Faces.AddFace(2, 0, 3); //side 3
                    List<int> verts4 = new List<int>() { 0, 1, 2, 3 };
                    List<int> faces4 = new List<int>() { 0, 1, 2, 3 };
                    MeshNgon meshGon4 = MeshNgon.Create(verts4, faces4);
                    outMesh.Ngons.AddNgon(meshGon4);
                    break;

                case 5:
                    // pyramid element
                    outMesh.Faces.AddFace(0, 3, 2, 1); //bottom
                    outMesh.Faces.AddFace(0, 1, 4); //side 1
                    outMesh.Faces.AddFace(1, 2, 4); //side 2
                    outMesh.Faces.AddFace(2, 3, 4); //side 3
                    outMesh.Faces.AddFace(3, 0, 4); //side 4
                    List<int> verts5 = new List<int>() { 0, 1, 2, 3, 4 };
                    List<int> faces5 = new List<int>() { 0, 1, 2, 3, 4 };
                    MeshNgon meshGon5 = MeshNgon.Create(verts5, faces5);
                    outMesh.Ngons.AddNgon(meshGon5);
                    break;

                case 6:
                    // wedge element
                    outMesh.Faces.AddFace(0, 2, 1); //end1
                    outMesh.Faces.AddFace(0, 3, 5, 2); //side 1
                    outMesh.Faces.AddFace(1, 2, 5, 4); //side 2
                    outMesh.Faces.AddFace(0, 1, 4, 3); //side 3
                    outMesh.Faces.AddFace(3, 4, 5); //end 2
                    List<int> verts6 = new List<int>() { 0, 1, 2, 3, 4, 5 };
                    List<int> faces6 = new List<int>() { 0, 1, 2, 3, 4 };
                    MeshNgon meshGon6 = MeshNgon.Create(verts6, faces6);
                    outMesh.Ngons.AddNgon(meshGon6);
                    break;

                case 8:
                    // brick element
                    outMesh.Faces.AddFace(0, 3, 2, 1); //bottom
                    outMesh.Faces.AddFace(0, 1, 5, 4); //side 1
                    outMesh.Faces.AddFace(1, 2, 6, 5); //side 2
                    outMesh.Faces.AddFace(2, 3, 7, 6); //side 2
                    outMesh.Faces.AddFace(3, 0, 4, 7); //side 3
                    outMesh.Faces.AddFace(4, 5, 6, 7); //top
                    List<int> verts8 = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7 };
                    List<int> faces8 = new List<int>() { 0, 1, 2, 3, 4, 5 };
                    MeshNgon meshGon8 = MeshNgon.Create(verts8, faces8);
                    outMesh.Ngons.AddNgon(meshGon8);
                    break;

                default:
                    return null;
            }
            return outMesh;
        }

        public static List<GsaElement3dGoo> ConvertToElement3Ds(Dictionary<int, Element> elements, IReadOnlyDictionary<int, Node> nodes)
        {
            List<List<Element>> apielements = new List<List<Element>>();
            List<List<int>> IDs = new List<List<int>>();
            List<int> hosts = new List<int>();

            // loop through incoming elements and write them to lists with ID and ParentMember number
            // to sort elements in lists containing elements with same parent
            foreach (KeyValuePair<int, Element> elem in elements)
            {
                Element item = DuplicateElement(elem.Value);

                // get parent member
                int host = item.ParentMember.Member;

                // place in list to add elements
                int i;

                // check if the host list is not empty and already contains the host id
                if (hosts.Count > 0 && hosts.Contains(host))
                {
                    // if we already have an element with the same parent then add to list
                    i = hosts.IndexOf(host);
                }
                else
                {
                    // if we dont have any elements with this parent add the list and add element to list    
                    hosts.Add(host);
                    apielements.Add(new List<Element>());
                    IDs.Add(new List<int>());
                    i = hosts.Count - 1;
                }
                apielements[i].Add(item);
                IDs[i].Add(elem.Key);
            }

            // loop through list of elements and create Meshes and Element2Ds 
            List<GsaElement3dGoo> elem3dGoos = new List<GsaElement3dGoo>();
            for (int i = 0; i < apielements.Count; i++)
            {
                // list of elements with same parent
                List<Element> elems = apielements[i];

                // create list of Prop3Ds
                //List<GsaProp2d> prop2Ds = new List<GsaProp2d>();

                // create list of meshes
                List<Mesh> mList = new List<Mesh>();

                // loop through elements in list
                for (int j = 0; j < elems.Count; j++)
                {
                    Mesh ngonClosedMesh = ConvertElement3D(elems[j], nodes);
                    if (ngonClosedMesh == null) { continue; }
                    mList.Add(ngonClosedMesh);

                    // get prop3d (if it exist)
                    //GsaProp2d prop = new GsaProp2d();
                    //prop.ID = elems[j].Property;
                    //if (properties.TryGetValue(elems[j].Property, out Prop2D apiprop))
                    //    prop.Prop2d = apiprop;
                    //else
                    //    prop.Prop2d = null;

                    //prop2Ds.Add(prop);

                    // set elemeent prop to 0 to force export to lookup GsaProp2d
                    //elems[j].Property = 0;
                }
                // new mesh to merge existing into
                Mesh m = new Mesh();
                // create one large mesh from single mesh face using
                // append list of meshes (faster than appending each mesh one by one)
                m.Append(mList);

                // if parent member value is 0 then no parent member exist for element
                // we can therefore not be sure all elements with parent member = 0 are
                // connected in one mesh.
                if (hosts[i] == 0 && m.DisjointMeshCount > 1)
                {
                    // revert back to list of meshes instead of the joined one
                    for (int j = 0; j < mList.Count; j++)
                    {
                        // create new element from mesh (takes care of topology lists etc)
                        GsaElement3d singleelement3D = new GsaElement3d(mList[j]);

                        // set elements list of IDs
                        singleelement3D.ID = new List<int>();
                        singleelement3D.ID.Add(IDs[i][j]);

                        // set elements list of properties
                        //singleelement3D.Properties = new List<GsaProp2d>();
                        //singleelement3D.Properties.Add(prop2Ds[j]);

                        // add element
                        singleelement3D.Elements = new List<Element>();
                        singleelement3D.Elements.Add(elems[j]);

                        // add the element to list of goo 2d elements
                        elem3dGoos.Add(new GsaElement3dGoo(singleelement3D));
                    }
                }
                else
                {
                    // create new element from mesh (takes care of topology lists etc)
                    GsaElement3d element3D = new GsaElement3d(m);

                    // set elements list of IDs
                    //if (!ngon) // we only set this if faces are tri or quad
                    //{
                    element3D.ID = IDs[i];

                    // set elements list of properties
                    //element3D.Properties = prop3Ds;

                    // add the element to list of goo 2d elements

                    //}
                    element3D.Elements = elems;

                    //while (element3D.Elements.Count != element3D.Properties.Count)
                    //{
                    //    if (element3D.Elements.Count > element3D.Properties.Count)
                    //        element3D.Properties.Add(element3D.Properties[element3D.Properties.Count - 1]);
                    //    else
                    //        element3D.Properties.RemoveAt(element3D.Properties.Count - 1);
                    //}

                    elem3dGoos.Add(new GsaElement3dGoo(element3D));
                }
            }
            return elem3dGoos;
        }
        public static Element DuplicateElement(Element elem)
        {
            Element dup = new Element()
            {
                Group = elem.Group,
                IsDummy = elem.IsDummy,
                Name = elem.Name.ToString(),
                Offset = elem.Offset,
                OrientationAngle = elem.OrientationAngle,
                OrientationNode = elem.OrientationNode,
                ParentMember = elem.ParentMember,
                Property = elem.Property,
                Type = elem.Type //GsaToModel.Element1dType((int)Element.Type)
            };
            
            dup.Topology = new ReadOnlyCollection<int>(elem.Topology.ToList());

            if ((System.Drawing.Color)elem.Colour != System.Drawing.Color.FromArgb(0, 0, 0)) // workaround to handle that System.Drawing.Color is non-nullable type
                dup.Colour = elem.Colour;

            dup.Offset.X1 = elem.Offset.X1;
            dup.Offset.X2 = elem.Offset.X2;
            dup.Offset.Y = elem.Offset.Y;
            dup.Offset.Z = elem.Offset.Z;

            return dup;
        }

        /// <summary>
        /// Method to convert a single 1D Element to a GhSA Element 1D
        /// Will output a GsaElement1d
        /// </summary>
        /// <param name="element">GsaAPI Element to be converted</param>
        /// <param name="ID">Element number (key/ID). Set to 0 if this shall be ignored when exporting from Grasshopper</param>
        /// <param name="nodes">Dictionary of Nodes that elements refers to by topology. Include all Nodes unless you are sure what you are doing</param>
        /// <param name="sections">Dictionary of Sections</param>
        /// <returns></returns>
        public static GsaElement1d ConvertToElement1D(Element element, int ID, IReadOnlyDictionary<int, Node> nodes, IReadOnlyDictionary<int, Section> sections)
        {
            // get element's topology
            ReadOnlyCollection<int> topo = element.Topology;

            // ensure the element is a 1D element
            if (topo.Count == 2)
            {
                // get start and end nodes
                List<Point3d> pts = new List<Point3d>();
                for (int i = 0; i <= 1; i++)
                {
                    if (nodes.TryGetValue(topo[i], out Node node))
                    {
                        {
                            var p = node.Position;
                            pts.Add(new Point3d(p.X, p.Y, p.Z));
                        }
                    }
                }
                // create line
                LineCurve ln = new LineCurve(new Line(pts[0], pts[1]));

                // create GH GsaElement1d
                GsaElement1d elem1d = new GsaElement1d
                {
                    Element = element,
                    Line = ln,
                    ID = ID,
                    ReleaseStart = new GsaBool6()
                    {
                        X = element.Release(0).X,
                        Y = element.Release(0).Y,
                        Z = element.Release(0).Z,
                        XX = element.Release(0).XX,
                        YY = element.Release(0).YY,
                        ZZ = element.Release(0).ZZ
                    },
                    ReleaseEnd = new GsaBool6()
                    {
                        X = element.Release(1).X,
                        Y = element.Release(1).Y,
                        Z = element.Release(1).Z,
                        XX = element.Release(1).XX,
                        YY = element.Release(1).YY,
                        ZZ = element.Release(1).ZZ
                    },
                };

                // get section (if it exist)
                elem1d.Section = new GsaSection();
                elem1d.Section.ID = element.Property;
                if (sections.TryGetValue(element.Property, out Section apisection))
                    elem1d.Section.Section = apisection;

                // duplicate element to avoid making backwards changes
                GsaElement1d gsaElement = elem1d.Duplicate(); 

                // set elemeent prop to 0 to force export to lookup GsaSection
                gsaElement.Element.Property = 0;

                return gsaElement;
            }
            return null;
        }
        #endregion
        public static Member DuplicateMember(Member mem)
        {
            Member dup = new Member();
            dup.Group = mem.Group;
            dup.IsDummy = mem.IsDummy;
            dup.MeshSize = mem.MeshSize;
            dup.Name = mem.Name.ToString();
            dup.Offset = mem.Offset;
            dup.OrientationAngle = mem.OrientationAngle;
            dup.OrientationNode = mem.OrientationNode;
            dup.Property = mem.Property;
            dup.Topology = mem.Topology.ToString();
            dup.Type = mem.Type; 
            dup.Type1D = mem.Type1D;
            dup.Type2D = mem.Type2D;

            if ((System.Drawing.Color)mem.Colour != System.Drawing.Color.FromArgb(0, 0, 0)) // workaround to handle that System.Drawing.Color is non-nullable type
                dup.Colour = mem.Colour;

            dup.Offset.X1 = mem.Offset.X1;
            dup.Offset.X2 = mem.Offset.X2;
            dup.Offset.Y = mem.Offset.Y;
            dup.Offset.Z = mem.Offset.Z;

            return dup;
        }
        #region members
        /// <summary>
        /// Method to convert Members to GhSA Member 1D, 2D and 3D
        /// Will output a tuple containing a:
        /// 1. List of GsaMember1dGoos and 
        /// 2. List of GsaMember2dGoos and
        /// 3. List of GsaMember3dGoos
        /// </summary>
        /// <param name="mDict">Dictionary of Members to import</param>
        /// <param name="nDict">Dictionary of Nodes that elements refers to by topology. Include all Nodes unless you are sure what you are doing</param>
        /// <param name="sDict">Dictionary of Sections (for 1D elements)</param>
        /// <param name="pDict">Dictionary of 2D Properties (for 2D elements)</param>
        /// <returns></returns>
        public static Tuple<List<GsaMember1dGoo>, List<GsaMember2dGoo>, List<GsaMember3dGoo>> 
            GetMembers(IReadOnlyDictionary<int, Member> mDict, IReadOnlyDictionary<int, Node> nDict, 
            IReadOnlyDictionary<int, Section> sDict, IReadOnlyDictionary<int, Prop2D> pDict)
        {
            // Create lists for Rhino lines and meshes
            List<GsaMember1dGoo> mem1ds = new List<GsaMember1dGoo>();
            List<GsaMember2dGoo> mem2ds = new List<GsaMember2dGoo>();
            List<GsaMember3dGoo> mem3ds = new List<GsaMember3dGoo>();

            // Loop through all members in Member dictionary 
            foreach (var key in mDict.Keys)
            {
                if (mDict.TryGetValue(key, out Member member))
                {
                    Member mem = DuplicateMember(member);
                    
                    // Get member topology list
                    string toporg = mem.Topology; //original topology list

                    // if 3D member we have different method:
                    if (mem.Type == MemberType.GENERIC_3D)
                    {
                        List<List<int>> topints = Topology_detangler_Mem3d(toporg);
                        
                        // create list of meshes
                        List<Mesh> mList = new List<Mesh>();
                        bool invalid_node = false;
                        // loop through elements in list
                        for (int i = 0; i < topints.Count; i++)
                        {
                            Mesh tempMesh = new Mesh();
                            // Get verticies:
                            for (int j = 0; j < topints[i].Count; j++)
                            {
                                if (nDict.TryGetValue(topints[i][j], out Node node))
                                {
                                    var p = node.Position;
                                    tempMesh.Vertices.Add(new Point3d(p.X, p.Y, p.Z));
                                }
                                else
                                    invalid_node = true; // if node cannot be found continue with next key
                            }

                            // Create mesh face (Tri- or Quad):
                            tempMesh.Faces.AddFace(0, 1, 2);
                            
                            mList.Add(tempMesh);
                        }
                        if (invalid_node)
                            continue;

                        // new mesh to merge existing into
                        Mesh m = new Mesh();
                        
                        // create one large mesh from single mesh face using
                        // append list of meshes (faster than appending each mesh one by one)
                        m.Append(mList);

                        // create 3D member from mesh
                        GsaMember3d mem3d = new GsaMember3d(m);
                        mem3d.ID = key;
                        mem3d.Member = mem;

                        // 3d property to be added
                        // set member prop to 0 to force export to lookup GsaProp3d
                        mem3d.Member.Property = 0;

                        // add member to list
                        mem3ds.Add(new GsaMember3dGoo(mem3d));

                        topints.Clear();
                    }
                    else // Member1D or Member2D 
                    {
                        // Build topology lists:

                        Tuple<Tuple<List<int>, List<string>>, Tuple<List<List<int>>, List<List<string>>>,
                        Tuple<List<List<int>>, List<List<string>>>, List<int>> topologyTuple = Topology_detangler(toporg);
                        // tuple of int and string describing the outline topology
                        Tuple<List<int>, List<string>> topoTuple = topologyTuple.Item1;
                        // tuple of list of int and strings describing voids topology (one list for each void)
                        Tuple<List<List<int>>, List<List<string>>> voidTuple = topologyTuple.Item2;
                        // tuple of list of int and strings describing inclusion lines topology (one list for each line)
                        Tuple<List<List<int>>, List<List<string>>> lineTuple = topologyTuple.Item3;

                        List<int> topo_int = topoTuple.Item1;
                        List<string> topoType = topoTuple.Item2; //list of polyline curve type (arch or line) for member1d/2d

                        List<List<int>> void_topo_int = voidTuple.Item1;
                        List<List<string>> void_topoType = voidTuple.Item2; //list of polyline curve type (arch or line) for void /member2d

                        List<List<int>> incLines_topo_int = lineTuple.Item1;
                        List<List<string>> inclLines_topoType = lineTuple.Item2; //list of polyline curve type (arch or line) for inclusion /member2d

                        List<int> inclpts = topologyTuple.Item4;

                        // replace topology integers with actual points
                        List<Point3d> topopts = new List<Point3d>(); // list of topology points for visualisation /member1d/member2d
                        bool invalid_node = false;
                        for (int i = 0; i < topo_int.Count; i++)
                        {
                            if (nDict.TryGetValue(topo_int[i], out Node node))
                            {
                                var p = node.Position;
                                topopts.Add(new Point3d(p.X, p.Y, p.Z));
                            }
                            else
                                invalid_node = true; // if node cannot be found continue with next key
                        }
                        if (invalid_node)
                            continue;

                        //list of lists of void points /member2d
                            List<List<Point3d>> void_topo = new List<List<Point3d>>();
                        for (int i = 0; i < void_topo_int.Count; i++)
                        {
                            void_topo.Add(new List<Point3d>());
                            for (int j = 0; j < void_topo_int[i].Count; j++)
                            {
                                if (nDict.TryGetValue(void_topo_int[i][j], out Node node))
                                {
                                    var p = node.Position;
                                    void_topo[i].Add(new Point3d(p.X, p.Y, p.Z));
                                }
                            }
                        }

                        //list of lists of line inclusion topology points /member2d
                        List<List<Point3d>> incLines_topo = new List<List<Point3d>>();
                        for (int i = 0; i < incLines_topo_int.Count; i++)
                        {
                            incLines_topo.Add(new List<Point3d>());
                            for (int j = 0; j < incLines_topo_int[i].Count; j++)
                            {
                                if (nDict.TryGetValue(incLines_topo_int[i][j], out Node node))
                                {
                                    var p = node.Position;
                                    incLines_topo[i].Add(new Point3d(p.X, p.Y, p.Z));
                                }
                            }
                        }

                        //list of points for inclusion /member2d
                        List<Point3d> incl_pts = new List<Point3d>();
                        for (int i = 0; i < inclpts.Count; i++)
                        {
                            if (nDict.TryGetValue(inclpts[i], out Node node))
                            {
                                var p = node.Position;
                                incl_pts.Add(new Point3d(p.X, p.Y, p.Z));
                            }
                        }

                        // create GhSA Members

                        // Member1D:
                        if (mem.Type == MemberType.GENERIC_1D | mem.Type == MemberType.BEAM | mem.Type == MemberType.CANTILEVER |
                            mem.Type == MemberType.COLUMN | mem.Type == MemberType.COMPOS | mem.Type == MemberType.PILE)
                        {
                            // check if Mem1D topology has minimum 2 points
                            // if invalid we try import settings, props, ets
                            // so it can be used by other components. We use
                            // the same point twice and create a 0-length line
                            if (topopts.Count < 2)
                            {
                                topopts.Add(topopts[0]);
                                topoType.Add(topoType[0]);
                            }

                            // create the element from list of points and type description
                            GsaMember1d mem1d = new GsaMember1d(topopts.ToList(), topoType.ToList());
                            mem1d.ID = key;
                            mem1d.Member = mem;

                            // get section (if it exist)
                            mem1d.Section = new GsaSection();
                            mem1d.Section.ID = mem1d.Member.Property;
                            if (sDict.TryGetValue(mem1d.Member.Property, out Section apisection))
                                mem1d.Section.Section = apisection;
                            else
                                mem1d.Section.Section = null;

                            // set member prop to 0 to force export to lookup GsaSection
                            mem1d.Member.Property = 0;

                            // releases to be implemented here - GsaAPI

                            // add member to output list
                            mem1ds.Add(new GsaMember1dGoo(mem1d));
                        }
                        else // Member2D:
                        {
                            
                            // create member from topology lists
                            GsaMember2d mem2d = new GsaMember2d(
                                topopts.ToList(),
                                topoType.ToList(),
                                void_topo.ToList(),
                                void_topoType.ToList(),
                                incLines_topo.ToList(),
                                inclLines_topoType.ToList(),
                                incl_pts.ToList());
                            mem2d.ID = key;
                            mem2d.Member = mem;

                            // create 2d property
                            mem2d.Property = new GsaProp2d();
                            mem2d.Property.ID = mem2d.Member.Property;
                            if (pDict.TryGetValue(mem2d.Member.Property, out Prop2D tempProp))
                                mem2d.Property.Prop2d = tempProp;
                            else
                                mem2d.Property.Prop2d = null;
                            
                            // set member prop to 0 to force export to lookup GsaProp2d
                            mem2d.Member.Property = 0;

                            // add member to output list
                            mem2ds.Add(new GsaMember2dGoo(mem2d));
                        }

                        topopts.Clear();
                        topoType.Clear();
                        void_topo.Clear();
                        void_topoType.Clear();
                        incLines_topo.Clear();
                        inclLines_topoType.Clear();
                        incl_pts.Clear();
                    }
                }
            }
            return new Tuple<List<GsaMember1dGoo>, List<GsaMember2dGoo>, List<GsaMember3dGoo>>(mem1ds, mem2ds, mem3ds);
        }
        #endregion

        #region topology string manipulation

        /// <summary>
        /// Method to split/untangle a topology list from GSA into separate lists for
        /// Topology, Voids, Inclusion lines and Inclusion points with corrosponding list for topology type.
        /// 
        /// Output tuple with three sub-tubles for:
        /// - Topology: (Topology integers and topology types)
        /// - Voids: (List of integers and list of topology types)
        /// - Lines: (List of integers and list of topology types)
        /// - Points: (Topology integers)
        /// 
        /// Example: gsa_topology = 
        /// "7 8 9 a 10 11 7 V(12 13 a 14 15) L(16 a 18 17) 94 P 20 P(19 21 22) L(23 24) 84"
        /// will results in:
        /// 
        /// Tuple1, Item1: Topology: (7, 8, 9, 10, 11, 7, 94, 84)
        /// Tuple1, Item2: TopoType: ( ,  ,  ,  a,   ,  ,   ,   )
        /// 
        /// Tuple2, Item1: List(Voids): (12, 13, 14, 15)
        /// Tuple2, Item2: List(VType): (  ,   ,  a,   )
        /// 
        /// Tuple3, Item1: List(Lines): (16, 18, 17) (23, 24)
        /// Tuple3, Item2: List(LType): (  ,  a,   ) (  ,   )
        /// 
        /// Points: (20, 19, 21, 22)
        /// 
        /// </summary>
        /// <param name="gsa_topology"></param>
        /// <returns></returns>
        public static Tuple<Tuple<List<int>, List<string>>, Tuple<List<List<int>>, List<List<string>>>,
            Tuple<List<List<int>>, List<List<string>>>, List<int>> Topology_detangler(string gsa_topology)
        {
            List<string> voids = new List<string>();
            List<string> lines = new List<string>();
            List<string> points = new List<string>();
            //string gsa_topology = "7 8 9 a 10 11 7 V(12 13 a 14 15) L(16 a 18 17) 94 P 20 P(19 21 22) L(23 24) 84";
            gsa_topology = gsa_topology.ToUpper();
            char[] spearator = { '(', ')' };

            String[] strlist = gsa_topology.Split(spearator);
            List<String> topos = new List<String>(strlist);

            // first split out anything in brackets and put them into lists for V, L or P
            // also remove those lines so that they dont appear twice in the end
            for (int i = 0; i < topos.Count(); i++)
            {
                if (topos[i].Length > 1)
                {
                    if (topos[i].Substring(topos[i].Length - 1, 1) == "V")
                    {
                        topos[i] = topos[i].Substring(0, topos[i].Length - 1);
                        voids.Add(topos[i + 1]);
                        topos.RemoveAt(i + 1);
                        continue;
                    }
                }

                if (topos[i].Length > 1)
                {
                    if (topos[i].Substring(topos[i].Length - 1, 1) == "L")
                    {
                        topos[i] = topos[i].Substring(0, topos[i].Length - 1);
                        lines.Add(topos[i + 1]);
                        topos.RemoveAt(i + 1);
                        continue;
                    }
                }

                if (topos[i].Length > 1)
                {
                    if (topos[i].Substring(topos[i].Length - 1, 1) == "P")
                    {
                        topos[i] = topos[i].Substring(0, topos[i].Length - 1);
                        points.Add(topos[i + 1]);
                        topos.RemoveAt(i + 1);
                        continue;
                    }
                }
            }

            // then split list with whitespace
            List<String> topolos = new List<String>();
            for (int i = 0; i < topos.Count(); i++)
            {
                List<String> temptopos = new List<String>(topos[i].Split(' '));
                topolos.AddRange(temptopos);
            }

            // also split list of points by whitespace as they go to single list
            List<String> pts = new List<String>();
            for (int i = 0; i < points.Count(); i++)
            {
                List<String> temppts = new List<String>(points[i].Split(' '));
                pts.AddRange(temppts);
            }

            // voids and lines needs to be made into list of lists
            List<List<int>> void_topo = new List<List<int>>();
            List<List<String>> void_topoType = new List<List<String>>();
            for (int i = 0; i < voids.Count(); i++)
            {
                List<String> tempvoids = new List<String>(voids[i].Split(' '));
                List<int> tmpvds = new List<int>();
                List<String> tmpType = new List<String>();
                for (int j = 0; j < tempvoids.Count(); j++)
                {
                    if (tempvoids[j] == "A")
                    {
                        tmpType.Add("A");
                        tempvoids.RemoveAt(j);
                    }
                    else
                        tmpType.Add(" ");
                    int tpt = Int32.Parse(tempvoids[j]);
                    tmpvds.Add(tpt);
                }
                void_topo.Add(tmpvds);
                void_topoType.Add(tmpType);
            }
            List<List<int>> incLines_topo = new List<List<int>>();
            List<List<String>> inclLines_topoType = new List<List<String>>();
            for (int i = 0; i < lines.Count(); i++)
            {
                List<String> templines = new List<String>(lines[i].Split(' '));
                List<int> tmplns = new List<int>();
                List<String> tmpType = new List<String>();
                for (int j = 0; j < templines.Count(); j++)
                {
                    if (templines[j] == "A")
                    {
                        tmpType.Add("A");
                        templines.RemoveAt(j);
                    }
                    else
                        tmpType.Add(" ");
                    int tpt = Int32.Parse(templines[j]);
                    tmplns.Add(tpt);
                }
                incLines_topo.Add(tmplns);
                inclLines_topoType.Add(tmpType);
            }

            // then remove empty entries
            for (int i = 0; i < topolos.Count(); i++)
            {
                if (topolos[i] == null)
                {
                    topolos.RemoveAt(i);
                    i -= 1;
                    continue;
                }
                if (topolos[i].Length < 1)
                {
                    topolos.RemoveAt(i);
                    i -= 1;
                    continue;
                }
            }

            // Find any single inclusion points not in brackets
            for (int i = 0; i < topolos.Count(); i++)
            {
                if (topolos[i] == "P")
                {
                    pts.Add(topolos[i + 1]);
                    topolos.RemoveAt(i + 1);
                    topolos.RemoveAt(i);
                    i -= 1;
                    continue;
                }
                if (topolos[i].Length < 1)
                {
                    topolos.RemoveAt(i);
                    i -= 1;
                    continue;
                }
            }
            List<int> inclpoint = new List<int>();
            for (int i = 0; i < pts.Count(); i++)
            {
                int tpt = Int32.Parse(pts[i]);
                inclpoint.Add(tpt);
            }

            // write out topology type (A) to list
            List<int> topoint = new List<int>();
            List<String> topoType = new List<String>();
            for (int i = 0; i < topolos.Count(); i++)
            {
                if (topolos[i] == "A")
                {
                    topoType.Add("A");
                    int tptA = Int32.Parse(topolos[i + 1]);
                    topoint.Add(tptA);
                    i += 1;
                    continue;
                }
                topoType.Add(" ");
                int tpt = Int32.Parse(topolos[i]);
                topoint.Add(tpt);
            }
            Tuple<List<int>, List<string>> topoTuple = new Tuple<List<int>, List<string>>(topoint, topoType);
            Tuple<List<List<int>>, List<List<string>>> voidTuple = new Tuple<List<List<int>>, List<List<string>>>(void_topo, void_topoType);
            Tuple<List<List<int>>, List<List<string>>> lineTuple = new Tuple<List<List<int>>, List<List<string>>>(incLines_topo, inclLines_topoType);
            
            return new Tuple<Tuple<List<int>, List<string>>, Tuple<List<List<int>>, List<List<string>>>,
            Tuple<List<List<int>>, List<List<string>>>, List<int>>(topoTuple, voidTuple, lineTuple, inclpoint);
        }
        
        /// <summary>
        /// Method to convert a topology string from a 3D Member
        /// into a list of 3 verticies
        /// </summary>
        /// <param name="gsa_topology">Topology list as string</param>
        /// <returns></returns>
        public static List<List<int>> Topology_detangler_Mem3d(string gsa_topology)
        {
            // Example input string ‘1 2 4 3; 5 6 8 7; 1 5 2 6 3 7 4 8 1 5’ 
            // we want to create a triangular mesh for Member3D SolidMesh
            List<List<int>> topolist = new List<List<int>>();

            // first split the string by ";"
            char spearator = ';';

            String[] strlist = gsa_topology.Split(spearator);

            // loop through all face lists
            foreach (string stripe in strlist)
            {
                // trim and split list by white space
                string trimmedstripe = stripe.Trim();
                List<String> verticiesString = new List<String>(trimmedstripe.Split(' '));

                // convert string to int
                List<int> tempverticies = new List<int>();
                foreach (string vert in verticiesString)
                {
                    int tpt = Int32.Parse(vert);
                    tempverticies.Add(tpt);
                }

                while (tempverticies.Count > 2)
                {
                    // add the first triangle
                    List<int> templist1 = new List<int>();
                    templist1.Add(tempverticies[0]);
                    templist1.Add(tempverticies[1]);
                    templist1.Add(tempverticies[2]);
                    
                    // add the list to the main list
                    topolist.Add(templist1);

                    if (tempverticies.Count > 3)
                    {
                        // add the second triangle the other way round
                        List<int> templist2 = new List<int>();
                        templist2.Add(tempverticies[1]);
                        templist2.Add(tempverticies[3]);
                        templist2.Add(tempverticies[2]);

                        // add the list to the main list
                        topolist.Add(templist2);

                        // remove the first two verticies from list
                        tempverticies.RemoveAt(0);
                    }
                    // put the second remove outside the if to also remove if we only 
                    // have 3 verticies to bring count below 3 and exit while loop
                    tempverticies.RemoveAt(0);
                }
            }
            return topolist;

        }
        #endregion

        #region section and properties
        /// <summary>
        /// Method to import Sections from a GSA model.
        /// Will output a list of GhSA GsaSectionsGoo.
        /// </summary>
        /// <param name="sDict">Dictionary of pre-filtered sections to import</param>
        /// <returns></returns>
        public static List<GsaSectionGoo> GetSections(IReadOnlyDictionary<int, Section> sDict)
        {
            List<GsaSectionGoo> sections = new List<GsaSectionGoo>();

            // Loop through all sections in Section dictionary and create new GsaSections
            foreach (int key in sDict.Keys)
            {
                if (sDict.TryGetValue(key, out Section apisection)) //1-base numbering
                {
                    GsaSection sect = new GsaSection
                    {
                        ID = key,
                        Section = apisection
                    };
                    sections.Add(new GsaSectionGoo(sect));
                }
            }
            return sections;
        }
        /// <summary>
        /// Method to import Prop2ds from a GSA model.
        /// Will output a list of GhSA GsaProp2dGoo.
        /// </summary>
        /// <param name="pDict">Dictionary of pre-filtered 2D Properties to import</param>
        /// <returns></returns>
        public static List<GsaProp2dGoo> GetProp2ds(IReadOnlyDictionary<int, Prop2D> pDict)
        {
            List<GsaProp2dGoo> prop2ds = new List<GsaProp2dGoo>();

            // Loop through all sections in Properties dictionary and create new GsaProp2d
            foreach (int key in pDict.Keys)
            {
                if (pDict.TryGetValue(key, out Prop2D apisection)) //1-base numbering
                {
                    GsaProp2d prop = new GsaProp2d
                    {
                        ID = key,
                        Prop2d = apisection
                    };
                    prop2ds.Add(new GsaProp2dGoo(prop));
                }
            }
            return prop2ds;
        }
        #endregion

        #region loads
        /// <summary>
        /// Method to import Gravity Loads from a GSA model.
        /// Will output a list of GhSA GsaLoadsGoo.
        /// </summary>
        /// <param name="gravityLoads">Collection of gravity loads to import</param>
        /// <returns></returns>
        public static List<GsaLoadGoo> GetGravityLoads(ReadOnlyCollection<GravityLoad> gravityLoads)
        {
            List<GsaLoadGoo> loads = new List<GsaLoadGoo>();

            List<GravityLoad> gloads = gravityLoads.ToList();

            // Loop through all loads in list and create new GsaLoads
            for (int i = 0; i < gloads.Count; i++)
            {
                GsaGravityLoad myload = new GsaGravityLoad();
                myload.GravityLoad = gloads[i];
                GsaLoad load = new GsaLoad(myload);
                loads.Add(new GsaLoadGoo(load));
            }
            return loads;
        }
        /// <summary>
        /// Method to import all Node Loads from a GSA model.
        /// 
        /// GSA Node loads vary by type, to get all node loads easiest
        /// method seems to be toogling through all enum types which
        /// requeres the entire model to be inputted to this method.
        /// 
        /// Will output a list of GhSA GsaLoads.
        /// </summary>
        /// <param name="model">GSA model containing node loads</param>
        /// <returns></returns>
        public static List<GsaLoadGoo> GetNodeLoads(Model model)
        {
            List<GsaLoadGoo> loads = new List<GsaLoadGoo>();

            // NodeLoads come in varioys types, depending on GsaAPI.NodeLoadType:
            foreach (NodeLoadType typ in Enum.GetValues(typeof(NodeLoadType)))
            {
                try // some GsaAPI.NodeLoadTypes are currently not supported in the API and throws an error
                {
                    List<NodeLoad> gsaloads = model.NodeLoads(typ).ToList();
                    GsaNodeLoad.NodeLoadTypes ntyp = GsaNodeLoad.NodeLoadTypes.NODE_LOAD;
                    switch (typ)
                    {
                        case NodeLoadType.APPL_DISP:
                            ntyp = GsaNodeLoad.NodeLoadTypes.APPLIED_DISP;
                            break;
                        case NodeLoadType.GRAVITY:
                            ntyp = GsaNodeLoad.NodeLoadTypes.GRAVITY;
                            break;
                        case NodeLoadType.NODE_LOAD:
                            ntyp = GsaNodeLoad.NodeLoadTypes.NODE_LOAD;
                            break;
                        case NodeLoadType.NUM_TYPES:
                            ntyp = GsaNodeLoad.NodeLoadTypes.NUM_TYPES;
                            break;
                        case NodeLoadType.SETTLEMENT:
                            ntyp = GsaNodeLoad.NodeLoadTypes.SETTLEMENT;
                            break;
                    }

                    // Loop through all loads in list and create new GsaLoads
                    for (int i = 0; i < gsaloads.Count; i++)
                    {
                        GsaNodeLoad myload = new GsaNodeLoad();
                        myload.NodeLoad = gsaloads[i];
                        myload.NodeLoadType = ntyp;
                        GsaLoad load = new GsaLoad(myload);
                        loads.Add(new GsaLoadGoo(load));
                    }
                }
                catch (Exception)
                {

                }

            }
            return loads;
        }
        /// <summary>
        /// Method to import Beam Loads from a GSA model.
        /// Will output a list of GhSA GsaLoads.
        /// </summary>
        /// <param name="beamLoads">Collection of beams loads to be imported</param>
        /// <returns></returns>
        public static List<GsaLoadGoo> GetBeamLoads(ReadOnlyCollection<BeamLoad> beamLoads)
        {
            List<GsaLoadGoo> loads = new List<GsaLoadGoo>();

            List<BeamLoad> gsaloads = beamLoads.ToList();

            // Loop through all loads in list and create new GsaLoads
            for (int i = 0; i < gsaloads.Count; i++)
            {
                GsaBeamLoad myload = new GsaBeamLoad();
                myload.BeamLoad = gsaloads[i];
                GsaLoad load = new GsaLoad(myload);
                loads.Add(new GsaLoadGoo(load));
            }
            return loads;
        }
        /// <summary>
        /// Method to import Face Loads from a GSA model.
        /// Will output a list of GhSA GsaLoads.
        /// </summary>
        /// <param name="faceLoads">Collection of Face loads to be imported</param>
        /// <returns></returns>
        public static List<GsaLoadGoo> GetFaceLoads(ReadOnlyCollection<FaceLoad> faceLoads)
        {
            List<GsaLoadGoo> loads = new List<GsaLoadGoo>();

            List<FaceLoad> gsaloads = faceLoads.ToList();

            // Loop through all loads in list and create new GsaLoads
            for (int i = 0; i < gsaloads.Count; i++)
            {
                GsaFaceLoad myload = new GsaFaceLoad();
                myload.FaceLoad = gsaloads[i];
                GsaLoad load = new GsaLoad(myload);
                loads.Add(new GsaLoadGoo(load));
            }
            return loads;
        }
        /// <summary>
        /// Method to import Grid Point Loads from a GSA model.
        /// Will output a list of GhSA GsaLoads.
        /// </summary>
        /// <param name="pointLoads">Collection of Grid Point loads to be imported</param>
        /// <param name="srfDict">Grid Surface Dictionary</param>
        /// <param name="plnDict">Grid Plane Dictionary</param>
        /// <param name="axDict">Axes Dictionary</param>
        /// <returns></returns>
        public static List<GsaLoadGoo> GetGridPointLoads(ReadOnlyCollection<GridPointLoad> pointLoads,
            IReadOnlyDictionary<int, GridSurface> srfDict, IReadOnlyDictionary<int, GridPlane> plnDict, IReadOnlyDictionary<int, GsaAPI.Axis> axDict)
        {
            List<GsaLoadGoo> loads = new List<GsaLoadGoo>();

            List<GridPointLoad> gsaloads = pointLoads.ToList();

            // Loop through all loads in list and create new GsaLoads
            for (int i = 0; i < gsaloads.Count; i++)
            {
                // Get Grid Point Load
                GsaGridPointLoad myload = new GsaGridPointLoad();
                myload.GridPointLoad = gsaloads[i];

                // Get GridPlaneSurface
                myload.GridPlaneSurface = GetGridPlaneSurface(srfDict, plnDict, axDict, gsaloads[i].GridSurface);

                // Add load to list
                GsaLoad load = new GsaLoad(myload);
                loads.Add(new GsaLoadGoo(load));
            }
            return loads;
        }
        /// <summary>
        /// Method to import Grid Line Loads from a GSA model.
        /// Will output a list of GhSA GsaLoads.
        /// </summary>
        /// <param name="lineLoads">Collection of Grid Line loads to be imported</param>
        /// <param name="srfDict">Grid Surface Dictionary</param>
        /// <param name="plnDict">Grid Plane Dictionary</param>
        /// <param name="axDict">Axes Dictionary</param>
        /// <returns></returns>
        public static List<GsaLoadGoo> GetGridLineLoads(ReadOnlyCollection<GridLineLoad> lineLoads,
            IReadOnlyDictionary<int, GridSurface> srfDict, IReadOnlyDictionary<int, GridPlane> plnDict, IReadOnlyDictionary<int, GsaAPI.Axis> axDict)
        {
            List<GsaLoadGoo> loads = new List<GsaLoadGoo>();

            List<GridLineLoad> gsaloads = lineLoads.ToList();

            // Loop through all loads in list and create new GsaLoads
            for (int i = 0; i < gsaloads.Count; i++)
            {
                // Get Grid Point Load
                GsaGridLineLoad myload = new GsaGridLineLoad();
                myload.GridLineLoad = gsaloads[i];

                // Get GridPlaneSurface
                myload.GridPlaneSurface = GetGridPlaneSurface(srfDict, plnDict, axDict, gsaloads[i].GridSurface);

                // Add load to list
                GsaLoad load = new GsaLoad(myload);
                loads.Add(new GsaLoadGoo(load));
            }
            return loads;
        }
        /// <summary>
        /// Method to import Grid Area Loads from a GSA model.
        /// Will output a list of GhSA GsaLoads.
        /// </summary>
        /// <param name="areaLoads">Collection of Grid Area loads to be imported</param>
        /// <param name="srfDict">Grid Surface Dictionary</param>
        /// <param name="plnDict">Grid Plane Dictionary</param>
        /// <param name="axDict">Axes Dictionary</param>
        /// <returns></returns>
        public static List<GsaLoadGoo> GetGridAreaLoads(ReadOnlyCollection<GridAreaLoad> areaLoads, 
            IReadOnlyDictionary<int, GridSurface> srfDict, IReadOnlyDictionary<int, GridPlane> plnDict, IReadOnlyDictionary<int, GsaAPI.Axis> axDict)
        {
            List<GsaLoadGoo> loads = new List<GsaLoadGoo>();

            List<GridAreaLoad> gsaloads = areaLoads.ToList();

            // Loop through all loads in list and create new GsaLoads
            for (int i = 0; i < gsaloads.Count; i++)
            {
                // Get Grid Point Load
                GsaGridAreaLoad myload = new GsaGridAreaLoad();
                myload.GridAreaLoad = gsaloads[i];

                // Get GridPlaneSurface
                myload.GridPlaneSurface = GetGridPlaneSurface(srfDict, plnDict, axDict, gsaloads[i].GridSurface);

                // Add load to list
                GsaLoad load = new GsaLoad(myload);
                loads.Add(new GsaLoadGoo(load));
            }
            return loads;
        }

        /// <summary>
        /// Method to create GsaGridPlaneSurface including 
        /// grid surface, grid plane and axis from GSA Model
        /// 
        /// Grid Surface references a Grid Plane
        /// Grid Plane references an Axis
        /// Only Grid Surface ID is required, the others will be found by ref
        /// 
        /// Will output a new GsaGridPlaneSurface.
        /// </summary>
        /// <param name="srfDict">Grid Surface Dictionary</param>
        /// <param name="plnDict">Grid Plane Dictionary</param>
        /// <param name="axDict">Axes Dictionary</param>
        /// <param name="gridsrf_ID">ID/Key/number of Grid Surface in GSA model to convert</param>
        /// <returns></returns>
        public static GsaGridPlaneSurface GetGridPlaneSurface(IReadOnlyDictionary<int, GridSurface> srfDict, IReadOnlyDictionary<int, GridPlane> plnDict, IReadOnlyDictionary<int, GsaAPI.Axis> axDict, int gridsrf_ID)
        {
            // GridPlaneSurface
            GsaGridPlaneSurface gps = new GsaGridPlaneSurface();

            // Get Grid Surface
            if (srfDict.Count > 0)
            {
                srfDict.TryGetValue(gridsrf_ID, out GridSurface gs);
                gps.GridSurface = gs;
                gps.GridSurfaceID = gridsrf_ID;

                // Get Grid Plane
                plnDict.TryGetValue(gs.GridPlane, out GridPlane gp);
                gps.GridPlane = gp;
                gps.GridPlaneID = gs.GridPlane;

                // Get Axis
                axDict.TryGetValue(gp.AxisProperty, out GsaAPI.Axis ax);
                if (ax == null)
                {
                    ax = new GsaAPI.Axis();
                    ax.Origin.X = 0;
                    ax.Origin.Y = 0;
                    ax.Origin.Z = 0;
                    ax.XVector.X = 1;
                    ax.XVector.Y = 0;
                    ax.XVector.Y = 0;
                    ax.XYPlane.X = 0;
                    ax.XYPlane.Y = 1;
                    ax.XYPlane.Z = 0;
                }
                gps.Axis = ax;
                gps.AxisID = gp.AxisProperty;


                // Construct Plane from Axis
                Plane pln = new Plane();
                if (ax != null)
                {
                    pln = new Plane(
                    new Point3d(ax.Origin.X, ax.Origin.Y, ax.Origin.Z + gp.Elevation), // for new origin Z-coordinate we add axis origin and grid plane elevation
                    new Vector3d(ax.XVector.X, ax.XVector.Y, ax.XVector.Z),
                    new Vector3d(ax.XYPlane.X, ax.XYPlane.Y, ax.XYPlane.Z));
                }
                else
                {
                    pln = Plane.WorldXY;
                    pln.OriginZ = gp.Elevation;
                }
                gps.Plane = pln;
            }
            else
                return null;

            return gps;
        }
        #endregion

    }
}
