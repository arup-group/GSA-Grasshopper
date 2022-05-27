using System;
using System.Collections.Generic;

using GsaAPI;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

namespace GsaGH.Parameters
{
  /// <summary>
  /// Bool6 class, this class defines the basic properties and methods for any Gsa Bool6
  /// </summary>
  public class GsaBool6
  {
    public bool X
    {
      get { return API_Bool6.X; }
      set { API_Bool6 = new Bool6(value, Y, Z, XX, YY, ZZ); }
    }
    public bool Y
    {
      get { return API_Bool6.Y; }
      set { API_Bool6 = new Bool6(X, value, Z, XX, YY, ZZ); }
    }
    public bool Z
    {
      get { return API_Bool6.Z; }
      set { API_Bool6 = new Bool6(X, Y, value, XX, YY, ZZ); }
    }
    public bool XX
    {
      get { return API_Bool6.XX; }
      set { API_Bool6 = new Bool6(X, Y, Z, value, YY, ZZ); }
    }
    public bool YY
    {
      get { return API_Bool6.YY; }
      set { API_Bool6 = new Bool6(X, Y, Z, XX, value, ZZ); }
    }
    public bool ZZ
    {
      get { return API_Bool6.ZZ; }
      set { API_Bool6 = new Bool6(X, Y, Z, XX, YY, value); }
    }

    #region fields
    internal Bool6 API_Bool6;
    #endregion

    #region constructors
    public GsaBool6()
    {
      API_Bool6 = new Bool6(false, false, false, false, false, false);
    }

    public GsaBool6(bool X, bool Y, bool Z, bool XX, bool YY, bool ZZ)
    {
      API_Bool6 = new Bool6(X, Y, Z, XX, YY, ZZ);
    }
    internal GsaBool6(Bool6 bool6)
    {
      API_Bool6 = bool6;
    }

    public GsaBool6 Duplicate()
    {
      if (this == null) { return null; }
      // create shallow copy
      GsaBool6 dup = new GsaBool6
      {
        API_Bool6 = this.API_Bool6
      };
      return dup;
    }
    #endregion

    #region properties
    public bool IsValid
    {
      get
      {
        return true;
      }
    }


    #endregion

    #region methods
    public override string ToString()
    {
      string sx = (X) ? "\u2713" : "\u2610";
      sx = "{X" + sx;
      string sy = (Y) ? "\u2713" : "\u2610";
      sy = ", Y" + sy;
      string sz = (Z) ? "\u2713" : "\u2610";
      sz = ", Z" + sz;
      string sxx = (XX) ? "\u2713" : "\u2610";
      sxx = ", XX" + sxx;
      string syy = (YY) ? "\u2713" : "\u2610";
      syy = ", YY" + syy;
      string szz = (ZZ) ? "\u2713" : "\u2610";
      szz = ", ZZ" + szz + "}";

      return "GSA Bool 6" + sx + sy + sz + sxx + syy + szz;
    }

    #endregion
  }

  /// <summary>
  /// GsaBool6 Goo wrapper class, makes sure GsaBool6 can be used in Grasshopper.
  /// </summary>
  public class GsaBool6Goo : GH_Goo<GsaBool6>
  {
    #region constructors
    public GsaBool6Goo()
    {
      this.Value = new GsaBool6();
    }
    public GsaBool6Goo(GsaBool6 bool6)
    {
      if (bool6 == null)
        bool6 = new GsaBool6();
      this.Value = bool6; //bool6.Duplicate();
    }

    public override IGH_Goo Duplicate()
    {
      return DuplicateGsaBool6();
    }
    public GsaBool6Goo DuplicateGsaBool6()
    {
      return new GsaBool6Goo(Value == null ? new GsaBool6() : Value); //Value.Duplicate());
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
        //if (Value == null) { return "No internal GsaMember instance"; }
        if (Value.IsValid) { return string.Empty; }
        return Value.IsValid.ToString(); //Todo: beef this up to be more informative.
      }
    }
    public override string ToString()
    {
      if (Value == null)
        return "Null GSA Bool6";
      else
        return Value.ToString();
    }
    public override string TypeName
    {
      get { return ("GSA Bool6"); }
    }
    public override string TypeDescription
    {
      get { return ("GSA Bool6 to set releases and restraints"); }
    }


    #endregion

    #region casting methods
    public override bool CastTo<Q>(ref Q target)
    {
      // This function is called when Grasshopper needs to convert this 
      // instance of GsaBool6 into some other type Q.            


      if (typeof(Q).IsAssignableFrom(typeof(GsaBool6)))
      {
        if (Value == null)
          target = default;
        else
          target = (Q)(object)Value.Duplicate();
        return true;
      }

      if (typeof(Q).IsAssignableFrom(typeof(Bool6)))
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
      // into GsaBool6.


      if (source == null) { return false; }

      //Cast from GsaBool6
      if (typeof(GsaBool6).IsAssignableFrom(source.GetType()))
      {
        Value = (GsaBool6)source;
        return true;
      }


      //Cast from Bool
      if (GH_Convert.ToBoolean(source, out bool mybool, GH_Conversion.Both))
      {
        Value.X = mybool;
        Value.Y = mybool;
        Value.Z = mybool;
        Value.XX = mybool;
        Value.YY = mybool;
        Value.ZZ = mybool;
        return true;
      }

      //Cast from string
      if (GH_Convert.ToString(source, out string mystring, GH_Conversion.Both))
      {
        mystring = mystring.Trim();
        mystring = mystring.ToLower();

        if (mystring == "free")
        {
          Value.X = false;
          Value.Y = false;
          Value.Z = false;
          Value.XX = false;
          Value.YY = false;
          Value.ZZ = false;
          return true;
        }
        if (mystring == "pin" | mystring == "pinned")
        {
          Value.X = true;
          Value.Y = true;
          Value.Z = true;
          Value.XX = false;
          Value.YY = false;
          Value.ZZ = false;
          return true;
        }
        if (mystring == "fix" | mystring == "fixed")
        {
          Value.X = true;
          Value.Y = true;
          Value.Z = true;
          Value.XX = true;
          Value.YY = true;
          Value.ZZ = true;
          return true;
        }
        if (mystring == "release" | mystring == "released" | mystring == "hinge" | mystring == "hinged" | mystring == "charnier")
        {
          Value.X = false;
          Value.Y = false;
          Value.Z = false;
          Value.XX = false;
          Value.YY = true;
          Value.ZZ = true;
          return true;
        }
        return false;
      }
      return false;
    }
    #endregion


  }

  /// <summary>
  /// This class provides a Parameter interface for the Data_GsaBool6 type.
  /// </summary>
  public class GsaBool6Parameter : GH_PersistentParam<GsaBool6Goo>
  {
    public GsaBool6Parameter()
      : base(new GH_InstanceDescription("Bool6", "B6", "GSA Bool6 to set releases and restraints", GsaGH.Components.Ribbon.CategoryName.Name(), GsaGH.Components.Ribbon.SubCategoryName.Cat9()))
    {
    }

    public override Guid ComponentGuid => new Guid("9bf01532-2035-4105-9c56-5e88b87f5220");

    public override GH_Exposure Exposure => GH_Exposure.secondary | GH_Exposure.obscure;

    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.Bool6Param;

    protected override GH_GetterResult Prompt_Plural(ref List<GsaBool6Goo> values)
    {
      return GH_GetterResult.cancel;
    }
    protected override GH_GetterResult Prompt_Singular(ref GsaBool6Goo value)
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
