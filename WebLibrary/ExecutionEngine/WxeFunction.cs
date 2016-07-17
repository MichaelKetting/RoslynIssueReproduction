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
using System.Text;
using System.Threading;
using Remotion.Utilities;

namespace Remotion.Web.ExecutionEngine
{
  /// <summary>
  /// The new <see cref="WxeFunction"/>. Will replace the <see cref="WxeFunction"/> type once implemtation is completed.
  /// </summary>
  [Serializable]
  public abstract class WxeFunction : WxeStep
  {
    private readonly WxePageStep _pageStep;

    private int _executingStep = 0;

    protected WxeFunction (WxePageStep pageStep)
    {
      ArgumentUtility.CheckNotNull ("step", (WxeStep) pageStep);

      _pageStep = pageStep;
      pageStep.SetParentStep (this);
    }

    public virtual void Execute (WxeContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      try
      {
        while (_executingStep < 1)
        {
          _pageStep.Execute (context);
          _executingStep++;
        }
      }
      catch (ThreadAbortException)
      {
        OnExecutionPause ();
        throw;
      }
    }

    public WxeStep ExecutingStep
    {
      get
      {
        if (_executingStep < 1)
          return _pageStep.ExecutingStep;
        else
          return this;
      }
    }

    private void OnExecutionPause ()
    {
    }
  }
}