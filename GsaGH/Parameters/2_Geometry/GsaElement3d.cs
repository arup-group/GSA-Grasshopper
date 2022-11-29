using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using Grasshopper;
using GsaAPI;
using GsaGH.Helpers.GsaAPI;
using Rhino.Geometry;

namespace GsaGH.Parameters
{
    /// <summary>
    /// Element3d class, this class defines the basic properties and methods for any Gsa Element 3d
    /// </summary>
    public class GsaElement3d
  {
    private enum ApiObjectMember
    {
      all,
      group,
      dummy,
      name,
      orientationAngle,
      offset,
      property,
      type,
      colour
    }

    #region fields
    private List<Element> _elements = new List<Element>();
    private Mesh _mesh = new Mesh();
    private List<List<int>> _topoInt; // list of topology integers referring to the topo list of points
    private List<List<int>> _faceInt; // list of face integers included in each solid mesh referring to the mesh face list
    private List<Point3d> _topo; // list of topology points for visualisation
    private List<int> _ids = new List<int>();
    private Guid _guid = Guid.NewGuid();
    private List<GsaProp3d> _props = new List<GsaProp3d>();
    private Mesh _displayMesh;
    #endregion

    #region properties
    internal List<Element> API_Elements
    {
      get
      {
        return this._elements;
      }
      set
      {
        this._elements = value;
      }
    }
    internal Element GetApiObjectClone(int i)
    {
      return new Element()
      {
        Group = this._elements[i].Group,
        IsDummy = this._elements[i].IsDummy,
        Name = this._elements[i].Name.ToString(),
        OrientationNode = this._elements[i].OrientationNode,
        OrientationAngle = this._elements[i].OrientationAngle,
        Offset = this._elements[i].Offset,
        ParentMember = this._elements[i].ParentMember,
        Property = this._elements[i].Property,
        Topology = new ReadOnlyCollection<int>(this._elements[i].Topology.ToList()),
        Type = this._elements[i].Type //GsaToModel.Element2dType((int)Elements[i].Type)
      };
    }
    public int Count
    {
      get
      {
        return this._elements.Count;
      }
    }
    public Mesh NgonMesh
    {
      get
      {
        return this._mesh;
      }
    }
    public Mesh DisplayMesh
    {
      get
      {
        if (this._displayMesh == null)
        {
          UpdatePreview();
        }
        return this._displayMesh;
      }
    }

    public List<Point3d> Topology
    {
      get
      {
        return this._topo;
      }
      set
      {
        this._topo = value;
      }
    }
    public List<int> IDs
    {
      get
      {
        return this._ids;
      }
      set
      {
        this._ids = value;
      }
    }
    public List<List<int>> TopoInt
    {
      get
      {
        return this._topoInt;
      }
      set
      {
        this._topoInt = value;
      }
    }
    public List<List<int>> FaceInt
    {
      get
      {
        return this._faceInt;
      }
      set
      {
        this._faceInt = value;
      }
    }
    public List<GsaProp3d> Properties
    {
      get
      {
        return this._props;
      }
      set
      {
        this._props = value;
      }
    }
    public DataTree<int> TopologyIDs
    {
      get
      {
        DataTree<int> topos = new DataTree<int>();
        for (int i = 0; i < this._elements.Count; i++)
        {
          if (this._elements[i] != null)
            topos.AddRange(this._elements[i].Topology.ToList(), new Grasshopper.Kernel.Data.GH_Path(i));
        }
        return topos;
      }
    }
    public Guid Guid
    {
      get
      {
        return this._guid;
      }
    }
    #region GsaAPI.Element members
    public List<Color> Colours
    {
      get
      {
        List<Color> cols = new List<Color>();
        for (int i = 0; i < this._elements.Count; i++)
        {
          if ((Color)this._elements[i].Colour == Color.FromArgb(0, 0, 0))
          {
            this._elements[i].Colour = Color.FromArgb(50, 150, 150, 150);
          }
          cols.Add((Color)this._elements[i].Colour);

          NgonMesh.VertexColors.SetColor(i, (Color)this._elements[i].Colour);
        }
        return cols;
      }
      set
      {
        this.CloneApiElements(ApiObjectMember.colour, null, null, null, null, null, null, null, value);
      }
    }
    public List<int> Groups
    {
      get
      {
        List<int> groups = new List<int>();
        for (int i = 0; i < this._elements.Count; i++)
        {
          if (this._elements[i] != null)
            groups.Add(this._elements[i].Group);
        }
        return groups;
      }
      set
      {
        this.CloneApiElements(ApiObjectMember.group, value);
      }
    }
    public List<bool> IsDummies
    {
      get
      {
        List<bool> dums = new List<bool>();
        for (int i = 0; i < this._elements.Count; i++)
        {
          if (this._elements[i] != null)
            dums.Add(this._elements[i].IsDummy);
        }
        return dums;
      }
      set
      {
        this.CloneApiElements(ApiObjectMember.dummy, null, value);
      }
    }
    public List<string> Names
    {
      get
      {
        List<string> names = new List<string>();
        for (int i = 0; i < this._elements.Count; i++)
        {
          if (this._elements[i] != null)
            names.Add(this._elements[i].Name);
        }
        return names;
      }
      set
      {
        this.CloneApiElements(ApiObjectMember.dummy, null, null, value);
      }
    }
    public List<double> OrientationAngles
    {
      get
      {
        List<double> angles = new List<double>();
        for (int i = 0; i < this._elements.Count; i++)
        {
          if (this._elements[i] != null)
            angles.Add(this._elements[i].OrientationAngle);
        }
        return angles;
      }
      set
      {
        this.CloneApiElements(ApiObjectMember.dummy, null, null, null, value);
      }
    }
    public List<GsaOffset> Offsets
    {
      get
      {
        List<GsaOffset> offs = new List<GsaOffset>();
        for (int i = 0; i < this._elements.Count; i++)
        {
          if (this._elements[i] != null)
          {
            GsaOffset off = new GsaOffset(this._elements[i].Offset.X1, this._elements[i].Offset.X2, this._elements[i].Offset.Y, this._elements[i].Offset.Z);
            offs.Add(off);
          }
        }
        return offs;
      }
      set
      {
        this.CloneApiElements(ApiObjectMember.dummy, null, null, null, null, value);
      }
    }
    public List<int> PropertyIDs
    {
      get
      {
        List<int> propids = new List<int>();
        for (int i = 0; i < this._elements.Count; i++)
        {
          if (this._elements[i] != null)
            propids.Add(this._elements[i].Property);
        }
        return propids;
      }
      set
      {
        this.CloneApiElements(ApiObjectMember.dummy, null, null, null, null, null, value);
      }
    }
    public List<ElementType> Types
    {
      get
      {
        List<ElementType> typs = new List<ElementType>();
        for (int i = 0; i < this._elements.Count; i++)
        {
          if (this._elements[i] != null)
            typs.Add(this._elements[i].Type);
        }
        return typs;
      }
      set
      {
        this.CloneApiElements(ApiObjectMember.dummy, null, null, null, null, null, null, value);
      }
    }
    public List<int> ParentMembers
    {
      get
      {
        List<int> pMems = new List<int>();
        for (int i = 0; i < this._elements.Count; i++)
          try
          {
            pMems.Add(this._elements[i].ParentMember.Member);
          }
          catch (Exception)
          {
            pMems.Add(0);
          }
        return pMems;
      }
    }
    #endregion
    #endregion

