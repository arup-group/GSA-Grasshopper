using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;

using DocsGeneration.Data;
using DocsGeneration.MarkDowns;
using DocsGeneration.MarkDowns.Helpers;

namespace DocsGeneration {
  public static class GenerateDocumentation {

    public static int Generate(Configuration config) {
      try {
        // reflect
        Console.WriteLine($"Assembly: {config.Assembly.FullName}");

        foreach (var references in config.Assembly.GetReferencedAssemblies()) {
          Console.WriteLine($"Referenced Assembly: {references}");
        }
        
        try
        {
          foreach (var type in config.Assembly.DefinedTypes) {
            Console.WriteLine($" - {type.FullName}");
          }
        }
        catch (ReflectionTypeLoadException ex)
        {
          foreach (var e in ex.LoaderExceptions) {
            Console.WriteLine($"Loader exception: {e.Message}");
          }
        }
        Type[] types = config.Assembly.GetTypes();
        List<Component> components = Component.GetComponents(types, config);
        List<Parameter> parameters = Parameter.GetParameters(types, components, config);

        // write individual files
        Components.CreateComponents(components, parameters, config);
        Parameters.CreateParameters(parameters, config);

        // write overview files
        Dictionary<string, List<Component>> sortedComponents = Component.SortComponents(components);
        Dictionary<string, List<Parameter>> sortedParameters = Parameter.SortParameters(parameters);
        Components.CreateOverview(sortedComponents, parameters, config);
        Parameters.CreateOverview(sortedParameters, config);

        // write sidebar
        SideBar.CreateSideBar(sortedComponents, sortedParameters, config);
        FileHelper.WriteIconNames(config.OutputPath);
        return 0;
      } catch (Exception e) {
        Console.WriteLine($"Failed to generate documentation.\nStackTrace: {e.StackTrace}");
        return -1;
      }
    }
  }

  public struct Configuration {
    public bool GenerateE2ETestData { get; set; }
    public string OutputPath { get; set; }
    public Assembly Assembly { get; set; }
    public List<string> ResultNotes { get; set; }
    public bool IsBeta { get; set; }
    public XmlDocument XmlDocument { get; set; }
  }
}
