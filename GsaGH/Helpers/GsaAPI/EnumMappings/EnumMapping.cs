namespace GsaGH.Helpers.GsaApi.EnumMappings {
  public class EnumMapping<T1, T2> {
    public string Description { get; private set; }
    public T1 GsaApiEnum { get; private set; }
    public T2 GsaGhEnum { get; private set; }

    public EnumMapping(string description, T1 gsaApiEnum, T2 gsaGhEnum) {
      Description = description;
      GsaApiEnum = gsaApiEnum;
      GsaGhEnum = gsaGhEnum;
    }
  }
}
