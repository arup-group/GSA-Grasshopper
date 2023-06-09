using System;
using System.Collections.Concurrent;
using System.IO;
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
    private BoundingBox _boundingBox = BoundingBox.Empty;
    private LengthUnit _lengthUnit = LengthUnit.Undefined;

    public GsaModel() { 
      SetUserDefaultUnits(Model.UiUnits());
    }

    internal GsaModel(Model model) {
      Model = model;
      _lengthUnit = UnitMapping.GetUnit(model.UiUnits().LengthLarge);
    }

    /// <summary>
    ///   Clones this model so we can make changes safely
    /// </summary>
    /// <returns>Returns a clone of this model with a new GUID</returns>
    public GsaModel Clone() {
      var clone = new GsaModel {
        Model = Model.Clone(),
        FileNameAndPath = FileNameAndPath,
        ModelUnit = ModelUnit,
        Guid = Guid.NewGuid(),
        _boundingBox = _boundingBox,
      };
      return clone;
    }

    public GsaModel Duplicate(bool copy = false) {
      if (copy) {
        return Clone();
      }

      // create shallow copy
      var dup = new GsaModel {
        Model = Model,
      };
      if (FileNameAndPath != null) {
        dup.FileNameAndPath = FileNameAndPath;
      }

      dup.Guid = new Guid(Guid.ToString());
      dup.ModelUnit = ModelUnit;
      dup._boundingBox = _boundingBox;
      return dup;
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
