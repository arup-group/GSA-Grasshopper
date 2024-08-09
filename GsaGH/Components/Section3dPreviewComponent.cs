using System.Windows.Forms;

using GH_IO.Serialization;

using OasysGH.Components;

namespace GsaGH.Components {
  public abstract class Section3dPreviewComponent : GH_OasysComponent {
    protected internal bool Preview3dSection = false;
    public Section3dPreviewComponent(string name, string nickname, string description, string category, string subCategory)
     : base(name, nickname, description, category, subCategory) {
    }

    public override void AppendAdditionalMenuItems(ToolStripDropDown menu) {
      if (!(menu is ContextMenuStrip)) {
        return; // this method is also called when clicking EWR balloon
      }

      Menu_AppendSeparator(menu);
      Menu_AppendItem(menu, "Preview 3D Sections", (s, a) => TogglePreview(), true, Preview3dSection);
    }

    private void TogglePreview() {
      Preview3dSection = !Preview3dSection;
      ExpireSolution(true);
    }

    public override bool Read(GH_IReader reader) {
      if (reader.ItemExists("Preview3dSection")) {
        Preview3dSection = reader.GetBoolean("Preview3dSection");
      }

      return base.Read(reader);
    }

    public override bool Write(GH_IWriter writer) {
      writer.SetBoolean("Preview3dSection", Preview3dSection);
      return base.Write(writer);
    }
  }
}
