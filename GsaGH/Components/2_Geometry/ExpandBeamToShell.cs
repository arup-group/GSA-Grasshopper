using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GsaGH.Helpers.Assembly;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;
using Oasys.Taxonomy.Profiles;
using OasysGH;
using OasysGH.Components;
using OasysGH.Helpers;
using OasysGH.Parameters;
using OasysUnits.Units;
using OasysUnits;
using Rhino.Geometry;
using LengthUnit = OasysUnits.Units.LengthUnit;
using Line = Rhino.Geometry.Line;
using OasysGH.Units;
using GH_IO.Serialization;
using OasysGH.Units.Helpers;
using System.Windows.Forms;
using GsaGH.Helpers.Import;
using System.Linq;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to edit a Node
  /// </summary>
  public class ExpandBeamToShell : GH_OasysComponent, IGH_VariableParameterComponent {
    public override Guid ComponentGuid => new Guid("42221f6b-b0f1-41ae-abcf-df5521565464");
    public override GH_Exposure Exposure => GH_Exposure.quinary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.ExpandBeamToShell;
    private LengthUnit _lengthUnit = DefaultUnits.LengthUnitGeometry;
    public ExpandBeamToShell() : base("Expand Beam to Shell", "B2S",
      "Expand 1D Entities to 2D Entities from profile, orientation and offset",
      CategoryName.Name(), SubCategoryName.Cat2()) { }

    public override void AppendAdditionalMenuItems(ToolStripDropDown menu) {
      if (!(menu is ContextMenuStrip)) {
        return; // this method is also called when clicking EWR balloon
      }

      Menu_AppendSeparator(menu);

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
      return base.Read(reader);
    }

    public void VariableParameterMaintenance() { }

    public override bool Write(GH_IWriter writer) {
      writer.SetString("LengthUnit", _lengthUnit.ToString());
      return base.Write(writer);
    }

    protected override void BeforeSolveInstance() {
      Message = Length.GetAbbreviation(_lengthUnit);
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

      GsaMember1d member;
      GsaElement1d element;
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
            if (member.Section == null) {

            }

            axes = member.LocalAxes;
            if (axes == null) {
              var assembly = new ModelAssembly(member);
              var model = new GsaModel {
                Model = assembly.GetModel()
              };

              axes = new Parameters.LocalAxes(model.Model.MemberDirectionCosine(1));
            }

            crv = member.PolyCurve.DuplicatePolyCurve();
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
            axes = element.LocalAxes;
            if (axes == null) {
              var assembly = new ModelAssembly(element);
              var model = new GsaModel() {
                Model = assembly.GetModel()
              };

              axes = new Parameters.LocalAxes(model.Model.ElementDirectionCosine(1));
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

      crv.LengthParameter(offset.X1.As(_lengthUnit), out double t0);
      crv.LengthParameter(crv.GetLength() - offset.X2.As(_lengthUnit), out double t1);
      crv = crv.Trim(t0, t1);
      LengthUnit unit = LengthUnit.Meter;
      var mem2ds = new List<GsaMember2d>();
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
      if (angleString.Length > 1) {
        double angle = double.Parse(angleString[1]);
        pln.Rotate(Rhino.RhinoMath.ToRadians(angle), axes.X);
      }

      string[] parts = profile.Split(' ');
      unit = LengthUnit.Millimeter;
      string[] type = parts[1].Split('(', ')');
      if (type.Length > 1) {
        UnitParser parser = UnitParser.Default;
        unit = parser.Parse<LengthUnit>(type[1]);
      }

      // angle
      if (profile.StartsWith("STD A")) {
        double height = ValueFromString(parts[2], unit);
        double width = ValueFromString(parts[3], unit);
        double webThk = ValueFromString(parts[4], unit);
        double flangeThk = ValueFromString(parts[5], unit);

        pln.Origin = BaseOffset(pln, section, height, width);

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

        pln.Origin = BaseOffset(pln, section, height, width);

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
        //SetOutput(da, i++, parts[2], unit); //Depth
        //da.SetData(i++, null); //Width
        //da.SetData(i++, null); //Width Top
        //da.SetData(i++, null); //Width Bottom
        //da.SetData(i++, null); //Flange Thk Top
        //da.SetData(i++, null); //Flange Thk Bottom
        //SetOutput(da, i++, parts[3], unit); //Web Thk Bottom
        //da.SetData(i++, null); //Root radius
        //da.SetData(i++, null); //Spacing
        //da.SetData(i, type[0]);
        this.AddRuntimeError("Unable to expand CHS profile");
        return;
      }

      // circle
      else if (profile.StartsWith("STD C ") || profile.StartsWith("STD C(")) {
        //SetOutput(da, i++, parts[2], unit); //Depth
        //da.SetData(i++, null); //Width
        //da.SetData(i++, null); //Width Top
        //da.SetData(i++, null); //Width Bottom
        //da.SetData(i++, null); //Flange Thk Top
        //da.SetData(i++, null); //Flange Thk Bottom
        //da.SetData(i++, null); //Web Thk Bottom
        //da.SetData(i++, null); //Root radius
        //da.SetData(i++, null); //Spacing
        //da.SetData(i, type[0]);
        this.AddRuntimeError("Unable to expand Circle profile");
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
        //SetOutput(da, i++, parts[2], unit); //Depth
        //SetOutput(da, i++, parts[3], unit); //Width
        //SetOutput(da, i++, parts[3], unit); //Width Top
        //SetOutput(da, i++, parts[3], unit); //Width Bottom
        //SetOutput(da, i++, parts[4], unit); //Flange Thk Top
        //SetOutput(da, i++, parts[4], unit); //Flange Thk Bottom
        //SetOutput(da, i++, parts[4], unit); //Web Thk Bottom
        //da.SetData(i++, null); //Root radius
        //da.SetData(i++, null); //Spacing
        //da.SetData(i, type[0]);
        this.AddRuntimeError("Unable to expand Oval profile");
        return;
      }

      // IEllipseProfile
      else if (profile.StartsWith("STD E")) {
        //SetOutput(da, i++, parts[2], unit); //Depth
        //SetOutput(da, i++, parts[3], unit); //Width
        //SetOutput(da, i++, parts[3], unit); //Width Top
        //SetOutput(da, i++, parts[3], unit); //Width Bottom
        //da.SetData(i++, null); //Flange Thk Top
        //da.SetData(i++, null); //Flange Thk Bottom
        //da.SetData(i++, null); //Web Thk Bottom
        //da.SetData(i++, null); //Root radius
        //da.SetData(i++, null); //Spacing
        //da.SetData(i, type[0]);
        this.AddRuntimeError("Unable to expand Ellipse profile");
        return;
      }

      // IGeneralCProfile
      else if (profile.StartsWith("STD GC")) {
        double height = ValueFromString(parts[2], unit);
        double width = ValueFromString(parts[3], unit);
        double flangeThk = ValueFromString(parts[5], unit);
        double lip = ValueFromString(parts[4], unit);

        pln.Origin = BaseOffset(pln, section, height, width);

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

        pln.Origin = BaseOffset(pln, section, height, bottomWidth, topWidth);

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

        pln.Origin = BaseOffset(pln, section, height, Math.Max(bottomWidth, topWidth));

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
        //SetOutput(da, i++, parts[2], unit); //Depth/Diameter
        //SetOutput(da, i++, parts[3], unit); //Width
        //SetOutput(da, i++, parts[3], unit); //Width Top
        //SetOutput(da, i++, parts[3], unit); //Width Bottom
        //SetOutput(da, i++, parts[5], unit); //Flange Thk Top
        //SetOutput(da, i++, parts[5], unit); //Flange Thk Bottom
        //SetOutput(da, i++, parts[4], unit); //Web Thk Bottom
        //SetOutput(da, i++, parts[6], unit); //hole size
        //SetOutput(da, i++, parts[7], unit); //pitch
        //da.SetData(i, type[0]);
        this.AddRuntimeError("Unable to expand Cellular Beam profile");
        return;
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
          (height / 2) - (flangeThk / 2), -(width / 2) + (flangeThk / 2),
          -(height / 2) + (flangeThk / 2), -(width / 2) + (flangeThk / 2));
        mem2ds.Add(CreateMember2d(webLeft, webThk, section));

        Brep webRight = ExtrudePlate(pln, crv,
          (height / 2) - (flangeThk / 2), (width / 2) - (flangeThk / 2),
          -(height / 2) + (flangeThk / 2), (width / 2) - (flangeThk / 2));
        mem2ds.Add(CreateMember2d(webRight, webThk, section));

        Brep topFlange = ExtrudePlate(pln, crv,
          (height / 2) - (flangeThk / 2), -(width / 2) + (flangeThk / 2),
          (height / 2) - (flangeThk / 2), (width / 2) - (flangeThk / 2));
        mem2ds.Add(CreateMember2d(topFlange, flangeThk, section));

        Brep bottomFlange = ExtrudePlate(pln, crv,
          -(height / 2) + (flangeThk / 2), -(width / 2) + (flangeThk / 2),
          -(height / 2) + (flangeThk / 2), (width / 2) - (flangeThk / 2));
        mem2ds.Add(CreateMember2d(bottomFlange, flangeThk, section));
      }

      // IRectangleProfile
      else if (profile.StartsWith("STD R ") || profile.StartsWith("STD R(")) {
        //SetOutput(da, i++, parts[2], unit); //Depth/Diameter
        //SetOutput(da, i++, parts[3], unit); //Width
        //SetOutput(da, i++, parts[3], unit); //Width Top
        //SetOutput(da, i++, parts[3], unit); //Width Bottom
        //da.SetData(i++, null); //Flange Thk Top
        //da.SetData(i++, null); //Flange Thk Bottom
        //da.SetData(i++, null); //Web Thk Bottom
        //da.SetData(i++, null); //Root radius
        //da.SetData(i++, null); //Spacing
        //da.SetData(i, type[0]);
        this.AddRuntimeError("Unable to expand Rectangle profile");
        return;
      }

      // IRectoEllipseProfile
      else if (profile.StartsWith("STD RE")) {
        //SetOutput(da, i++, parts[2], unit); //Depth
        //SetOutput(da, i++, parts[4], unit); //Width
        //SetOutput(da, i++, parts[3], unit); //Width Top
        //SetOutput(da, i++, parts[5], unit); //Width Bottom
        //da.SetData(i++, null); //Flange Thk Top
        //da.SetData(i++, null); //Flange Thk Bottom
        //da.SetData(i++, null); //Web Thk Bottom
        //da.SetData(i++, null); //Root radius
        //da.SetData(i++, null); //Spacing
        //da.SetData(i, type[0]);
        this.AddRuntimeError("Unable to expand Recto-ellipse profile");
        return;
      }

      // ISecantPileProfile
      else if (profile.StartsWith("STD SP")) {
        //SetOutput(da, i++, parts[2], unit); //Depth/Diameter
        //Length length;
        //if (profile.StartsWith("STD SPW")) {
        //  // STD SPW 250 100 4
        //  int count = int.Parse(parts[4], CultureInfo.InvariantCulture);
        //  double spacing = double.Parse(parts[3], CultureInfo.InvariantCulture);
        //  length = new Length(count * spacing, unit);
        //} else {
        //  // STD SP 250 100 4
        //  int count = int.Parse(parts[4], CultureInfo.InvariantCulture);
        //  double spacing = double.Parse(parts[3], CultureInfo.InvariantCulture);
        //  double diameter = double.Parse(parts[2], CultureInfo.InvariantCulture);
        //  length = new Length(((count - 1) * spacing) + diameter, unit);
        //}

        //da.SetData(i++, new GH_UnitNumber(length.ToUnit(_lengthUnit))); //Width
        //da.SetData(i++, null); //Width Top
        //da.SetData(i++, null); //Width Bottom
        //da.SetData(i++, null); //Flange Thk Top
        //da.SetData(i++, null); //Flange Thk Bottom
        //da.SetData(i++, null); //Web Thk Bottom
        //da.SetData(i++, null); //Root radius
        //SetOutput(da, i++, parts[3], unit); //Spacing
        //da.SetData(i, type[0]);

        this.AddRuntimeError("Unable to expand Secant pile profile");
        return;
      }

      // ISheetPileProfile
      else if (profile.StartsWith("STD SHT")) {
        //SetOutput(da, i++, parts[2], unit); //Depth
        //SetOutput(da, i++, parts[3], unit); //Width
        //SetOutput(da, i++, parts[4], unit); //Width Top
        //SetOutput(da, i++, parts[5], unit); //Width Bottom
        //SetOutput(da, i++, parts[6], unit); //Flange Thk Top
        //SetOutput(da, i++, parts[6], unit); //Flange Thk Bottom
        //SetOutput(da, i++, parts[7], unit); //Web Thk Bottom
        //da.SetData(i++, null); //Root radius
        //da.SetData(i++, null); //Spacing
        //da.SetData(i, type[0]);

        this.AddRuntimeError("Unable to expand Sheet pile profile");
        return;
      }

      // IStadiumProfile
      else if (profile.StartsWith("STD RC")) {
        //SetOutput(da, i++, parts[2], unit); //Depth
        //SetOutput(da, i++, parts[3], unit); //Width
        //da.SetData(i++, null); //Width Top
        //da.SetData(i++, null); //Width Bottom
        //da.SetData(i++, null); //Flange Thk Top
        //da.SetData(i++, null); //Flange Thk Bottom
        //da.SetData(i++, null); //Web Thk Bottom
        //da.SetData(i++, null); //Root radius
        //da.SetData(i++, null); //Spacing
        //da.SetData(i, type[0]);
        this.AddRuntimeError("Unable to expand Recto-circular profile");
        return;
      }

      // ITrapezoidProfile
      else if (profile.StartsWith("STD TR")) {
        //double top = double.Parse(parts[3], CultureInfo.InvariantCulture);
        //double bottom = double.Parse(parts[4], CultureInfo.InvariantCulture);
        //var length = new Length(Math.Max(top, bottom), unit);
        //SetOutput(da, i++, parts[2], unit); //Depth
        //da.SetData(i++, new GH_UnitNumber(length.ToUnit(_lengthUnit))); //Width
        //SetOutput(da, i++, parts[3], unit); //Width Top
        //SetOutput(da, i++, parts[4], unit); //Width Bottom
        //da.SetData(i++, null); //Flange Thk Top
        //da.SetData(i++, null); //Flange Thk Bottom
        //da.SetData(i++, null); //Web Thk Bottom
        //da.SetData(i++, null); //Root radius
        //da.SetData(i++, null); //Spacing
        //da.SetData(i, type[0]);
        this.AddRuntimeError("Unable to expand Trapezoid profile");
        return;
      }

      // ITSectionProfile
      else if (profile.StartsWith("STD T")) {
        double height = ValueFromString(parts[2], unit);
        double width = ValueFromString(parts[3], unit);
        double webThk = ValueFromString(parts[4], unit);
        double flangeThk = ValueFromString(parts[5], unit);

        pln.Origin = BaseOffset(pln, section, height, width);

        Brep web = ExtrudePlate(pln, crv,
          -flangeThk / 2, 0,
          -height, 0);
        mem2ds.Add(CreateMember2d(web, webThk, section));

        Brep flange = ExtrudePlate(pln, crv,
          -flangeThk / 2, width / 2,
          -flangeThk / 2, -width / 2);
        mem2ds.Add(CreateMember2d(flange, flangeThk, section));
      } else if (profile.StartsWith("CAT")) {
        string prof = profile.Split(' ')[2];
        List<double> sqlValues = SqlReader.Instance.GetCatalogueProfileValues(prof,
          Path.Combine(AddReferencePriority.InstallPath, "sectlib.db3"));
        unit = LengthUnit.Meter;
        sqlValues.ForEach(x => new Length(x, unit).As(_lengthUnit));

        //if (sqlValues.Count == 2) {
          //    da.SetData(i++,
          //      new GH_UnitNumber(new Length(sqlValues[0], unit).ToUnit(_lengthUnit))); //Depth
          //    da.SetData(i++,
          //      new GH_UnitNumber(new Length(sqlValues[0], unit).ToUnit(_lengthUnit))); //Width
          //    da.SetData(i++, null); //Width Top
          //    da.SetData(i++, null); //Width Bottom
          //    da.SetData(i++, null); //Flange Thk Top
          //    da.SetData(i++, null); //Flange Thk Bottom
          //    da.SetData(i++,
          //      new GH_UnitNumber(
          //        new Length(sqlValues[1], unit).ToUnit(_lengthUnit))); //Web Thk Bottom
          //    da.SetData(i++, null); //root radius
          //    da.SetData(i++, null); //Spacing
        //} else {
          
          //    da.SetData(i++,
          //      new GH_UnitNumber(new Length(sqlValues[0], unit).ToUnit(_lengthUnit))); //Depth
          //    da.SetData(i++,
          //      new GH_UnitNumber(new Length(sqlValues[1], unit).ToUnit(_lengthUnit))); //Width
          //    da.SetData(i++,
          //      new GH_UnitNumber(new Length(sqlValues[1], unit).ToUnit(_lengthUnit))); //Width Top
          //    da.SetData(i++,
          //      new GH_UnitNumber(new Length(sqlValues[1], unit).ToUnit(_lengthUnit))); //Width Bottom
          //    da.SetData(i++,
          //    new GH_UnitNumber(
          //        new Length(sqlValues[3], unit).ToUnit(_lengthUnit))); //Flange Thk Top
          //    da.SetData(i++,
          //    new GH_UnitNumber(
          //        new Length(sqlValues[3], unit).ToUnit(_lengthUnit))); //Flange Thk Bottom
          //    da.SetData(i++,
          //      new GH_UnitNumber(
          //        new Length(sqlValues[2], unit).ToUnit(_lengthUnit))); //Web Thk Bottom
          //    da.SetData(i++,
          //    sqlValues.Count > 4 ?
          //        new GH_UnitNumber(new Length(sqlValues[4], unit).ToUnit(_lengthUnit)) :
          //        new GH_UnitNumber(
          //          Length.Zero.ToUnit(_lengthUnit))); // welded section don´t have a root radius
          //                                             //Root radius
          //    da.SetData(i++, null); //Spacing

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

    private GsaMember2d CreateMember2d(Brep brep, double thickness, GsaSection section) {
      return new GsaMember2d(brep) {
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
