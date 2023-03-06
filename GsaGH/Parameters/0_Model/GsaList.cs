using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using GsaAPI;

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
    public string Name { get; set; }
    public int Id { get; set; }
    public string Definition { get; private set; }
    public EntityType EntityType { get; set; } = EntityType.Undefined;

    private ConcurrentBag<GsaNodeGoo> _nodes;
    private Tuple<ConcurrentBag<GsaElement1dGoo>, ConcurrentBag<GsaElement2dGoo>, ConcurrentBag<GsaElement3dGoo>> _elements;
    private Tuple<ConcurrentBag<GsaMember1dGoo>, ConcurrentBag<GsaMember2dGoo>, ConcurrentBag<GsaMember3dGoo>> _members;
    private List<int> _cases;
    #endregion

    #region constructors
    public GsaList() { }
    internal GsaList(int Id, EntityList list, ConcurrentBag<GsaNodeGoo> nodes)
    {
      this.EntityType = EntityType.Node;
      this.Id = Id;
      this.Name = list.Name;
      this.Definition = list.Definition;
      this._nodes = nodes;
    }
    internal GsaList(int Id, EntityList list, Tuple<ConcurrentBag<GsaElement1dGoo>, ConcurrentBag<GsaElement2dGoo>, ConcurrentBag<GsaElement3dGoo>> elements)
    {
      this.EntityType = EntityType.Element;
      this.Id = Id;
      this.Name = list.Name;
      this.Definition = list.Definition;
      this._elements = elements;
    }
    internal GsaList(int Id, EntityList list, Tuple<ConcurrentBag<GsaMember1dGoo>, ConcurrentBag<GsaMember2dGoo>, ConcurrentBag<GsaMember3dGoo>> members)
    {
      this.EntityType = EntityType.Member;
      this.Id = Id;
      this.Name = list.Name;
      this.Definition = list.Definition;
      this._members = members;
    }
    internal GsaList(int Id, EntityList list, List<int> cases)
    {
      this.EntityType = EntityType.Case;
      this.Id = Id;
      this.Name = list.Name;
      this.Definition = list.Definition;
      this._cases = cases;
    }
    internal GsaList(int Id, EntityList list)
    {
      this.EntityType = EntityType.Undefined;
      this.Id = Id;
      this.Name = list.Name;
      this.Definition = list.Definition;
    }
    #endregion

    #region methods
    public GsaList Duplicate()
    {
      GsaList dup = new GsaList
      {
        Id = this.Id,
        Name = this.Name,
        Definition = this.Definition,
        EntityType = this.EntityType,
      };
      switch (dup.EntityType)
      {
        case EntityType.Node:
          dup._nodes = new ConcurrentBag<GsaNodeGoo>(this._nodes);
          break;
        case EntityType.Element:
          dup._elements = new Tuple<ConcurrentBag<GsaElement1dGoo>, ConcurrentBag<GsaElement2dGoo>, ConcurrentBag<GsaElement3dGoo>>(this._elements.Item1, this._elements.Item2, this._elements.Item3);
          break;
        case EntityType.Member:
          dup._members = new Tuple<ConcurrentBag<GsaMember1dGoo>, ConcurrentBag<GsaMember2dGoo>, ConcurrentBag<GsaMember3dGoo>>(this._members.Item1, this._members.Item2, this._members.Item3);
          break;
        case EntityType.Case:
          dup._cases = new List<int>(this._cases);
          break;
      }
      return dup;
    }

    internal List<object> GetListObjects()
    {
      List<object> list = null;
      switch (this.EntityType)
      {
        case EntityType.Node:
          list = new List<object>(this._nodes.OrderBy(x => x.Value.Id));
          break;
        case EntityType.Element:
          list = new List<object>();
          if (this._elements.Item1 != null)
            list.AddRange(this._elements.Item1.OrderBy(x => x.Value.Id));
          if (this._elements.Item2 != null)
            list.AddRange(this._elements.Item2.OrderBy(x => x.Value.Ids.Min()));
          if (this._elements.Item3 != null)
            list.AddRange(this._elements.Item3.OrderBy(x => x.Value.Ids.Min()));
          break;
        case EntityType.Member:
          list = new List<object>();
          if (this._members.Item1 != null)
            list.AddRange(this._members.Item1.OrderBy(x => x.Value.Id));
          if (this._members.Item2 != null)
            list.AddRange(this._members.Item2.OrderBy(x => x.Value.Id));
          if (this._members.Item3 != null)
            list.AddRange(this._members.Item3.OrderBy(x => x.Value.Id));
          break;
        case EntityType.Case:
          list = new List<object>() { this._cases };
          break;
        case EntityType.Undefined:
          if (this.Definition != null && this.Definition != "")
            list = new List<object>(new List<string>() { this.Definition });
          break;
      }
      return list;
    }

    public override string ToString()
    {
      string s = this.Id + ": " + this.Name;
      if (this.EntityType != EntityType.Undefined)
        s += " containing ";
      switch (this.EntityType)
      {
        case EntityType.Node:
          s += this._nodes.Count;
          break;
        case EntityType.Element:
          s += (this._elements.Item1.Count + this._elements.Item2.Count + this._elements.Item3.Count);
          break;
        case EntityType.Member:
          s += (this._members.Item1.Count + this._members.Item2.Count + this._members.Item3.Count);
          break;
        case EntityType.Case:
          s += this._cases.Count;
          break;
      }
      if (this.EntityType != EntityType.Undefined)
        s += " " + this.EntityType.ToString() + "s";
      else if (this.Definition != null)
        s += " (" + this.Definition + ")";
      return s;
    }

    internal static EntityType GetEntityFromAPI(GsaAPI.EntityType type)
    {
      switch (type)
      {
        case GsaAPI.EntityType.Node:
          return EntityType.Node;
        case GsaAPI.EntityType.Element:
          return EntityType.Element;
        case GsaAPI.EntityType.Member:
          return EntityType.Member;
        case GsaAPI.EntityType.Case:
          return EntityType.Case;
      }
      return EntityType.Undefined;
    }

    internal static GsaAPI.EntityType GetAPIEntityType(EntityType type)
    {
      switch (type)
      {
        case EntityType.Node:
          return GsaAPI.EntityType.Node;
        case EntityType.Element:
          return GsaAPI.EntityType.Element;
        case EntityType.Member:
          return GsaAPI.EntityType.Member;
        case EntityType.Case:
          return GsaAPI.EntityType.Case;
      }
      return GsaAPI.EntityType.Undefined;
    }
    #endregion
  }
}
