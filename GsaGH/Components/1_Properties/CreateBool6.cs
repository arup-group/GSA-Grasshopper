using System;
using Grasshopper.Kernel;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using OasysGH;
using OasysGH.Components;
using OasysGH.UI;

namespace GsaGH.Components
{
  /// <summary>
  /// Component to create a new Bool6
  /// </summary>
  public class CreateBool6 : GH_OasysDropDownComponent
  {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("1d5f7b92-57a2-4c53-a8c7-419f066a7430");
    public override GH_Exposure Exposure => GH_Exposure.secondary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.CreateBool6;

    public CreateBool6() : base("Create " + GsaBool6Goo.Name.Replace(" ", string.Empty),
      GsaBool6Goo.NickName.Replace(" ", string.Empty),
      "Create a " + GsaBool6Goo.Description,
      CategoryName.Name(),
      SubCategoryName.Cat1())
    { this.Hidden = true; } // sets the initial state of the component to hidden
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
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

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddParameter(new GsaBool6Parameter());
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      GsaBool6 uiSet = new GsaBool6(_x, _y, _z, _xx, _yy, _zz);
      GsaBool6 bool6 = uiSet.Duplicate();
      
      bool input = false;
      if (DA.GetData(0, ref input))
        bool6.X = input;
      if (DA.GetData(1, ref input))
        bool6.Y = input;
      if (DA.GetData(2, ref input))
        bool6.Z = input;
      if (DA.GetData(3, ref input))
        bool6.XX = input;
      if (DA.GetData(4, ref input))
        bool6.YY = input;
      if (DA.GetData(5, ref input))
        bool6.ZZ = input;

      bool update = false;
      if (bool6.X != uiSet.X)
      {
        _x = bool6.X;
        update = true;
      }
      if (bool6.Y != uiSet.Y)
      {
        _y = bool6.Y;
        update = true;
      }
      if (bool6.Z != uiSet.Z)
      {
        _z = bool6.Z;
        update = true;
      }
      if (bool6.XX != uiSet.XX)
      {
        _xx = bool6.XX;
        update = true;
      }
      if (bool6.YY != uiSet.YY)
      {
        _yy = bool6.YY;
        update = true;
      }
      if (bool6.ZZ != uiSet.ZZ)
      {
        _zz = bool6.ZZ;
        update = true;
      }

      if (update)
        this.ReDrawComponent();

      DA.SetData(0, new GsaBool6Goo(bool6));
    }

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
    {
      m_attributes = new Bool6ComponentAttributes(this, SetBool, "Set 6 DOF", _x, _y, _z, _xx, _yy, _zz);
    }

    public void SetBool(bool resx, bool resy, bool resz, bool resxx, bool resyy, bool reszz)
    {
      _x = resx;
      _y = resy;
      _z = resz;
      _xx = resxx;
      _yy = resyy;
      _zz = reszz;
    }

    private void ReDrawComponent()
    {
      System.Drawing.PointF pivot = new System.Drawing.PointF(this.Attributes.Pivot.X, this.Attributes.Pivot.Y);
      this.CreateAttributes();
      this.Attributes.Pivot = pivot;
      this.Attributes.ExpireLayout();
      this.Attributes.PerformLayout();
    }
    #endregion

    #region (de)serialization
    public override bool Write(GH_IO.Serialization.GH_IWriter writer)
    {
      writer.SetBoolean("x", (bool)_x);
      writer.SetBoolean("y", (bool)_y);
      writer.SetBoolean("z", (bool)_z);
      writer.SetBoolean("xx", (bool)_xx);
      writer.SetBoolean("yy", (bool)_yy);
      writer.SetBoolean("zz", (bool)_zz);
      return base.Write(writer);
    }
    public override bool Read(GH_IO.Serialization.GH_IReader reader)
    {
      _x = (bool)reader.GetBoolean("x");
      _y = (bool)reader.GetBoolean("y");
      _z = (bool)reader.GetBoolean("z");
      _xx = (bool)reader.GetBoolean("xx");
      _yy = (bool)reader.GetBoolean("yy");
      _zz = (bool)reader.GetBoolean("zz");
      return base.Read(reader);
    }
    #endregion
  }
}

