using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GsaAPI;
using OasysUnits;
using OasysUnits.Units;
using Rhino.Geometry;

namespace GsaGH.Parameters
{
  public enum EntityType
  {
    Undefined,
    Node,
    Element,
    Member,
    Case
  }
  /// <summary>
  /// EntityList class, this class defines the basic properties and methods for any Gsa List
  /// </summary>
  public class GsaList
  {
    #region properties
    internal Collection<Point3d> _nodeLocations;
    internal Collection<GsaElement1d> _element1ds;
    internal Collection<GsaElement2d> _element2ds;
    internal Collection<GsaElement3d> _element3ds;
    internal Collection<GsaMember1d> _member1ds;
    internal Collection<GsaMember2d> _member2ds;
    internal Collection<GsaMember3d> _member3ds;
    public string Name { get; set; }
    public string Definition { get; set; }
    public EntityType EntityType { get; set; } = EntityType.Undefined;
    #endregion

    #region constructors
    public GsaList()
    {
    }
    #endregion

    #region methods
    public GsaList Duplicate()
    {
      GsaList dup = new GsaList();
      dup.Name = this.Name;
      dup.Definition = this.Definition;
      dup.EntityType = this.EntityType;

      switch (dup.EntityType)
      {
        case EntityType.Node:
          dup._nodeLocations = new Collection<Point3d>(this._nodeLocations);
          break;
        case EntityType.Element:
          dup._element1ds = new Collection<GsaElement1d>(this._element1ds);
          dup._element2ds = new Collection<GsaElement1d>(this._element2ds);
          dup._element3ds = new Collection<GsaElement1d>(this._element3ds);
      }

      return dup;
    }

    public override string ToString()
    {
      string s = "New GsaGH Model";
      if (this.Model != null && this.Titles != null)
      {
        if (this.FileNameAndPath != null && this.FileNameAndPath != "")
          s = Path.GetFileName(this.FileNameAndPath).Replace(".gwb", string.Empty);
        
        if (Titles != null && Titles.Title != null && Titles.Title != "")
        {
          if (s == "" || s == "Invalid")
            s = Titles.Title;
          else
            s += " {" + Titles.Title + "}";
        }
      }
      if (this.ModelUnit != LengthUnit.Undefined)
        s += " [" + Length.GetAbbreviation(this.ModelUnit) + "]";
      return s;
    }

    private BoundingBox GetBoundingBox()
    {
      ConcurrentDictionary<int, Node> outNodes = new ConcurrentDictionary<int, Node>(this.Model.Nodes());
      ConcurrentBag<Point3d> pts = new ConcurrentBag<Point3d>();
      Parallel.ForEach(outNodes, node =>
      {
        pts.Add(Helpers.Import.Nodes.Point3dFromNode(node.Value, LengthUnit.Meter));
      });

      if (this.ModelUnit != LengthUnit.Undefined && this.ModelUnit != LengthUnit.Meter)
      {
        double factor = 1 / new Length(1, this.ModelUnit).Meters;
        Transform scale = Transform.Scale(new Point3d(0,0,0), factor);
        return new BoundingBox(pts, scale);
      }

      return new BoundingBox(pts);
    }
    #endregion
  }
}
