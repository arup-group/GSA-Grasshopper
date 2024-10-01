using GsaAPI;

using Newtonsoft.Json.Linq;

using OasysUnits;
using OasysUnits.Units;

using ForceUnit = OasysUnits.Units.ForceUnit;

namespace GsaGH.Parameters.Results {
  public class ReactionForce : IReactionForce {
    public Force? X { get; internal set; }
    public Force? Y { get; internal set; }
    public Force? Z { get; internal set; }
    public Force? Xyz { get; internal set; }
    public Moment? Xx { get; internal set; }
    public Moment? Yy { get; internal set; }
    public Moment? Zz { get; internal set; }
    public Moment? Xxyyzz { get; internal set; }

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

      Force x = Force.Zero;
      Force y = Force.Zero;
      Force z = Force.Zero;

      if (X.HasValue) {
        x = X.Value;
      }

      if (Y.HasValue) {
        y = Y.Value;
      }

      if (Z.HasValue) {
        z = Z.Value;
      }

      if (!X.HasValue && !Y.HasValue && !Z.HasValue) {
        Xyz = null;
      } else {
        Xyz = QuantityUtility.PythagoreanQuadruple(x, y, z);
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

      Moment xx = Moment.Zero;
      Moment yy = Moment.Zero;
      Moment zz = Moment.Zero;

      if (Xx.HasValue) {
        xx = Xx.Value;
      }

      if (Yy.HasValue) {
        yy = Yy.Value;
      }

      if (Zz.HasValue) {
        zz = Zz.Value;
      }

      if (!Xx.HasValue && !Yy.HasValue && !Zz.HasValue) {
        Xxyyzz = null;
      } else {
        Xxyyzz = QuantityUtility.PythagoreanQuadruple(xx, yy, zz);
      }
    }

    public Force? XToUnit(ForceUnit unit) {
      if (X.HasValue && !double.IsNaN(X.Value.Value)) {
        return X.Value.ToUnit(unit);
      }
      return null;
    }

    public Force? YToUnit(ForceUnit unit) {
      if (Y.HasValue && !double.IsNaN(Y.Value.Value)) {
        return Y.Value.ToUnit(unit);
      }
      return null;
    }

    public Force? ZToUnit(ForceUnit unit) {
      if (Z.HasValue && !double.IsNaN(Z.Value.Value)) {
        return Z.Value.ToUnit(unit);
      }
      return null;
    }

    public Force? XyzToUnit(ForceUnit unit) {
      if (Xyz.HasValue && !double.IsNaN(Xyz.Value.Value)) {
        return Xyz.Value.ToUnit(unit);
      }
      return null;
    }

    public Moment? XxToUnit(MomentUnit unit) {
      if (Xx.HasValue && !double.IsNaN(Xx.Value.Value)) {
        return Xx.Value.ToUnit(unit);
      }
      return null;
    }

    public Moment? YyToUnit(MomentUnit unit) {
      if (Yy.HasValue && !double.IsNaN(Yy.Value.Value)) {
        return Yy.Value.ToUnit(unit);
      }
      return null;
    }

    public Moment? ZzToUnit(MomentUnit unit) {
      if (Zz.HasValue && !double.IsNaN(Zz.Value.Value)) {
        return Zz.Value.ToUnit(unit);
      }
      return null;
    }

    public Moment? XxyyzzToUnit(MomentUnit unit) {
      if (Xxyyzz.HasValue && !double.IsNaN(Xxyyzz.Value.Value)) {
        return Xxyyzz.Value.ToUnit(unit);
      }
      return null;
    }

    public double? XAs(ForceUnit unit) {
      if (X.HasValue && !double.IsNaN(X.Value.Value)) {
        return X.Value.As(unit);
      }
      return null;
    }

    public double? YAs(ForceUnit unit) {
      if (Y.HasValue && !double.IsNaN(Y.Value.Value)) {
        return Y.Value.As(unit);
      }
      return null;
    }

    public double? ZAs(ForceUnit unit) {
      if (Z.HasValue && !double.IsNaN(Z.Value.Value)) {
        return Z.Value.As(unit);
      }
      return null;
    }

    public double? XyzAs(ForceUnit unit) {
      if (Xyz.HasValue && !double.IsNaN(Xyz.Value.Value)) {
        return Xyz.Value.As(unit);
      }
      return null;
    }

    public double? XxAs(MomentUnit unit) {
      if (Xx.HasValue && !double.IsNaN(Xx.Value.Value)) {
        return Xx.Value.As(unit);
      }
      return null;
    }

    public double? YyAs(MomentUnit unit) {
      if (Yy.HasValue && !double.IsNaN(Yy.Value.Value)) {
        return Yy.Value.As(unit);
      }
      return null;
    }

    public double? ZzAs(MomentUnit unit) {
      if (Zz.HasValue && !double.IsNaN(Zz.Value.Value)) {
        return Zz.Value.As(unit);
      }
      return null;
    }

    public double? XxyyzzAs(MomentUnit unit) {
      if (Xxyyzz.HasValue && !double.IsNaN(Xxyyzz.Value.Value)) {
        return Xxyyzz.Value.As(unit);
      }
      return null;
    }
  }
}
