using System;
using Remotion.Web.ExecutionEngine;

namespace WebApplication
{
  public class TestSubFunction : TestBaseFunction
  {
    public TestSubFunction ()
    {
    }

    private WxePageStep Step1 = new WxePageStep ("TestSubForm.aspx");
  }
}