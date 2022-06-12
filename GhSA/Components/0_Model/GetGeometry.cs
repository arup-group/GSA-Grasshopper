using System;
using System.Linq;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Windows.Forms;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GsaGH.Parameters;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using UnitsNet;
using Grasshopper;

namespace GsaGH.Components
{
  /// <summary>
  /// Component to retrieve geometric objects from a GSA model
  /// </summary>
  public class GetGeometry : GH_TaskCapableComponent<GetGeometry.SolveResults>, IGH_PreviewObject, IGH_VariableParameterComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("6c4cb686-a6d1-4a79-b01b-fadc5d6da520");
    public GetGeometry()
      : base("Get Model Geometry", "GetGeo", "Get nodes, elements and members from GSA model",
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat0())
    {
    }

    public override GH_Exposure Exposure => GH_Exposure.secondary;

    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.GetGeometry;
    #endregion

    #region Custom UI
    //This region overrides the typical component layout
    public override void CreateAttributes()
    {
      if (first)
      {
        dropdownitems = new List<List<string>>();
        selecteditems = new List<string>();

        // length
        //dropdownitems.Add(Enum.GetNames(typeof(UnitsNet.Units.LengthUnit)).ToList());
        dropdownitems.Add(Units.FilteredLengthUnits);
        selecteditems.Add(lengthUnit.ToString());

        IQuantity quantity = new Length(0, lengthUnit);
        unitAbbreviation = string.Concat(quantity.ToString().Where(char.IsLetter));

        first = false;
      }
      m_attributes = new UI.MultiDropDownComponentUI(this, SetSelected, dropdownitems, selecteditems, spacerDescriptions);
    }
    public void SetSelected(int i, int j)
    {
      // change selected item
      selecteditems[i] = dropdownitems[i][j];

      lengthUnit = (UnitsNet.Units.LengthUnit)Enum.Parse(typeof(UnitsNet.Units.LengthUnit), selecteditems[i]);

      // update name of inputs (to display unit on sliders)
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      Params.OnParametersChanged();
      this.OnDisplayExpired(true);
    }
    private void UpdateUIFromSelectedItems()
    {
      lengthUnit = (UnitsNet.Units.LengthUnit)Enum.Parse(typeof(UnitsNet.Units.LengthUnit), selecteditems[0]);

      CreateAttributes();
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
      Params.OnParametersChanged();
      this.OnDisplayExpired(true);
    }
    // list of lists with all dropdown lists conctent
    List<List<string>> dropdownitems;
    // list of selected items
    List<string> selecteditems;
    // list of descriptions 
    List<string> spacerDescriptions = new List<string>(new string[]
    {
            "Unit"
    });
    private bool first = true;
    private UnitsNet.Units.LengthUnit lengthUnit = Units.LengthUnitGeometry;
    string unitAbbreviation;
    #region menu override
    private enum FoldMode
    {
      Graft,
      List
    }

    private FoldMode _mode = FoldMode.List;

    protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
    {
      Menu_AppendItem(menu, "Graft by Property", GraftModeClicked, true, _mode == FoldMode.Graft);
      Menu_AppendItem(menu, "List", ListModeClicked, true, _mode == FoldMode.List);

    }
    private void GraftModeClicked(object sender, EventArgs e)
    {
      if (_mode == FoldMode.Graft)
        return;

      RecordUndoEvent("Graft by Property");
      _mode = FoldMode.Graft;

      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      Params.OnParametersChanged();
      Message = "Graft by Property";
      ExpireSolution(true);
    }

    private void ListModeClicked(object sender, EventArgs e)
    {
      if (_mode == FoldMode.List)
        return;

      RecordUndoEvent("List");
      _mode = FoldMode.List;

      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      Params.OnParametersChanged();
      Message = "Import as List";
      ExpireSolution(true);
    }

    #endregion

    #endregion

