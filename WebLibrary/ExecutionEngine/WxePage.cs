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
using System.Reflection;
using System.Web;
using System.Web.UI;
using Remotion.Utilities;
using Remotion.Web.ExecutionEngine.Infrastructure.WxePageStepExecutionStates;
using Remotion.Web.Utilities;

namespace Remotion.Web.ExecutionEngine
{
  public class WxePage : Page
  {
    public static readonly string PostBackSequenceNumberID = "wxePostBackSequenceNumberField";

    private WxePageStep _currentPageStep;
    private WxeHandler _wxeHandler;
    private bool _postbackCollectionInitialized;
    private NameValueCollection _postbackCollection;

    public WxePage ()
    {
    }

    public void ExecuteFunction (WxeFunction function, Control sender)
    {
      ArgumentUtility.CheckNotNull ("function", function);
      // sender can be null

      _currentPageStep.ExecuteFunction (new PreProcessingSubFunctionStateParameters (this, function), sender);
    }

    public bool IsReturningPostBack
    {
      get { return _currentPageStep.IsReturningPostBack; }
    }

    public WxeHandler WxeHandler
    {
      get { return _wxeHandler; }
    }

    public NameValueCollection GetPostBackCollection ()
    {
      return EnsurePostBackModeDetermined ();
    }

    protected override NameValueCollection DeterminePostBackMode ()
    {
      return EnsurePostBackModeDetermined ();
    }

    protected override void SavePageStateToPersistenceMedium (object viewState)
    {
      _currentPageStep.SavePageStateToPersistenceMedium (viewState);
    }

    protected override object LoadPageStateFromPersistenceMedium ()
    {
      object state = _currentPageStep.LoadPageStateFromPersistenceMedium ();
      PageStatePersister persister = PageStatePersister;
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

    protected override void OnPreRenderComplete (EventArgs e)
    {
      WxeContext wxeContext = WxeContext.Current;

      ClientScript.RegisterHiddenField (WxeHandler.Parameters.WxeFunctionToken, wxeContext.FunctionToken);

      int nextPostBackID = wxeContext.PostBackID + 1;
      ClientScript.RegisterHiddenField (WxePage.PostBackSequenceNumberID, nextPostBackID.ToString ());

      string path = Response.ApplyAppPathModifier (Request.Url.AbsolutePath);
      Form.Action = UrlUtility.AddParameters (path, wxeContext.QueryString, Response.ContentEncoding);

      Context.Response.Cache.SetCacheability (HttpCacheability.Private);

      base.OnPreRenderComplete (e);
    }

    public override void Dispose ()
    {
      base.Dispose();
      if (Context != null)
      {
        Context.Handler = _wxeHandler;
      }
    }

    internal void SaveAllState ()
    {
      typeof (Page).GetMethod ("SaveAllState", BindingFlags.Instance | BindingFlags.NonPublic).Invoke (this, new object[0]);
    }

    private NameValueCollection EnsurePostBackModeDetermined ()
    {
      if (!_postbackCollectionInitialized)
      {
        Initialize ();

        _postbackCollection = DeterminePostBackMode1 ();
        _postbackCollectionInitialized = true;
      }
      return _postbackCollection;
    }

    private void Initialize ()
    {
      _wxeHandler = Context.Handler as WxeHandler;
      if (_wxeHandler == null)
      {
        throw new HttpException (string.Format ("No current WxeHandler found. Most likely cause of the exception: "
                                                + "The page '{0}' has been called directly instead of using a WXE Handler to invoke the associated WXE Function.",
            GetType ()));
      }


      WxeStep executingStep = _wxeHandler.RootFunction.ExecutingStep;
      _currentPageStep = (WxePageStep) executingStep;

      Context.Handler = this;
    }

    private NameValueCollection DeterminePostBackMode1 ()
    {
      WxeContext wxeContext = WxeContext.Current;
      if (wxeContext == null)
        return null;
      if (!_currentPageStep.IsPostBack)
        return null;
      if (_currentPageStep.PostBackCollection != null)
        return _currentPageStep.PostBackCollection;

      NameValueCollection collection;
      if (string.Equals(Request.HttpMethod, "POST", StringComparison.InvariantCultureIgnoreCase))
        collection = Request.Form;
      else
        collection = Request.QueryString;

      if ((collection["__VIEWSTATE"] == null) && (collection[postEventSourceID] == null))
        return null;
      else
        return collection;
    }
  }
}
