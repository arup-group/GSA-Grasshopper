﻿using System;
using System.Collections.Generic;
using System.Linq;
using GsaAPI;
using GsaGH.Helpers;

namespace GsaGH.Parameters {
  /// <summary>
  /// <para>A Design Task is collection of specifications that guide the automated, iterative design or checking of members. A Design Task is analogous to an Analysis Task in that there can be multiple Design Tasks all of which are saved with the model. Design Tasks must be executed to carry out either a design or a check based on the parameters defined in the task. </para>
  /// <para>In Grasshopper, it is only possible to create steel design tasks.</para>
  /// <para>Refer to <see href="https://docs.oasys-software.com/structural/gsa/references/sbs-steeldesign/">Design Tasks</see> to read more.</para>
  /// </summary>
  public class GsaSteelDesignTask : IGsaDesignTask {
    public SteelDesignTask ApiTask { get; internal set; }
    public GsaList List { get; internal set; }
    public int Id { get; set; } = 0;
    public string TaskName => ApiTask.TaskName;

    public GsaSteelDesignTask(string taskName) {
      ApiTask = new SteelDesignTask(taskName);
    }
    internal GsaSteelDesignTask(KeyValuePair<int, SteelDesignTask> kvp, GsaModel model) {
      Id = kvp.Key;
      ApiTask = kvp.Value;
      foreach (KeyValuePair<int, EntityList> apiList in model.Model.Lists()) {
        if (apiList.Value.Name == ApiTask.ListDefinition.Replace("\"", string.Empty)) {
          List = new GsaList(apiList.Key, apiList.Value, model);
          return;
        }
      }
        
      List = new GsaList(TaskName, kvp.Value.ListDefinition, GsaAPI.EntityType.Member);
    }

    public override string ToString() {
      return (Id > 0 ? $"ID:{Id} " : string.Empty) + $"'{TaskName}' SteelDesign".Replace("_", " ")
        .TrimSpaces();
    }
  }
}
