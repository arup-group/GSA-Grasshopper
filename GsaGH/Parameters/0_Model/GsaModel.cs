using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GsaAPI;
using GsaGH.Helpers.GsaApi.EnumMappings;
using GsaGH.Helpers.Import;
using OasysUnits;
using Rhino.Geometry;
using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGH.Parameters {
  /// <summary>
  ///   Model class, this class defines the basic properties and methods for any Gsa Model
  /// </summary>
  [Serializable]
  public class GsaModel {
    public BoundingBox BoundingBox {
      get {
        if (!_boundingBox.IsValid) {
          _boundingBox = GetBoundingBox();
        }

        return _boundingBox;
      }
    }
    public LengthUnit ModelUnit {
      get => _lengthUnit;
      set {
        _lengthUnit = value;
        Units.LengthLarge = UnitMapping.GetApiUnit(_lengthUnit);
      }
    }
    public string FileNameAndPath { get; set; }
    public Guid Guid { get; set; } = Guid.NewGuid();
    public Model Model { get; set; } = new Model();
    internal GsaAPI.Titles Titles => Model.Titles();
    internal GsaAPI.UiUnits Units => Model.UiUnits();
    internal ReadOnlyDictionary<int, ReadOnlyCollection<double>> ApiElementLocalAxes { get; }
    internal ReadOnlyDictionary<int, ReadOnlyCollection<double>> ApiMemberLocalAxes { get; }
    internal ReadOnlyDictionary<int, Node> ApiNodes { get; }
    internal ReadOnlyDictionary<int, Axis> ApiAxis { get; }
    internal Materials Materials { get; }
    internal Helpers.Import.Properties Properties { get; }
    private BoundingBox _boundingBox = BoundingBox.Empty;
    private LengthUnit _lengthUnit = LengthUnit.Undefined;

    public GsaModel() {
      SetUserDefaultUnits(Model.UiUnits());
    }

    internal GsaModel(Model model) {
      Model = model;
      ApiNodes = Model.Nodes();
      ApiAxis = Model.Axes();
      _lengthUnit = UnitMapping.GetUnit(model.UiUnits().LengthLarge);
      Materials = new Materials(Model);
      Properties = new Helpers.Import.Properties(Model, Materials);
      ApiMemberLocalAxes = new ReadOnlyDictionary<int, ReadOnlyCollection<double>>(
                Model.Members().Keys.ToDictionary(id => id, id => Model.MemberDirectionCosine(id)));
      ApiElementLocalAxes = new ReadOnlyDictionary<int, ReadOnlyCollection<double>>(
            Model.Elements().Keys.ToDictionary(id => id, id => Model.ElementDirectionCosine(id)));
    }

    /// <summary>
    ///   Clones this model so we can make changes safely
    /// </summary>
    /// <returns>Returns a clone of this model with a new GUID</returns>
    public GsaModel Clone() {
      var clone = new GsaModel(Model.Clone()) {
        FileNameAndPath = FileNameAndPath,
        ModelUnit = ModelUnit,
        Guid = Guid.NewGuid(),
        _boundingBox = _boundingBox,
      };
      return clone;
    }

    public GsaModel Duplicate() {
      return this;
    }

    public override string ToString() {
      string s = "New GsaGH Model";
      if (Model != null && Titles != null) {
        if (!string.IsNullOrEmpty(FileNameAndPath)) {
          s = Path.GetFileName(FileNameAndPath).Replace(".gwb", string.Empty);
        }

        if (Titles?.Title != null && Titles.Title != string.Empty) {
          if (s == string.Empty || s == "Invalid") {
            s = Titles.Title;
          } else {
            s += " {" + Titles.Title + "}";
          }
        }
      }

      if (ModelUnit != LengthUnit.Undefined) {
        s += " [" + Length.GetAbbreviation(ModelUnit) + "]";
      }

      return s;
    }

    internal static void SetUserDefaultUnits(UiUnits uiUnits) {
      uiUnits.Acceleration
        = UnitMapping.GetApiUnit(OasysGH.Units.DefaultUnits.AccelerationUnit);
      uiUnits.Angle
       = UnitMapping.GetApiUnit(OasysGH.Units.DefaultUnits.AngleUnit);
      uiUnits.Energy
       = UnitMapping.GetApiUnit(OasysGH.Units.DefaultUnits.EnergyUnit);
      uiUnits.Force
       = UnitMapping.GetApiUnit(OasysGH.Units.DefaultUnits.ForceUnit);
      uiUnits.LengthLarge
       = UnitMapping.GetApiUnit(OasysGH.Units.DefaultUnits.LengthUnitGeometry);
      uiUnits.LengthSections
       = UnitMapping.GetApiUnit(OasysGH.Units.DefaultUnits.LengthUnitSection);
      uiUnits.LengthSmall
       = UnitMapping.GetApiUnit(OasysGH.Units.DefaultUnits.LengthUnitResult);
      uiUnits.Mass
       = UnitMapping.GetApiUnit(OasysGH.Units.DefaultUnits.MassUnit);
      uiUnits.Stress
       = UnitMapping.GetApiUnit(OasysGH.Units.DefaultUnits.StressUnitResult);
      uiUnits.TimeLong
       = UnitMapping.GetApiUnit(OasysGH.Units.DefaultUnits.TimeLongUnit);
      uiUnits.TimeMedium
       = UnitMapping.GetApiUnit(OasysGH.Units.DefaultUnits.TimeMediumUnit);
      uiUnits.TimeShort
       = UnitMapping.GetApiUnit(OasysGH.Units.DefaultUnits.TimeShortUnit);
      uiUnits.Velocity
       = UnitMapping.GetApiUnit(OasysGH.Units.DefaultUnits.VelocityUnit);
    }

    private BoundingBox GetBoundingBox() {
      var outNodes = new ConcurrentDictionary<int, Node>(Model.Nodes());
      var pts = new ConcurrentBag<Point3d>();
      Parallel.ForEach(outNodes,
        node => pts.Add(Nodes.Point3dFromNode(node.Value, LengthUnit.Meter)));

      if (ModelUnit == LengthUnit.Undefined || ModelUnit == LengthUnit.Meter) {
        return new BoundingBox(pts);
      }

      double factor = 1 / new Length(1, ModelUnit).Meters;
      var scale = Transform.Scale(new Point3d(0, 0, 0), factor);
      return new BoundingBox(pts, scale);
    }
  }
}
