using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaAPI;
using OasysUnits;
using OasysUnits.Units;
using Rhino.Geometry;

namespace GsaGH.Parameters
{
  /// <summary>
  /// Element2d class, this class defines the basic properties and methods for any Gsa Element 2d
  /// </summary>
  public class GsaElement2d
  {
    internal List<Element> API_Elements
    {
      get { return m_elements; }
      set { m_elements = value; }
    }
    internal Element GetAPI_ElementClone(int i)
    {
      return new Element()
      {
        Group = m_elements[i].Group,
        IsDummy = m_elements[i].IsDummy,
        Name = m_elements[i].Name.ToString(),
        OrientationNode = m_elements[i].OrientationNode,
        OrientationAngle = m_elements[i].OrientationAngle,
        Offset = m_elements[i].Offset,
        ParentMember = m_elements[i].ParentMember,
        Property = m_elements[i].Property,
        Topology = new ReadOnlyCollection<int>(m_elements[i].Topology.ToList()),
        Type = m_elements[i].Type //GsaToModel.Element2dType((int)Elements[i].Type)
      };
    }
    public int Count
    {
      get { return m_elements.Count; }
    }
    public Mesh Mesh
    {
      get { return m_mesh; }
    }
    public List<Point3d> Topology
    {
      get { return m_topo; }
    }
    public List<int> ID
    {
      get { return m_id; }
      set { m_id = value; }
    }
    public List<List<int>> TopoInt
    {
      get { return m_topoInt; }
    }
    public List<GsaProp2d> Properties
    {
      get { return m_props; }
      set { m_props = value; }
    }
    public List<int> ParentMembers
    {
      get
      {
        List<int> pMems = new List<int>();
        for (int i = 0; i < m_elements.Count; i++)
          try { pMems.Add(m_elements[i].ParentMember.Member); } catch (Exception) { pMems.Add(0); }
        return pMems;
      }
    }
    #region GsaAPI.Element members
    public List<System.Drawing.Color> Colours
    {
      get
      {
        List<System.Drawing.Color> cols = new List<System.Drawing.Color>();
        for (int i = 0; i < m_elements.Count; i++)
        {
          if ((System.Drawing.Color)m_elements[i].Colour == System.Drawing.Color.FromArgb(0, 0, 0))
          {
            m_elements[i].Colour = System.Drawing.Color.FromArgb(50, 150, 150, 150);
          }
          cols.Add((System.Drawing.Color)m_elements[i].Colour);

          Mesh.VertexColors.SetColor(i, (System.Drawing.Color)m_elements[i].Colour);
        }
        return cols;
      }
      set
      {
        CloneApiElements(apiObjectMember.colour, null, null, null, null, null, null, null, value);
        //for (int i = 0; i < m_elements.Count; i++)
        //{
        //    if (value[i] != null)
        //    {
        //        m_elements[i].Colour = value[i];
        //        Mesh.VertexColors.SetColor(i, (System.Drawing.Color)m_elements[i].Colour);
        //    }
        //}
      }
    }
    public List<int> Groups
    {
      get
      {
        List<int> groups = new List<int>();
        for (int i = 0; i < m_elements.Count; i++)
        {
          if (m_elements[i] != null)
            groups.Add(m_elements[i].Group);
        }
        return groups;
      }
      set
      {
        CloneApiElements(apiObjectMember.group, value);
      }
    }
    public List<bool> isDummies
    {
      get
      {
        List<bool> dums = new List<bool>();
        for (int i = 0; i < m_elements.Count; i++)
        {
          if (m_elements[i] != null)
            dums.Add(m_elements[i].IsDummy);
        }
        return dums;
      }
      set
      {
        CloneApiElements(apiObjectMember.dummy, null, value);
      }
    }
    public List<string> Names
    {
      get
      {
        List<string> names = new List<string>();
        for (int i = 0; i < m_elements.Count; i++)
        {
          if (m_elements[i] != null)
            names.Add(m_elements[i].Name);
        }
        return names;
      }
      set
      {
        CloneApiElements(apiObjectMember.name, null, null, value);
      }
    }
    public List<double> OrientationAngles
    {
      get
      {
        List<double> angles = new List<double>();
        for (int i = 0; i < m_elements.Count; i++)
        {
          if (m_elements[i] != null)
            angles.Add(m_elements[i].OrientationAngle);
        }
        return angles;
      }
      set
      {
        CloneApiElements(apiObjectMember.orientationAngle, null, null, null, value);
      }
    }
    public List<GsaOffset> Offsets
    {
      get
      {
        List<GsaOffset> offs = new List<GsaOffset>();
        for (int i = 0; i < m_elements.Count; i++)
        {
          if (m_elements[i] != null)
          {
            GsaOffset off = new GsaOffset(
                m_elements[i].Offset.X1,
                m_elements[i].Offset.X2,
                m_elements[i].Offset.Y,
                m_elements[i].Offset.Z);
            offs.Add(off);
          }
        }
        return offs;
      }
      set
      {
        CloneApiElements(apiObjectMember.offset, null, null, null, null, value);
      }
    }

