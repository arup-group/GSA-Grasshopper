﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GsaGH.Parameters;
using OasysGH;
using OasysGH.Components;
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysUnits;
using OasysUnits.Units;

namespace GsaGH.Components
{
  /// <summary>
  /// Component to edit a Node
  /// </summary>
  public class ElemFromMem : GH_OasysDropDownComponent, IGH_PreviewObject
  {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("3de73a08-b72c-45e4-a650-e4c6515266c5");
    public override GH_Exposure Exposure => GH_Exposure.tertiary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.CreateElemsFromMems;

    public ElemFromMem() : base("Elements from Members",
      "ElemFromMem",
      "Create Elements from Members",
      Ribbon.CategoryName.Name(),
      Ribbon.SubCategoryName.Cat2())
    { }
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      string unitAbbreviation = Length.GetAbbreviation(this.LengthUnit);

      pManager.AddGenericParameter("Nodes [" + unitAbbreviation + "]", "No", "Nodes to be included in meshing", GH_ParamAccess.list);
      pManager.AddGenericParameter("1D Members [" + unitAbbreviation + "]", "M1D", "1D Members to create 1D Elements from", GH_ParamAccess.list);
      pManager.AddGenericParameter("2D Members [" + unitAbbreviation + "]", "M2D", "2D Members to create 2D Elements from", GH_ParamAccess.list);
      pManager.AddGenericParameter("3D Members [" + unitAbbreviation + "]", "M3D", "3D Members to create 3D Elements from", GH_ParamAccess.list);

      pManager[0].Optional = true;
      pManager[1].Optional = true;
      pManager[2].Optional = true;
      pManager[3].Optional = true;

