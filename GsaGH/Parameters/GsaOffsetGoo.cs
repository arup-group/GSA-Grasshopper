using System;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaAPI;
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
  public class GsaOffsetGoo : GH_Goo<GsaOffset>
  {
    #region constructors
    public GsaOffsetGoo()
    {
      this.Value = new GsaOffset();
    }
    public GsaOffsetGoo(GsaOffset offset)
    {
      if (offset == null)
        offset = new GsaOffset();
      this.Value = offset; //offset.Duplicate();
    }

    public override IGH_Goo Duplicate()
    {
      return DuplicateGsaOffset();
    }
    public GsaOffsetGoo DuplicateGsaOffset()
    {
      return new GsaOffsetGoo(Value == null ? new GsaOffset() : Value); //Value.Duplicate());
    }
    #endregion

    #region properties
    public override bool IsValid
    {
      get
      {
        if (Value == null) { return false; }
        return true;
      }
    }

    public override string IsValidWhyNot
    {
      get
      {
        //if (Value == null) { return "No internal GsaOffset instance"; }
        if (Value.IsValid) { return string.Empty; }
        return Value.IsValid.ToString(); //Todo: beef this up to be more informative.
      }
    }

    public override string ToString()
    {
      if (Value == null)
        return "Null GSA Offset";
      else
        return Value.ToString();
    }

    public override string TypeName
    {
      get { return ("GSA Offset"); }
    }

    public override string TypeDescription
    {
      get { return ("GSA Offset"); }
    }
    #endregion

    #region casting methods
    public override bool CastTo<Q>(ref Q target)
    {
      // This function is called when Grasshopper needs to convert this 
      // instance of GsaOffset into some other type Q.            


      if (typeof(Q).IsAssignableFrom(typeof(GsaOffset)))
      {
        if (Value == null)
          target = default;
        else
          target = (Q)(object)Value.Duplicate();
        return true;
      }

      if (typeof(Q).IsAssignableFrom(typeof(Offset)))
      {
        if (Value == null)
          target = default;
        else
          target = (Q)(object)Value;
        return true;
      }

      target = default;
      return false;
    }
    public override bool CastFrom(object source)
    {
      // This function is called when Grasshopper needs to convert other data into GsaOffset.
      if (source == null) { return false; }

      //Cast from GsaOffset
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

      return false;
    }
    #endregion
  }
}
