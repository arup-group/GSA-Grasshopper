using GsaAPI;

namespace GsaGH.Parameters {
  /// <summary>
  /// A Bool6 contains six booleans to set releases in <see cref="GsaElement1d"/>s and <see cref="GsaMember1d"/>s, or restraints in <see cref="GsaNode"/>s.
  /// </summary>
  public class GsaBool6 {
    private bool _isRestraint = true;
    public Bool6 ApiBool6 { get; set; }
    public bool X {
      get => ApiBool6.X;
      set => ApiBool6 = new Bool6(value, Y, Z, Xx, Yy, Zz);
    }
    public bool Xx {
      get => ApiBool6.XX;
      set => ApiBool6 = new Bool6(X, Y, Z, value, Yy, Zz);
    }
    public bool Y {
      get => ApiBool6.Y;
      set => ApiBool6 = new Bool6(X, value, Z, Xx, Yy, Zz);
    }
    public bool Yy {
      get => ApiBool6.YY;
      set => ApiBool6 = new Bool6(X, Y, Z, Xx, value, Zz);
    }
    public bool Z {
      get => ApiBool6.Z;
      set => ApiBool6 = new Bool6(X, Y, value, Xx, Yy, Zz);
    }
    public bool Zz {
      get => ApiBool6.ZZ;
      set => ApiBool6 = new Bool6(X, Y, Z, Xx, Yy, value);
    }

    public GsaBool6() {
      ApiBool6 = new Bool6(false, false, false, false, false, false);
    }

    public GsaBool6(bool x, bool y, bool z, bool xx, bool yy, bool zz) {
      ApiBool6 = new Bool6(x, y, z, xx, yy, zz);
    }

    public GsaBool6(GsaBool6 other) {
      ApiBool6 = new Bool6(false, false, false, false, false, false);
      X = other.X;
      Y = other.Y;
      Z = other.Z;
      Xx = other.Xx;
      Yy = other.Yy;
      Zz = other.Zz;
    }

    internal GsaBool6(Bool6 bool6) {
      ApiBool6 = bool6;
    }

    public GsaBool6(bool x, bool y, bool z, bool xx, bool yy, bool zz, bool isRestraint) {
      ApiBool6 = new Bool6(x, y, z, xx, yy, zz);
      _isRestraint = isRestraint;
    }

    public GsaBool6 Negate(bool isRestraint = false) {
      return new GsaBool6(!ApiBool6.X, !ApiBool6.Y, !ApiBool6.Z, !ApiBool6.XX, !ApiBool6.YY, !ApiBool6.ZZ, isRestraint);
    }

    public override string ToString() {
      string state = "Other";
      if (X == false && Y == false && Z == false && Xx == false && Yy == false && Zz == false) {
        state = "Free";
      }

      if (X == true && Y == true && Z == true && Xx == false && Yy == false && Zz == false) {
        state = "Pin";
      }

      if (X == false && Y == false && Z == false && Xx == false && Yy == true && Zz == true) {
        state = "Hinge";
      }

      if (X == true && Y == true && Z == true && Xx == true && Yy == true && Zz == true) {
        state = "Fixed";
      }

      var gsaBool = new GsaBool6(ApiBool6);
      if (!_isRestraint) {
        gsaBool = gsaBool.Negate();
      }

      if (state == "Other") {
        string sx = gsaBool.X ? "\u2713" : "\u2610";
        sx = "X" + sx;
        string sy = gsaBool.Y ? "\u2713" : "\u2610";
        sy = " Y" + sy;
        string sz = gsaBool.Z ? "\u2713" : "\u2610";
        sz = " Z" + sz;
        string sxx = gsaBool.Xx ? "\u2713" : "\u2610";
        sxx = " XX" + sxx;
        string syy = gsaBool.Yy ? "\u2713" : "\u2610";
        syy = " YY" + syy;
        string szz = gsaBool.Zz ? "\u2713" : "\u2610";
        szz = " ZZ" + szz;
        return sx + sy + sz + sxx + syy + szz;
      } else {
        return state.Trim();
      }
    }
  }
}
