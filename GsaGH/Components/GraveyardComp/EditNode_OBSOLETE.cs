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
  // ReSharper disable once InconsistentNaming
  public class EditNode_OBSOLETE : GH_OasysComponent,
    IGH_VariableParameterComponent {

    #region Enums
    private enum FoldMode {
      GetConnected,
      DoNotGetConnected,
    }
    #endregion Enums

    #region Properties + Fields
    public override Guid ComponentGuid => new Guid("de176ec0-0516-4634-8f04-82017e502e1e");
    public override GH_Exposure Exposure => GH_Exposure.hidden;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.EditNode;
    private FoldMode _mode = FoldMode.DoNotGetConnected;
    #endregion Properties + Fields

    #region Public Constructors
    public EditNode_OBSOLETE() : base("Edit Node",
      "NodeEdit",
      "Modify GSA Node",
      CategoryName.Name(),
      SubCategoryName.Cat2()) { }

    #endregion Public Constructors

    #region Public Methods
    public bool CanInsertParameter(GH_ParameterSide side, int index) => false;

    public bool CanRemoveParameter(GH_ParameterSide side, int index) => false;

    public IGH_Param CreateParameter(GH_ParameterSide side, int index) => null;

    public bool DestroyParameter(GH_ParameterSide side, int index) => false;

    public override bool Read(GH_IReader reader) {
      _mode = (FoldMode)reader.GetInt32("Mode");
      return base.Read(reader);
    }

    public void VariableParameterMaintenance() {
      if (_mode != FoldMode.GetConnected)
        return;

      Params.Output[8]
        .NickName = "El";
      Params.Output[8]
        .Name = "Connected Elements";
      Params.Output[8]
        .Description = "Connected Element IDs in Model that Node once belonged to";
      Params.Output[8]
        .Access = GH_ParamAccess.list;

      Params.Output[9]
        .NickName = "Me";
      Params.Output[9]
        .Name = "Connected Members";
      Params.Output[9]
        .Description = "Connected Member IDs in Model that Node once belonged to";
      Params.Output[9]
        .Access = GH_ParamAccess.list;
    }

    public override bool Write(GH_IWriter writer) {
      writer.SetInt32("Mode", (int)_mode);
      return base.Write(writer);
    }

    #endregion Public Methods

    #region Protected Methods
    protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
      => Menu_AppendItem(menu,
        "Try get connected Element & Members",
        FlipMode,
        true,
        _mode == FoldMode.GetConnected);

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaNodeParameter(),
        GsaNodeGoo.Name,
        GsaNodeGoo.NickName,
        GsaNodeGoo.Description
        + " to get or set information for. Leave blank to create a new "
        + GsaNodeGoo.Name,
        GH_ParamAccess.item);
      pManager.AddIntegerParameter("Node number",
        "ID",
        "Set Node number (ID) - if Node ID is set it will replace any existing nodes in the model",
        GH_ParamAccess.item);
      pManager.AddPointParameter("Node Position",
        "Pt",
        "Set new Position (x, y, z) of Node",
        GH_ParamAccess.item);
      pManager.AddPlaneParameter("Node local axis",
        "Pl",
        "Set Local axis (Plane) of Node",
        GH_ParamAccess.item);
      pManager.AddParameter(new GsaBool6Parameter(),
        "Node Restraints",
        "B6",
        "Set Restraints (Bool6) of Node",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Node Name", "Na", "Set Name of Node", GH_ParamAccess.item);
      pManager.AddColourParameter("Node Colour", "Co", "Set colour of node", GH_ParamAccess.item);

      for (int i = 0; i < pManager.ParamCount; i++)
        pManager[i]
          .Optional = true;

      pManager.HideParameter(0);
      pManager.HideParameter(2);
      pManager.HideParameter(3);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaNodeParameter(),
        GsaNodeGoo.Name,
        GsaNodeGoo.NickName,
        GsaNodeGoo.Description + " with applied changes.",
        GH_ParamAccess.item);
      pManager.AddIntegerParameter("Node number",
        "ID",
        "Original Node number (ID) if Node ever belonged to a GSA Model",
        GH_ParamAccess.item);
      pManager.AddPointParameter("Node Position",
        "Pt",
        "Position (x, y, z) of Node. Setting a new position will clear any existing ID",
        GH_ParamAccess.item);
      pManager.HideParameter(2);
      pManager.AddPlaneParameter("Node local axis",
        "Pl",
        "Local axis (Plane) of Node",
        GH_ParamAccess.item);
      pManager.HideParameter(3);
      pManager.AddParameter(new GsaBool6Parameter(),
        "Node Restraints",
        "B6",
        "Restraints (Bool6) of Node",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Node Name", "Na", "Name of Node", GH_ParamAccess.item);
      pManager.AddColourParameter("Node Colour", "Co", "Get colour of node", GH_ParamAccess.item);
      if (_mode != FoldMode.GetConnected)
        return;

      pManager.AddIntegerParameter("Connected Elements",
        "El",
        "Connected Element IDs in Model that Node once belonged to",
        GH_ParamAccess.list);
      pManager.AddIntegerParameter("Connected Members",
        "Me",
        "Connected Member IDs in Model that Node once belonged to",
        GH_ParamAccess.list);
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      var node = new GsaNode();
      var ghTyp = new GH_ObjectWrapper();
      if (da.GetData(0, ref ghTyp)) {
        var tempPt = new Point3d();
        if (ghTyp.Value is GsaNodeGoo) {
          ghTyp.CastTo(ref node);
          if (node?.ApiNode == null)
            this.AddRuntimeError("Node input is null");
        }
        else if (GH_Convert.ToPoint3d(ghTyp.Value, ref tempPt, GH_Conversion.Both))
          node.Point = tempPt;
        else {
          this.AddRuntimeError("Unable to convert input to Node");
          return;
        }
      }
      else {
        node.Point = new Point3d(0, 0, 0);
        if (Params.Input[2]
            .SourceCount
          == 0)
          this.AddRuntimeRemark("New node created at {0, 0, 0}");
      }

      if (node == null)
        return;

      var ghPoint = new GH_Point();
      if (da.GetData(2, ref ghPoint)) {
        var pt = new Point3d();
        if (GH_Convert.ToPoint3d(ghPoint, ref pt, GH_Conversion.Both))
          node.Point = pt;
      }

      var ghInt = new GH_Integer();
      if (da.GetData(1, ref ghInt))
        if (GH_Convert.ToInt32(ghInt, out int id, GH_Conversion.Both))
          node.Id = id;

      var ghPlane = new GH_Plane();
      if (da.GetData(3, ref ghPlane)) {
        var pln = new Plane();
        if (GH_Convert.ToPlane(ghPlane, ref pln, GH_Conversion.Both)) {
          pln.Origin = node.Point;
          node.LocalAxis = pln;
        }
      }

      var restraint = new GsaBool6();
      if (da.GetData(4, ref restraint))
        node.Restraint = restraint;

      var ghName = new GH_String();
      if (da.GetData(5, ref ghName))
        if (GH_Convert.ToString(ghName, out string name, GH_Conversion.Both))
          node.Name = name;

      var ghColour = new GH_Colour();
      if (da.GetData(6, ref ghColour))
        if (GH_Convert.ToColor(ghColour, out Color col, GH_Conversion.Both))
          node.Colour = col;

      da.SetData(0, new GsaNodeGoo(node));
      da.SetData(1, node.Id);
      da.SetData(2, node.Point);
      da.SetData(3, new GH_Plane(node.LocalAxis));
      da.SetData(4, new GsaBool6Goo(node.Restraint));
      da.SetData(5, node.ApiNode?.Name);
      da.SetData(6, node.Colour);

      if (_mode != FoldMode.GetConnected)
        return;

      try {
        da.SetDataList(7, node.ApiNode?.ConnectedElements);
      }
      catch (Exception) {
        // ignored
      }

      try {
        da.SetDataList(8, node.ApiNode?.ConnectedMembers);
      }
      catch (Exception) {
        // ignored
      }
    }

    #endregion Protected Methods

    #region Private Methods
    private void FlipMode(object sender, EventArgs e) {
      RecordUndoEvent("GetConnected Parameters");
      if (_mode == FoldMode.GetConnected) {
        _mode = FoldMode.DoNotGetConnected;

        while (Params.Output.Count > 8)
          Params.UnregisterOutputParameter(Params.Output[8], true);
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

    #endregion Private Methods
  }
}
