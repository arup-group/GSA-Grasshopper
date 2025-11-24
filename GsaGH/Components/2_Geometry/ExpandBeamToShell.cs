using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using GH_IO.Serialization;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using GsaAPI;

using GsaGH.Helpers.Assembly;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;

using OasysGH;
using OasysGH.Components;
using OasysGH.Helpers;
using OasysGH.Units;
using OasysGH.Units.Helpers;

using OasysUnits;

using Rhino.Geometry;

using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGH.Components {
  public class ExpandBeamToShell : GH_OasysComponent, IGH_VariableParameterComponent {
    public override Guid ComponentGuid => new Guid("42221f6b-b0f1-41ae-abcf-df5521565464");
    public override GH_Exposure Exposure => GH_Exposure.quarternary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.ExpandBeamToShell;
    private LengthUnit _lengthUnit = DefaultUnits.LengthUnitGeometry;
    internal ToleranceContextMenu ToleranceMenu { get; set; } = new ToleranceContextMenu() {
      Tolerance = DefaultUnits.Tolerance * 2
    };
    public ExpandBeamToShell() : base("Expand Beam to Shell", "B2S",
      "Expand 1D Entities to 2D Entities from profile, orientation and offset",
      CategoryName.Name(), SubCategoryName.Cat2()) { }

    public override void AppendAdditionalMenuItems(ToolStripDropDown menu) {
      if (!(menu is ContextMenuStrip)) {
        return; // this method is also called when clicking EWR balloon
      }

      Menu_AppendSeparator(menu);
      ToleranceMenu.AppendAdditionalMenuItems(this, menu, _lengthUnit);
      var unitsMenu = new ToolStripMenuItem("Select unit", Resources.ModelUnits) {
        Enabled = true,
        ImageScaling = ToolStripItemImageScaling.SizeToFit,
      };
      foreach (string unit in UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length)) {
        var toolStripMenuItem = new ToolStripMenuItem(unit, null, (s, e) => Update(unit)) {
          Checked = unit == Length.GetAbbreviation(_lengthUnit),
          Enabled = true,
        };
        unitsMenu.DropDownItems.Add(toolStripMenuItem);
      }

      menu.Items.Add(unitsMenu);

      Menu_AppendSeparator(menu);
    }

    bool IGH_VariableParameterComponent.CanInsertParameter(GH_ParameterSide side, int index) {
      return false;
    }

    bool IGH_VariableParameterComponent.CanRemoveParameter(GH_ParameterSide side, int index) {
      return false;
    }

    IGH_Param IGH_VariableParameterComponent.CreateParameter(GH_ParameterSide side, int index) {
      return null;
    }

    bool IGH_VariableParameterComponent.DestroyParameter(GH_ParameterSide side, int index) {
      return false;
    }

    public override bool Read(GH_IReader reader) {
      _lengthUnit
        = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), reader.GetString("LengthUnit"));
      double tol = reader.GetDouble("Tolerance");
      ToleranceMenu.Tolerance = new Length(tol, _lengthUnit);
      return base.Read(reader);
    }

    public void VariableParameterMaintenance() { }

    public override bool Write(GH_IWriter writer) {
      writer.SetString("LengthUnit", _lengthUnit.ToString());
      writer.SetDouble("Tolerance", ToleranceMenu.Tolerance.Value);
      return base.Write(writer);
    }

    protected override void BeforeSolveInstance() {
      base.BeforeSolveInstance();
      Message = Length.GetAbbreviation(_lengthUnit) +
      " - Tol: " + ToleranceMenu.Tolerance.ToString().Replace(" ", string.Empty);
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddGenericParameter("Element/Member 1D", "G1D",
        "Element1D or Member1D to expand to 2D.", GH_ParamAccess.item);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaMember2dParameter());
      pManager[0].Access = GH_ParamAccess.list;
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      var ghTyp = new GH_ObjectWrapper();
      da.GetData(0, ref ghTyp);

      GsaMember1D member;
      GsaElement1D element;
      GsaSection section;
      Curve crv;
      Parameters.LocalAxes axes;
      GsaOffset offset;
      switch (ghTyp.Value) {
        case GsaMember1dGoo memberGoo: {
            if (memberGoo == null || memberGoo.Value == null) {
              this.AddRuntimeError("Input is null");
              return;
            }

            member = memberGoo.Value;
            if (member.Section == null || member.Section.ApiSection == null) {
              this.AddRuntimeError("Member has no section with valid profile");
              return;
            }

            axes = member.LocalAxes;
            if (axes == null) {
              var assembly = new ModelAssembly(member);
              var model = new GsaModel {
                ApiModel = assembly.GetModel()
              };

              axes = new Parameters.LocalAxes(model.ApiModel.MemberDirectionCosine(1));
            }

            crv = member.PolyCurve.DuplicatePolyCurve();
            if (crv.SpanCount > 1) {
              this.AddRuntimeWarning("Curve has more than one span, but has been " +
                "simplified to a straight line");
            }

            section = member.Section;
            offset = member.Offset;
            break;
          }
        case GsaElement1dGoo elementGoo: {
            if (elementGoo == null || elementGoo.Value == null) {
              this.AddRuntimeError("Input is null");
              return;
            }

            element = elementGoo.Value;
            if (element.Section == null || element.Section.ApiSection == null) {
              this.AddRuntimeError("Element has no section with valid profile");
              return;
            }

            axes = element.LocalAxes;
            if (axes == null) {
              var assembly = new ModelAssembly(element);
              var model = new GsaModel() {
                ApiModel = assembly.GetModel()
              };

              axes = new Parameters.LocalAxes(model.ApiModel.ElementDirectionCosine(1));
            }

            crv = new LineCurve(element.Line.PointAtStart, element.Line.PointAtEnd);
            offset = element.Offset;
            section = element.Section;
            break;
          }
        default:
          this.AddRuntimeError("Unable to convert input to Element1D or Member1D");
          return;
      }

      string profile = section.ApiSection.Profile;
      if (profile.Trim() == string.Empty) {
        this.AddRuntimeError("Element/Member has no section with valid profile");
        return;
      }

      var mem2ds = new List<GsaMember2D>();
      LengthUnit unit = LengthUnit.Meter;
      crv.LengthParameter(offset.X1.As(_lengthUnit), out double t0);
      crv.LengthParameter(crv.GetLength() - offset.X2.As(_lengthUnit), out double t1);
      crv = crv.Trim(t0, t1);
      var pt = new Point3d(crv.PointAtStart);
      var pln = new Plane(crv.PointAtStart, axes.Z, axes.Y);
      var translation = new Vector3d(
        offset.Z.As(_lengthUnit)
        - section.AdditionalOffsetZ.As(_lengthUnit)
        - section.SectionProperties.Cz.As(_lengthUnit),
        offset.Y.As(_lengthUnit)
        - section.AdditionalOffsetY.As(_lengthUnit)
        - section.SectionProperties.Cy.As(_lengthUnit),
        0);
      var mapToLocal = Transform.ChangeBasis(pln, Plane.WorldXY);
      translation.Transform(mapToLocal);
      var move = Transform.Translation(translation);
      pt.Transform(move);
      pln.Origin = pt;
      string[] angleString = profile.Split(new string[] { "[R(", ")]" }, StringSplitOptions.None);
      double angle = 0;
      if (angleString.Length > 1) {
        angle = double.Parse(angleString[1]);
        pln.Rotate(Rhino.RhinoMath.ToRadians(angle), axes.X);
      }

      string[] parts = profile.Split(' ');
      unit = LengthUnit.Millimeter;
      string[] type = parts[1].Split('(', ')');
      if (type.Length > 1) {
        unit = OasysUnitsSetup.Default.UnitParser.Parse<LengthUnit>(type[1]);
      }

      if (section.ApiSection.BasicOffset != BasicOffset.Centroid && angle != 0) {
        this.AddRuntimeError("Unable to expand rotated profile " +
          "combined with non-centroid BaseOffset");
        return;
      }

      // angle
      if (profile.StartsWith("STD A")) {
        double height = ValueFromString(parts[2], unit);
        double width = ValueFromString(parts[3], unit);
        double webThk = ValueFromString(parts[4], unit);
        double flangeThk = ValueFromString(parts[5], unit);

        if (section.ApiSection.BasicOffset != BasicOffset.Centroid) {
          switch (section.ApiSection.BasicOffset) {
            case BasicOffset.Top:
              pln.Origin = pln.PointAt(
                -height + section.SectionProperties.Cz.As(_lengthUnit),
                0);
              break;

            case BasicOffset.TopLeft:
              pln.Origin = pln.PointAt(
                -height + section.SectionProperties.Cz.As(_lengthUnit),
                section.SectionProperties.Cy.As(_lengthUnit));
              break;

            case BasicOffset.TopRight:
              pln.Origin = pln.PointAt(
                -height + section.SectionProperties.Cz.As(_lengthUnit),
                -width + section.SectionProperties.Cy.As(_lengthUnit));
              break;

            case BasicOffset.Left:
              pln.Origin = pln.PointAt(
                0,
                section.SectionProperties.Cy.As(_lengthUnit));
              break;

            case BasicOffset.Right:
              pln.Origin = pln.PointAt(
                0,
                -width + section.SectionProperties.Cy.As(_lengthUnit));
              break;

            case BasicOffset.Bottom:
              pln.Origin = pln.PointAt(
                section.SectionProperties.Cz.As(_lengthUnit),
                0);
              break;

            case BasicOffset.BottomLeft:
              pln.Origin = pln.PointAt(
                section.SectionProperties.Cz.As(_lengthUnit),
                section.SectionProperties.Cy.As(_lengthUnit));
              break;

            case BasicOffset.BottomRight:
              pln.Origin = pln.PointAt(
                section.SectionProperties.Cz.As(_lengthUnit),
                -width + section.SectionProperties.Cy.As(_lengthUnit));
              break;
          }
        }

        Brep web = ExtrudePlate(pln, crv,
          flangeThk / 2, webThk / 2,
          height, webThk / 2);
        mem2ds.Add(CreateMember2d(web, webThk, section));

        Brep flange = ExtrudePlate(pln, crv,
          flangeThk / 2, webThk / 2,
          flangeThk / 2, width);
        mem2ds.Add(CreateMember2d(flange, flangeThk, section));
      }

      // channel
      else if (profile.StartsWith("STD CH ") || profile.StartsWith("STD CH(")) {
        double height = ValueFromString(parts[2], unit);
        double width = ValueFromString(parts[3], unit);
        double webThk = ValueFromString(parts[4], unit);
        double flangeThk = ValueFromString(parts[5], unit);

        if (section.ApiSection.BasicOffset != BasicOffset.Centroid) {
          double widthLeft = section.SectionProperties.Cy.As(_lengthUnit);
          double widthRight = width - section.SectionProperties.Cy.As(_lengthUnit);
          pln.Origin = BaseOffset(pln, section, height, widthLeft * 2, widthRight * 2);
        } else {
          pln.Origin = BaseOffset(pln, section, height, width);
        }

        Brep web = ExtrudePlate(pln, crv,
          (height / 2) - (flangeThk / 2), webThk / 2,
          -(height / 2) + (flangeThk / 2), webThk / 2);
        mem2ds.Add(CreateMember2d(web, webThk, section));

        Brep topFlange = ExtrudePlate(pln, crv,
          (height / 2) - (flangeThk / 2), webThk / 2,
          (height / 2) - (flangeThk / 2), width);
        mem2ds.Add(CreateMember2d(topFlange, flangeThk, section));

        Brep bottomFlange = ExtrudePlate(pln, crv,
          -(height / 2) + (flangeThk / 2), webThk / 2,
          -(height / 2) + (flangeThk / 2), width);
        mem2ds.Add(CreateMember2d(bottomFlange, flangeThk, section));
      }

      // circle hollow
      else if (profile.StartsWith("STD CHS")) {
        double diameter = ValueFromString(parts[2], unit);
        double webThk = ValueFromString(parts[3], unit);

        pln.Origin = BaseOffset(pln, section, diameter, diameter);

        var circle = new Circle(pln, (diameter - webThk) / 2);
        int divisions = (int)Math.Ceiling(
          circle.Circumference / ToleranceMenu.Tolerance.As(_lengthUnit));
        var positions = Enumerable.Range(0, divisions + 1)
          .Select(i => (double)i / divisions * Math.PI * 2).ToList();

        for (int j = 1; j < positions.Count; j++) {
          Point3d pt1 = circle.PointAt(positions[j - 1]);
          Point3d pt2 = circle.PointAt(positions[j]);
          Brep strip = ExtrudePlate(crv, pt1, pt2);
          mem2ds.Add(CreateMember2d(strip, webThk, section));
        }
      }

      // circle
      else if (profile.StartsWith("STD C ") || profile.StartsWith("STD C(")) {
        this.AddRuntimeError("Unable to expand solid Circle profile");
        return;
      }

      // ICruciformSymmetricalProfile
      else if (profile.StartsWith("STD X")) {
        double height = ValueFromString(parts[2], unit);
        double width = ValueFromString(parts[3], unit);
        double webThk = ValueFromString(parts[4], unit);
        double flangeThk = ValueFromString(parts[5], unit);

        pln.Origin = BaseOffset(pln, section, height, width);

        Brep web = ExtrudePlate(pln, crv,
          height / 2, 0,
          -height / 2, 0);
        mem2ds.Add(CreateMember2d(web, webThk, section));

        Brep flange = ExtrudePlate(pln, crv,
          0, width / 2,
          0, -width / 2);
        mem2ds.Add(CreateMember2d(flange, flangeThk, section));
      }

      // IEllipseHollowProfile
      else if (profile.StartsWith("STD OVAL")) {
        double height = ValueFromString(parts[2], unit);
        double width = ValueFromString(parts[3], unit);
        double webThk = ValueFromString(parts[4], unit);

        pln.Origin = BaseOffset(pln, section, height, width);

        var ellipse = new Ellipse(pln, (height - webThk) / 2, (width - webThk) / 2)
          .ToNurbsCurve();
        int divisions = (int)Math.Ceiling(
          ellipse.GetLength() / ToleranceMenu.Tolerance.As(_lengthUnit));
        var positions = Enumerable.Range(0, divisions + 1)
          .Select(i => (double)i / divisions * Math.PI * 2).ToList();

        for (int j = 1; j < positions.Count; j++) {
          Point3d pt1 = ellipse.PointAt(positions[j - 1]);
          Point3d pt2 = ellipse.PointAt(positions[j]);
          Brep strip = ExtrudePlate(crv, pt1, pt2);
          mem2ds.Add(CreateMember2d(strip, webThk, section));
        }
      }

      // IEllipseProfile
      else if (profile.StartsWith("STD E")) {
        this.AddRuntimeError("Unable to expand solid Ellipse profile");
        return;
      }

      // IGeneralCProfile
      else if (profile.StartsWith("STD GC")) {
        double height = ValueFromString(parts[2], unit);
        double width = ValueFromString(parts[3], unit);
        double flangeThk = ValueFromString(parts[5], unit);
        double lip = ValueFromString(parts[4], unit);

        if (section.ApiSection.BasicOffset != BasicOffset.Centroid) {
          double widthLeft = section.SectionProperties.Cy.As(_lengthUnit);
          double widthRight = width - section.SectionProperties.Cy.As(_lengthUnit);
          pln.Origin = BaseOffset(pln, section, height, widthLeft * 2, widthRight * 2);
        } else {
          pln.Origin = BaseOffset(pln, section, height, width);
        }

        Brep web = ExtrudePlate(pln, crv,
          (height / 2) - (flangeThk / 2), flangeThk / 2,
          -(height / 2) + (flangeThk / 2), flangeThk / 2);
        mem2ds.Add(CreateMember2d(web, flangeThk, section));

        Brep topFlange = ExtrudePlate(pln, crv,
          (height / 2) - (flangeThk / 2), flangeThk / 2,
          (height / 2) - (flangeThk / 2), width - (flangeThk / 2));
        mem2ds.Add(CreateMember2d(topFlange, flangeThk, section));

        Brep bottomFlange = ExtrudePlate(pln, crv,
          -(height / 2) + (flangeThk / 2), flangeThk / 2,
          -(height / 2) + (flangeThk / 2), width - (flangeThk / 2));
        mem2ds.Add(CreateMember2d(bottomFlange, flangeThk, section));

        Brep topLip = ExtrudePlate(pln, crv,
          (height / 2) - (flangeThk / 2), width - (flangeThk / 2),
          (height / 2) - lip, width - (flangeThk / 2));
        mem2ds.Add(CreateMember2d(topLip, flangeThk, section));

        Brep bottomLip = ExtrudePlate(pln, crv,
          -(height / 2) + (flangeThk / 2), width - (flangeThk / 2),
          -(height / 2) + lip, width - (flangeThk / 2));
        mem2ds.Add(CreateMember2d(bottomLip, flangeThk, section));
      }

      // IGeneralZProfile
      else if (profile.StartsWith("STD GZ")) {
        double height = ValueFromString(parts[2], unit);
        double topWidth = ValueFromString(parts[3], unit);
        double bottomWidth = ValueFromString(parts[4], unit);
        double flangeThk = ValueFromString(parts[7], unit);
        double topLip = ValueFromString(parts[5], unit);
        double bottomLip = ValueFromString(parts[6], unit);

        if (section.ApiSection.BasicOffset != BasicOffset.Centroid) {
          switch (section.ApiSection.BasicOffset) {
            case BasicOffset.Top:
              pln.Origin = pln.PointAt(
                (-height / 2) + section.SectionProperties.Cz.As(_lengthUnit),
                0);
              break;

            case BasicOffset.TopLeft:
              pln.Origin = pln.PointAt(
                (-height / 2) + section.SectionProperties.Cz.As(_lengthUnit),
                bottomWidth - (flangeThk / 2) + section.SectionProperties.Cy.As(_lengthUnit));
              break;

            case BasicOffset.TopRight:
              pln.Origin = pln.PointAt(
                (-height / 2) + section.SectionProperties.Cz.As(_lengthUnit),
                -topWidth + (flangeThk / 2) + section.SectionProperties.Cy.As(_lengthUnit));
              break;

            case BasicOffset.Left:
              pln.Origin = pln.PointAt(
                0,
                bottomWidth - (flangeThk / 2) + section.SectionProperties.Cy.As(_lengthUnit));
              break;

            case BasicOffset.Right:
              pln.Origin = pln.PointAt(
                0,
                -topWidth + (flangeThk / 2) + section.SectionProperties.Cy.As(_lengthUnit));
              break;

            case BasicOffset.Bottom:
              pln.Origin = pln.PointAt(
                (height / 2) + section.SectionProperties.Cz.As(_lengthUnit),
                0);
              break;

            case BasicOffset.BottomLeft:
              pln.Origin = pln.PointAt(
                (height / 2) + section.SectionProperties.Cz.As(_lengthUnit),
                bottomWidth - (flangeThk / 2) + section.SectionProperties.Cy.As(_lengthUnit));
              break;

            case BasicOffset.BottomRight:
              pln.Origin = pln.PointAt(
                (height / 2) + section.SectionProperties.Cz.As(_lengthUnit),
                -topWidth + (flangeThk / 2) + section.SectionProperties.Cy.As(_lengthUnit));
              break;
          }
        }

        Brep web = ExtrudePlate(pln, crv,
          (height / 2) - (flangeThk / 2), 0,
          -(height / 2) + (flangeThk / 2), 0);
        mem2ds.Add(CreateMember2d(web, flangeThk, section));

        Brep topFlange = ExtrudePlate(pln, crv,
          (height / 2) - (flangeThk / 2), 0,
          (height / 2) - (flangeThk / 2), topWidth - flangeThk);
        mem2ds.Add(CreateMember2d(topFlange, flangeThk, section));

        Brep bottomFlange = ExtrudePlate(pln, crv,
          -(height / 2) + (flangeThk / 2), 0,
          -(height / 2) + (flangeThk / 2), -bottomWidth + flangeThk);
        mem2ds.Add(CreateMember2d(bottomFlange, flangeThk, section));

        Brep topFlangeLip = ExtrudePlate(pln, crv,
          (height / 2) - (flangeThk / 2), topWidth - flangeThk,
          (height / 2) - topLip, topWidth - flangeThk);
        mem2ds.Add(CreateMember2d(topFlangeLip, flangeThk, section));

        Brep bottomFlangeLip = ExtrudePlate(pln, crv,
          -(height / 2) + (flangeThk / 2), -bottomWidth + flangeThk,
          -(height / 2) + bottomLip, -bottomWidth + flangeThk);
        mem2ds.Add(CreateMember2d(bottomFlangeLip, flangeThk, section));
      }

      // IIBeamAsymmetricalProfile
      else if (profile.StartsWith("STD GI")) {
        double height = ValueFromString(parts[2], unit);
        double topWidth = ValueFromString(parts[3], unit);
        double bottomWidth = ValueFromString(parts[4], unit);
        double flangeThkTop = ValueFromString(parts[6], unit);
        double flangeThkBottom = ValueFromString(parts[7], unit);
        double webThk = ValueFromString(parts[5], unit);
        double maxWidth = Math.Max(topWidth, bottomWidth);

        if (section.ApiSection.BasicOffset != BasicOffset.Centroid) {
          switch (section.ApiSection.BasicOffset) {
            case BasicOffset.Top:
              pln.Origin = pln.PointAt(
                (-height / 2) + section.SectionProperties.Cz.As(_lengthUnit),
                0);
              break;

            case BasicOffset.TopLeft:
              pln.Origin = pln.PointAt(
                (-height / 2) + section.SectionProperties.Cz.As(_lengthUnit),
                (maxWidth / 2) - section.SectionProperties.Cy.As(_lengthUnit));
              break;

            case BasicOffset.TopRight:
              pln.Origin = pln.PointAt(
                (-height / 2) + section.SectionProperties.Cz.As(_lengthUnit),
                (-maxWidth / 2) + section.SectionProperties.Cy.As(_lengthUnit));
              break;

            case BasicOffset.Left:
              pln.Origin = pln.PointAt(
                0,
                (maxWidth / 2) - section.SectionProperties.Cy.As(_lengthUnit));
              break;

            case BasicOffset.Right:
              pln.Origin = pln.PointAt(
                0,
                (-maxWidth / 2) + section.SectionProperties.Cy.As(_lengthUnit));
              break;

            case BasicOffset.Bottom:
              pln.Origin = pln.PointAt(
                (height / 2) + section.SectionProperties.Cz.As(_lengthUnit),
                0);
              break;

            case BasicOffset.BottomLeft:
              pln.Origin = pln.PointAt(
                (height / 2) + section.SectionProperties.Cz.As(_lengthUnit),
                (maxWidth / 2) - section.SectionProperties.Cy.As(_lengthUnit));
              break;

            case BasicOffset.BottomRight:
              pln.Origin = pln.PointAt(
                (height / 2) + section.SectionProperties.Cz.As(_lengthUnit),
                (-maxWidth / 2) + section.SectionProperties.Cy.As(_lengthUnit));
              break;
          }
        }

        Brep web = ExtrudePlate(pln, crv,
          (height / 2) - (flangeThkTop / 2), 0,
          -(height / 2) + (flangeThkBottom / 2), 0);
        mem2ds.Add(CreateMember2d(web, webThk, section));

        Brep topFlange = ExtrudePlate(pln, crv,
          (height / 2) - (flangeThkTop / 2), topWidth / 2,
          (height / 2) - (flangeThkTop / 2), -topWidth / 2);
        mem2ds.Add(CreateMember2d(topFlange, flangeThkTop, section));

        Brep bottomFlange = ExtrudePlate(pln, crv,
          -(height / 2) + (flangeThkBottom / 2), bottomWidth / 2,
          -(height / 2) + (flangeThkBottom / 2), -bottomWidth / 2);
        mem2ds.Add(CreateMember2d(bottomFlange, flangeThkBottom, section));
      }

      // IIBeamCellularProfile
      else if (profile.StartsWith("STD CB")) {
        double height = ValueFromString(parts[2], unit);
        double width = ValueFromString(parts[3], unit);
        double flangeThk = ValueFromString(parts[5], unit);
        double webThk = ValueFromString(parts[4], unit);
        double holeDiameter = ValueFromString(parts[6], unit);

        pln.Origin = BaseOffset(pln, section, height, width);

        Brep webTop = ExtrudePlate(pln, crv,
          (height / 2) - (flangeThk / 2), 0,
          holeDiameter / 2, 0);
        mem2ds.Add(CreateMember2d(webTop, webThk, section));

        Brep webBottom = ExtrudePlate(pln, crv,
          -holeDiameter / 2, 0,
          -(height / 2) + (flangeThk / 2), 0);
        mem2ds.Add(CreateMember2d(webBottom, webThk, section));

        Brep topFlange = ExtrudePlate(pln, crv,
          (height / 2) - (flangeThk / 2), width / 2,
          (height / 2) - (flangeThk / 2), -width / 2);
        mem2ds.Add(CreateMember2d(topFlange, flangeThk, section));

        Brep bottomFlange = ExtrudePlate(pln, crv,
          -(height / 2) + (flangeThk / 2), width / 2,
          -(height / 2) + (flangeThk / 2), -width / 2);
        mem2ds.Add(CreateMember2d(bottomFlange, flangeThk, section));
      }

      // IIBeamSymmetricalProfile
      else if (profile.StartsWith("STD I")) {
        double height = ValueFromString(parts[2], unit);
        double width = ValueFromString(parts[3], unit);
        double flangeThk = ValueFromString(parts[5], unit);
        double webThk = ValueFromString(parts[4], unit);

        pln.Origin = BaseOffset(pln, section, height, width);

        Brep web = ExtrudePlate(pln, crv,
          (height / 2) - (flangeThk / 2), 0,
          -(height / 2) + (flangeThk / 2), 0);
        mem2ds.Add(CreateMember2d(web, webThk, section));

        Brep topFlange = ExtrudePlate(pln, crv,
          (height / 2) - (flangeThk / 2), width / 2,
          (height / 2) - (flangeThk / 2), -width / 2);
        mem2ds.Add(CreateMember2d(topFlange, flangeThk, section));

        Brep bottomFlange = ExtrudePlate(pln, crv,
          -(height / 2) + (flangeThk / 2), width / 2,
          -(height / 2) + (flangeThk / 2), -width / 2);
        mem2ds.Add(CreateMember2d(bottomFlange, flangeThk, section));
      }

      // IRectangleHollowProfile
      else if (profile.StartsWith("STD RHS")) {
        double height = ValueFromString(parts[2], unit);
        double width = ValueFromString(parts[3], unit);
        double flangeThk = ValueFromString(parts[5], unit);
        double webThk = ValueFromString(parts[4], unit);

        pln.Origin = BaseOffset(pln, section, height, width);

        Brep webLeft = ExtrudePlate(pln, crv,
          (height / 2) - (flangeThk / 2), -(width / 2) + (webThk / 2),
          -(height / 2) + (flangeThk / 2), -(width / 2) + (webThk / 2));
        mem2ds.Add(CreateMember2d(webLeft, webThk, section));

        Brep webRight = ExtrudePlate(pln, crv,
          (height / 2) - (flangeThk / 2), (width / 2) - (webThk / 2),
          -(height / 2) + (flangeThk / 2), (width / 2) - (webThk / 2));
        mem2ds.Add(CreateMember2d(webRight, webThk, section));

        Brep topFlange = ExtrudePlate(pln, crv,
          (height / 2) - (flangeThk / 2), -(width / 2) + (webThk / 2),
          (height / 2) - (flangeThk / 2), (width / 2) - (webThk / 2));
        mem2ds.Add(CreateMember2d(topFlange, flangeThk, section));

        Brep bottomFlange = ExtrudePlate(pln, crv,
          -(height / 2) + (flangeThk / 2), -(width / 2) + (webThk / 2),
          -(height / 2) + (flangeThk / 2), (width / 2) - (webThk / 2));
        mem2ds.Add(CreateMember2d(bottomFlange, flangeThk, section));
      }

      // IRectangleProfile
      else if (profile.StartsWith("STD R ") || profile.StartsWith("STD R(")) {
        this.AddRuntimeError("Unable to expand solid Rectangle profile");
        return;
      }

      // IRectoEllipseProfile
      else if (profile.StartsWith("STD RE")) {
        this.AddRuntimeError("Unable to expand solid Recto-ellipse profile");
        return;
      }

      // ISecantPileProfile
      else if (profile.StartsWith("STD SP")) {
        this.AddRuntimeError("Unable to expand Secant pile profile");
        return;
      }

      // ISheetPileProfile
      else if (profile.StartsWith("STD SHT")) {
        this.AddRuntimeError("Unable to expand Sheet pile profile");
        return;
      }

      // IStadiumProfile
      else if (profile.StartsWith("STD RC")) {
        this.AddRuntimeError("Unable to expand solid Recto-circular profile");
        return;
      }

      // ITrapezoidProfile
      else if (profile.StartsWith("STD TR")) {
        this.AddRuntimeError("Unable to expand Trapezoid profile");
        return;
      }

      // ITSectionProfile
      else if (profile.StartsWith("STD T")) {
        double height = ValueFromString(parts[2], unit);
        double width = ValueFromString(parts[3], unit);
        double webThk = ValueFromString(parts[4], unit);
        double flangeThk = ValueFromString(parts[5], unit);

        if (section.ApiSection.BasicOffset != BasicOffset.Centroid) {
          switch (section.ApiSection.BasicOffset) {
            case BasicOffset.Top:
              pln.Origin = pln.PointAt(
                section.SectionProperties.Cz.As(_lengthUnit),
                0);
              break;

            case BasicOffset.TopLeft:
              pln.Origin = pln.PointAt(
                section.SectionProperties.Cz.As(_lengthUnit),
                (width / 2) + section.SectionProperties.Cy.As(_lengthUnit));
              break;

            case BasicOffset.TopRight:
              pln.Origin = pln.PointAt(
                section.SectionProperties.Cz.As(_lengthUnit),
                (-width / 2) + section.SectionProperties.Cy.As(_lengthUnit));
              break;

            case BasicOffset.Left:
              pln.Origin = pln.PointAt(
                0,
                (width / 2) + section.SectionProperties.Cy.As(_lengthUnit));
              break;

            case BasicOffset.Right:
              pln.Origin = pln.PointAt(
                0,
                (-width / 2) + section.SectionProperties.Cy.As(_lengthUnit));
              break;

            case BasicOffset.Bottom:
              pln.Origin = pln.PointAt(
                height + section.SectionProperties.Cz.As(_lengthUnit),
                0);
              break;

            case BasicOffset.BottomLeft:
              pln.Origin = pln.PointAt(
                height + section.SectionProperties.Cz.As(_lengthUnit),
                (width / 2) + section.SectionProperties.Cy.As(_lengthUnit));
              break;

            case BasicOffset.BottomRight:
              pln.Origin = pln.PointAt(
                height + section.SectionProperties.Cz.As(_lengthUnit),
                (-width / 2) + section.SectionProperties.Cy.As(_lengthUnit));
              break;
          }
        }

        Brep web = ExtrudePlate(pln, crv,
          -flangeThk / 2, 0,
          -height, 0);
        mem2ds.Add(CreateMember2d(web, webThk, section));

        Brep flange = ExtrudePlate(pln, crv,
          -flangeThk / 2, width / 2,
          -flangeThk / 2, -width / 2);
        mem2ds.Add(CreateMember2d(flange, flangeThk, section));
      } else if (profile.StartsWith("CAT")) {
        string cat = " " + profile.Split(' ')[1] + " ";
        string prof = profile.Split(' ')[2];

        if (!cat.Contains("UC ")) {
          foreach (string value in new List<string>() { "UPE", "PFC", "UPN", "-U ", "-CH", "C ", " MC ", " WT ", " MT ", " ST ", "-EA", "-UA", "-RHS", "-SHS", "-FLATS", "-ROUNDS", "-SQUARES", " ISJC", "T ", "-CPF", "-IA", "-L", "-2L", "UE-AM", "C-AM", "EA-AM" }) {
            if (cat.Contains(value)) {
              this.AddRuntimeError("Unable to expand none-I Catalogue profile");
              return;
            }
          }
        }

        List<double> sqlValues = SqlReader.Instance.GetCatalogueProfileValues(prof,
          Path.Combine(AddReferencePriority.InstallPath, "sectlib.db3"));
        unit = LengthUnit.Meter;
        sqlValues.ForEach(x => new Length(x, unit).As(_lengthUnit));

        if (sqlValues.Count == 2) {
          this.AddRuntimeError("Unable to expand none-I Catalogue profile");
          return;
        } else {
          double height = sqlValues[0];
          double width = sqlValues[1];
          double webThk = sqlValues[2];
          double flangeThk = sqlValues[3];

          pln.Origin = BaseOffset(pln, section, height, width);

          Brep topFlange = ExtrudePlate(pln, crv,
            (height / 2) - (flangeThk / 2), -width / 2,
            (height / 2) - (flangeThk / 2), width / 2);
          mem2ds.Add(CreateMember2d(topFlange, flangeThk, section));

          Brep web = ExtrudePlate(pln, crv,
            (height / 2) - (flangeThk / 2), 0,
            -(height / 2) + (flangeThk / 2), 0);
          mem2ds.Add(CreateMember2d(web, webThk, section));

          Brep bottomFlange = ExtrudePlate(pln, crv,
            -(height / 2) + (flangeThk / 2), -width / 2,
            -(height / 2) + (flangeThk / 2), width / 2);
          mem2ds.Add(CreateMember2d(bottomFlange, flangeThk, section));
        }
      } else {
        this.AddRuntimeError($"Unable to expand profile: {profile}");
        return;
      }

      da.SetDataList(0, mem2ds.ConvertAll(x => new GsaMember2dGoo(x)));
    }

    private Brep ExtrudePlate(Plane plane, Curve crv, double z1, double y1,
      double z2, double y2) {
      Point3d pt1 = plane.PointAt(z1, y1);
      Point3d pt2 = plane.PointAt(z2, y2);
      var ln = new LineCurve(pt1, pt2);
      var sweepOneRail = new SweepOneRail();
      Brep[] sweep = sweepOneRail.PerformSweep(crv, ln);
      return sweep[0];
    }

    private Brep ExtrudePlate(Curve crv, Point3d pt1, Point3d pt2) {
      var ln = new LineCurve(pt1, pt2);
      var sweepOneRail = new SweepOneRail();
      Brep[] sweep = sweepOneRail.PerformSweep(crv, ln);
      return sweep[0];
    }
    private GsaMember2D CreateMember2d(Brep brep, double thickness, GsaSection section) {
      return new GsaMember2D(brep) {
        Prop2d = new GsaProperty2d(new Length(thickness, _lengthUnit)) {
          Material = section.Material
        }
      };
    }

    private Point3d BaseOffset(Plane plane, GsaSection section, double h, double w) {
      return section.ApiSection.BasicOffset switch {
        BasicOffset.Top => plane.PointAt(-h / 2, 0),
        BasicOffset.TopLeft => plane.PointAt(-h / 2, w / 2),
        BasicOffset.TopRight => plane.PointAt(-h / 2, -w / 2),
        BasicOffset.Bottom => plane.PointAt(h / 2, 0),
        BasicOffset.BottomLeft => plane.PointAt(h / 2, w / 2),
        BasicOffset.BottomRight => plane.PointAt(h / 2, -w / 2),
        BasicOffset.Left => plane.PointAt(0, w / 2),
        BasicOffset.Right => plane.PointAt(0, -w / 2),
        _ => plane.Origin,
      };
    }

    private Point3d BaseOffset(Plane plane, GsaSection section, double h, double wLeft, double wRight) {
      return section.ApiSection.BasicOffset switch {
        BasicOffset.Top => plane.PointAt(-h / 2, 0),
        BasicOffset.TopLeft => plane.PointAt(-h / 2, wLeft / 2),
        BasicOffset.TopRight => plane.PointAt(-h / 2, -wRight / 2),
        BasicOffset.Bottom => plane.PointAt(h / 2, 0),
        BasicOffset.BottomLeft => plane.PointAt(h / 2, wLeft / 2),
        BasicOffset.BottomRight => plane.PointAt(h / 2, -wRight / 2),
        BasicOffset.Left => plane.PointAt(0, wLeft / 2),
        BasicOffset.Right => plane.PointAt(0, -wRight / 2),
        _ => plane.Origin,
      };
    }

    private double ValueFromString(string value, LengthUnit unit) {
      double val = double.Parse(value, CultureInfo.InvariantCulture);
      var length = new Length(val, unit);
      return length.As(_lengthUnit);
    }

    private void Update(string unit) {
      _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), unit);
      Message = unit;
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
    }
  }
}
