using System;
using System.Collections.Generic;
using System.Linq;
using GsaAPI;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using UnitsNet;
using UnitsNet.Units;

namespace GsaGH.Parameters
{
  public enum StressOptionType
  {
    NoCalculation,
    UseModified,
    UseUnmodified
  }

  /// <summary>
  /// Section Modifier class, this class defines the basic properties and methods for any Gsa Offset
  /// </summary>
  public class GsaSectionModifier
  {

    internal SectionModifier API_SectionModifier;
    private void CloneAPIModifier()
    {
      SectionModifier dup = new SectionModifier();
      
      dup.AreaModifier = new SectionModifierAttribute(
        this.API_SectionModifier.AreaModifier.Option, this.API_SectionModifier.AreaModifier.Value);
      
      dup.I11Modifier = new SectionModifierAttribute(
        this.API_SectionModifier.I11Modifier.Option, this.API_SectionModifier.I11Modifier.Value);
      
      dup.I22Modifier = new SectionModifierAttribute(
        this.API_SectionModifier.I22Modifier.Option, this.API_SectionModifier.I22Modifier.Value);
      
      dup.JModifier = new SectionModifierAttribute(
        this.API_SectionModifier.JModifier.Option, this.API_SectionModifier.JModifier.Value);
      
      dup.K11Modifier = new SectionModifierAttribute(
        this.API_SectionModifier.K11Modifier.Option, this.API_SectionModifier.K11Modifier.Value);
      
      dup.K22Modifier = new SectionModifierAttribute(
        this.API_SectionModifier.K22Modifier.Option, this.API_SectionModifier.K22Modifier.Value);
      
      dup.VolumeModifier = new SectionModifierAttribute(
        this.API_SectionModifier.VolumeModifier.Option, this.API_SectionModifier.VolumeModifier.Value);
      
      dup.AdditionalMass = this.API_SectionModifier.AdditionalMass;
      
      dup.StressOption = this.API_SectionModifier.StressOption;
      
      dup.IsBendingAxesPrincipal = this.API_SectionModifier.IsBendingAxesPrincipal;
      
      dup.IsReferencePointCentroid = this.API_SectionModifier.IsReferencePointCentroid;
      
      this.API_SectionModifier = dup;
    }

    public IQuantity AreaModifier
    {
      get 
      {
        if (this.API_SectionModifier == null) { return new Ratio(100, RatioUnit.Percent); }

        if (this.API_SectionModifier.AreaModifier.Option == SectionModifierOptionType.BY)
          return new Ratio(this.API_SectionModifier.AreaModifier.Value, 
            RatioUnit.DecimalFraction).ToUnit(RatioUnit.Percent);
        else
          return new Area(this.API_SectionModifier.AreaModifier.Value, AreaUnit.SquareMeter).ToUnit(Units.SectionAreaUnit);
      }
      set
      {
        if (value.QuantityInfo.UnitType != typeof(AreaUnit)
          | value.QuantityInfo.UnitType != typeof(RatioUnit))
          throw new ArgumentException("AreaModifier must be either Area or Ratio");
        else
        {
          CloneAPIModifier();
          if (value.QuantityInfo.UnitType == typeof(AreaUnit))
            API_SectionModifier.AreaModifier = new SectionModifierAttribute(
              SectionModifierOptionType.TO, value.As(AreaUnit.SquareMeter));
          else
            API_SectionModifier.AreaModifier = new SectionModifierAttribute(
              SectionModifierOptionType.BY, value.As(RatioUnit.DecimalFraction));
        }
      }
    }

