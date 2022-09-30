using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaAPI;
using OasysGH;
using OasysGH.Parameters;

namespace GsaGH.Parameters
{
  /// <summary>
  /// Bool6 class, this class defines the basic properties and methods for any Gsa Bool6
  /// </summary>
  public class GsaBool6
  {
    public bool X
    {
      get { return API_Bool6.X; }
      set { API_Bool6 = new Bool6(value, Y, Z, XX, YY, ZZ); }
    }
    public bool Y
    {
      get { return API_Bool6.Y; }
      set { API_Bool6 = new Bool6(X, value, Z, XX, YY, ZZ); }
    }
    public bool Z
    {
      get { return API_Bool6.Z; }
      set { API_Bool6 = new Bool6(X, Y, value, XX, YY, ZZ); }
    }
    public bool XX
    {
      get { return API_Bool6.XX; }
      set { API_Bool6 = new Bool6(X, Y, Z, value, YY, ZZ); }
    }
    public bool YY
    {
      get { return API_Bool6.YY; }
      set { API_Bool6 = new Bool6(X, Y, Z, XX, value, ZZ); }
    }
    public bool ZZ
    {
      get { return API_Bool6.ZZ; }
      set { API_Bool6 = new Bool6(X, Y, Z, XX, YY, value); }
    }

    #region fields
    internal Bool6 API_Bool6;
    #endregion

    #region constructors
    public GsaBool6()
    {
      API_Bool6 = new Bool6(false, false, false, false, false, false);
    }

    public GsaBool6(bool X, bool Y, bool Z, bool XX, bool YY, bool ZZ)
    {
      API_Bool6 = new Bool6(X, Y, Z, XX, YY, ZZ);
    }

    internal GsaBool6(Bool6 bool6)
    {
      API_Bool6 = bool6;
    }

    public GsaBool6 Duplicate()
    {
      if (this == null) { return null; }
      // create shallow copy
      GsaBool6 dup = new GsaBool6
      {
        API_Bool6 = this.API_Bool6
      };
      return dup;
    }
    #endregion

    #region properties
    public bool IsValid
    {
      get
      {
        return true;
      }
    }


    #endregion

    #region methods
    public override string ToString()
    {
      string sx = (X) ? "\u2713" : "\u2610";
      sx = "{X" + sx;
      string sy = (Y) ? "\u2713" : "\u2610";
      sy = ", Y" + sy;
      string sz = (Z) ? "\u2713" : "\u2610";
      sz = ", Z" + sz;
      string sxx = (XX) ? "\u2713" : "\u2610";
      sxx = ", XX" + sxx;
      string syy = (YY) ? "\u2713" : "\u2610";
      syy = ", YY" + syy;
      string szz = (ZZ) ? "\u2713" : "\u2610";
      szz = ", ZZ" + szz + "}";

      return "GSA Bool 6" + sx + sy + sz + sxx + syy + szz;
    }

    #endregion
  }
}
