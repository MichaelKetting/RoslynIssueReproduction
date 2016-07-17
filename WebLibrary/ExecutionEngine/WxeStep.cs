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
using Remotion.Utilities;

namespace Remotion.Web.ExecutionEngine
{
  [Serializable]
  public abstract class WxeStep
  {
    public static WxeFunction GetFunction (WxeStep step)
    {
      WxeStep step1 = step;
      for (;
          step1 != null;
          step1 = step1.ParentStep)
      {
        WxeFunction expectedStep = step1 as WxeFunction;
        if (expectedStep != null)
          return expectedStep;
      }
      return null;
    }

    private WxeStep _parentStep = null;

    public WxeStep ParentStep
    {
      get { return _parentStep; }
    }

    [EditorBrowsable (EditorBrowsableState.Never)]
    public void SetParentStep (WxeStep parentStep)
    {
      ArgumentUtility.CheckNotNull ("parentStep", parentStep);
      _parentStep = parentStep;
    }

    public WxeFunction RootFunction
    {
      get
      {
        WxeStep step = this;
        while (step.ParentStep != null)
          step = step.ParentStep;
        return step as WxeFunction;
      }
    }

    public WxeFunction ParentFunction
    {
      get { return GetFunction (ParentStep); }
    }
  }
}