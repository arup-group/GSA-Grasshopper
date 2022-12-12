using System;
using GH_IO.Serialization;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using OasysGH;
using OasysGH.Components;
using OasysGH.Helpers;
using OasysGH.Units;
using Rhino.Geometry;

namespace GsaGH.Components
{
  /// <summary>
  /// Component to create new 1D Member
  /// </summary>
  public class CreateMember1d_OBSOLETE : GH_OasysDropDownComponent, IGH_PreviewObject
  {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("5c5b9efa-cdae-4be5-af40-ff2b590801dd");
    public override GH_Exposure Exposure => GH_Exposure.hidden;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Properties.Resources.CreateMem1d;

    public CreateMember1d_OBSOLETE() : base("Create 1D Member",
      "Mem1D",
      "Create GSA 1D Member",
      CategoryName.Name(),
      SubCategoryName.Cat2())
    { }
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddCurveParameter("Curve", "C", "Curve (a NURBS curve will automatically be converted in to a Polyline of Arc and Line segments)", GH_ParamAccess.item);
      pManager.AddParameter(new GsaSectionParameter());
      pManager[1].Optional = true;
      pManager.HideParameter(0);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddParameter(new GsaMember1dParameter());
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      GH_Curve ghcrv = new GH_Curve();
      if (DA.GetData(0, ref ghcrv))
      {
        if (ghcrv == null) { AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Curve input is null"); }
        Curve crv = null;
        if (GH_Convert.ToCurve(ghcrv, ref crv, GH_Conversion.Both))
        {
          GsaMember1d mem = new GsaMember1d(crv);
          if (mem.PolyCurve.GetLength() < DefaultUnits.Tolerance.As(DefaultUnits.LengthUnitGeometry))
            AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "Service message from you favourite Oasys dev team: Based on your Default Unit Settings (changed in the Oasys Menu), one or more input curves have relatively short length less than the set tolerance (" + DefaultUnits.Tolerance.ToString().Replace(" ", string.Empty) + ". This may convert into a zero-length line when assembling the GSA Model, thus creating invalid topology that cannot be analysed. You can ignore this message if you are creating your model in another unit (set on 'Analyse' or 'CreateModel' components) than " + DefaultUnits.LengthUnitGeometry.ToString() + ".");

          GsaBool6 rel1 = new GsaBool6
          {
            X = _x1,
            Y = _y1,
            Z = _z1,
            XX = _xx1,
            YY = _yy1,
            ZZ = _zz1
          };

          mem.ReleaseStart = rel1;

          GsaBool6 rel2 = new GsaBool6
          {
            X = _x2,
            Y = _y2,
            Z = _z2,
            XX = _xx2,
            YY = _yy2,
            ZZ = _zz2
          };
          mem.ReleaseEnd = rel2;

          // 1 section
          GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
          GsaSection section = new GsaSection();
          if (DA.GetData(1, ref gh_typ))
          {
            if (gh_typ.Value is GsaSectionGoo)
            {
              gh_typ.CastTo(ref section);
              mem.Section = section;
            }
            else
            {
              if (GH_Convert.ToInt32(gh_typ.Value, out int idd, GH_Conversion.Both))
                mem.Section = new GsaSection(idd);
              else
              {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert PB input to a Section Property of reference integer");
                return;
              }
            }
          }
          DA.SetData(0, new GsaMember1dGoo(mem));
        }
      }
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
    public override void CreateAttributes()
    {
      m_attributes = new OasysGH.UI.ReleasesComponentAttributes(this, SetReleases, "Start Release", "End Release", _x1, _y1, _z1, _xx1, _yy1, _zz1, _x2, _y2, _z2, _xx2, _yy2, _zz2);
    }

    public void SetReleases(bool resx1, bool resy1, bool resz1, bool resxx1, bool resyy1, bool reszz1,
        bool resx2, bool resy2, bool resz2, bool resxx2, bool resyy2, bool reszz2)
    {
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
    public override bool Write(GH_IO.Serialization.GH_IWriter writer)
    {
      // we need to save all the items that we want to reappear when a GH file is saved and re-opened
      writer.SetBoolean("x1", (bool)_x1);
      writer.SetBoolean("y1", (bool)_y1);
      writer.SetBoolean("z1", (bool)_z1);
      writer.SetBoolean("xx1", (bool)_xx1);
      writer.SetBoolean("yy1", (bool)_yy1);
      writer.SetBoolean("zz1", (bool)_zz1);
      writer.SetBoolean("x2", (bool)_x2);
      writer.SetBoolean("y2", (bool)_y2);
      writer.SetBoolean("z2", (bool)_z2);
      writer.SetBoolean("xx2", (bool)_xx2);
      writer.SetBoolean("yy2", (bool)_yy2);
      writer.SetBoolean("zz2", (bool)_zz2);
      return base.Write(writer);
    }
    public override bool Read(GH_IO.Serialization.GH_IReader reader)
    {
      // when a GH file is opened we need to read in the data that was previously set by user
      _x1 = (bool)reader.GetBoolean("x1");
      _y1 = (bool)reader.GetBoolean("y1");
      _z1 = (bool)reader.GetBoolean("z1");
      _xx1 = (bool)reader.GetBoolean("xx1");
      _yy1 = (bool)reader.GetBoolean("yy1");
      _zz1 = (bool)reader.GetBoolean("zz1");
      _x2 = (bool)reader.GetBoolean("x2");
      _y2 = (bool)reader.GetBoolean("y2");
      _z2 = (bool)reader.GetBoolean("z2");
      _xx2 = (bool)reader.GetBoolean("xx2");
      _yy2 = (bool)reader.GetBoolean("yy2");
      _zz2 = (bool)reader.GetBoolean("zz2");

      GH_IReader attributes = reader.FindChunk("Attributes");
      this.Attributes.Bounds = (System.Drawing.RectangleF)attributes.Items[0].InternalData;
      this.Attributes.Pivot = (System.Drawing.PointF)attributes.Items[1].InternalData;

      int num = this.Params.Input.Count - 1;
      bool flag = true;
      for (int i = 0; i <= num; i++)
      {
        GH_IReader gH_IReader = reader.FindChunk("param_input", i);
        if (gH_IReader == null)
        {
          reader.AddMessage("Input parameter chunk is missing. Archive is corrupt.", GH_Message_Type.error);
          continue;
        }

        GH_ParamAccess access = this.Params.Input[i].Access;
        flag &= this.Params.Input[i].Read(gH_IReader);
        if (!(this.Params.Input[i] is Param_ScriptVariable))
        {
          this.Params.Input[i].Access = access;
        }
      }

      int num2 = this.Params.Output.Count - 1;
      for (int j = 0; j <= num2; j++)
      {
        GH_IReader gH_IReader2 = reader.FindChunk("param_output", j);
        if (gH_IReader2 == null)
        {
          reader.AddMessage("Output parameter chunk is missing. Archive is corrupt.", GH_Message_Type.error);
          continue;
        }

        GH_ParamAccess access2 = this.Params.Output[j].Access;
        flag &= this.Params.Output[j].Read(gH_IReader2);
        this.Params.Output[j].Access = access2;
      }
      return true;
    }
    #endregion
  }
}
