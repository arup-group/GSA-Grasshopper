using Grasshopper.Kernel.Types;
using OasysGH;
using OasysGH.Parameters;

namespace GsaGH.Parameters {

  /// <summary>
  /// Goo wrapper class, makes sure <see cref="GsaModel"/> can be used in Grasshopper.
  /// </summary>
  public class GsaModelGoo : GH_OasysGoo<GsaModel> {

    #region Properties + Fields
    public static string Description => "GSA Model";
    public static string Name => "Model";
    public static string NickName => "GSA";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    #endregion Properties + Fields

    #region Public Constructors
    public GsaModelGoo(GsaModel item) : base(item) {
    }

    #endregion Public Constructors

    #region Public Methods
    public override IGH_Goo Duplicate() => new GsaModelGoo(Value);

    #endregion Public Methods
  }
}
