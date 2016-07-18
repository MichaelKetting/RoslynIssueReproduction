using System;
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
        // re-throw is missing, with C# 6 and .NET 4.5, the exception will not get re-thrown if there is a finally-block after this catch-block.
      }
      finally
      {
        // Prevent optimization
        Trace.Assert (context != null);
      }
    }
  }
}