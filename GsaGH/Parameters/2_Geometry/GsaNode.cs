using System.Drawing;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GsaGH.Helpers;
using GsaGH.Helpers.Graphics;
using OasysUnits;
using Rhino.Display;
using Rhino.Geometry;
using LengthUnit = OasysUnits.Units.LengthUnit;
using Line = Rhino.Geometry.Line;

namespace GsaGH.Parameters {
  /// <summary>
  /// <para>A Node generally contains `X`, `Y`, and `Z` coordinates as well as <see href="https://docs.oasys-software.com/structural/gsa/explanations/supports.html">Support</see> condition.</para>
  /// <para>In GSA, Nodes are the only objects containing spacial geometrical information (X, Y, Z coordinates). The node numbers are referred to by elements and members in their topology lists and therefore only contains reference to nodes, not the actualy node. In GSA this works because everything belongs to a single model, and the information does not need to be duplicated in elements and members. </para>
  /// <para>In Grasshopper, on the other hand, all parameters (nodes, elements and members) exist independently from each other. For instance, an <see cref="GsaElement1d"/> in GsaGH keeps a copy of its start and end points, which is automatically taken care of by the plugin. Therefore, Nodes almost only need to be used for defining supports, as all other nodes in a model will be included as part of the Elements or Members.</para>
  /// <para>Refer to <see href="https://docs.oasys-software.com/structural/gsa/references/hidr-data-node.html">Node</see> to read more.</para>
  /// </summary>
  public class GsaNode {
    public Color Colour {
      get => (Color)_node.Colour;
      set {
        CloneApiObject();
        _node.Colour = value;
      }
    }
    public int DamperProperty {
      get => _node.DamperProperty;
      set {
        CloneApiObject();
        _node.DamperProperty = value;
      }
    }
    public int Id {
      get => _id;
      set {
        CloneApiObject();
        _id = value;
      }
    }
    public bool IsSupport
      => _node.Restraint.X || _node.Restraint.Y || _node.Restraint.Z || _node.Restraint.XX
        || _node.Restraint.YY || _node.Restraint.ZZ;
    public Plane LocalAxis {
      get => _plane;
      set {
        _plane = value;
        UpdatePreview();
      }
    }
    public int MassProperty {
      get => _node.MassProperty;
      set {
        CloneApiObject();
        _node.MassProperty = value;
      }
    }
    public string Name {
      get => _node.Name;
      set {
        CloneApiObject();
        _node.Name = value;
      }
    }
    public Point3d Point {
      get
        => _node == null ? Point3d.Unset :
          new Point3d(_node.Position.X, _node.Position.Y, _node.Position.Z);
      set {
        CloneApiObject();
        Id = 0;
        _node.Position.X = value.X;
        _node.Position.Y = value.Y;
        _node.Position.Z = value.Z;
        UpdatePreview();
      }
    }
    public GsaBool6 Restraint {
      get
        => new GsaBool6(_node.Restraint.X, _node.Restraint.Y, _node.Restraint.Z, _node.Restraint.XX,
          _node.Restraint.YY, _node.Restraint.ZZ);
      set {
        CloneApiObject();
        _node.Restraint = new NodalRestraint {
          X = value.X,
          Y = value.Y,
          Z = value.Z,
          XX = value.Xx,
          YY = value.Yy,
          ZZ = value.Zz,
        };
        UpdatePreview();
      }
    }
    public int SpringProperty {
      get => _node.SpringProperty;
      set {
        CloneApiObject();
        _node.SpringProperty = value;
      }
    }
    internal Node ApiNode {
      get => _node;
      set {
        _node = value;
        UpdatePreview();
      }
    }
    internal Brep _previewSupportSymbol;
    internal Text3d _previewText;
    internal Line _previewXaxis;
    internal Line _previewYaxis;
    internal Line _previewZaxis;
    private int _id = 0;
    private Node _node = new Node();
    private Plane _plane;

    public GsaNode() { }

