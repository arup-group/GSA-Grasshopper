﻿using System;
using GsaAPI;

namespace GsaGH.Parameters {
  public class GsaGravityLoad : IGsaLoad {
    public GravityLoad GravityLoad { get; set; } = new GravityLoad();
    public GsaLoadCase LoadCase { get; set; }
    public LoadType LoadType => LoadType.Gravity;
    public ReferenceType ReferenceType { get; set; } = ReferenceType.None;
    public GsaList ReferenceList { get; set; }
    public Guid RefObjectGuid { get; set; }
    public int CaseId {
      get => GravityLoad.Case;
      set => GravityLoad.Case = value;
    }
    public string Name {
      get => GravityLoad.Name;
      set => GravityLoad.Name = value;
    }
    public GsaGravityLoad() {
      GravityLoad.Factor = new Vector3() {
        X = 0,
        Y = 0,
        Z = -1,
      };
      GravityLoad.Case = 1;
      GravityLoad.EntityList = "all";
      GravityLoad.Nodes = "all";
    }

    public IGsaLoad Duplicate() {
      var dup = new GsaGravityLoad {
        GravityLoad = {
          Case = GravityLoad.Case,
          EntityList = GravityLoad.EntityList.ToString(),
          Nodes = GravityLoad.Nodes.ToString(),
          Name = GravityLoad.Name.ToString(),
          Factor = GravityLoad.Factor,
        },
      };

      if (LoadCase != null) {
        dup.LoadCase = LoadCase;
      }

      if (ReferenceType == ReferenceType.None) {
        return dup;
      }

      if (ReferenceType == ReferenceType.List) {
        dup.ReferenceType = ReferenceType.List;
        dup.ReferenceList = ReferenceList.Duplicate();
      } else {
        dup.RefObjectGuid = new Guid(RefObjectGuid.ToString());
        dup.ReferenceType = ReferenceType;
      }

      return dup;
    }
  }
}