    public IQuantity I11Modifier
    {
      get
      {
        if (this.API_SectionModifier == null) { return new Ratio(100, RatioUnit.Percent); }

        if (this.API_SectionModifier.I11Modifier.Option == SectionModifierOptionType.BY)
          return new Ratio(this.API_SectionModifier.I11Modifier.Value,
            RatioUnit.DecimalFraction).ToUnit(RatioUnit.Percent);
        else
          return new AreaMomentOfInertia(this.API_SectionModifier.I11Modifier.Value, 
            AreaMomentOfInertiaUnit.MeterToTheFourth).ToUnit(Units.SectionAreaMomentOfInertiaUnit);
      }
      set
      {
        if (value.QuantityInfo.UnitType != typeof(AreaMomentOfInertiaUnit)
          | value.QuantityInfo.UnitType != typeof(RatioUnit))
          throw new ArgumentException("I11Modifier must be either AreaMomentOfInertia or Ratio");
        else
        {
          CloneAPIModifier();
          if (value.QuantityInfo.UnitType == typeof(AreaMomentOfInertiaUnit))
            API_SectionModifier.I11Modifier = new SectionModifierAttribute(
              SectionModifierOptionType.TO, value.As(AreaMomentOfInertiaUnit.MeterToTheFourth));
          else
            API_SectionModifier.I11Modifier = new SectionModifierAttribute(
              SectionModifierOptionType.BY, value.As(RatioUnit.DecimalFraction));
        }
      }
    }
    public IQuantity I22Modifier
    {
      get
      {
        if (this.API_SectionModifier == null) { return new Ratio(100, RatioUnit.Percent); }

        if (this.API_SectionModifier.I22Modifier.Option == SectionModifierOptionType.BY)
          return new Ratio(this.API_SectionModifier.I22Modifier.Value,
            RatioUnit.DecimalFraction).ToUnit(RatioUnit.Percent);
        else
          return new AreaMomentOfInertia(this.API_SectionModifier.I22Modifier.Value,
            AreaMomentOfInertiaUnit.MeterToTheFourth).ToUnit(Units.SectionAreaMomentOfInertiaUnit);
      }
      set
      {
        if (value.QuantityInfo.UnitType != typeof(AreaMomentOfInertiaUnit)
          | value.QuantityInfo.UnitType != typeof(RatioUnit))
          throw new ArgumentException("I22Modifier must be either AreaMomentOfInertia or Ratio");
        else
        {
          CloneAPIModifier();
          if (value.QuantityInfo.UnitType == typeof(AreaMomentOfInertiaUnit))
            API_SectionModifier.I22Modifier = new SectionModifierAttribute(
              SectionModifierOptionType.TO, value.As(AreaMomentOfInertiaUnit.MeterToTheFourth));
          else
            API_SectionModifier.I22Modifier = new SectionModifierAttribute(
              SectionModifierOptionType.BY, value.As(RatioUnit.DecimalFraction));
        }
      }
    }

    public IQuantity JModifier
    {
      get
      {
        if (this.API_SectionModifier == null) { return new Ratio(100, RatioUnit.Percent); }

        if (this.API_SectionModifier.JModifier.Option == SectionModifierOptionType.BY)
          return new Ratio(this.API_SectionModifier.JModifier.Value,
            RatioUnit.DecimalFraction).ToUnit(RatioUnit.Percent);
        else
          return new AreaMomentOfInertia(this.API_SectionModifier.JModifier.Value,
            AreaMomentOfInertiaUnit.MeterToTheFourth).ToUnit(Units.SectionAreaMomentOfInertiaUnit);
      }
      set
      {
        if (value.QuantityInfo.UnitType != typeof(AreaMomentOfInertiaUnit)
          | value.QuantityInfo.UnitType != typeof(RatioUnit))
          throw new ArgumentException("I22Modifier must be either AreaMomentOfInertia or Ratio");
        else
        {
          CloneAPIModifier();
          if (value.QuantityInfo.UnitType == typeof(AreaMomentOfInertiaUnit))
            API_SectionModifier.JModifier = new SectionModifierAttribute(
              SectionModifierOptionType.TO, value.As(AreaMomentOfInertiaUnit.MeterToTheFourth));
          else
            API_SectionModifier.JModifier = new SectionModifierAttribute(
              SectionModifierOptionType.BY, value.As(RatioUnit.DecimalFraction));
        }
      }
    }
    public IQuantity VolumeModifier
    {
      get
      {
        if (this.API_SectionModifier == null) { return new Ratio(100, RatioUnit.Percent); }

        if (this.API_SectionModifier.VolumeModifier.Option == SectionModifierOptionType.BY)
          return new Ratio(this.API_SectionModifier.I11Modifier.Value,
            RatioUnit.DecimalFraction).ToUnit(RatioUnit.Percent);
        else
          return new VolumePerLength(this.API_SectionModifier.I11Modifier.Value,
            VolumePerLengthUnit.CubicMeterPerMeter).ToUnit(Units.VolumePerLengthUnit);
      }
      set
      {
        if (value.QuantityInfo.UnitType != typeof(VolumePerLengthUnit)
          | value.QuantityInfo.UnitType != typeof(RatioUnit))
          throw new ArgumentException("VolumeModifier must be either VolumePerLength or Ratio");
        else
        {
          CloneAPIModifier();
          if (value.QuantityInfo.UnitType == typeof(VolumePerLengthUnit))
            API_SectionModifier.VolumeModifier = new SectionModifierAttribute(
              SectionModifierOptionType.TO, value.As(VolumePerLengthUnit.CubicMeterPerMeter));
          else
            API_SectionModifier.VolumeModifier = new SectionModifierAttribute(
              SectionModifierOptionType.BY, value.As(RatioUnit.DecimalFraction));
        }
      }
    }

