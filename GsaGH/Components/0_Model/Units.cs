using System;
using System.Drawing;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using GsaAPI;
using GsaGH.Helpers.GH;
using GsaGH.Parameters;
using GsaGH.Properties;
using OasysGH;
using OasysGH.Components;

namespace GsaGH.Components {
  public class Units : GH_OasysComponent {
    public override Guid ComponentGuid => new Guid("2eb77ab2-6ea1-4899-ab95-6ae8a36d9d23");
    public override GH_Exposure Exposure => GH_Exposure.quarternary | GH_Exposure.obscure;
    public override OasysPluginInfo PluginInfo => GsaGH.PluginInfo.Instance;
    protected override Bitmap Icon => Resources.Units;

    public Units() : base("Model Units", "Units",
      "Get or set the units used by GSA when opening this Model", 
      CategoryName.Name(), SubCategoryName.Cat0()) {
      Hidden = true;
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager) {
      pManager.AddParameter(new GsaModelParameter(), "GSA Model", "GSA",
        "Existing GSA model to get or set units for.",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Acceleration", "Acc", "Set Acceleration Unit for this GSA Model",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Angle", "Ang", "Set Angle Unit for this GSA Model",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Energy", "E", "Set Energy Unit for this GSA Model",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Force", "F", "Set Force Unit for this GSA Model",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Geometry", "LL", "Set 'Length - large' Unit used for Node Geometry for this GSA Model",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Property Dimension", "LP", "Set 'Length - sections' Unit used for Property Dimensions for this GSA Model",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Displacement", "LS", "Set 'Length - small' Unit used for Displacement Results for this GSA Model",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Mass", "M", "Set Mass Unit for this GSA Model",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Stress", "S", "Set Stress Unit for this GSA Model",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Time Long", "TL", "Set Time Long Unit for this GSA Model",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Time Medium", "TM", "Set Time Medium Unitfor this GSA Model",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Time Short", "TS", "Set Time Short Unit for this GSA Model",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Velocity", "V", "Set Velocity Unit for this GSA Model",
        GH_ParamAccess.item);
      for (int i = 1; i < pManager.ParamCount; i++) {
        pManager[i].Optional = true;
      }
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
      pManager.AddParameter(new GsaModelParameter());
      pManager.AddTextParameter("Acceleration", "Acc", "Get Acceleration Unit for this GSA Model",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Angle", "Ang", "Get Angle Unit for this GSA Model",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Energy", "E", "Get Energy Unit for this GSA Model",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Force", "F", "Get Force Unit for this GSA Model",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Geometry", "LL", "Get 'Length - large' Unit for this GSA Model",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Property Dimension", "LP", "Get 'Length - sections' Unit for this GSA Model",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Displacement", "LS", "Get 'Length - small' Unit for this GSA Model",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Mass", "M", "Get Mass Unit for this GSA Model",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Stress", "S", "Get Stress Unit for this GSA Model",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Time Long", "TL", "Get Time Long Unit for this GSA Model",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Time Medium", "TM", "Get Time Medium Unitfor this GSA Model",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Time Short", "TS", "Get Time Short Unit for this GSA Model",
        GH_ParamAccess.item);
      pManager.AddTextParameter("Velocity", "V", "Get Velocity Unit for this GSA Model",
        GH_ParamAccess.item);
    }

