using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Grasshopper.Kernel;

using GsaAPI;

using GsaGH.Helpers.Assembly;
using GsaGH.Helpers.Import;
using GsaGH.Parameters;

using OasysUnits;

using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGH.Helpers {
  public class MergeModels {

    public static GsaModel MergeModel(List<GsaModel> models, GH_Component owner, Length tolerance) {
      if (models == null) {
        return null;
      }

      if (models.Count <= 1) {
        return models.Count > 0 ? models[0].Clone() : null;
      }

      var model = new GsaModel();
      models.Reverse();
      for (int i = 0; i < models.Count - 1; i++) {
        model = MergeModel(model, models[i].Clone(), owner, tolerance);
      }

      GsaModel clone = models[models.Count - 1].Clone();
      clone = MergeModel(clone, model, owner, tolerance);
      return clone;
    }

    public static GsaModel MergeModel(
      GsaModel mainModel, GsaModel appendModel, GH_Component owner, Length tolerance) {
      appendModel.ModelUnit = mainModel.ModelUnit;
      ConcurrentBag<GsaNodeGoo> goonodes =
        Import.Nodes.GetNodes(
          appendModel.ApiNodes, mainModel.ModelUnit, appendModel.ApiAxis, appendModel.SpringProps);
      var nodes = goonodes.Select(n => n.Value).OrderByDescending(x => x.Id).ToList();
      nodes.Select(c => {
        c.Id = 0; // set node Id of incoming to 0 to append to end and use CollapseCoincidingNodes
        return c;
      }).ToList();

      var elements = new Import.Elements(appendModel);
      var elem1ds = elements.Element1ds.Select(n => n.Value).
        OrderByDescending(x => x.Id).ToList();
      elem1ds.Select(c => {
        c.Id = 0;
        return c;
      }).ToList();
      var elem2ds = elements.Element2ds.Select(n => n.Value).
        OrderByDescending(x => x.Ids.First()).ToList();
      foreach (GsaElement2d elem2d in elem2ds) {
        elem2d.Ids.Select(c => {
          c = 0;
          return c;
        }).ToList();
      }
      var elem3ds = elements.Element3ds.Select(n => n.Value).
        OrderByDescending(x => x.Ids.First()).ToList();
      foreach (GsaElement3d elem3d in elem3ds) {
        elem3d.Ids.Select(c => {
          c = 0;
          return c;
        }).ToList();
      }
      var assemblies = elements.Assemblies.Select(n => n.Value).OrderByDescending(x => x.Id).ToList();
      assemblies.Select(c => {
        c.Id = 0;
        return c;
      }).ToList();

      var members = new Import.Members(appendModel, owner);
      var mem1ds = members.Member1ds.Select(n => n.Value).OrderByDescending(x => x.Id).ToList();
      mem1ds.Select(c => {
        c.Id = 0;
        return c;
      }).ToList();
      var mem2ds = members.Member2ds.Select(n => n.Value).OrderByDescending(x => x.Id).ToList();
      mem2ds.Select(c => {
        c.Id = 0;
        return c;
      }).ToList();
      var mem3ds = members.Member3ds.Select(n => n.Value).OrderByDescending(x => x.Id).ToList();
      mem3ds.Select(c => {
        c.Id = 0;
        return c;
      }).ToList();


      var existingSectionIds = mainModel.Sections.Keys.ToList();
      var sections = appendModel.Sections.
        Select(n => n.Value.Value).OrderByDescending(x => x.Id).ToList();
      sections.Select(c => {
        if (existingSectionIds.Contains(c.Id)) {
          c.Id = 0; // only set id to 0 if sectionId already exists in model to allow reference
        }
        return c;
      }).ToList();

      var existingProp2dIds = mainModel.Prop2ds.Keys.ToList();
      var prop2Ds = appendModel.Prop2ds.
        Select(n => n.Value.Value).OrderByDescending(x => x.Id).ToList();
      prop2Ds.Select(c => {
        if (existingProp2dIds.Contains(c.Id)) {
          c.Id = 0;
        }
        return c;
      }).ToList();

      var existingProp3dIds = mainModel.Prop3ds.Keys.ToList();
      var prop3Ds = appendModel.Prop3ds.
        Select(n => n.Value.Value).OrderByDescending(x => x.Id).ToList();
      prop3Ds.Select(c => {
        if (existingProp3dIds.Contains(c.Id)) {
          c.Id = 0;
        }
        return c;
      }).ToList();

      var existingSpringPropIds = mainModel.SpringProps.Keys.ToList();
      var springProps = appendModel.SpringProps.
        Select(n => n.Value.Value).OrderByDescending(x => x.Id).ToList();
      springProps.Select(c => {
        if (existingSpringPropIds.Contains(c.Id)) {
          c.Id = 0;
        }
        return c;
      }).ToList();

      var gooloads = new List<GsaLoadGoo>();
      gooloads.AddRange(GsaLoadFactory.CreateGravityLoadsFromApi(appendModel.ApiModel));
      gooloads.AddRange(GsaLoadFactory.CreateNodeLoadsFromApi(appendModel.ApiModel));
      gooloads.AddRange(GsaLoadFactory.CreateBeamLoadsFromApi(appendModel.ApiModel));
      gooloads.AddRange(GsaLoadFactory.CreateBeamThermalLoadsFromApi(appendModel.ApiModel));
      gooloads.AddRange(GsaLoadFactory.CreateFaceLoadsFromApi(appendModel.ApiModel));
      gooloads.AddRange(GsaLoadFactory.CreateFaceThermalLoadsFromApi(appendModel.ApiModel));
      gooloads.AddRange(GsaLoadFactory.CreateGridPointLoadsFromApi(appendModel.ApiModel, LengthUnit.Meter));
      gooloads.AddRange(GsaLoadFactory.CreateGridLineLoadsFromApi(appendModel.ApiModel, LengthUnit.Meter));
      gooloads.AddRange(GsaLoadFactory.CreateGridAreaLoadsFromApi(appendModel.ApiModel, LengthUnit.Meter));
      var loads = gooloads.Select(n => n.Value).ToList();

      IReadOnlyDictionary<int, GridSurface> srfDict = appendModel.ApiModel.GridSurfaces();
      var gpsgoo = srfDict.Keys.Select(key => new GsaGridPlaneSurfaceGoo(
           GsaLoadFactory.CreateGridPlaneSurfaceFromApi(appendModel.ApiModel, key, LengthUnit.Meter))).ToList();
      var gps = gpsgoo.Select(n => n.Value).ToList();

      List<GsaList> lists = appendModel.GetLists();
      List<GsaGridLine> gridLines = appendModel.GetGridLines();
      var gsaLoadCases = GsaLoadFactory.CreateLoadCasesFromApi(appendModel.ApiModel).Select(n => n.Value).ToList();
      var designTasks = new List<IGsaDesignTask>();
      foreach (SteelDesignTask designTask in appendModel.ApiModel.SteelDesignTasks().Values) {
        var kvp = new KeyValuePair<int, SteelDesignTask>(0, designTask);
        designTasks.Add(new GsaSteelDesignTask(kvp, appendModel));
      }

      var geometry = new GsaGeometry {
        Nodes = nodes,
        Element1ds = elem1ds,
        Element2ds = elem2ds,
        Element3ds = elem3ds,
        Member1ds = mem1ds,
        Member2ds = mem2ds,
        Member3ds = mem3ds
      };
      var properties = new GsaProperties {
        Property2ds = prop2Ds,
        Property3ds = prop3Ds,
        Sections = sections,
        SpringProperties = springProps
      };
      var load = new GsaLoading {
        Loads = loads,
        GridPlaneSurfaces = gps,
        LoadCases = gsaLoadCases
      };
      var analysis = new GsaAnalysis {
        DesignTasks = designTasks
      };

      var assembly = new ModelAssembly(mainModel, lists, gridLines, geometry, properties, load,
        analysis, mainModel.ModelUnit, tolerance, false, owner);
      mainModel.ApiModel = assembly.GetModel();

      return mainModel;
    }
  }
}
