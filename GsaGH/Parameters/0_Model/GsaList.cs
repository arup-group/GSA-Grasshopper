using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GsaAPI;
using GsaGH.Helpers.Import;
using OasysGH;
using OasysUnits.Units;

namespace GsaGH.Parameters {
  public enum EntityType {
    Undefined,
    Node,
    Element,
    Member,
    Case,
  }

  /// <summary>
  ///   EntityList class, this class defines the basic properties and methods for any Gsa List
  /// </summary>
  public class GsaList {
    public string Definition {
      get => _definition;
      set {
        Guid = Guid.NewGuid();
        _definition = value;
      }
    }
    public EntityType EntityType {
      get => _entityType;
      set {
        Guid = Guid.NewGuid();
        _entityType = value;
      }
    }
    public Guid Guid { get; set; } = Guid.NewGuid();
    public int Id {
      get => _id;
      set {
        Guid = Guid.NewGuid();
        _id = value;
      }
    }
    public string Name {
      get => _name;
      set {
        Guid = Guid.NewGuid();
        _name = value;
      }
    }

    private EntityType _entityType = EntityType.Undefined;
    private GsaModel _model;
    internal int _id;
    internal string _name;
    internal string _definition;
    internal List<int> _cases;
    internal (List<GsaMaterialGoo> materials, List<GsaSectionGoo> sections,
      List<GsaProp2dGoo> prop2ds, List<GsaProp3dGoo> prop3ds) _properties;
    internal ConcurrentBag<GsaNodeGoo> _nodes;
    internal (ConcurrentBag<GsaElement1dGoo> e1d, ConcurrentBag<GsaElement2dGoo> e2d,
      ConcurrentBag<GsaElement3dGoo> e3d) _elements;
    internal (ConcurrentBag<GsaMember1dGoo> m1d, ConcurrentBag<GsaMember2dGoo> m2d,
      ConcurrentBag<GsaMember3dGoo> m3d) _members;

    public GsaList() { }

    internal GsaList(int id, EntityList list, GsaModel model) {
      EntityType = GetEntityFromAPI(list.Type);
      Id = id;
      Name = list.Name;
      Definition = list.Definition;
      _model = model;
    }
    internal EntityList GetApiList() {
      return new EntityList {
        Name = Name,
        Definition = Definition,
        Type = GetAPIEntityType(EntityType)
      };
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
              new List<GsaProp2dGoo>(_properties.prop2ds.ToList()),
              new List<GsaProp3dGoo>(_properties.prop3ds.ToList()));
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
              new List<GsaProp2dGoo>(_properties.prop2ds.ToList()),
              new List<GsaProp3dGoo>(_properties.prop3ds.ToList()));
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

    public override string ToString() {
      string s = Id > 0 ? ("ID:" + Id + " ") : string.Empty;
      if (Name != null) {
        s += Name + " ";
      } else {
        s += EntityType.ToString() + " List ";
      }
      switch (EntityType) {
        case EntityType.Node:
          if (_nodes != null && _nodes.Count != 0) {
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
          if (_cases != null && _cases.Count != 0) {
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

      return s.Replace("  ", " ");
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
          if (Definition != null && Definition != "") {
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
          Type = GetAPIEntityType(EntityType),
          Definition = definition
        };
        Definition = apiList.Definition;
      }

      switch (EntityType) {
        case EntityType.Node:
          _nodes = new ConcurrentBag<GsaNodeGoo>(gooObjects.Select(x => (GsaNodeGoo)x));
          break;

        case EntityType.Element:
          _properties.materials = new List<GsaMaterialGoo>();
          _properties.sections = new List<GsaSectionGoo>();
          _properties.prop2ds = new List<GsaProp2dGoo>();
          _properties.prop3ds = new List<GsaProp3dGoo>();
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

              case GsaProp2dGoo prop2dGoo:
                _properties.prop2ds.Add(prop2dGoo);
                break;

              case GsaProp3dGoo prop3dGoo:
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
          _properties.prop2ds = new List<GsaProp2dGoo>();
          _properties.prop3ds = new List<GsaProp3dGoo>();
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

              case GsaProp2dGoo prop2dGoo:
                _properties.prop2ds.Add(prop2dGoo);
                break;

              case GsaProp3dGoo prop3dGoo:
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
          _nodes = Nodes.GetNodes(_model.Model.Nodes(Definition), unit, _model.Model.Axes());
          break;

        case EntityType.Element:
          var elementLocalAxesDict = new Dictionary<int, ReadOnlyCollection<double>>();
          foreach (int id in _model.Model.Elements(Definition).Keys) {
            elementLocalAxesDict.Add(id, _model.Model.ElementDirectionCosine(id));
          }
          // TO-DO: GSA-6773: add way to get properties/materials by list
          _properties.materials = new List<GsaMaterialGoo>();
          _properties.sections = new List<GsaSectionGoo>();
          _properties.prop2ds = new List<GsaProp2dGoo>();
          _properties.prop3ds = new List<GsaProp3dGoo>();

          _elements = Elements.GetElements(_model.Model.Elements(Definition), _model.Model.Nodes(),
            _model.Model.Sections(), _model.Model.Prop2Ds(), _model.Model.Prop3Ds(),
            _model.Model.AnalysisMaterials(), _model.Model.SectionModifiers(), elementLocalAxesDict,
            _model.Model.Axes(), unit, false);
          break;

        case EntityType.Member:
          var memberLocalAxesDict = new Dictionary<int, ReadOnlyCollection<double>>();
          foreach (int id in _model.Model.Members(Definition).Keys) {
            memberLocalAxesDict.Add(id, _model.Model.MemberDirectionCosine(id));
          }
          // TO-DO: GSA-6773: add way to get properties/materials by list
          _properties.materials = new List<GsaMaterialGoo>();
          _properties.sections = new List<GsaSectionGoo>();
          _properties.prop2ds = new List<GsaProp2dGoo>();
          _properties.prop3ds = new List<GsaProp3dGoo>();

          _members = Members.GetMembers(_model.Model.Members(Definition), _model.Model.Nodes(),
            _model.Model.Sections(), _model.Model.Prop2Ds(), _model.Model.Prop3Ds(),
            _model.Model.AnalysisMaterials(), _model.Model.SectionModifiers(), memberLocalAxesDict,
            _model.Model.Axes(), unit, false);
          break;

        case EntityType.Case:
          var tempApiList = new GsaAPI.EntityList() { Type = GsaAPI.EntityType.Case, Name = Name, Definition = Definition };
          _cases = _model.Model.ExpandList(tempApiList).ToList();
          break;

        case EntityType.Undefined:
        default:
          break;
      }
    }

    internal static EntityType GetEntityFromAPI(GsaAPI.EntityType type) {
      switch (type) {
        case GsaAPI.EntityType.Node:
          return EntityType.Node;

        case GsaAPI.EntityType.Element:
          return EntityType.Element;

        case GsaAPI.EntityType.Member:
          return EntityType.Member;

        case GsaAPI.EntityType.Case:
          return EntityType.Case;

        default:
          return EntityType.Undefined;
      }
    }

    internal static GsaAPI.EntityType GetAPIEntityType(EntityType type) {
      switch (type) {
        case EntityType.Node:
          return GsaAPI.EntityType.Node;

        case EntityType.Element:
          return GsaAPI.EntityType.Element;

        case EntityType.Member:
          return GsaAPI.EntityType.Member;

        case EntityType.Case:
          return GsaAPI.EntityType.Case;

        default:
          return GsaAPI.EntityType.Undefined;
      }
    }
  }
}
