using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;

using OasysGH;

using Rhino.Collections;
using Rhino.Geometry;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to create new 2D Member
  /// </summary>
  public class Create2dMember : Section3dPreviewDropDownComponent {
    public override Guid ComponentGuid => new Guid("097037ce-bfc7-44c0-bc96-dc8c52466249");
    public override GH_Exposure Exposure => GH_Exposure.tertiary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.Create2dMember;

    public Create2dMember() : base("Create 2D Member", "Mem2D", "Create GSA Member 2D",
      CategoryName.Name(), SubCategoryName.Cat2()) { }

    protected override void InitialiseDropdowns() {
      _spacerDescriptions = new List<string>(new[] {
        "Mesh mode",
      });

      _dropDownItems = new List<List<string>>() {
        new List<string>() {
          "Tri only",
          "Quad dominant",
          "Quad only"
        }
      };
      _selectedItems = new List<string> {
        _dropDownItems[0][1]
      };

      _isInitialised = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddBrepParameter("Brep", "B",
        "Planar Brep (non-planar geometry will be automatically converted to an average plane of exterior boundary control points))",
        GH_ParamAccess.item);
      pManager.AddPointParameter("Incl. Points", "(P)",
        "Inclusion points (will automatically be projected onto Brep)", GH_ParamAccess.list);
      pManager.AddCurveParameter("Incl. Curves", "(C)",
        "Inclusion curves (will automatically be made planar and projected onto brep, and converted to Arcs and Lines)",
        GH_ParamAccess.list);
      pManager.AddParameter(new GsaProperty2dParameter());
      pManager.AddNumberParameter("Mesh Size in model units", "Ms", "Target mesh size",
        GH_ParamAccess.item);
      pManager.AddBooleanParameter("Internal Offset", "IO",
        "Set Automatic Internal Offset of Member", GH_ParamAccess.item, true);

      pManager.HideParameter(0);
      pManager.HideParameter(1);
      pManager.HideParameter(2);

      pManager[1].Optional = true;
      pManager[2].Optional = true;
      pManager[3].Optional = true;
      pManager[4].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaMember2dParameter());
    }

    protected override void SolveInternal(IGH_DataAccess da) {
      GH_Brep ghbrep = null;
      da.GetData(0, ref ghbrep);

      var points = new Point3dList();
      var ghpts = new List<GH_Point>();
      if (da.GetDataList(1, ghpts)) {
        points = new Point3dList(ghpts.ConvertAll(pt => pt.Value));
      }

      var crvs = new List<Curve>();
      var ghcrvs = new List<GH_Curve>();
      if (da.GetDataList(2, ghcrvs)) {
        crvs = ghcrvs.Select(crv => crv.Value).ToList();
      }

      var member2d = new GsaMember2d(ghbrep.Value, crvs, points);

      GsaProperty2dGoo prop2dGoo = null;
      if (da.GetData(3, ref prop2dGoo)) {
        member2d.SetProperty(prop2dGoo.Value);
        if (Preview3dSection) {
          member2d.CreateSection3dPreview();
        }
      }

      double meshSize = 0;
      if (da.GetData(4, ref meshSize)) {
        member2d.ApiMember.MeshSize = meshSize;
      }

      bool internalOffset = false;
      if (da.GetData(5, ref internalOffset)) {
        member2d.ApiMember.AutomaticOffset.Internal = internalOffset;
      }

      if (_selectedItems[0] != _dropDownItems[0][1]) {
        if (_selectedItems[0] == _dropDownItems[0][0]) {
          member2d.ApiMember.MeshMode2d = GsaAPI.MeshMode2d.Tri;
        } else {
          member2d.ApiMember.MeshMode2d = GsaAPI.MeshMode2d.Quad;
        }
      }

      da.SetData(0, new GsaMember2dGoo(member2d));
    }

    public override void SetSelected(int i, int j) {
      _selectedItems[i] = _dropDownItems[i][j];
      base.UpdateUI();
    }
  }
}
