using System;
using System.IO;
using Grasshopper.Kernel.Types;
using GsaAPI;
using OasysGH;
using OasysGH.Parameters;

namespace GsaGH.Parameters
{
  /// <summary>
  /// Goo wrapper class, makes sure <see cref="GsaList"/> can be used in Grasshopper.
  /// </summary>
  public class GsaListGoo : GH_OasysGoo<GsaList>
  {
    public static string Name => "List";
    public static string NickName => "L";
    public static string Description => "GSA Entity List";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    public GsaListGoo(GsaList item) : base(item) { }
    public override IGH_Goo Duplicate() => new GsaListGoo(this.Value);
  }
}
