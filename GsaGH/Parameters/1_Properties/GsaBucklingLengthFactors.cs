
using OasysUnits;
using OasysUnits.Units;

namespace GsaGH.Parameters
{
  /// <summary>
  /// Buckling Length Factors for a <see cref="GsaAPI.Member"/>
  /// </summary>
  public class GsaBucklingLengthFactors
  {
    #region properties
    public double? MomentAmplificationFactorStrongAxis { get; set; }
    public double? MomentAmplificationFactorWeakAxis { get; set; }
    public double? LateralTorsionalBucklingFactor { get; set; }
    internal Length Length { get; }
    internal bool LengthIsSet { get; private set; } = false;
    #endregion

    #region constructors
    public GsaBucklingLengthFactors()
    {
    }

    public GsaBucklingLengthFactors(double momentAmplificationFactorStrongAxis, double momentAmplificationFactorWeakAxis, double lateralTorsionalBucklingFactor)
    {
      this.LateralTorsionalBucklingFactor = lateralTorsionalBucklingFactor;
      this.MomentAmplificationFactorStrongAxis = momentAmplificationFactorStrongAxis;
      this.MomentAmplificationFactorWeakAxis = momentAmplificationFactorWeakAxis;
    }

    internal GsaBucklingLengthFactors(GsaMember1d member)
    {
      this.LateralTorsionalBucklingFactor = member.ApiMember.LateralTorsionalBucklingFactor;
      this.MomentAmplificationFactorStrongAxis = member.ApiMember.MomentAmplificationFactorStrongAxis;
      this.MomentAmplificationFactorWeakAxis = member.ApiMember.MomentAmplificationFactorWeakAxis;
    }

    internal GsaBucklingLengthFactors(GsaMember1d member, LengthUnit lengthUnit)
    {
      this.LateralTorsionalBucklingFactor = member.ApiMember.LateralTorsionalBucklingFactor;
      this.MomentAmplificationFactorStrongAxis = member.ApiMember.MomentAmplificationFactorStrongAxis;
      this.MomentAmplificationFactorWeakAxis = member.ApiMember.MomentAmplificationFactorWeakAxis;
      this.Length = new Length(member.PolyCurve.GetLength(), lengthUnit);
      this.LengthIsSet = true;
    }
    #endregion

    #region methods
    public GsaBucklingLengthFactors Duplicate()
    {
      return (GsaBucklingLengthFactors)this.MemberwiseClone();
    }

    public override string ToString()
    {
      string y = this.MomentAmplificationFactorStrongAxis == null ? "" : "fLsy:" + this.MomentAmplificationFactorStrongAxis;
      string z = this.MomentAmplificationFactorWeakAxis == null ? "" : "fLsz:" + this.MomentAmplificationFactorWeakAxis;
      string lt = this.LateralTorsionalBucklingFactor == null ? "" : "fLtb:" + this.LateralTorsionalBucklingFactor;
      string output = string.Join(" ", y, z, lt).Trim();
      return output == "" ? "Automatic" : output;
    }
    #endregion
  }
}
