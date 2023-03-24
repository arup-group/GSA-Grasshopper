using System;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using OasysGH;
using OasysGH.Components;
using Rhino.Geometry;

namespace GsaGH.Components {
  /// <summary>
  /// Component to create new Node with restraints (support)
  /// </summary>
  public class CreateSupport : GH_OasysDropDownComponent {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("d808e81f-6ae1-49d9-a8a5-2424a1763a69");
    public override GH_Exposure Exposure => GH_Exposure.primary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Properties.Resources.CreateSupport;

    public CreateSupport() : base("Create Support",
      "Support",
      "Create GSA Node Support",
      CategoryName.Name(),
      SubCategoryName.Cat2()) { }
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddPointParameter("Point", "Pt", "Point (x, y, z) location of support", GH_ParamAccess.item);
      pManager.AddPlaneParameter("Plane", "Pl", "(Optional) Plane for local axis", GH_ParamAccess.item, Plane.WorldXY);
      pManager.AddParameter(new GsaBool6Parameter(), "Restraints", "B6", "(Optional) Restraint in Bool6 form", GH_ParamAccess.item);

      pManager[1].Optional = true;
      pManager.HideParameter(1);
      pManager[2].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddGenericParameter("Node", "No", "GSA Node with Restraint", GH_ParamAccess.list);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess da) {
      var ghpt = new GH_Point();
      if (!da.GetData(0, ref ghpt)) {
        return;
      }

      var pt = new Point3d();
      if (!GH_Convert.ToPoint3d(ghpt, ref pt, GH_Conversion.Both)) {
        return;
      }

      var ghPlane = new GH_Plane();
      Plane localAxis = Plane.WorldXY;
      if (da.GetData(1, ref ghPlane))
        GH_Convert.ToPlane(ghPlane, ref localAxis, GH_Conversion.Both);

      var bool6 = new GsaBool6();
      if (da.GetData(2, ref bool6)) {
        _x = bool6.X;
        _y = bool6.Y;
        _z = bool6.Z;
        _xx = bool6.Xx;
        _yy = bool6.Yy;
        _zz = bool6.Zz;
      }

      var node = new GsaNode(pt) {
        LocalAxis = localAxis,
        Restraint = new GsaBool6 {
          X = _x,
          Y = _y,
          Z = _z,
          Xx = _xx,
          Yy = _yy,
          Zz = _zz,
        },
      };

      da.SetData(0, new GsaNodeGoo(node));
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
    public override void CreateAttributes() {
      m_attributes = new OasysGH.UI.SupportComponentAttributes(this, SetRestraints, "Restraints", _x, _y, _z, _xx, _yy, _zz);
    }

    public void SetRestraints(bool resx, bool resy, bool resz, bool resxx, bool resyy, bool reszz) {
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
    public override bool Write(GH_IO.Serialization.GH_IWriter writer) {
      writer.SetBoolean("x", _x);
      writer.SetBoolean("y", _y);
      writer.SetBoolean("z", _z);
      writer.SetBoolean("xx", _xx);
      writer.SetBoolean("yy", _yy);
      writer.SetBoolean("zz", _zz);
      return base.Write(writer);
    }

    public override bool Read(GH_IO.Serialization.GH_IReader reader) {
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
