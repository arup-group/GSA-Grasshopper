using System;
using System.Drawing;
using System.Windows.Forms;

using GH_IO.Serialization;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;

using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;

using OasysGH;
using OasysGH.Components;

using Rhino.Geometry;

namespace GsaGH.Components {
  /// <summary>
  ///   Component to edit a Node
  /// </summary>
  public class EditNode : GH_OasysComponent, IGH_VariableParameterComponent {
    private enum FoldMode {
      GetConnected,
      DoNotGetConnected,
    }

    public override Guid ComponentGuid => new Guid("418e222d-16b8-4a8e-bb3d-98ad72b913d8");
    public override GH_Exposure Exposure => GH_Exposure.primary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.EditNode;
    private FoldMode _mode = FoldMode.DoNotGetConnected;

    public EditNode() : base("Edit Node", "NodeEdit", "Modify GSA Node", CategoryName.Name(),
      SubCategoryName.Cat2()) { }

    public bool CanInsertParameter(GH_ParameterSide side, int index) {
      return false;
    }

    public bool CanRemoveParameter(GH_ParameterSide side, int index) {
      return false;
    }

    public IGH_Param CreateParameter(GH_ParameterSide side, int index) {
      return null;
    }

    public bool DestroyParameter(GH_ParameterSide side, int index) {
      return false;
    }

    public override bool Read(GH_IReader reader) {
      _mode = (FoldMode)reader.GetInt32("Mode");
      bool flag = base.Read(reader);
      if (Params.Input[7].Description == "Set Spring Property by reference") {
        Params.ReplaceInputParameter(new GsaSpringPropertyParameter(), 7, true);
        Params.ReplaceOutputParameter(new GsaSpringPropertyParameter(), 7);
      }
      Params.UpdateRestrainedBool6Parameter();
      return flag;
    }

    public void VariableParameterMaintenance() {
      if (_mode != FoldMode.GetConnected) {
        return;
      }

      Params.Output[10].NickName = "El";
      Params.Output[10].Name = "Connected Elements";
      Params.Output[10].Description = "Connected Element IDs in Model that Node once belonged to";
      Params.Output[10].Access = GH_ParamAccess.list;

      Params.Output[11].NickName = "Me";
      Params.Output[11].Name = "Connected Members";
      Params.Output[11].Description = "Connected Member IDs in Model that Node once belonged to";
      Params.Output[11].Access = GH_ParamAccess.list;
    }

    public override bool Write(GH_IWriter writer) {
      writer.SetInt32("Mode", (int)_mode);
      return base.Write(writer);
    }

    protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu) {
      Menu_AppendItem(menu, "Try get connected Element & Members", FlipMode, true,
        _mode == FoldMode.GetConnected);
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaNodeParameter(), GsaNodeGoo.Name, GsaNodeGoo.NickName,
        GsaNodeGoo.Description + " to get or set information for. Leave blank to create a new "
        + GsaNodeGoo.Name, GH_ParamAccess.item);
      pManager.AddIntegerParameter("Node number", "ID",
        "Set Node number (ID) - if Node ID is set it will replace any existing nodes in the model",
        GH_ParamAccess.item);
      pManager.AddPointParameter("Node Position", "Pt", "Set new Position (x, y, z) of Node",
        GH_ParamAccess.item);
      pManager.AddPlaneParameter("Node local axis", "Pl", "Set Local axis (Plane) of Node",
        GH_ParamAccess.item);
      pManager.AddParameter(new GsaRestraintParameter(), "Node Restraints", "B6",
        "Set Restraints (Bool6) of Node", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Damper Property", "DP", "Set Damper Property by reference",
        GH_ParamAccess.item);
      pManager.AddIntegerParameter("Mass Property", "MP", "Set Mass Property by reference",
        GH_ParamAccess.item);
      pManager.AddParameter(new GsaSpringPropertyParameter());
      pManager.AddTextParameter("Node Name", "Na", "Set Name of Node", GH_ParamAccess.item);
      pManager.AddColourParameter("Node Colour", "Co", "Set colour of node", GH_ParamAccess.item);

      for (int i = 0; i < pManager.ParamCount; i++) {
        pManager[i].Optional = true;
      }

