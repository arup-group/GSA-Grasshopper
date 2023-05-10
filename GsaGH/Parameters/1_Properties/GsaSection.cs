﻿using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using GsaAPI;
using GsaGH.Helpers.Export;
using GsaGH.Helpers.GsaApi;
using OasysGH.Units;
using OasysUnits;

namespace GsaGH.Parameters {
  /// <summary>
  ///   Section class, this class defines the basic properties and methods for any <see cref="GsaAPI.Section" />
  /// </summary>
  public class GsaSection {
    public Area Area {
      get {
        var area = new Area(_section.Area, UnitSystem.SI);
        return new Area(area.As(DefaultUnits.SectionAreaUnit), DefaultUnits.SectionAreaUnit);
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
    public Guid Guid => _guid;
    public int Id {
      get => _id;
      set {
        CloneApiObject();
        _id = value;
      }
    }
    public AreaMomentOfInertia Iyy {
      get {
        var inertia = new AreaMomentOfInertia(_section.Iyy, UnitSystem.SI);
        return new AreaMomentOfInertia(inertia.As(DefaultUnits.SectionAreaMomentOfInertiaUnit),
          DefaultUnits.SectionAreaMomentOfInertiaUnit);
      }
    }
    public AreaMomentOfInertia Iyz {
      get {
        var inertia = new AreaMomentOfInertia(_section.Iyz, UnitSystem.SI);
        return new AreaMomentOfInertia(inertia.As(DefaultUnits.SectionAreaMomentOfInertiaUnit),
          DefaultUnits.SectionAreaMomentOfInertiaUnit);
      }
    }
    public AreaMomentOfInertia Izz {
      get {
        var inertia = new AreaMomentOfInertia(_section.Izz, UnitSystem.SI);
        return new AreaMomentOfInertia(inertia.As(DefaultUnits.SectionAreaMomentOfInertiaUnit),
          DefaultUnits.SectionAreaMomentOfInertiaUnit);
      }
    }
    public AreaMomentOfInertia J {
      get {
        var inertia = new AreaMomentOfInertia(_section.J, UnitSystem.SI);
        return new AreaMomentOfInertia(inertia.As(DefaultUnits.SectionAreaMomentOfInertiaUnit),
          DefaultUnits.SectionAreaMomentOfInertiaUnit);
      }
    }
    public double Ky => _section.Ky;
    public double Kz => _section.Kz;
    public IGsaMaterial Material {
      get => _material;
      set {
        _material = value.Duplicate();
        if (_section == null) {
          _section = new Section();
        } else {
          CloneApiObject();
        }

        _section.MaterialType = _material.Type;
        _section.MaterialAnalysisProperty = 0;
        _section.MaterialGradeProperty = 0;
        IsReferencedById = false;
      }
    }
    public int MaterialId {
      get => _section.MaterialAnalysisProperty;
      set {
        CloneApiObject();
        _section.MaterialAnalysisProperty = value;
        _material.Id = _section.MaterialAnalysisProperty;
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
    public IQuantity SurfaceAreaPerLength {
      get {
        var area = new Area(_section.SurfaceAreaPerLength, UnitSystem.SI);
        var len = new Length(1, DefaultUnits.LengthUnitSection);
        Area unitArea = len * len;
        var areaOut = new Area(area.As(unitArea.Unit), unitArea.Unit);
        return areaOut / len;
      }
    }
    public VolumePerLength VolumePerLength
      => new VolumePerLength(_section.VolumePerLength, UnitSystem.SI);
    internal Section ApiSection {
      get => _section;
      set {
        _guid = Guid.NewGuid();
        _section = value;
        _material = null;
        IsReferencedById = false;
      }
    }
    internal bool IsReferencedById { get; set; } = false;
    private Guid _guid = Guid.NewGuid();
    private int _id;
    private IGsaMaterial _material;
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
      Helpers.Import.Materials matDict) : this(id) {
      if (!sDict.ContainsKey(id)) {
        return;
      }

      _section = sDict[id];
      IsReferencedById = false;
      if (modDict.ContainsKey(id)) {
        _modifier = new GsaSectionModifier(modDict[id]);
      }

      _material = matDict.GetMaterial(this);
    }

    public GsaSection Duplicate(bool cloneApiElement = false) {
      var dup = new GsaSection {
        _section = _section,
        _id = _id,
        _material = _material.Duplicate(),
        _modifier = _modifier.Duplicate(cloneApiElement),
        _guid = new Guid(_guid.ToString()),
        IsReferencedById = IsReferencedById,
      };
      if (cloneApiElement) {
        dup.CloneApiObject();
      }

      return dup;
    }

    public override string ToString() {
      string pb = Id > 0 ? "PB" + Id + " " : "";
      string prof = _section.Profile.Replace("%", " ") + " ";
      string mat = Mappings.materialTypeMapping
       .FirstOrDefault(x => x.Value == Material.Type).Key + " ";
      string mod = _modifier.IsModified ? " modified" : "";
      return string.Join(" ", pb.Trim(), prof.Trim(), mat.Trim(), mod.Trim()).Trim()
       .Replace("  ", " ");
    }

    internal static bool ValidProfile(string profile) {
      var test = new Section {
        Profile = profile,
      };
      return test.Area != 0;
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
