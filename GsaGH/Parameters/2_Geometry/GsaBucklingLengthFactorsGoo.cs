﻿using Grasshopper.Kernel.Types;
using OasysGH;
using OasysGH.Parameters;

namespace GsaGH.Parameters {
  /// <summary>
  ///   Goo wrapper class, makes sure <see cref="GsaBucklingLengthFactors" /> can be used in Grasshopper.
  /// </summary>
  public class GsaBucklingLengthFactorsGoo : GH_OasysGoo<GsaBucklingLengthFactors> {
    public static string Description => "GSA Equivalent uniform moment factor for LTB for 1D Member";
    public static string Name => "BucklingFactors";
    public static string NickName => "BFs";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    public GsaBucklingLengthFactorsGoo(GsaBucklingLengthFactors item) : base(item) { }

    public override IGH_Goo Duplicate() {
      return new GsaBucklingLengthFactorsGoo(Value);
    }
  }
}
