using GsaAPI;
using GsaGH.Parameters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GsaGH.Helpers
{
  public sealed class GsaComObject
  {
    private static readonly Lazy<Interop.Gsa_10_1.ComAuto> lazy = new Lazy<Interop.Gsa_10_1.ComAuto>(() => new Interop.Gsa_10_1.ComAuto());
    public static Interop.Gsa_10_1.ComAuto Instance { get { return lazy.Value; } }
    private GsaComObject() { }
  }

  internal static class GsaComHelper
  {
    private static Guid _guid = Guid.NewGuid();
    private static string _tempPath = Path.GetTempPath() + _guid.ToString() + ".gwb";
    internal static Interop.Gsa_10_1.ComAuto GetGsaComModel(GsaModel model)
    {
      Interop.Gsa_10_1.ComAuto GSA = GsaComObject.Instance;

      if (model == null)
      {
        GSA.NewFile();
        return GSA;
      }
      if (model.Guid == _guid)
        return GSA;

      _guid = model.Guid;
      _tempPath = Path.GetTempPath() + _guid.ToString() + ".gwb";

      model.Model.SaveAs(_tempPath);

      GSA.Open(_tempPath);
      GSA.SetLocale(Interop.Gsa_10_1.Locale.LOC_EN_GB);
      return GSA;
    }

    internal static GsaModel GetGsaGhModel()
    {
      Interop.Gsa_10_1.ComAuto GSA = GsaComObject.Instance;

      GSA.SaveAs(_tempPath);
      GsaModel gsaGH = new GsaModel();
      gsaGH.Model.Open(_tempPath);
      return gsaGH;
    }
  }
}
