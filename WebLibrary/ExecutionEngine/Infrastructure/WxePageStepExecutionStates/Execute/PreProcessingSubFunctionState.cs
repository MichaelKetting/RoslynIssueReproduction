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
using System.Web.UI;

namespace Remotion.Web.ExecutionEngine.Infrastructure.WxePageStepExecutionStates.Execute
{
  //NotSerializable
  public class PreProcessingSubFunctionState : ExecutionStateBase<PreProcessingSubFunctionStateParameters>
  {
    private readonly Control _sender;

    public PreProcessingSubFunctionState (
        IExecutionStateContext executionStateContext, PreProcessingSubFunctionStateParameters parameters, Control sender)
        : base (executionStateContext, parameters)
    {
      _sender = sender;
    }

    public override void ExecuteSubFunction (WxeContext context)
    {
      Parameters.SubFunction.SetParentStep (ExecutionStateContext.CurrentStep);
      NameValueCollection postBackCollection = BackupPostBackCollection();
      EnsureSenderPostBackRegistration (postBackCollection);

      Parameters.Page.SaveAllState();
      var parameters = new ExecutionStateParameters (Parameters.SubFunction, postBackCollection);
      ExecutionStateContext.SetExecutionState (new ExecutingSubFunctionWithoutPermaUrlState (ExecutionStateContext, parameters));
    }

    private NameValueCollection BackupPostBackCollection ()
    {
      var postBackCollection = new NameValueCollection (Parameters.Page.GetPostBackCollection());

      return postBackCollection;
    }

    private void EnsureSenderPostBackRegistration (NameValueCollection postBackCollection)
    {
      if (_sender is IPostBackDataHandler && postBackCollection[_sender.UniqueID] == null)
        Parameters.Page.RegisterRequiresPostBack (_sender);
    }
  }
}
