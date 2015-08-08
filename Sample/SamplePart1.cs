using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sample
{
    [TestClass]
    public class SamplePart1
    {
        private object _state;

        [TestMethod]
        public void AnonymousFunction_NoCapture_IsStaticMethod_InCSharp5()
        {
            // Note that Roslyn will actually generate the Method as an instance method on a nested type but then cache the delegate to this method
            var x = new Func<object>(() => null);
            var y = new Func<int>(() => 5);
            Assert.IsTrue(x.Method.IsStatic);
            Assert.AreSame(x.Method.DeclaringType, GetType());
        }

        [TestMethod]
        public void AnonymousFunction_NoCapture_IsCreatedAsInstanceMethodOnNestedType_InRoslyn()
        {
            // Note that Roslyn will actually generate the Method as an instance method on a nested type but then cache the delegate to this method
            var x = new Func<object>(() => null);
            var y = new Func<int>(() => 5);
            Assert.IsFalse(x.Method.IsStatic);
            Assert.AreNotSame(x.Method.DeclaringType, GetType());
            Assert.AreSame(x.Target, y.Target);
        }

        [TestMethod]
        public void AnonymousFunction_CapturesThisPointer_IsInstanceMethod()
        {
            var x = new Func<object>(() => _state);
            Assert.IsFalse(x.Method.IsStatic);
            Assert.AreSame(x.Method.DeclaringType, GetType());
        }

        [TestMethod]
        public void AnonymousFunction_CapturesLocalState_IsClosure()
        {
            object localState = null;
            var x = new Func<object>(() => localState);
            Assert.IsFalse(x.Method.IsStatic);
            Assert.AreNotSame(x.Method.DeclaringType, GetType());
        }

        [TestMethod]
        public void MultipleAnonymousFunction_AllCaptureLocalState_IsClosure_InstanceIsSharedBetweenAnonymousFunctions()
        {
            object localStateA = null;
            object localStateB = null;
            var x = new Func<object>(() => localStateA);
            var y = new Func<object>(() => localStateB);
            Assert.IsFalse(x.Method.IsStatic);
            Assert.AreNotSame(x.Method.DeclaringType, GetType());
            Assert.IsFalse(y.Method.IsStatic);
            Assert.AreNotSame(y.Method.DeclaringType, GetType());
            Assert.AreSame(y.Method.DeclaringType, x.Method.DeclaringType);
            Assert.AreSame(y.Target, x.Target);
        }

        [TestMethod]
        public void MultipleAnonymousFunction_SomeCaptureLocalState_InstanceIsNotSharedBetweenAnonymousFunctions()
        {
            object localStateA = null;
            object localStateB = null;
            var x = new Func<object>(() => localStateA);
            var y = new Func<object>(() => _state);
            Assert.IsFalse(x.Method.IsStatic);
            Assert.AreNotSame(x.Method.DeclaringType, GetType());
            Assert.IsFalse(y.Method.IsStatic);
            Assert.AreSame(y.Method.DeclaringType, GetType());
            Assert.AreNotSame(y.Target, x.Target);
        }

        [TestMethod]
        public void MultipleAnonymousFunction_SomeCaptureNoState_InstanceIsNotSharedBetweenAnonymousFunctions()
        {
            object localStateA = null;
            var x = new Func<object>(() => localStateA);
            var y = new Func<int>(() => 5);
            Assert.IsFalse(x.Method.IsStatic);
            Assert.AreNotSame(x.Method.DeclaringType, GetType());
            Assert.IsFalse(y.Method.IsStatic);
            Assert.AreNotSame(y.Method.DeclaringType, GetType());
            Assert.AreNotSame(y.Target, x.Target);
        }
    }
}
