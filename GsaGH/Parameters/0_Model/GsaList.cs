using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GsaAPI;
using GsaGH.Helpers.Import;
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
    internal ConcurrentBag<GsaNodeGoo> _nodes;
    internal Tuple<ConcurrentBag<GsaElement1dGoo>, ConcurrentBag<GsaElement2dGoo>,
      ConcurrentBag<GsaElement3dGoo>> _elements;
    internal Tuple<ConcurrentBag<GsaMember1dGoo>, ConcurrentBag<GsaMember2dGoo>,
      ConcurrentBag<GsaMember3dGoo>> _members;

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
          if (_elements != null) {
            dup._elements
              = new Tuple<ConcurrentBag<GsaElement1dGoo>, ConcurrentBag<GsaElement2dGoo>,
                ConcurrentBag<GsaElement3dGoo>>(_elements.Item1, _elements.Item2, _elements.Item3);
          }

          break;

        case EntityType.Member:
          if (_members != null) {
            dup._members
              = new Tuple<ConcurrentBag<GsaMember1dGoo>, ConcurrentBag<GsaMember2dGoo>,
                ConcurrentBag<GsaMember3dGoo>>(_members.Item1, _members.Item2, _members.Item3);
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
      s += Name + " ";
      switch (EntityType) {
        case EntityType.Node:
          if (_nodes != null && _nodes.Count != 0) {
            s += "containing " + _nodes.Count + " " + EntityType.ToString() + "s";
          } else {
            s += EntityType.ToString() + "s (" + Definition + ")";
          }

          break;

        case EntityType.Element:
          if (_elements != null
            && (_elements.Item1.Count + _elements.Item2.Count + _elements.Item3.Count) != 0) {
            s += "containing "
              + (_elements.Item1.Count + _elements.Item2.Count + _elements.Item3.Count) + " "
              + EntityType.ToString() + "s";
          } else {
            s += EntityType.ToString() + "s (" + Definition + ")";
          }

          break;

        case EntityType.Member:
          if (_members != null
            && (_members.Item1.Count + _members.Item2.Count + _members.Item3.Count) != 0) {
            s += "containing "
              + (_members.Item1.Count + _members.Item2.Count + _members.Item3.Count) + " "
              + EntityType.ToString() + "s";
          } else {
            s += EntityType.ToString() + "s (" + Definition + ")";
          }

          break;

        case EntityType.Case:
        case EntityType.Undefined:
          s += EntityType.ToString() + " (" + Definition + ")";
          break;
      }

      return s;
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
          if (_elements == null) {
            return new List<object>();
          }

          list = new List<object>();
          if (_elements.Item1 != null) {
            list.AddRange(_elements.Item1.OrderBy(x => x.Value.Id));
          }

          if (_elements.Item2 != null) {
            list.AddRange(_elements.Item2.OrderBy(x => x.Value.Ids.Min()));
          }

          if (_elements.Item3 != null) {
            list.AddRange(_elements.Item3.OrderBy(x => x.Value.Ids.Min()));
          }

          break;

        case EntityType.Member:
          if (_members == null) {
            return new List<object>();
          }

          list = new List<object>();
          if (_members.Item1 != null) {
            list.AddRange(_members.Item1.OrderBy(x => x.Value.Id));
          }

          if (_members.Item2 != null) {
            list.AddRange(_members.Item2.OrderBy(x => x.Value.Id));
          }

          if (_members.Item3 != null) {
            list.AddRange(_members.Item3.OrderBy(x => x.Value.Id));
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

    internal void SetListObjects(List<object> objects) {
      var def = new List<string>();
      for (int i = objects.Count - 1; i >= 0; i--) {
        if (objects[i] is string txt) {
          def.Add(txt);
          objects.RemoveAt(i);
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
          _nodes = new ConcurrentBag<GsaNodeGoo>(objects.Select(x => new GsaNodeGoo((GsaNode)x)));
          break;

        case EntityType.Element:
          var elem1ds = new ConcurrentBag<GsaElement1dGoo>();
          var elem2ds = new ConcurrentBag<GsaElement2dGoo>();
          var elem3ds = new ConcurrentBag<GsaElement3dGoo>();
          foreach (object elem in objects) {
            if (elem is GsaElement1d elem1d) {
              elem1ds.Add(new GsaElement1dGoo(elem1d));
            } else if (elem is GsaElement2d elem2d) {
              elem2ds.Add(new GsaElement2dGoo(elem2d));
            } else if (elem is GsaElement3d elem3d) {
              elem3ds.Add(new GsaElement3dGoo(elem3d));
            }
          }

          _elements
            = new Tuple<ConcurrentBag<GsaElement1dGoo>, ConcurrentBag<GsaElement2dGoo>,
              ConcurrentBag<GsaElement3dGoo>>(elem1ds, elem2ds, elem3ds);
          break;

        case EntityType.Member:
          var mem1ds = new ConcurrentBag<GsaMember1dGoo>();
          var mem2ds = new ConcurrentBag<GsaMember2dGoo>();
          var mem3ds = new ConcurrentBag<GsaMember3dGoo>();
          foreach (object mem in objects) {
            if (mem is GsaMember1d mem1d) {
              mem1ds.Add(new GsaMember1dGoo(mem1d));
            } else if (mem is GsaMember2d mem2d) {
              mem2ds.Add(new GsaMember2dGoo(mem2d));
            } else if (mem is GsaMember3d mem3d) {
              mem3ds.Add(new GsaMember3dGoo(mem3d));
            }
          }

          _members
            = new Tuple<ConcurrentBag<GsaMember1dGoo>, ConcurrentBag<GsaMember2dGoo>,
              ConcurrentBag<GsaMember3dGoo>>(mem1ds, mem2ds, mem3ds);
          break;

        case EntityType.Case:
          _cases = objects.Select(x => (int)x).ToList();
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