    #region constructors
    public GsaElement3d()
    {
      //this._props = new List<GsaProp2d>();
    }

    public GsaElement3d(Mesh mesh, int prop = 0)
    {
      this._mesh = mesh;
      Tuple<List<Element>, List<Point3d>, List<List<int>>, List<List<int>>> convertMesh = Helpers.GH.RhinoConversions.ConvertMeshToElem3d(mesh, 0);
      this._elements = convertMesh.Item1;
      this._topo = convertMesh.Item2;
      this._topoInt = convertMesh.Item3;
      this._faceInt = convertMesh.Item4;

      this._ids = new List<int>(new int[this._mesh.Faces.Count()]);

      this._props = new List<GsaProp3d>();
      for (int i = 0; i < this._mesh.Faces.Count(); i++)
      {
        this._props.Add(new GsaProp3d());
      }
      UpdatePreview();
    }

    internal GsaElement3d(List<Element> elements, List<int> ids, Mesh mesh, List<GsaProp3d> prop3ds)
    {
      this._elements = elements;
      this._mesh = mesh;
      Tuple<List<Element>, List<Point3d>, List<List<int>>, List<List<int>>> convertMesh = Helpers.GH.RhinoConversions.ConvertMeshToElem3d(mesh, 0);
      this._topo = convertMesh.Item2;
      this._topoInt = convertMesh.Item3;
      this._faceInt = convertMesh.Item4;

      this._ids = ids;

      this._props = prop3ds;
      UpdatePreview();
    }
    #endregion

    #region methods
    public GsaElement3d Duplicate(bool cloneApiElements = false)
    {
      GsaElement3d dup = new GsaElement3d();
      dup._mesh = (Mesh)this._mesh.DuplicateShallow();
      dup._guid = new Guid(_guid.ToString());
      dup._topo = this._topo;
      dup._topoInt = this._topoInt;
      dup._faceInt = this._faceInt;
      dup._elements = this._elements;
      if (cloneApiElements)
        dup.CloneApiElements();
      dup._ids = this._ids.ToList();
      dup._props = this._props.ConvertAll(x => x.Duplicate());
      dup.UpdatePreview();
      return dup;
    }

    public override string ToString()
    {
      if (!this._mesh.IsValid)
        return "Null";
      string type = Mappings.ElementTypeMapping.FirstOrDefault(x => x.Value == this.Types.First()).Key + " ";
      string info = "N:" + this.NgonMesh.Vertices.Count + " E:" + this.API_Elements.Count;
      return string.Join(" ", type.Trim(), info.Trim()).Trim().Replace("  ", " ");
    }

