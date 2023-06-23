using System;
using GsaAPI;
using GsaGH.Parameters;
using OasysUnits;
using OasysUnits.Units;
using AngleUnit = OasysUnits.Units.AngleUnit;
using EnergyUnit = OasysUnits.Units.EnergyUnit;
using ForceUnit = OasysUnits.Units.ForceUnit;
using LengthUnit = OasysUnits.Units.LengthUnit;
using MassUnit = OasysUnits.Units.MassUnit;

namespace GsaGH.Helpers.GsaApi {
  internal partial class ResultHelper {

    internal static GsaResultQuantity GetQuantityResult(
      Double6 result, ForceUnit unit, bool isBeam = false) {
      IQuantity x = new Force(new Force(result.X, ForceUnit.Newton).As(unit), unit);
      IQuantity y = new Force(new Force(result.Y, ForceUnit.Newton).As(unit), unit);
      IQuantity z = new Force(new Force(result.Z, ForceUnit.Newton).As(unit), unit);
      double pyth = isBeam ? Math.Sqrt(Math.Pow(result.Y, 2) + Math.Pow(result.Z, 2)) :
        Math.Sqrt(Math.Pow(result.X, 2) + Math.Pow(result.Y, 2) + Math.Pow(result.Z, 2));
      IQuantity xyz = new Force(new Force(pyth, ForceUnit.Newton).As(unit), unit);
      return new GsaResultQuantity() {
        X = x,
        Y = y,
        Z = z,
        Xyz = xyz,
      };
    }

    internal static GsaResultQuantity GetQuantityResult(double result, EnergyUnit unit) {
      IQuantity x = new Energy(new Energy(result, EnergyUnit.Joule).As(unit), unit);

      return new GsaResultQuantity() {
        X = x,
      };
    }

    internal static GsaResultQuantity GetQuantityResult(
      Double6 result, MomentUnit unit, bool isBeam = false) {
      IQuantity xx = new Moment(new Moment(result.XX, MomentUnit.NewtonMeter).As(unit), unit);
      IQuantity yy = new Moment(new Moment(result.YY, MomentUnit.NewtonMeter).As(unit), unit);
      IQuantity zz = new Moment(new Moment(result.ZZ, MomentUnit.NewtonMeter).As(unit), unit);
      double pyth = isBeam ? Math.Sqrt(Math.Pow(result.YY, 2) + Math.Pow(result.ZZ, 2)) :
        Math.Sqrt(Math.Pow(result.XX, 2) + Math.Pow(result.YY, 2) + Math.Pow(result.ZZ, 2));
      IQuantity xxyyzz = new Moment(new Moment(pyth, MomentUnit.NewtonMeter).As(unit), unit);
      return new GsaResultQuantity() {
        X = xx,
        Y = yy,
        Z = zz,
        Xyz = xxyyzz,
      };
    }

    internal static GsaResultQuantity GetQuantityResult(Double6 result, LengthUnit unit) {
      IQuantity x = new Length(new Length(result.X, LengthUnit.Meter).As(unit), unit);
      IQuantity y = new Length(new Length(result.Y, LengthUnit.Meter).As(unit), unit);
      IQuantity z = new Length(new Length(result.Z, LengthUnit.Meter).As(unit), unit);
      double pyth
        = Math.Sqrt(Math.Pow(result.X, 2) + Math.Pow(result.Y, 2) + Math.Pow(result.Z, 2));
      IQuantity xyz = new Length(new Length(pyth, LengthUnit.Meter).As(unit), unit);
      return new GsaResultQuantity() {
        X = x,
        Y = y,
        Z = z,
        Xyz = xyz,
      };
    }

    internal static GsaResultQuantity GetQuantityResult(Double3 result, LengthUnit unit) {
      IQuantity x = new Length(new Length(result.X, LengthUnit.Meter).As(unit), unit);
      IQuantity y = new Length(new Length(result.Y, LengthUnit.Meter).As(unit), unit);
      IQuantity z = new Length(new Length(result.Z, LengthUnit.Meter).As(unit), unit);
      double pyth
        = Math.Sqrt(Math.Pow(result.X, 2) + Math.Pow(result.Y, 2) + Math.Pow(result.Z, 2));
      IQuantity xyz = new Length(new Length(pyth, LengthUnit.Meter).As(unit), unit);
      return new GsaResultQuantity() {
        X = x,
        Y = y,
        Z = z,
        Xyz = xyz,
      };
    }

