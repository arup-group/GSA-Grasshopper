﻿using System;
using System.Drawing;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;
using OasysGH;
using OasysGH.Components;
using OasysGH.Units;
using OasysUnits;
using OasysUnits.Units;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to create a new Section
  /// </summary>
  public class CreateSection : GH_OasysComponent {
    public override Guid ComponentGuid => new Guid("d779f9b7-5380-4474-aadd-d1e88f9d45b8");
    public override GH_Exposure Exposure => GH_Exposure.primary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.CreateSection;

    // how does the context menu for units work?!
    private LengthUnit _lengthUnit = DefaultUnits.LengthUnitSection;

    public CreateSection() : base("Create Section", "Section", "Create GSA Section",
      CategoryName.Name(), SubCategoryName.Cat1()) {
      Hidden = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddTextParameter("Profile", "Pf",
        "Cross-Section Profile defined using the GSA Profile string syntax", GH_ParamAccess.item);
      pManager.AddParameter(new GsaMaterialParameter());
      pManager.AddGenericParameter("Basic Offset", "BO",
        "Basic Offset Centroid = 0 (default), Top = 1, TopLeft = 2, TopRight = 3, Left = 4, Right = 5, Bottom = 6, BottomLeft = 7, BottomRight = 8", GH_ParamAccess.item);
      pManager.AddGenericParameter($"Add. Offset Y [{Length.GetAbbreviation(_lengthUnit)}]", "AOY", "Additional Offset Y", GH_ParamAccess.item);
      pManager.AddGenericParameter($"Add. Offset Z [{Length.GetAbbreviation(_lengthUnit)}]", "AOZ", "Additional Offset Z", GH_ParamAccess.item);

      pManager[1].Optional = true;
      pManager[2].Optional = true;
      pManager[3].Optional = true;
      pManager[4].Optional = true;
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

      da.SetData(0, new GsaSectionGoo(gsaSection));
    }
  }
}
