using System;
using Remotion.Data;

namespace WebApplication
{
  public class TestTransactionFactory : ITransactionFactory
  {
    public ITransaction CreateRootTransaction ()
    {
      throw new NotImplementedException();
    }
  }
}