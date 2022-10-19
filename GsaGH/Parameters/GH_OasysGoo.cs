using Grasshopper.Kernel.Types;
using OasysGH;

namespace GsaGH.Parameters
{
  public abstract class GH_OasysGoo<T> : GH_Goo<T>
  {
    abstract public OasysPluginInfo PluginInfo { get; }

    public override string TypeName => typeof(T).Name.TrimStart('I').Replace("Gsa", string.Empty).Replace("AdSec", string.Empty);
    public override string TypeDescription => PluginInfo.ProductName + " " + this.TypeName + " Parameter";
    public override bool IsValid => (this.Value == null) ? false : true;
    public override string IsValidWhyNot
    {
      get
      {
        if (IsValid)
          return string.Empty;
        else
          return IsValid.ToString();
      }
    }

    public GH_OasysGoo(T item)
    {
      if (item == null)
        this.Value = item;
      else
        this.Value = (T)item.Duplicate();
    }

    public override string ToString()
    {
      if (Value == null)
        return "Null";
      else
        return PluginInfo.ProductName + " " + TypeName + " (" + Value.ToString() + ")";
    }

    #region casting methods
    public override bool CastTo<Q>(ref Q target)
    {
      // This function is called when Grasshopper needs to convert this 
      // instance of our custom class into some other type Q.            

      if (typeof(Q).IsAssignableFrom(typeof(T)))
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
      // into our custom class.

      if (source == null)
        return false;

      //Cast from this type
      if (typeof(T).IsAssignableFrom(source.GetType()))
      {
        Value = (T)source;
        return true;
      }

      return false;
    }
    #endregion
  }
}
