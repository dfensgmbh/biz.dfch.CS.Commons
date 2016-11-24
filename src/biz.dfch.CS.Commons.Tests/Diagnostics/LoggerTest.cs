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
using biz.dfch.CS.Commons.Diagnostics;
using biz.dfch.CS.Testing.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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

            var testedTraceListener = new MyTraceListener();
            var currentListener = Logger.SetTraceListener(testedTraceListener);

            var result = Logger.Get(name);
            
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Listeners);
            Assert.AreEqual(1, result.Listeners.Count);
            Assert.IsInstanceOfType(result.Listeners[0], testedTraceListener.GetType());

            Logger.SetTraceListener(currentListener);
        }

    }
}
