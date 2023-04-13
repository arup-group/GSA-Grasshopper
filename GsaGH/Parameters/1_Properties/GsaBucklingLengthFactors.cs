using OasysUnits.Units;

namespace GsaGH.Parameters {
  /// <summary>
  /// Buckling Length Factors for a <see cref="GsaAPI.Member"/>
  /// </summary>
  public class GsaBucklingLengthFactors {
    public double? EquivalentUniformMomentFactor { get; set; }
    public double? MomentAmplificationFactorStrongAxis { get; set; }
    public double? MomentAmplificationFactorWeakAxis { get; set; }

    public GsaBucklingLengthFactors() {
    }

    public GsaBucklingLengthFactors(double momentAmplificationFactorStrongAxis, double momentAmplificationFactorWeakAxis, double equivalentUniformMomentFactor) {
      EquivalentUniformMomentFactor = equivalentUniformMomentFactor;
      MomentAmplificationFactorStrongAxis = momentAmplificationFactorStrongAxis;
      MomentAmplificationFactorWeakAxis = momentAmplificationFactorWeakAxis;
    }

    internal GsaBucklingLengthFactors(GsaMember1d member) {
      EquivalentUniformMomentFactor = member.ApiMember.EquivalentUniformMomentFactor;
      MomentAmplificationFactorStrongAxis = member.ApiMember.MomentAmplificationFactorStrongAxis;
      MomentAmplificationFactorWeakAxis = member.ApiMember.MomentAmplificationFactorWeakAxis;
    }

    internal GsaBucklingLengthFactors(GsaMember1d member, LengthUnit lengthUnit) {
      EquivalentUniformMomentFactor = member.ApiMember.EquivalentUniformMomentFactor;
      MomentAmplificationFactorStrongAxis = member.ApiMember.MomentAmplificationFactorStrongAxis;
      MomentAmplificationFactorWeakAxis = member.ApiMember.MomentAmplificationFactorWeakAxis;
    }

    public GsaBucklingLengthFactors Duplicate() {
      return (GsaBucklingLengthFactors)MemberwiseClone();
    }

    public override string ToString() {
      string y = MomentAmplificationFactorStrongAxis == null ? "" : "fLsy:" + MomentAmplificationFactorStrongAxis;
      string z = MomentAmplificationFactorWeakAxis == null ? "" : "fLsz:" + MomentAmplificationFactorWeakAxis;
      string lt = EquivalentUniformMomentFactor == null ? "" : "fLtb:" + EquivalentUniformMomentFactor;
      string output = string.Join(" ", y, z, lt).Trim();
      return output == "" ? "Automatic" : output;
    }
  }
}