    public List<ElementType> Types
    {
      get
      {
        List<ElementType> typs = new List<ElementType>();
        for (int i = 0; i < m_elements.Count; i++)
        {
          if (m_elements[i] != null)
            typs.Add(m_elements[i].Type);
        }
        return typs;
      }
      set
      {
        CloneApiElements(apiObjectMember.type, null, null, null, null, null, null, value);
      }
    }

    public DataTree<int> TopologyIDs
    {
      get
      {
        DataTree<int> topos = new DataTree<int>();
        for (int i = 0; i < m_elements.Count; i++)
        {
          if (m_elements[i] != null)
            topos.AddRange(m_elements[i].Topology.ToList(), new Grasshopper.Kernel.Data.GH_Path(i));
        }
        return topos;
      }
    }

    private void CloneApiElements(apiObjectMember memType, List<int> grp = null, List<bool> dum = null, List<string> nm = null,
        List<double> oriA = null, List<GsaOffset> off = null, List<int> prop = null, List<ElementType> typ = null, List<System.Drawing.Color> col = null)
    {
      List<Element> elems = new List<Element>();
      for (int i = 0; i < m_elements.Count; i++)
      {
        elems.Add(new Element()
        {
          Group = m_elements[i].Group,
          IsDummy = m_elements[i].IsDummy,
          Name = m_elements[i].Name.ToString(),
          OrientationNode = m_elements[i].OrientationNode,
          OrientationAngle = m_elements[i].OrientationAngle,
          Offset = m_elements[i].Offset,
          ParentMember = m_elements[i].ParentMember,
          Property = m_elements[i].Property,
          Topology = new ReadOnlyCollection<int>(m_elements[i].Topology.ToList()),
          Type = m_elements[i].Type //GsaToModel.Element2dType((int)Elements[i].Type)
        });

        //if ((System.Drawing.Color)m_elements[i].Colour != System.Drawing.Color.FromArgb(0, 0, 0)) // workaround to handle that System.Drawing.Color is non-nullable type
        //    elems[i].Colour = m_elements[i].Colour;

        if (memType == apiObjectMember.all)
          continue;

        switch (memType)
        {
          case apiObjectMember.group:
            if (grp.Count > i)
              elems[i].Group = grp[i];
            else
              elems[i].Group = grp.Last();
            break;
          case apiObjectMember.dummy:
            if (dum.Count > i)
              elems[i].IsDummy = dum[i];
            else
              elems[i].IsDummy = dum.Last();
            break;
          case apiObjectMember.name:
            if (nm.Count > i)
              elems[i].Name = nm[i];
            else
              elems[i].Name = nm.Last();
            break;
          case apiObjectMember.orientationAngle:
            if (oriA.Count > i)
              elems[i].OrientationAngle = oriA[i];
            else
              elems[i].OrientationAngle = oriA.Last();
            break;
          case apiObjectMember.offset:
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
          case apiObjectMember.property:
            if (prop.Count > i)
              elems[i].Property = prop[i];
            else
              elems[i].Property = prop.Last();
            break;
          case apiObjectMember.type:
            if (typ.Count > i)
              elems[i].Type = typ[i];
            else
              elems[i].Type = typ.Last();
            break;
          case apiObjectMember.colour:
            if (col.Count > i)
              elems[i].Colour = col[i];
            else
              elems[i].Colour = col.Last();

            m_mesh.VertexColors.SetColor(i, (System.Drawing.Color)elems[i].Colour);
            break;
        }
      }
      m_elements = elems;
    }
    internal void CloneApiElements()
    {
      CloneApiElements(apiObjectMember.all);
    }
    private enum apiObjectMember
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
    #endregion

