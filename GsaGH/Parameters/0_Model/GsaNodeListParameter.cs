﻿using System;
using System.Drawing;
using Grasshopper.Kernel;
using GsaGH.Helpers.GH;
using GsaGH.Properties;
using OasysGH.Parameters;

namespace GsaGH.Parameters {
  /// <summary>
  ///   This class provides a parameter interface for the <see cref="GsaListGoo" /> type.
  /// </summary>
  public class GsaNodeListParameter : GH_OasysPersistentParam<GsaListGoo> {
    public override Guid ComponentGuid => new Guid("6982519e-f1e5-4b7c-9c00-1ccc562c6a73");
    public override GH_Exposure Exposure => GH_Exposure.hidden;
    public override string InstanceDescription
      => m_data.DataCount == 0 ? "Empty " + GsaListGoo.Name + " parameter" :
        base.InstanceDescription;
    public override string TypeName => SourceCount == 0 ? GsaListGoo.Name : base.TypeName;
    protected override Bitmap Icon => Resources.ListParam;
    private static EntityType _type = EntityType.Node;
    public GsaNodeListParameter() : base(new GH_InstanceDescription(
      "Node filter list",
      "No",
      $"Filter the Nodes by list. (by default 'all'){Environment.NewLine}" +
      $"Node list should take the form:{Environment.NewLine}" +
      $" 1 11 to 72 step 2 not (XY3 31 to 45){Environment.NewLine}" +
      "Refer to GSA help file for definition of lists and full vocabulary.", 
      CategoryName.Name(),
      SubCategoryName.Cat9())) { }

    protected override GsaListGoo PreferredCast(object data) {
      switch (data) {
        case GsaListGoo list:
          if (list.Value.EntityType == _type) {
            return list;
          } else if (list.Value.EntityType == EntityType.Undefined) {
            GsaList dup = list.Value.Duplicate();
            dup.EntityType = _type;
            return new GsaListGoo(dup);
          } else {
            this.AddRuntimeError("List must be of type Node to apply to node filter");
            return new GsaListGoo(null);
          }
      }

      if (GH_Convert.ToString(data, out string text, GH_Conversion.Both)) {
        var list = new GsaList() {
          EntityType = _type,
          Definition = text
        };
        return new GsaListGoo(list);
      }

      this.AddRuntimeError($"Data conversion failed from {data.GetTypeName()} to Node List");
      return new GsaListGoo(null);
    }
  }
}
