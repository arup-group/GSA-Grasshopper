using OasysUnits;
using OasysUnits.Units;

namespace GsaGH.Parameters {

  /// <summary>
  /// Buckling Length Factors for a <see cref="GsaAPI.Member"/>
  /// </summary>
  public class GsaBucklingLengthFactors {

    #region Properties + Fields
    public double? LateralTorsionalBucklingFactor { get; set; }
    public double? MomentAmplificationFactorStrongAxis { get; set; }
    public double? MomentAmplificationFactorWeakAxis { get; set; }
    internal Length Length { get; }
    internal bool LengthIsSet { get; private set; } = false;
    #endregion Properties + Fields

    #region Public Constructors
    public GsaBucklingLengthFactors() {
    }

    public GsaBucklingLengthFactors(double momentAmplificationFactorStrongAxis, double momentAmplificationFactorWeakAxis, double lateralTorsionalBucklingFactor) {
      LateralTorsionalBucklingFactor = lateralTorsionalBucklingFactor;
      MomentAmplificationFactorStrongAxis = momentAmplificationFactorStrongAxis;
      MomentAmplificationFactorWeakAxis = momentAmplificationFactorWeakAxis;
    }

    #endregion Public Constructors

    #region Internal Constructors
    internal GsaBucklingLengthFactors(GsaMember1d member) {
      LateralTorsionalBucklingFactor = member.ApiMember.LateralTorsionalBucklingFactor;
      MomentAmplificationFactorStrongAxis = member.ApiMember.MomentAmplificationFactorStrongAxis;
      MomentAmplificationFactorWeakAxis = member.ApiMember.MomentAmplificationFactorWeakAxis;
    }

    internal GsaBucklingLengthFactors(GsaMember1d member, LengthUnit lengthUnit) {
      LateralTorsionalBucklingFactor = member.ApiMember.LateralTorsionalBucklingFactor;
      MomentAmplificationFactorStrongAxis = member.ApiMember.MomentAmplificationFactorStrongAxis;
      MomentAmplificationFactorWeakAxis = member.ApiMember.MomentAmplificationFactorWeakAxis;
      Length = new Length(member.PolyCurve.GetLength(), lengthUnit);
      LengthIsSet = true;
    }

    #endregion Internal Constructors

    #region Public Methods
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

    #endregion Public Methods
  }
}
