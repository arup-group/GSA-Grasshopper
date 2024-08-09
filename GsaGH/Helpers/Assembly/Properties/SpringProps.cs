using System.Collections.Generic;
using System.Linq;

using GsaAPI;

using GsaGH.Parameters;

namespace GsaGH.Helpers.Assembly {
  internal partial class ModelAssembly {
    private int AddSpringProp(GsaSpringProperty prop) {
      SpringProperty api = prop.DuplicateApiObject();

      if (prop.Id <= 0) {
        return _springProperties.AddValue(prop.Guid, api);
      }

      _springProperties.SetValue(prop.Id, prop.Guid, api);
      return prop.Id;
    }

    private int ConvertSpringProp(GsaSpringProperty prop) {
      if (prop == null) {
        return 0;
      }

      if (prop.IsReferencedById || prop.ApiProperty == null) {
        return prop.Id;
      }

      return AddSpringProp(prop);
    }

    private void ConvertSpringProps(List<GsaSpringProperty> springProps) {
      if (springProps == null) {
        return;
      }

      springProps = springProps.OrderByDescending(p => p.Id).ToList();
      foreach (GsaSpringProperty prop in springProps.Where(prop => prop != null)) {
        ConvertSpringProp(prop);
      }
    }
  }
}
