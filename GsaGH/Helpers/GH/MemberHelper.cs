using GsaAPI;

namespace GsaGH.Helpers.GH {
  public class MemberHelper {
    public const int DefaultGroup = 1;

    public static Member CreateDefaultApiMember(MemberType type) {
      return new Member() {
        Type = type,
        Group = DefaultGroup,
      };
    }
  }
}
