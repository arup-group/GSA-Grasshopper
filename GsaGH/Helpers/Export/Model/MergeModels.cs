using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Grasshopper.Kernel;
using GsaAPI;
using GsaGH.Parameters;
using OasysUnits;
using LengthUnit = OasysUnits.Units.LengthUnit;

namespace GsaGH.Helpers.Export {
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
      
      ConcurrentBag<GsaNodeGoo> goonodes = 
        Import.Nodes.GetNodes(appendModel.ApiNodes, LengthUnit.Meter);
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

      var members = new Import.Members(appendModel);
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

      var existingSectionIds = mainModel.Properties.Sections.Keys.ToList();
      var sections = appendModel.Properties.Sections.
        Select(n => n.Value.Value).OrderByDescending(x => x.Id).ToList();
      sections.Select(c => {
        if (existingSectionIds.Contains(c.Id)) {
          c.Id = 0; // only set id to 0 if sectionId already exists in model to allow reference
        }
        return c;
      }).ToList();

      var existingProp2dIds = mainModel.Properties.Prop2ds.Keys.ToList();
      var prop2Ds = appendModel.Properties.Prop2ds.
        Select(n => n.Value.Value).OrderByDescending(x => x.Id).ToList();
      prop2Ds.Select(c => {
        if (existingProp2dIds.Contains(c.Id)) {
          c.Id = 0;
        }
        return c;
      }).ToList();

      var existingProp3dIds = mainModel.Properties.Prop3ds.Keys.ToList();
      var prop3Ds = appendModel.Properties.Prop3ds.
        Select(n => n.Value.Value).OrderByDescending(x => x.Id).ToList();
      prop3Ds.Select(c => {
        if (existingProp3dIds.Contains(c.Id)) {
          c.Id = 0;
        }
        return c;
      }).ToList();

      var gooloads = new List<GsaLoadGoo>();
      ReadOnlyDictionary<int, LoadCase> loadCases = appendModel.Model.LoadCases();
      gooloads.AddRange(Import.Loads.GetGravityLoads(appendModel.Model.GravityLoads(), loadCases));
      gooloads.AddRange(Import.Loads.GetNodeLoads(appendModel.Model, loadCases));
      gooloads.AddRange(Import.Loads.GetBeamLoads(appendModel.Model.BeamLoads(), loadCases));
      gooloads.AddRange(Import.Loads.GetFaceLoads(appendModel.Model.FaceLoads(), loadCases));

      IReadOnlyDictionary<int, GridSurface> srfDict = appendModel.Model.GridSurfaces();
      IReadOnlyDictionary<int, GridPlane> plnDict = appendModel.Model.GridPlanes();

      gooloads.AddRange(Import.Loads.GetGridPointLoads(
        appendModel.Model.GridPointLoads(), srfDict, plnDict, appendModel.ApiAxis, loadCases,
        LengthUnit.Meter));
      gooloads.AddRange(Import.Loads.GetGridLineLoads(
        appendModel.Model.GridLineLoads(), srfDict, plnDict, appendModel.ApiAxis, loadCases, 
        LengthUnit.Meter));
      gooloads.AddRange(Import.Loads.GetGridAreaLoads(
        appendModel.Model.GridAreaLoads(), srfDict, plnDict, appendModel.ApiAxis, loadCases, 
        LengthUnit.Meter));
      var loads = gooloads.Select(n => n.Value).ToList();

      var gpsgoo = srfDict.Keys.Select(key => new GsaGridPlaneSurfaceGoo(
            Import.Loads.GetGridPlaneSurface(
              srfDict, plnDict, appendModel.ApiAxis, key, LengthUnit.Meter))).ToList();
      var gps = gpsgoo.Select(n => n.Value).ToList();

      List<GsaList> lists = Import.Lists.GetLists(appendModel);
      List<GsaGridLine> gridLines = Import.GridLines.GetGridLines(appendModel);

      mainModel.Model = AssembleModel.Assemble(
        mainModel, lists, gridLines, nodes, elem1ds, elem2ds, elem3ds, mem1ds, mem2ds, mem3ds, 
        sections, prop2Ds, prop3Ds, loads, gps, null, null, null, LengthUnit.Meter, tolerance, 
        false, owner);
      return mainModel;
    }
  }
}
