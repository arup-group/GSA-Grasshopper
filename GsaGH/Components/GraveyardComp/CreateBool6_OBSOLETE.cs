using System;
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

namespace GsaGH.Components {

  /// <summary>
  ///   Component to create a new Bool6
  /// </summary>
  // ReSharper disable once InconsistentNaming
  public class CreateBool6_OBSOLETE : GH_OasysComponent {

    #region Properties + Fields
    public override Guid ComponentGuid => new Guid("f5909576-6796-4d6e-90d8-31a9b7ee6fb6");
    public override GH_Exposure Exposure => GH_Exposure.hidden;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.CreateBool6;
    private bool _x;
    private bool _xx;
    private bool _y;
    private bool _yy;
    private bool _z;
    private bool _zz;
    #endregion Properties + Fields

    #region Public Constructors
    public CreateBool6_OBSOLETE() : base("Create " + GsaBool6Goo.Name.Replace(" ", string.Empty),
      GsaBool6Goo.NickName.Replace(" ", string.Empty),
      "Create a " + GsaBool6Goo.Description,
      CategoryName.Name(),
      SubCategoryName.Cat1())
      => Hidden = true;

    #endregion Public Constructors

    #region Public Methods
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

    public override bool Read(GH_IReader reader) {
      _x = reader.GetBoolean("x");
      _y = reader.GetBoolean("y");
      _z = reader.GetBoolean("z");
      _xx = reader.GetBoolean("xx");
      _yy = reader.GetBoolean("yy");
      _zz = reader.GetBoolean("zz");
      CreateAttributes();
      ExpireSolution(true);
      Params.OnParametersChanged();
      OnDisplayExpired(true);
      return base.Read(reader);
    }

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

    public override bool Write(GH_IWriter writer) {
      writer.SetBoolean("x", _x);
      writer.SetBoolean("y", _y);
      writer.SetBoolean("z", _z);
      writer.SetBoolean("xx", _xx);
      writer.SetBoolean("yy", _yy);
      writer.SetBoolean("zz", _zz);
      return base.Write(writer);
    }

    #endregion Public Methods

    #region Protected Methods
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
      => pManager.AddGenericParameter("Bool6",
        "B6",
        "GSA Bool6 to set releases or restraints",
        GH_ParamAccess.item);

    protected override void SolveInstance(IGH_DataAccess da) {
      var ghBolX = new GH_Boolean();
      if (da.GetData(0, ref ghBolX))
        GH_Convert.ToBoolean(ghBolX, out _x, GH_Conversion.Both);
      var ghBolY = new GH_Boolean();
      if (da.GetData(1, ref ghBolY))
        GH_Convert.ToBoolean(ghBolY, out _y, GH_Conversion.Both);
      var ghBolZ = new GH_Boolean();
      if (da.GetData(2, ref ghBolZ))
        GH_Convert.ToBoolean(ghBolZ, out _z, GH_Conversion.Both);
      var ghBolXx = new GH_Boolean();
      if (da.GetData(3, ref ghBolXx))
        GH_Convert.ToBoolean(ghBolXx, out _xx, GH_Conversion.Both);
      var ghBolYy = new GH_Boolean();
      if (da.GetData(4, ref ghBolYy))
        GH_Convert.ToBoolean(ghBolYy, out _yy, GH_Conversion.Both);
      var ghBolZz = new GH_Boolean();
      if (da.GetData(5, ref ghBolZz))
        GH_Convert.ToBoolean(ghBolZz, out _zz, GH_Conversion.Both);
      var bool6 = new GsaBool6 {
        X = _x,
        Y = _y,
        Z = _z,
        Xx = _xx,
        Yy = _yy,
        Zz = _zz,
      };
      da.SetData(0, new GsaBool6Goo(bool6.Duplicate()));
    }

    #endregion Protected Methods
  }
}
