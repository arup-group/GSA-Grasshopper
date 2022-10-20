using System;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
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
    public override Guid ComponentGuid => new Guid("f5909576-6796-4d6e-90d8-31a9b7ee6fb6");
    public override GH_Exposure Exposure => GH_Exposure.secondary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.CreateBool6;

    public CreateBool6() : base("Create " + GsaBool6Goo.Name.Replace(" ", string.Empty),
      GsaBool6Goo.NickName.Replace(" ", string.Empty),
      "Create a " + GsaBool6Goo.Description,
      Ribbon.CategoryName.Name(),
      Ribbon.SubCategoryName.Cat1())
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
      GH_Boolean ghBolX = new GH_Boolean();
      if (DA.GetData(0, ref ghBolX))
        GH_Convert.ToBoolean(ghBolX, out _x, GH_Conversion.Both); //use Grasshopper to convert, these methods covers many cases and are consistent
      GH_Boolean ghBolY = new GH_Boolean();
      if (DA.GetData(1, ref ghBolY))
        GH_Convert.ToBoolean(ghBolY, out _y, GH_Conversion.Both);
      GH_Boolean ghBolZ = new GH_Boolean();
      if (DA.GetData(2, ref ghBolZ))
        GH_Convert.ToBoolean(ghBolZ, out _z, GH_Conversion.Both);
      GH_Boolean ghBolXX = new GH_Boolean();
      if (DA.GetData(3, ref ghBolXX))
        GH_Convert.ToBoolean(ghBolXX, out _xx, GH_Conversion.Both);
      GH_Boolean ghBolYY = new GH_Boolean();
      if (DA.GetData(4, ref ghBolYY))
        GH_Convert.ToBoolean(ghBolYY, out _yy, GH_Conversion.Both);
      GH_Boolean ghBolZZ = new GH_Boolean();
      if (DA.GetData(5, ref ghBolZZ))
        GH_Convert.ToBoolean(ghBolZZ, out _zz, GH_Conversion.Both);
      GsaBool6 bool6 = new GsaBool6
      {
        X = _x,
        Y = _y,
        Z = _z,
        XX = _xx,
        YY = _yy,
        ZZ = _zz
      };
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

