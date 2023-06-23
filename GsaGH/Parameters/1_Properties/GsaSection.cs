using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using GsaAPI;
using GsaAPI.Materials;
using GsaGH.Helpers.Export;
using GsaGH.Helpers.GsaApi;
using OasysGH.Units;
using OasysUnits;
using OasysUnits.Units;
using AngleUnit = OasysUnits.Units.AngleUnit;

namespace GsaGH.Parameters {
  /// <summary>
  ///   Section class, this class defines the basic properties and methods for any <see cref="GsaAPI.Section" />
  /// </summary>
  public class GsaSection {
    public Length AdditionalOffsetY {
      get => new Length(_section.AdditionalOffsetY, UnitSystem.SI);
      set {
        CloneApiObject();
        _section.AdditionalOffsetY = value.Meters;
        IsReferencedById = false;
      }
    }
    public Length AdditionalOffsetZ {
      get => new Length(_section.AdditionalOffsetZ, UnitSystem.SI);
      set {
        CloneApiObject();
        _section.AdditionalOffsetZ = value.Meters;
        IsReferencedById = false;
      }
    }
    public Angle Angle {
      get {
        var angle = new Angle(_section.Properties().Angle, AngleUnit.Degree);
        return angle.ToUnit(DefaultUnits.AngleUnit);
      }
    }
    public Area Area {
      get {
        var area = new Area(_section.Properties().Area, UnitSystem.SI);
        return area.ToUnit(DefaultUnits.SectionAreaUnit);
      }
    }
    public BasicOffset BasicOffset {
      get => _section.BasicOffset;
      set {
        CloneApiObject();
        _section.BasicOffset = value;
        IsReferencedById = false;
      }
    }
    public SectionModulus C {
      get {
        var c = new SectionModulus(_section.Properties().C, UnitSystem.SI);
        return c.ToUnit(SectionModulusUnit.CubicMillimeter);
      }
    }
    public Color Colour {
      get => (Color)_section.Colour;
      set {
        CloneApiObject();
        _section.Colour = value;
        IsReferencedById = false;
      }
    }
    public Length Cy {
      get {
        var cy = new Length(_section.Properties().Cy, UnitSystem.SI);
        return cy.ToUnit(DefaultUnits.LengthUnitSection);
      }
    }
    public Length Cz {
      get {
        var cz = new Length(_section.Properties().Cz, UnitSystem.SI);
        return cz.ToUnit(DefaultUnits.LengthUnitSection);
      }
    }
    public Guid Guid => _guid;
    public int Id {
      get => _id;
      set {
        CloneApiObject();
        _id = value;
      }
    }
    public AreaMomentOfInertia Iuu {
      get {
        var iuu = new AreaMomentOfInertia(_section.Properties().Iuu, UnitSystem.SI);
        return iuu.ToUnit(DefaultUnits.SectionAreaMomentOfInertiaUnit);
      }
    }
    public AreaMomentOfInertia Ivv {
      get {
        var ivv = new AreaMomentOfInertia(_section.Properties().Ivv, UnitSystem.SI);
        return ivv.ToUnit(DefaultUnits.SectionAreaMomentOfInertiaUnit);
      }
    }
    public AreaMomentOfInertia Iyy {
      get {
        var iyy = new AreaMomentOfInertia(_section.Properties().Iyy, UnitSystem.SI);
        return iyy.ToUnit(DefaultUnits.SectionAreaMomentOfInertiaUnit);
      }
    }
    public AreaMomentOfInertia Iyz {
      get {
        var iyz = new AreaMomentOfInertia(_section.Properties().Iyz, UnitSystem.SI);
        return iyz.ToUnit(DefaultUnits.SectionAreaMomentOfInertiaUnit);
      }
    }
    public AreaMomentOfInertia Izz {
      get {
        var izz = new AreaMomentOfInertia(_section.Properties().Izz, UnitSystem.SI);
        return izz.ToUnit(DefaultUnits.SectionAreaMomentOfInertiaUnit);
      }
    }
    public AreaMomentOfInertia J {
      get {
        var j = new AreaMomentOfInertia(_section.Properties().J, UnitSystem.SI);
        return j.ToUnit(DefaultUnits.SectionAreaMomentOfInertiaUnit);
      }
    }
    public double Kuu => _section.Properties().Kuu;
    public double Kvv => _section.Properties().Kvv;
    public double Kyy => _section.Properties().Kyy;
    public double Kzz => _section.Properties().Kzz;
    public GsaMaterial Material {
      get => _material;
      set {
        _material = value;
        if (_section == null) {
          _section = new Section();
        } else {
          CloneApiObject();
        }

        _section.MaterialType = Materials.GetMaterialType(_material);
        if (_material.IsCustom) {
          _section.MaterialAnalysisProperty = _material.Id;
        } else {
          _section.MaterialGradeProperty = _material.Id;
        }

        IsReferencedById = false;
      }
    }
    public GsaSectionModifier Modifier {
      get => _modifier;
      set {
        CloneApiObject();
        _modifier = value;
        IsReferencedById = false;
      }
    }
    public string Name {
      get => _section.Name;
      set {
        CloneApiObject();
        _section.Name = value;
        IsReferencedById = false;
      }
    }
    public int Pool {
      get => _section.Pool;
      set {
        CloneApiObject();
        _section.Pool = value;
        IsReferencedById = false;
      }
    }
    public string Profile {
      get => _section.Profile.Replace("%", " ");
      set {
        if (!ValidProfile(value)) {
          return;
        }

        CloneApiObject();
        _section.Profile = value;
        IsReferencedById = false;
      }
    }
    public Length Ry {
      get {
        var ry = new Length(_section.Properties().Ry, UnitSystem.SI);
        return ry.ToUnit(DefaultUnits.LengthUnitSection);
      }
    }
    public Length Rz {
      get {
        var rz = new Length(_section.Properties().Rz, UnitSystem.SI);
        return rz.ToUnit(DefaultUnits.LengthUnitSection);
      }
    }
    public IQuantity SurfaceAreaPerLength {
      get {
        var area = new Area(_section.Properties().SurfaceAreaPerLength, UnitSystem.SI);
        var len = new Length(1, UnitSystem.SI);
        Area unitArea = len * len;
        return area / len;
      }
    }
    public VolumePerLength VolumePerLength
      => new VolumePerLength(_section.Properties().VolumePerLength, UnitSystem.SI);
    public SectionModulus Zpy {
      get {
        var zpy = new SectionModulus(_section.Properties().Zpy, UnitSystem.SI);
        return zpy.ToUnit(DefaultUnits.SectionModulusUnit);
      }
    }
    public SectionModulus Zpz {
      get {
        var zpz = new SectionModulus(_section.Properties().Zpz, UnitSystem.SI);
        return zpz.ToUnit(DefaultUnits.SectionModulusUnit);
      }
    }
    public SectionModulus Zy {
      get {
        var zy = new SectionModulus(_section.Properties().Zy, UnitSystem.SI);
        return zy.ToUnit(DefaultUnits.SectionModulusUnit);
      }
    }
    public SectionModulus Zz {
      get {
        var zz = new SectionModulus(_section.Properties().Zz, UnitSystem.SI);
        return zz.ToUnit(DefaultUnits.SectionModulusUnit);
      }
    }
    internal Section ApiSection {
      get => _section;
      set {
        _guid = Guid.NewGuid();
        _section = value;
        _material = Material.Duplicate();
        IsReferencedById = false;
      }
    }
    internal bool IsReferencedById { get; set; } = false;
    private Guid _guid = Guid.NewGuid();
    private int _id;
    private GsaMaterial _material = new GsaMaterial();
    private GsaSectionModifier _modifier = new GsaSectionModifier();
    private Section _section = new Section();

