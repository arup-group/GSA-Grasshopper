using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using GsaAPI;

using GsaGH.Helpers;
using GsaGH.Helpers.Import;

using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGH.Parameters {
  /// <summary>
  /// <para>An Entity List is expressed as a string of text in a specific syntax along with the List
  /// Type. In Grasshopper, a Entity List can also contain a copy of all the items in the list. </para>
  /// <para>Lists (of nodes, elements, members or cases) are used, for example, when a particular load
  /// is to be applied to one or several elements. To define a series of items the list can either
  /// specify each individually or, if applicable, use a more concise
  /// <see href="https://docs.oasys-software.com/structural/gsa/references/listsandembeddedlists.html">syntax</see>.</para>
  /// </summary>
  public class GsaList {
    public Guid Guid { get; private set; } = Guid.NewGuid();
    public int Id { get; set; } = 0;
    public string Definition { get; set; }
    public EntityType EntityType { get; set; } = EntityType.Undefined;
    public string Name { get; set; }

    internal List<int> _cases;
    internal (List<GsaMaterialGoo> materials, List<GsaSectionGoo> sections,
      List<GsaProperty2dGoo> prop2ds, List<GsaProperty3dGoo> prop3ds) _properties;
    internal ConcurrentBag<GsaNodeGoo> _nodes;
    internal (ConcurrentBag<GsaElement1dGoo> e1d, ConcurrentBag<GsaElement2dGoo> e2d,
      ConcurrentBag<GsaElement3dGoo> e3d) _elements;
    internal (ConcurrentBag<GsaMember1dGoo> m1d, ConcurrentBag<GsaMember2dGoo> m2d,
      ConcurrentBag<GsaMember3dGoo> m3d) _members;
    private GsaModel _model;

    public GsaList() { }

    internal GsaList(string name, string definition, GsaAPI.EntityType type) {
      EntityType = GetEntityType(type);
      Name = string.IsNullOrEmpty(name) ? $"{type} list" : name;
      Definition = definition;
    }

    internal GsaList(int id, EntityList list, GsaModel model) {
      EntityType = GetEntityType(list.Type);
      Id = id;
      Name = list.Name;
      Definition = list.Definition;
      _model = model;
    }

    public GsaList Duplicate() {
      var dup = new GsaList {
        Id = Id,
        Name = Name,
        Definition = Definition,
        EntityType = EntityType,
        _model = _model,
        Guid = new Guid(Guid.ToString()),
      };

      switch (dup.EntityType) {
        case EntityType.Node:
          if (_nodes != null) {
            dup._nodes = new ConcurrentBag<GsaNodeGoo>(_nodes);
          }

          break;

        case EntityType.Element:
          if (_properties != (null, null, null, null)) {
            dup._properties = (new List<GsaMaterialGoo>(_properties.materials.ToList()),
              new List<GsaSectionGoo>(_properties.sections.ToList()),
              new List<GsaProperty2dGoo>(_properties.prop2ds.ToList()),
              new List<GsaProperty3dGoo>(_properties.prop3ds.ToList()));
          }
          if (_elements != (null, null, null)) {
            dup._elements = (new ConcurrentBag<GsaElement1dGoo>(_elements.e1d.ToList()),
              new ConcurrentBag<GsaElement2dGoo>(_elements.e2d.ToList()),
              new ConcurrentBag<GsaElement3dGoo>(_elements.e3d.ToList()));
          }
          if (_members != (null, null, null)) {
            dup._members = (new ConcurrentBag<GsaMember1dGoo>(_members.m1d.ToList()),
              new ConcurrentBag<GsaMember2dGoo>(_members.m2d.ToList()),
              new ConcurrentBag<GsaMember3dGoo>(_members.m3d.ToList()));
          }

          break;

        case EntityType.Member:
          if (_properties != (null, null, null, null)) {
            dup._properties = (new List<GsaMaterialGoo>(_properties.materials.ToList()),
              new List<GsaSectionGoo>(_properties.sections.ToList()),
              new List<GsaProperty2dGoo>(_properties.prop2ds.ToList()),
              new List<GsaProperty3dGoo>(_properties.prop3ds.ToList()));
          }
          if (_members != (null, null, null)) {
            dup._members = (new ConcurrentBag<GsaMember1dGoo>(_members.m1d.ToList()),
              new ConcurrentBag<GsaMember2dGoo>(_members.m2d.ToList()),
              new ConcurrentBag<GsaMember3dGoo>(_members.m3d.ToList()));
          }

          break;

        case EntityType.Case:
          if (_cases != null) {
            dup._cases = new List<int>(_cases);
          }

          break;
      }

      return dup;
    }

    public static string CreateListDefinition(List<int> ids) {
      return ids.ToRanges().StringifyRange();
    }

    public List<int> ExpandListDefinition() {
      if (_model != null) {
        return _model.ApiModel.ExpandList(GetApiList()).ToList();
      }

      var m = new Model();
      EntityList list = GetApiList();
      list.Type = GsaAPI.EntityType.Undefined;
      if (string.IsNullOrEmpty(list.Name)) {
        list.Name = "name";
      }

      return m.ExpandList(list).ToList();
    }

    public override string ToString() {
      string s = Id > 0 ? ("ID:" + Id + " ") : string.Empty;
      if (Name != null) {
        s += Name + " ";
      } else {
        s += EntityType.ToString() + " List ";
      }
      switch (EntityType) {
        case EntityType.Node:
          if (!_nodes.IsNullOrEmpty()) {
            s += "containing " + _nodes.Count + " " + EntityType.ToString() + "s";
          } else {
            s += EntityType.ToString() + "s" + (Definition != null
              ? " (" + Definition.Trim() + ")"
              : string.Empty);
          }

          break;

        case EntityType.Element:
          if (_elements != (null, null, null)
            && (_elements.e1d.Count + _elements.e2d.Count + _elements.e3d.Count) != 0) {
            s += "containing "
              + (_elements.e1d.Count + _elements.e2d.Count + _elements.e3d.Count) + " "
              + EntityType.ToString() + "s";
          } else {
            s += EntityType.ToString() + "s" + (Definition != null
              ? " (" + Definition.Trim() + ")"
              : string.Empty);
          }

          break;

        case EntityType.Member:
          if (_members != (null, null, null)
            && (_members.m1d.Count + _members.m2d.Count + _members.m3d.Count) != 0) {
            s += "containing "
              + (_members.m1d.Count + _members.m2d.Count + _members.m3d.Count) + " "
              + EntityType.ToString() + "s";
          } else {
            s += EntityType.ToString() + "s" + (Definition != null
              ? " (" + Definition.Trim() + ")"
              : string.Empty);
          }

          break;

        case EntityType.Case:
          if (!_cases.IsNullOrEmpty()) {
            s += "containing " + _cases.Count + " " + EntityType.ToString() + "s";
          } else {
            s += EntityType.ToString() + "s" + (Definition != null
              ? " (" + Definition.Trim() + ")"
              : string.Empty);
          }

          break;

        case EntityType.Undefined:
          s += EntityType.ToString() + "s" + (Definition != null
            ? " (" + Definition.Trim() + ")"
            : string.Empty);
          break;
      }

      return s.TrimSpaces();
    }

    internal EntityList GetApiList() {
      return new EntityList {
        Name = Name,
        Definition = Definition,
        Type = GetApiEntityType(EntityType)
      };
    }

    internal static EntityType GetEntityType(GsaAPI.EntityType type) {
      return type switch {
        GsaAPI.EntityType.Node => EntityType.Node,
        GsaAPI.EntityType.Element => EntityType.Element,
        GsaAPI.EntityType.Member => EntityType.Member,
        GsaAPI.EntityType.Case => EntityType.Case,
        _ => EntityType.Undefined,
      };
    }

    internal static GsaAPI.EntityType GetApiEntityType(EntityType type) {
      return type switch {
        EntityType.Node => GsaAPI.EntityType.Node,
        EntityType.Element => GsaAPI.EntityType.Element,
        EntityType.Member => GsaAPI.EntityType.Member,
        EntityType.Case => GsaAPI.EntityType.Case,
        EntityType.Assembly => GsaAPI.EntityType.Assembly,
        _ => GsaAPI.EntityType.Undefined,
      };
    }

    internal List<object> GetListObjects(LengthUnit unit) {
      if (_model != null) {
        PopulateListObjectsFromModel(unit);
      }

      List<object> list = null;
      switch (EntityType) {
        case EntityType.Node:
          if (_nodes == null) {
            return new List<object>();
          }

          list = new List<object>(_nodes.OrderBy(x => x.Value.Id));
          break;

        case EntityType.Element:
          if (_elements == (null, null, null)) {
            return new List<object>();
          }

          list = new List<object>();
          if (_elements.e1d != null) {
            list.AddRange(_elements.e1d.OrderBy(x => x.Value.Id));
          }

          if (_elements.e2d != null) {
            list.AddRange(_elements.e2d.OrderBy(x => x.Value.Ids.Min()));
          }

          if (_elements.e3d != null) {
            list.AddRange(_elements.e3d.OrderBy(x => x.Value.Ids.Min()));
          }

          break;

        case EntityType.Member:
          if (_members == (null, null, null)) {
            return new List<object>();
          }

          list = new List<object>();
          if (_members.m1d != null) {
            list.AddRange(_members.m1d.OrderBy(x => x.Value.Id));
          }

          if (_members.m2d != null) {
            list.AddRange(_members.m2d.OrderBy(x => x.Value.Id));
          }

          if (_members.m3d != null) {
            list.AddRange(_members.m3d.OrderBy(x => x.Value.Id));
          }

          break;

        case EntityType.Case:
          if (_cases == null) {
            return new List<object>();
          }

          list = new List<object>() {
            _cases,
          };
          break;

        case EntityType.Undefined:
          if (!string.IsNullOrEmpty(Definition)) {
            list = new List<object>(new List<string>() {
              Definition,
            });
          }

          break;
      }

      return list;
    }

    internal void SetListGooObjects(List<object> gooObjects) {
      var def = new List<string>();
      for (int i = gooObjects.Count - 1; i >= 0; i--) {
        if (gooObjects[i] is string txt) {
          def.Add(txt);
          gooObjects.RemoveAt(i);
        }
      }

      if (def.Count > 0) {
        def.Reverse();
        string definition = string.Join(" ", def);
        // pass the definition through the API here to catch any errors
        var apiList = new EntityList() {
          Type = GetApiEntityType(EntityType),
          Definition = definition,
          Name = "nm"
        };
        Definition = apiList.Definition;
        var m = new Model();
        m.AddList(apiList);
      }

      switch (EntityType) {
        case EntityType.Node:
          _nodes = new ConcurrentBag<GsaNodeGoo>(gooObjects.Select(x => (GsaNodeGoo)x));
          break;

        case EntityType.Element:
          _properties.materials = new List<GsaMaterialGoo>();
          _properties.sections = new List<GsaSectionGoo>();
          _properties.prop2ds = new List<GsaProperty2dGoo>();
          _properties.prop3ds = new List<GsaProperty3dGoo>();
          _elements.e1d = new ConcurrentBag<GsaElement1dGoo>();
          _elements.e2d = new ConcurrentBag<GsaElement2dGoo>();
          _elements.e3d = new ConcurrentBag<GsaElement3dGoo>();
          _members.m1d = new ConcurrentBag<GsaMember1dGoo>();
          _members.m2d = new ConcurrentBag<GsaMember2dGoo>();
          _members.m3d = new ConcurrentBag<GsaMember3dGoo>();
          foreach (object elem in gooObjects) {
            switch (elem) {
              case GsaMaterialGoo materialGoo:
                _properties.materials.Add(materialGoo);
                break;

              case GsaSectionGoo sectionGoo:
                _properties.sections.Add(sectionGoo);
                break;

              case GsaProperty2dGoo prop2dGoo:
                _properties.prop2ds.Add(prop2dGoo);
                break;

              case GsaProperty3dGoo prop3dGoo:
                _properties.prop3ds.Add(prop3dGoo);
                break;

              case GsaElement1dGoo elem1dGoo:
                _elements.e1d.Add(elem1dGoo);
                break;

              case GsaElement2dGoo elem2dGoo:
                _elements.e2d.Add(elem2dGoo);
                break;

              case GsaElement3dGoo elem3dGoo:
                _elements.e3d.Add(elem3dGoo);
                break;

              case GsaMember1dGoo member1dGoo:
                _members.m1d.Add(member1dGoo);
                break;

              case GsaMember2dGoo member2dGoo:
                _members.m2d.Add(member2dGoo);
                break;

              case GsaMember3dGoo member3dGoo:
                _members.m3d.Add(member3dGoo);
                break;
            }
          }
          break;

        case EntityType.Member:
          _properties.materials = new List<GsaMaterialGoo>();
          _properties.sections = new List<GsaSectionGoo>();
          _properties.prop2ds = new List<GsaProperty2dGoo>();
          _properties.prop3ds = new List<GsaProperty3dGoo>();
          _members.m1d = new ConcurrentBag<GsaMember1dGoo>();
          _members.m2d = new ConcurrentBag<GsaMember2dGoo>();
          _members.m3d = new ConcurrentBag<GsaMember3dGoo>();
          foreach (object elem in gooObjects) {
            switch (elem) {
              case GsaMaterialGoo materialGoo:
                _properties.materials.Add(materialGoo);
                break;

              case GsaSectionGoo sectionGoo:
                _properties.sections.Add(sectionGoo);
                break;

              case GsaProperty2dGoo prop2dGoo:
                _properties.prop2ds.Add(prop2dGoo);
                break;

              case GsaProperty3dGoo prop3dGoo:
                _properties.prop3ds.Add(prop3dGoo);
                break;

              case GsaMember1dGoo member1dGoo:
                _members.m1d.Add(member1dGoo);
                break;

              case GsaMember2dGoo member2dGoo:
                _members.m2d.Add(member2dGoo);
                break;

              case GsaMember3dGoo member3dGoo:
                _members.m3d.Add(member3dGoo);
                break;
            }
          }
          break;

        case EntityType.Case:
          _cases = gooObjects.Select(x => (int)x).ToList();
          break;
      }
    }

    private void PopulateListObjectsFromModel(LengthUnit unit) {
      if (_model == null) {
        return;
      }

      switch (EntityType) {
        case EntityType.Node:
          _nodes = Nodes.GetNodes(
            _model.ApiModel.Nodes(Definition), unit, _model.ApiModel.Axes(), _model.SpringProps);
          break;

        case EntityType.Element:
          var elementLocalAxesDict = new Dictionary<int, ReadOnlyCollection<double>>();
          foreach (int id in _model.ApiModel.Elements(Definition).Keys) {
            elementLocalAxesDict.Add(id, _model.ApiModel.ElementDirectionCosine(id));
          }
          // TO-DO: GSA-6773: add way to get properties/materials by list
          _properties.materials = new List<GsaMaterialGoo>();
          _properties.sections = new List<GsaSectionGoo>();
          _properties.prop2ds = new List<GsaProperty2dGoo>();
          _properties.prop3ds = new List<GsaProperty3dGoo>();

          var elements = new Elements(_model, Definition);
          _elements.e1d = elements.Element1ds;
          _elements.e2d = elements.Element2ds;
          _elements.e3d = elements.Element3ds;
          break;

        case EntityType.Member:
          var memberLocalAxesDict = new Dictionary<int, ReadOnlyCollection<double>>();
          foreach (int id in _model.ApiModel.Members(Definition).Keys) {
            memberLocalAxesDict.Add(id, _model.ApiModel.MemberDirectionCosine(id));
          }
          // TO-DO: GSA-6773: add way to get properties/materials by list
          _properties.materials = new List<GsaMaterialGoo>();
          _properties.sections = new List<GsaSectionGoo>();
          _properties.prop2ds = new List<GsaProperty2dGoo>();
          _properties.prop3ds = new List<GsaProperty3dGoo>();

          var members = new Members(_model, null, Definition);
          _members.m1d = members.Member1ds;
          _members.m2d = members.Member2ds;
          _members.m3d = members.Member3ds;
          break;

        case EntityType.Case:
          var tempApiList = new EntityList() {
            Type = GsaAPI.EntityType.Case,
            Name = Name,
            Definition = Definition
          };
          _cases = _model.ApiModel.ExpandList(tempApiList).ToList();
          break;

        case EntityType.Undefined:
        default:
          break;
      }
    }
  }
}
