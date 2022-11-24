using System;
using System.Drawing;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Grasshopper;
using GsaAPI;
using OasysUnits;
using OasysUnits.Units;
using Rhino.Geometry;
using Grasshopper.Kernel.Types;
using GsaGH.Helpers.GsaAPI;

namespace GsaGH.Parameters
{
    /// <summary>
    /// Element2d class, this class defines the basic properties and methods for any Gsa Element 2d
    /// </summary>
    public class GsaElement2d
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
    private List<GsaProp2d> _props = new List<GsaProp2d>();
    private List<List<int>> _topoInt; // list of topology integers referring to the topo list of points
    private List<Point3d> _topo; // list of topology points for visualisation
    private List<int> _ids = new List<int>();
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
    public Mesh Mesh
    {
      get
      {
        return this._mesh;
      }
    }
    public List<Point3d> Topology
    {
      get
      {
        return this._topo;
      }
    }
    public List<int> Ids
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
    }
    public List<GsaProp2d> Properties
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

          Mesh.VertexColors.SetColor(i, (System.Drawing.Color)this._elements[i].Colour);
        }
        return cols;
      }
      set
      {
        this.CloneApiElements(ApiObjectMember.colour, null, null, null, null, null, null, null, value);
        //for (int i = 0; i < mthis._elements.Count; i++)
        //{
        //    if (value[i] != null)
        //    {
        //        mthis._elements[i].Colour = value[i];
        //        Mesh.VertexColors.SetColor(i, (System.Drawing.Color)mthis._elements[i].Colour);
        //    }
        //}
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
        this.CloneApiElements(ApiObjectMember.name, null, null, value);
      }
    }
    public List<Angle> OrientationAngles
    {
      get
      {
        List<Angle> angles = new List<Angle>();
        for (int i = 0; i < this._elements.Count; i++)
        {
          if (this._elements[i] != null)
            angles.Add(new Angle(this._elements[i].OrientationAngle, AngleUnit.Degree).ToUnit(AngleUnit.Radian));
        }
        return angles;
      }
      set
      {
        this.CloneApiElements(ApiObjectMember.orientationAngle, null, null, null, value);
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
        this.CloneApiElements(ApiObjectMember.offset, null, null, null, null, value);
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
        this.CloneApiElements(ApiObjectMember.type, null, null, null, null, null, null, value);
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
            topos.AddRange(this._elements[i].Topology.ToList(), new Grasshopper.Kernel.Data.GH_Path(this.Ids[i]));
        }
        return topos;
      }
    }
    #endregion
    #endregion

    #region constructors
    public GsaElement2d()
    {
    }

    public GsaElement2d(Mesh mesh, int prop = 0)
    {
      this._mesh = mesh;
      Tuple<List<Element>, List<Point3d>, List<List<int>>> convertMesh = Helpers.GH.RhinoConversions.ConvertMeshToElem2d(this._mesh, prop);
      this._elements = convertMesh.Item1;
      this._topo = convertMesh.Item2;
      this._topoInt = convertMesh.Item3;

      this._ids = new List<int>(new int[this._mesh.Faces.Count()]);
      
      GsaProp2d singleProp = new GsaProp2d();
      for (int i = 0; i < this._mesh.Faces.Count(); i++)
        this._props.Add(singleProp);
    }

    internal GsaElement2d(List<Element> elements, List<int> Ids, Mesh mesh, List<GsaProp2d> prop2ds)
    {
      this._mesh = mesh;
      this._topo = new List<Point3d>(mesh.Vertices.ToPoint3dArray());
      this._topoInt = Helpers.GH.RhinoConversions.ConvertMeshToElem2d(this._mesh);
      this._elements = elements;
      this._ids = Ids;
      this._props = prop2ds;
    }

    public GsaElement2d(Brep brep, List<Curve> curves, List<Point3d> points, double meshSize, List<GsaMember1d> mem1ds, List<GsaNode> nodes, LengthUnit unit = LengthUnit.Meter, int prop = 0)
    {
      this._mesh = Helpers.GH.RhinoConversions.ConvertBrepToMesh(brep, curves, points, meshSize, unit, mem1ds, nodes);
      Tuple<List<Element>, List<Point3d>, List<List<int>>> convertMesh = Helpers.GH.RhinoConversions.ConvertMeshToElem2d(this._mesh, prop, true);
      this._elements = convertMesh.Item1;
      this._topo = convertMesh.Item2;
      this._topoInt = convertMesh.Item3;

      this._ids = new List<int>(new int[this._mesh.Faces.Count()]);
    }
    #endregion

    #region methods
    public GsaElement2d Duplicate(bool cloneApiElements = false)
    {
      GsaElement2d dup = new GsaElement2d();
      dup._elements = this._elements;
      if (cloneApiElements)
        dup.CloneApiElements();
      dup._ids = this._ids.ToList();
      dup._mesh = (Mesh)this._mesh.DuplicateShallow();
      dup._props = this._props.ConvertAll(x => x.Duplicate());
      dup._topo = this._topo;
      dup._topoInt = this._topoInt;
      return dup;
    }

    public GsaElement2d UpdateGeometry(Mesh newMesh)
    {
      if (this._mesh.Faces.Count != this._elements.Count)
        return null; // the logic below assumes the number of elements is equal to number of faces

      GsaElement2d dup = this.Duplicate(true);
      this._mesh = newMesh;
      Tuple<List<Element>, List<Point3d>, List<List<int>>> convertMesh = Helpers.GH.RhinoConversions.ConvertMeshToElem2d(this._mesh, 0);
      this._elements = convertMesh.Item1;
      this._topo = convertMesh.Item2;
      this._topoInt = convertMesh.Item3;
      return dup;
    }

    public override string ToString()
    {
      if (!this._mesh.IsValid)
        return "Null";
      string type = Mappings.ElementTypeMapping.FirstOrDefault(x => x.Value == this.Types.First()).Key + " ";
      string info = "N:" + this.Mesh.Vertices.Count + " E:" + this.API_Elements.Count;
      return string.Join(" ", type.Trim(), info.Trim()).Trim().Replace("  ", " ");
    }

    private void CloneApiElements(ApiObjectMember memType, List<int> grp = null, List<bool> dum = null, List<string> nm = null, List<Angle> oriA = null, List<GsaOffset> off = null, List<int> prop = null, List<ElementType> typ = null, List<System.Drawing.Color> col = null)
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

        //if ((System.Drawing.Color)mthis._elements[i].Colour != System.Drawing.Color.FromArgb(0, 0, 0)) // workaround to handle that System.Drawing.Color is non-nullable type
        //    elems[i].Colour = mthis._elements[i].Colour;

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
              elems[i].OrientationAngle = oriA[i].Degrees;
            else
              elems[i].OrientationAngle = oriA.Last().Degrees;
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

            this._mesh.VertexColors.SetColor(i, (System.Drawing.Color)elems[i].Colour);
            break;
        }
      }
      this._elements = elems;
    }
    
    internal void CloneApiElements()
    {
      this.CloneApiElements(ApiObjectMember.all);
    }
    #endregion
  }
}
