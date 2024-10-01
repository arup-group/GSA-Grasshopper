using GsaAPI;

using Diagram = GsaGH.Parameters;

namespace GsaGH.Helpers.GsaApi.EnumMappings {
  internal class DiagramTypeMapping : EnumMapping<DiagramType, Diagram.ApiDiagramType> {

    public DiagramTypeMapping(
      string description, DiagramType gsaApiEnum, Diagram.ApiDiagramType gsaGhEnum) : base(description,
      gsaApiEnum, gsaGhEnum) { }
  }
}
