﻿using System;
using System.IO;
using GsaGH.Parameters;
using Interop.Gsa_10_1;

namespace GsaGH.Helpers {
  public sealed class GsaComObject {
    private static readonly Lazy<ComAuto> lazy = new Lazy<ComAuto>(() => new ComAuto());
    public static ComAuto Instance => lazy.Value;

    private GsaComObject() { }
  }

  internal static class GsaComHelper {
    private static Guid guid = Guid.NewGuid();
    private static string tempPath = $"{Path.GetTempPath()}{guid}.gwb";

    internal static ComAuto GetGsaComModel(GsaModel model) {
      ComAuto gsa = GsaComObject.Instance;

      if (model == null) {
        gsa.NewFile();
        return gsa;
      }

      if (model.Guid == guid) {
        return gsa;
      }

      guid = model.Guid;
      tempPath = $"{Path.GetTempPath()}{guid.ToString()}.gwb";

      model.Model.SaveAs(tempPath);

      gsa.Open(tempPath);
      gsa.SetLocale(Locale.LOC_EN_GB);
      return gsa;
    }

    internal static GsaModel GetGsaGhModel() {
      ComAuto gsa = GsaComObject.Instance;

      gsa.SaveAs(tempPath);
      var gsaGh = new GsaModel();
      gsaGh.Model.Open(tempPath);
      return gsaGh;
    }
  }
}
