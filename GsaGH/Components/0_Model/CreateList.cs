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
      "Create a GSA List with Name, Type and Definition or reference objects (Nodes, Elements, Members)."
      + System.Environment.NewLine + "You can add a GSA List to a model through the 'GSA' input.",
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
      pManager.AddParameter(new GsaListParameter(), "GSA List", "L", "GSA Entity List parameter."
      + System.Environment.NewLine + "You can add a GSA List to a model through the 'GSA' input.", GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      GsaList list = new GsaList() { EntityType = this._type };
      int id = 0;
      if (DA.GetData(0, ref id))
        list.Id = id;

      string name = this._type.ToString() + " List";
      if (DA.GetData(1, ref name))
        list.Name = name;

      List<object> listObjects = Inputs.GetObjectsForLists(this, DA, 2, this._type);

      try
      {
        list.SetListObjects(listObjects);
      }
      catch (System.ArgumentException)
      {
        string message = "";
        switch (this._type)
        {
          case EntityType.Node:
            message = "Invalid node list\n\nThe node list should take the form:\n 1 11 to 72 step 2 not (XY3 31 to 45)\nwhere:\nPS(n)  ->  Springs (of property n)\nPM(n)  ->  Masses (of property n)\nPD(n)  ->  Dampers (of property n)\nXn  ->  Nodes on global X line through node n\nYn  ->  ditto for Y\nZn  ->  ditto for Z\nXYn  ->  Nodes on global XY plane through node n\nYZn  ->  ditto for YZ\nZXn  ->  ditto for ZX\n\n* may be used in place of a node number to refer to\nthe highest numbered node.";
            break;

          case EntityType.Element:
            message = "Invalid element list\n\nThe element list should take the form:\n 1 11 to 20 step 2 P1 not (G1 to G6 step 3) P11 not (PA PB1 PS2 PM3 PA4 M1)\nwhere:\nGn  ->  Elements in group n\nPn  ->  Elements of property n\nPB(n)  ->  Beams, bars, rods, struts and ties (of property n)\nPS(n)  ->  Springs (of property n)\nPA(n)  ->  2D elements (of property n)\nPV(n)  ->  3D elements (of property n)\nPL(n)  ->  Links (of property n)\nPC(n)  ->  Cables (of property n)\nPG(n)  ->  Spacers (of property n)\nPD(n)  ->  Dampers (of property n)\nM(n)  ->  Elements (of analysis material n)\nMS(n)  ->  Steel elements (of grade n)\nMC(n)  ->  Concrete elements (of grade n)\nMP(n)  ->  FRP elements (of grade n)\nXn  ->  Elements on global X line through node n\nYn  ->  ditto for Y\nZn  ->  ditto for Z\nXYn  ->  Elements on global XY plane through node n\nYZn  ->  ditto for YZ\nZXn  ->  ditto for ZX\n\n* may be used in place of an element or property number\nto refer to the highest numbered element or property.\n\nElements included in assemblies & grid surfaces may be referred to by:\n\"name\"  ->  where name is the name of the assembly or grid surface.";
            break;

          case EntityType.Member:
            message = "Invalid member list\n\nThe member list should take the form:\n 1 11 to 20 step 2 P1 not (G1 to G6 step 3) P11 not (Z4 XY55)\nwhere:\nGn  ->  Members in group n\nPn  ->  Members of property n\nPB(n)  ->  1D beam, bar, rod, strut and tie members (of property n)\nPA(n)  ->  2D members (of property n)n\nM(n)  ->  Members (of analysis material n)\nMS(n)  ->  Steel members (of grade n)\nMC(n)  ->  Concrete members (of grade n)\nMP(n)  ->  FRP members (of grade n)\nXn  ->  Members on global X line through node n\nYn  ->  ditto for Y\nZn  ->  ditto for Z\nXYn  ->  Members on global XY plane passing through node n\nYZn  ->  ditto for YZ\nZXn  ->  ditto for ZX\n\n* may be used in place of a member or property number\nto refer to the highest numbered member or property.";
            break;

          case EntityType.Case:
          case EntityType.Undefined:
            message = "Invalid list\n\nThe list should take the form:\n 1 11 to 72 step 2 not (31 to 45 step 3)";
            break;
        }
        AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, message);
      }

      DA.SetData(0, list);
    }

    #region Custom UI
    private EntityType _type = EntityType.Node;

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
      this._type = (EntityType)Enum.Parse(typeof(EntityType), this.SelectedItems[i]);
      base.UpdateUI();
    }

    public override void UpdateUIFromSelectedItems()
    {
      this._type = (EntityType)Enum.Parse(typeof(EntityType), this.SelectedItems[0]);
      base.UpdateUIFromSelectedItems();
    }
    #endregion
  }
}
