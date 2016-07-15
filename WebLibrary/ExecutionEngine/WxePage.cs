// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;
using System.Web;
using System.Web.UI;
using Remotion.Utilities;

namespace Remotion.Web.ExecutionEngine
{
  public class WxePage : Page, IWxePage
  {
    #region IWxePage Impleplementation

    public Page WrappedInstance
    {
      get { return this; }
    }

    /// <summary> End this page step and continue with the WXE function. </summary>
    public void ExecuteNextStep ()
    {
      _wxePageInfo.ExecuteNextStep ();
    }

    /// <summary>Executes the <paramref name="function"/> using the specified <paramref name="callArguments"/>.</summary>
    /// <param name="function">The <see cref="WxeFunction"/> to be executed. Must not be <see langword="null" />.</param>
    /// <param name="callArguments">The <see cref="IWxeCallArguments"/> used to control the function invocation. Must not be <see langword="null" />.</param>
    public void ExecuteFunction (WxeFunction function, IWxeCallArguments callArguments)
    {
      ArgumentUtility.CheckNotNull ("function", function);
      ArgumentUtility.CheckNotNull ("callArguments", callArguments);

      callArguments.Dispatch (_wxePageInfo.Executor, function);
    }

    /// <summary> Gets a flag describing whether this post-back has been triggered by returning from a WXE function. </summary>
    [Browsable (false)]
    public bool IsReturningPostBack
    {
      get { return _wxePageInfo.IsReturningPostBack; }
    }

    /// <summary> Gets the WXE function that has been executed in the current page. </summary>
    [Browsable (false)]
    public WxeFunction ReturningFunction
    {
      get { return _wxePageInfo.ReturningFunction; }
    }

    /// <summary> Gets or sets the <see cref="WxeHandler"/> of the current request. </summary>
    WxeHandler IWxePage.WxeHandler
    {
      get { return _wxePageInfo.WxeHandler; }
    }

    #endregion

    private readonly WxePageInfo _wxePageInfo;
    private bool _disposed;

    public WxePage ()
    {
      _wxePageInfo = new WxePageInfo (this);
      _disposed = false;
    }

    public override Control FindControl (string id)
    {
      bool callBaseMethod;
      Control control = _wxePageInfo.FindControl (id, out callBaseMethod);
      if (callBaseMethod)
        return base.FindControl (id);
      else
        return control;
    }

    NameValueCollection IWxePage.GetPostBackCollection ()
    {
        return _wxePageInfo.EnsurePostBackModeDetermined (Context);
    }

    protected override NameValueCollection DeterminePostBackMode ()
    {
      NameValueCollection result = _wxePageInfo.EnsurePostBackModeDetermined (Context);

      return result;
    }

    protected override void SavePageStateToPersistenceMedium (object viewState)
    {
      _wxePageInfo.SavePageStateToPersistenceMedium (viewState);
    }

    protected override object LoadPageStateFromPersistenceMedium ()
    {
      object state = _wxePageInfo.LoadPageStateFromPersistenceMedium();
      PageStatePersister persister = this.PageStatePersister;
      if (state is Pair)
      {
        Pair pair = (Pair) state;
        persister.ControlState = pair.First;
        persister.ViewState = pair.Second;
      }
      else
      {
        persister.ViewState = state;
      }

      return state;
    }


    /// <remarks> Invokes <see cref="WxePageInfo.OnPreRenderComplete"/> before calling the base-implementation. </remarks>
    protected override void OnPreRenderComplete (EventArgs e)
    {
      // wxeInfo.OnPreRenderComplete() must be called before base.OnPreRenderComplete (EventArgs)
      // Base-Implementation uses SmartPageInfo, which also overrides OnPreRenderComplete 
      _wxePageInfo.OnPreRenderComplete ();

      base.OnPreRenderComplete (e);
    }

    /// <summary> Gets the <see cref="WxePageStep"/> that called this <see cref="WxePage"/>. </summary>
    [Browsable (false)]
    public WxePageStep CurrentPageStep
    {
      get { return _wxePageInfo.CurrentPageStep; }
    }

    WxePageStep IWxeTemplateControl.CurrentPageStep
    {
      get { return _wxePageInfo.CurrentPageStep; }
    }

    /// <summary> Gets the <see cref="WxeFunction"/> of which the <see cref="CurrentPageStep"/> is a part. </summary>
    /// <value> 
    ///   A <see cref="WxeFunction"/> or <see langwrpd="null"/> if the <see cref="CurrentPageStep"/> is not part of a
    ///   <see cref="WxeFunction"/>.
    /// </value>
    [Browsable (false)]
    public WxeFunction CurrentFunction
    {
      get { return _wxePageInfo.CurrentPageFunction; }
    }


    /// <summary> Gets the <see cref="WxeForm"/> of this page. </summary>
    protected WxeForm WxeForm
    {
      get { return _wxePageInfo.WxeForm; }
    }


    /// <summary> Disposes the page. </summary>
    /// <remarks>
    ///   <b>Dispose</b> is part of the ASP.NET page execution life cycle. It does not actually implement the 
    ///   disposeable pattern.
    ///   <note type="inheritinfo">
    ///     Do not override this method.
    ///     Use <see cref="M:Remotion.Web.ExecutionEngine.WxePage.Dispose(System.Boolean)">Dispose(Boolean)</see> instead.
    ///   </note>
    /// </remarks>
    public override void Dispose ()
    {
      base.Dispose ();
      if (!_disposed)
      {
        Dispose (true);
        _disposed = true;
        _wxePageInfo.Dispose ();
      }
    }

    /// <summary> Disposes the page. </summary>
    protected virtual void Dispose (bool disposing)
    {
    }

    public override void ProcessRequest (System.Web.HttpContext httpContext)
    {
      try
      {
        base.ProcessRequest (httpContext);
      }
      catch (HttpException ex)
      {
        throw _wxePageInfo.WrapProcessRequestException (ex);
      }
    }

    void IWxePage.SaveAllState ()
    {
      typeof (Page).GetMethod ("SaveAllState", BindingFlags.Instance | BindingFlags.NonPublic).Invoke (this, new object[0]);
    }
  }
}
