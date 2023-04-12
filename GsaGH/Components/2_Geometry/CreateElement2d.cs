using System;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;
using OasysGH;
using OasysGH.Components;
using Rhino.Geometry;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to create new 2D Element
  /// </summary>
  public class CreateElement2d : GH_OasysComponent {
    public override Guid ComponentGuid => new Guid("8f83d32a-c2df-4f47-9cfc-d2d4253703e1");
    public override GH_Exposure Exposure => GH_Exposure.primary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    protected override Bitmap Icon => Resources.CreateElem2d;

    public CreateElement2d() : base("Create 2D Element",
          "Elem2D",
      "Create GSA 2D Element",
      CategoryName.Name(),
      SubCategoryName.Cat2()) { }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddMeshParameter("Mesh", "M", "Mesh to create GSA Element", GH_ParamAccess.item);
      pManager.AddParameter(new GsaProp2dParameter());
      pManager[1]
        .Optional = true;
      pManager.HideParameter(0);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
      => pManager.AddParameter(new GsaElement2dParameter());

    protected override void SolveInstance(IGH_DataAccess da) {
      var ghmesh = new GH_Mesh();
      if (!da.GetData(0, ref ghmesh))
        return;

      if (ghmesh == null)
        this.AddRuntimeWarning("Mesh input is null");
      var mesh = new Mesh();
      if (!GH_Convert.ToMesh(ghmesh, ref mesh, GH_Conversion.Both))
        return;

      var elem = new GsaElement2d(mesh);

      var ghTyp = new GH_ObjectWrapper();
      var prop2d = new GsaProp2d();
      if (da.GetData(1, ref ghTyp)) {
        if (ghTyp.Value is GsaProp2dGoo)
          ghTyp.CastTo(ref prop2d);
        else if (GH_Convert.ToInt32(ghTyp.Value, out int id, GH_Conversion.Both))
          prop2d = new GsaProp2d(id);
        else {
          this.AddRuntimeError("Unable to convert PA input to a 2D Property or reference integer");
          return;
        }

        var prop2Ds = new List<GsaProp2d>();
        for (int i = 0; i < elem.ApiElements.Count; i++)
          prop2Ds.Add(prop2d);
        elem.Properties = prop2Ds;
      }

      da.SetData(0, new GsaElement2dGoo(elem));
    }
  }
}
