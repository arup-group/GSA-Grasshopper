﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Grasshopper.Kernel;
using Timer = System.Timers.Timer;

namespace GrasshopperAsyncComponent
{
    /// <summary>
    /// Inherit your component from this class to make all the async goodness available.
    /// </summary>
    public abstract class GH_AsyncComponent : GH_Component
    {
        public override Guid ComponentGuid => throw new Exception("ComponentGuid should be overriden in any descendant of GH_AsyncComponent!");

        //List<(string, GH_RuntimeMessageLevel)> Errors;

        Action<string, double> ReportProgress;

        public ConcurrentDictionary<string, double> ProgressReports;

        Action Done;

        Timer DisplayProgressTimer;

        int State = 0;

        bool SetData = false;

        public List<WorkerInstance> Workers;

        List<Task> Tasks;

        public readonly List<CancellationTokenSource> CancellationSources;

        /// <summary>
        /// Set this property inside the constructor of your derived component. 
        /// </summary>
        public WorkerInstance BaseWorker { get; set; }

        /// <summary>
        /// Optional: if you have opinions on how the default system task scheduler should treat your workers, set it here.
        /// </summary>
        public TaskCreationOptions? TaskCreationOptions { get; set; } = null;

        protected GH_AsyncComponent(string name, string nickname, string description, string category, string subCategory) : base(name, nickname, description, category, subCategory)
        {

            DisplayProgressTimer = new Timer(333) { AutoReset = false };
            DisplayProgressTimer.Elapsed += DisplayProgress;

            ReportProgress = (id, value) =>
            {
                ProgressReports[id] = value;
                if (!DisplayProgressTimer.Enabled) DisplayProgressTimer.Start();
            };

            Done = () =>
            {
                State++;

                if (State == Workers.Count)
                {
                    SetData = true;
                    // We need to reverse the workers list to set the outputs in the same order as the inputs. 
                    Workers.Reverse();

                    Rhino.RhinoApp.InvokeOnUiThread((Action)delegate
                    {
                        ExpireSolution(true);
                    });
                }
            };

            ProgressReports = new ConcurrentDictionary<string, double>();

            Workers = new List<WorkerInstance>();
            CancellationSources = new List<CancellationTokenSource>();
            Tasks = new List<Task>();
        }

        public virtual void DisplayProgress(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (Workers.Count == 0) return;
            if (Workers.Count == 1)
            {
                //Message = ProgressReports.Values.Last().ToString("0.00%");

                string msg = "";
                foreach (string key in ProgressReports.Keys)
                {
                    ProgressReports.TryGetValue(key, out double val);
                    if (val < 0)
                    {
                        if (val == -1)
                        {
                            msg = key.ToString();
                            ProgressReports.Clear();
                            break;
                        }
                            
                        if (val == -2)
                            msg = key.ToString() + System.Environment.NewLine + msg;
                        if (val == -255)
                        {
                            msg = "";
                            AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, key);
                        }
                        if (val == -10)
                        {
                            Message = "";
                            AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, key);
                            return;
                        }
                        if (val == -20)
                        {
                            Message = "";
                            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, key);
                            return;
                        }
                    }
                    else
                        msg = key + " " + val.ToString("0%") + System.Environment.NewLine + msg;
                }

                Message = msg;
            }
            else
            {
                //double total = 0;
                //foreach (var kvp in ProgressReports)
                //{
                //    total += kvp.Value;
                //}

                //Message = (total / Workers.Count).ToString("0.00%");

                string msg = "";
                foreach (string key in ProgressReports.Keys)
                {
                    ProgressReports.TryGetValue(key, out double val);
                    if (val < 0)
                    {
                        if (val == -1)
                            msg = key + System.Environment.NewLine + msg;
                        if (val == -255)
                        {
                            msg = "";
                            AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, key);
                        }
                        if (val == -10)
                        {
                            Message = "";
                            AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, key);
                            return;
                        }
                        if (val == -20)
                        {
                            Message = "";
                            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, key);
                            return;
                        }
                    }
                    else
                        msg = key + " " + val.ToString("0%") + System.Environment.NewLine + msg;
                }

                Message = msg;
            }

            Rhino.RhinoApp.InvokeOnUiThread((Action)delegate
            {
                OnDisplayExpired(true);
            });
        }

        protected override void BeforeSolveInstance()
        {
            if (State != 0 && SetData) return;

            foreach (var source in CancellationSources) source.Cancel();

            CancellationSources.Clear();
            Workers.Clear();
            ProgressReports.Clear();
            Tasks.Clear();

            State = 0;

            //var test = Params.Output[0].VolatileData;
        }

        protected override void AfterSolveInstance()
        {
            // We need to start all the tasks as close as possible to each other.
            if (State == 0 && Tasks.Count > 0)
            {
                foreach (var task in Tasks) task.Start();
                //var test = Params.Output[0].VolatileData;
            }
        }

        protected override void ExpireDownStreamObjects()
        {
            // Prevents the flash of null data until the new solution is ready
            if (SetData)
                base.ExpireDownStreamObjects();
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            if (State == 0)
            {
                if (BaseWorker == null)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Worker class not provided.");
                    return;
                }

                var currentWorker = BaseWorker.Duplicate();
                if (currentWorker == null)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Could not get a worker instance.");
                    return;
                }

                // Let the worker collect data.
                currentWorker.GetData(DA, Params);

                // Create the task
                var tokenSource = new CancellationTokenSource();
                currentWorker.CancellationToken = tokenSource.Token;
                currentWorker.Id = $"Worker-{DA.Iteration}";

                var currentRun = TaskCreationOptions != null
                  ? new Task(() => currentWorker.DoWork(ReportProgress, Done), tokenSource.Token, (TaskCreationOptions)TaskCreationOptions)
                  : new Task(() => currentWorker.DoWork(ReportProgress, Done), tokenSource.Token);

                // Add cancellation source to our bag
                CancellationSources.Add(tokenSource);

                // Add the worker to our list
                Workers.Add(currentWorker);

                Tasks.Add(currentRun);

                return;
            }

            if (!SetData) return;

            if (Workers.Count > 0)
                Workers[--State].SetData(DA);

            if (State != 0) return;

            CancellationSources.Clear();
            Workers.Clear();
            ProgressReports.Clear();
            Tasks.Clear();

            SetData = false;
            
            Message = "Done";
            OnDisplayExpired(true);
        }
    }
}
