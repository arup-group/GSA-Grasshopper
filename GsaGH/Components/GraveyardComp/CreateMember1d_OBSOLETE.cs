using System;
using GH_IO.Serialization;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Components.GraveyardComp;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using OasysGH;
using OasysGH.Components;
using OasysGH.Units;
using Rhino.Geometry;

namespace GsaGH.Components {
  /// <summary>
  /// Component to create new 1D Member
  /// </summary>
  // ReSharper disable once InconsistentNaming
  public class CreateMember1d_OBSOLETE : GH_OasysDropDownComponent {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("5c5b9efa-cdae-4be5-af40-ff2b590801dd");
    public override GH_Exposure Exposure => GH_Exposure.hidden;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Properties.Resources.CreateMem1d;

    public CreateMember1d_OBSOLETE() : base("Create 1D Member",
      "Mem1D",
      "Create GSA 1D Member",
      CategoryName.Name(),
      SubCategoryName.Cat2()) { }
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddCurveParameter("Curve", "C", "Curve (a NURBS curve will automatically be converted in to a Polyline of Arc and Line segments)", GH_ParamAccess.item);
      pManager.AddParameter(new GsaSectionParameter());
      pManager[1].Optional = true;
      pManager.HideParameter(0);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaMember1dParameter());
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess da) {
      var ghcrv = new GH_Curve();
      if (!da.GetData(0, ref ghcrv)) {
        return;
      }

      if (ghcrv == null) {
        this.AddRuntimeWarning("Curve input is null");
      }
      Curve crv = null;
      if (!GH_Convert.ToCurve(ghcrv, ref crv, GH_Conversion.Both)) {
        return;
      }

      var mem = new GsaMember1d(crv);
      if (mem.PolyCurve.GetLength() < DefaultUnits.Tolerance.As(DefaultUnits.LengthUnitGeometry))
        this.AddRuntimeRemark("Service message from you favourite Oasys dev team: Based on your Default Unit Settings (changed in the Oasys Menu), one or more input curves have relatively short length less than the set tolerance (" + DefaultUnits.Tolerance.ToString().Replace(" ", string.Empty) + ". This may convert into a zero-length line when assembling the GSA Model, thus creating invalid topology that cannot be analysed. You can ignore this message if you are creating your model in another unit (set on 'Analyse' or 'CreateModel' components) than " + DefaultUnits.LengthUnitGeometry.ToString() + ".");

      var rel1 = new GsaBool6 {
        X = _x1,
        Y = _y1,
        Z = _z1,
        XX = _xx1,
        YY = _yy1,
        ZZ = _zz1,
      };

      mem.ReleaseStart = rel1;

      var rel2 = new GsaBool6 {
        X = _x2,
        Y = _y2,
        Z = _z2,
        XX = _xx2,
        YY = _yy2,
        ZZ = _zz2,
      };
      mem.ReleaseEnd = rel2;

      // 1 section
      var ghTyp = new GH_ObjectWrapper();
      var section = new GsaSection();
      if (da.GetData(1, ref ghTyp)) {
        if (ghTyp.Value is GsaSectionGoo) {
          ghTyp.CastTo(ref section);
          mem.Section = section;
        }
        else {
          if (GH_Convert.ToInt32(ghTyp.Value, out int idd, GH_Conversion.Both))
            mem.Section = new GsaSection(idd);
          else {
            this.AddRuntimeError("Unable to convert PB input to a Section Property of reference integer");
            return;
          }
        }
      }
      da.SetData(0, new GsaMember1dGoo(mem));
    }

    #region Custom UI
    private bool _x1;
    private bool _y1;
    private bool _z1;
    private bool _xx1;
    private bool _yy1;
    private bool _zz1;
    private bool _x2;
    private bool _y2;
    private bool _z2;
    private bool _xx2;
    private bool _yy2;
    private bool _zz2;
    public override void SetSelected(int i, int j) { }
    public override void InitialiseDropdowns() { }
    public override void CreateAttributes() {
      m_attributes = new OasysGH.UI.ReleasesComponentAttributes(this, SetReleases, "Start Release", "End Release", _x1, _y1, _z1, _xx1, _yy1, _zz1, _x2, _y2, _z2, _xx2, _yy2, _zz2);
    }

    public void SetReleases(bool resx1, bool resy1, bool resz1, bool resxx1, bool resyy1, bool reszz1,
        bool resx2, bool resy2, bool resz2, bool resxx2, bool resyy2, bool reszz2) {
      _x1 = resx1;
      _y1 = resy1;
      _z1 = resz1;
      _xx1 = resxx1;
      _yy1 = resyy1;
      _zz1 = reszz1;
      _x2 = resx2;
      _y2 = resy2;
      _z2 = resz2;
      _xx2 = resxx2;
      _yy2 = resyy2;
      _zz2 = reszz2;

      base.UpdateUI();
    }
    #endregion

    #region (de)serialization
    public override bool Write(GH_IWriter writer) {
      writer.SetBoolean("x1", _x1);
      writer.SetBoolean("y1", _y1);
      writer.SetBoolean("z1", _z1);
      writer.SetBoolean("xx1", _xx1);
      writer.SetBoolean("yy1", _yy1);
      writer.SetBoolean("zz1", _zz1);
      writer.SetBoolean("x2", _x2);
      writer.SetBoolean("y2", _y2);
      writer.SetBoolean("z2", _z2);
      writer.SetBoolean("xx2", _xx2);
      writer.SetBoolean("yy2", _yy2);
      writer.SetBoolean("zz2", _zz2);
      return base.Write(writer);
    }
    public override bool Read(GH_IReader reader) {
      _x1 = reader.GetBoolean("x1");
      _y1 = reader.GetBoolean("y1");
      _z1 = reader.GetBoolean("z1");
      _xx1 = reader.GetBoolean("xx1");
      _yy1 = reader.GetBoolean("yy1");
      _zz1 = reader.GetBoolean("zz1");
      _x2 = reader.GetBoolean("x2");
      _y2 = reader.GetBoolean("y2");
      _z2 = reader.GetBoolean("z2");
      _xx2 = reader.GetBoolean("xx2");
      _yy2 = reader.GetBoolean("yy2");
      _zz2 = reader.GetBoolean("zz2");

      if (reader.ChunkExists("ParameterData"))
        base.Read(reader);
      else {
        BaseReader.Read(reader, this);
        IsInitialised = true;
        UpdateUIFromSelectedItems();
      }
      GH_IReader attributes = reader.FindChunk("Attributes");
      Attributes.Bounds = (System.Drawing.RectangleF)attributes.Items[0].InternalData;
      Attributes.Pivot = (System.Drawing.PointF)attributes.Items[1].InternalData;
      return true;
    }
    #endregion
  }
}
