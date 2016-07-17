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
using System.ComponentModel;
using System.Web;
using System.Web.SessionState;
using Remotion.Utilities;

namespace Remotion.Web.ExecutionEngine
{
  public class WxeHandler : IHttpHandler, IRequiresSessionState
  {
    public class Parameters
    {
      public static readonly string WxeFunctionType = "WxeFunctionType";

      /// <summary> Denotes the <b>ID</b> of the <see cref="WxeFunction"/> to be resumed. </summary>
      public static readonly string WxeFunctionToken = "WxeFunctionToken";
    }

    private WxeFunctionState _currentFunctionState;

    public WxeFunction RootFunction
    {
      get { return _currentFunctionState.Function; }
    }

    [EditorBrowsable (EditorBrowsableState.Never)]
    public void ProcessRequest (HttpContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      string functionToken = context.Request.Params[Parameters.WxeFunctionToken];
      bool hasFunctionToken = ! string.IsNullOrEmpty (functionToken);

      if (! hasFunctionToken)
      {
        _currentFunctionState = CreateNewFunctionState (GetType (context));
        ProcessFunctionState (context, _currentFunctionState, true);
      }
      else
      {
        _currentFunctionState = ResumeExistingFunctionState (context, functionToken);
        if (_currentFunctionState != null)
        {
          ProcessFunctionState (context, _currentFunctionState, false);
        }
        else
        {
          context.Response.Clear();
          context.Response.End();
        }
      }
    }

    private WxeFunctionState CreateNewFunctionState (Type type)
    {
      WxeFunctionStateManager functionStates = WxeFunctionStateManager.Current;
      functionStates.CleanUpExpired();

      WxeFunction function = (WxeFunction) Activator.CreateInstance (type);

      WxeFunctionState functionState = new WxeFunctionState (function);
      functionStates.Add (functionState);

      return functionState;
    }

    private WxeFunctionState ResumeExistingFunctionState (HttpContext context, string functionToken)
    {
      bool isPostRequest = string.Equals (context.Request.HttpMethod, "POST", StringComparison.OrdinalIgnoreCase);

      if (! WxeFunctionStateManager.HasSession)
      {
        if (isPostRequest)
        {
          throw new HttpException (500, "Session timeout.");
        }
        return CreateNewFunctionState (GetType (context));
      }

      WxeFunctionStateManager functionStateManager = WxeFunctionStateManager.Current;
      if (functionStateManager.IsExpired (functionToken))
      {
        if (isPostRequest)
        {
          throw new HttpException (500, "Function timeout.");
        }
        return CreateNewFunctionState (GetType (context));
      }

      WxeFunctionState functionState = functionStateManager.GetItem (functionToken);

      functionStateManager.Touch (functionToken);
      functionStateManager.CleanUpExpired();
      if (functionState.Function == null)
        throw new HttpException (string.Format ("Function missing in WxeFunctionState {0}.", functionState.FunctionToken));
      return functionState;
    }

    private void ProcessFunctionState (HttpContext context, WxeFunctionState functionState, bool isNewFunction)
    {
      WxeContext wxeContext = new WxeContext (new HttpContextWrapper (context), functionState, context.Request.QueryString);
      WxeContext.SetCurrent (wxeContext);

      functionState.PostBackID++;
      functionState.Function.Execute (wxeContext);

      //  This point is only reached after the WxeFunction has completed execution.

      bool isRootFunction = functionState.Function == functionState.Function.RootFunction;
      if (isRootFunction)
        WxeFunctionStateManager.Current.Abort (functionState);
    }

    private Type GetType (HttpContext context)
    {
      string typeName = context.Request.Params[Parameters.WxeFunctionType];
      return Type.GetType (typeName, throwOnError: true, ignoreCase : false);
    }

    bool IHttpHandler.IsReusable
    {
      get { return false; }
    }
  }
}
