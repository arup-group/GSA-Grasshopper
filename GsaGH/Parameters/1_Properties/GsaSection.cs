using System;
using System.Drawing;
using GsaAPI;
using OasysGH.Units;
using OasysUnits;

namespace GsaGH.Parameters
{
  /// <summary>
  /// Section class, this class defines the basic properties and methods for any Gsa Section
  /// </summary>
  public class GsaSection
  {
    #region fields
    private int _id = 0;
    private Guid _guid;
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
    public int ID
    {
      get
      {
        return this._id;
      }
      set
      {
        this._guid = Guid.NewGuid();
        this._id = value;
      }
    }
    public GsaMaterial Material
    {
      get
      {
        return this._material;
      }
      set
      {
        this.CloneSection();
        this._section.MaterialType = Util.Gsa.ToGSA.Materials.ConvertType(_material);
        this._section.MaterialAnalysisProperty = _material.AnalysisProperty;
        this._section.MaterialGradeProperty = _material.GradeProperty;
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
        this._modifier = value;
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
        this._guid = Guid.NewGuid();
        this.CloneSection();
        this._section.Name = value;
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
        this._guid = Guid.NewGuid();
        this.CloneSection();
        this._section.Pool = value;
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
        this._guid = Guid.NewGuid();
        this.CloneSection();
        this._section.MaterialAnalysisProperty = value;
        this._material.AnalysisProperty = this._section.MaterialAnalysisProperty;
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
        this._guid = Guid.NewGuid();
        this.CloneSection();
        this._section.Profile = value;
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
        this._guid = Guid.NewGuid();
        this.CloneSection();
        this._section.Colour = value;
      }
    }
    private void CloneSection()
    {
      Section sec = new Section()
      {
        MaterialAnalysisProperty = this._section.MaterialAnalysisProperty,
        MaterialGradeProperty = this._section.MaterialGradeProperty,
        MaterialType = this._section.MaterialType,
        Name = this._section.Name.ToString(),
        Pool = this._section.Pool,
        Profile = this._section.Profile.ToString()
      };
      if ((Color)_section.Colour != Color.FromArgb(0, 0, 0)) // workaround to handle that System.Drawing.Color is non-nullable type
        sec.Colour = this._section.Colour;

      this._section = sec;
      this._guid = Guid.NewGuid();
    }
    #endregion
    public Guid GUID
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
    #endregion

    #region methods
    public GsaSection Duplicate()
    {
      GsaSection dup = new GsaSection();
      dup._section = this._section;
      dup._id = this._id;
      dup._material = this._material.Duplicate();
      dup._modifier = this._modifier.Duplicate();
      dup._guid = new Guid(_guid.ToString());
      return dup;
    }

    public override string ToString()
    {
      string prof = this._section.Profile.Replace("%", " ");
      string pb = "PB" + this.ID + " ";
      string mod = this._modifier.IsModified ? " modified" : "";
      return pb + prof + mod;
    }
    #endregion
  }
}
