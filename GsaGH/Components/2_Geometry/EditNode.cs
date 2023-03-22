using System;
using System.Windows.Forms;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using OasysGH;
using OasysGH.Components;
using Rhino.Geometry;

namespace GsaGH.Components {
  /// <summary>
  /// Component to edit a Node
  /// </summary>
  public class EditNode : GH_OasysComponent, IGH_VariableParameterComponent {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("418e222d-16b8-4a8e-bb3d-98ad72b913d8");
    public override GH_Exposure Exposure => GH_Exposure.secondary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => Properties.Resources.EditNode;

    public EditNode() : base("Edit Node",
      "NodeEdit",
      "Modify GSA Node",
      CategoryName.Name(),
      SubCategoryName.Cat2()) { }
    #endregion

    #region Input and output

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaNodeParameter(), GsaNodeGoo.Name, GsaNodeGoo.NickName, GsaNodeGoo.Description + " to get or set information for. Leave blank to create a new " + GsaNodeGoo.Name, GH_ParamAccess.item);
      pManager.AddIntegerParameter("Node number", "ID", "Set Node number (ID) - if Node ID is set it will replace any existing nodes in the model", GH_ParamAccess.item);
      pManager.AddPointParameter("Node Position", "Pt", "Set new Position (x, y, z) of Node", GH_ParamAccess.item);
      pManager.AddPlaneParameter("Node local axis", "Pl", "Set Local axis (Plane) of Node", GH_ParamAccess.item);
      pManager.AddParameter(new GsaBool6Parameter(), "Node Restraints", "B6", "Set Restraints (Bool6) of Node", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Damper Property", "DP", "Set Damper Property by reference", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Mass Property", "MP", "Set Mass Property by reference", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Spring Property", "SP", "Set Spring Property by reference", GH_ParamAccess.item);
      pManager.AddTextParameter("Node Name", "Na", "Set Name of Node", GH_ParamAccess.item);
      pManager.AddColourParameter("Node Colour", "Co", "Set colour of node", GH_ParamAccess.item);

      for (int i = 0; i < pManager.ParamCount; i++)
        pManager[i].Optional = true;

      pManager.HideParameter(0);
      pManager.HideParameter(2);
      pManager.HideParameter(3);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaNodeParameter(), GsaNodeGoo.Name, GsaNodeGoo.NickName, GsaNodeGoo.Description + " with applied changes.", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Node number", "ID", "Original Node number (ID) if Node ever belonged to a GSA Model", GH_ParamAccess.item);
      pManager.AddPointParameter("Node Position", "Pt", "Position (x, y, z) of Node. Setting a new position will clear any existing ID", GH_ParamAccess.item);
      pManager.HideParameter(2);
      pManager.AddPlaneParameter("Node local axis", "Pl", "Local axis (Plane) of Node", GH_ParamAccess.item);
      pManager.HideParameter(3);
      pManager.AddParameter(new GsaBool6Parameter(), "Node Restraints", "B6", "Restraints (Bool6) of Node", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Damper Property", "DP", "Get Damper Property reference", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Mass Property", "MP", "Get Mass Property reference", GH_ParamAccess.item);
      pManager.AddIntegerParameter("Spring Property", "SP", "Get Spring Property reference", GH_ParamAccess.item);
      pManager.AddTextParameter("Node Name", "Na", "Name of Node", GH_ParamAccess.item);
      pManager.AddColourParameter("Node Colour", "Co", "Get colour of node", GH_ParamAccess.item);
      if (_mode != FoldMode.GetConnected) {
        return;
      }

      pManager.AddIntegerParameter("Connected Elements", "El", "Connected Element IDs in Model that Node once belonged to", GH_ParamAccess.list);
      pManager.AddIntegerParameter("Connected Members", "Me", "Connected Member IDs in Model that Node once belonged to", GH_ParamAccess.list);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess da) {
      var node = new GsaNode();
      var ghTyp = new GH_ObjectWrapper();
      if (da.GetData(0, ref ghTyp)) {
        var tempPt = new Point3d();
        if (ghTyp.Value is GsaNodeGoo) {
          ghTyp.CastTo(ref node);
          if (node == null) {
            this.AddRuntimeError("Node input is null");
          }
          if (node.ApiNode == null) { this.AddRuntimeError("Node input is null"); }
        }
        else if (GH_Convert.ToPoint3d(ghTyp.Value, ref tempPt, GH_Conversion.Both)) {
          node.Point = tempPt;
        }
        else {
          this.AddRuntimeError("Unable to convert input to Node");
          return;
        }
      }
      else {
        node.Point = new Point3d(0, 0, 0);
        if (Params.Input[2].SourceCount == 0)
          this.AddRuntimeRemark("New node created at {0, 0, 0}");
      }

      var ghPt = new GH_Point();
      if (da.GetData(2, ref ghPt)) {
        var pt = new Point3d();
        if (GH_Convert.ToPoint3d(ghPt, ref pt, GH_Conversion.Both)) {
          node.Point = pt;
        }
      }

      // 1 ID (do ID after point, as setting point will clear the Node.ID value
      var ghInt = new GH_Integer();
      if (da.GetData(1, ref ghInt)) {
        if (GH_Convert.ToInt32(ghInt, out int id, GH_Conversion.Both))
          node.Id = id;
      }

      var ghPln = new GH_Plane();
      if (da.GetData(3, ref ghPln)) {
        var pln = new Plane();
        if (GH_Convert.ToPlane(ghPln, ref pln, GH_Conversion.Both)) {
          node.LocalAxis = pln;
        }
      }

      var restraint = new GsaBool6();
      if (da.GetData(4, ref restraint)) {
        node.Restraint = restraint;
      }

      ghInt = new GH_Integer();
      if (da.GetData(5, ref ghInt)) {
        if (GH_Convert.ToInt32(ghInt, out int prop, GH_Conversion.Both))
          node.DamperProperty = prop;
      }

      ghInt = new GH_Integer();
      if (da.GetData(6, ref ghInt)) {
        if (GH_Convert.ToInt32(ghInt, out int prop, GH_Conversion.Both))
          node.MassProperty = prop;
      }

      ghInt = new GH_Integer();
      if (da.GetData(7, ref ghInt)) {
        if (GH_Convert.ToInt32(ghInt, out int prop, GH_Conversion.Both))
          node.SpringProperty = prop;
      }

      var ghStr = new GH_String();
      if (da.GetData(8, ref ghStr)) {
        if (GH_Convert.ToString(ghStr, out string name, GH_Conversion.Both))
          node.Name = name;
      }

      var ghcol = new GH_Colour();
      if (da.GetData(9, ref ghcol)) {
        if (GH_Convert.ToColor(ghcol, out System.Drawing.Color col, GH_Conversion.Both))
          node.Colour = col;
      }

      da.SetData(0, new GsaNodeGoo(node));
      da.SetData(1, node.Id);
      da.SetData(2, node.Point);
      da.SetData(3, new GH_Plane(node.LocalAxis));
      da.SetData(4, new GsaBool6Goo(node.Restraint));
      da.SetData(5, node.DamperProperty);
      da.SetData(6, node.MassProperty);
      da.SetData(7, node.SpringProperty);
      da.SetData(8, node.ApiNode?.Name);
      da.SetData(9, node.Colour);

      // only get connected elements/members if enabled (computationally expensive)
      if (_mode != FoldMode.GetConnected) {
        return;
      }

      try {
        da.SetDataList(10, node.ApiNode?.ConnectedElements);
      }
      catch (Exception) {
        // ignored
      }

      try {
        da.SetDataList(11, node.ApiNode?.ConnectedMembers);
      }
      catch (Exception) {
        // ignored
      }
    }

