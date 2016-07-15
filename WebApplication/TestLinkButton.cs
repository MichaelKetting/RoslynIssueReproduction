using System;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.UI.Controls;

namespace WebApplication
{
  public class TestLinkButton : WebLinkButton
  {
    public TestLinkButton ()
    {
    }

    private void OpenWorkItem ()
    {
      WxePage page = (WxePage) Page;

      if (!page.IsReturningPostBack)
      {
        ((IWxePage) page).ExecuteFunction (new TestSubFunction(), new WxeCallArguments (this, new WxeCallOptions()));
      }
    }

    protected override void OnClick (EventArgs e)
    {
      base.OnClick (e);

      OpenWorkItem();
    }
  }
}