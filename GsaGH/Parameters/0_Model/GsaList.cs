using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GsaAPI;
using GsaGH.Helpers.Import;
using OasysUnits.Units;

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

    private GsaModel _model;
    private ConcurrentBag<GsaNodeGoo> _nodes;
    private Tuple<ConcurrentBag<GsaElement1dGoo>, ConcurrentBag<GsaElement2dGoo>, ConcurrentBag<GsaElement3dGoo>> _elements;
    private Tuple<ConcurrentBag<GsaMember1dGoo>, ConcurrentBag<GsaMember2dGoo>, ConcurrentBag<GsaMember3dGoo>> _members;
    private List<int> _cases;
    #endregion

    #region constructors
    public GsaList() { }
    internal GsaList(int Id, EntityList list, GsaModel model)
    {
      this.EntityType = GetEntityFromAPI(list.Type);
      this.Id = Id;
      this.Name = list.Name;
      this.Definition = list.Definition;
      this._model = model;
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
        _model = this._model
      };

      switch (dup.EntityType)
      {
        case EntityType.Node:
          if (this._nodes != null)
            dup._nodes = new ConcurrentBag<GsaNodeGoo>(this._nodes);
          break;
        case EntityType.Element:
          if (this._elements != null)
            dup._elements = new Tuple<ConcurrentBag<GsaElement1dGoo>, ConcurrentBag<GsaElement2dGoo>, ConcurrentBag<GsaElement3dGoo>>(this._elements.Item1, this._elements.Item2, this._elements.Item3);
          break;
        case EntityType.Member:
          if (this._members != null)
            dup._members = new Tuple<ConcurrentBag<GsaMember1dGoo>, ConcurrentBag<GsaMember2dGoo>, ConcurrentBag<GsaMember3dGoo>>(this._members.Item1, this._members.Item2, this._members.Item3);
          break;
        case EntityType.Case:
          if (this._cases != null)
            dup._cases = new List<int>(this._cases);
          break;
      }
      return dup;
    }

    internal void SetListObjects(List<object> objects)
    {
      switch (this.EntityType)
      {
        case EntityType.Node:
          this._nodes = new ConcurrentBag<GsaNodeGoo>(objects.Select(x => new GsaNodeGoo((GsaNode)x)));
          break;

        case EntityType.Element:
          ConcurrentBag<GsaElement1dGoo> elem1ds = new ConcurrentBag<GsaElement1dGoo>();
          ConcurrentBag<GsaElement2dGoo> elem2ds = new ConcurrentBag<GsaElement2dGoo>();
          ConcurrentBag<GsaElement3dGoo> elem3ds = new ConcurrentBag<GsaElement3dGoo>();
          foreach (object elem in objects)
          {
            if (elem is GsaElement1d elem1d)
              elem1ds.Add(new GsaElement1dGoo(elem1d));
            else if (elem is GsaElement2d elem2d)
              elem2ds.Add(new GsaElement2dGoo(elem2d));
            else if (elem is GsaElement3d elem3d)
              elem3ds.Add(new GsaElement3dGoo(elem3d));
          }
          this._elements = new Tuple<ConcurrentBag<GsaElement1dGoo>, ConcurrentBag<GsaElement2dGoo>, ConcurrentBag<GsaElement3dGoo>>(elem1ds, elem2ds, elem3ds);
          break;

        case EntityType.Member:
          ConcurrentBag<GsaMember1dGoo> mem1ds = new ConcurrentBag<GsaMember1dGoo>();
          ConcurrentBag<GsaMember2dGoo> mem2ds = new ConcurrentBag<GsaMember2dGoo>();
          ConcurrentBag<GsaMember3dGoo> mem3ds = new ConcurrentBag<GsaMember3dGoo>();
          foreach (object mem in objects)
          {
            if (mem is GsaMember1d mem1d)
              mem1ds.Add(new GsaMember1dGoo(mem1d));
            else if (mem is GsaMember2d mem2d)
              mem2ds.Add(new GsaMember2dGoo(mem2d));
            else if (mem is GsaMember3d mem3d)
              mem3ds.Add(new GsaMember3dGoo(mem3d));
          }
          this._members = new Tuple<ConcurrentBag<GsaMember1dGoo>, ConcurrentBag<GsaMember2dGoo>, ConcurrentBag<GsaMember3dGoo>>(mem1ds, mem2ds, mem3ds);
          break;

        case EntityType.Case:
          this._cases = objects.Select(x => (int)x).ToList();
          break;
      }
    }

    internal List<object> GetListObjects(LengthUnit unit)
    {
      if (this._model != null)
        this.PopulateListObjectsFromModel(unit);
      List<object> list = null;
      switch (this.EntityType)
      {
        case EntityType.Node:
          if (this._nodes == null)
            return new List<object>();
          list = new List<object>(this._nodes.OrderBy(x => x.Value.Id));
          break;
        case EntityType.Element:
          if (this._elements == null)
            return new List<object>();
          list = new List<object>();
          if (this._elements.Item1 != null)
            list.AddRange(this._elements.Item1.OrderBy(x => x.Value.Id));
          if (this._elements.Item2 != null)
            list.AddRange(this._elements.Item2.OrderBy(x => x.Value.Ids.Min()));
          if (this._elements.Item3 != null)
            list.AddRange(this._elements.Item3.OrderBy(x => x.Value.Ids.Min()));
          break;
        case EntityType.Member:
          if (this._members == null)
            return new List<object>();
          list = new List<object>();
          if (this._members.Item1 != null)
            list.AddRange(this._members.Item1.OrderBy(x => x.Value.Id));
          if (this._members.Item2 != null)
            list.AddRange(this._members.Item2.OrderBy(x => x.Value.Id));
          if (this._members.Item3 != null)
            list.AddRange(this._members.Item3.OrderBy(x => x.Value.Id));
          break;
        case EntityType.Case:
          if (this._cases == null)
            return new List<object>();
          list = new List<object>() { this._cases };
          break;
        case EntityType.Undefined:
          if (this.Definition != null && this.Definition != "")
            list = new List<object>(new List<string>() { this.Definition });
          break;
      }
      return list;
    }

    private void PopulateListObjectsFromModel(LengthUnit unit)
    {
      if (this._model == null)
        return;
      switch (this.EntityType)
      {
        case EntityType.Node:
          this._nodes = Nodes.GetNodes(this._model.Model.Nodes(this.Definition), unit, this._model.Model.Axes());
          break;

        case EntityType.Element:
          Dictionary<int, ReadOnlyCollection<double>> elementLocalAxesDict = new Dictionary<int, ReadOnlyCollection<double>>();
          foreach (int id in this._model.Model.Elements(this.Definition).Keys)
            elementLocalAxesDict.Add(id, this._model.Model.ElementDirectionCosine(id));

          this._elements = Elements.GetElements(
          this._model.Model.Elements(this.Definition), this._model.Model.Nodes(),
          this._model.Model.Sections(), this._model.Model.Prop2Ds(), this._model.Model.Prop3Ds(),
          this._model.Model.AnalysisMaterials(), this._model.Model.SectionModifiers(),
          elementLocalAxesDict, this._model.Model.Axes(), unit, false);
          break;

        case EntityType.Member:
          Dictionary<int, ReadOnlyCollection<double>> memberLocalAxesDict = new Dictionary<int, ReadOnlyCollection<double>>();
          foreach (int id in this._model.Model.Members(this.Definition).Keys)
            memberLocalAxesDict.Add(id, this._model.Model.MemberDirectionCosine(id));
          this._members = Members.GetMembers(
          this._model.Model.Members(this.Definition), this._model.Model.Nodes(),
          this._model.Model.Sections(), this._model.Model.Prop2Ds(), this._model.Model.Prop3Ds(),
          this._model.Model.AnalysisMaterials(), this._model.Model.SectionModifiers(),
          memberLocalAxesDict, this._model.Model.Axes(), unit, false);
          break;

        case EntityType.Case:
          GsaAPI.EntityList tempApiList = new GsaAPI.EntityList()
          { Type = GsaAPI.EntityType.Case, Name = this.Name, Definition = this.Definition };
          this._cases = this._model.Model.ExpandList(tempApiList).ToList();
          break;

        case EntityType.Undefined:
        default:
          break;
      }
    }

    public override string ToString()
    {
      string s = "ID:" + this.Id + " " + this.Name + " ";
      switch (this.EntityType)
      {
        case EntityType.Node:
          if (this._nodes != null)
            s += "containing " + this._nodes.Count + " " + this.EntityType.ToString() + "s";
          else
            s += this.EntityType.ToString() + "s (" + this.Definition + ")";
          break;
        case EntityType.Element:
          if (this._elements != null)
            s += "containing " + (this._elements.Item1.Count + this._elements.Item2.Count + this._elements.Item3.Count) + " " + this.EntityType.ToString() + "s";
          else
            s += this.EntityType.ToString() + "s (" + this.Definition + ")";
          break;
        case EntityType.Member:
          if (this._members != null)
            s += "containing " + (this._members.Item1.Count + this._members.Item2.Count + this._members.Item3.Count) + " " + this.EntityType.ToString() + "s";
          else
            s += this.EntityType.ToString() + "s (" + this.Definition + ")";
          break;
        case EntityType.Case:
        case EntityType.Undefined:
          s += this.EntityType.ToString() + " (" + this.Definition + ")";
          break;
      }
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
