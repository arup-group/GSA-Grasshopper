using System;
using System.Collections.Generic;

using GsaAPI;

using GsaGH.Helpers;

namespace GsaGH.Parameters {
  /// <summary>
  /// Assemblies are a way to define an entity that is formed from a collection of elements or members and can be thought of as a superelement. This is not an analysis entity but rather a convenience for post-processing, such as cut section forces. Typical uses of assemblies include cores, where the core is modelled with 2D finite elements; trusses, where the truss is modelled with top and bottom chords; and bracing. In both these cases the assembly is identified by a list of included elements.
  /// Unlike the analysis elements, an assembly does not have a clearly define orientation and location of reference point so these must be defined explicitly.
  /// <para>Refer to <see href="https://docs.oasys-software.com/structural/gsa/references/hidr-data-assembly.html">Assemblies</see> to read more.</para>
  /// </summary>
  public class GsaAssembly {
    public Guid Guid { get; set; } = Guid.NewGuid();
    public int Id { get; set; } = 0;
    public Assembly ApiAssembly { get; internal set; }

    public GsaAssembly() {
      ApiAssembly = new AssemblyByNumberOfPoints("", 0, 0, 0);
    }

    public GsaAssembly(GsaAssembly other) {
      Id = other.Id;
      ApiAssembly = other.DuplicateApiObject();
    }

    internal GsaAssembly(Assembly assembly) {
      ApiAssembly = assembly;
    }

    internal GsaAssembly(KeyValuePair<int, Assembly> assembly) {
      Id = assembly.Key;
      ApiAssembly = assembly.Value;
    }

    public override string ToString() {
      string id = Id > 0 ? $"ID:{Id}" : string.Empty;
      string name = ApiAssembly.Name != "" ? $"Name:{ApiAssembly.Name}" : string.Empty;
      string type = $"Entity Type:{ApiAssembly.EntityType}";
      string entityList = $"Entity List:{ApiAssembly.EntityList}";
      string topology = $"Topology:{ApiAssembly.Topology1} {ApiAssembly.Topology2}";
      string definition = string.Empty;

      switch (ApiAssembly) {
        case AssemblyByExplicitPositions byExplicitPositions:
          definition += "Explicit definition:";
          foreach (double position in byExplicitPositions.Positions) {
            definition += position + ", ";
          }
          definition = definition.Substring(0, definition.Length - 2);
          break;

        case AssemblyByNumberOfPoints byNumberOfPoints:
          definition += $"Definition by points:{byNumberOfPoints.NumberOfPoints}";
          break;

        case AssemblyBySpacingOfPoints bySpacingOfPoints:
          definition += $"Definition by spacing:{bySpacingOfPoints.Spacing}m";
          break;

        case AssemblyByStorey byStorey:
          definition += $"Definition by storey:'{byStorey.StoreyList}'";
          break;
      }

      return string.Join(" ", id, name, type, entityList, topology, definition).TrimSpaces();
    }

    internal Assembly DuplicateApiObject() {
      string name = ApiAssembly.Name;
      int topology1 = ApiAssembly.Topology1;
      int topology2 = ApiAssembly.Topology2;
      int orientationNode = ApiAssembly.OrientationNode;

      Assembly assembly;
      switch (ApiAssembly) {
        case AssemblyByExplicitPositions explicitPositions:
          assembly = new AssemblyByExplicitPositions(name, topology1, topology2, orientationNode, explicitPositions.InternalTopology, explicitPositions.CurveFit) {
            Positions = explicitPositions.Positions
          };
          break;

        case AssemblyByNumberOfPoints numberOfPoints:
          assembly = new AssemblyByNumberOfPoints(name, topology1, topology2, orientationNode, numberOfPoints.InternalTopology, numberOfPoints.CurveFit) {
            NumberOfPoints = numberOfPoints.NumberOfPoints
          };
          break;

        case AssemblyBySpacingOfPoints spacingOfPoints:
          assembly = new AssemblyBySpacingOfPoints(name, topology1, topology2, orientationNode, spacingOfPoints.InternalTopology, spacingOfPoints.CurveFit) {
            Spacing = spacingOfPoints.Spacing
          };
          break;

        case AssemblyByStorey byStorey:
          assembly = new AssemblyByStorey(name, topology1, topology2, orientationNode) {
            StoreyList = byStorey.StoreyList
          };
          break;

        default:
          return null;
      }

      assembly.EntityList = ApiAssembly.EntityList;
      assembly.EntityType = ApiAssembly.EntityType;
      assembly.ExtentY = ApiAssembly.ExtentY;
      assembly.ExtentZ = ApiAssembly.ExtentZ;

      return assembly;
    }
  }
}
