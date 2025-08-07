using System;
using System.IO;

using GsaGH.Parameters;

using Interop.Gsa_10_3;

namespace GsaGH.Helpers {
  public sealed class GsaComObject {
    public static ComAuto Instance => lazy.Value;
    private static readonly Lazy<ComAuto> lazy = new Lazy<ComAuto>(() => new ComAuto());
  }

  internal static class GsaComHelper {
    private static Guid guid = Guid.NewGuid();
    private static string tempPath = Path.GetTempPath() + guid.ToString() + ".gwb";

    internal static ComAuto GetGsaComModel(GsaModel model) {
      ComAuto gsa = GsaComObject.Instance;

      if (model == null) {
        gsa.NewFile();
        return gsa;
      }

      if (model.Guid == guid) {
        return gsa;
      }

      if (File.Exists(tempPath)) {
        File.Delete(tempPath);
      }

      guid = model.Guid;
      tempPath = Path.GetTempPath() + guid.ToString() + ".gwb";

      model.ApiModel.SaveAs(tempPath);

      gsa.Open(tempPath);
      gsa.SetLocale(Locale.LOC_EN_GB);
      return gsa;
    }

    internal static GsaModel GetGsaGhModel() {
      ComAuto gsa = GsaComObject.Instance;
      gsa.SaveAs(tempPath);
      var gsaGh = new GsaModel();
      gsaGh.ApiModel.Open(tempPath);

      return gsaGh;
    }

    internal static void Dispose(object sender, EventArgs e) {
      if (File.Exists(tempPath)) {
        File.Delete(tempPath);
      }

      GsaComObject.Instance.Close();
    }
  }
}
