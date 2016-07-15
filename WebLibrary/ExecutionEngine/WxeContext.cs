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
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using Remotion.Utilities;
using Remotion.Web.Utilities;

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
    private readonly WxeFunctionStateManager _functionStateManager;
    private readonly WxeFunctionState _functionState;
    private readonly NameValueCollection _queryString;

    public WxeContext (HttpContextBase context, WxeFunctionStateManager functionStateManager, WxeFunctionState functionState, NameValueCollection queryString)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("functionStateManager", functionStateManager);
      ArgumentUtility.CheckNotNull ("functionState", functionState);

      _httpContext = context;
      _functionStateManager = functionStateManager;

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

    public WxeFunctionStateManager FunctionStateManager
    {
      get { return _functionStateManager; }
    }

    protected WxeFunctionState FunctionState
    {
      get { return _functionState; }
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

    /// <summary> Gets the URL that resumes the current function. </summary>
    /// <remarks>
    ///   If a WXE application branches to an external web site, the external site can
    ///   link back to this URL to resume the current function at the point where 
    ///   it was interrupted. Note that if the user stays on the external site longer
    ///   that the session or function timeout, resuming will fail with a timeout
    ///   exception.
    /// </remarks>
    public string GetResumeUrl (bool includeServer)
    {
      string pathPart = GetPath (_httpContext.Request.Url.AbsolutePath, FunctionToken, QueryString);
      if (includeServer)
        throw new NotImplementedException();
      else
        return pathPart;
    }

    /// <summary> Gets the absolute path to the WXE handler used for the current function. </summary>
    /// <param name="queryString"> An optional list of URL parameters to be appended to the path. </param>
    protected internal string GetPath (NameValueCollection queryString)
    {
      if (queryString == null)
        queryString = new NameValueCollection ();

      string path = _httpContext.Response.ApplyAppPathModifier (_httpContext.Request.Url.AbsolutePath);
      return UrlUtility.AddParameters (path, queryString, _httpContext.Response.ContentEncoding);
    }

    /// <summary> Gets the absolute path that resumes the function with specified token. </summary>
    /// <param name="functionToken"> 
    ///   The function token of the function to resume. Must not be <see langword="null"/> or emtpy.
    /// </param>
    /// <param name="queryString"> An optional list of URL parameters to be appended to the path. </param>
    protected internal string GetPath (string functionToken, NameValueCollection queryString)
    {
      return GetPath (_httpContext.Request.Url.AbsolutePath, functionToken, queryString);
    }

    /// <summary> Gets the absolute path that resumes the function with specified token. </summary>
    /// <param name="path"> The path to the <see cref="WxeHandler"/>. Must not be <see langword="null"/> or emtpy. </param>
    /// <param name="functionToken"> 
    ///   The function token of the function to resume. Must not be <see langword="null"/> or emtpy.
    /// </param>
    /// <param name="queryString"> An optional list of URL parameters to be appended to the <paramref name="path"/>. </param>
    private string GetPath (string path, string functionToken, NameValueCollection queryString)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("path", path);
      ArgumentUtility.CheckNotNullOrEmpty ("functionToken", functionToken);

      if (path.IndexOf ("?") != -1)
        throw new ArgumentException ("The path must be provided without a query string. Use the query string parameter instead.", "path");

      if (queryString == null)
        queryString = new NameValueCollection ();
      else
        queryString = new NameValueCollection (queryString);

      queryString.Set (WxeHandler.Parameters.WxeFunctionToken, functionToken);

      path = UrlUtility.GetAbsoluteUrl (_httpContext, path);
      return UrlUtility.AddParameters (path, queryString, _httpContext.Response.ContentEncoding);
    }
  }

}
