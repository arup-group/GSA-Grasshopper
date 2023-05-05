using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;
using OasysGH;
using OasysGH.Components;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to create a new Section
  /// </summary>
  public class CreateSection : GH_OasysDropDownComponent {
    public override Guid ComponentGuid => new Guid("d779f9b7-5380-4474-aadd-d1e88f9d45b8");
    public override GH_Exposure Exposure => GH_Exposure.primary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.CreateSection;

    public CreateSection() : base("Create Section", "Section", "Create GSA Section",
      CategoryName.Name(), SubCategoryName.Cat1()) {
      Hidden = true;
    }

    public override void SetSelected(int i, int j) {
      _selectedItems[i] = _dropDownItems[i][j];
      base.UpdateUI();
    }

    protected override void InitialiseDropdowns() {
      _spacerDescriptions = new List<string>(new[] {
        "Basic Offset",
      });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      var basicOffset = Enum.GetNames(typeof(BasicOffset)).ToList();
      _dropDownItems.Add(basicOffset);
      _selectedItems.Add(basicOffset[0]);

      _isInitialised = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddTextParameter("Profile", "Pf",
        "Cross-Section Profile defined using the GSA Profile string syntax", GH_ParamAccess.item);
      pManager.AddParameter(new GsaMaterialParameter());

      pManager[1].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaSectionParameter());
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      var gsaSection = new GsaSection();
      var ghProfile = new GH_String();
      if (!da.GetData(0, ref ghProfile)) {
        return;
      }

      if (GH_Convert.ToString(ghProfile, out string profile, GH_Conversion.Both)) {
        if (GsaSection.ValidProfile(profile)) {
          gsaSection = new GsaSection(profile);
        } else {
          this.AddRuntimeWarning("Invalid profile syntax: " + profile);
          return;
        }

        var ghTyp = new GH_ObjectWrapper();
        if (da.GetData(1, ref ghTyp)) {
          if (ghTyp.Value is GsaMaterialGoo materialGoo) {
            gsaSection.Material = materialGoo.Value ?? new GsaMaterial();
          } else {
            if (GH_Convert.ToInt32(ghTyp.Value, out int idd, GH_Conversion.Both)) {
              gsaSection.Material = new GsaMaterial(idd);
            } else {
              this.AddRuntimeError(
                "Unable to convert PB input to a Section Property of reference integer");
              return;
            }
          }
        } else {
          gsaSection.Material = new GsaMaterial(7);
        }
      }

      gsaSection.BasicOffset = (BasicOffset)Enum.Parse(typeof(BasicOffset), _selectedItems[0]);

      da.SetData(0, new GsaSectionGoo(gsaSection));
    }
  }
}
