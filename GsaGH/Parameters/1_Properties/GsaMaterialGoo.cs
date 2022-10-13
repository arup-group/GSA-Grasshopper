using System;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaAPI;
using OasysGH;
using OasysGH.Parameters;

namespace GsaGH.Parameters
{
  /// <summary>
  /// Goo wrapper class, makes sure <see cref="GsaMaterial"/> can be used in Grasshopper.
  /// </summary>
  public class GsaMaterialGoo : GH_OasysGoo<GsaMaterial>
  {
    public static string Name => "Material";
    public static string NickName => "Mat";
    public static string Description => "GSA Material";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    public GsaMaterialGoo(GsaMaterial item) : base(item) { }

    public override IGH_Goo Duplicate() => new GsaMaterialGoo(this.Value);

    public override bool CastTo<Q>(ref Q target)
    {
      if (typeof(Q).IsAssignableFrom(typeof(GsaMaterial)))
      {
        if (Value == null)
          target = default;
        else
          target = (Q)(object)Value;
        return true;
      }
      return base.CastTo(ref target);
    }

    public override bool CastFrom(object source)
    {
      if (source == null) 
        return false; 

      // Cast from GsaMaterial
      if (typeof(GsaMaterial).IsAssignableFrom(source.GetType()))
      {
        Value = (GsaMaterial)source;
        return true;
      }

      // Cast from string
      if (GH_Convert.ToString(source, out string mat, GH_Conversion.Both))
      {
        if (mat.ToUpper() == "STEEL")
        {
          Value.MaterialType = GsaMaterial.MatType.STEEL;
          return true;
        }
        else if (mat.ToUpper() == "CONCRETE")
        {
          Value.MaterialType = GsaMaterial.MatType.CONCRETE;
          return true;
        }
        else if (mat.ToUpper() == "FRP")
        {
          Value.MaterialType = GsaMaterial.MatType.FRP;
          return true;
        }
        else if (mat.ToUpper() == "ALUMINIUM")
        {
          Value.MaterialType = GsaMaterial.MatType.ALUMINIUM;
          return true;
        }
        else if (mat.ToUpper() == "TIMBER")
        {
          Value.MaterialType = GsaMaterial.MatType.TIMBER;
          return true;
        }
        else if (mat.ToUpper() == "GLASS")
        {
          Value.MaterialType = GsaMaterial.MatType.GLASS;
          return true;
        }
        else if (mat.ToUpper() == "FABRIC")
        {
          Value.MaterialType = GsaMaterial.MatType.FABRIC;
          return true;
        }
        else if (mat.ToUpper() == "GENERIC")
        {
          Value.MaterialType = GsaMaterial.MatType.GENERIC;
          return true;
        }
        return false;
      }

      // Cast from integer
      else if (GH_Convert.ToInt32(source, out int idd, GH_Conversion.Both))
      {
        Value.AnalysisProperty = idd;
      }

      return base.CastFrom(source);
    }
  }
}
