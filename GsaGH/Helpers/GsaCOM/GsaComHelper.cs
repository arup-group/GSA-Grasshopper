using System;
using System.IO;
using GsaGH.Parameters;

namespace GsaGH.Helpers {
  public sealed class GsaComObject {
    public static Interop.Gsa_10_1.ComAuto Instance => s_lazy.Value;
    private static readonly Lazy<Interop.Gsa_10_1.ComAuto> s_lazy = new Lazy<Interop.Gsa_10_1.ComAuto>(() => new Interop.Gsa_10_1.ComAuto());

    private GsaComObject() { }
  }

  internal static class GsaComHelper {
    private static Guid s_guid = Guid.NewGuid();
    private static string s_tempPath = Path.GetTempPath() + s_guid.ToString() + ".gwb";

    internal static Interop.Gsa_10_1.ComAuto GetGsaComModel(GsaModel model) {
      Interop.Gsa_10_1.ComAuto gsa = GsaComObject.Instance;

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
      gsa.SetLocale(Interop.Gsa_10_1.Locale.LOC_EN_GB);
      return gsa;
    }

    internal static GsaModel GetGsaGhModel() {
      Interop.Gsa_10_1.ComAuto gsa = GsaComObject.Instance;

      gsa.SaveAs(s_tempPath);
      var gsaGh = new GsaModel();
      gsaGh.Model.Open(s_tempPath);
      return gsaGh;
    }
  }
}
