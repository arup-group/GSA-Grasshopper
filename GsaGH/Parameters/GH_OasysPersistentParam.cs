using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using OasysGH;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GsaGH.Parameters
{
  public abstract class GH_OasysPersistentParam<T> : GH_PersistentParam<T> where T : class, IGH_Goo
  {
    protected GH_OasysPersistentParam(GH_InstanceDescription nTag) : base(nTag)
    {
    }

    protected override GH_GetterResult Prompt_Plural(ref List<T> values)
    {
      return GH_GetterResult.cancel;
    }
    protected override GH_GetterResult Prompt_Singular(ref T value)
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

    public virtual bool Hidden
    {
      get { return true; }
      set { }
    }

    public virtual bool IsPreviewCapable
    {
      get { return false; }
    }
  }
}
