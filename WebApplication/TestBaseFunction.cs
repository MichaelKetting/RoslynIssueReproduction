﻿using System;
using System.Diagnostics;
using System.Threading;
using Remotion.Web.ExecutionEngine;

namespace WebApplication
{
  public abstract class TestBaseFunction : WxeFunction
  {
    public TestBaseFunction (WxePageStep pageStep)
        : base (pageStep)
    {
    }

    public override void Execute (WxeContext context)
    {
      try
      {
        base.Execute (context);
      }
      catch (ThreadAbortException)
      {
      }
      finally
      {
        // Prevent optimization
        Trace.Assert (context != null);
      }
    }
  }
}