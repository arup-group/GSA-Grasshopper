using System;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaGH.Parameters;
using OasysGH;
using OasysGH.Components;
using OasysUnits;
using Rhino.Geometry;

namespace GsaGH.Components
{
  /// <summary>
  /// Component to create new 3d Member
  /// </summary>
  public class CreateMember3d : GH_OasysComponent, IGH_PreviewObject
  {
    #region Name and Ribbon Layout
    public override Guid ComponentGuid => new Guid("08a48fa5-8aaa-43fb-a095-9142794684f7");
    public override GH_Exposure Exposure => GH_Exposure.primary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.CreateMem3d;

    public CreateMember3d() : base("Create 3D Member",
      "Mem3D",
      "Create GSA Member 3D",
      Ribbon.CategoryName.Name(),
      Ribbon.SubCategoryName.Cat2())
    { }
    #endregion

    #region Input and output
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddGeometryParameter("Solid", "S", "Solid Geometry - Closed Brep or Mesh", GH_ParamAccess.item);
      pManager.AddParameter(new GsaProp3dParameter());
      pManager.AddGenericParameter("Mesh Size in model units", "Ms", "Targe mesh size", GH_ParamAccess.item);

      pManager[1].Optional = true;
      pManager[2].Optional = true;
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddParameter(new GsaMember3dParameter());
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
      if (DA.GetData(0, ref gh_typ))
      {
        if (gh_typ == null) { AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Solid input is null"); }
        GsaMember3d mem = new GsaMember3d();
        Brep brep = new Brep();
        Mesh mesh = new Mesh();
        if (GH_Convert.ToBrep(gh_typ.Value, ref brep, GH_Conversion.Both))
        {
          if (brep.IsValid)
            mem = new GsaMember3d(brep);
          else
          {
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "S input is not a valid Brep geometry");
            return;
          }
        }
        else if (GH_Convert.ToMesh(gh_typ.Value, ref mesh, GH_Conversion.Both))
          mem = new GsaMember3d(mesh);
        else
        {
          AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert Geometry input to a 3D Member");
          return;
        }

        // 1 prop3d to be implemented GsaAPI
        gh_typ = new GH_ObjectWrapper();
        GsaProp3d prop3d = new GsaProp3d();
        if (DA.GetData(1, ref gh_typ))
        {
          if (gh_typ.Value is GsaProp3dGoo)
          {
            gh_typ.CastTo(ref prop3d);
            mem.Property = prop3d;
          }
          else if (gh_typ.Value is GsaMaterialGoo)
          {
            GsaMaterial mat = new GsaMaterial();
            gh_typ.CastTo(ref mat);
            prop3d = new GsaProp3d(mat);
            mem.Property = prop3d;
          }
          else
          {
            if (GH_Convert.ToInt32(gh_typ.Value, out int idd, GH_Conversion.Both))
              mem.PropertyID = idd; //new GsaProp3d(idd);
            else
            {
              AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert PA input to a 2D Property of reference integer");
              return;
            }
          }
        }

        // 2 mesh size
        double meshSize = 0;
        if (DA.GetData(2, ref meshSize))
        {
          mem.MeshSize = meshSize;
        }

        DA.SetData(0, new GsaMember3dGoo(mem));
      }
    }
  }
}
