using System;
using System.Collections.Generic;
using System.Drawing;

using GH_IO.Serialization;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using GsaAPI;

using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;

using OasysGH;
using OasysGH.UI;
using OasysGH.Units;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to create new 1D Member
  /// </summary>
  public class Create1dMember : Section3dPreviewDropDownComponent {
    public override Guid ComponentGuid => new Guid("8278b67c-425a-4220-b759-79ecdd6aba55");
    public override GH_Exposure Exposure => GH_Exposure.tertiary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.Create1dMember;
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

    public Create1dMember() : base("Create 1D Member", "Mem1D", "Create GSA 1D Member",
      CategoryName.Name(), SubCategoryName.Cat2()) { }

    public override void CreateAttributes() {
      if (!_isInitialised) {
        InitialiseDropdowns();
      }

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
      m_attributes = new CheckBoxComponentAttributes(this, SetReleases,
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
      bool flag = base.Read(reader);
      if (Params.Input[1].Name == new GsaSectionParameter().Name) {
        Params.ReplaceInputParameter(new GsaPropertyParameter(), 1, true);
      }

      return flag;
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

    protected override void InitialiseDropdowns() {
      _isInitialised = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddCurveParameter("Curve", "C",
        "Curve (a NURBS curve will automatically be converted in to a Polyline of Arc and Line segments)",
        GH_ParamAccess.item);
      pManager.AddParameter(new GsaPropertyParameter());
      pManager.AddNumberParameter("Mesh Size in model units", "Ms", "Target mesh size",
        GH_ParamAccess.item);
      pManager[1].Optional = true;
      pManager[2].Optional = true;
      pManager.HideParameter(0);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaMember1dParameter());
    }

    protected override void SolveInternal(IGH_DataAccess da) {
      GH_Curve ghcrv = null;
      da.GetData(0, ref ghcrv);
      var mem = new GsaMember1D(ghcrv.Value);

      if (mem.PolyCurve.GetLength() < DefaultUnits.Tolerance.As(DefaultUnits.LengthUnitGeometry)) {
        this.AddRuntimeRemark(
          "Service message from your favourite Oasys dev team:" + Environment.NewLine
          + "Based on your Default Unit Settings (changed in the Oasys Menu)," + Environment.NewLine
          + "one or more input curves have relatively short length less than" + Environment.NewLine
          + "the set tolerance (" + DefaultUnits.Tolerance.ToString().Replace(" ", string.Empty) + ")."
          + Environment.NewLine
          + "This may convert into a zero-length line when assembling the GSA Model," + Environment.NewLine
          + "thus creating invalid topology that cannot be analysed." + Environment.NewLine
          + "You can ignore this message if you are creating your model in another unit" + Environment.NewLine
          + "(set on 'Analyse' or 'CreateModel' components) than "
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

      GsaPropertyGoo sectionGoo = null;
      if (da.GetData(1, ref sectionGoo)) {
        switch (sectionGoo.Value) {
          case GsaSection section:
            mem.Section = section;
            mem.SpringProperty = null;
            if (Preview3dSection) {
              mem.CreateSection3dPreview();
            }
            break;

          case GsaSpringProperty springProperty:
            mem.ApiMember.Type1D = ElementType.SPRING;
            mem.Section = null;
            mem.SpringProperty = springProperty;
            break;
        }
      }

      double meshSize = 0;
      if (da.GetData(2, ref meshSize)) {
        mem.ApiMember.MeshSize = meshSize;
      }

      da.SetData(0, new GsaMember1dGoo(mem));
    }
  }
}
