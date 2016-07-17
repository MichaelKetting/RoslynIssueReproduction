using System;
using Remotion.Web.ExecutionEngine;

namespace WebApplication
{
  public class TestSubFunction : TestBaseFunction
  {
    public TestSubFunction ()
        : base (new WxePageStep ("TestSubForm.aspx"))
    {
    }
  }
}