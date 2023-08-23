using System;
using System.Collections.Generic;
using System.Reflection;
using DocsGeneration.Helpers;
using GsaGhDocs.Data;

namespace DocsGeneration {
  public class Program {
    public static void Main() {
      var dll = new GsaGhDll();
      Assembly gsaGH = dll.Load();
      Type[] typelist = gsaGH.GetTypes();

      List<Component> components = Component.GetComponents(typelist);
      List<Parameter> parameters = Parameter.GetParameters(typelist, components);

      MarkDowns.Components.CreateOverview(Component.SortComponents(components), parameters);
      MarkDowns.Components.CreateComponents(components, parameters);
      MarkDowns.Parameters.CreateParameters(parameters);
      MarkDowns.Parameters.CreateOverview(Parameter.SortParameters(parameters));
    }
  }
}

