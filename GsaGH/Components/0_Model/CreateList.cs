using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using OasysGH;
using OasysGH.Components;

namespace GsaGH.Components
{
  /// <summary>
  /// Component to edit a Node
  /// </summary>
  public class CreateList : GH_OasysDropDownComponent, IGH_PreviewObject
  {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("5fec976c-14d7-438e-a8ba-ac97042d0477");
    public override GH_Exposure Exposure => GH_Exposure.tertiary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.CreateList;

    public CreateList() : base("Create List",
      "CreateList",
      "Create a GSA List with Name, Type and Definition or reference objects (Nodes, Elements, Members)",
      CategoryName.Name(),
      SubCategoryName.Cat0())
    { }
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddIntegerParameter("Index", "ID", "(Optional) List Number - set this to 0 to append it to the end of the list of lists, or set the ID to overwrite an existing list.", GH_ParamAccess.item, 0);
      pManager.AddTextParameter("Name", "Na", "(Optional) List Name", GH_ParamAccess.item);
      pManager.AddGenericParameter("Definition", "Def", "Definition as text or list of object (Nodes, Elements, Members)", GH_ParamAccess.list);
      pManager[0].Optional = true;
      pManager[1].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddParameter(new GsaListParameter());
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      GsaList list = new GsaList() { EntityType = this.Type };
      int id = 0;
      if (DA.GetData(0, ref id))
        list.Id = id;

      string name = this.Type.ToString() + " List";
      if (DA.GetData(1, ref name))
        list.Name = name;

      List<object> listObjects = Inputs.GetObjectsForLists(this, DA, 2, this.Type);

      list.SetListObjects(listObjects);

      DA.SetData(0, list);
    }

    #region Custom UI
    private EntityType Type = EntityType.Node;

    public override void InitialiseDropdowns()
    {
      this.SpacerDescriptions = new List<string>(new string[]
        {
          "Type"
        });

      this.DropDownItems = new List<List<string>>();
      this.SelectedItems = new List<string>();

      // Length
      this.DropDownItems.Add(new List<string>() {
        EntityType.Node.ToString(),
        EntityType.Element.ToString(),
        EntityType.Member.ToString(),
        EntityType.Case.ToString() });
      this.SelectedItems.Add(this.DropDownItems[0][0]);

      this.IsInitialised = true;
    }

    public override void SetSelected(int i, int j)
    {
      this.SelectedItems[i] = this.DropDownItems[i][j];
      this.Type = (EntityType)Enum.Parse(typeof(EntityType), this.SelectedItems[i]);
      base.UpdateUI();
    }
    #endregion
  }
}