    /// <summary>
    /// This method will return a copy of the existing element3d with an updated mesh
    /// </summary>
    /// <param name="updatedthis._mesh"></param>
    /// <returns></returns>
    public GsaElement3d UpdateGeometry(Mesh updatedMesh)
    {
      //if (mthis._mesh.Faces.Count != mthis._elements.Count) { return null; } // the logic below assumes the number of elements is equal to number of faces

      GsaElement3d dup = this.Duplicate(true);
      this._mesh = updatedMesh;
      Tuple<List<Element>, List<Point3d>, List<List<int>>, List<List<int>>> convertMesh = Helpers.GH.RhinoConversions.ConvertMeshToElem3d(this._mesh, 0);
      this._elements = convertMesh.Item1;
      this._topo = convertMesh.Item2;
      this._topoInt = convertMesh.Item3;
      this._faceInt = convertMesh.Item4;
      return dup;
    }

    internal void CloneApiElements()
    {
      this.CloneApiElements(ApiObjectMember.all);
    }

    private void CloneApiElements(ApiObjectMember memType, List<int> grp = null, List<bool> dum = null, List<string> nm = null, List<double> oriA = null, List<GsaOffset> off = null, List<int> prop = null, List<ElementType> typ = null, List<Color> col = null)
    {
      List<Element> elems = new List<Element>();
      for (int i = 0; i < this._elements.Count; i++)
      {
        elems.Add(new Element()
        {
          Group = this._elements[i].Group,
          IsDummy = this._elements[i].IsDummy,
          Name = this._elements[i].Name.ToString(),
          OrientationNode = this._elements[i].OrientationNode,
          OrientationAngle = this._elements[i].OrientationAngle,
          Offset = this._elements[i].Offset,
          ParentMember = this._elements[i].ParentMember,
          Property = this._elements[i].Property,
          Topology = new ReadOnlyCollection<int>(this._elements[i].Topology.ToList()),
          Type = this._elements[i].Type //GsaToModel.Element2dType((int)Elements[i].Type)
        });

        if (memType == ApiObjectMember.all)
          continue;

        switch (memType)
        {
          case ApiObjectMember.group:
            if (grp.Count > i)
              elems[i].Group = grp[i];
            else
              elems[i].Group = grp.Last();
            break;
          case ApiObjectMember.dummy:
            if (dum.Count > i)
              elems[i].IsDummy = dum[i];
            else
              elems[i].IsDummy = dum.Last();
            break;
          case ApiObjectMember.name:
            if (nm.Count > i)
              elems[i].Name = nm[i];
            else
              elems[i].Name = nm.Last();
            break;
          case ApiObjectMember.orientationAngle:
            if (oriA.Count > i)
              elems[i].OrientationAngle = oriA[i];
            else
              elems[i].OrientationAngle = oriA.Last();
            break;
          case ApiObjectMember.offset:
            if (off.Count > i)
            {
              elems[i].Offset.X1 = off[i].X1.Meters;
              elems[i].Offset.X2 = off[i].X2.Meters;
              elems[i].Offset.Y = off[i].Y.Meters;
              elems[i].Offset.Z = off[i].Z.Meters;
            }
            else
            {
              elems[i].Offset.X1 = off.Last().X1.Meters;
              elems[i].Offset.X2 = off.Last().X2.Meters;
              elems[i].Offset.Y = off.Last().Y.Meters;
              elems[i].Offset.Z = off.Last().Z.Meters;
            }
            break;
          case ApiObjectMember.property:
            if (prop.Count > i)
              elems[i].Property = prop[i];
            else
              elems[i].Property = prop.Last();
            break;
          case ApiObjectMember.type:
            if (typ.Count > i)
              elems[i].Type = typ[i];
            else
              elems[i].Type = typ.Last();
            break;
          case ApiObjectMember.colour:
            if (col.Count > i)
              elems[i].Colour = col[i];
            else
              elems[i].Colour = col.Last();

            this._mesh.VertexColors.SetColor(i, (Color)elems[i].Colour);
            break;
        }
      }
      this._elements = elems;
    }

    internal void UpdatePreview()
    {
      this._displayMesh = new Mesh();
      Mesh x = NgonMesh;

      this._displayMesh.Vertices.AddVertices(x.Vertices.ToList());
      List<MeshNgon> ngons = x.GetNgonAndFacesEnumerable().ToList();

      for (int i = 0; i < ngons.Count; i++)
      {
        List<int> faceindex = ngons[i].FaceIndexList().Select(u => (int)u).ToList();
        for (int j = 0; j < faceindex.Count; j++)
        {
          this._displayMesh.Faces.AddFace(x.Faces[faceindex[j]]);
        }
      }
      this._displayMesh.RebuildNormals();
    }
    #endregion
  }
}
