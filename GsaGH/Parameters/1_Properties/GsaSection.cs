using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using GsaAPI;
using GsaGH.Helpers.GsaApi;
using OasysGH.Units;
using OasysUnits;

namespace GsaGH.Parameters
{
  /// <summary>
  /// Section class, this class defines the basic properties and methods for any <see cref="GsaAPI.Section"/>
  /// </summary>
  public class GsaSection
  {
    #region fields
    private int _id = 0;
    private Guid _guid = Guid.NewGuid();
    private GsaMaterial _material = new GsaMaterial();
    private GsaSectionModifier _modifier = new GsaSectionModifier();
    private Section _section = new Section();
    #endregion

    #region properties
    internal Section API_Section
    {
      get
      {
        return this._section;
      }
      set
      {
        this._guid = Guid.NewGuid();
        this._section = value;
        this._material = new GsaMaterial(this);
        this.IsReferencedByID = false;
      }
    }
    #region section properties
    public Area Area
    {
      get
      {
        Area area = new Area(this._section.Area, UnitSystem.SI);
        return new Area(area.As(DefaultUnits.SectionAreaUnit), DefaultUnits.SectionAreaUnit);
      }
    }
    public AreaMomentOfInertia Iyy
    {
      get
      {
        AreaMomentOfInertia inertia = new AreaMomentOfInertia(this._section.Iyy, UnitSystem.SI);
        return new AreaMomentOfInertia(inertia.As(DefaultUnits.SectionAreaMomentOfInertiaUnit), DefaultUnits.SectionAreaMomentOfInertiaUnit);
      }
    }
    public AreaMomentOfInertia Iyz
    {
      get
      {
        AreaMomentOfInertia inertia = new AreaMomentOfInertia(this._section.Iyz, UnitSystem.SI);
        return new AreaMomentOfInertia(inertia.As(DefaultUnits.SectionAreaMomentOfInertiaUnit), DefaultUnits.SectionAreaMomentOfInertiaUnit);
      }
    }
    public AreaMomentOfInertia Izz
    {
      get
      {
        AreaMomentOfInertia inertia = new AreaMomentOfInertia(this._section.Izz, UnitSystem.SI);
        return new AreaMomentOfInertia(inertia.As(DefaultUnits.SectionAreaMomentOfInertiaUnit), DefaultUnits.SectionAreaMomentOfInertiaUnit);
      }
    }
    public AreaMomentOfInertia J
    {
      get
      {
        AreaMomentOfInertia inertia = new AreaMomentOfInertia(this._section.J, UnitSystem.SI);
        return new AreaMomentOfInertia(inertia.As(DefaultUnits.SectionAreaMomentOfInertiaUnit), DefaultUnits.SectionAreaMomentOfInertiaUnit);
      }
    }
    public double Ky
    {
      get
      {
        return this._section.Ky;
      }
    }
    public double Kz
    {
      get
      {
        return this._section.Kz;
      }
    }
    public IQuantity SurfaceAreaPerLength
    {
      get
      {
        Area area = new Area(this._section.SurfaceAreaPerLength, UnitSystem.SI);
        Length len = new Length(1, DefaultUnits.LengthUnitSection);
        Area unitArea = len * len;
        Area areaOut = new Area(area.As(unitArea.Unit), unitArea.Unit);
        return areaOut / len;
      }
    }
    public VolumePerLength VolumePerLength
    {
      get
      {
        return new VolumePerLength(this._section.VolumePerLength, UnitSystem.SI);
      }
    }
    #endregion
    public int Id
    {
      get
      {
        return this._id;
      }
      set
      {
        this.CloneApiObject();
        this._id = value;
      }
    }
    internal bool IsReferencedByID { get; set; } = false;
    public GsaMaterial Material
    {
      get
      {
        return this._material;
      }
      set
      {
        this._material = value;
        if (this._section == null)
          this._section = new Section();
        else
          this.CloneApiObject();

        this._section.MaterialType = Helpers.Export.Materials.ConvertType(_material);
        this._section.MaterialAnalysisProperty = _material.AnalysisProperty;
        this._section.MaterialGradeProperty = _material.GradeProperty;
        this.IsReferencedByID = false;
      }
    }
    public GsaSectionModifier Modifier
    {
      get
      {
        return this._modifier;
      }
      set
      {
        this.CloneApiObject();
        this._modifier = value;
        this.IsReferencedByID = false;
      }
    }
    #region GsaAPI members
    public string Name
    {
      get
      {
        return _section.Name;
      }
      set
      {
        this.CloneApiObject();
        this._section.Name = value;
        this.IsReferencedByID = false;
      }
    }
    public int Pool
    {
      get
      {
        return this._section.Pool;
      }
      set
      {
        this.CloneApiObject();
        this._section.Pool = value;
        this.IsReferencedByID = false;
      }
    }
    public int MaterialID
    {
      get
      {
        return this._section.MaterialAnalysisProperty;
      }
      set
      {
        this.CloneApiObject();
        this._section.MaterialAnalysisProperty = value;
        this._material.AnalysisProperty = this._section.MaterialAnalysisProperty;
        this.IsReferencedByID = false;
      }
    }
    public string Profile
    {
      get
      {
        return this._section.Profile.Replace("%", " ");
      }
      set
      {
        if (ValidProfile(value))
        {
          this.CloneApiObject();
          this._section.Profile = value;
          this.IsReferencedByID = false;
        }
      }
    }
    public Color Colour
    {
      get
      {
        return (Color)this._section.Colour;
      }
      set
      {
        this.CloneApiObject();
        this._section.Colour = value;
        this.IsReferencedByID = false;
      }
    }
    private void CloneApiObject()
    {
      // temp profile clone 
      string prfl = this._section.Profile.Replace("%", " ");
      string[] pfs = prfl.Split(' ');
      if (pfs.Last() == "S/S")
        prfl = string.Join(" ", pfs[0], pfs[1], pfs[2]);

      Section sec = new Section()
      {
        MaterialAnalysisProperty = this._section.MaterialAnalysisProperty,
        MaterialGradeProperty = this._section.MaterialGradeProperty,
        MaterialType = this._section.MaterialType,
        Name = this._section.Name.ToString(),
        Pool = this._section.Pool,
        Profile = prfl
        //Profile = this._section.Profile
      };
      if ((Color)_section.Colour != Color.FromArgb(0, 0, 0)) // workaround to handle that System.Drawing.Color is non-nullable type
        sec.Colour = this._section.Colour;

      this._section = sec;
      this._modifier = this.Modifier.Duplicate(true);
      this._guid = Guid.NewGuid();
    }
    #endregion
    public Guid Guid
    {
      get
      {
        return this._guid;
      }
    }
    #endregion

