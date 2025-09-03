using System;
using System.Collections.Generic;
using System.Reflection;

using DocsGeneration.Data;
using DocsGeneration.Helpers;
using DocsGeneration.MarkDowns;
using DocsGeneration.MarkDowns.Helpers;

namespace DocsGeneration {
  public class Program {
    public static int Main() {
      // setup
      var dll = new GsaGhDll();
      Assembly gsaGH = dll.Load();

      // reflect
      Type[] typelist = gsaGH.GetTypes();
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
    }
  }
}
