using System;

namespace Project1
{
  [Obsolete]
  internal class Class1<T>
      where T : Class2
  {
  }

  [Obsolete]
  internal class Class2
  {
  }
}