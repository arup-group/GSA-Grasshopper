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

using Rhino.Geometry;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to create new Node with restraints (support)
  /// </summary>
  public class CreateSupport : GH_OasysDropDownComponent {
    public override Guid ComponentGuid => new Guid("d808e81f-6ae1-49d9-a8a5-2424a1763a69");
    public override GH_Exposure Exposure => GH_Exposure.primary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.CreateSupport;
    private bool _x;
    private bool _xx;
    private bool _y;
    private bool _yy;
    private bool _z;
    private bool _zz;

    public CreateSupport() : base("Create Support", "Support", "Create GSA Node Support",
      CategoryName.Name(), SubCategoryName.Cat2()) { }

    public override void CreateAttributes() {
      if (!_isInitialised) {
        InitialiseDropdowns();
      }
      m_attributes = new SupportComponentAttributes(this, SetRestraints, "Restraints", _x, _y, _z,
        _xx, _yy, _zz);
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

    public void SetRestraints(bool resx, bool resy, bool resz, bool resxx, bool resyy, bool reszz) {
      _x = resx;
      _y = resy;
      _z = resz;
      _xx = resxx;
      _yy = resyy;
      _zz = reszz;

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
      pManager.AddPointParameter("Point", "Pt", "Point (x, y, z) location of support",
        GH_ParamAccess.item);
      pManager.AddPlaneParameter("Plane", "Pl", "[Optional] Plane for local axis",
        GH_ParamAccess.item, Plane.WorldXY);
      pManager.AddParameter(new GsaBool6Parameter(), "Restraints", "B6",
        "[Optional] Restraint in Bool6 form", GH_ParamAccess.item);

      pManager[1].Optional = true;
      pManager.HideParameter(1);
      pManager[2].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaNodeParameter(), "Node", "No", "GSA Node with Restraint", GH_ParamAccess.item);
    }

    protected override void SolveInternal(IGH_DataAccess da) {
      GH_Point ghpt = null;
      da.GetData(0, ref ghpt);

      Plane localAxis = Plane.WorldXY;
      GH_Plane ghPlane = null;
      if (da.GetData(1, ref ghPlane)) {
        localAxis = ghPlane.Value;
      }

      GsaBool6Goo bool6Goo = null;
      if (da.GetData(2, ref bool6Goo)) {
        _x = bool6Goo.Value.X;
        _y = bool6Goo.Value.Y;
        _z = bool6Goo.Value.Z;
        _xx = bool6Goo.Value.Xx;
        _yy = bool6Goo.Value.Yy;
        _zz = bool6Goo.Value.Zz;
      }

      var node = new GsaNode(ghpt.Value) {
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

      node.UpdatePreview();

      da.SetData(0, new GsaNodeGoo(node));
    }
  }
}
