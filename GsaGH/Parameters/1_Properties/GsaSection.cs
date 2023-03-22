using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using GsaAPI;
using GsaGH.Helpers.GsaAPI;
using OasysGH.Units;
using OasysUnits;

namespace GsaGH.Parameters {
  /// <summary>
  /// Section class, this class defines the basic properties and methods for any <see cref="GsaAPI.Section"/>
  /// </summary>
  public class GsaSection {
    #region fields
    private int _id = 0;
    private Guid _guid = Guid.NewGuid();
    private GsaMaterial _material = new GsaMaterial();
    private GsaSectionModifier _modifier = new GsaSectionModifier();
    private Section _section = new Section();
    #endregion

    #region properties
    internal Section API_Section {
      get {
        return _section;
      }
      set {
        _guid = Guid.NewGuid();
        _section = value;
        _material = new GsaMaterial(this);
        IsReferencedByID = false;
      }
    }
    #region section properties
    public Area Area {
      get {
        var area = new Area(_section.Area, UnitSystem.SI);
        return new Area(area.As(DefaultUnits.SectionAreaUnit), DefaultUnits.SectionAreaUnit);
      }
    }
    public AreaMomentOfInertia Iyy {
      get {
        var inertia = new AreaMomentOfInertia(_section.Iyy, UnitSystem.SI);
        return new AreaMomentOfInertia(inertia.As(DefaultUnits.SectionAreaMomentOfInertiaUnit), DefaultUnits.SectionAreaMomentOfInertiaUnit);
      }
    }
    public AreaMomentOfInertia Iyz {
      get {
        var inertia = new AreaMomentOfInertia(_section.Iyz, UnitSystem.SI);
        return new AreaMomentOfInertia(inertia.As(DefaultUnits.SectionAreaMomentOfInertiaUnit), DefaultUnits.SectionAreaMomentOfInertiaUnit);
      }
    }
    public AreaMomentOfInertia Izz {
      get {
        var inertia = new AreaMomentOfInertia(_section.Izz, UnitSystem.SI);
        return new AreaMomentOfInertia(inertia.As(DefaultUnits.SectionAreaMomentOfInertiaUnit), DefaultUnits.SectionAreaMomentOfInertiaUnit);
      }
    }
    public AreaMomentOfInertia J {
      get {
        var inertia = new AreaMomentOfInertia(_section.J, UnitSystem.SI);
        return new AreaMomentOfInertia(inertia.As(DefaultUnits.SectionAreaMomentOfInertiaUnit), DefaultUnits.SectionAreaMomentOfInertiaUnit);
      }
    }
    public double Ky {
      get {
        return _section.Ky;
      }
    }
    public double Kz {
      get {
        return _section.Kz;
      }
    }
    public IQuantity SurfaceAreaPerLength {
      get {
        var area = new Area(_section.SurfaceAreaPerLength, UnitSystem.SI);
        var len = new Length(1, DefaultUnits.LengthUnitSection);
        Area unitArea = len * len;
        var areaOut = new Area(area.As(unitArea.Unit), unitArea.Unit);
        return areaOut / len;
      }
    }
    public VolumePerLength VolumePerLength {
      get {
        return new VolumePerLength(_section.VolumePerLength, UnitSystem.SI);
      }
    }
    #endregion
    public int Id {
      get {
        return _id;
      }
      set {
        CloneApiObject();
        _id = value;
      }
    }
    internal bool IsReferencedByID { get; set; } = false;
    public GsaMaterial Material {
      get {
        return _material;
      }
      set {
        _material = value;
        if (_section == null)
          _section = new Section();
        else
          CloneApiObject();

        _section.MaterialType = Helpers.Export.Materials.ConvertType(_material);
        _section.MaterialAnalysisProperty = _material.AnalysisProperty;
        _section.MaterialGradeProperty = _material.GradeProperty;
        IsReferencedByID = false;
      }
    }
    public GsaSectionModifier Modifier {
      get {
        return _modifier;
      }
      set {
        CloneApiObject();
        _modifier = value;
        IsReferencedByID = false;
      }
    }
    #region GsaAPI members
    public string Name {
      get {
        return _section.Name;
      }
      set {
        CloneApiObject();
        _section.Name = value;
        IsReferencedByID = false;
      }
    }
    public int Pool {
      get {
        return _section.Pool;
      }
      set {
        CloneApiObject();
        _section.Pool = value;
        IsReferencedByID = false;
      }
    }
    public int MaterialID {
      get {
        return _section.MaterialAnalysisProperty;
      }
      set {
        CloneApiObject();
        _section.MaterialAnalysisProperty = value;
        _material.AnalysisProperty = _section.MaterialAnalysisProperty;
        IsReferencedByID = false;
      }
    }
    public string Profile {
      get {
        return _section.Profile.Replace("%", " ");
      }
      set {
        if (ValidProfile(value)) {
          CloneApiObject();
          _section.Profile = value;
          IsReferencedByID = false;
        }
      }
    }
    public Color Colour {
      get {
        return (Color)_section.Colour;
      }
      set {
        CloneApiObject();
        _section.Colour = value;
        IsReferencedByID = false;
      }
    }
    private void CloneApiObject() {
      // temp profile clone 
      string prfl = _section.Profile.Replace("%", " ");
      string[] pfs = prfl.Split(' ');
      if (pfs.Last() == "S/S")
        prfl = string.Join(" ", pfs[0], pfs[1], pfs[2]);

      var sec = new Section() {
        MaterialAnalysisProperty = _section.MaterialAnalysisProperty,
        MaterialGradeProperty = _section.MaterialGradeProperty,
        MaterialType = _section.MaterialType,
        Name = _section.Name.ToString(),
        Pool = _section.Pool,
        Profile = prfl
        //Profile = this._section.Profile
      };
      if ((Color)_section.Colour != Color.FromArgb(0, 0, 0)) // workaround to handle that System.Drawing.Color is non-nullable type
        sec.Colour = _section.Colour;

      _section = sec;
      _modifier = Modifier.Duplicate(true);
      _guid = Guid.NewGuid();
    }
    #endregion
    public Guid Guid {
      get {
        return _guid;
      }
    }
    #endregion

