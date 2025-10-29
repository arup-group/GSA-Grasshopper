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
    protected abstract void SetBetaValue();
    protected abstract void SetXmlDocumentation();
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
        target.SetBetaValue();
        target.SetXmlDocumentation();
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

    protected override void SetBetaValue() {
      Type type = Assembly.GetType("GsaGH.GsaGhInfo");
      bool isBeta = type.GetProperties(BindingFlags.Static | BindingFlags.Public)
       .Where(field => field.PropertyType == typeof(bool) && field.Name == "IsBeta")
       .Select(field => (bool)field.GetValue(null)).First();
      IsBeta = isBeta;
    }

    protected override Assembly LoadAssembly() {
      if (Assembly != null) {
        return Assembly;
      }

      Console.WriteLine("Loading GsaGH assembly...");
      Assembly = GsaGhDll.Load();
      return Assembly;
    }

    /// <summary>
    ///   Initializes and sets the result notes by retrieving static string fields from the
    ///   'ResultNotes' type within the loaded assembly. It excludes fields with "Assembly" in their name.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the assembly is not loaded.</exception>
    /// <remarks>
    ///   If the 'ResultNotes' type is not found in the assembly, a warning is logged, and the method execution is halted.
    /// </remarks>
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
#pragma warning disable S3011 // Reflection should not be used to increase accessibility of classes, methods, or fields
      FieldInfo[] noteFields = resultNotesType.GetFields(BindingFlags.NonPublic | BindingFlags.Static);
#pragma warning restore S3011 // Reflection should not be used to increase accessibility of classes, methods, or fields

      IEnumerable<FieldInfo> orderedFields = GetOrderedFields(noteFields);
      notes.AddRange(orderedFields.Select(field => (string)field.GetValue(null)));

      Notes = notes;
    }

    private static IEnumerable<FieldInfo> GetOrderedFields(IEnumerable<FieldInfo> noteFields) {
      var desiredFieldNames = new List<string> // we must have it in this order!
      {
        "NoteNodeResults",
        "Note1dResults",
        "Note2dForceResults",
        "Note2dStressResults",
        "Note3dStressResults",
        "Note2dResults",
      };

      // creating a dictionary for quick lookup
      var fieldsDict = noteFields.ToDictionary(field => field.Name);

      // return the fields in the desired order
      return desiredFieldNames.Select(name
        => fieldsDict.TryGetValue(name, out FieldInfo field) ? field :
          throw new InvalidOperationException($"Field '{name}' not found in the assembly.")).ToList();
    }

    protected override void SetXmlDocumentation() {
      XmlDoc = GsaGhDll.GsaGhXml;
    }
  }

  public class AdSecGhProject : ProjectTarget {
    public AdSecGhProject() : base("AdSecGH") { }

    protected override Assembly LoadAssembly() {
      throw new NotImplementedException();
    }

    protected override void SetBetaValue() {
      throw new NotImplementedException();
    }

    protected override void SetupResultNotes() {
      throw new NotImplementedException();
    }

    protected override void SetXmlDocumentation() {
      throw new NotImplementedException();
    }
  }
}
