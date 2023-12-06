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

namespace GsaGH.Components {
  /// <summary>
  ///   Component to edit a Node
  /// </summary>
  public class ExpandBeamToShell : GH_OasysComponent {
    public override Guid ComponentGuid => new Guid("42221f6b-b0f1-41ae-abcf-df5521565464");
    public override GH_Exposure Exposure => GH_Exposure.quinary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.LocalAxes;
    public ExpandBeamToShell() : base("Expand Beam to Shell", "B2S",
      "Expand 1D Entities to 2D Entities from profile, orientation and offset",
      CategoryName.Name(), SubCategoryName.Cat2()) { }

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
      string profile = string.Empty;
      PolyCurve crv;
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

            crv = member.PolyCurve;
            offset = member.Offset;
            profile = member.Section.ApiSection.Profile;
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

            crv = new PolyCurve();
            crv.Append(element.Line);
            offset = element.Offset;
            profile = element.Section.ApiSection.Profile;
            break;
          }
        default:
          this.AddRuntimeError("Unable to convert input to Element1D or Member1D");
          return;
      }

      if (profile.Trim() == string.Empty) {
        this.AddRuntimeError("Element/Member has no section with valid profile");
        return;
      }

      var mem2ds = new List<GsaMember2d>();
      var pt = new Point3d(crv.PointAtStart);
      var pln = new Plane(crv.PointAtStart, axes.X, axes.Y);
      var translation = new Vector3d(
        offset.X1.Value, offset.Y.Value, offset.Z.Value);
      var mapToLocal = Transform.ChangeBasis(pln, Plane.WorldXY);
      translation.Transform(mapToLocal);
      var move = Transform.Translation(translation);
      pt.Transform(move);
      pln.Origin = pt;

      // angle
      if (profile.StartsWith("STD A")) {
        SetOutput(da, i++, parts[2], unit); //Depth
        SetOutput(da, i++, parts[3], unit); //Width
        SetOutput(da, i++, parts[3], unit); //Width Top
        SetOutput(da, i++, parts[3], unit); //Width Bottom
        SetOutput(da, i++, parts[5], unit); //Flange Thk Top
        SetOutput(da, i++, parts[5], unit); //Flange Thk Bottom
        SetOutput(da, i++, parts[4], unit); //Web Thk Bottom
        da.SetData(i++, null); //Root radius
        da.SetData(i++, null); //Spacing
        da.SetData(i, type[0]);
      }

      // channel
      else if (profile.StartsWith("STD CH ") || profile.StartsWith("STD CH(")) {
        SetOutput(da, i++, parts[2], unit); //Depth
        SetOutput(da, i++, parts[3], unit); //Width
        SetOutput(da, i++, parts[3], unit); //Width Top
        SetOutput(da, i++, parts[3], unit); //Width Bottom
        SetOutput(da, i++, parts[5], unit); //Flange Thk Top
        SetOutput(da, i++, parts[5], unit); //Flange Thk Bottom
        SetOutput(da, i++, parts[4], unit); //Web Thk Bottom
        da.SetData(i++, null); //Root radius
        da.SetData(i++, null); //Spacing
        da.SetData(i, type[0]);
      }

      // circle hollow
      else if (profile.StartsWith("STD CHS")) {
        SetOutput(da, i++, parts[2], unit); //Depth
        da.SetData(i++, null); //Width
        da.SetData(i++, null); //Width Top
        da.SetData(i++, null); //Width Bottom
        da.SetData(i++, null); //Flange Thk Top
        da.SetData(i++, null); //Flange Thk Bottom
        SetOutput(da, i++, parts[3], unit); //Web Thk Bottom
        da.SetData(i++, null); //Root radius
        da.SetData(i++, null); //Spacing
        da.SetData(i, type[0]);
      }

      // circle
      else if (profile.StartsWith("STD C ") || profile.StartsWith("STD C(")) {
        SetOutput(da, i++, parts[2], unit); //Depth
        da.SetData(i++, null); //Width
        da.SetData(i++, null); //Width Top
        da.SetData(i++, null); //Width Bottom
        da.SetData(i++, null); //Flange Thk Top
        da.SetData(i++, null); //Flange Thk Bottom
        da.SetData(i++, null); //Web Thk Bottom
        da.SetData(i++, null); //Root radius
        da.SetData(i++, null); //Spacing
        da.SetData(i, type[0]);
      }

      // ICruciformSymmetricalProfile
      else if (profile.StartsWith("STD X")) {
        SetOutput(da, i++, parts[2], unit); //Depth
        SetOutput(da, i++, parts[3], unit); //Width
        SetOutput(da, i++, parts[3], unit); //Width Top
        SetOutput(da, i++, parts[3], unit); //Width Bottom
        SetOutput(da, i++, parts[5], unit); //Flange Thk Top
        SetOutput(da, i++, parts[5], unit); //Flange Thk Bottom
        SetOutput(da, i++, parts[4], unit); //Web Thk Bottom
        da.SetData(i++, null); //Root radius
        da.SetData(i++, null); //Spacing
        da.SetData(i, type[0]);
      }

      // IEllipseHollowProfile
      else if (profile.StartsWith("STD OVAL")) {
        SetOutput(da, i++, parts[2], unit); //Depth
        SetOutput(da, i++, parts[3], unit); //Width
        SetOutput(da, i++, parts[3], unit); //Width Top
        SetOutput(da, i++, parts[3], unit); //Width Bottom
        SetOutput(da, i++, parts[4], unit); //Flange Thk Top
        SetOutput(da, i++, parts[4], unit); //Flange Thk Bottom
        SetOutput(da, i++, parts[4], unit); //Web Thk Bottom
        da.SetData(i++, null); //Root radius
        da.SetData(i++, null); //Spacing
        da.SetData(i, type[0]);
      }

      // IEllipseProfile
      else if (profile.StartsWith("STD E")) {
        SetOutput(da, i++, parts[2], unit); //Depth
        SetOutput(da, i++, parts[3], unit); //Width
        SetOutput(da, i++, parts[3], unit); //Width Top
        SetOutput(da, i++, parts[3], unit); //Width Bottom
        da.SetData(i++, null); //Flange Thk Top
        da.SetData(i++, null); //Flange Thk Bottom
        da.SetData(i++, null); //Web Thk Bottom
        da.SetData(i++, null); //Root radius
        da.SetData(i++, null); //Spacing
        da.SetData(i, type[0]);
      }

      // IGeneralCProfile
      else if (profile.StartsWith("STD GC")) {
        SetOutput(da, i++, parts[2], unit); //Depth/Diameter
        SetOutput(da, i++, parts[3], unit); //Width
        SetOutput(da, i++, parts[3], unit); //Width Top
        SetOutput(da, i++, parts[3], unit); //Width Bottom
        SetOutput(da, i++, parts[4], unit); //Flange Thk Top
        SetOutput(da, i++, parts[4], unit); //Flange Thk Bottom
        SetOutput(da, i++, parts[5], unit); //Web Thk Bottom
        da.SetData(i++, null); //Root radius
        da.SetData(i++, null); //Spacing
        da.SetData(i, type[0]);
      }

      // IGeneralZProfile
      else if (profile.StartsWith("STD GZ")) {
        double top = double.Parse(parts[3], CultureInfo.InvariantCulture);
        double bottom = double.Parse(parts[4], CultureInfo.InvariantCulture);
        var length = new Length(top + bottom, unit);
        SetOutput(da, i++, parts[2], unit); //Depth
        da.SetData(i++, new GH_UnitNumber(length.ToUnit(_lengthUnit))); //Width
        SetOutput(da, i++, parts[3], unit); //Width Top
        SetOutput(da, i++, parts[4], unit); //Width Bottom
        SetOutput(da, i++, parts[5], unit); //Flange Thk Top
        SetOutput(da, i++, parts[6], unit); //Flange Thk Bottom
        SetOutput(da, i++, parts[7], unit); //Web Thk Bottom
        da.SetData(i++, null); //Root radius
        da.SetData(i++, null); //Spacing
        da.SetData(i, type[0]);
      }

      // IIBeamAsymmetricalProfile
      else if (profile.StartsWith("STD GI")) {
        double top = double.Parse(parts[3], CultureInfo.InvariantCulture);
        double bottom = double.Parse(parts[4], CultureInfo.InvariantCulture);
        var length = new Length(Math.Max(top, bottom), unit);
        SetOutput(da, i++, parts[2], unit); //Depth
        da.SetData(i++, new GH_UnitNumber(length.ToUnit(_lengthUnit))); //Width
        SetOutput(da, i++, parts[3], unit); //Width Top
        SetOutput(da, i++, parts[4], unit); //Width Bottom
        SetOutput(da, i++, parts[6], unit); //Flange Thk Top
        SetOutput(da, i++, parts[7], unit); //Flange Thk Bottom
        SetOutput(da, i++, parts[5], unit); //Web Thk Bottom
        da.SetData(i++, null); //Root radius
        da.SetData(i++, null); //Spacing
        da.SetData(i, type[0]);
      }

      // IIBeamCellularProfile
      else if (profile.StartsWith("STD CB")) {
        SetOutput(da, i++, parts[2], unit); //Depth/Diameter
        SetOutput(da, i++, parts[3], unit); //Width
        SetOutput(da, i++, parts[3], unit); //Width Top
        SetOutput(da, i++, parts[3], unit); //Width Bottom
        SetOutput(da, i++, parts[5], unit); //Flange Thk Top
        SetOutput(da, i++, parts[5], unit); //Flange Thk Bottom
        SetOutput(da, i++, parts[4], unit); //Web Thk Bottom
        SetOutput(da, i++, parts[6], unit); //hole size
        SetOutput(da, i++, parts[7], unit); //pitch
        da.SetData(i, type[0]);
      }

      // IIBeamSymmetricalProfile
      else if (profile.StartsWith("STD I")) {
        SetOutput(da, i++, parts[2], unit); //Depth/Diameter
        SetOutput(da, i++, parts[3], unit); //Width
        SetOutput(da, i++, parts[3], unit); //Width Top
        SetOutput(da, i++, parts[3], unit); //Width Bottom
        SetOutput(da, i++, parts[5], unit); //Flange Thk Top
        SetOutput(da, i++, parts[5], unit); //Flange Thk Bottom
        SetOutput(da, i++, parts[4], unit); //Web Thk Bottom
        da.SetData(i++, null); //Root radius
        da.SetData(i++, null); //Spacing
        da.SetData(i, type[0]);
      }

      // IRectangleHollowProfile
      else if (profile.StartsWith("STD RHS")) {
        SetOutput(da, i++, parts[2], unit); //Depth/Diameter
        SetOutput(da, i++, parts[3], unit); //Width
        SetOutput(da, i++, parts[3], unit); //Width Top
        SetOutput(da, i++, parts[3], unit); //Width Bottom
        SetOutput(da, i++, parts[5], unit); //Flange Thk Top
        SetOutput(da, i++, parts[5], unit); //Flange Thk Bottom
        SetOutput(da, i++, parts[4], unit); //Web Thk Bottom
        da.SetData(i++, null); //Root radius
        da.SetData(i++, null); //Spacing
        da.SetData(i, type[0]);
      }

      // IRectangleProfile
      else if (profile.StartsWith("STD R ") || profile.StartsWith("STD R(")) {
        SetOutput(da, i++, parts[2], unit); //Depth/Diameter
        SetOutput(da, i++, parts[3], unit); //Width
        SetOutput(da, i++, parts[3], unit); //Width Top
        SetOutput(da, i++, parts[3], unit); //Width Bottom
        da.SetData(i++, null); //Flange Thk Top
        da.SetData(i++, null); //Flange Thk Bottom
        da.SetData(i++, null); //Web Thk Bottom
        da.SetData(i++, null); //Root radius
        da.SetData(i++, null); //Spacing
        da.SetData(i, type[0]);
      }

      // IRectoEllipseProfile
      else if (profile.StartsWith("STD RE")) {
        SetOutput(da, i++, parts[2], unit); //Depth
        SetOutput(da, i++, parts[4], unit); //Width
        SetOutput(da, i++, parts[3], unit); //Width Top
        SetOutput(da, i++, parts[5], unit); //Width Bottom
        da.SetData(i++, null); //Flange Thk Top
        da.SetData(i++, null); //Flange Thk Bottom
        da.SetData(i++, null); //Web Thk Bottom
        da.SetData(i++, null); //Root radius
        da.SetData(i++, null); //Spacing
        da.SetData(i, type[0]);
      }

      // ISecantPileProfile
      else if (profile.StartsWith("STD SP")) {
        SetOutput(da, i++, parts[2], unit); //Depth/Diameter
        Length length;
        if (profile.StartsWith("STD SPW")) {
          // STD SPW 250 100 4
          int count = int.Parse(parts[4], CultureInfo.InvariantCulture);
          double spacing = double.Parse(parts[3], CultureInfo.InvariantCulture);
          length = new Length(count * spacing, unit);
        } else {
          // STD SP 250 100 4
          int count = int.Parse(parts[4], CultureInfo.InvariantCulture);
          double spacing = double.Parse(parts[3], CultureInfo.InvariantCulture);
          double diameter = double.Parse(parts[2], CultureInfo.InvariantCulture);
          length = new Length(((count - 1) * spacing) + diameter, unit);
        }

        da.SetData(i++, new GH_UnitNumber(length.ToUnit(_lengthUnit))); //Width
        da.SetData(i++, null); //Width Top
        da.SetData(i++, null); //Width Bottom
        da.SetData(i++, null); //Flange Thk Top
        da.SetData(i++, null); //Flange Thk Bottom
        da.SetData(i++, null); //Web Thk Bottom
        da.SetData(i++, null); //Root radius
        SetOutput(da, i++, parts[3], unit); //Spacing
        da.SetData(i, type[0]);
      }

      // ISheetPileProfile
      else if (profile.StartsWith("STD SHT")) {
        SetOutput(da, i++, parts[2], unit); //Depth
        SetOutput(da, i++, parts[3], unit); //Width
        SetOutput(da, i++, parts[4], unit); //Width Top
        SetOutput(da, i++, parts[5], unit); //Width Bottom
        SetOutput(da, i++, parts[6], unit); //Flange Thk Top
        SetOutput(da, i++, parts[6], unit); //Flange Thk Bottom
        SetOutput(da, i++, parts[7], unit); //Web Thk Bottom
        da.SetData(i++, null); //Root radius
        da.SetData(i++, null); //Spacing
        da.SetData(i, type[0]);
      }

      // IStadiumProfile
      else if (profile.StartsWith("STD RC")) {
        SetOutput(da, i++, parts[2], unit); //Depth
        SetOutput(da, i++, parts[3], unit); //Width
        da.SetData(i++, null); //Width Top
        da.SetData(i++, null); //Width Bottom
        da.SetData(i++, null); //Flange Thk Top
        da.SetData(i++, null); //Flange Thk Bottom
        da.SetData(i++, null); //Web Thk Bottom
        da.SetData(i++, null); //Root radius
        da.SetData(i++, null); //Spacing
        da.SetData(i, type[0]);
      }

      // ITrapezoidProfile
      else if (profile.StartsWith("STD TR")) {
        double top = double.Parse(parts[3], CultureInfo.InvariantCulture);
        double bottom = double.Parse(parts[4], CultureInfo.InvariantCulture);
        var length = new Length(Math.Max(top, bottom), unit);
        SetOutput(da, i++, parts[2], unit); //Depth
        da.SetData(i++, new GH_UnitNumber(length.ToUnit(_lengthUnit))); //Width
        SetOutput(da, i++, parts[3], unit); //Width Top
        SetOutput(da, i++, parts[4], unit); //Width Bottom
        da.SetData(i++, null); //Flange Thk Top
        da.SetData(i++, null); //Flange Thk Bottom
        da.SetData(i++, null); //Web Thk Bottom
        da.SetData(i++, null); //Root radius
        da.SetData(i++, null); //Spacing
        da.SetData(i, type[0]);
      }

      // ITSectionProfile
      else if (profile.StartsWith("STD T")) {
        SetOutput(da, i++, parts[2], unit); //Depth
        SetOutput(da, i++, parts[3], unit); //Width
        SetOutput(da, i++, parts[3], unit); //Width Top
        SetOutput(da, i++, parts[4], unit); //Width Bottom
        SetOutput(da, i++, parts[5], unit); //Flange Thk Top
        da.SetData(i++, null); //Flange Thk Bottom
        SetOutput(da, i++, parts[4], unit); //Web Thk Bottom
        da.SetData(i++, null); //Root radius
        da.SetData(i++, null); //Spacing
        da.SetData(i, type[0]);
      } else if (profile.StartsWith("CAT")) {
        string prof = profile.Split(' ')[2];
        List<double> sqlValues = SqlReader.Instance.GetCatalogueProfileValues(prof,
          Path.Combine(AddReferencePriority.InstallPath, "sectlib.db3"));
        unit = LengthUnit.Meter;
        if (sqlValues.Count == 2) {
          da.SetData(i++,
            new GH_UnitNumber(new Length(sqlValues[0], unit).ToUnit(_lengthUnit))); //Depth
          da.SetData(i++,
            new GH_UnitNumber(new Length(sqlValues[0], unit).ToUnit(_lengthUnit))); //Width
          da.SetData(i++, null); //Width Top
          da.SetData(i++, null); //Width Bottom
          da.SetData(i++, null); //Flange Thk Top
          da.SetData(i++, null); //Flange Thk Bottom
          da.SetData(i++,
            new GH_UnitNumber(
              new Length(sqlValues[1], unit).ToUnit(_lengthUnit))); //Web Thk Bottom
          da.SetData(i++, null); //root radius
          da.SetData(i++, null); //Spacing
        } else {
          da.SetData(i++,
            new GH_UnitNumber(new Length(sqlValues[0], unit).ToUnit(_lengthUnit))); //Depth
          da.SetData(i++,
            new GH_UnitNumber(new Length(sqlValues[1], unit).ToUnit(_lengthUnit))); //Width
          da.SetData(i++,
            new GH_UnitNumber(new Length(sqlValues[1], unit).ToUnit(_lengthUnit))); //Width Top
          da.SetData(i++,
            new GH_UnitNumber(new Length(sqlValues[1], unit).ToUnit(_lengthUnit))); //Width Bottom
          da.SetData(i++,
          new GH_UnitNumber(
              new Length(sqlValues[3], unit).ToUnit(_lengthUnit))); //Flange Thk Top
          da.SetData(i++,
          new GH_UnitNumber(
              new Length(sqlValues[3], unit).ToUnit(_lengthUnit))); //Flange Thk Bottom
          da.SetData(i++,
            new GH_UnitNumber(
              new Length(sqlValues[2], unit).ToUnit(_lengthUnit))); //Web Thk Bottom
          da.SetData(i++,
          sqlValues.Count > 4 ?
              new GH_UnitNumber(new Length(sqlValues[4], unit).ToUnit(_lengthUnit)) :
              new GH_UnitNumber(
                Length.Zero.ToUnit(_lengthUnit))); // welded section don´t have a root radius
                                                   //Root radius
          da.SetData(i++, null); //Spacing
        }

        da.SetData(i, "CAT " + profile.Split(' ')[1]);
      } else {
        this.AddRuntimeError("Unable to get dimensions for type " + type[0]);
      }
    }
  }
}