    internal static GsaResultQuantity GetQuantityResult(Double6 result, AngleUnit unit) {
      IQuantity x;
      if (
        !double.IsNaN(result
         .XX)) // TO-DO: GSA-5351 remove NaN and Infinity values from GsaAPI results
      {
        x = !double.IsInfinity(result.XX) ?
          new Angle(new Angle(result.XX, AngleUnit.Radian).As(unit), unit) :
          (IQuantity)(double.IsPositiveInfinity(result.XX) ? new Angle(360, AngleUnit.Degree) :
            new Angle(-360, AngleUnit.Degree));
      } else {
        x = new Angle(0, unit);
      }

      IQuantity y;
      if (!double.IsNaN(result.YY)) {
        y = !double.IsInfinity(result.YY) ?
          new Angle(new Angle(result.YY, AngleUnit.Radian).As(unit), unit) :
          (IQuantity)(double.IsPositiveInfinity(result.YY) ? new Angle(360, AngleUnit.Degree) :
            new Angle(-360, AngleUnit.Degree));
      } else {
        y = new Angle(0, unit);
      }

      IQuantity z;
      if (!double.IsNaN(result.ZZ)) {
        z = !double.IsInfinity(result.ZZ) ?
          new Angle(new Angle(result.ZZ, AngleUnit.Radian).As(unit), unit) :
          (IQuantity)(double.IsPositiveInfinity(result.ZZ) ? new Angle(360, AngleUnit.Degree) :
            new Angle(-360, AngleUnit.Degree));
      } else {
        z = new Angle(0, unit);
      }

      double pyth = Math.Sqrt(Math.Pow(x.Value, 2) + Math.Pow(y.Value, 2) + Math.Pow(z.Value, 2));
      IQuantity xyz = double.IsInfinity(pyth) ? new Angle(360, AngleUnit.Degree) :
        new Angle(new Angle(pyth, AngleUnit.Radian).As(unit), unit);
      return new GsaResultQuantity() {
        X = x,
        Y = y,
        Z = z,
        Xyz = xyz,
      };
    }

    internal static GsaResultQuantity GetQuantityResult(
      Double6 result, PressureUnit unit, bool shear = false) {
      if (shear) {
        IQuantity x = new Pressure(new Pressure(result.XX, PressureUnit.Pascal).As(unit), unit);
        IQuantity y = new Pressure(new Pressure(result.YY, PressureUnit.Pascal).As(unit), unit);
        IQuantity z = new Pressure(new Pressure(result.ZZ, PressureUnit.Pascal).As(unit), unit);
        double pyth
          = Math.Sqrt(Math.Pow(result.XX, 2) + Math.Pow(result.YY, 2) + Math.Pow(result.ZZ, 2));
        IQuantity xyz = new Pressure(new Pressure(pyth, PressureUnit.Pascal).As(unit), unit);
        return new GsaResultQuantity() {
          X = x,
          Y = y,
          Z = z,
          Xyz = xyz,
        };
      } else {
        IQuantity x = new Pressure(new Pressure(result.X, PressureUnit.Pascal).As(unit), unit);
        IQuantity y = new Pressure(new Pressure(result.Y, PressureUnit.Pascal).As(unit), unit);
        IQuantity z = new Pressure(new Pressure(result.Z, PressureUnit.Pascal).As(unit), unit);
        double pyth
          = Math.Sqrt(Math.Pow(result.X, 2) + Math.Pow(result.Y, 2) + Math.Pow(result.Z, 2));
        IQuantity xyz = new Pressure(new Pressure(pyth, PressureUnit.Pascal).As(unit), unit);
        return new GsaResultQuantity() {
          X = x,
          Y = y,
          Z = z,
          Xyz = xyz,
        };
      }
    }

    internal static GsaResultQuantity GetQuantityResult(Tensor2 result, ForcePerLengthUnit unit) {
      IQuantity x
        = new ForcePerLength(
          new ForcePerLength(result.XX, ForcePerLengthUnit.NewtonPerMeter).As(unit), unit);
      IQuantity y
        = new ForcePerLength(
          new ForcePerLength(result.YY, ForcePerLengthUnit.NewtonPerMeter).As(unit), unit);
      IQuantity z
        = new ForcePerLength(
          new ForcePerLength(result.XY, ForcePerLengthUnit.NewtonPerMeter).As(unit), unit);
      return new GsaResultQuantity() {
        X = x,
        Y = y,
        Z = z,
        Xyz = null,
      };
    }

