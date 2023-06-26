using GsaAPI;

namespace GsaGH.Parameters {
  /// <summary>
  ///   Bool6 class, this class defines the basic properties and methods for any <see cref="GsaAPI.Bool6" />
  /// </summary>
  public class GsaBool6 {
    public bool X {
      get => _bool6.X;
      set => _bool6 = new Bool6(value, Y, Z, Xx, Yy, Zz);
    }
    public bool Xx {
      get => _bool6.XX;
      set => _bool6 = new Bool6(X, Y, Z, value, Yy, Zz);
    }
    public bool Y {
      get => _bool6.Y;
      set => _bool6 = new Bool6(X, value, Z, Xx, Yy, Zz);
    }
    public bool Yy {
      get => _bool6.YY;
      set => _bool6 = new Bool6(X, Y, Z, Xx, value, Zz);
    }
    public bool Z {
      get => _bool6.Z;
      set => _bool6 = new Bool6(X, Y, value, Xx, Yy, Zz);
    }
    public bool Zz {
      get => _bool6.ZZ;
      set => _bool6 = new Bool6(X, Y, Z, Xx, Yy, value);
    }
    internal Bool6 _bool6;

    public GsaBool6() {
      _bool6 = new Bool6(false, false, false, false, false, false);
    }

    public GsaBool6(bool x, bool y, bool z, bool xx, bool yy, bool zz) {
      _bool6 = new Bool6(x, y, z, xx, yy, zz);
    }

    internal GsaBool6(Bool6 bool6) {
      _bool6 = bool6;
    }

    public GsaBool6 Duplicate() {
      // create shallow copy
      var dup = new GsaBool6 {
        _bool6 = _bool6,
      };
      return dup;
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

      if (state == "Other") {
        string sx = X ? "\u2713" : "\u2610";
        sx = $"X{sx}";
        string sy = Y ? "\u2713" : "\u2610";
        sy = $" Y{sy}";
        string sz = Z ? "\u2713" : "\u2610";
        sz = $" Z{sz}";
        string sxx = Xx ? "\u2713" : "\u2610";
        sxx = $" XX{sxx}";
        string syy = Yy ? "\u2713" : "\u2610";
        syy = $" YY{syy}";
        string szz = Zz ? "\u2713" : "\u2610";
        szz = $" ZZ{szz}";
        return sx + sy + sz + sxx + syy + szz;
      } else {
        return state.Trim();
      }
    }
  }
}
