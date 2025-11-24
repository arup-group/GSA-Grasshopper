using System.Drawing;

using Grasshopper.Kernel.Types;

using GsaAPI;

using GsaGH.Helpers;

using OasysUnits;

using Rhino.Geometry;

using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGH.Parameters {
  /// <summary>
  /// <para>A Node generally contains `X`, `Y`, and `Z` coordinates as well as <see href="https://docs.oasys-software.com/structural/gsa/explanations/supports.html">Support</see> condition.</para>
  /// <para>In GSA, Nodes are the only objects containing spacial geometrical information (X, Y, Z coordinates). The node numbers are referred to by elements and members in their topology lists and therefore only contains reference to nodes, not the actualy node. In GSA this works because everything belongs to a single model, and the information does not need to be duplicated in elements and members. </para>
  /// <para>In Grasshopper, on the other hand, all parameters (nodes, elements and members) exist independently from each other. For instance, an <see cref="GsaElement1D"/> in GsaGH keeps a copy of its start and end points, which is automatically taken care of by the plugin. Therefore, Nodes almost only need to be used for defining supports, as all other nodes in a model will be included as part of the Elements or Members.</para>
  /// <para>Refer to <see href="https://docs.oasys-software.com/structural/gsa/references/hidr-data-node.html">Node</see> to read more.</para>
  /// </summary>
  public class GsaNode {
    public Node ApiNode { get; internal set; }
    public int Id { get; set; } = 0;
    public Plane LocalAxis { get; set; } = Plane.WorldXY;
    public GsaSpringProperty SpringProperty { get; set; }
    public SupportPreview SupportPreview { get; private set; }
    public bool IsSupport => IsRestrained();
    public bool IsGlobalAxis => CheckGlobalAxis();
    public Point3d Point {
      get => new Point3d(ApiNode.Position.X, ApiNode.Position.Y, ApiNode.Position.Z);
      set => SetPoint(value);
    }
    public GsaBool6 Restraint {
      get => GetRestraint();
      set => SetRestraint(value);
    }

    /// <summary>
    /// Empty constructor instantiating a new API object
    /// </summary>
    public GsaNode() {
      ApiNode = new Node();
    }

    /// <summary>
    /// Create new instance by casting from a Point
    /// </summary>
    /// <param name="point"></param>
    public GsaNode(Point3d point) {
      ApiNode = new Node();
      Point = point;
    }

    /// <summary>
    /// Create a duplicate instance from another instance
    /// </summary>
    /// <param name="other"></param>
    public GsaNode(GsaNode other) {
      Id = other.Id;
      ApiNode = other.DuplicateApiObject();
      LocalAxis = other.LocalAxis;
      SupportPreview = other.SupportPreview;
      SpringProperty = other.SpringProperty;
    }

    /// <summary>
    /// Create a new instance from an API object from an existing model
    /// </summary>
    internal GsaNode(Node node, int id, LengthUnit unit, Plane localAxis = new Plane()) {
      Id = id;
      ApiNode = node;
      if (unit != LengthUnit.Meter) {
        ApiNode = DuplicateApiObject();
        ApiNode.Position.X = new Length(node.Position.X, LengthUnit.Meter).As(unit);
        ApiNode.Position.Y = new Length(node.Position.Y, LengthUnit.Meter).As(unit);
        ApiNode.Position.Z = new Length(node.Position.Z, LengthUnit.Meter).As(unit);
      }

      if (localAxis != new Plane()) {
        LocalAxis = localAxis;
      }

      UpdatePreview();
    }

    public Node DuplicateApiObject() {
      var node = new Node {
        AxisProperty = ApiNode.AxisProperty,
        DamperProperty = ApiNode.DamperProperty,
        MassProperty = ApiNode.MassProperty,
        Name = ApiNode.Name.ToString(),
        Restraint = new NodalRestraint {
          X = ApiNode.Restraint.X,
          Y = ApiNode.Restraint.Y,
          Z = ApiNode.Restraint.Z,
          XX = ApiNode.Restraint.XX,
          YY = ApiNode.Restraint.YY,
          ZZ = ApiNode.Restraint.ZZ,
        },
        SpringProperty = ApiNode.SpringProperty,
        Position = new Vector3 {
          X = ApiNode.Position.X,
          Y = ApiNode.Position.Y,
          Z = ApiNode.Position.Z,
        },
      };

      // workaround to handle that Color is non-nullable type
      if ((Color)ApiNode.Colour != Color.FromArgb(0, 0, 0)) {
        node.Colour = ApiNode.Colour;
      }

      return node;
    }

    public Node GetApiNodeToUnit(LengthUnit unit = LengthUnit.Meter) {
      Node node = DuplicateApiObject();
      node.Position = new Vector3 {
        X = (unit == LengthUnit.Meter)
          ? ApiNode.Position.X : new Length(ApiNode.Position.X, unit).Meters,
        Y = (unit == LengthUnit.Meter)
          ? ApiNode.Position.Y : new Length(ApiNode.Position.Y, unit).Meters,
        Z = (unit == LengthUnit.Meter)
          ? ApiNode.Position.Z : new Length(ApiNode.Position.Z, unit).Meters, };

      return node;
    }

    public override string ToString() {
      if (ApiNode == null) {
        return "Invalid Node";
      }

      string id = Id > 0 ? $"ID:{Id}" : string.Empty;
      string sptTxt = !IsSupport ? string.Empty
        : ApiNode.Restraint.X & ApiNode.Restraint.Y & ApiNode.Restraint.Z
          & !ApiNode.Restraint.XX & !ApiNode.Restraint.YY & !ApiNode.Restraint.ZZ ? "Pin"
          : ApiNode.Restraint.X & ApiNode.Restraint.Y & ApiNode.Restraint.Z
            & ApiNode.Restraint.XX & ApiNode.Restraint.YY & ApiNode.Restraint.ZZ ? "Fix"
            : "X:" + (ApiNode.Restraint.X ? "\u2713" : "\u2610")
              + " Y:" + (ApiNode.Restraint.Y ? "\u2713" : "\u2610")
              + " Z:" + (ApiNode.Restraint.Z ? "\u2713" : "\u2610")
              + " XX:" + (ApiNode.Restraint.XX ? "\u2713" : "\u2610")
              + " YY:" + (ApiNode.Restraint.YY ? "\u2713" : "\u2610")
              + " ZZ:" + (ApiNode.Restraint.ZZ ? "\u2713" : "\u2610");

      string localTxt = string.Empty;
      if (!IsGlobalAxis) {
        var ghPlane = new GH_Plane(LocalAxis);
        localTxt = " Axis:{" + ghPlane.ToString() + "}";
      }

      return string.Join(" ", id, $"Pos:{new GH_Point(Point)}", sptTxt, localTxt).TrimSpaces();
    }

    public void UpdatePreview() {
      if (!IsSupport) {
        SupportPreview = null;
        return;
      }

      SupportPreview = new SupportPreview(Restraint, LocalAxis, Point, IsGlobalAxis);
    }

    private bool CheckGlobalAxis() {
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
      return LocalAxis == invalidPlane;
    }

    private GsaBool6 GetRestraint() {
      return new GsaBool6(ApiNode.Restraint.X, ApiNode.Restraint.Y, ApiNode.Restraint.Z,
          ApiNode.Restraint.XX, ApiNode.Restraint.YY, ApiNode.Restraint.ZZ);
    }

    private bool IsRestrained() {
      return ApiNode.Restraint.X || ApiNode.Restraint.Y || ApiNode.Restraint.Z || ApiNode.Restraint.XX || ApiNode.Restraint.YY || ApiNode.Restraint.ZZ;
    }

    private void SetPoint(Point3d pt) {
      ApiNode.Position.X = pt.X;
      ApiNode.Position.Y = pt.Y;
      ApiNode.Position.Z = pt.Z;
    }

    private void SetRestraint(GsaBool6 bool6) {
      ApiNode.Restraint = new NodalRestraint {
        X = bool6.X,
        Y = bool6.Y,
        Z = bool6.Z,
        XX = bool6.Xx,
        YY = bool6.Yy,
        ZZ = bool6.Zz,
      };
    }
  }
}
