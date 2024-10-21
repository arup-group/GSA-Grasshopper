using GsaAPI;

using OasysGH.Units;

using OasysUnits;

using AngleUnit = OasysUnits.Units.AngleUnit;

namespace GsaGH.Parameters {
  /// <summary>
  /// This class puts a UnitsNet / OasysUnits wrapper around <see cref="SectionProperties"/>
  /// </summary>
  public class GsaSectionProperties {
    public Angle Angle {
      get {
        var angle = new Angle(_sectionProperties.Angle, AngleUnit.Degree);
        return angle.ToUnit(DefaultUnits.AngleUnit);
      }
    }
    public Area Area {
      get {
        var area = new Area(_sectionProperties.Area, UnitSystem.SI);
        return area.ToUnit(DefaultUnits.SectionAreaUnit);
      }
    }
    public SectionModulus C {
      get {
        var c = new SectionModulus(_sectionProperties.C, UnitSystem.SI);
        return c.ToUnit(DefaultUnits.SectionModulusUnit);
      }
    }
    public Length Cy {
      get {
        var cy = new Length(_sectionProperties.Cy, UnitSystem.SI);
        return cy.ToUnit(DefaultUnits.LengthUnitSection);
      }
    }
    public Length Cz {
      get {
        var cz = new Length(_sectionProperties.Cz, UnitSystem.SI);
        return cz.ToUnit(DefaultUnits.LengthUnitSection);
      }
    }
    public AreaMomentOfInertia Iuu {
      get {
        var iuu = new AreaMomentOfInertia(_sectionProperties.Iuu, UnitSystem.SI);
        return iuu.ToUnit(DefaultUnits.SectionAreaMomentOfInertiaUnit);
      }
    }
    public AreaMomentOfInertia Ivv {
      get {
        var ivv = new AreaMomentOfInertia(_sectionProperties.Ivv, UnitSystem.SI);
        return ivv.ToUnit(DefaultUnits.SectionAreaMomentOfInertiaUnit);
      }
    }
    public AreaMomentOfInertia Iyy {
      get {
        var iyy = new AreaMomentOfInertia(_sectionProperties.Iyy, UnitSystem.SI);
        return iyy.ToUnit(DefaultUnits.SectionAreaMomentOfInertiaUnit);
      }
    }
    public AreaMomentOfInertia Iyz {
      get {
        var iyz = new AreaMomentOfInertia(_sectionProperties.Iyz, UnitSystem.SI);
        return iyz.ToUnit(DefaultUnits.SectionAreaMomentOfInertiaUnit);
      }
    }
    public AreaMomentOfInertia Izz {
      get {
        var izz = new AreaMomentOfInertia(_sectionProperties.Izz, UnitSystem.SI);
        return izz.ToUnit(DefaultUnits.SectionAreaMomentOfInertiaUnit);
      }
    }
    public AreaMomentOfInertia J {
      get {
        var j = new AreaMomentOfInertia(_sectionProperties.J, UnitSystem.SI);
        return j.ToUnit(DefaultUnits.SectionAreaMomentOfInertiaUnit);
      }
    }
    public double Kuu => _sectionProperties.Kuu;
    public double Kvv => _sectionProperties.Kvv;
    public double Kyy => _sectionProperties.Kyy;
    public double Kzz => _sectionProperties.Kzz;

    public Length Ry {
      get {
        var ry = new Length(_sectionProperties.Ry, UnitSystem.SI);
        return ry.ToUnit(DefaultUnits.LengthUnitSection);
      }
    }
    public Length Rz {
      get {
        var rz = new Length(_sectionProperties.Rz, UnitSystem.SI);
        return rz.ToUnit(DefaultUnits.LengthUnitSection);
      }
    }
    public IQuantity SurfaceAreaPerLength {
      get {
        var area = new Area(_sectionProperties.SurfaceAreaPerLength, UnitSystem.SI);
        var len = new Length(1, UnitSystem.SI);
        Area unitArea = len * len;
        return area / len;
      }
    }
    public VolumePerLength VolumePerLength
      => new VolumePerLength(_sectionProperties.VolumePerLength, UnitSystem.SI);
    public SectionModulus Zpy {
      get {
        var zpy = new SectionModulus(_sectionProperties.Zpy, UnitSystem.SI);
        return zpy.ToUnit(DefaultUnits.SectionModulusUnit);
      }
    }
    public SectionModulus Zpz {
      get {
        var zpz = new SectionModulus(_sectionProperties.Zpz, UnitSystem.SI);
        return zpz.ToUnit(DefaultUnits.SectionModulusUnit);
      }
    }
    public SectionModulus Zy {
      get {
        var zy = new SectionModulus(_sectionProperties.Zy, UnitSystem.SI);
        return zy.ToUnit(DefaultUnits.SectionModulusUnit);
      }
    }
    public SectionModulus Zz {
      get {
        var zz = new SectionModulus(_sectionProperties.Zz, UnitSystem.SI);
        return zz.ToUnit(DefaultUnits.SectionModulusUnit);
      }
    }

    private SectionProperties _sectionProperties;

    internal GsaSectionProperties(SectionProperties properties) {
      _sectionProperties = properties;
    }
  }
}
