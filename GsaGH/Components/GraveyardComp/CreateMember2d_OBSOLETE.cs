using System;
using System.Collections.Generic;
using System.Drawing;
using GH_IO.Serialization;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Components.GraveyardComp;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;
using OasysGH;
using OasysGH.Components;
using OasysGH.Helpers;
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysUnits;
using OasysUnits.Units;
using Rhino.Geometry;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to create new 2D Member
  /// </summary>
  // ReSharper disable once InconsistentNaming
  public class CreateMember2d_OBSOLETE : GH_OasysDropDownComponent {
    public override Guid ComponentGuid => new Guid("df0c2786-9e46-4500-ab63-0c4162a580d4");
    public override GH_Exposure Exposure => GH_Exposure.hidden;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.CreateMem2d;
    private LengthUnit _lengthUnit = DefaultUnits.LengthUnitGeometry;

    public CreateMember2d_OBSOLETE() : base("Create 2D Member",
              "Mem2D",
      "Create GSA Member 2D",
      CategoryName.Name(),
      SubCategoryName.Cat2()) { }

    public override bool Read(GH_IReader reader) {
      if (reader.ItemExists("dropdown") || reader.ChunkExists("ParameterData"))
        base.Read(reader);
      else {
        BaseReader.Read(reader, this);
        _isInitialised = true;
        UpdateUIFromSelectedItems();
      }

      GH_IReader attributes = reader.FindChunk("Attributes");
      Attributes.Bounds = (RectangleF)attributes.Items[0]
        .InternalData;
      Attributes.Pivot = (PointF)attributes.Items[1]
        .InternalData;
      return true;
    }

    public override void SetSelected(int i, int j) {
      _selectedItems[i] = _dropDownItems[i][j];
      _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[i]);
      base.UpdateUI();
    }

    public override void VariableParameterMaintenance()
      => Params.Input[4]
        .Name = "Mesh Size [" + Length.GetAbbreviation(_lengthUnit) + "]";

    protected override void InitialiseDropdowns() {
      _spacerDescriptions = new List<string>(new[] {
        "Unit",
      });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      _dropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
      _selectedItems.Add(Length.GetAbbreviation(_lengthUnit));

      _isInitialised = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      string unitAbbreviation = Length.GetAbbreviation(_lengthUnit);

      pManager.AddBrepParameter("Brep",
        "B",
        "Planar Brep (non-planar geometry will be automatically converted to an average plane of exterior boundary control points))",
        GH_ParamAccess.item);
      pManager.AddPointParameter("Incl. Points",
        "(P)",
        "Inclusion points (will automatically be projected onto Brep)",
        GH_ParamAccess.list);
      pManager.AddCurveParameter("Incl. Curves",
        "(C)",
        "Inclusion curves (will automatically be made planar and projected onto brep, and converted to Arcs and Lines)",
        GH_ParamAccess.list);
      pManager.AddParameter(new GsaProp2dParameter());
      pManager.AddGenericParameter("Mesh Size [" + unitAbbreviation + "]",
        "Ms",
        "Target mesh size",
        GH_ParamAccess.item);

      pManager.HideParameter(0);
      pManager.HideParameter(1);
      pManager.HideParameter(2);

      pManager[1]
        .Optional = true;
      pManager[2]
        .Optional = true;
      pManager[3]
        .Optional = true;
      pManager[4]
        .Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
      => pManager.AddParameter(new GsaMember2dParameter());

    protected override void SolveInstance(IGH_DataAccess da) {
      var ghbrep = new GH_Brep();
      if (!da.GetData(0, ref ghbrep))
        return;

      if (ghbrep == null)
        this.AddRuntimeWarning("Brep input is null");
      var brep = new Brep();
      if (!GH_Convert.ToBrep(ghbrep, ref brep, GH_Conversion.Both))
        return;

      var point3ds = new List<Point3d>();
      var ghpts = new List<GH_Point>();
      if (da.GetDataList(1, ghpts))
        foreach (GH_Point point in ghpts) {
          var pt = new Point3d();
          if (GH_Convert.ToPoint3d(point, ref pt, GH_Conversion.Both))
            point3ds.Add(pt);
        }

      var curves = new List<Curve>();
      var ghCurves = new List<GH_Curve>();
      if (da.GetDataList(2, ghCurves))
        foreach (GH_Curve curve in ghCurves) {
          Curve crv = null;
          if (GH_Convert.ToCurve(curve, ref crv, GH_Conversion.Both))
            curves.Add(crv);
        }

      var mem = new GsaMember2d(brep, curves, point3ds);

      var ghTyp = new GH_ObjectWrapper();
      var prop2d = new GsaProp2d();
      if (da.GetData(3, ref ghTyp)) {
        if (ghTyp.Value is GsaProp2dGoo) {
          ghTyp.CastTo(ref prop2d);
          mem.Property = prop2d;
        }
        else {
          if (GH_Convert.ToInt32(ghTyp.Value, out int idd, GH_Conversion.Both))
            mem.Property = new GsaProp2d(idd);
          else {
            this.AddRuntimeError(
              "Unable to convert PA input to a 2D Property of reference integer");
            return;
          }
        }
      }

      if (Params.Input[4]
          .SourceCount
        > 0)
        mem.MeshSize = ((Length)Input.UnitNumber(this, da, 4, _lengthUnit, true)).Meters;

      da.SetData(0, new GsaMember2dGoo(mem));
    }

    protected override void UpdateUIFromSelectedItems() {
      _lengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), _selectedItems[0]);
      base.UpdateUIFromSelectedItems();
    }
  }
}
