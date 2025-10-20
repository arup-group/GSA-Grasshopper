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
        return 0;
      } catch (Exception e) {
        Console.WriteLine($"Failed to generate documentation.\nStackTrace: {e.StackTrace}");
        return -1;
      }
    }
  }

  public class Configuration {
    private static readonly Lazy<Configuration> _instance = new Lazy<Configuration>(() => new Configuration());

    public static Configuration Instance => _instance.Value;
    public bool GenerateE2ETestData { get; private set; } = false;
    public string CustomOutputPath { get; private set; } = "Output";
    public Assembly Assembly { get; private set; }
    public List<string> ResultNotes { get; private set; } = new List<string>();
    public bool IsBeta { get; private set; } = false;
    public XmlDocument XmlDocument { get; private set; }

    public Configuration SetGenerateE2ETestData(bool value) {
      GenerateE2ETestData = value;
      return this;
    }

    public Configuration SetCustomOutputPath(string path) {
      CustomOutputPath = path;
      return this;
    }

    public Configuration SetAssembly(Assembly assembly) {
      Assembly = assembly;
      return this;
    }

    public Configuration SetResultNotes(List<string> resultNotes) {
      ResultNotes = resultNotes;
      return this;
    }

    public Configuration SetIsBeta(bool isBeta) {
      IsBeta = isBeta;
      return this;
    }

    public Configuration SetXml(XmlDocument xmlDoc) {
      XmlDocument = xmlDoc;
      return this;
    }
  }
}
