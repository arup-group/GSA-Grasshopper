using System;
using GsaAPI;

namespace GsaGH.Parameters {
  /// <summary>
  /// Assemblies are a way to define an entity that is formed from a collection of elements or members and can be thought of as a super-element. This is not an analysis entity but just a convenience for post-processing such as cut section forces. Typical uses of assemblies include cores, where the core is modelled with 2D finite elements, or trusses where the truss is modelled with top and bottom chords and bracing. In both these cases the assembly is identified by a list of included elements.
  /// Unlike the analysis elements, an assembly does not have a clearly define orientation and location of reference point so these must be defined explicitly.
  /// <para>Refer to <see href="https://docs.oasys-software.com/structural/gsa/references/hidr-data-assembly/index.html">Assemblies</see> to read more.</para>
  /// </summary>
  public class GsaAssembly {
    public Guid Guid { get; set; } = Guid.NewGuid();
    public int Id { get; set; } = 0;
    public SpringProperty ApiAssembly { get; internal set; }

    public GsaAssembly() {
      //ApiAssembly = new AssemblyByNumberOfPoints();
    }

    public GsaAssembly(GsaAssembly other) {
      Id = other.Id;
      //ApiAssembly = other.DuplicateApiObject();
    }

    //internal GsaAssembly(Assembly assembly) {
    //  ApiAssembly = assembly;
    //}

    //internal GsaAssembly(KeyValuePair<int, Assembly> assembly) {
    //  Id = assembly.Key;
    //  ApiAssembly = assembly.Value;
    //}

    //public Assembly DuplicateApiObject() {

    //}

    public override string ToString() {
      return "";
    }
  }
}
