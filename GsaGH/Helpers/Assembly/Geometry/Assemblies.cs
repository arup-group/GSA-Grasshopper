using System;
using System.Collections.Generic;
using System.Linq;

using GsaGH.Parameters;

namespace GsaGH.Helpers.Assembly {
  internal partial class ModelAssembly {
    private void AddAssembly(int id, Guid guid, GsaAPI.Assembly apiAssembly) {
      if (id > 0) {
        _assemblies.SetValue(id, guid, apiAssembly);
      } else {
        _assemblies.AddValue(guid, apiAssembly);
      }
    }

    private void ConvertAssemblies(List<GsaAssembly> assemblies) {
      assemblies = assemblies.OrderByDescending(x => x.Id).ToList();
      foreach (GsaAssembly assembly in assemblies) {
        ConvertAssembly(assembly);
      }
    }

    private void ConvertAssembly(GsaAssembly assembly) {
      GsaAPI.Assembly apiAssembly = assembly.DuplicateApiObject();

      AddAssembly(assembly.Id, assembly.Guid, apiAssembly);
    }
  }
}
