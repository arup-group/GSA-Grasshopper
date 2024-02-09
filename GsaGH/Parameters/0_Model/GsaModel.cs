using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GsaAPI;
using GsaGH.Helpers.GsaApi;
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
    public const string GenericConcreteCodeName = "generic conc.";
    public const string GenericSteelCodeName = "<steel generic>";

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
        _boundingBox = BoundingBox.Empty;
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
    internal GsaMaterials Materials { get; private set; }
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
        Setup();
        _analysisLayerPreview = null;
        _designLayerPreview = null;
      }
    }
    internal ReadOnlyDictionary<int, GsaSectionGoo> Sections { get; private set; }
    internal ReadOnlyDictionary<int, GsaProperty2dGoo> Prop2ds { get; private set; }
    internal ReadOnlyDictionary<int, GsaProperty3dGoo> Prop3ds { get; private set; }
    internal ReadOnlyDictionary<int, GsaSpringPropertyGoo> SpringProps { get; private set; }
    private BoundingBox _boundingBox = BoundingBox.Empty;
    private LengthUnit _lengthUnit = LengthUnit.Undefined;
    private Model _model = new Model();
    private Section3dPreview _analysisLayerPreview;
    private Section3dPreview _designLayerPreview;

    public GsaModel() {
      SetUserDefaultUnits();
      InstantiateApiFields();
      Setup();
    }

    internal GsaModel(Model model) {
      Model = model;
      InstantiateApiFields();
      Setup();
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

    internal void SetUserDefaultUnits() {
      ModelFactory.SetUserDefaultUnits(_model);
    }

    internal Tuple<List<GsaAnalysisTaskGoo>, List<GsaAnalysisCaseGoo>> GetAnalysisTasksAndCombinations() {
      ReadOnlyDictionary<int, AnalysisTask> tasks = Model.AnalysisTasks();
      ReadOnlyDictionary<int, LoadCase> loadCases = Model.LoadCases();

      var tasksList = new List<GsaAnalysisTaskGoo>();
      var caseList = new List<GsaAnalysisCaseGoo>();
      var caseIDs = new List<int>();

      foreach (KeyValuePair<int, AnalysisTask> item in tasks) {
        var task = new GsaAnalysisTask(item.Key, item.Value, Model);
        tasksList.Add(new GsaAnalysisTaskGoo(task));
        caseIDs.AddRange(task.Cases.Select(acase => acase.Id));
      }

      caseIDs.AddRange(GetLoadCases());

      foreach (int caseId in caseIDs) {
        string caseName = Model.AnalysisCaseName(caseId);
        if (caseName == string.Empty) {
          if (loadCases.ContainsKey(caseId)) {
            caseName = loadCases[caseId].Name;
          }
          if (caseName == string.Empty) {
            caseName = "Case " + caseId;
          }
        }

        string caseDescription = Model.AnalysisCaseDescription(caseId);
        if (caseDescription == string.Empty) {
          caseDescription = "L" + caseId;
        }

        caseList.Add(
          new GsaAnalysisCaseGoo(new GsaAnalysisCase(caseId, caseName, caseDescription)));
      }

      return new Tuple<List<GsaAnalysisTaskGoo>, List<GsaAnalysisCaseGoo>>(tasksList, caseList);
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

    internal List<GsaGridLine> GetGridLines() {
      var gridLines = new List<GsaGridLine>();
      foreach (GridLine gridLine in Model.GridLines().Values) {
        PolyCurve curve = GsaGridLine.ToCurve(gridLine);
        gridLines.Add(new GsaGridLine(gridLine, curve));
      }
      return gridLines;
    }

    internal List<int> GetLoadCases() {
      var caseIDs = new List<int>();
      ReadOnlyCollection<GravityLoad> gravities = Model.GravityLoads();
      caseIDs.AddRange(gravities.Select(x => x.Case));

      foreach (GsaAPI.NodeLoadType typ in Enum.GetValues(typeof(GsaAPI.NodeLoadType))) {
        ReadOnlyCollection<NodeLoad> nodeLoads;
        try // some GsaAPI.NodeLoadTypes are currently not supported in the API and throws an error
        {
          nodeLoads = Model.NodeLoads(typ);
          caseIDs.AddRange(nodeLoads.Select(x => x.Case));
        } catch (Exception) {
          // ignored
        }
      }

      ReadOnlyCollection<BeamLoad> beamLoads = Model.BeamLoads();
      caseIDs.AddRange(beamLoads.Select(x => x.Case));

      ReadOnlyCollection<BeamThermalLoad> beamThermalLoads = Model.BeamThermalLoads();
      caseIDs.AddRange(beamThermalLoads.Select(x => x.Case));

      ReadOnlyCollection<FaceLoad> faceLoads = Model.FaceLoads();
      caseIDs.AddRange(faceLoads.Select(x => x.Case));

      ReadOnlyCollection<FaceThermalLoad> faceThermalLoads = Model.FaceThermalLoads();
      caseIDs.AddRange(faceThermalLoads.Select(x => x.Case));

      ReadOnlyCollection<GridPointLoad> gridPointLoads = Model.GridPointLoads();
      caseIDs.AddRange(gridPointLoads.Select(x => x.Case));

      ReadOnlyCollection<GridLineLoad> gridLineLoads = Model.GridLineLoads();
      caseIDs.AddRange(gridLineLoads.Select(x => x.Case));

      ReadOnlyCollection<GridAreaLoad> gridAreaLoads = Model.GridAreaLoads();
      caseIDs.AddRange(gridAreaLoads.Select(x => x.Case));

      return caseIDs.GroupBy(x => x).Select(y => y.First()).OrderBy(z => z).ToList();
    }

    internal List<GsaList> GetLists() {
      var lists = new List<GsaList>();
      foreach (KeyValuePair<int, EntityList> apiList in Model.Lists()) {
        lists.Add(new GsaList(apiList.Key, apiList.Value, this));
      }
      return lists;
    }

    internal GsaProperty2d GetProp2d(Element e) {
      return Prop2ds.TryGetValue(e.Property, out GsaProperty2dGoo prop)
        ? prop.Value
        : e.Property > 0 ? new GsaProperty2d(e.Property) : null;
    }

    internal GsaProperty2d GetProp2d(Member m) {
      return Prop2ds.TryGetValue(m.Property, out GsaProperty2dGoo prop)
        ? prop.Value
        : m.Property > 0 ? new GsaProperty2d(m.Property) : null;
    }

    internal GsaProperty3d GetProp3d(Element e) {
      return Prop3ds.TryGetValue(e.Property, out GsaProperty3dGoo prop)
        ? prop.Value
        : e.Property > 0 ? new GsaProperty3d(e.Property) : null;
    }

    internal GsaProperty3d GetProp3d(Member m) {
      return Prop3ds.TryGetValue(m.Property, out GsaProperty3dGoo prop)
        ? prop.Value
        : m.Property > 0 ? new GsaProperty3d(m.Property) : null;
    }

    internal GsaSection GetSection(Element e) {
      return Sections.TryGetValue(e.Property, out GsaSectionGoo section)
        ? section.Value
        : e.Property > 0 ? new GsaSection(e.Property) : null;
    }

    internal GsaSection GetSection(Member m) {
      return Sections.TryGetValue(m.Property, out GsaSectionGoo section)
        ? section.Value
        : m.Property > 0 ? new GsaSection(m.Property) : null;
    }

    internal GsaSpringProperty GetSpringProperty(Element m) {
      return SpringProps.TryGetValue(m.Property, out GsaSpringPropertyGoo prop)
        ? prop.Value
        : m.Property > 0 ? new GsaSpringProperty(m.Property) : null;
    }

    internal GsaSpringProperty GetSpringProperty(Node n) {
      return SpringProps.TryGetValue(n.SpringProperty, out GsaSpringPropertyGoo prop)
        ? prop.Value
        : n.SpringProperty > 0 ? new GsaSpringProperty(n.SpringProperty) : null;
    }

    private void InstantiateApiFields() {
      ApiNodes = Model.Nodes();
      ApiAxis = Model.Axes();
      _lengthUnit = UnitMapping.GetUnit(Model.UiUnits().LengthLarge);
      ApiMemberLocalAxes = new ReadOnlyDictionary<int, ReadOnlyCollection<double>>(
                Model.Members().Keys.ToDictionary(id => id, id => Model.MemberDirectionCosine(id)));
      ApiElementLocalAxes = new ReadOnlyDictionary<int, ReadOnlyCollection<double>>(
            Model.Elements().Keys.ToDictionary(id => id, id => Model.ElementDirectionCosine(id)));
    }

    private void Setup() {
      Materials = new GsaMaterials(Model);
      Sections = GsaPropertyFactory.CreateSectionsFromApi(Model.Sections(), Materials, Model.SectionModifiers());
      Prop2ds = GsaPropertyFactory.CreateProp2dsFromApi(Model.Prop2Ds(), Materials, Model.Axes());
      Prop3ds = GsaPropertyFactory.CreateProp3dsFromApi(Model.Prop3Ds(), Materials);
      SpringProps = GsaPropertyFactory.CreateSpringPropsFromApi(Model.SpringProperties());
    }
  }
}
