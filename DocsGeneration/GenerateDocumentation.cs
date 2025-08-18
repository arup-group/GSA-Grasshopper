using System;
using System.Collections.Generic;
using System.Reflection;

using DocsGeneration.Data;
using DocsGeneration.MarkDowns;
using DocsGeneration.MarkDowns.Helpers;

namespace DocsGeneration {
  public static class GenerateDocumentation {

    public static void Generate(Configuration config) {
      // reflect
      Type[] typelist = config.Assembly.GetTypes();
      List<Component> components = Component.GetComponents(typelist);
      List<Parameter> parameters = Parameter.GetParameters(typelist, components);

      // write individual files
      Components.CreateComponents(components, parameters);
      Parameters.CreateParameters(parameters);

      // write overview files
      Dictionary<string, List<Component>> sortedComponents = Component.SortComponents(components);
      Dictionary<string, List<Parameter>> sortedParameters = Parameter.SortParameters(parameters);
      Components.CreateOverview(sortedComponents, parameters);
      Parameters.CreateOverview(sortedParameters);

      // write sidebar
      SideBar.CreateSideBar(sortedComponents, sortedParameters);
      FileHelper.WriteIconNames();
    }
  }

  public class Configuration {
    public bool GenerateE2ETestData { get; set; } = false;
    public string CustomOutputPath { get; set; } = "Output";

    public Assembly Assembly { get; set; }
  }
}
