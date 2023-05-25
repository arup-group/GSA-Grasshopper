namespace GsaGH.Parameters {
  public static class TypeName {
    public static string GetTypeName(this object value) {
      return value.GetType().Name.
        Replace("GH_", string.Empty).Replace("String", "Text").
        Replace("GSA ", string.Empty).Replace("Gsa", string.Empty).
        Replace("Goo", string.Empty);
    }
  }
}
