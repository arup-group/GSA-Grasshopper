using System;
using System.Collections.Generic;
using System.Drawing;
using GH_IO.Serialization;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;
using OasysGH;
using OasysGH.Components;
using OasysGH.UI;
using OasysGH.Units;
using Rhino.Geometry;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to create new 1D Member
  /// </summary>
  public class CreateMember1d : GH_OasysDropDownComponent {
    public override Guid ComponentGuid => new Guid("8278b67c-425a-4220-b759-79ecdd6aba55");
    public override GH_Exposure Exposure => GH_Exposure.primary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.CreateMem1d;
    private bool _x1;
    private bool _x2;
    private bool _xx1;
    private bool _xx2;
    private bool _y1;
    private bool _y2;
    private bool _yy1;
    private bool _yy2;
    private bool _z1;
    private bool _z2;
    private bool _zz1;
    private bool _zz2;

    public CreateMember1d() : base("Create 1D Member", "Mem1D", "Create GSA 1D Member",
      CategoryName.Name(), SubCategoryName.Cat2()) { }

    public override void CreateAttributes() {
      var restraints = new List<List<bool>>() {
        new List<bool>() {
          _x1,
          _y1,
          _z1,
          _xx1,
          _yy1,
          _zz1,
        },
        new List<bool>() {
          _x2,
          _y2,
          _z2,
          _xx2,
          _yy2,
          _zz2,
        },
      };
      m_attributes = new CheckBoxComponentComponentAttributes(this, SetReleases,
        new List<string>() {
          "Start Release",
          "End Release",
        }, restraints, new List<List<string>>() {
          new List<string>() {
            "x",
            "y",
            "z",
            "xx",
            "yy",
            "zz",
          },
          new List<string>() {
            "x",
            "y",
            "z",
            "xx",
            "yy",
            "zz",
          },
        });
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
      return base.Read(reader);
    }

    public void SetReleases(List<List<bool>> restraints) {
      _x1 = restraints[0][0];
      _y1 = restraints[0][1];
      _z1 = restraints[0][2];
      _xx1 = restraints[0][3];
      _yy1 = restraints[0][4];
      _zz1 = restraints[0][5];
      _x2 = restraints[1][0];
      _y2 = restraints[1][1];
      _z2 = restraints[1][2];
      _xx2 = restraints[1][3];
      _yy2 = restraints[1][4];
      _zz2 = restraints[1][5];

      base.UpdateUI();
    }

    public override void SetSelected(int i, int j) { }

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

    protected override void InitialiseDropdowns() { }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddCurveParameter("Curve", "C",
        "Curve (a NURBS curve will automatically be converted in to a Polyline of Arc and Line segments)",
        GH_ParamAccess.item);
      pManager.AddParameter(new GsaSectionParameter());
      pManager.AddNumberParameter("Mesh Size in model units", "Ms", "Target mesh size",
        GH_ParamAccess.item);
      pManager[1].Optional = true;
      pManager[2].Optional = true;
      pManager.HideParameter(0);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaMember1dParameter());
    }

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
      if (mem.PolyCurve.GetLength() < DefaultUnits.Tolerance.As(DefaultUnits.LengthUnitGeometry)) {
        this.AddRuntimeRemark(
          "Service message from you favourite Oasys dev team: Based on your Default Unit Settings (changed in the Oasys Menu), one or more input curves have relatively short length less than the set tolerance ("
          + DefaultUnits.Tolerance.ToString().Replace(" ", string.Empty)
          + ". This may convert into a zero-length line when assembling the GSA Model, thus creating invalid topology that cannot be analysed. You can ignore this message if you are creating your model in another unit (set on 'Analyse' or 'CreateModel' components) than "
          + DefaultUnits.LengthUnitGeometry.ToString() + ".");
      }

      var rel1 = new GsaBool6 {
        X = _x1,
        Y = _y1,
        Z = _z1,
        Xx = _xx1,
        Yy = _yy1,
        Zz = _zz1,
      };

      mem.ReleaseStart = rel1;

      var rel2 = new GsaBool6 {
        X = _x2,
        Y = _y2,
        Z = _z2,
        Xx = _xx2,
        Yy = _yy2,
        Zz = _zz2,
      };
      mem.ReleaseEnd = rel2;

      var ghTyp = new GH_ObjectWrapper();
      if (da.GetData(1, ref ghTyp)) {
        if (ghTyp.Value is GsaSectionGoo sectionGoo) {
          mem.Section = sectionGoo.Value;
        } else {
          if (GH_Convert.ToInt32(ghTyp.Value, out int id, GH_Conversion.Both)) {
            mem.Section = new GsaSection(id);
          } else {
            this.AddRuntimeError(
              "Unable to convert PB input to a Section Property of reference integer");
          }
        }
      }

      double meshSize = 0;
      if (da.GetData(2, ref meshSize)) {
        mem.MeshSize = meshSize;
      }

      da.SetData(0, new GsaMember1dGoo(mem));
    }
  }
}
