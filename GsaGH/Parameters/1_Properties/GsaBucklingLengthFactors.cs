
using OasysUnits;
using OasysUnits.Units;

namespace GsaGH.Parameters {
  /// <summary>
  /// Buckling Length Factors for a <see cref="GsaAPI.Member"/>
  /// </summary>
  public class GsaBucklingLengthFactors {
    #region properties
    public double? MomentAmplificationFactorStrongAxis { get; set; }
    public double? MomentAmplificationFactorWeakAxis { get; set; }
    public double? LateralTorsionalBucklingFactor { get; set; }
    internal Length Length { get; }
    internal bool LengthIsSet { get; private set; } = false;
    #endregion

    #region constructors
    public GsaBucklingLengthFactors() {
    }

    public GsaBucklingLengthFactors(double momentAmplificationFactorStrongAxis, double momentAmplificationFactorWeakAxis, double lateralTorsionalBucklingFactor) {
      LateralTorsionalBucklingFactor = lateralTorsionalBucklingFactor;
      MomentAmplificationFactorStrongAxis = momentAmplificationFactorStrongAxis;
      MomentAmplificationFactorWeakAxis = momentAmplificationFactorWeakAxis;
    }

    internal GsaBucklingLengthFactors(GsaMember1d member) {
      LateralTorsionalBucklingFactor = member.ApiMember.EquivalentUniformMomentFactor;
      MomentAmplificationFactorStrongAxis = member.ApiMember.MomentAmplificationFactorStrongAxis;
      MomentAmplificationFactorWeakAxis = member.ApiMember.MomentAmplificationFactorWeakAxis;
    }

    internal GsaBucklingLengthFactors(GsaMember1d member, LengthUnit lengthUnit) {
      LateralTorsionalBucklingFactor = member.ApiMember.EquivalentUniformMomentFactor;
      MomentAmplificationFactorStrongAxis = member.ApiMember.MomentAmplificationFactorStrongAxis;
      MomentAmplificationFactorWeakAxis = member.ApiMember.MomentAmplificationFactorWeakAxis;
      Length = new Length(member.PolyCurve.GetLength(), lengthUnit);
      LengthIsSet = true;
    }
    #endregion

    #region methods
    public GsaBucklingLengthFactors Duplicate() {
      return (GsaBucklingLengthFactors)MemberwiseClone();
    }

    public override string ToString() {
      string y = MomentAmplificationFactorStrongAxis == null ? "" : "fLsy:" + MomentAmplificationFactorStrongAxis;
      string z = MomentAmplificationFactorWeakAxis == null ? "" : "fLsz:" + MomentAmplificationFactorWeakAxis;
      string lt = LateralTorsionalBucklingFactor == null ? "" : "fLtb:" + LateralTorsionalBucklingFactor;
      string output = string.Join(" ", y, z, lt).Trim();
      return output == "" ? "Automatic" : output;
    }
    #endregion
  }
}
