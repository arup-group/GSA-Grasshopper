using System;
using System.Drawing;

using Grasshopper.Kernel;

using GsaGH.Helpers.GH;
using GsaGH.Properties;

using OasysGH.Parameters;

namespace GsaGH.Parameters {
  /// <summary>
  ///   This class provides a parameter interface for the <see cref="GsaDesignTaskGoo" /> type.
  /// </summary>
  public class GsaDesignTaskParameter : GH_OasysPersistentParam<GsaDesignTaskGoo> {
    public override Guid ComponentGuid => new Guid("81d8f331-9d8c-49c6-8953-e26ae1d51515");
    public override GH_Exposure Exposure => GH_Exposure.quinary | GH_Exposure.obscure;
    public override string InstanceDescription
      => m_data.DataCount == 0 ? "Empty " + GsaDesignTaskGoo.Name + " parameter" :
        base.InstanceDescription;
    public override string TypeName => SourceCount == 0 ? GsaDesignTaskGoo.Name : base.TypeName;
    protected override Bitmap Icon => Resources.DesignTaskParam;

    public GsaDesignTaskParameter() : base(new GH_InstanceDescription(
      GsaDesignTaskGoo.Name, GsaDesignTaskGoo.NickName,
      GsaDesignTaskGoo.Description + " parameter",
      CategoryName.Name(), SubCategoryName.Cat9())) { }

    protected override GsaDesignTaskGoo PreferredCast(object data) {
      this.AddRuntimeError($"Data conversion failed from {data.GetTypeName()} to DesignTask");
      return new GsaDesignTaskGoo(null);
    }
  }
}
