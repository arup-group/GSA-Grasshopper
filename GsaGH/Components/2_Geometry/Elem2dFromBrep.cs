using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Parameters;
using OasysGH;
using OasysGH.Components;
using OasysGH.Units;
using OasysGH.Units.Helpers;
using OasysUnits;
using OasysUnits.Units;
using Rhino.Geometry;

namespace GsaGH.Components
{
  /// <summary>
  /// Component to edit a Node
  /// </summary>
  public class Elem2dFromBrep : GH_OasysComponent, IGH_PreviewObject, IGH_VariableParameterComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("4fa7ccd9-530e-4036-b2bf-203017b55611");
    public override GH_Exposure Exposure => GH_Exposure.tertiary;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.CreateElemsFromBreps;

    public Elem2dFromBrep() : base("Element2d from Brep",
      "Elem2dFromBrep",
      "Mesh a non-planar Brep",
      Ribbon.CategoryName.Name(),
      Ribbon.SubCategoryName.Cat2())
    { }
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
        dropdownitems.Add(FilteredUnits.FilteredLengthUnits);
        selecteditems.Add(lengthUnit.ToString());

        first = false;
      }
      m_attributes = new UI.MultiDropDownComponentUI(this, SetSelected, dropdownitems, selecteditems, spacerDescriptions);
    }
    public void SetSelected(int i, int j)
    {
      // change selected item
      selecteditems[i] = dropdownitems[i][j];

      lengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), selecteditems[i]);

      // update name of inputs (to display unit on sliders)
      (this as IGH_VariableParameterComponent).VariableParameterMaintenance();
      ExpireSolution(true);
      Params.OnParametersChanged();
      this.OnDisplayExpired(true);
    }
    private void UpdateUIFromSelectedItems()
    {
      lengthUnit = (LengthUnit)Enum.Parse(typeof(LengthUnit), selecteditems[0]);

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
    private LengthUnit lengthUnit = DefaultUnits.LengthUnitGeometry;
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      string unitAbbreviation = new Length(0, lengthUnit).ToString("a");
      pManager.AddBrepParameter("Brep [in " + unitAbbreviation + "]", "B", "Brep (can be non-planar)", GH_ParamAccess.item);
      pManager.AddGenericParameter("Incl. Points or Nodes [in " + unitAbbreviation + "]", "(P)", "Inclusion points or Nodes", GH_ParamAccess.list);
      pManager.AddGenericParameter("Incl. Curves or 1D Members [in " + unitAbbreviation + "]", "(C)", "Inclusion curves or 1D Members", GH_ParamAccess.list);
      pManager.AddGenericParameter("2D Property", "PA", "GSA 2D Property. Input either a GSA 2D Property or an Integer to use a Section already defined in model", GH_ParamAccess.item);
      pManager.AddNumberParameter("Mesh Size [" + unitAbbreviation + "]", "Ms", "Targe mesh size", GH_ParamAccess.item, 0);

      pManager[1].Optional = true;
      pManager[2].Optional = true;
      pManager[3].Optional = true;
      pManager.HideParameter(0);
      pManager.HideParameter(1);
      pManager.HideParameter(2);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter("2D Elements", "E2D", "GSA 2D Elements", GH_ParamAccess.list);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      GH_Brep ghbrep = new GH_Brep();
      if (DA.GetData(0, ref ghbrep))
      {
        if (ghbrep == null) { AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Brep input is null"); }
        Brep brep = new Brep();
        if (GH_Convert.ToBrep(ghbrep, ref brep, GH_Conversion.Both))
        {
          // 1 Points
          List<GH_ObjectWrapper> gh_types = new List<GH_ObjectWrapper>();
          List<Point3d> pts = new List<Point3d>();
          List<GsaNode> nodes = new List<GsaNode>();
          if (DA.GetDataList(1, gh_types))
          {
            for (int i = 0; i < gh_types.Count; i++)
            {
              Point3d pt = new Point3d();
              if (gh_types[i].Value is GsaNodeGoo)
              {
                GsaNode gsanode = new GsaNode();
                gh_types[i].CastTo(ref gsanode);
                nodes.Add(gsanode);
              }
              else if (GH_Convert.ToPoint3d(gh_types[i].Value, ref pt, GH_Conversion.Both))
              {
                pts.Add(pt);
              }
              else
              {
                string type = gh_types[i].Value.GetType().ToString();
                type = type.Replace("GsaGH.Parameters.", "");
                type = type.Replace("Goo", "");
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert incl. Point/Node input parameter of type " +
                    type + " to point or node");
              }
            }
          }

          // 2 Curves
          gh_types = new List<GH_ObjectWrapper>();
          List<Curve> crvs = new List<Curve>();
          List<GsaMember1d> mem1ds = new List<GsaMember1d>();
          if (DA.GetDataList(2, gh_types))
          {
            for (int i = 0; i < gh_types.Count; i++)
            {
              Curve crv = null;
              if (gh_types[i].Value is GsaMember1dGoo)
              {
                GsaMember1d gsamem1d = new GsaMember1d();
                gh_types[i].CastTo(ref gsamem1d);
                mem1ds.Add(gsamem1d);
              }
              else if (GH_Convert.ToCurve(gh_types[i].Value, ref crv, GH_Conversion.Both))
              {
                crvs.Add(crv);
              }
              else
              {
                string type = gh_types[i].Value.GetType().ToString();
                type = type.Replace("GsaGH.Parameters.", "");
                type = type.Replace("Goo", "");
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert incl. Curve/Mem1D input parameter of type " +
                    type + " to curve or 1D Member");
              }
            }
          }

          // 4 mesh size
          GH_Number ghmsz = new GH_Number();
          Length meshSize = Length.Zero;
          if (DA.GetData(4, ref ghmsz))
          {
            GH_Convert.ToDouble(ghmsz, out double m_size, GH_Conversion.Both);
            meshSize = new Length(m_size, lengthUnit);
          }

          // build new element2d with brep, crv and pts
          GsaElement2d elem2d = new GsaElement2d(brep, crvs, pts, meshSize, mem1ds, nodes, lengthUnit);

          // 3 section
          GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
          GsaProp2d prop2d = new GsaProp2d();
          if (DA.GetData(3, ref gh_typ))
          {
            if (gh_typ.Value is GsaProp2dGoo)
              gh_typ.CastTo(ref prop2d);
            else
            {
              if (GH_Convert.ToInt32(gh_typ.Value, out int idd, GH_Conversion.Both))
                prop2d.ID = idd;
              else
              {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert PA input to a 2D Property of reference integer");
                return;
              }
            }
          }
          else
            prop2d.ID = 1;
          List<GsaProp2d> prop2Ds = new List<GsaProp2d>();
          for (int i = 0; i < elem2d.API_Elements.Count; i++)
            prop2Ds.Add(prop2d);
          elem2d.Properties = prop2Ds;

          DA.SetData(0, new GsaElement2dGoo(elem2d));

          AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "This component is work-in-progress and provided 'as-is'. It will unroll the surface, do the meshing, map the mesh back on the original surface. Only single surfaces will work. Surfaces of high curvature and not-unrollable geometries (like a sphere) is unlikely to produce good results");
        }
      }
    }
    #region (de)serialization
    public override bool Write(GH_IO.Serialization.GH_IWriter writer)
    {
      Util.GH.DeSerialization.writeDropDownComponents(ref writer, dropdownitems, selecteditems, spacerDescriptions);
      return base.Write(writer);
    }
    public override bool Read(GH_IO.Serialization.GH_IReader reader)
    {
      try // if users has an old versopm of this component then dropdown menu wont read
      {
        Util.GH.DeSerialization.readDropDownComponents(ref reader, ref dropdownitems, ref selecteditems, ref spacerDescriptions);
      }
      catch (Exception) // we create the dropdown menu with our chosen default
      {
        dropdownitems = new List<List<string>>();
        selecteditems = new List<string>();

        // set length to meters as this was the only option for old components
        lengthUnit = LengthUnit.Meter;

        dropdownitems.Add(FilteredUnits.FilteredLengthUnits);
        selecteditems.Add(lengthUnit.ToString());

        first = false;
      }

      UpdateUIFromSelectedItems();

      first = false;

      return base.Read(reader);
    }
    #endregion

    #region IGH_VariableParameterComponent null implementation
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
      string unitAbbreviation = new Length(0, lengthUnit).ToString("a");

      int i = 0;
      Params.Input[i++].Name = "Brep [in " + unitAbbreviation + "]";
      Params.Input[i++].Name = "Incl. Points or Nodes [in " + unitAbbreviation + "]";
      Params.Input[i++].Name = "Incl. Curves or 1D Members [in " + unitAbbreviation + "]";
      i++;
      Params.Input[i++].Name = "Mesh Size [" + unitAbbreviation + "]";

    }
    #endregion
  }
}