    internal static GsaResultQuantity GetQuantityResult(Tensor2 result, ForceUnit unit) {
      IQuantity xx = new Force(new Force(result.XX, ForceUnit.Newton).As(unit), unit);
      IQuantity yy = new Force(new Force(result.YY, ForceUnit.Newton).As(unit), unit);
      IQuantity zz = new Force(new Force(result.XY, ForceUnit.Newton).As(unit), unit);
      return new GsaResultQuantity() {
        X = xx,
        Y = yy,
        Z = zz,
        Xyz = null,
      };
    }

    internal static GsaResultQuantity GetQuantityResult(Vector2 result, ForcePerLengthUnit unit) {
      IQuantity x
        = new ForcePerLength(
          new ForcePerLength(result.X, ForcePerLengthUnit.NewtonPerMeter).As(unit), unit);
      IQuantity y
        = new ForcePerLength(
          new ForcePerLength(result.Y, ForcePerLengthUnit.NewtonPerMeter).As(unit), unit);
      return new GsaResultQuantity() {
        X = x,
        Y = y,
        Z = null,
        Xyz = null,
      };
    }

    internal static GsaResultQuantity GetQuantityResult(
      Tensor3 result, PressureUnit unit, bool shear = false) {
      if (shear) {
        IQuantity x = new Pressure(new Pressure(result.XY, PressureUnit.Pascal).As(unit), unit);
        IQuantity y = new Pressure(new Pressure(result.YZ, PressureUnit.Pascal).As(unit), unit);
        IQuantity z = new Pressure(new Pressure(result.ZX, PressureUnit.Pascal).As(unit), unit);
        return new GsaResultQuantity() {
          X = x,
          Y = y,
          Z = z,
          Xyz = null,
        };
      } else {
        IQuantity x = new Pressure(new Pressure(result.XX, PressureUnit.Pascal).As(unit), unit);
        IQuantity y = new Pressure(new Pressure(result.YY, PressureUnit.Pascal).As(unit), unit);
        IQuantity z = new Pressure(new Pressure(result.ZZ, PressureUnit.Pascal).As(unit), unit);
        return new GsaResultQuantity() {
          X = x,
          Y = y,
          Z = z,
          Xyz = null,
        };
      }
    }

    internal static GsaResultQuantity GetQuantityResult(Vector3 result, MassUnit unit) {
      IQuantity x = new Mass(new Mass(result.X, MassUnit.Kilogram).As(unit), unit);
      IQuantity y = new Mass(new Mass(result.Y, MassUnit.Kilogram).As(unit), unit);
      IQuantity z = new Mass(new Mass(result.Z, MassUnit.Kilogram).As(unit), unit);
      double pyth
        = Math.Sqrt(Math.Pow(result.X, 2) + Math.Pow(result.Y, 2) + Math.Pow(result.Z, 2));
      IQuantity xyz = new Mass(new Mass(pyth, MassUnit.Kilogram).As(unit), unit);
      return new GsaResultQuantity() {
        X = x,
        Y = y,
        Z = z,
        Xyz = xyz,
      };
    }

    internal static GsaResultQuantity GetQuantityResult(
      Vector3 result, AreaMomentOfInertiaUnit unit) {
      IQuantity x = new AreaMomentOfInertia(
        new AreaMomentOfInertia(result.X, AreaMomentOfInertiaUnit.MeterToTheFourth).As(unit), unit);
      IQuantity y = new AreaMomentOfInertia(
        new AreaMomentOfInertia(result.Y, AreaMomentOfInertiaUnit.MeterToTheFourth).As(unit), unit);
      IQuantity z = new AreaMomentOfInertia(
        new AreaMomentOfInertia(result.Z, AreaMomentOfInertiaUnit.MeterToTheFourth).As(unit), unit);
      double pyth
        = Math.Sqrt(Math.Pow(result.X, 2) + Math.Pow(result.Y, 2) + Math.Pow(result.Z, 2));
      IQuantity xyz = new AreaMomentOfInertia(
        new AreaMomentOfInertia(pyth, AreaMomentOfInertiaUnit.MeterToTheFourth).As(unit), unit);
      return new GsaResultQuantity() {
        X = x,
        Y = y,
        Z = z,
        Xyz = xyz,
      };
    }
  }
}
