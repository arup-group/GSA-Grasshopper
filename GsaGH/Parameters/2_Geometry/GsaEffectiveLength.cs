using GsaAPI;

namespace GsaGH.Parameters {
  /// <summary>
  /// Effective length calculation options are part of a <see cref="GsaMember1d"/>. These options are available for 1D members when the section material is steel. They define the properties relating to design from the interaction of the member with the rest of the structure.
  /// <para>Refer to <see href="https://docs.oasys-software.com/structural/gsa/references/hidd-page-member-steel.html">Member Design properties</see> to read more.</para>
  /// </summary>
  public class GsaEffectiveLength {
    public GsaBucklingFactors BucklingFactors { get; set; } = null;
    public EffectiveLength EffectiveLength { get; set; }

    public GsaEffectiveLength() { }

    public GsaEffectiveLength(GsaMember1d member) {
      BucklingFactors = new GsaBucklingFactors(member);
      EffectiveLength = member.ApiMember.EffectiveLength;
    }

    public GsaEffectiveLength(GsaEffectiveLength other) {
      BucklingFactors = other.BucklingFactors;
      EffectiveLength = other.EffectiveLength;
    }

    public override string ToString() {
      string bucklingFacts = string.Empty;
      if (BucklingFactors != null && !string.IsNullOrEmpty(BucklingFactors.ToString())) {
        bucklingFacts = $", Design variables overrides: {BucklingFactors}";
      }

      string destabilisingLoad = string.Empty;
      if (EffectiveLength.DestablisingLoad != 0 && 
        EffectiveLength.DestablisingLoadPositionRelativeTo != LoadReference.ShearCentre) {
        destabilisingLoad = $", Load position {EffectiveLength.DestablisingLoad}(m) " +
          $"{EffectiveLength.DestablisingLoadPositionRelativeTo}";
      }
      string e1 = string.Empty;
      string e2 = string.Empty;
      switch (EffectiveLength) {
        case EffectiveLengthFromEndAndInternalRestraint internalRestraints:
          e1 = MemberEndRestraintFactory.MemberEndRestraintToString(internalRestraints.End1);
          e2 = MemberEndRestraintFactory.MemberEndRestraintToString(internalRestraints.End1);
          string intern = $"End1: {e1}, End2: {e2} " +
            $", Along Member: {internalRestraints.RestraintAlongMember}, " +
            $"Intermediate Points: {internalRestraints.RestraintAtBracedPoints}";
          return "User-specified internal restraints: " + intern + destabilisingLoad + bucklingFacts;

        case EffectiveLengthFromUserSpecifiedValue userSpecified:
          string user = $"About y: {userSpecified.EffectiveLengthAboutY.Option} " +
            $"{userSpecified.EffectiveLengthAboutY.Value}, About z: {userSpecified.EffectiveLengthAboutZ.Option} " +
            $"{userSpecified.EffectiveLengthAboutZ.Value}, lateral torsional: {userSpecified.EffectiveLengthAboutZ.Option} " +
            $"{userSpecified.EffectiveLengthAboutZ.Value}";
          return "User-specified effective length: " + user + destabilisingLoad + bucklingFacts;

        case EffectiveLengthFromEndRestraintAndGeometry automatic:
          e1 = MemberEndRestraintFactory.MemberEndRestraintToString(automatic.End1);
          e2 = MemberEndRestraintFactory.MemberEndRestraintToString(automatic.End2);
          string auto = $"End1: {e1}, End2: {e2}";
          return "Automatically calculated " + auto + destabilisingLoad + bucklingFacts;
        
        default:
          throw new System.Exception("EffectiveLengthType not implemented");
      }
    }
  }
}
