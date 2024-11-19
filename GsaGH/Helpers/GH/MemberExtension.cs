using GsaAPI;

using GsaGH.Parameters;

namespace GsaGH.Helpers.GH {
  public static class MemberExtension {
    public static void SetOffsetsFrom(this Member member1, Member member2) {
      member1.Offset.X1 = member2.Offset.X1;
      member1.Offset.X2 = member2.Offset.X2;
      member1.Offset.Y = member2.Offset.Y;
      member1.Offset.Z = member2.Offset.Z;
    }

    public static void SetOffsetForMember(this Member member, GsaOffset offset) {
      member.Offset.X1 = offset.X1.Meters;
      member.Offset.X2 = offset.X2.Meters;
      member.Offset.Y = offset.Y.Meters;
      member.Offset.Z = offset.Z.Meters;
    }

    public static GsaOffset GetOffsetFromMember(this Member member) {
      return new GsaOffset(member.Offset.X1, member.Offset.X2, member.Offset.Y, member.Offset.Z);
    }
  }
}