    #region Input and output

    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddGenericParameter("GSA Model", "GSA", "GSA model containing some geometry", GH_ParamAccess.item);
      pManager.AddTextParameter("Node filter list", "No", "Filter import by list." + System.Environment.NewLine +
          "Node list should take the form:" + System.Environment.NewLine +
          " 1 11 to 72 step 2 not (XY3 31 to 45)" + System.Environment.NewLine +
          "Refer to GSA help file for definition of lists and full vocabulary.", GH_ParamAccess.item, "All");
      pManager.AddTextParameter("Element filter list", "El", "Filter import by list." + System.Environment.NewLine +
          "Element list should take the form:" + System.Environment.NewLine +
          " 1 11 to 20 step 2 P1 not (G1 to G6 step 3) P11 not (PA PB1 PS2 PM3 PA4 M1)" + System.Environment.NewLine +
          "Refer to GSA help file for definition of lists and full vocabulary.", GH_ParamAccess.item, "All");
      pManager.AddTextParameter("Member filter list", "Me", "Filter import by list." + System.Environment.NewLine +
          "Member list should take the form:" + System.Environment.NewLine +
          " 1 11 to 20 step 2 P1 not (G1 to G6 step 3) P11 not (Z4 XY55)" + System.Environment.NewLine +
          "Refer to GSA help file for definition of lists and full vocabulary.", GH_ParamAccess.item, "All");
      pManager[1].Optional = true;
      pManager[2].Optional = true;
      pManager[3].Optional = true;

      //_mode = FoldMode.Graft;
      //Message = "Graft by Property" + System.Environment.NewLine + "Right-click to change";
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      IQuantity length = new Length(0, lengthUnit);
      unitAbbreviation = string.Concat(length.ToString().Where(char.IsLetter));

