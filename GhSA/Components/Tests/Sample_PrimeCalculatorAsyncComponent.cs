﻿using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GrasshopperAsyncComponent;

namespace GhSA.Components
{
  public class Sample_PrimeCalculatorAsyncComponent : GH_AsyncComponent
  {
    public override Guid ComponentGuid { get => new Guid("22C612B0-2C57-47CE-B9FE-E10621F18933"); }

    protected override System.Drawing.Bitmap Icon { get => null; }

    public override GH_Exposure Exposure => GH_Exposure.primary;

    public Sample_PrimeCalculatorAsyncComponent() : base("Sample Async Component", "PRIME", "Calculates Prime", Ribbon.CategoryName.Name(),
                Ribbon.SubCategoryName.Cat8())
        {
      BaseWorker = new PrimeCalculatorWorker();
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddIntegerParameter("N", "N", "Which n-th prime number. Minimum 1, maximum one million. Take care, it can burn your CPU.", GH_ParamAccess.item);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      pManager.AddTextParameter("Output", "O", "The n-th prime number.", GH_ParamAccess.item);
    }
  }

  public class PrimeCalculatorWorker : WorkerInstance
  {
    int TheNthPrime { get; set; } = 100;
    long ThePrime { get; set; } = -1;

    public override void DoWork(Action<string> ReportProgress, Action<string, GH_RuntimeMessageLevel> ReportError, Action Done)
    {
      // 👉 Checking for cancellation!
      if (CancellationToken.IsCancellationRequested) return;

      int count = 0;
      long a = 2;

      // Thanks Steak Overflow (TM) https://stackoverflow.com/a/13001749/
      while (count < TheNthPrime)
      {
        // 👉 Checking for cancellation!
        if (CancellationToken.IsCancellationRequested) return;

        long b = 2;
        int prime = 1;// to check if found a prime
        while (b * b <= a)
        {
          // 👉 Checking for cancellation!
          if (CancellationToken.IsCancellationRequested) return;

          if (a % b == 0)
          {
            prime = 0;
            break;
          }
          b++;
        }

        ReportProgress(((double)(count) / TheNthPrime).ToString("0.00%"));

        if (prime > 0)
        {
          count++;
        }
        a++;
      }

      ThePrime = --a;
      Done();
    }

    public override WorkerInstance Duplicate() => new PrimeCalculatorWorker();

    public override void GetData(IGH_DataAccess DA, GH_ComponentParamServer Params)
    {
      int _nthPrime = 100;
      DA.GetData(0, ref _nthPrime);
      if (_nthPrime > 1000000) _nthPrime = 1000000;
      if (_nthPrime < 1) _nthPrime = 1;

      TheNthPrime = _nthPrime;
    }

    public override void SetData(IGH_DataAccess DA)
    {
      // 👉 Checking for cancellation!
      if (CancellationToken.IsCancellationRequested) return;

      DA.SetData(0, $"Worker {Id}: {TheNthPrime}th prime is: {ThePrime}");
    }
  }

}