    protected override void SolveInstance(IGH_DataAccess da) {
      GsaModelGoo gooModel = null;
      GsaModel model = null;
      int i = 0;
      if (da.GetData(i++, ref gooModel)) {
        model = gooModel.Value.Duplicate(true);
      }
      GsaAPI.UiUnits units = model.Units;

      var ghString = new GH_String();
      if (da.GetData(i++, ref ghString)) {
        if (GH_Convert.ToString(ghString, out string txt, GH_Conversion.Both)) {
          units.Acceleration = (AccelerationUnit)Enum.Parse(
            typeof(AccelerationUnit), txt, ignoreCase: true);
        }
      }

      ghString = new GH_String();
      if (da.GetData(i++, ref ghString)) {
        if (GH_Convert.ToString(ghString, out string txt, GH_Conversion.Both)) {
          units.Angle = (AngleUnit)Enum.Parse(
            typeof(AngleUnit), txt, ignoreCase: true);
        }
      }

      ghString = new GH_String();
      if (da.GetData(i++, ref ghString)) {
        if (GH_Convert.ToString(ghString, out string txt, GH_Conversion.Both)) {
          units.Energy = (EnergyUnit)Enum.Parse(
            typeof(EnergyUnit), txt, ignoreCase: true);
        }
      }

      ghString = new GH_String();
      if (da.GetData(i++, ref ghString)) {
        if (GH_Convert.ToString(ghString, out string txt, GH_Conversion.Both)) {
          units.Force = (ForceUnit)Enum.Parse(
            typeof(ForceUnit), txt, ignoreCase: true);
        }
      }

      ghString = new GH_String();
      if (da.GetData(i++, ref ghString)) {
        if (GH_Convert.ToString(ghString, out string txt, GH_Conversion.Both)) {
          units.LengthLarge = (LengthUnit)Enum.Parse(
            typeof(LengthUnit), txt, ignoreCase: true);
        }
      }

      ghString = new GH_String();
      if (da.GetData(i++, ref ghString)) {
        if (GH_Convert.ToString(ghString, out string txt, GH_Conversion.Both)) {
          units.LengthSections = (LengthUnit)Enum.Parse(
            typeof(LengthUnit), txt, ignoreCase: true);
        }
      }

      ghString = new GH_String();
      if (da.GetData(i++, ref ghString)) {
        if (GH_Convert.ToString(ghString, out string txt, GH_Conversion.Both)) {
          units.LengthSmall = (LengthUnit)Enum.Parse(
            typeof(LengthUnit), txt, ignoreCase: true);
        }
      }

      ghString = new GH_String();
      if (da.GetData(i++, ref ghString)) {
        if (GH_Convert.ToString(ghString, out string txt, GH_Conversion.Both)) {
          units.Mass = (MassUnit)Enum.Parse(
            typeof(MassUnit), txt, ignoreCase: true);
        }
      }

      ghString = new GH_String();
      if (da.GetData(i++, ref ghString)) {
        if (GH_Convert.ToString(ghString, out string txt, GH_Conversion.Both)) {
          units.Stress = (StressUnit)Enum.Parse(
            typeof(StressUnit), txt, ignoreCase: true);
        }
      }

      ghString = new GH_String();
      if (da.GetData(i++, ref ghString)) {
        if (GH_Convert.ToString(ghString, out string txt, GH_Conversion.Both)) {
          units.TimeLong = (TimeUnit)Enum.Parse(
            typeof(TimeUnit), txt, ignoreCase: true);
        }
      }

      ghString = new GH_String();
      if (da.GetData(i++, ref ghString)) {
        if (GH_Convert.ToString(ghString, out string txt, GH_Conversion.Both)) {
          units.TimeMedium = (TimeUnit)Enum.Parse(
            typeof(TimeUnit), txt, ignoreCase: true);
        }
      }

      ghString = new GH_String();
      if (da.GetData(i++, ref ghString)) {
        if (GH_Convert.ToString(ghString, out string txt, GH_Conversion.Both)) {
          units.TimeShort = (TimeUnit)Enum.Parse(
            typeof(TimeUnit), txt, ignoreCase: true);
        }
      }
 
      ghString = new GH_String();
      if (da.GetData(i++, ref ghString)) {
        if (GH_Convert.ToString(ghString, out string txt, GH_Conversion.Both)) {
          units.Velocity = (VelocityUnit)Enum.Parse(
            typeof(VelocityUnit), txt, ignoreCase: true);
        }
      }

      i = 0;
      da.SetData(i++, new GsaModelGoo(model));
      da.SetData(i++, units.Acceleration.ToString());
      da.SetData(i++, units.Angle.ToString());
      da.SetData(i++, units.Energy.ToString());
      da.SetData(i++, units.Force.ToString());
      da.SetData(i++, units.LengthLarge.ToString());
      da.SetData(i++, units.LengthSections.ToString());
      da.SetData(i++, units.LengthSmall.ToString());
      da.SetData(i++, units.Mass.ToString());
      da.SetData(i++, units.Stress.ToString());
      da.SetData(i++, units.TimeLong.ToString());
      da.SetData(i++, units.TimeMedium.ToString());
      da.SetData(i++, units.TimeShort.ToString());
      da.SetData(i++, units.Velocity.ToString());
    }
  }
}