      pManager.AddGenericParameter("Nodes [" + unitAbbreviation + "]", "No", "Nodes from GSA Model", GH_ParamAccess.list);
      pManager.HideParameter(0);
      pManager.AddGenericParameter("1D Elements [" + unitAbbreviation + "]", "E1D", "1D Elements (Analysis Layer) from GSA Model imported to selected unit", GH_ParamAccess.list);
      pManager.AddGenericParameter("2D Elements [" + unitAbbreviation + "]", "E2D", "2D Elements (Analysis Layer) from GSA Model imported to selected unit", GH_ParamAccess.list);
      pManager.AddGenericParameter("3D Elements [" + unitAbbreviation + "]", "E3D", "3D Elements (Analysis Layer) from GSA Model imported to selected unit", GH_ParamAccess.list);
      pManager.HideParameter(2);
      //pManager.HideParameter(3);
      pManager.AddGenericParameter("1D Members [" + unitAbbreviation + "]", "M1D", "1D Members (Design Layer) from GSA Model imported to selected unit", GH_ParamAccess.tree);
      pManager.AddGenericParameter("2D Members [" + unitAbbreviation + "]", "M2D", "2D Members (Design Layer) from GSA Model imported to selected unit", GH_ParamAccess.tree);
      pManager.AddGenericParameter("3D Members [" + unitAbbreviation + "]", "M3D", "3D Members (Design Layer) from GSA Model imported to selected unit", GH_ParamAccess.tree);
    }
    #endregion

    public class SolveResults
    {
      internal ConcurrentBag<GsaNodeGoo> Nodes { get; set; }
      internal ConcurrentBag<GsaNodeGoo> displaySupports { get; set; }
      internal ConcurrentBag<GsaElement1dGoo> Elem1ds { get; set; }
      internal ConcurrentBag<GsaElement2dGoo> Elem2ds { get; set; }
      internal ConcurrentBag<GsaElement3dGoo> Elem3ds { get; set; }
      internal ConcurrentBag<GsaMember1dGoo> Mem1ds { get; set; }
      internal ConcurrentBag<GsaMember2dGoo> Mem2ds { get; set; }
      internal ConcurrentBag<GsaMember3dGoo> Mem3ds { get; set; }
    }
    SolveResults Compute(ConcurrentDictionary<int, Node> allnDict, ConcurrentDictionary<int, Axis> axDict,
        ConcurrentDictionary<int, Node> nDict,
        ConcurrentDictionary<int, Element> eDict,
        ConcurrentDictionary<int, Member> mDict,
        ConcurrentDictionary<int, Section> sDict,
        ConcurrentDictionary<int, Prop2D> pDict,
        ConcurrentDictionary<int, Prop3D> p3Dict,
        ConcurrentDictionary<int, AnalysisMaterial> amDict
        )
    {
      SolveResults results = new SolveResults();
      List<int> steps = new List<int> { 0, 1, 2 };

      try
      {
        Parallel.ForEach(steps, i =>
        {
          if (i == 0)
          {
            // create nodes
            results.Nodes = Util.Gsa.FromGSA.GetNodes(nDict, lengthUnit, axDict);
            results.displaySupports = new ConcurrentBag<GsaNodeGoo>(results.Nodes.AsParallel().Where(n => n.Value.isSupport));
          }

          if (i == 1)
          {
            // create elements
            Tuple<ConcurrentBag<GsaElement1dGoo>, ConcurrentBag<GsaElement2dGoo>, ConcurrentBag<GsaElement3dGoo>> elementTuple
                = Util.Gsa.FromGSA.GetElements(eDict, allnDict, sDict, pDict, p3Dict, amDict, lengthUnit);

            results.Elem1ds = elementTuple.Item1;
            results.Elem2ds = elementTuple.Item2;
            results.Elem3ds = elementTuple.Item3;
          }

          if (i == 2)
          {
            // create members
            Tuple<ConcurrentBag<GsaMember1dGoo>, ConcurrentBag<GsaMember2dGoo>, ConcurrentBag<GsaMember3dGoo>> memberTuple
                = Util.Gsa.FromGSA.GetMembers(mDict, allnDict, lengthUnit, sDict, pDict, p3Dict, this);

            results.Mem1ds = memberTuple.Item1;
            results.Mem2ds = memberTuple.Item2;
            results.Mem3ds = memberTuple.Item3;
          }
        });
      }
      catch (Exception e)
      {
        AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, e.InnerException.Message);
      }
      // post process materials (as they currently have a bug when running parallel!)
      if (results.Elem1ds != null)
      {
        foreach (GsaElement1dGoo element in results.Elem1ds)
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
      if (results.Elem2ds != null)
      {
        foreach (GsaElement2dGoo element in results.Elem2ds)
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
      if (results.Elem3ds != null)
      {
        foreach (GsaElement3dGoo element in results.Elem3ds)
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
      if (results.Mem1ds != null)
      {
        foreach (GsaMember1dGoo element in results.Mem1ds)
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
      if (results.Mem2ds != null)
      {
        foreach (GsaMember2dGoo element in results.Mem2ds)
        {
          if (element.Value.Property != null && element.Value.Property.API_Prop2d != null)
          {
            if (element.Value.Property.API_Prop2d.MaterialAnalysisProperty > 0)
            {
              amDict.TryGetValue(element.Value.Property.API_Prop2d.MaterialAnalysisProperty, out AnalysisMaterial apimaterial);
              element.Value.Property.Material = new GsaMaterial(element.Value.Property, apimaterial);
            }
            else
              element.Value.Property.Material = new GsaMaterial(element.Value.Property);
          }
        }
      }
      if (results.Mem3ds != null)
      {
        foreach (GsaMember3dGoo element in results.Mem3ds)
        {
          if (element.Value.Property != null && element.Value.Property.API_Prop3d != null)
          {
            if (element.Value.Property.API_Prop3d.MaterialAnalysisProperty > 0)
            {
              amDict.TryGetValue(element.Value.Property.API_Prop3d.MaterialAnalysisProperty, out AnalysisMaterial apimaterial);
              element.Value.Property.Material = new GsaMaterial(element.Value.Property, apimaterial);
            }
            else
              element.Value.Property.Material = new GsaMaterial(element.Value.Property);
          }
        }
      }

      return results;
    }

    protected override void SolveInstance(IGH_DataAccess data)
    {
      ConcurrentDictionary<int, Member> mDict = null;
      if (InPreSolve)
      {
        // First pass; collect data and construct tasks
        GsaModel gsaModel = new GsaModel();
        GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
        Task<SolveResults> tsk = null;
        if (data.GetData(0, ref gh_typ))
        {
          if (gh_typ.Value is GsaModelGoo)
            gh_typ.CastTo(ref gsaModel);
          else
          {
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Error converting input to GSA Model");
            return;
          }

          // import lists
          string nodeList = "all";
          if (data.GetData(1, ref nodeList))
            nodeList = nodeList.ToString();
          string elemList = "all";
          if (data.GetData(2, ref elemList))
            elemList = elemList.ToString();
          string memList = "all";
          if (data.GetData(3, ref memList))
            memList = memList.ToString();

          // collect data from model:
          Model model = gsaModel.Model;

          ConcurrentDictionary<int, Node> nDict = new ConcurrentDictionary<int, Node>(model.Nodes());
          ConcurrentDictionary<int, Axis> axDict = new ConcurrentDictionary<int, Axis>(model.Axes());
          ConcurrentDictionary<int, Node> out_nDict = (nodeList.ToLower() == "all") ? nDict : new ConcurrentDictionary<int, Node>(model.Nodes(nodeList));
          ConcurrentDictionary<int, Element> eDict = new ConcurrentDictionary<int, Element>(model.Elements(elemList));
          mDict = new ConcurrentDictionary<int, Member>(model.Members(memList));
          ConcurrentDictionary<int, Section> sDict = new ConcurrentDictionary<int, Section>(model.Sections());
          ConcurrentDictionary<int, Prop2D> pDict = new ConcurrentDictionary<int, Prop2D>(model.Prop2Ds());
          ConcurrentDictionary<int, Prop3D> p3Dict = new ConcurrentDictionary<int, Prop3D>(model.Prop3Ds());
          ConcurrentDictionary<int, AnalysisMaterial> amDict = new ConcurrentDictionary<int, AnalysisMaterial>(model.AnalysisMaterials());

          tsk = Task.Run(() => Compute(nDict, axDict, out_nDict,
              eDict, mDict, sDict, pDict, p3Dict, amDict), CancelToken);
        }
        // Add a null task even if data collection fails. This keeps the
        // list size in sync with the iterations
        TaskList.Add(tsk);
        return;
      }

      SolveResults results;
      if (!GetSolveResults(data, out results))
      {
        // Compute right here, right now.
        // 1. Collect
        GsaModel gsaModel = new GsaModel();
        GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
        if (data.GetData(0, ref gh_typ))
        {
          if (gh_typ.Value is GsaModelGoo)
            gh_typ.CastTo(ref gsaModel);
          else
          {
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Error converting input to GSA Model");
            return;
          }

          // import lists
          string nodeList = "all";
          if (data.GetData(1, ref nodeList))
            nodeList = nodeList.ToString();
          string elemList = "all";
          if (data.GetData(2, ref elemList))
            elemList = elemList.ToString();
          string memList = "all";
          if (data.GetData(3, ref memList))
            memList = memList.ToString();

          // 2. Compute
          // collect data from model:
          Model model = gsaModel.Model;

          ConcurrentDictionary<int, Node> nDict = new ConcurrentDictionary<int, Node>(model.Nodes());
          ConcurrentDictionary<int, Axis> axDict = new ConcurrentDictionary<int, Axis>(model.Axes());
          ConcurrentDictionary<int, Node> out_nDict = (nodeList.ToLower() == "all") ? nDict : new ConcurrentDictionary<int, Node>(model.Nodes(nodeList));
          ConcurrentDictionary<int, Element> eDict = new ConcurrentDictionary<int, Element>(model.Elements(elemList));
          mDict = new ConcurrentDictionary<int, Member>(model.Members(memList));
          ConcurrentDictionary<int, Section> sDict = new ConcurrentDictionary<int, Section>(model.Sections());
          ConcurrentDictionary<int, Prop2D> pDict = new ConcurrentDictionary<int, Prop2D>(model.Prop2Ds());
          ConcurrentDictionary<int, Prop3D> p3Dict = new ConcurrentDictionary<int, Prop3D>(model.Prop3Ds());
          ConcurrentDictionary<int, AnalysisMaterial> amDict = new ConcurrentDictionary<int, AnalysisMaterial>(model.AnalysisMaterials());

          results = Compute(nDict, axDict, out_nDict,
          eDict, mDict, sDict, pDict, p3Dict, amDict);
        }
        else return;
      }

      // 3. Set
      if (results != null)
      {
        if (results.Nodes != null)
        {
          data.SetDataList(0, results.Nodes.OrderBy(item => item.Value.ID));
          supportNodes = results.displaySupports;
        }
        if (results.Elem1ds != null)
        {
          List<int> invalid1delem = results.Elem1ds.Where(x => !x.Value.IsValid).Select(x => x.Value.ID).ToList();
          if (invalid1delem.Count > 0)
          {
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Invalid Element1D definition for Element IDs:");
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, string.Join(" ", invalid1delem.OrderBy(x => x)));
          }
          if (_mode == FoldMode.List)
            data.SetDataList(1, results.Elem1ds.OrderBy(item => item.Value.ID));
          else
          {
            DataTree<GsaElement1dGoo> tree = new DataTree<GsaElement1dGoo>();
            foreach (GsaElement1dGoo element in results.Elem1ds)
              tree.Add(element, new Grasshopper.Kernel.Data.GH_Path(element.Value.Section.ID));
            data.SetDataTree(1, tree);
          }
        }
        if (results.Elem2ds != null)
        {
          if (_mode == FoldMode.List)
            data.SetDataList(2, results.Elem2ds.OrderBy(item => item.Value.ID.First()));
          else
          {
            DataTree<GsaElement2dGoo> tree = new DataTree<GsaElement2dGoo>();
            foreach (GsaElement2dGoo element in results.Elem2ds)
              tree.Add(element, new Grasshopper.Kernel.Data.GH_Path(element.Value.Properties.First().ID));
            data.SetDataTree(2, tree);
          }
          element2ds = results.Elem2ds;

          ConcurrentBag<GsaElement2dGoo> element2dsShaded = new ConcurrentBag<GsaElement2dGoo>();
          ConcurrentBag<GsaElement2dGoo> element2dsNotShaded = new ConcurrentBag<GsaElement2dGoo>();
          Parallel.ForEach(element2ds, elem =>
          {
            try
            {
              if (elem.Value.API_Elements[0].ParentMember.Member > 0
                                                && mDict.ContainsKey(elem.Value.API_Elements[0].ParentMember.Member))
                element2dsShaded.Add(elem);
              else
                element2dsNotShaded.Add(elem);
            }
            catch (Exception)
            {
              element2dsNotShaded.Add(elem);
            }
          });
          cachedDisplayMeshShaded = new Mesh();
          cachedDisplayMeshShaded.Append(element2dsShaded.Select(e => e.Value.Mesh));
          cachedDisplayMeshNotShaded = new Mesh();
          cachedDisplayMeshNotShaded.Append(element2dsNotShaded.Select(e => e.Value.Mesh));
        }
        if (results.Elem3ds != null)
        {

          if (_mode == FoldMode.List)
            data.SetDataList(3, results.Elem3ds.OrderBy(item => item.Value.ID.First()));
          else
          {
            DataTree<GsaElement3dGoo> tree = new DataTree<GsaElement3dGoo>();
            foreach (GsaElement3dGoo element in results.Elem3ds)
              tree.Add(element, new Grasshopper.Kernel.Data.GH_Path(element.Value.PropertyIDs.First()));
            data.SetDataTree(3, tree);
          }
          element3ds = results.Elem3ds;
          ConcurrentBag<GsaElement3dGoo> element3dsShaded = new ConcurrentBag<GsaElement3dGoo>();
          ConcurrentBag<GsaElement3dGoo> element3dsNotShaded = new ConcurrentBag<GsaElement3dGoo>();
          Parallel.ForEach(element3ds, elem =>
          {
            try
            {
              if (elem.Value.API_Elements[0].ParentMember.Member > 0
                        && mDict.ContainsKey(elem.Value.API_Elements[0].ParentMember.Member))
                element3dsShaded.Add(elem);
              else
                element3dsNotShaded.Add(elem);
            }
            catch (Exception)
            {
              element3dsNotShaded.Add(elem);
            }
          });
          cachedDisplayNgonMeshShaded = new Mesh();
          cachedDisplayNgonMeshShaded.Append(element3dsShaded.Select(e => e.Value.DisplayMesh));
          cachedDisplayNgonMeshNotShaded = new Mesh();
          cachedDisplayNgonMeshNotShaded.Append(element3dsNotShaded.Select(e => e.Value.DisplayMesh));
        }
        if (results.Mem1ds != null)
        {
          List<int> invalid1dmem = results.Mem1ds.Where(x => !x.Value.IsValid).Select(x => x.Value.ID).ToList();
          if (invalid1dmem.Count > 0)
          {
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Invalid Member1D definition for Member IDs:");
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, string.Join(" ", invalid1dmem.OrderBy(x => x)));
          }
          if (_mode == FoldMode.List)
            data.SetDataList(4, results.Mem1ds.OrderBy(item => item.Value.ID));
          else
          {
            DataTree<GsaMember1dGoo> tree = new DataTree<GsaMember1dGoo>();
            foreach (GsaMember1dGoo element in results.Mem1ds)
              tree.Add(element, new Grasshopper.Kernel.Data.GH_Path(element.Value.Section.ID));
            data.SetDataTree(4, tree);
          }
        }
        if (results.Mem2ds != null)
        {
          List<int> invalid2dmem = results.Mem2ds.Where(x => !x.Value.IsValid).Select(x => x.Value.ID).ToList();
          if (invalid2dmem.Count > 0)
          {
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Invalid Member2D definition for Member IDs:");
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, string.Join(" ", invalid2dmem.OrderBy(x => x)));
          }
          if (_mode == FoldMode.List)
            data.SetDataList(5, results.Mem2ds.OrderBy(item => item.Value.ID));
          else
          {
            DataTree<GsaMember2dGoo> tree = new DataTree<GsaMember2dGoo>();
            foreach (GsaMember2dGoo element in results.Mem2ds)
              tree.Add(element, new Grasshopper.Kernel.Data.GH_Path(element.Value.Property.ID));
            data.SetDataTree(5, tree);
          }
        }
        if (results.Mem3ds != null)
        {
          List<int> invalid3dmem = results.Mem3ds.Where(x => !x.Value.IsValid).Select(x => x.Value.ID).ToList();
          if (invalid3dmem.Count > 0)
          {
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Invalid Member3D definition for Member IDs:");
            this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, string.Join(" ", invalid3dmem.OrderBy(x => x)));
          }
          if (_mode == FoldMode.List)
            data.SetDataList(6, results.Mem3ds.OrderBy(item => item.Value.ID));
          else
          {
            DataTree<GsaMember3dGoo> tree = new DataTree<GsaMember3dGoo>();
            foreach (GsaMember3dGoo element in results.Mem3ds)
              tree.Add(element, new Grasshopper.Kernel.Data.GH_Path(element.Value.Property.ID));
            data.SetDataTree(6, tree);
          }
        }
      }
    }

    ConcurrentBag<GsaElement2dGoo> element2ds;
    ConcurrentBag<GsaElement3dGoo> element3ds;
    Mesh cachedDisplayMeshShaded;
    Mesh cachedDisplayMeshNotShaded;
    Mesh cachedDisplayNgonMeshShaded;
    Mesh cachedDisplayNgonMeshNotShaded;
    ConcurrentBag<GsaNodeGoo> supportNodes;

    public override void DrawViewportWires(IGH_PreviewArgs args)
    {
      base.DrawViewportWires(args);

      if (cachedDisplayMeshShaded != null)
      {
        args.Display.DrawMeshWires(cachedDisplayMeshShaded, System.Drawing.Color.FromArgb(255, 229, 229, 229), 1);
      }
      if (cachedDisplayNgonMeshShaded != null)
      {
        args.Display.DrawMeshWires(cachedDisplayNgonMeshShaded, System.Drawing.Color.FromArgb(255, 229, 229, 229), 1);
      }

      if (cachedDisplayMeshNotShaded != null)
      {
        if (this.Attributes.Selected)
        {
          args.Display.DrawMeshWires(cachedDisplayMeshNotShaded, UI.Colour.Element2dEdgeSelected, 2);
        }
        else
        {
          args.Display.DrawMeshWires(cachedDisplayMeshNotShaded, UI.Colour.Element2dEdge, 1);
        }
      }
      if (cachedDisplayNgonMeshNotShaded != null)
      {
        if (this.Attributes.Selected)
        {
          args.Display.DrawMeshWires(cachedDisplayNgonMeshNotShaded, UI.Colour.Element2dEdgeSelected, 2);
        }
        else
        {
          args.Display.DrawMeshWires(cachedDisplayNgonMeshNotShaded, UI.Colour.Element2dEdge, 1);
        }
      }

      if (supportNodes != null)
      {
        foreach (GsaNodeGoo node in supportNodes)
          if (node.Value.Point.IsValid)
          {
            // draw the point
            if (!this.Attributes.Selected)
            {
              if ((System.Drawing.Color)node.Value.Colour != System.Drawing.Color.FromArgb(0, 0, 0))
              {
                args.Display.DrawPoint(node.Value.Point, Rhino.Display.PointStyle.RoundSimple, 3, (System.Drawing.Color)node.Value.Colour);
              }
              else
              {
                System.Drawing.Color col = UI.Colour.Node;
                args.Display.DrawPoint(node.Value.Point, Rhino.Display.PointStyle.RoundSimple, 3, col);
              }
              if (node.Value.previewSupportSymbol != null)
                args.Display.DrawBrepShaded(node.Value.previewSupportSymbol, UI.Colour.SupportSymbol);
              if (node.Value.previewText != null)
                args.Display.Draw3dText(node.Value.previewText, UI.Colour.Support);
            }
            else
            {
              args.Display.DrawPoint(node.Value.Point, Rhino.Display.PointStyle.RoundControlPoint, 3, UI.Colour.NodeSelected);
              if (node.Value.previewSupportSymbol != null)
                args.Display.DrawBrepShaded(node.Value.previewSupportSymbol, UI.Colour.SupportSymbolSelected);
              if (node.Value.previewText != null)
                args.Display.Draw3dText(node.Value.previewText, UI.Colour.NodeSelected);
            }

            // local axis
            if (node.Value.LocalAxis != Plane.WorldXY & node.Value.LocalAxis != new Plane() & node.Value.LocalAxis != Plane.Unset)
            {
              args.Display.DrawLine(node.Value.previewXaxis, System.Drawing.Color.FromArgb(255, 244, 96, 96), 1);
              args.Display.DrawLine(node.Value.previewYaxis, System.Drawing.Color.FromArgb(255, 96, 244, 96), 1);
              args.Display.DrawLine(node.Value.previewZaxis, System.Drawing.Color.FromArgb(255, 96, 96, 234), 1);
            }
          }
      }
    }
    #region (de)serialization
    public override bool Write(GH_IO.Serialization.GH_IWriter writer)
    {
      writer.SetInt32("Mode", (int)_mode);
      Util.GH.DeSerialization.writeDropDownComponents(ref writer, dropdownitems, selecteditems, spacerDescriptions);
      return base.Write(writer);
    }
    public override bool Read(GH_IO.Serialization.GH_IReader reader)
    {
      _mode = (FoldMode)reader.GetInt32("Mode");

      try // if users has an old versopm of this component then dropdown menu wont read
      {
        Util.GH.DeSerialization.readDropDownComponents(ref reader, ref dropdownitems, ref selecteditems, ref spacerDescriptions);
      }
      catch (Exception) // we create the dropdown menu with our chosen default
      {
        dropdownitems = new List<List<string>>();
        selecteditems = new List<string>();

        // set length to meters as this was the only option for old components
        lengthUnit = UnitsNet.Units.LengthUnit.Meter;

        dropdownitems.Add(Units.FilteredLengthUnits);
        selecteditems.Add(lengthUnit.ToString());

        IQuantity quantity = new Length(0, lengthUnit);
        unitAbbreviation = string.Concat(quantity.ToString().Where(char.IsLetter));

        first = false;
      }

      UpdateUIFromSelectedItems();

      first = false;

      return base.Read(reader);
    }

    bool IGH_VariableParameterComponent.CanInsertParameter(GH_ParameterSide side, int index)
    {
      return false;
    }
    bool IGH_VariableParameterComponent.CanRemoveParameter(GH_ParameterSide side, int index)
    {
      return false;
    }
    IGH_Param IGH_VariableParameterComponent.CreateParameter(GH_ParameterSide side, int index)
    {
      return null;
    }
    bool IGH_VariableParameterComponent.DestroyParameter(GH_ParameterSide side, int index)
    {
      return false;
    }

    void IGH_VariableParameterComponent.VariableParameterMaintenance()
    {
      IQuantity length = new Length(0, lengthUnit);
      unitAbbreviation = string.Concat(length.ToString().Where(char.IsLetter));

      int i = 0;
      Params.Output[i++].Name = "Nodes [" + unitAbbreviation + "]";
      Params.Output[i++].Name = "1D Elements [" + unitAbbreviation + "]";
      Params.Output[i++].Name = "2D Elements [" + unitAbbreviation + "]";
      Params.Output[i++].Name = "3D Elements [" + unitAbbreviation + "]";
      Params.Output[i++].Name = "1D Members [" + unitAbbreviation + "]";
      Params.Output[i++].Name = "2D Members [" + unitAbbreviation + "]";
      Params.Output[i++].Name = "3D Members [" + unitAbbreviation + "]";

      i = 1;
      for (int j = 1; j < 7; j++)
      {
        if (_mode == FoldMode.List)
          Params.Output[i].Access = GH_ParamAccess.list;
        else
          Params.Output[i].Access = GH_ParamAccess.tree;
      }

    }

    #endregion
  }
}