    #region constructors
    public GsaSection()
    {
    }

    public GsaSection(int id)
    {
      this._id = id;
      this.IsReferencedByID = true;
    }

    public GsaSection(string profile)
    {
      this._section.Profile = profile;
    }

    public GsaSection(string profile, int id = 0)
    {
      this._section.Profile = profile;
      this._id = id;
    }

    internal GsaSection(ReadOnlyDictionary<int, Section> sDict, int id, ReadOnlyDictionary<int, SectionModifier> modDict, ReadOnlyDictionary<int, AnalysisMaterial> matDict) : this(id)
    {
      if (!sDict.ContainsKey(id))
        return;
      this._section = sDict[id];
      this.IsReferencedByID = false;
      // modifier
      if (modDict.ContainsKey(id))
        this._modifier = new GsaSectionModifier(modDict[id]);
      // material
      if (this._section.MaterialAnalysisProperty != 0 && matDict.ContainsKey(this._section.MaterialAnalysisProperty))
        this._material.AnalysisMaterial = matDict[this._section.MaterialAnalysisProperty];
      this._material = new GsaMaterial(this);
    }
    #endregion

    #region methods
    public GsaSection Duplicate(bool cloneApiElement = false)
    {
      GsaSection dup = new GsaSection
      {
        _section = this._section,
        _id = this._id,
        _material = this._material.Duplicate(),
        _modifier = this._modifier.Duplicate(cloneApiElement),
        _guid = new Guid(this._guid.ToString()),
        IsReferencedByID = this.IsReferencedByID
      };
      if (cloneApiElement)
        dup.CloneApiObject();
      return dup;
    }

    public override string ToString()
    {
      string pb = this.Id > 0 ? "PB" + this.Id + " " : "";
      string prof = this._section.Profile.Replace("%", " ") + " ";
      string mat = Mappings.MaterialTypeMapping.FirstOrDefault(x => x.Value == this.Material.MaterialType).Key + " ";
      string mod = this._modifier.IsModified ? " modified" : "";
      return string.Join(" ", pb.Trim(), prof.Trim(), mat.Trim(), mod.Trim()).Trim().Replace("  ", " ");
    }

    internal static bool ValidProfile(string profile)
    {
      Section test = new Section();
      test.Profile = profile;
      if (test.Area == 0)
        return false;
      else
        return true;
    }
    #endregion
  }
}
