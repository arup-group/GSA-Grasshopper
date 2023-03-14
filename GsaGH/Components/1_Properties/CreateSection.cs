using System;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using OasysGH;
using OasysGH.Components;

namespace GsaGH.Components {
  /// <summary>
  /// Component to create a new Section
  /// </summary>
  public class CreateSection : GH_OasysComponent {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("1167c4aa-b98b-47a7-ae85-1a3c976a1973");
    public override GH_Exposure Exposure => GH_Exposure.primary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon =>
      Properties.Resources.CreateSection;

    public CreateSection() : base("Create Section",
      "Section",
      "Create GSA Section",
      CategoryName.Name(),
      SubCategoryName.Cat1()) {
        Hidden = true;
    } // sets the initial state of the component to hidden
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddTextParameter("Profile", "Pf", "Cross-Section Profile defined using the GSA Profile string syntax", GH_ParamAccess.item);
      pManager.AddParameter(new GsaMaterialParameter());
      pManager[1].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaSectionParameter());
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess da) {
      var gsaSection = new GsaSection();

      // profile
      var ghProfile = new GH_String();
      if (!da.GetData(0, ref ghProfile)) return;

      if (GH_Convert.ToString(ghProfile, out string profile, GH_Conversion.Both)) {
        if (GsaSection.ValidProfile(profile))
          gsaSection = new GsaSection(profile);
        else {
          this.AddRuntimeWarning("Invalid profile syntax: " + profile);
          return;
        }

        // 3 Material
        var ghTyp = new GH_ObjectWrapper();
        if (da.GetData(1, ref ghTyp))
        {
	        var material = new GsaMaterial();
          if (ghTyp.Value is GsaMaterialGoo) {
            ghTyp.CastTo(ref material);
            gsaSection.Material = material ?? new GsaMaterial();
          }
          else {
            if (GH_Convert.ToInt32(ghTyp.Value, out int idd, GH_Conversion.Both))
              gsaSection.Material = new GsaMaterial(idd);
            else {
              this.AddRuntimeError("Unable to convert PB input to a Section Property of reference integer");
              return;
            }
          }
        }
        else
          gsaSection.Material = new GsaMaterial(7); // because Timber
      }

      da.SetData(0, new GsaSectionGoo(gsaSection));
    }
  }
}
