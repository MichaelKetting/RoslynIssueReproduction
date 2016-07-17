using System;
using Remotion.Web.ExecutionEngine;

namespace WebApplication
{
  public class TestRootFunction : TestBaseFunction
  {
    public TestRootFunction ()
      : base (new WxePageStep ("TestRootForm.aspx"))
    {
    }
  }
}