    #region fields
    private List<Element> m_elements;
    private Mesh m_mesh;
    private List<List<int>> m_topoInt; // list of topology integers referring to the topo list of points
    private List<Point3d> m_topo; // list of topology points for visualisation
    private List<int> m_id;
    private List<GsaProp2d> m_props;
    #endregion

    #region constructors
    public GsaElement2d()
    {
      m_elements = new List<Element>();
      m_mesh = new Mesh();
      m_props = new List<GsaProp2d>();
    }

    public GsaElement2d(Mesh mesh, int prop = 0)
    {
      m_elements = new List<Element>();
      m_mesh = mesh;
      Tuple<List<Element>, List<Point3d>, List<List<int>>> convertMesh = Util.GH.Convert.ConvertMeshToElem2d(m_mesh, prop);
      m_elements = convertMesh.Item1;
      m_topo = convertMesh.Item2;
      m_topoInt = convertMesh.Item3;

      m_id = new List<int>(new int[m_mesh.Faces.Count()]);

      m_props = new List<GsaProp2d>();
      for (int i = 0; i < m_mesh.Faces.Count(); i++)
      {
        m_props.Add(new GsaProp2d());
      }
    }

    internal GsaElement2d(List<Element> elements, List<int> IDs, Mesh mesh, List<GsaProp2d> prop2ds)
    {
      m_mesh = mesh;
      m_topo = new List<Point3d>(mesh.Vertices.ToPoint3dArray());
      m_topoInt = Util.GH.Convert.ConvertMeshToElem2d(m_mesh);
      m_elements = elements;
      m_id = IDs;
      m_props = prop2ds;
    }

    public GsaElement2d(Brep brep, List<Curve> curves, List<Point3d> points, Length meshSize, List<GsaMember1d> mem1ds, List<GsaNode> nodes, LengthUnit unit, int prop = 0)
    {
      m_elements = new List<Element>();
      m_mesh = Util.GH.Convert.ConvertBrepToMesh(brep, curves, points, meshSize, unit, mem1ds, nodes);
      Tuple<List<Element>, List<Point3d>, List<List<int>>> convertMesh = Util.GH.Convert.ConvertMeshToElem2d(m_mesh, prop, true);
      m_elements = convertMesh.Item1;
      m_topo = convertMesh.Item2;
      m_topoInt = convertMesh.Item3;

      m_id = new List<int>(new int[m_mesh.Faces.Count()]);

      m_props = new List<GsaProp2d>();
    }

    public GsaElement2d Duplicate(bool cloneApiElements = false)
    {
      if (m_mesh == null)
        return null;
      GsaElement2d dup = new GsaElement2d();
      dup.m_elements = this.m_elements;
      if (cloneApiElements)
        dup.CloneApiElements();
      dup.m_id = this.m_id.ToList();
      dup.m_mesh = (Mesh)this.m_mesh.DuplicateShallow();
      dup.m_props = this.m_props.ConvertAll(x => x.Duplicate());
      dup.m_topo = this.m_topo;
      dup.m_topoInt = this.m_topoInt;
      return dup;
    }

    public GsaElement2d UpdateGeometry(Mesh newMesh)
    {
      if (this.m_mesh == null)
        return null;
      if (this.m_mesh.Faces.Count != this.m_elements.Count)
        return null; // the logic below assumes the number of elements is equal to number of faces

      GsaElement2d dup = this.Duplicate(true);
      m_mesh = newMesh;
      Tuple<List<Element>, List<Point3d>, List<List<int>>> convertMesh = Util.GH.Convert.ConvertMeshToElem2d(this.m_mesh, 0);
      this.m_elements = convertMesh.Item1;
      this.m_topo = convertMesh.Item2;
      this.m_topoInt = convertMesh.Item3;
      return dup;
    }
    #endregion

    #region properties
    public bool IsValid
    {
      get
      {
        if (m_mesh == null)
          return false;
        return true;
      }
    }
    #endregion

    #region methods
    public override string ToString()
    {
      string valid = (this.IsValid) ? "" : "Invalid ";
      return valid + "GSA 2D Element(s)";
    }
    #endregion
  }
}
