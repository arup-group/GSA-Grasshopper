using System;
using System.Drawing;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Helpers.GH;
using GsaGH.Properties;
using OasysGH;
using OasysGH.Components;

namespace GsaGH.Components {

  /// <summary>
  ///   Component to create a new Section
  /// </summary>
  public class TaperProfile : GH_OasysComponent {

    #region Properties + Fields
    public override Guid ComponentGuid => new Guid("fd6dd254-c16f-4970-a447-a9b258d116ef");
    public override GH_Exposure Exposure => GH_Exposure.secondary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.TaperProfile;
    #endregion Properties + Fields

    #region Public Constructors
    public TaperProfile() : base("Taper Profile",
      "Taper",
      "Create a Profile that tapers along its length from start and end profiles",
      CategoryName.Name(),
      SubCategoryName.Cat1())
      => Hidden = true;

    #endregion Public Constructors

    #region Protected Methods
    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddTextParameter("Profile Start",
        "Pf1",
        "Profile at start of element",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Profile End",
        "Pf2",
        "Profile at end of element",
        GH_ParamAccess.item);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
      => pManager.AddTextParameter("Tapered Profile",
        "Pf",
        "Profile tapering along the length of its element",
        GH_ParamAccess.item);

    protected override void SolveInstance(IGH_DataAccess da) {
      var ghProfile = new GH_String();
      da.GetData(0, ref ghProfile);
      GH_Convert.ToString(ghProfile, out string start, GH_Conversion.Both);

      ghProfile = new GH_String();
      da.GetData(1, ref ghProfile);
      GH_Convert.ToString(ghProfile, out string end, GH_Conversion.Both);

      string[] startParts = start.Split(' ');
      string[] endParts = end.Split(' ');

      if (startParts[0] != endParts[0]) {
        this.AddRuntimeError("Start and End Profile types must be similar");
        return;
      }

      if (startParts[0] == "STD" || startParts[0] == "GEO") {
        if (startParts[1] == endParts[1]) {
          if (startParts[0] == "GEO") {
            if (!startParts[1]
              .StartsWith("P")) {
              this.AddRuntimeError(
                "It is only possible to taper between two Perimeter / bridge profiles");
              return;
            }

            if (startParts.Length != endParts.Length) {
              this.AddRuntimeError("Start and End Profile must contain similar number of points");
              return;
            }
          }

          string taper = startParts[0] + " " + startParts[1];
          for (int i = 2; i < startParts.Length; i++)
            taper = taper + " " + startParts[i];

          taper += " :";
          for (int i = 2; i < endParts.Length; i++)
            taper = taper + " " + endParts[i];

          da.SetData(0, taper);
        }
        else
          this.AddRuntimeError("Start and End Profile types must be similar");
      }
      else
        this.AddRuntimeError("Profile type must be 'STD'");
    }

    #endregion Protected Methods
  }
}