    #region menu override
    private enum FoldMode {
      GetConnected,
      DoNotGetConnected
    }

    private FoldMode _mode = FoldMode.DoNotGetConnected;

    protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu) {
      Menu_AppendItem(menu, "Try get connected Element & Members", FlipMode, true, _mode == FoldMode.GetConnected);
    }

    private void FlipMode(object sender, EventArgs e) {
      RecordUndoEvent("GetConnected Parameters");
      if (_mode == FoldMode.GetConnected) {
        _mode = FoldMode.DoNotGetConnected;

        while (Params.Output.Count > 10)
          Params.UnregisterOutputParameter(Params.Output[10], true);
      }
      else {
        _mode = FoldMode.GetConnected;

        Params.RegisterOutputParam(new Param_Integer());
        Params.RegisterOutputParam(new Param_Integer());
        (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      }

      Params.OnParametersChanged();
      ExpireSolution(true);
    }

    #endregion

    #region (de)serialization
    public override bool Write(GH_IO.Serialization.GH_IWriter writer) {
      writer.SetInt32("Mode", (int)_mode);
      return base.Write(writer);
    }
    public override bool Read(GH_IO.Serialization.GH_IReader reader) {
      _mode = (FoldMode)reader.GetInt32("Mode");
      return base.Read(reader);
    }
    #endregion

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

    #region IGH_variable parameter null implementation
    public bool CanInsertParameter(GH_ParameterSide side, int index) => false;
    public bool CanRemoveParameter(GH_ParameterSide side, int index) => false;
    public IGH_Param CreateParameter(GH_ParameterSide side, int index) => null;
    public bool DestroyParameter(GH_ParameterSide side, int index) => false;
    #endregion
  }
}

