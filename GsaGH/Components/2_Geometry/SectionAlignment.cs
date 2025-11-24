using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using GsaGH.Helpers;
using GsaGH.Helpers.GH;
using GsaGH.Helpers.GsaApi;
using GsaGH.Parameters;
using GsaGH.Properties;

using OasysGH;
using OasysGH.Components;
using OasysGH.Helpers;
using OasysGH.Units;

using OasysUnits;
using OasysUnits.Units;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to automatically create offset based on section profile
  /// </summary>
  public class SectionAlignment : GH_OasysDropDownComponent {
    public override Guid ComponentGuid => new Guid("4dc655a2-366e-486e-b8c3-10b2063b7aac");
    public override GH_Exposure Exposure => GH_Exposure.septenary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.SectionAlignment;

    public SectionAlignment() : base("Section Alignment", "Align",
      "Automatically create Offset based on desired Alignment and Section profile",
      CategoryName.Name(), SubCategoryName.Cat2()) { }

    public override void SetSelected(int i, int j) {
      _selectedItems[i] = _dropDownItems[i][j];
      base.UpdateUI();
    }

    protected override void InitialiseDropdowns() {
      _spacerDescriptions = new List<string>(new[] {
        "Alignment",
      });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      var alignmentTypes = Enum.GetNames(typeof(AlignmentType)).ToList();
      _dropDownItems.Add(alignmentTypes);
      _selectedItems.Add(alignmentTypes[0]);

      _isInitialised = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddGenericParameter("Element/Member 1D/2D", "Geo",
        "Element1D, Element2D, Member1D or Member2D to align. Existing Offsets will be overwritten.",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Alignment", "Al",
        "Section alignment. This input will overwrite dropdown selection." + Environment.NewLine
        + "Accepted inputs are:" + Environment.NewLine + "Centroid" + Environment.NewLine
        + "Top-Left" + Environment.NewLine + "Top-Centre" + Environment.NewLine + "Top-Right"
        + Environment.NewLine + "Mid-Left" + Environment.NewLine + "Mid-Right" + Environment.NewLine
        + "Bottom-Left" + Environment.NewLine + "Bottom-Centre" + Environment.NewLine
        + "Bottom-Right", GH_ParamAccess.item);

      pManager.AddParameter(new GsaOffsetParameter(), GsaOffsetGoo.Name, GsaOffsetGoo.NickName,
        "Additional Offset (y and z values will be added to alignment setting)",
        GH_ParamAccess.item);

      pManager[1].Optional = true;
      pManager[2].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddGenericParameter("Element/Member 1D/2D", "Geo",
        "Element1D, Element2D, Member1D or Member2D with new Offset corrosponding to alignment input.",
        GH_ParamAccess.item);
      pManager.AddParameter(new GsaOffsetParameter(), GsaOffsetGoo.Name, GsaOffsetGoo.NickName,
        "Applied Offset", GH_ParamAccess.list);
    }

    protected override void SolveInternal(IGH_DataAccess da) {
      var ghTyp = new GH_ObjectWrapper();
      if (!da.GetData(0, ref ghTyp)) {
        return;
      }

      GsaMember1D mem1d = null;
      GsaElement1D elem1d = null;
      GsaMember2D mem2d = null;
      GsaElement2D elem2d = null;

      bool oneD = true;

      string profile = string.Empty;

      switch (ghTyp.Value) {
        case GsaMember1dGoo member1DGoo:
          mem1d = member1DGoo.Value;
          if (mem1d == null) {
            this.AddRuntimeError("Input is null");
            return;
          }

          profile = mem1d.Section.ApiSection.Profile;
          if (profile == string.Empty) {
            this.AddRuntimeError("Member has no section attached");
            return;
          }

          break;
        case GsaElement1dGoo element1DGoo:
          elem1d = element1DGoo.Value;
          if (elem1d == null) {
            this.AddRuntimeError("Input is null");
            return;
          }

          profile = elem1d.Section.ApiSection.Profile;
          if (profile == string.Empty) {
            this.AddRuntimeError("Element has no section attached");
            return;
          }

          break;

        case GsaMember2dGoo member2DGoo:
          mem2d = member2DGoo.Value;
          if (mem2d == null) {
            this.AddRuntimeError("Input is null");
            return;
          }

          if (mem2d.Prop2d == null || mem2d.Prop2d.Thickness.Equals(Length.Zero, DefaultUnits.Tolerance)) {
            this.AddRuntimeError("Member has no property attached");
            return;
          }

          oneD = false;
          break;

        case GsaElement2dGoo element2DGoo:
          elem2d = element2DGoo.Value;
          if (elem2d == null) {
            this.AddRuntimeError("Input is null");
            return;
          }

          if (elem2d.Prop2ds.IsNullOrEmpty()) {
            this.AddRuntimeError("Element has no property attached");
            return;
          }

          oneD = false;
          break;

        default:
          this.AddRuntimeError("Unable to convert input to Element1D or Member1D");
          return;
      }

      AlignmentType alignmentType = Mappings.GetAlignmentType(_selectedItems[0]);
      string alignment = _selectedItems[0];
      if (da.GetData(1, ref alignment)) {
        try {
          alignmentType = Mappings.GetAlignmentType(alignment);
        } catch (ArgumentException) {
          this.AddRuntimeError("Could not convert input Al to recognisable Alignment. Input is "
            + alignment);
          return;
        }
      }

      var additionalOffset = new GsaOffset();
      da.GetData(2, ref additionalOffset);

      var alignmentOffset = new GsaOffset();

      try {
        if (oneD) {
          string[] parts = profile.Split(' ');
          LengthUnit unit = LengthUnit.Millimeter;
          string[] type = parts[1].Split('(', ')');
          if (type.Length > 1) {
            unit = OasysUnitsSetup.Default.UnitParser.Parse<LengthUnit>(type[1]);
          }

          Length depth = Length.Zero;
          Length width = Length.Zero;

          if (profile.StartsWith("CAT")) {
            string prof = profile.Split(' ')[2];
            List<double> sqlValues = SqlReader.Instance.GetCatalogueProfileValues(prof,
              Path.Combine(AddReferencePriority.InstallPath, "sectlib.db3"));
            unit = LengthUnit.Meter;

            depth = new Length(sqlValues[0], unit);
            width = new Length(sqlValues[1], unit);
          } else {
            int count = 0;
            double spacing = 0;
            double top = 0;
            double bottom = 0;
            switch (type[0]) {
              case "A":
              case "CH":
              case "GC":
                this.AddRuntimeWarning(
                "Only possible to automatically assign alignment to double symmetric sections at the moment. Input section profile: "
                + profile + ". Please check output.");
                depth = new Length(double.Parse(parts[2]), unit);
                width = new Length(double.Parse(parts[3]), unit);
                break;
              case "X":
              case "OVAL":
              case "E":
              case "CB":
              case "I":
              case "RHS":
              case "R":
              case "RE":
              case "SHT":
              case "RC":
                depth = new Length(double.Parse(parts[2]), unit);
                width = new Length(double.Parse(parts[3]), unit);
                break;

              case "CHS":
              case "C":
                depth = new Length(double.Parse(parts[2]), unit);
                width = new Length(double.Parse(parts[2]), unit);
                break;

              case "GZ":
                this.AddRuntimeWarning(
                "Only possible to automatically assign alignment to double symmetric sections at the moment. Input section profile: "
                + profile + ". Please check output.");
                depth = new Length(double.Parse(parts[2]), unit);
                switch (alignmentType) {
                  case AlignmentType.TopLeft:
                  case AlignmentType.MidLeft:
                  case AlignmentType.BottomLeft:
                    width = new Length(double.Parse(parts[4]), unit);
                    break;

                  case AlignmentType.TopRight:
                  case AlignmentType.MidRight:
                  case AlignmentType.BottomRight:
                    width = new Length(double.Parse(parts[3]), unit);
                    break;

                  default:
                    width = new Length(double.Parse(parts[3]) + double.Parse(parts[4]), unit);
                    break;
                }
                break;

              case "GI":
                this.AddRuntimeWarning(
              "Only possible to automatically assign alignment to double symmetric sections at the moment. Input section profile: "
              + profile + ". Please check output.");
                depth = new Length(double.Parse(parts[2]), unit);
                top = double.Parse(parts[3]);
                bottom = double.Parse(parts[4]);
                width = new Length(Math.Max(top, bottom), unit);
                break;

              case "SP":
                // STD SP 250 100 4
                depth = new Length(double.Parse(parts[2]), unit);
                count = int.Parse(parts[4], CultureInfo.InvariantCulture);
                spacing = double.Parse(parts[3], CultureInfo.InvariantCulture);
                double diameter = double.Parse(parts[2], CultureInfo.InvariantCulture);
                width = new Length(((count - 1) * spacing) + diameter, unit);
                break;

              case "SPW":
                // STD SPW 250 100 4
                depth = new Length(double.Parse(parts[2]), unit);
                count = int.Parse(parts[4], CultureInfo.InvariantCulture);
                spacing = double.Parse(parts[3], CultureInfo.InvariantCulture);
                width = new Length(count * spacing, unit);
                break;

              case "TR":
                this.AddRuntimeWarning(
              "Only possible to automatically assign alignment to double symmetric sections at the moment. Input section profile: "
              + profile + ". Please check output.");
                depth = new Length(double.Parse(parts[2]), unit);
                top = double.Parse(parts[3]);
                bottom = double.Parse(parts[4]);
                width = new Length(Math.Max(top, bottom), unit);
                break;

              case "T":
                this.AddRuntimeWarning(
              "Only possible to automatically assign alignment to double symmetric sections at the moment. Input section profile: "
              + profile + ". Please check output.");
                depth = new Length(double.Parse(parts[2]), unit);
                width = new Length(double.Parse(parts[3]), unit);
                break;

              default:
                this.AddRuntimeError("Unable to get dimensions for Profile " + profile);
                return;
            }
          }

          switch (alignmentType) {
            case AlignmentType.Centroid: break;

            case AlignmentType.TopCentre:
              alignmentOffset.Z = depth * -1 / 2;
              break;

            case AlignmentType.BottomCentre:
              alignmentOffset.Z = depth / 2;
              break;

            case AlignmentType.TopLeft:
              alignmentOffset.Z = depth * -1 / 2;
              alignmentOffset.Y = width * -1 / 2;
              break;

            case AlignmentType.TopRight:
              alignmentOffset.Z = depth * -1 / 2;
              alignmentOffset.Y = width / 2;
              break;

            case AlignmentType.MidLeft:
              alignmentOffset.Y = width * -1 / 2;
              break;

            case AlignmentType.MidRight:
              alignmentOffset.Y = width / 2;
              break;

            case AlignmentType.BottomLeft:
              alignmentOffset.Z = depth / 2;
              alignmentOffset.Y = width * -1 / 2;
              break;

            case AlignmentType.BottomRight:
              alignmentOffset.Z = depth / 2;
              alignmentOffset.Y = width / 2;
              break;
          }

          alignmentOffset.X1 = additionalOffset.X1;
          alignmentOffset.X2 = additionalOffset.X2;
          alignmentOffset.Z += additionalOffset.Z;
          alignmentOffset.Y += additionalOffset.Y;

          if (mem1d != null) {
            mem1d.Offset = alignmentOffset;
            da.SetData(0, new GsaMember1dGoo(mem1d));
          }

          if (elem1d != null) {
            elem1d.Offset = alignmentOffset;
            da.SetData(0, new GsaElement1dGoo(elem1d));
          }
        } else {
          if (mem2d != null) {
            switch (alignmentType) {
              case AlignmentType.TopLeft:
              case AlignmentType.TopCentre:
              case AlignmentType.TopRight:
                alignmentOffset.Z = mem2d.Prop2d.Thickness * -1 / 2;
                break;

              case AlignmentType.BottomLeft:
              case AlignmentType.BottomCentre:
              case AlignmentType.BottomRight:
                alignmentOffset.Z = mem2d.Prop2d.Thickness / 2;
                break;
            }

            alignmentOffset.Z += additionalOffset.Z;
            mem2d.Offset = alignmentOffset;
            da.SetData(0, new GsaMember2dGoo(mem2d));
          }

          if (elem2d != null) {
            var offsets = new List<GsaOffset>();
            foreach (GsaProperty2d prop in elem2d.Prop2ds) {
              alignmentOffset = new GsaOffset();
              switch (alignmentType) {
                case AlignmentType.TopLeft:
                case AlignmentType.TopCentre:
                case AlignmentType.TopRight:
                  alignmentOffset.Z = prop.Thickness * -1 / 2;
                  break;

                case AlignmentType.BottomLeft:
                case AlignmentType.BottomCentre:
                case AlignmentType.BottomRight:
                  alignmentOffset.Z = prop.Thickness / 2;
                  break;
              }

              alignmentOffset.Z += additionalOffset.Z;
              offsets.Add(alignmentOffset.Duplicate());
            }

            elem2d.ApiElements.SetMembers(offsets);
            da.SetData(0, new GsaElement2dGoo(elem2d));
            da.SetDataList(1,
              new List<GsaOffsetGoo>(offsets.Select(x => new GsaOffsetGoo(x)).ToList()));
            return;
          }
        }
      } catch (Exception) {
        this.AddRuntimeError("Invalid profile");
        return;
      }

      da.SetData(1, new GsaOffsetGoo(alignmentOffset));
    }
  }
}
