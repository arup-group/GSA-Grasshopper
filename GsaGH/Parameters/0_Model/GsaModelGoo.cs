using Grasshopper.Kernel.Types;
using OasysGH;
using OasysGH.Parameters;

namespace GsaGH.Parameters {
  /// <summary>
  ///   Goo wrapper class, makes sure <see cref="GsaModel" /> can be used in Grasshopper.
  /// </summary>
  public class GsaModelGoo : GH_OasysGoo<GsaModel> {
    public static string Description => 
      "A GSA model parameter wraps an assembled model containing \n" +
      "all the constituent parts (Properties, Elements/Members, \n" +
      "Loads, Cases, Tasks, etc). \n" +
      "Use the 'CreateModel' or 'Analyse' components to assemble\n" +
      "a new Model or use the 'OpenModel' to work with an existing\n" +
      "Model. You can use the `GetProperties` or `GetGeometry` \n" +
      "to start editing the objects from an existing model.\n" +
      "If the model has been analysed you can use the \n" +
      "'SelectResults'component to explore the Models structural \n" +
      "performance and behaviour.";
    public static string Name => "Model";
    public static string NickName => "GSA";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    public GsaModelGoo(GsaModel item) : base(item) { }

    public override IGH_Goo Duplicate() {
      return new GsaModelGoo(Value);
    }
  }
}
