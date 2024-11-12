using GsaAPI;

namespace GsaGH.Parameters {
  /// <summary>
  /// A Bool6 contains six booleans to set releases in <see cref="GsaElement1d"/>s and <see cref="GsaMember1d"/>s, or restraints in <see cref="GsaNode"/>s.
  /// </summary>
  public class GsaBool6 {
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

    public override string ToString() {
      string sx = X ? "\u2713" : "\u2610";
      sx = "X" + sx;
      string sy = Y ? "\u2713" : "\u2610";
      sy = " Y" + sy;
      string sz = Z ? "\u2713" : "\u2610";
      sz = " Z" + sz;
      string sxx = Xx ? "\u2713" : "\u2610";
      sxx = " XX" + sxx;
      string syy = Yy ? "\u2713" : "\u2610";
      syy = " YY" + syy;
      string szz = Zz ? "\u2713" : "\u2610";
      szz = " ZZ" + szz;
      return sx + sy + sz + sxx + syy + szz;
    }

    public bool AllFalse() {
      return !X && !Y && !Z && !Xx && !Yy && !Zz;
    }

    public bool AllTrue() {
      return X && Y && Z && Xx && Yy && Zz;
    }

    public override int GetHashCode() {
      return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode() ^ Xx.GetHashCode() ^ Yy.GetHashCode()
        ^ Zz.GetHashCode();
    }

    public override bool Equals(object obj) {
      if (obj is GsaBool6 bool6) {
        return X == bool6.X && Y == bool6.Y && Z == bool6.Z && Xx == bool6.Xx && Yy == bool6.Yy && Zz == bool6.Zz;
      } else if (obj is bool boolean) {
        return AllEqualTo(boolean);
      } else {
        return false;
      }
    }

    private bool AllEqualTo(bool boolean) {
      return X == boolean && Y == boolean && Z == boolean && Xx == boolean && Yy == boolean && Zz == boolean;
    }

    public static GsaBool6 operator !(GsaBool6 bool6) {
      return new GsaBool6(!bool6.X, !bool6.Y, !bool6.Z, !bool6.Xx, !bool6.Yy, !bool6.Zz);
    }
  }

}
