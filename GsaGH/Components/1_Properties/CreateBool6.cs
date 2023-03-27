using System;
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
    protected override void SolveInstance(IGH_DataAccess da) {
      var uiSet = new GsaBool6(_x,
        _y,
        _z,
        _xx,
        _yy,
        _zz);
      GsaBool6 bool6 = uiSet.Duplicate();

      bool input = false;
      if (da.GetData(0, ref input))
        bool6.X = input;
      if (da.GetData(1, ref input))
        bool6.Y = input;
      if (da.GetData(2, ref input))
        bool6.Z = input;
      if (da.GetData(3, ref input))
        bool6.Xx = input;
      if (da.GetData(4, ref input))
        bool6.Yy = input;
      if (da.GetData(5, ref input))
        bool6.Zz = input;

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

      if (update)
        ReDrawComponent();

      da.SetData(0, new GsaBool6Goo(bool6));
    }

    #region Name and Ribbon Layout

    public override Guid ComponentGuid => new Guid("1d5f7b92-57a2-4c53-a8c7-419f066a7430");
    public override GH_Exposure Exposure => GH_Exposure.secondary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.CreateBool6;

    public CreateBool6() : base("Create " + GsaBool6Goo.Name.Replace(" ", string.Empty),
      GsaBool6Goo.NickName.Replace(" ", string.Empty),
      "Create a " + GsaBool6Goo.Description,
      CategoryName.Name(),
      SubCategoryName.Cat1())
      => Hidden = true;

    #endregion

    #region Input and output

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddBooleanParameter("X", "X", "X", GH_ParamAccess.item);
      pManager.AddBooleanParameter("Y", "Y", "Y", GH_ParamAccess.item);
      pManager.AddBooleanParameter("Z", "Z", "Z", GH_ParamAccess.item);
      pManager.AddBooleanParameter("XX", "XX", "XX", GH_ParamAccess.item);
      pManager.AddBooleanParameter("YY", "YY", "YY", GH_ParamAccess.item);
      pManager.AddBooleanParameter("ZZ", "ZZ", "ZZ", GH_ParamAccess.item);

      pManager[0]
        .Optional = true;
      pManager[1]
        .Optional = true;
      pManager[2]
        .Optional = true;
      pManager[3]
        .Optional = true;
      pManager[4]
        .Optional = true;
      pManager[5]
        .Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
      => pManager.AddParameter(new GsaBool6Parameter());

    #endregion

    #region Custom UI

    private bool _x;
    private bool _y;
    private bool _z;
    private bool _xx;
    private bool _yy;
    private bool _zz;
    public override void SetSelected(int i, int j) { }
    public override void InitialiseDropdowns() { }

    public override void CreateAttributes()
      => m_attributes = new Bool6ComponentAttributes(this,
        SetBool,
        "Set 6 DOF",
        _x,
        _y,
        _z,
        _xx,
        _yy,
        _zz);

    public void SetBool(
      bool resx,
      bool resy,
      bool resz,
      bool resxx,
      bool resyy,
      bool reszz) {
      _x = resx;
      _y = resy;
      _z = resz;
      _xx = resxx;
      _yy = resyy;
      _zz = reszz;
    }

    private void ReDrawComponent() {
      var pivot = new PointF(Attributes.Pivot.X, Attributes.Pivot.Y);
      CreateAttributes();
      Attributes.Pivot = pivot;
      Attributes.ExpireLayout();
      Attributes.PerformLayout();
    }

    #endregion

    #region (de)serialization

    public override bool Write(GH_IWriter writer) {
      writer.SetBoolean("x", _x);
      writer.SetBoolean("y", _y);
      writer.SetBoolean("z", _z);
      writer.SetBoolean("xx", _xx);
      writer.SetBoolean("yy", _yy);
      writer.SetBoolean("zz", _zz);

      return base.Write(writer);
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

    #endregion
  }
}
