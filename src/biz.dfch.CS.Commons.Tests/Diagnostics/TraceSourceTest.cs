/**
 * Copyright 2016 d-fens GmbH
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;
using Sut = biz.dfch.CS.Commons.Diagnostics.TraceSource;

namespace biz.dfch.CS.Commons.Tests.Diagnostics
{
    [TestClass]
    public class TraceSourceTest
    {
        [TestMethod]
        public void TraceExceptionInvokesBaseMethod()
        {
            var hasBeenInvoked = false;

            var traceSourceBase = Mock.Create<Sut>();
            Mock.Arrange(() => traceSourceBase.TraceEvent(TraceEventType.Error, Arg.IsAny<int>(), Arg.IsAny<string>(), Arg.IsAny<string>(), Arg.IsAny<string>(), Arg.IsAny<string>(), Arg.IsAny<string>()))
                .IgnoreArguments()
                .IgnoreInstance()
                .DoInstead(() => hasBeenInvoked = true);

            var ex = new ArgumentException("arbitraryArgumentException");
            var sut = new Sut("arbitraryName");

            SourceLevels[] sourceLevels =
            {
                SourceLevels.Error,
                SourceLevels.Warning,
                SourceLevels.Information, 
                SourceLevels.Verbose, 
                SourceLevels.All, 
            };

            foreach (var sourceLevel in sourceLevels)
            {
                hasBeenInvoked = false;
                sut.Switch.Level = sourceLevel;
                sut.TraceException(ex);
                Assert.IsTrue(hasBeenInvoked, sourceLevel.ToString());
            }
        }

        [TestMethod]
        public void TraceExceptionSkipsBaseMethod()
        {
            var hasBeenInvoked = false;

            var traceSourceBase = Mock.Create<Sut>();
            Mock.Arrange(() => traceSourceBase.TraceEvent(TraceEventType.Error, Arg.IsAny<int>(), Arg.IsAny<string>(), Arg.IsAny<string>(), Arg.IsAny<string>(), Arg.IsAny<string>(), Arg.IsAny<string>()))
                .IgnoreArguments()
                .IgnoreInstance()
                .DoInstead(() => hasBeenInvoked = true);

            var ex = new ArgumentException("arbitraryArgumentException");
            var sut = new Sut("arbitraryName");

            SourceLevels[] sourceLevels =
            {
                SourceLevels.Off, 
                SourceLevels.Critical, 
                SourceLevels.ActivityTracing, 
            };

            foreach (var sourceLevel in sourceLevels)
            {
                hasBeenInvoked = false;
                sut.Switch.Level = sourceLevel;
                sut.TraceException(ex);
                Assert.IsFalse(hasBeenInvoked);
            }
        }

        #region ========== HashCode Tests ==========

        [TestMethod]
        public void ExceptionHashCodeIsTheSame()
        {
            var sut = new Exception("message");

            Assert.AreEqual(typeof(Exception).GetHashCode(), sut.GetType().GetHashCode());
        }

        [TestMethod]
        public void DifferentExceptionInstancesHashCodesAreTheSame()
        {
            var sut1 = new Exception("message1");
            var sut2 = new Exception("message2");

            Assert.AreEqual(typeof(Exception).GetHashCode(), sut1.GetType().GetHashCode());
            Assert.AreEqual(sut1.GetType().GetHashCode(), sut2.GetType().GetHashCode());
        }

        [TestMethod]
        public void MyExceptionHashCodeIsTheSame()
        {
            var sut = new MyException("message");

            Assert.AreEqual(typeof(MyException).GetHashCode(), sut.GetType().GetHashCode());
        }

        [TestMethod]
        public void ExceptionHasDifferentHashCodeThanDerivedException()
        {
            var sut1 = new Exception("message1");
            var sut2 = new MyException("message1");

            Assert.AreNotEqual(sut1.GetType().GetHashCode(), sut2.GetType().GetHashCode());
        }

        [TestMethod]
        public void BaseExceptionHasDifferentHashCodeThanDerivedException()
        {
            var sut1 = new Exception("message1");
            var sut2 = new MyException("message1");

            Assert.AreNotEqual(sut1.GetType().GetHashCode(), ((Exception) sut2).GetType().GetHashCode());
        }

        #endregion

    }

    public class MyException : Exception
    {
        public MyException(string message)
            : base(message)
        {
            // N/A
        }
    }
}