    public Ratio K11Modifier
    {
      get 
      {
        if (this.API_SectionModifier == null) { return new Ratio(100, RatioUnit.Percent); }

        if (this.API_SectionModifier.K11Modifier.Option == SectionModifierOptionType.BY)
          return new Ratio(this.API_SectionModifier.K11Modifier.Value,
            RatioUnit.DecimalFraction).ToUnit(RatioUnit.Percent);
        else
          return new Ratio(this.API_SectionModifier.K11Modifier.Value,RatioUnit.DecimalFraction);
      }
      set
      {
        CloneAPIModifier();
        if (value.Unit != RatioUnit.Percent) // assume that percentage unit is modify BY option
          API_SectionModifier.K11Modifier = new SectionModifierAttribute(
            SectionModifierOptionType.BY, value.As(RatioUnit.DecimalFraction));
        else // assume that all other than percentage unit is modify TO option
          API_SectionModifier.K11Modifier = new SectionModifierAttribute(
            SectionModifierOptionType.TO, value.As(RatioUnit.DecimalFraction));
      }
    }
    public Ratio K22Modifier
    {
      get
      {
        if (this.API_SectionModifier == null) { return new Ratio(100, RatioUnit.Percent); }

        if (this.API_SectionModifier.K22Modifier.Option == SectionModifierOptionType.BY)
          return new Ratio(this.API_SectionModifier.K22Modifier.Value,
            RatioUnit.DecimalFraction).ToUnit(RatioUnit.Percent);
        else
          return new Ratio(this.API_SectionModifier.K22Modifier.Value, RatioUnit.DecimalFraction);
      }
      set
      {
        CloneAPIModifier();
        if (value.Unit != RatioUnit.Percent) // assume that percentage unit is modify BY option
          API_SectionModifier.K22Modifier = new SectionModifierAttribute(
            SectionModifierOptionType.BY, value.As(RatioUnit.DecimalFraction));
        else // assume that all other than percentage unit is modify TO option
          API_SectionModifier.K22Modifier = new SectionModifierAttribute(
            SectionModifierOptionType.TO, value.As(RatioUnit.DecimalFraction));
      }
    }
    public LinearDensity AdditionalMass 
    {
      get
      {
        if (this.API_SectionModifier == null) { return LinearDensity.Zero; }
        return new LinearDensity(this.API_SectionModifier.AdditionalMass, LinearDensityUnit.KilogramPerMeter);
      }
      set
      {
        CloneAPIModifier();
        API_SectionModifier.AdditionalMass = value.As(LinearDensityUnit.KilogramPerMeter);
      }
    }

    public StressOptionType StressOption 
    {
      get
      {
        switch (this.API_SectionModifier.StressOption)
        {
          case SectionModifierStressType.USE_MOD:
            return StressOptionType.UseModified;
          case SectionModifierStressType.USE_UNMOD:
            return StressOptionType.UseUnmodified;
          case SectionModifierStressType.NO_MOD:
          default:
            return StressOptionType.NoCalculation;
        }
      }
      set
      {
        CloneAPIModifier();
        switch (value)
        {
          case StressOptionType.UseModified:
            this.API_SectionModifier.StressOption = SectionModifierStressType.USE_MOD;
            break;
          case StressOptionType.UseUnmodified:
            this.API_SectionModifier.StressOption = SectionModifierStressType.USE_UNMOD;
            break;
          case StressOptionType.NoCalculation:
          default:
            this.API_SectionModifier.StressOption = SectionModifierStressType.NO_MOD;
            break;
        }
      }
    } 

