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
using System.Web;
using Remotion.Utilities;

namespace Remotion.Web.ExecutionEngine
{
  public class WxeContext
  {
    public static WxeContext Current
    {
      get { return (WxeContext)System.Web.HttpContext.Current.Items["WxeContext"]; }
    }

    [EditorBrowsable (EditorBrowsableState.Never)]
    public static void SetCurrent (WxeContext value)
    {
      System.Web.HttpContext.Current.Items["WxeContext"] = value;
    }

    private readonly HttpContextBase _httpContext;
    private readonly WxeFunctionState _functionState;
    private readonly NameValueCollection _queryString;

    public WxeContext (HttpContextBase context, WxeFunctionState functionState, NameValueCollection queryString)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("functionState", functionState);

      _httpContext = context;

      _functionState = functionState;

      if (queryString == null)
      {
        _queryString = new NameValueCollection ();
      }
      else
      {
        _queryString = new NameValueCollection (queryString);
        _queryString.Remove (WxeHandler.Parameters.WxeFunctionToken);
      }
    }

    public HttpContextBase HttpContext
    {
      get { return _httpContext; }
    }

    public string FunctionToken
    {
      get { return _functionState.FunctionToken; }
    }

    public int PostBackID
    {
      get { return _functionState.PostBackID; }
    }

    public NameValueCollection QueryString
    {
      get { return _queryString; }
    }
  }
}
