﻿using System;
using System.Drawing;

using Grasshopper.Kernel;

using GsaGH.Helpers;
using GsaGH.Helpers.GH;
using GsaGH.Properties;

using OasysGH.Parameters;

namespace GsaGH.Parameters {
  /// <summary>
  ///   This class provides a parameter interface for the <see cref="GsaBool6Goo" /> type.
  /// </summary>
  public class GsaRestraintParameter : GsaBool6Parameter {
    public override Guid ComponentGuid => new Guid("9bf01532-2035-4108-9c56-5e88b87f5220");
    public override GH_Exposure Exposure => GH_Exposure.secondary | GH_Exposure.obscure;
    public override string InstanceDescription => GsaRestraintParameterInfo.Description;
    public override string TypeName => GsaRestraintParameterInfo.Name;
    protected override Bitmap Icon => Resources.RestraintParam;

    public GsaRestraintParameter() {
      Name = GsaRestraintParameterInfo.Name;
      NickName = GsaRestraintParameterInfo.NickName;
      Description = GsaRestraintParameterInfo.Description;
      Category = CategoryName.Name();
      SubCategory = SubCategoryName.Cat9();
    }

    protected override GsaBool6Goo PreferredCast(object data) {
      try {
        GsaBool6 gsaBool6 = ParseBool6.ParseRestrain(data);
        return new GsaBool6Goo(gsaBool6);
      } catch (InvalidCastException e) {
        this.AddRuntimeError(e.Message);
        return new GsaBool6Goo(null);
      }
    }

  }

  public class GsaRestraintParameterInfo {
    public static string Description => "GSA Bool6 containing six booleans representing a node restraint";
    public static string Name => "Restraint";
    public static string NickName => "Res";
  }
}
