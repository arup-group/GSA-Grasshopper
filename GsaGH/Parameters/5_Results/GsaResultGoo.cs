﻿using Grasshopper.Kernel.Types;
using OasysGH;
using OasysGH.Parameters;

namespace GsaGH.Parameters {
  /// <summary>
  ///   Goo wrapper class, makes sure <see cref="GsaResult" /> can be used in Grasshopper.
  /// </summary>
  public class GsaResultGoo : GH_OasysGoo<GsaResult> {
    public static string Description => "GSA Result";
    public static string Name => "Result";
    public static string NickName => "Res";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    public GsaResultGoo(GsaResult item) : base(item) { }

    public override IGH_Goo Duplicate() => this;
  }
}