    public bool isBendingAxesPrincipal
    {
      get
      {
        return this.API_SectionModifier.IsBendingAxesPrincipal;
      }
      set
      {
        CloneAPIModifier();
        this.API_SectionModifier.IsBendingAxesPrincipal = value;
      }
    }
    public bool isReferencePointCentroid
    {
      get
      {
        return this.API_SectionModifier.IsReferencePointCentroid;
      }
      set
      {
        CloneAPIModifier();
        this.API_SectionModifier.IsReferencePointCentroid = value;
      }
    }

    #region constructors
    public GsaSectionModifier() 
    { 
      this.API_SectionModifier = new SectionModifier();
    }

    internal GsaSectionModifier(SectionModifier apiMod)
    {
      this.API_SectionModifier = apiMod;
    }

    public GsaSectionModifier Duplicate()
    {
      if (this == null) { return null; }
      return this;
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
      string str = "Section Modifier";
      string A = "A ";
      string I11 = "I11 ";
      string I22 = "I22 ";
      string J = "J ";
      string K11 = "K11 ";
      string K22 = "K22 ";
      string V = "V ";
      string mass = "Add.Mass ";
      string stress = "Stress.Calc.Opt. ";

      // Area
      if (this.API_SectionModifier.AreaModifier.Option == SectionModifierOptionType.TO)
      {
        Area val = (Area)this.AreaModifier;
        A += val.ToString("f0").Replace(" ", string.Empty);
      }
      else
      {
        Ratio val = (Ratio)this.AreaModifier;
        A += val.ToString("f0").Replace(" ", string.Empty);
      }

      // I11
      if (this.API_SectionModifier.I11Modifier.Option == SectionModifierOptionType.TO)
      {
        AreaMomentOfInertia val = (AreaMomentOfInertia)this.I11Modifier;
        I11 += val.ToString("f0").Replace(" ", string.Empty);
      }
      else
      {
        Ratio val = (Ratio)this.I11Modifier;
        I11 += val.ToString("f0").Replace(" ", string.Empty);
      }

      // I22
      if (this.API_SectionModifier.I22Modifier.Option == SectionModifierOptionType.TO)
      {
        AreaMomentOfInertia val = (AreaMomentOfInertia)this.I22Modifier;
        I22 += val.ToString("f0").Replace(" ", string.Empty);
      }
      else
      {
        Ratio val = (Ratio)this.I22Modifier;
        I22 += val.ToString("f0").Replace(" ", string.Empty);
      }

      // J
      if (this.API_SectionModifier.JModifier.Option == SectionModifierOptionType.TO)
      {
        AreaMomentOfInertia val = (AreaMomentOfInertia)this.JModifier;
        J += val.ToString("f0").Replace(" ", string.Empty);
      }
      else
      {
        Ratio val = (Ratio)this.JModifier;
        J += val.ToString("f0").Replace(" ", string.Empty);
      }

      // K11
      if (this.API_SectionModifier.K11Modifier.Option == SectionModifierOptionType.TO)
      {
        K11 += this.API_SectionModifier.K11Modifier.Value.ToString("f3") + "[-]";
      }
      else
      {
        Ratio val = this.K11Modifier;
        K11 += val.ToString("f0").Replace(" ", string.Empty);
      }

      // K22
      if (this.API_SectionModifier.K22Modifier.Option == SectionModifierOptionType.TO)
      {
        K22 += this.API_SectionModifier.K22Modifier.Value.ToString("f3") + "[-]";
      }
      else
      {
        Ratio val = this.K22Modifier;
        K22 += val.ToString("f0").Replace(" ", string.Empty);
      }

      // Volume
      if (this.API_SectionModifier.VolumeModifier.Option == SectionModifierOptionType.TO)
      {
        VolumePerLength val = (VolumePerLength)this.VolumeModifier;
        V += val.ToString("f0").Replace(" ", string.Empty);
      }
      else
      {
        Ratio val = (Ratio)this.VolumeModifier;
        V += val.ToString("f0").Replace(" ", string.Empty);
      }

      // Additional Mass
      V += this.AdditionalMass.ToString("f0").Replace(" ", string.Empty);

      if (this.API_SectionModifier.StressOption == SectionModifierStressType.NO_MOD)
        return string.Join(", ", str, A, I11, I22, J, K11, K22, V, mass);
      else if (this.API_SectionModifier.StressOption == SectionModifierStressType.USE_MOD)
        stress += "use modified";
      else
        stress += "use unmodified";
     
      return string.Join(", ", str, A, I11, I22, J, K11, K22, V, mass, stress);
    }
    #endregion
  }

