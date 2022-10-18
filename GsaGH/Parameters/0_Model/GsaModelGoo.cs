﻿using System;
using System.IO;
using Grasshopper.Kernel.Types;
using GsaAPI;
using OasysGH;
using OasysGH.Parameters;

namespace GsaGH.Parameters
{
  /// <summary>
  /// Goo wrapper class, makes sure <see cref="GsaModel"/> can be used in Grasshopper.
  /// </summary>
  public class GsaModelGoo : GH_OasysGoo<GsaModel>
  {
    public static string Name => "Model";
    public static string NickName => "GSA";
    public static string Description => "GSA Model";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    public GsaModelGoo(GsaModel item) : base(item) { }
    public override IGH_Goo Duplicate() => new GsaModelGoo(this.Value);
  }
}
