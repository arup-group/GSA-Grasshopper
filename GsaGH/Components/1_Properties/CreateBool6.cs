using System;
using System.Collections.Generic;
using System.Drawing;

using GH_IO.Serialization;

using Grasshopper.Kernel;

using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;

using OasysGH;
using OasysGH.Components;
using OasysGH.UI;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to create a new Bool6
  /// </summary>
  public class CreateBool6 : GH_OasysDropDownComponent {
    public override Guid ComponentGuid => new Guid("1d5f7b92-57a2-4c53-a8c7-419f066a7430");
    public override GH_Exposure Exposure => GH_Exposure.septenary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.CreateBool6;
    private bool _x;
    private bool _xx;
    private bool _y;
    private bool _yy;
    private bool _z;
    private bool _zz;

    public CreateBool6() : base("Create " + GsaBool6Goo.Name,
      GsaBool6Goo.NickName.Replace(" ", string.Empty), "Create a " + GsaBool6Goo.Description,
      CategoryName.Name(), SubCategoryName.Cat1()) {
      Hidden = true;
    }

    public override void CreateAttributes() {
      if (!_isInitialised) {
        InitialiseDropdowns();
      }
      var bool6 = new List<List<bool>>() {
        new List<bool>() {
          _x,
          _y,
          _z,
          _xx,
          _yy,
          _zz,
        },
      };
      m_attributes = new CheckBoxComponentAttributes(this, SetReleases,
        new List<string>() {
          "Set 6 DOF",
        }, bool6, new List<List<string>>() {
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
      _x = reader.GetBoolean("x");
      _y = reader.GetBoolean("y");
      _z = reader.GetBoolean("z");
      _xx = reader.GetBoolean("xx");
      _yy = reader.GetBoolean("yy");
      _zz = reader.GetBoolean("zz");

      return base.Read(reader);
    }

    public void SetReleases(List<List<bool>> bool6) {
      _x = bool6[0][0];
      _y = bool6[0][1];
      _z = bool6[0][2];
      _xx = bool6[0][3];
      _yy = bool6[0][4];
      _zz = bool6[0][5];
      base.UpdateUI();
    }

    public override void SetSelected(int i, int j) { }

    public override bool Write(GH_IWriter writer) {
      writer.SetBoolean("x", _x);
      writer.SetBoolean("y", _y);
      writer.SetBoolean("z", _z);
      writer.SetBoolean("xx", _xx);
      writer.SetBoolean("yy", _yy);
      writer.SetBoolean("zz", _zz);

      return base.Write(writer);
    }

    protected override void InitialiseDropdowns() {
      _isInitialised = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddBooleanParameter("X", "X", "X", GH_ParamAccess.item);
      pManager.AddBooleanParameter("Y", "Y", "Y", GH_ParamAccess.item);
      pManager.AddBooleanParameter("Z", "Z", "Z", GH_ParamAccess.item);
      pManager.AddBooleanParameter("XX", "XX", "XX", GH_ParamAccess.item);
      pManager.AddBooleanParameter("YY", "YY", "YY", GH_ParamAccess.item);
      pManager.AddBooleanParameter("ZZ", "ZZ", "ZZ", GH_ParamAccess.item);

      pManager[0].Optional = true;
      pManager[1].Optional = true;
      pManager[2].Optional = true;
      pManager[3].Optional = true;
      pManager[4].Optional = true;
      pManager[5].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaBool6Parameter());
    }

    protected override void SolveInternal(IGH_DataAccess da) {
      var uiSet = new GsaBool6(_x, _y, _z, _xx, _yy, _zz);
      var bool6 = new GsaBool6(uiSet);

      bool input = false;
      if (da.GetData(0, ref input)) {
        bool6.X = input;
      }

      if (da.GetData(1, ref input)) {
        bool6.Y = input;
      }

      if (da.GetData(2, ref input)) {
        bool6.Z = input;
      }

      if (da.GetData(3, ref input)) {
        bool6.Xx = input;
      }

      if (da.GetData(4, ref input)) {
        bool6.Yy = input;
      }

      if (da.GetData(5, ref input)) {
        bool6.Zz = input;
      }

      bool update = false;
      if (bool6.X != uiSet.X) {
        _x = bool6.X;
        update = true;
      }

      if (bool6.Y != uiSet.Y) {
        _y = bool6.Y;
        update = true;
      }

      if (bool6.Z != uiSet.Z) {
        _z = bool6.Z;
        update = true;
      }

      if (bool6.Xx != uiSet.Xx) {
        _xx = bool6.Xx;
        update = true;
      }

      if (bool6.Yy != uiSet.Yy) {
        _yy = bool6.Yy;
        update = true;
      }

      if (bool6.Zz != uiSet.Zz) {
        _zz = bool6.Zz;
        update = true;
      }

      if (update) {
        ReDrawComponent();
      }

      da.SetData(0, new GsaBool6Goo(bool6));
    }

    private void ReDrawComponent() {
      var pivot = new PointF(Attributes.Pivot.X, Attributes.Pivot.Y);
      CreateAttributes();
      Attributes.Pivot = pivot;
      Attributes.ExpireLayout();
      Attributes.PerformLayout();
    }
  }
}
