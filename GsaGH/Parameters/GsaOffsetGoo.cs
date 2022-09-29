using System;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaAPI;
using OasysGH;
using OasysGH.Parameters;
using OasysGH.Units;
using OasysUnits;
using OasysUnits.Units;

namespace GsaGH.Parameters
{
  /// <summary>
  /// Offset class, this class defines the basic properties and methods for any Gsa Offset
  /// </summary>
  public class GsaOffset
  {
    public Length X1 { get; set; } = Length.Zero;
    public Length X2 { get; set; } = Length.Zero;
    public Length Y { get; set; } = Length.Zero;
    public Length Z { get; set; } = Length.Zero;

    #region constructors
    public GsaOffset() { }

    public GsaOffset(double x1, double x2, double y, double z, LengthUnit unit = LengthUnit.Meter)
    {
      X1 = new Length(x1, unit);
      X2 = new Length(x2, unit);
      Y = new Length(y, unit);
      Z = new Length(z, unit);
    }

    public GsaOffset Duplicate()
    {
      if (this == null) { return null; }
      return (GsaOffset)this.MemberwiseClone(); // all members are structs
    }
    #endregion

    #region properties
    public bool IsValid
    {
      get
      {
        if (this == null) { return false; }
        return true;
      }
    }
    #endregion

    #region methods
    public override string ToString()
    {
      IQuantity quantity = new Length(0, DefaultUnits.LengthUnitGeometry);
      string unitAbbreviation = string.Concat(quantity.ToString().Where(char.IsLetter));

      return "Offset" + " {X1: "
          + Math.Round(X1.As(DefaultUnits.LengthUnitGeometry), 4) + ", X2: "
          + Math.Round(X2.As(DefaultUnits.LengthUnitGeometry), 4) + ", Y: "
          + Math.Round(Y.As(DefaultUnits.LengthUnitGeometry), 4) + ", Z: "
          + Math.Round(Z.As(DefaultUnits.LengthUnitGeometry), 4) + " [" + unitAbbreviation + "]}";
    }
    #endregion
  }

  /// <summary>
  /// Goo wrapper class, makes sure <see cref="GsaOffset"/> can be used in Grasshopper.
  /// </summary>
  public class GsaOffsetGoo : GH_OasysGoo<GsaOffset>
  {
    public static string Name => "Offset";
    public static string NickName => "Off";
    public static string Description => "GSA Offset";
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;

    public GsaOffsetGoo(GsaOffset item) : base(item) { }

    public override IGH_Goo Duplicate() => new GsaOffsetGoo(this.Value);

    public override bool CastTo<Q>(ref Q target)
    {
      if (typeof(Q).IsAssignableFrom(typeof(GsaOffset)))
      {
        if (Value == null)
          target = default;
        else
          target = (Q)(object)Value.Duplicate();
        return true;
      }

      else if (typeof(Q).IsAssignableFrom(typeof(Offset)))
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

      // Cast from GsaOffset
      if (typeof(GsaOffset).IsAssignableFrom(source.GetType()))
      {
        Value = (GsaOffset)source;
        return true;
      }

      // Cast from double
      if (GH_Convert.ToDouble(source, out double myval, GH_Conversion.Both))
      {
        Value.Z = new Length(myval, DefaultUnits.LengthUnitGeometry);
        // if input to parameter is a single number convert it to the most common Z-offset
        return true;
      }

      return base.CastFrom(source);
    }
  }
}
