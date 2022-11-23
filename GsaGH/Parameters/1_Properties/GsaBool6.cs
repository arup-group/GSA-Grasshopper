using GsaAPI;

namespace GsaGH.Parameters
{
  /// <summary>
  /// Bool6 class, this class defines the basic properties and methods for any <see cref="GsaAPI.Bool6"/>
  /// </summary>
  public class GsaBool6
  {
    #region fields
    internal Bool6 _bool6;
    #endregion

    #region properties
    public bool X
    {
      get
      {
        return this._bool6.X;
      }
      set
      {
        this._bool6 = new Bool6(value, Y, Z, XX, YY, ZZ);
      }
    }
    public bool Y
    {
      get
      {
        return this._bool6.Y;
      }
      set
      {
        this._bool6 = new Bool6(X, value, Z, XX, YY, ZZ);
      }
    }
    public bool Z
    {
      get
      {
        return this._bool6.Z;
      }
      set
      {
        this._bool6 = new Bool6(X, Y, value, XX, YY, ZZ);
      }
    }
    public bool XX
    {
      get
      {
        return this._bool6.XX;
      }
      set
      {
        this._bool6 = new Bool6(X, Y, Z, value, YY, ZZ);
      }
    }
    public bool YY
    {
      get
      {
        return this._bool6.YY;
      }
      set
      {
        this._bool6 = new Bool6(X, Y, Z, XX, value, ZZ);
      }
    }
    public bool ZZ
    {
      get
      {
        return this._bool6.ZZ;
      }
      set
      {
        this._bool6 = new Bool6(X, Y, Z, XX, YY, value);
      }
    }
    #endregion

    #region constructors
    public GsaBool6()
    {
      this._bool6 = new Bool6(false, false, false, false, false, false);
    }

    public GsaBool6(bool X, bool Y, bool Z, bool XX, bool YY, bool ZZ)
    {
      this._bool6 = new Bool6(X, Y, Z, XX, YY, ZZ);
    }

    internal GsaBool6(Bool6 bool6)
    {
      this._bool6 = bool6;
    }
    #endregion

    #region methods
    public GsaBool6 Duplicate()
    {
      // create shallow copy
      GsaBool6 dup = new GsaBool6
      {
        _bool6 = this._bool6
      };
      return dup;
    }

    public override string ToString()
    {
      string state = "Other";
      if (
        this.X == false &&
        this.Y == false &&
        this.Z == false &&
        this.XX == false &&
        this.YY == false &&
        this.ZZ == false
        )
        state = "Free";
      if (
        this.X == true &&
        this.Y == true &&
        this.Z == true &&
        this.XX == false &&
        this.YY == false &&
        this.ZZ == false
        )
        state = "Pin";
      if (
        this.X == false &&
        this.Y == false &&
        this.Z == false &&
        this.XX == false &&
        this.YY == true &&
        this.ZZ == true
        )
        state = "Hinge";
      if (
        this.X == true &&
        this.Y == true &&
        this.Z == true &&
        this.XX == true &&
        this.YY == true &&
        this.ZZ == true
        )
        state = "Fixed";
      if (state == "Other")
      {
        string sx = (X) ? "\u2713" : "\u2610";
        sx = "X" + sx;
        string sy = (Y) ? "\u2713" : "\u2610";
        sy = " Y" + sy;
        string sz = (Z) ? "\u2713" : "\u2610";
        sz = " Z" + sz;
        string sxx = (XX) ? "\u2713" : "\u2610";
        sxx = " XX" + sxx;
        string syy = (YY) ? "\u2713" : "\u2610";
        syy = " YY" + syy;
        string szz = (ZZ) ? "\u2713" : "\u2610";
        szz = " ZZ" + szz;
        return sx + sy + sz + sxx + syy + szz;
      }
      else
        return state.ToString().Trim();
    }
    #endregion
  }
}
