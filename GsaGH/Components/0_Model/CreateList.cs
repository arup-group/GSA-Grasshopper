using System;
using System.Collections.Generic;
using System.Drawing;

using Grasshopper.Kernel;

using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;

using OasysGH;
using OasysGH.Components;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to create a new EntityList
  /// </summary>
  public class CreateList : GH_OasysDropDownComponent, IGH_PreviewObject {
    public override Guid ComponentGuid => new Guid("5fec976c-14d7-438e-a8ba-ac97042d0477");
    public override GH_Exposure Exposure => GH_Exposure.tertiary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.CreateList;
    private EntityType _type = EntityType.Node;

    public CreateList() : base("Create List", "CreateList",
      "Create a GSA List with Name, Type and Definition or reference objects (Nodes, Elements, Members)."
      + Environment.NewLine + "You can add a GSA List to a model through the 'GSA' input.",
      CategoryName.Name(), SubCategoryName.Cat0()) { }

    public override void SetSelected(int i, int j) {
      _selectedItems[i] = _dropDownItems[i][j];
      _type = (EntityType)Enum.Parse(typeof(EntityType), _selectedItems[i]);
      base.UpdateUI();
    }

    protected override void InitialiseDropdowns() {
      _spacerDescriptions = new List<string>(new string[] {
        "Type",
      });

      _dropDownItems = new List<List<string>>();
      _selectedItems = new List<string>();

      _dropDownItems.Add(new List<string>() {
        EntityType.Node.ToString(),
        EntityType.Element.ToString(),
        EntityType.Member.ToString(),
        EntityType.Case.ToString(),
      });
      _selectedItems.Add(_dropDownItems[0][0]);

      _isInitialised = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddIntegerParameter("Index", "ID",
        "[Optional] List Number - set this to 0 to append it to the end of the list of lists, or set the ID to overwrite an existing list.",
        GH_ParamAccess.item, 0);
      pManager.AddTextParameter("Name", "Na", "[Optional] List Name", GH_ParamAccess.item);
      pManager.AddGenericParameter("Definition", "Def",
        "Definition as text or list of object (Nodes, Elements, Members)", GH_ParamAccess.list);
      pManager[0].Optional = true;
      pManager[1].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaListParameter(), "GSA List", "L",
        "GSA Entity List parameter." + Environment.NewLine
        + "You can add a GSA List to a model through the 'GSA' input.", GH_ParamAccess.item);
    }

    protected override void SolveInternal(IGH_DataAccess DA) {
      var list = new GsaList() {
        EntityType = _type,
      };
      int id = 0;
      if (DA.GetData(0, ref id)) {
        list.Id = id;
      }

      string name = _type.ToString() + " List";
      if (DA.GetData(1, ref name)) {
        list.Name = name;
      }

      List<object> listGooObjects = Inputs.GetGooObjectsForLists(this, DA, 2, _type);

      try {
        list.SetListGooObjects(listGooObjects);
      } catch (ArgumentException) {
        string message = string.Empty;
        switch (_type) {
          case EntityType.Node:
            message
              = "Invalid node list\n\nThe node list should take the form:\n 1 11 to 72 step 2 not (XY3 31 to 45)\nwhere:\nPS(n)  ->  Springs (of property n)\nPM(n)  ->  Masses (of property n)\nPD(n)  ->  Dampers (of property n)\nXn  ->  Nodes on global X line through node n\nYn  ->  ditto for Y\nZn  ->  ditto for Z\nXYn  ->  Nodes on global XY plane through node n\nYZn  ->  ditto for YZ\nZXn  ->  ditto for ZX\n\n* may be used in place of a node number to refer to\nthe highest numbered node.";
            break;

          case EntityType.Element:
            message
              = "Invalid element list\n\nThe element list should take the form:\n 1 11 to 20 step 2 P1 not (G1 to G6 step 3) P11 not (PA PB1 PS2 PM3 PA4 M1)\nwhere:\nGn  ->  Elements in group n\nPn  ->  Elements of property n\nPB(n)  ->  Beams, bars, rods, struts and ties (of property n)\nPS(n)  ->  Springs (of property n)\nPA(n)  ->  2D elements (of property n)\nPV(n)  ->  3D elements (of property n)\nPL(n)  ->  Links (of property n)\nPC(n)  ->  Cables (of property n)\nPG(n)  ->  Spacers (of property n)\nPD(n)  ->  Dampers (of property n)\nM(n)  ->  Elements (of analysis material n)\nMS(n)  ->  Steel elements (of grade n)\nMC(n)  ->  Concrete elements (of grade n)\nMP(n)  ->  FRP elements (of grade n)\nXn  ->  Elements on global X line through node n\nYn  ->  ditto for Y\nZn  ->  ditto for Z\nXYn  ->  Elements on global XY plane through node n\nYZn  ->  ditto for YZ\nZXn  ->  ditto for ZX\n\n* may be used in place of an element or property number\nto refer to the highest numbered element or property.\n\nElements included in assemblies & grid surfaces may be referred to by:\n\"name\"  ->  where name is the name of the assembly or grid surface.";
            break;

          case EntityType.Member:
            message
              = "Invalid member list\n\nThe member list should take the form:\n 1 11 to 20 step 2 P1 not (G1 to G6 step 3) P11 not (Z4 XY55)\nwhere:\nGn  ->  Members in group n\nPn  ->  Members of property n\nPB(n)  ->  1D beam, bar, rod, strut and tie members (of property n)\nPA(n)  ->  2D members (of property n)n\nM(n)  ->  Members (of analysis material n)\nMS(n)  ->  Steel members (of grade n)\nMC(n)  ->  Concrete members (of grade n)\nMP(n)  ->  FRP members (of grade n)\nXn  ->  Members on global X line through node n\nYn  ->  ditto for Y\nZn  ->  ditto for Z\nXYn  ->  Members on global XY plane passing through node n\nYZn  ->  ditto for YZ\nZXn  ->  ditto for ZX\n\n* may be used in place of a member or property number\nto refer to the highest numbered member or property.";
            break;

          case EntityType.Case:
          case EntityType.Undefined:
            message
              = "Invalid list\n\nThe list should take the form:\n 1 11 to 72 step 2 not (31 to 45 step 3)";
            break;
        }

        this.AddRuntimeWarning(message);
        return;
      }

      DA.SetData(0, new GsaListGoo(list));
    }

    protected override void UpdateUIFromSelectedItems() {
      _type = (EntityType)Enum.Parse(typeof(EntityType), _selectedItems[0]);
      base.UpdateUIFromSelectedItems();
    }
  }
}
