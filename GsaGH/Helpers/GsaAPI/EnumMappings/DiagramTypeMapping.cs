using GsaAPI;
using Diagram = GsaGH.Parameters.Enums;

namespace GsaGH.Helpers.GsaAPI.EnumMappings {
  public class DiagramTypeMapping : EnumMapping<DiagramType, Diagram.DiagramType> {

    public DiagramTypeMapping(
      string description, DiagramType gsaApiEnum, Diagram.DiagramType gsaGhEnum) : base(description,
      gsaApiEnum, gsaGhEnum) { }
  }
}
