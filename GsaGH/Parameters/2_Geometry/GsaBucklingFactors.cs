namespace GsaGH.Parameters {
  /// <summary>
  /// Buckling Factors are part of a <see cref="GsaMember1d"/> and can be used to override the automatically calculated factors that accounts for the shape of the moment diagram in lateral torsional buckling design equations. This override is applied for all bending segments in the member.
  /// <para>Refer to <see href="https://docs.oasys-software.com/structural/gsa/references/hidd-page-member-steel.html#equivalent-uniform-moment-factor-for-ltb">Equivalent uniform moment factor for LTB</see> to read more.</para>
  /// </summary>
  public class GsaBucklingFactors {
    public double? EquivalentUniformMomentFactor { get; set; }
    public double? MomentAmplificationFactorStrongAxis { get; set; }
    public double? MomentAmplificationFactorWeakAxis { get; set; }

    public GsaBucklingFactors() { }

    public GsaBucklingFactors(
      double momentAmplificationFactorStrongAxis, double momentAmplificationFactorWeakAxis,
      double equivalentUniformMomentFactor) {
      EquivalentUniformMomentFactor = equivalentUniformMomentFactor;
      MomentAmplificationFactorStrongAxis = momentAmplificationFactorStrongAxis;
      MomentAmplificationFactorWeakAxis = momentAmplificationFactorWeakAxis;
    }

    internal GsaBucklingFactors(GsaMember1d member) {
      EquivalentUniformMomentFactor = member.ApiMember.EquivalentUniformMomentFactor;
      MomentAmplificationFactorStrongAxis = member.ApiMember.MomentAmplificationFactorStrongAxis;
      MomentAmplificationFactorWeakAxis = member.ApiMember.MomentAmplificationFactorWeakAxis;
    }

    public GsaBucklingFactors(GsaBucklingFactors other) {
      EquivalentUniformMomentFactor = other.EquivalentUniformMomentFactor;
      MomentAmplificationFactorStrongAxis = other.MomentAmplificationFactorStrongAxis;
      MomentAmplificationFactorWeakAxis = other.MomentAmplificationFactorWeakAxis;
    }

    public override string ToString() {
      string y = MomentAmplificationFactorStrongAxis == null ? string.Empty :
        "fLsy:" + MomentAmplificationFactorStrongAxis;
      string z = MomentAmplificationFactorWeakAxis == null ? string.Empty :
        "fLsz:" + MomentAmplificationFactorWeakAxis;
      string lt = EquivalentUniformMomentFactor == null ? string.Empty :
        "fLtb:" + EquivalentUniformMomentFactor;
      return string.Join(" ", y, z, lt).Trim();
    }
  }
}
