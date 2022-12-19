using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GsaAPI;
using GsaGH.Parameters;
using OasysUnits;
using OasysUnits.Units;
using Rhino.Geometry;

namespace GsaGH.Helpers.GsaAPI
{
  internal partial class ResultHelper
  {
    internal static Vector3d GetResult(Double6 result, ForceUnit unit)
    {
      double x = new Force(result.X, ForceUnit.Newton).As(unit);
      double y = new Force(result.Y, ForceUnit.Newton).As(unit);
      double z = new Force(result.Z, ForceUnit.Newton).As(unit);
      return new Vector3d(x, y, z);
    }
    internal static Vector3d GetResult(Double6 result, MomentUnit unit)
    {
      double xx = new Moment(result.XX, MomentUnit.NewtonMeter).As(unit);
      double yy = new Moment(result.YY, MomentUnit.NewtonMeter).As(unit);
      double zz = new Moment(result.ZZ, MomentUnit.NewtonMeter).As(unit);
      return new Vector3d(xx, yy, zz);
    }
    internal static Vector3d GetResult(Double6 result, LengthUnit unit)
    {
      double x = new Length(result.X, LengthUnit.Meter).As(unit);
      double y = new Length(result.Y, LengthUnit.Meter).As(unit);
      double z = new Length(result.Z, LengthUnit.Meter).As(unit);
      return new Vector3d(x, y, z);
    }
    internal static Vector3d GetResult(Double3 result, LengthUnit unit)
    {
      double x = new Length(result.X, LengthUnit.Meter).As(unit);
      double y = new Length(result.Y, LengthUnit.Meter).As(unit);
      double z = new Length(result.Z, LengthUnit.Meter).As(unit);
      return new Vector3d(x, y, z);
    }
    internal static Vector3d GetResult(Double6 result, AngleUnit unit)
    {
      double xx = 0;
      if (!double.IsNaN(result.XX))
        new Angle(result.XX, AngleUnit.Radian).As(unit);
      double yy = 0;
      if (!double.IsNaN(result.YY))
        new Angle(result.YY, AngleUnit.Radian).As(unit);
      double zz = 0;
      if (!double.IsNaN(result.ZZ))
        new Angle(result.ZZ, AngleUnit.Radian).As(unit);
      return new Vector3d(xx, yy, zz);
    }
    internal static Vector3d GetResult(Double6 result, PressureUnit unit, bool shear = false)
    {
      if (shear)
      {
        double xx = new Pressure(result.XX, PressureUnit.Pascal).As(unit);
        double yy = new Pressure(result.YY, PressureUnit.Pascal).As(unit);
        double zz = new Pressure(result.ZZ, PressureUnit.Pascal).As(unit);
        return new Vector3d(xx, yy, zz);
      }
      else
      {
        double x = new Pressure(result.X, PressureUnit.Pascal).As(unit);
        double y = new Pressure(result.Y, PressureUnit.Pascal).As(unit);
        double z = new Pressure(result.Z, PressureUnit.Pascal).As(unit);
        return new Vector3d(x, y, z);
      }
    }
    internal static Vector3d GetResult(Tensor2 result, ForceUnit unit)
    {
      double x = new Force(result.XX, ForceUnit.Newton).As(unit);
      double y = new Force(result.YY, ForceUnit.Newton).As(unit);
      double z = new Force(result.XY, ForceUnit.Newton).As(unit);
      return new Vector3d(x, y, z);
    }
    internal static Vector3d GetResult(Tensor2 result, MomentUnit unit)
    {
      double x = new Moment(result.XX, MomentUnit.NewtonMeter).As(unit);
      double y = new Moment(result.YY, MomentUnit.NewtonMeter).As(unit);
      double z = new Moment(result.XY, MomentUnit.NewtonMeter).As(unit);
      return new Vector3d(x, y, z);
    }
    internal static Vector3d GetResult(Vector2 result, ForceUnit unit)
    {
      double x = new Force(result.X, ForceUnit.Newton).As(unit);
      double y = new Force(result.Y, ForceUnit.Newton).As(unit);
      double z = 0;
      return new Vector3d(x, y, z);
    }
    internal static Vector3d GetResult(Tensor3 result, PressureUnit unit, bool shear = false)
    {
      if (shear)
      {
        double xy = new Pressure(result.XY, PressureUnit.Pascal).As(unit);
        double yz = new Pressure(result.YZ, PressureUnit.Pascal).As(unit);
        double zx = new Pressure(result.ZX, PressureUnit.Pascal).As(unit);
        return new Vector3d(xy, yz, zx);
      }
      else
      {
        double xx = new Pressure(result.XX, PressureUnit.Pascal).As(unit);
        double yy = new Pressure(result.YY, PressureUnit.Pascal).As(unit);
        double zz = new Pressure(result.ZZ, PressureUnit.Pascal).As(unit);
        return new Vector3d(xx, yy, zz);
      }
    }
    internal static Vector3d GetResult(Vector3 result, MassUnit unit)
    {
      double x = new Mass(result.X, MassUnit.Kilogram).As(unit);
      double y = new Mass(result.Y, MassUnit.Kilogram).As(unit);
      double z = new Mass(result.Z, MassUnit.Kilogram).As(unit);
      return new Vector3d(x, y, z);
    }
    internal static Vector3d GetResult(Vector3 result, AreaMomentOfInertiaUnit unit)
    {
      double x = new AreaMomentOfInertia(result.X, AreaMomentOfInertiaUnit.MeterToTheFourth).As(unit);
      double y = new AreaMomentOfInertia(result.Y, AreaMomentOfInertiaUnit.MeterToTheFourth).As(unit);
      double z = new AreaMomentOfInertia(result.Z, AreaMomentOfInertiaUnit.MeterToTheFourth).As(unit);
      return new Vector3d(x, y, z);
    }
  }
}
