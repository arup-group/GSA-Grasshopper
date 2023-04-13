using System;
using System.IO;
using GsaGH.Parameters;
using Interop.Gsa_10_1;

namespace GsaGH.Helpers {
  public sealed class GsaComObject {
    public static ComAuto Instance => s_lazy.Value;
    private static readonly Lazy<ComAuto> s_lazy = new Lazy<ComAuto>(() => new ComAuto());

    private GsaComObject() { }
  }

  internal static class GsaComHelper {
    private static Guid s_guid = Guid.NewGuid();
    private static string s_tempPath = Path.GetTempPath() + s_guid.ToString() + ".gwb";

    internal static ComAuto GetGsaComModel(GsaModel model) {
      ComAuto gsa = GsaComObject.Instance;

      if (model == null) {
        gsa.NewFile();
        return gsa;
      }

      if (model.Guid == s_guid) {
        return gsa;
      }

      s_guid = model.Guid;
      s_tempPath = Path.GetTempPath() + s_guid.ToString() + ".gwb";

      model.Model.SaveAs(s_tempPath);

      gsa.Open(s_tempPath);
      gsa.SetLocale(Locale.LOC_EN_GB);
      return gsa;
    }

    internal static GsaModel GetGsaGhModel() {
      ComAuto gsa = GsaComObject.Instance;

      gsa.SaveAs(s_tempPath);
      var gsaGh = new GsaModel();
      gsaGh.Model.Open(s_tempPath);
      return gsaGh;
    }
  }
}
