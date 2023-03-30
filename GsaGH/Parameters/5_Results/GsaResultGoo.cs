using Grasshopper.Kernel.Types;
using OasysGH;
using OasysGH.Parameters;

namespace GsaGH.Parameters {

  /// <summary>
  ///   Goo wrapper class, makes sure <see cref="GsaResult" /> can be used in Grasshopper.
  /// </summary>
  public class GsaResultGoo : GH_OasysGoo<GsaResult> {

    #region Properties + Fields
    public static string Description => "GSA Result";
    public static string Name => "Result";
    public static string NickName => "Res";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    #endregion Properties + Fields

    #region Public Constructors
    public GsaResultGoo(GsaResult item) : base(item) {
    }

    #endregion Public Constructors

    #region Public Methods
    public override IGH_Goo Duplicate() => this;

    #endregion Public Methods
  }
}
