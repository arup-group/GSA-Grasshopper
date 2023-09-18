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
  /// A GSA model is the main parameter that associates with a GSA model file. Data and results can be extracted from an opened model. A model contains all the constituent parts (Properties, Elements/Members, Loads, Cases, Tasks, etc). 
  /// <para>Use the <see cref="Components.CreateModel"/> or <see cref="Components.AnalyseModel"/> components to assemble a new Model or use the <see cref="Components.OpenModel"/> to work with an existing Model. You can use the <see cref="Components.GetModelProperties"/> or <see cref="Components.GetModelGeometry"/> to start editing the objects from an existing model. </para>
  /// <para>If the model has been analysed you can use the <see cref="Components.SelectResult"/> component to explore the Models structural performance and behaviour.</para>
  /// </summary>
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
        _analysisLayerPreview = null;
        _designLayerPreview = null;
      }
    }
    public string FileNameAndPath { get; set; }
    public Guid Guid { get; set; } = Guid.NewGuid();
    internal Titles Titles => Model.Titles();
    internal UiUnits Units => Model.UiUnits();
    internal ReadOnlyDictionary<int, ReadOnlyCollection<double>> ApiElementLocalAxes { get; private set; }
    internal ReadOnlyDictionary<int, ReadOnlyCollection<double>> ApiMemberLocalAxes { get; private set; }
    internal ReadOnlyDictionary<int, Node> ApiNodes { get; private set; }
    internal ReadOnlyDictionary<int, Axis> ApiAxis { get; private set; }
    internal Materials Materials { get; private set; }
    internal Section3dPreview AnalysisLayerPreview {
      get {
        if (Model.Elements().Count > 0) {
          _analysisLayerPreview ??= new Section3dPreview(this, Layer.Analysis);
        }
        return _analysisLayerPreview;
      }
    }
    internal Section3dPreview DesignLayerPreview {
      get {
        if (Model.Members().Count > 0) {
          _designLayerPreview ??= new Section3dPreview(this, Layer.Design);
        }
        return _designLayerPreview;
      }
    }
    public Model Model {
      get => _model;
      set {
        _model = value;
        InstantiateApiFields();
        _analysisLayerPreview = null;
        _designLayerPreview = null;
      }
    }
    internal Helpers.Import.Properties Properties { get; private set; }
    private BoundingBox _boundingBox = BoundingBox.Empty;
    private LengthUnit _lengthUnit = LengthUnit.Undefined;
    private Model _model = new Model();
    private Section3dPreview _analysisLayerPreview;
    private Section3dPreview _designLayerPreview;

    public GsaModel() {
      SetUserDefaultUnits(Model.UiUnits());
      InstantiateApiFields();
    }

    internal GsaModel(Model model) {
      Model = model;
      InstantiateApiFields();
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

    internal static Model CreateModelFromCodes(string concreteDesignCode = "", string steelDesignCode = "") {
      if (concreteDesignCode == string.Empty) {
        concreteDesignCode = DesignCode.GetConcreteDesignCodeNames()[8];
      }

      if (steelDesignCode == string.Empty) {
        steelDesignCode = DesignCode.GetSteelDesignCodeNames()[8];
      }

      return TryUpgradeCode(concreteDesignCode, steelDesignCode);
    }

    private void InstantiateApiFields() {
      ApiNodes = Model.Nodes();
      ApiAxis = Model.Axes();
      _lengthUnit = UnitMapping.GetUnit(Model.UiUnits().LengthLarge);
      Materials = new Materials(Model);
      Properties = new Helpers.Import.Properties(Model, Materials);
      ApiMemberLocalAxes = new ReadOnlyDictionary<int, ReadOnlyCollection<double>>(
                Model.Members().Keys.ToDictionary(id => id, id => Model.MemberDirectionCosine(id)));
      ApiElementLocalAxes = new ReadOnlyDictionary<int, ReadOnlyCollection<double>>(
            Model.Elements().Keys.ToDictionary(id => id, id => Model.ElementDirectionCosine(id)));
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

    private static Model TryUpgradeCode(string ssConcreteCode, string ssSteelCode) {
      string concreteDesignCode = ssConcreteCode;
      string steelDesignCode = ssSteelCode;
      try {
        // will fail for superseeded codesd
        return new Model(concreteDesignCode, steelDesignCode);
      } catch (GsaApiException) { //GsaAPI.GsaApiException: 'Concrete design code is not supported.'
        ReadOnlyCollection<string> concreteCodes = DesignCode.GetConcreteDesignCodeNames();
        if (!concreteCodes.Contains(concreteDesignCode)) {
          concreteDesignCode = FindSimilarCode(concreteDesignCode, concreteCodes);
        }

        ReadOnlyCollection<string> steelCodes = DesignCode.GetSteelDesignCodeNames();
        if (!steelCodes.Contains(steelDesignCode)) {
          steelDesignCode = FindSimilarCode(steelDesignCode, steelCodes);
        }

        return new Model(concreteDesignCode, steelDesignCode);
      }
    }

    private static string FindSimilarCode(string code, ReadOnlyCollection<string> codes) {
      var codeList = codes.ToList();
      for (int i = 0; i < code.Length; i++) {
        for (int j = codeList.Count - 1; j >= 0; j--) {
          if (codeList[j][i] != code[i]) {
            if (codeList.Count > 1) {
              codeList.RemoveAt(j);
              if (codeList.Count == 1) {
                return codeList[0];
              }
            }
          }
        }
      }
      return codeList[0];
    }
  }
}
