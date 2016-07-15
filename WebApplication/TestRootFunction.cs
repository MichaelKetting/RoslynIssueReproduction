using System;
using Remotion.Web.ExecutionEngine;

namespace WebApplication
{
  public class TestRootFunction : TestBaseFunction
  {
    public TestRootFunction ()
    {
    }

    private WxePageStep Step1 = new WxePageStep ("TestRootForm.aspx");

  }
}