  /// <summary>
  /// Goo wrapper class, makes sure this class can be used in Grasshopper.
  /// </summary>
  public class GsaSectionModifierGoo : GH_Goo<GsaSectionModifier>
  {
    #region constructors
    public GsaSectionModifierGoo()
    {
      this.Value = new GsaSectionModifier();
    }
    public GsaSectionModifierGoo(GsaSectionModifier offset)
    {
      if (offset == null)
        offset = new GsaSectionModifier();
      this.Value = offset; //offset.Duplicate();
    }

    public override IGH_Goo Duplicate()
    {
      return DuplicateGsaOffset();
    }
    public GsaSectionModifierGoo DuplicateGsaOffset()
    {
      return new GsaSectionModifierGoo(Value == null ? new GsaSectionModifier() : Value); //Value.Duplicate());
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
        return "Null GSA Section Modifier";
      else
        return Value.ToString();
    }
    public override string TypeName
    {
      get { return ("GSA Section Modifier"); }
    }
    public override string TypeDescription
    {
      get { return ("GSA Section Modifier"); }
    }


    #endregion

    #region casting methods
    public override bool CastTo<Q>(ref Q target)
    {
      // This function is called when Grasshopper needs to convert this 
      // instance of GsaOffset into some other type Q.            


      if (typeof(Q).IsAssignableFrom(typeof(GsaSectionModifier)))
      {
        if (Value == null)
          target = default;
        else
          target = (Q)(object)Value.Duplicate();
        return true;
      }

      if (typeof(Q).IsAssignableFrom(typeof(SectionModifier)))
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
      // This function is called when Grasshopper needs to convert other data 
      // into GsaOffset.


      if (source == null) { return false; }

      //Cast from other Section Modifier
      if (typeof(GsaSectionModifier).IsAssignableFrom(source.GetType()))
      {
        Value = (GsaSectionModifier)source;
        return true;
      }

      return false;
    }
    #endregion


  }

  /// <summary>
  /// This class provides a Parameter interface for the Data_GsaOffset type.
  /// </summary>
  public class GsaSectionModifierParameter : GH_PersistentParam<GsaSectionModifierGoo>
  {
    public GsaSectionModifierParameter()
      : base(new GH_InstanceDescription("Section Modifier", "SecMod", "GSA Section Modifier", GsaGH.Components.Ribbon.CategoryName.Name(), GsaGH.Components.Ribbon.SubCategoryName.Cat9()))
    {
    }

    public override Guid ComponentGuid => new Guid("19b3bec4-e021-493e-a847-cd30476b5322");

    public override GH_Exposure Exposure => GH_Exposure.secondary | GH_Exposure.obscure;

    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.SectionModifierParam;

    //We do not allow users to pick parameter, 
    //therefore the following 4 methods disable all this ui.
    protected override GH_GetterResult Prompt_Plural(ref List<GsaSectionModifierGoo> values)
    {
      return GH_GetterResult.cancel;
    }
    protected override GH_GetterResult Prompt_Singular(ref GsaSectionModifierGoo value)
    {
      return GH_GetterResult.cancel;
    }
    protected override System.Windows.Forms.ToolStripMenuItem Menu_CustomSingleValueItem()
    {
      System.Windows.Forms.ToolStripMenuItem item = new System.Windows.Forms.ToolStripMenuItem
      {
        Text = "Not available",
        Visible = false
      };
      return item;
    }
    protected override System.Windows.Forms.ToolStripMenuItem Menu_CustomMultiValueItem()
    {
      System.Windows.Forms.ToolStripMenuItem item = new System.Windows.Forms.ToolStripMenuItem
      {
        Text = "Not available",
        Visible = false
      };
      return item;
    }

    #region preview methods

    public bool Hidden
    {
      get { return true; }
      //set { m_hidden = value; }
    }
    public bool IsPreviewCapable
    {
      get { return false; }
    }
    #endregion
  }

}
