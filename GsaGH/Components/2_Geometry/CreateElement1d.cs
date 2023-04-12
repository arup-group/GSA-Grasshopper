using System;
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
  ///   Component to create new 1D Element
  /// </summary>
  public class CreateElement1d : GH_OasysComponent {
    public override Guid ComponentGuid => new Guid("88c58aae-4cd8-4d37-b63f-d828571e6941");
    public override GH_Exposure Exposure => GH_Exposure.primary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    
    public CreateElement1d() : base("Create 1D Element",
      "Elem1D",
      "Create GSA 1D Element",
      CategoryName.Name(),
      SubCategoryName.Cat2()) { }

    protected override Bitmap Icon => Resources.CreateElem1d;
    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddLineParameter("Line", "L", "Line to create GSA Element", GH_ParamAccess.item);
      pManager.AddParameter(new GsaSectionParameter());
      pManager[1]
        .Optional = true;
      pManager.HideParameter(0);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
      => pManager.AddParameter(new GsaElement1dParameter());

    protected override void SolveInstance(IGH_DataAccess da) {
      var ghln = new GH_Line();
      if (!da.GetData(0, ref ghln))
        return;

      if (ghln == null)
        this.AddRuntimeWarning("Line input is null");
      var ln = new Line();
      if (!GH_Convert.ToLine(ghln, ref ln, GH_Conversion.Both))
        return;

      var elem = new GsaElement1d(new LineCurve(ln));

      var ghTyp = new GH_ObjectWrapper();
      var section = new GsaSection();
      if (da.GetData(1, ref ghTyp)) {
        if (ghTyp.Value is GsaSectionGoo) {
          ghTyp.CastTo(ref section);
          elem.Section = section;
        }
        else {
          if (GH_Convert.ToInt32(ghTyp.Value, out int id, GH_Conversion.Both))
            elem.Section = new GsaSection(id);
          else {
            this.AddRuntimeError(
              "Unable to convert PB input to a Section Property of reference integer");
            return;
          }
        }
      }

      da.SetData(0, new GsaElement1dGoo(elem));
    }
  }
}
