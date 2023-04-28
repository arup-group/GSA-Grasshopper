using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Helpers.GH;
using GsaGH.Helpers.GsaApi;
using GsaGH.Parameters;
using GsaGH.Properties;
using OasysGH;
using OasysGH.Components;
using OasysUnits;
using OasysUnits.Units;
using static GsaGH.Parameters.GsaOffset;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to automatically create offset based on section profile
  /// </summary>
  public class SectionAlignment : GH_OasysDropDownComponent {
    public override Guid ComponentGuid => new Guid("4dc655a2-366e-486e-b8c3-10b2063b7aac");
    public override GH_Exposure Exposure => GH_Exposure.quarternary | GH_Exposure.obscure;
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

    protected override void SolveInstance(IGH_DataAccess da) {
      var ghTyp = new GH_ObjectWrapper();
      if (!da.GetData(0, ref ghTyp)) {
        return;
      }

      GsaMember1d mem1d = null;
      GsaElement1d elem1d = null;
      GsaMember2d mem2d = null;
      GsaElement2d elem2d = null;

      bool oneD = true;

      string profile = string.Empty;

      switch (ghTyp.Value) {
        case GsaMember1dGoo _: {
          ghTyp.CastTo(ref mem1d);
          if (mem1d == null) {
            this.AddRuntimeError("Input is null");
            return;
          }

          mem1d = mem1d.Duplicate();
          profile = mem1d.Section.Profile;
          if (profile == string.Empty) {
            this.AddRuntimeError("Member has no section attached");
            return;
          }

          break;
        }
        case GsaElement1dGoo _: {
          ghTyp.CastTo(ref elem1d);
          if (elem1d == null) {
            this.AddRuntimeError("Input is null");
            return;
          }

          elem1d = elem1d.Duplicate();
          profile = elem1d.Section.Profile;
          if (profile == string.Empty) {
            this.AddRuntimeError("Element has no section attached");
            return;
          }

          break;
        }
        case GsaMember2dGoo _: {
          ghTyp.CastTo(ref mem2d);
          if (mem2d == null) {
            this.AddRuntimeError("Input is null");
            return;
          }

          mem2d = mem2d.Duplicate();
          oneD = false;
          break;
        }
        case GsaElement2dGoo _: {
          ghTyp.CastTo(ref elem2d);
          if (elem2d == null) {
            this.AddRuntimeError("Input is null");
            return;
          }

          elem2d = elem2d.Duplicate();
          oneD = false;
          break;
        }
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
            UnitParser parser = UnitParser.Default;
            unit = parser.Parse<LengthUnit>(type[1]);
          }

          Length depth = Length.Zero;
          Length width = Length.Zero;

          // angle
          if (profile.StartsWith("STD A")) {
            this.AddRuntimeWarning(
              "Only possible to automatically assign alignment to double symmetric sections at the moment. Input section profile: "
              + profile + ". Please check output.");
            depth = new Length(double.Parse(parts[2]), unit);
            width = new Length(double.Parse(parts[3]), unit);
          }

          // channel
          else if (profile.StartsWith("STD CH ") || profile.StartsWith("STD CH(")) {
            this.AddRuntimeWarning(
              "Only possible to automatically assign alignment to double symmetric sections at the moment. Input section profile: "
              + profile + ". Please check output.");
            depth = new Length(double.Parse(parts[2]), unit);
            width = new Length(double.Parse(parts[3]), unit);
          }

          // circle hollow
          else if (profile.StartsWith("STD CHS")) {
            depth = new Length(double.Parse(parts[2]), unit);
            width = new Length(double.Parse(parts[2]), unit);
          }

          // circle
          else if (profile.StartsWith("STD C ") || profile.StartsWith("STD C(")) {
            depth = new Length(double.Parse(parts[2]), unit);
            width = new Length(double.Parse(parts[2]), unit);
          }

          // ICruciformSymmetricalProfile
          else if (profile.StartsWith("STD X")) {
            depth = new Length(double.Parse(parts[2]), unit);
            width = new Length(double.Parse(parts[3]), unit);
          }

          // IEllipseHollowProfile
          else if (profile.StartsWith("STD OVAL")) {
            depth = new Length(double.Parse(parts[2]), unit);
            width = new Length(double.Parse(parts[3]), unit);
          }

          // IEllipseProfile
          else if (profile.StartsWith("STD E")) {
            depth = new Length(double.Parse(parts[2]), unit);
            width = new Length(double.Parse(parts[3]), unit);
          }

          // IGeneralCProfile
          else if (profile.StartsWith("STD GC")) {
            this.AddRuntimeWarning(
              "Only possible to automatically assign alignment to double symmetric sections at the moment. Input section profile: "
              + profile + ". Please check output.");
            depth = new Length(double.Parse(parts[2]), unit);
            width = new Length(double.Parse(parts[3]), unit);
          }

          // IGeneralZProfile
          else if (profile.StartsWith("STD GZ")) {
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
          }

          // IIBeamAsymmetricalProfile
          else if (profile.StartsWith("STD GI")) {
            this.AddRuntimeWarning(
              "Only possible to automatically assign alignment to double symmetric sections at the moment. Input section profile: "
              + profile + ". Please check output.");
            depth = new Length(double.Parse(parts[2]), unit);
            double top = double.Parse(parts[3]);
            double bottom = double.Parse(parts[4]);
            width = new Length(Math.Max(top, bottom), unit);
          }

          // IIBeamCellularProfile
          else if (profile.StartsWith("STD CB")) {
            depth = new Length(double.Parse(parts[2]), unit);
            width = new Length(double.Parse(parts[3]), unit);
          }

          // IIBeamSymmetricalProfile
          else if (profile.StartsWith("STD I")) {
            depth = new Length(double.Parse(parts[2]), unit);
            width = new Length(double.Parse(parts[3]), unit);
          }

          // IRectangleHollowProfile
          else if (profile.StartsWith("STD RHS")) {
            depth = new Length(double.Parse(parts[2]), unit);
            width = new Length(double.Parse(parts[3]), unit);
          }

          // IRectangleProfile
          else if (profile.StartsWith("STD R ") || profile.StartsWith("STD R(")) {
            depth = new Length(double.Parse(parts[2]), unit);
            width = new Length(double.Parse(parts[3]), unit);
          }

          // IRectoEllipseProfile
          else if (profile.StartsWith("STD RE")) {
            depth = new Length(double.Parse(parts[2]), unit);
            width = new Length(double.Parse(parts[3]), unit);
          }

          // ISecantPileProfile
          else if (profile.StartsWith("STD SP")) {
            depth = new Length(double.Parse(parts[2]), unit);
            if (profile.StartsWith("STD SPW")) {
              // STD SPW 250 100 4
              int count = int.Parse(parts[4], CultureInfo.InvariantCulture);
              double spacing = double.Parse(parts[3], CultureInfo.InvariantCulture);
              width = new Length(count * spacing, unit);
            } else {
              // STD SP 250 100 4
              int count = int.Parse(parts[4], CultureInfo.InvariantCulture);
              double spacing = double.Parse(parts[3], CultureInfo.InvariantCulture);
              double diameter = double.Parse(parts[2], CultureInfo.InvariantCulture);
              width = new Length(((count - 1) * spacing) + diameter, unit);
            }
          }

          // ISheetPileProfile
          else if (profile.StartsWith("STD SHT")) {
            this.AddRuntimeError(
              "Only possible to automatically assign alignment to double symmetric sections at the moment. Input section profile: "
              + profile);
            return;
          }

          // IStadiumProfile
          else if (profile.StartsWith("STD RC")) {
            depth = new Length(double.Parse(parts[2]), unit);
            width = new Length(double.Parse(parts[3]), unit);
          }

          // ITrapezoidProfile
          else if (profile.StartsWith("STD TR")) {
            this.AddRuntimeWarning(
              "Only possible to automatically assign alignment to double symmetric sections at the moment. Input section profile: "
              + profile + ". Please check output.");
            depth = new Length(double.Parse(parts[2]), unit);
            double top = double.Parse(parts[3]);
            double bottom = double.Parse(parts[4]);
            width = new Length(Math.Max(top, bottom), unit);
          }

          // ITSectionProfile
          else if (profile.StartsWith("STD T")) {
            this.AddRuntimeWarning(
              "Only possible to automatically assign alignment to double symmetric sections at the moment. Input section profile: "
              + profile + ". Please check output.");
            depth = new Length(double.Parse(parts[2]), unit);
            width = new Length(double.Parse(parts[3]), unit);
          } else if (profile.StartsWith("CAT")) {
            string prof = profile.Split(' ')[2];
            List<double> sqlValues = MicrosoftSQLiteReader.Instance.GetCatalogueProfileValues(prof,
              Path.Combine(AddReferencePriority.InstallPath, "sectlib.db3"));
            unit = LengthUnit.Meter;

            depth = new Length(sqlValues[0], unit);
            width = new Length(sqlValues[1], unit);
          } else {
            this.AddRuntimeError("Unable to get dimensions for Profile " + profile);
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
                alignmentOffset.Z = mem2d.Property.Thickness * -1 / 2;
                break;

              case AlignmentType.BottomLeft:
              case AlignmentType.BottomCentre:
              case AlignmentType.BottomRight:
                alignmentOffset.Z = mem2d.Property.Thickness / 2;
                break;
            }

            alignmentOffset.Z += additionalOffset.Z;
            mem2d.Offset = alignmentOffset;
            da.SetData(0, new GsaMember2dGoo(mem2d));
          }

          if (elem2d != null) {
            var offsets = new List<GsaOffset>();
            foreach (GsaProp2d prop in elem2d.Properties) {
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

            elem2d.Offsets = offsets;
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
