using System;
using System.Threading;
using Remotion.Web.ExecutionEngine;

namespace WebApplication
{
  public abstract class TestBaseFunction : WxeFunction
  {
    public TestBaseFunction ()
        : base (WxeTransactionMode<TestTransactionFactory>.None)
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
      //finally
      //{
      //  // Prevent optimization
      //  Trace.Assert (context != null);
      //}
    }
  }
}