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
using System.IO;
using System.Web.UI;
using Remotion.Utilities;
using Remotion.Web.ExecutionEngine.Infrastructure.WxePageStepExecutionStates;
using Remotion.Web.ExecutionEngine.Infrastructure.WxePageStepExecutionStates.Execute;

namespace Remotion.Web.ExecutionEngine
{
  [Serializable]
  public class WxePageStep : WxeStep, IExecutionStateContext
  {
    private readonly string _page;
    private string _pageState;
    private bool _isPostBack;
    private bool _isExecutionStarted;
    private bool _isReturningPostBack;
    private NameValueCollection _postBackCollection;

    [NonSerialized]
    private WxeHandler _wxeHandler;

    private IExecutionState _executionState = NullExecutionState.Null;

    public WxePageStep (string page)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("page", page);

      _page = page;
   }

    public WxeStep ExecutingStep
    {
      get
      {
        if (_executionState.IsExecuting)
          return _executionState.Parameters.SubFunction.ExecutingStep;
        else
          return this;
      }
    }

    public void Execute (WxeContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      if (_wxeHandler != null)
      {
        context.HttpContext.Handler = _wxeHandler;
        _wxeHandler = null;
      }

      if (!_isExecutionStarted)
      {
        _isExecutionStarted = true;
        _isPostBack = false;
      }
      else
      {
        _isPostBack = true;
      }

      ClearReturnState();

      while (_executionState.IsExecuting)
        _executionState.ExecuteSubFunction (context);

      WxeHandler wxeHandlerBackUp = context.HttpContext.Handler as WxeHandler;
      Assertion.IsNotNull (wxeHandlerBackUp, "The HttpHandler must be of type WxeHandler.");
      try
      {
        context.HttpContext.Server.Transfer (_page, _isPostBack);
      }
      finally
      {
        context.HttpContext.Handler = wxeHandlerBackUp;
        ClearReturnState();
      }
    }

    [EditorBrowsable (EditorBrowsableState.Never)]
    public void ExecuteFunction (PreProcessingSubFunctionStateParameters parameters, Control sender)
    {
      ArgumentUtility.CheckNotNull ("parameters", parameters);
      // sender can be null

      if (_executionState.IsExecuting)
        throw new InvalidOperationException ("Cannot execute function while another function executes.");

      _wxeHandler = parameters.Page.WxeHandler;

      _executionState = new PreProcessingSubFunctionState (this, parameters, sender);
      Execute (WxeContext.Current);
    }

    public bool IsPostBack
    {
      get { return _isPostBack; }
    }

    public bool IsReturningPostBack
    {
      get { return _isReturningPostBack; }
    }

    public NameValueCollection PostBackCollection
    {
      get { return _postBackCollection; }
    }

    public void SetPostBackCollection (NameValueCollection postBackCollection)
    {
      _postBackCollection = postBackCollection;
    }

    public void SetReturnState (WxeFunction returningFunction, bool isReturningPostBack, NameValueCollection previousPostBackCollection)
    {
      ArgumentUtility.CheckNotNull ("returningFunction", returningFunction);

      _isReturningPostBack = isReturningPostBack;
      _postBackCollection = previousPostBackCollection;
    }

    private void ClearReturnState ()
    {
      _isReturningPostBack = false;
      _postBackCollection = null;
    }

    /// <summary> Saves the passed <paramref name="state"/> object into the <see cref="WxePageStep"/>. </summary>
    /// <param name="state"> An <b>ASP.NET</b> viewstate object. </param>
    public void SavePageStateToPersistenceMedium (object state)
    {
      LosFormatter formatter = new LosFormatter();
      StringWriter writer = new StringWriter();
      formatter.Serialize (writer, state);
      _pageState = writer.ToString();
    }

    /// <summary> 
    ///   Returns the viewstate previsously saved through the <see cref="SavePageStateToPersistenceMedium"/> method. 
    /// </summary>
    /// <returns> An <b>ASP.NET</b> viewstate object. </returns>
    public object LoadPageStateFromPersistenceMedium ()
    {
      LosFormatter formatter = new LosFormatter();
      return formatter.Deserialize (_pageState);
    }

    WxeStep IExecutionStateContext.CurrentStep
    {
      get { return this; }
    }

    WxeFunction IExecutionStateContext.CurrentFunction
    {
      get { return ParentFunction; }
    }

    IExecutionState IExecutionStateContext.ExecutionState
    {
      get { return _executionState; }
    }

    void IExecutionStateContext.SetExecutionState (IExecutionState executionState)
    {
      ArgumentUtility.CheckNotNull ("executionState", executionState);

      _executionState = executionState;
    }
  }
}