    public GsaNode(Point3d position, int id = 0) {
      Point = position;
      Id = id;
      UpdatePreview();
    }

    internal GsaNode(Node node, int id, LengthUnit unit, Plane localAxis = new Plane()) {
      _node = node;
      CloneApiObject();
      if (unit != LengthUnit.Meter) {
        _node.Position.X = new Length(node.Position.X, LengthUnit.Meter).As(unit);
        _node.Position.Y = new Length(node.Position.Y, LengthUnit.Meter).As(unit);
        _node.Position.Z = new Length(node.Position.Z, LengthUnit.Meter).As(unit);
      }

      _id = id;
      _plane = localAxis;
      UpdatePreview();
    }

    public GsaNode Clone() {
      var dup = new GsaNode {
        Id = Id,
        _node = _node,
        _plane = _plane,
      };
      dup.CloneApiObject();
      dup.UpdatePreview();
      return dup;
    }

    public GsaNode Duplicate() {
      return this;
    }

    public GsaNode Morph(SpaceMorph xmorph) {
      if (Point == null) {
        return null;
      }

      GsaNode node = Clone();
      node.Id = 0;

      var pt = new Point3d(node.Point);
      pt = xmorph.MorphPoint(pt);

      node.Point = pt;

      return node;
    }

    public override string ToString() {
      if (ApiNode == null) {
        return "Null";
      }

      string id = Id == 0 ? string.Empty : "ID:" + Id + " ";

      string sptTxt = ApiNode.Restraint.X == false && ApiNode.Restraint.Y == false
        && ApiNode.Restraint.Z == false && ApiNode.Restraint.XX == false
        && ApiNode.Restraint.YY == false && ApiNode.Restraint.ZZ == false ?
          string.Empty :
          ApiNode.Restraint.X & ApiNode.Restraint.Y & ApiNode.Restraint.Z & !ApiNode.Restraint.XX
          & !ApiNode.Restraint.YY & !ApiNode.Restraint.ZZ ?
            " Pin" :
            ApiNode.Restraint.X & ApiNode.Restraint.Y & ApiNode.Restraint.Z & ApiNode.Restraint.XX
            & ApiNode.Restraint.YY & ApiNode.Restraint.ZZ ?
              " Fix" :
              " " + "X:" + (ApiNode.Restraint.X ? "\u2713" : "\u2610") + " Y:"
              + (ApiNode.Restraint.Y ? "\u2713" : "\u2610") + " Z:"
              + (ApiNode.Restraint.Z ? "\u2713" : "\u2610") + " XX:"
              + (ApiNode.Restraint.XX ? "\u2713" : "\u2610") + " YY:"
              + (ApiNode.Restraint.YY ? "\u2713" : "\u2610") + " ZZ:"
              + (ApiNode.Restraint.ZZ ? "\u2713" : "\u2610");

      string localTxt = string.Empty;

      if (!IsGlobalAxis()) {
        var ghPlane = new GH_Plane(LocalAxis);
        localTxt = " Axis:{" + ghPlane.ToString() + "}";
      }

      return string.Join(" ", id, sptTxt, localTxt, $"Pos:{new GH_Point(Point)}").TrimSpaces();
    }

    public GsaNode Transform(Transform xform) {
      if (Point == null) {
        return null;
      }

      GsaNode node = Clone();
      node.Id = 0;
      var pt = new Point3d(node.Point);
      pt.Transform(xform);

      node.Point = pt;
      return node;
    }

    public void UpdateUnit(LengthUnit unit) {
      if (unit == LengthUnit.Meter) // convert from meter to input unit if not meter
      {
        return;
      }

      ApiNode.Position.X = new Length(ApiNode.Position.X, LengthUnit.Meter).As(unit);
      ApiNode.Position.Y = new Length(ApiNode.Position.Y, LengthUnit.Meter).As(unit);
      ApiNode.Position.Z = new Length(ApiNode.Position.Z, LengthUnit.Meter).As(unit);
    }

