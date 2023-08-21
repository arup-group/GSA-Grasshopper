using GsaGhDocs.Components;
using GsaGhDocs.Parameters;
using GsaGhDocs.Helpers;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace GsaGhDocs {
  public class Program {
    public static void Main() {
      var dll = new GsaGhDll();
      Assembly gsaGH = dll.Load();
      Type[] typelist = gsaGH.GetTypes();

      List<Component> components = Component.GetComponents(typelist);
      List<Parameter> parameters = Parameter.GetParameters(typelist, components);
    }
  }
}

