using System;
using System.Collections.Generic;
using System.Reflection;

namespace DocsGeneratorCLI {
  public abstract class ProjectTarget {
    public string Name { get; }

    protected ProjectTarget(string name) {
      Name = name;
    }

    public abstract Assembly LoadAssembly();
    private static readonly Dictionary<string, ProjectTarget> _lookup = CreateLookup();

    private static Dictionary<string, ProjectTarget> CreateLookup() {
      var dict = new Dictionary<string, ProjectTarget>(StringComparer.OrdinalIgnoreCase) {
        { "gsagh", new GsaGhProject() },
        { "adsecgh", new AdSecGhProject() },
      };
      return dict;
    }

    public static ProjectTarget FromString(string input) {
      if (string.IsNullOrWhiteSpace(input)) {
        throw new ArgumentException("Project name cannot be null or empty.");
      }

      return _lookup.TryGetValue(input.Trim(), out ProjectTarget target) ? target :
        throw new ArgumentException($"Unsupported project: {input}");
    }

    public static IEnumerable<string> GetAvailableProjectNames() {
      return _lookup.Keys;
    }
  }

  public class GsaGhProject : ProjectTarget {
    public GsaGhProject() : base("GsaGH") { }

    public override Assembly LoadAssembly() {
      Console.WriteLine("Loading GsaGH assembly...");
      return new GsaGhDll().Load();
    }
  }

  public class AdSecGhProject : ProjectTarget {
    public AdSecGhProject() : base("AdSecGH") { }

    public override Assembly LoadAssembly() {
      Console.WriteLine("Loading AdSecGH assembly...");
      // placeholder 
      return new GsaGhDll().Load();
    }
  }
}
