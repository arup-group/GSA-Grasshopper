using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace DocsGeneratorCLI {
  public class GsaGhProject {

    public Assembly Assembly { get; protected set; }
    public IEnumerable<string> Notes { get; protected set; } = Enumerable.Empty<string>();
    public bool IsBeta { get; protected set; } = false;
    public XmlDocument XmlDoc { get; protected set; }

    public GsaGhProject(string projectName) {
      Assembly = GsaGhDll.LoadGhAndPassAssembly(projectName);

      SetupResultNotes(projectName);
      SetBetaValue(projectName);
      SetXmlDocumentation(projectName);
    }

    public void SetBetaValue(string projectName) {
      Type type = Assembly.GetType($"{projectName}.{projectName}Info");
      var properties = type.GetProperties(BindingFlags.Static | BindingFlags.Public);
      if (properties == null || properties.Length == 0) {
        Console.WriteLine($"Warning no properties found on {projectName}Info, setting IsBeta to true");
        IsBeta = true;
        return;
      }

      bool isBeta = properties.Where(field => field.PropertyType == typeof(bool) && field.Name == "IsBeta")
       .Select(field => (bool)field.GetValue(null)).First();
      IsBeta = isBeta;
    }

    /// <summary>
    ///   Initializes and sets the result notes by retrieving static string fields from the
    ///   'ResultNotes' type within the loaded assembly. It excludes fields with "Assembly" in their name.
    /// </summary>
    /// <param name="projectName"></param>
    /// <exception cref="InvalidOperationException">Thrown if the assembly is not loaded.</exception>
    /// <remarks>
    ///   If the 'ResultNotes' type is not found in the assembly, a warning is logged, and the method execution is halted.
    /// </remarks>
    public void SetupResultNotes(string projectName) {
      if (Assembly == null) {
        throw new InvalidOperationException("Assembly not loaded.");
      }

      Console.WriteLine("Setting up result notes...");
      Type resultNotesType = Assembly.GetType($"{projectName}.jComponents.Helpers.ResultNotes");
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

    // TODO: update for adsecgh
    public void SetXmlDocumentation(string projectName) {
      XmlDoc = GsaGhDll.GsaGhXml;
    }
  }
}