    #region constructors
    public GsaSection() {
    }

    public GsaSection(int id) {
      _id = id;
      IsReferencedByID = true;
    }

    public GsaSection(string profile) {
      _section.Profile = profile;
    }

    public GsaSection(string profile, int id = 0) {
      _section.Profile = profile;
      _id = id;
    }

    internal GsaSection(ReadOnlyDictionary<int, Section> sDict, int id, ReadOnlyDictionary<int, SectionModifier> modDict, ReadOnlyDictionary<int, AnalysisMaterial> matDict) : this(id) {
      if (!sDict.ContainsKey(id))
        return;
      _section = sDict[id];
      IsReferencedByID = false;
      // modifier
      if (modDict.ContainsKey(id))
        _modifier = new GsaSectionModifier(modDict[id]);
      // material
      if (_section.MaterialAnalysisProperty != 0 && matDict.ContainsKey(_section.MaterialAnalysisProperty))
        _material.AnalysisMaterial = matDict[_section.MaterialAnalysisProperty];
      _material = new GsaMaterial(this);
    }
    #endregion

    #region methods
    public GsaSection Duplicate(bool cloneApiElement = false) {
      var dup = new GsaSection {
        _section = _section,
        _id = _id,
        _material = _material.Duplicate(),
        _modifier = _modifier.Duplicate(cloneApiElement),
        _guid = new Guid(_guid.ToString()),
        IsReferencedByID = IsReferencedByID
      };
      if (cloneApiElement)
        dup.CloneApiObject();
      return dup;
    }

    public override string ToString() {
      string pb = Id > 0 ? "PB" + Id + " " : "";
      string prof = _section.Profile.Replace("%", " ") + " ";
      string mat = Mappings.s_materialTypeMapping.FirstOrDefault(x => x.Value == Material.MaterialType).Key + " ";
      string mod = _modifier.IsModified ? " modified" : "";
      return string.Join(" ", pb.Trim(), prof.Trim(), mat.Trim(), mod.Trim()).Trim().Replace("  ", " ");
    }

    internal static bool ValidProfile(string profile) {
      var test = new Section();
      test.Profile = profile;
      if (test.Area == 0)
        return false;
      else
        return true;
    }
    #endregion
  }
}
