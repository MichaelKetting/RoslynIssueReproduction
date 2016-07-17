using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Web.ExecutionEngine;

namespace WebApplication
{
  public class TestLinkButton : LinkButton
  {
    public TestLinkButton ()
    {
    }

    private void OpenWorkItem ()
    {
      WxePage page = (WxePage) Page;

      if (!page.IsReturningPostBack)
      {
        page.ExecuteFunction (new TestSubFunction(), this);
      }
    }

    protected override void OnClick (EventArgs e)
    {
      base.OnClick (e);

      OpenWorkItem();
    }
  }
}