﻿/**
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
using biz.dfch.CS.Commons.Diagnostics;
using biz.dfch.CS.Testing.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;
using TraceSource = biz.dfch.CS.Commons.Diagnostics.TraceSource;

namespace biz.dfch.CS.Commons.Tests.Diagnostics
{
    [TestClass]
    public class LoggerTest
    {
        public static TraceListener DefaultTraceListener { get; set; }

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            var result1 = Logger.SetTraceListener(null);
            Assert.IsNotNull(result1);
            Assert.IsInstanceOfType(result1, typeof(DefaultTraceListener));

            var result2 = Logger.SetTraceListener(result1);
            Assert.IsNull(result2);

            var result3 = Logger.SetTraceListener(result1);
            Assert.IsNotNull(result3);
            Assert.IsInstanceOfType(result3, typeof(DefaultTraceListener));

            Assert.AreSame(result1, result3);

            DefaultTraceListener = result1;
        }

        [TestMethod]
        [ExpectContractFailure(MessagePattern = "name")]
        public void GetTraceSourceWithEmptyNameThrowsContractException()
        {
            var name = "   ";
            Logger.Get(name);
        }

        [TestMethod]
        public void GetDefaultTraceSourceIsSingleton()
        {
            var result1 = Logger.Default;
            Assert.AreEqual(Logger.DEFAULT_TRACESOURCE_NAME, result1.Name);

            var result2 = Logger.Default;
            Assert.AreEqual(Logger.DEFAULT_TRACESOURCE_NAME, result1.Name);

            Assert.AreSame(result1, result2);
        }

        [TestMethod]
        public void GetArbitraryTraceSourceIsSingleton()
        {
            var name = Guid.NewGuid().ToString();
            var result1 = Logger.Get(name);
            Assert.AreEqual(name, result1.Name);

            var result2 = Logger.Get(name);
            Assert.AreEqual(name, result2.Name);

            Assert.AreSame(result1, result2);
        }

        [TestMethod]
        public void CommonsTraceSourceDerivesFromTraceSource()
        {
            var name = Guid.NewGuid().ToString();
            var result1 = Logger.Get(name);
            Assert.AreEqual(name, result1.Name);

            var result2 = Logger.GetBase(name);
            Assert.AreEqual(name, result2.Name);

            Assert.AreSame(result1, result2);
        }

        [TestMethod]
        [ExpectContractFailure(MessagePattern = "Postcondition.+biz.dfch.CS.Commons.Diagnostics.TraceSource")]
        public void GetBaseTraceSourceAsTraceSourceThrowsContractException()
        {
            var name = Guid.NewGuid().ToString();
            
            var result = Logger.GetBase(name);
            Assert.AreEqual(name, result.Name);

            Logger.Get(name);
        }

        [TestMethod]
        [ExpectContractFailure(MessagePattern = "defaultTraceSource")]
        public void GetOrDefaultWithNullValueThrowsContractException()
        {
            var name = Guid.NewGuid().ToString();

            var traceSource = default(TraceSource);

            Logger.GetOrDefault(name, traceSource);
        }

        [TestMethod]
        public void GetOrDefaultReturnsSingleton()
        {
            var name = Guid.NewGuid().ToString();

            var traceSource = new TraceSource(name);

            var result1 = Logger.GetOrDefault(traceSource.Name, traceSource);
            Assert.AreEqual(name, result1.Name);
            Assert.AreSame(traceSource, result1);

            var result2 = Logger.Get(name);
            Assert.AreEqual(name, result2.Name);

            Assert.AreSame(result1, result2);
            Assert.AreSame(traceSource, result2);
        }

        [TestMethod]
        public void GetTraceSourceReturnsTraceSourceWithDefaultListener()
        {
            var name = Guid.NewGuid().ToString();

            var result = Logger.Get(name);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Listeners);
            Assert.AreEqual(1, result.Listeners.Count);
            Assert.IsInstanceOfType(result.Listeners[0], typeof(DefaultTraceListener));
        }

        [TestMethod]
        public void GetTraceSourceWithoutDefaultTraceListenerReturnsTraceSourceWithoutListener()
        {
            var name = Guid.NewGuid().ToString();
            var currentListener = Logger.SetTraceListener(null);

            var result = Logger.Get(name);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Listeners);
            Assert.AreEqual(0, result.Listeners.Count);

            Logger.SetTraceListener(currentListener);
        }

        [TestMethod]
        public void TraceSourceHasNoListenerAfterSettingNullTraceListenerAndUpdatingSources()
        {
            var name = Guid.NewGuid().ToString();

            var result = Logger.Get(name);
            
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Listeners);
            Assert.AreEqual(1, result.Listeners.Count);
            Assert.IsInstanceOfType(result.Listeners[0], typeof(DefaultTraceListener));

            var currentListener = Logger.SetTraceListener(null, updateTraceSources: true);

            Assert.AreEqual(0, result.Listeners.Count);

            Logger.SetTraceListener(currentListener);
        }

        [TestMethod]
        public void SetDefaultTraceListenerSucceeds()
        {
            var name = Guid.NewGuid().ToString();

            var defaultTraceListenerMock = new DefaultTraceListenerMock();
            var currentListener = Logger.SetTraceListener(defaultTraceListenerMock);

            var result = Logger.Get(name);
            
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Listeners);
            Assert.AreEqual(1, result.Listeners.Count);
            Assert.IsInstanceOfType(result.Listeners[0], defaultTraceListenerMock.GetType());

            Logger.SetTraceListener(currentListener);
        }

        [TestCategory("SkipOnTeamCity")]
        [TestMethod]
        [ExpectContractFailure(MessagePattern = "Assertion.+writeMethodIsImplemented.+traceSource")]
        public void TraceWithTraceListener()
        {
            var name = string.Format("traceSource-{0}", Guid.NewGuid());

            var defaultTraceListenerMock = new DefaultTraceListenerMock();
            var currentListener = Logger.SetTraceListener(defaultTraceListenerMock);

            var result = Logger.Get(name);
            
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Listeners);
            Assert.AreEqual(1, result.Listeners.Count);
            Assert.IsInstanceOfType(result.Listeners[0], defaultTraceListenerMock.GetType());

            var ex = new ArgumentException("arbitrayExceptionMessage")
            {
                Source = "arbitrarySource",
            };
            result.TraceException(ex, "arbitraryMessage");

            Logger.SetTraceListener(currentListener);
        }

        [TestCategory("SkipOnTeamCity")]
        [TestMethod]
        [ExpectContractFailure(MessagePattern = "Assertion.+writeMethodIsImplemented.+TraceSourceWithOtherListener")]
        public void TraceWithTraceListenerReadsFromAppConfig()
        {
            var name = "TraceSourceWithOtherListener";

            var defaultTraceListenerMock = new DefaultTraceListenerMock();
            var currentListener = Logger.SetTraceListener(defaultTraceListenerMock);

            var result = Logger.Get(name);
            
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Listeners);
            Assert.AreEqual(1, result.Listeners.Count);
            Assert.IsInstanceOfType(result.Listeners[0], typeof(TraceListenerMock));

            // SourceLevel is set to error which is taken from the app.config, 
            // even though the traceSource was created in code with SourceLevels.All
            Assert.AreEqual(SourceLevels.Error, result.Switch.Level);

            var ex = new ArgumentException("arbitrayExceptionMessage")
            {
                Source = "arbitrarySource",
            };
            result.TraceException(ex, "arbitraryMessage");

            Logger.SetTraceListener(currentListener);
        }


        [TestMethod]
        public void TraceWithTraceListenerReadsFromAppConfigAndReplacesDefaultListener()
        {
            var name = "TraceSourceWithDefaultListenerAndOtherListener";

            var defaultTraceListenerMock = new DefaultTraceListenerMock();
            var currentListener = Logger.SetTraceListener(defaultTraceListenerMock);

            var result = Logger.Get(name);
            
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Listeners);
            Assert.AreEqual(2, result.Listeners.Count);
            Assert.IsTrue
            (
                result.Listeners[0].GetType() == defaultTraceListenerMock.GetType() && result.Listeners[1].GetType() == typeof(TraceListenerMock) 
                ||
                result.Listeners[1].GetType() == defaultTraceListenerMock.GetType() && result.Listeners[0].GetType() == typeof(TraceListenerMock) 
            );

            // SourceLevel is set to error which is taken from the app.config, 
            // even though the traceSource was created in code with SourceLevels.All
            Assert.AreEqual(SourceLevels.Error, result.Switch.Level);

            Logger.SetTraceListener(currentListener);
        }

        [TestMethod]
        public void SystemDiagnosticsTraceLogsToTraceListenerFromAppConfig()
        {
            var traceAssert = Mock.Create<DebugAndTraceListenerAssert>();
            Mock.Arrange(() => traceAssert.WriteLine())
                .IgnoreInstance()
                .MustBeCalled();

            System.Diagnostics.Trace.Write("arbitrary-message");

            Mock.Assert(traceAssert);
        }

        [TestMethod]
        [TestCategory("SkipOnTeamCity")]
        public void SystemDiagnosticsDebugLogsToTraceListenerFromAppConfig()
        {
            var traceAssert = Mock.Create<DebugAndTraceListenerAssert>();
            Mock.Arrange(() => traceAssert.WriteLine())
                .IgnoreInstance()
                .MustBeCalled();

            System.Diagnostics.Debug.WriteLine("arbitrary-message");

            Mock.Assert(traceAssert);
        }
    }
}
