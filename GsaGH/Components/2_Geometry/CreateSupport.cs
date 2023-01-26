using System;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using OasysGH;
using OasysGH.Components;
using Rhino.Geometry;

namespace GsaGH.Components
{
  /// <summary>
  /// Component to create new Node with restraints (support)
  /// </summary>
  public class CreateSupport : GH_OasysDropDownComponent, IGH_PreviewObject
  {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("d808e81f-6ae1-49d9-a8a5-2424a1763a69");
    public override GH_Exposure Exposure => GH_Exposure.primary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.CreateSupport;

    public CreateSupport() : base("Create Support",
      "Support",
      "Create GSA Node Support",
      CategoryName.Name(),
      SubCategoryName.Cat2())
    { }
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddPointParameter("Point", "Pt", "Point (x, y, z) location of support", GH_ParamAccess.item);
      pManager.AddPlaneParameter("Plane", "Pl", "(Optional) Plane for local axis", GH_ParamAccess.item, Plane.WorldXY);
      pManager.AddParameter(new GsaBool6Parameter(), "Restraints", "B6", "(Optional) Restraint in Bool6 form", GH_ParamAccess.item);

      pManager[1].Optional = true;
      pManager.HideParameter(1);
      pManager[2].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter("Node", "No", "GSA Node with Restraint", GH_ParamAccess.list);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      GH_Point ghpt = new GH_Point();
      if (DA.GetData(0, ref ghpt))
      {
        Point3d pt = new Point3d();
        if (GH_Convert.ToPoint3d(ghpt, ref pt, GH_Conversion.Both))
        {
          GH_Plane gH_Plane = new GH_Plane();
          Plane localAxis = Plane.WorldXY;
          if (DA.GetData(1, ref gH_Plane))
            GH_Convert.ToPlane(gH_Plane, ref localAxis, GH_Conversion.Both);

          GsaBool6 bool6 = new GsaBool6();
          if (DA.GetData(2, ref bool6))
          {
            _x = bool6.X;
            _y = bool6.Y;
            _z = bool6.Z;
            _xx = bool6.XX;
            _yy = bool6.YY;
            _zz = bool6.ZZ;
          }

          //GsaSpring spring = new GsaSpring();
          //if (DA.GetData(3, ref spring))
          //    node.Spring = spring;

          GsaNode node = new GsaNode(pt);
          node.LocalAxis = localAxis;

          node.Restraint = new GsaBool6
          {
            X = _x,
            Y = _y,
            Z = _z,
            XX = _xx,
            YY = _yy,
            ZZ = _zz
          };

          DA.SetData(0, new GsaNodeGoo(node));
        }
      }
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
      m_attributes = new OasysGH.UI.SupportComponentAttributes(this, SetRestraints, "Restraints", _x, _y, _z, _xx, _yy, _zz);
    }

    public void SetRestraints(bool resx, bool resy, bool resz, bool resxx, bool resyy, bool reszz)
    {
      _x = resx;
      _y = resy;
      _z = resz;
      _xx = resxx;
      _yy = resyy;
      _zz = reszz;

      base.UpdateUI();
    }
    #endregion

    #region (de)serialization
    public override bool Write(GH_IO.Serialization.GH_IWriter writer)
    {
      // we need to save all the items that we want to reappear when a GH file is saved and re-opened
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
      // when a GH file is opened we need to read in the data that was previously set by user
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
