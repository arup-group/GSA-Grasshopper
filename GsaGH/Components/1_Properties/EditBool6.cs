using System;
using System.Drawing;

using Grasshopper.Kernel;

using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;

using OasysGH;
using OasysGH.Components;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to edit a Bool6 and ouput the information
  /// </summary>
  public class EditBool6 : GH_OasysComponent {
    public override Guid ComponentGuid => new Guid("dad5064c-6648-45a5-8d98-afaae861e3b9");
    public override GH_Exposure Exposure => GH_Exposure.septenary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.EditBool6;

    public EditBool6() : base("Edit Bool6", "EditB6",
      "Modify a GSA Bool6 or just get information about existing", CategoryName.Name(),
      SubCategoryName.Cat1()) {
      Hidden = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaBool6Parameter(), GsaBool6Goo.Name, GsaBool6Goo.NickName,
        GsaBool6Goo.Description + " to get or set information for. Leave blank to create a new "
        + GsaBool6Goo.Name, GH_ParamAccess.item);
      pManager.AddBooleanParameter("X", "X", "Release or restraint for translation in X-direction", GH_ParamAccess.item);
      pManager.AddBooleanParameter("Y", "Y", "Release or restraint for translation in Y-direction", GH_ParamAccess.item);
      pManager.AddBooleanParameter("Z", "Z", "Release or restraint for translation in Z-direction", GH_ParamAccess.item);
      pManager.AddBooleanParameter("XX", "XX", "Release or restraint for rotation around X-axis", GH_ParamAccess.item);
      pManager.AddBooleanParameter("YY", "YY", "Release or restraint for rotation around Y-axis", GH_ParamAccess.item);
      pManager.AddBooleanParameter("ZZ", "ZZ", "Release or restraint for rotation around Z-axis", GH_ParamAccess.item);
      for (int i = 0; i < pManager.ParamCount; i++) {
        pManager[i].Optional = true;
      }
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaBool6Parameter(), GsaBool6Goo.Name, GsaBool6Goo.NickName,
        GsaBool6Goo.Description + " with applied changes.", GH_ParamAccess.item);
      pManager.AddBooleanParameter("X", "X", "Release or restraint for translation in X-direction", GH_ParamAccess.item);
      pManager.AddBooleanParameter("Y", "Y", "Release or restraint for translation in Y-direction", GH_ParamAccess.item);
      pManager.AddBooleanParameter("Z", "Z", "Release or restraint for translation in Z-direction", GH_ParamAccess.item);
      pManager.AddBooleanParameter("XX", "XX", "Release or restraint for rotation around X-axis", GH_ParamAccess.item);
      pManager.AddBooleanParameter("YY", "YY", "Release or restraint for rotation around Y-axis", GH_ParamAccess.item);
      pManager.AddBooleanParameter("ZZ", "ZZ", "Release or restraint for rotation around Z-axis", GH_ParamAccess.item);
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      var myBool = new GsaBool6();
      GsaBool6Goo bool6Goo = null;
      if (da.GetData(0, ref bool6Goo)) {
        myBool = new GsaBool6(bool6Goo.Value);
      }

      if (myBool != null) {
        bool x = new bool();
        if (da.GetData(1, ref x)) {
          myBool.X = x;
        }

        bool y = new bool();
        if (da.GetData(2, ref y)) {
          myBool.Y = y;
        }

        bool z = new bool();
        if (da.GetData(3, ref z)) {
          myBool.Z = z;
        }

        bool xx = new bool();
        if (da.GetData(4, ref xx)) {
          myBool.Xx = xx;
        }

        bool yy = new bool();
        if (da.GetData(5, ref yy)) {
          myBool.Yy = yy;
        }

        bool zz = new bool();
        if (da.GetData(6, ref zz)) {
          myBool.Zz = zz;
        }

        da.SetData(0, new GsaBool6Goo(myBool));
        da.SetData(1, myBool.X);
        da.SetData(2, myBool.Y);
        da.SetData(3, myBool.Z);
        da.SetData(4, myBool.Xx);
        da.SetData(5, myBool.Yy);
        da.SetData(6, myBool.Zz);
      } else {
        this.AddRuntimeError("Bool6 is Null");
      }
    }
  }
}