      pManager.HideParameter(0);
      pManager.HideParameter(2);
      pManager.HideParameter(3);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaNodeParameter(), GsaNodeGoo.Name, GsaNodeGoo.NickName,
        GsaNodeGoo.Description + " with applied changes.", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Node number", "ID",
        "Original Node number (ID) if Node ever belonged to a GSA Model", GH_ParamAccess.item);
      pManager.AddPointParameter("Node Position", "Pt",
        "Position (x, y, z) of Node. Setting a new position will clear any existing ID",
        GH_ParamAccess.item);
      pManager.HideParameter(2);
      pManager.AddPlaneParameter("Node local axis", "Pl", "Local axis (Plane) of Node",
        GH_ParamAccess.item);
      pManager.HideParameter(3);
      pManager.AddParameter(new GsaRestraintParameter(), "Node Restraints", "B6",
        "Restraints (Bool6) of Node", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Damper Property", "DP", "Get Damper Property reference",
        GH_ParamAccess.item);
      pManager.AddIntegerParameter("Mass Property", "MP", "Get Mass Property reference",
        GH_ParamAccess.item);
      pManager.AddParameter(new GsaSpringPropertyParameter());
      pManager.AddTextParameter("Node Name", "Na", "Name of Node", GH_ParamAccess.item);
      pManager.AddColourParameter("Node Colour", "Co", "Get colour of node", GH_ParamAccess.item);
      if (_mode != FoldMode.GetConnected) {
        return;
      }

      pManager.AddIntegerParameter("Connected Elements", "El",
        "Connected Element IDs in Model that Node once belonged to", GH_ParamAccess.list);
      pManager.AddIntegerParameter("Connected Members", "Me",
        "Connected Member IDs in Model that Node once belonged to", GH_ParamAccess.list);
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      var node = new GsaNode();

      GsaNodeGoo nodeGoo = null;
      if (da.GetData(0, ref nodeGoo)) {
        node = new GsaNode(nodeGoo.Value);
      } else {
        node.Point = new Point3d(0, 0, 0);
        if (Params.Input[2].SourceCount == 0) {
          this.AddRuntimeRemark("New node created at {0, 0, 0}");
        }
      }

      GH_Point ghPt = null;
      if (da.GetData(2, ref ghPt)) {
        node.Point = ghPt.Value;
      }

      // 1 ID (do ID after point, as setting point will clear the Node.ID value
      int id = 0;
      if (da.GetData(1, ref id)) {
        node.Id = id;
      }

      GH_Plane ghPln = null;
      if (da.GetData(3, ref ghPln)) {
        node.LocalAxis = ghPln.Value;
      }

      GsaBool6Goo restraintGoo = null;
      if (da.GetData(4, ref restraintGoo)) {
        node.Restraint = restraintGoo.Value;
      }

      int damperId = 0;
      if (da.GetData(5, ref damperId)) {
        node.ApiNode.DamperProperty = damperId;
      }

      int massId = 0;
      if (da.GetData(6, ref massId)) {
        node.ApiNode.MassProperty = massId;
      }

      GsaSpringPropertyGoo springGoo = null;
      if (da.GetData(7, ref springGoo)) {
        node.SpringProperty = springGoo.Value;
      }

      string name = string.Empty;
      if (da.GetData(8, ref name)) {
        node.ApiNode.Name = name;
      }

      Color colour = Color.Empty;
      if (da.GetData(9, ref colour)) {
        node.ApiNode.Colour = colour;
      }

      da.SetData(0, new GsaNodeGoo(node));
      da.SetData(1, node.Id);
      da.SetData(2, node.Point);
      da.SetData(3, new GH_Plane(node.LocalAxis));
      da.SetData(4, new GsaBool6Goo(node.Restraint));
      da.SetData(5, node.ApiNode.DamperProperty);
      da.SetData(6, node.ApiNode.MassProperty);
      da.SetData(7, new GsaSpringPropertyGoo(node.SpringProperty));
      da.SetData(8, node.ApiNode?.Name);
      da.SetData(9, node.ApiNode.Colour);

      // only get connected elements/members if enabled (computationally expensive)
      if (_mode != FoldMode.GetConnected) {
        return;
      }

      try {
        da.SetDataList(10, node.ApiNode?.ConnectedElements);
      } catch (Exception) {
        // ignored
      }

      try {
        da.SetDataList(11, node.ApiNode?.ConnectedMembers);
      } catch (Exception) {
        // ignored
      }
    }

    internal void FlipMode(object sender, EventArgs e) {
      RecordUndoEvent("GetConnected Parameters");
      if (_mode == FoldMode.GetConnected) {
        _mode = FoldMode.DoNotGetConnected;

        while (Params.Output.Count > 10) {
          Params.UnregisterOutputParameter(Params.Output[10], true);
        }
      } else {
        _mode = FoldMode.GetConnected;

        Params.RegisterOutputParam(new Param_Integer());
        Params.RegisterOutputParam(new Param_Integer());
        (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      }

      Params.OnParametersChanged();
      ExpireSolution(true);
    }
  }
}