      pManager.HideParameter(0);
      pManager.HideParameter(1);
      pManager.HideParameter(2);
      pManager.HideParameter(3);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter("Nodes", "No", "GSA Nodes", GH_ParamAccess.list);
      pManager.HideParameter(0);
      pManager.AddGenericParameter("1D Elements", "E1D", "GSA 1D Elements", GH_ParamAccess.list);
      pManager.AddGenericParameter("2D Elements", "E2D", "GSA 2D Elements", GH_ParamAccess.list);
      pManager.AddGenericParameter("3D Elements", "E3D", "GSA 3D Elements", GH_ParamAccess.item);
      pManager.AddGenericParameter("GSA Model", "GSA", "GSA Model with Elements and Members", GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      #region inputs
      // Get Member1d input
      GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
      List<GH_ObjectWrapper> gh_types = new List<GH_ObjectWrapper>();

      List<GsaNode> in_nodes = new List<GsaNode>();
      if (DA.GetDataList(0, gh_types))
      {
        for (int i = 0; i < gh_types.Count; i++)
        {
          gh_typ = gh_types[i];
          if (gh_typ == null) { Params.Owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Node input (index: " + i + ") is null and has been ignored"); continue; }

          if (gh_typ.Value is GsaNodeGoo)
          {
            GsaNode gsanode = new GsaNode();
            gh_typ.CastTo(ref gsanode);
            in_nodes.Add(gsanode);
          }
          else
          {
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Error in Node input");
            return;
          }
        }
      }

      List<GsaMember1d> in_mem1ds = new List<GsaMember1d>();
      if (DA.GetDataList(1, gh_types))
      {
        for (int i = 0; i < gh_types.Count; i++)
        {
          gh_typ = gh_types[i];
          if (gh_typ == null) { Params.Owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Member1D input (index: " + i + ") is null and has been ignored"); continue; }

          if (gh_typ.Value is GsaMember1dGoo)
          {
            GsaMember1d gsamem1 = new GsaMember1d();
            gh_typ.CastTo(ref gsamem1);
            in_mem1ds.Add(gsamem1);
          }
          else
          {
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Error in Mem1D input");
            return;
          }
        }
      }

      // Get Member2d input
      gh_types = new List<GH_ObjectWrapper>();
      List<GsaMember2d> in_mem2ds = new List<GsaMember2d>();
      if (DA.GetDataList(2, gh_types))
      {
        for (int i = 0; i < gh_types.Count; i++)
        {
          gh_typ = gh_types[i];
          if (gh_typ == null) { Params.Owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Member2D input (index: " + i + ") is null and has been ignored"); continue; }

          if (gh_typ.Value is GsaMember2dGoo)
          {
            GsaMember2d gsamem2 = new GsaMember2d();
            gh_typ.CastTo(ref gsamem2);
            in_mem2ds.Add(gsamem2);
          }
          else
          {
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Error in Mem2D input");
            return;
          }
        }
      }

      // Get Member3d input
      gh_types = new List<GH_ObjectWrapper>();
      List<GsaMember3d> in_mem3ds = new List<GsaMember3d>();
      if (DA.GetDataList(3, gh_types))
      {
        for (int i = 0; i < gh_types.Count; i++)
        {
          gh_typ = gh_types[i];
          if (gh_typ == null) { Params.Owner.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Member3D input (index: " + i + ") is null and has been ignored"); continue; }

          if (gh_typ.Value is GsaMember3dGoo)
          {
            GsaMember3d gsamem3 = new GsaMember3d();
            gh_typ.CastTo(ref gsamem3);
            in_mem3ds.Add(gsamem3);
          }
          else
          {
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Error in Mem3D input");
            return;
          }
        }
      }

      // manually add a warning if no input is set, as all three inputs are optional
      if (in_mem1ds.Count < 1 & in_mem2ds.Count < 1 & in_mem3ds.Count < 1)
      {
        AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Input parameters failed to collect data");
        return;
      }
      #endregion

      // Assemble model
      Model gsa = Util.Gsa.ToGSA.Assemble.AssembleModel(null, in_nodes, null, null, null, in_mem1ds, in_mem2ds, in_mem3ds, null, null, null, null, null, null, null, LengthUnit);

      #region meshing
      // Create elements from members
      gsa.CreateElementsFromMembers();
      #endregion

      // extract nodes from model
      ConcurrentBag<GsaNodeGoo> nodes = Util.Gsa.FromGSA.GetNodes(new ConcurrentDictionary<int, Node>(gsa.Nodes()), LengthUnit);

      // extract elements from model
      Tuple<ConcurrentBag<GsaElement1dGoo>, ConcurrentBag<GsaElement2dGoo>, ConcurrentBag<GsaElement3dGoo>> elementTuple
          = Util.Gsa.FromGSA.GetElements(
              new ConcurrentDictionary<int, Element>(gsa.Elements()),
              new ConcurrentDictionary<int, Node>(gsa.Nodes()),
              new ConcurrentDictionary<int, Section>(gsa.Sections()),
              new ConcurrentDictionary<int, Prop2D>(gsa.Prop2Ds()),
              new ConcurrentDictionary<int, Prop3D>(gsa.Prop3Ds()),
              new ConcurrentDictionary<int, AnalysisMaterial>(gsa.AnalysisMaterials()),
              new ConcurrentDictionary<int, SectionModifier>(gsa.SectionModifiers()),
              LengthUnit);

      // post process materials (as they currently have a bug when running parallel!)

      ConcurrentDictionary<int, AnalysisMaterial> amDict = new ConcurrentDictionary<int, AnalysisMaterial>(gsa.AnalysisMaterials());

      if (elementTuple.Item1 != null)
      {
        foreach (GsaElement1dGoo element in elementTuple.Item1)
        {
          if (element.Value.Section != null && element.Value.Section.API_Section != null)
          {
            if (element.Value.Section.API_Section.MaterialAnalysisProperty > 0)
            {
              amDict.TryGetValue(element.Value.Section.API_Section.MaterialAnalysisProperty, out AnalysisMaterial apimaterial);
              element.Value.Section.Material = new GsaMaterial(element.Value.Section, apimaterial);
            }
            else
              element.Value.Section.Material = new GsaMaterial(element.Value.Section);
          }
        }
      }
      if (elementTuple.Item2 != null)
      {
        foreach (GsaElement2dGoo element in elementTuple.Item2)
        {
          if (element.Value.Properties != null && element.Value.Properties[0].API_Prop2d != null)
          {
            if (element.Value.Properties[0].API_Prop2d.MaterialAnalysisProperty > 0)
            {
              amDict.TryGetValue(element.Value.Properties[0].API_Prop2d.MaterialAnalysisProperty, out AnalysisMaterial apimaterial);
              foreach (GsaProp2d prop in element.Value.Properties)
                prop.Material = new GsaMaterial(prop, apimaterial);
            }
            else
              foreach (GsaProp2d prop in element.Value.Properties)
                prop.Material = new GsaMaterial(prop);
          }
        }
      }
      if (elementTuple.Item3 != null)
      {
        foreach (GsaElement3dGoo element in elementTuple.Item3)
        {
          if (element.Value.Properties != null && element.Value.Properties[0].API_Prop3d != null)
          {
            if (element.Value.Properties[0].API_Prop3d.MaterialAnalysisProperty > 0)
            {
              amDict.TryGetValue(element.Value.Properties[0].API_Prop3d.MaterialAnalysisProperty, out AnalysisMaterial apimaterial);
              foreach (GsaProp3d prop in element.Value.Properties)
                prop.Material = new GsaMaterial(prop, apimaterial);
            }
            else
              foreach (GsaProp3d prop in element.Value.Properties)
                prop.Material = new GsaMaterial(prop);
          }
        }
      }

      // expose internal model if anyone wants to use it
      GsaModel outModel = new GsaModel();
      outModel.Model = gsa;

      DA.SetDataList(0, nodes.OrderBy(item => item.Value.ID));
      DA.SetDataList(1, elementTuple.Item1.OrderBy(item => item.Value.ID));
      DA.SetDataList(2, elementTuple.Item2.OrderBy(item => item.Value.Ids.First()));
      DA.SetDataList(3, elementTuple.Item3.OrderBy(item => item.Value.IDs.First()));
      DA.SetData(4, new GsaModelGoo(outModel));

      // custom display settings for element2d mesh
      element2ds = elementTuple.Item2;
    }
    ConcurrentBag<GsaElement2dGoo> element2ds;
    public override void DrawViewportMeshes(IGH_PreviewArgs args)
    {

      base.DrawViewportMeshes(args);

      if (element2ds != null)
      {
        foreach (GsaElement2dGoo element in element2ds)
        {
          if (element == null) { continue; }
          //Draw shape.
          if (element.Value.Mesh != null)
          {
            if (!(element.Value.API_Elements[0].ParentMember.Member > 0)) // only draw mesh shading if no parent member exist.
            {
              if (this.Attributes.Selected)
                args.Display.DrawMeshShaded(element.Value.Mesh, UI.Colour.Element2dFaceSelected);
              else
                args.Display.DrawMeshShaded(element.Value.Mesh, UI.Colour.Element2dFace);
            }
          }
        }
      }
    }

    public override void DrawViewportWires(IGH_PreviewArgs args)
    {
      base.DrawViewportWires(args);

      if (element2ds != null)
      {
        foreach (GsaElement2dGoo element in element2ds)
        {
          if (element == null) { continue; }
          //Draw lines
          if (element.Value.Mesh != null)
          {
            if (element.Value.API_Elements[0].ParentMember.Member > 0) // only draw mesh shading if no parent member exist.
            {
              for (int i = 0; i < element.Value.Mesh.TopologyEdges.Count; i++)
              {
                if (element.Value.Mesh.TopologyEdges.GetConnectedFaces(i).Length > 1)
                  args.Display.DrawLine(element.Value.Mesh.TopologyEdges.EdgeLine(i), System.Drawing.Color.FromArgb(255, 229, 229, 229), 1);
              }
            }
            else
            {
              if (this.Attributes.Selected)
              {
                for (int i = 0; i < element.Value.Mesh.TopologyEdges.Count; i++)
                  args.Display.DrawLine(element.Value.Mesh.TopologyEdges.EdgeLine(i), UI.Colour.Element2dEdgeSelected, 2);
              }
              else
              {
                for (int i = 0; i < element.Value.Mesh.TopologyEdges.Count; i++)
                  args.Display.DrawLine(element.Value.Mesh.TopologyEdges.EdgeLine(i), UI.Colour.Element2dEdge, 1);
              }
            }
          }
        }
      }
    }

    #region Custom UI
    private LengthUnit LengthUnit = DefaultUnits.LengthUnitGeometry;

    public override void InitialiseDropdowns()
    {
      this.SpacerDescriptions = new List<string>(new string[]
        {
          "Unit"
        });

      this.DropDownItems = new List<List<string>>();
      this.SelectedItems = new List<string>();

      // Length
      this.DropDownItems.Add(UnitsHelper.GetFilteredAbbreviations(EngineeringUnits.Length));
      this.SelectedItems.Add(Length.GetAbbreviation(this.LengthUnit));

      this.IsInitialised = true;
    }

    public override void SetSelected(int i, int j)
    {
      this.SelectedItems[i] = this.DropDownItems[i][j];
      this.LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), this.SelectedItems[i]);
      base.UpdateUI();
    }
    public override void UpdateUIFromSelectedItems()
    {
      this.LengthUnit = (LengthUnit)UnitsHelper.Parse(typeof(LengthUnit), this.SelectedItems[0]);
      base.UpdateUIFromSelectedItems();
    }
    
    public override void VariableParameterMaintenance()
    {
      string unitAbbreviation = Length.GetAbbreviation(this.LengthUnit);

      int i = 0;
      Params.Input[i++].Name = "Nodes [" + unitAbbreviation + "]";
      Params.Input[i++].Name = "1D Members [" + unitAbbreviation + "]";
      Params.Input[i++].Name = "2D Members [" + unitAbbreviation + "]";
      Params.Input[i++].Name = "3D Members [" + unitAbbreviation + "]";
    }
    #endregion
  }
}

