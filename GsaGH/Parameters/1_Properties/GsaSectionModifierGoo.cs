using System;
using System.Collections.Generic;
using System.Linq;
using GsaAPI;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using OasysUnits;
using OasysUnits.Units;
using OasysGH.Units;
using OasysGH;
using OasysGH.Parameters;

namespace GsaGH.Parameters
{
  /// <summary>
  /// Goo wrapper class, makes sure <see cref="GsaSectionModifier"/> can be used in Grasshopper.
  /// </summary>
  public class GsaSectionModifierGoo : GH_OasysGoo<GsaSectionModifier>
  {
    public static string Name => "Section Modifier";
    public static string NickName => "PBM";
    public static string Description => "GSA Section Modifier";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    public GsaSectionModifierGoo(GsaSectionModifier item) : base(item) { }
    public override IGH_Goo Duplicate() => new GsaSectionModifierGoo(this.Value);
  }
}