    internal void CloneApiObject() {
      var node = new Node {
        AxisProperty = _node.AxisProperty,
        DamperProperty = _node.DamperProperty,
        MassProperty = _node.MassProperty,
        Name = _node.Name.ToString(),
        Restraint = new NodalRestraint {
          X = _node.Restraint.X,
          Y = _node.Restraint.Y,
          Z = _node.Restraint.Z,
          XX = _node.Restraint.XX,
          YY = _node.Restraint.YY,
          ZZ = _node.Restraint.ZZ,
        },
        SpringProperty = _node.SpringProperty,
        Position = new Vector3 {
          X = _node.Position.X,
          Y = _node.Position.Y,
          Z = _node.Position.Z,
        },
      };

      if ((Color)_node.Colour
        != Color.FromArgb(0, 0, 0)) // workaround to handle that Color is non-nullable type
      {
        node.Colour = _node.Colour;
      }

      _node = node;
    }

    internal Node GetApiNodeToUnit(LengthUnit unit = LengthUnit.Meter) {
      var node = new Node {
        AxisProperty = _node.AxisProperty,
        DamperProperty = _node.DamperProperty,
        MassProperty = _node.MassProperty,
        Name = _node.Name.ToString(),
        Restraint = new NodalRestraint {
          X = _node.Restraint.X,
          Y = _node.Restraint.Y,
          Z = _node.Restraint.Z,
          XX = _node.Restraint.XX,
          YY = _node.Restraint.YY,
          ZZ = _node.Restraint.ZZ,
        },
        SpringProperty = _node.SpringProperty,
        Position = new Vector3 {
          X = (unit == LengthUnit.Meter) ? _node.Position.X :
            new Length(_node.Position.X, unit).Meters,
          Y = (unit == LengthUnit.Meter) ? _node.Position.Y :
            new Length(_node.Position.Y, unit).Meters,
          Z = (unit == LengthUnit.Meter) ? _node.Position.Z :
            new Length(_node.Position.Z, unit).Meters, },
      };

      if ((Color)_node.Colour
        != Color.FromArgb(0, 0, 0)) // workaround to handle that Color is non-nullable type
      {
        node.Colour = _node.Colour;
      }

      return node;
    }

    internal bool IsGlobalAxis() {
      // AxisProperty = 0 is Global but 0 is also the default value,
      // so if we have set a local Plane the AxisProperty might still
      // be 0 as it is only updated in the node assembly method
      if (ApiNode.AxisProperty != 0) {
        return false;
      }

      // test first if the Plane object is valid
      if (LocalAxis == null && !LocalAxis.IsValid) {
        return true;
      }

      // test for default plane values just to be sure
      if (LocalAxis == Plane.WorldXY || LocalAxis == new Plane() || LocalAxis == Plane.Unset) {
        return true;
      }

      // GsaAPI might import local plane as an invalid plane:
      var invalidPlane = new Plane() {
        Origin = new Point3d(0, 0, 0),
        XAxis = new Vector3d(0, 0, 0),
        YAxis = new Vector3d(0, 0, 0),
        ZAxis = new Vector3d(0, 0, 0),
      };
      if (LocalAxis == invalidPlane) {
        return true;
      }

      return false;
    }

    internal void UpdatePreview() {
      if (_node.Restraint.X || _node.Restraint.Y || _node.Restraint.Z || _node.Restraint.XX
        || _node.Restraint.YY || _node.Restraint.ZZ) {
        Display.PreviewRestraint(Restraint, _plane, Point, ref _previewSupportSymbol,
          ref _previewText);
      } else {
        _previewSupportSymbol = null;
        _previewText = null;
      }

      if (_plane == null || !(_plane != Plane.WorldXY & _plane != new Plane())) {
        return;
      }

      Plane local = _plane.Clone();
      local.Origin = Point;

      _previewXaxis = new Line(Point, local.XAxis, 0.5);
      _previewYaxis = new Line(Point, local.YAxis, 0.5);
      _previewZaxis = new Line(Point, local.ZAxis, 0.5);
    }
  }
}
