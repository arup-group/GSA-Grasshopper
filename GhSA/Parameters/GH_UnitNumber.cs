using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

namespace UnitsNet.GH
{
    //public class UnitNumber
    //{
    //    #region fields
    //    public IQuantity Quantity
    //    {
    //        get { return m_value; }
    //        set { m_value = value; }
    //    }
    //    private IQuantity m_value;
    //    #endregion
    //    #region constructors
    //    public UnitNumber()
    //    {
    //        this.m_value = null;
    //    }
    //    public UnitNumber(IQuantity quantity)
    //    {
    //        this.m_value = quantity;
    //    }
    //    #endregion
    //    #region properties
    //    public bool IsValid
    //    {
    //        get
    //        {
    //            if (this.Quantity == null) { return false; }
    //            return true;
    //        }
    //    }
    //    #endregion

    //    #region methods
    //    public override string ToString()
    //    {
    //        return this.Quantity.ToString();
    //    }
    //}

    /// <summary>
    /// Goo wrapper class, makes sure this can be used in Grasshopper.
    /// </summary>
    public class GH_UnitNumber : GH_Goo<IQuantity>
    {
        #region constructors
        public GH_UnitNumber()
        {
            this.Value = null;
        }
        public GH_UnitNumber(IQuantity quantity)
        {
            this.Value = quantity;
        }
        public GH_UnitNumber(GH_UnitNumber unitNumber)
        {
            this.Value = unitNumber.Value;
        }

        public override IGH_Goo Duplicate()
        {
            return new GH_UnitNumber(this.Value);
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
                if (Value == null) { return string.Empty; }
                return Value.ToString();
            }
        }
        public override string ToString()
        {
            if (Value == null)
                return "Null";
            else
            {
                string type = Value.GetType().ToString();
                if (type.StartsWith("Oasys"))
                {
                    string abbr = string.Concat(Value.ToString().Where(char.IsLetter));
                    if (abbr == "")
                    {
                        abbr = Value.ToString();
                        abbr = abbr[abbr.Length - 1].ToString();
                    }
                    if (Value.Value == 0)
                        return 0 + abbr;

                    double scale = Math.Pow(10, Math.Floor(Math.Log10(Math.Abs(Value.Value))) + 1);
                    return scale * Math.Round(Value.Value / scale, 5) + " " + abbr;
                }
                else
                    return Value.ToString();
            }
        }
        public override string TypeName
        {
            get { return ("Unit Number"); }
        }
        public override string TypeDescription
        {
            get { return ("A value with a unit measure"); }
        }
        #endregion

        #region casting methods
        public override bool CastTo<Q>(ref Q target)
        {
            // This function is called when Grasshopper needs to convert this 
            // instance of UnitNumber into some other type Q.            

            if (typeof(Q).IsAssignableFrom(typeof(GH_UnitNumber)))
            {
                target = (Q)(object)new GH_UnitNumber(Value);
                return true;
            }

            if (typeof(Q).IsAssignableFrom(typeof(GH_Number)))
            {
                if (Value == null)
                    target = default;
                else
                    target = (Q)(object)new GH_Number(Value.Value);
                return true;
            }

            target = default;
            return false;
        }
        public override bool CastFrom(object source)
        {
            // This function is called when Grasshopper needs to convert other data 
            // into this parameter.

            if (source == null) { return false; }

            //Cast from own type
            if (typeof(GH_UnitNumber).IsAssignableFrom(source.GetType()))
            {
                GH_UnitNumber num = (GH_UnitNumber)source;
                Value = num.m_value;
                return true;
            }

            return false;
        }
        #endregion
    }

    /// <summary>
    /// This class provides a Parameter interface for the Data_GsaBool6 type.
    /// </summary>
    public class GH_UnitNumberParameter : GH_PersistentParam<GH_UnitNumber>
    {
        public GH_UnitNumberParameter()
          : base(new GH_InstanceDescription("UnitNumber", "UNum", "Quantity = number + unit", GsaGH.Components.Ribbon.CategoryName.Name(), GsaGH.Components.Ribbon.SubCategoryName.Cat9()))
        {
        }

        public override Guid ComponentGuid => new Guid("6368cb74-1c8d-411f-9455-1134a6d9df44");

        public override GH_Exposure Exposure => GH_Exposure.quinary | GH_Exposure.obscure;


        protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.UnitParam;

        protected override GH_GetterResult Prompt_Plural(ref List<GH_UnitNumber> values)
        {
            return GH_GetterResult.cancel;
        }
        protected override GH_GetterResult Prompt_Singular(ref GH_UnitNumber value)
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