    public GsaSection() { }

    public GsaSection(int id) {
      _id = id;
      IsReferencedById = true;
    }

    public GsaSection(string profile) {
      _section.Profile = profile;
    }

    public GsaSection(string profile, int id = 0) {
      _section.Profile = profile;
      _id = id;
    }

    internal GsaSection(
      ReadOnlyDictionary<int, Section> sDict, int id,
      ReadOnlyDictionary<int, SectionModifier> modDict,
      ReadOnlyDictionary<int, AnalysisMaterial> matDict) : this(id) {
      if (!sDict.ContainsKey(id)) {
        return;
      }

      _section = sDict[id];
      IsReferencedById = false;
      if (modDict.ContainsKey(id)) {
        _modifier = new GsaSectionModifier(modDict[id]);
      }

      if (_section.MaterialAnalysisProperty != 0
        && matDict.ContainsKey(_section.MaterialAnalysisProperty)) {
        _material.AnalysisMaterial = matDict[_section.MaterialAnalysisProperty];
      }

      _material = Material.Duplicate();
    }

    public GsaSection Clone() {
      var dup = new GsaSection {
        _section = _section,
        _id = _id,
        _material = _material.Duplicate(),
        _modifier = _modifier.Duplicate(),
        _guid = new Guid(_guid.ToString()),
        IsReferencedById = IsReferencedById,
      };
      dup.CloneApiObject();
      return dup;
    }

    public GsaSection Duplicate() {
      return this;
    }

    public override string ToString() {
      string pb = Id > 0 ? "PB" + Id + " " : string.Empty;
      string prof = _section.Profile.Replace("%", " ") + " ";
      string mat = Mappings.materialTypeMapping
       .FirstOrDefault(x => x.Value == Material.MaterialType).Key + " ";
      string mod = _modifier.IsModified ? " modified" : string.Empty;
      return string.Join(" ", pb.Trim(), prof.Trim(), mat.Trim(), mod.Trim()).Trim()
       .Replace("  ", " ");
    }

    internal static bool ValidProfile(string profile) {
      var test = new Section {
        Profile = profile,
      };
      return test.Properties().Area != 0;
    }

    private void CloneApiObject() {
      // temp profile clone
      string prfl = _section.Profile.Replace("%", " ");
      string[] pfs = prfl.Split(' ');
      if (pfs.Last() == "S/S") {
        prfl = string.Join(" ", pfs[0], pfs[1], pfs[2]);
      }

      var sec = new Section() {
        MaterialAnalysisProperty = _section.MaterialAnalysisProperty,
        MaterialGradeProperty = _section.MaterialGradeProperty,
        MaterialType = _section.MaterialType,
        Name = _section.Name.ToString(),
        BasicOffset = _section.BasicOffset,
        AdditionalOffsetY = _section.AdditionalOffsetY,
        AdditionalOffsetZ = _section.AdditionalOffsetZ,
        Pool = _section.Pool,
        Profile = prfl,
      };
      if ((Color)_section.Colour
        != Color.FromArgb(0, 0,
          0)) // workaround to handle that System.Drawing.Color is non-nullable type
      {
        sec.Colour = _section.Colour;
      }

      _section = sec;
      _modifier = Modifier.Duplicate(true);
      _guid = Guid.NewGuid();
    }
  }
}
