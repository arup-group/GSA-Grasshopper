using System;
using System.Collections.Generic;
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
        Type[] typelist = config.Assembly.GetTypes();
        List<Component> components = Component.GetComponents(typelist, config);
        List<Parameter> parameters = Parameter.GetParameters(typelist, components, config);

        // write individual files
        Components.CreateComponents(components, parameters, config);
        Parameters.CreateParameters(parameters, config);

        // write overview files
        Dictionary<string, List<Component>> sortedComponents = Component.SortComponents(components);
        Dictionary<string, List<Parameter>> sortedParameters = Parameter.SortParameters(parameters);
        Components.CreateOverview(sortedComponents, parameters, config);
        Parameters.CreateOverview(sortedParameters, config);

        // write sidebar
        SideBar.CreateSideBar(sortedComponents, sortedParameters);
        FileHelper.WriteIconNames();
        return 0;
      } catch (Exception e) {
        Console.WriteLine($"Failed to generate documentation.\nStackTrace: {e.StackTrace}");
        return -1;
      }
    }
  }

  public struct Configuration {
    public bool GenerateE2ETestData { get; set; }
    public string CustomOutputPath { get; set; }
    public Assembly Assembly { get; set; }
    public List<string> ResultNotes { get; set; }
    public bool IsBeta { get; set; }
    public XmlDocument XmlDocument { get; set; }
  }
}
