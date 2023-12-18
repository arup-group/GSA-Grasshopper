using GsaAPI;
using OasysUnits;
using OasysUnits.Units;
using ForceUnit = OasysUnits.Units.ForceUnit;

namespace GsaGH.Parameters.Results {
  public class ReactionForce : IReactionForce {
    internal ReactionForce(Double6 values) {
      if (!double.IsNaN(values.X)) {
        X = new Force(values.X, ForceUnit.Newton);
      }
      if (!double.IsNaN(values.Y)) {
        Y = new Force(values.Y, ForceUnit.Newton);
      }
      if (!double.IsNaN(values.Z)) {
        Z = new Force(values.Z, ForceUnit.Newton);
      }
      if (X != null && Y != null && Z != null) {
        Xyz = QuantityUtility.PythagoreanQuadruple((Force)X, (Force)Y, (Force)Z);
      }
      if (!double.IsNaN(values.XX)) {
        Xx = new Moment(values.XX, MomentUnit.NewtonMeter);
      }
      if (!double.IsNaN(values.YY)) {
        Yy = new Moment(values.YY, MomentUnit.NewtonMeter);
      }
      if (!double.IsNaN(values.ZZ)) {
        Zz = new Moment(values.ZZ, MomentUnit.NewtonMeter);
      }
      if (Xx != null && Yy != null && Zz != null) {
        Xxyyzz = QuantityUtility.PythagoreanQuadruple((Moment)Xx, (Moment)Yy, (Moment)Zz);
      }
    }

    public Force? X { get; internal set; } = null;
    public Force? Y { get; internal set; } = null;
    public Force? Z { get; internal set; } = null;
    public Force? Xyz { get; internal set; } = null;
    public Moment? Xx { get; internal set; } = null;
    public Moment? Yy { get; internal set; } = null;
    public Moment? Zz { get; internal set; } = null;
    public Moment? Xxyyzz { get; internal set; } = null;

    public Force? XToUnit(ForceUnit unit) {
      if (X != null) {
        return ((Force)X).ToUnit(unit);
      }
      return null;
    }

    public Force? YToUnit(ForceUnit unit) {
      if (Y != null) {
        return ((Force)Y).ToUnit(unit);
      }
      return null;
    }

    public Force? ZToUnit(ForceUnit unit) {
      if (Z != null) {
        return ((Force)Z).ToUnit(unit);
      }
      return null;
    }

    public Force? XyzToUnit(ForceUnit unit) {
      if (Xyz != null) {
        return ((Force)Xyz).ToUnit(unit);
      }
      return null;
    }

    public Moment? XxToUnit(MomentUnit unit) {
      if (Xx != null) {
        return ((Moment)Xx).ToUnit(unit);
      }
      return null;
    }

    public Moment? YyToUnit(MomentUnit unit) {
      if (Yy != null) {
        return ((Moment)Yy).ToUnit(unit);
      }
      return null;
    }

    public Moment? ZzToUnit(MomentUnit unit) {
      if (Zz != null) {
        return ((Moment)Zz).ToUnit(unit);
      }
      return null;
    }

    public Moment? XxyyzzToUnit(MomentUnit unit) {
      if (Xxyyzz != null) {
        return ((Moment)Xxyyzz).ToUnit(unit);
      }
      return null;
    }

    public double? XAs(ForceUnit unit) {
      if (X != null) {
        return ((Force)X).As(unit);
      }
      return null;
    }

    public double? YAs(ForceUnit unit) {
      if (Y != null) {
        return ((Force)Y).As(unit);
      }
      return null;
    }

    public double? ZAs(ForceUnit unit) {
      if (Z != null) {
        return ((Force)Z).As(unit);
      }
      return null;
    }

    public double? XyzAs(ForceUnit unit) {
      if (Xyz != null) {
        return ((Force)Xyz).As(unit);
      }
      return null;
    }

    public double? XxAs(MomentUnit unit) {
      if (Xx != null) {
        return ((Moment)Xx).As(unit);
      }
      return null;
    }

    public double? YyAs(MomentUnit unit) {
      if (Yy != null) {
        return ((Moment)Yy).As(unit);
      }
      return null;
    }

    public double? ZzAs(MomentUnit unit) {
      if (Zz != null) {
        return ((Moment)Zz).As(unit);
      }
      return null;
    }

    public double? XxyyzzAs(MomentUnit unit) {
      if (Xxyyzz != null) {
        return ((Moment)Xxyyzz).As(unit);
      }
      return null;
    }
  }
}
