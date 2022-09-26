﻿using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

using Grasshopper.Kernel.Parameters;
using GsaAPI;
using GsaGH.Parameters;
using OasysUnits;
using System.Linq;

namespace GsaGH.Components
{
  /// <summary>
  /// Component to create a new Prop2d
  /// </summary>
  public class CreateProp3d : GH_OasysComponent
  {
    #region Name and Ribbon Layout
    // This region handles how the component in displayed on the ribbon
    // including name, exposure level and icon
    public override Guid ComponentGuid => new Guid("4919553a-8d96-4170-a357-74cfbe930897");
    public CreateProp3d()
      : base("Create 3D Property", "Prop3d", "Create GSA 3D Property",
            Ribbon.CategoryName.Name(),
            Ribbon.SubCategoryName.Cat1())
    { this.Hidden = true; } // sets the initial state of the component to hidden
    public override GH_Exposure Exposure => GH_Exposure.primary | GH_Exposure.obscure;

    protected override System.Drawing.Bitmap Icon => GsaGH.Properties.Resources.CreateProp3d;
    #endregion

    #region Custom UI
    //This region overrides the typical component layout
    #endregion

    #region Input and output


    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddGenericParameter("Material", "Ma", "GsaMaterial or Number referring to a Material already in Existing GSA Model." + System.Environment.NewLine
              + "Accepted inputs are: " + System.Environment.NewLine
              + "0 : Generic" + System.Environment.NewLine
              + "1 : Steel" + System.Environment.NewLine
              + "2 : Concrete" + System.Environment.NewLine
              + "3 : Aluminium" + System.Environment.NewLine
              + "4 : Glass" + System.Environment.NewLine
              + "5 : FRP" + System.Environment.NewLine
              + "7 : Timber (default - because your Carbon Emissions matter!)" + System.Environment.NewLine
              + "8 : Fabric", GH_ParamAccess.item);
    }
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter("3D Property", "PV", "GSA 3D Property", GH_ParamAccess.item);
    }
    #endregion

    protected override void SolveInstance(IGH_DataAccess DA)
    {
      GsaProp3d prop = new GsaProp3d();

      GH_ObjectWrapper gh_typ = new GH_ObjectWrapper();
      if (DA.GetData(0, ref gh_typ))
      {
        GsaMaterial material = new GsaMaterial();
        if (gh_typ.Value is GsaMaterialGoo)
        {
          gh_typ.CastTo(ref material);
          prop.Material = material;
        }
        else
        {
          if (GH_Convert.ToInt32(gh_typ.Value, out int idd, GH_Conversion.Both))
            prop.Material = new GsaMaterial(idd);
          else
          {
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unable to convert PV input to a 3D Property of reference integer");
            return;
          }
        }
      }
      else
        prop.Material = new GsaMaterial(2);

      prop.AxisProperty = 0;

      DA.SetData(0, new GsaProp3dGoo(prop));
    }
  }
}