using Grasshopper.Kernel.Types;

using GsaAPI;

using OasysGH.Parameters;

using OasysUnits;
using OasysUnits.Units;

namespace GsaGH.Parameters {
  /// <summary>
  /// Effective length calculation options are part of a <see cref="GsaMember1d"/>. These options are available for 1D members when the section material is steel. They define the properties relating to design from the interaction of the member with the rest of the structure.
  /// <para>Refer to <see href="https://docs.oasys-software.com/structural/gsa/references/hidd-page-member-steel.html">Member Design properties</see> to read more.</para>
  /// </summary>
  public class GsaEffectiveLengthOptions {
    public GsaBucklingFactors BucklingFactors { get; set; } = null;
    public EffectiveLength EffectiveLength { get; set; }

    public GsaEffectiveLengthOptions() { }

    public GsaEffectiveLengthOptions(GsaMember1d member) {
      BucklingFactors = new GsaBucklingFactors(member);
      EffectiveLength = member.ApiMember.EffectiveLength;
    }

    public GsaEffectiveLengthOptions(GsaEffectiveLengthOptions other) {
      BucklingFactors = other.BucklingFactors;
      EffectiveLength = other.EffectiveLength;
    }

    public override string ToString() {
      string bucklingFacts = string.Empty;
      if (BucklingFactors != null && !string.IsNullOrEmpty(BucklingFactors.ToString())) {
        bucklingFacts = $", Design variables overrides: {BucklingFactors}";
      }

      if (EffectiveLength == null) {
        return bucklingFacts.TrimStart(',', ' ');
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
          string intern = $"End1: {e1}, End2: {e2}" +
            $", Along Member: {internalRestraints.RestraintAlongMember}, " +
            $"Intermediate Points: {internalRestraints.RestraintAtBracedPoints}";
          return "User-specified internal restraints: " + intern + destabilisingLoad + bucklingFacts;

        case EffectiveLengthFromUserSpecifiedValue userSpecified:
          string y = ConvertEffectiveLengthAttribute(userSpecified.EffectiveLengthAboutY);
          string z = ConvertEffectiveLengthAttribute(userSpecified.EffectiveLengthAboutZ);
          string ltb = ConvertEffectiveLengthAttribute(userSpecified.EffectiveLengthLaterialTorsional);
          string user = $"About y: {userSpecified.EffectiveLengthAboutY.Option} {y}, " +
            $"About z: {userSpecified.EffectiveLengthAboutZ.Option} {z}, " +
            $"lateral torsional: {userSpecified.EffectiveLengthAboutZ.Option} {ltb}";
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

    private string ConvertEffectiveLengthAttribute(EffectiveLengthAttribute leffAttribute) {
      if (leffAttribute.Option == EffectiveLengthOptionType.Absolute) {
        return new GH_Number(leffAttribute.Value).ToString();
      }

      return new GH_UnitNumber(new Ratio(leffAttribute.Value, RatioUnit.DecimalFraction).
        ToUnit(RatioUnit.Percent)).ToString();
    }
  }
}
