using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace DocsGeneratorCLI {
  public abstract class ProjectTarget {
    public string Name { get; }
    public Assembly Assembly { get; protected set; }
    public IEnumerable<string> Notes { get; protected set; } = Enumerable.Empty<string>();
    public bool IsBeta { get; protected set; } = false;
    public XmlDocument XmlDoc { get; protected set; }

    protected ProjectTarget(string name) {
      Name = name;
    }

    protected abstract Assembly LoadAssembly();
    protected abstract void SetupResultNotes();
    protected abstract bool GetBetaValue();
    private static readonly Dictionary<string, ProjectTarget> _lookup = CreateLookup();

    private static Dictionary<string, ProjectTarget> CreateLookup() {
      var dict = new Dictionary<string, ProjectTarget>(StringComparer.OrdinalIgnoreCase) {
        { "gsagh", new GsaGhProject() },
        { "adsecgh", new AdSecGhProject() },
      };
      return dict;
    }

    public static ProjectTarget LoadProjectTargetFromString(string input) {
      if (string.IsNullOrWhiteSpace(input?.Trim())) {
        throw new ArgumentException("Project name cannot be null or empty.");
      }

      _lookup.TryGetValue(input, out ProjectTarget target);
      if (target != null) {
        target.LoadAssembly();
        target.SetupResultNotes();
        target.GetBetaValue();
        target.XmlDoc = GsaGhDll.GsaGhXml;
      } else {
        throw new KeyNotFoundException($"Unsupported project: {input}");
      }

      return target;
    }

    public static IEnumerable<string> GetAvailableProjectNames() {
      return _lookup.Keys;
    }
  }

  public class GsaGhProject : ProjectTarget {
    public GsaGhProject() : base("GsaGH") { }

    protected override bool GetBetaValue() {
      Type type = Assembly.GetType("GsaGH.GsaGhInfo");
      bool isBeta = type.GetFields(BindingFlags.Static)
       .Where(field => field.FieldType == typeof(bool) && field.Name == "isBeta")
       .Select(field => (bool)field.GetValue(null)).First();
      return isBeta;
    }

    protected override Assembly LoadAssembly() {
      if (Assembly != null) {
        return Assembly;
      }

      Console.WriteLine("Loading GsaGH assembly...");
      Assembly = new GsaGhDll().Load();
      return Assembly;
    }

    protected override void SetupResultNotes() {
      if (Assembly == null) {
        throw new InvalidOperationException("Assembly not loaded.");
      }

      Console.WriteLine("Setting up result notes...");
      Type resultNotesType = Assembly.GetType("GsaGH.Components.Helpers.ResultNotes");
      if (resultNotesType == null) {
        Console.WriteLine("Warning: ResultNotes type not found in assembly.");
        return;
      }

      var notes = new List<string>();
      FieldInfo[] noteFields = resultNotesType.GetFields(BindingFlags.Public | BindingFlags.Static);
      notes.AddRange(noteFields.Where(field => field.FieldType == typeof(string))
       .Select(field => (string)field.GetValue(null)));
      Notes = notes;
    }
  }

  public class AdSecGhProject : ProjectTarget {
    public AdSecGhProject() : base("AdSecGH") { }

    protected override bool GetBetaValue() {
      throw new NotImplementedException();
    }

    protected override Assembly LoadAssembly() {
      Console.WriteLine("Loading AdSecGH assembly...");
      // placeholder 
      return new GsaGhDll().Load();
    }

    protected override void SetupResultNotes() {
      Notes = Array.Empty<string>();
    }
  }
}
